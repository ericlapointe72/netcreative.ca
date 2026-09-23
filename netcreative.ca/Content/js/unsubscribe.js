(function () {
    var card = document.querySelector('.unsubscribe__container .card');
    if (!card) {
        return;
    }

    var emailField = card.querySelector('.tb__unsubscribe');
    var submitButton = card.querySelector('.unsubscribe__submit');

    function submit() {
        submitButton.disabled = true;

        var body = 'email=' + encodeURIComponent(emailField.value);

        fetch('UnsubscribeHandler.ashx', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            credentials: 'same-origin',
            body: body
        })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                submitButton.disabled = false;
                showToast(result.message, result.success ? 'success' : 'error');

                if (result.success) {
                    emailField.value = '';
                }
            })
            .catch(function () {
                submitButton.disabled = false;
                showToast(card.getAttribute('data-network-error'), 'error');
            });
    }

    submitButton.addEventListener('click', submit);

    emailField.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            submit();
        }
    });

    // form1 is shared by the whole page and contains other real submit buttons
    // elsewhere (e.g. the newsletter subscribe box), so guard against any native
    // submission ever escaping from within this card (e.g. Enter on autofill)
    var form = card.closest('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (card.contains(e.target)) {
                e.preventDefault();
            }
        });
    }
})();
