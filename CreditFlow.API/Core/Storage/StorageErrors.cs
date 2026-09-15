using CreditFlow.API.Core.Errors;

namespace CreditFlow.API.Core.Storage;

public static class StorageErrors
{
    public static ErrorDefinition InvalidFileType(string extension, IEnumerable<string> allowedExtensions) => new(
        "storage_invalid_file_type",
        $"Tipo de archivo no permitido: {extension}. Formatos aceptados: {string.Join(", ", allowedExtensions)}.");

    public static ErrorDefinition FileTooLarge(long maximumMegabytes) => new(
        "storage_file_too_large",
        $"El archivo excede el tamaño máximo permitido ({maximumMegabytes}MB).");
}
