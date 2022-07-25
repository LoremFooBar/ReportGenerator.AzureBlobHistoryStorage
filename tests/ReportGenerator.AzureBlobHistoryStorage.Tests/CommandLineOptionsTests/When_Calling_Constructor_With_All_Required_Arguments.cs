using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.CommandLineOptionsTests;

public class When_Calling_Constructor_With_All_Required_Arguments : SpecificationBase
{
    private PluginOptions _pluginOptions;

    protected override void When()
    {
        base.When();

        var arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "historycontainerurl", "value1" },
            { "writesastoken", "value2" },
            { "repositoryname", "value3" },
        };

        _pluginOptions = new PluginOptions(arguments);
    }

    [Then]
    public void It_Should_Create_CommandLineOptions_Instance()
    {
        _pluginOptions.Should().NotBeNull();
        _pluginOptions.HistoryContainerUrl.Should().Be("value1");
        _pluginOptions.WriteSasToken.Should().Be("value2");
        _pluginOptions.RepositoryName.Should().Be("value3");
    }
}
