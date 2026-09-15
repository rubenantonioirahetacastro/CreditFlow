using CreditFlow.API.Core.Storage;
using CreditFlow.API.Core.Errors;
using Microsoft.AspNetCore.Http;

namespace CreditFlow.API.Core.Storage;
public static class ImageUploadHelper
{
    public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public static async Task<string?> ValidateAndUploadAsync(
        IFormFile? file,
        string folder,
        IBlobStorageService blobService)
    {
        if (file == null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new RequestValidationException(
                StorageErrors.InvalidFileType(extension, AllowedExtensions));

        if (file.Length > MaxFileSizeBytes)
            throw new RequestValidationException(
                StorageErrors.FileTooLarge(MaxFileSizeBytes / (1024 * 1024)));

        var fileName = $"{Guid.NewGuid()}{extension}";
        await using var stream = file.OpenReadStream();
        return await blobService.UploadImageAsync(stream, folder, fileName);
    }
}
