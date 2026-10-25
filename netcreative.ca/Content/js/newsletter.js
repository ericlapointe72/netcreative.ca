// Admin > Newsletter: the preview of the email follows what is typed (same parts, same order as Newsletter.BodyHtml on the server),
// the subject has a character counter, and the send button says "sending" once the form is on its way (a mailing to every subscriber
// takes a while, and a second click would send it twice).
(function () {
    'use strict';

    function field(name) {
        return document.querySelector('[id$="' + name + '"]');
    }

    var subject = field('TextBox_Newsletter_Object');
    var intro = field('TextBox_Newsletter_Intro');
    var body = field('TextBox_Newsletter_Body');
    var greeting = field('TextBox_Newsletter_Greeting');
    var testMode = field('RadioButton_Newsletter_Test');
    var sendButton = field('Button_Newsletter_Send');
    var preview = document.getElementById('news-preview');
    var count = document.getElementById('news-count');

    if (!subject || !intro || !body || !greeting || !preview) {
        return;
    }

    var emptyText = preview.getAttribute('data-empty');

    // a part that is empty leaves no paragraph in the email (intro, closing), or shows a grey "(empty)" (subject, message)
    function show(id, text, required) {
        var node = document.getElementById(id);
        var value = text.trim();

        node.textContent = value || (required ? emptyText : '');
        node.classList.toggle('news-empty', !value && required);
        node.style.display = value || required ? '' : 'none';
    }

    function render() {
        var prefix = testMode && testMode.checked ? '[Test] ' : '';
        var subjectText = subject.value.trim();

        show('np-subject', subjectText ? prefix + subjectText : '', true);
        show('np-intro', intro.value, false);
        show('np-body', body.value, true);
        show('np-greeting', greeting.value, false);

        if (count) {
            var length = subjectText.length;
            count.textContent = count.getAttribute('data-template').replace('{0}', length);
            count.classList.toggle('news-count--over', length > parseInt(count.getAttribute('data-ideal'), 10));
        }
    }

    [subject, intro, body, greeting].forEach(function (box) {
        box.addEventListener('input', render);
    });

    if (sendButton) {
        sendButton.addEventListener('click', function (event) {
            // a cancelled confirmation (sending to everybody) leaves the button as it is
            if (event.defaultPrevented) {
                return;
            }

            var text = sendButton.parentNode.getAttribute('data-sending');

            // later than the click itself: a button disabled during its own click would not be part of the form that is sent
            setTimeout(function () {
                sendButton.disabled = true;
                if (text) {
                    sendButton.value = text;
                }
            }, 50);
        });
    }

    // the Back button can bring the page back from memory with the button still disabled
    window.addEventListener('pageshow', function (event) {
        if (event.persisted && sendButton) {
            sendButton.disabled = false;
        }
    });

    render();
})();
