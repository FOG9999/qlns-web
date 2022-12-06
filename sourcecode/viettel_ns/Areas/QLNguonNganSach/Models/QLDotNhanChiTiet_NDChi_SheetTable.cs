using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLDotNhanChiTiet_NDChi_SheetTable : SheetTable
    {
        //private readonly IQLDotNhanBTC _dmService = QLDotNhanService.Default;
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        public QLDotNhanChiTiet_NDChi_SheetTable()
        {

        }
        public string sMaLoaiDuToan { get; set; }

        public QLDotNhanChiTiet_NDChi_SheetTable(Guid? iID_DotNhanChiTiet, string sMaLoaiDuToan, string username, Dictionary<string, string> query)
        {
            this.sMaLoaiDuToan = sMaLoaiDuToan;
            var filters = new Dictionary<string, string>();

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iID_DotNhanChiTiet, sMaLoaiDuToan, username, filters);
        }

        #region private methods

        private void fillSheet(Guid? iID_DotNhanChiTiet, string sMaLoaiDuToan, string username, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = _qLNguonNSService.GetAllDMNoiDungChiBQPTheoDotNhan(iID_DotNhanChiTiet, sMaLoaiDuToan, username, _filters);
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha

            ColumnNameId = "iID_NoiDungChi";
            ColumnNameParentId = "iID_Parent";
            ColumnNameIsParent = "bLaHangCha";

            #endregion

            fillData();
        }

        private void fillData()
        {
            updateSummaryRows1();
            updateColumnIDs("iID_NoiDungChi,iID_DotNhanChiTiet");
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

                        //}
                        //else
                        //{
                        //    if (iID_MaMucLucNganSach == Convert.ToString(dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]))
                        //    {
                        //        S += Convert.ToDouble(dtChiTiet.Rows[j][columns[k].ColumnName]);
                        //        S1 += S;
                        //    }
                        //}
                    }
                    if (dtChiTiet.Rows[i][columns[k].ColumnName] == DBNull.Value || (Convert.ToDouble(dtChiTiet.Rows[i][columns[k].ColumnName]) != S) && S != 0)
                    {
                        dtChiTiet.Rows[i][columns[k].ColumnName] = S;
                    }
                    //if (S1 != 0 && Convert.ToString(dtChiTiet.Rows[i]["iID_MaDonVi"]) == "")
                    //{
                    //    dtChiTiet.Rows[i][columns[k].ColumnName] = S1;
                    //    dtChiTiet.Rows[i]["bLaHangCha"] = true;
                    //}
                }
            }

        }

        #endregion

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns
            if (this.sMaLoaiDuToan == "001" || this.sMaLoaiDuToan == "002")
            {
                list = new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sTenNoiDungChi", header: "Nội dung chi", columnWidth:780, isFixed: true, hasSearch: true),
                    new SheetColumn(columnName: "soKiemTra", header: "Số kiểm tra đã giao cho đơn vị", columnWidth:200, isFixed: true, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "SoTien", header: "Số tiền", columnWidth:200, dataType: 1,isReadonly: false),
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:600,isReadonly: false),


                    // cot khac
                    new SheetColumn(columnName: "iID_NoiDungChi", isHidden: true),
                    new SheetColumn(columnName: "iID_DotNhanChiTiet", isHidden: true),
                    new SheetColumn(columnName: "iID_DotNhanChiTiet_NDChi", isHidden: true),
                    new SheetColumn(columnName: "sMaNoiDungChi", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

                return list;
            }

            list = new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sTenNoiDungChi", header: "Nội dung chi", columnWidth:780, isFixed: true, hasSearch: true),
                    //new SheetColumn(columnName: "soKiemTra", header: "Số kiểm tra", columnWidth:200, isFixed: true, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "SoTien", header: "Số tiền", columnWidth:200, dataType: 1,isReadonly: false),
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:600,isReadonly: false),


                    // cot khac
                    new SheetColumn(columnName: "iID_NoiDungChi", isHidden: true),
                    new SheetColumn(columnName: "iID_DotNhanChiTiet", isHidden: true),
                    new SheetColumn(columnName: "iID_DotNhanChiTiet_NDChi", isHidden: true),
                    new SheetColumn(columnName: "sMaNoiDungChi", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }
    }
}