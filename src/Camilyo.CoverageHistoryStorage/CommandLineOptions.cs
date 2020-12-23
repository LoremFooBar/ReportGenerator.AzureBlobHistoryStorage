using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Camilyo.CoverageHistoryStorage
{
    public class CommandLineOptions
    {
        public string HistoryContainerUrl { get; }
        public string WriteSasToken { get; }
        public string RepositoryName { get; }

        public CommandLineOptions(IReadOnlyDictionary<string, string> commandLineArguments)
        {
            if (!commandLineArguments.TryGetValue("HISTORYCONTAINERURL", out string? historyContainerUrl)) {
                throw new CommandLineArgumentMissingException("--historycontainerurl");
            }

            if (!commandLineArguments.TryGetValue("WRITESASTOKEN", out string? sasTokenArgument)) {
                throw new CommandLineArgumentMissingException("--writesastoken");
            }

            string? bitbucketRepoSlug = Environment.GetEnvironmentVariable("BITBUCKET_REPO_SLUG");
            if (!commandLineArguments.TryGetValue("REPOSITORYNAME", out string? repositoryNameArgument) &&
                string.IsNullOrWhiteSpace(bitbucketRepoSlug)) {
                throw new CommandLineArgumentMissingException("--REPOSITORYNAME");
            }

            HistoryContainerUrl = historyContainerUrl;
            WriteSasToken = sasTokenArgument;
            RepositoryName = (repositoryNameArgument ?? bitbucketRepoSlug)!;
        }
    }

    [PublicAPI]
    public sealed class CommandLineArgumentMissingException : Exception
    {
        public string ArgumentName { get; }

        public CommandLineArgumentMissingException(string argumentName) : base($"missing argument {argumentName}")
        {
            ArgumentName = argumentName;
            Console.WriteLine(Message);
        }
    }
}
