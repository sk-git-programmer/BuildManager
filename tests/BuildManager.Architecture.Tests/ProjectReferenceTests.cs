namespace BuildManager.Architecture.Tests;

public sealed class ProjectReferenceTests
{
    private static readonly IReadOnlyList<ProjectFile> _projects = ProjectFile.LoadAll();

    public static TheoryData<string> ProductionProjectNames()
    {
        TheoryData<string> data = [.. _projects.Select(project => project.Name)];
        return data;
    }

    [Fact]
    public void AllowedProjectReferences_ComparedWithRepository_ListExactlyTheProductionProjects()
    {
        var discovered = _projects.Select(project => project.Name).ToHashSet();
        var configured = DependencyRules.AllowedProjectReferences.Keys.ToHashSet();

        var missingInRules = discovered.Except(configured);
        var missingInRepository = configured.Except(discovered);

        missingInRules.ShouldBeEmpty(
            "New production project found. Add it to DependencyRules and docs/ARCHITECTURE.md.");
        missingInRepository.ShouldBeEmpty(
            "DependencyRules lists a project that does not exist. Remove it.");
    }

    [Theory]
    [MemberData(nameof(ProductionProjectNames))]
    public void ProjectReferences_OfProductionProject_AreAllowedByArchitecture(string projectName)
    {
        var project = _projects.Single(p => p.Name == projectName);
        var allowed = DependencyRules.AllowedProjectReferences.GetValueOrDefault(projectName) ?? [];

        var forbidden = project.ProjectReferences.Except(allowed);

        forbidden.ShouldBeEmpty($"{projectName} references projects not allowed by docs/ARCHITECTURE.md.");
    }

    [Fact]
    public void Domain_ProjectFile_HasNoPackageOrFrameworkReferences()
    {
        var domain = _projects.Single(project => project.Name == ProjectNames.Domain);

        var externalReferences = domain.PackageReferences.Concat(domain.FrameworkReferences);

        externalReferences.ShouldBeEmpty("Domain must depend on the BCL only.");
    }
}
