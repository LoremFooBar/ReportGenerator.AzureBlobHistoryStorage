using System.Collections.Generic;

namespace Camilyo.CoverageHistoryStorage
{
    public interface ICommandLineArgumentsParser
    {
        Dictionary<string, string> GetCommandLineArguments();
    }
}
