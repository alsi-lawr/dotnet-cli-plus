using LanguageExt.Common;
using Microsoft.DotNet.Cli.Sln.Internal;

namespace Dotnet.CLI.Plus.Solution;

public static class SlnFileFactory
{
    
    public static void AddDirectoriesAsProjects(this SlnFile slnFile,
        IReadOnlyCollection<SolutionProjectMap> solutionFolders)
    {
        if (solutionFolders.Count == 1)
        {
            slnFile.AddDirectory(solutionFolders.Single());
            return;
        }

        var hierarchies = solutionFolders.Zip(solutionFolders.Skip(1),
            (parent, directory) => new SolutionDirectoryHierarchy(directory, parent));
        var nestedProjectsSection = slnFile.Sections.GetOrCreateSection(
            "NestedProjects",
            SlnSectionType.PreProcess);
        
        foreach (var hierarchy in hierarchies)
        {
            slnFile.NestDirectories(nestedProjectsSection.Properties, hierarchy);
        }
    }

    private static void NestDirectories(
        this SlnFile slnFile,
        SlnPropertySet properties,
        SolutionDirectoryHierarchy solutionDirectoryHierarchy)
    {
        var project = slnFile.AddDirectory(solutionDirectoryHierarchy.ProjectMap);
        var parentProject = slnFile.AddDirectory(solutionDirectoryHierarchy.ParentProjectMap);
        if (properties.ContainsKey(project.Id) && properties[project.Id] == parentProject.Id)
            return;

        properties[project.Id] = parentProject.Id;
    }

    private static SlnProject AddDirectory(this SlnFile slnFile,
        SolutionProjectMap solutionProjectMap)
    {
        var matchingDirectories = slnFile.Projects
            .Where(p => new SolutionProjectMap(p.Name, p.FilePath) == solutionProjectMap).ToList();

        return matchingDirectories.Count == 0 ? slnFile.AddProject(solutionProjectMap) : matchingDirectories.First();
    }

    private static SlnProject AddProject(this SlnFile slnFile, SolutionProjectMap solutionProjectMap)
    {
        var proj = new SlnProject()
        {
            Id = Guid.NewGuid().ToString("B").ToUpper(),
            TypeGuid = ProjectTypeGuids.SolutionFolderGuid,
            Name = solutionProjectMap.Name,
            FilePath = solutionProjectMap.Path,
        };
        slnFile.Projects.Add(proj);
        return proj;
    }

    private static bool NestedMapExists(this SlnFile slnFile, SlnPropertySet properties, SlnProject project,
        SlnProject parent) =>
        properties.ContainsKey(project.Id) &&
        properties[project.Id].Equals(parent.Id, StringComparison.OrdinalIgnoreCase);
}