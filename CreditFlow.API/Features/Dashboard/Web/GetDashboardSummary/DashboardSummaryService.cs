using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Dashboard.Web.GetDashboardSummary;

/// <summary>
/// Agrega los indicadores de cartera (KPIs, series y distribuciones) que
/// alimentan el dashboard de la página Home. Todo el cálculo se hace en
/// memoria sobre las tablas de negocio ya existentes (Creditos, CredCalendario,
/// Agencias, Personas, CatalogoCodigos) — no se crean tablas ni columnas nuevas.
/// </summary>
public class DashboardSummaryService : IDashboardSummaryService
{
    // Catálogo 116 = "Estados del Credito"; 109 = "SUB_PRODUCTO" (1=Diario,
    // 2=Semanal, 3=Quincenal, 4=Mensual). NEstado=50 se usa como "Cancelado".
    private const int COD_CATALOGO_ESTADO_CREDITO = 116;
    private const int COD_CATALOGO_SUBPRODUCTO = 109;
    private const int ESTADO_CANCELADO = 50;
    private const int CUOTA_PAGADA = 1;

    private readonly DbNegocioContext _context;

    public DashboardSummaryService(DbNegocioContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync()
    {
        var hoy = DateTime.Today;

        var creditos = await _context.Creditos.AsNoTracking().ToListAsync();
        var calendario = await _context.CredCalendarios.AsNoTracking().ToListAsync();
        var agencias = await _context.Agencias.AsNoTracking().ToListAsync();
        var estadosCatalogo = await _context.CatalogoCodigos.AsNoTracking()
            .Where(c => c.NCodigo == COD_CATALOGO_ESTADO_CREDITO)
            .ToListAsync();
        var subProductoCatalogo = await _context.CatalogoCodigos.AsNoTracking()
            .Where(c => c.NCodigo == COD_CATALOGO_SUBPRODUCTO)
            .ToListAsync();

        var idsPersona = creditos.Where(c => c.IdPersona.HasValue).Select(c => c.IdPersona!.Value).Distinct().ToList();
        var personas = idsPersona.Count == 0
            ? new List<Persona>()
            : await _context.Personas.AsNoTracking().Where(p => idsPersona.Contains(p.IdPersona)).ToListAsync();

        var agenciaPorCodigo = agencias.ToDictionary(a => a.NCodAge, a => a.CNomAge);
        var personaPorId = personas.ToDictionary(p => p.IdPersona, p => p);
        var estadoPorValor = estadosCatalogo
            .Where(c => c.NValor != c.NCodigo) // excluye la fila "cabecera" del catálogo (NValor == NCodigo)
            .ToDictionary(c => c.NValor, c => c.CNomCod);
        var subProductoPorValor = subProductoCatalogo
            .Where(c => c.NValor != c.NCodigo)
            .ToDictionary(c => c.NValor, c => c.CNomCod);

        // Calendario agrupado por crédito, para calcular mora y cartera en riesgo
        // sin volver a golpear la base de datos por cada crédito.
        var calendarioPorCredito = calendario
            .GroupBy(c => (c.NCodAge ?? 0, c.NCodCred ?? 0))
            .ToDictionary(g => g.Key, g => g.ToList());

        string EtiquetaEstado(int nEstado) =>
            estadoPorValor.TryGetValue(nEstado, out var nombre) ? nombre : (nEstado == 0 ? "Sin estado" : $"Estado {nEstado}");

        string EtiquetaSubProducto(int nSubProd) =>
            subProductoPorValor.TryGetValue(nSubProd, out var nombre) ? nombre : $"Subproducto {nSubProd}";

        string EtiquetaAgencia(int nCodAge) =>
            agenciaPorCodigo.TryGetValue(nCodAge, out var nombre) ? nombre : "Sin agencia";

        // ---- KPIs ----
        var creditosActivos = creditos.Where(c => c.NEstado != ESTADO_CANCELADO).ToList();
        var carteraVigente = creditosActivos.Sum(c => c.NSaldoK);
        var montoColocadoTotal = creditos.Sum(c => c.NPrestamo);

        decimal carteraEnRiesgo = 0m;
        foreach (var credito in creditosActivos)
        {
            if (!calendarioPorCredito.TryGetValue((credito.NCodAge, credito.NCodCred), out var cuotas))
                continue;

            carteraEnRiesgo += cuotas
                .Where(c => c.DFecVenc.Date < hoy && c.NEstado != CUOTA_PAGADA)
                .Sum(c => Math.Max(0m, (c.NTotalCuota ?? 0m) - (c.NCapPag + c.NIntPag + c.NIntMorPag + c.NIgvPag)));
        }

        var porcentajeMorosidad = carteraVigente > 0 ? Math.Round(carteraEnRiesgo / carteraVigente * 100m, 2) : 0m;

        var baseTasa = creditosActivos.Where(c => c.NSaldoK > 0 && c.NTasaComp.HasValue).ToList();
        var tasaPromedioPonderada = baseTasa.Sum(c => c.NSaldoK) > 0
            ? Math.Round(baseTasa.Sum(c => c.NSaldoK * c.NTasaComp!.Value) / baseTasa.Sum(c => c.NSaldoK), 2)
            : 0m;

        var hace30Dias = hoy.AddDays(-30);
        var creditosUltimos30 = creditos.Where(c => c.DFecVig.Date >= hace30Dias).ToList();

        var kpis = new DashboardKpis
        {
            CarteraVigente = carteraVigente,
            MontoColocadoTotal = montoColocadoTotal,
            CreditosActivos = creditosActivos.Count,
            CreditosTotales = creditos.Count,
            CarteraEnRiesgo = carteraEnRiesgo,
            PorcentajeMorosidad = porcentajeMorosidad,
            TasaPromedioPonderada = tasaPromedioPonderada,
            CreditosUltimos30Dias = creditosUltimos30.Count,
            MontoUltimos30Dias = creditosUltimos30.Sum(c => c.NPrestamo)
        };

        // ---- Colocación mensual (últimos 12 meses, incluye meses en cero) ----
        var colocacionMensual = new List<SerieMensualItem>();
        var mesInicio = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-11);
        for (var i = 0; i < 12; i++)
        {
            var mes = mesInicio.AddMonths(i);
            var delMes = creditos.Where(c => c.DFecVig.Year == mes.Year && c.DFecVig.Month == mes.Month).ToList();
            colocacionMensual.Add(new SerieMensualItem
            {
                Periodo = mes.ToString("MMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-ES")),
                Monto = delMes.Sum(c => c.NPrestamo),
                Cantidad = delMes.Count
            });
        }

        // ---- Distribución por estado ----
        var porEstado = creditos
            .GroupBy(c => c.NEstado)
            .Select(g => new DistribucionItem { Etiqueta = EtiquetaEstado(g.Key), Cantidad = g.Count(), Monto = g.Sum(c => c.NPrestamo) })
            .OrderByDescending(d => d.Monto)
            .ToList();

        // ---- Distribución por subproducto ----
        var porSubProducto = creditos
            .GroupBy(c => c.NSubProd)
            .Select(g => new DistribucionItem { Etiqueta = EtiquetaSubProducto(g.Key), Cantidad = g.Count(), Monto = g.Sum(c => c.NPrestamo) })
            .OrderByDescending(d => d.Monto)
            .ToList();

        // ---- Distribución por agencia ----
        var porAgencia = creditos
            .GroupBy(c => c.NCodAge)
            .Select(g => new DistribucionItem { Etiqueta = EtiquetaAgencia(g.Key), Cantidad = g.Count(), Monto = g.Sum(c => c.NPrestamo) })
            .OrderByDescending(d => d.Monto)
            .ToList();

        // ---- Distribución de mora (por crédito, según su cuota vencida más antigua sin pagar) ----
        var rangos = new (string Rango, int Min, int Max)[]
        {
            ("Al día", int.MinValue, 0),
            ("1-30 días", 1, 30),
            ("31-60 días", 31, 60),
            ("61-90 días", 61, 90),
            ("Más de 90 días", 91, int.MaxValue)
        };
        var distribucionMora = rangos.Select(r => new RangoMoraItem { Rango = r.Rango, Cantidad = 0, Monto = 0m }).ToList();

        foreach (var credito in creditosActivos)
        {
            var diasAtraso = 0;
            if (calendarioPorCredito.TryGetValue((credito.NCodAge, credito.NCodCred), out var cuotas))
            {
                var vencidaMasAntigua = cuotas
                    .Where(c => c.DFecVenc.Date < hoy && c.NEstado != CUOTA_PAGADA)
                    .OrderBy(c => c.DFecVenc)
                    .FirstOrDefault();

                if (vencidaMasAntigua != null)
                    diasAtraso = (hoy - vencidaMasAntigua.DFecVenc.Date).Days;
            }

            var indice = Array.FindIndex(rangos, r => diasAtraso >= r.Min && diasAtraso <= r.Max);
            if (indice < 0) indice = 0;

            distribucionMora[indice].Cantidad++;
            distribucionMora[indice].Monto += credito.NSaldoK;
        }

        // ---- Top créditos por monto ----
        var topCreditos = creditos
            .OrderByDescending(c => c.NPrestamo)
            .Take(8)
            .Select(c => new TopCreditoItem
            {
                NCodAge = c.NCodAge,
                NCodCred = c.NCodCred,
                Cliente = c.IdPersona.HasValue && personaPorId.TryGetValue(c.IdPersona.Value, out var p)
                    ? $"{p.CNombres} {p.CPrimerApellido} {p.CSegundoApellido}".Replace("  ", " ").Trim()
                    : "Sin identificar",
                Monto = c.NPrestamo,
                Estado = EtiquetaEstado(c.NEstado),
                Agencia = EtiquetaAgencia(c.NCodAge)
            })
            .ToList();

        return new DashboardSummaryResponse
        {
            Kpis = kpis,
            ColocacionMensual = colocacionMensual,
            PorEstado = porEstado,
            PorSubProducto = porSubProducto,
            PorAgencia = porAgencia,
            DistribucionMora = distribucionMora,
            TopCreditos = topCreditos
        };
    }
}
