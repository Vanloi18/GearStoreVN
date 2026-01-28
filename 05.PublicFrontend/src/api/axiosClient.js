import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'http://localhost:5122/api', // Confirmed port from admin requirement, though previous file said 5001. User req says 5122.
    headers: {
        'Content-Type': 'application/json',
    },
});

axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => {
        // Unwrap logic: backend usually returns ApiResponse { success: bool, data: T, message: string }
        const resData = response.data;
        if (resData && typeof resData === 'object' && 'data' in resData) {
            return resData.data;
        }
        return resData;
    },
    (error) => {
        if (error.response && error.response.status === 401) {
            localStorage.removeItem('access_token');
            // Optional: Redirect to login or dispatch event
            // window.location.href = '/login'; // Can be annoying if public pages share client
        }
        return Promise.reject(error);
    }
);

export default axiosClient;
