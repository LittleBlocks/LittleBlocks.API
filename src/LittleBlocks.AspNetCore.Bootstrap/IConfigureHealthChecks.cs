namespace LittleBlocks.AspNetCore.Bootstrap;

public interface IConfigureHealthChecks : IExtendPipeline
{
    IExtendPipeline ConfigureHealthChecks(Action<IHealthChecksBuilder> configure);
}
