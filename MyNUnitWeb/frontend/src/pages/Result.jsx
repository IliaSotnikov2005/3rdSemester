import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  Paper,
  Divider,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import SuccessIcon from "@mui/icons-material/CheckCircle";
import FailedIcon from "@mui/icons-material/Cancel";
import IgnoredIcon from "@mui/icons-material/RemoveCircle";
import ErroredIcon from "@mui/icons-material/Error";
import api from "../api";

const Result = () => {
  const { id } = useParams();
  const [testRun, setTestRun] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTestRun = async () => {
      try {
        const response = await api.get(`/TestRuns/${id}`);
        setTestRun(response.data);
      } catch (err) {
        setError("Failed to load test results.");
        console.error("Error fetching test run:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchTestRun();
  }, [id]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <Typography variant="h6" color="error">
          {error}
        </Typography>
      </Box>
    );
  }

  if (!testRun) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <Typography variant="h6">No test results found.</Typography>
      </Box>
    );
  }

  const getStatusIcon = (status) => {
    switch (status) {
      case 0:
        return <SuccessIcon color="success" />;
      case 1:
        return <FailedIcon color="error" />;
      case 2:
        return <IgnoredIcon color="primary" />;
      case 3:
        return <ErroredIcon color="warning" />;
      default:
        return null;
    }
  };

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="80vh"
      gap={3}
      p={3}
    >
      <Typography variant="h4" component="h1" gutterBottom>
        Test Results
      </Typography>

      <Paper elevation={3} sx={{ p: 3, width: "100%", maxWidth: 800 }}>
        <Typography variant="h6" gutterBottom>
          Test Run Summary
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <Box display="flex" flexDirection="column" gap={1}>
          <Typography>
            <strong>Total Tests:</strong>{" "}
            {testRun.result.testAssemblyResults.reduce(
              (acc, assembly) => acc + assembly.testClassResults.reduce(
                (acc, cls) => acc + cls.testResults.length, 0), 0)}
          </Typography>
          <Typography>
            <strong>Passed:</strong> {testRun.result.passed}
          </Typography>
          <Typography>
            <strong>Failed:</strong> {testRun.result.failed}
          </Typography>
          <Typography>
            <strong>Ignored:</strong> {testRun.result.ignored}
          </Typography>
        </Box>
      </Paper>

      <Paper elevation={3} sx={{ p: 3, width: "100%", maxWidth: 800 }}>
        <Typography variant="h6" gutterBottom>
          Test Details
        </Typography>
        <Divider sx={{ mb: 2 }} />

        {testRun.result.testAssemblyResults.map((assembly) => (
          <Accordion key={assembly.id} sx={{ mb: 2 }}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Typography variant="subtitle1">
                <strong>Assembly:</strong> {assembly.assemblyName}
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              {assembly.testClassResults.map((testClass) => (
                <Accordion key={testClass.id} sx={{ mb: 2 }}>
                  <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                    <Typography variant="subtitle1">
                      <strong>Class:</strong> {testClass.testClassName}
                    </Typography>
                  </AccordionSummary>
                  <AccordionDetails>
                    <List>
                      {testClass.testResults.map((test) => (
                        <ListItem key={test.id} sx={{ pl: 0 }}>
                          <ListItemText
                            primary={test.name}
                            secondary={
                              <>
                                <Typography variant="body2" color="text.secondary">
                                  {test.message}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                  Duration: {test.timeElapsed ? `${test.timeElapsed} s` : "N/A"}
                                </Typography>
                              </>
                            }
                          />
                          {getStatusIcon(test.status)}
                        </ListItem>
                      ))}
                    </List>
                  </AccordionDetails>
                </Accordion>
              ))}
            </AccordionDetails>
          </Accordion>
        ))}
      </Paper>
    </Box>
  );
};

export default Result;