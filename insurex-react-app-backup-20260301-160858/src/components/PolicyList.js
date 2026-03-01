import React, { useEffect, useState } from 'react';
import { getPolicies, deletePolicy } from '../services/api';

function PolicyList() {
  const [policies, setPolicies] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPolicies();
  }, []);

  const fetchPolicies = async () => {
    try {
      const response = await getPolicies();
      setPolicies(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching policies:', error);
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this policy?')) {
      try {
        await deletePolicy(id);
        fetchPolicies();
      } catch (error) {
        console.error('Error deleting policy:', error);
      }
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div className="policy-list">
      <h2>Policies</h2>
      <table>
        <thead>
          <tr>
            <th>Policy Number</th>
            <th>Policy Holder</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Status</th>
            <th>Premium</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {policies.map(policy => (
            <tr key={policy.id}>
              <td>{policy.policyNumber}</td>
              <td>{policy.policyHolder}</td>
              <td>{new Date(policy.startDate).toLocaleDateString()}</td>
              <td>{new Date(policy.endDate).toLocaleDateString()}</td>
              <td>{policy.status}</td>
              <td>${policy.premium}</td>
              <td>
                <button onClick={() => handleDelete(policy.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default PolicyList;
