import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthCallback = ({ setisLogin, setUserName }) => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const queryParams = new URLSearchParams(window.location.search);
        const token = queryParams.get('token');
        const UserId = queryParams.get('UserId');
        const Roles = queryParams.get('Roles');
        const UserName = queryParams.get('userName');
        setTimeout(() => {
            if (Roles === "admin") {
                navigate('/adminboard')

            } else if (Roles === "user") {
                navigate('/');
            } else {
                alert("Only the User or Admin can Login to This site")
            }
         
        }, [500])
        if (token) {
            localStorage.setItem('token', token);
            localStorage.setItem('userName', UserName);
            localStorage.setItem('userId', UserId);
            localStorage.setItem('roles', Roles);
            setisLogin(true);
            setLoading(false);
            navigate('/'); 
        } else {
            setLoading(false);
            setError('Login failed. Missing token or username.');
            navigate('/SignIn');
        }
    }, [navigate, setisLogin, setUserName]);

    if (loading) {
        return <p>Redirecting...</p>;
    }

    if (error) {
        return <p>{error}</p>;
    }

    return null;
};

export default AuthCallback;
