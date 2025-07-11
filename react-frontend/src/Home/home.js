import React, { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import "../css/layout.css";
import "../css/home.css";
import "../css/loading.css";

const Home = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const hasAnimated = useRef(false);
    const statRefs = useRef([]);

    const getUserData = () => {
        const data = sessionStorage.getItem("user");
        return data ? JSON.parse(data) : null;
    };

    const sec1C = [
        "Personalized Learning Paths",
        "Real-Time Feedback and Correction",
        "Conversational AI Practice",
        "Adaptive Content Difficulty",
        "Multilingual Support",
        "Goal-Oriented Learning",
        "Context-Based Learning",
        "Voice Recognition and Pronunciation Scoring",
        "Memory-Optimized Review"
    ];

    useEffect(() => {
        const user = getUserData();
        setIsLoggedIn(user !== null);
        setIsLoading(false);
    }, []);

    useEffect(() => {
        const observer = new IntersectionObserver(
            (entries) => {
                if (!hasAnimated.current) {
                    for (let entry of entries) {
                        if (entry.isIntersecting) {
                            hasAnimated.current = true;
                            animateCounters();
                            break;
                        }
                    }
                }
            },
            { threshold: 0.3 }
        );

        statRefs.current.forEach(ref => {
            if (ref) observer.observe(ref);
        });

        return () => {
            statRefs.current.forEach(ref => {
                if (ref) observer.unobserve(ref);
            });
        };
    }, []);

    const animateCounters = () => {
        statRefs.current.forEach((counter) => {
            const target = +counter.getAttribute("data-target");
            let count = 0;
            const duration = 1500; // animation duration in ms
            const startTime = performance.now();

            const updateCount = (currentTime) => {
                const elapsed = currentTime - startTime;
                if (elapsed < duration) {
                    const progress = elapsed / duration;
                    count = Math.floor(progress * target);
                    counter.innerText = count.toLocaleString();
                    requestAnimationFrame(updateCount);
                } else {
                    counter.innerText = target.toLocaleString();
                }
            };

            requestAnimationFrame(updateCount);
        });
    };


    if (isLoading) {
        return (
            <div className="loading-container">
                <div className="spinner"></div>
                <p>Loading...</p>
            </div>
        );
    }

    return (
        <>
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

            {/* Main Content */}
            <main>
                <header className="header" style={{
                    backgroundImage: "url('/images/Home/pat.jpg')"
                }}>
                    <div className="header-text">
                        <h5>Welcome To Duckie!</h5>
                        <h1>Learn Any Language You Want With Duckie</h1>
                        <h4>Duckie is an AI-Powered English Learning platform created to make English Learning easy and fast</h4>
                        <a href="#">Learn More</a>
                    </div>
                    <div className="container header-img" style={{
                        backgroundImage: "url('/images/layout/header.jpg')",
                        backgroundSize: "cover",
                        backgroundPosition: "center"
                    }}>
                    </div>
                </header>

                {/* Section 1 */}
                <section className="home-sec1">
                    <div className="text">
                        <h6>Practice Advice</h6>
                        <h2>Make online education accessible</h2>
                        <p>Duckie has multiple reasons why it should be your first choice in learning English</p>
                    </div>
                    <div className="cards">
                        {["star", "circle", "conversation", "chart", "earth", "goal", "books", "headphones", "brain"].map((icon, index) => (
                            <div key={index} className={`sec1C${index}`}>
                                <img src={`/images/Home/${icon}.png`} alt="Icon" />
                                <h5>{sec1C[index]}</h5>
                            </div>
                        ))}
                    </div>
                </section>

                {/* Section 2 */}
                <section className="sec2">
                    <div className="container header-img" style={{
                        backgroundImage: "url('/images/Home/sec2.jpg')",
                        backgroundSize: "cover",
                        backgroundPosition: "center"
                    }}>
                    </div>
                    <div className="sec2-text">
                        <h2>Learn & Grow Your Skills From Anywhere</h2>
                        <p>Problems trying to resolve the conflict between the two major realms of Classical physics: Newtonian mechanics</p>
                        <div>
                            <div>
                                <h5>Online Lessons</h5>
                                <p>Learn any language you want while sitting in the comfort of your bed</p>
                            </div>
                            <div>
                                <h5>Constant Growth</h5>
                                <p>Master your skills with the different ways of practice in Duckie!</p>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Stats Section (Animated) */}
                <section className="stats-section" id="stats">
                    {[
                        { label: "Lessons & Counting", target: 6879 },
                        { label: "Courses & Videos", target: 1327 },
                        { label: "Certified Students", target: 1359 },
                        { label: "Registered Events", target: 1557 },
                    ].map((stat, index) => (
                        <div key={index} className="stat">
                            <h2
                                data-target={stat.target}
                                ref={(el) => (statRefs.current[index] = el)}
                            >
                                0
                            </h2>
                            <p>{stat.label}</p>
                        </div>
                    ))}
                </section>

                {/*Course Cards*/ }
                <section className="course-cards">
                    <div className="card">
                        <div className="text">
                            <p className="subtitle">POPULAR COURSES</p>
                            <h3>Get The Best Courses & Upgrade Your Skills</h3>
                            <Link to="/authform" className="join-btn">JOIN WITH US</Link>

                        </div>
                        <img src="/images/Home/person2.jpg" alt="Course" />
                    </div>
                    <div className="card dark">
                        <div className="text">
                            <p className="subtitle">POPULAR COURSES</p>
                            <h3>Get The Best Courses & Upgrade Your Skills</h3>
                            <Link to="/authform" className="join-btn">JOIN WITH US</Link>
                        </div>
                        <img src="/images/Home/person1.jpg" alt="Course" />
                    </div>
                </section>

                {/*Testimonial sction*/ }
                <section className="testimonial-section">
                    <div className="testimonial-wrapper">
                        <div className="testimonial-image">
                            <img src="/images/Home/person3.png" alt="Testimonial" />
                        </div>
                        <div className="testimonial-card">
                            <div className="quote-icon">“</div>
                            <p className="testimonial-text">
                                Lorem ipsum dolor sit amet, <span>consectetur adipiscing elit</span>, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
                                <span> Ut enim ad minim veniam</span>, quis nostrud exercitation ullamco laboris nisi ut aliquip. Lorem ipsum dolor sit amet, <span>consectetur adipiscing elit</span>.
                            </p>
                            <div className="testimonial-user">
                                <img src="/images/Home/chat.png" alt="Gloria Burnett" />
                                <div>
                                    <h5>Gloria Burnett</h5>
                                    <p>Software Developer</p>
                                </div>
                            </div>
                            <div className="testimonial-dots">
                                <span className="dot active"></span>
                                <span className="dot"></span>
                                <span className="dot"></span>
                            </div>
                            <div className="testimonial-shape"></div>
                        </div>
                    </div>
                </section>


                {/* Video Section */}
                <section className="home-sec5">
                    <div className="video-container">
                        <h2>See Duckie In Action</h2>
                        <p>Watch how Duckie helps you master languages with smart AI features and fun learning experiences.</p>
                        <div className="video-wrapper">
                            <video controls poster="/images/Home/video-poster.jpg">
                                <source src="/videos/snake.mp4" type="video/mp4" />
                                Your browser does not support the video tag.
                            </video>
                        </div>
                    </div>
                </section>


                {/* Footer */}
                <footer className="home-foot">
                    <div className="footer-column">
                        <h3>Get In Touch</h3>
                        <p>Feel free to connect with us</p>
                        <div className="social-icons">
                            <img src="/images/Home/facebook.png" alt="Facebook" />
                            <img src="/images/Home/instagram.png" alt="Instagram" />
                            <img src="/images/Home/twitter-sign.png" alt="Twitter" />
                        </div>
                    </div>
                    <div className="footer-column">
                        <h3>Quick Links</h3>
                        <ul>
                            <li><a href="#">About Us</a></li>
                            <li><a href="#">Blog</a></li>
                        </ul>
                    </div>
                </footer>

            </main>
        </>
    );
};

export default Home;
