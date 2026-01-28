import axiosClient from './axiosClient';

const adminProductsApi = {
    getAll: (params) => {
        return axiosClient.get('/products', { params });
    },
    getById: (id) => {
        return axiosClient.get(`/products/${id}`);
    },
    create: (data) => {
        return axiosClient.post('/products', data);
    },
    update: (id, data) => {
        return axiosClient.put(`/products/${id}`, data);
    },
    delete: (id) => {
        return axiosClient.delete(`/products/${id}`);
    },

    // Variant Management
    addVariant: (productId, data) => {
        return axiosClient.post(`/admin/products/${productId}/variants`, data);
    },
    updateVariant: (productId, variantId, data) => {
        return axiosClient.put(`/admin/products/${productId}/variants/${variantId}`, data);
    },
    deleteVariant: (productId, variantId) => {
        return axiosClient.delete(`/admin/products/${productId}/variants/${variantId}`);
    },
};

export default adminProductsApi;