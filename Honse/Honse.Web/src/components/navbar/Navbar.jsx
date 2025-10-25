import './Navbar.css';
import { Link } from 'react-router';
import { useAuth } from '../../contexts/AuthContext';


function Navbar() {

  const {isLoggedIn} = useAuth();
  const {logoutUser} = useAuth();

 return (
    <header>
        <div className='main--header'>
          <Link to='/'><h1 className='logo'>Honse</h1></Link>

          <nav className='navbar'>
            {
              isLoggedIn() ?
                <>
                  <Link to='/products'>Products</Link>
                  <Link to='/restaurants'>Restaurants</Link>
                  <Link to='/public/login'onClick={() => {logoutUser();}}>Log out</Link>
                </>
                :
                <>
                  <Link to='/public/restaurants'>Restaurants</Link>
                  <Link to='/public/login'>Log in</Link>
                  <Link to='/public/register'>Register</Link>
                </>
              }
            
          </nav>
        </div> 

    </header>
  )
}

export default Navbar;