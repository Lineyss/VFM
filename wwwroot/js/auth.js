var form = document.querySelector("form");
var button = document.querySelector("form > button"); 

form.addEventListener("submit", (e) => {
    e.preventDefault();

    button.disabled = true;

    var _form = new FormData(form);

    let url;

    fetch(location.href,{
        method: 'POST',
        body: _form
    }).then(response => {
        if (response.redirected) {
            location.href = response.url;
        }
        return {
           "text": response.statusText,
            "status": response.status
        }
    }).then(data => {
        if (data['status'] != 200) {
            window.alert(data['text']);
        }
    })

    button.disabled = false;
})