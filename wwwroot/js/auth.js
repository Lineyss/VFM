import { hrefLoad } from './clientManager.js'

const form = document.querySelector("form");

form.addEventListener("submit", (e) => {
    e.preventDefault();

    url = location.origin + "/api/Auth/Login";

    const _form = new FormData(form);

    console.log(url);

    fetch(url, {
        method: 'POST',
        body: _form
    }).then(response => {
        console.log(response.body);
        return response.json();
    }).then(data => {
        const jwtToken = data.token;
        localStorage.setItem("jwtToken", jwtToken);
        hrefLoad(location.origin + "/VirtualFileManager");
    }).catch(error => {
        window.alert(error);
    });
});