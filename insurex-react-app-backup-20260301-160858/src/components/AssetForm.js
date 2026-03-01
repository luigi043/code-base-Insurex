import React, { useState } from 'react';
import { createAsset } from '../services/api';

const assetTypes = [
  'Vehicle', 'Property', 'Watercraft', 'Aviation',
  'Stock', 'AccountsReceivable', 'Machinery',
  'PlantEquipment', 'BusinessInterruption', 'Keyman', 'ElectronicEquipment'
];

function AssetForm({ policyId, onAssetCreated }) {
  const [formData, setFormData] = useState({
    assetType: 'Vehicle',
    description: '',
    financeValue: 0,
    insuredValue: 0,
    policyId: policyId,
    assetData: {}
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await createAsset(formData);
      onAssetCreated(response.data);
      // Reset form
      setFormData({
        assetType: 'Vehicle',
        description: '',
        financeValue: 0,
        insuredValue: 0,
        policyId: policyId,
        assetData: {}
      });
    } catch (error) {
      console.error('Error creating asset:', error);
    }
  };

  return (
    <div className="asset-form">
      <h3>Add New Asset</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Asset Type:</label>
          <select
            value={formData.assetType}
            onChange={(e) => setFormData({...formData, assetType: e.target.value})}
          >
            {assetTypes.map(type => (
              <option key={type} value={type}>{type}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Description:</label>
          <input
            type="text"
            value={formData.description}
            onChange={(e) => setFormData({...formData, description: e.target.value})}
            required
          />
        </div>
        <div>
          <label>Finance Value:</label>
          <input
            type="number"
            value={formData.financeValue}
            onChange={(e) => setFormData({...formData, financeValue: parseFloat(e.target.value)})}
            required
          />
        </div>
        <div>
          <label>Insured Value:</label>
          <input
            type="number"
            value={formData.insuredValue}
            onChange={(e) => setFormData({...formData, insuredValue: parseFloat(e.target.value)})}
            required
          />
        </div>
        <button type="submit">Add Asset</button>
      </form>
    </div>
  );
}

export default AssetForm;
