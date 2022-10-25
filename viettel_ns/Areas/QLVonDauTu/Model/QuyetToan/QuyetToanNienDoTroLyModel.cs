using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;

namespace VIETTEL.Areas.QLVonDauTu.Model.QuyetToan
{
    public class QuyetToanNienDoTroLyModel
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;
        private readonly INganSachNewService _nsService = NganSachNewService.Default;

        public List<SelectListItem> GetDataThongTinNganhByLoaiNganSach(string sLNS, int iNamKeHoach)
        {
            IEnumerable<NS_MucLucNganSach> data = _nsService.GetDataThongTinChiTietLoaiNganSach(sLNS, iNamKeHoach);

            return (from lns in data
                    select new SelectListItem()
                    {
                        Text = string.Format("{0} - {1} - {2} - {3}", lns.sM, lns.sTM, lns.sTTM, lns.sNG),
                        Value = lns.iID_MaMucLucNganSach.ToString()
                    }).Distinct().ToList();
        }

        public List<SelectListItem> GetDataDropDownDuAn(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = _vdtService.GetDuAnInQuyetToanNienDo(iID_DonViQuanLyID, iID_NguonVonID, iID_LoaiNguonVonID, dNgayQuyetDinh, iNamKeHoach, iID_NganhID);
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem() { Text = Convert.ToString(dr["sTenDuAn"]), Value = Convert.ToString(dr["iID_DuAnID"]) + "|" + Convert.ToString(dr["TenPhanCap"]) });
            }
            return lstData;
        }

        public bool InsertNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data, List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstDataDetail, string userLogin)
        {
            data.dDateCreate = DateTime.Now;
            data.sUserCreate = userLogin;
            if (!_vdtService.InsertQuyetToanNienDoTroLy(ref data)) return false;
            lstDataDetail = lstDataDetail.Select(n => { n.iID_DeNghiQuyetToanNienDoID = data.iID_DeNghiQuyetToanNienDoID; return n; }).ToList();
            if (!_vdtService.InsertQuyetToanNienDoTroLyChiTiet(lstDataDetail))
            {
                _vdtService.RemoveQuyetToanNienDoTroLy(data.iID_DeNghiQuyetToanNienDoID);
                return false;
            }
            return true;
        }

        public bool InsertQuyetToanNienDoTroLyChiTiet(Guid iId, List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstDataDetail)
        {
            lstDataDetail = lstDataDetail.Select(n => { n.iID_DeNghiQuyetToanNienDoID = iId; return n; }).ToList();
            return !_vdtService.InsertQuyetToanNienDoTroLyChiTiet(lstDataDetail);
        }

        public bool UpdateQuyetToanNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data, string sUserName)
        {
            var dataUpdate = _vdtService.GetQuyetToanNienDoTroLyById(data.iID_DeNghiQuyetToanNienDoID);
            dataUpdate.sNguoiDeNghi = data.sNguoiDeNghi;
            dataUpdate.sSoDeNghi = data.sSoDeNghi;
            dataUpdate.dNgayDeNghi = data.dNgayDeNghi;
            dataUpdate.dDateUpdate = DateTime.Now;
            dataUpdate.sUserUpdate = sUserName;
            return _vdtService.UpdateQuyetToanNienDoTroLy(dataUpdate);
        }
    }
}