namespace Dotnet.CLI.Plus.Solution;

public static class SlnActionValidator
{
    private record SolutionAndTargetDirectories(string SolutionDir, string TargetDir);

    private static SolutionAndTargetDirectories? GetSolutionAndTargetDirectories(string solutionPath, string targetPath)
    {
        var fullRootDirectory = Path.GetDirectoryName(Path.GetFullPath(solutionPath));
        var fullTargetDirectory = Path.GetDirectoryName(Path.GetFullPath(targetPath));
        if (fullRootDirectory is null || fullTargetDirectory is null)
            return null;

        return new SolutionAndTargetDirectories(fullRootDirectory, fullTargetDirectory);
    }

    public static bool IsSolutionAndPathColinear(string solutionPath, string targetPath)
    {
        var comboDirs = GetSolutionAndTargetDirectories(solutionPath, targetPath);

        return comboDirs is not null && comboDirs.TargetDir.StartsWith(comboDirs.SolutionDir);
    }

    public static IEnumerable<SolutionProjectMap> GetDirectoryTree(string targetPath)
    {
        var dirTree = targetPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
        
        return dirTree.Select((val, index) =>
            new SolutionProjectMap(val, string.Join("/", dirTree.Take(index + 1)) + Path.DirectorySeparatorChar));
    }

    public static IEnumerable<SolutionProjectMap> GetDirectoryTree(string solutionPath, string targetPath)
    {
        var comboDirs = GetSolutionAndTargetDirectories(solutionPath, targetPath);
        if (comboDirs is null)
            return [];

        var solutionDirs =
            comboDirs.SolutionDir.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var targetDirs = comboDirs.TargetDir.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var dirTree = targetDirs[solutionDirs.Length..];
        return dirTree.Select((val, index) =>
            new SolutionProjectMap(val, string.Join("/", dirTree.Take(index + 1)) + Path.DirectorySeparatorChar));
    }
}