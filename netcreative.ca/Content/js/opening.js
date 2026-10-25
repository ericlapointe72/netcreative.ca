// Admin > Opening hours: a closed day greys out its times, the preview under the table follows what is typed (same wording as the
// footer of the site: OpeningHours.Line on the server), and one button copies Monday's hours to Tuesday-Friday.
(function () {
    'use strict';

    var list = document.getElementById('opening-days');
    var preview = document.getElementById('opening-preview');
    var copyButton = document.getElementById('opening-copy');

    if (!list || !preview) {
        return;
    }

    var english = preview.getAttribute('data-english') === '1';
    var closedText = preview.getAttribute('data-closed');
    var hourSeparator = preview.getAttribute('data-hour-separator');
    var timeSeparator = preview.getAttribute('data-time-separator');
    var amText = preview.getAttribute('data-am');
    var pmText = preview.getAttribute('data-pm');
    var invalidText = preview.getAttribute('data-invalid');

    var rows = Array.prototype.slice.call(list.querySelectorAll('.opening-day'));

    // "08:30", "8:30:00", "8h30", "8" -> minutes after midnight; null when it is not a time (same rule as the server)
    function minutesOf(text) {
        var match = /^(\d{1,2})\s*(?:[:hH]\s*(\d{1,2})?)?(?::\d{1,2})?$/.exec((text || '').trim());
        if (!match) {
            return null;
        }

        var hour = parseInt(match[1], 10);
        var minute = match[2] ? parseInt(match[2], 10) : 0;

        return hour > 23 || minute > 59 ? null : hour * 60 + minute;
    }

    function pad(n) {
        return (n < 10 ? '0' : '') + n;
    }

    // 8h30 / 16h45 in French, 8:30am / 4:45pm in English
    function timeText(minutes) {
        var hour = Math.floor(minutes / 60);
        var sign = '';

        if (english) {
            sign = hour < 12 ? amText : pmText;
            hour = hour % 12 === 0 ? 12 : hour % 12;
        }

        return hour + timeSeparator + pad(minutes % 60) + sign;
    }

    function parts(row) {
        var inputs = row.querySelectorAll('.opening-time input');

        return {
            open: row.querySelector('.opening-switch input'),
            from: inputs[0],
            to: inputs[1]
        };
    }

    function render() {
        preview.textContent = '';

        rows.forEach(function (row) {
            var p = parts(row);
            var open = p.open.checked;
            var text = row.getAttribute('data-name') + ': ';
            var problem = false;

            if (!open) {
                text += closedText;
            } else {
                var from = minutesOf(p.from.value);
                var to = minutesOf(p.to.value);

                if (from === null || to === null || from < 60 || to <= from) {
                    text += invalidText;
                    problem = true;
                } else {
                    text += timeText(from) + hourSeparator + timeText(to);
                }
            }

            row.classList.toggle('opening-day--closed', !open);
            row.classList.toggle('opening-day--invalid', problem);
            p.from.disabled = !open;
            p.to.disabled = !open;

            var line = document.createElement('div');
            line.className = 'opening-preview__line' + (problem ? ' opening-preview__line--invalid' : '');
            line.textContent = text;
            preview.appendChild(line);
        });
    }

    list.addEventListener('change', render);
    list.addEventListener('input', render);

    if (copyButton) {
        copyButton.addEventListener('click', function () {
            var monday = parts(rows[0]);

            for (var i = 1; i <= 4 && i < rows.length; i++) {
                var p = parts(rows[i]);
                p.open.checked = monday.open.checked;
                p.from.value = monday.from.value;
                p.to.value = monday.to.value;
            }

            render();
        });
    }

    render();
})();
