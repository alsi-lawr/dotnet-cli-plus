using Spectre.Console.Cli;

namespace Dotnet.CLI.Plus.Commands.CentralPackageManagement;

public class CentralPackageManagementCommand : Command<CentralPackageManagementCommand.Settings>
{
    public class Settings : CommandSettings
    {
        
    }


    public override int Execute(CommandContext context, Settings settings)
    {
        throw new NotImplementedException();
    }
}