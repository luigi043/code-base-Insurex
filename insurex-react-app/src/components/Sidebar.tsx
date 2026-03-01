import React from 'react';
import { Drawer, List, ListItem, ListItemIcon, ListItemText, Divider, Box } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PolicyIcon from '@mui/icons-material/Policy';
import AssignmentIcon from '@mui/icons-material/Assignment';
import DescriptionIcon from '@mui/icons-material/Description';
import AssessmentIcon from '@mui/icons-material/Assessment';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import { useAuth } from '../contexts/AuthContext';

const drawerWidth = 240;

const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();

  const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard', roles: ['Admin', 'User'] },
    { text: 'Policies', icon: <PolicyIcon />, path: '/policies', roles: ['Admin', 'User'] },
    { text: 'Assets', icon: <AssignmentIcon />, path: '/assets', roles: ['Admin', 'User'] },
    { text: 'Claims', icon: <DescriptionIcon />, path: '/claims', roles: ['Admin', 'User'] },
    { text: 'Reports', icon: <AssessmentIcon />, path: '/reports', roles: ['Admin'] },
    { text: 'Admin', icon: <AdminPanelSettingsIcon />, path: '/admin', roles: ['Admin'] },
  ];

  const filteredMenu = menuItems.filter(item => 
    item.roles.includes(user?.role || '')
  );

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
          bgcolor: 'primary.main',
          color: 'white',
        },
      }}
    >
      <Toolbar />
      <Box sx={{ overflow: 'auto' }}>
        <List>
          {filteredMenu.map((item) => (
            <ListItem
              button
              key={item.text}
              onClick={() => navigate(item.path)}
              selected={location.pathname.startsWith(item.path)}
              sx={{
                '&.Mui-selected': {
                  bgcolor: 'primary.dark',
                },
                '&:hover': {
                  bgcolor: 'primary.light',
                },
              }}
            >
              <ListItemIcon sx={{ color: 'white' }}>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
        </List>
        <Divider sx={{ bgcolor: 'rgba(255,255,255,0.2)' }} />
      </Box>
    </Drawer>
  );
};

// Add this to fix the missing Toolbar import
import Toolbar from '@mui/material/Toolbar';

export default Sidebar;