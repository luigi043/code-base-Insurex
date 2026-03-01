import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button, Chip } from '@mui/material';
import { useNavigate } from 'react-router-dom';
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

const ClaimList: React.FC = () => {
    const [claims, setClaims] = useState<Claim[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchClaims();
    }, []);

    const fetchClaims = async () => {
        try {
            const response = await api.get('/claims');
            setClaims(response.data);
        } catch (error) {
            console.error('Error fetching claims:', error);
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

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>Claims</Typography>
            <Button 
                variant="contained" 
                color="primary" 
                onClick={() => navigate('/claims/create')}
                sx={{ mb: 2 }}
            >
                Create New Claim
            </Button>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Claim Number</TableCell>
                            <TableCell>Policy ID</TableCell>
                            <TableCell>Date of Loss</TableCell>
                            <TableCell>Amount</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {claims.map((claim) => (
                            <TableRow key={claim.id}>
                                <TableCell>{claim.claimNumber}</TableCell>
                                <TableCell>{claim.policyId}</TableCell>
                                <TableCell>{new Date(claim.dateOfLoss).toLocaleDateString()}</TableCell>
                                <TableCell>R {claim.claimAmount.toLocaleString()}</TableCell>
                                <TableCell>
                                    <Chip 
                                        label={claim.status} 
                                        color={getStatusColor(claim.status) as any}
                                        size="small"
                                    />
                                </TableCell>
                                <TableCell>
                                    <Button 
                                        size="small" 
                                        onClick={() => navigate(/claims/)}
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

export default ClaimList;
