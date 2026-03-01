import React, { useEffect, useState } from 'react';
import {
    Container,
    Grid,
    Paper,
    Typography,
    Box,
    Card,
    CardContent,
    CircularProgress,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Chip
} from '@mui/material';
import {
    Policy as PolicyIcon,
    Assignment as AssetIcon,
    Warning as WarningIcon,
    AttachMoney as MoneyIcon
} from '@mui/icons-material';
import { dashboardService, DashboardStats } from '../services/dashboardService';

const Dashboard: React.FC = () => {
    const [stats, setStats] = useState<DashboardStats | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadStats();
    }, []);

    const loadStats = async () => {
        try {
            setLoading(true);
            const data = await dashboardService.getStats();
            setStats(data);
            setError(null);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Erro ao carregar estatísticas');
        } finally {
            setLoading(false);
        }
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    const StatCard = ({ title, value, icon, color }: any) => (
        <Card>
            <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Box>
                        <Typography color="textSecondary" gutterBottom variant="body2">
                            {title}
                        </Typography>
                        <Typography variant="h4" component="h2">
                            {value}
                        </Typography>
                    </Box>
                    <Box sx={{ color, fontSize: 40 }}>
                        {icon}
                    </Box>
                </Box>
            </CardContent>
        </Card>
    );

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Container maxWidth="lg">
                <Paper sx={{ p: 3, bgcolor: '#ffebee' }}>
                    <Typography color="error">{error}</Typography>
                </Paper>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg">
            <Typography variant="h4" gutterBottom>
                Dashboard
            </Typography>

            <Grid container spacing={3} sx={{ mb: 4 }}>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Total de Apólices"
                        value={stats?.totalPolicies || 0}
                        icon={<PolicyIcon fontSize="inherit" />}
                        color="#1976d2"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Apólices Ativas"
                        value={stats?.activePolicies || 0}
                        icon={<PolicyIcon fontSize="inherit" />}
                        color="#2e7d32"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Total de Ativos"
                        value={stats?.totalAssets || 0}
                        icon={<AssetIcon fontSize="inherit" />}
                        color="#ed6c02"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="A Vencer (30 dias)"
                        value={stats?.expiringSoon || 0}
                        icon={<WarningIcon fontSize="inherit" />}
                        color="#d32f2f"
                    />
                </Grid>
            </Grid>

            <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                    <Paper sx={{ p: 2 }}>
                        <Typography variant="h6" gutterBottom>
                            Valor Total Segurado
                        </Typography>
                        <Typography variant="h4" color="primary">
                            {formatCurrency(stats?.totalInsuredValue || 0)}
                        </Typography>
                    </Paper>
                </Grid>

                <Grid item xs={12} md={6}>
                    <Paper sx={{ p: 2 }}>
                        <Typography variant="h6" gutterBottom>
                            Apólices Recentes
                        </Typography>
                        <TableContainer>
                            <Table size="small">
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Número</TableCell>
                                        <TableCell>Segurado</TableCell>
                                        <TableCell>Status</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {stats?.recentPolicies.map((policy) => (
                                        <TableRow key={policy.id}>
                                            <TableCell>{policy.policyNumber}</TableCell>
                                            <TableCell>{policy.policyHolder}</TableCell>
                                            <TableCell>
                                                <Chip
                                                    label={policy.status}
                                                    size="small"
                                                    color={policy.status === 'Active' ? 'success' : 'default'}
                                                />
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Paper>
                </Grid>
            </Grid>
        </Container>
    );
};

export default Dashboard;
