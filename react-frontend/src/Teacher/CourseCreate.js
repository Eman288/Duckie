import React, { useState } from "react";
import "../css/course.css";
import "../css/layout.css";
import "../css/loading.css";


const CourseCreate = () => {
    const [formData, setFormData] = useState({
        Title: "",
        Description: "",
        Price: "",
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
        data.append("Title", formData.Title);
        data.append("Description", formData.Description);
        data.append("Price", formData.Price);
        data.append("Picture", formData.Picture);

        try {
            const response = await fetch("http://localhost:5168/api/teacherapi/add-course", {
                method: "POST",
                credentials: "include",
                body: data,
            });

            const result = await response.json();

            if (!response.ok) setError(result.message || "Course creation failed!");
            else {
                alert("Course created successfully!");
                window.location.href = "/teacher/dashboard"; // Change if needed
            }
        } catch (err) {
            setError("Network error: " + err.message);
        }
    };

    return (
        <div className="unit-create-container">
            <h2 className="unit-create-heading">Create a New Course</h2>
            {error && <p className="error-message">{error}</p>}

            <form onSubmit={handleSubmit} className="unit-create-form">
                <div className="row">
                    <div className="field-group">
                        <label>Course Title</label>
                        <input
                            type="text"
                            name="Title"
                            value={formData.Title}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="field-group">
                        <label>Price</label>
                        <input
                            type="number"
                            step="0.01"
                            name="Price"
                            value={formData.Price}
                            onChange={handleChange}
                            required
                        />
                    </div>
                </div>

                <div className="row">
                    <div className="field-group">
                        <label>Description</label>
                        <textarea
                            name="Description"
                            value={formData.Description}
                            onChange={handleChange}
                            required
                            placeholder="Enter course description"
                        />
                    </div>
                </div>

                <div className="row file-row">
                    <div className="field-group file-field">
                        <label>Course Picture</label>
                        <label className="file-box">
                            {formData.Picture ? formData.Picture.name : "Click or drag image here"}
                            <input
                                type="file"
                                name="Picture"
                                accept="image/*"
                                onChange={handleFileChange}
                                required
                            />
                        </label>
                    </div>
                </div>

                <button type="submit" className="create-unit-btn">Create Course</button>
            </form>
        </div>
    );
};

export default CourseCreate;
