using LibGit2Sharp;

namespace Camilyo.CoverageHistoryStorage
{
    public interface IGitRepositoryAccessor
    {
        public IRepository GetRepository();
    }
}
