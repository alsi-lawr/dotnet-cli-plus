using System.ComponentModel;
using Spectre.Console.Cli;

namespace Dotnet.CLI.Plus.Commands.Sln;

public class SlnRootCommand : Command<SlnRootCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<PATH>")]
        [Description("Path to the sln file.")]
        public string SlnPath { get; set; } = string.Empty;
    }

    public override int Execute(CommandContext ctx, Settings settings)
    {
        return 0;
    }
}