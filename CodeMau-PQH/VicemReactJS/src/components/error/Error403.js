import React from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { FaExclamationTriangle } from 'react-icons/fa';

const Error403 = () => {
    const navigate = useNavigate();
    const handleBackToHome = () => {
        navigate('/');
    };
    return (
        <Container className="d-flex flex-column align-items-center justify-content-center mt-5">
            <Row className="text-center">
                <Col>
                    <FaExclamationTriangle size={60} className="text-warning mb-4" />
                    <h1 className="display-4">403 - Forbidden</h1>
                    <h3 className="lead">Bạn không có quyền truy cập vào trang này.</h3>
                    <h3>Vui lòng liên hệ quản trị viên để được hỗ trợ!</h3>
                    <Button variant="primary" onClick={handleBackToHome}>Quay lại trang chủ</Button>
                </Col>
            </Row>
        </Container>
    );
};
export default Error403;