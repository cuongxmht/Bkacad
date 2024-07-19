import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, InputGroup, DropdownButton, DropdownItem } from 'react-bootstrap';
import AddDaily from './AddDaily';
import { FaRegEdit } from 'react-icons/fa';
import { MdDeleteForever } from 'react-icons/md';
import { useNavigate } from 'react-router-dom';
import refreshAccessToken from '../account/RefreshAccessToken';

const DaiLyList = () => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL + '/api/DaiLy';
    const [daiLys, setDaiLys] = useState([]);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] =useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selectedDaiLy, setSelectedDaiLy] = useState(null);
    const [deleteID, setDeleteID] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();
    useEffect(() => {
        fetchDaiLys();
        debugger;
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
    const fetchDaiLys = async () => {
        debugger;
        try {
            const response = await refreshAccessToken.get(apiUrl);
            setLoading(false);
            setDaiLys(response.data);
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
    const handleShowEditModal = (daiLy) => {
        setSelectedDaiLy(daiLy);
        setShowEditModal(true);
    };
    
    const handleCloseEditModal = () => {
        setSelectedDaiLy(null);
        setShowEditModal(false);
    };
    
    const handleShowDeleteModal = (daiLy) => {
        setSelectedDaiLy(daiLy);
        setDeleteID(daiLy.daiLyId);
        setShowDeleteModal(true);
    };
    
      const handleCloseDeleteModal = () => {
        setDeleteID(null);
        setSelectedDaiLy(null);
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
            fetchDaiLys();
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
                        <th>Ten Dai ly</th>
                        <th>Dia chi</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {daiLys.map((daiLy) => (
                        <tr key={daiLy.daiLyId}>
                            <td>{daiLy.daiLyId}</td>
                            <td>{daiLy.tenDaiLy}</td>
                            <td>{daiLy.diaChi}</td>
                            <td>
                            <InputGroup className='mb-3'>
                                <DropdownButton variant="success" title="Actions" id="input-group-dropdown-1">
                                    <DropdownItem onClick={() => handleShowEditModal(daiLy)}><FaRegEdit className="text-success ud-cursor mb-1 mx-1"/>Edit</DropdownItem>
                                    <DropdownItem variant="danger" onClick={() => handleShowDeleteModal(daiLy)}><MdDeleteForever size={20} className="text-danger ud-cursor mb-1 mx-0"/>Delete</DropdownItem>
                                </DropdownButton>
                            </InputGroup>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            {/* {selectedEmployee && (
                <EditDai show={showEditModal} handleClose={handleCloseEditModal} fetchEmployees={fetchEmployees} employee={selectedEmployee} />
            )} */}
            <AddDaily show={showAddModal} handleClose={handleCloseAddModal} fetchData={fetchDaiLys} />
            {/* {selectedEmployee && (
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
            )} */}
        </div>
    );
};

export default DaiLyList;