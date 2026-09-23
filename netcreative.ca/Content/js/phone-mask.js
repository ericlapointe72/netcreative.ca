(function () {
    var phone = document.querySelector('.tb__phone');
    if (!phone) return;

    function format(digits) {
        digits = digits.slice(0, 10);

        if (digits.length === 0) return '';
        if (digits.length < 4) return '(' + digits;
        if (digits.length < 7) return '(' + digits.slice(0, 3) + ') ' + digits.slice(3);
        return '(' + digits.slice(0, 3) + ') ' + digits.slice(3, 6) + '-' + digits.slice(6);
    }

    phone.addEventListener('input', function () {
        var digits = phone.value.replace(/\D/g, '');
        phone.value = format(digits);
        // masked inputs are typed left-to-right only, so pinning the caret to the end
        // avoids it jumping around as punctuation gets inserted mid-string
        phone.selectionStart = phone.selectionEnd = phone.value.length;
    });
})();
