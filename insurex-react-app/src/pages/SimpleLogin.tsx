import React, { useState } from 'react';

function Login() {
    const [email, setEmail] = useState('admin@insurex.com');
    const [password, setPassword] = useState('password');
    const [message, setMessage] = useState('');
    const [token, setToken] = useState('');

    const login = async () => {
        try {
            setMessage('Logging in...');
            const res = await fetch('http://localhost:5013/api/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });
            const data = await res.json();
            if (res.ok) {
                setMessage('Success!');
                setToken(data.token);
            } else {
                setMessage('Failed: ' + (data.message || 'Unknown error'));
            }
        } catch (err: any) {
            setMessage('Error: ' + err.message);
        }
    };

    const register = async () => {
        try {
            setMessage('Registering...');
            const res = await fetch('http://localhost:5013/api/Auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password, role: 'Admin' })
            });
            const data = await res.json();
            if (res.ok) {
                setMessage('Registered! Try login now.');
            } else {
                setMessage('Failed: ' + (data.message || 'Unknown error'));
            }
        } catch (err: any) {
            setMessage('Error: ' + err.message);
        }
    };

    return (
        <div style={{ padding: 20, fontFamily: 'Arial' }}>
            <h2>InsureX Login</h2>
            <div>
                <div>Email:</div>
                <input value={email} onChange={e => setEmail(e.target.value)} style={{ width: 300, padding: 5 }} />
            </div>
            <div style={{ marginTop: 10 }}>
                <div>Password:</div>
                <input type="password" value={password} onChange={e => setPassword(e.target.value)} style={{ width: 300, padding: 5 }} />
            </div>
            <div style={{ marginTop: 20 }}>
                <button onClick={register} style={{ marginRight: 10, padding: '10px 20px' }}>Register</button>
                <button onClick={login} style={{ padding: '10px 20px' }}>Login</button>
            </div>
            <div style={{ marginTop: 20, padding: 10, background: '#f0f0f0' }}>{message}</div>
            {token && <div style={{ marginTop: 10, wordBreak: 'break-all' }}>Token: {token.substring(0, 50)}...</div>}
        </div>
    );
}

export default Login;
