import React, { useState, useEffect } from 'react';
import { useParams } from "react-router-dom";
import "../css/home.css";
import "../css/layout.css";


const Lesson = () => {
	const[lessons, setLessons] = useState([]);
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState("");

    const fetchLesson = async (id) => {
        try {
            setLoading(true);
            setError("");

            const response = await fetch(`http://localhost:5168/api/lessonapi/lessons/${id}`, {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ type })
            });

            const text = await response.text();
            console.log("Raw Response:", text); // Debugging

            const data = JSON.parse(text);

            if (!response.ok) throw new Error(data.error || "Failed to load data");

            //  Ensure user is set
            setUser(data.lessons);



            //reset the data
            setLessons([]);

        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
	}

	const { id } = useParams();

	useEffect(() => {
		fetchLesson(id);
	}, []);


	return (
    //    {/* Lessons Section*/ }
    //    {
    //    lessons.length > 0 && (
    //        <div>
    //            <h3>Your Units</h3>
    //            <div className="lessons">
    //                {lessons.map((lesson) => (
    //                    <div key={'l' + lesson.id}>
    //                        <h4>{lesson.name}</h4>
    //                        <Link to={`/lessonContent/${lesson.id}`}>Start</Link>
    //                    </div>
    //                ))}
    //            </div>
    //        </div>
    //    )
        //}
    <h1>hi</h1>
	);
}

export default Lesson;