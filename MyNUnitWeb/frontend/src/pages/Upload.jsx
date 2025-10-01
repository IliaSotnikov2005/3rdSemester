import { useState, useEffect } from "react";
import { Button, Typography, Box, List, ListItem, ListItemText, IconButton } from "@mui/material";
import { useNavigate } from "react-router-dom";
import DeleteIcon from "@mui/icons-material/Delete";
import api from "../api";

const Upload = () => {
  const [files, setFiles] = useState([]);
  const [isTesting, setIsTesting] = useState(false);
  const [dots, setDots] = useState(".");
  const navigate = useNavigate();

  const validateFiles = (files) => {
    const maxSize = 25 * 1024 * 1024;

    if (files.length === 0) {
      return "Please select at least one file.";
    }

    let totalSize = 0;
    for (const file of files) {
      totalSize += file.size;

      if (!file.name.toLowerCase().endsWith(".dll")) {
        return `File ${file.name} is not a .dll file.`;
      }
    }

    if (totalSize > maxSize) {
      return "Total size of files exceeds 25 MB.";
    }

    return null;
  };

  const handleFileChange = (event) => {
    const selectedFiles = Array.from(event.target.files);
    console.log("Selected files:", selectedFiles);

    const validationError = validateFiles(selectedFiles);

    if (validationError) {
      alert(validationError);
      event.target.value = "";
    } else {
      setFiles((prevFiles) => [...prevFiles, ...selectedFiles]);
      console.log("Files after update:", files);
    }
  };

  const handleRemoveFile = (fileName) => {
    setFiles((prevFiles) => prevFiles.filter((file) => file.name !== fileName));
  };

  const handleStartTests = async () => {
    setIsTesting(true);

    try {
      const formData = new FormData();
      files.forEach((file) => formData.append("files", file));

      const response = await api.post("/TestRuns/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      console.log("Response from server:", response.data);

      navigate(`/result/${response.data.id}`);
    } catch (error) {
      console.error("Error uploading files:", error);
      alert("An error occurred while uploading files.");
    } finally {
      setIsTesting(false);
    }
  };

  useEffect(() => {
    if (isTesting) {
      const interval = setInterval(() => {
        setDots((prevDots) => {
          if (prevDots.length >= 3) return ".";
          return prevDots + ".";
        });
      }, 200);
      return () => clearInterval(interval);
    }
  }, [isTesting]);

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="80vh"
      gap={3}
    >
      <Typography variant="h4" component="h1">
        Choose your .dlls here
      </Typography>

      <input
        type="file"
        accept=".dll"
        multiple
        onChange={handleFileChange}
        style={{ display: "none" }}
        id="file-upload"
      />
      <label htmlFor="file-upload">
        <Button variant="contained" component="span">
          Select Files
        </Button>
      </label>

      {files.length > 0 && (
        <List sx={{ width: "100%", maxWidth: 400, bgcolor: "background.paper" }}>
          {files.map((file) => (
            <ListItem
              key={file.name}
              secondaryAction={
                <IconButton edge="end" onClick={() => handleRemoveFile(file.name)}>
                  <DeleteIcon />
                </IconButton>
              }
            >
              <ListItemText
                primary={file.name}
                secondary={`${(file.size / 1024).toFixed(2)} KB`}
              />
            </ListItem>
          ))}
        </List>
      )}

      <Button
        variant="contained"
        color="primary"
        disabled={files.length === 0 || isTesting}
        onClick={handleStartTests}
        sx={{
          backgroundColor: isTesting ? "#9e9e9e" : "primary.main",
          color: isTesting ? "#000000" : "white",
          "&:hover": {
            backgroundColor: isTesting ? "#9e9e9e" : "primary.dark",
          },
        }}
      >
        {isTesting ? `Тестирование${dots}` : "Start Tests!"}
      </Button>
    </Box>
  );
};

export default Upload;