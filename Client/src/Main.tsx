import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css';
import HabitTracker from './HabitTracker';
import 'react-image-crop/dist/ReactCrop.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <HabitTracker/>
  </StrictMode>,
)
