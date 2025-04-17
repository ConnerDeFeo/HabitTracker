import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';
import HomePage from './HomePage';
import CreateAccount from './CreateAccount';
import Login from './Login';

function App() {

  return (
    <>
      <Router>
        <Routes>
          <Route path='/' element={<HomePage/>}/>
          <Route path='CreateAccount' element={<CreateAccount/>}/>
          <Route path='Login' element={<Login/>}/>
        </Routes>
      </Router>
    </>
  );
}

export default App;
