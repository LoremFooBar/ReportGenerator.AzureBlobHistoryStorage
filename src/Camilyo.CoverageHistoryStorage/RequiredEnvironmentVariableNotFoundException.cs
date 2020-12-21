using System;

namespace Camilyo.CoverageHistoryStorage
{
    public class RequiredEnvironmentVariableNotFoundException : Exception
    {
        public RequiredEnvironmentVariableNotFoundException(string variableName) :
            base($"Required environment variable {variableName} not found")
        {
        }
    }
}
