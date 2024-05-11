import { countingChars, createInputContainer, sendRequest } from './main.js'
import JSZip from 'jszip';
import './lib/docx-preview.min.js';

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
const buttonCreateF = document.getElementById("createF");
const buttonUpdateF = document.getElementById("updateF");
const content = document.querySelector(".content");
const reader = new FileReader();
reader.onloadstart = () => {
    viewOrHiddenLoad(false);
};
reader.onloadend = () => {
    viewOrHiddenLoad(true);
}

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
            let data = this.response? JSON.parse(this.response) : this.response;
            if (this.status == 403) alert("Ошибка: У вас нету прав для совершения этого действия");
            else if (this.status == 400 && data) alert(data.errorText);
            else if (this.status == 200) {
                for (const element of data) {
                    createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size, element.isFile);
                }
                alert('Файл(ы) загруженны');
            } else alert("Не предвиденная ошибка. Попробуйте позже.")
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

const getFormCreateOrUpdateFileOrDirectory = (isFile) => {
    formInPopupH1.innerHTML = 'Создать файл/директорию';

    let inputBox = createInputContainer('fileName', 'text', 'Название файла', 260, '');
    formInPopup.appendChild(inputBox);

    let div = document.createElement("div");
    let p = document.createElement('p');
    p.innerHTML = 'Это файл?'
    let inputCheckBox = document.createElement('input');
    inputCheckBox.name = 'isFile';
    inputCheckBox.type = 'checkbox';
    inputCheckBox.checked = isFile;

    div.appendChild(inputCheckBox);
    div.appendChild(p);
    formInPopup.appendChild(div)

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
        if (pathArray.length == 1)
            buttonUpdateF.disabled = false;
        else
            buttonUpdateF.disabled = true;
    });
    tdCheckBox.appendChild(checkBox);

    const tdImage = document.createElement("td");
    const image = document.createElement("img");
    image.src = imgPath;
    tdImage.appendChild(image);

    tr.append(tdCheckBox, tdImage, createSimpleTd(fileName), createSimpleTd(fullPath), createSimpleTd(dateCreate), createSimpleTd(dateChange));
    tr.appendChild(createSimpleTd(size));

    tbody.appendChild(tr);

    tr.addEventListener("dblclick", function () {
        const path = this.childNodes[3].textContent;
        const isFileInt = this.getAttribute("isfile");
        location.href = `${location.origin}${location.pathname}${covertPropertyesToUrl({ ...getPropertyes(), pageNumber:1, path, isFile: isFileInt })}`;
    })
}

const createSimpleTd = (text) => {
    const td = document.createElement("td");
    td.textContent = text;
    return td;
}

const displayPdfDoc = (data) => {
    const pdfDoc = document.createElement('object');
    pdfDoc.style.cssText = 'width:100%; height:100%;'
    pdfDoc.data = data;
    pdfDoc.type = 'application/pdf';

    content.appendChild(pdfDoc);
}

const viewNotFoundMessageOnPage = (message) => {
    const h2 = document.createElement("h2");
    h2.textContent = message;
    content.innerHTML = '';
    content.appendChild(h2);
}

const main = async () => {
    let propertyes = getPropertyes();

    if (propertyes['isFile'] == 'false' || propertyes['isFile'] == false) {
        const url = mainUrl + covertPropertyesToUrl(propertyes);

        sendRequest(url, null, 'GET', true, function () {
            if (this.status == 200) {
                const data = JSON.parse(this.response);
                createPagination(data.totalNumberPages);

                for (const element of data.currentItems) {
                    createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size, element.isFile);
                }
            }
            else viewNotFoundMessageOnPage("Ничего не найдено.");
        },function () {
            console.log(performance.now());
            viewOrHiddenPopup(false);
            viewOrHiddenLoad(false);
        },function () {
            viewOrHiddenPopup(true);
            viewOrHiddenLoad(true);
            console.log(performance.now())
        });

        document.querySelector(".close").addEventListener("click", () => {
            viewOrHiddenPopup(true);
        });

        buttonCreateF.addEventListener("click", () => {
            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            getFormCreateOrUpdateFileOrDirectory(true);
            formInPopup.addEventListener("submit", (e) => {
                e.preventDefault();

                let path = getPropertyes()['path'];
                const isFile = formInPopup.querySelector('input[type=checkbox]').checked;

                if (path[path.length - 1] == '//') path += formInPopup.fileName.value;
                else path += `//${formInPopup.fileName.value}`;
            
                let url = mainUrl + `?path=${path}&isFile=${isFile}`;

                sendRequest(url, null, 'POST', false, function () {
                    let data = this.response ? JSON.parse(this.response) : this.response;
                    if (this.status == 403) alert("Ошибка: У вас нету прав для совершения этого действия");
                    else if (this.status == 400 && data) alert(data.errorText);
                    else if (this.status == 200) createContentRow(data.icon, data.fileName, data.fullPath, data.dateCreate, data.dateChange, data.size, data.isFile);
                    else alert("Не предвиденная ошибка. Попробуйте позже.")
                },function () {
                    viewOrHiddenLoad(false);
                },function () {
                    viewOrHiddenLoad(true);
                });
            });
        });

        buttonUpdateF.addEventListener("click", () => {
            let selectTrs = document.querySelectorAll(".selectTr");
            if (selectTrs.length != 1) return;

            let isFile = (selectTrs[0].getAttribute("isfile") === 'true');

            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            getFormCreateOrUpdateFileOrDirectory(isFile);
            formInPopup.querySelector('input[type=checkbox]').disabled = true;
            formInPopup.addEventListener('submit', (e) => {
                e.preventDefault();
                let fullPath = document.querySelector('.selectTr').children[3].innerHTML;
                console.log(document.querySelector('.selectTr'));

                let url = mainUrl + `?path=${fullPath}&fileName=${formInPopup.fileName.value}`;

                console.log(url);

                sendRequest(url, null, 'PUT', false, function () {
                    let data = this.response ? JSON.parse(this.response) : this.response;
                    if (this.status == 403) alert("Ошибка: У вас нету прав для совершения этого действия");
                    else if (this.status == 400 && data) alert(data.errorText ?? "Не предвиденная ошибка. Попробуйте позже.");
                    else if (this.status == 200) location.reload();
                    else alert("Не предвиденная ошибка. Попробуйте позже.")
                }, function () {
                    viewOrHiddenLoad(false);
                }, function () {
                    viewOrHiddenLoad(true);
                })
            });
        });

        buttonUpload.addEventListener("click", () => {
            viewOrHiddenPopup(false);
            formInPopup.innerHTML = '';
            createdInputFileElement = 1;
            getFormUploadFiles();
        });
    }
    else if (propertyes['isFile'] == 'true' || propertyes['isFile'] == true) {
        pathArray.push(propertyes['path']);
        content.innerHTML = '';

        content.style.cssText = "height:100% !important; align-items: center !important;";

        const pagination = document.querySelector('.pagination');
        content.parentElement.removeChild(pagination);

        buttonUpload.disabled = true;
        buttonCreateF.disabled = true;
        downloadButton.disabled = false;
        deleteButton.disabled = false;

        const url = mainUrl + '/download';

        let paths = JSON.stringify(pathArray);
        sendRequest(url, paths, 'POST', true, function () {
            if (this.status == 400 && this.response) viewNotFoundMessageOnPage("Не удалось открыть файл.");
            else if (this.status == 200) {
                const blob = this.response;
                let type = blob.type.split('/')[1];
                type = type.toLowerCase();

                console.log(type);

                switch (type) {
                    case 'pdf':
                        reader.onload = () => displayPdfDoc(reader.result);
                        reader.readAsDataURL(blob);
                        break;
                    case 'jpg':
                    case 'png':
                    case 'gif':
                    case 'bmp':
                    case 'bmp ico':
                    case 'icon':
                    case 'webmn':
                    case 'webp':
                    case 'tif':
                    case 'tiff':
                        const img = document.createElement('img');
                        img.style.cssText = "max-height:100%; max-width: 100%;"
                        img.src = URL.createObjectURL(blob);
                        content.appendChild(img);
                        break;
                    case 'svg':
                        reader.onload = () => {
                            const arrayBuffer = reader.result;
                            const svgString = new TextDecoder().decode(arrayBuffer);

                            const svgBlob = new Blob([svgString], { type: 'image/svg+xml' });
                            const svgUrl = URL.createObjectURL(svgBlob);

                            const image = document.createElement('img');
                            image.src = svgUrl;
                            content.appendChild(image);
                        };
                        reader.readAsArrayBuffer(blob);
                        break
                    case 'doc':
                    case 'docx':
                        docx.renderAsync(blob, content).then(x => console.log("docx: finished"));
                        break
                    case 'xls':
                    case 'xlsx':
                    case 'ppt':
                    case 'pptx':
                        const url = `${URL.createObjectURL(blob)}.${type}`;
                        console.log(url);
                        const frame = document.createElement('iframe');
                        frame.src = url;
                        frame.width = '100%';
                        frame.height = '100%';
                        frame.frameBorder = '0';

                        content.appendChild(frame);
                        break
                    default:
                        viewNotFoundMessageOnPage("Не удалось открыть файл.");
                        break;
                }
            }
        }, function () {
            viewOrHiddenPopup(false);
            viewOrHiddenLoad(false);
        }, function () {
            viewOrHiddenPopup(true);
            viewOrHiddenLoad(true);
        }, 'Content-Type', 'application/json', 'blob')
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
            if (this.status == 400 && this.response) alert(JSON.parse(this.response).errorText);
            else if (this.status == 200) {
                const blob = this.response;
                const url = URL.createObjectURL(blob);

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
            else if (this.status == 403) alert("Ошибка: У вас нету прав для совершения этого действия");
            else alert("Не предвиденная ошибка. Попробуйте позже.");
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
                if (this.status == 400 && this.response) alert(JSON.parse(this.response).errorText);
                else if (this.status == 200) location.reload();
                else if (this.status == 403) alert("Ошибка: У вас нету прав для совершения этого действия");
                else alert("Не предвиденная ошибка. Попробуйте позже.");
            },function () {
                viewOrHiddenLoad(false);
            },function () {
                viewOrHiddenLoad(true);
            }, 'Content-Type', 'application/json');
        }
    })
}

main();