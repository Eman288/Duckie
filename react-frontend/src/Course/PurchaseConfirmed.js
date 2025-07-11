import React from "react";
import { Link } from "react-router-dom";
import "../css/course.css"; // or create a new css file if you'd like

const PurchaseConfirmed = () => {
    return (
        <div className="confirmed-container">
            <h1>🎉 Purchase Confirmed!</h1>
            <img
                src="/images/Home/happy_duck.png"
                alt="Confirmed Illustration"
                className="confirmed-img"
            />
            <p>Thank you for your purchase. Enjoy your new course!</p>
            <Link to="/dashboard" className="back-link">
                Back to Dashboard
            </Link>
        </div>
    );
};

export default PurchaseConfirmed;
