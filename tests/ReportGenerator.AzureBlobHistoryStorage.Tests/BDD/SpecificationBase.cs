using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable MethodHasAsyncOverload

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

[PublicAPI]
public abstract class SpecificationBase
{
    private void ThrowExceptionInCaseBothSyncAndAsyncMethodImplementationsArePresent()
    {
        var methodImplementations = new List<Tuple<string, string, string, string>>
        {
            Tuple.Create("BeforeAllTests",
                "Void BeforeAllTests()",
                "BeforeAllTestsAsync",
                "System.Threading.Tasks.Task BeforeAllTestsAsync()"),

            Tuple.Create("Given",
                "Void Given()",
                "GivenAsync",
                "System.Threading.Tasks.Task GivenAsync()"),

            Tuple.Create("When",
                "Void When()",
                "WhenAsync",
                "System.Threading.Tasks.Task WhenAsync()"),

            Tuple.Create("CleanUp",
                "Void CleanUp()",
                "CleanUpAsync",
                "System.Threading.Tasks.Task CleanUpAsync()"),

            Tuple.Create("AfterAllTests",
                "Void AfterAllTests()",
                "AfterAllTestsAsync",
                "System.Threading.Tasks.Task AfterAllTestsAsync()"),
        };

        string[] methodsToLookFor = methodImplementations.Select(x => x.Item2)
            .Union(methodImplementations.Select(x => x.Item4))
            .ToArray();

        var specificationBaseType = typeof(SpecificationBase);
        var specificationType = GetType();

        var allMethods =
            specificationType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var allMethodNames = allMethods.Where(x => x.DeclaringType != specificationBaseType)
            .Select(x => x.ToString())
            .Where(x => methodsToLookFor.Contains(x))
            .ToList();

        foreach (var methodImplementation in methodImplementations) {
            if (allMethodNames.Contains(methodImplementation.Item2) &&
                allMethodNames.Contains(methodImplementation.Item4)) {
                throw new Exception(
                    $"Overriding both {methodImplementation.Item1} and {methodImplementation.Item3} is not allowed.");
            }
        }
    }

    [OneTimeSetUp]
    public virtual async Task TestFixtureSetUp()
    {
        ThrowExceptionInCaseBothSyncAndAsyncMethodImplementationsArePresent();

        BeforeAllTests();
        await BeforeAllTestsAsync();
    }

    [SetUp]
    public virtual async Task SetUp()
    {
        Given();
        await GivenAsync();

        When();
        await WhenAsync();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        CleanUp();
        await CleanUpAsync();
    }

    [OneTimeTearDown]
    public virtual async Task TestFixtureTearDown()
    {
        AfterAllTests();
        await AfterAllTestsAsync();
    }

    protected virtual void BeforeAllTests() { }

    protected virtual void Given() { }

    protected virtual void When() { }

    protected virtual void CleanUp() { }

    protected virtual void AfterAllTests() { }

    protected virtual Task BeforeAllTestsAsync() => Task.CompletedTask;

    protected virtual Task GivenAsync() => Task.CompletedTask;

    protected virtual Task WhenAsync() => Task.CompletedTask;

    protected virtual Task CleanUpAsync() => Task.CompletedTask;

    protected virtual Task AfterAllTestsAsync() => Task.CompletedTask;
}
