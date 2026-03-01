import React, { useState } from 'react';
import { Container, Typography, Paper, Grid, Button, TextField, MenuItem } from '@mui/material';
import api from '../services/api';

const Reports: React.FC = () => {
    const [reportType, setReportType] = useState('uninsured');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');

    const generateReport = async () => {
        try {
            const response = await api.get(/reports/, {
                params: { startDate, endDate }
            });
            // Handle report data
            console.log(response.data);
        } catch (error) {
            console.error('Error generating report:', error);
        }
    };

    return (
        <Container maxWidth="lg" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>Reports</Typography>
            <Paper sx={{ p: 3 }}>
                <Grid container spacing={3}>
                    <Grid item xs={12} md={4}>
                        <TextField
                            fullWidth
                            select
                            label="Report Type"
                            value={reportType}
                            onChange={(e) => setReportType(e.target.value)}
                        >
                            <MenuItem value="uninsured">Uninsured Assets</MenuItem>
                            <MenuItem value="expiring">Expiring Policies</MenuItem>
                            <MenuItem value="claims">Claims Summary</MenuItem>
                            <MenuItem value="premiums">Premiums Collected</MenuItem>
                        </TextField>
                    </Grid>
                    <Grid item xs={12} md={3}>
                        <TextField
                            fullWidth
                            label="Start Date"
                            type="date"
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                            InputLabelProps={{ shrink: true }}
                        />
                    </Grid>
                    <Grid item xs={12} md={3}>
                        <TextField
                            fullWidth
                            label="End Date"
                            type="date"
                            value={endDate}
                            onChange={(e) => setEndDate(e.target.value)}
                            InputLabelProps={{ shrink: true }}
                        />
                    </Grid>
                    <Grid item xs={12} md={2}>
                        <Button 
                            fullWidth 
                            variant="contained" 
                            onClick={generateReport}
                            sx={{ height: '56px' }}
                        >
                            Generate
                        </Button>
                    </Grid>
                </Grid>
            </Paper>
        </Container>
    );
};

export default Reports;
