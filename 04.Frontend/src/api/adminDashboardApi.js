import axiosClient from './axiosClient';

const adminDashboardApi = {
    getStats: () => {
        return axiosClient.get('/admin/dashboard');
    },
};

export default adminDashboardApi;
