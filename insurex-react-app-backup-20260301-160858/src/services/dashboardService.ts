import api from './api';

export interface DashboardStats {
    totalPolicies: number;
    activePolicies: number;
    totalAssets: number;
    totalInsuredValue: number;
    expiringSoon: number;
    recentPolicies: Array<{
        id: number;
        policyNumber: string;
        policyHolder: string;
        status: string;
        createdAt: string;
    }>;
}

export const dashboardService = {
    getStats: async (): Promise<DashboardStats> => {
        const response = await api.get('/api/v1/Dashboard/stats');
        return response.data;
    },

    getPoliciesByStatus: async (): Promise<Array<{ status: string; count: number }>> => {
        const response = await api.get('/api/v1/Dashboard/policies-by-status');
        return response.data;
    },

    getAssetsByType: async (): Promise<Array<{ type: string; count: number }>> => {
        const response = await api.get('/api/v1/Dashboard/assets-by-type');
        return response.data;
    }
};
