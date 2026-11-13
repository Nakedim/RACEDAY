namespace RaceDay.Api.Services
{
    public interface BlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task<Stream> DownloadFileAsync(string fileName, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string containerName);
    }
}
