import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import authApi from '../api/authApi';

const Register = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: ''
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (formData.password !== formData.confirmPassword) {
            return setError("Passwords don't match");
        }

        setError('');
        setLoading(true);
        try {
            await authApi.register({
                firstName: formData.firstName,
                lastName: formData.lastName,
                email: formData.email,
                password: formData.password,
                confirmPassword: formData.confirmPassword
            });
            // Redirect to login on success
            navigate('/login');
        } catch (err) {
            setError(err.response?.data?.message || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '500px', margin: '4rem auto' }}>
            <div className="card" style={{ padding: '2.5rem', boxShadow: '0 10px 25px rgba(0,0,0,0.05)' }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <h2 style={{ fontSize: '1.5rem', fontWeight: 800, color: '#333', textTransform: 'uppercase', letterSpacing: '1px' }}>
                        TẠO TÀI KHOẢN <span style={{ color: 'var(--primary-color)' }}>GEARSTORE</span>
                    </h2>
                    <p style={{ fontSize: '0.85rem', color: '#666', marginTop: '0.5rem' }}>Đăng ký để nhận nhiều ưu đãi hấp dẫn</p>
                </div>

                {error && (
                    <div style={{
                        backgroundColor: '#FFF1F0',
                        color: 'var(--primary-color)',
                        padding: '0.75rem',
                        borderRadius: '4px',
                        marginBottom: '1.5rem',
                        textAlign: 'center',
                        fontSize: '0.85rem',
                        border: '1px solid #FFA39E',
                        fontWeight: 600
                    }}>
                        ⚠️ {error}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                        <div className="form-group">
                            <label className="form-label">HỌ <span style={{ color: 'red' }}>*</span></label>
                            <input
                                type="text"
                                name="firstName"
                                className="form-input"
                                placeholder="Nguyễn"
                                value={formData.firstName}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label className="form-label">TÊN <span style={{ color: 'red' }}>*</span></label>
                            <input
                                type="text"
                                name="lastName"
                                className="form-input"
                                placeholder="Vân Anh"
                                value={formData.lastName}
                                onChange={handleChange}
                                required
                            />
                        </div>
                    </div>

                    <div className="form-group">
                        <label className="form-label">EMAIL ĐĂNG KÝ <span style={{ color: 'red' }}>*</span></label>
                        <input
                            type="email"
                            name="email"
                            className="form-input"
                            placeholder="example@gmail.com"
                            value={formData.email}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">MẬT KHẨU <span style={{ color: 'red' }}>*</span></label>
                        <input
                            type="password"
                            name="password"
                            className="form-input"
                            placeholder="Ít nhất 6 ký tự"
                            value={formData.password}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label">XÁC NHẬN MẬT KHẨU <span style={{ color: 'red' }}>*</span></label>
                        <input
                            type="password"
                            name="confirmPassword"
                            className="form-input"
                            placeholder="Nhập lại mật khẩu"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ width: '100%', padding: '1rem', fontWeight: 800, marginTop: '1rem', borderRadius: '4px' }}
                        disabled={loading}
                    >
                        {loading ? 'ĐANG ĐĂNG KÝ...' : 'ĐĂNG KÝ TÀI KHOẢN'}
                    </button>
                </form>

                <div style={{ position: 'relative', margin: '2rem 0', textAlign: 'center' }}>
                    <hr style={{ border: 'none', borderTop: '1px solid #EEE' }} />
                    <span style={{ position: 'absolute', top: '-10px', left: '50%', transform: 'translateX(-50%)', background: '#FFF', padding: '0 10px', fontSize: '0.75rem', color: '#999' }}>HOẶC</span>
                </div>

                <p style={{ textAlign: 'center', color: '#666', fontSize: '0.9rem' }}>
                    Đã có tài khoản? <Link to="/login" style={{ color: 'var(--primary-color)', fontWeight: 700 }}>Đăng nhập ngay</Link>
                </p>
            </div>
        </div>
    );
};

export default Register;
