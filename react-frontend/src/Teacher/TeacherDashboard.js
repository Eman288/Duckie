import React, { useState, useEffect } from 'react';
import { Link } from "react-router-dom";
import "../css/teacher.css";
import "../css/layout.css";
import "../css/loading.css";

const TeacherDashboard = () => {
    const [user, setUser] = useState(null);
    const [courses, setCourses] = useState([]);
    const [purchases, setPurchases] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [activeSection, setActiveSection] = useState("courses");

    const fetchCourses = async () => {
        try {
            setLoading(true);
            const res = await fetch("http://localhost:5168/api/teacherapi/my-courses", { credentials: "include" });
            const data = await res.json();
            setCourses(data);
            setActiveSection("courses");
        } catch (err) {
            setError("Failed to fetch courses");
        } finally {
            setLoading(false);
            console.log(courses);
        }
    };

    const fetchPurchases = async () => {
        try {
            setLoading(true);
            const res = await fetch("http://localhost:5168/api/teacherapi/my-sells", { credentials: "include" });
            const data = await res.json();
            setPurchases(data);
            setActiveSection("purchases");
        } catch (err) {
            setError("Failed to fetch purchases");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const stored = sessionStorage.getItem("teacherData");
        if (stored) {
            setUser(JSON.parse(stored));
        }
        fetchCourses();
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
                    <Link to="/teacher/profile" className="pro">
                        <img className="logo" src="/images/Home/profile.png" alt="Profile" />
                    </Link>
                </div>
            </nav>

            <div className="dashboard-container">
                <h2 className="dashboard-heading">Welcome Teacher, {user.name} <span>🎓</span></h2>

                <div className="dashboard-nav-links">
                    <button onClick={fetchCourses}>My Courses</button>
                    <button onClick={fetchPurchases}>My Purchases</button>
                </div>

                {loading && <p className="dashboard-loading">Loading...</p>}

                <div className="dashboard-section">
                    {activeSection === "courses" && (
                        <>
                            <Link to="/teacher/courseCreate" className="dashboard-create-btn">+ Add Course</Link>
                            <div className="course-grid">
                                {courses.length > 0 ? courses.map(course => (
                                    <div key={course.id} className="course-card">
                                        <img src={encodeURI(`http://localhost:5168${course.picture}`)} />
                                        <h3>{course.title}</h3>
                                        <p>{course.description?.slice(0, 80)}...</p>
                                        <Link to={`/teacher/courseView/${course.id}`} className="explore-btn">Explore</Link>
                                    </div>
                                )) : <p className="dashboard-empty">No courses found.</p>}
                            </div>
                        </>
                    )}

                    {activeSection === "purchases" && (
                        <>
                            {purchases.length > 0 ? (
                                <table className="dashboard-table">
                                    <thead>
                                        <tr>
                                            <th>Purchase ID</th>
                                            <th>Student</th>
                                            <th>Course</th>
                                            <th>Date</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {purchases.map(p => (
                                            <tr key={p.id}>
                                                <td>{p.id}</td>
                                                <td>{p.student?.name || p.studentId}</td>
                                                <td>{p.course?.title || p.courseId}</td>
                                                <td>{new Date(p.purchaseDate).toLocaleDateString()}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : <p className="dashboard-empty">No purchases found.</p>}
                        </>
                    )}
                </div>
            </div>
        </div>
    );
};

export default TeacherDashboard;
