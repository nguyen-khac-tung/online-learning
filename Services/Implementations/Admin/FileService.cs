using Online_Learning.Services.Interfaces;


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;


namespace Online_Learning.Services.Implementations.Admin
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _uploadBasePath;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger, IConfiguration configuration)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _uploadBasePath = _configuration["FileStorage:UploadPath"] ?? "uploads";
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder)
        {
            var uploadedFiles = new List<string>();
            if (files == null || !files.Any())
            {
                _logger.LogInformation("No files provided for upload.");
                return uploadedFiles;
            }

            if (string.IsNullOrEmpty(folder))
            {
                _logger.LogError("Folder name cannot be null or empty.");
                throw new ArgumentException("Folder name cannot be null or empty.", nameof(folder));
            }

            var basePath = !string.IsNullOrEmpty(_environment.WebRootPath)
                ? _environment.WebRootPath
                : _environment.ContentRootPath;
            var uploadsFolder = Path.Combine(basePath, _uploadBasePath, folder);
            var relativePath = $"/{_uploadBasePath}/{folder}/".Replace("\\", "/");

            try
            {
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Created directory: {UploadsFolder}", uploadsFolder);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        if (file.Length > 10 * 1024 * 1024)
                        {
                            _logger.LogWarning("File {FileName} exceeds 10MB limit.", file.FileName);
                            throw new InvalidOperationException($"File {file.FileName} exceeds 10MB limit.");
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            _logger.LogWarning("Invalid file type for {FileName}. Allowed types: {AllowedTypes}", file.FileName, string.Join(", ", allowedExtensions));
                            throw new InvalidOperationException($"File {file.FileName} has an invalid type. Allowed types: {string.Join(", ", allowedExtensions)}.");
                        }

                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        uploadedFiles.Add($"{relativePath}{fileName}");
                        _logger.LogInformation("Uploaded file: {FileName} to {FilePath}", file.FileName, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files to folder: {Folder}", folder);
                throw;
            }
            return uploadedFiles;
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("File path is null or empty.");
                    return false;
                }

                var basePath = !string.IsNullOrEmpty(_environment.WebRootPath)
                    ? _environment.WebRootPath
                    : _environment.ContentRootPath;
                var fullPath = Path.Combine(basePath, filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Deleted file: {FullPath}", fullPath);
                    return true;
                }
                _logger.LogWarning("File not found: {FullPath}", fullPath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<bool> DeleteFilesAsync(List<string> filePaths)
        {
            try
            {
                if (filePaths == null || !filePaths.Any())
                {
                    _logger.LogInformation("No file paths provided for deletion.");
                    return true;
                }

                var results = new List<bool>();
                foreach (var filePath in filePaths)
                {
                    results.Add(await DeleteFileAsync(filePath));
                }
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple files.");
                return false;
            }
        }
    }
}