import React, { useEffect, useState } from 'react';
import adminUsersApi from '../../api/adminUsersApi';

const Users = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const [error, setError] = useState('');

    const fetchUsers = async () => {
        setLoading(true);
        try {
            // Axios interceptor already unwraps ApiResponse<T>.data
            const data = await adminUsersApi.getAll();
            const items = data.items || (Array.isArray(data) ? data : []);
            setUsers(items);
            setError('');
        } catch (err) {
            setError('Failed to fetch users');
            console.error('Users error:', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const handleToggleRole = async (user) => {
        if (updating) return;

        const isCurrentlyAdmin = Array.isArray(user.roles)
            ? user.roles.includes('Admin')
            : user.role === 'Admin';

        const newRole = isCurrentlyAdmin ? 'User' : 'Admin';

        if (!window.confirm(`Are you sure you want to change role to ${newRole}?`)) return;

        setUpdating(true);
        try {
            await adminUsersApi.updateRole(user.id, newRole);
            await fetchUsers();
            alert(`User role updated to ${newRole}`);
        } catch (err) {
            alert('Failed to update role');
            console.error(err);
        } finally {
            setUpdating(false);
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
        backgroundColor: '#722ed1'
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: '#ff4d4f' }}>{error}</div>;

    return (
        <div>
            <h1 style={{ marginBottom: '2rem' }}>Users Management</h1>
            <div style={{ overflowX: 'auto' }}>
                <table style={tableStyle}>
                    <thead>
                        <tr>
                            <th style={thStyle}>Name</th>
                            <th style={thStyle}>Email</th>
                            <th style={thStyle}>Role</th>
                            <th style={thStyle}>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.length === 0 ? (
                            <tr>
                                <td colSpan="4" style={{ ...tdStyle, textAlign: 'center', color: '#999' }}>No users found</td>
                            </tr>
                        ) : (
                            users.map(user => (
                                <tr key={user.id}>
                                    <td style={tdStyle}>{user.firstName} {user.lastName}</td>
                                    <td style={tdStyle}>{user.email}</td>
                                    <td style={tdStyle}>
                                        {Array.isArray(user.roles) ? user.roles.join(', ') : user.role}
                                    </td>
                                    <td style={tdStyle}>
                                        <button
                                            onClick={() => handleToggleRole(user)}
                                            style={btnStyle}
                                            disabled={updating}
                                        >
                                            Toggle Role
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default Users;
