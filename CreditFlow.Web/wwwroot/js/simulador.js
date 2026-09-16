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
