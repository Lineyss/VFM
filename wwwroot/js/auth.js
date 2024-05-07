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

    await sendRequest(location.href, _form, 'POST', false, function () {
        if (this.status / 100 == 4) {
            let response = JSON.parse(this.response);
            alert(response.errorText);
        }
        else {
            if (this.responseURL) location.href = this.responseURL;
        }
    });

    button.disabled = false;
})
