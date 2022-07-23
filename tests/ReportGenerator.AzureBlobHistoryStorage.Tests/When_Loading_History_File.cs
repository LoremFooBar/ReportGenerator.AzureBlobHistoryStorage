using System.Text;
using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests;

public class When_Loading_History_File : HistoryStorageSpecificationBase
{
    private Stream _responseStream;

    protected override void When()
    {
        base.When();

        var filePaths = HistoryStorage.GetHistoryFilePaths().ToList();
        _responseStream = HistoryStorage.LoadFile(filePaths.First());
    }

    [Then]
    public void It_Should_Return_File_Content()
    {
        byte[] buffer = new byte[_responseStream.Length];
        _ = _responseStream.Read(buffer, 0, (int)_responseStream.Length);
        string fileContent = Encoding.UTF8.GetString(buffer);
        fileContent.Should().Be(FakeCoverageFileContent);
    }
}
