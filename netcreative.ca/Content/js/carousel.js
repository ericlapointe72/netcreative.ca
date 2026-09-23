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

    var realCount = images.length;

    // clone the first/last slide to the opposite end so wrapping can glide straight into a
    // twin image instead of either sweeping back across every photo or cutting instantly —
    // once the glide lands on the clone we snap to the real slide behind it unseen (below)
    var firstClone = images[0].cloneNode(true);
    var lastClone = images[realCount - 1].cloneNode(true);
    firstClone.setAttribute('aria-hidden', 'true');
    lastClone.setAttribute('aria-hidden', 'true');
    track.appendChild(firstClone);
    track.insertBefore(lastClone, images[0]);

    // track layout is now [lastClone, real 0..n-1, firstClone]; a slide's position in that
    // strip is always its real index + 1
    var index = 0;
    var timer = null;
    var scrollTimer = null;

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

    function scrollToPos(pos, smooth) {
        // scrollTo() on the track itself only scrolls its own horizontal strip;
        // scrollIntoView() would also drag the whole page down to reveal it (e.g. during autoplay).
        // 'auto' defers to the CSS scroll-behavior (smooth, see public.css) rather than meaning
        // "instant", so a real instant jump needs the explicit 'instant' value.
        track.scrollTo({ left: pos * track.clientWidth, behavior: smooth === false ? 'instant' : 'smooth' });
    }

    function goTo(n, smooth) {
        index = (n + realCount) % realCount;
        // stepping past either end targets the clone one slide further out, so the glide
        // continues past the real edge instead of stopping or reversing; landing there gets
        // corrected to the real slide once the scroll settles (see the scroll listener)
        var pos = n < 0 ? 0 : (n >= realCount ? realCount + 1 : n + 1);
        scrollToPos(pos, smooth);
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

    // native scroll/touch handles dragging the strip directly; once any scroll (ours or the
    // user's) settles, land on the nearest slide and, if that's one of the clones, snap
    // straight to the real slide it stands in for — invisible since the clone is identical
    track.addEventListener('scroll', function () {
        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(function () {
            var pos = Math.round(track.scrollLeft / track.clientWidth);
            if (pos <= 0) {
                index = realCount - 1;
                scrollToPos(realCount, false);
            } else if (pos >= realCount + 1) {
                index = 0;
                scrollToPos(1, false);
            } else {
                index = pos - 1;
            }
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

    goTo(0, false);
    startAutoplay();
})();
