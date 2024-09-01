namespace Dotnet.CLI.Plus.CommandContextAccessor;

public abstract class CliErrorHandler(string? message = null) : Exception(message)
{
    public abstract int DisplayCliInfo();
}

public abstract class CliErrorHandler<TErrorData>(string? message = null) : Exception(message)
{
    public abstract int DisplayCliInfo(TErrorData data);
}