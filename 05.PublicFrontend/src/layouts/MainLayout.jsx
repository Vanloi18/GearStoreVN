import React, { useState, useEffect } from 'react';
import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { useCart } from '../context/CartContext';

const MainLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { cartCount } = useCart();
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        const checkAuth = () => {
            const token = localStorage.getItem('access_token');
            setIsAuthenticated(!!token);
        };

        checkAuth();
        // Listen to storage events or just re-check on location change (poor man's auth state)
        window.addEventListener('storage', checkAuth);

        return () => {
            window.removeEventListener('storage', checkAuth);
        };
    }, [location]);

    const handleLogout = () => {
        localStorage.removeItem('access_token');
        setIsAuthenticated(false);
        navigate('/login');
    };

    return (
        <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', backgroundColor: '#F4F4F4' }}>
            {/* Top Bar for Contact/Promo */}
            <div style={{ backgroundColor: '#111', color: '#fff', padding: '0.5rem 0', fontSize: '0.8rem' }}>
                <div className="container" style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <span>Chào mừng đến với GearStore - Hệ sinh thái Gaming hàng đầu</span>
                    <div style={{ display: 'flex', gap: '1rem' }}>
                        <span>Hotline: 1900 6789</span>
                        <span>Hệ thống Showroom</span>
                    </div>
                </div>
            </div>

            <header style={{ backgroundColor: '#fff', boxShadow: '0 2px 10px rgba(0,0,0,0.05)', position: 'sticky', top: 0, zIndex: 1000 }}>
                {/* Main Header */}
                <div className="container" style={{ padding: '1rem', display: 'flex', alignItems: 'center', gap: '2rem' }}>
                    <Link to="/" style={{ fontSize: '1.8rem', fontWeight: '900', color: 'var(--primary-color)', letterSpacing: '-1px' }}>
                        GEAR<span style={{ color: '#333' }}>STORE</span>
                    </Link>

                    {/* Search Bar - Iconic GearVN feature */}
                    <div style={{ flex: 1, position: 'relative', display: 'flex' }}>
                        <input
                            type="text"
                            placeholder="Bạn cần tìm gì? (Laptop, Bàn phím, Chuột...)"
                            style={{
                                width: '100%',
                                padding: '0.75rem 1rem',
                                paddingRight: '3rem',
                                border: '2px solid #EEE',
                                borderRadius: '4px',
                                outline: 'none',
                                transition: '0.2s'
                            }}
                            onFocus={(e) => e.target.style.borderColor = 'var(--primary-color)'}
                            onBlur={(e) => e.target.style.borderColor = '#EEE'}
                        />
                        <button style={{
                            position: 'absolute', right: '0', top: 0, bottom: 0,
                            padding: '0 1rem', background: 'var(--primary-color)', color: 'white', border: 'none', borderRadius: '0 4px 4px 0'
                        }}>
                            🔍
                        </button>
                    </div>

                    <nav style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
                        <Link to="/products" style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                            <span style={{ fontSize: '1.2rem' }}>📦</span>
                            <span style={{ fontSize: '0.7rem', fontWeight: 700 }}>SẢN PHẨM</span>
                        </Link>

                        {isAuthenticated ? (
                            <Link to="/orders" style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                                <span style={{ fontSize: '1.2rem' }}>📋</span>
                                <span style={{ fontSize: '0.7rem', fontWeight: 700 }}>ĐƠN HÀNG</span>
                            </Link>
                        ) : null}

                        <Link to="/cart" style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center', position: 'relative' }}>
                            <span style={{ fontSize: '1.2rem' }}>🛒</span>
                            <span style={{ fontSize: '0.7rem', fontWeight: 700 }}>GIỎ HÀNG</span>
                            {cartCount > 0 && (
                                <span style={{
                                    position: 'absolute', top: '-5px', right: '0',
                                    backgroundColor: 'var(--accent-color)', color: '#000',
                                    fontSize: '0.65rem', fontWeight: '900',
                                    padding: '2px 5px', borderRadius: '10px'
                                }}>
                                    {cartCount}
                                </span>
                            )}
                        </Link>

                        {isAuthenticated ? (
                            <button onClick={handleLogout} className="btn" style={{ fontSize: '0.7rem', fontWeight: 700 }}>
                                <span style={{ border: '2px solid #333', padding: '0.2rem 0.5rem', borderRadius: '4px' }}>ĐĂNG XUẤT</span>
                            </button>
                        ) : (
                            <Link to="/login" style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                                <span style={{ fontSize: '1.2rem' }}>👤</span>
                                <span style={{ fontSize: '0.7rem', fontWeight: 700 }}>TÀI KHOẢN</span>
                            </Link>
                        )}
                    </nav>
                </div>

                {/* Categories Bar */}
                <div style={{ borderTop: '1px solid #EEE', backgroundColor: '#fff' }}>
                    <div className="container" style={{ display: 'flex', gap: '2rem', padding: '0.5rem 1rem' }}>
                        {['LAPTOP', 'PC GAMING', 'MÀN HÌNH', 'BÀN PHÍM', 'CHUỘT', 'TAI NGHE', 'LINH KIỆN'].map(cat => (
                            <Link key={cat} to={`/products?category=${cat}`} style={{ fontSize: '0.85rem', fontWeight: 700, whiteSpace: 'nowrap' }}>
                                {cat}
                            </Link>
                        ))}
                    </div>
                </div>
            </header>

            <main style={{ flex: 1, padding: '2rem 0' }}>
                <div className="container">
                    <Outlet />
                </div>
            </main>

            <footer style={{ backgroundColor: '#fff', borderTop: '4px solid var(--primary-color)', padding: '3rem 0', marginTop: '3rem' }}>
                <div className="container">
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '2rem' }}>
                        <div>
                            <h4 style={{ color: 'var(--primary-color)', marginBottom: '1rem' }}>GEARSTORE</h4>
                            <p style={{ fontSize: '0.9rem', color: '#666' }}>Hệ thống cửa hàng Hi-end PC & Gaming Gear chuyên nghiệp nhất Việt Nam.</p>
                        </div>
                        <div>
                            <h4 style={{ marginBottom: '1rem' }}>CHÍNH SÁCH</h4>
                            <ul style={{ listStyle: 'none', padding: 0, fontSize: '0.85rem', color: '#666', lineHeight: '2' }}>
                                <li>Chính sách bảo hành</li>
                                <li>Chính sách vận chuyển</li>
                                <li>Chính sách đổi trả</li>
                            </ul>
                        </div>
                        <div>
                            <h4 style={{ marginBottom: '1rem' }}>THANH TOÁN</h4>
                            <p style={{ fontSize: '0.85rem', color: '#666' }}>Hỗ trợ trả góp 0%, thanh toán qua thẻ, ví điện tử...</p>
                        </div>
                    </div>
                    <div style={{ textAlign: 'center', marginTop: '3rem', paddingTop: '1.5rem', borderTop: '1px solid #EEE', fontSize: '0.8rem', color: '#999' }}>
                        &copy; {new Date().getFullYear()} GearStore. Bản quyền thuộc về Team Dev GearStore.
                    </div>
                </div>
            </footer>
        </div>
    );
};

export default MainLayout;
