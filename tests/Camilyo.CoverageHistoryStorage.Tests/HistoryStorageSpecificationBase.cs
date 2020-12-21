﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Camilyo.Framework.Testing;
using LibGit2Sharp;
using Moq;
using RichardSzalay.MockHttp;

namespace Camilyo.CoverageHistoryStorage.Tests
{
    public class HistoryStorageSpecificationBase : SpecificationBase
    {
        protected const string FakeCoverageFileName = "fakeFile.xml";
        protected const string FakeCoverageFileContent = "file_content";
        protected readonly Uri ContainerUri = new Uri("https://storage.com/history?sas_token");

        private IEnumerable<Commit> _fakeCommitLog;

        protected Branch FakeHead;
        protected AzureBlobHistoryStorage HistoryStorage;
        protected Mock<BlobContainerClient> BlobContainerClientMock;

        protected override void BeforeAllTests()
        {
            base.BeforeAllTests();

            _fakeCommitLog = new[]
            {
                Mock.Of<Commit>(commit => commit.Sha == "commit1"),
                Mock.Of<Commit>(commit => commit.Sha == "commit2"),
                Mock.Of<Commit>(commit => commit.Sha == "commit3")
            }.AsEnumerable();

            FakeHead = Mock.Of<Branch>(b => b.Tip == _fakeCommitLog.First());
        }

        protected override void Given()
        {
            base.Given();

            SetupFakeEnvironment();

            var gitRepositoryAccessor = SetupGitRepositoryAccessor();
            BlobContainerClientMock = SetupBlobContainerClientMock();
            var httpClient = SetupHttpClient();

            HistoryStorage =
                new AzureBlobHistoryStorage(gitRepositoryAccessor, BlobContainerClientMock.Object, httpClient);
        }

        private HttpClient SetupHttpClient()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            string patternContainerUrl = $"{new UriBuilder(ContainerUri) {Query = ""}}/*";
            mockHttpMessageHandler
                .When(HttpMethod.Get, patternContainerUrl)
                .Respond(new StringContent(FakeCoverageFileContent));
            var httpClient = mockHttpMessageHandler.ToHttpClient();
            return httpClient;
        }

        private Mock<BlobContainerClient> SetupBlobContainerClientMock()
        {
            var blobContainerClientMock = new Mock<BlobContainerClient>();

            foreach (var commit in _fakeCommitLog) {
                var blobItems = Mock.Of<Pageable<BlobItem>>(items => items.GetEnumerator() == BlobEnumerator(commit));
                blobContainerClientMock
                    .Setup(client =>
                        client.GetBlobs(BlobTraits.None, BlobStates.None, commit.Sha, CancellationToken.None))
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

        private IGitRepositoryAccessor SetupGitRepositoryAccessor()
        {
            var commitLog = Mock.Of<IQueryableCommitLog>(log => log.GetEnumerator() == _fakeCommitLog.GetEnumerator());
            var repo = Mock.Of<IRepository>(repository =>
                repository.Commits == commitLog && repository.Head == FakeHead);
            var gitRepositoryAccessor = Mock.Of<IGitRepositoryAccessor>(accessor => accessor.GetRepository() == repo);
            return gitRepositoryAccessor;
        }


        private static IEnumerator<BlobItem> BlobEnumerator(Commit commit)
        {
            string blobName = $"{commit.Sha}/{FakeCoverageFileName}";
            yield return BlobsModelFactory.BlobItem(blobName);
        }


        private static void SetupFakeEnvironment()
        {
            Environment.SetEnvironmentVariable("COVERAGE_AZURE_STORAGE_WRITE_SAS_TOKEN", "token");
            Environment.SetEnvironmentVariable("COVERAGE_HISTORY_BLOB_URL", "https://url.com/blob");
        }
    }
}
