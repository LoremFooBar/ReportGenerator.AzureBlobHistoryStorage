namespace ReportGenerator.AzureBlobHistoryStorage;

public interface IGitRepositoryAccessor
{
    public Task<IEnumerable<string>> GetCommitIdsAsync(int numOfCommits);

    public Task<string> GetHeadCommitIdAsync();
}
