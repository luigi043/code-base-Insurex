import React, { useEffect, useState } from 'react';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button } from '@mui/material';
import { policyService, Policy } from '../services/policyService';
import { useNavigate } from 'react-router-dom';

const PoliciesPage: React.FC = () => {
    const [policies, setPolicies] = useState<Policy[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        policyService.getAll().then(setPolicies);
    }, []);

    return (
        <Container maxWidth="lg">
            <Typography variant="h4" gutterBottom>
                Apólices
            </Typography>
            <Button variant="contained" onClick={() => navigate('/policies/new')} sx={{ mb: 2 }}>
                Nova Apólice
            </Button>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Número</TableCell>
                            <TableCell>Segurado</TableCell>
                            <TableCell>Email</TableCell>
                            <TableCell>Status</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {policies.map((policy) => (
                            <TableRow key={policy.id}>
                                <TableCell>{policy.policyNumber}</TableCell>
                                <TableCell>{policy.policyHolder}</TableCell>
                                <TableCell>{policy.email}</TableCell>
                                <TableCell>{policy.status}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Container>
    );
};

export default PoliciesPage;
