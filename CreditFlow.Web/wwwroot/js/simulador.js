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
window.cdsFitViewport = window.simuladorFitViewport;

// Botón «Atrás» de la barra superior: vuelve en el historial si la página anterior es de esta aplicación;
// si se abrió directo (sin historial propio), navega al nivel superior indicado.
window.cdsGoBack = function (fallbackUrl) {
    var mismoOrigen = document.referrer && new URL(document.referrer).origin === window.location.origin;
    if (window.history.length > 1 && (mismoOrigen || window.history.state)) {
        window.history.back();
    } else {
        window.location.href = fallbackUrl || '/';
    }
};
