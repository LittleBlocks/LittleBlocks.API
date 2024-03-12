using OneOf;

namespace LittleBlocks.Sample.Minimal.WebAPI.Extensions;

public static class OneOfExtensions
{
    public static IResult MapResult<T>(this OneOf<T, Exception> oneOf, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return oneOf.Match<IResult>(
            TypedResults.Ok,
            error =>
            {
                context.Features.Set(error.Message);
                return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
            });
    }
}