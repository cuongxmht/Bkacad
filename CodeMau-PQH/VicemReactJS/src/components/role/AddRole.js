import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from '../account/RefreshAccessToken';

const AddRole = ({ show, handleClose, fetchRoles }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/Role';
    const [newRole, setNewRole] = useState('');

    const handleInputChange = (e) => {
        setNewRole(e.target.value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await refreshAccessToken.post(apiUrl, newRole);
            handleClose();
            fetchRoles();
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Add New Role</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="mb-3">
                        <Form.Label>Name:</Form.Label>
                        <Form.Control type="text" name="name" value={newRole.name} onChange={handleInputChange} required />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" onClick={handleSubmit}>+ Add</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default AddRole;