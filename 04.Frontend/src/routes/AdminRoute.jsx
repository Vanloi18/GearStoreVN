import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { isTokenExpired, hasRole } from '../utils/jwt';

const AdminRoute = () => {
    const token = localStorage.getItem('access_token');

    // 1. Check if token exists
    if (!token) {
        return <Navigate to="/admin/login" replace />;
    }

    // 2. Check if token is expired
    if (isTokenExpired(token)) {
        localStorage.removeItem('access_token');
        return <Navigate to="/admin/login" replace />;
    }

    // 3. Check for Admin role
    if (!hasRole(token, 'Admin')) {
        // Optional: Redirect to unauthorized page or login
        localStorage.removeItem('access_token');
        return <Navigate to="/admin/login" replace />;
    }

    return <Outlet />;
};

export default AdminRoute;
