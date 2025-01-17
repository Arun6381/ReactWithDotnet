import React, { useState } from 'react';
import axios from 'axios';

const LogoutComponent = () => {
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogout = async () => {
        setLoading(true);
        setMessage('');
        try {
            await axios.post('https://localhost:7217/api/auth/logout', {}, { withCredentials: true });
            setMessage('Logged out successfully');
        } catch (error) {
            setMessage(error.response?.data?.Message || 'Logout failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <button onClick={handleLogout} disabled={loading}>
                {loading ? 'Logging out...' : 'Logout'}
            </button>
            {message && <p>{message}</p>}
        </div>
    );
};

export default LogoutComponent;
