import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./Home/home";

// student
import Register from "./Student/Register";
import Login from "./Student/Login";
import Profile from "./Student/Profile"
import Dashboard from "./Student/Dashboard";
import AuthForm from "./Student/AuthForm";
import Leaderboard from "./Student/Leadeboard";
import EvaluationQuiz from "./Student/EvaluationQuiz";
import EvaluationQuizSetup from "./Student/EvaluationQuizSetup";

// teacher
import TeacherLogin from "./Teacher/TeacherLogin";
import TeacherDashboard from "./Teacher/TeacherDashboard";

// course
import CourseCreate from "./Teacher/CourseCreate";
import CourseUpdate from "./Teacher/CourseUpdate";
import CourseView from "./Teacher/CourseView";
import CourseViewStudent from "./Course/CourseView";
import CourseContent from "./Course/CourseContent";
import CourseContentFinished from "./Course/CourseContentFinished";
import PurchaseConfirmed from "./Course/PurchaseConfirmed";
import CoursePurchase from "./Course/CoursePurchase";
import CourseContentCreate from "./Teacher/CourseContentCreate";
import CourseContentUpdate from "./Teacher/CourseContentUpdate";



// unit
import Unit from "./Unit/Unit";
import UnitUpdate from "./Unit/UnitUpdate";
import UnitDelete from "./Unit/UnitDelete";
import UnitCreate from "./Unit/UnitCreate";
import UnitDisplay from "./Unit/UnitDisplay";

// lesson
import Lesson from "./Lesson/Lesson";
import LessonContent from "./Lesson/LessonContent";
import LessonUpdate from "./Lesson/LessonUpdate";
import LessonDelete from "./Lesson/LessonDelete";
import LessonCreate from "./Lesson/LessonCreate";
import QuizLesson from "./Lesson/QuizLesson";
import QuizFinish from "./Lesson/QuizFinish";

// situation
import SituationPage from "./Situation/Situation";

//admin
import AdminLogin from "./Admin/Login";
import AdminProfile from "./Admin/Profile";
import AdminDashboard from "./Admin/Dashboard";



function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home />} /> {/* Home page */}
                {/*Student Routes*/}
                <Route path="/register" element={<Register />} /> {/* Register page */}
                <Route path="/login" element={<Login />} /> {/*Login Page*/}
                <Route path="/profile" element={<Profile />} /> {/*Profile Page*/}
                <Route path="/dashboard" element={<Dashboard />} /> {/*Dashboard Page*/}
                <Route path="/authform" element={<AuthForm />} />
                <Route path="/leaderboard" element={<Leaderboard />} />
                <Route path="/evaluationQuizSetup" element={<EvaluationQuizSetup />} /> 
                <Route path="/evaluationQuiz" element={<EvaluationQuiz /> } /> 

                {/*Teacher Routes*/}
                <Route path="/teacher/login" element={<TeacherLogin />} /> {/* login page */}
                <Route path="/teacher/dashboard" element={<TeacherDashboard />} /> 

                {/*Course Routes*/}
                <Route path="/teacher/courseCreate" element={<CourseCreate />} /> 
                <Route path="/teacher/courseUpdate/:id" element={<CourseUpdate />} /> 
                <Route path="/teacher/courseView/:id" element={<CourseView />} />
                <Route path="/course/coursePurchase/:id" element={<CoursePurchase />} />
                <Route path="/course/courseContentFinished" element={<CourseContentFinished />} />
                <Route path="/course/courseContent/:id" element={<CourseContent />} />
                <Route path="/course/purchaseConfirmed" element={<PurchaseConfirmed />} />
                <Route path="/course/courseView/:id" element={<CourseViewStudent />} />
                <Route path="/teacher/courseContentCreate/:id" element={<CourseContentCreate />} />
                <Route path="/teacher/courseContentUpdate/:id" element={<CourseContentUpdate />} /> 


                {/*Unit Routes*/}
                <Route path="/unit/:id" element={<Unit />} />
                <Route path="/unit/update/:id" element={<UnitUpdate />} />
                <Route path="/unit/delete/:id" element={<UnitDelete />} />
                <Route path="/unit/create" element={<UnitCreate />} />
                <Route path="/unit/display/:id" element={<UnitDisplay />} />


                {/*Lesson Routes*/}
                <Route path="/lesson/:id" element={<Lesson />} />
                <Route path="/lessonContent/*" element={<LessonContent />} />
                <Route path="/lesson/update/:id" element={<LessonUpdate />} />
                <Route path="/lesson/delete/:id" element={<LessonDelete />} />
                <Route path="/lesson/create/:id" element={<LessonCreate />} />
                <Route path="/quizLesson/*" element={<QuizLesson />} />
                <Route path="/quizFinish/:lessonId" element={<QuizFinish />} />

                {/*Situation Routes*/ }
                <Route path="/situation/:id" element={<SituationPage /> } />
                {/* Admin Routes*/}
                <Route path="/admin/login" element={<AdminLogin />} />
                <Route path="/admin/dashboard" element={<AdminDashboard />} />
                <Route path="/admin/profile" element={<AdminProfile />} />

            </Routes>
        </Router>
    );
}

export default App;
