// The map of the footer. Google's map is only loaded when the visitor asks for it (the button), or has accepted the cookies notice:
// loading it sends the visitor's address to Google, so it does not happen on its own for somebody who declined or did not answer.
(function () {
    'use strict';

    var box = document.getElementById('foot-map');
    if (!box) {
        return;
    }

    var button = document.getElementById('foot-map-show');
    var loaded = false;

    function load() {
        if (loaded) {
            return;
        }

        loaded = true;

        var frame = document.createElement('iframe');
        frame.className = 'foot__map-img';
        frame.src = box.getAttribute('data-src');
        frame.title = box.getAttribute('data-title') || '';
        frame.loading = 'lazy';
        frame.referrerPolicy = 'no-referrer-when-downgrade';
        frame.setAttribute('allowfullscreen', '');
        frame.style.border = '0';

        box.parentNode.replaceChild(frame, box);
    }

    if (button) {
        button.addEventListener('click', load);
    }

    // somebody who accepted the notice (now, or on an earlier visit) sees the map without asking
    function accepted() {
        return document.cookie.split(';').some(function (part) {
            return part.trim() === 'netcreative_cookie_consent=accepted';
        });
    }

    if (accepted()) {
        load();
    }

    var acceptButton = document.getElementById('cookie__accept');
    if (acceptButton) {
        acceptButton.addEventListener('click', load);
    }
})();
