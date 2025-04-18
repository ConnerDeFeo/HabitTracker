import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';
import HomePage from './HomePage';
import CreateAccount from './CreateAccount';
import Login from './Login';
import Navbar from './components/Navbar';
import './index.css';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Router>
        <Navbar/>
        <Routes>
          <Route path='/' element={<HomePage/>}/>
          <Route path='CreateAccount' element={<CreateAccount/>}/>
          <Route path='Login' element={<Login/>}/>
        </Routes>
      </Router>
  </StrictMode>,
)
