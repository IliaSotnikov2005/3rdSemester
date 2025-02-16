import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  CircularProgress,
} from "@mui/material";
import api from "../api"; // Импортируем API-клиент

const History = () => {
  const [testRuns, setTestRuns] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Загрузка данных о тестовых прогонах
  useEffect(() => {
    const fetchTestRuns = async () => {
      try {
        const response = await api.get("/TestRuns");
        setTestRuns(response.data);
      } catch (err) {
        setError("Failed to load test history.");
        console.error("Error fetching test runs:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchTestRuns();
  }, []);

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

  return (
    <div style={{ padding: "20px" }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Test History
      </Typography>

      {/* Таблица с историей тестовых прогонов */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Launch Time</TableCell>
              <TableCell>Total Tests</TableCell>
              <TableCell>Passed</TableCell>
              <TableCell>Failed</TableCell>
              <TableCell>Ignored</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {testRuns.map((testRun) => (
              <TableRow
                key={testRun.id}
                hover
                style={{ cursor: "pointer" }}
                onClick={() => navigate(`/result/${testRun.id}`)} // Переход на страницу результата
              >
                <TableCell>{testRun.id}</TableCell>
                <TableCell>
                  {new Date(testRun.launchTime).toLocaleString()}
                </TableCell>
                <TableCell>
                  {testRun.result.testAssemblyResults.reduce(
                    (acc, assembly) => acc + assembly.testClassResults.reduce(
                      (acc, cls) => acc + cls.testResults.length, 0), 0)}
                </TableCell>
                <TableCell>{testRun.result.passed}</TableCell>
                <TableCell>{testRun.result.failed}</TableCell>
                <TableCell>{testRun.result.ignored}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </div>
  );
};

export default History;