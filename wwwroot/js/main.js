export const countingChars = (input) => {
    const parent = input.parentElement;
    const span = parent.querySelector("snap");
    span.textContent = `${input.value.length}/${input.maxLength}`
}

export const createInputContainer = (name, type, placeholder, maxLenght, value) => {
    let div = document.createElement("div");
    div.classList.add("inputConteiner");

    let snap = document.createElement("snap");
    snap.setAttribute("id", 'charCount');

    let input = document.createElement("input");
    input.required = true;
    input.maxLength = maxLenght;
    input.placeholder = placeholder;
    input.type = type;
    input.name = name;
    input.value = value;

    div.appendChild(input);
    div.appendChild(snap);

    countingChars(input);

    input.addEventListener("input", function () {
        countingChars(this);
    });

    return div;
}

export const sendRequest = async (url, body, method, isAsync, fOnLoad, fLoadStart, fLoadEnd, header, value, responseType) => {
    let httpRequest = new XMLHttpRequest();

    try {
        httpRequest.onloadstart = fLoadStart;
        httpRequest.onloadend = fLoadEnd;
    }
    catch { }

    httpRequest.onload = fOnLoad;

    httpRequest.onerror = () => {
        alert("Не предвиденная ошибка, попробуйте позже.")
    };

    httpRequest.open(method, url, isAsync);
    if (responseType) httpRequest.responseType = responseType;
    if (header) httpRequest.setRequestHeader(header, value);
    httpRequest.send(body);
}

export const viewNotFoundMessageOnPage = (message,content) => {
    const h2 = document.createElement("h2");
    h2.textContent = message;
    content.innerHTML = '';
    content.appendChild(h2);
}