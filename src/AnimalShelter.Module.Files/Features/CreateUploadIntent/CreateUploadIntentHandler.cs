using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Files.Domain;
using AnimalShelter.Module.Files.Infrastructure;
using AnimalShelter.Module.Files.Infrastructure.Database;
using AnimalShelter.Module.Files.Infrastructure.Storage;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace AnimalShelter.Module.Files.Features.CreateUploadIntent;

internal sealed record CreateUploadIntentRequest(string FileName, long Size, string ContentType);
internal sealed record CreateUploadIntentResult(Guid fileId, string UploadUrl, DateTimeOffset ExpiresAt);

internal sealed class CreateUploadIntentHandler(
    FilesDbContext context,
    IBlobStorage storage,
    IStorageKeyStrategy keys,
    IOptions<S3Options> options,
    TimeProvider clock)
    : IFeatureHandler<CreateUploadIntentRequest, ErrorOr<CreateUploadIntentResult>>
{
    public async Task<ErrorOr<CreateUploadIntentResult>> HandleAsync(CreateUploadIntentRequest request, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var ttl = options.Value.UploadUrlTtl;

        var id = Guid.CreateVersion7();
        var key = keys.ForOriginal(id, request.FileName);

        var file = FileObject.CreatePending(id, key, request.FileName, request.ContentType, request.Size, now);
        context.Files.Add(file);
        await context.SaveChangesAsync(ct);

        var url = await storage.CreateUploadUrlAsync(file.StorageKey, file.ContentType, ttl, ct);

        return new CreateUploadIntentResult(file.Id, url.ToString(), now + ttl);
    }
}
