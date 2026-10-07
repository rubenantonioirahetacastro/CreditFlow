# CreditFlow API — arquitectura y reglas de implementación

Este documento es el contrato de arquitectura de `CreditFlow.API`. Está dirigido a personas y asistentes de IA que agreguen o modifiquen funcionalidades.

La prioridad es conservar el comportamiento del negocio, mantener una estructura **feature-first** y evitar que una implementación nueva introduzca dependencias, validaciones, autorización o formatos de error diferentes.

Las palabras **DEBE**, **NO DEBE**, **PUEDE** y **RECOMENDADO** expresan el nivel de obligatoriedad de una regla.

## Principios obligatorios

1. La organización principal es por feature, no por tipo técnico global.
2. Un endpoint debe ser delgado: recibe HTTP, delega la operación y devuelve el resultado.
3. Las reglas que consultan datos o modifican estado pertenecen al handler/service de la operación.
4. Las validaciones de entrada deben ejecutarse antes de modificar datos.
5. Los errores esperados deben tener un código estable y un mensaje público en español.
6. Los errores inesperados pasan por `Core/Errors/GlobalExceptionHandler`; no se capturan en cada controller.
7. La autorización se determina con el token y políticas del servidor. Un endpoint operativo nunca recibe el rol del solicitante.
8. Los nombres internos de carpetas, clases y métodos nuevos se escriben en inglés. Las rutas y respuestas públicas de la API se escriben en español.
9. No se crea una abstracción compartida hasta que exista reutilización real o una responsabilidad transversal clara.
10. Toda modificación debe compilar la solución completa y preservar los contratos consumidos por móvil y web.

## Estructura general

```text
CreditFlow.API/
  Core/
    Diagnostics/
    Email/
    Errors/
    Finance/
    Security/
    Serialization/
    Storage/

  Domain/
    Entities/

  Features/
    Agency/
    Authentication/
    Catalog/
    Client/
    Credit/
    Dashboard/
    Employee/
    Geography/
    Payment/
    Roles/
    Simulator/
    Verification/

  Infrastructure/
    Data/
    Diagnostics/
    Services/

  Program.cs
```

### `Features`

Contiene las capacidades de negocio y los casos de uso expuestos por la API. Cada cambio debe comenzar buscando la feature propietaria del comportamiento.

Una operación completa puede tener esta forma:

```text
Features/
  Credit/
    Web/
      UpdateCreditDecision/
        UpdateCreditDecisionController.cs
        UpdateCreditDecisionRequest.cs
        UpdateCreditDecisionValidator.cs
        IUpdateCreditDecisionHandler.cs
        UpdateCreditDecisionHandler.cs

    Shared/
      Errors/
        CreditErrors.cs
```

No todas las operaciones necesitan todos esos archivos. Una consulta sencilla puede usar controller + handler; no se crean interfaces, validators o services vacíos solo para simular una arquitectura más compleja.

### `Core`

Contiene mecanismos técnicos reutilizables por múltiples features y que no pertenecen al negocio de una sola feature.

Ejemplos válidos:

- Traducción global de excepciones a respuestas HTTP.
- Seguridad, claims, IDs de rol y políticas.
- Serialización común.
- Contratos transversales para almacenamiento, correo o diagnóstico.
- Algoritmos financieros verdaderamente compartidos.
- Validación técnica de archivos usada por varias features.

`Core` **NO DEBE**:

- Conocer controllers o DTOs de una feature.
- Contener reglas como “un crédito debe estar verificado para aprobarse”.
- Convertirse en un cajón para archivos difíciles de clasificar.
- Depender de `Features` o `Infrastructure`.

### `Infrastructure`

Contiene implementaciones técnicas externas o persistentes:

- `DbNegocioContext`, configuración de EF Core y migraciones.
- Azure Blob Storage y almacenamiento local.
- SMTP.
- Persistencia de logs y telemetría.
- Integraciones externas concretas.

Una interfaz transversal puede vivir en `Core` y su implementación en `Infrastructure`. Por ejemplo:

```text
Core/Storage/IBlobStorageService.cs
Infrastructure/Services/AzureBlobStorageService.cs
```

Una feature puede depender de una interfaz de `Core`; `Core` nunca depende de la implementación de `Infrastructure`.

### `Domain/Entities`

Las entidades representan el esquema relacional compartido y mapeado por EF Core. Están fuera de una feature porque tablas como `Creditos`, `Personas`, `UsuarioLogin` y catálogos participan en varios flujos.

En este proyecto son entidades de persistencia compartidas, no un dominio DDD completamente aislado. Por eso:

- Se conservan los nombres y tipos requeridos por el esquema existente.
- No se mueven copias de una misma entidad a cada feature.
- No se agregan propiedades solo para resolver necesidades visuales de móvil o web.
- No deben devolverse directamente como contrato público si exponen columnas internas o crean acoplamiento. El endpoint debe proyectar a su DTO/response.
- Las reglas de una operación no se colocan automáticamente en una entidad de EF; permanecen en la feature cuando dependen del caso de uso.

## Cómo decidir entre `Mobile`, `Web` y `Shared`

La decisión depende del consumidor y del contrato, no del dispositivo donde fue escrito el código.

### `Mobile`

Se usa cuando el endpoint o flujo existe únicamente para la aplicación móvil.

Ejemplos:

- Guardar una verificación realizada en campo.
- Obtener la pantalla inicial de un empleado móvil.
- Registrar un pago desde la app.

### `Web`

Se usa cuando el endpoint o flujo existe únicamente para el portal web administrativo u operativo.

Ejemplos:

- Administrar agencias, empleados, roles o líneas de crédito.
- Obtener la bandeja de evaluación.
- Actualizar la decisión de un crédito.

### `Shared`

Se usa solamente cuando móvil y web utilizan la misma regla, contrato o capacidad real.

Puede ser `Shared` cuando:

- Ambos clientes invocan el mismo endpoint con el mismo contrato y significado.
- Dos operaciones de la misma feature reutilizan una regla de negocio idéntica.
- Un error representa una condición general de toda la feature.

No debe ser `Shared` cuando:

- Dos clases solo se parecen, pero tienen reglas diferentes.
- Únicamente existe un consumidor “por si acaso luego se reutiliza”.
- Para compartirlo sería necesario agregar flags como `isMobile` o `isWeb`.

Regla práctica: si cambiar el comportamiento móvil obliga innecesariamente a probar o modificar web, probablemente no debía ser compartido.

## Flujo estándar de una petición

```text
Cliente HTTP
  -> autenticación/autorización
  -> model binding y validación del request
  -> controller
  -> validator de la operación, si es necesario
  -> handler/service de la feature
  -> DbContext o interfaces de Core
  -> response DTO
  -> cliente HTTP
```

### Controller

El controller **DEBE** limitarse a responsabilidades HTTP:

- Declarar ruta, verbo, consumo y autorización.
- Recibir route/query/body/form y `CancellationToken`.
- Delegar al handler/service.
- Devolver `Ok`, `Created`, `NoContent` u otro resultado exitoso.

El controller **NO DEBE**:

- Tener consultas extensas de EF Core si la operación ya amerita un handler.
- Decidir reglas complejas de negocio.
- Agregar el token o buscar manualmente el rol por nombre.
- Capturar `Exception`, `ArgumentException` o `InvalidOperationException` para fabricar respuestas.
- Repetir el formato de error global.

Ejemplo:

```csharp
[HttpPut("actualizar-evaluacion")]
public async Task<IActionResult> Update(
    UpdateCreditDecisionRequest request,
    CancellationToken cancellationToken)
{
    await handler.ExecuteAsync(request, cancellationToken);
    return NoContent();
}
```

### Request y validación estructural

Las restricciones independientes y declarativas viven en el request:

- Campo obligatorio: `[Required]`.
- Rango numérico: `[Range]`.
- Longitud: `[StringLength]`.
- Formato de correo: `[EmailAddress]`.

Todos los controllers usan `[ApiController]`. El formato automático de errores se configura globalmente en `Program.cs` y produce:

```json
{
  "codigo": "validation_error",
  "mensaje": "El monto debe ser mayor que cero.",
  "errores": {
    "Monto": ["El monto debe ser mayor que cero."]
  }
}
```

No se repite `if (request.Amount <= 0) return BadRequest(...)` cuando un atributo puede expresar correctamente la condición.

### Validator de operación

Se crea un validator dentro de la operación cuando la regla compara campos o necesita una validación compuesta sin consultar persistencia.

Ejemplos:

- Plazo mínimo no puede superar plazo máximo.
- Monto mínimo no puede superar monto máximo.
- El estado solicitado pertenece al conjunto permitido.
- Cierta combinación de campos es incompatible.

El validator puede lanzar `RequestValidationException` con un error definido por la feature.

No se crea un `GlobalValidator` con reglas de todos los módulos.

### Handler o service

Las reglas que consultan base de datos, modifican estado o coordinan varias acciones pertenecen al handler/service.

Ejemplos:

- Verificar que un crédito exista.
- Aprobar solamente si existe una verificación.
- Evitar una segunda verificación del mismo crédito.
- Resolver una línea de crédito según producto, monto y plazo.
- Aplicar un pago a cuotas pendientes.

Un handler **DEBE**:

- Mantener atómica la operación cuando corresponda.
- Usar una transacción para múltiples escrituras dependientes.
- Pasar `CancellationToken` a EF Core y servicios que lo admitan.
- Lanzar errores tipados para fallos esperados.
- Dejar que errores inesperados lleguen al manejador global.

## Errores

### Contrato público

Todo error nuevo debe poder representarse con:

```json
{
  "codigo": "credit_verification_required",
  "mensaje": "La solicitud debe estar verificada antes de poder aprobarse."
}
```

- `codigo`: estable, en inglés, `snake_case`, útil para que móvil y web reaccionen sin comparar textos.
- `mensaje`: público, claro y en español.
- `errores`: opcional; contiene errores por campo.

Los clientes **NO DEBEN** tomar decisiones comparando `mensaje`; deben usar `codigo` o el estado HTTP.

### Dónde se define un error

Un error va en `Features/<Feature>/Shared/Errors/<Feature>Errors.cs` cuando representa una condición reconocible de esa feature o se utiliza en más de una operación.

Ejemplos:

- `CreditErrors.VerificationRequired`.
- `PaymentErrors.NoPendingInstallments`.
- `EmployeeErrors.RoleNotFound(...)`.

Un error puede permanecer junto al endpoint cuando es completamente local, no se reutiliza y extraerlo no mejora la comprensión. No se deben llenar catálogos globales con mensajes de una sola línea usados una vez.

Un error va en `Core` únicamente si es técnico y transversal, por ejemplo tipo/tamaño de archivo o un fallo común de infraestructura.

### Tipos y códigos HTTP

| Situación | Tipo | HTTP |
|---|---|---:|
| Request inválido | `RequestValidationException` | 400 |
| Token/identidad insuficiente | `UnauthorizedAppException` | 401 |
| Recurso inexistente | `ResourceNotFoundException` | 404 |
| Conflicto con estado actual o duplicado | `ResourceConflictException` | 409 |
| Regla de negocio no procesable | `BusinessRuleException` | 422 |
| Fallo inesperado | excepción original | 500 |

No se usa `Exception`, `ArgumentException` o `InvalidOperationException` para representar una regla esperada del negocio.

`GlobalExceptionHandler` es la única frontera para excepciones no controladas. Nunca se expone `exception.Message`, stack trace, SQL, rutas internas o configuración al cliente.

Los errores esperados se registran como advertencia. Los fallos inesperados se registran como error con su excepción.

## Autenticación, roles y autorización

### Fuente de identidad

La identidad se resuelve al iniciar sesión y se incluye en el JWT mediante claims. Las operaciones posteriores leen esos claims.

Claims relevantes:

- `NameIdentifier`: usuario autenticado.
- `IdRol`: uno o varios roles activos.
- `IdPersona`: persona vinculada, cuando existe.
- `IdEmpleado`: empleado vinculado, cuando existe.
- `Documento`: documento autenticado.

### Regla obligatoria sobre roles

Un endpoint operativo **NO DEBE recibir el rol del usuario** en body, query, form o route para decidir permisos.

Incorrecto:

```json
{
  "idRol": 3,
  "credito": 120
}
```

Correcto:

- El cliente envía la operación solicitada.
- La API identifica al usuario y sus roles mediante el JWT.
- La policy decide si puede acceder.
- El handler usa `IdEmpleado` o `IdPersona` del claim cuando necesita identificar al actor.

La única excepción válida es una operación administrativa cuyo propósito sea **asignar o cambiar el rol de otra cuenta**, por ejemplo crear o editar un empleado. En ese caso `IdRol` es dato de la entidad administrada, no autorización del solicitante.

### Roles por ID

- Las reglas usan `Core/Security/RoleIds.cs`.
- Nunca se autoriza comparando nombres como `"Administrador"` o `"Verificador"`.
- Los nombres de rol son únicamente etiquetas de presentación.
- Los administradores globales son los roles configurados en `RoleIds.GlobalAdministrators`.
- Un usuario puede tener múltiples roles activos; no se debe asumir un único claim `IdRol`.

### Capacidades

- `Core/Security/Capabilities.cs` define los códigos estables (`credit_request`, `simulate`, `prospect`, `verify`, `assigned_clients`, `offline_sync`) y `Core/Security/RoleCapabilities.cs` es el único mapa rol -> capacidades.
- El login móvil devuelve `Capacidades` junto a `IdRol` e `IdRoles`, y también `IdUsuario` e `IdEmpleado` (contrato aditivo). `IdPersona` puede ser nulo (un empleado puede no tener registro en `Personas`); la identidad que nunca falta es `IdUsuario`, y es la que la app usa como dueño de sus datos locales. Los clientes solo las usan para decidir la experiencia; la autorización sigue resolviéndose en el servidor con policies.
- `RoleCapabilities.MobileAccess` (login móvil), `RoleCapabilities.EmployeeAccess` (datos comunes del empleado) y `RoleCapabilities.VerificationAccess` (operaciones de verificación) **se derivan del mismo mapa**: el acceso real y lo que ve la app no pueden desalinearse. No se mantienen listas de roles aparte para estas capacidades.
- Un rol nuevo se registra en `RoleIds` y en `RoleCapabilities`; no se agrega lógica por rol en móvil ni web.

### Policies

Los endpoints usan `[Authorize]` o `[Authorize(Policy = ...)]` con las policies de `Core/Security/AuthorizationPolicies.cs`.

Si aparece una capacidad de autorización nueva y se reutiliza, se agrega una policy. No se copian arreglos de roles dentro de cada controller.

No debe enviarse el rol desde móvil/web para escoger datos. El endpoint debe conocer el alcance autorizado por el token, la agencia o el empleado autenticado.

## Consumo de endpoints y reutilización técnica

Todo consumidor oficial de la API debe pasar por el cliente HTTP compartido de su plataforma:

- Web: `CreditFlow.Web/Core/HttpClient/IApiClient`.
- Móvil: cliente/self-call compartido y su mapper común de errores.

Esos clientes son responsables de:

- Adjuntar el token.
- Serializar/deserializar.
- Detectar conexión, timeout y respuesta inválida.
- Leer `codigo`, `mensaje` y `errores`.
- Mapear `401`, `403`, `404`, `409`, `422` y `500`.

Una feature cliente no debe utilizar `HttpClient`/Ktor directamente ni duplicar `try/catch`, headers o parseo de errores.

Dentro de la API no se invoca otro controller mediante HTTP. Se reutiliza el handler/service o una interfaz compartida de la feature.

## Acceso a datos

- Las consultas de solo lectura usan `AsNoTracking()`.
- Se proyecta en SQL al DTO cuando sea posible; no se carga una entidad completa para mostrar dos campos.
- Se evita N+1. Las consultas dentro de bucles deben justificarse o agruparse.
- Toda consulta debe limitarse por las claves correctas. Para crédito normalmente se requiere la clave compuesta `NCodAge + NCodCred`.
- No se usa solamente `NCodCred` si no está garantizado que sea globalmente único.
- Las escrituras relacionadas usan una transacción.
- Una transacción no debe incluir llamadas lentas externas si pueden ejecutarse después del commit.
- Se usa UTC para auditoría, tokens y eventos (`DateTime.UtcNow`). La presentación local corresponde al cliente.
- Los cambios del esquema se realizan mediante una migración o script versionado y luego se actualizan `DbNegocioContext` y las entidades.
- No se confía únicamente en validación de aplicación para unicidad o integridad: las restricciones críticas también pertenecen a la base de datos.

## Reglas actuales importantes del negocio

Estas reglas deben preservarse mientras negocio no solicite un cambio explícito:

### Crédito

- Los estados provienen del catálogo correspondiente; no se inventan nombres o IDs nuevos en una pantalla.
- Una decisión web solamente admite los estados definidos por `UpdateCreditDecisionValidator`.
- Un crédito solo puede aprobarse cuando existe una fila de verificación para la combinación agencia/crédito.
- La API, no móvil ni web, es responsable de aplicar el cambio definitivo de estado.
- La línea de crédito debe resolverse con producto, subproducto, monto, plazo y demás condiciones correspondientes.
- Cuando un crédito ya tiene `NCodLinea`, el simulador debe respetar esa línea asignada aunque ya no sea la línea activa para solicitudes nuevas.

### Verificación

- Solo una identidad autorizada por la policy de verificación puede guardar la operación.
- El verificador se obtiene de `IdEmpleado` en el token; no se recibe su nombre, rol o ID como autoridad desde el cliente.
- La ubicación es obligatoria y debe cumplir latitud `[-90, 90]` y longitud `[-180, 180]`.
- La solicitud debe encontrarse en el estado permitido para verificar.
- No se registra una segunda verificación para el mismo crédito.
- La verificación, actualización de datos, fotografías, auditoría y cambio de estado deben confirmarse como una sola operación transaccional.
- La API cambia el crédito al estado verificado al guardar correctamente; el cliente no envía el estado final.

### Evaluación

- El estado solicitado no es una decisión seleccionable para guardar.
- Aprobación exige verificación previa.
- Los estados operativos posteriores, como desembolso o vigencia, no se asignan desde la pantalla de evaluación.

### Pagos

- El monto debe ser mayor que cero.
- El crédito y su condición de calendario deben existir.
- Deben existir cuotas pendientes antes de aplicar el pago.
- La distribución del pago y actualización de saldo se realiza en el servidor.

### Autenticación

- Los permisos se evalúan por ID de rol, nunca por nombre.
- Los roles activos se incluyen en el token al iniciar sesión.
- Los endpoints operativos no aceptan roles declarados por el cliente.
- Los errores de login mantienen `Exito` y `Mensaje` por compatibilidad, y además deben incluir `Codigo` cuando sean modificados o creados.
- Contraseñas, tokens y credenciales nunca se escriben en logs ni respuestas.

## Registro de dependencias

Cada feature registra sus dependencias en:

```text
Features/<Feature>/<Feature>FeatureRegistration.cs
```

`Program.cs` debe limitarse a invocar `AddCreditFeature()`, `AddVerificationFeature()`, etc., además de configurar infraestructura transversal.

No se agregan individualmente en `Program.cs` todos los handlers internos de una feature.

Lifetimes recomendados:

- `Scoped`: handlers/services que usan `DbContext` o datos de petición.
- `Singleton`: servicios inmutables y thread-safe que no dependen de estado scoped.
- `Transient`: componentes livianos sin estado cuando realmente sea necesario.

Nunca se inyecta un servicio scoped dentro de un singleton.

## Nombres y contratos

- Carpetas, namespaces y clases internas nuevas: inglés claro.
- Rutas públicas: español y consistentes con las existentes.
- JSON público: español mientras sea parte del contrato existente.
- Códigos de error: inglés en `snake_case`.
- No se renombran rutas o campos consumidos sin coordinar y actualizar móvil + web.
- Se prefieren nombres perceptibles, como `GetCreditDetail`, `SaveVerification` o `UpdateCreditDecision`.
- Se evita `Manager`, `Helper` o `Utils` si el nombre no expresa una responsabilidad concreta.

Los endpoints públicos deben documentarse en Swagger y usar tipos request/response explícitos. No se devuelven objetos anónimos para contratos nuevos salvo respuestas triviales que no se reutilicen.

## Qué no hacer

- Crear carpetas globales `Controllers`, `Services`, `DTOs` o `Repositories` para funcionalidades nuevas.
- Crear un repositorio genérico para esconder EF Core sin aportar una regla o abstracción útil.
- Crear un caso de uso que solamente llame a otro método sin agregar intención, regla o coordinación.
- Duplicar validaciones entre controller, handler, móvil y web.
- Capturar todas las excepciones y devolver `400`.
- Exponer mensajes técnicos de SQL o stack traces.
- Comparar nombres de rol.
- Aceptar `IdRol` para autorizar al propio solicitante.
- Poner reglas específicas de crédito, pagos o verificación en `Core`.
- Compartir DTOs entre móvil y web si sus contratos evolucionan de forma distinta.
- Guardar secretos reales en `appsettings.json` versionado.
- Agregar números mágicos de estados o catálogos sin una constante con nombre o una consulta a su catálogo fuente.

## Plantilla para una feature nueva

```text
Features/
  Example/
    ExampleFeatureRegistration.cs

    Shared/
      Errors/
        ExampleErrors.cs

    Mobile/                      # solo si existe contrato móvil
      ExecuteExample/
        ExecuteExampleController.cs
        ExecuteExampleRequest.cs
        ExecuteExampleValidator.cs   # solo si hace falta
        IExecuteExampleHandler.cs     # solo si la interfaz aporta valor
        ExecuteExampleHandler.cs

    Web/                         # solo si existe contrato web
      GetExamples/
        GetExamplesController.cs
        GetExamplesResponse.cs
        GetExamplesHandler.cs
```

## Checklist antes de terminar una implementación

- [ ] Identifiqué la feature propietaria.
- [ ] Decidí `Mobile`, `Web` o `Shared` según consumidores reales.
- [ ] El controller solo contiene comportamiento HTTP.
- [ ] Las validaciones simples están en el request.
- [ ] Las comparaciones entre campos están en un validator de la operación.
- [ ] Las reglas con base de datos están en el handler/service.
- [ ] Los errores esperados tienen código estable y mensaje en español.
- [ ] No usé excepciones genéricas para reglas esperadas.
- [ ] No agregué `try/catch` repetido al controller.
- [ ] El endpoint usa `[Authorize]` o una policy adecuada.
- [ ] No recibo el rol del solicitante.
- [ ] Si necesito el actor, lo obtengo de claims.
- [ ] No comparo nombres de rol.
- [ ] Las consultas de lectura usan `AsNoTracking()` cuando aplica.
- [ ] Evité consultas N+1.
- [ ] Las escrituras atómicas usan transacción.
- [ ] Pasé `CancellationToken` cuando aplica.
- [ ] El response es un DTO y no expone datos internos innecesarios.
- [ ] Registré dependencias en `<Feature>FeatureRegistration`.
- [ ] No dupliqué infraestructura disponible en `Core`.
- [ ] Conservé contratos existentes de móvil y web.
- [ ] Actualicé este documento si introduje una decisión arquitectónica nueva.
- [ ] Ejecuté `dotnet build CreditFlow.sln --no-restore`.
- [ ] Ejecuté `git diff --check`.

## Regla final para asistentes de IA

Antes de modificar `CreditFlow.API`, un asistente debe leer este documento y revisar implementaciones vecinas dentro de la misma feature. No debe inferir autorización para cambiar contratos, estados, catálogos o esquema de base de datos.

Si una solicitud contradice una regla documentada de negocio, el asistente debe señalar la contradicción antes de aplicar el cambio. Si el negocio confirma una regla nueva, debe actualizar implementación, pruebas/validaciones, clientes afectados y este documento en la misma entrega.
