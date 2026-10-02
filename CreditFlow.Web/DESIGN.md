# Guía visual de CreditFlow.Web

Este documento es el contrato visual de la Web. **Toda página debe verse idéntica a la pantalla de referencia**: mismo tipo de letra, tamaños, pesos, interlineado, colores, espaciados, radios y comportamiento. No se aceptan variaciones locales.

- **Pantalla de referencia:** `Features/Simulator/Pages/SimuladorCredito.razor` (Simulador de cronograma de cuotas) y sus componentes en `Features/Simulator/Components`.
- **Fuente de verdad de los valores:** los tokens de `wwwroot/app.css` (bloque «Sistema visual»). Este documento explica **qué token usar en cada caso**; nunca repite un valor literal en una feature.

Para cambiar el aspecto de toda la aplicación se modifica el token en `app.css`, nunca una página.

## 1. Reglas obligatorias

1. Ningún archivo de una feature o componente declara tamaños de letra, pesos, interlineados, colores, radios o espaciados literales cuando existe un token para ese rol. Se usan `var(--cds-*)`.
2. Solo existen los tamaños de la escala (sección 3). No se usan 11px, 12.5px, 15px ni otros intermedios.
3. Solo existen los pesos `regular` (500), `strong` (600) y `bold` (700). `bold` solo en la fila de totales.
4. El interlineado de todo texto es `--cds-line-height` (1.4). `--cds-line-height-tight` (1) solo para la cifra *hero* y para íconos o letras dentro de cajas de tamaño fijo.
5. No se usa el atajo `font: … inherit`. Es CSS inválido y el navegador descarta la regla completa. Se declaran `font-size`, `font-weight` y `line-height` por separado.
6. El color transmite significado. Verde y rojo solo para estados (éxito o descuadre) y totales; nunca para columnas completas ni decoración.
7. Toda página se titula con `<CdsPageHeader>` (sección 9). Ninguna página renderiza `<h1>`–`<h3>` como título ni `<PageTitle>` propio.
8. Toda animación respeta `@media (prefers-reduced-motion: reduce)` y se desactiva ahí.

## 2. Tipo de letra

Única familia: `--cds-font-family` (Segoe UI y sus alternativas). Todo texto la hereda. Nunca se aplica a íconos (`.rzi`).

La raíz de cada página usa la clase global `cds-page` (definida en `app.css`), que aplica la base tipográfica. Nunca se redeclara en la página:

```razor
<div class="cds-page cds-page--fill mi-pagina" @ref="paginaRootRef"> ... </div>
```

- `cds-page`: familia, `body`, peso `regular`, interlineado 1.4 y color `value`.
- `cds-page--fill`: la página ocupa exactamente el alto visible y sus tarjetas manejan el scroll interno. Se acompaña de `JS.FitAsync(paginaRootRef)` en `OnAfterRenderAsync`.
- La clase propia de la página (`mi-pagina`) solo define distribución: relleno `--cds-space-page-*` y `gap`.

Cifras en tablas, montos e inputs numéricos: `font-variant-numeric: tabular-nums` para que las columnas se alineen.

Los componentes de Radzen (inputs, textarea, diálogos, grillas) toman su texto de las variables `--rz-body-font-size`, `--rz-body-line-height`, `--rz-text-*` e `--rz-input-placeholder-color`, que en `app.css` están conectadas a los tokens `--cds-*`. No se redefinen en features: si un control de Radzen se ve con otro tamaño o color, se corrige esa conexión global.

## 3. Escala tipográfica

| Token | Valor | Uso |
|---|---|---|
| `--cds-font-size-label` | 12px | Rótulos de campo («Agencia», «Préstamo»), rótulos de KPI («Total a pagar»), encabezado de panel («Condiciones») |
| `--cds-font-size-small` | 13px | Celdas y encabezados de tabla, pills, chips de estado, badges («12 pagos»), leyendas |
| `--cds-font-size-body` | 14px | Texto base, inputs, selects, mensajes, valores de KPI, fila de totales |
| `--cds-font-size-title` | 16px | Título de tarjeta («Detalle de cuotas») y valor destacado de KPI |
| `--cds-font-size-heading` | 26px | Solo el título del formulario de la pantalla de acceso |
| `--cds-font-size-hero` | 40px | Cifra principal de la pantalla (máximo una por página). Tracking `--cds-letter-spacing-hero` |

Rótulos (`label`) llevan `letter-spacing: var(--cds-letter-spacing-label)`.

## 4. Paleta de marca y color de texto

### Paleta de marca

| Token | Valor | Uso |
|---|---|---|
| `--cds-brand-600` | #3061F2 | Color principal (`--cds-primary`): botones primarios, enlaces, pill seleccionada, foco, barra «Capital», «Flow» del logo |
| `--cds-brand-500` | #4973F2 | Hover y acentos secundarios (ícono del título en la barra superior) |
| `--cds-brand-400` | #809DF2 | Acento suave y anillo de foco |
| `--cds-brand-200` | #BBC8F2 | Bordes/contornos de elementos seleccionados |
| `--cds-neutral-50` | #F2F2F2 | Fondo general de la aplicación |
| `--cds-brand-700` / `-800` | derivados | Solo estados presionado/hover oscuros |
| `--cds-brand-tint` | #3061F2 al 10% | Fondo del ítem activo del menú y filas informativas |

Las features nunca usan `--cds-brand-*` directamente: consumen los roles (`--cds-primary`, `--cds-link`, `--cds-action-dark`, `--cds-surface-*`), que ya están mapeados a la paleta.

### Logo

- Completo: `wwwroot/images/crediflow-logo.svg` (barra superior a 35px de alto; login a 44px).
- Símbolo: `wwwroot/images/crediflow-mark.svg` (favicon y barra superior con el menú oculto).
- Nunca se redibuja ni se recolorea; se usa siempre el archivo.

### Color de texto por rol

| Token de rol | Equivale a | Se usa en |
|---|---|---|
| `--cds-color-value` | `--cds-text-primary` | Datos y valores, títulos, texto que escribe el usuario, fila de totales |
| `--cds-color-content` | `--cds-text-body` | Texto general, encabezados de tabla, pills, leyendas, montos en negrita de una fila |
| `--cds-color-label` | `--cds-text-secondary` | Rótulos de campo y de KPI, celdas normales de tabla |
| `--cds-color-hint` | `--cds-text-subtle` | Solo íconos decorativos (engranaje, «i» de ayuda) y placeholders |

Jerarquía: **los datos resaltan y los rótulos acompañan**. Si todo va oscuro y en negrita, la pantalla se satura.

Colores de estado: `--cds-success-*` (cuadrado, total general, saldo en cero) y `--cds-danger-*` (descuadre, errores). Los estados de crédito solo mediante `CreditStatusPalette`.

## 5. Espaciado y radios

| Token | Valor | Uso |
|---|---|---|
| `--cds-space-page-y` / `--cds-space-page-x` | 15px / 18px | Relleno de la página |
| `--cds-space-layout-top` / `--cds-space-layout-x` | 13px / 18px | Margen del contenedor de contenido del layout (junto con `--rz-layout-body-padding`: 12px) |
| `--cds-content-inset` / `--cds-content-top` | 48px / ≈40px | Distancia total del menú y de la barra superior al contenido; la barra superior alinea su título con `--cds-content-inset` |
| `--cds-space-columns` | 20px | Entre columnas o tarjetas lado a lado |
| `--cds-space-card` | 16px | Entre bloques dentro de una tarjeta y entre tarjetas apiladas |
| `--cds-space-stack` | 14px | Entre campos de un formulario; separación de secciones |
| `--cds-space-field` | 6px | Entre un rótulo y su control |
| `--cds-space-cell-y` / `--cds-space-cell-x` | 10px / 12px | Relleno de celdas y encabezados de tabla |
| `--cds-radius-card` | 16px | Tarjetas (ya aplicado por `CdsCard`) |
| `--cds-radius-control` | 10px | Inputs y selects |
| `--cds-radius-chip` | 9px | Pills y chips seleccionables |
| `--cds-radius-badge` | 7px | Badges informativos |

## 6. Componentes compartidos del patrón

Cada patrón visual se implementa **una sola vez** y las páginas lo componen. Así quedan idénticas por construcción.

| Patrón | Componente | Notas |
|---|---|---|
| Base de página | clase `cds-page` (+ `cds-page--fill`) | `app.css` |
| Título | `CdsPageHeader` | `Title`, `Icon` y `Description` en la barra superior + pestaña (sección 9) |
| Tarjeta | `CdsCard` | `Padding="None"` para tarjetas de tabla |
| Encabezado de tarjeta de tabla | `CdsCardHeader` | `Title`, `Badge` («12 pagos») y `<Actions>` a la derecha (estado, Excel, PDF) |
| Indicador de resumen | `CdsKpi` | `Label`, `Value`, `Description`, `Strong`, `Icon`. Resumen de página (fila de métricas): cada indicador con `Icon` (recuadro 44px en `--cds-surface-selected`) y `Strong`, del mismo ancho, separados por líneas verticales `--cds-border-soft`. Resumen junto a una cifra *hero* (Simulador): sin ícono y un solo `Strong` |
| Chip de estado | `CdsStatusBadge Variant="Chip"` | `Appearance` `Success`/`Danger`, `Icon`, `Celebrate` + `@key` para animar en cada resultado |
| Pestañas (filtros sobre una tabla, secciones de un expediente) | `CdsTabs` + `CdsTab` (`Text`, `Count` opcional) | Texto `small`; activa en `strong`/`value` con línea `--cds-primary` de 2px sobre el borde inferior y contador en `--cds-surface-selected`; sin colores de estado |
| Estado de un registro en tabla | texto plano (columna con `Property`) | Sin badges ni colores en tablas. `CreditStatusBadge` se reserva para fichas de detalle |
| Identificador que abre un detalle | `<a class="cds-link" href="...">` | Clase global de `app.css`; con `@onclick:stopPropagation` si la fila también navega |
| Exportar | `CdsExportButton` | `Format` Excel / Pdf |
| Tabla de datos | `GridEstandar` | Ya aplica la especificación de tabla. Dentro de una tarjeta: `FillHeight="true"`, `ShowExportButton="false"` y exportar desde `<Actions>` con `@ref` → `ExportarExcelAsync()` |
| Tabla calculada / de detalle | grilla CSS propia de la feature | Solo cuando no es un listado de registros (p. ej. cronograma con fila de totales) |
| Campo de una ficha (rótulo + valor) | `CdsKpi` sin ícono | Mismo componente que los indicadores; `ChildContent` para valores con enlace. Grilla de 2–4 columnas con `gap: var(--cds-space-stack) 24px`. Valor vacío muestra «—» |
| Panel lateral para crear/editar | `CdsDrawer` | 440px a la derecha, fondo `--cds-backdrop` difuminado, cierre con Esc o clic fuera; pie con Cancelar + acción principal |
| Campo de formulario | `CdsFormField` + `CdsTextField NativeForm` | Rótulo `label`; ayuda o error (`--cds-danger-text`) debajo. Validación en vivo: el error reemplaza a la ayuda y la acción de guardar queda deshabilitada |
| Interruptor | `CdsSwitch` | Con texto a la derecha (Activo/Inactivo). Un cambio de estado se guarda al instante |
| Tarjeta seleccionable | `CdsOptionCard` | Ícono, nombre, frase y radio; selección en color de marca |
| Aviso con Deshacer | `CdsUndoToast` | Abajo al centro, fondo `--cds-surface-inverse`, 6 s; tras cada cambio guardado |
| Códigos y valores técnicos | clase `cds-mono` | Misma fuente del sistema con cifras tabulares (no se usa una fuente monoespaciada) |
| Avatar | `Avatar` | `Tamano="32"` en tablas y menú; `Sutil="true"` cuando no hay persona concreta |

## 7. Patrones de la pantalla de referencia

### Tarjetas
Siempre `CdsCard`. Contenido interno en columna con `gap: var(--cds-space-card)`. Una tarjeta sin relleno (`CdsCardPadding.None`) se usa para tablas con encabezado propio.

### Formularios (panel «Condiciones»)
- Encabezado del panel: rótulo `label` + acción a la derecha (botón `Base`, outlined, `ExtraSmall`).
- Campo: rótulo `label` / `--cds-color-label` / `regular`, y debajo el control a `--cds-space-field`.
- Inputs y selects: `body`, `--cds-color-value`, borde `--cds-border-default`, radio `--cds-radius-control`, relleno `8px 11px`.
- Input numérico corto: alineado a la derecha, `strong`, `tabular-nums`. Si es moneda, lleva prefijo `$` en recuadro (`--cds-surface-muted`, 20×20, radio 6px).
- Slider + input: el input se actualiza en vivo al arrastrar (`oninput`) y la acción costosa (llamada a la API) se ejecuta solo al soltar (`onchange`).
- Pills: `small`, `regular`, `--cds-color-content`, radio `--cds-radius-chip`, relleno `5px 11px`. Seleccionada: fondo `--cds-action-dark`, texto blanco.
- Sección avanzada: separador `1px --cds-border-soft` con `--cds-space-stack` arriba y abajo, y título con ícono (`--cds-color-hint`).

### Resumen / KPI
- Rótulo: `label`, `regular`, `--cds-color-label`.
- Valor: `body`, `--cds-color-value`. Valor destacado: `title`, `strong`.
- Cifra principal: `hero`, `strong`, interlineado `tight`.

### Tablas
- Encabezado de tabla: `small`, `strong`, `--cds-color-content`, fondo `--cds-surface-subtle`, fijo arriba (`sticky`).
- Celdas: `small`, `regular`, `--cds-color-label`, `tabular-nums`. Encabezados siempre centrados (lo aplica `GridEstandar`). Celdas centradas (`TextAlign="TextAlign.Center"`), excepto nombres de personas, alineados a la izquierda (`TextAlign="TextAlign.Left"`), relleno `--cds-space-cell-*`, separador `1px --cds-surface-muted`, hover `--cds-surface-hover`.
- Solo **una** columna en negrita por fila (el total de la fila): `strong`, `--cds-color-content`. Sin color verde por fila.
- Nombres de personas con solo la inicial de cada palabra en mayúscula («Juan Pérez»), mediante `NameFormatter.ToDisplayName` (`Core/Utils/Format`), expuesto como propiedad del modelo para que ordenar, filtrar y exportar usen el mismo texto.
- El estado de cada registro va como texto plano, sin badge ni color. Un campo vacío muestra un texto explícito («Sin Agencia»), nunca una celda en blanco.
- El identificador del registro (p. ej. número de crédito `agencia-crédito`) es la primera columna, fija, y es un enlace `cds-link` a su detalle.
- Fila de totales: `body`, `bold`, `--cds-color-value`, borde superior `2px --cds-border-default`. **Siempre anclada al fondo** de la tabla (`margin-top: auto` dentro de un contenedor flex en columna + `sticky; bottom: 0`), aunque la página tenga pocas filas. Solo el total general va en `--cds-success-text-alt`.
- Encabezado de la tarjeta de tabla: título `title` + badge con el conteo; a la derecha, chip de estado y botones de exportación.
- Paginación: `RadzenPager` centrado, 10 filas por página, resumen «Página {0} de {1} ({2} elementos)».

### Chip de estado
- Misma caja que los botones pequeños vecinos: el contenedor usa `align-items: stretch`, radio `--rz-border-radius`, borde `--rz-border-width`.
- Ícono `check_circle` en éxito, `error` en descuadre; texto `small`, `strong`.
- En éxito, animación de entrada (aparece + pulso del borde + check con rebote), reiniciada con `@key` en cada resultado nuevo.

### Exportación
Siempre `CdsExportButton` (`Format="CdsExportFormat.Excel"` o `Pdf`), a la derecha y en ese orden: estado, Excel, PDF. Excel genera `.xlsx` real, no CSV.

### Estados de carga, error y vacío
`CdsPageState` para estados bloqueantes y `ICdsNotificationService` para resultados de acciones (ver `ARCHITECTURE.md`).

### Pantalla de detalle (referencia: Evaluación de crédito)
- Dos columnas: contenido (`minmax(0, 1fr)`) y panel de acción fijo de 360px (`position: sticky`); en pantallas angostas, una sola columna.
- Ruta de la barra superior con el listado de origen como enlace: `Path` con `new PagePathItem("Bandeja de verificación", "/…")`. No se usa un botón «← Volver».
- Ficha principal en `CdsCard`: `Avatar` 44px, nombre con `NameFormatter`, `CreditStatusBadge Variant="Subtle"` (fondo blanco, borde fino y punto de marca; sin colores de estado) y una línea de datos `small`/`label` separados por «·»; acción principal (`CdsButton` pequeño) a la derecha.
- Resumen numérico con `CdsKpi` con ícono (mismo patrón que la Bandeja).
- Secciones en `CdsCard Padding="None"` + `CdsCardHeader`, cuerpo con relleno `16px 20px 20px`.
- Acciones de una sección (p. ej. editar) en `<Actions>` del `CdsCardHeader` como botón de ícono: `CdsButton Icon="edit" Variant="Text" ButtonStyle="Base" Size="ExtraSmall"` con `title`/`aria-label`.
- Montos con signo: ingresos `+` en `--cds-success-text-alt`, gastos `−` en `--cds-danger-text`.
- Fotos (negocio, documento de identidad, etc.): nunca en línea; un botón `CdsButton` (`Variant="Outlined"`, `ButtonStyle="Base"`, `Size="Small"`, ícono y conteo en el texto, deshabilitado si no hay fotos) abre la galería modal (`DialogService.OpenAsync<FotoGaleria>`).
- Historial como línea de tiempo con marcador `--cds-primary` y aro `--cds-surface-selected`.
- Panel con gráficos (referencia: Panel de control): resumen con `CdsKpi` con ícono en una tarjeta con `CdsCardHeader` (badge con la hora de actualización y botón Actualizar); cada gráfico en `CdsCard Padding="None"` + `CdsCardHeader` (badge con el alcance), alto 280px. Una sola serie por gráfico, siempre en `--cds-primary`, sin leyenda; categorías con nombres largos o comparaciones de ranking en barras horizontales ordenadas de mayor a menor; nunca donas ni un color distinto por gráfico. Más de ~10 categorías (p. ej. agencias): `RankingList` (posición, nombre, barra proporcional y monto, con scroll) en lugar de gráfico. Series mensuales: mes corto y el año solo donde empieza («Sep 2025, Oct, …, Ene 2026»). Todo `RadzenChart` lleva `Culture` es-SV y, en ejes de montos, `Formatter` explícito es-SV (Radzen formatea las etiquetas de datos con la cultura invariante y mostraría «¤»). La aplicación fija es-SV como cultura global en `Program.cs`. Gráficos de columnas: título del eje vertical (`RadzenAxisTitle`, con la unidad), etiquetas de datos y polígono de frecuencias curvo (`RadzenLineSeries` con `Interpolation.Spline` sobre los mismos datos, mismo eje, `--cds-primary-dark` 2px con marcadores de 8px); con dos series va leyenda abajo. Ejes y grilla recesivos (`--rz-chart-*` a tokens).
- Decisión: opciones como tarjetas seleccionables (`CdsOptionCard`) con ícono en recuadro, nombre `body`/`strong`, frase de efecto `label` y radio a la derecha. Seleccionada en color de marca (borde `--cds-primary`, fondo `--cds-surface-selected`), nunca con el color del estado. Una opción no disponible muestra candado, borde punteado y el motivo en la frase.

### Mantenimiento maestro–detalle (referencia: Catálogos de códigos)
- Sin panel de indicadores arriba: la pantalla es directamente dos columnas de alto completo, lista maestra de 380px y detalle.
- Lista maestra: `CdsCardHeader` con conteo y botón «Nuevo»; buscador; ítems con insignia de código, nombre, clave técnica y conteo. Seleccionado: fondo `--cds-surface-selected`, borde interior `--cds-border-selected` e insignia `--cds-primary`.
- Detalle: `CdsCardHeader` con `Lead` (insignia de código), `Subtitle` («Clave X · N activos de M») y acciones (Excel + acción principal); `CdsTabs` con conteo y filtro de texto; tabla sin paginación con pie «Mostrando X de Y». La columna de nombre («Nombre del catálogo») alinea a la izquierda tanto el encabezado como las celdas.
- Crear/editar en `CdsDrawer`; eliminar con diálogo que ofrece «Desactivar» como alternativa segura; cada cambio muestra `CdsUndoToast`.

### Pantalla de acceso (referencia: Login)
- Pantalla dividida sin barra superior (`LoginLayout`): panel de marca oscuro (`AuthHero`, tokens `--cds-auth-*`) y formulario centrado (`LoginForm`) de máximo 400px. Bajo 960px se oculta el panel de marca.
- Título del formulario en `--cds-font-size-heading` (único uso fuera de la escala de pantallas internas); la cifra *hero* la usa el titular del panel de marca.
- Campos `AuthField`: 56px de alto, ícono a la izquierda, rótulo flotante y botón para mostrar/ocultar la contraseña. Botón principal (`CdsButton`) de 52px con flecha que avanza al pasar el cursor y spinner al enviar.
- Escena de otorgamiento (`DisbursementScene`, ejemplo ilustrativo): tarjeta de vidrio con el monto en contador tipo odómetro, etapas Solicitud → Evaluación → Aprobación → Desembolso que se completan en un ciclo de 9 s y monedas que saltan al desembolsar. Monedas que suben por el fondo del panel.
- Animaciones: luces del panel que se desplazan, entrada escalonada de textos y campos, la escena en bucle y sacudida de la tarjeta cuando el acceso falla. Todas se desactivan con `prefers-reduced-motion` (la escena queda en su estado final).
- El login se renderiza estático (SSR, ver `App.razor`): si fuera interactivo, el circuito reemplazaría el HTML prerenderizado y reiniciaría las animaciones. Lo que requiere comportamiento en el navegador (mostrar contraseña, «Verificando…», limpiar el error) vive en `wwwroot/js/auth.js`.

## 8. Menú lateral
- «Otorgamiento» es un encabezado fijo de sección, siempre desplegado.
- Íconos del menú con trazo fino: `--rz-icon-weight: var(--cds-icon-weight)` (300) y `--rz-icon-grade: var(--cds-icon-grade)` (-25), fijos aunque el ítem esté activo.
- El buscador («Buscar en el menú») va fijo arriba de Home dentro del menú lateral y filtra sus opciones; el usuario vive en la barra superior (sección 9).

## 9. Barra superior y títulos de página

La barra superior (`Components/Layout/AppTopBar`) es blanca, de alto `--cds-header-height` (4rem) y con borde inferior `--cds-border-neutral`. Tiene tres zonas:

1. **Marca**, del mismo ancho que el menú lateral (`--rz-sidebar-width`) y separada por un borde vertical: logo completo de CrediFlow (`images/crediflow-logo.svg`, 35px de alto, lienzo recortado al contenido), que muestra u oculta el menú. Con el menú oculto, la zona mide `--cds-content-inset` y muestra solo el símbolo (`images/crediflow-mark.svg`, 35px), para que el título siga alineado con el contenido.
2. **Atrás, ruta y título de la página**, con relleno izquierdo `--cds-content-inset` para quedar alineado con el borde de las tarjetas del contenido: botón «Atrás» (`<`, 32px con borde; en todas las páginas menos Home; vuelve en el historial o, si la página se abrió directo, al último nivel de la ruta con enlace o a Inicio), ícono de la página en `--cds-primary-accent-light` y una ruta estilo explorador de Windows `Inicio / Otorgamiento / Procesamiento de Datos / Bandeja de verificación`. Los niveles superiores van en `small`/`regular`/`label` separados por `/` (`hint`); «Inicio» es enlace a Home; el último nivel es el título `title`/`strong`/`value`. Debajo, una descripción `label`/`regular`/`label`.
3. **Acciones y usuario**: botones de ícono de 36px con borde (mensajes y notificaciones) y el usuario (avatar sutil de 32px, nombre `small`/`strong`, rol `label`, flecha). Al hacer clic se abre un menú con «Cerrar sesión».

Los íconos de la barra usan el trazo fino `--cds-icon-weight`.

El encabezado **solo** lo publica cada página con, como primer elemento:

```razor
<CdsPageHeader Title="Bandeja de verificación"
               Icon="inbox"
               Path="@(new PagePathItem[] { "Otorgamiento", "Procesamiento de Datos" })"
               Description="Créditos pendientes de verificación y aprobación." />
```

- `Icon`: el mismo ícono que la opción de la página en el menú lateral.
- `Description`: una sola frase corta que diga para qué sirve la página.
- `Path`: los grupos del menú lateral que contienen la página, de arriba hacia abajo (sin «Inicio» ni el título). Se omite en opciones de primer nivel.
- `ShowHome="false"`: solo en Home, para que la ruta no empiece con «Inicio».

`CdsPageHeader`:
- escribe el encabezado en la barra superior (`PageHeaderService`) y el título en la pestaña del navegador (`<PageTitle>`);
- se actualiza si el título cambia (títulos dinámicos, por ejemplo con parámetros de la URL);
- al salir de la página limpia el encabezado solo si todavía le pertenece, así la navegación no borra el de la página nueva.

Prohibido en páginas: `<h1>`–`<h3>` como título, `<PageTitle>` directo, e inyectar `PageHeaderService`.

## 10. Lista de comprobación visual

- [ ] La raíz de la página usa `cds-page` y no redeclara tipografía.
- [ ] Todos los tamaños, pesos, colores, espacios y radios usan tokens `--cds-*`.
- [ ] No hay atajos `font: … inherit`.
- [ ] Rótulos en `--cds-color-label` y peso `regular`; valores en `--cds-color-value`.
- [ ] Tabla: una sola columna en negrita, sin verde por fila, totales anclados al fondo.
- [ ] Los patrones de la sección 6 se usan mediante sus componentes compartidos, sin copias locales.
- [ ] Encabezado con `CdsPageHeader` (título, ícono del menú y descripción), sin `<h3>` ni `<PageTitle>` propios.
- [ ] Animaciones desactivadas con `prefers-reduced-motion`.
- [ ] Comparada lado a lado con el Simulador, no hay ninguna diferencia.
