import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from '../account/RefreshAccessToken';

const AddDaiLy = ({ show, handleClose, fetchData }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/DaiLy';
    const [newDaiLy, setNewDaiLy] = useState({
        tenDaiLy: '',
        diaChi: ''
    });

    const handleInputChange = (e) => {
        setNewDaiLy({ ...newDaiLy, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await refreshAccessToken.post(apiUrl, newDaiLy);
            handleClose();
            fetchData();
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Add New Dai ly</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="mb-3">
                        <Form.Label>Ten Dai ly:</Form.Label>
                        <Form.Control type="text" name="tenDaiLy" value={newDaiLy.tenDaiLy} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Dai chi:</Form.Label>
                        <Form.Control type="text" name="diaChi" value={newDaiLy.diaChi} onChange={handleInputChange} required />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" onClick={handleSubmit}>+ Add</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default AddDaiLy;