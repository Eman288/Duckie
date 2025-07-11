import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "../css/course.css";
import "../css/layout.css";
import "../css/loading.css";

const CourseContentCreate = () => {
    const { id } = useParams(); // course ID from URL
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        Title: "",
        Content: null,
    });
    const [error, setError] = useState("");

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleFileChange = (e) => {
        setFormData({ ...formData, Content: e.target.files[0] });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = new FormData();
        data.append("CourseId", id);
        data.append("Title", formData.Title);
        data.append("Content", formData.Content);

        try {
            const response = await fetch("http://localhost:5168/api/teacherapi/add-courseContent", {
                method: "POST",
                body: data,
                credentials: "include",
            });

            const result = await response.json();

            if (!response.ok) setError(result.message || "Upload failed!");
            else {
                alert("Course content uploaded successfully!");
                navigate(`/teacher/courseView/${id}`);
            }
        } catch (err) {
            setError("Network error: " + err.message);
        }
    };

    return (
        <div className="unit-create-container">
            <h2 className="unit-create-heading">Add Course Content</h2>
            {error && <p className="error-message">{error}</p>}

            <form onSubmit={handleSubmit} className="unit-create-form">
                <div className="field-group">
                    <label>Content Title</label>
                    <input
                        type="text"
                        name="Title"
                        value={formData.Title}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div className="field-group file-field">
                    <label>Upload Content (.json)</label>
                    <label className="file-box">
                        {formData.Content ? formData.Content.name : "Click or drag JSON file here"}
                        <input
                            type="file"
                            name="Content"
                            accept=".json"
                            onChange={handleFileChange}
                            required
                        />
                    </label>
                </div>
                <button type="submit" className="create-unit-btn">Upload Content</button>
            </form>
        </div>
    );
};

export default CourseContentCreate;
