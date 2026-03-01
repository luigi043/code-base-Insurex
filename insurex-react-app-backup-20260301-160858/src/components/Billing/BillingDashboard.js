import React, { useState, useEffect } from 'react';
import { billingService } from '../../services/billingService';
import './Billing.css';

const BillingDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      const data = await billingService.getDashboard();
      setDashboardData(data);
      setError(null);
    } catch (err) {
      setError('Failed to load billing dashboard');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2
    }).format(amount || 0);
  };

  if (loading) return <div className="loading">Loading dashboard...</div>;
  if (error) return <div className="error">{error}</div>;
  if (!dashboardData) return <div className="error">No data available</div>;

  return (
    <div className="billing-dashboard">
      <h2>Billing Dashboard</h2>
      
      <div className="dashboard-stats">
        <div className="stat-card primary">
          <h3>Current Month Invoiced</h3>
          <p className="stat-value">{formatCurrency(dashboardData.currentMonthInvoiced)}</p>
        </div>
        
        <div className="stat-card success">
          <h3>Current Month Paid</h3>
          <p className="stat-value">{formatCurrency(dashboardData.currentMonthPaid)}</p>
        </div>
        
        <div className="stat-card warning">
          <h3>Overdue Invoices</h3>
          <p className="stat-value">{dashboardData.overdueInvoices}</p>
          <p className="stat-sub">{formatCurrency(dashboardData.overdueAmount)}</p>
        </div>
        
        <div className="stat-card info">
          <h3>Upcoming Invoices</h3>
          <p className="stat-value">{dashboardData.upcomingInvoices}</p>
        </div>
        
        <div className="stat-card danger">
          <h3>Total Outstanding</h3>
          <p className="stat-value">{formatCurrency(dashboardData.totalOutstanding)}</p>
        </div>
      </div>

      <div className="dashboard-charts">
        <div className="chart-container">
          <h3>Monthly Overview</h3>
          <div className="simple-chart">
            <div className="chart-bar">
              <div className="bar-label">Invoiced</div>
              <div className="bar-container">
                <div 
                  className="bar bar-primary" 
                  style={{ width: '100%' }}
                ></div>
              </div>
            </div>
            <div className="chart-bar">
              <div className="bar-label">Paid</div>
              <div className="bar-container">
                <div 
                  className="bar bar-success" 
                  style={{ 
                    width: dashboardData.currentMonthInvoiced > 0 
                      ? `${(dashboardData.currentMonthPaid / dashboardData.currentMonthInvoiced) * 100}%` 
                      : '0%' 
                  }}
                ></div>
              </div>
            </div>
          </div>
        </div>

        <div className="quick-actions">
          <h3>Quick Actions</h3>
          <button 
            className="btn-primary" 
            onClick={() => window.location.href = '/billing/invoices/new'}
          >
            Create New Invoice
          </button>
          <button 
            className="btn-secondary" 
            onClick={() => window.location.href = '/billing/invoices'}
          >
            View All Invoices
          </button>
        </div>
      </div>
    </div>
  );
};

export default BillingDashboard;
