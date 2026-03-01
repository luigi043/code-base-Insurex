import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button, Chip } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../../services/api';

interface Policy {
    id: number;
    policyNumber: string;
    customerName: string;
    startDate: string;
    endDate: string;
    status: string;
    totalInsuredValue: number;
}

const PolicyList: React.FC = () => {
    const [policies, setPolicies] = useState<Policy[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        fetchPolicies();
    }, []);

    const fetchPolicies = async () => {
        try {
            const response = await api.get('/policies');
            setPolicies(response.data);
        } catch (error) {
            console.error('Error fetching policies:', error);
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

    return (
        <Container maxWidth="lg">
            <Typography variant="h4" gutterBottom>Policies</Typography>
            <Button 
                variant="contained" 
                color="primary" 
                onClick={() => navigate('/policies/create')}
                sx={{ mb: 2 }}
            >
                Create New Policy
            </Button>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Policy Number</TableCell>
                            <TableCell>Customer</TableCell>
                            <TableCell>Start Date</TableCell>
                            <TableCell>End Date</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Insured Value</TableCell>
                            <TableCell>Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {policies.map((policy) => (
                            <TableRow key={policy.id}>
                                <TableCell>{policy.policyNumber}</TableCell>
                                <TableCell>{policy.customerName}</TableCell>
                                <TableCell>{new Date(policy.startDate).toLocaleDateString()}</TableCell>
                                <TableCell>{new Date(policy.endDate).toLocaleDateString()}</TableCell>
                                <TableCell>
                                    <Chip 
                                        label={policy.status} 
                                        color={getStatusColor(policy.status) as any}
                                        size="small"
                                    />
                                </TableCell>
                                <TableCell>R {policy.totalInsuredValue.toLocaleString()}</TableCell>
                                <TableCell>
                                    <Button 
                                        size="small" 
                                        onClick={() => navigate(/policies/)}
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

export default PolicyList;
