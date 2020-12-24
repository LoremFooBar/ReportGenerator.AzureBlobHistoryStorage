using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Palmmedia.ReportGenerator.Core.Reporting.History;

namespace Camilyo.CoverageHistoryStorage
{
    public class AzureBlobHistoryStorage : IHistoryStorage
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly IGitRepositoryAccessor _gitRepositoryAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _repositoryName;

        [ExcludeFromCodeCoverage]
        public AzureBlobHistoryStorage()
        {
            var commandLineArgumentsParser = new CommandLineArgumentsParser();
            var commandLineArguments = commandLineArgumentsParser.GetCommandLineArguments();
            var options = new CommandLineOptions(commandLineArguments);

            _repositoryName = options.RepositoryName;
            _gitRepositoryAccessor = new GitRepositoryAccessor();
            _httpClient = new HttpClient();

            var historyContainerUrl = new Uri(options.HistoryContainerUrl);
            var blobContainerUri = new UriBuilder(historyContainerUrl) {Query = options.WriteSasToken}.Uri;
            _blobContainerClient = new BlobContainerClient(blobContainerUri);
        }

        /// <summary>
        /// Initialize new instance of <see cref="AzureBlobHistoryStorage" /> for tests only
        /// </summary>
        public AzureBlobHistoryStorage(IGitRepositoryAccessor gitRepositoryAccessor,
            BlobContainerClient blobContainerClient, HttpClient httpClient, string repositoryName)
        {
            _gitRepositoryAccessor = gitRepositoryAccessor;
            _blobContainerClient = blobContainerClient;
            _httpClient = httpClient;
            _repositoryName = repositoryName;
        }

        public IEnumerable<string> GetHistoryFilePaths()
        {
            var commitIds = _gitRepositoryAccessor.GetCommitIdsAsync(50).GetAwaiter().GetResult();
            foreach (var commitId in commitIds) {
                Pageable<BlobItem> blobs;
                try {
                    blobs = _blobContainerClient.GetBlobs(prefix: $"{_repositoryName}/{commitId}");
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    throw;
                }

                var blob = blobs.LastOrDefault();
                if (blob == null) {
                    continue;
                }

                var blobUri =
                    new UriBuilder(_blobContainerClient.Uri) {Query = _blobContainerClient.Uri.Query};
                blobUri.Path += blobUri.Path.EndsWith('/') ? blob.Name : $"/{blob.Name}";
                string blobUriString = blobUri.ToString();
                yield return blobUriString;
            }
        }

        public Stream LoadFile(string filePath) => _httpClient.GetStreamAsync(filePath).GetAwaiter().GetResult();

        public void SaveFile(Stream stream, string fileName)
        {
            string headCommitId = _gitRepositoryAccessor.GetHeadCommitIdAsync().GetAwaiter().GetResult();
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
}
