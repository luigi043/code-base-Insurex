import React from 'react';
import { Container, Typography, Paper } from '@mui/material';

const Dashboard: React.FC = () => {
  return (
    <Container>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h4">Dashboard</Typography>
        <Typography>Welcome to InsureX Dashboard</Typography>
      </Paper>
    </Container>
  );
};

export default Dashboard;