import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../../services/api';

interface Asset {
    id: number;
    assetType: string;
    financeValue: number;
    insuredValue: number;
    status: string;
    policyId: number;
}

const AssetList: React.FC = () => {
    const [assets, setAssets] = useState<Asset[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchAssets();
    }, []);

    const fetchAssets = async () => {
        try {
            const response = await api.get('/assets');
            setAssets(response.data);
        } catch (error) {
            console.error('Error fetching assets:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <Typography>Loading...</Typography>;

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>Assets</Typography>
            <Button 
                variant="contained" 
                color="primary" 
                onClick={() => navigate('/assets/create')}
                sx={{ mb: 2 }}
            >
                Create New Asset
            </Button>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>ID</TableCell>
                            <TableCell>Type</TableCell>
                            <TableCell>Finance Value</TableCell>
                            <TableCell>Insured Value</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Policy ID</TableCell>
                            <TableCell>Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {assets.map((asset) => (
                            <TableRow key={asset.id}>
                                <TableCell>{asset.id}</TableCell>
                                <TableCell>{asset.assetType}</TableCell>
                                <TableCell>R {asset.financeValue.toLocaleString()}</TableCell>
                                <TableCell>R {asset.insuredValue.toLocaleString()}</TableCell>
                                <TableCell>{asset.status}</TableCell>
                                <TableCell>{asset.policyId}</TableCell>
                                <TableCell>
                                    <Button 
                                        size="small" 
                                        onClick={() => navigate(/assets/)}
                                    >
                                        View
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Container>
    );
};

export default AssetList;
