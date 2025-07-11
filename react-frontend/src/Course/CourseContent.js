import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import "../css/quiz.css"; // Reuse your quiz styles

const CourseContent = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [lessonData, setLessonData] = useState(null);
    const [error, setError] = useState("");
    const [questionQueue, setQuestionQueue] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [selected, setSelected] = useState(null);
    const [correctAnswers, setCorrectAnswers] = useState(new Set());
    const [feedback, setFeedback] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchLesson = async () => {
            try {
                const res = await fetch(`http://localhost:5168/api/teacherapi/view-courseContent/${id}`, {
                    credentials: "include",
                });
                const data = await res.json();
                if (!res.ok) throw new Error(data.message || "Failed to fetch course content.");

                const fullUrl = `http://localhost:5168${data.courseContent.contentUrl}`;
                const jsonRes = await fetch(fullUrl);
                const jsonData = await jsonRes.json();
                setLessonData(jsonData);

                const queue = jsonData.questions.map((q, i) => ({ ...q, originalIndex: i }));
                setQuestionQueue(queue);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchLesson();
    }, [id]);

    const playSound = (type) => {
        const audio = new Audio(`/sounds/${type}.mp3`);
        audio.play();
    };

    const handleAnswer = (answer) => {
        if (selected !== null) return;

        setSelected(answer);
        const isCorrect = answer === questionQueue[currentIndex]?.correct;

        if (isCorrect) {
            setCorrectAnswers((prev) => new Set(prev).add(questionQueue[currentIndex]?.originalIndex));
            setFeedback("correct");
            playSound("correct");
        } else {
            setFeedback("wrong");
            playSound("wrong");
            setQuestionQueue((prev) => [...prev, questionQueue[currentIndex]]);
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
    const allAnsweredCorrectly = correctAnswers.size === lessonData.questions.length;

    return (
        <div className="Quiz-container">
            <div className="topBar-lesson">
                <div className="progress-bar">
                    <div className="bar" style={{ width: `${(correctAnswers.size / lessonData.questions.length) * 100}%` }}></div>
                </div>
            </div>
            <div className="quizContent">
                <div className="question"><h2>{currentQuestion.question}</h2></div>
                {currentQuestion.answers.map((a, i) => {
                    let className = "quiz-answer";
                    if (selected) {
                        if (a === selected) {
                            className += a === currentQuestion.correct ? " correct" : " wrong";
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
                {currentQuestion.voiceUrl && (
                    <audio controls src={currentQuestion.voiceUrl}>
                        Your browser does not support audio.
                    </audio>
                )}
            </div>

            {feedback && (
                <div className={`popup-feedback-fixed ${feedback}`}>
                    <div className="popup-feedback-text">
                        {feedback === "correct" ? "✅ Correct!" : "❌ Incorrect!"}
                    </div>
                    {currentIndex + 1 < questionQueue.length ? (
                        <button onClick={handleNext}>Next</button>
                    ) : allAnsweredCorrectly ? (
                        <Link to="/course/courseContentFinished" className="next-quiz-link">
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

export default CourseContent;
