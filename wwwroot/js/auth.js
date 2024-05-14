import { countingChars, sendRequest } from './main.js'

document.querySelectorAll("form > div > input").forEach(element => {
    countingChars(element);
    element.addEventListener("input", function () {
        countingChars(this);
    });
});

var form = document.querySelector("form");
var button = document.querySelector("form > button"); 

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    button.disabled = true;

    var _form = new FormData(form);

    let url = `${location.origin}/api/Auth/Login?isCookie=true`

    sendRequest(url, _form, 'POST', false, function () {
        if (this.status / 100 == 4) {
            let response = JSON.parse(this.response);
            alert(response.errorText);
        }
        else {
            if (this.responseURL) location.reload();
        }
    });

    button.disabled = false;
})
