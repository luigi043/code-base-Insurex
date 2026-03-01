import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import ClaimsList from './components/Claims/ClaimsList';
import ClaimDetail from './components/Claims/ClaimDetail';
import ClaimForm from './components/Claims/ClaimForm';
import BillingDashboard from './components/Billing/BillingDashboard';
import InvoicesList from './components/Billing/InvoicesList';
import InvoiceForm from './components/Billing/InvoiceForm';
import InvoiceDetail from './components/Billing/InvoiceDetail';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <nav className="navbar">
          <div className="nav-brand">InsureX</div>
          <div className="nav-links">
            <Link to="/">Dashboard</Link>
            <Link to="/policies">Policies</Link>
            <Link to="/assets">Assets</Link>
            <Link to="/claims">Claims</Link>
            <Link to="/billing">Billing</Link>
          </div>
        </nav>

        <div className="container">
          <Routes>
            <Route path="/" element={<h1>Dashboard</h1>} />
            <Route path="/policies" element={<h1>Policies</h1>} />
            <Route path="/assets" element={<h1>Assets</h1>} />
            <Route path="/claims" element={<ClaimsList />} />
            <Route path="/claims/new" element={<ClaimForm />} />
            <Route path="/claims/:id" element={<ClaimDetail />} />
            
            {/* Billing Routes */}
            <Route path="/billing" element={<BillingDashboard />} />
            <Route path="/billing/invoices" element={<InvoicesList />} />
            <Route path="/billing/invoices/new" element={<InvoiceForm />} />
            <Route path="/billing/invoices/:id" element={<InvoiceDetail />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
