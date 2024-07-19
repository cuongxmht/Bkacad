import React, { useState, useEffect } from 'react';
import refreshAccessToken from '../account/RefreshAccessToken';
import { Table, Button } from 'react-bootstrap';
import AddDaily from './AddDaily';

const ListDaily = () => {
    const apiUrl=process.env.REACT_APP_API_BASE_URL+"/api/Daily"
    const[daiLys, setDaiLys]=useState([]);
    const[showAddModal,setShowAddModal]=useState(false);
    useEffect(()=>{
        fetchData();
    },[]);
    const fetchData=async ()=>{
        const response=await refreshAccessToken.get(apiUrl);
        setDaiLys(response.data);
    }
    const handleShowAddModal=()=>setShowAddModal(true);
    const handleCloseAddModal=()=>setShowAddModal(false);
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
                        <th>Dai ly ID</th>
                        <th>Ten dai ly</th>
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
                            {/* <InputGroup className='mb-3'>
                                <DropdownButton variant="success" title="Actions" id="input-group-dropdown-1">
                                    <DropdownItem onClick={() => handleShowEditModal(employee)}><FaRegEdit className="text-success ud-cursor mb-1 mx-1"/>Edit</DropdownItem>
                                    <DropdownItem variant="danger" onClick={() => handleShowDeleteModal(employee)}><MdDeleteForever size={20} className="text-danger ud-cursor mb-1 mx-0"/>Delete</DropdownItem>
                                </DropdownButton>
                            </InputGroup> */}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            <AddDaily show={showAddModal} fetchData={fetchData} handleClose ={handleCloseAddModal} ></AddDaily>
        </div>
    );
};

export default ListDaily;