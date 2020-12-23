using System.IO;
using System.Linq;
using System.Threading;
using Camilyo.Framework.Testing;
using Moq;

namespace Camilyo.CoverageHistoryStorage.Tests
{
    public class When_Saving_New_History_File : HistoryStorageSpecificationBase
    {
        protected override void When()
        {
            base.When();

            var historyFilePaths = HistoryStorage.GetHistoryFilePaths();
            var stream = HistoryStorage.LoadFile(historyFilePaths.First());
            HistoryStorage.SaveFile(stream, FakeCoverageFileName);
        }

        [Then]
        public void It_Should_Upload_Blob()
        {
            BlobContainerClientMock.Verify(client =>
                client.UploadBlob($"{RepositoryName}/{FakeHead.Tip.Sha}/{FakeCoverageFileName}", It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
