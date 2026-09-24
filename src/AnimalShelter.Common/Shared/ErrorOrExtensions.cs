using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace AnimalShelter.Common.Shared;

public static class ErrorOrExtensions
{
    public static IResult ToHttpResult<T>(this ErrorOr<T> result)
        => result.Match(value => value is Success ? Results.NoContent() : Results.Ok(value),
            ToProblem);

    public static IResult ToHttpResult<T>(this ErrorOr<T> result, Func<T, IResult> onSuccess)
        => result.Match(onSuccess, ToProblem);

    public static IResult ToProblem(IReadOnlyCollection<Error> errors)
    {
        var extensions = new Dictionary<string, object?>
        {
            ["code"] = errors.First().Code,
            ["errors"] = errors.Select(error => new
            {
                code = error.Code,
                details = error.Description,
                meta = error.Metadata
            }).ToArray()
        };

        return Results.Problem(
            detail: errors.Count == 1
                ? errors.First().Description
                : $"The request failed with {errors.Count} errors.",
            statusCode: ToStatusCode(errors.First()),
            extensions: extensions);
    }

    private static int ToStatusCode(Error error) => error.Type switch
    {
        ErrorType.Validation    => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized  => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden     => StatusCodes.Status403Forbidden,
        ErrorType.NotFound      => StatusCodes.Status404NotFound,
        ErrorType.Conflict      => StatusCodes.Status409Conflict,
        _                       => throw new InvalidOperationException(
                                    $"Error type '{error.Type}' must not be returned with ErrorOr " +
                                    $"(error code '{error.Code}'). Throw an exception instead.")
    };
}
