import axios from 'axios';

const API_URL = 'http://localhost:5012/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
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

export const billingService = {
  // Get all invoices with optional filters
  getInvoices: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.status) params.append('status', filters.status);
    if (filters.policyId) params.append('policyId', filters.policyId);
    if (filters.fromDate) params.append('fromDate', filters.fromDate);
    if (filters.toDate) params.append('toDate', filters.toDate);
    
    const response = await api.get(`/billing/invoices?${params}`);
    return response.data;
  },

  // Get invoice by ID
  getInvoiceById: async (id) => {
    const response = await api.get(`/billing/invoices/${id}`);
    return response.data;
  },

  // Create new invoice
  createInvoice: async (invoiceData) => {
    const response = await api.post('/billing/invoices', invoiceData);
    return response.data;
  },

  // Send invoice
  sendInvoice: async (id) => {
    const response = await api.post(`/billing/invoices/${id}/send`);
    return response.data;
  },

  // Add payment to invoice
  addPayment: async (id, paymentData) => {
    const response = await api.post(`/billing/invoices/${id}/payments`, paymentData);
    return response.data;
  },

  // Get billing dashboard data
  getDashboard: async () => {
    const response = await api.get('/billing/dashboard');
    return response.data;
  },

  // Get invoice PDF
  getInvoicePdf: async (id) => {
    const response = await api.get(`/billing/invoices/${id}/pdf`, {
      responseType: 'blob'
    });
    return response.data;
  }
};
