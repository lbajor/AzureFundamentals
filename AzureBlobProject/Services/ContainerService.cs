using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containers = new();

            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(blobContainerItem.Name);
            }

            return containers;
        }

        public async Task<List<string>> GetAllCotainerAndBlobs()
        {
            List<string> containerAndBlobName = new();
            containerAndBlobName.Add("-----Account Name : " + _blobServiceClient.AccountName + "-----");
            containerAndBlobName.Add("-------------------------------------------------------------------");

            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerAndBlobName.Add("-----"+blobContainerItem.Name);
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);

                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    containerAndBlobName.Add("->" + blobItem.Name);
                }
                containerAndBlobName.Add("-------------------------------------------------------------------");
            }

            return containerAndBlobName;
        }
    }
}
