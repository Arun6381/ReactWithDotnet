import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './CSS/Login.css';
import google from './googleas.jpg';
import github from './githubs.jpg';

const LoginComponent = ({ setisLogin, setUserName }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');

        try {
            const response = await axios.post(
                'https://localhost:7217/api/auth/login',
                { email, Password: password },
                { withCredentials: true }
            );
            const data = response.data;
            console.log(response.data)
            localStorage.setItem('token', data.token);
            localStorage.setItem('UserId', data.userID);
            localStorage.setItem('firstName', data.firstName);
            localStorage.setItem('roles', data.roles);

            setUserName(data.userName);
            setMessage(data.Message || 'Login successful');
            setisLogin(true);
            const role=localStorage.getItem('roles');

            if (response.status === 200 && role !== "admin") {
                navigate('/');
            } else {
                navigate('/adminboard')
            }
        } catch (error) {
            setMessage(error.response?.data?.Message || 'Login failed');
        } finally {
            setLoading(false);
        }
    };

    const handleGoogleLogin = () => {
        window.location.href = 'https://localhost:7217/api/auth/glogin';
    };

    const handleGithubLogin = () => {
        window.location.href = 'https://localhost:7217/api/auth/githublogin'; 
    };
    const handleMicrosoftLogin = () => {
        window.location.href = 'https://localhost:7217/api/auth/azureadlogin'; 
    };

    return (
        <div className="login-container">
            <h2 className="login-heading">Login</h2>
            <form className="login-form" onSubmit={handleLogin}>
                <div className="form-group">
                    <label htmlFor="email" className="form-label">Email</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        className="form-input"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password" className="form-label">Password</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        className="form-input"
                    />
                </div>
                <button type="submit" className="submit-btn" disabled={loading}>
                    {loading ? 'Logging in...' : 'Login'}
                </button>
            </form>

            <span className="google-login-btn" onClick={handleGoogleLogin}>
                <img src={google} alt="google" className="imggoogle" />
                
                
            </span>
            <span className="google-login-btn" onClick={handleMicrosoftLogin}>
                <img src={google} alt="google" className="imggoogle" />
                
                
            </span>

            <span className="github-login-btn" onClick={handleGithubLogin}>
                <img src={github} alt="github" className="imggoogle" />
            </span>

            {message && <p className="message">{message}</p>}
        </div>
    );
};

export default LoginComponent;
