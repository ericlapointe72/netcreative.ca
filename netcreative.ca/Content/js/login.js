(function () {
    var card = document.querySelector('.login__card');
    if (!card) {
        return;
    }

    var userField = card.querySelector('.tb__user');
    var passwordField = card.querySelector('.tb__pass');
    var submitButton = card.querySelector('.login__submit');

    function submit() {
        submitButton.disabled = true;

        var body = 'user=' + encodeURIComponent(userField.value) + '&password=' + encodeURIComponent(passwordField.value);

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

                if (!result.success) {
                    showToast(result.message, 'error');
                }
            })
            .catch(function () {
                submitButton.disabled = false;
                showToast(card.getAttribute('data-network-error'), 'error');
            });
    }

    submitButton.addEventListener('click', submit);

    [userField, passwordField].forEach(function (field) {
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
