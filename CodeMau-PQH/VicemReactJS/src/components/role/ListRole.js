import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, InputGroup, DropdownButton, DropdownItem } from 'react-bootstrap';
import AddRole from './AddRole';
import EditRole from './EditRole';
import AssignPermissionsToRole from './AssignPermissionsToRole';
import { FaRegEdit } from 'react-icons/fa';
import { MdDeleteForever } from 'react-icons/md';
import { MdAssignmentAdd, MdAssignmentInd } from "react-icons/md";
import refreshAccessToken from '../account/RefreshAccessToken';

const ListRole = () => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/Role/';
    const [roles, setRoles] = useState([]);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] =useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [showAssignModal, setShowAssignModal] = useState(false);
    const [selectedRole, setSelectedRole] = useState(null);
    const [viewOrEditPermission, setViewOrEditPermission] = useState(false);
    const [selectedID, setSelectedID] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    useEffect(() => {
        fetchRoles();
    }, []);
    const fetchRoles = async () => {
        try {
            const response = await refreshAccessToken.get(apiUrl);
            setRoles(response.data);
            setLoading(false);
        } catch (error) {
            setError(error);
            setLoading(false);
        }
    };
    const handleShowAddModal = () => {
        setShowAddModal(true);
    };
    const handleCloseAddModal = () => {
        setShowAddModal(false);
    };
    const handleShowEditModal = (role) => {
        setSelectedRole(role);
        setShowEditModal(true);
    };
    
    const handleCloseEditModal = () => {
        setSelectedRole(null);
        setShowEditModal(false);
    };
    
    const handleShowDeleteModal = (role) => {
        setSelectedRole(role);
        setSelectedID(role.id);
        setShowDeleteModal(true);
    };
    
    const handleCloseDeleteModal = () => {
        setSelectedID(null);
        setSelectedRole(null);
        setShowDeleteModal(false);
    };
    const handleShowAssignModal = (role, viewOrEdit) => {
        setSelectedRole(role);
        setSelectedID(role.id);
        setViewOrEditPermission(viewOrEdit)
        setShowAssignModal(true);
    }
    const handleCloseAssignModal = () => {
        setSelectedID(null);
        setSelectedRole(null);
        setShowAssignModal(false);
    }
    
    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }
    const handleConfirmDelete = async () => {
        try {
            await refreshAccessToken.delete(`${apiUrl}${selectedID}`);
            fetchRoles();
            handleCloseDeleteModal();
        } catch (error) {
          console.error(error);
        }
      };
    return (
        <div className='container'>
            <div className="d-flex justify-content-between mb-2">
                <br/>
                <Button variant="primary" onClick={handleShowAddModal}>
                + Create new
                </Button>
            </div>
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Normalized Name</th>
                    </tr>
                </thead>
                <tbody>
                    {roles.map((role) => (
                        <tr key={role.id}>
                            <td>{role.id}</td>
                            <td>{role.name}</td>
                            <td>{role.normalizedName}</td>
                            <td>
                            <InputGroup className='mb-3'>
                                <DropdownButton variant="success" title="Actions" id="input-group-dropdown-1">
                                    <DropdownItem onClick={() => handleShowEditModal(role)}><FaRegEdit className="text-success ud-cursor mb-1 mx-1"/>Edit</DropdownItem>
                                    <DropdownItem variant="danger" onClick={() => handleShowDeleteModal(role)}><MdDeleteForever size={20} className="text-danger ud-cursor mb-1 mx-0"/>Delete</DropdownItem>
                                    <DropdownItem variant="primary" onClick={() => handleShowAssignModal(role, true)}><MdAssignmentAdd size={20} className="text-primary ud-cursor mb-1 mx-0"/>Assign Permission</DropdownItem>
                                    <DropdownItem variant="primary" onClick={() => handleShowAssignModal(role, false)}><MdAssignmentInd size={20} className="text-primary ud-cursor mb-1 mx-0"/>View Permission</DropdownItem>
                                </DropdownButton>
                            </InputGroup>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            {selectedRole && (
                <>
                    <EditRole show={showEditModal} handleClose={handleCloseEditModal} fetchRoles={fetchRoles} role={selectedRole} />
                    <AssignPermissionsToRole show={showAssignModal} handleClose={handleCloseAssignModal} selectedRoleID={selectedID} actionViewOrEdit={viewOrEditPermission} />
                </>
            )}
            <AddRole show={showAddModal} handleClose={handleCloseAddModal} fetchRoles={fetchRoles} />
            {selectedRole && (
                <Modal show={showDeleteModal} onHide={handleCloseDeleteModal}>
                    <Modal.Header closeButton>
                    <Modal.Title>Bạn có muốn xoá bản ghi này không?</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                    <p>{selectedRole.id}-{selectedRole.name}</p>
                    </Modal.Body>
                    <Modal.Footer>
                    <Button variant="secondary" onClick={handleCloseDeleteModal}>
                        Cancel
                    </Button>
                    <Button variant="danger" onClick={handleConfirmDelete}>
                        Delete
                    </Button>
                    </Modal.Footer>
                </Modal>
            )}
        </div>
    );
};

export default ListRole;