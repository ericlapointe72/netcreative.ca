const acceptButton = document.querySelector('#cookie__accept');
const declineButton = document.querySelector('#cookie__decline');
const panel = document.querySelector('.cookie__panel');

// the answer is kept for six months, for the whole site, only sent with this site's own requests, and over HTTPS only
function setConsent(value) {
    panel.classList.add('cookie__panel-hide');

    const expires = new Date(Date.now() + 86400000 * 180).toUTCString();
    const secure = location.protocol === 'https:' ? '; Secure' : '';

    document.cookie = 'netcreative_cookie_consent=' + value + '; expires=' + expires + '; path=/; SameSite=Lax' + secure;
}

if (acceptButton) acceptButton.addEventListener('click', function () { setConsent('accepted'); });
if (declineButton) declineButton.addEventListener('click', function () { setConsent('declined'); });
