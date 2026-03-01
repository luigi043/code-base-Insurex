import React, { useState, useEffect } from 'react';
import { Container, Typography, Paper, TextField, Button, Grid, MenuItem } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Policy {
    id: number;
    policyNumber: string;
}

const ClaimCreate: React.FC = () => {
    const navigate = useNavigate();
    const [policies, setPolicies] = useState<Policy[]>([]);
    const [formData, setFormData] = useState({
        policyId: '',
        claimNumber: '',
        dateOfLoss: '',
        claimAmount: '',
        status: 'Submitted',
        description: ''
    });

    useEffect(() => {
        fetchPolicies();
    }, []);

    const fetchPolicies = async () => {
        try {
            const response = await api.get('/policies');
            setPolicies(response.data);
        } catch (error) {
            console.error('Error fetching policies:', error);
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await api.post('/claims', formData);
            navigate('/claims');
        } catch (error) {
            console.error('Error creating claim:', error);
        }
    };

    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>Create New Claim</Typography>
            <Paper sx={{ p: 3 }}>
                <form onSubmit={handleSubmit}>
                    <Grid container spacing={3}>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                select
                                label="Policy"
                                name="policyId"
                                value={formData.policyId}
                                onChange={handleChange}
                                required
                            >
                                {policies.map((policy) => (
                                    <MenuItem key={policy.id} value={policy.id}>
                                        {policy.policyNumber}
                                    </MenuItem>
                                ))}
                            </TextField>
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Claim Number"
                                name="claimNumber"
                                value={formData.claimNumber}
                                onChange={handleChange}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Date of Loss"
                                name="dateOfLoss"
                                type="date"
                                value={formData.dateOfLoss}
                                onChange={handleChange}
                                InputLabelProps={{ shrink: true }}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Claim Amount"
                                name="claimAmount"
                                type="number"
                                value={formData.claimAmount}
                                onChange={handleChange}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                select
                                label="Status"
                                name="status"
                                value={formData.status}
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="Submitted">Submitted</MenuItem>
                                <MenuItem value="In Review">In Review</MenuItem>
                                <MenuItem value="Approved">Approved</MenuItem>
                                <MenuItem value="Rejected">Rejected</MenuItem>
                            </TextField>
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Description"
                                name="description"
                                multiline
                                rows={4}
                                value={formData.description}
                                onChange={handleChange}
                                required
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <Button type="submit" variant="contained" color="primary">
                                Create Claim
                            </Button>
                            <Button 
                                variant="outlined" 
                                onClick={() => navigate('/claims')}
                                sx={{ ml: 2 }}
                            >
                                Cancel
                            </Button>
                        </Grid>
                    </Grid>
                </form>
            </Paper>
        </Container>
    );
};

export default ClaimCreate;
