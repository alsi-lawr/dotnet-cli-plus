using System.ComponentModel;
using Dotnet.CLI.Plus.CommandContextAccessor;
using Microsoft.Build.Construction;
using Microsoft.Build.Shared;
using Spectre.Console;
using Spectre.Console.Cli;
namespace Dotnet.CLI.Plus.Commands.Sln;

public class SlnAddCommand : Command<SlnAddCommand.Settings>
{
    private static readonly CommandHierarchy Hierarchy = new("add", "sln");
    
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[path]")]
        [Description("""
                     Path to add to the solution. Can be one of the following:
                        - Directory
                        - File
                        - Project
                     """)]
        public string PathToAdd { get; set; }
        
        [CommandOption("-s|--solution-folder <PARENT_FOLDER>")]
        [Description("The parent solution folder to add the target to.")]
        public string ParentFolder { get; set; }
        
        [CommandOption("-o|--output <SOLUTION_REFERENCE>")]
        [Description("The folder name inside the solution")]
        public string SolutionReference { get; set; }
        
        [CommandOption("-i|--include-hierarchy")]
        [Description("Include directory hierarchy in solution")]
        [DefaultValue(false)]
        public bool IncludeHierarchy { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(PathToAdd))
                return ValidationResult.Error("Please specify a path to add to the solution.");

            if (!Path.Exists(PathToAdd))
                return ValidationResult.Error("Please specify a valid path.");
            
            return base.Validate();
        }
        
    }

    public override int Execute(CommandContext ctx, Settings settings) =>
        ctx.WrappedParentSettingsAccessor<SlnRootCommand.Settings, InvalidCommandData, CommandHierarchy>(
            data: Hierarchy,
            onSuccess: parentSettings => ProcessSettings(parentSettings, settings)
        );

    private static int ProcessSettings(SlnRootCommand.Settings parentSettings, Settings settings)
    {
        var parseResult = SlnParser.Parse(parentSettings.SlnPath);
        return parseResult.Match<int>(
            Succ: SolutionFile => AddToSolution(SolutionFile, parentSettings, settings),
            Fail: exception => exception.PrintErrors<SolutionParseError>()
        );
    }

    private static int AddToSolution(SolutionFile file, SlnRootCommand.Settings parentSettings, Settings settings) =>
        settings.PathToAdd switch
        {
            var path when Path.GetExtension(path) == ".csproj"
                => AddProjectToSolution(file, settings),
            var path when (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory
                => AddDirectoryToSolution(file, parentSettings, settings),
            _ => AddFileToSolution(file, settings),
        };

    private static int AddDirectoryToSolution(SolutionFile file, SlnRootCommand.Settings parentSettings, Settings settings)
    { 
        if (!SlnActionValidator.IsSolutionAndPathColinear(parentSettings.SlnPath, settings.PathToAdd))
            return 1;
            
        List<string> directories = SlnActionValidator.GetDirectoryTree(parentSettings.SlnPath, settings.PathToAdd).ToList();
        if (!settings.IncludeHierarchy)
        {
            directories = [directories[^1]];
        }
        
        var newProjects = directories.Select(d => ProjectInSolution.)
    }

    private static int AddProjectToSolution(SolutionFile file, Settings settings)
    {
        
    }

    private static int AddFileToSolution(SolutionFile file, Settings settings)
    {
        
    }
}