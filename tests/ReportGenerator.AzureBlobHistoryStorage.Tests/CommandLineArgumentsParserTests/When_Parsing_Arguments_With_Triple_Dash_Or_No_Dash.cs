﻿using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.CommandLineArgumentsParserTests;

public class When_Parsing_Arguments_With_No_Dash : SpecificationBase
{
    private Dictionary<string, string> _parsedArguments;

    protected override void When()
    {
        base.When();

        _parsedArguments = CommandLineArgumentsParser.ParseCommandLineArguments(new[] { "arg1:val1", "arg2:val2" });
    }

    [Then]
    public void It_Should_Return_Dictionary_With_All_Arguments_Names_And_Values()
    {
        _parsedArguments.Should().BeEmpty();
    }
}
