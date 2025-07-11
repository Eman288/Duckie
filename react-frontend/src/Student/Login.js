import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../css/login.css";
import "../css/layout.css";


const Login = () => {
    const [formData, setFormData] = useState({
        email: "",
        password: ""
    });

    const [error, setError] = useState(""); // Define error state
    const navigate = useNavigate(); // For navigation

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value, // Update state dynamically
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = new FormData(); // Fixed typo
        data.append("email", formData.email);
        data.append("password", formData.password);

        try {
            const response = await fetch("http://localhost:5168/api/studentapi/login", {
                method: "POST",
                body: data,
                credentials: "include",
            });

            const result = await response.json();

            if (!response.ok) {
                setError(result.message || "Login failed!");
            } else {
                alert("Login successful!");
                navigate("/dashboard"); // Use React Router for navigation
            }
            // setting the session storage
            if (data.studentData) {
                sessionStorage.setItem("user", JSON.stringify(data.studentData)); // Store only studentData
                console.log("Student Data Saved:", data.studentData);
            } else {
                console.error("Login failed:", data.message);
            }
        } catch (error) {
            setError("A Network Error Occurred");
        }
    };

    return (
        <div class="login-box">
            <div className="login-container">
            <div className="log">
                <h2>Login to Learn and Enjoy fast</h2>
            </div>

            {error && <p className="error-message">{error}</p>} {/* Show error message if exists */}

            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label htmlFor="email">Email:</label>
                    <input
                        id="email"
                        name="email"
                        type="email"
                        placeholder="example@gmail.com"
                        value={formData.email} // Controlled input
                        onChange={handleChange} // Update state
                        required
                    />
                </div>

                <div className="mb-3">
                    <label htmlFor="password">Password:</label>
                    <input
                        id="password"
                        name="password"
                        type="password"
                        placeholder="Password"
                        value={formData.password} // Controlled input
                        onChange={handleChange} // Update state
                        required
                    />
                </div>

                <button type="submit">Login</button>
            </form>

            <p className="mt-3">
                Don't have an account? <Link to="/register">Register</Link>
            </p>
        </div>
        </div>
    );
};

export default Login;
