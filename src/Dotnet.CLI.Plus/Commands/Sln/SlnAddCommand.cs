using System.ComponentModel;
using Dotnet.CLI.Plus.CommandContextAccessor;
using Dotnet.CLI.Plus.Solution;
using Microsoft.Build.Construction;
using Microsoft.Build.Shared;
using Microsoft.DotNet.Cli.Sln.Internal;
using Spectre.Console;
using Spectre.Console.Cli;
namespace Dotnet.CLI.Plus.Commands.Sln;

public class SlnAddCommand : Command<SlnAddCommand.Settings>
{
    private static readonly CommandHierarchy Hierarchy = new("add", "sln");
    
    public class Settings : SlnRootCommand.Settings
    {
        [CommandArgument(0, "[path]")]
        [Description("""
                     Path to add to the solution. Can be one of the following:
                        - Directory, like "dir1"
                        - Directory hierarchy, like "dir1/dir2", which will nest dir2 in dir1.
                        - File
                        - Project
                     """)]
        public string PathToAdd { get; set; }
        
        [CommandOption("-s|--solution-folder <PARENT_FOLDER>")]
        [Description("The parent solution folder to add the target to.")]
        public string ParentFolder { get; set; }
        
        [CommandOption("-o|--output <SOLUTION_REFERENCE>")]
        [Description("The project name inside the solution")]
        public string SolutionReference { get; set; }
        
        [CommandOption("-i|--include-hierarchy")]
        [Description("Include directory hierarchy in solution")]
        [DefaultValue(false)]
        public bool IncludeHierarchy { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(PathToAdd))
                return ValidationResult.Error("Please specify a path to add to the solution.");

            if (!Path.Exists($"{Path.GetDirectoryName(SlnPath)}{Path.DirectorySeparatorChar}{PathToAdd}"))
                return ValidationResult.Error("Please specify a valid path. Path must exist in the file system.");
            
            return base.Validate();
        }

        public void NotifyRedundantDirectoryOptions()
        {
            if(!string.IsNullOrEmpty(SolutionReference))
                AnsiConsole.MarkupLine(
                    "[yellow]Directory aliases aren't supported for adding directories to solution.[/]");
            
            if(!string.IsNullOrEmpty(ParentFolder))
                AnsiConsole.MarkupLine("[yellow]Parent folders should be specified in directory hierarchy.[/]");
            
            if(IncludeHierarchy)
                AnsiConsole.MarkupLine("[yellow]Include hierarchy is redundant for adding directories to solution.[/]");
        }
    }

    public override int Execute(CommandContext ctx, Settings settings) =>
        ProcessSettings(settings);
        // ctx.WrappedParentSettingsAccessor<SlnRootCommand.Settings, InvalidCommandData, CommandHierarchy>(
        //     data: Hierarchy,
        //     onSuccess: parentSettings => ProcessSettings(parentSettings, settings)
        // );

    private static int ProcessSettings( Settings settings)
    {
        var parseResult = SlnParser.Parse(settings.SlnPath);
        return parseResult.Match<int>(
            Succ: slnFile => AddToSolution(slnFile, settings),
            Fail: exception => exception.PrintErrors<SolutionParseError>()
        );
    }

    private static int AddToSolution(SlnFile file, Settings settings) =>
        settings.PathToAdd switch
        {
            var path when Path.GetExtension(path) == ".csproj"
                => AddProjectToSolution(file, settings),
            var path when (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory
                => AddDirectoryToSolution(file, settings),
            _ => AddFileToSolution(file, settings),
        };

    private static int AddDirectoryToSolution(SlnFile file, Settings settings)
    { 
        settings.NotifyRedundantDirectoryOptions();
        
        var directories = SlnActionValidator.GetDirectoryTree(settings.PathToAdd).ToList();

        file.AddDirectoriesAsProjects(directories);
        file.Write();
        return 0;
    }

    private static int AddProjectToSolution(SlnFile file, Settings settings)
    {
        return 0;
    }

    private static int AddFileToSolution(SlnFile file, Settings settings)
    {
        return 0;
    }
}