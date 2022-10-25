using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class MappingNdcMLNSChiTiet_SheetTable : SheetTable
    {
        private readonly IDanhMucService _qLNguonNSService = DanhMucService.Default;
        public MappingNdcMLNSChiTiet_SheetTable()
        {

        }

        public MappingNdcMLNSChiTiet_SheetTable(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> query)
        {
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iID_NoiDungChi, iNamLamViec, filters);
        }

        #region private methods

        private void fillSheet(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = _qLNguonNSService.GetAllMLNSChiTietMapping(iID_NoiDungChi, iNamLamViec, _filters);
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "iID_MaMucLucNganSach";
            ColumnNameParentId = "iID_MaMucLucNganSach_Cha";
            ColumnNameIsParent = "bLaHangCha";
            #endregion

            ParentRowEditable = true;
            fillData();
        }

        private void fillData()
        {
            updateColumnIDs("iID_MaMucLucNganSach,IdMap");
            updateColumns();
            updateColumnsParent();
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
                    new SheetColumn(columnName: "sLNS", header: "LNS", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sL", header: "L", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sK", header: "K", columnWidth:100, align: "center", hasSearch: true,  isReadonly: true),
                    new SheetColumn(columnName: "sM", header: "M", columnWidth:100, align: "center", hasSearch: true,  isReadonly: true),
                    new SheetColumn(columnName: "sTM", header: "TM", columnWidth:100, align: "center", hasSearch: true,  isReadonly: true),
                    new SheetColumn(columnName: "sTTM", header: "TTM", columnWidth:100, align: "center", hasSearch: true,  isReadonly: true),
                    new SheetColumn(columnName: "sNG", header: "NG", columnWidth:100, align: "center", hasSearch: true,  isReadonly: true),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:720, isReadonly: true),

                    // cot nhap
                    new SheetColumn(columnName: "isMap", header: "Chọn/Bỏ chọn", columnWidth:120, align: "center", hasSearch: true, isReadonly: false, dataType: 2),

                    // cot khac
                    new SheetColumn(columnName: "iID_MaMucLucNganSach", isHidden: true),
                    new SheetColumn(columnName: "iID_MaMucLucNganSach_Cha", isHidden: true),
                    new SheetColumn(columnName: "IdMap", isHidden: true),
                    new SheetColumn(columnName: "sXauNoiMa", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }
    }
}