using Azure.Storage.Blobs;
using Hyme.Application.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Hyme.Infrastructure.Services
{
    public class BlobService : IBlobService
    {
        
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _originalImageContainer;
        private readonly BlobContainerClient _originalVideoContainer;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _originalImageContainer = _blobServiceClient.GetBlobContainerClient("image-original");
            _originalVideoContainer = _blobServiceClient.GetBlobContainerClient("video-original");
        }

        public async Task DeleteImageAsync(string fileName)
        {
            await _originalImageContainer.DeleteBlobAsync(fileName);
        }

        public async Task DeleteVideoAsync(string fileName)
        {
            await _originalVideoContainer.DeleteBlobAsync(fileName);
        }

        public async Task UploadImageAsync(byte[] image, string fileName)
        {
            await _originalImageContainer.UploadBlobAsync(fileName, new BinaryData(image));
        }

        public async Task UploadVideoAsync(byte[] video, string fileName)
        {
            await _originalVideoContainer.UploadBlobAsync(fileName, new BinaryData(video));
        }
    }
}
