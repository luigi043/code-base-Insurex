import React from 'react';
import { Drawer, List, ListItem, ListItemIcon, ListItemText, Divider, Box } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PolicyIcon from '@mui/icons-material/Policy';
import AssetIcon from '@mui/icons-material/Inventory';
import ClaimIcon from '@mui/icons-material/LocalHospital';
import ReportIcon from '@mui/icons-material/Assessment';
import AdminIcon from '@mui/icons-material/AdminPanelSettings';

const drawerWidth = 240;

const Sidebar: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const menuItems = [
        { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
        { text: 'Policies', icon: <PolicyIcon />, path: '/policies' },
        { text: 'Assets', icon: <AssetIcon />, path: '/assets' },
        { text: 'Claims', icon: <ClaimIcon />, path: '/claims' },
        { text: 'Reports', icon: <ReportIcon />, path: '/reports' },
        { text: 'Admin', icon: <AdminIcon />, path: '/admin' },
    ];

    return (
        <Drawer
            variant="permanent"
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                '& .MuiDrawer-paper': {
                    width: drawerWidth,
                    boxSizing: 'border-box',
                    mt: 8,
                },
            }}
        >
            <Box sx={{ overflow: 'auto' }}>
                <List>
                    {menuItems.map((item) => (
                        <ListItem
                            button
                            key={item.text}
                            onClick={() => navigate(item.path)}
                            selected={location.pathname === item.path}
                        >
                            <ListItemIcon>{item.icon}</ListItemIcon>
                            <ListItemText primary={item.text} />
                        </ListItem>
                    ))}
                </List>
                <Divider />
            </Box>
        </Drawer>
    );
};

export default Sidebar;
