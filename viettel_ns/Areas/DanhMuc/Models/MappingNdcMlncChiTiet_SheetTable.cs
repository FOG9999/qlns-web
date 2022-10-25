using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class MappingNdcMlncChiTiet_SheetTable : SheetTable
    {
        private readonly IDanhMucService _qLNguonNSService = DanhMucService.Default;
        public MappingNdcMlncChiTiet_SheetTable()
        {

        }

        public MappingNdcMlncChiTiet_SheetTable(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> query)
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
            dtChiTiet = _qLNguonNSService.GetAllMLNCChiTietMapping(iID_NoiDungChi, iNamLamViec, _filters);

            foreach (DataRow item in dtChiTiet.Rows)
            {
                if (item["KyHieuCha"].ToString() == "NULL")
                    item["KyHieuCha"] = "";
            }

            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "Id";
            ColumnNameParentId = "Id_Parent";
            ColumnNameIsParent = "IsParent";
            #endregion

            ParentRowEditable = true;
            fillData();
        }

        private void fillData()
        {
            updateColumnIDs("Id,IdMap");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();

            //updateColumnIDs("iID_NoiDungChi");
            //updateColumns();
            //updateColumnsParent1();
            //updateCellsEditable();
            //updateCellsValue();
            //updateChanges();
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
                    // cot nhap
                    new SheetColumn(columnName: "Loai", header: "Loại nhập", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Nganh_Parent", header: "Ngành quản lý", columnWidth:120, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Nganh", header: "Mã ngành", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "M", header: "Mã mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "KyHieu", header: "Mã mục lục", columnWidth:120, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "STT", header: "Ký hiệu", columnWidth:120, align: "center", isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:600, isReadonly: true),
                    new SheetColumn(columnName: "KyHieuCha", header: "Mã mục lục cha", columnWidth:90, hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "STTBC", header: "STT in báo cáo", columnWidth:70, align: "left", isReadonly: true),
                    new SheetColumn(columnName: "isMap", header: "Chọn/Bỏ chọn", columnWidth:120, hasSearch: true, align: "center", isReadonly: false, dataType: 2),
                    //new SheetColumn(columnName: "IsChecked", header: "Chọn/Bỏ chọn", columnWidth:120, align: "left", isReadonly: false, dataType: 2),

                    // cot khac
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "Id_Parent", isHidden: true),
                    new SheetColumn(columnName: "IdMap", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }
    }
}