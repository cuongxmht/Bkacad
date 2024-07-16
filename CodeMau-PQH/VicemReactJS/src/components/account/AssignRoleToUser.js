import React, { useEffect, useState } from 'react';
import { Alert, Button, Form, ListGroup, Modal } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import refreshAccessToken from './RefreshAccessToken';


const AssignRoleToUser = ({ show, handleClose, selectedUser, fetchData }) => {
    const apiURL = process.env.REACT_APP_API_BASE_URL ;
    const [roles, setRoles] = useState([]);
    const [selectedRoles, setSelectedRoles] = useState([]);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        if (show) {
            fetchRoles();
            setSelectedRoles(selectedUser.roles);
        }
    }, [show, selectedUser]);
    const fetchRoles = async () => {
        try {
            // const token = localStorage.getItem('token');
            // if (!token) {
            //     throw new Error("No token found");
            // }
            // const config = {
            //     headers: { Authorization: `Bearer ${token}` }
            // };
            const response = await refreshAccessToken.get(apiURL +'/api/Role');
            setRoles(response.data);
        } catch (error) {
            handleErrorResponse(error);
        }
    };
    const handleRoleChange = (roleName) => {
        const updatedRoles = [...selectedRoles];
        if (updatedRoles.includes(roleName)) {
            setSelectedRoles(updatedRoles.filter(role => role !== roleName));
        } else {
            setSelectedRoles([...updatedRoles, roleName]);
        }
    };
    const handleSubmit = async (e) => {
        console.log(selectedRoles);
        e.preventDefault();
        try {
            const data = {
                userId: selectedUser.id,
                roleNames: selectedRoles
            };
            await refreshAccessToken.post( apiURL + '/api/UserRole/AddRolesToUser', data);
            fetchData();
            handleClose();
        } catch (error) {
            handleErrorResponse(error);
        }
    };
    const handleErrorResponse = (error) => {
        if (error.response) {
            switch (error.response.status) {
                case 403:
                    navigate('/403');
                    break;
                case 404:
                    navigate('/404');
                    break;
                case 500:
                    navigate('/500');
                    break;
                default:
                    setError(new Error('An unexpected error occurred.'));
            }
        } else {
            setError(error);
        }
    };
    const renderRoles = () => {
        return roles.map(role => (
            <ListGroup.Item key={role.id}>
                <Form.Check
                    type="checkbox"
                    label={role.name}
                    checked={selectedRoles.includes(role.name)}
                    onChange={() => handleRoleChange(role.name)}
                />
            </ListGroup.Item>
        ));
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Assign Roles to User</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {error && <Alert variant="danger">{error.message}</Alert>}
                <Form onSubmit={handleSubmit}>
                    <ListGroup>
                        {renderRoles()}
                    </ListGroup>
                    <Button variant="primary" type="submit" className="mt-3">
                        Save Changes
                    </Button>
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default AssignRoleToUser;