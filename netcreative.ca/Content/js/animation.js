var animCtn = document.querySelector(".animation__container");

var animationTimer = setInterval(function () {
    animCtn.classList.add("close");
}, 5000)