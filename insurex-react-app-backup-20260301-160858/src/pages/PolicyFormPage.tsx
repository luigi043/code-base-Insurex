import React, { useState, useEffect } from 'react';
import {
    Container,
    Box,
    Typography,
    TextField,
    Button,
    Paper,
    Grid,
    MenuItem,
    FormControl,
    InputLabel,
    Select,
    Alert,
    CircularProgress,
    Divider
} from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { policyService, Policy, CreatePolicyDto, UpdatePolicyDto } from '../services/policyService';

const PolicyFormPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEditMode = !!id;

    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [formData, setFormData] = useState<CreatePolicyDto>({
        policyHolder: '',
        email: '',
        phone: '',
        startDate: new Date().toISOString().split('T')[0],
        endDate: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0],
        premium: 0,
        policyType: 'Personal',
        notes: ''
    });

    useEffect(() => {
        if (isEditMode) {
            loadPolicy();
        }
    }, [id]);

    const loadPolicy = async () => {
        try {
            setLoading(true);
            const data = await policyService.getById(parseInt(id!));
            setFormData({
                policyHolder: data.policyHolder,
                email: data.email,
                phone: data.phone || '',
                startDate: data.startDate.split('T')[0],
                endDate: data.endDate.split('T')[0],
                premium: data.premium,
                policyType: data.policyType,
                notes: data.notes || ''
            });
            setError(null);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Erro ao carregar apólice');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | { name?: string; value: unknown }>) => {
        const { name, value } = e.target;
        setFormData((prev: CreatePolicyDto) => ({
            ...prev,
            [name as string]: value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);

        try {
            if (isEditMode) {
                await policyService.update(parseInt(id!), formData as UpdatePolicyDto);
            } else {
                await policyService.create(formData);
            }
            navigate('/policies');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Erro ao salvar apólice');
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
            <Typography variant="h5" component="h2" gutterBottom>
                {isEditMode ? 'Editar Apólice' : 'Nova Apólice'}
            </Typography>

            <Paper sx={{ p: 3, mt: 2 }}>
                {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

                <form onSubmit={handleSubmit}>
                    <Grid container spacing={3}>
                        <Grid item xs={12}>
                            <Typography variant="subtitle1" fontWeight="bold" gutterBottom>
                                Informações do Segurado
                            </Typography>
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="policyHolder"
                                label="Nome do Segurado"
                                fullWidth
                                required
                                value={formData.policyHolder}
                                onChange={handleChange}
                            />
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="email"
                                label="Email"
                                type="email"
                                fullWidth
                                required
                                value={formData.email}
                                onChange={handleChange}
                            />
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="phone"
                                label="Telefone"
                                fullWidth
                                value={formData.phone}
                                onChange={handleChange}
                                placeholder="(11) 99999-9999"
                            />
                        </Grid>

                        <Grid item xs={12}>
                            <Divider sx={{ my: 2 }} />
                            <Typography variant="subtitle1" fontWeight="bold" gutterBottom>
                                Detalhes da Apólice
                            </Typography>
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="startDate"
                                label="Data de Início"
                                type="date"
                                fullWidth
                                required
                                InputLabelProps={{ shrink: true }}
                                value={formData.startDate}
                                onChange={handleChange}
                            />
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="endDate"
                                label="Data de Término"
                                type="date"
                                fullWidth
                                required
                                InputLabelProps={{ shrink: true }}
                                value={formData.endDate}
                                onChange={handleChange}
                            />
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <TextField
                                name="premium"
                                label="Prêmio (R$)"
                                type="number"
                                fullWidth
                                required
                                value={formData.premium}
                                onChange={handleChange}
                                inputProps={{ min: 0, step: 0.01 }}
                            />
                        </Grid>

                        <Grid item xs={12} md={6}>
                            <FormControl fullWidth required>
                                <InputLabel>Tipo de Apólice</InputLabel>
                                <Select
                                    name="policyType"
                                    value={formData.policyType}
                                    label="Tipo de Apólice"
                                    onChange={handleChange as any}
                                >
                                    <MenuItem value="Personal">Pessoal</MenuItem>
                                    <MenuItem value="Business">Empresarial</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>

                        <Grid item xs={12}>
                            <TextField
                                name="notes"
                                label="Observações"
                                multiline
                                rows={4}
                                fullWidth
                                value={formData.notes}
                                onChange={handleChange}
                            />
                        </Grid>

                        <Grid item xs={12}>
                            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                                <Button
                                    variant="outlined"
                                    onClick={() => navigate('/policies')}
                                >
                                    Cancelar
                                </Button>
                                <Button
                                    type="submit"
                                    variant="contained"
                                    disabled={saving}
                                >
                                    {saving ? <CircularProgress size={24} /> : (isEditMode ? 'Atualizar' : 'Criar')}
                                </Button>
                            </Box>
                        </Grid>
                    </Grid>
                </form>
            </Paper>
        </Container>
    );
};

export default PolicyFormPage;
