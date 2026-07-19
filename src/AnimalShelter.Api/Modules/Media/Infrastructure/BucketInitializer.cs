using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class BucketInitializer(
    IAmazonS3 s3,
    IOptions<S3StorageOptions> options,
    ILogger<BucketInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bucket = options.Value.BucketName;

        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(s3, bucket);
        if (!exists)
        {
            await s3.PutBucketAsync(bucket, cancellationToken);
            logger.LogInformation("Created bucket {Bucket}", bucket);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
