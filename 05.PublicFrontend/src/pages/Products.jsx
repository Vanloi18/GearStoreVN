import React, { useEffect, useState } from 'react';
import productApi from '../api/productApi';
import { useCart } from '../context/CartContext';
import ProductCard from '../components/ProductCard';
import { useNavigate } from 'react-router-dom';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const { addToCart } = useCart();

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const data = await productApi.getAll();
                // Check for PagedResult structure (.items) or direct array
                if (data && data.items && Array.isArray(data.items)) {
                    setProducts(data.items);
                } else if (Array.isArray(data)) {
                    setProducts(data);
                } else if (data.data && Array.isArray(data.data)) {
                    setProducts(data.data);
                }
            } catch (error) {
                console.error("Failed to load products", error);
            } finally {
                setLoading(false);
            }
        };
        fetchProducts();
    }, []);

    const handleAddToCart = (product) => {
        addToCart(product, 1);
        alert('Added to cart!');
    };

    if (loading) return <div>Loading...</div>;

    return (
        <div style={{ display: 'grid', gridTemplateColumns: '260px 1fr', gap: '2rem' }}>
            {/* Sidebar Filters */}
            <aside>
                <div style={{ backgroundColor: 'white', padding: '1.5rem', borderRadius: '4px', border: '1px solid #EEE' }}>
                    <h4 style={{ marginBottom: '1.5rem', borderBottom: '2px solid var(--primary-color)', paddingBottom: '0.5rem', fontSize: '0.9rem' }}>DANH MỤC</h4>
                    <ul style={{ listStyle: 'none', padding: 0, fontSize: '0.9rem', lineHeight: '2.5' }}>
                        {['Laptop Gaming', 'Laptop Văn Phòng', 'PC GVN', 'Linh kiện PC', 'Màn hình', 'Bàn phím', 'Chuột'].map(c => (
                            <li key={c} style={{ cursor: 'pointer', display: 'flex', justifyContent: 'space-between' }}>
                                <span>{c}</span>
                                <span style={{ color: '#CCC' }}>&rsaquo;</span>
                            </li>
                        ))}
                    </ul>

                    <h4 style={{ margin: '2rem 0 1rem 0', borderBottom: '2px solid var(--primary-color)', paddingBottom: '0.5rem', fontSize: '0.9rem' }}>KHOẢNG GIÁ</h4>
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', fontSize: '0.85rem' }}>
                        {['Dưới 10 triệu', '10tr - 20tr', '20tr - 30tr', 'Trên 30 triệu'].map(p => (
                            <label key={p} style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                                <input type="checkbox" /> {p}
                            </label>
                        ))}
                    </div>
                </div>
            </aside>

            {/* Product Grid */}
            <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', backgroundColor: '#fff', padding: '1rem', borderRadius: '4px', border: '1px solid #EEE' }}>
                    <h1 style={{ fontSize: '1.2rem', margin: 0 }}>TẤT CẢ SẢN PHẨM</h1>
                    <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', fontSize: '0.85rem' }}>
                        <span>Sắp xếp:</span>
                        <select style={{ padding: '0.3rem', border: '1px solid #DDD', borderRadius: '4px', outline: 'none' }}>
                            <option>Mới nhất</option>
                            <option>Giá thấp đến cao</option>
                            <option>Giá cao đến thấp</option>
                        </select>
                    </div>
                </div>

                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: '1rem' }}>
                    {products.length > 0 ? (
                        products.map(product => (
                            <ProductCard key={product.id} product={product} onAddToCart={() => handleAddToCart(product)} />
                        ))
                    ) : (
                        <div style={{ textAlign: 'center', gridColumn: '1/-1', padding: '4rem' }}>
                            <h3>Không tìm thấy sản phẩm nào.</h3>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Products;
