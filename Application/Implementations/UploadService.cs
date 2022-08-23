using System.IO;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Utilities.Helper;

namespace Application.Implementations
{
    public class UploadService : IUploadService
    {
        private static readonly string
            StaticFolder = ConfigurationHelper.Configuration.GetValue<string>("StaticFolder");

        private static readonly string
            UploadFolder = ConfigurationHelper.Configuration.GetValue<string>("UploadFolder");

        public async Task<string> UploadFile(IFormFile file)
        {
            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
            var filePath = Path.Combine(StaticFolder, UploadFolder, fileName);
            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public bool DeleteFile(string fileName)
        {
            var path = Path.Combine(StaticFolder, UploadFolder, fileName);
            if (!File.Exists(path)) return false;
            File.Delete(path);
            return true;
        }
    }
}