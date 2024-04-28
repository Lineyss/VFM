export const countingChars = (input) => {
    const parent = input.parentElement;
    const span = parent.querySelector("snap");
    span.textContent = `${input.value.length}/${input.maxLength}`
}

/*const createPaginatorLink = (text, currentPageNumber, pageNumbers, url) => {
    const a = document.createElement("a");
    a.textContent = text;
    a.classList.add("paginationElement");

    if (currentPageNumber === pageNumbers) a.classList.add("selectA");

    a.href = url;

    return a;
}

export const createPagination = async (maxPagination, currentPageNumber, pageNumbers) => {
    let paginationContainer = document.querySelector('.pagination')

    const currentPage = currentPageNumber;
    const maxBlocksInPage = Math.min(3, maxPagination - currentPage + 1);

    for (let i = currentPage; i < currentPage + maxBlocksInPage; i++) {
        paginationContainer.appendChild(createPaginatorLink(i, i, pageNumbers));
    }

    if (maxPagination - currentPage > 2) paginationContainer.appendChild(createPaginatorLink("...", maxPagination, pageNumbers));

    if (currentPage + 2 < maxPagination) paginationContainer.appendChild(createPaginatorLink(maxPagination, maxPagination, pageNumbers));

    if (currentPage > 1) paginationContainer.insertBefore(createPaginatorLink("←", currentPage - 1, pageNumbers), paginationContainer.firstChild);

    if (currentPage < maxPagination) paginationContainer.appendChild(createPaginatorLink("→", currentPage + 1, pageNumbers));
}*/