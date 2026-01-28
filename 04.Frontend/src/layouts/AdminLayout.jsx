import React from 'react';
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';

const AdminLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        localStorage.removeItem('access_token');
        navigate('/login');
    };

    const navStyle = {
        width: '250px',
        backgroundColor: '#001529',
        color: 'white',
        display: 'flex',
        flexDirection: 'column',
        height: '100vh',
        position: 'fixed',
        left: 0,
        top: 0
    };

    const mainStyle = {
        marginLeft: '250px',
        padding: '2rem',
        backgroundColor: '#f0f2f5',
        minHeight: '100vh'
    };

    const linkStyle = (path) => ({
        color: location.pathname === path ? 'white' : '#a6adb4',
        padding: '1rem 1.5rem',
        textDecoration: 'none',
        backgroundColor: location.pathname === path ? '#1890ff' : 'transparent',
        display: 'block'
    });

    return (
        <div style={{ display: 'flex' }}>
            <nav style={navStyle}>
                <div style={{ padding: '1.5rem', fontSize: '1.25rem', fontWeight: 'bold', borderBottom: '1px solid #333' }}>
                    GearStore Admin
                </div>
                <div style={{ flex: 1, paddingTop: '1rem' }}>
                    <Link to="/admin/dashboard" style={linkStyle('/admin/dashboard')}>Dashboard</Link>
                    <Link to="/admin/orders" style={linkStyle('/admin/orders')}>Orders</Link>
                    <Link to="/admin/users" style={linkStyle('/admin/users')}>Users</Link>
                    <Link to="/admin/products" style={linkStyle('/admin/products')}>Products</Link>
                </div>
                <div style={{ padding: '1rem' }}>
                    <button
                        onClick={handleLogout}
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            backgroundColor: '#ff4d4f',
                            color: 'white',
                            border: 'none',
                            cursor: 'pointer',
                            borderRadius: '4px'
                        }}
                    >
                        Logout
                    </button>
                </div>
            </nav>
            <main style={mainStyle}>
                <Outlet />
            </main>
        </div>
    );
};

export default AdminLayout;
