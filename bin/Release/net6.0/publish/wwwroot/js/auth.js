﻿import { countingChars, sendRequest } from './main.js'

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

    let url = `${location.origin}/api/Auth/Login/Cookie`

    sendRequest(url, _form, 'POST', false, function () {
        try {
            console.log(this.status);
            if (this.status / 100 == 4) {
                let response = JSON.parse(this.response);
                alert(response.errorText);
            }
            else {
                if (this.responseURL) location.reload();
            }
        }
        catch {
            alert("Не предвиденная ошибка. Попробуйте позже.");
        }
    });

    button.disabled = false;
})
