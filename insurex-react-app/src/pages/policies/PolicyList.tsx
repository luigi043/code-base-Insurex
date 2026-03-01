import React from 'react';
import { Container, Typography, Paper } from '@mui/material';

const PolicyList: React.FC = () => {
  return (
    <Container>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h4">Policies</Typography>
        <Typography>Policy list will appear here</Typography>
      </Paper>
    </Container>
  );
};

export default PolicyList;