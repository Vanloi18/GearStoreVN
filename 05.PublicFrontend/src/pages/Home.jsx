import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import productApi from '../api/productApi';
import ProductCard from '../components/ProductCard';

const Home = () => {
    const [featuredProducts, setFeaturedProducts] = useState([]);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const data = await productApi.getAll();
                // Just take first 4 as featured
                if (Array.isArray(data)) {
                    setFeaturedProducts(data.slice(0, 4));
                } else if (data.data && Array.isArray(data.data)) {
                    setFeaturedProducts(data.data.slice(0, 4));
                }
            } catch (error) {
                console.error("Failed to load products", error);
            }
        };
        fetchProducts();
    }, []);

    return (
        <div>
            {/* Professional Hero Section */}
            <div style={{
                display: 'grid',
                gridTemplateColumns: '1fr 300px',
                gap: '1rem',
                marginBottom: '3rem'
            }}>
                <div style={{
                    backgroundColor: '#E01020',
                    color: 'white',
                    padding: '4rem',
                    borderRadius: '4px',
                    backgroundImage: 'linear-gradient(45deg, #E01020 0%, #ff4d4f 100%)',
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center'
                }}>
                    <h1 style={{ fontSize: '3.5rem', fontWeight: 900, marginBottom: '1rem', letterSpacing: '-2px' }}>VŨ TRỤ<br />GAMING</h1>
                    <p style={{ fontSize: '1.25rem', marginBottom: '2rem', opacity: 0.9 }}>Trải nghiệm cấu hình đột phá, dẫn đầu mọi cuộc chơi.</p>
                    <div style={{ display: 'flex', gap: '1rem' }}>
                        <Link to="/products" className="btn" style={{ backgroundColor: 'white', color: '#E01020', padding: '0.8rem 2.5rem', fontSize: '1rem', borderRadius: '4px' }}>
                            MUA NGAY
                        </Link>
                        <Link to="/products" className="btn" style={{ backgroundColor: 'rgba(255,255,255,0.2)', color: 'white', border: '1px solid rgba(255,255,255,0.4)', padding: '0.8rem 2.5rem', fontSize: '1rem' }}>
                            CHI TIẾT
                        </Link>
                    </div>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                    <div style={{ flex: 1, backgroundColor: '#333', borderRadius: '4px', padding: '1.5rem', color: 'white', display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                        <h4 style={{ color: 'var(--accent-color)' }}>SẢN PHẨM MỚI</h4>
                        <p style={{ fontSize: '0.8rem' }}>Cập nhật xu hướng tech 2024</p>
                    </div>
                    <div style={{ flex: 1, backgroundColor: '#EEE', borderRadius: '4px', padding: '1.5rem', display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                        <h4 style={{ color: '#E01020' }}>TRẢ GÓP 0%</h4>
                        <p style={{ fontSize: '0.8rem' }}>Duyệt hồ sơ trong 15 phút</p>
                    </div>
                </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
                <h2 className="section-title" style={{ margin: 0 }}>SẢN PHẨM NỔI BẬT</h2>
                <Link to="/products" style={{ color: 'var(--primary-color)', fontWeight: 700, fontSize: '0.9rem' }}>XEM TẤT CẢ &rang;</Link>
            </div>

            <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(230px, 1fr))',
                gap: '1.5rem',
                marginBottom: '4rem'
            }}>
                {featuredProducts.length > 0 ? (
                    featuredProducts.map(product => (
                        <ProductCard key={product.id} product={product} onAddToCart={() => {/* Handled by card or context */ }} />
                    ))
                ) : (
                    <p>Đang tải sản phẩm...</p>
                )}
            </div>

            {/* Sub Banner */}
            <div style={{
                backgroundColor: '#fff',
                padding: '2rem',
                borderRadius: '4px',
                border: '1px solid #EEE',
                display: 'flex',
                justifyContent: 'space-around',
                marginBottom: '4rem'
            }}>
                <div style={{ textAlign: 'center' }}>
                    <span style={{ fontSize: '2rem' }}>🚚</span>
                    <h5 style={{ margin: '0.5rem 0' }}>GIAO HÀNG TOÀN QUỐC</h5>
                </div>
                <div style={{ textAlign: 'center' }}>
                    <span style={{ fontSize: '2rem' }}>🛡️</span>
                    <h5 style={{ margin: '0.5rem 0' }}>BẢO HÀNH TẬN TÂM</h5>
                </div>
                <div style={{ textAlign: 'center' }}>
                    <span style={{ fontSize: '2rem' }}>🔄</span>
                    <h5 style={{ margin: '0.5rem 0' }}>ĐỔI TRẢ 7 NGÀY</h5>
                </div>
            </div>
        </div>
    );
};

export default Home;
