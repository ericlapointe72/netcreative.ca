window.onscroll = () => {
    const Ypos1 = window.pageYOffset;
    const s = document.querySelector(".button__top");

    if (Ypos1 > 600) {
        s.classList.add("button__top-show");
    }
    else {
        s.classList.remove("button__top-show");
    }
}