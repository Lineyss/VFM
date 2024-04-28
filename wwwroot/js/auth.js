import { countingChars } from './main.js'

document.querySelectorAll("form > div > input").forEach(element => {
    countingChars(element);
    element.addEventListener("input", function () {
        countingChars(this);
    });
});

var form = document.querySelector("form");
var button = document.querySelector("form > button"); 

form.addEventListener("submit", (e) => {
    e.preventDefault();

    button.disabled = true;

    var _form = new FormData(form);

    fetch(location.href, {
        method: 'POST',
        body: _form
    }).then(response => {
        if (response.redirected) location.href = response.url;
    }).catch(error => {
        alter(error);
    });

    button.disabled = false;
})
