import React, { useEffect, useState } from 'react';
import adminProductsApi from '../../api/adminProductsApi';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [deleting, setDeleting] = useState(false);
    const [error, setError] = useState('');

    // --- Variant Logic State ---
    const [variantActionLoading, setVariantActionLoading] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [variantForm, setVariantForm] = useState({ name: '', price: 0, stock: 0, sku: '' });
    const [editingVariant, setEditingVariant] = useState(null);

    const fetchProducts = async () => {
        setLoading(true);
        try {
            // Axios interceptor already unwraps ApiResponse<T>.data to PagedResult
            const data = await adminProductsApi.getAll();
            // PagedResult has .items
            const items = data.items || (Array.isArray(data) ? data : []);
            setProducts(items);
            setError('');
        } catch (err) {
            setError('Failed to fetch products');
            console.error('Products error:', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProducts();
    }, []);

    const handleDelete = async (id) => {
        if (deleting) return;
        if (!window.confirm('Are you sure you want to delete this product?')) return;

        setDeleting(true);
        try {
            await adminProductsApi.delete(id);
            await fetchProducts();
        } catch (err) {
            alert('Failed to delete product');
            console.error(err);
        } finally {
            setDeleting(false);
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

    const btnStyle = {
        padding: '0.5rem 1rem',
        border: 'none',
        borderRadius: '4px',
        color: 'white',
        cursor: 'pointer',
        fontSize: '0.9rem',
        backgroundColor: '#ff4d4f'
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: '#ff4d4f' }}>{error}</div>;

    const handleOpenVariants = async (productId) => {
        // Show modal immediately or waiting? Better to wait for data or show loading in modal.
        // We will fetch then show.
        try {
            const data = await adminProductsApi.getById(productId);
            // Verify structure: data might be wrapping ApiResponse or just payload
            const product = data.data || data;
            setSelectedProduct(product);
            setShowModal(true);
            setVariantForm({ name: '', price: product.price, stock: 0, sku: '' });
        } catch (err) {
            alert('Failed to load product details');
            console.error(err);
        }
    };

    const handleCloseModal = () => {
        if (variantActionLoading) return; // Prevent closing while action active
        setShowModal(false);
        setSelectedProduct(null);
    };

    const handleCancelEdit = () => {
        setEditingVariant(null);
        setVariantForm({ name: '', price: selectedProduct.price, stock: 0, sku: '' });
    };

    const handleStartEdit = (variant) => {
        setEditingVariant(variant);
        setVariantForm({
            name: variant.name,
            price: variant.price,
            stock: variant.stock,
            sku: variant.sku || ''
        });
    };

    const handleSubmitVariant = async (e) => {
        e.preventDefault();
        setVariantActionLoading(true);
        try {
            if (editingVariant) {
                await adminProductsApi.updateVariant(selectedProduct.id, editingVariant.id, variantForm);
                alert('Variant updated successfully');
            } else {
                await adminProductsApi.addVariant(selectedProduct.id, variantForm);
                alert('Variant added successfully');
            }

            // Refresh product details
            const data = await adminProductsApi.getById(selectedProduct.id);
            setSelectedProduct(data.items || data.data || data);

            // Reset form and editing state
            handleCancelEdit();
        } catch (err) {
            alert('Operation failed: ' + (err.response?.data?.message || err.message));
        } finally {
            setVariantActionLoading(false);
        }
    };

    const handleDeleteVariant = async (variantId) => {
        if (!window.confirm('Delete this variant?')) return;
        setVariantActionLoading(true);
        try {
            // Updated to pass productId as well
            await adminProductsApi.deleteVariant(selectedProduct.id, variantId);
            // Refresh product details
            const data = await adminProductsApi.getById(selectedProduct.id);
            setSelectedProduct(data.data || data);
        } catch (err) {
            alert('Failed to delete variant: ' + (err.response?.data?.message || err.message));
        } finally {
            setVariantActionLoading(false);
        }
    };

    // Styles
    const modalOverlayStyle = {
        position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
        backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1000
    };
    const modalContentStyle = {
        backgroundColor: 'white', padding: '2rem', borderRadius: '8px', width: '600px', maxHeight: '90vh', overflowY: 'auto'
    };

    return (
        <div>
            <h1 style={{ marginBottom: '2rem' }}>Products Management</h1>
            <div style={{ overflowX: 'auto' }}>
                <table style={tableStyle}>
                    <thead>
                        <tr>
                            <th style={thStyle}>Name</th>
                            <th style={thStyle}>Category</th>
                            <th style={thStyle}>Price</th>
                            <th style={thStyle}>Stock</th>
                            <th style={thStyle}>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {products.length === 0 ? (
                            <tr>
                                <td colSpan="5" style={{ ...tdStyle, textAlign: 'center', color: '#999' }}>No products found</td>
                            </tr>
                        ) : (
                            products.map(product => (
                                <tr key={product.id}>
                                    <td style={tdStyle}>{product.name}</td>
                                    <td style={tdStyle}>{product.categoryName || product.categoryId}</td>
                                    <td style={tdStyle}>${product.price}</td>
                                    <td style={tdStyle}>{product.stock}</td>
                                    <td style={tdStyle}>
                                        <button
                                            onClick={() => handleOpenVariants(product.id)}
                                            style={{ ...btnStyle, backgroundColor: '#1890ff', marginRight: '0.5rem' }}
                                        >
                                            Variants
                                        </button>
                                        <button
                                            onClick={() => handleDelete(product.id)}
                                            style={btnStyle}
                                            disabled={deleting}
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* Variant Management Modal */}
            {showModal && selectedProduct && (
                <div style={modalOverlayStyle}>
                    <div style={modalContentStyle}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
                            <h2 style={{ margin: 0 }}>Manage Variants – {selectedProduct.name}</h2>
                            <button onClick={handleCloseModal} style={{ background: 'none', border: 'none', fontSize: '1.5rem', cursor: 'pointer' }}>&times;</button>
                        </div>

                        {/* Existing Variants List */}
                        <div style={{ marginBottom: '2rem' }}>
                            <h3 style={{ fontSize: '1.1rem', marginBottom: '1rem' }}>Existing Variants</h3>
                            {selectedProduct.variants && selectedProduct.variants.length > 0 ? (
                                <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                                    <thead>
                                        <tr style={{ backgroundColor: '#f5f5f5' }}>
                                            <th style={{ padding: '0.5rem', textAlign: 'left' }}>Name</th>
                                            <th style={{ padding: '0.5rem', textAlign: 'left' }}>SKU</th>
                                            <th style={{ padding: '0.5rem', textAlign: 'left' }}>Price</th>
                                            <th style={{ padding: '0.5rem', textAlign: 'left' }}>Stock</th>
                                            <th style={{ padding: '0.5rem', textAlign: 'left' }}>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {selectedProduct.variants.map(v => (
                                            <tr key={v.id} style={{ borderBottom: '1px solid #eee' }}>
                                                <td style={{ padding: '0.5rem' }}>{v.name}</td>
                                                <td style={{ padding: '0.5rem' }}>{v.sku || '-'}</td>
                                                <td style={{ padding: '0.5rem' }}>${v.price}</td>
                                                <td style={{ padding: '0.5rem' }}>{v.stock}</td>
                                                <td style={{ padding: '0.5rem' }}>
                                                    <button
                                                        onClick={() => handleStartEdit(v)}
                                                        style={{ ...btnStyle, backgroundColor: '#faad14', padding: '0.25rem 0.5rem', fontSize: '0.8rem', marginRight: '0.25rem' }}
                                                        disabled={variantActionLoading}
                                                    >
                                                        Edit
                                                    </button>
                                                    <button
                                                        onClick={() => handleDeleteVariant(v.id)}
                                                        style={{ ...btnStyle, padding: '0.25rem 0.5rem', fontSize: '0.8rem', opacity: variantActionLoading ? 0.5 : 1 }}
                                                        disabled={variantActionLoading}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p style={{ color: '#666', fontStyle: 'italic' }}>No variants yet.</p>
                            )}
                        </div>

                        {/* Add/Edit Variant Form */}
                        <div style={{ padding: '1.5rem', backgroundColor: '#f9fafb', borderRadius: '4px' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                                <h3 style={{ fontSize: '1.1rem', margin: 0 }}>{editingVariant ? 'Edit Variant' : 'Add New Variant'}</h3>
                                {editingVariant && (
                                    <button onClick={handleCancelEdit} style={{ background: 'none', border: 'none', color: '#1890ff', cursor: 'pointer', fontSize: '0.9rem' }}>Cancel Edit</button>
                                )}
                            </div>
                            <form onSubmit={handleSubmitVariant}>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1rem' }}>
                                    <div>
                                        <label style={{ display: 'block', marginBottom: '0.25rem', fontSize: '0.9rem' }}>Name <span style={{ color: 'red' }}>*</span></label>
                                        <input
                                            type="text"
                                            value={variantForm.name}
                                            onChange={e => setVariantForm({ ...variantForm, name: e.target.value })}
                                            required
                                            style={{ width: '100%', padding: '0.5rem', border: '1px solid #d9d9d9', borderRadius: '4px' }}
                                            placeholder="e.g. Size L, Red"
                                            disabled={variantActionLoading}
                                        />
                                    </div>
                                    <div>
                                        <label style={{ display: 'block', marginBottom: '0.25rem', fontSize: '0.9rem' }}>SKU (Optional)</label>
                                        <input
                                            type="text"
                                            value={variantForm.sku}
                                            onChange={e => setVariantForm({ ...variantForm, sku: e.target.value })}
                                            style={{ width: '100%', padding: '0.5rem', border: '1px solid #d9d9d9', borderRadius: '4px' }}
                                            disabled={variantActionLoading}
                                        />
                                    </div>
                                    <div>
                                        <label style={{ display: 'block', marginBottom: '0.25rem', fontSize: '0.9rem' }}>Price <span style={{ color: 'red' }}>*</span></label>
                                        <input
                                            type="number"
                                            step="0.01"
                                            min="0"
                                            value={variantForm.price}
                                            onChange={e => setVariantForm({ ...variantForm, price: parseFloat(e.target.value) })}
                                            required
                                            style={{ width: '100%', padding: '0.5rem', border: '1px solid #d9d9d9', borderRadius: '4px' }}
                                            disabled={variantActionLoading}
                                        />
                                    </div>
                                    <div>
                                        <label style={{ display: 'block', marginBottom: '0.25rem', fontSize: '0.9rem' }}>Stock <span style={{ color: 'red' }}>*</span></label>
                                        <input
                                            type="number"
                                            min="0"
                                            value={variantForm.stock}
                                            onChange={e => setVariantForm({ ...variantForm, stock: parseInt(e.target.value) })}
                                            required
                                            style={{ width: '100%', padding: '0.5rem', border: '1px solid #d9d9d9', borderRadius: '4px' }}
                                            disabled={variantActionLoading}
                                        />
                                    </div>
                                </div>
                                <button
                                    type="submit"
                                    style={{ ...btnStyle, backgroundColor: editingVariant ? '#faad14' : '#52c41a', width: '100%', opacity: variantActionLoading ? 0.5 : 1 }}
                                    disabled={variantActionLoading}
                                >
                                    {variantActionLoading ? 'Processing...' : (editingVariant ? 'Update Variant' : 'Add Variant')}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Products;