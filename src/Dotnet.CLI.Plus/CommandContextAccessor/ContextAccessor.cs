using Dotnet.CLI.Plus.Commands;
using LanguageExt.Common;
using Spectre.Console.Cli;
using InvalidCommandType = Dotnet.CLI.Plus.CommandContextAccessor.InvalidCommandData.InvalidCommandType;

namespace Dotnet.CLI.Plus.CommandContextAccessor;

public static class ContextAccessor
{
    public static Result<T> ParentSettings<T>(this CommandContext ctx)
        where T : CommandSettings
    {
        if (ctx.Data is not IDictionary<string, object?> dataDictionary)
        {
            return new Result<T>(new InvalidCommandData(InvalidCommandType.NoDataDictionary));
        }
        
        if (!dataDictionary.TryGetValue("ParentSettings", out var parentSettings))
        {
            return new Result<T>(new InvalidCommandData(InvalidCommandType.NoParentSettings));
        }

        return parentSettings is not T settings
            ? new Result<T>(new InvalidCommandData(InvalidCommandType.InvalidParentSettingsType))
            : new Result<T>(settings);
    }

    public static int WrappedParentSettingsAccessor<TParent, TException, TErrorData>(
        this CommandContext ctx,
        TErrorData data,
        Func<TParent, int> onSuccess
        )
        where TParent : CommandSettings
        where TException : CliErrorHandler<TErrorData>
    {
        
        var parentSettingsResult = ctx.ParentSettings<TParent>();
        return parentSettingsResult.Match(
           Succ: onSuccess,
           Fail: errors => errors.PrintErrors<TException, TErrorData>(data)
        );
    }

    public static int PrintErrors<TException, TErrorData>(this Exception? exception, TErrorData data)
        where TException : CliErrorHandler<TErrorData> =>
        (exception as TException)?.DisplayCliInfo(data) ?? 1;
    
    public static int PrintErrors<TException>(this Exception? exception)
        where TException : CliErrorHandler =>
        (exception as TException)?.DisplayCliInfo() ?? 1;
}