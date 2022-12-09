using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoach5NamDuocDuyet_ChiTiet_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;
        private Guid _idKeHoach5Nam = Guid.Empty;

        public int iGiaiDoanTu { get; set; }
        public KeHoach5NamDuocDuyet_ChiTiet_SheetTable()
        {

        }

        public KeHoach5NamDuocDuyet_ChiTiet_SheetTable(string iID_KeHoach5NamID, int iNamLamViec, int iGiaiDoanTu, Dictionary<string, string> query, List<KeHoach5NamChiTietDuocDuyetTempForSave> listKHVChiTiet, List<KeHoach5NamChiTietDuocDuyetTempForSave> currentShowingList)
        {
            this.iGiaiDoanTu = iGiaiDoanTu;
            this._idKeHoach5Nam = !string.IsNullOrEmpty(iID_KeHoach5NamID) ? Guid.Parse(iID_KeHoach5NamID) : Guid.Empty;
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iID_KeHoach5NamID, iNamLamViec, filters, listKHVChiTiet, currentShowingList);
        }


        #region private methods

        private void fillSheet(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> filters, List<KeHoach5NamChiTietDuocDuyetTempForSave> listKHVChiTiet, List<KeHoach5NamChiTietDuocDuyetTempForSave> currentShowingList)
        {

            _filters = filters ?? new Dictionary<string, string>();

            var dtChiTietSaved = _iQLVonDauTuService.GetListKH5NamDuocDuyetChiTietById(iID_KeHoach5NamID, iNamLamViec, _filters);
            if (listKHVChiTiet == null)
            {
                dtChiTiet = dtChiTietSaved;
            }
            else
            {
                // map các trường của model vào DataTable
                PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(KeHoach5NamChiTietDuocDuyetTempForSave));
                DataTable table = new DataTable();
                foreach (PropertyDescriptor prop in properties)
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                List<Guid> listKeyID = new List<Guid>();
                // list đang hiện nhưng chưa lưu                
                foreach (KeHoach5NamChiTietDuocDuyetTempForSave item in currentShowingList)
                {
                    listKeyID.Add(item.keyID);
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    table.Rows.Add(row);
                }
                // list mới chọn thêm
                foreach (KeHoach5NamChiTietDuocDuyetTempForSave item in listKHVChiTiet)
                {
                    Guid g = item.keyID;
                    if (!listKeyID.Contains(g))
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyDescriptor prop in properties)
                        {
                       
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                        table.Rows.Add(row);
                    }                        
                }
                // kết hợp với danh sách đã lưu
                List<VDT_KHV_KeHoach5Nam_ChiTiet> listSaved = _iQLVonDauTuService.GetListKH5NamDuocDuyetChiTietByIdInList(iID_KeHoach5NamID, iNamLamViec, _filters).ToList();
                List<string> noMappingProps = new List<string>();
                noMappingProps.Add("sTenLoaiCongTrinh");
                noMappingProps.Add("sTenDonViQL");
                noMappingProps.Add("sTenNganSach");
                noMappingProps.Add("sThoiGianThucHien");
                noMappingProps.Add("sDiaDiem");
                noMappingProps.Add("sDonVi");
                noMappingProps.Add("keyID");
                foreach (VDT_KHV_KeHoach5Nam_ChiTiet item in listSaved)
                {
                    DataRow row = table.NewRow();
                    KeHoach5NamChiTietDuocDuyetTempForSave tempSave = new KeHoach5NamChiTietDuocDuyetTempForSave();
                    foreach (PropertyDescriptor prop in properties)
                    {                        
                        if (!noMappingProps.Contains(prop.Name))
                        {
                            prop.SetValue(tempSave, prop.GetValue(item));
                        }
                        row[prop.Name] = prop.GetValue(tempSave) ?? DBNull.Value;
                    }
                    noMappingProps.ForEach(p =>
                    {
                        row[p] = tempSave.GetType().GetProperty(p).GetValue(tempSave, null);
                    });
                    row["sDiaDiem"] = tempSave.getDiaDiemByDuAnId();
                    row["sThoiGianThucHien"] = tempSave.getThoiGianThucHienByDuAnId();
                    table.Rows.Add(row);
                }

                dtChiTiet = table;
            }
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "iID_KeHoach5Nam_ChiTietID";
            ColumnNameParentId = "";
            ColumnNameIsParent = "";
            #endregion

            fillData(iID_KeHoach5NamID);
        }

        private void fillData(string iID_KeHoach5NamID)
        {
            updateColumnIDs("iID_KeHoach5Nam_ChiTietID");
            updateColumns();
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
                arrLaHangCha.Add(Convert.ToBoolean(r[ColumnNameIsParent]));


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
                        //if (string.IsNullOrEmpty(chungTu["iID_MaDonVi"].ToString()) && string.IsNullOrEmpty(Filters["iID_MaDonVi"]) && chungTu["iID_MaTinhChatCapThu"].ToString() != "1")
                        //{
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
            var cVonDaGiao = "Vốn đã giao năm " + (this.iGiaiDoanTu - 1);
            var cVonBoTri = "Vốn bố trí năm " + this.iGiaiDoanTu + " - " + (this.iGiaiDoanTu + 4);
            var cVonBoTriSauNam = "Vốn bố trí sau năm " + (this.iGiaiDoanTu + 4);

            var cVonDaGiaoDc = "Vốn đã giao năm " + (this.iGiaiDoanTu - 1) + "(Sau điều chỉnh)";
            var cVonBoTriDc = "Vốn bố trí năm " + this.iGiaiDoanTu + " - " + (this.iGiaiDoanTu + 4) + "(Sau điều chỉnh)";
            var cVonBoTriSauNamDc = "Vốn bố trí sau năm " + (this.iGiaiDoanTu + 4) + "(Sau điều chỉnh)";

            var list = new List<SheetColumn>();

            bool isReadOnlyDd = false;
            bool isHiddenDd = true;
            int indexGroupHeader = 5;
            var itemKhthDd = _iQLVonDauTuService.GetKeHoach5NamDuocDuyetById(_idKeHoach5Nam);
            if(itemKhthDd != null && itemKhthDd.iID_ParentId.HasValue)
            {
                isReadOnlyDd = true;
                indexGroupHeader = 8;
                isHiddenDd = false;
            }

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:150, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sTen", header: "Tên dự án", columnWidth:150, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    //new SheetColumn(columnName: "sTenDonViQL", header: "Đơn vị thực hiện dự án", columnWidth:150, align: "left", isFixed: true, hasSearch: true, dataType: 3, isReadonly: true),
                    new SheetColumn(columnName: "sDonVi", header: "Đơn vị", columnWidth:150, align: "left", isFixed: true, hasSearch: true, dataType: 3, isReadonly: true),
                    new SheetColumn(columnName: "sDiaDiem", header: "Địa điểm thực hiện", columnWidth:150, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sThoiGianThucHien", header: "Thời gian thực hiện", columnWidth:150, align: "center", isFixed: true, hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTenNganSach", header: "Nguồn vốn", columnWidth:150, align: "left", isFixed: true, hasSearch: false, isReadonly: true),

                    new SheetColumn(columnName: "fHanMucDauTu", header: "Hạn mức đầu tư", headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: indexGroupHeader, columnWidth:150, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fTongNhuCauNSQP", header: "Tổng nhu cầu NSQP", headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fVonDaGiao", header: cVonDaGiao, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: isReadOnlyDd),
                    new SheetColumn(columnName: "fVonDaGiaoDc", header: cVonDaGiaoDc, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: false, isHidden: isHiddenDd),
                    new SheetColumn(columnName: "fVonBoTriTuNamDenNam", header: cVonBoTri, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: isReadOnlyDd),
                    new SheetColumn(columnName: "fVonBoTriTuNamDenNamDc", header: cVonBoTriDc, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: false, isHidden: isHiddenDd),
                    new SheetColumn(columnName: "fGiaTriBoTri", header: cVonBoTriSauNam, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriBoTriDc", header: cVonBoTriSauNamDc, headerGroup: "KẾ HOẠCH BỘ GIAO", headerGroupIndex: 8, columnWidth:150, dataType: 1, isReadonly: true, isHidden: isHiddenDd),

                    new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:250, align: "left", hasSearch: false, isReadonly: false),
                    
                    // cot khac
                    new SheetColumn(columnName: "iID_KeHoach5Nam_ChiTietID", isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoach5NamID", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID",isHidden: true),
                    new SheetColumn(columnName: "iID_DonViID", isHidden: true),
                    new SheetColumn(columnName: "keyID", isHidden: true),
                };

            #endregion

            list = list.OrderByDescending(item => !item.IsHidden).ToList();

            return list;
        }
    }
}