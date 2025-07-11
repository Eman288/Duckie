import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";

import "../css/profile.css";
import "../css/layout.css";
import "../css/loading.css";

const Profile = () => {
    const [error, setError] = useState("");
    const [user, setUser] = useState(null);
    const navigate = useNavigate();
    const [editMode, setEditMode] = useState({});
    const [formData, setFormData] = useState({});
    const [loading, setLoading] = useState(false);
    const [isLoggedIn, setIsLoggedIn] = useState(false);


    const fetchProfile = async () => {
        try {
            setError("");
            setLoading(true);

            const response = await fetch("http://localhost:5168/api/adminapi/profile", {
                method: "POST",
                credentials: "include"
            });

            const text = await response.text();
            const data = JSON.parse(text);

            if (!response.ok) throw new Error(data.error || "Failed to load data");

            setUser(data.user);
            setFormData(data.user); // prefill formData
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const getUserData = () => {
        const data = sessionStorage.getItem("user");
        return data ? JSON.parse(data) : null;
    };

    useEffect(() => {
        const user = getUserData();
        setIsLoggedIn(user !== null);
        fetchProfile();
    }, []);

    if (error) {
        if (error === "No User is logged in") {
            navigate("/authform");
        } else {
            return <p>Error: {error}</p>;
        }
    }

    if (loading || !user) return <p>Loading...</p>;

    const handleEditToggle = (field) => {
        setEditMode((prev) => ({ ...prev, [field]: !prev[field] }));
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleUpdate = async () => {
        try {
            const response = await fetch("http://localhost:5168/api/adminapi/update", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(formData)
            });

            const data = await response.json();
            if (!response.ok) throw new Error(data.error || "Failed to update");

            alert("Profile updated!");
            setEditMode({});
            setUser(formData);
        } catch (err) {
            alert("Error: " + err.message);
        }
    };

    const handleOutput = async () => {

        try {
            const response = await fetch("http://localhost:5168/api/adminapi/logout", {
                method: "POST",
                credentials: "include"
            });

            const data = await response.json();
            if (!response.ok) throw new Error(data.error || "Failed to logout");

            if (data.message === "User Logged Out!") {
                sessionStorage.removeItem("user"); // delete the session
                navigate("/");
            }

        }
        catch (err) {
            alert("Error: " + err.message);
        }
    };

    const Field = ({ label, field, type = "text" }) => (
        <div className="profile-field">
            <label>{label}</label>
            {editMode[field] ? (
                <input
                    type={type}
                    name={field}
                    value={formData[field] || ""}
                    onChange={handleChange}
                />
            ) : (
                <span>{user[field]}</span>
            )}
            <img
                src="/images/Student/pen.png"
                alt="Edit"
                className="edit-icon"
                onClick={() => handleEditToggle(field)}
            />
        </div>
    );

    return (
        <div>
            {/* Navigation */}
            {
                isLoggedIn ? (
                    <nav className="nav1">
                        <img src="/images/Layout/Icon.png" alt="Logo" />
                        <ul>
                            <li><Link to="/">Home</Link></li>
                            <li><a href="#">About Us</a></li>
                            <li><Link to="/dashboard">Dashboard</Link></li>
                            <li><Link to="/leaderboard">Leader Board</Link></li>
                        </ul>
                        <div className="buttons">
                            <Link to="/profile" className="pro"><img className="logo" src="/images/Home/profile.png" alt="Profile" /></Link>
                        </div>
                    </nav>
                ) : (
                    <nav className="nav2">
                        <div className="img">
                            <img src="/images/Layout/Icon.png" alt="Logo" />
                        </div>
                        <ul>
                            <li><Link to="/">Home</Link></li>
                            <li><a href="#">About Us</a></li>
                        </ul>
                        <div className="buttons">
                            <Link to="/authform" className="register">Join Us</Link>
                        </div>
                    </nav>
                )
            }
            <div className="profile-container">
                <h2>Your Profile</h2>
                <div className="profile-box">
                    <div className="profile-pic">
                        <img src={user.picUrl || "/images/Home/profile.png"} alt="Profile" />
                    </div>
                    <div className="profile-info">
                        <Field label="Name" field="name" />
                        <Field label="Email" field="email" />
                        <Field label="Password" field="password" type="password" />
                        <Field label="Birthday" field="birthday" type="date" />
                        <Field label="Join Date" field="joinDate" />
                        <Field label="Total Points" field="totalPoints" />
                    </div>
                </div>
                <button className="update-button" onClick={handleUpdate}>Update</button>
                <button className="logout-button" onClick={handleOutput}>Logout</button>

            </div>
        </div>
    );
};

export default Profile;
