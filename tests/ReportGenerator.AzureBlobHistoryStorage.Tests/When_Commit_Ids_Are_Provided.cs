using Moq;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests;

public class When_Commit_Ids_Are_Provided : HistoryStorageSpecificationBase
{
    protected override void Given()
    {
        ProvideCommitIds = true;
        base.Given();
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        var historyFilePaths = HistoryStorage.GetHistoryFilePaths().ToList();
        var stream = HistoryStorage.LoadFile(historyFilePaths.First());
        HistoryStorage.SaveFile(stream, FakeCoverageFileName);
    }

    [Then]
    public void It_Should_Not_Use_Git_Commands()
    {
        Mock.Get(GitRepositoryAccessor).VerifyNoOtherCalls();
    }
}
