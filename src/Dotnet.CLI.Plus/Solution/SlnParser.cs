using System.Runtime.ExceptionServices;
using Dotnet.CLI.Plus.CommandContextAccessor;
using LanguageExt.Common;
using Microsoft.DotNet.Cli.Sln.Internal;
using static Dotnet.CLI.Plus.CommandContextAccessor.SolutionParseError;

namespace Dotnet.CLI.Plus.Solution;

public static class SlnParser
{
    public static Result<SlnFile> Parse(string slnPath)
        => slnPath[^1] == Path.DirectorySeparatorChar
            ? ParseFromDirectory(slnPath)
            : ParseFromFile(slnPath);

    private static Result<SlnFile> ParseFromDirectory(string slnDirectory)
    {
        DirectoryInfo? dir = null;
        try
        {
            dir = new DirectoryInfo(slnDirectory);
        }
        catch
        {
            // ignored
        }

        if (dir is null || !dir.Exists)
        {
            return new Result<SlnFile>(new SolutionParseError(ParseErrorType.DirectoryNotFound));
        }

        var slnFiles = dir.GetFiles("*.sln", SearchOption.AllDirectories);
        return slnFiles.Length switch
        {
            0 => new Result<SlnFile>(new SolutionParseError(ParseErrorType.FileNotFound)),
            > 1 => new Result<SlnFile>(new SolutionParseError(ParseErrorType.MultipleSolutionsFound)),
            _ => ParseFromFile(slnFiles[0].FullName)
        };
    }

    private static Result<SlnFile> ParseFromFile(string slnFile)
    {
        if (!File.Exists(slnFile))
        {
            return new Result<SlnFile>(new SolutionParseError(ParseErrorType.FileNotFound));
        }

        if (!Path.GetExtension(slnFile).Equals(".sln"))
        {
            return new Result<SlnFile>(new SolutionParseError(ParseErrorType.NotASolutionFile));
        }

        try
        {
            SlnFile.Read(slnFile);
            return new Result<SlnFile>(SlnFile.Read(slnFile));
        }
        catch (InvalidSolutionFormatException e)
        {
            return new Result<SlnFile>(new SolutionParseError(ParseErrorType.InternalParsingError,
                ExceptionDispatchInfo.Capture(e)));
        }
    }
}