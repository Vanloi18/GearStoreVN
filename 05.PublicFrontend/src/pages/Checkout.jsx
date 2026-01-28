import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import orderApi from '../api/orderApi';
import { useCart } from '../context/CartContext';

const Checkout = () => {
    const navigate = useNavigate();
    const { cartItems, cartTotal, clearCart } = useCart();
    const [formData, setFormData] = useState({
        customerName: '',
        customerPhone: '',
        shippingAddress: '',
        notes: ''
    });
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (cartItems.length === 0) {
            navigate('/cart');
        }
    }, [navigate, cartItems]);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handlePlaceOrder = async (e) => {
        e.preventDefault();
        setLoading(true);

        const orderData = {
            ...formData,
            paymentMethod: 1, // COD for now
            items: cartItems.map(item => ({
                productId: item.id,
                variantId: item.variantId,
                quantity: item.quantity
            }))
        };

        try {
            await orderApi.createOrder(orderData);
            clearCart();
            alert('Order placed successfully!');
            navigate('/');
        } catch (error) {
            console.error("Failed to place order", error);
            alert('Failed to place order. ' + (error.response?.data?.message || error.message));
        } finally {
            setLoading(false);
        }
    };

    if (cartItems.length === 0) return null;

    const formattedPrice = (price) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price * 25000).replace('₫', '') + '₫';
    };

    if (cartItems.length === 0) return null;

    return (
        <div style={{ maxWidth: '1100px', margin: '0 auto' }}>
            <h1 style={{ fontSize: '1.8rem', fontWeight: 800, marginBottom: '2.5rem' }}>THÔNG TIN THANH TOÁN</h1>

            <div style={{ display: 'grid', gridTemplateColumns: '1.5fr 1fr', gap: '3rem' }}>
                <div>
                    <div className="card" style={{ padding: '2rem', border: '1px solid #EEE' }}>
                        <h3 style={{ margin: '0 0 1.5rem 0', fontSize: '1.1rem', fontWeight: 800, color: 'var(--primary-color)' }}>1. THÔNG TIN KHÁCH HÀNG</h3>
                        <form onSubmit={handlePlaceOrder}>
                            <div className="form-group">
                                <label className="form-label">HỌ VÀ TÊN <span style={{ color: 'red' }}>*</span></label>
                                <input
                                    type="text"
                                    className="form-input"
                                    name="customerName"
                                    placeholder="Nhập họ và tên người nhận"
                                    value={formData.customerName}
                                    onChange={handleChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label className="form-label">SỐ ĐIỆN THOẠI <span style={{ color: 'red' }}>*</span></label>
                                <input
                                    type="tel"
                                    className="form-input"
                                    name="customerPhone"
                                    placeholder="Nhập số điện thoại để liên lạc"
                                    value={formData.customerPhone}
                                    onChange={handleChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label className="form-label">ĐỊA CHỈ NHẬN HÀNG <span style={{ color: 'red' }}>*</span></label>
                                <textarea
                                    className="form-input"
                                    name="shippingAddress"
                                    rows="3"
                                    placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành phố"
                                    value={formData.shippingAddress}
                                    onChange={handleChange}
                                    required
                                ></textarea>
                            </div>
                            <div className="form-group">
                                <label className="form-label">GHI CHÚ ĐƠN HÀNG</label>
                                <textarea
                                    className="form-input"
                                    name="notes"
                                    rows="2"
                                    placeholder="Yêu cầu khác (Ví dụ: Giao giờ hành chính)"
                                    value={formData.notes}
                                    onChange={handleChange}
                                ></textarea>
                            </div>

                            <div style={{ marginTop: '2.5rem', paddingTop: '1.5rem', borderTop: '1px solid #EEE' }}>
                                <h3 style={{ margin: '0 0 1.5rem 0', fontSize: '1.1rem', fontWeight: 800, color: 'var(--primary-color)' }}>2. PHƯƠNG THỨC THANH TOÁN</h3>
                                <div style={{ padding: '1rem', border: '2px solid var(--primary-color)', borderRadius: '4px', background: '#FFF7F7', display: 'flex', alignItems: 'center', gap: '1rem' }}>
                                    <input type="radio" checked readOnly />
                                    <div>
                                        <div style={{ fontWeight: 700 }}>Thanh toán khi nhận hàng (COD)</div>
                                        <div style={{ fontSize: '0.8rem', color: '#666' }}>Bạn sẽ thanh toán bằng tiền mặt khi shipper giao hàng đến.</div>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>

                <div style={{ alignSelf: 'start' }}>
                    <div className="card" style={{ padding: '1.5rem', border: '1px solid #DDD', background: '#FDFDFD' }}>
                        <h3 style={{ marginTop: 0, fontSize: '1.1rem', fontWeight: 800, borderBottom: '1px solid #EEE', paddingBottom: '1rem', marginBottom: '1.5rem' }}>TỔNG ĐƠN HÀNG</h3>

                        <div style={{ maxHeight: '400px', overflowY: 'auto', marginBottom: '1.5rem' }}>
                            {cartItems.map(item => (
                                <div key={`${item.id}-${item.variantId || 'base'}`} style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                                    <div style={{ width: '50px', height: '50px', background: '#FFF', border: '1px solid #EEE', display: 'flex', alignItems: 'center', justifyContent: 'center', borderRadius: '4px' }}>
                                        {item.imageUrl ? <img src={item.imageUrl} alt={item.name} style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }} /> : '⌨️'}
                                    </div>
                                    <div style={{ flex: 1 }}>
                                        <div style={{ fontSize: '0.85rem', fontWeight: 600, lineHeight: '1.4' }}>{item.quantity}x {item.name}</div>
                                        {item.variantName && <div style={{ fontSize: '0.75rem', color: '#999' }}>Phân loại: {item.variantName}</div>}
                                        <div style={{ fontSize: '0.85rem', color: 'var(--primary-color)', fontWeight: 700, marginTop: '2px' }}>{formattedPrice(item.price * item.quantity)}</div>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <div style={{ borderTop: '2px dashed #EEE', paddingTop: '1.5rem', display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem' }}>
                                <span>Tạm tính</span>
                                <span style={{ fontWeight: 600 }}>{formattedPrice(cartTotal)}</span>
                            </div>
                            <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem' }}>
                                <span>Phí vận chuyển</span>
                                <span style={{ color: '#22c55e', fontWeight: 600 }}>Miễn phí</span>
                            </div>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '1rem', paddingTop: '1rem', borderTop: '1px solid #EEE' }}>
                                <span style={{ fontWeight: 800, fontSize: '1.1rem' }}>TỔNG CỘNG</span>
                                <span style={{ color: 'var(--primary-color)', fontWeight: 900, fontSize: '1.4rem' }}>{formattedPrice(cartTotal)}</span>
                            </div>
                        </div>

                        <button
                            onClick={handlePlaceOrder}
                            className="btn btn-primary"
                            style={{ width: '100%', fontSize: '1.1rem', padding: '1.2rem', marginTop: '2rem', borderRadius: '4px', fontWeight: 800 }}
                            disabled={loading}
                        >
                            {loading ? 'ĐANG XỬ LÝ...' : 'ĐẶT HÀNG NGAY'}
                        </button>
                    </div>

                    <div style={{ marginTop: '1.5rem', padding: '1rem', background: '#F8F8F8', borderRadius: '4px', fontSize: '0.75rem', color: '#666', lineHeight: '1.6' }}>
                        🛡️ <b> GearStore Cam kết:</b> Sản phẩm chính hãng, bảo hành tận tâm, hỗ trợ kỹ thuật trọn đời sản phẩm.
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Checkout;
