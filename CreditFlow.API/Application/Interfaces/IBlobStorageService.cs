using System.IO;
using System.Threading.Tasks;

namespace CreditFlow.API.Application.Interfaces
{
    public interface IBlobStorageService
    {
        // Sube un stream de imagen a blob storage y devuelve la ruta relativa (ej: "clientes/123/documentacion-a1b2c3.jpg")
        Task<string> UploadImageAsync(Stream fileStream, string folder, string fileName);

        // Descarga una imagen por su ruta relativa. Devuelve null si no existe.
        Task<Stream?> DownloadImageAsync(string blobPath);
    }
}
