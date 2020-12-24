using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camilyo.CoverageHistoryStorage
{
    public interface IGitRepositoryAccessor
    {
        public Task<IEnumerable<string>> GetCommitIdsAsync(int numOfCommits);

        public Task<string> GetHeadCommitIdAsync();
    }
}
