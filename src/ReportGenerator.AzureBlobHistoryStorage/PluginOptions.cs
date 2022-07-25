using JetBrains.Annotations;

namespace ReportGenerator.AzureBlobHistoryStorage;

public class PluginOptions
{
    public PluginOptions(IReadOnlyDictionary<string, string> commandLineArguments)
    {
        commandLineArguments.TryGetValue("DEBUG", out string? debug);
        Debug = debug?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

        Console.WriteLine(Debug ? $"command line arguments: {string.Join(" | ", commandLineArguments)}" : "NO DEBUG");

        if (!commandLineArguments.TryGetValue("HISTORYCONTAINERURL", out string? historyContainerUrl))
            throw new CommandLineArgumentMissingException("-historycontainerurl");

        if (!commandLineArguments.TryGetValue("WRITESASTOKEN", out string? sasTokenArgument))
            throw new CommandLineArgumentMissingException("-writesastoken");

        if (!commandLineArguments.TryGetValue("REPOSITORYNAME", out string? repositoryNameArgument))
            throw new CommandLineArgumentMissingException("-repositoryname");

        HistoryContainerUrl = historyContainerUrl;
        WriteSasToken = sasTokenArgument;
        RepositoryName = repositoryNameArgument;
    }

    public string HistoryContainerUrl { get; }
    public string WriteSasToken { get; }
    public string RepositoryName { get; }
    public bool Debug { get; }
}

[PublicAPI]
public sealed class CommandLineArgumentMissingException : Exception
{
    public CommandLineArgumentMissingException(string argumentName) : base($"missing argument {argumentName}")
    {
        ArgumentName = argumentName;
        Console.WriteLine(Message);
    }

    public string ArgumentName { get; }
}
