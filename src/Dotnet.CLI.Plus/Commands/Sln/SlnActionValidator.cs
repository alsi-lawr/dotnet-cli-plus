using System.Runtime.CompilerServices;

namespace Dotnet.CLI.Plus.Commands.Sln;

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
    
    public static IEnumerable<string> GetDirectoryTree(string solutionPath, string targetPath)
    {
        var comboDirs = GetSolutionAndTargetDirectories(solutionPath, targetPath);
        if (comboDirs is null)
            return [];
        
        var solutionDirs = comboDirs.SolutionDir.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var targetDirs = comboDirs.TargetDir.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        
        return targetDirs[solutionDirs.Length..];
    }
}