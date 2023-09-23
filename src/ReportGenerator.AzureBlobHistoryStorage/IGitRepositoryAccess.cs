namespace ReportGenerator.AzureBlobHistoryStorage;

public interface IGitRepositoryAccessor
{
    public Task<IEnumerable<string>> GetCommitIdsAsync(int numOfCommits);

    public Task<string> GetHeadCommitIdAsync();

    /// <summary>
    /// Avoid git error "detected dubious ownership in repository"
    /// </summary>
    public Task VerifyDirectoryIsSafeAsync();
}
