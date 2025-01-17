import React, { useEffect } from "react";
import axios from 'axios';
import { FaCartPlus } from "react-icons/fa";
import { Link, useNavigate } from "react-router-dom";

const Navbar = ({ isLogin, setisLogin, UserName, setUserName }) => {
    const roles = localStorage.getItem('roles');
    const userNameform = localStorage.getItem('firstName');
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            setisLogin(true);
            const userName = localStorage.getItem('userName');
            setUserName(userName);
        }
    }, [setisLogin, setUserName]);

    const handleLogout = async () => {
        try {
            await axios.post('https://localhost:7217/api/auth/logout', {}, { withCredentials: true });
            localStorage.clear();

            //localStorage.removeItem('token');
            //localStorage.removeItem('userId');
            //localStorage.removeItem('firstName');
            //localStorage.removeItem('userName');
            //localStorage.removeItem('roles');
            alert('Logged out successfully');
            setUserName("");
            setisLogin(false);
        } catch (error) {
            alert(error.response?.data?.Message || 'Logout failed');
        }
        navigate("/signIn");
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-light">
            <div className="container-fluid">
                <Link className="navbar-brand">Product Manager</Link>
                <div className="collapse navbar-collapse">
                    {roles === "user" ? (
                        <ul className="navbar-nav me-auto">
                            <li className="nav-item">
                                <Link className="nav-link" to="/">Home</Link>
                            </li>
                            <li className="nav-item">
                                <Link className="nav-link" to="/categories/create">Create Categories</Link>
                            </li>
                            <li className="nav-item">
                                <Link className="nav-link" to="/products/create">Create Products</Link>
                            </li>
                        </ul>
                    ): roles === "admin"? (
                         <ul className="navbar-nav me-auto">
                            <li className="nav-item">
                                <Link className="nav-link" to="/adminboard">Admin Dashboard</Link>
                            </li>
                            <li className="nav-item">
                                    <Link className="nav-link" to="/chartandGraph">Chart And Graph</Link>
                            </li>
                            {/*<li className="nav-item">*/}
                            {/*    <Link className="nav-link" to="/products/create">Create Products</Link>*/}
                            {/*</li>*/}
                        </ul>
                       
                       
                    ):""}

                    <ul className="navbar-nav ms-auto">
                        {roles === "user" && (
                            <li style={{ color: "black", fontWeight: "bold", marginRight: "10px" }}>
                                <span onClick={() => navigate('/add-to-cart')}>
                                    <FaCartPlus />
                                </span>
                            </li>
                        )}
                        {isLogin ? (
                            <>
                                <li>
                                    <span style={{ color: "black", fontWeight: "bold", marginRight: "10px" }}>
                                        {userNameform}{UserName}
                                    </span>
                                </li>
                                <li className="nav-item">
                                    <button className="btn btn-outline-danger" onClick={handleLogout}>
                                        Logout
                                    </button>
                                </li>
                            </>
                        ) : (
                            <>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/signin">Sign In</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/signup">Sign Up</Link>
                                </li>
                            </>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
