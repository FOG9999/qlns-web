using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class DuAn_KeHoach5NamDeXuat_ChiTiet_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        public int iGiaiDoanTu { get; set; }
        public DuAn_KeHoach5NamDeXuat_ChiTiet_SheetTable()
        {

        }

        public DuAn_KeHoach5NamDeXuat_ChiTiet_SheetTable(string Id, int iNamLamViec, int iGiaiDoanTu, Dictionary<string, string> query, DataTable currListCT)
        {
            this.iGiaiDoanTu = iGiaiDoanTu;
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(Id, iNamLamViec, filters, currListCT);
        }

        #region private methods

        private void fillSheet(string Id, int iNamLamViec, Dictionary<string, string> filters, DataTable currListCT)
        {
            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = _iQLVonDauTuService.GetListKH5NamDeXuatChiTietById(Id, iNamLamViec, _filters);
            if (currListCT != null)
            {
                // check xem bản ghi nào đang được chọn rồi thi set isMap = 1
                dtChiTiet.Columns.Add("isMap");
                foreach (DataRow dataRow in dtChiTiet.Rows)
                {
                    dataRow["isMap"] = false;
                }
                foreach (DataRow row in currListCT.Rows)
                {
                    foreach (DataRow dataRow in dtChiTiet.Rows)
                    {
                        if (dataRow["sTen"].ToString().ToLower() == row["sTen"].ToString().ToLower())
                        {                            
                            dataRow["isMap"] = true;
                        }
                    }
                }
            }
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "iID_KeHoach5Nam_DeXuat_ChiTietID";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bIsParent";
            #endregion

            fillData();
        }

        private void fillData()
        {
            updateSummaryRows1();
            //updateColumnIDs("Id");
            UpdateColumIDsKH5NamDXChiTiet("iID_KeHoach5Nam_DeXuat_ChiTietID");
            updateColumns();
            updateColumnsParent1();
            //updateColumnsParent();
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
        protected void updateSummaryRows1()
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
            var cNameNam1 = "Năm " + this.iGiaiDoanTu;
            var cNameNam2 = "Năm " + (this.iGiaiDoanTu + 1);
            var cNameNam3 = "Năm " + (this.iGiaiDoanTu + 2);
            var cNameNam4 = "Năm " + (this.iGiaiDoanTu + 3);
            var cNameNam5 = "Năm " + (this.iGiaiDoanTu + 4);

            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "isMap", header: "Chọn/Bỏ chọn", columnWidth:120, align: "center", isReadonly: false, dataType: 2),
                    new SheetColumn(columnName: "sSTT", header: "STT", columnWidth:50, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTen", header: "Tên Group - dự án", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sDonViThucHienDuAn", header: "Đơn vị thực hiện dự án", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sDiaDiem", header: "Địa điểm thực hiện", columnWidth:150, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "iGiaiDoanTu", header: "Thời gian từ", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "iGiaiDoanDen", header: "Thời gian đến", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sDuAnCha", header: "Group - Dự án - Chi tiết cha", columnWidth:200, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:200, align: "left", hasSearch: false, isReadonly: true),

                    new SheetColumn(columnName: "sTenNganSach", header: "Nguồn vốn", headerGroup: "HẠN MỨC ĐẦU TƯ ĐỀ NGHỊ", headerGroupIndex: 2, columnWidth:120, isReadonly: true),
                    new SheetColumn(columnName: "fHanMucDauTu", header: "Hạn mức đầu tư", headerGroup: "HẠN MỨC ĐẦU TƯ ĐỀ NGHỊ", headerGroupIndex: 2, columnWidth:120, dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fTongSoNhuCauNSQP", header: "Tổng số nhu cầu NSQP", columnWidth:170, align: "left", hasSearch: false, dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fTongSo", header: "Tổng số", headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuNhat", header: cNameNam1, headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuHai", header: cNameNam2, headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuBa", header: cNameNam3, headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuTu", header: cNameNam4, headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fGiaTriNamThuNam", header: cNameNam5, headerGroup: "NHU CẦU BỐ TRÍ VỐN NSQP 2021 - 2025", headerGroupIndex: 6, columnWidth:120, dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fGiaTriBoTri", header: "Vốn bố trí sau năm " + cNameNam5, columnWidth:170, align: "left", hasSearch: false, dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:250, align: "left", hasSearch: false, isReadonly: true),
                    
                    // cot khac
                    new SheetColumn(columnName: "iID_KeHoach5Nam_DeXuat_ChiTietID", isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoach5NamID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "iID_LoaiCongTrinhID", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonVonID", isHidden: true),
                    new SheetColumn(columnName: "numChild", isHidden: true),
                    new SheetColumn(columnName: "iIDReference", isHidden: true),
                    new SheetColumn(columnName: "iLevel", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),

                };

            #endregion

            return list;
        }
    }
}