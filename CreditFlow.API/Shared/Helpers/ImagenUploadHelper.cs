using CreditFlow.API.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CreditFlow.API.Shared.Helpers;
public static class ImagenUploadHelper
{
    public static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
    public const long TamanoMaximoBytes = 5 * 1024 * 1024; // 5MB

    public static async Task<string?> ValidarYSubirAsync(IFormFile? archivo, string carpeta, IBlobStorageService blobService)
    {
        if (archivo == null || archivo.Length == 0)
            return null;

        var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (!ExtensionesPermitidas.Contains(ext))
            throw new InvalidOperationException($"Tipo de archivo no permitido: {ext}. Formatos aceptados: {string.Join(", ", ExtensionesPermitidas)}.");

        if (archivo.Length > TamanoMaximoBytes)
            throw new InvalidOperationException($"El archivo excede el tamaño máximo permitido ({TamanoMaximoBytes / (1024 * 1024)}MB).");

        var fileName = $"{Guid.NewGuid()}{ext}";
        await using var stream = archivo.OpenReadStream();
        return await blobService.UploadImageAsync(stream, carpeta, fileName);
    }
}
