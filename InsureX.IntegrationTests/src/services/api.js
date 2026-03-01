import axios from 'axios';

const API_URL = 'http://localhost:5012/api';

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
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Auth endpoints
export const login = (email, password) => 
  api.post('/auth/login', { email, password });

export const register = (userData) => 
  api.post('/auth/register', userData);

// Policy endpoints
export const getPolicies = () => api.get('/policies');
export const getPolicy = (id) => api.get(`/policies/${id}`);
export const createPolicy = (policy) => api.post('/policies', policy);
export const updatePolicy = (id, policy) => api.put(`/policies/${id}`, policy);
export const deletePolicy = (id) => api.delete(`/policies/${id}`);

// Asset endpoints
export const getAssets = (params) => api.get('/assets', { params });
export const getAsset = (id) => api.get(`/assets/${id}`);
export const createAsset = (asset) => api.post('/assets', asset);
export const getAssetsByPolicy = (policyId) => api.get(`/assets/policy/${policyId}`);

// Claim endpoints
export const getClaims = () => api.get('/claims');
export const getClaim = (id) => api.get(`/claims/${id}`);
export const createClaim = (claim) => api.post('/claims', claim);
export const updateClaim = (id, claim) => api.put(`/claims/${id}`, claim);
export const deleteClaim = (id) => api.delete(`/claims/${id}`);
export const getClaimsByPolicy = (policyId) => api.get(`/claims/policy/${policyId}`);

export default api;
