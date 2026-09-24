namespace AnimalShelter.Common.Abstracts;

public interface IFeatureHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
}

public interface IFeatureHandler<TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken ct);
}
