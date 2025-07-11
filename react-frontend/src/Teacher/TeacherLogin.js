import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../css/teacher.css";
import "../css/layout.css";

const TeacherLogin = () => {
    const [formData, setFormData] = useState({ email: "", password: "" });
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await fetch("http://localhost:5168/api/teacherapi/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(formData),
            });

            const text = await response.text();
            let result;
            try {
                result = JSON.parse(text);
            } catch {
                throw new Error("Invalid server response");
            }

            if (!response.ok) {
                setError(result.message || "Login failed!");
            } else {
                sessionStorage.setItem("teacherData", JSON.stringify(result));
                navigate("/teacher/dashboard");
            }
        } catch (err) {
            console.error(err);
            setError("A network error occurred.");
        }
        finally {
            console.log("Sending:", formData);
        }

    };

    return (
        <div className="teacher-login-wrapper">
            <div className="teacher-login-box">
                <h2 className="login-title">Welcome Back, Teacher!</h2>
                <p className="login-sub">Sign in to manage your courses and track student progress</p>

                {error && <p className="error-message">{error}</p>}

                <form onSubmit={handleSubmit} className="login-form">
                    <div className="form-group">
                        <label htmlFor="email">Email Address</label>
                        <input
                            type="email"
                            name="email"
                            id="email"
                            placeholder="you@example.com"
                            value={formData.email}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            name="password"
                            id="password"
                            placeholder="••••••••"
                            value={formData.password}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <button type="submit" className="login-button">Login</button>
                </form>

                <p className="login-footer">Don't have an account? Contact your admin to get access.</p>
            </div>
        </div>
    );
};

export default TeacherLogin;
