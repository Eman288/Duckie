import React, { useEffect, useState } from "react";
import { useParams,Link, useNavigate } from "react-router-dom";

import "../css/course.css";
import "../css/layout.css";
import "../css/loading.css";

const CourseView = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [course, setCourse] = useState(null);
    const [error, setError] = useState("");
    const [editMode, setEditMode] = useState({ title: false, description: false, image: false });
    const [editData, setEditData] = useState({ title: "", description: "", image: null });

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const response = await fetch(`http://localhost:5168/api/teacherapi/view-course/${id}`, {
                    credentials: "include",
                });

                const data = await response.json();
                if (!response.ok) throw new Error(data.message || "Failed to fetch course");

                setCourse(data);
                setEditData({ title: data.title, description: data.description, image: null });
            } catch (err) {
                setError(err.message);
            }
        };

        fetchCourse();
    }, [id]);

    const handleAddContent = () => {
        navigate(`/teacher/courseContentCreate/${id}`);
    };

    const handlePublish = async () => {
        try {
            const res = await fetch(`http://localhost:5168/api/teacherapi/Active?courseId=${id}`, {
                method: "POST",
                credentials: "include",
            });
            const data = await res.json();
            if (!res.ok) throw new Error(data.message || "Failed to publish course");

            setCourse((prev) => ({ ...prev, isActive: true }));
            alert("Course published!");
        } catch (err) {
            alert(err.message);
        }
    };

    const handleEditToggle = (field) => {
        setEditMode((prev) => ({ ...prev, [field]: !prev[field] }));
    };

    const handleUpdate = async () => {
        const formData = new FormData();
        formData.append("Id", id);
        formData.append("Title", editData.title);
        formData.append("Description", editData.description);
        if (editData.image) {
            formData.append("Picture", editData.image);
        }

        try {
            const response = await fetch("http://localhost:5168/api/teacherapi/update-course", {
                method: "PUT",
                body: formData,
                credentials: "include",
            });

            const data = await response.json();
            if (!response.ok) throw new Error(data.message);

            setCourse((prev) => ({
                ...prev,
                title: editData.title,
                description: editData.description,
                picture: editData.image ? URL.createObjectURL(editData.image) : prev.picture,
            }));
            alert("Course updated successfully!");
            setEditMode({ title: false, description: false, image: false });
        } catch (err) {
            alert("Update failed: " + err.message);
        }
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
                    <Link to="/teacher/profile" className="pro">
                        <img className="logo" src="/images/Home/profile.png" alt="Profile" />
                    </Link>
                </div>
            </nav>

             <div className="course-container">
            <div className="course-header">
                <div className="image-section">
                    {editMode.image ? (
                        <input type="file" onChange={(e) => setEditData({ ...editData, image: e.target.files[0] })} />
                    ) : (
                            <img src={`http://localhost:5168${course.picture}`} alt="Course" className="course-image" />
                    )}
                    <button onClick={() => handleEditToggle("image")} className="edit-icon">
                        <img src="/images/Student/pen.png" alt="edit" width="18" />
                    </button>
                </div>

                <div className="course-details">
                    {editMode.title ? (
                        <input
                            type="text"
                            value={editData.title}
                            onChange={(e) => setEditData({ ...editData, title: e.target.value })}
                        />
                    ) : (
                        <h1>{course.title}</h1>
                    )}
                    <button onClick={() => handleEditToggle("title")} className="edit-icon">
                        <img src="/images/Student/pen.png" alt="edit" width="18" />
                    </button>

                    {editMode.description ? (
                        <textarea
                            value={editData.description}
                            onChange={(e) => setEditData({ ...editData, description: e.target.value })}
                        />
                    ) : (
                        <p className="course-description">{course.description}</p>
                    )}
                    <button onClick={() => handleEditToggle("description")} className="edit-icon">
                        <img src="/images/Student/pen.png" alt="edit" width="18" />
                    </button>

                    <p className="course-price">${course.price}</p>

                    <div className="course-actions">
                        <button className="publish-btn" onClick={handlePublish} disabled={course.isActive}>
                            {course.isActive ? "Published" : "Publish"}
                        </button>
                        <button className="add-content-btn" onClick={handleAddContent}>+ Add Content</button>
                        <button className="update-btn" onClick={handleUpdate}>Update</button>
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
                                    <button
                                        className="content-edit-btn"
                                        onClick={() => navigate(`/teacher/courseContentUpdate/${content.id}`)}
                                        title="Edit Content"
                                    >
                                        <img src="/images/Student/pen.png" alt="edit" width="14" />
                                    </button>
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
