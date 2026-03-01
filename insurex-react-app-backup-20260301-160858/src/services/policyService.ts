import api from './api';

export interface Policy {
    id: number;
    policyNumber: string;
    policyHolder: string;
    email: string;
    phone?: string;
    startDate: string;
    endDate: string;
    status: string;
    premium: number;
    policyType: string;
    partnerId?: number;
    notes?: string;
    assetCount: number;
    totalInsuredValue: number;
    createdAt: string;
}

export interface CreatePolicyDto {
    policyHolder: string;
    email: string;
    phone?: string;
    startDate: string;
    endDate: string;
    premium: number;
    policyType: string;
    partnerId?: number;
    notes?: string;
}

export interface UpdatePolicyDto {
    policyHolder?: string;
    email?: string;
    phone?: string;
    startDate?: string;
    endDate?: string;
    premium?: number;
    status?: string;
    policyType?: string;
    partnerId?: number;
    notes?: string;
}

export interface PolicyFilters {
    status?: string;
    policyType?: string;
    partnerId?: number;
    startDate?: string;
    endDate?: string;
}

export const policyService = {
    getAll: async (filters?: PolicyFilters): Promise<Policy[]> => {
        const params = new URLSearchParams();
        if (filters?.status) params.append('status', filters.status);
        if (filters?.policyType) params.append('policyType', filters.policyType);
        if (filters?.partnerId) params.append('partnerId', filters.partnerId.toString());
        if (filters?.startDate) params.append('startDate', filters.startDate);
        if (filters?.endDate) params.append('endDate', filters.endDate);
        
        const response = await api.get('/api/v1/Policies', { params });
        return response.data;
    },

    getById: async (id: number): Promise<Policy> => {
        const response = await api.get(`/api/v1/Policies/${id}`);
        return response.data;
    },

    create: async (data: CreatePolicyDto): Promise<Policy> => {
        const response = await api.post('/api/v1/Policies', data);
        return response.data;
    },

    update: async (id: number, data: UpdatePolicyDto): Promise<void> => {
        await api.put(`/api/v1/Policies/${id}`, data);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/api/v1/Policies/${id}`);
    },

    getAssets: async (id: number): Promise<any[]> => {
        const response = await api.get(`/api/v1/Policies/${id}/assets`);
        return response.data;
    },

    getClaims: async (id: number): Promise<any[]> => {
        const response = await api.get(`/api/v1/Policies/${id}/claims`);
        return response.data;
    },

    getTransactions: async (id: number): Promise<any[]> => {
        const response = await api.get(`/api/v1/Policies/${id}/transactions`);
        return response.data;
    }
};