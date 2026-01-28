import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { isTokenExpired, getUserRole } from '../utils/auth';

const AdminRoute = () => {
    const token = localStorage.getItem('access_token');

    if (!token) {
        return <Navigate to="/admin/login" replace />;
    }

    if (isTokenExpired(token)) {
        localStorage.removeItem('access_token');
        return <Navigate to="/admin/login" replace />;
    }

    const role = getUserRole(token);
    // Role might be a single string or an array of strings
    const isAdmin = Array.isArray(role) ? role.includes('Admin') : role === 'Admin';

    if (!isAdmin) {
        return <Navigate to="/admin/login" replace />;
    }

    return <Outlet />;
};

export default AdminRoute;
