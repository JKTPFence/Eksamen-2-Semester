window.scrollToHour = function (hour) {
    const wrap = document.querySelector('article.calendar-wrap');
    if (!wrap) return;
    const pixelsPerHour = 30 * 4;
    wrap.scrollTop = hour * pixelsPerHour;
};

window.downloadBase64File = function (fileName, base64String) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/octet-stream;base64,' + base64String;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}