

/*export function hrefLoad(url) {

    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem("jwtToken")
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            return response.text();
        })
        .then(data => {
            // Обновление URL адреса без перезагрузки страницы
            history.replaceState({ path: url }, '', url);

            // Отображение содержимого новой страницы на текущей странице
            document.open();
            document.write(data);
            document.close();
        })
        .catch(error => {
            console.error(error);
        });
}

const jwtToken = () => {
    const cookieString = document.cookie;
    const cookies = cookieString.split(';').map(cookie => cookie.trim());

    for (const cookie of cookies) {
        if (cookie.startsWith('jwtToken=')) {
            const jwtCookie = cookie.split('=')[1];
            return jwtCookie;
        }
    }

    return null;
}

*//*if (!jwtToken() && location.pathname != "/Auth/Login.html") location.href = location.origin + "/Auth/Login.html"
*//*

const exit = document.getElementById("Exit");
exit.addEventListener("click", () => {
    localStorage.removeItem('jwtToken');
});

*/