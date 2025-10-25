import { Outlet } from 'react-router';
import './App.css';
import Navbar from './components/navbar/Navbar';
import { UserProvider } from './contexts/AuthContext';

function App() {
  return (
    <>
		<UserProvider>
          <Navbar />
          <Outlet />
		</UserProvider>
	</>
  );
}

export default App;
