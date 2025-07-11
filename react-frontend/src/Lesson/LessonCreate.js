import React, { useState } from "react";
import { useParams } from "react-router-dom";

const LessonCreate = () => {
    const [formData, setFormData] = useState({
        Name: "",
        UnitId: "",
        Content: null,
        Quiz: null,
    });
    const [error, setError] = useState(null);
    const { unitId } = useParams();

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };
    const handleFileChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.files[0] });
    };
    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = new FormData();
        data.append("Name", formData.Name);
        data.append("UnitId", unitId);
        data.append("Content", formData.Content);
        data.append("Quiz", formData.Quiz);

        try {
            const response = await fetch("http://localhost:5168/api/lessonapi/CreateLesson", {
                method: "POST",
                credentials: "include",
                body: data,
            });
            const result = await response.json();

            if (!response.ok) {
                setError(result.message || "Lesson Creation failed!");
            } else {
                alert("Lesson Created successfully!");
                window.location.href = `/unit/display/${unitId}`;
            }
        } catch (error) {
            setError("A network error occurred: " + error);
        }
    };
    return (
        <div className="unit-create-container">
            <h2 className="unit-create-heading">Create New Lesson</h2>
            {error && <p className="error-message">{error}</p>}

            <form className="unit-create-form" onSubmit={handleSubmit}>
                <div className="row">
                    <div className="field-group">
                        <label htmlFor="Name">Lesson Name</label>
                        <input
                            type="text"
                            name="Name"
                            placeholder="Enter lesson name..."
                            value={formData.Name}
                            onChange={handleChange}
                            required
                        />
                    </div>
                </div>

                <div className="row">
                    <div className="field-group">
                        <label htmlFor="Content">Lesson Content (.json)</label>
                        <div className="file-box" onClick={() => document.getElementById("contentFile").click()}>
                            {formData.Content ? formData.Content.name : "Click to select lesson content (.json)"}
                        </div>
                        <input
                            id="contentFile"
                            type="file"
                            name="Content"
                            accept=".json"
                            onChange={handleFileChange}
                        />
                    </div>
                </div>

                <div className="row">
                    <div className="field-group">
                        <label htmlFor="Quiz">Lesson Quiz (.json)</label>
                        <div className="file-box" onClick={() => document.getElementById("quizFile").click()}>
                            {formData.Quiz ? formData.Quiz.name : "Click to select lesson quiz (.json)"}
                        </div>
                        <input
                            id="quizFile"
                            type="file"
                            name="Quiz"
                            accept=".json"
                            onChange={handleFileChange}
                        />
                    </div>
                </div>

                <button type="submit" className="create-unit-btn">
                    Create
                </button>
            </form>
        </div>
    );
};

export default LessonCreate;
