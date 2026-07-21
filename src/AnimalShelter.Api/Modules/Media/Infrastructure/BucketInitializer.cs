using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class BucketInitializer(
    IAmazonS3 s3,
    IOptions<S3StorageOptions> options,
    ILogger<BucketInitializer> logger) : IHostedService
{
    private readonly string _publicBucketPolicy = $$"""
    {
        "Version": "2012-10-17",
        "Statement": [
            {
                "Effect": "Allow",
                "Principal": "*",
                "Action": "s3:GetObject",
                "Resource": "arn:aws:s3:::{{options.Value.PublicBucketName}}/*"
            }
        ]
    }
    """;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bucketName = options.Value.PublicBucketName;

        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(s3, bucketName);
        if (!exists)
        {
            await s3.PutBucketAsync(bucketName, cancellationToken);
            logger.LogInformation("Created public bucket {PublicBucket}", bucketName);
        }

        await s3.PutBucketPolicyAsync(bucketName, _publicBucketPolicy, cancellationToken);
        logger.LogInformation("Apply public bucket policy");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
