window.scrollToHour = function (hour) {
    const rows = document.querySelectorAll('td.time-label');
    if (rows.length > hour) {
        rows[hour].scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};
