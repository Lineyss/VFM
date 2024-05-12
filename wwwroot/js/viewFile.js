import { sendRequest, viewNotFoundMessageOnPage } from './main.js';
import './lib/jszip.min.js';
import './lib/docx-preview.min.js';
import './lib/xlsx.full.min.js';

const content = document.querySelector('.content');
const loadPopup = document.querySelector('.loadPopup');

const filePath = new URLSearchParams(location.search).get('path');
const reader = new FileReader();

reader.onloadstart = () => {
    ViewOrHiddenLoad(false);
};
reader.onloadend = () => {
    ViewOrHiddenLoad(true);
}

class File {
    constructor(blob) {
        this.blob = blob;
        this.extention = blob.type.split('/')[1];
        this.extention = this.extention.toLowerCase();
    }

    displayPdfDoc = () => {
        const pdfDoc = document.createElement('object');
        pdfDoc.style.cssText = 'width:100%; height:100%;'
        pdfDoc.data = this.blob;
        pdfDoc.type = 'application/pdf';

        content.appendChild(pdfDoc);
    }

    displayTables = () => {
        reader.onload = (e) => {
            var data = new Uint8Array(e.target.result);
            var workbook = XLSX.read(data, { type: 'array' });
            var sheetName = workbook.SheetNames[0];
            var worksheet = workbook.Sheets[sheetName];
            var htmlStr = XLSX.utils.sheet_to_html(worksheet);

            content.innerHTML = htmlStr;
        };
        reader.readAsArrayBuffer(this.blob);
    }

    displayWord = () => {
        docx.renderAsync(this.blob, content).then(x => console.log("docx: finished"));
    }

    displayImage = () => {
        if (this.extention == 'svg') {
            reader.onload = () => {
                const arrayBuffer = reader.result;
                const svgString = new TextDecoder().decode(arrayBuffer);

                const svgBlob = new Blob([svgString], { type: 'image/svg+xml' });
                const svgUrl = URL.createObjectURL(svgBlob);

                const image = document.createElement('img');
                image.src = svgUrl;
                content.appendChild(image);
            };
            reader.readAsArrayBuffer(this.blob);
        }
        else {
            const img = document.createElement('img');
            img.style.cssText = "max-height:100%; max-width: 100%;"
            img.src = URL.createObjectURL(this.blob);
            content.appendChild(img);
        }
    }

    displayPresentation = () => {
        reader.onload = (e) => {
            var pptx = new PptxGenJS();
            pptx.read(e.target.result);

            var textContent = '';
            pptx.slides.forEach(function (slide) {
                slide.data.shapes.forEach(function (shape) {
                    if (shape.text) {
                        textContent += '<p>' + shape.text + '</p>';
                    }
                });
            });

            content.innerHTML = textContent;
        };

        reader.readAsArrayBuffer(this.blob);
    }
}

const ViewOrHiddenLoad = (bool) => {
    loadPopup.classList.toggle('hidden', bool);
}

const main = () => {
    const url = `${location.origin}/api/FileManager/download`;
    let path = [];
    path.push(filePath);
    path = JSON.stringify(path);

    sendRequest(url, path, 'POST', true, function () {
        if (this.status == 400 && this.response) viewNotFoundMessageOnPage("Не удалось открыть файл.", content);
        else if (this.status == 200) {
            const file = new File(this.response);
            switch (file.extention) {
                case 'pdf':
                    file.displayPdfDoc();
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
                case 'svg':
                    file.displayImage();
                    break;
                case 'doc':
                case 'docx':
                    file.displayWord();
                    break;
                case 'xls':
                case 'xlsx':
                case 'csv':
                    file.displayTables();
                    break;
                case 'ppt':
                case 'pptx':
                    file.displayPresentation();
                    break
                default:
                    viewNotFoundMessageOnPage("Не удалось открыть файл.", content);
                    break;
            }
        }
    }, null, null, 'Content-Type', 'application/json', 'blob')
}

main();