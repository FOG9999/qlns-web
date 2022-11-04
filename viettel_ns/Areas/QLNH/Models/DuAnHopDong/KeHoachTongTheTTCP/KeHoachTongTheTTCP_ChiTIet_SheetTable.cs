using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachTongTheTTCP
{
    public class KeHoachTongTheTTCP_ChiTIet_SheetTable : SheetTable
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private Guid? _idKhtt = Guid.NewGuid();
        public KeHoachTongTheTTCP_ChiTIet_SheetTable()
        {

        }

        public KeHoachTongTheTTCP_ChiTIet_SheetTable(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan, Dictionary<string, string> filters)
        {
            var conditions = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    if(c.ColumnName == "state")
                    {
                        conditions.Add(c.ColumnName, state);
                    }
                    else if (c.ColumnName == "KHTTCP_ID")
                    {
                        conditions.Add(c.ColumnName, KHTTCP_ID.ToString());
                    }
                    else
                    {
                        conditions.Add(c.ColumnName, filters.ContainsKey(c.ColumnName) ? filters[c.ColumnName] : "");
                    }
                });

            fillSheet(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan, conditions);
        }

        #region private methods

        private void fillSheet(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan, Dictionary<string, string> filters)
        {
            NH_KHTongTheTTCP_NVCViewModel itemQuery = _qlnhService.GetDetailKeHoachTongTheTTCP(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan);
            _filters = filters ?? new Dictionary<string, string>();
            if (itemQuery == null)
            {
                dtChiTiet = new DataTable();
            }
            else dtChiTiet = ToDataTable(itemQuery.Items);
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "ID";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bIsHasChild";
            #endregion

            fillData(state, KHTTCP_ID, iID_BQuanLyID);
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

        private void fillData(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID)
        {
            //updateSummaryRows1(state, KHTTCP_ID, iID_BQuanLyID);
            updateColumnIDsCTKHTH("ID");
            updateColumns();
            updateColumnsParent1();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }

        protected virtual void updateColumnsParent1()
        {
            //Xác định hàng là hàng cha, con
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                var isParent = Convert.ToString(r[ColumnNameParentId]) == "" || Convert.ToBoolean(r[ColumnNameIsParent]);
                arrLaHangCha.Add(isParent);
            }
        }

        /// <summary>
        /// Tính tổng cho các hàng cha tại các cột là number
        /// </summary>
        protected void updateSummaryRows1(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan)
        {
            var columns = Columns.Where(x => x.DataType == 1).ToList();
            NH_KHTongTheTTCP_NVCViewModel itemQuery = _qlnhService.GetDetailKeHoachTongTheTTCP(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan);
            //VDT_KHV_KeHoach5Nam_DeXuat itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(!string.IsNullOrEmpty(iID_KeHoach5NamID) ? Guid.Parse(iID_KeHoach5NamID) : Guid.Empty);
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
                    if (dtChiTiet.Rows[i][columns[k].ColumnName] == DBNull.Value || Convert.ToDouble(dtChiTiet.Rows[i][columns[k].ColumnName]) != S && S != 0)
                    {
                        dtChiTiet.Rows[i][columns[k].ColumnName] = S;
                    }
                }
            }

            //check neu la chung tu tong hop dieu chinh thi lay fGiaTriNamThuNhat = fGiaTriNamThuNhatDc
            //if (dtChiTiet.Rows.Count > 0)
            //{
            //    var iID_KeHoach5Nam_DeXuatID = Convert.ToString(dtChiTiet.Rows[0]["iID_KeHoach5Nam_DeXuatID"]);
            //    if (iID_KeHoach5Nam_DeXuatID != null && iID_KeHoach5Nam_DeXuatID != string.Empty)
            //    {
            //        var itemTH = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(Guid.Parse(dtChiTiet.Rows[0]["iID_KeHoach5Nam_DeXuatID"].ToString()));
            //        if (itemTH.sTongHop != null)
            //        {
            //            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            //            {
            //                if (Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuNhatDc"]) > 0 || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuHaiDc"]) > 0
            //                    || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuBaDc"]) > 0 || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuTuDc"]) > 0
            //                    || Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTriNamThuNamDc"]) > 0)
            //                {
            //                    dtChiTiet.Rows[i]["fGiaTriNamThuNhat"] = dtChiTiet.Rows[i]["fGiaTriNamThuNhatDc"];
            //                    dtChiTiet.Rows[i]["fGiaTriNamThuHai"] = dtChiTiet.Rows[i]["fGiaTriNamThuHaiDc"];
            //                    dtChiTiet.Rows[i]["fGiaTriNamThuBa"] = dtChiTiet.Rows[i]["fGiaTriNamThuBaDc"];
            //                    dtChiTiet.Rows[i]["fGiaTriNamThuTu"] = dtChiTiet.Rows[i]["fGiaTriNamThuTuDc"];
            //                    dtChiTiet.Rows[i]["fGiaTriNamThuNam"] = dtChiTiet.Rows[i]["fGiaTriNamThuNamDc"];
            //                    dtChiTiet.Rows[i]["fGiaTriBoTri"] = dtChiTiet.Rows[i]["fGiaTriBoTriDc"];
            //                }
            //            }
            //        }
            //    }
            //}

            //if (itemQuery != null && !itemQuery.iLoai.Equals(2))
            //{
            //    //Sau khi tinh tong cha thi luu lai vao DB
            //    IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> newData = ConvertToModels(dtChiTiet);
            //    _iQLVonDauTuService.KH_TTCP_NVC_Save(newData);
            //    //update fGiaTriKeHoach
            //    _iQLVonDauTuService.UpdateGiaTriKeHoach(iID_KeHoach5NamID);
            //}
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
                    var id = fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    if (numRow == 1 && (id == "" || id == null))
                    {
                        id = Guid.Empty.ToString();
                    }
                    _arrDSMaHang.Add(id);
                });
        }
        //end test

        //private IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> ConvertToModels(DataTable dataTable)
        //{
        //    var itemQuery = _iQLVonDauTuService.GetKeHoach5NamDeXuatById(_idKhtt);
        //    if (itemQuery != null && itemQuery.iID_ParentId != null && itemQuery.iID_ParentId.HasValue)
        //    {
        //        return dataTable.AsEnumerable().Select(row => new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
        //        {
        //            iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString()),
        //            fHanMucDauTu = Convert.ToDouble(row["fHanMucDauTu"]),
        //            fGiaTriNamThuNhat = Convert.ToDouble(row["fGiaTriNamThuNhatDc"]),
        //            fGiaTriNamThuHai = Convert.ToDouble(row["fGiaTriNamThuHaiDc"]),
        //            fGiaTriNamThuBa = Convert.ToDouble(row["fGiaTriNamThuBaDc"]),
        //            fGiaTriNamThuTu = Convert.ToDouble(row["fGiaTriNamThuTuDc"]),
        //            fGiaTriNamThuNam = Convert.ToDouble(row["fGiaTriNamThuNamDc"]),
        //            fGiaTriBoTri = Convert.ToDouble(row["fGiaTriBoTriDc"])
        //        });
        //    }
        //    else
        //    {
        //        return dataTable.AsEnumerable().Select(row => new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
        //        {
        //            iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(row["iID_KeHoach5Nam_DeXuat_ChiTietID"].ToString()),
        //            fHanMucDauTu = Convert.ToDouble(row["fHanMucDauTu"]),
        //            fGiaTriNamThuNhat = Convert.ToDouble(row["fGiaTriNamThuNhat"]),
        //            fGiaTriNamThuHai = Convert.ToDouble(row["fGiaTriNamThuHai"]),
        //            fGiaTriNamThuBa = Convert.ToDouble(row["fGiaTriNamThuBa"]),
        //            fGiaTriNamThuTu = Convert.ToDouble(row["fGiaTriNamThuTu"]),
        //            fGiaTriNamThuNam = Convert.ToDouble(row["fGiaTriNamThuNam"]),
        //            fGiaTriBoTri = Convert.ToDouble(row["fGiaTriBoTri"])
        //        });
        //    }
        //}

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
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "sMaThuTu", header: "STT", columnWidth: 100, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTenNhiemVuChi", header: "Tên Group - dự án", columnWidth:743, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTenPhongBan", header: "B Quản lý", columnWidth:500, align: "left", hasSearch: true, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTri", header: "Giá trị phê duyệt (USD)", columnWidth:200, align: "right", dataType: 1, hasSearch: false, isReadonly: false),
                    
                    // cot khac
                    new SheetColumn(columnName: "ID", isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                    new SheetColumn(columnName: "bIsHasChild", isHidden: true),
                    new SheetColumn(columnName: "iID_BQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "isAdd", isHidden: true),
                    new SheetColumn(columnName: "state", isHidden: true, hasSearch: true),
                    new SheetColumn(columnName: "KHTTCP_ID", isHidden: true, hasSearch: true),
                };

            #endregion

            return list;
        }
    }
}