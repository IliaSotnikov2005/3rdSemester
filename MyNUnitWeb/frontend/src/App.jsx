import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import Home from "./pages/Home";
import Upload from "./pages/Upload";
import History from "./pages/History";
import CursorTrail from "./components/CursorTrail";
import Result from "./pages/Result";

const App = () => {
  return (
    <Router>
      <CursorTrail />
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/upload" element={<Upload />} />
        <Route path="/history" element={<History />} />
        <Route path="/result/:id" element={<Result />} />
      </Routes>
    </Router>
  );
};

export default App;