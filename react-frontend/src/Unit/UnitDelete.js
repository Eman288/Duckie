import React, { useState, useEffect } from 'react';
import { useParams } from "react-router-dom";
import "../css/layout.css";
import "../css/lesson.css";
import "../css/loading.css";

const UnitDelete = () => {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { id } = useParams();

    const deleteUnit = async () => {
        try {
            const response = await fetch(`http://localhost:5168/api/unitapi/DeleteUnit?id=${id}`, {
                method: "DELETE",
                credentials: "include"
            });

            if (!response.ok) {
                const result = await response.json().catch(() => ({}));
                console.error("Server error response:", result); // log details
                setError(result.error || result.message || "Unit deletion failed!");
                return;
            }
             else {
                alert("Unit deletion successful!");
                window.location.href = `/admin/dashboard`;
            }
        } catch (err) {
            setError("A network error occurred: " + err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        deleteUnit();
    }, []);

    return (
        <div className="loading-container">
            <div className="spinner"></div>
            <p>{loading ? "Deleting unit..." : error ? `Error: ${error}` : "Done"}</p>
        </div>
    );
};

export default UnitDelete;
