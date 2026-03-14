using Microsoft.AspNetCore.Http;

namespace ShopRavenDb.Domain.Core.Interfaces.Services;

public interface IFileValidator
{
    bool IsValid(IFormFile file, out string errorMessage);
}
