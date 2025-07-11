import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "../css/layout.css";
import "../css/leaderboard.css";
import { useNavigate } from "react-router-dom";

const Leaderboard = () => {
    const [students, setStudents] = useState([]);
    const [error, setError] = useState("");
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const navigate = useNavigate();

    const getUserData = () => {
        const data = sessionStorage.getItem("user");
        return data ? JSON.parse(data) : null;
    };

    useEffect(() => {
        const user = getUserData();
        setIsLoggedIn(user !== null);

        const fetchLeaderboard = async () => {
            try {
                const response = await fetch("http://localhost:5168/api/studentapi/leaderboard", {
                    method: "GET",
                    credentials: "include",
                });

                if (!response.ok) {
                    const data = await response.json();
                    throw new Error(data.error || "Failed to fetch leaderboard");
                }

                const data = await response.json();
                setStudents(data);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchLeaderboard();
    }, []);

    return (

        <div>
            {/* Navigation */}
            {isLoggedIn ? (
                <nav className="nav1">
                    <img src="/images/Layout/Icon.png" alt="Logo" />
                    <ul>
                        <li><Link to="/">Home</Link></li>
                        <li><a href="#">About Us</a></li>
                        <li><Link to="/dashboard">Dashboard</Link></li>
                        <li><Link to="/leaderboard">Leader Board</Link></li>
                    </ul>
                    <div className="buttons">
                        <Link to="/profile" className="pro"><img className="logo" src="/images/Home/profile.png" alt="Profile" /></Link>
                    </div>
                </nav>
            ) : (
                <nav className="nav2">
                    <div className="img">
                        <img src="/images/Layout/Icon.png" alt="Logo" />
                    </div>
                    <ul>
                        <li><Link to="/">Home</Link></li>
                        <li><a href="#">About Us</a></li>
                    </ul>
                    <div className="buttons">
                        <Link to="/authform" className="register">Join Us</Link>
                    </div>
                </nav>
            )}
        <div className="leaderboard-page">
            <button onClick={() => navigate("/dashboard")} className="back-button">← Back to Dashboard</button>

            <h2>🏆 Leaderboard</h2>
            {error && <p className="error">{error}</p>}
            <div className="leaderboard-table">
                <table>
                    <thead>
                        <tr>
                            <th>Rank</th>
                            <th>Name</th>
                            <th>Total Points</th>
                        </tr>
                    </thead>
                    <tbody>
                        {students.map((student, index) => (
                            <tr key={student.id}>
                                <td>
                                    {index === 0 ? (
                                        <img src="/images/Student/1st-place.png" alt="1st" className="medal" />
                                    ) : index === 1 ? (
                                        <img src="/images/Student/2st-place.png" alt="2nd" className="medal" />
                                    ) : index === 2 ? (
                                        <img src="/images/Student/3st-place.png" alt="3rd" className="medal" />
                                    ) : (
                                        index + 1
                                    )}
                                </td>
                                <td>{student.name}</td>
                                <td>{student.totalPoints}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
        </div>

    );
};

export default Leaderboard;
