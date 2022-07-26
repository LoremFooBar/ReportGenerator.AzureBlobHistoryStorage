﻿using Moq;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests;

public class When_Saving_New_History_File : HistoryStorageSpecificationBase
{
    protected override void When()
    {
        base.When();

        var historyFilePaths = HistoryStorage.GetHistoryFilePaths().ToList();
        var stream = HistoryStorage.LoadFile(historyFilePaths.First());
        HistoryStorage.SaveFile(stream, FakeCoverageFileName);
    }

    [Then]
    public void It_Should_Upload_Blob()
    {
        BlobContainerClientMock.Verify(client =>
            client.UploadBlob($"{RepositoryName}/{FakeHeadCommitId}/{FakeCoverageFileName}", It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}
