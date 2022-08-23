using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadFile(IFormFile file);
        
        bool DeleteFile(string fileName);
    }
}