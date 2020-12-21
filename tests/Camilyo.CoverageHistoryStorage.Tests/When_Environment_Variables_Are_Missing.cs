using System;
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

            string coverageHistoryBlobUrl = Environment.GetEnvironmentVariable("COVERAGE_HISTORY_BLOB_URL");
            string sasToken = Environment.GetEnvironmentVariable("COVERAGE_AZURE_STORAGE_WRITE_SAS_TOKEN");

            Environment.SetEnvironmentVariable("COVERAGE_HISTORY_BLOB_URL", null);
            Environment.SetEnvironmentVariable("COVERAGE_AZURE_STORAGE_WRITE_SAS_TOKEN", null);

            try {
                _ = new AzureBlobHistoryStorage();
            }
            catch (RequiredEnvironmentVariableNotFoundException ex) {
                _exception = ex;
            }
            finally {
                Environment.SetEnvironmentVariable("COVERAGE_HISTORY_BLOB_URL", coverageHistoryBlobUrl);
                Environment.SetEnvironmentVariable("COVERAGE_AZURE_STORAGE_WRITE_SAS_TOKEN", sasToken);
            }
        }

        [Then]
        public void It_Should_Throw()
        {
            _exception.Should().NotBeNull();
        }
    }
}
