import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css';
import HabitTracker from './HabitTracker';
import 'react-image-crop/dist/ReactCrop.css';
import { GoogleOAuthProvider } from '@react-oauth/google';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_AUTH_CLIENT_ID}>
      <HabitTracker/>
    </GoogleOAuthProvider>
  </StrictMode>,
)
