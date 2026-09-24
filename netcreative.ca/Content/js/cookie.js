const acceptButton = document.querySelector('#cookie__accept');
const declineButton = document.querySelector('#cookie__decline');
const panel = document.querySelector('.cookie__panel');
let date = new Date(Date.now() + 86400000 * 7);

function setConsent(value) {
    panel.classList.add('cookie__panel-hide');
    document.cookie = 'netcreative_cookie_consent=' + value + '; expires=' + date;
}

if (acceptButton) acceptButton.addEventListener('click', function () { setConsent('accepted'); });
if (declineButton) declineButton.addEventListener('click', function () { setConsent('declined'); });
