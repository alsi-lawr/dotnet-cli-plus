using System.ComponentModel;
using Dotnet.CLI.Plus.CommandContextAccessor;
using Dotnet.CLI.Plus.Solution;
using Microsoft.Build.Construction;
using Microsoft.Build.Shared;
using Microsoft.DotNet.Cli.Sln.Internal;
using Spectre.Console;
using Spectre.Console.Cli;
namespace Dotnet.CLI.Plus.Commands.Sln;

public class SlnAddDirectoryCommand : Command<SlnAddDirectoryCommand.Settings>
{
    private static readonly CommandHierarchy Hierarchy = new("add", "sln");
    
    public class Settings : SlnRootCommand.Settings
    {
        [CommandArgument(0, "[path]")]
        [Description("""
                     Path to add to the solution. Can be one of the following:
                        - Directory, like "dir1"
                        - Directory hierarchy, like "dir1/dir2", which will nest dir2 in dir1.
                     """)]
        public string PathToAdd { get; set; }
    }

    public override int Execute(CommandContext ctx, Settings settings) =>
        ProcessSettings(settings);

    private static int ProcessSettings(Settings settings)
    {
        var parseResult = SlnParser.Parse(settings.SlnPath);
        return parseResult.Match<int>(
            Succ: slnFile => AddDirectoryToSolution(slnFile, settings),
            Fail: exception => exception.PrintErrors<SolutionParseError>()
        );
    }

    private static int AddDirectoryToSolution(SlnFile file, Settings settings)
    { 
        var directories = SlnActionValidator.GetDirectoryTree(settings.PathToAdd).ToList();

        file.AddDirectoriesAsProjects(directories);
        file.Write();
        return 0;
    }
}