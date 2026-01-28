/**
 * Utility to parse JWT and check expiration
 */

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
    } catch (e) {
        console.error('Failed to parse JWT', e);
        return null;
    }
};

export const isTokenExpired = (token) => {
    if (!token) return true;
    try {
        const decoded = parseJwt(token);
        if (!decoded || !decoded.exp) return true;

        // Date.now() is in ms, exp is in seconds
        // Add a 10s buffer for safety
        const currentTime = Date.now() / 1000;
        return decoded.exp < currentTime + 10;
    } catch (e) {
        return true;
    }
};

export const getUserRole = (token) => {
    if (!token) return null;
    const decoded = parseJwt(token);
    if (!decoded) return null;

    // Microsoft Identity uses complex claim types, or standard 'role'
    return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;
};

export const hasRole = (token, roleName) => {
    const role = getUserRole(token);
    if (!role) return false;
    return Array.isArray(role) ? role.includes(roleName) : role === roleName;
};
