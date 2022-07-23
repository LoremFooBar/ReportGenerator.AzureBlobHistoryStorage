using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests;

public class When_Getting_History_File_Paths : HistoryStorageSpecificationBase
{
    private List<string> _historyFilePaths;

    protected override void When()
    {
        base.When();

        _historyFilePaths = HistoryStorage.GetHistoryFilePaths().ToList();
    }

    [Then]
    public void It_Should_Return_Correct_Blob_Urls()
    {
        _historyFilePaths.Should().NotBeEmpty();
        _historyFilePaths.Should().HaveCount(3);

        var uris = _historyFilePaths.Select(s => new UriBuilder(s)).ToList();
        uris.Should().OnlyContain(u =>
            u.Host == ContainerUri.Host && u.Query == ContainerUri.Query && u.Scheme == ContainerUri.Scheme &&
            u.Uri.IsDefaultPort);
        uris.Should().SatisfyRespectively(
            first => first.Path.Should()
                .Be($"{ContainerUri.AbsolutePath}/{RepositoryName}/{FakeCommitIds[0]}/{FakeCoverageFileName}"),
            second => second.Path.Should()
                .Be($"{ContainerUri.AbsolutePath}/{RepositoryName}/{FakeCommitIds[1]}/{FakeCoverageFileName}"),
            third => third.Path.Should()
                .Be($"{ContainerUri.AbsolutePath}/{RepositoryName}/{FakeCommitIds[2]}/{FakeCoverageFileName}"));
    }
}
