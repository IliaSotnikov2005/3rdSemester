import React from "react";
import { AppBar, Toolbar, Typography, Button, Container } from "@mui/material";
import { Link } from "react-router-dom";

const Navbar = () => {
  return (
    <AppBar position="static" sx={{ backgroundColor: "#ffffff", color: "#000000", boxShadow: "none" }}>
      <Container maxWidth="lg">
        <Toolbar disableGutters sx={{ gap: 2 }}>
          <Typography
            variant="h6"
            component={Link}
            to="/"
            sx={{
              fontWeight: "bold",
              color: "inherit",
              textDecoration: "none",
              fontSize: "2rem",
            }}
          >
            MyNUnitWeb
          </Typography>

          <Button
            color="inherit"
            component={Link}
            to="/upload"
            sx={{ textTransform: "none", fontSize: "1.5rem" }}
          >
            Upload
          </Button>
          <Button
            color="inherit"
            component={Link}
            to="/history"
            sx={{ textTransform: "none", fontSize: "1.5rem" }}
          >
            History
          </Button>
        </Toolbar>
      </Container>
    </AppBar>
  );
};

export default Navbar;