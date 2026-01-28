import axiosClient from './axiosClient';

const adminOrdersApi = {
    getAll: () => {
        return axiosClient.get('/admin/orders');
    },
    updateStatus: (id, status) => {
        return axiosClient.put(`/admin/orders/${id}/status`, { status });
    },
};

export default adminOrdersApi;
