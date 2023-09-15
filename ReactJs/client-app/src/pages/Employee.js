
import axios from 'axios';
import { useEffect, useState } from 'react';

function Employee(){
    const[employeeId, setEmployeeId] = useState("");
    const[age, setAge] = useState(0);
    const[personId, setPersonId] = useState("");
    const[fullName, setFullName] = useState("");
    const[address, setAddress] = useState("");
    const[maHTPP, setMaHTPP] = useState("");
    const[daiLys, setDaiLy] = useState([]);
    const[isEdit, setEdit]=useState(false);
    const [htpp_selected, setHTPPSelected] = useState("");
    const[htpp_lst, setHTPP] = useState([]);

    const handleChange_HTPPSelected = (event) => {
        setHTPPSelected(event.target.value);
    };
    useEffect(()=>{
        (async()=>await getAllDaiLy())();
        (async()=>await getAllHTPP())();
    }, []);

    async function setDaiLyValue(daiLys){
        setMa(daiLys.maDaiLy);
        setTen(daiLys.tenDaiLy);
        setDiaChi(daiLys.diaChi);
        setNguoiDaiDien(daiLys.nguoiDaiDien);
        setDienThoai(daiLys.dienThoai);
        setMaHTPP(daiLys.maHTPP);
        setEdit(true);
        setHTPPSelected(daiLys.maHTPP);
    }
    async function setDaiLyNull(){
        setMa("");
        setTen("");
        setDiaChi("");
        setNguoiDaiDien("");
        setDienThoai("");
        setMaHTPP("");
        setEdit(false);

        if(htpp_lst!=null){
            setHTPPSelected(htpp_lst[0].maHTPP);
        }
    }
    async function getAllDaiLy(){
        const results=await axios.get("http://localhost:5252/api/DaiLy/getall");
        setDaiLy(results.data);
    }
    async function getAllHTPP(){
        const results=await axios.get("http://localhost:5252/api/HeThongPhanPhoi");
        setHTPP(results.data);
        if(results.data!=null){
            setHTPPSelected(results.data[0].maHTPP);
        }
    }

        async function createDaiLy(event){
            event.preventDefault();
            try{
                await axios.post("http://localhost:5252/api/DaiLy",{
                    maDaiLy: maDL,
                    tenDaiLy: tenDL,
                    diaChi: diaChi,
                    nguoiDaiDien: nguoiDaiDien,
                    dienThoai: dienThoai,
                    maHTPP: htpp_selected
                });
    
                alert('Tạo đại lý thành công.');
                setDaiLyNull();
                getAllDaiLy();
    
            }
            catch(err)
            {
                alert(err);
            }
        }
        async function deleteDaiLy(id){
            if(id===""|| id==="undefined")
            {
                alert('id không hợp lệ: ' + id);
            }
            const confirm=window.confirm('Bạn muốn xoa bản ghi Id='+id);
            if(!confirm)return;
            await axios.delete("http://localhost:5252/api/DaiLy/"+ id);
            alert("Đã xóa thành công.");
            setDaiLyNull();
            getAllDaiLy();
        }
        async function updateDaiLy(event) {
            event.preventDefault();
            try {
              await axios.put("http://localhost:5252/api/DaiLy/" + maDL,
                {
                    maDaiLy: maDL,
                    tenDaiLy: tenDL,
                    diaChi: diaChi,
                    nguoiDaiDien: nguoiDaiDien,
                    dienThoai: dienThoai,
                    maHTPP: htpp_selected
                }
              );
              alert("Cập nhật đại lý thành công!");
              setDaiLyNull();
                getAllDaiLy();
            } catch (err) {
              alert(err);
            }
          }
    return (
    <>
    
    </>
    )
}

export default Employee;