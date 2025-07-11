import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ReactPlayer from "react-player";
import "../css/layout.css";
import "../css/situation.css";

const SituationPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [situation, setSituation] = useState(null);
    const [error, setError] = useState("");
    const [watched, setWatched] = useState(false);

    // Get logged-in user info from sessionStorage
    const student = JSON.parse(sessionStorage.getItem("user"));
    const studentId = student?.id;

    useEffect(() => {
        const fetchSituation = async () => {
            try {
                const response = await fetch(
                    `http://localhost:5168/api/situationapi/getsituationvideo/${id}`,
                    {
                        method: "GET",
                        credentials: "include",
                    }
                );
                console.log("Situation ID:", id);

                let data = null;
                try {
                    data = await response.json();
                } catch {
                    data = null;
                }

                if (!response.ok) {
                    throw new Error(data?.error || "Failed to fetch situation");
                }

                setSituation(data);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchSituation();
    }, [id]);

    const handleWatched = async () => {
        if (!studentId) {
            setError("User not logged in");
            return;
        }

        try {
            const response = await fetch(
                `http://localhost:5168/api/situationapi/watched`,
                {
                    method: "POST",
                    credentials: "include",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({ studentId: studentId, situationId: id }),
                }
            );

            let data = null;
            try {
                data = await response.json();
            } catch {
                data = null;
            }

            if (!response.ok) {
                throw new Error(data?.error || "Failed to mark as watched");
            }

            setWatched(true);
        } catch (err) {
            setError(err.message);
        }
    };

    if (error) return <p>Error: {error}</p>;
    if (!situation) return <p>Loading...</p>;

    return (
        <div
            className="situation-page"
            style={{ backgroundColor: "var(--color-bg)", minHeight: "100vh", padding: "20px" }}
        >
            <button
                onClick={() => navigate("/dashboard")}
                style={{
                    backgroundColor: "var(--color-accent)",
                    color: "black",
                    padding: "10px 20px",
                    border: "none",
                    borderRadius: "10px",
                    cursor: "pointer",
                    marginBottom: "20px",
                }}
            >
                ← Back to Dashboard
            </button>

            <div className="video-container" style={{ textAlign: "center" }}>
                <h2 style={{ color: "var(--color-primary)" }}>{situation.name}</h2>
                <ReactPlayer
                    url={situation.videoUrl}
                    controls
                    light={situation.pic}
                    width="100%"
                    height="500px"
                    style={{ maxWidth: "900px", margin: "auto" }}
                />
            </div>

            <div style={{ marginTop: "30px", textAlign: "center" }}>
                <button
                    onClick={handleWatched}
                    disabled={watched}
                    style={{
                        backgroundColor: watched ? "gray" : "var(--color-secondary)",
                        color: "white",
                        padding: "12px 30px",
                        fontSize: "16px",
                        border: "none",
                        borderRadius: "8px",
                        cursor: watched ? "not-allowed" : "pointer",
                        marginBottom: "30px",
                    }}
                >
                    {watched ? "Marked as Watched" : "Watched"}
                </button>
            </div>
        </div>
    );
};

export default SituationPage;
