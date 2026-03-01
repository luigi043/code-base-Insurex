import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Grid, Button, Chip } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Claim {
    id: number;
    claimNumber: string;
    policyId: number;
    dateOfLoss: string;
    claimAmount: number;
    status: string;
    description: string;
}

const ClaimDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [claim, setClaim] = useState<Claim | null>(null);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchClaim();
    }, [id]);

    const fetchClaim = async () => {
        try {
            const response = await api.get(/claims/);
            setClaim(response.data);
        } catch (error) {
            console.error('Error fetching claim:', error);
        } finally {
            setLoading(false);
        }
    };

    const getStatusColor = (status: string) => {
        switch(status) {
            case 'Approved': return 'success';
            case 'Submitted': return 'info';
            case 'In Review': return 'warning';
            case 'Rejected': return 'error';
            default: return 'default';
        }
    };

    if (loading) return <Typography>Loading...</Typography>;
    if (!claim) return <Typography>Claim not found</Typography>;

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Button onClick={() => navigate('/claims')} sx={{ mb: 2 }}>
                Back to Claims
            </Button>
            <Paper sx={{ p: 3 }}>
                <Typography variant="h4" gutterBottom>
                    Claim {claim.claimNumber}
                    <Chip 
                        label={claim.status} 
                        color={getStatusColor(claim.status) as any}
                        size="small"
                        sx={{ ml: 2 }}
                    />
                </Typography>
                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <Typography variant="h6">Claim Details</Typography>
                        <Typography>Policy ID: {claim.policyId}</Typography>
                        <Typography>Date of Loss: {new Date(claim.dateOfLoss).toLocaleDateString()}</Typography>
                        <Typography>Claim Amount: R {claim.claimAmount.toLocaleString()}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="h6">Description</Typography>
                        <Typography>{claim.description}</Typography>
                    </Grid>
                </Grid>
                <Button 
                    variant="contained" 
                    color="primary" 
                    sx={{ mt: 2 }}
                    onClick={() => navigate(/claims//edit)}
                >
                    Edit Claim
                </Button>
            </Paper>
        </Container>
    );
};

export default ClaimDetail;
