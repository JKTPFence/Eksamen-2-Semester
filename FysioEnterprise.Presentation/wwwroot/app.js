window.scrollToHour = function (hour) {
    const wrap = document.querySelector('article.calendar-wrap');
    if (!wrap) return;
    const pixelsPerHour = 30 * 4;
    wrap.scrollTop = hour * pixelsPerHour;
};