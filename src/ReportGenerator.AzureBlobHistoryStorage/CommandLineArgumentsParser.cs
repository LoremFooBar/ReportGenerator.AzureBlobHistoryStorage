using System.Text.RegularExpressions;

namespace ReportGenerator.AzureBlobHistoryStorage;

public static class CommandLineArgumentsParser
{
    public static Dictionary<string, string> GetCommandLineArguments() =>
        ParseCommandLineArguments(Environment.GetCommandLineArgs());

    public static Dictionary<string, string> ParseCommandLineArguments(IEnumerable<string> args)
    {
        var namedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (string arg in args) {
            // note: matches with any number of dashes >1
            var match = Regex.Match(arg, "-(?<key>\\w{2,}):(?<value>.+)");

            if (match.Success) namedArguments[match.Groups["key"].Value] = match.Groups["value"].Value;
        }

        return namedArguments;
    }
}
