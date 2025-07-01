using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}