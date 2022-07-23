using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.GitRepositoryAccessorTests;

public class When_Getting_Head_Commit_Id : GitRepositoryAccessorSpecificationBase
{
    private string _headCommitId;

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        _headCommitId = await GitRepositoryAccessor.GetHeadCommitIdAsync();
    }

    [Then]
    public async Task It_Should_Return_Head_Commit_Id()
    {
        _headCommitId.Should().NotBeNullOrWhiteSpace()
            .And.HaveLength(40)
            .And.NotContainAny("\n", "\r\n");

        string headCommitFromLog = (await GitRepositoryAccessor.GetCommitIdsAsync(1)).FirstOrDefault();
        _headCommitId.Should().Be(headCommitFromLog);
    }
}
