# Instrucciones para asistentes

Antes de modificar `CreditFlow.Web`, leer completamente `ARCHITECTURE.md` y `DESIGN.md`, y revisar una implementación vecina dentro de la misma feature.

Toda página debe verse idéntica a la pantalla de referencia (Simulador de cronograma de cuotas): tipografía, tamaños, pesos, interlineado, colores por rol, espaciados y radios se toman exclusivamente de los tokens `--cds-*` descritos en `DESIGN.md`. El título de cada página se declara con `<CdsPageHeader Title="..." />` y se muestra solo en la barra superior; no usar `<h1>`–`<h3>` como título ni `<PageTitle>` directo.

Toda implementación debe conservar la organización feature-first, utilizar `IApiClient`, registrar dependencias desde la feature y mantener la autorización basada en IDs de rol. No crear carpetas globales de modelos o servicios ni usar `Maintenance` como contenedor genérico.

Antes de crear un botón, input, dropdown, chip, máscara o formatter nuevo, revisar `Core/UI/Components` y `Core/Utils/Format`. Los controles globales usan el prefijo `Cds`; los componentes exclusivos de una feature permanecen dentro de ella.

Toda superficie visual tipo tarjeta debe utilizar `CdsCard`. No repetir en una feature la combinación de fondo, borde y radio de una card; la feature solo puede aportar distribución interna, contenido y variantes funcionales mediante las extensiones previstas por el componente.

Si ya existe un componente con la misma responsabilidad, es obligatorio reutilizarlo. Para un estilo o comportamiento adicional, ampliar el componente existente mediante parámetros o variantes; no crear un componente paralelo. Si otra feature necesita un componente local, promoverlo antes de usarlo: a `Core/UI/Components` cuando sea visual y genérico, o a `Shared/<Capability>/Components` cuando represente un concepto funcional. Nunca copiar componentes entre features ni depender directamente de los componentes privados de otra feature.

Los colores de estados de crédito se resuelven únicamente mediante `Shared/Credit/Presentation/CreditStatusPalette.cs`. No duplicar colores de estados en páginas, grillas o componentes de una feature.

Todos los colores visuales consumen tokens `--cds-*` definidos en `wwwroot/app.css`. Está prohibido agregar valores hexadecimales, RGB o nombres de color directamente en features y componentes. Si no existe un token semántico apropiado, se crea primero en la paleta global.

La única fuente de verdad tipográfica es `--cds-font-family` en `wwwroot/app.css`. Toda pantalla y componente de texto debe heredarla o consumir el token; no declarar familias tipográficas dentro de una feature. Nunca aplicar esa fuente a `.rzi` ni a otra clase de iconos: las fuentes especializadas de iconografía deben conservarse.

Para estados bloqueantes de carga, error o ausencia de datos se utiliza `CdsPageState`; no crear spinners ni paneles equivalentes dentro de una feature. Los resultados no bloqueantes de acciones se comunican mediante `ICdsNotificationService`. Los errores iniciales permanecen visibles en la página con reintento, las eliminaciones requieren confirmación y las validaciones deben mostrarse dentro del formulario correspondiente.

Las páginas `.razor` son puntos de composición y no deben construir diseños mediante atributos `style`, controles HTML visuales repetidos ni superficies propias. Deben consumir componentes de `Core/UI`, `Shared` o `Features/<Feature>/Components`. Los componentes exclusivos de una feature conservan su diseño en `.razor.css`; cualquier valor visual dinámico se encapsula en el componente propietario mediante variables CSS.
