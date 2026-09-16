# Arquitectura de CreditFlow.Web

## Regla principal

La Web utiliza organización **feature-first**. Una implementación comienza identificando la funcionalidad propietaria, no el tipo técnico del archivo.

```text
Features/<Feature>/
├── Models/
├── Pages/
├── Services/
└── <Feature>FeatureRegistration.cs
```

No se deben volver a crear carpetas globales `Models`, `Services` o `Pages` para código de negocio.

## Relación con App y API

- Se reutilizan los mismos nombres conceptuales que en App y API cuando representan la misma capacidad: `Authentication`, `Credit`, `Simulator`, `Verification`, `Payment`, etc.
- Las estructuras no tienen que ser idénticas cuando la responsabilidad cambia entre proyectos.
- Una ruta pública en español no obliga a usar nombres internos en español.
- La Web no duplica reglas de negocio que pertenecen a la API.

## `Features/Maintenance`

`Maintenance` representa una sección funcional y de navegación propia de la Web. Sus elementos continúan separados por capacidad:

```text
Features/Maintenance/
├── Agency/
├── Catalog/
├── CreditLine/
├── Employee/
└── Role/
```

Reglas:

- No colocar archivos directamente en `Maintenance` salvo su registro de dependencias.
- No convertir `Maintenance` en una carpeta genérica para cualquier pantalla usada por un administrador.
- Una funcionalidad con identidad propia permanece como feature principal aunque un administrador pueda utilizarla.
- La existencia de `Maintenance` en la Web no obliga a crear la misma agrupación en la API.

## `Core`, `Shared` y `Components`

### `Core`

Infraestructura transversal que no pertenece a una feature: cliente HTTP, seguridad y navegación global.

El design system reutilizable vive en `Core/UI/Components`, siguiendo la misma convención del app. Sus componentes usan el prefijo `Cds`, por ejemplo `CdsButton`, `CdsTextField`, `CdsDropdown` y `CdsStatusBadge`.

`CdsCard` es la única superficie base para tarjetas. Centraliza fondo, borde, radio, espaciado e interacción. Las features pueden agregar clases para organizar el contenido, pero no deben reconstruir la superficie visual de una card con `background`, `border` y `border-radius` propios.

La tipografía global se define únicamente mediante `--cds-font-family` en `wwwroot/app.css`. Páginas, features, componentes, Bootstrap y Radzen deben consumir ese token; no se permiten familias tipográficas literales fuera de ese punto. Las clases de iconos, como `.rzi`, conservan obligatoriamente su fuente especializada y nunca reciben la fuente de texto global.

La paleta visual también tiene una única fuente de verdad: los tokens `--cds-*` de `wwwroot/app.css`. Ninguna feature, página, componente o modelo de presentación debe declarar valores hexadecimales, RGB o colores CSS literales. Se debe reutilizar el token semántico existente y, si aparece una necesidad visual distinta, registrar primero el nuevo token global.

Las máscaras y formatters puros viven en `Core/Utils/Format`. Deben poder utilizarse sin renderizar componentes.

### `Shared`

Contratos o servicios funcionales reutilizados realmente por varias features. No mover algo a `Shared` por una reutilización hipotética.

### `Components`

Contiene el shell propio de Blazor y los layouts. Los controles reutilizables del design system pertenecen a `Core/UI/Components`. Una página o componente exclusivo de una feature debe vivir en `Features/<Feature>/Components`.

La ubicación se decide por uso real:

- Usado únicamente por una feature: `Features/<Feature>/Components`.
- Control visual reutilizado por varias features: `Core/UI/Components`.
- Compartido entre varias features pero asociado a un concepto funcional concreto: `Shared/<Capability>/Components`.
- No mover componentes a compartido por reutilización futura o hipotética.

Reglas obligatorias de reutilización:

- Antes de crear un componente se debe buscar uno existente en `Core/UI/Components`, `Shared` y la feature actual.
- Si existe un componente que cumple la misma función, debe reutilizarse siempre. Está prohibido crear una copia, un equivalente local o una variante duplicada.
- Si se necesita otro estilo o comportamiento del mismo control, se amplía el componente existente mediante parámetros o variantes. Por ejemplo, un nuevo tipo visual de botón se agrega a `CdsButton`; no se crea otro botón paralelo.
- Para cualquier panel con apariencia de tarjeta se reutiliza `CdsCard` y se elige su `Padding`; una feature solamente define la distribución interna o una variante funcional realmente propia.
- Solo se crea un componente nuevo cuando su responsabilidad es realmente diferente y ningún componente existente puede extenderse sin mezclar conceptos.
- Si una segunda feature necesita un componente que actualmente pertenece a otra feature, no debe importarlo directamente ni copiarlo. Primero se promueve a una ubicación compartida.
- Un componente puramente visual promovido desde una feature pasa a `Core/UI/Components`.
- Un componente ligado a una capacidad del negocio pasa a `Shared/<Capability>/Components`.
- Después de promoverlo, ambas features deben utilizar la misma implementación compartida.
- Toda tipografía debe heredar o utilizar `var(--cds-font-family)`; para cambiar la fuente de la aplicación solo se modifica el token global de `wwwroot/app.css`.
- Todo color debe consumir un token global `var(--cds-*)`. Los valores reales de la paleta solo pueden existir en `wwwroot/app.css`.
- Las páginas son puntos de composición: coordinan navegación, estado y casos de uso, pero no construyen controles ni superficies visuales con estilos incrustados.
- El diseño reutilizable entre features pertenece a `Core/UI/Components`; el diseño exclusivo de un flujo pertenece a `Features/<Feature>/Components` y conserva sus reglas en un archivo `.razor.css`.
- No se permiten atributos `style` para construir diseños en páginas. Un valor visual calculado en ejecución debe quedar encapsulado dentro del componente propietario mediante variables CSS.

Un componente visual no debe contener reglas de negocio. Por ejemplo, `CdsDocumentField` aplica una máscara, mientras `Shared/Identity/Validation` valida el documento y la API conserva la regla definitiva.

### Estados y colores

- `CdsStatusBadge` solamente renderiza una apariencia; no conoce IDs ni nombres del negocio.
- La relación entre un estado de crédito y sus colores vive exclusivamente en `Shared/Credit/Presentation/CreditStatusPalette.cs`.
- `CreditStatusPalette` referencia los tokens globales de estado; no contiene valores hexadecimales ni RGB.
- Chips, filtros, grillas y tarjetas deben resolver la apariencia mediante `CreditStatusPalette.Resolve(statusId)` o utilizar `CreditStatusBadge`.
- Para agregar un estado de crédito nuevo se registra su ID y apariencia en esa paleta; no se escriben colores directamente en las pantallas.

### Estados de pantalla y notificaciones

- La carga inicial que bloquea una página o una región completa utiliza `CdsPageState` con `Loading`; no se crean spinners ni pantallas de carga particulares por feature.
- Un error que impide mostrar el contenido utiliza `CdsPageState` con `Error` y, cuando la operación puede repetirse, expone la acción de reintento.
- La ausencia válida de resultados utiliza `CdsPageState` con `Empty`. Las grillas deben conservar su estructura y acciones disponibles cuando el estado vacío permita crear el primer registro.
- Los resultados de una acción que no bloquean la pantalla, como guardar, actualizar, eliminar o una recarga secundaria, se comunican mediante `ICdsNotificationService`.
- No se utiliza una notificación flotante como única explicación de un error de carga inicial, porque puede desaparecer mientras la página continúa inutilizable.
- Las confirmaciones destructivas permanecen en un diálogo explícito.
- Los errores de validación se muestran junto al formulario o campo correspondiente; no se convierten en estados de página ni en notificaciones globales.
- Los componentes compartidos conservan semántica accesible: progreso y vacío anuncian `status`; los errores bloqueantes anuncian `alert`.

## Consumo de API

- Toda petición pasa por `Core/HttpClient/IApiClient`.
- Una feature no inyecta `HttpClient` directamente.
- Los errores de conexión, autenticación y respuestas de API se procesan mediante el cliente centralizado.
- Los endpoints operativos no envían el rol del usuario; la API lo obtiene del token.
- Los roles se comparan por ID mediante `Core/Security`, nunca por nombre.

## Dependencias entre features

- Una feature no debe utilizar el servicio privado de otra solo por conveniencia.
- Si dos features comparten exactamente el mismo contrato o comportamiento, se extrae a `Shared`.
- Si los casos de uso tienen significados diferentes, conservan modelos separados aunque sus campos se parezcan.
- Autenticación no administra empleados, roles ni catálogos.

## Validaciones

- Las validaciones de experiencia de usuario pertenecen a `Features/<Feature>/Validation` cuando contienen varias reglas o comparan campos.
- Una validación trivial puede utilizar Data Annotations directamente en el modelo del formulario.
- No colocar cadenas de validación y comparaciones de negocio dentro de una página `.razor` cuando puedan probarse de manera independiente.
- La Web puede anticipar una regla para orientar al usuario, pero la API siempre vuelve a validarla y conserva la autoridad.
- Los errores HTTP, de conexión y de deserialización no pertenecen a los validators; los resuelve `IApiClient`.

## Registro de dependencias

Cada feature registra sus propios servicios mediante `<Feature>FeatureRegistration.cs`. `Program.cs` compone features completas y no enumera todos sus servicios internos.

## Lista de comprobación

- [ ] La funcionalidad tiene una feature propietaria clara.
- [ ] Sus páginas, modelos y servicios están juntos.
- [ ] Revisé los componentes existentes antes de crear uno nuevo.
- [ ] Extendí el componente existente cuando la diferencia era únicamente una variante.
- [ ] No existe una copia local de un componente compartido.
- [ ] No se agregó una dependencia directa entre features sin justificarla.
- [ ] Las peticiones usan `IApiClient`.
- [ ] La autorización utiliza policies e IDs de rol.
- [ ] La ruta y el comportamiento existentes se conservaron.
- [ ] Las cargas, errores, vacíos y resultados de acciones usan el componente compartido apropiado.
- [ ] La página compone componentes de `Core`, `Shared` o su feature y no contiene diseños incrustados.
- [ ] El proyecto compila sin advertencias.
