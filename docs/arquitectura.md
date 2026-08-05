# Arquitectura de CreditFlow.API

Este documento resume la migración de la estructura del proyecto hacia una organización por capas (Domain / Application / Infrastructure / Shared), realizada en la rama `clean-architecture-v1` a partir de `main`, en 8 bloques de trabajo incrementales y commits separados.

## Estructura final de carpetas

```
CreditFlow.API/
├── Controllers/                    (13 archivos) — capa de presentación, sin mover
├── Domain/
│   └── Entities/                   (41 archivos) — entidades EF Core (namespace CreditFlow.API.Domain.Entities)
├── Application/
│   ├── Requests/                   (12 archivos) — DTOs de entrada de los endpoints (namespace CreditFlow.API.Application.Requests)
│   ├── DTOs/                       (4 archivos, 6 clases) — DTOs de salida de los endpoints (namespace CreditFlow.API.Application.DTOs)
│   ├── Interfaces/                 (13 archivos) — contratos de servicios de negocio y puertos técnicos (namespace CreditFlow.API.Application.Interfaces)
│   └── Services/                   (11 archivos) — implementación de la lógica de negocio (namespace CreditFlow.API.Application.Services)
├── Infrastructure/
│   ├── Services/                   (3 archivos) — implementaciones técnicas: Azure/Local Blob Storage, SMTP (namespace CreditFlow.API.Infrastructure.Services)
│   └── Data/
│       ├── DbNegocioContext.cs     — DbContext de EF Core (namespace CreditFlow.API.Infrastructure.Data)
│       └── Migrations/             (5 archivos) — historial de migraciones EF Core (namespace CreditFlow.API.Infrastructure.Data.Migrations)
├── Shared/
│   └── Helpers/                    (6 archivos) — utilidades transversales (namespace CreditFlow.API.Shared.Helpers)
├── Mappings/                       — extensiones de mapeo entidad↔DTO
└── Program.cs                      — composición raíz y registro de DI
```

`Services/` (carpeta original con implementaciones e interfaces mezcladas) quedó completamente vacía y fue eliminada del árbol.

## Decisión de acoplamiento pragmático: Application depende directamente de EF Core

En una clean architecture estricta, la capa Application no debería conocer el ORM: accedería a los datos a través de interfaces de repositorio definidas por Application e implementadas por Infrastructure. En este proyecto **no se introdujo esa capa de repositorios**. Las clases en `Application/Services/` inyectan y usan `DbNegocioContext` (de `Infrastructure.Data`) directamente vía `using CreditFlow.API.Infrastructure.Data;`.

Esto es una decisión pragmática, no un descuido:

- El proyecto ya tenía toda su lógica de negocio escrita contra `DbNegocioContext` y `DbSet<T>`/LINQ antes de esta migración. Introducir repositorios habría significado reescribir cada servicio, con alto riesgo de romper comportamiento, para un beneficio (poder sustituir EF Core por otro ORM) que este proyecto no necesita a corto plazo.
- El objetivo de los 8 bloques fue **reorganizar la ubicación física y los namespaces** para reflejar responsabilidades por capa, no invertir todas las dependencias del proyecto.
- Costo de esta decisión: Application queda acoplada a EF Core (no se puede testear con un ORM in-memory distinto, ni sustituir el motor de datos sin tocar Application). Si en el futuro se requiere aislar Application de EF Core, es una tarea aparte y bien delimitada: introducir interfaces de repositorio en `Application/Interfaces/` e implementarlas en `Infrastructure/Data/`.

## Namespaces vs. ubicación física

Cada carpeta declara el namespace que corresponde a su ruta (p. ej. `Domain/Entities/*.cs` → `CreditFlow.API.Domain.Entities`). Esto se corrigió explícitamente en los bloques 6 y 7, luego de detectar que un movimiento físico de archivos no había ido acompañado del cambio de `namespace` declarado dentro de cada archivo. Se verificó de punta a punta que no queda ningún rastro (declaración, `using`, ni referencia fully-qualified) de los namespaces viejos: `CreditFlow.API.Models`, `CreditFlow.API.Services`, `CreditFlow.API.Migrations`, `CreditFlow.API.Request`, `CreditFlow.API.Response`, `CreditFlow.API.Helpers`. La carpeta `Dto/` nunca existió en este árbol de trabajo (se descartó por búsqueda exhaustiva durante el bloque 5).

## ILineaCreditoAdminService — código muerto conocido

`Application/Interfaces/ILineaCreditoAdminService.cs` es una interfaz vacía (`public interface ILineaCreditoAdminService { }`), sin ninguna clase que la implemente, sin ninguna referencia en `Program.cs` ni en ningún consumidor del proyecto. Se confirmó por búsqueda exhaustiva en todo el repositorio (no solo `CreditFlow.API/`).

**Recomendación:** eliminarla en una tarea aparte, con su propio commit dedicado (p. ej. "chore: eliminar ILineaCreditoAdminService sin uso"), fuera de esta migración estructural. Se mantuvo movida (no eliminada) durante los bloques 6-8 por decisión explícita, para no mezclar una decisión de limpieza de código con el trabajo de reorganización de carpetas/namespaces.

## Deuda técnica conocida

### Gap de 14 entidades sin migración en el ModelSnapshot

Durante la validación de EF Core del bloque 7 (`dotnet ef migrations add` de prueba, revertido) se detectó que `Infrastructure/Data/Migrations/DbNegocioContextModelSnapshot.cs` está desincronizado respecto al modelo actual y respecto al propio `20260713232410_SincronizarModelo.Designer.cs` (la migración real más reciente): le faltan 14 entidades que sí están registradas como `DbSet` en `DbNegocioContext` y sí aparecen en el `BuildTargetModel` de `SincronizarModelo`:

`Agencia`, `CapacidadPago`, `CatSegmentoUsura`, `CredLineaCredito`, `Departamento`, `GarantiaFoto`, `LineaCatalogoAuxiliar`, `Mantenimiento`, `Municipio`, `PasswordChangeAudit`, `Role`, `SalarioMinimoVigente`, `TasaMaximaBcr`, `UsuarioLogin`, `UsuarioRole`.

Causa probable: en algún punto se corrió `Scaffold-DbContext ... -Force` contra la base de datos real (hay un comentario con ese comando al final de `Program.cs`), lo que regenera entidades y `DbContext` a partir del esquema real de la BD, pero **no** toca `Migrations/` ni el snapshot. Si la base de datos real ya tiene esas tablas, el código funciona en producción, pero la herramienta de migraciones de EF Core no lo sabe: `dotnet ef migrations add` generaría hoy una migración enorme y potencialmente destructiva (columnas renombradas, tablas recreadas) si se ejecutara sin revisar antes a mano.

**Este gap es preexistente a esta migración de arquitectura** (se confirmó comparando contra el commit inmediatamente anterior al bloque 7, antes de tocar cualquier namespace) y **se dejó fuera de alcance intencionalmente**. No se modificó el snapshot más allá del cambio de namespace correspondiente.

**Recomendación:** resolver en una tarea aparte, dedicada, que:
1. Revise a mano el diff real entre el modelo actual y el snapshot (`dotnet ef migrations add` en una rama descartable, revisando cada `CreateTable`/`RenameColumn`/`DropColumn` antes de aplicar nada).
2. Confirme contra la base de datos real cuáles de esas 14 tablas/columnas ya existen físicamente.
3. Genere una migración explícita y auditada que reconcilie el historial de EF Core con la realidad, sin pérdida de datos.

## Historial de los 8 bloques (rama `clean-architecture-v1`, desde `main`)

1. `refactor: mover Models a Domain/Entities`
2. `refactor: mover DbContext y migraciones a Infrastructure/Data`
3. `refactor: mover Services a Application e Infrastructure`
4. `refactor: mover Helpers a Shared/Helpers`
5. `refactor: mover Request/Response/Dto a Application/Requests y Application/DTOs`
6. `refactor: mover interfaces de servicios a Application/Interfaces e Infrastructure/Interfaces`
7. `fix: alinear namespaces declarados de Application/Services e Infrastructure/Services con su ubicación física`
8. `refactor: alinear namespaces de Domain/Entities e Infrastructure/Data con su ubicación física`

Cada commit se validó con `dotnet build` limpio (0 errores) antes de continuar al siguiente. El historial de migraciones de EF Core (`dotnet ef migrations list`) se validó al final de los bloques 7 y 8, confirmando que ambas migraciones (`20260324223508_InitialCreate`, `20260713232410_SincronizarModelo`) siguen siendo reconocidas correctamente.

No hay proyectos de test en la solución (`CreditFlow.sln` contiene un único proyecto); `dotnet test` no tiene nada que ejecutar.
