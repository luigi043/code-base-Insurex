import React, { useEffect, useState } from 'react';
import { getPolicies, getAssets, getClaims } from '../services/api';

function Dashboard() {
  const [stats, setStats] = useState({
    policies: 0,
    assets: 0,
    claims: 0
  });

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [policiesRes, assetsRes, claimsRes] = await Promise.all([
          getPolicies(),
          getAssets(),
          getClaims()
        ]);
        setStats({
          policies: policiesRes.data.length,
          assets: assetsRes.data.length,
          claims: claimsRes.data.length
        });
      } catch (error) {
        console.error('Error fetching stats:', error);
      }
    };
    fetchStats();
  }, []);

  return (
    <div className="dashboard">
      <h1>Dashboard</h1>
      <div className="stats-grid">
        <div className="stat-card">
          <h3>Policies</h3>
          <p>{stats.policies}</p>
        </div>
        <div className="stat-card">
          <h3>Assets</h3>
          <p>{stats.assets}</p>
        </div>
        <div className="stat-card">
          <h3>Claims</h3>
          <p>{stats.claims}</p>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
