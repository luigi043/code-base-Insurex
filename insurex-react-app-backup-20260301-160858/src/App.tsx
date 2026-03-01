import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import PolicyList from './pages/policies/PolicyList';
import PolicyDetail from './pages/policies/PolicyDetail';
import PolicyCreate from './pages/policies/PolicyCreate';
import AssetList from './pages/assets/AssetList';
import AssetCreate from './pages/assets/AssetCreate';
import AssetDetail from './pages/assets/AssetDetail';
import ClaimList from './pages/claims/ClaimList';
import ClaimCreate from './pages/claims/ClaimCreate';
import ClaimDetail from './pages/claims/ClaimDetail';
import Reports from './pages/reports/index';
import AdminPanel from './pages/admin/index';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<ProtectedRoute><Layout /></ProtectedRoute>}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="policies" element={<PolicyList />} />
            <Route path="policies/create" element={<PolicyCreate />} />
            <Route path="policies/:id" element={<PolicyDetail />} />
            <Route path="assets" element={<AssetList />} />
            <Route path="assets/create" element={<AssetCreate />} />
            <Route path="assets/:id" element={<AssetDetail />} />
            <Route path="claims" element={<ClaimList />} />
            <Route path="claims/create" element={<ClaimCreate />} />
            <Route path="claims/:id" element={<ClaimDetail />} />
            <Route path="reports" element={<Reports />} />
            <Route path="admin" element={<AdminPanel />} />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
