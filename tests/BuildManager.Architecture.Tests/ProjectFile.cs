using System.Xml.Linq;

namespace BuildManager.Architecture.Tests;

/// <summary>
/// A production project file parsed as XML. Checking references at this level catches rule violations
/// even before the reference is used in code (unused references are not visible in IL).
/// </summary>
internal sealed record ProjectFile(
    string Name,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> PackageReferences,
    IReadOnlyList<string> FrameworkReferences)
{
    private static readonly string[] _productionRoots = ["src", "aspire"];

    public static IReadOnlyList<ProjectFile> LoadAll() =>
        _productionRoots
            .Select(root => Path.Combine(RepositoryRoot.FullPath, root))
            .Where(Directory.Exists)
            .SelectMany(root => Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories))
            .Select(Load)
            .OrderBy(project => project.Name, StringComparer.Ordinal)
            .ToList();

    private static ProjectFile Load(string path)
    {
        var document = XDocument.Load(path);

        return new ProjectFile(
            Path.GetFileNameWithoutExtension(path),
            ReadIncludes(document, "ProjectReference").Select(ToProjectName).ToList(),
            ReadIncludes(document, "PackageReference"),
            ReadIncludes(document, "FrameworkReference"));
    }

    private static List<string> ReadIncludes(XDocument document, string elementName) =>
        document.Descendants(elementName)
            .Select(element => (string?)element.Attribute("Include"))
            .OfType<string>()
            .ToList();

    // Paths in .csproj use '\' on every OS; normalize so the file name is extracted correctly on Linux CI.
    private static string ToProjectName(string include) =>
        Path.GetFileNameWithoutExtension(include.Replace('\\', '/'));
}
