// Questions fréquentes (service.aspx): the answers fold and unfold with a height animation, one open at a time.
// Without this script every answer stays visible (the CSS only folds them once the "faq--js" class is there).
(function () {
    'use strict';

    var faq = document.querySelector('.faq');
    if (!faq) return;

    var items = faq.querySelectorAll('.faq__item');
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var DURATION = 300; // the same as the transition of the CSS (a little more)

    function parts(item) {
        return { button: item.querySelector('.faq__question'), panel: item.querySelector('.faq__panel') };
    }

    function isOpen(button) {
        return button.getAttribute('aria-expanded') === 'true';
    }

    function setOpen(button, panel, open) {
        button.setAttribute('aria-expanded', open ? 'true' : 'false');
        panel.dataset.expanded = open ? 'true' : 'false';
        clearTimeout(panel._faqTimer);

        if (reduceMotion) {
            panel.style.height = open ? 'auto' : '0px';
            return;
        }

        if (open) {
            // from the height it has now (0, or in the middle of closing) to its full height, then "auto" so that it follows the text
            // when the window is resized
            panel.style.height = panel.getBoundingClientRect().height + 'px';
            panel.offsetHeight; // the browser takes the starting height into account
            panel.style.height = panel.scrollHeight + 'px';
            panel._faqTimer = setTimeout(function () { panel.style.height = 'auto'; }, DURATION);
        } else {
            // "auto" cannot be animated: its height in pixels first
            panel.style.height = panel.getBoundingClientRect().height + 'px';
            panel.offsetHeight;
            panel.style.height = '0px';
        }
    }

    function closeOthers(except) {
        items.forEach(function (item) {
            var p = parts(item);
            if (item !== except && p.button && p.panel && isOpen(p.button)) {
                setOpen(p.button, p.panel, false);
            }
        });
    }

    faq.classList.add('faq--js');

    items.forEach(function (item) {
        var p = parts(item);
        if (!p.button || !p.panel) return;

        p.button.setAttribute('aria-expanded', 'false');
        p.panel.dataset.expanded = 'false';
        p.panel.style.height = '0px';

        p.button.addEventListener('click', function () {
            var open = isOpen(p.button);

            if (!open) {
                closeOthers(item);
            }

            setOpen(p.button, p.panel, !open);
        });
    });
})();
