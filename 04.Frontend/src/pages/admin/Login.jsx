import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authApi from '../../api/authApi';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            // Axios interceptor unwraps ApiResponse<T>.data automatically
            // Response is AuthResponseDto: { userId, email, token, roles }
            const response = await authApi.login(email, password);

            if (response && response.token) {
                localStorage.setItem('access_token', response.token);
                navigate('/admin');
            } else {
                setError('Login failed: Invalid response from server');
                console.error('Unexpected response structure:', response);
            }
        } catch (err) {
            console.error('Login error:', err);
            // Error interceptor extracts message from ApiResponse<T>
            setError(err.message || 'Invalid email or password');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif',
            padding: '1rem'
        }}>
            <form onSubmit={handleSubmit} style={{
                padding: '2.5rem',
                background: 'white',
                borderRadius: '12px',
                boxShadow: '0 20px 60px rgba(0,0,0,0.3)',
                width: '100%',
                maxWidth: '420px'
            }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <h2 style={{
                        margin: '0 0 0.5rem 0',
                        color: '#1a202c',
                        fontSize: '1.75rem',
                        fontWeight: '700'
                    }}>GearStore Admin</h2>
                    <p style={{
                        margin: 0,
                        color: '#718096',
                        fontSize: '0.875rem'
                    }}>Sign in to manage your store</p>
                </div>

                {error && (
                    <div style={{
                        color: '#c53030',
                        marginBottom: '1.5rem',
                        backgroundColor: '#fff5f5',
                        border: '1px solid #feb2b2',
                        padding: '0.75rem',
                        borderRadius: '6px',
                        fontSize: '0.875rem',
                        textAlign: 'center'
                    }}>
                        {error}
                    </div>
                )}

                <div style={{ marginBottom: '1.5rem' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '0.5rem',
                        color: '#2d3748',
                        fontWeight: '600',
                        fontSize: '0.875rem'
                    }}>Email Address</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        placeholder="admin@gearstore.com"
                        disabled={loading}
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            border: '2px solid #e2e8f0',
                            borderRadius: '6px',
                            fontSize: '1rem',
                            outline: 'none',
                            transition: 'all 0.2s',
                            backgroundColor: loading ? '#f7fafc' : 'white',
                            boxSizing: 'border-box'
                        }}
                        onFocus={(e) => e.target.style.borderColor = '#667eea'}
                        onBlur={(e) => e.target.style.borderColor = '#e2e8f0'}
                    />
                </div>

                <div style={{ marginBottom: '2rem' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '0.5rem',
                        color: '#2d3748',
                        fontWeight: '600',
                        fontSize: '0.875rem'
                    }}>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        placeholder="Enter your password"
                        disabled={loading}
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            border: '2px solid #e2e8f0',
                            borderRadius: '6px',
                            fontSize: '1rem',
                            outline: 'none',
                            transition: 'all 0.2s',
                            backgroundColor: loading ? '#f7fafc' : 'white',
                            boxSizing: 'border-box'
                        }}
                        onFocus={(e) => e.target.style.borderColor = '#667eea'}
                        onBlur={(e) => e.target.style.borderColor = '#e2e8f0'}
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    style={{
                        width: '100%',
                        padding: '0.875rem',
                        background: loading ? '#a0aec0' : 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        color: 'white',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: loading ? 'not-allowed' : 'pointer',
                        fontSize: '1rem',
                        fontWeight: '600',
                        transition: 'all 0.3s',
                        boxShadow: loading ? 'none' : '0 4px 12px rgba(102, 126, 234, 0.4)'
                    }}
                    onMouseOver={(e) => {
                        if (!loading) {
                            e.target.style.transform = 'translateY(-2px)';
                            e.target.style.boxShadow = '0 6px 16px rgba(102, 126, 234, 0.5)';
                        }
                    }}
                    onMouseOut={(e) => {
                        if (!loading) {
                            e.target.style.transform = 'translateY(0)';
                            e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.4)';
                        }
                    }}
                >
                    {loading ? 'Signing in...' : 'Sign In'}
                </button>

                <div style={{
                    marginTop: '1.5rem',
                    textAlign: 'center',
                    fontSize: '0.75rem',
                    color: '#a0aec0'
                }}>
                    Default: admin@gearstore.com / Admin@123
                </div>
            </form>
        </div>
    );
};

export default Login;
