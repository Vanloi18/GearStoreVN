import React, { useEffect, useState } from 'react';
import adminOrdersApi from '../../api/adminOrdersApi';

const OrderStatus = {
    Pending: 1,
    Processing: 2,
    Shipping: 3,
    Completed: 4,
    Cancelled: 5,
};

const getStatusName = (status) => {
    switch (status) {
        case 1: return 'Pending';
        case 2: return 'Processing';
        case 3: return 'Shipping';
        case 4: return 'Completed';
        case 5: return 'Cancelled';
        default: return 'Unknown';
    }
};

const statusOptions = [
    { value: 1, label: 'Pending' },
    { value: 2, label: 'Processing' },
    { value: 3, label: 'Shipping' },
    { value: 4, label: 'Completed' },
    { value: 5, label: 'Cancelled' },
];

const Orders = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(null); // ID of order being updated
    const [error, setError] = useState('');

    const fetchOrders = async () => {
        setLoading(true);
        try {
            const data = await adminOrdersApi.getAll();
            const items = data.items || (Array.isArray(data) ? data : (data.data || []));
            setOrders(items);
            setError('');
        } catch (err) {
            setError('Failed to fetch orders');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchOrders();
    }, []);

    const handleStatusChange = async (id, newStatus) => {
        setUpdating(id);
        try {
            await adminOrdersApi.updateStatus(id, parseInt(newStatus));

            // Optimistic Update
            setOrders(prev => prev.map(o =>
                o.id === id ? { ...o, status: parseInt(newStatus) } : o
            ));

            // Optional: Show success toast/alert
        } catch (err) {
            alert('Failed to update status');
            console.error(err);
            // Revert changes by refetching
            fetchOrders();
        } finally {
            setUpdating(null);
        }
    };

    const tableStyle = {
        width: '100%',
        borderCollapse: 'collapse',
        backgroundColor: 'white',
        boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
    };

    const thStyle = {
        textAlign: 'left',
        padding: '1rem',
        borderBottom: '2px solid #f0f0f0',
        backgroundColor: '#fafafa'
    };

    const tdStyle = {
        padding: '1rem',
        borderBottom: '1px solid #f0f0f0'
    };

    if (loading && orders.length === 0) return <div>Loading...</div>;
    if (error) return <div style={{ color: '#ff4d4f' }}>{error}</div>;

    return (
        <div>
            <h1 style={{ marginBottom: '2rem' }}>Orders Management</h1>
            <div style={{ overflowX: 'auto' }}>
                <table style={tableStyle}>
                    <thead>
                        <tr>
                            <th style={thStyle}>Order #</th>
                            <th style={thStyle}>Customer</th>
                            <th style={thStyle}>Total</th>
                            <th style={thStyle}>Date</th>
                            <th style={thStyle}>Current Status</th>
                            <th style={thStyle}>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.length === 0 ? (
                            <tr>
                                <td colSpan="6" style={{ ...tdStyle, textAlign: 'center', color: '#999' }}>No orders found</td>
                            </tr>
                        ) : (
                            orders.map(order => (
                                <tr key={order.id}>
                                    <td style={tdStyle}>{order.orderNumber || order.id}</td>
                                    <td style={tdStyle}>{order.customerName || order.userId}</td>
                                    <td style={tdStyle}>${order.totalAmount.toFixed(2)}</td>
                                    <td style={tdStyle}>{new Date(order.createdAt).toLocaleDateString()}</td>
                                    <td style={tdStyle}>
                                        <span style={{
                                            padding: '0.25rem 0.5rem',
                                            borderRadius: '4px',
                                            backgroundColor: '#e6f7ff',
                                            border: '1px solid #91d5ff',
                                            color: '#1890ff',
                                            fontWeight: 500
                                        }}>
                                            {getStatusName(order.status)}
                                        </span>
                                    </td>
                                    <td style={tdStyle}>
                                        <select
                                            value={order.status}
                                            onChange={(e) => handleStatusChange(order.id, e.target.value)}
                                            disabled={updating === order.id}
                                            style={{
                                                padding: '0.5rem',
                                                borderRadius: '4px',
                                                borderColor: '#d9d9d9',
                                                cursor: 'pointer'
                                            }}
                                        >
                                            {statusOptions.map(opt => (
                                                <option key={opt.value} value={opt.value}>
                                                    {opt.label}
                                                </option>
                                            ))}
                                        </select>
                                        {updating === order.id && <span style={{ marginLeft: '0.5rem', fontSize: '0.8rem' }}>Saving...</span>}
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default Orders;
