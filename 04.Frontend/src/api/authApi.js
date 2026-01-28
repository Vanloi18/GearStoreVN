import axiosClient from './axiosClient';

const authApi = {
    login: (email, password) => {
        return axiosClient.post('/auth/login', { email, password });
    },
};

export default authApi;
