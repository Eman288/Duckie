import React, { useState, useEffect } from 'react';
import { useLocation, Link } from "react-router-dom";
import "../css/quiz.css";

const QuizLesson = () => {
    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);
    const quizPath = searchParams.get("quizContent") || "";
    const lessonId = searchParams.get("lessonId"); // 👈 Extract lessonId

    // Extract lesson name from the quiz path
    const lessonName = quizPath.split("/").pop()?.replace(".json", "") || "unknown lesson";

    const [quizData, setQuizData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [questionQueue, setQuestionQueue] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selected, setSelected] = useState(null);
    const [correctAnswers, setCorrectAnswers] = useState(new Set());
    const [feedback, setFeedback] = useState(null);

    useEffect(() => {
        const fetchQuiz = async () => {
            try {
                const response = await fetch(`http://localhost:5168${quizPath}`);
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                const data = await response.json();
                setQuizData(data);
                setQuestionQueue(data.questions.map((q, i) => ({ ...q, originalIndex: i })));
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
                console.log(`Fetching Quiz Path: ${quizPath}`);
            }
        };
        if (quizPath) {
            fetchQuiz();
        }
    }, [quizPath]);

    const playSound = (type) => {
        const audio = new Audio(`/sounds/${type}.mp3`);
        audio.play();
    };
    const handleAnswer = (answer) => {
        if (selected !== null) return;

        setSelected(answer);
        const isCorrect = answer === questionQueue[currentIndex]?.correct;

        if (isCorrect) {
            setCorrectAnswers(prev => new Set(prev).add(questionQueue[currentIndex]?.originalIndex));
            setFeedback("correct");
            playSound("correct");
        } else {
            setFeedback("wrong");
            playSound("wrong");
            setQuestionQueue(prev => [...prev, questionQueue[currentIndex]]);
        }
    };
    const handleNext = () => {
        setSelected(null);
        setFeedback(null);
        setCurrentIndex(currentIndex + 1);
    };
    if (loading) return <div className="loading-container"><div className="spinner"></div><p>Loading...</p></div>;
    if (error) return <div>Error: {error}</div>;
    if (!questionQueue.length) return <div>No questions found.</div>;

    const currentQuestion = questionQueue[currentIndex];
    const allAnsweredCorrectly = correctAnswers.size === quizData.questions.length;

    return (
        <div className="Quiz-container">
            <div className="topBar-lesson">
                <div className="progress-bar">
                    <div className="bar" style={{ width: `${(correctAnswers.size / quizData.questions.length) * 100}%` }}></div>
                </div>
            </div>
            <div className="quizContent">
                <div className="question"><h2>{currentQuestion.question}</h2></div>
                {currentQuestion.answers.map((a, i) => {
                    let className = "quiz-answer";
                    if (selected) {
                        if (a === selected) {
                            const correct = a === currentQuestion.correct;
                            className += correct ? " correct" : " wrong";
                        } else if (a === currentQuestion.correct) {
                            className += " correct";
                        }
                    }
                    return (
                        <div key={i} className={className} onClick={() => handleAnswer(a)}>
                            <span>{a}</span>
                        </div>
                    );
                })}
            </div>
            {feedback && (
                <div className={`popup-feedback-fixed ${feedback}`}>
                    <div className="popup-feedback-text">
                        {feedback === "correct" ? "✅ Correct!" : "❌ Incorrect!"}
                    </div>
                    {currentIndex + 1 < questionQueue.length ? (
                        <button onClick={handleNext}>Next</button>
                    ) : allAnsweredCorrectly ? (
                        <Link
                            to={`/quizFinish/${encodeURIComponent(lessonId)}`} // 👈 Final link with lessonId
                            className="next-quiz-link"
                        >
                            Finish
                        </Link>
                    ) : (
                        <button onClick={handleNext}>Try Again</button>
                    )}
                </div>
            )}
        </div>
    );
};

export default QuizLesson;
