using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class DMNguon_SheetTable : SheetTable
    {
        private readonly IDanhMucService _qLNguonNSService = DanhMucService.Default;
        public DMNguon_SheetTable()
        {

        }

        public DMNguon_SheetTable(int iNamLamViec, Dictionary<string, string> query)
        {
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iNamLamViec, filters);
        }

        #region private methods

        private void fillSheet(int iNamLamViec, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = _qLNguonNSService.GetAllDMNguonByNamLamViec(iNamLamViec, _filters);
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "iID_Nguon";
            ColumnNameParentId = "";
            ColumnNameIsParent = "";
            #endregion

            fillData();
        }

        private void fillData()
        {
            updateColumnIDs("iID_Nguon");
            updateColumns();
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
        #endregion

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "iSTT", header:"STT", columnWidth: 4, align: "center", isReadonly: false),
                    new SheetColumn(columnName: "sMaCTMT", header: "Mã CTMT", columnWidth:10, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sLoai", header: "Loại", columnWidth:10, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sKhoan", header: "Khoản", columnWidth:10, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sNoiDung", header: "Mô tả", columnWidth:36, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sMaNguon", header: "Mã nguồn", columnWidth:10, align: "left", hasSearch: true),
                    new SheetColumn(columnName: "sMaNguonCha", header: "Nguồn cha", columnWidth:10, align: "left", hasSearch: true),
                    new SheetColumn(columnName: "sLoaiNganSach", header: "Loại ngân sách", columnWidth:10, align: "left", hasSearch: false, isReadonly: false, dataType: 3),

                    // cot khac
                    new SheetColumn(columnName: "iID_Nguon", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonCha", isHidden: true),
                    new SheetColumn(columnName: "numChild", isHidden: true),
                    new SheetColumn(columnName: "iLoaiNganSach", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),

                };

            #endregion

            return list;
        }
    }
}