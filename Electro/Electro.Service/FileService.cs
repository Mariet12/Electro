using eBook.Core.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Electro.Core.Interface;

namespace Electro.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            return $"{baseUrl}/uploads/{folder}/{uniqueFileName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            try
            {
                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }

        public async Task<string> SavePdfAsync(IFormFile file, string folderName)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = Path.Combine("uploads", folderName, fileName);
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // رجّع المسار النسبي فقط
            return "/" + relativePath.Replace("\\", "/");
        }

    }
}
