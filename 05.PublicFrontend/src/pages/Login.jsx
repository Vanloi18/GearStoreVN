import React, { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import authApi from '../api/authApi';

const Login = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const from = location.state?.from?.pathname || '/';

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const response = await authApi.login(email, password);
            // Assuming response contains { token: '...' }
            if (response.token) {
                localStorage.setItem('access_token', response.token);
                // Dispatch storage event manually for MainLayout to pick up (if in same tab)
                window.dispatchEvent(new Event('storage'));
                navigate(from, { replace: true });
            } else {
                setError('Login failed. Please try again.');
            }
        } catch (err) {
            setError(err.response?.data?.message || 'Invalid email or password');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '450px', margin: '4rem auto' }}>
            <div className="card" style={{ padding: '2.5rem', boxShadow: '0 10px 25px rgba(0,0,0,0.05)' }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <h2 style={{ fontSize: '1.5rem', fontWeight: 800, color: '#333', textTransform: 'uppercase', letterSpacing: '1px' }}>
                        ĐĂNG NHẬP <span style={{ color: 'var(--primary-color)' }}>GEARSTORE</span>
                    </h2>
                    <p style={{ fontSize: '0.85rem', color: '#666', marginTop: '0.5rem' }}>Bắt đầu trải nghiệm gaming đỉnh cao</p>
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
                    <div className="form-group">
                        <label className="form-label">EMAIL ĐĂNG NHẬP</label>
                        <input
                            type="email"
                            className="form-input"
                            placeholder="example@gmail.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                            style={{ padding: '0.8rem' }}
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label" style={{ display: 'flex', justifyContent: 'space-between' }}>
                            MẬT KHẨU
                            <span style={{ fontSize: '0.75rem', color: 'var(--primary-color)', cursor: 'pointer' }}>Quên mật khẩu?</span>
                        </label>
                        <input
                            type="password"
                            className="form-input"
                            placeholder="********"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            style={{ padding: '0.8rem' }}
                        />
                    </div>
                    <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ width: '100%', padding: '1rem', fontWeight: 800, marginTop: '1rem', borderRadius: '4px' }}
                        disabled={loading}
                    >
                        {loading ? 'ĐANG XỬ LÝ...' : 'ĐĂNG NHẬP NGAY'}
                    </button>
                </form>

                <div style={{ position: 'relative', margin: '2rem 0', textAlign: 'center' }}>
                    <hr style={{ border: 'none', borderTop: '1px solid #EEE' }} />
                    <span style={{ position: 'absolute', top: '-10px', left: '50%', transform: 'translateX(-50%)', background: '#FFF', padding: '0 10px', fontSize: '0.75rem', color: '#999' }}>HOẶC</span>
                </div>

                <p style={{ textAlign: 'center', color: '#666', fontSize: '0.9rem' }}>
                    Chưa có tài khoản? <Link to="/register" style={{ color: 'var(--primary-color)', fontWeight: 700 }}>Đăng ký tại đây</Link>
                </p>
            </div>
            <div style={{ textAlign: 'center', marginTop: '2rem', fontSize: '0.8rem', color: '#999' }}>
                Bằng việc đăng nhập, bạn đồng ý với Điều khoản và Chính sách của GearStore
            </div>
        </div>
    );
};

export default Login;
