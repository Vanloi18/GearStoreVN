import React, { useEffect, useState } from 'react';
import orderApi from '../api/orderApi';
import { Link } from 'react-router-dom';

const Orders = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const data = await orderApi.getMyOrders();
                // Handle unwrapped response or standard ApiResponse
                // axiosClient typically returns data directly if unwrapped, 
                // but orderApi might return the full ApiResponse if logic varies.
                // Based on axiosClient.js (Step 153/190): "return resData.data" if exists.
                // So 'data' here should be the array or the object containing the array.

                // Safety check
                let items = [];
                if (Array.isArray(data)) {
                    items = data;
                } else if (data && Array.isArray(data.orders)) {
                    items = data.orders;
                } else if (data && Array.isArray(data.data)) {
                    items = data.data;
                }

                setOrders(items);
            } catch (error) {
                console.error("Failed to load orders", error);
            } finally {
                setLoading(false);
            }
        };

        fetchOrders();
    }, []);

    const formattedPrice = (price) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price * 25000).replace('₫', '') + '₫';
    };

    if (loading) return <div style={{ textAlign: 'center', padding: '10rem' }}>🔔 Đang tải lịch sử đơn hàng...</div>;

    if (orders.length === 0) {
        return (
            <div style={{ textAlign: 'center', margin: '6rem auto', maxWidth: '600px' }}>
                <div style={{ fontSize: '5rem', marginBottom: '1.5rem' }}>📦</div>
                <h2 style={{ fontWeight: 800 }}>Bạn chưa có đơn hàng nào</h2>
                <p style={{ color: '#666', marginBottom: '2rem' }}>Hãy sắm cho mình những bộ Gear xịn xò để lấp đầy kho lưu trữ nhé!</p>
                <Link to="/products" className="btn btn-primary" style={{ padding: '1rem 3rem', fontSize: '1rem' }}>MUA SẮM NGAY</Link>
            </div>
        );
    }

    return (
        <div style={{ maxWidth: '1100px', margin: '0 auto' }}>
            <h1 style={{ fontSize: '1.8rem', fontWeight: 800, marginBottom: '2.5rem' }}>ĐƠN HÀNG CỦA TÔI</h1>

            <div className="card" style={{ padding: '0', border: '1px solid #EEE', overflow: 'hidden' }}>
                <table style={{ width: '100%', borderCollapse: 'collapse', minWidth: '800px' }}>
                    <thead>
                        <tr style={{ background: '#F9F9F9', borderBottom: '1px solid #EEE', textAlign: 'left' }}>
                            <th style={{ padding: '1.2rem', fontSize: '0.85rem', fontWeight: 700, textTransform: 'uppercase', color: '#666' }}>MÃ ĐƠN HÀNG</th>
                            <th style={{ padding: '1.2rem', fontSize: '0.85rem', fontWeight: 700, textTransform: 'uppercase', color: '#666' }}>NGÀY ĐẶT</th>
                            <th style={{ padding: '1.2rem', fontSize: '0.85rem', fontWeight: 700, textTransform: 'uppercase', color: '#666' }}>TỔNG TIỀN</th>
                            <th style={{ padding: '1.2rem', fontSize: '0.85rem', fontWeight: 700, textTransform: 'uppercase', color: '#666' }}>TRẠNG THÁI</th>
                            <th style={{ padding: '1.2rem', fontSize: '0.85rem', fontWeight: 700, textTransform: 'uppercase', color: '#666' }}>SẢN PHẨM</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map(order => (
                            <tr key={order.id} style={{ borderBottom: '1px solid #F5F5F5' }}>
                                <td style={{ padding: '1.2rem', fontWeight: 700, color: 'var(--primary-color)' }}>{order.orderNumber}</td>
                                <td style={{ padding: '1.2rem', fontSize: '0.9rem' }}>{new Date(order.createdAt).toLocaleDateString('vi-VN')}</td>
                                <td style={{ padding: '1.2rem', fontWeight: 800, color: '#111' }}>{formattedPrice(order.totalAmount)}</td>
                                <td style={{ padding: '1.2rem' }}>
                                    <span style={{
                                        padding: '0.4rem 0.8rem',
                                        borderRadius: '2px',
                                        fontSize: '0.75rem',
                                        fontWeight: 800,
                                        backgroundColor: getStatusColor(order.status).bg,
                                        color: getStatusColor(order.status).text,
                                        textTransform: 'uppercase'
                                    }}>
                                        {formatStatus(order.status)}
                                    </span>
                                </td>
                                <td style={{ padding: '1.2rem', fontSize: '0.85rem', color: '#444' }}>
                                    {order.items.map((i, idx) => (
                                        <div key={i.id} style={{ marginBottom: idx === order.items.length - 1 ? 0 : '0.25rem' }}>
                                            • {i.quantity}x {i.productName}
                                        </div>
                                    ))}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

// Helper to format status text (0 -> Pending, etc)
const formatStatus = (status) => {
    switch (status) {
        case 1: return 'Chờ xác nhận';
        case 2: return 'Đang xử lý';
        case 3: return 'Đang giao hàng';
        case 4: return 'Đã hoàn thành';
        case 5: return 'Đã hủy';
        default: return 'Không xác định';
    }
};

// Helper for status colors
const getStatusColor = (status) => {
    switch (status) {
        case 1: return { bg: '#FFF7E6', text: '#D46B08' }; // Warning gold
        case 2: return { bg: '#E6F7FF', text: '#096DD9' }; // Daybreak blue
        case 3: return { bg: '#F9F0FF', text: '#531DAB' }; // Golden purple
        case 4: return { bg: '#F6FFED', text: '#389E0D' }; // Polar green
        case 5: return { bg: '#FFF1F0', text: '#CF1322' }; // Dust red
        default: return { bg: '#F5F5F5', text: '#595959' };
    }
};

export default Orders;
