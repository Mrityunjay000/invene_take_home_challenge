import { SnackbarCloseReason, Snackbar, Alert, Button, IconButton } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';

export enum SnackbarSeverityLevels {
    success,
    error,
    warning,
    info
}

export default function SnackbarComponent(props: {
    open: boolean;
    severity: SnackbarSeverityLevels;
    message: string;
    duration?: number;
    closeMessage?: string;
    vertical?: 'top' | 'bottom';
    horizontal?: 'left' | 'right' | 'center';
    handleClose: (event?: React.SyntheticEvent | Event, 
                  reason?: SnackbarCloseReason) => void;
}) {
    const severity = props.severity === SnackbarSeverityLevels.success ? 'success' :
        props.severity === SnackbarSeverityLevels.error ? 'error' : 
        props.severity === SnackbarSeverityLevels.warning ? 'warning' : 'info';
        
    return(
        <Snackbar 
            open={props.open} 
            autoHideDuration={props.duration ?? 6000}
            onClose={props.handleClose}
            anchorOrigin={{ 
                vertical: props.vertical ?? 'bottom', 
                horizontal : props.horizontal ?? 'left'
            }}>
            <Alert
                severity={severity}
                sx={{ width: '100%' }}
                action={props.closeMessage ?
                    <Button color='primary' size='small' onClick={props.handleClose}>
                        {props.closeMessage}
                    </Button>
                    : 
                    <IconButton
                        size='small'
                        aria-label='close'
                        color='inherit'
                        onClick={props.handleClose}
                    >
                        <CloseIcon fontSize='small' />
                    </IconButton>
                }>
                {props.message}
            </Alert>
        </Snackbar>
    )
}