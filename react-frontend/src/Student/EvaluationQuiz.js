import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import "../css/evaluationQuiz.css";
import "../css/layout.css";
import "../css/loading.css";

const EvaluationQuiz = () => {
    const [quiz, setQuiz] = useState(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [score, setScore] = useState(0);
    const [selectedAnswer, setSelectedAnswer] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const quizUrl = "http://localhost:5168/Json/Quiz/EvaluationQuiz.json";

    const cleanJsonString = (str) => {
        // Remove BOM and trim spaces
        return str.trim().replace(/^\uFEFF/, "");
    };

    useEffect(() => {
        async function fetchQuiz() {
            try {
                setLoading(true);
                const res = await fetch(quizUrl);
                const text = await res.text();
                console.log("Raw JSON text:", JSON.stringify(text)); // Debug output
                const cleanText = cleanJsonString(text);
                const data = JSON.parse(cleanText);
                setQuiz(data);
                setLoading(false);
            } catch (err) {
                setError(err.message || "Something went wrong.");
                setLoading(false);
            }
        }
        fetchQuiz();
    }, []);

    const handleAnswerSelect = (answer) => {
        setSelectedAnswer(answer);
    };

    const determineLevel = (score, total) => {
        const percentage = (score / total) * 100;
        if (percentage >= 90) return "C2";
        if (percentage >= 80) return "C1";
        if (percentage >= 65) return "B2";
        if (percentage >= 50) return "B1";
        if (percentage >= 30) return "A2";
        return "A1";
    };

    const handleNext = async () => {
        if (selectedAnswer === null) {
            alert("Please select an answer.");
            return;
        }

        const isCorrect = selectedAnswer === quiz.questions[currentIndex].correct;
        if (isCorrect) setScore((prev) => prev + 1);

        if (currentIndex + 1 < quiz.questions.length) {
            setCurrentIndex(currentIndex + 1);
            setSelectedAnswer(null);
        } else {
            const finalScore = isCorrect ? score + 1 : score;
            const level = determineLevel(finalScore, quiz.questions.length);

            try {
                const res = await fetch(
                    `http://localhost:5168/api/studentapi/setlevel?level=${level}`,
                    {
                        method: "POST",
                        credentials: "include",
                    }
                );
                if (!res.ok) {
                    throw new Error("Failed to save level.");
                    console.log(res);
                }
            } catch (err) {
                alert("Failed to save level: " + err.message);
            }

            navigate("/dashboard");
        }
    };

    if (loading)
        return (
            <div className="loading-container">
                <div className="spinner"></div>
                <p>Loading...</p>
            </div>
        );
    if (error) return <p>Error: {error}</p>;
    if (!quiz) return null;

    const question = quiz.questions[currentIndex];

    return (
        <div className="evaluation-quiz-container">
            <h2>{quiz.lesson}</h2>
            <p>
                Question {currentIndex + 1} of {quiz.questions.length}
            </p>
            <h3>{question.question}</h3>

            <ul className="answer-list">
                {question.answers.map((ans) => (
                    <li key={ans}>
                        <label
                            className={
                                selectedAnswer === ans ? "answer-label selected" : "answer-label"
                            }
                        >
                            <input
                                type="radio"
                                name="answer"
                                value={ans}
                                checked={selectedAnswer === ans}
                                onChange={() => handleAnswerSelect(ans)}
                            />
                            {ans}
                        </label>
                    </li>
                ))}
            </ul>

            <button className="next-btn" onClick={handleNext}>
                {currentIndex + 1 === quiz.questions.length ? "Finish" : "Next"}
            </button>
        </div>
    );
};

export default EvaluationQuiz;
