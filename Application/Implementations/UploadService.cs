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
        public async Task<string> UploadFile(IFormFile file)
        {
            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
            var filePath = Path.Combine(ConfigurationHelper.Configuration.GetValue<string>("StaticFile"),
                ConfigurationHelper.Configuration.GetValue<string>("UploadPath"), fileName
            );
            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);
            return fileName;
        }
    }
}