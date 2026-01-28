export const parseJwt = (token) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            window
                .atob(base64)
                .split('')
                .map(function (c) {
                    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                })
                .join('')
        );

        return JSON.parse(jsonPayload);
    } catch (error) {
        return null; // Invalid token
    }
};

export const isTokenExpired = (token) => {
    const decoded = parseJwt(token);
    if (!decoded || !decoded.exp) return true;

    // exp is in seconds, Date.now() is in ms
    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
};

export const getUserRole = (token) => {
    const decoded = parseJwt(token);
    if (!decoded) return null;

    // ASP.NET Core Identity usually puts role in: 
    // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    // or simply "role" depending on config.
    // We should check both or standard ones.
    return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || null;
};

export const isAuthenticated = () => {
    const token = localStorage.getItem('access_token');
    if (!token) return false;
    return !isTokenExpired(token);
};
