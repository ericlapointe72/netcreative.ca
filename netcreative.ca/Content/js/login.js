(function () {
    var card = document.querySelector('.login__card');
    if (!card) {
        return;
    }

    var userField = card.querySelector('.tb__user');
    var passwordField = card.querySelector('.tb__pass');
    var codeField = card.querySelector('.tb__code');
    var submitButton = card.querySelector('.login__submit');
    var userBlock = card.querySelector('.card__user');
    var passwordBlock = card.querySelector('.card__password');
    var codeBlock = card.querySelector('.card__code');
    var codeHint = card.querySelector('.login__hint');

    var passwordLabel = submitButton.textContent;
    var codeLabel = submitButton.getAttribute('data-label-code') || passwordLabel;

    // when the server has accepted the password but wants a code too, this holds the token of the half-finished sign-in
    var pending = null;

    function showCodeStep(token) {
        pending = token;
        userBlock.hidden = true;
        passwordBlock.hidden = true;
        codeBlock.hidden = false;
        codeHint.hidden = false;
        submitButton.textContent = codeLabel;
        codeField.value = '';
        codeField.focus();
    }

    function showPasswordStep() {
        pending = null;
        userBlock.hidden = false;
        passwordBlock.hidden = false;
        codeBlock.hidden = true;
        codeHint.hidden = true;
        submitButton.textContent = passwordLabel;
        passwordField.value = '';
        codeField.value = '';
        passwordField.focus();
    }

    function submit() {
        submitButton.disabled = true;

        var body = pending
            ? 'pending=' + encodeURIComponent(pending) + '&code=' + encodeURIComponent(codeField.value)
            : 'user=' + encodeURIComponent(userField.value) + '&password=' + encodeURIComponent(passwordField.value);

        fetch('LoginHandler.ashx', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            credentials: 'same-origin',
            body: body
        })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.redirect) {
                    window.location.href = result.redirect;
                    return;
                }

                submitButton.disabled = false;

                if (result.needCode && result.pending) {
                    var wrongCode = !!pending;
                    showCodeStep(result.pending);

                    if (wrongCode && result.message) {
                        showToast(result.message, 'error');
                    }

                    return;
                }

                if (result.restart) {
                    showPasswordStep();
                }

                if (!result.success && result.message) {
                    showToast(result.message, 'error');
                }
            })
            .catch(function () {
                submitButton.disabled = false;
                showToast(card.getAttribute('data-network-error'), 'error');
            });
    }

    submitButton.addEventListener('click', submit);

    [userField, passwordField, codeField].forEach(function (field) {
        field.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                submit();
            }
        });
    });

    // form1 is shared by the whole page (master page subscribe box, cookie panel,
    // etc.) and contains a real submit button elsewhere, so guard against any native
    // submission ever escaping from within the login card (e.g. Enter on autofill)
    var form = card.closest('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (card.contains(e.target)) {
                e.preventDefault();
            }
        });
    }
})();
