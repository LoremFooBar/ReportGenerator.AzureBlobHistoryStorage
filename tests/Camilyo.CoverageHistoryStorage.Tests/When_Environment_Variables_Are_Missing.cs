using Camilyo.Framework.Testing;
using FluentAssertions;

namespace Camilyo.CoverageHistoryStorage.Tests
{
    public class When_Environment_Variables_Are_Missing : SpecificationBase
    {
        private RequiredEnvironmentVariableNotFoundException _exception;
        protected override void When()
        {
            base.When();

            try {
                _ = new AzureBlobHistoryStorage();
            }
            catch (RequiredEnvironmentVariableNotFoundException ex) {
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
