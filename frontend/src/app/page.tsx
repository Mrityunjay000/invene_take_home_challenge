'use client';

import LoadingComponent from '@/components/LoadingComponent';
import SnackbarComponent, { SnackbarSeverityLevels } from '@/components/SnackbarComponent';
import { Box, Button, SnackbarCloseReason, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";

export default function Home() {
  const [labOrder, setLabOrder] = useState<File>();
  const [submitDisabled, setSubmitDisabled] = useState<boolean>(true);
  const [loading, setLoading] = useState<boolean>(false);
  const [snackbarProps, setSnackbarProps] = useState({
    open: false,
    severity: SnackbarSeverityLevels.info,
    message: ''
  })

  useEffect(() => {
    if (labOrder)
      setSubmitDisabled(false);
    else 
      setSubmitDisabled(true);
  }, [labOrder])

  const handleSubmit = () => {
    setLoading(true);

    const apiUrl = 'https://localhost:7040';

    let data = new FormData();
    data.append('labOrder', labOrder!);

    axios.post(`https://localhost:7040/SanitizeLabOrder`, data, { 
      headers: {
        'Content-Type': 'multipart/form-data', 
      }
    })
    .then(() => {
      setLoading(false);
      setSnackbarProps({
        open: true,
        severity: SnackbarSeverityLevels.success,
        message: 'Lab Order Processed Sucessfully.'
      })
    })
    .catch(err => {
      console.log(err);
      setLoading(false);
      setSnackbarProps({
        open: true,
        severity: SnackbarSeverityLevels.error,
        message: err.response ? err.response.data.message : err.message
      })
    })
  }

  const handleSnackbarClose = (event?: React.SyntheticEvent | Event, 
      reason?: SnackbarCloseReason) => {

      if (reason === 'clickaway')
          return;

      setSnackbarProps({
          open: false,
          severity: SnackbarSeverityLevels.info,
          message: ''
      });
  };

  return (
    <>
    <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', p: 3 }}>
      <Typography variant="h3" gutterBottom>
        Lab Order Processor
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', mb: 3 }}>
        <Typography>Choose file to process</Typography>
        <input
          type='file'
          accept='.txt'
          id='attendeeInput'
          onChange={(e) => setLabOrder(e.target.files![0])}/>
      </Box>
      <Button 
        variant="contained" 
        onClick={handleSubmit} 
        disabled={submitDisabled}
      >
        Submit
      </Button>
    </Box>
    <LoadingComponent open={loading}/>
    <SnackbarComponent
        open={snackbarProps.open}
        severity={snackbarProps.severity}
        message={snackbarProps.message}
        handleClose={handleSnackbarClose}/> 
    </>
  );
}
