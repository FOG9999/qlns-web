using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoach5NamDeXuat_ChiTiet_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        private Guid? _idKhthdx = Guid.NewGuid();
        private List<DuAnKeHoach5Nam> _lstDuAnChecked = new List<DuAnKeHoach5Nam>();

        public int iGiaiDoanTu { get; set; }
        //public bool isTongHop { get; set; }
        public KeHoach5NamDeXuat_ChiTiet_SheetTable()
        {

        }

        public KeHoach5NamDeXuat_ChiTiet_SheetTable(string Id, int iNamLamViec, int iGiaiDoanTu, List<DuAnKeHoach5Nam> lstDuAnChecked, Dictionary<string, string> query)
        {
            if(!string.IsNullOrEmpty(Id))
            {
                _idKhthdx = Guid.Parse(Id);
            }
            _lstDuAnChecked = lstDuAnChecked;           
            this.iGiaiDoanTu = iGiaiDoanTu;
            //this.isTongHop = isTongHop;
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(Id, iNamLamViec, filters);
        }

        #region private methods

        private void fillSheet(string Id, int iNamLamViec, Dictionary<string, string> filters)
        {
            #region Lay XauNoiMa_Cha
            ColumnNameId = "iID_KeHoach5Nam_DeXuat_ChiTietID";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bIsParent";
            #endregion
            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(!string.IsNullOrEmpty(Id) ? Guid.Parse(Id) : Guid.Empty);

            _filters = filters ?? new Dictionary<string, string>();
            
            if(itemQuery != null && itemQuery.iLoai.Equals(2))
            {
                DataTable dtCt = _iQLVonDauTuService.GetListKH5NamDeXuatChuyenTiepChiTietById(Id, _filters);
                DataTable dt = null;
                dtChiTiet = dtCt;
                if (_lstDuAnChecked.Count > 0)
                {
                    var rows = dtCt.AsEnumerable().Where(row => _lstDuAnChecked.Any(item => item.IDDuAnID == row.Field<Guid>("iID_DuAnID")));
                    if (rows.Any())
                        dt = rows.CopyToDataTable();
                    dtChiTiet = dt;
                }
                
            }
            else
            {
                if (_lstDuAnChecked.Count > 0)
                {
                    var listId = _lstDuAnChecked.Select(x => x.IDDuAnID);                   //tất cả dự án được tick chọn
                    var listSaved = _iQLVonDauTuService.GetListDuAnSavedKHTHDeXuat(Guid.Parse(Id)).Select(x => x.iID_DuAnID);             //list dự án đã được lưu vào db
                    //string listIdDuan = String.Join(",", _lstDuAnChecked.Select(x => x.IDDuAnID.ToString()).ToList());
                    var listAdd = listId.Where(x => !listSaved.Contains(x));
                    string listIdDuan = String.Join(",", listAdd);                  //= listId - listSaved  
                    //var listId = listIdChungTu.Split(',').ToList();
                    DataTable itemSaved = _iQLVonDauTuService.GetListKH5NamDeXuatChiTietById(Id, iNamLamViec, _filters);        // lấy lại những dòng chi tiết đã được lưu vào db
                    DataTable itemAdd = _iQLVonDauTuService.GetListDuAnChosen(listIdDuan, iNamLamViec, _filters);               // lấy lên những dòng chi tiết thêm mới
                    //dtChiTiet = _iQLVonDauTuService.GetListDuAnChosen(listIdDuan, iNamLamViec, _filters);
                    
                    if(itemAdd != null)
                    {
                        List<string> listParent = new List<string>();
                        List<int> listRemoved = new List<int>();
                        for (int i = 0; i < itemAdd.Rows.Count; i++)
                        {
                            var row = itemAdd.Rows[i];
                            if (row["iLevel"].ToString() == "2" && row["numChild"].ToString() != "")
                            {
                                // nếu có con mà chưa có trong listParent thì xóa nguồn vốn
                                if (!listParent.Contains(row["iID_DuAnID"].ToString()))
                                {
                                    row["iID_NguonVonID"] = DBNull.Value;
                                    row["sTenNganSach"] = DBNull.Value;
                                    listParent.Add(row["iID_DuAnID"].ToString());
                                }
                                // có rồi thì bỏ dòng
                                else
                                {
                                    listRemoved.Add(i);
                                }
                            }
                        }

                        DataTable cloneList = new DataTable();
                        for (int i = 0; i < itemAdd.Columns.Count; i++)
                        {
                            cloneList.Columns.Add(itemAdd.Columns[i].ColumnName);
                        }
                        for (int i = 0; i < itemAdd.Rows.Count; i++)
                        {
                            if (!listRemoved.Contains(i))
                            {
                                cloneList.ImportRow(itemAdd.Rows[i]);
                            }
                        }

                        /*int sttCha = 1;
                        var map = new Dictionary<string, KeyValuePair<int, int>>();
                        var mapValue = new KeyValuePair<int, int>();
                        Guid currParentId = Guid.Empty;
                        for (int i = 0; i < cloneList.Rows.Count; i++)
                        {
                            var row = cloneList.Rows[i];
                            //nếu row có iLevel = 2(dòng dự án) thì đánh stt tăng dần bình thường
                            if (row["iLevel"].ToString() == "2")
                            {
                                row["sSTT"] = sttCha.ToString();
                                if (!map.ContainsKey(row["iID_DuAnID"].ToString()))
                                {
                                    map.Add(row["iID_DuAnID"].ToString(), new KeyValuePair<int, int>(sttCha, 0));
                                }
                                sttCha++;
                            }
                            //nếu row có iLevel = 3(dòng chi tiết) thì cần xác định dòng dự án cha và stt con max hiện tại
                            if (row["iLevel"].ToString() == "3" && map.TryGetValue(row["iID_DuAnID"].ToString(), out mapValue))
                            {
                                row["sSTT"] = map[row["iID_DuAnID"].ToString()].Key.ToString() + "." + (map[row["iID_DuAnID"].ToString()].Value + 1).ToString();
                                var newKeyPair = new KeyValuePair<int, int>(map[row["iID_DuAnID"].ToString()].Key, map[row["iID_DuAnID"].ToString()].Value + 1);
                                map[row["iID_DuAnID"].ToString()] = newKeyPair;             // cập nhật stt con max
                            }

                            // sinh khóa chính id cho quan hệ cha con
                            row[ColumnNameId] = Guid.NewGuid();
                            // nếu dự án có chi tiết
                            if ((row[ColumnNameIsParent].ToString() == "1" || Convert.ToBoolean(row[ColumnNameIsParent])) && row["iLevel"].ToString() == "2")
                            {
                                currParentId = Guid.Parse(row[ColumnNameId].ToString());
                                row["iID_ParentID"] = currParentId;
                            }
                            else if ((row[ColumnNameIsParent].ToString() == "0" || !Convert.ToBoolean(row[ColumnNameIsParent])) && row["iLevel"].ToString() == "3")
                            {
                                row["iID_ParentID"] = currParentId;
                            }
                            else row["iID_ParentID"] = row[ColumnNameId];
                        }*/                        
                        if(cloneList != null)
                        {
                            Guid currParentId = Guid.Empty;
                            for (int i = 0; i < cloneList.Rows.Count; i++)
                            {
                                var row = cloneList.Rows[i];
                                // sinh khóa chính id cho quan hệ cha con
                                row[ColumnNameId] = Guid.NewGuid();
                                // nếu dự án có chi tiết
                                if ((row[ColumnNameIsParent].ToString() == "1" || Convert.ToBoolean(row[ColumnNameIsParent])) && row["iLevel"].ToString() == "2")
                                {
                                    currParentId = Guid.Parse(row[ColumnNameId].ToString());
                                    row["iID_ParentID"] = currParentId;
                                }
                                else if ((row[ColumnNameIsParent].ToString() == "0" || !Convert.ToBoolean(row[ColumnNameIsParent])) && row["iLevel"].ToString() == "3")
                                {
                                    row["iID_ParentID"] = currParentId;
                                }
                                else row["iID_ParentID"] = row[ColumnNameId];
                            }
                        }
                        itemAdd = cloneList;
                        if (itemSaved != null)
                        {
                            itemSaved.Merge(itemAdd, true, MissingSchemaAction.Ignore);
                            dtChiTiet = itemSaved;
                        }
                        else
                        {
                            itemAdd.Merge(itemSaved, true, MissingSchemaAction.Ignore);
                            dtChiTiet = itemAdd;
                        }
                        /*itemAdd.Merge(itemSaved, true, MissingSchemaAction.Ignore);
                        dtChiTiet = itemAdd;*/                        
                    }
                    else
                    {
                        dtChiTiet = itemSaved;
                    }

                    //sinh STT
                    int sttCha = 1;
                    var map = new Dictionary<string, KeyValuePair<int, int>>();
                    var mapValue = new KeyValuePair<int, int>();                    
                    for (int i = 0; i < dtChiTiet.Rows.Count; i++)
                    {
                        var row = dtChiTiet.Rows[i];
                        //nếu row có iLevel = 2(dòng dự án) thì đánh stt tăng dần bình thường
                        if (row["iLevel"].ToString() == "2")
                        {
                            row["sSTT"] = sttCha.ToString();
                            if (!map.ContainsKey(row["iID_DuAnID"].ToString()))
                            {
                                map.Add(row["iID_DuAnID"].ToString(), new KeyValuePair<int, int>(sttCha, 0));
                            }
                            sttCha++;
                        }
                        //nếu row có iLevel = 3(dòng chi tiết) thì cần xác định dòng dự án cha và stt con max hiện tại
                        if (row["iLevel"].ToString() == "3" && map.TryGetValue(row["iID_DuAnID"].ToString(), out mapValue))
                        {
                            row["sSTT"] = map[row["iID_DuAnID"].ToString()].Key.ToString() + "." + (map[row["iID_DuAnID"].ToString()].Value + 1).ToString();
                            var newKeyPair = new KeyValuePair<int, int>(map[row["iID_DuAnID"].ToString()].Key, map[row["iID_DuAnID"].ToString()].Value + 1);
                            map[row["iID_DuAnID"].ToString()] = newKeyPair;             // cập nhật stt con max
                        }                        
                    }
                }
                else
                {
                    dtChiTiet = _iQLVonDauTuService.GetListKH5NamDeXuatChiTietById(Id, iNamLamViec, _filters);
                }
                
            }
            if (dtChiTiet == null)
            {
                dtChiTiet = new DataTable();
            }
            dtChiTiet_Cu = dtChiTiet.Copy();

            fillData(Id);
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties();

            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private void fillData(string iID_KeHoach5NamID)
        {
            updateSummaryRows1(iID_KeHoach5NamID);
            // updateColumnIDs("iID_KeHoach5Nam_DeXuat_ChiTietID");
            //updateColumnIDsCT("iID_KeHoach5Nam_DeXuat_ChiTietID");
            updateColumnIDsCTKHTH("iID_KeHoach5Nam_DeXuat_ChiTietID");
            //UpdateColumIDsKH5NamDXChiTiet("Id");
            updateColumns();
            updateColumnsParent1();
            //updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
            updateArrThayDoi();

        }

        private void updateArrThayDoi()
        {
            List<string> arrSpecial = new List<string>();
            arrSpecial.Add("sTen");
            arrSpecial.Add("iID_DonViID");
            arrSpecial.Add("sDiaDiem");
            arrSpecial.Add("iGiaiDoanTu");
            arrSpecial.Add("iGiaiDoanDen");
            arrSpecial.Add("iID_LoaiCongTrinhID");
            arrSpecial.Add("iID_NguonVonID");            
            arrSpecial.Add("sTenLoaiCongTrinh");            
            arrSpecial.Add("sTenNganSach");                                    
            arrSpecial.Add("iID_ParentID");            
            arrSpecial.Add("sSTT");            
            List<int> listIndex = new List<int>();
            arrSpecial.ForEach(h =>
            {
                listIndex.Add(arrDSMaCot.FindIndex(x => x == h));
            });
            for(int i = 0; i < _arrThayDoi.Count; i++)
            {
                listIndex.ForEach(ind =>
                {
                    _arrThayDoi[i][ind] = true;
                });
            }            
        }

        protected virtual void updateColumnsParent1()
        {
            //Xác định hàng là hàng cha, con
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                //arrLaHangCha.Add(Convert.ToBoolean(r[ColumnNameIsParent]));                       
                arrLaHangCha.Add((r[ColumnNameIsParent].ToString() == "0" || (r[ColumnNameIsParent].ToString() == "False")) ? false : true);                


                int parent = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(r[ColumnNameId]) == Convert.ToString(dtChiTiet.Rows[j][ColumnNameId]))
                    {
                        parent = j;
                        break;
                    }

                    if (Convert.ToString(r[ColumnNameParentId]) == Convert.ToString(dtChiTiet.Rows[j][ColumnNameId]))
                    {
                        parent = j;
                        break;
                    }
                }
                arrCSCha.Add(parent);
            }
        }

        /// <summary>
        /// Tính tổng cho các hàng cha tại các cột là number
        /// </summary>
        protected void updateSummaryRows1(string iID_KeHoach5NamID)
        {
            var columns = Columns.Where(x => x.DataType == 1).ToList();
            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(!string.IsNullOrEmpty(iID_KeHoach5NamID) ? Guid.Parse(iID_KeHoach5NamID) : Guid.Empty);
            //Tinh lai cac hang cha
            for (int i = dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                //Tinh lai cac hang cha
                string iID_MaMucLucNganSach = Convert.ToString(dtChiTiet.Rows[i][ColumnNameId]);
                for (int k = 0; k < columns.Count; k++)
                {
                    double S;
                    S = 0;
                    double S1 = 0;
                    for (int j = i + 1; j < dtChiTiet.Rows.Count; j++)
                    {
                        if (iID_MaMucLucNganSach == Convert.ToString(dtChiTiet.Rows[j][ColumnNameParentId]))
                        {
                            S += Convert.ToDouble(dtChiTiet.Rows[j][columns[k].ColumnName]);
                        }
                        if (iID_MaMucLucNganSach == Convert.ToString(dtChiTiet.Rows[j][ColumnNameId]))
                        {
                            S1 += Convert.ToDouble(dtChiTiet.Rows[j][columns[k].ColumnName]);
                        }
                    }
                    if (dtChiTiet.Rows[i][columns[k].ColumnName] == DBNull.Value || (Convert.ToDouble(dtChiTiet.Rows[i][columns[k].ColumnName]) != S) && S != 0)
                    {
                        dtChiTiet.Rows[i][columns[k].ColumnName] = S;                        
                    }
                }
            }

            //check neu la chung tu tong hop dieu chinh thi lay fGiaTriNamThuNhat = fGiaTriNamThuNhatDc
            if(dtChiTiet.Rows.Count > 0)
            {
                var iID_KeHoach5Nam_DeXuatID = Convert.ToString(dtChiTiet.Rows[0]["iID_KeHoach5Nam_DeXuatID"]);
                if (iID_KeHoach5Nam_DeXuatID != null && iID_KeHoach5Nam_DeXuatID != string.Empty)
                {
                    var itemTH = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Guid.Parse(dtChiTiet.Rows[0]["iID_KeHoach5Nam_DeXuatID"].ToString()));
                    if (itemTH.sTongHop != null)
                    {
                        for (int i = 0; i < dtChiTiet.Rows.Count; i++)
                        {
                            if (Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuNhatDc"]) > 0 || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuHaiDc"]) > 0
                                || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuBaDc"]) > 0 || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuTuDc"]) > 0
                                || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuNamDc"]) > 0)
                            {
                                dtChiTiet.Rows[i]["fGiaTriNamThuNhat"] = dtChiTiet.Rows[i]["fGiaTriNamThuNhatDc"];
                                dtChiTiet.Rows[i]["fGiaTriNamThuHai"] = dtChiTiet.Rows[i]["fGiaTriNamThuHaiDc"];
                                dtChiTiet.Rows[i]["fGiaTriNamThuBa"] = dtChiTiet.Rows[i]["fGiaTriNamThuBaDc"];
                                dtChiTiet.Rows[i]["fGiaTriNamThuTu"] = dtChiTiet.Rows[i]["fGiaTriNamThuTuDc"];
                                dtChiTiet.Rows[i]["fGiaTriNamThuNam"] = dtChiTiet.Rows[i]["fGiaTriNamThuNamDc"];
                                dtChiTiet.Rows[i]["fGiaTriBoTri"] = dtChiTiet.Rows[i]["fGiaTriBoTriDc"];
                            }
                        }
                    }
                }
            }                        

            if(itemQuery != null && !itemQuery.iLoai.Equals(2))
            {
                //Sau khi tinh tong cha thi luu lai vao DB
                IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> newData = ConvertToModels(dtChiTiet);
                _iQLVonDauTuService.SaveKeHoach5NamDeXuatChiTiet(newData);
            }
            //update fGiaTriKeHoach
            _iQLVonDauTuService.UpdateGiaTriKeHoach(iID_KeHoach5NamID);
        }

        //duonglt test
        protected void updateColumnIDsCTKHTH(string columns)
        {
            _arrDSMaHang = new List<string>();
            var fields = columns.Split(',').ToList();
            var numRow = dtChiTiet.Rows.Count;
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    //var id = !string.IsNullOrWhiteSpace(ColumnNameIsParent) && r.Field<bool>(ColumnNameIsParent) && !ParentRowEditable ? "" : fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    var id = fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    if (numRow == 1 && (id == "" || id == null))
                    {
                        id = Guid.Empty.ToString();
                    }
                    _arrDSMaHang.Add(id);
                });
        }
        //end test

        private IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> ConvertToModels(DataTable dataTable)
        {
            var itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(_idKhthdx);
            if (itemQuery != null && itemQuery.iID_ParentId != null && itemQuery.iID_ParentId.HasValue)
            {
                return dataTable.AsEnumerable().Select(row => new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
                {
                    iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString()),
                    fHanMucDauTu = Convert.ToDouble(row["fHanMucDauTu"]),
                    fGiaTriNamThuNhat = Convert.ToDouble(row["fGiaTriNamThuNhatDc"]),
                    fGiaTriNamThuHai = Convert.ToDouble(row["fGiaTriNamThuHaiDc"]),
                    fGiaTriNamThuBa = Convert.ToDouble(row["fGiaTriNamThuBaDc"]),
                    fGiaTriNamThuTu = Convert.ToDouble(row["fGiaTriNamThuTuDc"]),
                    fGiaTriNamThuNam = Convert.ToDouble(row["fGiaTriNamThuNamDc"]),
                    fGiaTriBoTri = Convert.ToDouble(row["fGiaTriBoTriDc"])
                });
            }
            else
            {
                return dataTable.AsEnumerable().Select(row => new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
                {
                    //iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString()),
                    iID_KeHoach5Nam_DeXuat_ChiTietID = row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString() != "" ? Guid.Parse(row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString()) : Guid.Empty,
                    fHanMucDauTu = Convert.ToDouble(row["fHanMucDauTu"]),
                    fGiaTriNamThuNhat = Convert.ToDouble(row["fGiaTriNamThuNhat"]),
                    fGiaTriNamThuHai = Convert.ToDouble(row["fGiaTriNamThuHai"]),
                    fGiaTriNamThuBa = Convert.ToDouble(row["fGiaTriNamThuBa"]),
                    fGiaTriNamThuTu = Convert.ToDouble(row["fGiaTriNamThuTu"]),
                    fGiaTriNamThuNam = Convert.ToDouble(row["fGiaTriNamThuNam"]),
                    fGiaTriBoTri = Convert.ToDouble(row["fGiaTriBoTri"])
                });
            }
        }

        protected virtual void UpdateColumIDsKH5NamDXChiTiet(string columns)
        {
            _arrDSMaHang = new List<string>();

            var fields = columns.Split(',').ToList();
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var id = fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    _arrDSMaHang.Add(id);
                });
        }
        #endregion

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var cNameNam1 = "Năm " + this.iGiaiDoanTu;
            var cNameNam2 = "Năm " + (this.iGiaiDoanTu + 1);
            var cNameNam3 = "Năm " + (this.iGiaiDoanTu + 2);
            var cNameNam4 = "Năm " + (this.iGiaiDoanTu + 3);
            var cNameNam5 = "Năm " + (this.iGiaiDoanTu + 4);
            var cNameVonBoTri = "Vốn bố trí sau năm " + (this.iGiaiDoanTu + 4);
            var cNameLuyKeVon = "Lũy kế vốn NSQP đã bố trí hết năm " + (this.iGiaiDoanTu - 2);
            var cNameVonBoTriNam = "Vốn NSQP đề nghị bố trí năm " + (this.iGiaiDoanTu - 1);

            var cNameNam1AfterModified = "Năm " + this.iGiaiDoanTu + "(Sau điều chỉnh)";
            var cNameNam2AfterModified = "Năm " + (this.iGiaiDoanTu + 1) + "(Sau điều chỉnh)";
            var cNameNam3AfterModified = "Năm " + (this.iGiaiDoanTu + 2) + "(Sau điều chỉnh)";
            var cNameNam4AfterModified = "Năm " + (this.iGiaiDoanTu + 3) + "(Sau điều chỉnh)";
            var cNameNam5AfterModified= "Năm " + (this.iGiaiDoanTu + 4) + "(Sau điều chỉnh)";
            var cNameVonBoTriModified = "Vốn bố trí sau năm " + (this.iGiaiDoanTu + 4) + "(Sau điều chỉnh)";

            var headerGroup = "NHU CẦU BỐ TRÍ VỐN " + this.iGiaiDoanTu + " - " + (this.iGiaiDoanTu + 4);

            var cNganhDeNghi = "Ngành đề nghị";
            var cCucTaiChinhDeXuat = "Cục tài chính đề xuất";
            var cVonBoTriNam = "Vốn bố trí năm " + (this.iGiaiDoanTu) +" - "+(this.iGiaiDoanTu+4);
            var cVonBoTriSauNam = "Vốn bố trí sau năm " + (this.iGiaiDoanTu + 4);
            var cRiengNamN = "Riêng năm " + (this.iGiaiDoanTu);
            var cHanMucDauTuCongTrinhMoMoi = "Hạn mức đầu tư công trình mở mới " + (this.iGiaiDoanTu);
            var cTongVonBoTri = "Tổng vốn bố trí";
            var cDuKienBoTriNamThu2 = "Dự kiến bố trí vốn " + (this.iGiaiDoanTu);
            var fCucTCDeXuat = "Cục tài chính đề xuất Kế hoạch " + this.iGiaiDoanTu + " - " + (this.iGiaiDoanTu + 4);

            bool isHiddenCt = true;
            bool isHiddenDc = true;
            int indexGroupHeader = 6;
            VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(_idKhthdx);
            if(itemQuery != null)
            {
                if(itemQuery.iID_ParentId != null && itemQuery.iID_ParentId.HasValue)
                {
                    isHiddenCt = true;
                    isHiddenDc = false;
                    indexGroupHeader = 11;
                }

                if(itemQuery.iLoai.Equals(2))
                {
                    isHiddenCt = false;
                }
            }

            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "sSTT", header: "STT", columnWidth:50, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTen", header: "Tên Group - dự án", columnWidth:200, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sQuyetDinhCTDT", header: "Số quyết định CTĐT", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sChuDauTu", header: "Chủ đầu tư", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    //new SheetColumn(columnName: "sDonViThucHienDuAn", header: "Đơn vị thực hiện dự án", columnWidth:200, align: "left", hasSearch: true, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "sDonVi", header: "Đơn vị", columnWidth:200, align: "left", hasSearch: true, dataType: 3, isReadonly: true),
                    new SheetColumn(columnName: "sDiaDiem", header: "Địa điểm thực hiện", columnWidth:150, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "iGiaiDoanTu", header: "Thời gian từ", columnWidth:100, align: "center", hasSearch: true, isReadonly: false, dataType: 3),
                    new SheetColumn(columnName: "iGiaiDoanDen", header: "Thời gian đến", columnWidth:100, align: "center", hasSearch: true, isReadonly: false, dataType: 3),
                    new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:200, align: "left", hasSearch: false, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "sTenNganSach", header: "Nguồn vốn", headerGroup: "HẠN MỨC ĐẦU TƯ ĐỀ NGHỊ", headerGroupIndex: 2, columnWidth:120, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "fHanMucDauTu", header: "Hạn mức đầu tư", headerGroup: "HẠN MỨC ĐẦU TƯ ĐỀ NGHỊ", headerGroupIndex: 2, columnWidth:120, dataType: 1, isReadonly: false),                  
                    new SheetColumn(columnName: "fLuyKeNSQPDaBoTri", header: cNameLuyKeVon, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenCt),
                    new SheetColumn(columnName: "fLuyKeNSQPDeNghiBoTri", header: cNameVonBoTriNam, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenCt),
                    new SheetColumn(columnName: "fTongSoNhuCauNSQP", header: "Tổng số nhu cầu NSQP", columnWidth:170, align: "left", hasSearch: false, dataType: 1, isReadonly: true, isHidden: !isHiddenCt),
                    new SheetColumn(columnName: "fTongSo", header: "Tổng số", headerGroup: headerGroup, headerGroupIndex: indexGroupHeader, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuNhat", header: cNameNam1, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTriNamThuNhatDc", header: cNameNam1AfterModified, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenDc),
                    new SheetColumn(columnName: "fGiaTriNamThuHai", header: cNameNam2, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTriNamThuHaiDc", header: cNameNam2AfterModified, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenDc),
                    new SheetColumn(columnName: "fGiaTriNamThuBa", header: cNameNam3, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTriNamThuBaDc", header: cNameNam3AfterModified, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenDc),
                    new SheetColumn(columnName: "fGiaTriNamThuTu", header: cNameNam4, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTriNamThuTuDc", header: cNameNam4AfterModified, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenDc),
                    new SheetColumn(columnName: "fGiaTriNamThuNam", header: cNameNam5, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTriNamThuNamDc", header: cNameNam5AfterModified, headerGroup: headerGroup, headerGroupIndex: 11, columnWidth:120, dataType: 1, isReadonly: false, isHidden: isHiddenDc),
                    new SheetColumn(columnName: "fGiaTriBoTri", header: cNameVonBoTri, columnWidth:170, align: "left", hasSearch: false, dataType: 1, isReadonly: true, isHidden: !isHiddenCt),
                    new SheetColumn(columnName: "fGiaTriBoTriDc", header: cNameVonBoTriModified, columnWidth:170, align: "left", hasSearch: false, dataType: 1, isReadonly: true, isHidden: isHiddenDc),
                    //Add Col mo moi
                    new SheetColumn(columnName: "fHanmucNganhDX", header: cHanMucDauTuCongTrinhMoMoi, headerGroup: cNganhDeNghi, headerGroupIndex: 4, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fTongVonBoTriNganh", header: cTongVonBoTri, headerGroup: cNganhDeNghi, headerGroupIndex: 4, columnWidth:120, dataType: 1, isReadonly: true,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fVon5namNganhDX", header: cVonBoTriNam, headerGroup: cNganhDeNghi, headerGroupIndex: 4, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fVonsaunamNganhDX", header: cVonBoTriSauNam, headerGroup: cNganhDeNghi, headerGroupIndex: 4, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),                   
                    new SheetColumn(columnName: "fHanmucCucTCDX", header: "Hạn mức đầu tư NSQP công trình mở mới", headerGroup: cCucTaiChinhDeXuat, headerGroupIndex: 5, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fTongVonBoTriCuc", header: cTongVonBoTri, headerGroup: cCucTaiChinhDeXuat, headerGroupIndex: 5, columnWidth:120, dataType: 1, isReadonly: true,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fVon5namCTCDX", header: cVonBoTriNam, headerGroup: cCucTaiChinhDeXuat, headerGroupIndex: 5, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fVonnamthunhatCTC", header: cRiengNamN, headerGroup: cCucTaiChinhDeXuat, headerGroupIndex: 5, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    new SheetColumn(columnName: "fVonsaunamCTCDexuat", header: cVonBoTriSauNam, headerGroup: cCucTaiChinhDeXuat, headerGroupIndex: 5, columnWidth:120, dataType: 1, isReadonly: false,isHidden:!isHiddenCt),
                    //Add Col mo moi
                    new SheetColumn(columnName: "fCucTCDeXuat", header: fCucTCDeXuat, columnWidth:120, dataType: 1, isReadonly: false,isHidden:isHiddenCt),
                    new SheetColumn(columnName: "fDuKienBoTriNamThu2", header: cDuKienBoTriNamThu2, columnWidth:120, dataType: 1, isReadonly: false,isHidden:isHiddenCt),
                    new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:250, align: "left", hasSearch: false, dataType: 0, isReadonly: false),                  
                    // cot khac
                    new SheetColumn(columnName: "sDuAnCha", header: "Group - Dự án - Chi tiết cha", columnWidth:200, align: "left", hasSearch: false, dataType: 3, isReadonly: false, isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoach5Nam_DeXuat_ChiTietID", isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoach5Nam_DeXuatID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "iID_LoaiCongTrinhID", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonVonID", isHidden: true),
                    new SheetColumn(columnName: "numChild", isHidden: true),
                    new SheetColumn(columnName: "iIDReference", isHidden: true),
                    new SheetColumn(columnName: "iLevel", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName: "iID_ParentModified", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAnID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViID", isHidden: true),
                };

            #endregion

            list = list.OrderByDescending(item => !item.IsHidden).ToList();

            return list;
        }
    }
}