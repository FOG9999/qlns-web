using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class QLDotNhan_SheetTable : SheetTable
    {
        private readonly IQLNguonNganSachService _dmService = QLNguonNganSachService.Default;
        public QLDotNhan_SheetTable()
        {

        }

        public QLDotNhan_SheetTable(string id, int iNamLamViec, Dictionary<string, string> query)
        {
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            //_entity = _capPhatService.GetChungTu(Guid.Parse(id));
            fillSheet(id, iNamLamViec, filters);
        }

        #region private methods

        private void fillSheet(string id, int iNamLamViec, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            //dtChiTiet = getDataChiTiet(_entity).Copy();
            dtChiTiet = _dmService.GetListDotNhanNguonChiTietDatatable(id, iNamLamViec, filters);
            //dtChiTiet = getDataChiTiet2(_entity, option).Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha


            ColumnNameId = "iID_Nguon";
            ColumnNameParentId = "iID_NguonCha";
            ColumnNameIsParent = "bLaHangCha";

            #endregion

            fillData();
        }

        private void fillData()
        {
            updateSummaryRows1();
            //insertSummaryRows("sMoTa");
            //DeleteRowsZero();
            UpdateColumIDDotNhan("iID_DotNhanChiTiet,iID_DotNhan,iID_Nguon");
            //updateColumnIDs("iID_MaCapPhatChiTiet,sXauNoiMa,iID_MaDonVi");
            updateColumns();
            updateColumnsParent1();
            updateCellsEditable();

            //CapNhap_arrEdit();
            //CapNhap_arrDuLieu();
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


        protected virtual void UpdateColumIDDotNhan(string columns)
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
                    new SheetColumn(columnName: "sMaCTMT", header: "Mã CTMT", columnWidth:100,align: "center", isFixed: true, hasSearch: true),
                    new SheetColumn(columnName: "sLoai", header: "Loại", columnWidth:100, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sKhoan", header: "Khoản", columnWidth:100, isFixed: true, align: "center", hasSearch: true),
                    new SheetColumn(columnName: "sNoiDung", header: "Nội dung", columnWidth:350, isFixed: true, hasSearch: true),

                    new SheetColumn(columnName: "fLuyKeNguonNhan", header: "Lũy kế nguồn nhận từ BTC", headerGroup: "Nguồn nhận từ BTC", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "SoTien", header: "Đợt nhận này", headerGroup: "Nguồn nhận từ BTC", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: false),
                    new SheetColumn(columnName: "fTongNguonNhan", header: "Tổng", headerGroup: "Nguồn nhận từ BTC", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: true),

                    new SheetColumn(columnName: "fLuyKeDaPhanNDC", header: "Lũy kế đã phân theo ND chi", headerGroup: "Nguồn đã chi theo nội dung chi", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "SoTienDaPhanNDC", header: "Đợt nhận này", headerGroup: "Nguồn đã chi theo nội dung chi", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "fTongDaPhan", header: "Tổng", headerGroup: "Nguồn đã chi theo nội dung chi", headerGroupIndex: 3, columnWidth:170, dataType: 1,isReadonly: true),

                    new SheetColumn(columnName: "SoTienConLai", header: "Số tiền còn lại", columnWidth:170, dataType: 1,isReadonly: true),
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:440,isReadonly: false),


                    // cot khac
                    new SheetColumn(columnName: "iID_DotNhanChiTiet", isHidden: true),
                    new SheetColumn(columnName: "iID_DotNhan", isHidden: true),
                    new SheetColumn(columnName: "sMaNguon", isHidden: true),
                    new SheetColumn(columnName: "iID_Nguon", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonCha", isHidden: true),
                    new SheetColumn(columnName: "sMaLoaiDuToan", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }
    }
}