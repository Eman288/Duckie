import React from "react";
import { Link } from "react-router-dom";
import "../css/layout.css";
import "../css/loading.css";
import "../css/course.css";

const CourseContentFinished = () => {
    return (
        <div className="finish-lesson-container">
            <h1>🎉 Great Job!</h1>
            <img src="/images/Home/happy_duck.png" alt="Congrats" className="finish-duck" />
            <p>You completed this lesson successfully. Keep going!</p>
            <Link to="/dashboard" className="finish-btn">Back to Dashboard</Link>
        </div>
    );
};

export default CourseContentFinished;
