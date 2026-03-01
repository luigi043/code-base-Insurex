import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';

// Import pages (you'll need to create these)
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
import ReportList from './pages/reports/ReportList';
import AdminPanel from './pages/admin/AdminPanel';
import UninsuredAssets from './pages/reports/UninsuredAssets';
import ExpiringPolicies from './pages/reports/ExpiringPolicies';

// Components
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import LoadingSpinner from './components/LoadingSpinner';

function AppContent() {
  const { isLoading } = useAuth();

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <Routes>
      {/* Public routes */}
      <Route path="/login" element={<Login />} />
      
      {/* Protected routes */}
      <Route path="/" element={
        <ProtectedRoute>
          <Layout />
        </ProtectedRoute>
      }>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        
        {/* Policy routes */}
        <Route path="policies">
          <Route index element={<PolicyList />} />
          <Route path="create" element={<PolicyCreate />} />
          <Route path=":id" element={<PolicyDetail />} />
          <Route path=":id/edit" element={<PolicyDetail />} />
        </Route>
        
        {/* Asset routes */}
        <Route path="assets">
          <Route index element={<AssetList />} />
          <Route path="create" element={<AssetCreate />} />
          <Route path=":id" element={<AssetDetail />} />
          <Route path="policy/:policyId" element={<AssetList />} />
        </Route>
        
        {/* Claim routes */}
        <Route path="claims">
          <Route index element={<ClaimList />} />
          <Route path="create" element={<ClaimCreate />} />
          <Route path=":id" element={<ClaimDetail />} />
          <Route path="policy/:policyId" element={<ClaimList />} />
        </Route>
        
        {/* Report routes */}
        <Route path="reports">
          <Route index element={<ReportList />} />
          <Route path="uninsured-assets" element={<UninsuredAssets />} />
          <Route path="expiring-policies" element={<ExpiringPolicies />} />
        </Route>
        
        {/* Admin routes */}
        <Route path="admin" element={
          <ProtectedRoute requiredRole="Admin">
            <AdminPanel />
          </ProtectedRoute>
        } />
      </Route>
      
      {/* Fallback */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

function App() {
  return (
    <Router>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </Router>
  );
}

export default App;