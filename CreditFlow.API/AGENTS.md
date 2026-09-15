# Instrucciones para asistentes de IA

Antes de analizar, crear o modificar código dentro de `CreditFlow.API`, se debe leer completamente [`ARCHITECTURE.md`](ARCHITECTURE.md).

`ARCHITECTURE.md` es el contrato vigente para:

- estructura feature-first;
- separación `Mobile`, `Web` y `Shared`;
- responsabilidades de `Core`, `Infrastructure` y `Domain/Entities`;
- validaciones y reglas de negocio;
- errores y códigos HTTP;
- autenticación, claims, roles y policies;
- acceso a datos y transacciones;
- registro de dependencias;
- compatibilidad con móvil y web.

Las reglas marcadas como **DEBE** o **NO DEBE** son obligatorias. Si una petición contradice una regla de negocio documentada, no se debe asumir silenciosamente el cambio: se debe señalar la contradicción y obtener confirmación.

Si una implementación introduce o modifica una decisión arquitectónica o regla de negocio, también debe actualizar `ARCHITECTURE.md`.
