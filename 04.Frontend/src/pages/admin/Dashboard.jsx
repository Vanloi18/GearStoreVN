import React, { useEffect, useState } from 'react';
import adminDashboardApi from '../../api/adminDashboardApi';

const Dashboard = () => {
    const [stats, setStats] = useState({
        totalUsers: 0,
        totalOrders: 0,
        totalProducts: 0,
        totalRevenue: 0
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchStats = async () => {
            try {
                // Axios interceptor already unwraps ApiResponse<T>.data
                // So 'data' is already the DashboardStatsDto
                const data = await adminDashboardApi.getStats();
                setStats(data);
            } catch (err) {
                setError('Failed to load dashboard stats');
                console.error('Dashboard error:', err);
            } finally {
                setLoading(false);
            }
        };
        fetchStats();
    }, []);

    const cardStyle = {
        backgroundColor: 'white',
        padding: '1.5rem',
        borderRadius: '8px',
        boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
        flex: 1,
        minWidth: '200px',
        textAlign: 'center'
    };

    const valueStyle = {
        fontSize: '2rem',
        fontWeight: 'bold',
        color: '#1890ff',
        marginTop: '0.5rem'
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: '#ff4d4f' }}>{error}</div>;

    return (
        <div>
            <h1 style={{ marginBottom: '2rem' }}>Dashboard</h1>
            <div style={{ display: 'flex', gap: '2rem', flexWrap: 'wrap' }}>
                <div style={cardStyle}>
                    <h3>Total Users</h3>
                    <div style={valueStyle}>{stats.totalUsers}</div>
                </div>
                <div style={cardStyle}>
                    <h3>Total Orders</h3>
                    <div style={valueStyle}>{stats.totalOrders}</div>
                </div>
                <div style={cardStyle}>
                    <h3>Total Products</h3>
                    <div style={valueStyle}>{stats.totalProducts}</div>
                </div>
                <div style={cardStyle}>
                    <h3>Total Revenue</h3>
                    <div style={{ ...valueStyle, color: '#52c41a' }}>${stats.totalRevenue?.toLocaleString()}</div>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
