using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLPhanNguon_NDChi_SheetTable : SheetTable
    {
        //private readonly IQLDotNhanBTC _dmService = QLDotNhanService.Default;
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        public QLPhanNguon_NDChi_SheetTable()
        {

        }

        public QLPhanNguon_NDChi_SheetTable(Guid? iIdNguon, Guid iIdPhanNguon, decimal rSoTienConLai, string username, Dictionary<string, string> query)
        {
            var filters = new Dictionary<string, string>();

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            //filters.Add("iIdPhanNguon", iIdPhanNguon.ToString());
            //_entity = _capPhatService.GetChungTu(Guid.Parse(id));
            fillSheet(iIdNguon, iIdPhanNguon, rSoTienConLai, username, filters);
        }

        #region private methods

        private void fillSheet(Guid? iIdNguon, Guid iIdPhanNguon, decimal rSoTienConLai, string username, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            //dtChiTiet = getDataChiTiet(_entity).Copy();
            dtChiTiet = _qLNguonNSService.GetAllDMNoiDungChiBQP2(iIdNguon, iIdPhanNguon, rSoTienConLai, username, _filters);
            //dtChiTiet = _dmService.GetListDotNhanNguonChiTietDatatable(id);
            //dtChiTiet = getDataChiTiet2(_entity, option).Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();

            //if (Filters.ContainsKey("iID_MaDonVi") && Filters["iID_MaDonVi"].IsNotEmpty())
            //{
            //    iID_MaDonVi = Filters["iID_MaDonVi"];
            //}
            //var donvis = iID_MaDonVi.ToList().Select(x => new NS_DonVi
            //{
            //    iID_MaDonVi = x,
            //    sMoTa = _nganSachService.GetDonVi(_entity.iNamLamViec.ToString(), x).sMoTa,
            //}).ToList();
            //var rows_new = new List<DataRow>();

            #region Lay XauNoiMa_Cha


            ColumnNameId = "iID_NoiDungChi";
            ColumnNameParentId = "iID_Cha";
            ColumnNameIsParent = "bLaHangCha";


            //for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            //{
            //    var row = dtChiTiet.Rows[i];
            //    bool isParent = true;
            //    if(row.Field<Guid?>(ColumnNameParentId) != null)
            //    {
            //        // lan nguoc len de lay hang cha gan nhat
            //        for (int j = i - 1; j >= 0; j--)
            //        {
            //            var row_hangcha = dtChiTiet.Rows[j];
            //            if (row_hangcha.Field<bool>(ColumnNameIsParent) &&
            //                row.Field<Guid>(ColumnNameParentId).ToString() == row_hangcha.Field<Guid>(ColumnNameId).ToString())
            //            {
            //                isParent = false;
            //                break;
            //            }
            //        }
            //    }

                
            //    row[ColumnNameIsParent] = isParent;
               
            //}

            #endregion

            //ColumnNameId = "sXauNoiMa";
            //ColumnNameParentId = "sXauNoiMa_Cha";
            //ColumnNameIsParent = "bLaHangCha";

            //fillterData(dtChiTiet, option);
            fillData();
        }

        private void fillData()
        {
            //updateSummaryRows1();
            //insertSummaryRows("sMoTa");
            //DeleteRowsZero();
            updateColumnIDs("iID_NoiDungChi");
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

            list = new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sTenNoiDungChi", header: "Nội dung chi", columnWidth:1000, isFixed: true, hasSearch: true),
                    //new SheetColumn(columnName: "sLoai", header: "Loại", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    //new SheetColumn(columnName: "sKhoan", header: "Khoản", columnWidth:40, isFixed: true, align: "center", hasSearch: true),
                    //new SheetColumn(columnName: "sNoiDung", header: "Nội dung", columnWidth:240, isFixed: true, hasSearch: true),
                    //new SheetColumn(columnName: "sXauNoiMa", header: "sXauNoiMa", columnWidth:160, isFixed: true),
                    //new SheetColumn(columnName: "sXauNoiMa_Cha", header: "sXauNoiMa_Cha", columnWidth:160, isFixed: true),

                    new SheetColumn(columnName: "SoTien", header: "Số tiền", columnWidth:260, dataType: 1,isReadonly: false),
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:563,isReadonly: false),


                    // cot khac
                    new SheetColumn(columnName: "iID_NoiDungChi", isHidden: true),
                    new SheetColumn(columnName: "iID_PhanNguon_NDChi", isHidden: true),
                    new SheetColumn(columnName: "iID_Nguon", isHidden: true),
                    new SheetColumn(columnName: "iID_PhanNguon", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            #endregion

            return list;
        }
    }
}