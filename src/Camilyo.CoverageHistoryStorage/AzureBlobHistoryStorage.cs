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
        private readonly IGitRepositoryAccessor _gitRepositoryAccessor;
        private readonly BlobContainerClient _blobContainerClient;

        private readonly HttpClient _httpClient;

        [ExcludeFromCodeCoverage]
        public AzureBlobHistoryStorage()
        {
            var commandLineArgumentsParser = new CommandLineArgumentsParser();
            var commandLineArguments = commandLineArgumentsParser.GetCommandLineArguments();
            if (!commandLineArguments.TryGetValue("HISTORYCONTAINERURL", out string? historyContainerUrlArgument)) {
                Console.WriteLine("Error - missing argument -historycontainerurl");
                throw new ArgumentException("missing argument -historycontainerurl");
            }

            if (!commandLineArguments.TryGetValue("WRITESASTOKEN", out string? writeAccessSasTokenArgument)) {
                Console.WriteLine("Error - missing argument -writesastoken");
                throw new ArgumentException("missing argument -writesastoken");
            }

            _gitRepositoryAccessor = new GitRepositoryAccessor();
            _httpClient = new HttpClient();

            var historyContainerUrl = new Uri(historyContainerUrlArgument);
            var blobContainerUri = new UriBuilder(historyContainerUrl) {Query = writeAccessSasTokenArgument}.Uri;
            _blobContainerClient = new BlobContainerClient(blobContainerUri);
        }

        /// <summary>
        /// Initialize new instance of <see cref="AzureBlobHistoryStorage"/> for tests only
        /// </summary>
        public AzureBlobHistoryStorage(IGitRepositoryAccessor gitRepositoryAccessor,
            BlobContainerClient blobContainerClient, HttpClient httpClient)
        {
            _gitRepositoryAccessor = gitRepositoryAccessor;
            _blobContainerClient = blobContainerClient;
            _httpClient = httpClient;
        }

        public IEnumerable<string> GetHistoryFilePaths()
        {
            using var repository = _gitRepositoryAccessor.GetRepository();

            var commits = repository.Commits.Take(200).ToList();
            foreach (var commit in commits) {
                Pageable<BlobItem> blobs;
                try {
                    blobs = _blobContainerClient.GetBlobs(prefix: commit.Sha);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    throw;
                }

                var blob = blobs.FirstOrDefault();
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
            using var repository = _gitRepositoryAccessor.GetRepository();

            string blobName = $"{repository.Head.Tip.Sha}/{fileName}";
            try {
                _blobContainerClient.UploadBlob(blobName, stream);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
