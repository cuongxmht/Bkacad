import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from '../account/RefreshAccessToken';

const AddEmployee = ({ show, handleClose, fetchEmployees }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/Employee';
    const [newEmployee, setNewEmployee] = useState({
        firstName: '',
        lastName: '',
        address: '',
        dateOfBirth: '',
        position: '',
        email: '',
        hireDate: '',
    });

    const handleInputChange = (e) => {
        setNewEmployee({ ...newEmployee, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const formattedEmployee = {
                ...newEmployee,
                dateOfBirth: new Date(newEmployee.dateOfBirth).toISOString().split('T')[0],
                hireDate: new Date(newEmployee.hireDate).toISOString().split('T')[0],
            };
            await refreshAccessToken.post(apiUrl, formattedEmployee);
            handleClose();
            fetchEmployees();
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Add New Employee</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="mb-3">
                        <Form.Label>First Name:</Form.Label>
                        <Form.Control type="text" name="firstName" value={newEmployee.firstName} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Last Name:</Form.Label>
                        <Form.Control type="text" name="lastName" value={newEmployee.lastName} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Address:</Form.Label>
                        <Form.Control type="text" name="address" value={newEmployee.address} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Date of Birth:</Form.Label>
                        <Form.Control type="date" name="dateOfBirth" value={newEmployee.dateOfBirth} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Position:</Form.Label>
                        <Form.Control type="text" name="position" value={newEmployee.position} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Email:</Form.Label>
                        <Form.Control type="email" name="email" value={newEmployee.email} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Hire Date:</Form.Label>
                        <Form.Control type="date" name="hireDate" value={newEmployee.hireDate} onChange={handleInputChange} required />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" onClick={handleSubmit}>+ Add</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default AddEmployee;