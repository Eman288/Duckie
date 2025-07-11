import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import "../css/course.css";
import "../css/layout.css";
import "../css/loading.css";

const CoursePurchase = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [course, setCourse] = useState(null);
    const [error, setError] = useState("");
    const [formData, setFormData] = useState({
        cardNumber: "",
        cvv: "",
        expireDate: "",
    });

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const res = await fetch(`http://localhost:5168/api/teacherapi/view-course-student/${id}`, {
                    credentials: "include",
                });
                const data = await res.json();
                if (!res.ok) throw new Error(data.message || "Failed to fetch course");

                setCourse(data);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchCourse();
    }, [id]);

    const handleChange = (e) => {
        setFormData((prev) => ({
            ...prev,
            [e.target.name]: e.target.value,
        }));
    };

    const handlePurchase = async () => {
        try {
            const payload = {
                courseId: course.id,
                cardNumber: formData.cardNumber,
                cvv: formData.cvv,
                expireDate: formData.expireDate,
                price: course.price,
            };

            const res = await fetch("http://localhost:5168/api/studentapi/purchase-course", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(payload),
            });

            const data = await res.json();
            if (!res.ok) throw new Error(data.message || "Purchase failed");

            navigate(`/course/purchaseConfirmed`);
        } catch (err) {
            alert("Purchase failed: " + err.message);
        }
    };

    if (error) return <p className="error-message">{error}</p>;
    if (!course) return (
        <div className="loading-container">
            <div className="spinner"></div>
            <p>Loading...</p>
        </div>
    );

    return (
        <div className="purchase-container">
            <h1>{course.title}</h1>
            <p>{course.description}</p>
            <p><strong>Price:</strong> ${course.price}</p>

            <div className="payment-form">
                <h3>Payment Details</h3>
                <input
                    type="text"
                    name="cardNumber"
                    placeholder="Card Number"
                    value={formData.cardNumber}
                    onChange={handleChange}
                />
                <input
                    type="text"
                    name="cvv"
                    placeholder="CVV"
                    value={formData.cvv}
                    onChange={handleChange}
                />
                <input
                    type="text"
                    name="expireDate"
                    placeholder="Expire Date (MM/YYYY)"
                    value={formData.expireDate}
                    onChange={handleChange}
                />
                <button onClick={handlePurchase}>Confirm Purchase</button>
            </div>

            <Link to="/dashboard" className="back-link">Back to Dashboard</Link>
        </div>
    );
};

export default CoursePurchase;
