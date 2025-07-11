import React, { useState } from "react";
import "../css/unit.css";

const UnitCreate = () => {
    const [formData, setFormData] = useState({
        Name: "",
        Level: "",
        QuizFile: null,
        UnitImageFile: null,
        Description: "",
        OrderWithInLevel: 0,
    });

    const [error, setError] = useState(null);

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });
    const handleFileChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.files[0] });

    const handleSubmit = async (e) => {
        e.preventDefault();
        const data = new FormData();
        Object.keys(formData).forEach((k) => data.append(k, formData[k]));

        try {
            const response = await fetch("http://localhost:5168/api/unitapi/CreateUnit", {
                method: "POST",
                credentials: "include",
                body: data,
            });

            const result = await response.json();

            if (!response.ok) setError(result.message || "Unit Creation failed!");
            else {
                alert("Unit Creation successful!");
                window.location.href = "/admin/dashboard";
            }
        } catch (error) {
            setError("A network error occurred: " + error.message);
        }
    };

    return (
        <div className="unit-create-container">
            <h2 className="unit-create-heading">Create a New Unit</h2>
            {error && <p className="error-message">{error}</p>}

            <form onSubmit={handleSubmit} className="unit-create-form">
                {/* Row 1 */}
                <div className="row">
                    <div className="field-group">
                        <label>Name</label>
                        <input type="text" name="Name" value={formData.Name} onChange={handleChange} required />
                    </div>
                    <div className="field-group">
                        <label>Level</label>
                        <select name="Level" value={formData.Level} onChange={handleChange} required>
                            <option value="">Select Level</option>
                            <option value="A1">A1</option>
                            <option value="A2">A2</option>
                            <option value="B1">B1</option>
                            <option value="B2">B2</option>
                            <option value="C1">C1</option>
                            <option value="C2">C2</option>
                        </select>
                    </div>
                </div>

                {/* Row 2 */}
                <div className="row">
                    <div className="field-group">
                        <label>Order Within Level</label>
                        <input
                            type="number"
                            name="OrderWithInLevel"
                            value={formData.OrderWithInLevel}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="field-group">
                        <label>Description</label>
                        <textarea
                            name="Description"
                            value={formData.Description}
                            onChange={handleChange}
                            required
                            placeholder="Enter unit description"
                        />
                    </div>
                </div>

                {/* Row 3 */}
                <div className="row file-row">
                    <div className="field-group file-field">
                        <label>Quiz JSON</label>
                        <label className="file-box">
                            {formData.QuizFile ? formData.QuizFile.name : "Click or Drag Quiz file here"}
                            <input type="file" name="QuizFile" accept=".json" onChange={handleFileChange} required />
                        </label>
                    </div>
                    <div className="field-group file-field">
                        <label>Unit Image</label>
                        <label className="file-box">
                            {formData.UnitImageFile ? formData.UnitImageFile.name : "Click or Drag Image here"}
                            <input type="file" name="UnitImageFile" accept="image/*" onChange={handleFileChange} required />
                        </label>
                    </div>
                </div>

                <button type="submit" className="create-unit-btn">Create Unit</button>
            </form>
        </div>
    );
};

export default UnitCreate;
