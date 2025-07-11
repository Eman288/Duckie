import React, { useState, useEffect } from 'react';
import { Link } from "react-router-dom";
import "../css/home.css";
import "../css/layout.css";
import "../css/admin.css";
import "../css/loading.css";

const Dashboard = () => {
    const [user, setUser] = useState(null);
    const [units, setUnits] = useState([]);
    const [situations, setSituations] = useState([]);
    const [conversations, setConversations] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [activeSection, setActiveSection] = useState('units'); // "units" by default

    const fetchDashboard = async (type) => {
        setActiveSection(type); // update active section
        try {
            setLoading(true);
            setError("");

            const response = await fetch("http://localhost:5168/api/adminapi/dashboard", {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ type })
            });

            const text = await response.text();
            const isJson = response.headers.get("content-type")?.includes("application/json");
            if (!isJson) throw new Error("Server did not return JSON");

            const data = JSON.parse(text);
            if (!response.ok) throw new Error(data.error || "Failed to load data");

            setUser(data.user);
            setUnits([]);
            setSituations([]);
            setConversations([]);
            if (type === "units") setUnits(data.units);
            else if (type === "situations") setSituations(data.situations);
            else if (type === "conversations") setConversations(data.conversations);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDashboard('units'); // load units by default
    }, []);

    if (error) return <p className="dashboard-error">Error: {error}</p>;
    if (!user) return <div className="loading-container"><div className="spinner"></div><p>Loading...</p></div>;

    return (
        <div className="dashboard-wrapper">
            <nav className="nav1">
                <img src="/images/Layout/Icon.png" alt="Logo" />
                <ul>
                    <li><Link to="/">Home</Link></li>
                    <li><a href="#">About Us</a></li>
                </ul>
                <div className="buttons">
                    <Link to="/admin/profile" className="pro"><img className="logo" src="/images/Home/profile.png" alt="Profile" /></Link>
                </div>
            </nav>

            <div className="dashboard-container">
                <h2 className="dashboard-heading">Welcome Admin, {user.name} <span>❕</span></h2>

                <div className="dashboard-nav-links">
                    <Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard('units'); }}>Learn</Link>
                    <Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard('situations'); }}>Situations</Link>
                    <Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard('conversations'); }}>Conversations</Link>
                </div>

                {loading && <p className="dashboard-loading">Loading...</p>}

                <div className="dashboard-section">

                    {/* 🟡 Show Units Section */}
                    {activeSection === 'units' && (
                        <>
                            <Link to="/unit/create" className="dashboard-create-btn">
                                + Create a New Unit
                            </Link>
                            {units.length > 0 ? (
                                <table className="dashboard-table">
                                    <thead>
                                        <tr>
                                            <th>Id</th>
                                            <th>Title</th>
                                            <th>Level</th>
                                            <th colSpan={2}>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {units.map((unit) => (
                                            <tr key={unit.id}>
                                                <td>{unit.id}</td>
                                                <td><Link to={`/unit/display/${unit.id}`}>{unit.name}</Link></td>
                                                <td>{unit.level}</td>
                                                <td><Link to={`/unit/update/${unit.id}`} className="dashboard-action-btn update">Update</Link></td>
                                                <td><Link to={`/unit/delete/${unit.id}`} className="dashboard-action-btn delete">Delete</Link></td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : <p className="dashboard-empty">No units found.</p>}
                        </>
                    )}

                    {/* 🟡 Show Situations Section */}
                    {activeSection === 'situations' && (
                        <>
                            <Link to="/situation/create" className="dashboard-create-btn">
                                + Create a New Situation
                            </Link>
                            {situations.length > 0 ? (
                                <table className="dashboard-table">
                                    <thead>
                                        <tr>
                                            <th>Id</th>
                                            <th>Name</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {situations.map((situation) => (
                                            <tr key={situation.id}>
                                                <td>{situation.id}</td>
                                                <td>{situation.name}</td>
                                                <td><Link to={`/situation/display/${situation.id}`} className="dashboard-action-btn view">View</Link></td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : <p className="dashboard-empty">No situations found.</p>}
                        </>
                    )}

                    {/* 🟡 Show Conversations Section */}
                    {activeSection === 'conversations' && (
                        <>
                            {conversations.length > 0 ? (
                                <ul className="dashboard-list">
                                    {conversations.map((conversation) => (
                                        <li key={conversation.id}>{conversation.name}</li>
                                    ))}
                                </ul>
                            ) : <p className="dashboard-empty">No conversations found.</p>}
                        </>
                    )}

                </div>
            </div>
        </div>
    );
};

export default Dashboard;
