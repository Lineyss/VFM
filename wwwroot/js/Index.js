const popupContainer = document.querySelector(".popupConteiner");
const popupMain = document.querySelector(".popupMain");
const load = document.querySelector(".load");
const paginationContainer = document.querySelector(".pagination");
const tbody = document.querySelector("tbody");

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

    if (pageNumber === getPropertyes().pageNumber) {
        a.classList.add("select");
    }

    a.href = `${location.origin}${location.pathname}${covertPropertyesToUrl({ ...getPropertyes(), pageNumber })}`;

    return a;
}

const createPagination = async (maxPagination) => {
    const currentPage = getPropertyes().pageNumber;
    const maxBlocksInPage = Math.min(3, maxPagination - currentPage + 1);

    for (let i = currentPage; i < currentPage + maxBlocksInPage; i++) {
        paginationContainer.appendChild(createPaginatorLink(i, i));
    }

    if (maxPagination - currentPage > 2) {
        paginationContainer.appendChild(createPaginatorLink("...", maxPagination));
    }

    if (currentPage > 1) {
        paginationContainer.insertBefore(createPaginatorLink("←", currentPage - 1), paginationContainer.firstChild);
    }

    if (currentPage < maxPagination) {
        paginationContainer.appendChild(createPaginatorLink("→", currentPage + 1));
    }
}

const createContentRow = (imgPath, fileName, fullPath, dateCreate, dateChange, size) => {
    const tr = document.createElement("tr");

    const tdCheckBox = document.createElement("td");
    const checkBox = document.createElement("input");
    checkBox.type = "checkbox";
    tdCheckBox.appendChild(checkBox);

    const tdImage = document.createElement("td");
    const image = document.createElement("img");
    image.src = imgPath;
    tdImage.appendChild(image);

    tr.append(tdCheckBox, tdImage, createSimpleTd(fileName), createSimpleTd(fullPath), createSimpleTd(dateCreate), createSimpleTd(dateChange), createSimpleTd(convertBytes(size)));

    tbody.appendChild(tr);
}

const convertBytes = (bytes) => {
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];

    let unitIndex = 0;
    while (bytes >= 1024 && unitIndex < units.length - 1) {
        bytes /= 1024;
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
        if (!response.ok) {
            throw new Error(response.statusText);
        }

        const data = await response.json();
        createPagination(data.totalNumberPages);

        for (const element of data.currentItems) {
            createContentRow(element.icon, element.fileName, element.fullPath, element.dateCreate, element.dateChange, element.size);
        }
    } catch (error) {
        const h2 = document.createElement("h2");
        h2.textContent = "Nothing found for this path";
        const content = document.querySelector(".content");
        content.innerHTML = '';
        content.appendChild(h2);
    } finally {
        viewOrHiddenPopup(true);
        viewOrHiddenLoad(true);
    }
}

const main = async () => {
    const url = `${location.origin}/api/FileManager${covertPropertyesToUrl()}`;
    await sendRequest(url);

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
}

viewOrHiddenPopup(false);
viewOrHiddenLoad(false);

main();