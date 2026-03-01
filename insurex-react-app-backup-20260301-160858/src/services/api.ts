import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:5012/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Add token to requests if it exists
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = Bearer ;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;
