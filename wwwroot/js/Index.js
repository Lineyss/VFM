import { countingChars, createInputContainer, sendRequest } from './main.js'

const popupContainer = document.querySelector(".popupConteiner");
const popupMain = document.querySelector(".popupMain");
const load = document.querySelector(".load");
const paginationContainer = document.querySelector(".pagination");
const tbody = document.querySelector("tbody");
const searchInput = document.getElementById('myInput');
const deleteButton = document.getElementById("delete");
const downloadButton = document.getElementById("download");
const formInPopup = document.querySelector(".formPopup > form");
const formInPopupH1 = document.querySelector(".formPopup > h1");
const buttonUpload = document.getElementById("upload");
const buttonCreateFo = document.getElementById("createFo");
const buttonCreateFi = document.getElementById("createFi");

let createdInputFileElement = 1;

let mainUrl = `${location.origin}/api/FileManager`

let pathArray = [];

const viewOrHiddenPopup = (bool) => {
    popupContainer.classList.toggle("hidden", bool);
}

const viewOrHiddenLoad = (bool) => {
    popupMain.classList.toggle("hidden", !bool);
    load.classList.toggle("hidden", bool);
}

const createInputFileBlock = () => {
    const div = document.createElement('div');
    const input = document.createElement('input');
    const button = document.createElement('button');

    input.type = 'file';
    input.required = true;
    input.name = 'files';

    button.innerHTML = '-';
    button.addEventListener('click', function () {
        let div = this.parentElement;
        div.parentElement.removeChild(div);
        createdInputFileElement -= 1;
    });

    div.appendChild(input);
    div.appendChild(button);

    return div;
}

const getFormUploadFiles = () => {
    formInPopupH1.innerHTML = 'Загрузить файлы';
    formInPopup.addEventListener('submit', (e) => {
        e.preventDefault();

        let path = getPropertyes()['path'];

        let url = mainUrl + '/upload?path=' + path;

        let form = new FormData(formInPopup);

        sendRequest(url, form, 'POST', false, function () {
            let data = JSON.parse(this.response);
            if (this.status / 100 == 4) alert(data.errorText);
            else {
                for (const element of data) {
                    createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size, element.isFile);
                }
                alert('Файл(ы) загруженны');
            }
        }, function () {
            viewOrHiddenLoad(false);
        }, function () {
            viewOrHiddenLoad(true);
        });
    });

    formInPopup.setAttribute('enctype', 'multipart/form-data');

    const createdInputButton = document.createElement('button');
    createdInputButton.innerHTML = '+';
    createdInputButton.setAttribute('style', 'margin: 5px 0;');
    createdInputButton.type = "button";

    createdInputButton.addEventListener("click", () => {
        if (createdInputFileElement < 5) {
            let div = createInputFileBlock();
            formInPopup.insertBefore(div, formInPopup.children[formInPopup.children.length - 2]);
            createdInputFileElement += 1;
        }
    });

    const sendButton = document.createElement('button');
    sendButton.innerHTML = 'Сохранить';
    sendButton.type = 'submit';

    formInPopup.appendChild(createInputFileBlock());
    formInPopup.appendChild(createdInputButton);
    formInPopup.appendChild(sendButton);
}

const getFormCreateFolderOrFiles = (isFile) => {
    formInPopup.setAttribute('isFile', isFile)
    formInPopup.addEventListener("submit", (e) => {
        e.preventDefault();

        let path = getPropertyes()['path'];
        const isFile = formInPopup.getAttribute('isFile');

        if (path[path.length - 1] == '//')
            path += formInPopup.fileName.value;
        else
            path += `//${formInPopup.fileName.value}`;
            
        let url = mainUrl + `?path=${path}&isFile=${isFile}`;

        sendRequest(url, null, 'POST', false, function () {
            let data = JSON.parse(this.response);
            if (this.status / 100 == 4) {
                alert(data.errorText);
            }
            else {
                createContentRow(data.icon, data.fileName, data.fullPath, data.dateCreate, data.dateChange, data.size, data.isFile);
            }
        },
        function () {
            viewOrHiddenLoad(false);
        },
        function () {
            viewOrHiddenLoad(true);
        });
    });

    if (isFile) formInPopupH1.innerHTML = 'Создать файл';
    else formInPopupH1.innerHTML = 'Создать директорию';

    let inputBox = createInputContainer('fileName', 'text', 'Название файла', 260, '');
    formInPopup.appendChild(inputBox);

    let button = document.createElement('button');
    button.innerHTML = 'Сохранить'
    button.setAttribute('type', 'submit');

    formInPopup.appendChild(button);
}

const getPropertyes = () => {
    const searchParams = new URLSearchParams(location.search);
    const pageNumber = searchParams.get("pageNumber") || 1;
    const isFile = searchParams.get("isFile") === "true";
    const path = searchParams.get("path") || '';

    return { pageNumber: +pageNumber, isFile, path };
}

const covertPropertyesToUrl = (propertyesDict = getPropertyes()) => {
    const searchParams = new URLSearchParams(propertyesDict);
    return `?${searchParams.toString()}`;
}

const createPaginatorLink = (text, pageNumber) => {
    const a = document.createElement("a");
    a.textContent = text;
    a.classList.add("paginationElement");

    if (pageNumber === getPropertyes().pageNumber)  a.classList.add("selectA");

    a.href = `${location.origin}${location.pathname}${covertPropertyesToUrl({ ...getPropertyes(), pageNumber })}`;

    return a;
}

const createPagination = async (maxPagination) => {
    const currentPage = getPropertyes().pageNumber;
    const maxBlocksInPage = Math.min(3, maxPagination - currentPage + 1);

    for (let i = currentPage; i < currentPage + maxBlocksInPage; i++) {
        paginationContainer.appendChild(createPaginatorLink(i, i));
    }

    if (maxPagination - currentPage > 2) paginationContainer.appendChild(createPaginatorLink("...", maxPagination));

    if (currentPage + 2 < maxPagination) paginationContainer.appendChild(createPaginatorLink(maxPagination, maxPagination));

    if (currentPage > 1) paginationContainer.insertBefore(createPaginatorLink("←", currentPage - 1), paginationContainer.firstChild);

    if (currentPage < maxPagination) paginationContainer.appendChild(createPaginatorLink("→", currentPage + 1));
}

const createContentRow = (imgPath, fileName, fullPath, dateCreate, dateChange, size, isFile) => {
    const tr = document.createElement("tr");

    tr.setAttribute("isfile", isFile);

    const tdCheckBox = document.createElement("td");
    const checkBox = document.createElement("input");
    checkBox.type = "checkbox";
    checkBox.addEventListener("click", function () {
        this.parentElement.parentElement.classList.toggle("selectTr", this.checked);
        const text = this.parentElement.parentElement.childNodes[3].textContent;

        if (this.checked) {
            pathArray.push(text);
        } else {
            const index = pathArray.indexOf(text);
            if (index > -1) pathArray.splice(index, 1);
        }

        if (pathArray.length == 0) {
            deleteButton.disabled = true;
            downloadButton.disabled = true;
        }
        else {
            deleteButton.disabled = false;
            downloadButton.disabled = false;
        }
    });
    tdCheckBox.appendChild(checkBox);

    const tdImage = document.createElement("td");
    const image = document.createElement("img");
    image.src = imgPath;
    tdImage.appendChild(image);

    tr.append(tdCheckBox, tdImage, createSimpleTd(fileName), createSimpleTd(fullPath), createSimpleTd(dateCreate), createSimpleTd(dateChange));
    tr.appendChild(createSimpleTd(convertBytes(size)));

    tbody.appendChild(tr);

    tr.addEventListener("dblclick", function () {
        const path = this.childNodes[3].textContent;
        const isFileInt = this.getAttribute("isfile");
        location.href = `${location.origin}${location.pathname}${covertPropertyesToUrl({ ...getPropertyes(), pageNumber:1, path, isFile: isFileInt })}`;
    })
}

const convertBytes = async (bytes) => {
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];

    let unitIndex = 0;

    bytes = Math.abs(bytes);

    if (bytes < 1) return bytes + ' ' + units[unitIndex];

    while (bytes >= 1024 && unitIndex < units.length - 1) {
        bytes
            /= 1024;
        unitIndex++;
    }

    return bytes.toFixed(2) + ' ' + units[unitIndex];
};

const createSimpleTd = (text) => {
    const td = document.createElement("td");
    td.textContent = text;
    return td;
}

const main = async () => {
    let propertyes = getPropertyes();

    if (propertyes['isFile'] == 'false' || propertyes['isFile'] == false) {
        const url = mainUrl + covertPropertyesToUrl(propertyes);

        sendRequest(url, null, 'GET', true, function () {
            if (this.status / 100 == 2) {
                const data = JSON.parse(this.response);
                createPagination(data.totalNumberPages);

                for (const element of data.currentItems) {
                    createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size, element.isFile);
                }
            }
            else {
                const h2 = document.createElement("h2");
                h2.textContent = "Ничего не найдено.";
                const content = document.querySelector(".content");
                content.innerHTML = '';
                content.appendChild(h2);
            }
        },
        function () {
            viewOrHiddenPopup(false);
            viewOrHiddenLoad(false);
        },
        function () {
            viewOrHiddenPopup(true);
            viewOrHiddenLoad(true);
        });

        document.querySelector(".close").addEventListener("click", () => {
            viewOrHiddenPopup(true);
        });

        buttonCreateFi.addEventListener("click", () => {
            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            getFormCreateFolderOrFiles(true);
        });

        buttonCreateFo.addEventListener("click", () => {
            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            getFormCreateFolderOrFiles(false);
        });

        buttonUpload.addEventListener("click", () => {
            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            createdInputFileElement = 1;
            getFormUploadFiles();
        });
    }
    else if (propertyes['isFile'] == 'true' || propertyes['isFile'] == true) {
        buttonUpload.disabled = true;
        viewOrHiddenPopup(true);
        viewOrHiddenLoad(true);
    }

    const text = propertyes['path'];

    searchInput.addEventListener("input", function () {
        countingChars(this);
    });

    searchInput.value = text;
    countingChars(searchInput);

    downloadButton.addEventListener("click", () => {
        const url = mainUrl + '/download';
        let paths = JSON.stringify(pathArray);

        sendRequest(url, paths, 'POST', true, function () {
            if (this.status / 100 == 4) alert(JSON.parse(this.response).errorText);
            else {
                const blob = this.response;
                const url = URL.createObjectURL(blob);

                console.log(url);                                                       

                const arrayUrl = url.split('/');
                let fileName = arrayUrl[arrayUrl.length - 1];
                const exist = blob.type.split('/')[1];

                fileName = fileName + '.' + exist;

                const a = document.createElement('a');
                a.classList.add('hidden');
                a.href = url;
                a.download = fileName;

                document.body.appendChild(a);

                a.click();

                document.body.removeChild(a);

                URL.revokeObjectURL(url);
            }
        }, function () {
            viewOrHiddenLoad(false);
        }, function () {
            viewOrHiddenLoad(true);
        }, 'Content-Type', 'application/json', 'blob');
    })

    deleteButton.addEventListener("click", () => {
        const result = confirm("Вы точно хотите удалить эти файла? Восстановить их будет невозможно.");
        if (result === true) {
            let paths = JSON.stringify(pathArray);
            sendRequest(mainUrl, paths, 'DELETE', false, function () {
                if (this.status / 100 == 4) alert(JSON.parse(this.response).errorText);
                else if (this.status / 100 == 2) {
                    location.reload();
                }
            },
            function () {
                viewOrHiddenLoad(false);
            },
            function () {
                viewOrHiddenLoad(true);
            }, 'Content-Type', 'application/json');
        }
    })
}

main();