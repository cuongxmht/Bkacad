import React, { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import refreshAccessToken from '../account/RefreshAccessToken';

const EditEmployee = ({ show, handleClose, fetchEmployees, employee }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL +'/api/Employee/';
    const [updatedEmployee, setUpdatedEmployee] = useState({
        employeeId:'',
        firstName: '',
        lastName: '',
        address: '',
        dateOfBirth: '',
        position: '',
        email: '',
        hireDate: '',
    });

    useEffect(() => {
        if (employee) {
            setUpdatedEmployee({
                employeeId : employee.employeeId,
                firstName: employee.firstName,
                lastName: employee.lastName,
                address: employee.address,
                dateOfBirth: employee.dateOfBirth.split('T')[0],  // Assuming the date is in ISO format
                position: employee.position,
                email: employee.email,
                hireDate: employee.hireDate.split('T')[0],  // Assuming the date is in ISO format
            });
        }
    }, [employee]);

    const handleInputChange = (e) => {
        setUpdatedEmployee({ ...updatedEmployee, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const formattedEmployee = {
                ...updatedEmployee,
                dateOfBirth: new Date(updatedEmployee.dateOfBirth).toISOString().split('T')[0],
                hireDate: new Date(updatedEmployee.hireDate).toISOString().split('T')[0],
            };

            await refreshAccessToken.put(`${apiUrl}${employee.employeeId}`, formattedEmployee);
            handleClose(); // Close modal after updating the employee
            fetchEmployees(); // Refresh the employee list after updating
        } catch (error) {
            console.error(error);
        }
    };
    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Edit Employee</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group className="mb-3">
                        <Form.Label>First Name:</Form.Label>
                        <Form.Control type="text" name="firstName" value={updatedEmployee.firstName} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Last Name:</Form.Label>
                        <Form.Control type="text" name="lastName" value={updatedEmployee.lastName} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Address:</Form.Label>
                        <Form.Control type="text" name="address" value={updatedEmployee.address} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Date of Birth:</Form.Label>
                        <Form.Control type="date" name="dateOfBirth" value={updatedEmployee.dateOfBirth} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Position:</Form.Label>
                        <Form.Control type="text" name="position" value={updatedEmployee.position} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Email:</Form.Label>
                        <Form.Control type="email" name="email" value={updatedEmployee.email} onChange={handleInputChange} required />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Hire Date:</Form.Label>
                        <Form.Control type="date" name="hireDate" value={updatedEmployee.hireDate} onChange={handleInputChange} required />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" type="button" onClick={handleSubmit}>Save Changes</Button>
            </Modal.Footer>
        </Modal>
    );
};
export default EditEmployee;