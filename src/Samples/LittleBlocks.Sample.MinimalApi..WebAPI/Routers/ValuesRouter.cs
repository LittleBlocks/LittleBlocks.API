using Microsoft.AspNetCore.Http.HttpResults;

namespace LittleBlocks.Sample.Minimal.WebAPI.Routers;

public static class ValuesRouter
{
    public static WebApplication MapValuesRouter(this WebApplication app)
    {
        var mapGroup = app.MapGroup("values");

        mapGroup.MapGet("", async Task<IResult> (
                HttpContext context,
                CancellationToken cancellationToken) => TypedResults.Ok(new[] { "value1", "value2" }))
            .WithName("GetAll")
            .Produces<IEnumerable<string>>()
            .WithOpenApi()
            .WithTags("values");

        mapGroup.MapGet("{id:int}", async Task<IResult> (
                int id,
                HttpContext context,
                CancellationToken cancellationToken) => TypedResults.Ok("value1"))
            .WithName("GetValueById")
            .Produces<string>()
            .WithOpenApi()
            .WithTags("values");

        mapGroup.MapPost("", async Task<IResult> (
                [FromBody] string value,
                HttpContext context,
                CancellationToken cancellationToken) =>TypedResults.Ok(value))
            .WithName("Post")
            .Produces<string>()
            .WithOpenApi()
            .WithTags("values");

        mapGroup.MapPut("{id}", async Task<IResult> (
                int id,
                [FromBody] string value,
                HttpContext context,
                CancellationToken cancellationToken) => TypedResults.Ok(value))
            .WithName("Put")
            .Produces<string>()
            .WithOpenApi()
            .WithTags("values");        
        
        mapGroup.MapDelete("{id}", async Task<IResult> (
                int id,
                HttpContext context,
                CancellationToken cancellationToken) =>TypedResults.Ok(true))
            .WithName("Delete")
            .Produces<bool>()
            .WithOpenApi()
            .WithTags("values");

        return app;
    }
}