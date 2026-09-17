(function () {
    var container = document.querySelector('.slideshow__container');
    if (!container) return;

    var card = container.closest('.card') || document;
    var slides = Array.prototype.slice.call(container.querySelectorAll('.slide'));
    var dots = Array.prototype.slice.call(card.querySelectorAll('.dot'));
    var prevBtn = container.querySelector('.prev');
    var nextBtn = container.querySelector('.next');
    var i18n = window.__i18n || {};

    var INTERVAL = 10000;
    var SWIPE_THRESHOLD = 40;
    var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    var index = 0;
    var timer = null;

    if (prevBtn && i18n.slidePrev) prevBtn.setAttribute('aria-label', i18n.slidePrev);
    if (nextBtn && i18n.slideNext) nextBtn.setAttribute('aria-label', i18n.slideNext);
    dots.forEach(function (dot, n) {
        if (i18n.slideDot) dot.setAttribute('aria-label', i18n.slideDot.replace('%d', n + 1));
    });

    function show(i) {
        index = (i + slides.length) % slides.length;
        slides.forEach(function (slide, n) {
            slide.classList.toggle('active', n === index);
            slide.setAttribute('aria-hidden', n === index ? 'false' : 'true');
        });
        dots.forEach(function (dot, n) {
            dot.classList.toggle('active', n === index);
            dot.setAttribute('aria-current', n === index ? 'true' : 'false');
        });
    }

    function stopAutoplay() {
        clearInterval(timer);
        timer = null;
    }

    function startAutoplay() {
        if (reduceMotion || document.hidden) return;
        stopAutoplay();
        timer = setInterval(function () { show(index + 1); }, INTERVAL);
    }

    function goTo(i) {
        show(i);
        startAutoplay();
    }

    if (prevBtn) prevBtn.addEventListener('click', function () { goTo(index - 1); });
    if (nextBtn) nextBtn.addEventListener('click', function () { goTo(index + 1); });
    dots.forEach(function (dot, n) { dot.addEventListener('click', function () { goTo(n); }); });

    container.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowRight') goTo(index + 1);
        if (e.key === 'ArrowLeft') goTo(index - 1);
    });

    container.addEventListener('mouseenter', stopAutoplay);
    container.addEventListener('mouseleave', startAutoplay);
    container.addEventListener('focusin', stopAutoplay);
    container.addEventListener('focusout', startAutoplay);
    document.addEventListener('visibilitychange', function () {
        if (document.hidden) stopAutoplay(); else startAutoplay();
    });

    // the slides are stacked (crossfade), not a scrollable strip, so touch swipe
    // still needs to be handled manually here (unlike the photo carousel)
    var touchStartX = null;
    var touchStartY = null;

    container.addEventListener('touchstart', function (e) {
        touchStartX = e.touches[0].clientX;
        touchStartY = e.touches[0].clientY;
    }, { passive: true });

    container.addEventListener('touchend', function (e) {
        if (touchStartX === null) return;
        var dx = touchStartX - e.changedTouches[0].clientX;
        var dy = touchStartY - e.changedTouches[0].clientY;
        touchStartX = null;
        touchStartY = null;
        if (Math.abs(dx) < SWIPE_THRESHOLD || Math.abs(dx) < Math.abs(dy)) return;
        goTo(index + (dx > 0 ? 1 : -1));
    }, { passive: true });

    show(0);
    startAutoplay();
})();
