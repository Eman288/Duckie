import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from "react-router-dom";
import "../css/dashboard.css";
import "../css/home.css";
import "../css/layout.css";
import "../css/loading.css";

const Dashboard = () => {
    const [user, setUser] = useState(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [units, setUnits] = useState({});
    const [situations, setSituations] = useState([]);
    const [courses, setCourses] = useState([]);
    const [conversations, setConversations] = useState([]);
    const [dones, setDones] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [isSidebarOpen, setSidebarOpen] = useState(true);

    const navigate = useNavigate();

    const fetchDashboard = async (type) => {
        try {
            setLoading(true);
            setError("");
            const response = await fetch("http://localhost:5168/api/studentapi/dashboard", {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ type })
            });
            const text = await response.text();
            const data = JSON.parse(text);
            if (!response.ok) {
                throw new Error(data.error || "Failed to load data");
            }
            setUser(data.user);
            setDones(data.dones);
            setUnits(type === "units" ? data.units : {});
            setSituations(type === "situations" ? data.situations : []);
            setConversations(type === "conversations" ? data.conversations : []);
            setCourses(type === "courses" ? data.courses : []);
            console.log(situations);
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
        const userData = getUserData();
        setIsLoggedIn(userData !== null);
        fetchDashboard("units");
    }, []);

    const toggleSidebar = () => {
        setSidebarOpen(!isSidebarOpen);
    };

    if (error) {
        if (error === "No user is logged in") {
            navigate("/authform");
        } else {
            return <p>Error: {error}</p>;
        }
    }

    if (!user) {
        return (
            <div className="loading-container">
                <div className="spinner"></div>
                <p>Loading...</p>
            </div>
        );
    }

    return (
        <div>
            {isLoggedIn ? (
                <nav className="nav1">
                    <img src="/images/Layout/Icon.png" alt="Logo" loading="lazy" />
                    <ul>
                        <li><Link to="/">Home</Link></li>
                        <li><a href="#">About Us</a></li>
                        <li><Link to="/dashboard">Dashboard</Link></li>
                        <li><Link to="/leaderboard">Leader Board</Link></li>
                    </ul>
                    <div className="buttons">
                        <Link to="/profile" className="pro"><img className="logo" src="/images/Home/profile.png" alt="Profile" loading="lazy" /></Link>
                    </div>
                </nav>
            ) : (
                <nav className="nav2">
                    <div className="img"><img src={user.picture} alt="Logo" loading="lazy" /></div>
                    <ul>
                        <li><Link to="/">Home</Link></li>
                        <li><a href="#">About Us</a></li>
                    </ul>
                    <div className="buttons">
                        <Link to="/authform" className="register">Join Us</Link>
                    </div>
                </nav>
            )}

            <div className="dashboard">
                <aside id="sidebar" style={{ width: isSidebarOpen ? '20%' : '5%' }}>
                    <img
                        onClick={toggleSidebar}
                        id="toggleArrow"
                        src={isSidebarOpen ? "/images/Home/arrow.png" : "/images/Home/arrow - Copy.png"}
                        alt="Toggle Sidebar"
                        style={{ width: "24px" }}
                        loading="lazy"
                    />
                    <div id="OpenSide" style={{ display: isSidebarOpen ? 'flex' : 'none' }}>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("units"); }}>Learn</Link></div>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("situations"); }}>Situations</Link></div>
                        <div><a href={`http://localhost:5000/chat/${user.id}`}>Conversations</a></div>
                        <div><a href={`http://localhost:5000/chat/${user.id}`}>Talk</a></div>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("courses"); }}>Courses</Link></div>
                    </div>
                    <div id="CloseSide" style={{ display: isSidebarOpen ? 'none' : 'flex' }}>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("units"); }}><img src="/images/Home/open-book.png" alt="Learn" loading="lazy" /></Link></div>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("situations"); }}><img src="/images/Home/play-button.png" alt="Situations" loading="lazy" /></Link></div>
                        <div><a href={`http://localhost:5000/chat/${user.id}`}><img src="/images/Home/chat.png" alt="Conversations" loading="lazy" /></a></div>
                        <div><a href={`http://localhost:5000/chat/${user.id}`}><img src="/images/Home/talk.png" alt="Talk" loading="lazy" /></a></div>
                        <div><Link to="#" onClick={(e) => { e.preventDefault(); fetchDashboard("courses"); }}><img src="/images/Home/course.png" alt="Courses" loading="lazy" /></Link></div>
                    </div>
                </aside>

                <div className="container dash-body" style={{ width: isSidebarOpen ? '80%' : '95%' }}>
                    <h2>Welcome Back, {user.name}<span style={{ color: "var(--blue-second)" }}>❕</span></h2>
                    <p>Total Points: {user.totalPoints}</p>
                    <p>Current Level: {user.level}</p>

                    {loading && <p>Loading...</p>}

                    {Object.keys(units).length > 0 && (
                        <div>
                            <h3>Continue Your Journey</h3>
                            <div className="units">
                                {Object.entries(units).map(([level, unitList]) => (
                                    <div key={level} className="unit-group">
                                        <div className="split"><h1>{level}</h1></div>
                                        {unitList.map((unit) => (
                                            <div key={'u' + unit.id} className={`unit ${!unit.isDone ? 'locked' : ''}`}>
                                                <img
                                                    className="content-img"
                                                    src={encodeURI(`http://localhost:5168${unit.picture}`)}
                                                    alt={unit.name}
                                                    loading="lazy"
                                                />
                                                <div className="content-box">
                                                    <h4>{unit.name}</h4>
                                                    <p>{unit.lessons ? unit.lessons.length : 0}</p>
                                                </div>
                                                {unit.isDone ? (
                                                    <Link className="content-button" to={`/unit/${unit.id}`}>Explore</Link>
                                                ) : (
                                                    <div className="content-button locked-button">Locked</div>
                                                )}
                                            </div>
                                        ))}
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {situations.length > 0 && (
                        <div>
                            <h3>Learn By Watching & Listening</h3>
                            <div className="situations">
                                {situations.map((situation) => (
                                    <div key={'s' + situation.id} className="unit">
                                        <img
                                            className="content-img"
                                            src="/images/Home/sec2.jpg"
                                            alt={situation.name}
                                            loading="lazy"
                                        />
                                        <div className="content-box">
                                            <h4>{situation.name}</h4>
                                            <p>{situation.watched ? "Watched" : "Not Watched"}</p>
                                        </div>
                                        <Link className="content-button" to={`/situation/${situation.id}`}>View</Link>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {courses.length > 0 && (
                        <div>
                            <h3>Explore Our Courses</h3>
                            <div
                                className="situations"
                            >
                                {courses.map((course) => (
                                    <div key={'c' + course.id} className="unit">
                                        <img
                                            className="content-img"
                                            src={encodeURI(`http://localhost:5168${course.picture}`)}
                                            alt={course.title}
                                            loading="lazy"
                                        />
                                        <div className="content-box">
                                            <h4>{course.title}</h4>
                                            <p>${ course.price}</p>
                                        </div>
                                        <Link className="content-button" to={`/course/courseView/${course.id}`}>Explore</Link>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
