import React, { useState, useEffect } from 'react';
import { useParams, Link } from "react-router-dom";
import "../css/home.css";
import "../css/layout.css";
import "../css/quiz.css";
import "../css/loading.css";

const QuizFinish = () => {
    const { lessonId } = useParams(); // 👈 Now it's lessonId
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        console.log("🧪 lessonId param:", lessonId);

        const SaveData = async (lessonId) => {
            try {
                const response = await fetch(`http://localhost:5168/api/lessonapi/savelesson?lessonId=${lessonId}`, {
                    method: "GET",
                    credentials: "include",
                });
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        SaveData(lessonId);
    }, [lessonId]);

    if (loading) {
        return (
            <div className="loading-container">
                <div className="spinner"></div>
                <p> Loading...</p>
            </div>
        );
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <div className="finish-container">
            <p>Your Progress Has Been Saved (❁´◡`❁)</p>
            <h1> +20 Points ✨ </h1>
            <Link to={"/dashboard"}>Back To Units</Link>
            <img src="/images/Home/happy_duck.png" alt="finish" />
        </div>
    );
};

export default QuizFinish;
