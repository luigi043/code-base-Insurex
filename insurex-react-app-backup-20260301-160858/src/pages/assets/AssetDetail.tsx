import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Grid, Button } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Asset {
    id: number;
    policyId: number;
    assetType: string;
    financeValue: number;
    insuredValue: number;
    status: string;
    assetData: string;
}

const AssetDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [asset, setAsset] = useState<Asset | null>(null);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchAsset();
    }, [id]);

    const fetchAsset = async () => {
        try {
            const response = await api.get(/assets/);
            setAsset(response.data);
        } catch (error) {
            console.error('Error fetching asset:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <Typography>Loading...</Typography>;
    if (!asset) return <Typography>Asset not found</Typography>;

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Button onClick={() => navigate('/assets')} sx={{ mb: 2 }}>
                Back to Assets
            </Button>
            <Paper sx={{ p: 3 }}>
                <Typography variant="h4" gutterBottom>
                    Asset #{asset.id} - {asset.assetType}
                </Typography>
                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <Typography variant="h6">Asset Details</Typography>
                        <Typography>Policy ID: {asset.policyId}</Typography>
                        <Typography>Type: {asset.assetType}</Typography>
                        <Typography>Finance Value: R {asset.financeValue.toLocaleString()}</Typography>
                        <Typography>Insured Value: R {asset.insuredValue.toLocaleString()}</Typography>
                        <Typography>Status: {asset.status}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="h6">Asset Data</Typography>
                        <pre style={{ background: '#f5f5f5', padding: '10px', borderRadius: '4px' }}>
                            {JSON.stringify(JSON.parse(asset.assetData), null, 2)}
                        </pre>
                    </Grid>
                </Grid>
            </Paper>
        </Container>
    );
};

export default AssetDetail;
