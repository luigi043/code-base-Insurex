import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Grid, Button, Chip } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../../services/api';

interface Policy {
    id: number;
    policyNumber: string;
    customerName: string;
    customerEmail: string;
    startDate: string;
    endDate: string;
    status: string;
    totalInsuredValue: number;
}

const PolicyDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [policy, setPolicy] = useState<Policy | null>(null);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchPolicy();
    }, [id]);

    const fetchPolicy = async () => {
        try {
            const response = await api.get(/policies/);
            setPolicy(response.data);
        } catch (error) {
            console.error('Error fetching policy:', error);
        } finally {
            setLoading(false);
        }
    };

    const getStatusColor = (status: string) => {
        switch(status) {
            case 'Active': return 'success';
            case 'Pending': return 'warning';
            case 'Suspended': return 'error';
            case 'Cancelled': return 'default';
            default: return 'default';
        }
    };

    if (loading) return <Typography>Loading...</Typography>;
    if (!policy) return <Typography>Policy not found</Typography>;

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Button onClick={() => navigate('/policies')} sx={{ mb: 2 }}>
                Back to Policies
            </Button>
            <Paper sx={{ p: 3 }}>
                <Typography variant="h4" gutterBottom>
                    Policy {policy.policyNumber}
                    <Chip 
                        label={policy.status} 
                        color={getStatusColor(policy.status) as any}
                        size="small"
                        sx={{ ml: 2 }}
                    />
                </Typography>
                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <Typography variant="h6">Customer Information</Typography>
                        <Typography>Name: {policy.customerName}</Typography>
                        <Typography>Email: {policy.customerEmail}</Typography>
                    </Grid>
                    <Grid item xs={12} md={6}>
                        <Typography variant="h6">Policy Details</Typography>
                        <Typography>Start Date: {new Date(policy.startDate).toLocaleDateString()}</Typography>
                        <Typography>End Date: {new Date(policy.endDate).toLocaleDateString()}</Typography>
                        <Typography>Total Insured Value: R {policy.totalInsuredValue.toLocaleString()}</Typography>
                    </Grid>
                </Grid>
            </Paper>
        </Container>
    );
};

export default PolicyDetail;
