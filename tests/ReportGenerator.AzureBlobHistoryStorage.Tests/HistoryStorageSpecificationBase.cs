using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;
using RichardSzalay.MockHttp;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests;

public class HistoryStorageSpecificationBase : SpecificationBase
{
    protected const string FakeCoverageFileName = "fakeFile.xml";
    protected const string FakeCoverageFileContent = "file_content";
    protected const string RepositoryName = "testRepoName";
    protected readonly Uri ContainerUri = new("https://storage.com/history?sas_token");

    protected Mock<BlobContainerClient> BlobContainerClientMock;

    protected AzureBlobHistoryStorage HistoryStorage;

    protected static List<string> FakeCommitIds { get; } = new()
    {
        "24e014a19f8af59ddf7b0adb3d12dbacbcb37465", "12b8122e490b0ebed975ca0f2eaead37b16a9705",
        "1b6cfaed161d810ab647c0a4f737dfd2c15d286e",
    };

    protected static string FakeHeadCommitId { get; } = FakeCommitIds.First();

    protected override void Given()
    {
        base.Given();

        var gitRepositoryAccessor = SetupGitRepositoryAccessor();
        BlobContainerClientMock = SetupBlobContainerClientMock();
        var httpClient = SetupHttpClient();

        HistoryStorage =
            new AzureBlobHistoryStorage(gitRepositoryAccessor, BlobContainerClientMock.Object, httpClient,
                RepositoryName);
    }

    private HttpClient SetupHttpClient()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        string patternContainerUrl = $"{new UriBuilder(ContainerUri) { Query = "" }}/*";
        mockHttpMessageHandler
            .When(HttpMethod.Get, patternContainerUrl)
            .Respond(new StringContent(FakeCoverageFileContent));
        var httpClient = mockHttpMessageHandler.ToHttpClient();

        return httpClient;
    }

    private Mock<BlobContainerClient> SetupBlobContainerClientMock()
    {
        var blobContainerClientMock = new Mock<BlobContainerClient>();

        foreach (string commitId in FakeCommitIds) {
            var blobItems =
                Mock.Of<Pageable<BlobItem>>(items => items.GetEnumerator() == FakeBlobEnumerator(commitId));
            blobContainerClientMock
                .Setup(client =>
                    client.GetBlobs(BlobTraits.None, BlobStates.None, $"{RepositoryName}/{commitId}",
                        CancellationToken.None))
                .Returns(blobItems);
        }

        blobContainerClientMock
            .SetupGet(client => client.Uri)
            .Returns(ContainerUri);
        blobContainerClientMock
            .Setup(client => client.UploadBlob(It.IsAny<string>(), It.IsAny<Stream>(), CancellationToken.None))
            .Returns(It.IsAny<Response<BlobContentInfo>>());

        return blobContainerClientMock;
    }

    private static IGitRepositoryAccessor SetupGitRepositoryAccessor()
    {
        var fakeCommitIdsTask = Task.FromResult(FakeCommitIds.AsEnumerable());
        var fakeHeadCommitIdTask = Task.FromResult(FakeHeadCommitId);

        var gitRepositoryAccessor = Mock.Of<IGitRepositoryAccessor>(accessor =>
            accessor.GetCommitIdsAsync(It.IsAny<int>()) == fakeCommitIdsTask
            && accessor.GetHeadCommitIdAsync() == fakeHeadCommitIdTask);

        return gitRepositoryAccessor;
    }

    private static IEnumerator<BlobItem> FakeBlobEnumerator(string commitId)
    {
        string blobName = $"{RepositoryName}/{commitId}/{FakeCoverageFileName}";

        yield return BlobsModelFactory.BlobItem(blobName);
    }
}
