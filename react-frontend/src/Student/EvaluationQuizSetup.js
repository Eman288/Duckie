import React from "react";
import { useNavigate } from "react-router-dom";
import "../css/evaluationQuiz.css";

const EvaluationQuizSetup = () => {
    const navigate = useNavigate();

    const handleStartFromBeginning = async () => {
        try {
            const response = await fetch(
                "http://localhost:5168/api/studentapi/setlevel?level=A1",
                {
                    method: "POST",
                    credentials: "include",
                }
            );
            if (!response.ok) {
                const errorData = await response.json();
                alert(errorData.message + '\n\n' + errorData.error || "Error setting level");
                console.error(errorData.error);
                return;
            }
            navigate("/dashboard");
        } catch (error) {
            console.error(error.message);
            alert("An error occurred while setting the level.");
        }
    };


    const handleTakeExam = () => {
        navigate("/evaluationQuiz");
    };
    return (
        <div className="eval-setup">
            <div className="eval-left">
                <h1>Welcome to Duckie!</h1>
                <p>We are glad you are here. Choose how you would like to start:</p>
                <div className="eval-image">
                    <img src="/images/Home/duck.png" alt="Welcome illustration" />
                </div>
            </div>
            <div className="eval-right">
                <button className="option-button" onClick={handleStartFromBeginning}>
                    Start from the Beginning
                </button>
                <button className="option-button" onClick={handleTakeExam}>
                    Take the Evaluation Exam
                </button>
            </div>
        </div>
    );
};

export default EvaluationQuizSetup;
