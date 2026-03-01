import React, { useState } from 'react';
import { Container, Typography, Paper, TextField, Button, Grid, MenuItem } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const assetTypes = [
    'Vehicle', 'Property', 'Watercraft', 'Aviation', 
    'Stock', 'AccountsReceivable', 'Machinery', 
    'PlantEquipment', 'BusinessInterruption', 'Keyman', 'ElectronicEquipment'
];

const AssetCreate: React.FC = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        policyId: '',
        assetType: 'Vehicle',
        financeValue: '',
        insuredValue: '',
        status: 'Active',
        assetData: '{}'
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await api.post('/assets', formData);
            navigate('/assets');
        } catch (error) {
            console.error('Error creating asset:', error);
        }
    };

    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>Create New Asset</Typography>
            <Paper sx={{ p: 3 }}>
                <form onSubmit={handleSubmit}>
                    <Grid container spacing={3}>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Policy ID"
                                name="policyId"
                                type="number"
                                value={formData.policyId}
                                onChange={handleChange}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                select
                                label="Asset Type"
                                name="assetType"
                                value={formData.assetType}
                                onChange={handleChange}
                                required
                            >
                                {assetTypes.map((type) => (
                                    <MenuItem key={type} value={type}>{type}</MenuItem>
                                ))}
                            </TextField>
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Finance Value"
                                name="financeValue"
                                type="number"
                                value={formData.financeValue}
                                onChange={handleChange}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} md={6}>
                            <TextField
                                fullWidth
                                label="Insured Value"
                                name="insuredValue"
                                type="number"
                                value={formData.insuredValue}
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
                                <MenuItem value="Active">Active</MenuItem>
                                <MenuItem value="Inactive">Inactive</MenuItem>
                            </TextField>
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Asset Data (JSON)"
                                name="assetData"
                                multiline
                                rows={4}
                                value={formData.assetData}
                                onChange={handleChange}
                                helperText="Enter asset-specific data as JSON"
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <Button type="submit" variant="contained" color="primary">
                                Create Asset
                            </Button>
                            <Button 
                                variant="outlined" 
                                onClick={() => navigate('/assets')}
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

export default AssetCreate;
