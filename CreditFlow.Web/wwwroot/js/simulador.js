// Ajusta el alto del simulador de crédito para que ocupe exactamente el
// espacio visible restante del viewport (debajo de la barra superior),
// sin depender de valores fijos en CSS que puedan desajustarse por el
// tema de Radzen. Así la página nunca produce scroll del documento.
window.simuladorFitViewport = function (el) {
    if (!el) return;

    function ajustar() {
        const top = el.getBoundingClientRect().top;
        const alto = window.innerHeight - top;
        el.style.height = Math.max(alto, 0) + 'px';
    }

    ajustar();

    if (!el.dataset.simFitBound) {
        window.addEventListener('resize', ajustar);
        el.dataset.simFitBound = '1';
    }
};
