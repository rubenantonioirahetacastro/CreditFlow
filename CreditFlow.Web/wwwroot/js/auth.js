// Pantalla de acceso (renderizado estático, sin circuito de Blazor): mostrar/ocultar contraseña, aviso de
// Bloq Mayús, estado «Verificando…» al enviar, limpiar el error al volver a escribir, y la luz que sigue al
// cursor con la inclinación 3D de la escena en el panel de marca.
(function () {
    var sinMovimiento = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    document.addEventListener('click', function (e) {
        var boton = e.target.closest('[data-auth-toggle]');
        if (!boton) return;

        var input = document.getElementById(boton.getAttribute('data-auth-toggle'));
        if (!input) return;

        var mostrar = input.type === 'password';
        input.type = mostrar ? 'text' : 'password';
        boton.setAttribute('aria-pressed', mostrar ? 'true' : 'false');
        boton.setAttribute('aria-label', mostrar ? 'Ocultar contraseña' : 'Mostrar contraseña');
        boton.title = boton.getAttribute('aria-label');

        var icono = boton.querySelector('.rzi');
        if (icono) icono.textContent = mostrar ? 'visibility_off' : 'visibility';
        input.focus();
    });

    document.addEventListener('input', function (e) {
        if (!e.target.classList || !e.target.classList.contains('auth-field__input')) return;

        var form = e.target.closest('form');
        if (!form) return;
        form.querySelectorAll('.auth-field--invalid').forEach(function (campo) {
            campo.classList.remove('auth-field--invalid');
            var input = campo.querySelector('input');
            if (input) input.setAttribute('aria-invalid', 'false');
        });
    });

    // Bloq Mayús: el aviso aparece mientras está activado y el foco está en la contraseña.
    function revisarMayusculas(e) {
        var input = e.target;
        if (!input.matches || !input.matches('[data-auth-caps]') || !e.getModifierState) return;

        var campo = input.closest('.auth-field');
        if (campo) campo.classList.toggle('auth-field--caps', e.getModifierState('CapsLock'));
    }

    document.addEventListener('keydown', revisarMayusculas);
    document.addEventListener('keyup', revisarMayusculas);
    document.addEventListener('focusout', function (e) {
        if (!e.target.matches || !e.target.matches('[data-auth-caps]')) return;
        var campo = e.target.closest('.auth-field');
        if (campo) campo.classList.remove('auth-field--caps');
    });

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form.matches || !form.matches('[data-auth-form]')) return;

        var tarjeta = form.closest('.login-card');
        if (tarjeta) tarjeta.classList.add('login-card--sending');

        var boton = form.querySelector('.login-submit');
        if (!boton) return;
        boton.classList.add('login-submit--loading');
        boton.setAttribute('aria-busy', 'true');
        var texto = boton.querySelector('.login-submit__text');
        if (texto) texto.textContent = 'Verificando…';
    });

    // Luz que sigue al cursor e inclinación 3D de la escena (máximo 6°), suavizadas con requestAnimationFrame.
    if (sinMovimiento) return;

    var pendiente = null;
    var programado = false;

    document.addEventListener('pointermove', function (e) {
        var panel = e.target.closest && e.target.closest('[data-auth-spotlight]');
        if (!panel) return;

        pendiente = { panel: panel, x: e.clientX, y: e.clientY };
        if (programado) return;
        programado = true;
        window.requestAnimationFrame(function () {
            programado = false;
            var p = pendiente;
            if (!p) return;
            var r = p.panel.getBoundingClientRect();
            var px = (p.x - r.left) / r.width;
            var py = (p.y - r.top) / r.height;

            p.panel.style.setProperty('--mx', (px * 100).toFixed(1) + '%');
            p.panel.style.setProperty('--my', (py * 100).toFixed(1) + '%');
            p.panel.style.setProperty('--spot', '1');

            var tilt = p.panel.querySelector('[data-auth-tilt]');
            if (tilt) {
                tilt.style.setProperty('--ry', ((px - 0.5) * 12).toFixed(2) + 'deg');
                tilt.style.setProperty('--rx', ((0.5 - py) * 12).toFixed(2) + 'deg');
            }
        });
    });

    document.addEventListener('pointerout', function (e) {
        var panel = e.target.closest && e.target.closest('[data-auth-spotlight]');
        if (!panel || (e.relatedTarget && panel.contains(e.relatedTarget))) return;

        pendiente = null;
        panel.style.setProperty('--spot', '0');
        var tilt = panel.querySelector('[data-auth-tilt]');
        if (tilt) {
            tilt.style.setProperty('--rx', '0deg');
            tilt.style.setProperty('--ry', '0deg');
        }
    });
})();
