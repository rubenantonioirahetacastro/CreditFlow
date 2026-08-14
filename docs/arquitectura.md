# Arquitectura de CreditFlow (API + Web)

Este documento resume dos migraciones sucesivas de estructura:

1. **`clean-architecture-v1`** (desde `main`): reorganizó `CreditFlow.API` en capas horizontales (`Domain` / `Application` / `Infrastructure` / `Shared`). Ver [Historial: clean-architecture-v1](#historial-clean-architecture-v1) más abajo.
2. **`feature-first-v1`** (desde `main`, posterior a la anterior): reorganizó **ambos proyectos** (`CreditFlow.API` y `CreditFlow.Web`) a una arquitectura feature-first / vertical-slice, tomando como referencia el patrón que `CreditFlow.Web/Features/BandejaVerificacion` y `Features/EvaluacionCredito` ya usaban. Es la estructura vigente hoy y se describe primero.

## Estructura actual — CreditFlow.API (feature-first)

Cada capacidad de negocio vive en `Features/<Nombre>/` con subcarpetas `Controllers/`, `Services/` (interfaz + implementación juntas), `Requests/` y `DTOs/` según aplique:

```
CreditFlow.API/Features/
├── Auth/                AuthController + TokenController (mismo dominio: login)
├── Mantenimientos/      agrupa los 5 CRUD administrativos que también están agrupados bajo un solo
│                        ítem "Mantenimientos" en el menú de navegación de la Web:
│   ├── Agencias/, Empleados/, Roles/, CatalogosCodigos/    uno por entidad
│   └── LineasCredito/   CRUD admin (LineaCreditoController) + ILineaCreditoService, el resolver de
│                        "línea aplicable a un subproducto/monto" que consumen Creditos y SolicitudCredito
├── BandejaVerificacion/ BandejaVerificacionController — solo GET listaCreditos-cpc; espeja el feature
│                        Web del mismo nombre (era una acción más de CreditoController, se separó)
├── EvaluacionCredito/   EvaluacionCreditoController — solo PUT actualizar-evaluacion; espeja el feature
│                        Web del mismo nombre (idem, separado de CreditoController)
├── Creditos/            lo que queda de CreditoController sin pantalla propia en el Web — lo consume
│                        la app móvil: simular-calendario, último crédito y crédito activo por persona.
│                        Llamado "Creditos" (no "Credito") a propósito — un namespace de un solo segmento
│                        igual al nombre de la entidad Domain.Entities.Credito produce CS0118 en C#
│                        ("es un namespace pero se usa como tipo") en cualquier archivo que la referencie
├── SolicitudCredito/    alta de solicitud (fotos, garantías, negocio, etc.)
├── Calendario/          generación de calendario de pagos; incluye Feriado y Gasto como colaboradores
│                        internos (sin controller propio, solo los usa CalendarioService)
├── Pago/                registro de pagos
├── CambioProducto/, Personas/, BuscarCliente/, Ubigeo/  controllers que hablan directo con
│                        DbNegocioContext, sin capa de servicio (no se introdujo una nueva) — sin
│                        feature propio del lado Web, por eso tampoco se subdividieron más
CreditFlow.API/
├── Domain/Entities/         sin cambios — 41 entidades EF Core, mapeadas por un único DbContext
├── Infrastructure/          DbNegocioContext + Migrations, y los 3 servicios técnicos (Blob, SMTP)
├── Application/Interfaces/  reducido a los 2 puertos técnicos realmente cross-feature:
│                            IBlobStorageService (Empleados + SolicitudCredito) e IEmailService (Auth + SolicitudCredito)
└── Shared/Helpers/          CalculadoraFinanciera, ErrorLogger, DateOnlyJsonConverter — multi-feature
```

## Estructura actual — CreditFlow.Web (feature-first)

Mismo patrón que ya usaban `Features/BandejaVerificacion` y `Features/EvaluacionCredito`: `Pages/`, `Services/`, `Models/` por feature.

```
CreditFlow.Web/Features/
├── Auth/                Login, PasswordTemporal, IAuthService/AuthApiService, AuthEndpoints (login/logout)
├── BandejaVerificacion/, EvaluacionCredito/    sin cambios — ya eran la referencia
├── Mantenimientos/      agrupa los 5 CRUD administrativos — mismo agrupamiento que el ítem único
│                        "Mantenimientos" del menú de navegación (NavMenu.razor)
│   ├── Agencias/, Roles/, LineasCredito/, CatalogosCodigos/   servicio migrado a IApiClient
│   └── Empleados/       el servicio se queda en HttpClient crudo (no IApiClient) porque sube fotos
│                        por multipart/form-data e IApiClient hoy solo habla JSON
└── AutorizarCredito/, BandejaCreditoReadecuacion/, BandejaCreditosAbonar/,
    BandejaCreditosAbonarAgencia/, BandejaCreditosDesembolsar/, RegistrarCreditosOficiales/,
    AbonoOficialDesembolso/     7 features, cada uno solo con su página placeholder ("en construcción")

CreditFlow.Web/
├── Shared/
│   ├── Components/GridEstandar.razor    grid genérico reutilizado por Mantenimientos + BandejaVerificacion
│   ├── Models/{RoleDto,ApiErrorResponse}.cs   RoleDto: lo usan Auth, Roles y el dropdown de Empleados;
│   │                                          ApiErrorResponse: lo usa IApiClient
│   └── CatalogoCodigos/     ya existía (carpeta renombrada de shared/ a Shared/ por consistencia de
│                            casing) — lookup de solo lectura reutilizado por EvaluacionCredito y CatalogosCodigos
├── Core/HttpClient/         IApiClient/ApiClient — se le agregó DeleteAsync (antes solo Get/Put/Post)
│                            porque Agencias, LineasCredito y CatalogosCodigos tienen borrado real por API
└── Components/              solo shell de la app: Layout/, App.razor, Routes.razor, _Imports.razor,
                             RedirectToLogin.razor, Pages/Home.razor, Pages/Error.razor
```

## Criterio para los límites de un feature en la API

La API tiene más de un consumidor (esta Web, y aparentemente una app móvil — de ahí `login-app` separado de `login-web`, y endpoints como `Credito/Persona/{id}/Ultimo` que ningún feature del Web usa). El criterio que se siguió:

- Si una acción de un controller la consume una pantalla/feature específica del Web, esa acción vive en el feature de la API con **el mismo nombre**, aunque eso implique partir un controller que antes mezclaba varias acciones (es lo que pasó con `CreditoController`: `listaCreditos-cpc` se separó a `Features/BandejaVerificacion` y `actualizar-evaluacion` a `Features/EvaluacionCredito`, porque cada una la consume una pantalla Web distinta). La ruta HTTP no cambia — varios controllers pueden declarar el mismo `[Route("api/Credito")]` mientras sus sub-rutas no choquen.
- Si una acción no la consume ninguna pantalla del Web (solo la app móvil, o ningún cliente conocido todavía), se queda agrupada por recurso/capacidad general (p. ej. lo que quedó en `Creditos`) — no tiene con qué feature del Web alinearse.
- Los 7 features placeholder de Otorgamiento en el Web (`AutorizarCredito`, `BandejaCreditoReadecuacion`, etc.) **no** tienen su contraparte en la API todavía, a propósito: no hay ningún endpoint que mover ni código que organizar. Crear carpetas vacías en la API solo para que el nombre coincida sería andamiaje sin contenido. Cuando se construya el backend de cada uno, ahí corresponde crear el feature de la API con el mismo nombre, siguiendo este mismo criterio.

## Alcance de la migración feature-first-v1 (qué se hizo y qué no)

- **Reorganización física + namespaces únicamente.** Mismas rutas HTTP, mismos contratos de DTOs, mismo comportamiento runtime. No se introdujo capa de repositorios ni se tocó `Domain/Entities`, `DbNegocioContext` ni las Migrations (mismo criterio que `clean-architecture-v1`, ver más abajo).
- **Única excepción con cambio de comportamiento interno** (sin cambiar el contrato HTTP): los 4 servicios de Mantenimientos en Web que eran JSON puro (Agencia, Role, LineaCredito, CatalogoCodigo) se migraron de `HttpClient` crudo + `AttachTokenAsync`/`ExtraerMensajeErrorAsync` duplicados en cada uno, a la abstracción `IApiClient` ya usada por `BandejaVerificacion`/`EvaluacionCredito`. Mismas URLs y headers; el único cambio observable es que el mensaje de error por defecto ante una respuesta no-JSON pasa a ser el genérico de `IApiClient` en vez de un texto particular por acción — solo se nota en ese caso borde, y solo si el servidor no devuelve JSON. `Empleado` se quedó en `HttpClient` crudo por el multipart de fotos (ver arriba).
- **Dos archivos ya marcados como muertos en el propio código se eliminaron**: `CreditFlow.Web/Components/Pages/Otorgamiento/BandejaVerificacion.razor` (comentario "Borrar este archivo", reemplazado hace tiempo por `Features/BandejaVerificacion`) y `CreditFlow.Web/Models/Mantenimientos/CatalogoCodigoDto.cs` (stub de 2 líneas sin clase).

### Corrección a la nota anterior sobre `ILineaCreditoAdminService`

La versión previa de este documento (sección `clean-architecture-v1`) decía que `ILineaCreditoAdminService` era una interfaz vacía sin implementación ni consumidores. **Eso ya no es así** (y probablemente no lo era desde hace tiempo): hoy tiene una implementación completa (`LineaCreditoAdminService`, en `Features/LineasCredito/Services/`), está registrada en `Program.cs`, y la usa `LineaCreditoController`. Queda como referencia histórica en la sección de abajo, pero no se debe seguir asumiendo como código muerto.

### `ISegmentoUsuraService` — eliminado

`ISegmentoUsuraService`/`SegmentoUsuraService`/`TasaUsuraException` (API, `Features/SegmentoUsura/`) estaban registrados en DI pero sin controller propio ni consumidores reales — `SimulacionCalendarioService` siempre reimplementó la misma validación de tope de usura inline (`ObtenerEvaluacionUsuraAsync`, que sigue ahí, activa, y sigue usando las entidades `CatSegmentoUsura`/`TasaMaximaBcr` directamente). Se detectó durante `feature-first-v1` y se eliminó después: los 3 archivos, su registro en `Program.cs`, y la carpeta `Features/SegmentoUsura/`. **No se tocó** la entidad `Domain/Entities/CatSegmentoUsura.cs`, su `DbSet` en `DbNegocioContext`, las migraciones, ni la lógica real de `SimulacionCalendarioService` — esas siguen vigentes y en uso.

### Hallazgos nuevos durante feature-first-v1 (documentados, no resueltos — fuera de alcance de un reorg)

- **`TokenController` vs `AuthController`** (API): `TokenController.Login` duplica `AuthController.Login_app`/`Login_web` con menos validaciones (sin bloqueo por intentos fallidos, sin auditoría). El Web usa `AuthController.Login_web`; `TokenController` parece legacy, posiblemente usado solo por un cliente móvil antiguo o ya no usado por nadie. Se movieron juntos a `Features/Auth/` por ser el mismo dominio (login), sin fusionar ni eliminar ninguno.
- **`AnalisisNegocio` duplicada** (API): existe tanto como entidad EF (`Domain/Entities/AnalisisNegocio.cs`) como una segunda clase casi vacía en `Shared/Helpers/AnalisisNegocio.cs` (mismo nombre, namespace distinto), sin ningún consumidor de esta última. Se dejó donde estaba.
- **`ICatalogoLookupService` / `CatalogoLookupApiService`** (Web, en `Services/` raíz): no están registrados en `Program.cs` ni los consume ninguna página — código muerto, aparentemente un intento anterior del mismo lookup que hoy resuelve `Shared/CatalogoCodigos/ObtenerCatalogoCodigos*`. Se dejaron donde estaban.

**Recomendación general para estos 3 puntos:** igual que con `ILineaCreditoAdminService` en su momento (y como ya se hizo con `SegmentoUsura`), resolver cada uno en una tarea aparte y dedicada (conectar o eliminar), no mezclado con trabajo de reorganización.

---

## Historial: clean-architecture-v1

Migración de la estructura del proyecto `CreditFlow.API` hacia una organización por capas (Domain / Application / Infrastructure / Shared), realizada en la rama `clean-architecture-v1` a partir de `main`, en 8 bloques de trabajo incrementales y commits separados. **Esta estructura por capas fue reemplazada por la feature-first descrita arriba**; esta sección queda como historial.

### Decisión de acoplamiento pragmático: Application depende directamente de EF Core

En una clean architecture estricta, la capa Application no debería conocer el ORM: accedería a los datos a través de interfaces de repositorio definidas por Application e implementadas por Infrastructure. En este proyecto **no se introdujo esa capa de repositorios** (y sigue sin introducirse tras `feature-first-v1`). Las clases de servicio inyectan y usan `DbNegocioContext` directamente.

Esto fue una decisión pragmática, no un descuido:

- El proyecto ya tenía toda su lógica de negocio escrita contra `DbNegocioContext` y `DbSet<T>`/LINQ antes de esta migración. Introducir repositorios habría significado reescribir cada servicio, con alto riesgo de romper comportamiento, para un beneficio (poder sustituir EF Core por otro ORM) que este proyecto no necesita a corto plazo.
- Costo de esta decisión: la capa de negocio queda acoplada a EF Core (no se puede testear con un ORM in-memory distinto, ni sustituir el motor de datos sin tocarla). Si en el futuro se requiere aislarla de EF Core, es una tarea aparte y bien delimitada: introducir interfaces de repositorio e implementarlas en `Infrastructure/Data/`.

### Namespaces vs. ubicación física

Cada carpeta declara el namespace que corresponde a su ruta. Esto se corrigió explícitamente en los bloques 6 y 7 de `clean-architecture-v1`, luego de detectar que un movimiento físico de archivos no había ido acompañado del cambio de `namespace` declarado dentro de cada archivo. Se verificó de punta a punta que no quedó ningún rastro (declaración, `using`, ni referencia fully-qualified) de los namespaces viejos: `CreditFlow.API.Models`, `CreditFlow.API.Services`, `CreditFlow.API.Migrations`, `CreditFlow.API.Request`, `CreditFlow.API.Response`, `CreditFlow.API.Helpers`.

**Nota de `feature-first-v1`:** el mismo tipo de desincronización volvió a aparecer durante esa migración, mecánicamente: varios archivos cuyo namespace declarado era la primera línea del archivo (sin ningún `using` antes) no coincidían con el patrón `sed` usado para el resto de los archivos (aparente problema de BOM/codificación) y quedaron con el namespace viejo hasta que el build lo evidenció. Se corrigieron manualmente uno por uno; el build final quedó en 0 errores en ambos proyectos.

### Historial de los 8 bloques de `clean-architecture-v1` (desde `main`)

1. `refactor: mover Models a Domain/Entities`
2. `refactor: mover DbContext y migraciones a Infrastructure/Data`
3. `refactor: mover Services a Application e Infrastructure`
4. `refactor: mover Helpers a Shared/Helpers`
5. `refactor: mover Request/Response/Dto a Application/Requests y Application/DTOs`
6. `refactor: mover interfaces de servicios a Application/Interfaces e Infrastructure/Interfaces`
7. `fix: alinear namespaces declarados de Application/Services e Infrastructure/Services con su ubicación física`
8. `refactor: alinear namespaces de Domain/Entities e Infrastructure/Data con su ubicación física`

Cada commit se validó con `dotnet build` limpio (0 errores) antes de continuar al siguiente.

## Deuda técnica conocida (no relacionada con ninguna de las dos migraciones de estructura)

### Gap de 14 entidades sin migración en el ModelSnapshot

Durante la validación de EF Core del bloque 7 de `clean-architecture-v1` (`dotnet ef migrations add` de prueba, revertido) se detectó que `Infrastructure/Data/Migrations/DbNegocioContextModelSnapshot.cs` está desincronizado respecto al modelo actual y respecto al propio `20260713232410_SincronizarModelo.Designer.cs` (la migración real más reciente): le faltan 14 entidades que sí están registradas como `DbSet` en `DbNegocioContext` y sí aparecen en el `BuildTargetModel` de `SincronizarModelo`:

`Agencia`, `CapacidadPago`, `CatSegmentoUsura`, `CredLineaCredito`, `Departamento`, `GarantiaFoto`, `LineaCatalogoAuxiliar`, `Mantenimiento`, `Municipio`, `PasswordChangeAudit`, `Role`, `SalarioMinimoVigente`, `TasaMaximaBcr`, `UsuarioLogin`, `UsuarioRole`.

Causa probable: en algún punto se corrió `Scaffold-DbContext ... -Force` contra la base de datos real (hay un comentario con ese comando al final de `Program.cs`), lo que regenera entidades y `DbContext` a partir del esquema real de la BD, pero **no** toca `Migrations/` ni el snapshot. Si la base de datos real ya tiene esas tablas, el código funciona en producción, pero la herramienta de migraciones de EF Core no lo sabe: `dotnet ef migrations add` generaría hoy una migración enorme y potencialmente destructiva (columnas renombradas, tablas recreadas) si se ejecutara sin revisar antes a mano.

**Recomendación:** resolver en una tarea aparte, dedicada, que:
1. Revise a mano el diff real entre el modelo actual y el snapshot (`dotnet ef migrations add` en una rama descartable, revisando cada `CreateTable`/`RenameColumn`/`DropColumn` antes de aplicar nada).
2. Confirme contra la base de datos real cuáles de esas 14 tablas/columnas ya existen físicamente.
3. Genere una migración explícita y auditada que reconcilie el historial de EF Core con la realidad, sin pérdida de datos.

No hay proyectos de test en la solución (`CreditFlow.sln` contiene los dos proyectos de aplicación, ninguno de test); `dotnet test` no tiene nada que ejecutar. La única red de seguridad durante ambas migraciones estructurales fue `dotnet build` limpio tras cada paso, más grep exhaustivo de namespaces viejos.
