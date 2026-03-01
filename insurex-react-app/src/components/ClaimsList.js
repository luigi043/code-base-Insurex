import React, { useEffect, useState } from 'react';
import { getClaims, updateClaim, deleteClaim } from '../services/api';

function ClaimsList() {
  const [claims, setClaims] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchClaims();
  }, []);

  const fetchClaims = async () => {
    try {
      const response = await getClaims();
      setClaims(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching claims:', error);
      setLoading(false);
    }
  };

  const handleStatusUpdate = async (id, status) => {
    try {
      await updateClaim(id, { status });
      fetchClaims();
    } catch (error) {
      console.error('Error updating claim:', error);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this claim?')) {
      try {
        await deleteClaim(id);
        fetchClaims();
      } catch (error) {
        console.error('Error deleting claim:', error);
      }
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div className="claims-list">
      <h2>Claims</h2>
      <table>
        <thead>
          <tr>
            <th>Claim Number</th>
            <th>Policy ID</th>
            <th>Claim Date</th>
            <th>Description</th>
            <th>Amount</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {claims.map(claim => (
            <tr key={claim.id}>
              <td>{claim.claimNumber}</td>
              <td>{claim.policyId}</td>
              <td>{new Date(claim.claimDate).toLocaleDateString()}</td>
              <td>{claim.description}</td>
              <td>${claim.claimAmount}</td>
              <td>{claim.status}</td>
              <td>
                <select
                  value={claim.status}
                  onChange={(e) => handleStatusUpdate(claim.id, e.target.value)}
                >
                  <option value="Pending">Pending</option>
                  <option value="Approved">Approved</option>
                  <option value="Rejected">Rejected</option>
                  <option value="Paid">Paid</option>
                </select>
                <button onClick={() => handleDelete(claim.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default ClaimsList;
