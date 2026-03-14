using Microsoft.AspNetCore.Http;
using ShopRavenDb.Domain.Core.Interfaces.Services;
using System.IO;
using System.Linq;

namespace ShopRavenDb.Infrastructure.CrossCutting.Validators
{
    public class FileValidator : IFileValidator
    {
        private readonly string[] _allowedExtensions = { ".pdf", ".png", ".jpg", ".jpeg" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public bool IsValid(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (file == null || file.Length == 0)
            {
                errorMessage = "FileIsEmpty";
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                errorMessage = "FileSizeExceeded";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                errorMessage = "InvalidFileExtension";
                return false;
            }

            return true;
        }
    }
}
