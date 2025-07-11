import React, { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";

import "../css/course.css";
import "../css/layout.css";
import "../css/loading.css";

const CourseView = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [course, setCourse] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const response = await fetch(`http://localhost:5168/api/teacherapi/view-course-student/${id}`, {
                    credentials: "include",
                });

                const data = await response.json();
                if (!response.ok) throw new Error(data.message || "Failed to fetch course");

                setCourse(data);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchCourse();
    }, [id]);


    const handleStartContent = (contentId) => {
        navigate(`/course/courseContent/${contentId}`);
    };

    if (error) return <p className="error-message">{error}</p>;
    if (!course) return (
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
                    <Link to="/profile" className="pro">
                        <img className="logo" src="/images/Home/profile.png" alt="Profile" />
                    </Link>
                </div>
            </nav>

            <div className="course-container">
                <div className="course-header">
                    <div className="image-section">
                        <img src={`http://localhost:5168${course.picture}`} alt="Course" className="course-image" />
                    </div>

                    <div className="course-details">
                        <h1>{course.title}</h1>
                        <p className="course-description">{course.description}</p>
                        <p className="course-price">${course.price}</p>

                        <div className="course-actions">
                            {!course.purchased && (
                                <Link to={`/course/coursePurchase/${course.id}`}>Purchase</Link>
                            )}
                        </div>
                    </div>
                </div>

                <div className="course-contents">
                    <h2>Course Contents</h2>
                    {course.contents && course.contents.length > 0 ? (
                        <ul className="course-content-list">
                            {course.contents.map((content) => (
                                <li key={content.id} className="course-content-item">
                                    <span>{content.title}</span>
                                    {course.purchased ? (
                                        <button
                                            className="start-content-btn"
                                            onClick={() => handleStartContent(content.id)}
                                            title="Start"
                                        >
                                            <img src="/images/Home/arrow.png" alt="start" width="20" />
                                        </button>
                                    ) : (
                                        <img
                                            src="/images/Home/lock.png"
                                            alt="locked"
                                            width="20"
                                            title="Locked"
                                        />
                                    )}
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p className="muted">No content added yet.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default CourseView;
