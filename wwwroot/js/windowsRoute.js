const oldHref = document.location.href;

window.onload = (e) => {
    e.preventDefault();
    var bodyList = document.querySelector("body")

    var observer = new MutationObserver(function (mutations) {
        if (oldHref != document.location.href) {
            console.log(document.location.href);
        }
    });

    var config = {
        childList: true,
        subtree: true
    };

    observer.observe(bodyList, config);
}