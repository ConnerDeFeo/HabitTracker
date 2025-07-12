import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css';
import HabitTracker from './HabitTracker';
import 'react-image-crop/dist/ReactCrop.css';
import { GoogleOAuthProvider } from '@react-oauth/google';

const CLIENT_ID="135528840167-dnauo3tvh2eijnlnnnon4qhdhd5hp05c.apps.googleusercontent.com";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <HabitTracker/>
    </GoogleOAuthProvider>
  </StrictMode>,
)
