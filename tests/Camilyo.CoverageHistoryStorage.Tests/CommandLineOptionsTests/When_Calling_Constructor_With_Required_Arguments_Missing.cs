using System;
using System.Collections.Generic;
using Camilyo.Framework.Testing;
using FluentAssertions;

namespace Camilyo.CoverageHistoryStorage.Tests.CommandLineOptionsTests
{
    public class When_Calling_Constructor_With_Required_Arguments_Missing : SpecificationBase
    {
        private CommandLineArgumentMissingException _exception1;
        private CommandLineArgumentMissingException _exception2;
        private CommandLineArgumentMissingException _exception3;
        private CommandLineArgumentMissingException _exception4;

        protected override void When()
        {
            base.When();

            try {
                _ = new CommandLineOptions(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
            catch (CommandLineArgumentMissingException ex1) {
                _exception1 = ex1;
            }

            try {
                _ = new CommandLineOptions(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"writesastoken", "value2"},
                    {"repositoryname", "value3"}
                });
            }
            catch (CommandLineArgumentMissingException ex2) {
                _exception2 = ex2;
            }

            try {
                _ = new CommandLineOptions(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"historycontainerurl", "value1"},
                    {"repositoryname", "value3"}
                });
            }
            catch (CommandLineArgumentMissingException ex3) {
                _exception3 = ex3;
            }

            try {
                _ = new CommandLineOptions(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"historycontainerurl", "value1"},
                    {"writesastoken", "value2"}
                });
            }
            catch (CommandLineArgumentMissingException ex4) {
                _exception4 = ex4;
            }
        }

        [Then]
        public void It_Should_Throw_CommandLineArgumentMissingException()
        {
            _exception1.Should().NotBeNull();
            _exception2.Should().NotBeNull();
            _exception3.Should().NotBeNull();
            _exception4.Should().NotBeNull();
        }
    }
}
