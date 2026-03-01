import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { billingService } from '../../services/billingService';
import './Billing.css';

const InvoiceDetail = () => {
  const { id } = useParams();
  const [invoice, setInvoice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [paymentData, setPaymentData] = useState({
    paymentDate: new Date().toISOString().split('T')[0],
    amount: '',
    paymentMethod: 'Bank Transfer',
    reference: '',
    notes: ''
  });

  useEffect(() => {
    loadInvoice();
  }, [id]);

  const loadInvoice = async () => {
    try {
      setLoading(true);
      const data = await billingService.getInvoiceById(id);
      setInvoice(data);
      setPaymentData(prev => ({ ...prev, amount: data.totalAmount - (data.payments?.reduce((sum, p) => sum + p.amount, 0) || 0) }));
      setError(null);
    } catch (err) {
      setError('Failed to load invoice');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSendInvoice = async () => {
    if (!window.confirm('Send this invoice to the customer?')) return;
    
    try {
      await billingService.sendInvoice(id);
      alert('Invoice sent successfully!');
      loadInvoice();
    } catch (err) {
      alert('Failed to send invoice');
    }
  };

  const handleAddPayment = async (e) => {
    e.preventDefault();
    
    try {
      await billingService.addPayment(id, paymentData);
      alert('Payment added successfully!');
      setShowPaymentModal(false);
      loadInvoice();
    } catch (err) {
      alert('Failed to add payment');
    }
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount || 0);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
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

  const totalPaid = invoice?.payments?.reduce((sum, p) => sum + p.amount, 0) || 0;
  const balanceDue = (invoice?.totalAmount || 0) - totalPaid;

  if (loading) return <div className="loading">Loading invoice...</div>;
  if (error) return <div className="error">{error}</div>;
  if (!invoice) return <div className="error">Invoice not found</div>;

  return (
    <div className="invoice-detail-container">
      <div className="detail-header">
        <div>
          <h2>Invoice {invoice.invoiceNumber}</h2>
          <p className="customer-name">{invoice.customerName}</p>
        </div>
        <div className="header-actions">
          <span className={`status-badge ${getStatusClass(invoice.status)}`}>
            {invoice.status}
          </span>
          {invoice.status === 'Draft' && (
            <button className="btn-primary" onClick={handleSendInvoice}>
              Send Invoice
            </button>
          )}
          {balanceDue > 0 && invoice.status !== 'Paid' && (
            <button className="btn-success" onClick={() => setShowPaymentModal(true)}>
              Record Payment
            </button>
          )}
          <button className="btn-secondary" onClick={() => window.location.href = '/billing/invoices'}>
            Back to List
          </button>
        </div>
      </div>

      <div className="detail-content">
        <div className="detail-grid">
          <div className="detail-section">
            <h3>Invoice Information</h3>
            <div className="info-row">
              <label>Invoice Number:</label>
              <span>{invoice.invoiceNumber}</span>
            </div>
            <div className="info-row">
              <label>Policy Number:</label>
              <span>{invoice.policyNumber}</span>
            </div>
            <div className="info-row">
              <label>Invoice Date:</label>
              <span>{formatDate(invoice.invoiceDate)}</span>
            </div>
            <div className="info-row">
              <label>Due Date:</label>
              <span className={new Date(invoice.dueDate) < new Date() && invoice.status !== 'Paid' ? 'overdue' : ''}>
                {formatDate(invoice.dueDate)}
              </span>
            </div>
          </div>

          <div className="detail-section">
            <h3>Customer Information</h3>
            <div className="info-row">
              <label>Name:</label>
              <span>{invoice.customerName}</span>
            </div>
            <div className="info-row">
              <label>Email:</label>
              <span>{invoice.customerEmail}</span>
            </div>
            <div className="info-row">
              <label>Phone:</label>
              <span>{invoice.customerPhone}</span>
            </div>
          </div>
        </div>

        <div className="detail-section">
          <h3>Invoice Items</h3>
          <table className="items-table">
            <thead>
              <tr>
                <th>Description</th>
                <th>Type</th>
                <th>Quantity</th>
                <th>Unit Price</th>
                <th>Amount</th>
              </tr>
            </thead>
            <tbody>
              {invoice.items.map(item => (
                <tr key={item.id}>
                  <td>{item.description}</td>
                  <td>{item.itemType}</td>
                  <td>{item.quantity}</td>
                  <td>{formatCurrency(item.unitPrice)}</td>
                  <td>{formatCurrency(item.amount)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr>
                <td colSpan="4" className="summary-label">Subtotal:</td>
                <td>{formatCurrency(invoice.amount)}</td>
              </tr>
              <tr>
                <td colSpan="4" className="summary-label">Tax:</td>
                <td>{formatCurrency(invoice.tax)}</td>
              </tr>
              <tr className="total-row">
                <td colSpan="4" className="summary-label">Total:</td>
                <td>{formatCurrency(invoice.totalAmount)}</td>
              </tr>
            </tfoot>
          </table>
        </div>

        {invoice.notes && (
          <div className="detail-section">
            <h3>Notes</h3>
            <p className="notes">{invoice.notes}</p>
          </div>
        )}

        <div className="detail-section">
          <h3>Payment History</h3>
          {invoice.payments.length === 0 ? (
            <p className="no-payments">No payments recorded yet</p>
          ) : (
            <table className="payments-table">
              <thead>
                <tr>
                  <th>Payment #</th>
                  <th>Date</th>
                  <th>Method</th>
                  <th>Amount</th>
                  <th>Reference</th>
                  <th>Status</th>
                  <th>Recorded By</th>
                </tr>
              </thead>
              <tbody>
                {invoice.payments.map(payment => (
                  <tr key={payment.id}>
                    <td>{payment.paymentNumber}</td>
                    <td>{formatDate(payment.paymentDate)}</td>
                    <td>{payment.paymentMethod}</td>
                    <td>{formatCurrency(payment.amount)}</td>
                    <td>{payment.reference || '-'}</td>
                    <td>
                      <span className={`status-badge status-${payment.status.toLowerCase()}`}>
                        {payment.status}
                      </span>
                    </td>
                    <td>{payment.createdByName}</td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr className="total-row">
                  <td colSpan="3" className="summary-label">Total Paid:</td>
                  <td>{formatCurrency(totalPaid)}</td>
                  <td colSpan="3"></td>
                </tr>
                {balanceDue > 0 && (
                  <tr className="balance-row">
                    <td colSpan="3" className="summary-label">Balance Due:</td>
                    <td>{formatCurrency(balanceDue)}</td>
                    <td colSpan="3"></td>
                  </tr>
                )}
              </tfoot>
            </table>
          )}
        </div>

        <div className="detail-footer">
          <small>Created on {formatDate(invoice.createdAt)} by {invoice.createdByName}</small>
        </div>
      </div>

      {/* Payment Modal */}
      {showPaymentModal && (
        <div className="modal">
          <div className="modal-content">
            <h3>Record Payment</h3>
            <form onSubmit={handleAddPayment}>
              <div className="form-group">
                <label>Payment Date *</label>
                <input
                  type="date"
                  value={paymentData.paymentDate}
                  onChange={(e) => setPaymentData({...paymentData, paymentDate: e.target.value})}
                  required
                />
              </div>

              <div className="form-group">
                <label>Amount *</label>
                <input
                  type="number"
                  step="0.01"
                  min="0.01"
                  max={balanceDue}
                  value={paymentData.amount}
                  onChange={(e) => setPaymentData({...paymentData, amount: e.target.value})}
                  required
                />
                <small>Balance due: {formatCurrency(balanceDue)}</small>
              </div>

              <div className="form-group">
                <label>Payment Method *</label>
                <select
                  value={paymentData.paymentMethod}
                  onChange={(e) => setPaymentData({...paymentData, paymentMethod: e.target.value})}
                  required
                >
                  <option value="Bank Transfer">Bank Transfer</option>
                  <option value="Credit Card">Credit Card</option>
                  <option value="Cash">Cash</option>
                  <option value="Cheque">Cheque</option>
                </select>
              </div>

              <div className="form-group">
                <label>Reference</label>
                <input
                  type="text"
                  value={paymentData.reference}
                  onChange={(e) => setPaymentData({...paymentData, reference: e.target.value})}
                  placeholder="Transaction ID, cheque number, etc."
                />
              </div>

              <div className="form-group">
                <label>Notes</label>
                <textarea
                  value={paymentData.notes}
                  onChange={(e) => setPaymentData({...paymentData, notes: e.target.value})}
                  rows="2"
                />
              </div>

              <div className="modal-actions">
                <button type="button" className="btn-secondary" onClick={() => setShowPaymentModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary">
                  Record Payment
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default InvoiceDetail;
