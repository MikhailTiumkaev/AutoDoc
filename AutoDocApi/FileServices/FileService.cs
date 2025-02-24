using Microsoft.AspNetCore.WebUtilities;

namespace AutoDocApi.FileServices
{
    public interface IFileService
    {
        Task<string> SaveSectionAsFileAsync(MultipartSection section, CancellationToken ct = default);
        Task<string> SaveFormFileAsFileAsync(IFormFile file, CancellationToken ct = default);
        
    }

    public class FileService: IFileService
    {
        public async Task<string> SaveFormFileAsFileAsync(IFormFile file, CancellationToken ct = default)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString();
            var fileFullName = $"{fileName}{extension}";

            var target = Path.Combine("UploadedFiles");
            Directory.CreateDirectory(target);

            var filePath = Path.Combine(target, fileFullName);
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024);
            await file.OpenReadStream().CopyToAsync(stream, ct)!;

            return filePath;
        }

        public async Task<string> SaveSectionAsFileAsync(MultipartSection section, CancellationToken ct = default)
        {    
            //large files can be stored in file storages like Azure Blob Storage, AWS S3, etc.  
            var extension = Path.GetExtension(section!.AsFileSection()?.FileName);
            var fileName = Guid.NewGuid().ToString();
            var fileFullName = $"{fileName}{extension}";

            var target = Path.Combine("UploadedFiles");
            Directory.CreateDirectory(target);

            var fileSection = section.AsFileSection();

            var filePath = Path.Combine(target, fileFullName);
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024);
            await fileSection?.FileStream?.CopyToAsync(stream, ct)!;

            return filePath;
        }
    }
}