using Camilyo.Framework.Testing;
using FluentAssertions;

namespace Camilyo.CoverageHistoryStorage.Tests
{
    public class When_Command_Line_Arguments_Are_Missing : SpecificationBase
    {
        private CommandLineArgumentMissingException _exception;

        protected override void When()
        {
            base.When();

            try {
                _ = new AzureBlobHistoryStorage();
            }
            catch (CommandLineArgumentMissingException ex) {
                _exception = ex;
            }
        }

        [Then]
        public void It_Should_Throw()
        {
            _exception.Should().NotBeNull();
        }
    }
}
