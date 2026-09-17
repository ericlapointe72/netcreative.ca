const button = document.querySelector('.cookie__button');
const panel = document.querySelector('.cookie__panel');
let date = new  Date(Date.now() + 86400000 * 7);

button.addEventListener('click', function () {
    panel.classList.add('cookie__panel-hide');
    document.cookie = 'netcreative_cookie_accepted=true; expires=' + date;
});
