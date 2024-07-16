import React, {useState, useEffect} from 'react';
import { Container, Navbar, Nav, Button } from 'react-bootstrap';
import LoginModal from '../account/LoginModal';
import { useNavigate } from 'react-router-dom';

const Menu = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [userName, setUserName] = useState('');
  const nav = useNavigate();
  useEffect(() => {
    const token = localStorage.getItem('token');
    const storedUserName = localStorage.getItem('userName');
    setIsLoggedIn(!!token);
    if (storedUserName) {
      setUserName(storedUserName);
    }
  }, []);
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userName');
    setIsLoggedIn(false);
    setUserName('');
    nav('/');
  };
  const handleLoginClick = () => {
    setShowLoginModal(true);
  };
  const handleCloseLoginModal = () => {
    setShowLoginModal(false);
  };
  return (
    <>
      <Navbar expand="lg" className="bg-body-tertiary">
        <Container>
          <Navbar.Brand href="/">Home</Navbar.Brand>
          {isLoggedIn && (
            <>
              <Nav.Link href='/user'>User</Nav.Link>
              <Nav.Link href='/role'>Role</Nav.Link>
              <Nav.Link href='/employee'>Employee</Nav.Link>
            </>
          )}
          <Nav className="ml-auto">
              {isLoggedIn ? (
                <>
                  <Nav.Link href="#">Xin chào: {userName}</Nav.Link>
                  <Nav.Link href="#" onClick={handleLogout}>Logout</Nav.Link>
                </>
              ) : (
                <Button onClick={handleLoginClick}>Login</Button>
              )}
            </Nav>

        </Container>
      </Navbar>
      <LoginModal show={showLoginModal} handleClose={handleCloseLoginModal} setIsLoggedIn={setIsLoggedIn} setUserName={setUserName}/>
    </>
  );
};
export default Menu;