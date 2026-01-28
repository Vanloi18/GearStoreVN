import axiosClient from './axiosClient';

const productApi = {
    getAll: () => {
        return axiosClient.get('/products');
    },
    getById: (id) => {
        return axiosClient.get(`/products/${id}`);
    }
};

export default productApi;
