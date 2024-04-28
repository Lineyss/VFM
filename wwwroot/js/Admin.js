import { countingChars } from './main.js'

const popupContainer = document.querySelector(".popupConteiner");
const load = document.querySelector(".loadPopup");
const createUserForm = document.querySelector(".formContent > form");
const paginationContainer = document.querySelector(".pagination");
const createUserFormContainer = document.querySelector(".formContainer")
const content = document.querySelector(".content");

let usersPassword = {};

let url = location.origin + "/api/User"

const validate = (form) => {
    const loginRegex = /^[a-zA-Z0-9_]{3,20}$/;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@$!%*?&\/])[A-Za-z\d._@$!%*?&\/]{3,25}$/;
    const formID = form.getAttribute('id');
    const password = form.password;
    const login = form.login;

    if (!loginRegex.test(login.value)) {
        login.setCustomValidity("1. Длинна логина дожно состовлять от 3 до 20 символов. \n 2. Логин не может содержать специальные символы, кроме нижнего подчеркивания.");
    }
    else {
        login.setCustomValidity('');
    }

    if (formID != 'userForm' || (formID == 'userForm' && password.value.length > 0)) {
        if (!passwordRegex.test(password.value)) {
            password.setCustomValidity("1. Должна быть хотя бы одна буква в нижнем регистре.\n2. Должна быть хотя бы одна буква в верхнем регистре.\n3. Соответствует любому символу из диапазона от a до z, от A до Z, от 0 до 9 или одному из специальных символов: . , _ , @, $, !, %, *, ?, & , / , \. \n4. Длина должна быть от 3 до 25 символов. ");
        }
        else {
            password.setCustomValidity('');
        }
    }
}

const viewOrHiddenPopup = (bool) => popupContainer.classList.toggle("hidden", bool);

const viewOrHiddenLoad = (bool) =>  load.classList.toggle("hidden", bool);

const viewOrHiddenCreateUserForm = (bool) => createUserFormContainer.classList.toggle("hidden", bool);

const createSelectBlock = (SpanText, selectName, optionSelected) => {
    let div = document.createElement("div");
    let span = document.createElement("span");
    let selected = document.createElement("select");
    let optionTrue = document.createElement('option');
    let optionFalse = document.createElement('option');

    let textTrueOption = document.createTextNode('Да');
    let textFalseOption = document.createTextNode('Нет');

    span.appendChild(document.createTextNode(SpanText));

    selected.name = selectName;

    optionTrue.value = true;
    optionTrue.appendChild(textTrueOption);

    optionFalse.value = false;
    optionFalse.appendChild(textFalseOption);

    if (optionSelected) {
        optionTrue.selected = true;
    }
    else {
        optionFalse.selected = true;
    }

    selected.appendChild(optionTrue);
    selected.appendChild(optionFalse);


    div.appendChild(span);
    div.appendChild(selected);

    return div;
}

const createInputContainer = (name, type, placeholder, maxLenght, value) => {
    let div = document.createElement("div");
    div.classList.add("inputConteiner");

    let snap = document.createElement("snap");
    snap.setAttribute("id", 'charCount');

    let input = document.createElement("input");
    if (name != 'password') {
        input.required = true;  
    }
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

const createUserElement = (ID, login, password, isAdmin, createF, deleteF, updateNameF, downloadF, uploadF) => {

    usersPassword[ID] = password;
    
    let form = document.createElement('form');
    form.id = 'userForm';
    form.addEventListener('submit', (e) => {
        e.preventDefault();
    });
    form.classList.add("userBlock");

    let divID = document.createElement('div');
    let p = document.createElement('p');
    let idText = document.createTextNode(ID);
    p.appendChild(idText);
    divID.appendChild(p);

    form.appendChild(divID);
    form.appendChild(createInputContainer('login', 'text', 'Логин', 20, login));
    form.appendChild(createInputContainer('password', 'password', 'Пароль', 25, ''));

    form.appendChild(createSelectBlock('Админ', 'isAdmin', isAdmin));
    form.appendChild(createSelectBlock('Создать папку/файл', 'createF', createF));
    form.appendChild(createSelectBlock('Удалить папку/файл', 'deleteF', deleteF));
    form.appendChild(createSelectBlock('Оновить имя папки/файда', 'updateNameF', updateNameF));
    form.appendChild(createSelectBlock('Скачивать файлы/папки', 'downloadF', downloadF));
    form.appendChild(createSelectBlock('Загружить файл/папку', 'uploadF', uploadF));

    let saveButton = document.createElement('button');
    saveButton.type = "submit";
    let saveButtonText = document.createTextNode('Сохранить');
    saveButton.appendChild(saveButtonText);
    saveButton.addEventListener("click", function () {
        const form = this.parentElement;
        validate(form);
        if (form.checkValidity()) {
            const id = form.querySelector('div > p').innerHTML;

            if (!form.password.value) {
                form.password.value = usersPassword[id];
            }

            const responseForm = new FormData(form);

            let urlUpdate = url + '/' + id;

            fetch(urlUpdate, {
                method: 'PUT',
                body: responseForm
            }).then(response => {
                if (response.ok) {
                    location.reload();
                }
                else throw new Error();
            }).catch(error => {
                alert("Error: Не удалось обновить данные о пользователе.")
            });
        }
    });

    let deleteButton = document.createElement('button');
    deleteButton.type = "submit";
    let deleteButtonText = document.createTextNode('Удалить');
    deleteButton.appendChild(deleteButtonText);
    deleteButton.addEventListener("click", function () {
        const pID = this.parentElement.querySelector("div > p");

        const ID = pID.innerHTML;

        let urlDelete = url + '/' + ID

        fetch(urlDelete, {
            method: 'DELETE'
        }).then(response => {
            if (response.ok) {
                const form = this.parentElement;
                content.removeChild(form);
            } else {
                throw new Error();
            }

        }).catch(error => {
            alert("Error: Не удалось удалить пользователя");
        });
    });

    form.appendChild(saveButton);
    form.appendChild(deleteButton);

    return form;
}

const getUsers = async () => {
    await fetch(url, {
        method: 'GET',
    }).then(response => {
        if (response.ok) return response.json();
    }).then(data => {
        if (!data) throw new Error();

        for (const user of data) {
            content.appendChild(createUserElement(user.id, user.login, user.password, user.isAdmin, user.createF, user.deleteF, user.updateNameF, user.downloadF, user.uploadF));
        }

    }).catch(error => {
        let h2 = document.createElement('h2');
        let h2Text = document.createTextNode("Не удалось получить список пользователей");
        h2.appendChild(h2Text);
        content.appendChild(h2);
    });

}

const main = async () => {
    viewOrHiddenPopup(false);
    viewOrHiddenCreateUserForm(true);
    viewOrHiddenLoad(false);

    document.querySelector(".formContent > form > button").addEventListener("click", function () {
        validate(this.parentElement);
    })

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
    });

    await getUsers();

    viewOrHiddenPopup(true);
    viewOrHiddenCreateUserForm(false);
    viewOrHiddenLoad(true);
};

main();
