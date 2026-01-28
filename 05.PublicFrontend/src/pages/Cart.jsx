import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import CartItem from '../components/CartItem';

const Cart = () => {
    const { cartItems, cartTotal, updateQuantity, removeFromCart } = useCart();

    const formattedPrice = (price) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price * 25000).replace('₫', '') + '₫';
    };

    if (cartItems.length === 0) {
        return (
            <div style={{ textAlign: 'center', margin: '6rem auto', maxWidth: '600px' }}>
                <div style={{ fontSize: '5rem', marginBottom: '1.5rem' }}>🛒</div>
                <h2 style={{ fontWeight: 800 }}>Giỏ hàng của bạn đang trống</h2>
                <p style={{ color: '#666', marginBottom: '2rem' }}>Hãy chọn cho mình những sản phẩm công nghệ đỉnh cao từ GearStore nhé!</p>
                <Link to="/products" className="btn btn-primary" style={{ padding: '1rem 3rem', fontSize: '1rem' }}>TIẾP TỤC MUA SẮM</Link>
            </div>
        );
    }

    return (
        <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '2.5rem' }}>
                <h1 style={{ fontSize: '1.8rem', fontWeight: 800, margin: 0 }}>GIỎ HÀNG CỦA BẠN</h1>
                <span style={{ color: '#999', fontSize: '1.1rem' }}>({cartItems.length} sản phẩm)</span>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 350px', gap: '2rem', alignItems: 'start' }}>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                    {cartItems.map(item => (
                        <CartItem
                            key={`${item.id}-${item.variantId || 'base'}`}
                            item={item}
                            onUpdateQuantity={updateQuantity}
                            onRemove={removeFromCart}
                        />
                    ))}
                </div>

                <div className="card" style={{ position: 'sticky', top: '100px', padding: '1.5rem', border: '1px solid #EEE' }}>
                    <h3 style={{ margin: '0 0 1.5rem 0', fontSize: '1.1rem', fontWeight: 800, borderBottom: '1px solid #EEE', paddingBottom: '1rem' }}>TỔNG ĐƠN HÀNG</h3>

                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1rem', fontSize: '0.95rem' }}>
                        <span>Tạm tính</span>
                        <span style={{ fontWeight: 600 }}>{formattedPrice(cartTotal)}</span>
                    </div>

                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1rem', fontSize: '0.95rem' }}>
                        <span>Giao hàng</span>
                        <span style={{ color: '#22c55e', fontWeight: 600 }}>Miễn phí</span>
                    </div>

                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '1.5rem', marginTop: '1.5rem', paddingTop: '1.5rem', borderTop: '2px dashed #EEE' }}>
                        <span style={{ fontWeight: 800, fontSize: '1.1rem' }}>Tổng cộng</span>
                        <span style={{ color: 'var(--primary-color)', fontWeight: 900, fontSize: '1.3rem' }}>{formattedPrice(cartTotal)}</span>
                    </div>

                    <Link
                        to="/checkout"
                        className="btn btn-primary"
                        style={{ display: 'block', textAlign: 'center', width: '100%', padding: '1.2rem', fontSize: '1rem', fontWeight: 800, borderRadius: '4px' }}
                    >
                        TIẾN HÀNH THANH TOÁN
                    </Link>

                    <p style={{ textAlign: 'center', fontSize: '0.75rem', color: '#999', marginTop: '1rem', lineHeight: '1.4' }}>
                        (Vui lòng kiểm tra lại số lượng và sản phẩm trước khi thanh toán)
                    </p>
                </div>
            </div>
        </div>
    );
};

export default Cart;
