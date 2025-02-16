import React, { useState } from "react";
import { Typography, Box } from "@mui/material";

const Home = () => {
  const [position, setPosition] = useState({ x: 0, y: 0 });

  const handleMouseMove = (event) => {
    const { clientX, clientY } = event;
    const centerX = window.innerWidth / 2;
    const centerY = window.innerHeight / 2;
    const offsetX = (clientX - centerX) / 20;
    const offsetY = (clientY - centerY) / 20;
    setPosition({ x: offsetX, y: offsetY });
  };

  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight="80vh"
      onMouseMove={handleMouseMove}
    >
      <Typography
        variant="h3"
        component="h1"
        sx={{
          transform: `translate(${position.x}px, ${position.y}px)`,
          transition: "transform 0.1s ease-out",
        }}
      >
        MyNUnitWeb - Upload, Test and Check!
      </Typography>
    </Box>
  );
};

export default Home;