import axios from 'axios';
import { isTokenExpired } from '../utils/jwt';

const axiosClient = axios.create({
  baseURL: 'http://localhost:5122/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Helper to handle forced logout
const handleLogout = () => {
  localStorage.removeItem('access_token');
  // Basic check to avoid redirect loops if already on login
  if (!window.location.pathname.includes('/login')) {
    window.location.href = '/admin/login';
  }
};

axiosClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('access_token');

    // 1. Check if token exists
    if (token) {
      // 2. Check if token is expired BEFORE request
      if (isTokenExpired(token)) {
        // Token expired - abort request and redirect
        handleLogout();

        // Cancel the request
        const controller = new AbortController();
        config.signal = controller.signal;
        controller.abort("Token expired");

        return Promise.reject(new Error("Token expired"));
      }

      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axiosClient.interceptors.response.use(
  (response) => {
    /*
      Unwrap logic: 
      Backend returns { success: bool, message: string, data: T }
      We want to return T if possible, or the whole object if structure varies.
      
      Safe check: if response.data exists and has a 'data' property, return that.
      Otherwise return response.data.
    */
    const resData = response.data;
    if (resData && typeof resData === 'object' && 'data' in resData) {
      return resData.data;
    }
    return resData;
  },
  (error) => {
    const { response } = error;

    // Handle 401 Unauthorized / 403 Forbidden
    if (response && (response.status === 401 || response.status === 403)) {
      handleLogout();
    }

    // Improve error message
    let errorMessage = 'An error occurred';
    if (response && response.data) {
      // Try to extract message from backend wrapper
      errorMessage = response.data.message || response.data.title || JSON.stringify(response.data);
    } else if (error.message) {
      errorMessage = error.message;
    }

    const enhancedError = new Error(errorMessage);
    enhancedError.originalError = error;
    enhancedError.status = response ? response.status : null;

    return Promise.reject(enhancedError);
  }
);

export default axiosClient;
