using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.CommandLineOptionsTests;

public class When_Calling_Constructor_With_All_Required_Arguments : SpecificationBase
{
    private CommandLineOptions _commandLineOptions;

    protected override void When()
    {
        base.When();

        var arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "historycontainerurl", "value1" },
            { "writesastoken", "value2" },
            { "repositoryname", "value3" },
        };

        _commandLineOptions = new CommandLineOptions(arguments);
    }

    [Then]
    public void It_Should_Create_CommandLineOptions_Instance()
    {
        _commandLineOptions.Should().NotBeNull();
        _commandLineOptions.HistoryContainerUrl.Should().Be("value1");
        _commandLineOptions.WriteSasToken.Should().Be("value2");
        _commandLineOptions.RepositoryName.Should().Be("value3");
    }
}
