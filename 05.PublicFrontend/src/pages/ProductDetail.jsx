import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import productApi from '../api/productApi';
import { useCart } from '../context/CartContext';

const ProductDetail = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [product, setProduct] = useState(null);
    const [quantity, setQuantity] = useState(1);
    const [loading, setLoading] = useState(true);
    const [selectedVariant, setSelectedVariant] = useState(null);

    const { addToCart } = useCart();

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                const data = await productApi.getById(id);
                setProduct(data.data || data);
            } catch (error) {
                console.error("Failed to load product", error);
            } finally {
                setLoading(false);
            }
        };
        fetchProduct();
    }, [id]);

    useEffect(() => {
        if (product && product.variants && product.variants.length > 0) {
            setSelectedVariant(product.variants[0]);
        } else {
            setSelectedVariant(null);
        }
    }, [product]);

    const handleAddToCart = () => {
        const price = selectedVariant ? selectedVariant.price : product.price;
        const variantId = selectedVariant ? selectedVariant.id : null;
        const variantName = selectedVariant ? selectedVariant.name : null;
        const currentStock = selectedVariant ? selectedVariant.stock : product.stock;

        if (currentStock < quantity) {
            alert('Insufficient stock!');
            return;
        }

        addToCart(product, quantity, variantId, variantName, price);
        alert('Đã thêm vào giỏ hàng!');
    };

    const formattedPrice = (price) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price * 25000).replace('₫', '') + '₫';
    };

    if (loading) return <div style={{ textAlign: 'center', padding: '10rem' }}>🔔 Đang tải thông tin sản phẩm...</div>;
    if (!product) return <div style={{ textAlign: 'center', padding: '10rem' }}>❌ Không tìm thấy sản phẩm</div>;

    const currentPrice = selectedVariant ? selectedVariant.price : product.price;
    const oldPrice = currentPrice * 1.15; // Fake discount
    const currentStock = selectedVariant ? selectedVariant.stock : product.stock;

    return (
        <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'minmax(400px, 1fr) 450px', gap: '2.5rem', flexWrap: 'wrap', backgroundColor: '#fff', padding: '2rem', borderRadius: '4px', border: '1px solid #EEE' }}>
                {/* Image Gallery Side */}
                <div style={{ position: 'sticky', top: '100px', height: 'fit-content' }}>
                    <div style={{
                        backgroundColor: '#fff',
                        height: '500px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        border: '1px solid #F4F4F4',
                        borderRadius: '4px',
                        overflow: 'hidden',
                        padding: '2rem'
                    }}>
                        {product.imageUrl ? (
                            <img src={product.imageUrl} alt={product.name} style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }} />
                        ) : (
                            <span style={{ fontSize: '5rem' }}>⌨️</span>
                        )}
                    </div>
                </div>

                {/* Info Side */}
                <div>
                    <h1 style={{ fontSize: '1.8rem', fontWeight: 700, marginBottom: '0.5rem', lineHeight: '1.3' }}>{product.name}</h1>
                    <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem', fontSize: '0.9rem', color: '#666' }}>
                        <span>Thương hiệu: <b style={{ color: 'var(--primary-color)' }}>{product.brandName || 'CHÍNH HÃNG'}</b></span>
                        <span>|</span>
                        <span>SKU: {selectedVariant?.sku || product.sku || 'N/A'}</span>
                    </div>

                    <div style={{ backgroundColor: '#F9F9F9', padding: '1.5rem', borderRadius: '4px', marginBottom: '2rem' }}>
                        <div style={{ display: 'flex', alignItems: 'baseline', gap: '1rem' }}>
                            <span style={{ fontSize: '2.2rem', fontWeight: 800, color: 'var(--primary-color)' }}>{formattedPrice(currentPrice)}</span>
                            <span style={{ fontSize: '1.1rem', color: '#999', textDecoration: 'line-through' }}>{formattedPrice(oldPrice)}</span>
                            <span style={{ background: 'var(--accent-color)', color: '#000', fontSize: '0.75rem', fontWeight: 800, padding: '2px 6px', borderRadius: '2px' }}>-15%</span>
                        </div>
                        <div style={{ marginTop: '0.5rem', color: '#22c55e', fontSize: '0.9rem', fontWeight: 600 }}>
                            ● Tiết kiệm: {formattedPrice(oldPrice - currentPrice)}
                        </div>
                    </div>

                    {/* Variant Selector */}
                    {product.variants && product.variants.length > 0 && (
                        <div style={{ marginBottom: '2rem' }}>
                            <label style={{ display: 'block', marginBottom: '1rem', fontWeight: '700', fontSize: '0.9rem' }}>CHỌN PHIÊN BẢN:</label>
                            <div style={{ display: 'flex', gap: '0.75rem', flexWrap: 'wrap' }}>
                                {product.variants.map(variant => (
                                    <button
                                        key={variant.id}
                                        onClick={() => setSelectedVariant(variant)}
                                        style={{
                                            padding: '0.75rem 1rem',
                                            borderRadius: '4px',
                                            border: selectedVariant?.id === variant.id ? '2px solid var(--primary-color)' : '1px solid #DDD',
                                            backgroundColor: 'white',
                                            color: selectedVariant?.id === variant.id ? 'var(--primary-color)' : '#333',
                                            fontWeight: 600,
                                            fontSize: '0.85rem',
                                            position: 'relative'
                                        }}
                                        disabled={variant.stock <= 0}
                                    >
                                        {variant.name}
                                        {selectedVariant?.id === variant.id && (
                                            <span style={{ position: 'absolute', bottom: '-1px', right: '-1px', width: '15px', height: '15px', background: 'var(--primary-color)', color: '#fff', fontSize: '10px', display: 'flex', alignItems: 'center', justifyContent: 'center', borderRadius: '4px 0 2px 0' }}>✓</span>
                                        )}
                                        {variant.stock <= 0 && <span style={{ opacity: 0.5, marginLeft: '5px' }}>(Hết hàng)</span>}
                                    </button>
                                ))}
                            </div>
                        </div>
                    )}

                    <div style={{ marginBottom: '2.5rem' }}>
                        <label style={{ display: 'block', marginBottom: '1rem', fontWeight: '700', fontSize: '0.9rem' }}>SỐ LƯỢNG:</label>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0' }}>
                            <button onClick={() => setQuantity(q => Math.max(1, q - 1))} style={{ width: '40px', height: '40px', border: '1px solid #DDD', background: '#FFF' }}>-</button>
                            <input
                                type="number"
                                min="1"
                                max={currentStock}
                                value={quantity}
                                onChange={(e) => setQuantity(Math.min(currentStock, Math.max(1, parseInt(e.target.value) || 1)))}
                                style={{ width: '60px', height: '40px', textAlign: 'center', borderTop: '1px solid #DDD', borderBottom: '1px solid #DDD', borderLeft: 'none', borderRight: 'none', appearance: 'none' }}
                            />
                            <button onClick={() => setQuantity(q => Math.min(currentStock, q + 1))} style={{ width: '40px', height: '40px', border: '1px solid #DDD', background: '#FFF' }}>+</button>
                            <span style={{ marginLeft: '1rem', fontSize: '0.85rem', color: '#666' }}>(Còn {currentStock} sản phẩm)</span>
                        </div>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                        <button
                            onClick={handleAddToCart}
                            className="btn btn-primary"
                            style={{ padding: '1.2rem', fontSize: '1rem', fontWeight: 800, borderRadius: '4px' }}
                            disabled={currentStock <= 0}
                        >
                            {currentStock > 0 ? 'THÊM VÀO GIỎ HÀNG' : 'HẾT HÀNG'}
                        </button>
                        <button
                            onClick={() => { handleAddToCart(); navigate('/cart'); }}
                            className="btn"
                            style={{ backgroundColor: '#ffce3e', color: '#111', padding: '1.2rem', fontSize: '1rem', fontWeight: 800, borderRadius: '4px' }}
                            disabled={currentStock <= 0}
                        >
                            MUA NGAY
                        </button>
                    </div>

                    <div style={{ marginTop: '2rem', padding: '1.5rem', background: '#FFFBE6', border: '1px dashed #FFE58F', borderRadius: '4px', fontSize: '0.85rem' }}>
                        <p style={{ margin: 0, fontWeight: 700, color: '#856404' }}>📢 ƯU ĐÃI ĐẶC BIỆT:</p>
                        <ul style={{ paddingLeft: '1.2rem', marginTop: '0.5rem', color: '#856404' }}>
                            <li>Tặng lót chuột chuyên dụng khi mua PC.</li>
                            <li>Bảo hành 1 đổi 1 trong 30 ngày lỗi NSX.</li>
                            <li>Hỗ trợ setup tại nhà khu vực nội thành.</li>
                        </ul>
                    </div>
                </div>
            </div>

            {/* Product Description Section */}
            <div style={{ marginTop: '2rem', backgroundColor: '#fff', padding: '2rem', borderRadius: '4px', border: '1px solid #EEE' }}>
                <h2 style={{ fontSize: '1.4rem', borderBottom: '2px solid #EEE', paddingBottom: '1rem', marginBottom: '1.5rem' }}>THÔNG TIN CHI TIẾT</h2>
                <div style={{ lineHeight: '1.8', color: '#333' }}>
                    {product.description || 'Đang cập nhật nội dung cho sản phẩm này...'}
                </div>
            </div>
        </div>
    );
};

export default ProductDetail;
