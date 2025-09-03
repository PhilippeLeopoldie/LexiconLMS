function setTheme(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
}


(function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    setTheme(savedTheme);
})();

document.addEventListener('DOMContentLoaded', function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    setTheme(savedTheme);
});

window.addEventListener('popstate', function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    setTheme(savedTheme);
});

const observer = new MutationObserver(function (mutations) {
    mutations.forEach(function (mutation) {
        if (mutation.type === 'attributes' && mutation.attributeName === 'data-bs-theme') {
        } else if (mutation.type === 'childList' || mutation.type === 'attributes') {

            const savedTheme = localStorage.getItem('theme') || 'light';
            if (document.documentElement.getAttribute('data-bs-theme') !== savedTheme) {
                setTheme(savedTheme);
            }
        }
    });
});

observer.observe(document.documentElement, {
    attributes: true,
    childList: true,
    subtree: true
});

setInterval(function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    if (document.documentElement.getAttribute('data-bs-theme') !== savedTheme) {
        setTheme(savedTheme);
    }
}, 100);