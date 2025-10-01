import React, { useState, useEffect } from "react";

const CursorTrail = () => {
  const [trails, setTrails] = useState([]);

  const handleMouseMove = (event) => {
    const { clientX, clientY } = event;

    const newTrail = {
      id: Date.now(),
      x: clientX,
      y: clientY,
      size: 10,
    };

    setTrails((prevTrails) => [...prevTrails, newTrail]);

    const interval = setInterval(() => {
      setTrails((prevTrails) =>
        prevTrails.map((trail) =>
          trail.id === newTrail.id ? { ...trail, size: trail.size * 0.9 } : trail
        )
      );
    }, 20);

    setTimeout(() => {
      setTrails((prevTrails) => prevTrails.filter((trail) => trail.id !== newTrail.id));
      clearInterval(interval);
    }, 200);
  };

  useEffect(() => {
    window.addEventListener("mousemove", handleMouseMove);
    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);

  return (
    <>
      {trails.map((trail) => (
        <div
          key={trail.id}
          style={{
            position: "fixed",
            left: trail.x,
            top: trail.y,
            width: `${trail.size}px`,
            height: `${trail.size}px`,
            backgroundColor: "rgba(0, 191, 255, 0.5)",
            borderRadius: "50%",
            transform: "translate(-50%, -50%)",
            pointerEvents: "none",
            zIndex: 9999,
            transition: "width 0.1s ease-out, height 0.1s ease-out, opacity 0.5s ease-out",
          }}
        />
      ))}
    </>
  );
};

export default CursorTrail;