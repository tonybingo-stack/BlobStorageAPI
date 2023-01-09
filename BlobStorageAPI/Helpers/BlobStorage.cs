

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageAPI.Helpers
{
    public class BlobStorage : IBlobStorage
    {
        public async Task<bool> CreateContainerForUser(string connectionString, string containerName)
        {
             var newContainer = BlobExtensions.AddContainer(connectionString, containerName);
             var newlyCreatedContainer = BlobExtensions.GetContainer(connectionString, containerName);

            if (await newlyCreatedContainer.ExistsAsync())
            {
                return true;
            }

            return false ;
        }

        public async Task<bool> DeleteContainerForUser(string connectionString, string containerName)
        {
            var deletedContainer = BlobExtensions.RemoveContainer(connectionString, containerName);
            var removedContainer = BlobExtensions.GetContainer(connectionString, containerName);

            if (await removedContainer.ExistsAsync())
            {
                return false;
            }

            return true;
        }
        public async Task<List<string>> GetAllDocuments(string connectionString, string containerName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);

            if (!await container.ExistsAsync())
            {
                return new List<string>();
            }

            List<string> blobs = new();

            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }
            return blobs;
        }

        public async Task UploadDocument(string connectionString, string containerName, string fileName, Stream fileContent)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            if (!await container.ExistsAsync())
            {
                BlobServiceClient blobServiceClient = new(connectionString);
                await blobServiceClient.CreateBlobContainerAsync(containerName);
                container = blobServiceClient.GetBlobContainerClient(containerName);
            }

            var bobclient = container.GetBlobClient(fileName);
            if (!bobclient.Exists())
            {
                fileContent.Position = 0;
                await container.UploadBlobAsync(fileName, fileContent);
            }
            else
            {
                fileContent.Position = 0;
                await bobclient.UploadAsync(fileContent, overwrite: true);
            }
        }
        public async Task<Uri> GetDocument(string connectionString, string containerName, string fileName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            if (await container.ExistsAsync())
            {
                var blobClient = container.GetBlobClient(fileName);
                if (blobClient.Exists())
                {
                    var content = blobClient.Uri;
                    return content;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

        }

   

        public async Task<bool> DeleteDocument(string connectionString, string containerName, string fileName)
        {
            var container = BlobExtensions.GetContainer(connectionString, containerName);
            if (!await container.ExistsAsync())
            {
                return false;
            }

            var blobClient = container.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class BlobExtensions
    {
        public static BlobContainerClient AddContainer(string connectionString, string containerName)
        {
            BlobServiceClient blobServiceClient = new(connectionString);
            return blobServiceClient.CreateBlobContainer(containerName);
        }

        public static BlobContainerClient RemoveContainer(string connectionString, string containerName)
        {
            BlobServiceClient blobServiceClient = new(connectionString);
            blobServiceClient.DeleteBlobContainer(containerName);
            return blobServiceClient.GetBlobContainerClient(containerName);
        }
        public static BlobContainerClient GetContainer(string connectionString, string containerName)
        {
            BlobServiceClient blobServiceClient = new(connectionString);
            return blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}
