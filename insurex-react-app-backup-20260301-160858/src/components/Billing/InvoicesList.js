import React, { useState, useEffect } from 'react';
import { billingService } from '../../services/billingService';
import './Billing.css';

const InvoicesList = () => {
  const [invoices, setInvoices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    status: '',
    fromDate: '',
    toDate: ''
  });

  useEffect(() => {
    loadInvoices();
  }, []);

  const loadInvoices = async () => {
    try {
      setLoading(true);
      const data = await billingService.getInvoices(filters);
      setInvoices(data);
      setError(null);
    } catch (err) {
      setError('Failed to load invoices');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({ ...prev, [name]: value }));
  };

  const applyFilters = () => {
    loadInvoices();
  };

  const clearFilters = () => {
    setFilters({
      status: '',
      fromDate: '',
      toDate: ''
    });
    setTimeout(() => loadInvoices(), 100);
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount || 0);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString();
  };

  const getStatusClass = (status) => {
    switch(status?.toLowerCase()) {
      case 'draft': return 'status-draft';
      case 'sent': return 'status-sent';
      case 'partial': return 'status-partial';
      case 'paid': return 'status-paid';
      case 'overdue': return 'status-overdue';
      case 'cancelled': return 'status-cancelled';
      default: return '';
    }
  };

  if (loading) return <div className="loading">Loading invoices...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="invoices-container">
      <div className="invoices-header">
        <h2>Invoices</h2>
        <button 
          className="btn-primary"
          onClick={() => window.location.href = '/billing/invoices/new'}
        >
          + New Invoice
        </button>
      </div>

      <div className="filters-section">
        <h3>Filters</h3>
        <div className="filters-grid">
          <div className="filter-group">
            <label>Status</label>
            <select name="status" value={filters.status} onChange={handleFilterChange}>
              <option value="">All</option>
              <option value="Draft">Draft</option>
              <option value="Sent">Sent</option>
              <option value="Partial">Partial</option>
              <option value="Paid">Paid</option>
              <option value="Overdue">Overdue</option>
            </select>
          </div>
          
          <div className="filter-group">
            <label>From Date</label>
            <input 
              type="date" 
              name="fromDate" 
              value={filters.fromDate} 
              onChange={handleFilterChange}
            />
          </div>
          
          <div className="filter-group">
            <label>To Date</label>
            <input 
              type="date" 
              name="toDate" 
              value={filters.toDate} 
              onChange={handleFilterChange}
            />
          </div>
        </div>
        
        <div className="filter-actions">
          <button className="btn-secondary" onClick={applyFilters}>Apply Filters</button>
          <button className="btn-text" onClick={clearFilters}>Clear</button>
        </div>
      </div>

      <div className="invoices-table-container">
        <table className="invoices-table">
          <thead>
            <tr>
              <th>Invoice #</th>
              <th>Policy #</th>
              <th>Customer</th>
              <th>Date</th>
              <th>Due Date</th>
              <th>Amount</th>
              <th>Status</th>
              <th>Paid</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {invoices.length === 0 ? (
              <tr>
                <td colSpan="9" className="no-data">No invoices found</td>
              </tr>
            ) : (
              invoices.map(invoice => (
                <tr key={invoice.id}>
                  <td>{invoice.invoiceNumber}</td>
                  <td>{invoice.policyNumber}</td>
                  <td>{invoice.customerName}</td>
                  <td>{formatDate(invoice.invoiceDate)}</td>
                  <td className={new Date(invoice.dueDate) < new Date() && invoice.status !== 'Paid' ? 'overdue' : ''}>
                    {formatDate(invoice.dueDate)}
                  </td>
                  <td>{formatCurrency(invoice.totalAmount)}</td>
                  <td>
                    <span className={`status-badge ${getStatusClass(invoice.status)}`}>
                      {invoice.status}
                    </span>
                  </td>
                  <td>{formatCurrency(invoice.paidAmount)}</td>
                  <td>
                    <button 
                      className="btn-view"
                      onClick={() => window.location.href = `/billing/invoices/${invoice.id}`}
                    >
                      View
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default InvoicesList;
