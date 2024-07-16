import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from './RefreshAccessToken';
function LoginModal({ show, handleClose, setIsLoggedIn, setUserName }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const handleLogin = async () => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL +'/api/Account/login';
    try {
      const response = await refreshAccessToken.post(apiUrl,
        { email, password, rememberMe });
        if (response.data && response.data.token) {
          const { token, userName } = response.data;
          localStorage.setItem('token', token);
          localStorage.setItem('userName',userName);
          setIsLoggedIn(true);
          setUserName(userName);
          handleClose();
        } else {
          throw new Error('Invalid response data');
        }
    } catch (error) {
      console.error('Error logging in', error);
      alert('Login failed. Please check your credentials.');
    }
  };
  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Login</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form>
          <Form.Group className="mb-3" controlId="formBasicEmail">
            <Form.Label>Email address</Form.Label>
            <Form.Control type="email" placeholder="Enter email" value={email} onChange={(e) => setEmail(e.target.value)} />
          </Form.Group>
          <Form.Group className="mb-3" controlId="formBasicPassword">
            <Form.Label>Password</Form.Label>
            <Form.Control type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />
          </Form.Group>
          <Form.Group className="mb-3" controlId="formBasicCheckbox">
            <Form.Check type="checkbox" label="Remember Me?" checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)} />
          </Form.Group>
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleClose}>Close</Button>
        <Button variant="primary" onClick={handleLogin}>Login</Button>
      </Modal.Footer>
    </Modal>
  );
}

export default LoginModal;