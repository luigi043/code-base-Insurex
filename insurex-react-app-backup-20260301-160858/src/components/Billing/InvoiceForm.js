import React, { useState, useEffect } from 'react';
import { billingService } from '../../services/billingService';
import { policiesService } from '../../services/policiesService';
import './Billing.css';

const InvoiceForm = () => {
  const [formData, setFormData] = useState({
    policyId: '',
    invoiceDate: new Date().toISOString().split('T')[0],
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    tax: 0,
    notes: '',
    items: [
      {
        description: '',
        quantity: 1,
        unitPrice: 0,
        itemType: 'Premium'
      }
    ]
  });

  const [policies, setPolicies] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [subtotal, setSubtotal] = useState(0);

  useEffect(() => {
    loadPolicies();
  }, []);

  useEffect(() => {
    // Calculate subtotal whenever items change
    const newSubtotal = formData.items.reduce(
      (sum, item) => sum + (item.quantity * item.unitPrice), 
      0
    );
    setSubtotal(newSubtotal);
  }, [formData.items]);

  const loadPolicies = async () => {
    try {
      const data = await policiesService.getAll();
      setPolicies(data);
    } catch (err) {
      console.error('Failed to load policies:', err);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleItemChange = (index, field, value) => {
    const updatedItems = [...formData.items];
    updatedItems[index][field] = field === 'quantity' || field === 'unitPrice' 
      ? parseFloat(value) || 0 
      : value;
    setFormData(prev => ({ ...prev, items: updatedItems }));
  };

  const addItem = () => {
    setFormData(prev => ({
      ...prev,
      items: [
        ...prev.items,
        {
          description: '',
          quantity: 1,
          unitPrice: 0,
          itemType: 'Premium'
        }
      ]
    }));
  };

  const removeItem = (index) => {
    if (formData.items.length > 1) {
      const updatedItems = formData.items.filter((_, i) => i !== index);
      setFormData(prev => ({ ...prev, items: updatedItems }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await billingService.createInvoice(formData);
      alert('Invoice created successfully!');
      window.location.href = '/billing/invoices';
    } catch (err) {
      setError('Failed to create invoice');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount || 0);
  };

  const total = subtotal + (formData.tax || 0);

  return (
    <div className="invoice-form-container">
      <h2>Create New Invoice</h2>
      
      {error && <div className="error">{error}</div>}
      
      <form onSubmit={handleSubmit} className="invoice-form">
        <div className="form-section">
          <h3>Invoice Details</h3>
          
          <div className="form-group">
            <label>Policy *</label>
            <select
              name="policyId"
              value={formData.policyId}
              onChange={handleInputChange}
              required
            >
              <option value="">Select Policy</option>
              {policies.map(policy => (
                <option key={policy.id} value={policy.id}>
                  {policy.policyNumber} - {policy.policyHolder}
                </option>
              ))}
            </select>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Invoice Date *</label>
              <input
                type="date"
                name="invoiceDate"
                value={formData.invoiceDate}
                onChange={handleInputChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Due Date *</label>
              <input
                type="date"
                name="dueDate"
                value={formData.dueDate}
                onChange={handleInputChange}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label>Notes</label>
            <textarea
              name="notes"
              value={formData.notes}
              onChange={handleInputChange}
              rows="3"
              placeholder="Optional notes..."
            />
          </div>
        </div>

        <div className="form-section">
          <h3>Invoice Items</h3>
          
          {formData.items.map((item, index) => (
            <div key={index} className="item-row">
              <div className="item-fields">
                <div className="form-group">
                  <label>Description</label>
                  <input
                    type="text"
                    value={item.description}
                    onChange={(e) => handleItemChange(index, 'description', e.target.value)}
                    placeholder="Item description"
                    required
                  />
                </div>
                
                <div className="form-group">
                  <label>Type</label>
                  <select
                    value={item.itemType}
                    onChange={(e) => handleItemChange(index, 'itemType', e.target.value)}
                  >
                    <option value="Premium">Premium</option>
                    <option value="Fee">Fee</option>
                    <option value="Tax">Tax</option>
                    <option value="Adjustment">Adjustment</option>
                  </select>
                </div>
                
                <div className="form-group">
                  <label>Qty</label>
                  <input
                    type="number"
                    min="1"
                    value={item.quantity}
                    onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
                    required
                  />
                </div>
                
                <div className="form-group">
                  <label>Unit Price</label>
                  <input
                    type="number"
                    min="0"
                    step="0.01"
                    value={item.unitPrice}
                    onChange={(e) => handleItemChange(index, 'unitPrice', e.target.value)}
                    required
                  />
                </div>
                
                <div className="item-amount">
                  {formatCurrency(item.quantity * item.unitPrice)}
                </div>
                
                {formData.items.length > 1 && (
                  <button 
                    type="button" 
                    className="btn-remove"
                    onClick={() => removeItem(index)}
                  >
                    ×
                  </button>
                )}
              </div>
            </div>
          ))}
          
          <button type="button" className="btn-add-item" onClick={addItem}>
            + Add Item
          </button>
        </div>

        <div className="form-section">
          <h3>Summary</h3>
          
          <div className="summary-row">
            <span>Subtotal:</span>
            <span>{formatCurrency(subtotal)}</span>
          </div>
          
          <div className="form-group summary-tax">
            <label>Tax:</label>
            <input
              type="number"
              name="tax"
              min="0"
              step="0.01"
              value={formData.tax}
              onChange={handleInputChange}
            />
          </div>
          
          <div className="summary-row total">
            <span>Total:</span>
            <span>{formatCurrency(total)}</span>
          </div>
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            className="btn-secondary" 
            onClick={() => window.location.href = '/billing/invoices'}
          >
            Cancel
          </button>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Creating...' : 'Create Invoice'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default InvoiceForm;
