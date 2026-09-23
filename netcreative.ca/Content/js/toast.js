(function () {
    var container = null;

    function getContainer() {
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            container.setAttribute('role', 'status');
            container.setAttribute('aria-live', 'polite');
            document.body.appendChild(container);
        }
        return container;
    }

    window.showToast = function (message, type, duration) {
        type = type === 'success' ? 'success' : 'error';
        duration = duration || 6000;

        var toast = document.createElement('div');
        toast.className = 'toast toast--' + type;

        var icon = document.createElement('i');
        icon.className = 'toast__icon fas ' + (type === 'success' ? 'fa-circle-check' : 'fa-circle-exclamation');
        icon.setAttribute('aria-hidden', 'true');

        var text = document.createElement('span');
        text.className = 'toast__text';
        text.textContent = message;

        var close = document.createElement('button');
        close.type = 'button';
        close.className = 'toast__close';
        close.setAttribute('aria-label', 'Fermer');
        close.innerHTML = '&times;';

        toast.appendChild(icon);
        toast.appendChild(text);
        toast.appendChild(close);
        getContainer().appendChild(toast);

        // two rAFs so the browser commits the initial (off-screen) state before the
        // "show" class is added, otherwise the transition can get skipped entirely
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                toast.classList.add('toast--show');
            });
        });

        var timer = setTimeout(dismiss, duration);

        function dismiss() {
            clearTimeout(timer);
            toast.classList.remove('toast--show');
            toast.addEventListener('transitionend', function () {
                toast.remove();
            }, { once: true });
        }

        close.addEventListener('click', dismiss);
    };
})();
