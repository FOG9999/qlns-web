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
    public class DanhMucDuAn_KH5NDD_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _iQLVonDauTuService = QLVonDauTuService.Default;

        public int iGiaiDoanTu { get; set; }
        public DanhMucDuAn_KH5NDD_SheetTable()
        {

        }

        public DanhMucDuAn_KH5NDD_SheetTable(string iID_KeHoach5NamID, int iNamLamViec, int iGiaiDoanTu, Dictionary<string, string> query, DataTable currListCT)
        {
            this.iGiaiDoanTu = iGiaiDoanTu;
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iID_KeHoach5NamID, iNamLamViec, filters, currListCT);
        }

        #region private methods

        private void fillSheet(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> filters, DataTable currListCT)
        {

            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = _iQLVonDauTuService.GetListDMDuAnKH5NDD(iNamLamViec, _filters);

            if(currListCT != null)
            {
                // check xem bản ghi nào đang được chọn rồi thi set isMap = 1
                foreach (DataRow row in currListCT.Rows)
                {
                    foreach(DataRow dataRow in dtChiTiet.Rows)
                    {
                        if (checkIfItemIsChecked(row, dataRow))
                        {
                            dataRow["isMap"] = true;
                        }
                    }
                }
            }
            
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "IdRow";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bLaHangCha";
            #endregion

            fillData();
        }

        private bool checkIfItemIsChecked(DataRow dataRow, DataRow rowToCheck)
        {
            string sTenLoaiCongTrinh = rowToCheck["sTenLoaiCongTrinh"].ToString().Split('-').Last().ToLower().Trim();
            string sTenNganSachRowToCheck = rowToCheck["sTenNganSach"].ToString().Split('.').Last().ToLower().Trim();
            string sTenNganSachRowChecked = rowToCheck["sTenNganSach"].ToString().Split('.').Last().ToLower().Trim();
            if (
                dataRow["sTen"].ToString().ToLower() == rowToCheck["sTenDuAn"].ToString().ToLower()
                && dataRow["sTenLoaiCongTrinh"].ToString().ToLower() == sTenLoaiCongTrinh
                && sTenNganSachRowChecked == sTenNganSachRowToCheck
            )
            {
                return true;
            }
            return false;
        }

        private void fillData()
        {
            updateSummaryRows1();
            updateColumnIDs("IdRow");
            //UpdateColumIDsKH5NamDXChiTiet("Id");
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

        private IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> ConvertToModels(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
            {
                iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.Parse(row["Id"].ToString()),
                fHanMucDauTu = Convert.ToDouble(row["fHanMucDauTu"]),
                fGiaTriNamThuNhat = Convert.ToDouble(row["fGiaTriNamThuNhat"]),
                fGiaTriNamThuHai = Convert.ToDouble(row["fGiaTriNamThuHai"]),
                fGiaTriNamThuBa = Convert.ToDouble(row["fGiaTriNamThuBa"]),
                fGiaTriNamThuTu = Convert.ToDouble(row["fGiaTriNamThuTu"]),
                fGiaTriNamThuNam = Convert.ToDouble(row["fGiaTriNamThuNam"]),
                fGiaTriBoTri = Convert.ToDouble(row["fGiaTriBoTri"])
            });
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
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "sMaDuAn", header: "Mã dự án", columnWidth:120, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTenDuAn", header: "Tên dự án", columnWidth:250, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTenCDT", header: "Chủ đầu tư", columnWidth:150, align: "left", hasSearch: false, dataType: 3, isReadonly: false),
                    //new SheetColumn(columnName: "sTenDonViQL", header: "Đơn vị thực hiện dự án", columnWidth:150, align: "left", hasSearch: false, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "sTenDonVi", header: "Đơn vị", columnWidth:150, align: "left", hasSearch: false, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "sDiaDiem", header: "Địa điểm thực hiện", columnWidth:150, align: "left", hasSearch: false, isReadonly: false),

                    new SheetColumn(columnName: "sKhoiCong", header: "Thời gian thực hiện từ", headerGroup: "THỜI GIAN KC/KT", align: "center", headerGroupIndex: 2, columnWidth:170, isReadonly: false),
                    new SheetColumn(columnName: "sKetThuc", header: "Thời gian thực hiện đến", headerGroup: "THỜI GIAN KC/KT", align: "center", headerGroupIndex: 2, columnWidth:170, isReadonly: false),
                    new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:200, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "sTenNganSach", header: "Nguồn vốn", columnWidth:200, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "fHanMucDauTu", header: "Hạn mức đầu tư", columnWidth:150, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "isMap", header: "Chọn/Bỏ chọn", columnWidth:120, align: "center", isReadonly: false, dataType: 2),

                    // cot khac
                    new SheetColumn(columnName: "sId_CDT", isHidden: true),
                    new SheetColumn(columnName: "IdRow", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAnID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "iID_ChuDauTuID", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAnKHTHDeXuatID", isHidden: true),
                    new SheetColumn(columnName: "iID_LoaiCongTrinhID", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonVonID", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAn_HangMucID", isHidden: true),
                    new SheetColumn(columnName: "numChild", isHidden: true),
                    new SheetColumn(columnName: "iLevel", isHidden: true),
                    new SheetColumn(columnName: "bLaHangCha", isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),

                };

            #endregion

            return list;
        }
    }
}