using System.Runtime.ExceptionServices;
using Spectre.Console;

namespace Dotnet.CLI.Plus.CommandContextAccessor;

public class SolutionManipulationError(
    SolutionManipulationError.ManipulationErrorType errorType,
    ExceptionDispatchInfo? exception = null)
    : CliErrorHandler
{
    private ManipulationErrorType ErrorType { get; } = errorType;
    private ExceptionDispatchInfo? Exception { get; } = exception;

    [Flags]
    public enum ManipulationErrorType 
    {
        CorruptedSolution,
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
            { ErrorType: ManipulationErrorType.FileNotFound} =>
                () => AnsiConsole.MarkupLine(
                    "[yellow]Solution file not found.[/]"),
            _ => throw new ArgumentOutOfRangeException()
        };

        writeError();
        return 1;
    }
}