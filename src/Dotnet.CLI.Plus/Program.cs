using Dotnet.CLI.Plus.Commands.Sln;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddBranch<SlnRootCommand.Settings>("sln", sln =>
    {
        sln.AddBranch("add", add =>
        {
            add.AddCommand<SlnAddDirectoryCommand>("directory")
                .WithAlias("dir")
                .WithDescription("Add a directory to your project.")
                .WithExample(["dotnet", "plus", "sln", "./", "add", "directory", "my/dir/tree"]);
        });
    });
});

app.Run(args);
