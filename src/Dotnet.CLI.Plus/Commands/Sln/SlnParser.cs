using System.Runtime.ExceptionServices;
using Dotnet.CLI.Plus.CommandContextAccessor;
using LanguageExt.Common;
using static Dotnet.CLI.Plus.CommandContextAccessor.SolutionParseError;

namespace Dotnet.CLI.Plus.Commands.Sln;

using Microsoft.Build.Construction;

public static class SlnParser
{
    public static Result<SolutionFile> Parse(string slnFile)
    {
        if (!File.Exists(slnFile))
        {
            return new Result<SolutionFile>(new SolutionParseError(ParseErrorType.FileNotFound));
        }

        if (!Path.GetExtension(slnFile).Equals(".sln"))
        {
            return new Result<SolutionFile>(new SolutionParseError(ParseErrorType.NotASolutionFile));
        }

        try
        {
            return new Result<SolutionFile>(SolutionFile.Parse(slnFile));
        }
        catch (Exception e)
        {
            return new Result<SolutionFile>(new SolutionParseError(ParseErrorType.InternalParsingError,
                ExceptionDispatchInfo.Capture(e)));
        }
    }

    

}