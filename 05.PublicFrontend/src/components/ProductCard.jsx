import React from 'react';
import { Link } from 'react-router-dom';

const ProductCard = ({ product, onAddToCart }) => {
    return (
        <div className="card" style={{
            display: 'flex', flexDirection: 'column', height: '100%',
            padding: '1rem', border: '1px solid #eee', position: 'relative',
            overflow: 'hidden'
        }}>
            {/* Promo Tag */}
            {product.stock > 0 && (
                <div style={{
                    position: 'absolute', top: '10px', left: '10px',
                    background: 'var(--primary-color)', color: 'white',
                    fontSize: '0.7rem', fontWeight: 700, padding: '2px 8px',
                    borderRadius: '2px', zIndex: 1
                }}>
                    MỚI
                </div>
            )}

            {/* Product Image Container */}
            <Link to={`/products/${product.id}`} style={{ height: '180px', marginBottom: '1rem', display: 'flex', alignItems: 'center', justifyContent: 'center', overflow: 'hidden' }}>
                {product.imageUrl || product.thumbnailImage ? (
                    <img src={product.imageUrl || product.thumbnailImage} alt={product.name} style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain', transition: '0.3s' }} />
                ) : (
                    <div style={{ width: '100%', height: '100%', background: '#F8F8F8', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#CCC', fontSize: '2rem' }}>
                        ⌨️
                    </div>
                )}
            </Link>

            <div style={{ display: 'flex', flexDirection: 'column', flex: 1 }}>
                <h3 style={{ margin: '0 0 0.5rem 0', fontSize: '0.95rem', fontWeight: 600, height: '2.8rem', overflow: 'hidden', lineClamp: 2, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                    <Link to={`/products/${product.id}`} style={{ color: '#333' }}>{product.name}</Link>
                </h3>

                <div style={{ marginBottom: '0.5rem' }}>
                    <span className="price" style={{ fontSize: '1.2rem' }}>
                        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price * 25000).replace('₫', '')}₫
                    </span>
                    {/* Fake original price for effect */}
                    <div style={{ fontSize: '0.8rem', color: '#999', textDecoration: 'line-through' }}>
                        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price * 28000).replace('₫', '')}₫
                    </div>
                </div>

                <div style={{ marginTop: 'auto', display: 'flex', gap: '0.5rem' }}>
                    <button
                        onClick={() => onAddToCart(product.id)}
                        className="btn btn-primary"
                        disabled={product.stock <= 0}
                        style={{ flex: 1, fontSize: '0.8rem', padding: '0.5rem' }}
                    >
                        {product.stock > 0 ? (
                            <><span>🛒</span> THÊM</>
                        ) : 'HẾT HÀNG'}
                    </button>
                    <Link
                        to={`/products/${product.id}`}
                        className="btn btn-outline"
                        style={{ padding: '0.5rem', fontSize: '0.8rem' }}
                    >
                        👁️
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;
