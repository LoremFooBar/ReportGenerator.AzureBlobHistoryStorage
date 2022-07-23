using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.GitRepositoryAccessorTests;

public class When_Getting_Commit_Ids : GitRepositoryAccessorSpecificationBase
{
    private IEnumerable<string> _commitIds;

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        _commitIds = await GitRepositoryAccessor.GetCommitIdsAsync(50);
    }

    [Then]
    public void It_Should_Return_Commit_Ids()
    {
        _commitIds.Should().NotBeEmpty()
            .And.OnlyHaveUniqueItems()
            .And.OnlyContain(s => s.Length == 40 && !s.Contains('\n'));
    }
}
