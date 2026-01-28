import React from 'react';

const CartItem = ({ item, onUpdateQuantity, onRemove }) => {
    const formattedPrice = (price) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price * 25000).replace('₫', '') + '₫';
    };

    return (
        <div className="card" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '1rem', border: '1px solid #EEE' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '1.5rem', flex: 1 }}>
                <div style={{ width: '100px', height: '100px', backgroundColor: '#fff', border: '1px solid #F4F4F4', borderRadius: '4px', display: 'flex', alignItems: 'center', justifyContent: 'center', overflow: 'hidden' }}>
                    {item.imageUrl ? (
                        <img src={item.imageUrl} alt={item.name} style={{ maxWidth: '90%', maxHeight: '90%', objectFit: 'contain' }} />
                    ) : (
                        <span style={{ fontSize: '2rem' }}>⌨️</span>
                    )}
                </div>
                <div>
                    <h4 style={{ margin: '0 0 0.5rem 0', fontSize: '1rem', fontWeight: 600 }}>{item.name}</h4>
                    {item.variantName && (
                        <div style={{ fontSize: '0.85rem', color: '#666', marginBottom: '0.5rem', background: '#F8F8F8', padding: '2px 8px', borderRadius: '2px', display: 'inline-block' }}>
                            Phân loại: <b>{item.variantName}</b>
                        </div>
                    )}
                    <div style={{ color: 'var(--primary-color)', fontWeight: 700 }}>{formattedPrice(item.price)}</div>
                </div>
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '3rem' }}>
                <div style={{ display: 'flex', alignItems: 'center', border: '1px solid #DDD', borderRadius: '4px', overflow: 'hidden' }}>
                    <button
                        style={{ width: '32px', height: '32px', border: 'none', background: '#F9F9F9', cursor: 'pointer' }}
                        onClick={() => onUpdateQuantity(item.id, item.quantity - 1, item.variantId)}
                        disabled={item.quantity <= 1}
                    >
                        -
                    </button>
                    <div style={{ width: '40px', textAlign: 'center', fontSize: '0.9rem', fontWeight: 600 }}>{item.quantity}</div>
                    <button
                        style={{ width: '32px', height: '32px', border: 'none', background: '#F9F9F9', cursor: 'pointer' }}
                        onClick={() => onUpdateQuantity(item.id, item.quantity + 1, item.variantId)}
                    >
                        +
                    </button>
                </div>

                <div style={{ textAlign: 'right', minWidth: '120px' }}>
                    <div style={{ color: 'var(--primary-color)', fontWeight: 800, fontSize: '1.1rem' }}>
                        {formattedPrice(item.price * item.quantity)}
                    </div>
                </div>

                <button
                    onClick={() => onRemove(item.id, item.variantId)}
                    style={{ background: 'none', border: 'none', color: '#999', cursor: 'pointer', fontSize: '1.2rem', padding: '0.5rem' }}
                    title="Xóa sản phẩm"
                >
                    🗑️
                </button>
            </div>
        </div>
    );
};

export default CartItem;
