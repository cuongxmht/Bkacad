import React, { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from '../account/RefreshAccessToken';

const EditRole = ({ show, handleClose, fetchRoles, role }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/Role/';
    const [updatedRole, setUpdatedRole] = useState('');

    useEffect(() => {
        if (role) {
            setUpdatedRole(role.name);
        }
    }, [role]);

    const handleInputChange = (e) => {
        setUpdatedRole(e.target.value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await refreshAccessToken.put(`${apiUrl}${role.id}`, updatedRole);
            handleClose();
            fetchRoles();
        } catch (error) {
            console.error(error);
        }
    };
    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Edit Role</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group className="mb-3">
                        <Form.Label>Name:</Form.Label>
                        <Form.Control type="text" name="firstName" value={updatedRole.name} onChange={handleInputChange} required />
                    </Form.Group>
                    
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" type="button" onClick={handleSubmit}>Save Changes</Button>
            </Modal.Footer>
        </Modal>
    );
};
export default EditRole;