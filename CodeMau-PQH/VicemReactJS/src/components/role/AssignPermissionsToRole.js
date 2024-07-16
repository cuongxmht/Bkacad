import React, { useEffect, useState } from 'react';
import { Modal, Button, Form, ListGroup, Alert } from 'react-bootstrap';
import { SystemPermissions } from './SystemPermissions';
import refreshAccessToken from '../account/RefreshAccessToken';

const AssignPermissionsToRole = ({ show, handleClose, selectedRoleID, actionViewOrEdit }) => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/RolePermission/';
    const [rolePermissions, setRolePermissions] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (selectedRoleID) {
            fetchRolePermissions(selectedRoleID);
        }
    }, [selectedRoleID]);

    const fetchRolePermissions = async () => {
        try {
            const response = await refreshAccessToken.get(`${apiUrl}${selectedRoleID}/permissions`);
            setRolePermissions(response.data.map(permission => SystemPermissions[permission]));
        } catch (error) {
            setError(error);
        }
    };

    const handlePermissionChange = (permission) => {
        if (rolePermissions.includes(permission)) {
            setRolePermissions(rolePermissions.filter(p => p !== permission));
        } else {
            setRolePermissions([...rolePermissions, permission]);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const permissionsToSubmit = rolePermissions.map(permission => 
                Object.keys(SystemPermissions).findIndex(key => SystemPermissions[key] === permission)
            );

            await refreshAccessToken.post(`${apiUrl}${selectedRoleID}/permissions`, permissionsToSubmit);
            handleClose();
        } catch (error) {
            setError(error);
        }
    };

    const renderPermissions = () => {
        return Object.keys(SystemPermissions).map(permission => (
            <ListGroup.Item key={permission}>
                <Form.Check
                    type="checkbox"
                    label={SystemPermissions[permission]}
                    checked={rolePermissions.includes(SystemPermissions[permission])}
                    onChange={() => handlePermissionChange(SystemPermissions[permission])}
                    disabled={!actionViewOrEdit}
                />
            </ListGroup.Item>
        ));
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>{actionViewOrEdit ? "Assign Permissions" : "View Permissions"}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {error && <Alert variant="danger">{error.message}</Alert>}
                <Form onSubmit={handleSubmit}>
                    <ListGroup>
                        {renderPermissions()}
                    </ListGroup>
                    {actionViewOrEdit && (
                        <Button variant="primary" type="submit" className="mt-3">
                            Save Changes
                        </Button>
                    )}
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default AssignPermissionsToRole;