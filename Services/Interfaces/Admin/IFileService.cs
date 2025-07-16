
namespace Online_Learning.Services.Interfaces.Admin
{
    public interface IFileService
    {
        Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> DeleteFilesAsync(List<string> filePaths);
    }
}