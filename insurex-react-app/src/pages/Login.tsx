import React, { useState } from 'react';
import { Container, Box, Typography, TextField, Button, Paper } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await api.post('/api/v1/Auth/login', { email, password });
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.user));
            navigate('/dashboard');
        } catch (error) {
            alert('Erro ao fazer login');
        }
    };

    return (
        <Container component="main" maxWidth="xs">
            <Box sx={{ mt: 8 }}>
                <Paper elevation={3} sx={{ p: 4 }}>
                    <Typography variant="h5" align="center" gutterBottom>
                        InsureX - Login
                    </Typography>
                    <form onSubmit={handleSubmit}>
                        <TextField margin="normal" required fullWidth label="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
                        <TextField margin="normal" required fullWidth label="Senha" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                        <Button type="submit" fullWidth variant="contained" sx={{ mt: 3 }}>
                            Entrar
                        </Button>
                    </form>
                </Paper>
            </Box>
        </Container>
    );
};

export default Login;
