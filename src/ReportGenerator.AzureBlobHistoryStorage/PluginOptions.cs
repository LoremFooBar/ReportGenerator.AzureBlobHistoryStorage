using JetBrains.Annotations;

namespace ReportGenerator.AzureBlobHistoryStorage;

public class PluginOptions
{
    public PluginOptions(IReadOnlyDictionary<string, string> commandLineArguments)
    {
        commandLineArguments.TryGetValue("DEBUG", out string? debug);
        bool isDebug = debug?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

        Console.WriteLine(isDebug ? $"command line arguments: {string.Join(" | ", commandLineArguments)}" : "NO DEBUG");

        if (!commandLineArguments.TryGetValue("HISTORYCONTAINERURL", out string? historyContainerUrl))
            throw new CommandLineArgumentMissingException("-historycontainerurl");

        if (!commandLineArguments.TryGetValue("WRITESASTOKEN", out string? sasTokenArgument))
            throw new CommandLineArgumentMissingException("-writesastoken");

        if (!commandLineArguments.TryGetValue("REPOSITORYNAME", out string? repositoryNameArgument))
            throw new CommandLineArgumentMissingException("-repositoryname");

        if (commandLineArguments.TryGetValue("SOURCEDIRECTORY", out string? sourceDirectory))
            SourceDirectory = sourceDirectory;

        if (commandLineArguments.TryGetValue("COMMITIDS", out string? commitIds)) {
            CommitIds = commitIds.Split(new[] { ',', '\n', ' ' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        HistoryContainerUrl = historyContainerUrl;
        WriteSasToken = sasTokenArgument;
        RepositoryName = repositoryNameArgument;
    }

    public string HistoryContainerUrl { get; }
    public string WriteSasToken { get; }
    public string RepositoryName { get; }
    public string? SourceDirectory { get; }
    public string[]? CommitIds { get; }
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
