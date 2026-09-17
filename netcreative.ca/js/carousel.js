(function () {
    var track = document.querySelector('.carousel__slide');
    if (!track) return;

    var images = Array.prototype.slice.call(track.querySelectorAll('.carousel__image'));
    var prevBtn = document.querySelector('#prev__btn');
    var nextBtn = document.querySelector('#next__btn');
    var navBtns = Array.prototype.slice.call(document.querySelectorAll('.nav__btn'));
    var i18n = window.__i18n || {};

    var INTERVAL = 5000;
    var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    var index = 0;
    var timer = null;
    var scrollTimer = null;
    var isSyncingScroll = false;

    if (prevBtn && i18n.carouselPrev) prevBtn.setAttribute('aria-label', i18n.carouselPrev);
    if (nextBtn && i18n.carouselNext) nextBtn.setAttribute('aria-label', i18n.carouselNext);
    navBtns.forEach(function (btn, n) {
        if (i18n.carouselDot) btn.setAttribute('aria-label', i18n.carouselDot.replace('%d', n + 1));
    });

    function updateControls() {
        navBtns.forEach(function (btn, n) {
            btn.classList.toggle('active', n === index);
            btn.setAttribute('aria-current', n === index ? 'true' : 'false');
        });
    }

    function goTo(n, smooth) {
        var wrapped = (n + images.length) % images.length;
        // wrapping past either end snaps instantly instead of sweeping visibly across every photo
        var isWrap = wrapped !== n;
        index = wrapped;
        isSyncingScroll = true;
        // scrollTo() on the track itself only scrolls its own horizontal strip;
        // scrollIntoView() would also drag the whole page down to reveal it (e.g. during autoplay)
        track.scrollTo({ left: index * track.clientWidth, behavior: (smooth === false || isWrap) ? 'auto' : 'smooth' });
        updateControls();
    }

    function stopAutoplay() {
        clearInterval(timer);
        timer = null;
    }

    function startAutoplay() {
        if (reduceMotion || document.hidden) return;
        stopAutoplay();
        timer = setInterval(function () { goTo(index + 1); }, INTERVAL);
    }

    if (prevBtn) prevBtn.addEventListener('click', function () { goTo(index - 1); startAutoplay(); });
    if (nextBtn) nextBtn.addEventListener('click', function () { goTo(index + 1); startAutoplay(); });
    navBtns.forEach(function (btn, n) { btn.addEventListener('click', function () { goTo(n); startAutoplay(); }); });

    track.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowRight') { goTo(index + 1); startAutoplay(); }
        if (e.key === 'ArrowLeft') { goTo(index - 1); startAutoplay(); }
    });

    // native scroll/touch handles dragging the strip directly; just keep the dots and
    // arrows in sync with wherever the user ends up
    track.addEventListener('scroll', function () {
        if (isSyncingScroll) { isSyncingScroll = false; return; }
        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(function () {
            var n = Math.round(track.scrollLeft / track.clientWidth);
            index = Math.max(0, Math.min(images.length - 1, n));
            updateControls();
        }, 100);
    }, { passive: true });

    window.addEventListener('resize', function () { goTo(index, false); });

    track.addEventListener('mouseenter', stopAutoplay);
    track.addEventListener('mouseleave', startAutoplay);
    track.addEventListener('focusin', stopAutoplay);
    track.addEventListener('focusout', startAutoplay);
    document.addEventListener('visibilitychange', function () {
        if (document.hidden) stopAutoplay(); else startAutoplay();
    });

    updateControls();
    startAutoplay();
})();
