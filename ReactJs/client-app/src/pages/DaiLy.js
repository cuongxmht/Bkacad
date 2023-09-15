import axios from 'axios';
import { useEffect, useState } from 'react';

function DaiLy(){
    const[maDL, setMa] = useState("");
    const[tenDL, setTen] = useState("");
    const[diaChi, setDiaChi] = useState("");
    const[nguoiDaiDien, setNguoiDaiDien] = useState("");
    const[dienThoai, setDienThoai] = useState("");
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
        <div>
        <h1>Danh sách Đại lý</h1>
        <hr></hr>
        <div class="container mt-4">
          <form>
            <div class="form-group">
            <div class="row">
                <div className='col-md-6'>
                
                    <div className='form-floating mb-1'>
                        <input type="text" className="form-control" id="maDaiLy" title='Mã đại lý' value={maDL}
                            onChange={(event) => { setMa(event.target.value); }} placeholder='Mã đại lý'/>
                        <label htmlFor="maDaiLy">Mã đại lý</label>
                    </div>
                    <div className='form-floating mb-1'>
                    <input type="text" class="form-control" id="tenDL" title='Tên đại lý' value={tenDL}
                        onChange={(event) => { setTen(event.target.value); }} placeholder='Tên đại lý'/>
                    <label for="tenDL">Tên đại lý</label>
                        </div>
                        <div className='form-floating mb-1'>
                    <input type="text" class="form-control" id="address" title='Địa chỉ' value={diaChi}
                        onChange={(event) => { setDiaChi(event.target.value); }} placeholder='Địa chỉ'/>
                    <label for="address">Địa chỉ</label>
                        </div>
                </div>
                <div className='col-md-6'>
                <div className='form-floating mb-1'>
                    <input type="text" class="form-control" id="nguoiDaiDien" title='Đại diện' value={nguoiDaiDien}
                        onChange={(event) => { setNguoiDaiDien(event.target.value); }} placeholder='Người đại diện' />
                    <label for="nguoiDaiDien">Người đại diện</label>
                        </div>
                        <div className='form-floating mb-1'>
                    <input type="text" class="form-control" id="dienThoai" title='Điện thoại' value={dienThoai}
                        onChange={(event) => { setDienThoai(event.target.value); }} placeholder='Số điện thoại' />
                    <label for="dienThoai">Số điện thoại</label>
                        </div>

                        <div className='form-floating mb-1'>
                    
                    {/* <input type="text" class="form-control" id="dienThoai" value={maHTPP}
                        onChange={(event) => { setMaHTPP(event.target.value); }} /> */}

                    <select className='form-control' id='htpp-selected' title='Chọn HTPP' placeholder='Hệ thống PP'
                        value={htpp_selected}
                        onChange={handleChange_HTPPSelected}
                    >
                        {
                            htpp_lst.map(function fn(hp){
                                return(
                                <option value={hp.maHTPP} key={hp.maHTPP}>{hp.tenHTPP}</option>
                                );
                            })
                        }
                    </select>
                    <label for="htpp-selected">Hệ thống phân phối</label>
                    </div>
                </div>
            </div>

            </div>
            <div class="mt-4 ">
                <button class="btn btn-secondary mx-1" onClick={setDaiLyNull}>Clear</button>
                <button class={isEdit? "btn btn-warning mx-1":"btn btn-primary mx-1"} onClick={isEdit?updateDaiLy : createDaiLy}>{isEdit?"Update" : "Register"}</button>
              
            </div>
          </form>
        </div>
        <br></br>
        <table class="table table-hover">
          <thead class="">
            <tr>
              <th scope="col">Mã ĐL</th>
              <th scope="col">Tên đại lý</th>
              <th scope="col">Địa chỉ</th>
              <th scope="col">Người đại diện</th>
              <th scope="col">Điện thoại</th>
              <th scope="col">Hệ thống phân phối</th>
              <th scope="col">Action</th>
            </tr>
          </thead>
          <tbody>
          {
            daiLys.map(function fn(dl) {
              return (
                  <tr key={dl.maDaiLy}>
                    <td>{dl.maDaiLy} </td>
                    <td>{dl.tenDaiLy}</td>
                    <td>{dl.diaChi}</td>
                    <td>{dl.nguoiDaiDien}</td>
                    <td>{dl.dienThoai}</td>
                    <td>{dl.heThongPhanPhoi.tenHTPP}</td>
                    <td>
                      <button type="button" class="btn btn-warning" onClick={() => setDaiLyValue(dl)}>Edit</button>
                      <button type="button" class="btn btn-danger" onClick={() => deleteDaiLy(dl.maDaiLy)}>Delete</button>
                    </td>
                  </tr>
              );
            })
          }
            </tbody>
      </table>
    </div>
    );

}






export default DaiLy;