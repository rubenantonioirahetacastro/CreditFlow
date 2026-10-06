using System.Security.Claims;
using CreditFlow.API.Core.Storage;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Authentication.Shared.GetCurrentUserPhoto;

/// <summary>
/// Foto de perfil del usuario autenticado (barra superior de la Web), para cualquier rol.
/// El usuario se toma del token; no recibe identificadores, así nadie puede pedir la foto de otro.
/// </summary>
[Route("api/Auth")]
[ApiController]
public class GetCurrentUserPhotoController(DbNegocioContext context, IBlobStorageService blobStorage) : ControllerBase
{
    [HttpGet("me/foto")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserPhoto()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return NotFound();

        var photoPath = await context.UsuarioLogins
            .AsNoTracking()
            .Where(user => user.IdUsuario == userId)
            .Select(user => user.VFoto)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(photoPath))
            return NotFound();

        var stream = await blobStorage.DownloadImageAsync(photoPath);
        if (stream is null)
            return NotFound();

        var contentType = Path.GetExtension(photoPath).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };

        return File(stream, contentType);
    }
}
