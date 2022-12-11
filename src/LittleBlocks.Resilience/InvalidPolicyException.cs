namespace LittleBlocks.Resilience;

public sealed class InvalidPolicyException : Exception
{
    public InvalidPolicyException(string message): base(message)
    {
    }
}
