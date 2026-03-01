import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AuthProvider } from './contexts/AuthContext';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import PoliciesPage from './pages/PoliciesPage';
import PolicyFormPage from './pages/PolicyFormPage';
import Layout from './components/Layout';

const theme = createTheme({
    palette: {
        primary: {
            main: '#1976d2',
        },
        secondary: {
            main: '#dc004e',
        },
    },
});

const PrivateRoute = ({ children }: { children: JSX.Element }) => {
    const token = localStorage.getItem('token');
    return token ? children : <Navigate to="/login" />;
};

function App() {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <AuthProvider>
                <Router>
                    <Routes>
                        <Route path="/login" element={<Login />} />
                        <Route
                            path="/dashboard"
                            element={
                                <PrivateRoute>
                                    <Layout>
                                        <Dashboard />
                                    </Layout>
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/policies"
                            element={
                                <PrivateRoute>
                                    <Layout>
                                        <PoliciesPage />
                                    </Layout>
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/policies/new"
                            element={
                                <PrivateRoute>
                                    <Layout>
                                        <PolicyFormPage />
                                    </Layout>
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/policies/edit/:id"
                            element={
                                <PrivateRoute>
                                    <Layout>
                                        <PolicyFormPage />
                                    </Layout>
                                </PrivateRoute>
                            }
                        />
                        <Route path="/" element={<Navigate to="/dashboard" />} />
                    </Routes>
                </Router>
            </AuthProvider>
        </ThemeProvider>
    );
}

export default App;
