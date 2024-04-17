var form = document.querySelector("form");

form.addEventListener("submit", (e) => {
    e.preventDefault();

    var _form = new FormData(form);

    fetch(location.href,{
        method: 'POST',
        body: _form
    }).then(response => {
        if (response.redirected) {
            location.href = response.url;
        }
        return response.json;
    }).then(data => {
        window.alert(data);
    }).catch(error => {
        window.alert("Ошибка: Не удалось отправить запрос");
    })
})