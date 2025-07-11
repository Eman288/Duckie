import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../css/auth.css";

const AuthForm = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [loginData, setLoginData] = useState({ email: "", password: "" });
    const [registerData, setRegisterData] = useState({
        Name: "",
        Email: "",
        Password: "",
        Birthdate: "",
        Picture: null,
    });
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleToggle = () => {
        setIsLogin(!isLogin);
        setError(null);
    };
    const handleLoginChange = (e) => {
        setLoginData({ ...loginData, [e.target.name]: e.target.value });
    };
    const handleRegisterChange = (e) => {
        setRegisterData({ ...registerData, [e.target.name]: e.target.value });
    };
    const handleFileChange = (e) => {
        setRegisterData({ ...registerData, Picture: e.target.files[0] });
    };
    const handleLoginSubmit = async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append("email", loginData.email);
        formData.append("password", loginData.password);

        try {
            const response = await fetch("http://localhost:5168/api/studentapi/login", {
                method: "POST",
                body: formData,
                credentials: "include",
            });
            const result = await response.json();

            if (!response.ok) {
                setError(result.message || "Login failed!");
                return;
            }

            alert("Login successful!");
            const user = result.studentData;

            sessionStorage.setItem("user", JSON.stringify(user));

            if (user.totalPoints < 0) {
                navigate("/evaluationQuizSetup");
            } else {
                console.log(user);
                navigate("/dashboard");
            }
        } catch (err) {
            setError("A network error occurred.");
        }
    };

    const [registerLoading, setRegisterLoading] = useState(false);

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        setRegisterLoading(true); // Start loading
        const formData = new FormData();
        formData.append("Name", registerData.Name);
        formData.append("Email", registerData.Email);
        formData.append("Password", registerData.Password);
        formData.append("Birthdate", registerData.Birthdate);
        if (registerData.Picture) {
            formData.append("Picture", registerData.Picture);
        }

        try {
            const response = await fetch("http://localhost:5168/api/studentapi/register", {
                method: "POST",
                body: formData,
            });
            const result = await response.json();

            if (!response.ok) {
                setError(result.message || "Registration failed!");
            } else {
                alert("Registration successful!");
                setIsLogin(true);
            }
        } catch (err) {
            setError("A network error occurred.");
        } finally {
            setRegisterLoading(false); // Stop loading
        }
    };


    return (
        <div className="auth-container">
            <div className={`form-container ${!isLogin ? "right-panel-active" : ""}`}>

                {/* Login Form */}
                <div className="form login-form">
                    <form onSubmit={handleLoginSubmit}>
                        <h2>Login</h2>
                        {error && isLogin && <p className="error">{error}</p>}
                        <input
                            name="email"
                            type="email"
                            placeholder="Email"
                            value={loginData.email}
                            onChange={handleLoginChange}
                            required
                        />
                        <input
                            name="password"
                            type="password"
                            placeholder="Password"
                            value={loginData.password}
                            onChange={handleLoginChange}
                            required
                        />
                        <button type="submit">Login</button>
                    </form>
                </div>

                {/* Register Form */}
                <div className="form register-form">
                    <form onSubmit={handleRegisterSubmit}>
                        <h2>Register</h2>
                        {error && !isLogin && <p className="error">{error}</p>}
                        <input
                            name="Name"
                            type="text"
                            placeholder="Full Name"
                            value={registerData.Name}
                            onChange={handleRegisterChange}
                            required
                        />
                        <input
                            name="Email"
                            type="email"
                            placeholder="Email"
                            value={registerData.Email}
                            onChange={handleRegisterChange}
                            required
                        />
                        <input
                            name="Password"
                            type="password"
                            placeholder="Password"
                            value={registerData.Password}
                            onChange={handleRegisterChange}
                            required
                        />
                        <input
                            name="Birthdate"
                            type="date"
                            value={registerData.Birthdate}
                            onChange={handleRegisterChange}
                            required
                        />
                        <label htmlFor="fileUpload" className="file-upload-label">
                            Upload Profile Image
                        </label>
                        <input
                            id="fileUpload"
                            type="file"
                            name="Picture"
                            onChange={handleFileChange}
                        />

                        <button type="submit" disabled={registerLoading}>
                            Register {registerLoading && <span className="spinner"></span>}
                        </button>
                    </form>
                </div>

                {/* Overlay Panel */}
                <div className="overlay-container">
                    <div className="overlay">
                        <div className="overlay-panel overlay-left">
                            <h3>Already have an account?</h3>
                            <button className="ghost" onClick={handleToggle}>
                                Login
                            </button>
                        </div>
                        <div className="overlay-panel overlay-right">
                            <h3>New here?</h3>
                            <button className="ghost" onClick={handleToggle}>
                                Register
                            </button>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default AuthForm;
