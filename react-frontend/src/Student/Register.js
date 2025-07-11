import React, { useState } from "react";
import "../css/layout.css";
import "../css/register.css";

const Register = () => {
    const [formData, setFormData] = useState({
        Name: "",
        Email: "",
        Password: "",
        Birthdate: "",
        Picture: null,
    });

    const [error, setError] = useState(null);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleFileChange = (e) => {
        setFormData({ ...formData, Picture: e.target.files[0] });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = new FormData();
        data.append("Name", formData.Name);
        data.append("Email", formData.Email);
        data.append("Password", formData.Password);
        data.append("Birthdate", formData.Birthdate);
        if (formData.Picture) {
            data.append("Picture", formData.Picture);
        }

        try {
            const response = await fetch("http://localhost:5168/api/studentapi/register", {
                method: "POST",
                body: data, 
            });

            const result = await response.json();
            if (!response.ok) {
                setError(result.message || "Registration failed!");
            } else {
                alert("Registration successful!");
                window.location.href = "/login";
            }
        } catch (error) {
            setError("A network error occurred.");
        }
    };


    return (
        <div className="main-container" style={{ backgroundImage: "url('/images/Student/reg.jpg')" } }>
            <div className="register-box">
                <h2>Register now and Start Learning</h2>
                {error && <p className="text-danger">{error}</p>}
                <form onSubmit={handleSubmit}>
                    <label htmlFor="Name">Name</label>
                    <input type="text" name="Name" placeholder="Enter your full name" value={formData.Name} onChange={handleChange} required />

                    <label htmlFor="Email">Email</label>
                    <input type="email" name="Email" placeholder="example@gmail.com" value={formData.Email} onChange={handleChange} required />

                    <label htmlFor="Password">Password</label>
                    <input type="password" name="Password" placeholder="Create a strong password" value={formData.Password} onChange={handleChange} required />

                    <label htmlFor="Birthdate">Birthdate</label>
                    <input type="date" name="Birthdate" value={formData.Birthdate} onChange={handleChange} required />

                    <label htmlFor="Picture">Profile Picture</label>
                    <input type="file" name="Picture" onChange={handleFileChange} />

                    <h4>Already have an account?</h4>
                    <a href="/login">Login</a>

                    <button type="submit" className="register-btn">Register</button>
                </form>
            </div>
        </div>
    );
};

export default Register;
