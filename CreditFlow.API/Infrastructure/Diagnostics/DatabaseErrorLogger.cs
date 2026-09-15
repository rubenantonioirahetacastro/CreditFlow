using CreditFlow.API.Core.Diagnostics;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using System;
using System.Runtime.CompilerServices;

namespace CreditFlow.API.Infrastructure.Diagnostics
{
    public class DatabaseErrorLogger
        : IErrorLogger
    {
        private readonly DbNegocioContext _context;
        public DatabaseErrorLogger(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            Exception exception,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "")
        {
            try
            {
                var origin = $"{Path.GetFileNameWithoutExtension(file)}.{method}";

                _context.LogErrores.Add(new LogErrore
                {
                    Origen = origin,
                    Mensaje = exception.InnerException?.Message ?? exception.Message,
                    StackTrace = exception.StackTrace,
                    TipoExcepcion = exception.GetType().Name,
                    FechaError = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
            catch
            {
                Console.WriteLine($"Fallo log BD: {exception.Message}");
            }
        }
    }

}
