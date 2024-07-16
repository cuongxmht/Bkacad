import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, InputGroup, DropdownButton, DropdownItem } from 'react-bootstrap';
import AddEmployee from './AddEmployee';
import EditEmployee from './EditEmployee';
import { FaRegEdit } from 'react-icons/fa';
import { MdDeleteForever } from 'react-icons/md';
import { useNavigate } from 'react-router-dom';
import refreshAccessToken from '../account/RefreshAccessToken';

const ListEmployee = () => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/Employee';
    const [employees, setEmployees] = useState([]);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] =useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selectedEmployee, setSelectedEmployee] = useState(null);
    const [deleteID, setDeleteID] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();
    useEffect(() => {
        fetchEmployees();
    }, []);
    const handleErrorResponse = (error) => {
        debugger;
        if (error.response) {
            switch (error.response.status) {
                case 403:
                    navigate('/403');
                    break;
                case 404:
                    navigate('/404');
                    break;
                default:
                    setError(new Error('An unexpected error occurred.'));
            }
        } else {
            setError(error);
        }
    };
    const fetchEmployees = async () => {
        debugger;
        try {
            const response = await refreshAccessToken.get(apiUrl);
            setLoading(false);
            setEmployees(response.data);
        } catch (error) {
            setLoading(false);
            handleErrorResponse(error);
        }
    };
    const handleShowAddModal = () => {
        setShowAddModal(true);
    };
    const handleCloseAddModal = () => {
        setShowAddModal(false);
    };
    const handleShowEditModal = (employee) => {
        setSelectedEmployee(employee);
        setShowEditModal(true);
    };
    
    const handleCloseEditModal = () => {
        setSelectedEmployee(null);
        setShowEditModal(false);
    };
    
    const handleShowDeleteModal = (employee) => {
        setSelectedEmployee(employee);
        setDeleteID(employee.employeeId);
        setShowDeleteModal(true);
    };
    
      const handleCloseDeleteModal = () => {
        setDeleteID(null);
        setSelectedEmployee(null);
        setShowDeleteModal(false);
      };
    
    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }
    const handleConfirmDelete = async () => {
        try {
            await refreshAccessToken.delete(`${apiUrl}/${deleteID}`);
            fetchEmployees();
            handleCloseDeleteModal();
        } catch (error) {
            handleErrorResponse(error);
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
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Address</th>
                        <th>Position</th>
                        <th>Email</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {employees.map((employee) => (
                        <tr key={employee.employeeId}>
                            <td>{employee.employeeId}</td>
                            <td>{employee.firstName}</td>
                            <td>{employee.lastName}</td>
                            <td>{employee.address}</td>
                            <td>{employee.position}</td>
                            <td>{employee.email}</td>
                            <td>
                            <InputGroup className='mb-3'>
                                <DropdownButton variant="success" title="Actions" id="input-group-dropdown-1">
                                    <DropdownItem onClick={() => handleShowEditModal(employee)}><FaRegEdit className="text-success ud-cursor mb-1 mx-1"/>Edit</DropdownItem>
                                    <DropdownItem variant="danger" onClick={() => handleShowDeleteModal(employee)}><MdDeleteForever size={20} className="text-danger ud-cursor mb-1 mx-0"/>Delete</DropdownItem>
                                </DropdownButton>
                            </InputGroup>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            {selectedEmployee && (
                <EditEmployee show={showEditModal} handleClose={handleCloseEditModal} fetchEmployees={fetchEmployees} employee={selectedEmployee} />
            )}
            <AddEmployee show={showAddModal} handleClose={handleCloseAddModal} fetchEmployees={fetchEmployees} />
            {selectedEmployee && (
                <Modal show={showDeleteModal} onHide={handleCloseDeleteModal}>
                    <Modal.Header closeButton>
                    <Modal.Title>Bạn có muốn xoá bản ghi này không?</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                    <p>{selectedEmployee.employeeId}-{selectedEmployee.firstName} {selectedEmployee.lastName}-{selectedEmployee.position}</p>
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

export default ListEmployee;