const viewOrHiddenPopup = (bool) => {
    let popupConteiner = document.querySelector(".popupConteiner");
    if (bool !== undefined)
        return popupConteiner.classList.toggle("hidden", bool);

    return popupConteiner.classList.toggle("hidden");
}

const viewOrHiddenLoad = (bool) => {
    let popupMain = document.querySelector(".popupMain");
    let load = document.querySelector(".load");

    if (bool !== undefined) {
        popupMain.classList.toggle("hidden", !bool);
        return load.classList.toggle("hidden", bool);
    }

    popupMain.classList.toggle("hidden");
    return load.classList.toggle("hidden");
}

const covertPropertyesToUrl = (propertyesDict) => {
    let arrayStringPropertyes = [];

    for (key in propertyesDict) {
        let stringProperty = key + '=' + propertyesDict[key];
        arrayStringPropertyes.push(stringProperty);
    }

    const stringPropertyes = arrayStringPropertyes.join('&');

    const url = location.origin + location.pathname + '?' + stringPropertyes;

    return url;
}

const getPropertyes = () => {
    let dictionary = {};
    const propertyesString = location.search.substring(1, location.search.length);

    const propertyesArray = propertyesString.split('&');
    console.log(propertyesArray);

    for (let i = 0; i < propertyesArray.length; i++) {
        let subArray = propertyesArray[i].split('=');
        dictionary[subArray[0]] = subArray[1];
    }

    return dictionary;
}

const createPaginations = (maxPagination) => {

    const createA = (text, href = undefined, addEventClick = true) => {
        let a = document.createElement("a");
        try {
            if (text == getPropertyes()['pageNumber'])
            {
                a.classList.add('select')
            }

        }
        catch { }

        text = document.createTextNode(text);
        a.appendChild(text);
        if (href) a.href = href;
        a.classList.add('paginationElement');

        if (addEventClick) {
            a.addEventListener("click", function (e) {
                e.preventDefault();

                propertyes = getPropertyes();

                propertyes['pageNumber'] = this.href[this.href.length-1];

                location.href = covertPropertyesToUrl(propertyes);
            });
        }

        return a;
    }

    let maxBlocksInPage = 3;

    let currentPage = getPropertyes()['pageNumber'];
    currentPage = currentPage !== undefined ? Number(currentPage) : 1;

    let lastIndex = currentPage + 2;

    lastIndex = Math.abs(maxPagination - lastIndex);

    const needHelpBlock = (lastIndex > 1);

    const needViewMaxPage = !((currentPage + 1) <= maxPagination);

    if (currentPage + 2 >= maxPagination) maxBlocksInPage = 2;  

    if (currentPage + 1 >= maxPagination) maxBlocksInPage = 1;

    let arrayPaginations = [];

    if (!needViewMaxPage) {
        for (let i = 0; i < maxBlocksInPage; i++) {
            let a = createA(currentPage + i, currentPage + i);
            arrayPaginations.push(a);
        }
    }

    if (needHelpBlock) {
        let a = createA(text = '...', undefined, addEventClick = false);
        arrayPaginations.push(a);
    }

    let lastA = createA(maxPagination, maxPagination);
    arrayPaginations.push(lastA);

    if (Number(currentPage) - 1 > 0) {
        let a = createA('←',currentPage - 1);
        arrayPaginations.splice(0, 0, a);
    }

    if (Number(currentPage) + 1 <= maxPagination) {
        let a = createA('→',currentPage + 1);
        arrayPaginations.push(a);
    }

    for (let a of arrayPaginations) {
        document.querySelector(".pagination").appendChild(a);
    }
}

const createContent = (imgPath, fileName, fullPath, dateCreate, dateChange, size) => {

    const convertBytes = (bytes) => {
        if (bytes < 1024) {
            return bytes + ' B';
        } else if (bytes < 1024 * 1024) {
            return (bytes / 1024).toFixed(2) + ' KB';
        } else if (bytes < 1024 * 1024 * 1024) {
            return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
        } else {
            return (bytes / (1024 * 1024 * 1024)).toFixed(2) + ' GB';
        }
    }

    let tr = document.createElement("tr");

    let tdChechBox = document.createElement("td");
    let checkBox = document.createElement("input");
    checkBox.type = "checkbox";
    tdChechBox.appendChild(checkBox);

    let tdImage = document.createElement("td");
    let image = document.createElement("img");
    image.src = imgPath;
    tdImage.appendChild(image);

    let tdFileName = document.createElement("td");
    tdFileName.appendChild(fileName);

    let tdFullPath = document.createElement("td");
    tdFullPath.appendChild(fullPath);

    let tdDateCreateFile = document.createElement("td");
    tdDateCreateFile.appendChild(dateCreate);

    let tdDateChangeFile = document.createElement("td");
    tdDateChangeFile.appendChild(dateChange);

    let tdSizeFile = document.createElement("td");
    tdSizeFile.appendChild(convertBytes(size));

    tr.appendChild(tdChechBox);
    tr.appendChild(tdImage);
    tr.appendChild(tdFileName);
    tr.appendChild(tdFullPath);
    tr.appendChild(tdDateCreateFile);
    tr.appendChild(tdDateChangeFile);
    tr.appendChild(tdSizeFile);

    document.querySelector("tbody").appendChild(tr);
}

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

createPaginations(5);

/*viewOrHiddenPopup(false);
viewOrHiddenLoad(false);

let url = location.origin + "/api/FileManager?path=F://"

fetch(url , {
    method: "GET",
}).then(response => {
    if (response.ok) {   
        return response.json(); 
    } else {
        let h2 = document.createElement("h2");
        let h2Text = document.createTextNode("Ничего не нашли по данному пути");

        h2.appendChild(h2Text);

        let content = document.querySelector(".content");

        let tbodyElement = document.querySelector("tbody");
        if (tbodyElement) {
            content.removeChild(tbodyElement);
        }

        content.appendChild(h2);
    }
}).then(data => {
})
.catch(error => {
    console.log(error);
});

viewOrHiddenPopup(true);
viewOrHiddenLoad(true);*/

