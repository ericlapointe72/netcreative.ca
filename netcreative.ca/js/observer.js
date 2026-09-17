const io = document.querySelectorAll('.observer');
const options = { threshold: 0, rootMargin: "0px 0px -50px 0px" };

const appearOnScroll = new IntersectionObserver(function (entries, appearOnScroll) {
    entries.forEach(entry => {
        if (!entry.isIntersecting) {
            return;
        } else {
            entry.target.classList.add('appear');
            appearOnScroll.unobserve(entry.target);
        }
    })
},  options)

io.forEach(fader => {
    appearOnScroll.observe(fader);
})