import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from './layouts/AdminLayout';
import Dashboard from './pages/admin/Dashboard';
import Orders from './pages/admin/Orders';
import Users from './pages/admin/Users';
import Products from './pages/admin/Products';
import Login from './pages/admin/Login';
import AdminRoute from './routes/AdminRoute';
import ErrorBoundary from './components/common/ErrorBoundary';

const App = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/admin/login" element={<Login />} />

                <Route element={<AdminRoute />}>
                    <Route path="/admin" element={
                        <ErrorBoundary>
                            <AdminLayout />
                        </ErrorBoundary>
                    }>
                        <Route index element={<Navigate to="/admin/dashboard" replace />} />
                        <Route path="dashboard" element={<Dashboard />} />
                        <Route path="orders" element={<Orders />} />
                        <Route path="users" element={<Users />} />
                        <Route path="products" element={<Products />} />
                    </Route>
                </Route>

                <Route path="/" element={<Navigate to="/admin" replace />} />
                <Route path="*" element={<div>404 Not Found</div>} />
            </Routes>
        </BrowserRouter>
    );
};

export default App;
