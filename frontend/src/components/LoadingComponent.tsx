import { Typography } from '@mui/material';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Modal from '@mui/material/Modal';

const style = {
    position: 'absolute',
    top: '40%',
    left: '45%',
    transform: 'translate(-50%, -50%),',
    padding: '20px',
    backgroundColor: 'white',
    boxShadow: '24',
    borderRadius: '5px',
    outline: '0',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    textAlign: 'center',
}

export default function LoadingComponent({ open, message }: { open: boolean; message?: string; }) {
    return(
        <Modal
            open={open}
            aria-labelledby='loading-component'
            aria-describedby='loading-component'>
            <Box sx={style}>
                <CircularProgress/>
                <Typography variant='body1' sx={{ mt: 1 }}>
                    {message ?? 'Loading...'}
                </Typography>
            </Box>
        </Modal>
    );
}