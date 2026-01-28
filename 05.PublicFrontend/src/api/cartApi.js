import axiosClient from './axiosClient';

const cartApi = {
    getCart: () => {
        return axiosClient.get('/cart');
    },
    addItem: (productId, quantity) => {
        return axiosClient.post('/cart/items', { productId, quantity });
    },
    updateItem: (itemId, quantity) => {
        // Assuming PUT /api/cart/items/{id} takes { quantity }
        return axiosClient.put(`/cart/items/${itemId}`, { quantity });
    },
    removeItem: (itemId) => {
        return axiosClient.delete(`/cart/items/${itemId}`);
    }
};

export default cartApi;
