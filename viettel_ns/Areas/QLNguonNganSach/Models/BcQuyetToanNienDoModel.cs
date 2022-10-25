using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Models;
using static Viettel.Extensions.Constants;
using static VIETTEL.Common.Constants;
using CoQuanThanhToan = VIETTEL.Common.Constants.CoQuanThanhToan;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class BcQuyetToanNienDoModel
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

        public List<SelectListItem> GetCoQuanThanhToan()
        {
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            lstCbx.Add(new SelectListItem() { Text = CoQuanThanhToan.TypeName.KHO_BAC, Value = ((int)CoQuanThanhToan.Type.KHO_BAC).ToString() });
            lstCbx.Add(new SelectListItem() { Text = CoQuanThanhToan.TypeName.CQTC, Value = ((int)CoQuanThanhToan.Type.CQTC).ToString() });
            return lstCbx;
        }

        public List<SelectListItem> GetLoaiThanhToan()
        {
            List<SelectListItem> lstCbx = new List<SelectListItem>();
            lstCbx.Add(new SelectListItem() { Text = LOAI_QUYET_TOAN_NIEN_DO.TypeName.THANH_TOAN, Value = ((int)LOAI_QUYET_TOAN_NIEN_DO.Type.THANH_TOAN).ToString() });
            lstCbx.Add(new SelectListItem() { Text = LOAI_QUYET_TOAN_NIEN_DO.TypeName.TAM_UNG, Value = ((int)LOAI_QUYET_TOAN_NIEN_DO.Type.TAM_UNG).ToString() });
            return lstCbx;
        }

        public List<VdtQtBcQuyetToanNienDoViewModel> GetPagingIndex(ref PagingInfo _paging, string iIdMaDonViQuanLy = null, int? iIdNguonVon = null, DateTime? dNgayDeNghiFrom = null, DateTime? dNgayDeNghiTo = null, int? iNamKeHoach = null, string txtSoChungTu = null)
        {
            var data = _vdtService.GetAllBaoCaoQuyetToanPaging(ref _paging, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, txtSoChungTu);
            if (data == null) return new List<VdtQtBcQuyetToanNienDoViewModel>();
            return data.ToList();
        }

        public VDT_QT_BCQuyetToanNienDo GetBaoCaoQuyetToanById(Guid iId)
        {
            VDT_QT_BCQuyetToanNienDo data = _vdtService.GetBaoCaoQuyetToanById(iId);
            return data ?? new VDT_QT_BCQuyetToanNienDo();
        }

        public Guid InsertBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data, string sUserName)
        {
            data.iID_BCQuyetToanNienDoID = Guid.NewGuid();
            data.sUserCreate = data.sUserUpdate = sUserName;
            data.dDateCreate = data.dDateUpdate = DateTime.Now;
            data.bIsDuyet = false;
            data.bIsCanBoDuyet = false;
            _vdtService.InsertVdtQtBcQuyetToanNienDo(data);
            return data.iID_BCQuyetToanNienDoID;
        }

        public Guid UpdateBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data, string sUserName)
        {
            var dataUpdate = _vdtService.GetBaoCaoQuyetToanById(data.iID_BCQuyetToanNienDoID);
            if (dataUpdate == null || dataUpdate.iID_BCQuyetToanNienDoID == Guid.Empty) return InsertBcQuyetToanNienDo(data, sUserName);
            dataUpdate.sSoDeNghi = data.sSoDeNghi;
            dataUpdate.dNgayDeNghi = data.dNgayDeNghi;
            dataUpdate.sUserUpdate = sUserName;
            dataUpdate.dDateUpdate = DateTime.Now;
            _vdtService.UpdateVdtQtBcQuyetToanNienDo(data);
            return data.iID_BCQuyetToanNienDoID;
        }

        public bool DeleteBCQuyetToanNienDo(Guid iId)
        {
            return _vdtService.DeleteBaoCaoQuyetToan(iId);
        }

        public List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNam(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            List<BcquyetToanNienDoVonNamChiTietViewModel> lstData;
            if (iIDQuyetToan.HasValue && iIDQuyetToan.Value != Guid.Empty)
            {
                lstData = _vdtService.GetQuyetToanVonNamById(iIDQuyetToan);
                if (lstData != null && lstData.Count != 0) return lstData;
            }
            return _vdtService.GetQuyetToanVonNam(iIdMaDonVi, iNamKeHoach, iIdNguonVon);
        }

        public List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTich(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> lstData;
            if (iIDQuyetToan.HasValue && iIDQuyetToan.Value != Guid.Empty)
            {
                lstData = _vdtService.GetQuyetToanVonNam_PhanTichById(iIDQuyetToan);
                if (lstData != null && lstData.Count != 0) return lstData;
            }
            return _vdtService.GetQuyetToanVonNam_PhanTich(iIdMaDonVi, iNamKeHoach, iIdNguonVon);
        }

        public List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUng(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            List<BcquyetToanNienDoVonUngChiTietViewModel> lstData;
            if (iIDQuyetToan.HasValue && iIDQuyetToan.Value != Guid.Empty)
            {
                lstData = _vdtService.GetQuyetToanVonUngById(iIDQuyetToan);
                if (lstData != null && lstData.Count != 0) return lstData;
            }
            return _vdtService.GetQuyetToanVonUng(iIdMaDonVi, iNamKeHoach, iIdNguonVon);
        }

        public bool UpdateQuyetToanNienDoChiTiet(Guid iIdQuyetToan, List<VDT_QT_BCQuyetToanNienDo_ChiTiet_01> lstData)
        {
            return _vdtService.UpdateQuyetToanNienDoChiTiet(iIdQuyetToan, lstData);
        }

        public bool UpdateQuyetToanNienDoChiTietPhanTich(Guid iIdQuyetToan, List<VDT_QT_BCQuyetToanNienDo_PhanTich> lstData)
        {
            return _vdtService.UpdateQuyetToanNienDoChiTietPhanTich(iIdQuyetToan, lstData);
        }
    }
}