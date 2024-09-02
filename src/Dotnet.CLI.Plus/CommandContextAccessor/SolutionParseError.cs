using System.Runtime.ExceptionServices;
using Spectre.Console;

namespace Dotnet.CLI.Plus.CommandContextAccessor;

public class SolutionParseError(SolutionParseError.ParseErrorType parseError, ExceptionDispatchInfo? exception = null)
    : CliErrorHandler
{
    private ParseErrorType ParseError { get; } = parseError;
    private ExceptionDispatchInfo? Exception { get; } = exception;

    [Flags]
    public enum ParseErrorType
    {
        FileNotFound,
        DirectoryNotFound,
        NotASolutionFile,
        InternalParsingError,
        MultipleSolutionsFound
    }

    public override int DisplayCliInfo()
    {
        Action writeError = this switch
        {
            { ParseError: ParseErrorType.FileNotFound} =>
                () => AnsiConsole.MarkupLine(
                    "[yellow]Solution file not found.[/]"),
            { ParseError: ParseErrorType.NotASolutionFile } =>
                () => AnsiConsole.MarkupLine(
                    $"[red]Solution file is not a solution file.[/]"),
            { ParseError: ParseErrorType.InternalParsingError } =>
                () =>
                {
                    if (Exception is null)
                    {
                        AnsiConsole.MarkupLine("[red]Failed to parse solution file.[/]");
                        return;
                    }
                    
                    AnsiConsole.MarkupLine(
                        $"[red]Parser failed to read solution file with exception:[/]");
                    AnsiConsole.WriteException(Exception.SourceException);
                },
            _ => throw new ArgumentOutOfRangeException()
        };

        writeError();
        return 1;
    }
}