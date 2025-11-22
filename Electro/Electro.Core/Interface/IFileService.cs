using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string filePath);

        Task<string> SavePdfAsync(IFormFile file, string folderName);
    }
}
