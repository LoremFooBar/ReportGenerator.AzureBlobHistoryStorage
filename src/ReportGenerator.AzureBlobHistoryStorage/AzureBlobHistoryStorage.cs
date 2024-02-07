using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Palmmedia.ReportGenerator.Core.Reporting.History;

namespace ReportGenerator.AzureBlobHistoryStorage;

public class AzureBlobHistoryStorage : IHistoryStorage
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly string[]? _commitIds;
    private readonly IGitRepositoryAccessor _gitRepositoryAccessor;
    private readonly HttpClient _httpClient;
    private readonly string _repositoryName;

    [ExcludeFromCodeCoverage]
    public AzureBlobHistoryStorage()
    {
        var commandLineArguments = CommandLineArgumentsParser.GetCommandLineArguments();
        var optionsFromEnvironment = EnvironmentVariablesOptionsReader.ReadOptionsFromEnvironment();
        var options = new PluginOptions(optionsFromEnvironment.Any() ? optionsFromEnvironment : commandLineArguments);

        _repositoryName = options.RepositoryName;
        _gitRepositoryAccessor = new GitRepositoryAccessor(options.SourceDirectory);
        _httpClient = new HttpClient();

        var historyContainerUrl = new Uri(options.HistoryContainerUrl);
        var blobContainerUri = new UriBuilder(historyContainerUrl) { Query = options.WriteSasToken }.Uri;
        _blobContainerClient = new BlobContainerClient(blobContainerUri);

        _commitIds = options.CommitIds;
    }

    /// <summary>
    /// Initialize new instance of <see cref="AzureBlobHistoryStorage" /> for tests only
    /// </summary>
    public AzureBlobHistoryStorage(IGitRepositoryAccessor gitRepositoryAccessor,
        BlobContainerClient blobContainerClient, HttpClient httpClient, string repositoryName,
        string[]? commitIds = null)
    {
        _gitRepositoryAccessor = gitRepositoryAccessor;
        _blobContainerClient = blobContainerClient;
        _httpClient = httpClient;
        _repositoryName = repositoryName;
        _commitIds = commitIds;
    }

    public IEnumerable<string> GetHistoryFilePaths()
    {
        var commitIds = _commitIds ?? _gitRepositoryAccessor.GetCommitIdsAsync(50).GetAwaiter().GetResult();

        foreach (string commitId in commitIds) {
            Pageable<BlobItem> blobs;

            try {
                blobs = _blobContainerClient.GetBlobs(prefix: $"{_repositoryName}/{commitId}");
            }
            catch (Exception ex) {
                Console.WriteLine(ex);

                throw;
            }

            var blob = blobs.LastOrDefault();

            if (blob == null) continue;

            var blobUri =
                new UriBuilder(_blobContainerClient.Uri) { Query = _blobContainerClient.Uri.Query };
            blobUri.Path += blobUri.Path.EndsWith('/') ? blob.Name : $"/{blob.Name}";
            string blobUriString = blobUri.ToString();

            yield return blobUriString;
        }
    }

    public Stream LoadFile(string filePath) => _httpClient.GetStreamAsync(filePath).GetAwaiter().GetResult();

    public void SaveFile(Stream stream, string fileName)
    {
        string headCommitId = _commitIds?.FirstOrDefault() ??
                              _gitRepositoryAccessor.GetHeadCommitIdAsync().GetAwaiter().GetResult();
        string blobName = $"{_repositoryName}/{headCommitId}/{fileName}";

        try {
            _blobContainerClient.UploadBlob(blobName, stream);
        }
        catch (Exception ex) {
            Console.WriteLine(ex);

            throw;
        }
    }

    [ExcludeFromCodeCoverage]
    ~AzureBlobHistoryStorage()
    {
        try {
            _httpClient.Dispose();
        }
        catch {
            // ignored
        }
    }
}
