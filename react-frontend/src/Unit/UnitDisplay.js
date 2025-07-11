import React, { useState, useEffect } from 'react';
import { Link, useParams } from "react-router-dom";
import "../css/dashboard.css";
import "../css/home.css";
import "../css/layout.css";
import "../css/admin.css";
import "../css/loading.css";

const UnitDisplay = () => {
    const { id } = useParams();
    const [user, setUser] = useState(null);
    const [lessons, setLessons] = useState([]);
    const [unitId, setUnitId] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const fetchLessons = async (id) => {
        try {
            setLoading(true);
            setError("");

            const response = await fetch(`http://localhost:5168/api/adminapi/lessondisplay`, {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ UnitId: parseInt(id) })
            });

            const contentType = response.headers.get("content-type");
            if (!contentType || !contentType.includes("application/json")) {
                const raw = await response.text();
                throw new Error("Server did not return JSON. Response was:\n" + raw);
            }

            const data = await response.json();

            if (!response.ok) throw new Error(data.error || "Failed to load data");

            setUser(data.user);
            setUnitId(data.unitId);
            setLessons(data.lessons);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchLessons(id);
    }, [id]);

    if (error) return <p className="dashboard-error">Error: {error}</p>;
    if (!user) return (
        <div className="loading-container">
            <div className="spinner"></div>
            <p>Loading...</p>
        </div>
    );

    return (
        <div>
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
                <h2 className="dashboard-heading">Welcome {user.name}, here are the lessons</h2>

                <div className="dashboard-section">
                    <Link to={`/lesson/create/${unitId}`} className="dashboard-create-btn">
                        + Create a New Lesson
                    </Link>

                    {loading && <p className="dashboard-loading">Loading lessons...</p>}
                    {error && <p className="dashboard-error">Error: {error}</p>}

                    {lessons.length > 0 ? (
                        <table className="dashboard-table">
                            <thead>
                                <tr>
                                    <th>Id</th>
                                    <th>Title</th>
                                    <th colSpan={2}>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {lessons.map((lesson) => (
                                    <tr key={lesson.id}>
                                        <td>{lesson.id}</td>
                                        <td>{lesson.name}</td>
                                        <td>
                                            <Link to={`/lesson/update/${lesson.id}`} className="dashboard-action-btn update">Update</Link>
                                        </td>
                                        <td>
                                            <Link to={`/lesson/delete/${lesson.id}`} className="dashboard-action-btn delete">Delete</Link>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        !loading && <p className="dashboard-empty">No lessons found for this unit.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default UnitDisplay;
