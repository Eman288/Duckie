import React, { useState, useEffect } from 'react';
import { Link, useParams } from "react-router-dom";
import "../css/home.css";
import "../css/layout.css";
import "../css/lesson.css";
import "../css/dashboard.css";
import "../css/loading.css";

const Unit = () => {
    const [lessons, setLessons] = useState([]);
    const [unit, setUnit] = useState(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [error, setError] = useState("");
    const { id } = useParams();

    const fetchLesson = async (unitId) => {
        try {
            setLoading(true);
            setError("");
            const response = await fetch(`http://localhost:5168/api/lessonapi/lessons?unitId=${unitId}`, {
                method: "GET",
                credentials: "include",
            });
            const text = await response.text();
            const data = JSON.parse(text);
            if (!response.ok) throw new Error(data.message || "Failed to load data");

            setLessons(data.lessons);
            setUnit(data.unit);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (lessons.length > 0) {
            const doneCount = lessons.filter(lesson => lesson.isDone).length;
            const calculatedProgress = Math.round((doneCount / lessons.length) * 100);
            setProgress(calculatedProgress);
        }
    }, [lessons]);

    useEffect(() => {
        const circle = document.querySelector('.progress-ring__circle');
        const radius = 120;  // corrected from 60 to 120, as your SVG radius is 120
        const circumference = 2 * Math.PI * radius;
        const offset = circumference - (progress / 100) * circumference;

        if (circle) {
            circle.style.strokeDashoffset = offset;
        }
    }, [progress]);

    const getUserData = () => {
        const data = sessionStorage.getItem("user");
        return data ? JSON.parse(data) : null;
    };

    useEffect(() => {
        fetchLesson(id);
        const user = getUserData();
        setIsLoggedIn(user !== null);
    }, [id]);

    return (
        <div>
            {isLoggedIn ? (
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
            )}

            <div className="unitPage">
                <div
                    className="unitPage-bg"
                    style={{
                        backgroundImage: "url('/images/Unit/pattern.jpg')",
                        backgroundSize: "cover",
                        backgroundPosition: "center"
                    }}
                />
                <div className="unitPage-content">
                    <div><Link className="backunit" to="/dashboard">Back To Units</Link></div>

                    <div className="unit-top">
                        <div className="unit-des">
                            {unit && (
                                <>
                                    <img src={`http://localhost:5168${unit.picture}`} alt={unit.name} />
                                    <h1>{unit.name}</h1>
                                    <p>{unit.description}</p>
                                </>
                            )}
                        </div>

                        <div className="progress-wrapper">
                            <div className="progress-circle">
                                <svg className="progress-ring" width="260" height="260">
                                    <circle className="progress-ring__background" cx="130" cy="130" r="120" />
                                    <circle className="progress-ring__circle" cx="130" cy="130" r="120" />
                                </svg>
                                <div className="progress-label">{progress}%</div>
                            </div>
                        </div>
                    </div>

                    <div className="hrbox">
                        <div className="hrU1 hr"></div>
                        <div className="hrU2 hr"></div>
                        <div className="hrU3 hr"></div>
                    </div>

                    <div className="container">
                        {loading && <p>Loading...</p>}
                        {error && <p>Error: {error}</p>}

                        {lessons.length > 0 ? (
                            <table className="unitPage">
                                <thead>
                                    <tr>
                                        <th>Lesson</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {lessons.map((lesson) => (
                                        <tr
                                            key={'l' + lesson.id}
                                            className={lesson.isDone ? "" : "locked-lesson"}
                                        >
                                            <td>{lesson.name}</td>
                                            <td style={{ width: '25%' }}>
                                                {lesson.isDone ? (
                                                    <Link to={`/lessonContent${lesson.content}?quizContent=${lesson.quiz}&lessonId=${lesson.id}`}>
                                                        <img src="/images/Home/arrow.png" alt="Start Lesson" className="start-btn" />
                                                    </Link>
                                                ) : (
                                                    <>
                                                        <img src="/images/Home/lock.png" alt="Locked" className="locked-icon" />
                                                    </>
                                                )}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            !loading && <p>No lessons found for this unit.</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Unit;
