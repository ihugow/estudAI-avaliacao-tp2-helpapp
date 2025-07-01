using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StockApp.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options; // Added for IOptions
using StockApp.Application.Configurations;

namespace StockApp.API.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageUploadSettings _settings; // Added for settings

        public ImageService(IWebHostEnvironment webHostEnvironment, IOptions<ImageUploadSettings> settings)
        {
            _webHostEnvironment = webHostEnvironment;
            _settings = settings.Value; // Get settings value
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File not selected or is empty.");
            }

            // Validate file size
            if (file.Length > _settings.MaxFileSizeMB * 1024 * 1024) // Convert MB to bytes
            {
                throw new ArgumentException($"File size exceeds the maximum limit of {_settings.MaxFileSizeMB} MB.");
            }

            // Validate file extension and content type
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(fileExtension) || !file.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("Invalid file type. Only image files (jpg, jpeg, png, gif) are allowed.");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }
    }
}