import axios from "axios";
import React, { useState } from "react";
import { useNavigate } from 'react-router-dom';

import "./CSS/Signup.css";

const Signup = () => {
    const [formData, setFormData] = useState({
        firstName: "",
        lastName: "",
        dateOfBirth: "",
        gender: "",
        phoneNumber: "",
        emailAddress: "",
        username: "",
        passwordHash: "",
    });

    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage("");

        // Simple validation
        if (!formData.firstName || !formData.lastName || !formData.emailAddress || !formData.passwordHash) {
            setMessage("Please fill in all required fields.");
            return;
        }

        try {
            const response = await axios.post("https://localhost:7217/api/users", formData, {
                headers: {
                    "Content-Type": "application/json",
                },
            });
            
            alert(response.data.message || "User created successfully!");
            
                navigate('/signin');
            
        } catch (error) {
            if (error.response) {
                setMessage(error.response.data.message || "An error occurred.");
            } else {
                setMessage("An error occurred: " + error.message);
            }
        }
    };

    return (
        <div className="signup-form">
            <h2>Signup Form</h2>
            {message && <p className="error-message">{message}</p>}
            <form onSubmit={handleSubmit}>
                {/* Input Fields */}
                <input
                    type="text"
                    name="firstName"
                    placeholder="First Name"
                    value={formData.firstName}
                    onChange={handleChange}
                    required
                />
                <input
                    type="text"
                    name="lastName"
                    placeholder="Last Name"
                    value={formData.lastName}
                    onChange={handleChange}
                    required
                />
                <input
                    type="date"
                    name="dateOfBirth"
                    value={formData.dateOfBirth}
                    onChange={handleChange}
                    required
                />
                <select
                    name="gender"
                    value={formData.gender}
                    onChange={handleChange}
                    required
                >
                    <option value="">Select Gender</option>
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                </select>
                <input
                    type="tel"
                    name="phoneNumber"
                    placeholder="Phone Number"
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    required
                />
                <input
                    type="email"
                    name="emailAddress"
                    placeholder="Email"
                    value={formData.emailAddress}
                    onChange={handleChange}
                    required
                />
                <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={formData.username}
                    onChange={handleChange}
                    required
                />
                <input
                    type="password"
                    name="passwordHash"
                    placeholder="Password"
                    value={formData.passwordHash}
                    onChange={handleChange}
                    required
                />
                <button type="submit">Signup</button>
            </form>
        </div>
    );
};

export default Signup;
