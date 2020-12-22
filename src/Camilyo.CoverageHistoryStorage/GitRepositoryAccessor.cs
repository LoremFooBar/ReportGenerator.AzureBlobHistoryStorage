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
            try {
                string? repositoryPath = Repository.Discover(Environment.CurrentDirectory);
                var repository = new Repository(repositoryPath);
                return repository;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
