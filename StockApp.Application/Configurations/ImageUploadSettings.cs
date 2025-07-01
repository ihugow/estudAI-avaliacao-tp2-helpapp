using System.Collections.Generic;

namespace StockApp.Application.Configurations
{
    public class ImageUploadSettings
    {
        public List<string> AllowedExtensions { get; set; }
        public int MaxFileSizeMB { get; set; }
    }
}