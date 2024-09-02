namespace Dotnet.CLI.Plus.Solution;

public record struct SolutionDirectoryHierarchy(SolutionProjectMap ProjectMap, SolutionProjectMap ParentProjectMap);
public record struct SolutionProjectMap(string Name, string Path);