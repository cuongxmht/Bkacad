import axios from 'axios';
import { useEffect, useState } from 'react';

function Person(){
    const[id, setPersonId]=useState("");
    const[fName, setFullName]=useState("");
    const[address, setAddress]=useState("");
    const[nonSignName, setNonSignName] = useState("");
    const[persons, setPerson]=useState([]);
    const[isEdit, setEdit]=useState(false);

    useEffect(()=>{
        (async()=>await getAllPersons())();
    },[]);

    async function setPersonValue(persons){
        setFullName(persons.fullName);
        setAddress(persons.address);
        setPersonId(persons.personId);
        setNonSignName(persons.nonSignName);
        setEdit(true);
    }
    async function setPersonNull(){
        setFullName("");
        setAddress("");
        setPersonId("");
        setNonSignName("");
        setEdit(false);
    }
    async function getAllPersons(){
        const results=await axios.get("http://localhost:5252/api/Person");
        setPerson(results.data);
    }
    async function createPerson(event){
        event.preventDefault();
        try{
            await axios.post("http://localhost:5252/api/Person/create-person",{
                personId:id,
                fullName:fName,
                address:address
            });

            alert('Tạo person thành công.');
            setPersonNull();
            getAllPersons();

        }
        catch(err)
        {
            alert(err);
        }
    }
    async function deletePerson(id){
        if(id===""|| id==="undefined")
        {
            alert('id không hợp lệ: ' + id);
        }
        const confirm=window.confirm('Bạn muốn xoa bản ghi Id='+id);
        if(!confirm)return;
        await axios.delete("http://localhost:5252/api/Person/delete/"+ id);
        alert("Đã xóa thành công.");
        setPersonNull();
        getAllPersons();
    }
    async function updatePerson(event) {
        event.preventDefault();
        try {
          await axios.put("http://localhost:5252/api/Person/update-person/" + id,
            {
                personId: id,
                fullName: fName,
                address: address
            }
          );
          alert("Update successful person!");
          setPersonNull();
          getAllPersons();
        } catch (err) {
          alert(err);
        }
      }


    return (
        <div>
        <h1>Danh sach Person</h1>
        <hr></hr>
        <div class="container mt-4">
          <form>
            <label>PersonID</label>
            <div class="form-group">
              <input type="text" class="form-control" id="personID" value={id}
                onChange={(event) => { setPersonId(event.target.value); }} />
              <label>Person Name</label>
              <input type="text" class="form-control" id="fullName" value={fName}
                onChange={(event) => { setFullName(event.target.value); }} />
              <label>Address</label>
              <input type="text" class="form-control" id="address" value={address}
                onChange={(event) => { setAddress(event.target.value); }} />
                
            </div>
            <div class="mt-4 ">
                <button class="btn btn-secondary mx-1" onClick={setPersonNull}>Clear</button>
                <button class={isEdit? "btn btn-warning mx-1":"btn btn-primary mx-1"} onClick={isEdit?updatePerson : createPerson}>{isEdit?"Update" : "Register"}</button>
              {/* <button class="btn btn-warning mt-4" onClick={updatePerson}>Update</button> */}
            </div>
          </form>
        </div>
        <br></br>
        <table class="table table-hover">
          <thead class="">
            <tr>
              <th scope="col">Person Id</th>
              <th scope="col">Person Name</th>
              <th scope="col">Address</th>
              <th scope="col">NonSignName</th>
              <th scope="col">Action</th>
            </tr>
          </thead>
          {
            persons.map(function fn(ps) {
              return (
                <tbody>
                  <tr>
                    <td>{ps.personId} </td>
                    <td>{ps.fullName}</td>
                    <td>{ps.address}</td>
                    <td>{ps.nonSignName}</td>
                    <td>
                      <button type="button" class="btn btn-warning" onClick={() => setPersonValue(ps)}>Edit</button>
                      <button type="button" class="btn btn-danger" onClick={() => deletePerson(ps.personId)}>Delete</button>
                    </td>
                  </tr>
                </tbody>
              );
            })
          }
      </table>
    </div>
    );
}

// const Person = ()=>{
//     return (
//         <h2>Person page.</h2>
//     );
// };

export default Person;