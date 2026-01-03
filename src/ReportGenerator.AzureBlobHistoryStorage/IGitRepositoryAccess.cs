namespace ReportGenerator.AzureBlobHistoryStorage;

public interface IGitRepositoryAccessor
{
    Task<IEnumerable<string>> GetCommitIdsAsync(int numOfCommits);

    Task<string> GetHeadCommitIdAsync();
}
