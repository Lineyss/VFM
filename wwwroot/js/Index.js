import { countingChars } from './main.js'

const popupContainer = document.querySelector(".popupConteiner");
const popupMain = document.querySelector(".popupMain");
const load = document.querySelector(".load");
const paginationContainer = document.querySelector(".pagination");
const tbody = document.querySelector("tbody");
const searchInput = document.getElementById('myInput');
const deleteButton = document.getElementById("delete");
const downloadButton = document.getElementById("download");

let mainUrl = `${location.origin}/api/FileManager`

let pathArray = [];

const viewOrHiddenPopup = (bool) => {
    popupContainer.classList.toggle("hidden", bool);
}

const viewOrHiddenLoad = (bool) => {
    popupMain.classList.toggle("hidden", !bool);
    load.classList.toggle("hidden", bool);
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

    tr.append(tdCheckBox, tdImage, createSimpleTd(fileName), createSimpleTd(fullPath), createSimpleTd(dateCreate), createSimpleTd(dateChange), createSimpleTd(convertBytes(size)));

    tbody.appendChild(tr);

    tr.addEventListener("dblclick", function () {
        const path = this.childNodes[3].textContent;
        const isFileInt = this.getAttribute("isfile");
        location.href = `${location.origin}${location.pathname}${covertPropertyesToUrl({ ...getPropertyes(), path, isFile: isFileInt })}`;
    })
}

const convertBytes = (bytes) => {
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

const sendRequest = async (url) => {
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error();

        const data = await response.json();
        createPagination(data.totalNumberPages);

        for (const element of data.currentItems) {
            createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size, element.isFile);
        }

    } catch (error) {
        const h2 = document.createElement("h2");
        h2.textContent = "Ничего не найдено.";
        const content = document.querySelector(".content");
        content.innerHTML = '';
        content.appendChild(h2);
    } finally {
        viewOrHiddenPopup(true);
        viewOrHiddenLoad(true);
    }
}

const main = async () => {
    viewOrHiddenPopup(false);
    viewOrHiddenLoad(false);

    const url = mainUrl + covertPropertyesToUrl();

    const send = sendRequest(url);

    const text = getPropertyes()['path'];

    searchInput.addEventListener("input", function () {
        countingChars(this);
    });

    searchInput.value = text;
    countingChars(searchInput);

    downloadButton.addEventListener("click", () => {
        const url = mainUrl + '/download';
        let paths = JSON.stringify(pathArray);
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: paths
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Не удалось скачать файл');
            }
            return response.blob();
        })
        .then(bob => {
            const url = window.URL.createObjectURL(bob);

            const arrayUrl = url.split('/');
            let fileName = arrayUrl[arrayUrl.length - 1];
            const exist = bob.type.split('/')[1];
            
            fileName = fileName + '.' + exist;

            const a = document.createElement('a');

            a.classList.add("hidden");
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();

            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            alert(error);
        });
    })

    deleteButton.addEventListener("click", () => {
        const result = confirm("Вы точно хотите удалить эти файла? Восстановить их будет невозможно.");
        if (result === true) {
            let paths = JSON.stringify(pathArray);
            console.log(paths);
            fetch(mainUrl, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: paths
            }).then(response => {
                if (!response.ok) throw new Error("Не удалось удалить файлы");
                else location.reload();
            }).catch(error => {
                alert(error);
            })
        }
    })

    document.querySelector(".close").addEventListener("click", () => {
        viewOrHiddenPopup(true);
    });

    document.getElementById("createFi").addEventListener("click", () => {
        viewOrHiddenPopup(false);
    });

    document.getElementById("createFo").addEventListener("click", () => {
        viewOrHiddenPopup(false);
    });

    document.getElementById("upload").addEventListener("click", () => {
        viewOrHiddenPopup(false);
    });

    await send;
}


main();