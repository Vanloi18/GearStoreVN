import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import authApi from '../../api/authApi';

const parseJwt = (token) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            window
                .atob(base64)
                .split('')
                .map(function (c) {
                    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                })
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch (error) {
        return null;
    }
};

const Login = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const response = await authApi.login(email, password);
            const token = response.token || response.data?.token;

            if (!token) {
                setError('Invalid response from server');
                setLoading(false);
                return;
            }

            const decoded = parseJwt(token);
            if (!decoded) {
                setError('Invalid token received');
                setLoading(false);
                return;
            }

            const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;
            const isAdmin = Array.isArray(role) ? role.includes('Admin') : role === 'Admin';

            if (!isAdmin) {
                setError('Access denied. Admin role required.');
                setLoading(false);
                return;
            }

            localStorage.setItem('access_token', token);
            navigate('/admin/dashboard');
        } catch (err) {
            setError(err.response?.data?.message || 'Invalid email or password');
            setLoading(false);
        }
    };

    return (
        <div style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            height: '100vh',
            backgroundColor: '#f0f2f5',
            fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif'
        }}>
            <form onSubmit={handleSubmit} style={{
                padding: '2.5rem',
                background: 'white',
                borderRadius: '8px',
                boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
                width: '100%',
                maxWidth: '400px'
            }}>
                <h2 style={{
                    textAlign: 'center',
                    marginBottom: '2rem',
                    color: '#333'
                }}>Admin Login</h2>

                {error && <div style={{
                    color: '#ff4d4f',
                    marginBottom: '1rem',
                    textAlign: 'center',
                    backgroundColor: '#fff1f0',
                    border: '1px solid #ffccc7',
                    padding: '0.5rem',
                    borderRadius: '4px'
                }}>{error}</div>}

                <div style={{ marginBottom: '1.5rem' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '0.5rem',
                        color: '#555',
                        fontWeight: '500'
                    }}>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        disabled={loading}
                        placeholder="admin@example.com"
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            border: '1px solid #d9d9d9',
                            borderRadius: '4px',
                            fontSize: '1rem',
                            outline: 'none',
                            transition: 'border-color 0.3s'
                        }}
                    />
                </div>

                <div style={{ marginBottom: '2rem' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '0.5rem',
                        color: '#555',
                        fontWeight: '500'
                    }}>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        disabled={loading}
                        placeholder="••••••••"
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            border: '1px solid #d9d9d9',
                            borderRadius: '4px',
                            fontSize: '1rem',
                            outline: 'none',
                            transition: 'border-color 0.3s'
                        }}
                    />
                </div>

                <button type="submit" disabled={loading} style={{
                    width: '100%',
                    padding: '0.75rem',
                    backgroundColor: loading ? '#91d5ff' : '#1890ff',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: loading ? 'not-allowed' : 'pointer',
                    fontSize: '1rem',
                    fontWeight: '500',
                    transition: 'background-color 0.3s'
                }}>
                    {loading ? 'Logging in...' : 'Login'}
                </button>
            </form>
        </div>
    );
};

export default Login;
