const viewOrHiddenPopup = (bool) => {
    let popupConteiner = document.querySelector(".popupConteiner");
    if (bool != undefined)
        return popupConteiner.classList.toggle("hidden", bool);

    return popupConteiner.classList.toggle("hidden");
}

const viewOrHiddenLoad = (bool) => {
    let popupMain = document.querySelector(".popupMain");
    let load = document.querySelector(".load");

    if (bool != undefined)
        popupMain.classList.toggle("hidden", !bool);
        return load.classList.toggle("hidden", bool);
    popupMain.classList.toggle("hidden");
    return load.classList.toggle("hidden");
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


viewOrHiddenPopup(false);
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
viewOrHiddenLoad(true);

