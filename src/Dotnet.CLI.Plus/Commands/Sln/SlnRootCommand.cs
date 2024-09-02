using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Dotnet.CLI.Plus.Commands.Sln;

public class SlnRootCommand : Command<SlnRootCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<PATH>")]
        [Description("Path to the sln file. Supports directory paths.")]
        public string SlnPath { get; set; } = string.Empty;

        public override ValidationResult Validate()
        {
            if(!Path.Exists(SlnPath))
                return ValidationResult.Error("Sln path is invalid.");
            
            return base.Validate();
        }
    }

    public override int Execute(CommandContext ctx, Settings settings)
    {
        return 0;
    }
}