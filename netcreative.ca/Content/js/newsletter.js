// Admin > Newsletter: the preview of the email follows what is typed, the subject has a character counter, and the send button says
// "sending" once the form is on its way (a mailing to every subscriber takes a while, and a second click would send it twice).
// The preview is NOT drawn here: the message and the closing line are sent to NewsletterPreview.ashx, which answers with the HTML that
// the e-mail will hold (the same code makes both), so what is previewed is what is sent.
(function () {
    'use strict';

    function field(name) {
        return document.querySelector('[id$="' + name + '"]');
    }

    var subject = field('TextBox_Newsletter_Object');
    var body = field('TextBox_Newsletter_Body');
    var greeting = field('TextBox_Newsletter_Greeting');
    var testMode = field('RadioButton_Newsletter_Test');
    var sendButton = field('Button_Newsletter_Send');
    var preview = document.getElementById('news-preview');
    var content = document.getElementById('np-content');
    var count = document.getElementById('news-count');
    var script = document.currentScript || document.querySelector('script[data-preview]');

    if (!subject || !body || !greeting || !preview || !content) {
        return;
    }

    var emptyText = preview.getAttribute('data-empty');
    var previewUrl = script ? script.getAttribute('data-preview') : null;
    var timer = null;
    var requestNumber = 0;

    // the subject is plain text; an empty one shows a grey "(empty)"
    function showSubject() {
        var prefix = testMode && testMode.checked ? '[Test] ' : '';
        var subjectText = subject.value.trim();
        var node = document.getElementById('np-subject');

        node.textContent = subjectText ? prefix + subjectText : emptyText;
        node.classList.toggle('news-empty', !subjectText);

        if (count) {
            var length = subjectText.length;
            count.textContent = count.getAttribute('data-template').replace('{0}', length);
            count.classList.toggle('news-count--over', length > parseInt(count.getAttribute('data-ideal'), 10));
        }
    }

    // the message, as the server writes it; an answer that comes back after a newer one was asked for is ignored
    function showContent() {
        var number = ++requestNumber;

        if (!body.value.trim()) {
            content.innerHTML = '<p class="news-empty">' + emptyText + '</p>';
            return;
        }

        if (!previewUrl) {
            return;
        }

        var data = new URLSearchParams();
        data.append('message', body.value);
        data.append('closing', greeting.value);

        fetch(previewUrl, {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
            body: data.toString()
        }).then(function (response) {
            return response.ok ? response.text() : null;
        }).then(function (html) {
            if (html !== null && number === requestNumber) {
                content.innerHTML = html;
            }
        }).catch(function () {
            // the preview stays as it was: the page works without it
        });
    }

    function later() {
        clearTimeout(timer);
        timer = setTimeout(showContent, 250);
    }

    subject.addEventListener('input', showSubject);
    body.addEventListener('input', later);
    greeting.addEventListener('input', later);

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

    showSubject();
    showContent();
})();
