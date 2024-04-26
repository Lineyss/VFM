import { countingChars } from './main.js'

const popupContainer = document.querySelector(".popupConteiner");
const load = document.querySelector(".loadPopup");
const createUserForm = document.querySelector(".formContent > form");
const createUserFormContainer = document.querySelector(".formContainer")
const content = document.querySelector(".content");

let url = location.origin + "/api/User"

const validate = () => {
    const loginRegex = /^[a-zA-Z0-9_]{3,20}$/;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@$!%*?&\/])[A-Za-z\d._@$!%*?&\/]{3,25}$/;

    const password = createUserForm.password;
    const login = createUserForm.login;

    if (!loginRegex.test(createUserForm.login.value)) {
        login.setCustomValidity("1. Длинна логина дожно состовлять от 3 до 20 символов. \n 2. Логин не может содержать специальные символы, кроме нижнего подчеркивания.");
    }
    else {
        login.setCustomValidity('');
    }

    if (!passwordRegex.test(createUserForm.password.value)) {
        password.setCustomValidity("1. Должна быть хотя бы одна буква в нижнем регистре.\n2. Должна быть хотя бы одна буква в верхнем регистре.\n3. Соответствует любому символу из диапазона от a до z, от A до Z, от 0 до 9 или одному из специальных символов: . , _ , @, $, !, %, *, ?, & , / , \. \n4. Длина должна быть от 3 до 25 символов. ");
    }
    else {
        password.setCustomValidity('');
    }
}

const viewOrHiddenPopup = (bool) => popupContainer.classList.toggle("hidden", bool);

const viewOrHiddenLoad = (bool) =>  load.classList.toggle("hidden", bool);

const viewOrHiddenCreateUserForm = (bool) => createUserFormContainer.classList.toggle("hidden", bool);

const createSelectBlock = (SpanText, selectName, optionSelected) => {
    let div = document.createElement("div");
    let span = document.createElement("span");
    let selected = document.createElement("select");

    span.appendChild(document.createTextNode(SpanText));

    selected.setAttribute('name', selectName);

    
}

const createUserElement = (ID, login, password, isAdmin, createF, deleteF, updateNameF, downloadF, uploadF) => {
    let form = document.createElement('form');
    form.classList.add("userBlock");


}

const getUsers = async () => {

    viewOrHiddenPopup(false);
    viewOrHiddenLoad(false);

    fetch(url, {
        method: 'GET',
    }).then(response => {
        if (response.ok) return response.json();
    }).then(data => {
        if (!data) throw new Error("Не удалось получить список пользователей");

    }).catch(error => {

    });

    viewOrHiddenPopup(true);
    viewOrHiddenLoad(true);
}

const main = async () => {

    document.querySelector(".formContent > form > button").addEventListener("click", validate)

    document.getElementById("createUser").addEventListener("click", () => {
        viewOrHiddenPopup(false);
        viewOrHiddenCreateUserForm(false);
    });

    createUserForm.addEventListener("submit", (e) => {
        e.preventDefault();

        let form = new FormData(createUserForm);

        viewOrHiddenLoad(true);

        fetch(url, {
            method: 'POST',
            body: form
        }).then(response => {
            if (!response.ok)
                return response.json();
            else
                location.reload();
        }).then(data => {
            if (data) throw new Error(data.errorText.split(':')[1]);
        }).catch(error => {
            alert(error);
        });

        viewOrHiddenLoad(false);
    })

    document.querySelector(".close").addEventListener("click", () => {
        viewOrHiddenPopup(true);
    })

    document.querySelectorAll(".inputConteiner > input").forEach(element => {
        countingChars(element);
        element.addEventListener("input", function () {
            countingChars(this);
        });
    });

};

main();