namespace AnimalShelter.Api.Modules.Media.Public;

public interface IMediaUrlProvider
{
    Task<string?> GetPublicUrlAsync(Guid fileObjectId, CancellationToken cancellationToken);    
    Task<IReadOnlyDictionary<Guid, string>> GetPublicUrlsAsync(IReadOnlyCollection<Guid> fileObjectIds, CancellationToken cancellationToken);
}
