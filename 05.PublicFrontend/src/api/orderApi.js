import axiosClient from './axiosClient';

const orderApi = {
    createOrder: (orderData) => {
        // payload: { customerName, customerPhone, shippingAddress, items: [...], ... }
        return axiosClient.post('/orders/direct', orderData);
    },
    getMyOrders: () => {
        return axiosClient.get('/orders/my');
    }
};

export default orderApi;
