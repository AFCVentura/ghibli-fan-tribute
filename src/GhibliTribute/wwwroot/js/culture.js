export function setCulture(culture) {
    const cookieName = '.AspNetCore.Culture';

    const cookieValue = `c=${encodeURIComponent(culture)}|uic=${encodeURIComponent(culture)}`;

    document.cookie = `${cookieName}=${cookieValue};path=/`;

    location.reload();
}

export function getCultureCookie() {
    return document.cookie;
}