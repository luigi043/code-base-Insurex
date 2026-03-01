import React, { useState } from 'react';
import './App.css';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import PolicyList from './components/PolicyList';
import ClaimsList from './components/ClaimsList';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('token'));
  const [currentView, setCurrentView] = useState('dashboard');

  const handleLogin = (token) => {
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
  };

  if (!isAuthenticated) {
    return <Login onLogin={handleLogin} />;
  }

  return (
    <div className="App">
      <nav className="navbar">
        <h1>InsureX</h1>
        <div className="nav-links">
          <button onClick={() => setCurrentView('dashboard')}>Dashboard</button>
          <button onClick={() => setCurrentView('policies')}>Policies</button>
          <button onClick={() => setCurrentView('claims')}>Claims</button>
          <button onClick={handleLogout}>Logout</button>
        </div>
      </nav>
      <main className="main-content">
        {currentView === 'dashboard' && <Dashboard />}
        {currentView === 'policies' && <PolicyList />}
        {currentView === 'claims' && <ClaimsList />}
      </main>
    </div>
  );
}

export default App;
