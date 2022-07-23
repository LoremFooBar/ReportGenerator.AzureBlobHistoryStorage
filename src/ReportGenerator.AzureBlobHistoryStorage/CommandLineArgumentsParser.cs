using System.Text.RegularExpressions;

namespace ReportGenerator.AzureBlobHistoryStorage;

public class CommandLineArgumentsParser : ICommandLineArgumentsParser
{
    public Dictionary<string, string> GetCommandLineArguments()
    {
        var namedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (string arg in Environment.GetCommandLineArgs()) {
            var match = Regex.Match(arg, "--(?<key>\\w{2,}):(?<value>.+)");

            if (match.Success) namedArguments[match.Groups["key"].Value] = match.Groups["value"].Value;
        }

        return namedArguments;
    }
}
