using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class MappingNdcMLNS_SheetTable : SheetTable
    {
        //private readonly IQLDotNhanBTC _dmService = QLDotNhanService.Default;
        private readonly IDanhMucService _qLNguonNSService = DanhMucService.Default;
        public MappingNdcMLNS_SheetTable()
        {

        }

        public MappingNdcMLNS_SheetTable(int iNamLamViec, Dictionary<string, string> query)
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

            dtChiTiet = _qLNguonNSService.GetAllDMNoiDungChiMappingMLNS(iNamLamViec, _filters);

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
            updateColumnIDs("iID_NoiDungChi");
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

        #endregion

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sMaNoiDungChi", header: "Mã nội dung chi", columnWidth:200, align: "left", hasSearch: true),
                    new SheetColumn(columnName: "sTenNoiDungChi", header: "Tên nội dung chi", columnWidth:460, hasSearch: true),
                    new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:700, hasSearch: true),
                    new SheetColumn(columnName: "sMap", header: "Đã/Chưa map", columnWidth:178, align: "center", isReadonly: true,hasSearch: true),

                    // cot khac
                    new SheetColumn(columnName: "iID_NoiDungChi", isHidden: true),
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