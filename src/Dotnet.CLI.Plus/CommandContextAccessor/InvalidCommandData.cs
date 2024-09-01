using Spectre.Console;

namespace Dotnet.CLI.Plus.CommandContextAccessor;

public class InvalidCommandData(InvalidCommandData.InvalidCommandType type)
    : CliErrorHandler<CommandHierarchy>("Invalid command data")
{
    [Flags]
    public enum InvalidCommandType
    {
        NoDataDictionary,
        NoParentSettings,
        InvalidParentSettingsType,
    }

    private InvalidCommandType CommandType { get; } = type;

    public override int DisplayCliInfo(CommandHierarchy hierarchy)
    {
        Action writeError = this switch
        {
            { CommandType: InvalidCommandType.NoParentSettings | InvalidCommandType.NoDataDictionary } =>
                () => AnsiConsole.MarkupLine(
                    $"[red]{hierarchy.CommandName} requires {hierarchy.ParentCommandName}[/]"),
            { CommandType: InvalidCommandType.InvalidParentSettingsType } =>
                () => AnsiConsole.MarkupLine(
                    $"[red]{hierarchy.CommandName} requires valid {hierarchy.ParentCommandName} settings"),
            _ => throw new ArgumentOutOfRangeException()
        };

        writeError();
        return 1;
    }
}