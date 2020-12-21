using System;
using System.Diagnostics.CodeAnalysis;
using LibGit2Sharp;

namespace Camilyo.CoverageHistoryStorage
{
    internal class GitRepositoryAccessor : IGitRepositoryAccessor
    {
        [ExcludeFromCodeCoverage]
        public IRepository GetRepository()
        {
            string? repositoryPath = Repository.Discover(Environment.CurrentDirectory);
            var repository = new Repository(repositoryPath);
            return repository;
        }
    }
}
