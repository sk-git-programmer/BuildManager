namespace BuildManager.Architecture.Tests;

/// <summary>
/// Locates the repository root by walking up from the test output directory to the solution file.
/// </summary>
internal static class RepositoryRoot
{
    private const string SolutionFileName = "BuildManager.slnx";

    public static string FullPath { get; } = Find();

    private static string Find()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not find '{SolutionFileName}' in '{AppContext.BaseDirectory}' or any parent directory.");
    }
}
