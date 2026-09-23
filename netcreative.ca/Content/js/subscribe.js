(function () {
    var box = document.querySelector('.foot__subscribe');
    if (!box) {
        return;
    }

    var emailField = box.querySelector('.tb__subscribe');
    var submitButton = box.querySelector('.subscribe__submit');

    function submit() {
        submitButton.disabled = true;

        var body = 'email=' + encodeURIComponent(emailField.value);

        fetch('SubscribeHandler.ashx', {
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
                showToast(box.getAttribute('data-network-error'), 'error');
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
    // elsewhere (e.g. the contact form), so guard against any native submission
    // ever escaping from within the subscribe box (e.g. Enter on autofill)
    var form = box.closest('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (box.contains(e.target)) {
                e.preventDefault();
            }
        });
    }
})();
