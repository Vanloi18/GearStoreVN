import axiosClient from './axiosClient';

const authApi = {
    login: (email, password) => {
        return axiosClient.post('/auth/login', { email, password });
    },
    register: (data) => {
        // data: { firstName, lastName, email, password, confirmPassword }
        return axiosClient.post('/auth/register', data);
    }
};

export default authApi;
