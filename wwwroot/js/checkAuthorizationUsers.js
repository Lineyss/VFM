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

if (!jwtToken() && location.pathname != "/auth/index.html") location.href = location.origin + "/auth/index.html"

console.log(location.origin);