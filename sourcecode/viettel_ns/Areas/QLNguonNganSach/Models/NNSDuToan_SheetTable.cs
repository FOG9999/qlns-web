using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToan_SheetTable : SheetTable
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private NNS_DuToan _entity;

        public NNSDuToan_SheetTable()
        {

        }

        public NNSDuToan_SheetTable(string id, Dictionary<string, string> query, List<NNSDuToanChiTietGetSoTienByDonViModel> aListXauNoiMa, List<NNSDuToanChiTietTongTienModel> aListTongTien, bool isSaoChep, string sUserName, string idNhiemVu = null)
        {
            var filters = new Dictionary<string, string>();

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });
            _entity = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(id));
            fillSheetNew(id, filters, aListXauNoiMa, aListTongTien, isSaoChep, sUserName, idNhiemVu);
        }

        public List<NNS_DuToanChiTietParentModel> ConvertDataTableToList(DataTable aDataTable)
        {
            var result = new List<NNS_DuToanChiTietParentModel>();

            for (int i = 0; i < aDataTable.Rows.Count; i++)
            {
                var row = dtChiTiet.Rows[i];
                var model = new NNS_DuToanChiTietParentModel();

                if (row["iID_DuToanChiTiet"] != null)
                {
                    model.iID_DuToanChiTiet = row["iID_DuToanChiTiet"].ToString();
                }
                if (row["sMaNoiDungChi"] != null)
                {
                    model.sMaNoiDungChi = row["sMaNoiDungChi"].ToString();
                }
                if (row["sTenNoiDungChi"] != null)
                {
                    model.sTenNoiDungChi = row["sTenNoiDungChi"].ToString();
                }
                if (row["iID_MaDonVi"] != null)
                {
                    model.iID_MaDonVi = row["iID_MaDonVi"].ToString();
                }
                if (row["TenDonVi"] != null)
                {
                    model.TenDonVi = row["TenDonVi"].ToString();
                }
                if (row["sMaPhongBan"] != null)
                {
                    model.sMaPhongBan = row["sMaPhongBan"].ToString();
                }
                if (row["sTenPhongBan"] != null)
                {
                    model.sTenPhongBan = row["sTenPhongBan"].ToString();
                }
                if (row["SoTien"] != null)
                {
                    model.SoTien = row["SoTien"].ToString();
                }
                if (row["GhiChu"] != null)
                {
                    model.GhiChu = row["GhiChu"].ToString();
                }
                if (row["bLaHangCha"] != null)
                {
                    model.bLaHangCha = Boolean.Parse(row["bLaHangCha"].ToString());
                }
                if (row["bConLai"] != null)
                {
                    model.bConLai = Boolean.Parse(row["bConLai"].ToString());
                }
                if (row["iThayDoi"] != null)
                {
                    model.iThayDoi = row["iThayDoi"].ToString();
                }
                if (row["IdCheck"] != null)
                {
                    model.IdCheck = row["IdCheck"].ToString();
                }
                if (row["iID_NhiemVu"] != null)
                {
                    model.iID_NhiemVu = row["iID_NhiemVu"].ToString();
                }
                if (row["iID_MaChungTu"] != null)
                {
                    model.iID_MaChungTu = row["iID_MaChungTu"].ToString();
                }

                result.Add(model);
            }

            return result.OrderBy(x => x.sMaNoiDungChi).ToList();
        }

        public List<NNS_DuToanChiTietParentModel> ConvertListToTree(List<NNS_DuToanChiTietParentModel> aListValue, Guid aId, int aLevel, string STT)
        {
            var result = new List<NNS_DuToanChiTietParentModel>();

            if (aListValue != null && aListValue.Any())
            {
                var listParent = aListValue.Where(x => x.IdNoiDungChiParent == aId).ToList();
                if (listParent != null && listParent.Any())
                {
                    var count = 1;
                    foreach (var item in listParent)
                    {
                        var model = new NNS_DuToanChiTietParentModel();
                        model.STT = $"{STT}.{count}";
                        model.Id = item.Id;
                        model.iID_DuToanChiTiet = item.iID_DuToanChiTiet;
                        model.sMaNoiDungChi = item.sMaNoiDungChi;
                        model.sTenNoiDungChi = item.sTenNoiDungChi;
                        model.iID_MaDonVi = item.iID_MaDonVi;
                        model.TenDonVi = item.TenDonVi;
                        model.sMaPhongBan = item.sMaPhongBan;
                        model.sTenPhongBan = item.sTenPhongBan;
                        model.SoTien = item.SoTien;
                        model.GhiChu = item.GhiChu;
                        model.sMauSac = item.sMauSac;
                        model.sFontBold = item.sFontBold;
                        model.sFontColor = item.sFontColor;
                        model.iThayDoi = item.iThayDoi;
                        model.Level = aLevel;
                        model.IdNoiDungChi = item.IdNoiDungChi;
                        model.IdNoiDungChiParent = item.IdNoiDungChiParent;
                        model.bLaHangCha = item.bLaHangCha;
                        model.IdCheck = item.IdCheck;
                        model.iID_NhiemVu = item.iID_NhiemVu;
                        model.iID_MaChungTu = item.iID_MaChungTu;
                        var listChild = ConvertListToTree(aListValue, model.IdNoiDungChi, model.Level + 1, model.STT);
                        if (listChild != null && listChild.Any())
                        {
                            model.ListChild = listChild;
                        }

                        result.Add(model);

                        if (model.Level == 2)
                        {
                            var modelCheck = result.Where(x => x.sMaNoiDungChi == item.sMaNoiDungChi && x.Level == 2 && x.TenDonVi == "--Còn lại--").ToList();
                            if (modelCheck == null || !modelCheck.Any())
                            {
                                var modelConLai = new NNS_DuToanChiTietParentModel();
                                modelConLai.STT = $"{STT}.{count}";
                                modelConLai.sMaNoiDungChi = item.sMaNoiDungChi;
                                modelConLai.sTenNoiDungChi = item.sTenNoiDungChi;
                                modelConLai.TenDonVi = "--Còn lại--";
                                modelConLai.Level = aLevel;
                                modelConLai.bLaHangCha = true;
                                modelConLai.bConLai = true;
                                modelConLai.iID_NhiemVu = item.iID_NhiemVu;
                                modelConLai.iID_MaChungTu = item.iID_MaChungTu;
                                modelConLai.IdCheck = Guid.NewGuid().ToString();
                                result.Add(modelConLai);
                            }
                        }

                        count++;
                    }
                }
            }

            return result;
        }

        public List<NNS_DuToanChiTietParentModel> ConvertTreeToFlat(List<NNS_DuToanChiTietParentModel> aListValue)
        {
            var result = new List<NNS_DuToanChiTietParentModel>();

            if (aListValue != null && aListValue.Any())
            {
                foreach (var item in aListValue)
                {
                    result.Add(item);
                    if (item.ListChild != null && item.ListChild.Any())
                    {
                        var listChild = ConvertTreeToFlat(item.ListChild);
                        if (listChild != null && listChild.Any())
                        {
                            result.AddRange(listChild);
                        }
                    }
                }
            }

            return result;
        }

        #region private method
        private void fillData()
        {
            updateColumnIDs("iID_DuToanChiTiet,iID_NhiemVu");
            updateColumns();
            updateColumnsParentNNSDuToan();
            updateCellsEditable();

            updateCellsValue();
            updateChanges1();
        }

        private void fillSheetNew(string id, Dictionary<string, string> filters, List<NNSDuToanChiTietGetSoTienByDonViModel> aListXauNoiMa, List<NNSDuToanChiTietTongTienModel> aListTongTien, bool isSaoChep, string sUserName, string iID_NhiemVu = null)
        {

            _filters = filters ?? new Dictionary<string, string>();
            var listDataTable = _qLNguonNSService.GetListChiTietDuToanNew(_entity.iID_DuToan.ToString(), sUserName, isSaoChep, aListXauNoiMa, iID_NhiemVu, filters);
            if (!string.IsNullOrEmpty(iID_NhiemVu))
            {
                if (listDataTable != null && listDataTable.Any())
                {
                    foreach (var item in listDataTable)
                    {
                        item.iID_NhiemVu = iID_NhiemVu;
                    }
                }
            }

            dtChiTiet = listDataTable.ToDataTable();
            //dtChiTiet.Columns.Add("iThayDoi");
            dtChiTiet.Columns.Add("STT");
            dtChiTiet.Columns.Add("STTNumber");
            dtChiTiet.Columns.Add("IdCheck");
            dtChiTiet.Columns.Add("IdsChild");
            dtChiTiet_Cu = dtChiTiet.Copy();
            ColumnNameIsParent = "bLaHangCha";

            #region them row conlai
            var dtRowCount = dtChiTiet.Rows.Count;
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var row = dtChiTiet.Rows[i];
                var sMaNoiDungChi = row["sMaNoiDungChi"];
                if (row.Field<bool>(ColumnNameIsParent))
                {
                    if (aListTongTien != null && aListTongTien.Any())
                    {
                        var tongTien = aListTongTien.FirstOrDefault(x => x.sMaNoiDungChi == row["sMaNoiDungChi"].ToString());
                        if (tongTien != null)
                        {
                            row["SoTien"] = tongTien.SoTien;
                        }
                    }
                }

                if (row.Field<bool>("bConLai"))
                {
                    // add row conlai
                    DataRow dr = dtChiTiet.NewRow();
                    dr["sMaNoiDungChi"] = row["sMaNoiDungChi"];
                    dr["sTenNoiDungChi"] = row["sTenNoiDungChi"];
                    //modelConLai.Level = 1;
                    dr["bLaHangCha"] = true;
                    dr["bConLai"] = false;
                    dr["TenDonVi"] = "--Còn lại--";
                    dr["iID_NhiemVu"] = row["iID_NhiemVu"];
                    dr["iID_MaChungTu"] = row["iID_MaChungTu"];
                    dr["IdCheck"] = Guid.NewGuid().ToString();
                    dr["depth"] = row["depth"];
                    dr["rootParent"] = row["rootParent"];
                    dr["location"] = row["location"];
                    dr["iID_NoiDungChiCha"] = row["iID_NoiDungChi"];
                    dr["iThayDoi"] = 0;
                    dtChiTiet.Rows.InsertAt(dr, i + 1);
                    i++;
                }
            }

            var dtView = dtChiTiet.DefaultView;
            //dtView.Sort = "location, depth";
            dtChiTiet = dtView.ToTable();

            #endregion
            fillData();
        }

        /// <summary>
        /// Hàm cập nhập mảng thay đổi dữ liệu
        /// </summary>
        protected void updateChanges1()
        {
            _arrThayDoi = new List<List<bool>>();
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var row = dtChiTiet.Rows[i];
                var items = new List<bool>();
                for (int j = 0; j < arrDSMaCot.Count; j++)
                {
                    if (row["iThayDoi"].ToString() == "True")
                    {
                        items.Add(true);
                    }
                    else
                    {
                        items.Add(false);
                    }
                }

                _arrThayDoi.Add(items);
            }
        }

        protected virtual void updateColumnsParentNNSDuToan()
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
                    if (Convert.ToString(r["sMaNoiDungChi"]) == Convert.ToString(dtChiTiet.Rows[j]["sMaNoiDungChi"])
                        && string.IsNullOrWhiteSpace(Convert.ToString(dtChiTiet.Rows[j]["iID_DuToanChiTiet"])))
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
                    new SheetColumn(columnName: "sTenNoiDungChi", header: "Nội dung chi", columnWidth:400, align: "left", hasSearch: true, isReadonly: false, isFixed: true),
                    new SheetColumn(columnName: "TenDonVi", header: "Đơn vị", columnWidth:250, align: "left", hasSearch: true,  isReadonly: false, dataType: 3),
                    new SheetColumn(columnName: "sTenPhongBan", header: "Phòng ban", columnWidth:300, align: "left", hasSearch: true, isReadonly: false, dataType: 3),
                    new SheetColumn(columnName: "SoTien", header: "Số tiền", columnWidth:200, dataType: 1, isReadonly: false),
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:680, isReadonly: false),

                    // cot khac
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "sMaNoiDungChi", isHidden: true, isReadonly: false),
                    new SheetColumn(columnName: "sMaPhongBan", isHidden: true, isReadonly: false),
                    new SheetColumn(columnName: "iID_MaDonVi", isHidden: true, isReadonly: false),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName: "iThayDoi",isHidden: true),
                    new SheetColumn(columnName: "STTNumber",isHidden: true),
                    new SheetColumn(columnName: "STT",isHidden: true),
                    new SheetColumn(columnName:"IdCheck",isHidden:true),
                    new SheetColumn(columnName:"IdsChild",isHidden:true),
                    new SheetColumn(columnName:"iID_NhiemVu",isHidden:true,isReadonly: false),
                    new SheetColumn(columnName:"iID_MaChungTu",isHidden:true,isReadonly: false),
                    new SheetColumn(columnName:"bTongTien",isHidden:true,isReadonly:true),
                    new SheetColumn(columnName:"iID_DuToanChiTiet",isHidden:true,isReadonly:true),
                    new SheetColumn(columnName: "bLaHangCha", isHidden:true, isReadonly:true),
                    new SheetColumn(columnName: "depth", isHidden:true, isReadonly:true),
                    new SheetColumn(columnName: "rootParent", isHidden:true, isReadonly:true),
                    new SheetColumn(columnName: "iID_NoiDungChiCha", isHidden:true, isReadonly:true),
                    new SheetColumn(columnName: "iID_NoiDungChi", isHidden:true, isReadonly:true)


                    //new SheetColumn(columnName: "bLaHangCha",isHidden:true),
                    //   new SheetColumn(columnName: "bConLai",isHidden:true),
                };

            #endregion

            return list;
        }
    }
}