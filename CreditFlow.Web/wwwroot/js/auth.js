// Pantalla de acceso (renderizado estático, sin circuito de Blazor): mostrar/ocultar contraseña,
// estado «Verificando…» al enviar y limpiar el error al volver a escribir.
(function () {
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

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form.matches || !form.matches('[data-auth-form]')) return;

        var boton = form.querySelector('.login-submit');
        if (!boton) return;
        boton.classList.add('login-submit--loading');
        boton.setAttribute('aria-busy', 'true');
        var texto = boton.querySelector('.login-submit__text');
        if (texto) texto.textContent = 'Verificando…';
    });
})();
