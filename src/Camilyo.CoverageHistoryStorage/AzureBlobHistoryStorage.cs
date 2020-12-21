using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using Azure.Storage.Blobs;
using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core.Reporting.History;

namespace Camilyo.CoverageHistoryStorage
{
    [PublicAPI]
    public class AzureBlobHistoryStorage : IHistoryStorage
    {
        private readonly IGitRepositoryAccessor _gitRepositoryAccessor;
        private readonly BlobContainerClient _blobContainerClient;

        private readonly HttpClient _httpClient;

        [ExcludeFromCodeCoverage]
        public AzureBlobHistoryStorage()
        {
            _gitRepositoryAccessor = new GitRepositoryAccessor();
            _httpClient = new HttpClient();

            var historyContainerUrl = new Uri(GetRequiredEnvironmentVariable("COVERAGE_HISTORY_BLOB_URL"));
            string writeAccessSasToken = GetRequiredEnvironmentVariable("COVERAGE_AZURE_STORAGE_WRITE_SAS_TOKEN");
            var blobContainerUri = new UriBuilder(historyContainerUrl) {Query = writeAccessSasToken}.Uri;
            _blobContainerClient = new BlobContainerClient(blobContainerUri);
        }

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
                var blobs = _blobContainerClient.GetBlobs(prefix: commit.Sha);
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
            _blobContainerClient.UploadBlob(blobName, stream);
        }

        private static string GetRequiredEnvironmentVariable(string variableName) =>
            Environment.GetEnvironmentVariable(variableName) ??
            throw new RequiredEnvironmentVariableNotFoundException(variableName);
    }
}
