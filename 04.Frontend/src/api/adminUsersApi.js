import axiosClient from './axiosClient';

const adminUsersApi = {
    getAll: () => {
        return axiosClient.get('/admin/users');
    },
    updateRole: (id, role) => {
        return axiosClient.put(`/admin/users/${id}/role`, null, { params: { role } });
    },
};

export default adminUsersApi;
