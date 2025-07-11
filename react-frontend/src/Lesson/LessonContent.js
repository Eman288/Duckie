import React, { useState, useEffect } from "react";
import { useLocation, Link } from "react-router-dom";
import "../css/lesson.css";

const LessonContent = () => {
    const [lessonData, setLessonData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [wordIndex, setWordIndex] = useState(0);

    const location = useLocation();

    useEffect(() => {
        const fetchLesson = async () => {
            try {
                const rawPath = location.pathname.replace("/lessonContent/", "");
                const decodedPath = decodeURIComponent(rawPath);
                console.log("Original pathname:", location.pathname);
                console.log("Raw path:", rawPath);
                console.log("Decoded path:", decodedPath);

                const response = await fetch(`http://localhost:5168/${decodedPath}`);
                if (!response.ok) {
                    throw new Error(`Lesson fetch error! Status: ${response.status}`);
                }
                const data = await response.json();
                setLessonData(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchLesson();
    }, [location.pathname]);

    // Get quiz path AND lessonId
    const searchParams = new URLSearchParams(location.search);
    const rawQuizPath = searchParams.get("quizContent");
    const quizPath = rawQuizPath ? decodeURIComponent(rawQuizPath) : "";
    const lessonId = searchParams.get("lessonId"); // Extract lessonId

    if (loading) {
        return (
            <div className="lesson-loading">
                <div className="spinner"></div>Loading...
            </div>
        );
    }
    if (error) {
        return <div className="lesson-error">Error: {error}</div>;
    }

    const word = lessonData.words[wordIndex];
    const handleNext = () => {
        if (wordIndex < lessonData.words.length - 1) {
            setWordIndex(wordIndex + 1);
        }
    };
    const speakWord = (text) => {
        const utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = "en-US";
        utterance.rate = 0.9;
        speechSynthesis.speak(utterance);
    };
    const progress = ((wordIndex + 1) / lessonData.words.length) * 100;

    return (
        <div className="lesson-wrapper">
            <div className="lesson-progress-container">
                <Link to="/dashboard">
                    <img src="/images/Lesson/exit.png" alt="X" />
                </Link>
                <div className="lesson-progress-bar">
                    <div
                        className="lesson-progress-fill"
                        style={{ width: `${progress}%` }}
                    />
                </div>
                <div className="lesson-progress-text">
                    {wordIndex + 1} / {lessonData.words.length}
                </div>
            </div>

            <div className="lesson-card">
                <div className="lesson-word-row">
                    <div>
                        <h2 className="lesson-en">{word.word}</h2>
                        <h2 className="lesson-ar">{word.arabic}</h2>
                    </div>
                    <button className="lesson-sound" onClick={() => speakWord(word.word)}>
                        <img src="/images/Lesson/sound.png" alt="Play sound" />
                    </button>
                </div>

                <div className="lesson-text-section">
                    <p><strong>Definition:</strong> <span>{word.definition}</span></p>
                    <p><strong>تعريف:</strong> <span>{word.arabic_definition}</span></p>
                    <p><strong>Example:</strong> <span>{word.example.en}</span></p>
                    <p><strong>مثال:</strong> <span>{word.example.ar}</span></p>
                </div>

                <div className="lesson-button-wrapper">
                    {wordIndex < lessonData.words.length - 1 ? (
                        <button className="lesson-next-btn" onClick={handleNext}>
                            Next
                        </button>
                    ) : (
                        <Link
                            className="lesson-next-btn"
                            to={`/quizLesson?quizContent=${encodeURIComponent(quizPath)}&lessonId=${lessonId}`}
                        >
                            Go to Quiz
                        </Link>
                    )}
                </div>
            </div>
        </div>
    );
};

export default LessonContent;
