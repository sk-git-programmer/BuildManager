using System.Reflection;
using NetArchTest.Rules;

namespace BuildManager.Architecture.Tests;

public sealed class LayerDependencyTests
{
    public static TheoryData<string, string> ForbiddenDependencies()
    {
        var data = new TheoryData<string, string>();

        foreach (var (assemblyName, forbiddenNamespaces) in DependencyRules.ForbiddenNamespaces)
        {
            foreach (var forbiddenNamespace in forbiddenNamespaces)
            {
                data.Add(assemblyName, forbiddenNamespace);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(ForbiddenDependencies))]
    public void Types_InAssembly_DoNotDependOnForbiddenNamespace(string assemblyName, string forbiddenNamespace)
    {
        var assembly = Assembly.Load(new AssemblyName(assemblyName));

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(forbiddenNamespace)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"{assemblyName} must not depend on {forbiddenNamespace}. " +
            $"Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
