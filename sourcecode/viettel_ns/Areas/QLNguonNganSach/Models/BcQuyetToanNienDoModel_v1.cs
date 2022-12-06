//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Viettel.Domain.DomainModel;
//using Viettel.Models.QLVonDauTu;
//using Viettel.Services;
//using VIETTEL.Models;
//using static VIETTEL.Common.Constants;

//namespace VIETTEL.Areas.QLNguonNganSach.Models
//{
//    public class BcQuyetToanNienDoModel
//    {
//        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

//        public List<SelectListItem> GetCoQuanThanhToan()
//        {
//            List<SelectListItem> lstCbx = new List<SelectListItem>();
//            lstCbx.Add(new SelectListItem() { Text = CoQuanThanhToan.TypeName.KHO_BAC, Value = ((int)CoQuanThanhToan.Type.KHO_BAC).ToString() });
//            lstCbx.Add(new SelectListItem() { Text = CoQuanThanhToan.TypeName.CQTC, Value = ((int)CoQuanThanhToan.Type.CQTC).ToString() });
//            return lstCbx;
//        }

//        public List<SelectListItem> GetLoaiThanhToan()
//        {
//            List<SelectListItem> lstCbx = new List<SelectListItem>();
//            lstCbx.Add(new SelectListItem() { Text = LoaiThanhToan.TypeName.THANH_TOAN, Value = ((int)LoaiThanhToan.Type.THANH_TOAN).ToString() });
//            lstCbx.Add(new SelectListItem() { Text = LoaiThanhToan.TypeName.TAM_UNG, Value = ((int)LoaiThanhToan.Type.TAM_UNG).ToString() });
//            return lstCbx;
//        }

//        public List<VdtQtBcQuyetToanNienDoViewModel> GetPagingIndex(ref PagingInfo _paging, string iIdMaDonViQuanLy = null, int? iIdNguonVon = null, DateTime? dNgayDeNghiFrom = null, DateTime? dNgayDeNghiTo = null, int? iNamKeHoach = null)
//        {
//            var data = _vdtService.GetAllBaoCaoQuyetToanPaging(ref _paging, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach);
//            if (data == null) return new List<VdtQtBcQuyetToanNienDoViewModel>();
//            return data.ToList();
//        }

//        public VDT_QT_BCQuyetToanNienDo GetBaoCaoQuyetToanById(Guid iId)
//        {
//            return _vdtService.GetBaoCaoQuyetToanById(iId);
//        }

//        public bool InsertBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data, string sUserName)
//        {
//            data.iID_BCQuyetToanNienDoID = Guid.NewGuid();
//            data.sUserCreate = data.sUserUpdate = sUserName;
//            data.dDateCreate = data.dDateUpdate = DateTime.Now;
//            data.bIsDuyet = false;
//            data.bIsCanBoDuyet = false;
//            return _vdtService.InsertVdtQtBcQuyetToanNienDo(data);
//        }

//        public bool UpdateBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data, string sUserName)
//        {
//            var dataUpdate = _vdtService.GetBaoCaoQuyetToanById(data.iID_BCQuyetToanNienDoID);
//            if (dataUpdate == null || dataUpdate.iID_BCQuyetToanNienDoID == Guid.Empty) return false;
//            dataUpdate.sSoDeNghi = data.sSoDeNghi;
//            dataUpdate.dNgayDeNghi = data.dNgayDeNghi;
//            dataUpdate.sUserUpdate = sUserName;
//            dataUpdate.dDateUpdate = DateTime.Now;
//            return _vdtService.UpdateVdtQtBcQuyetToanNienDo(data);
//        }

//        public bool DeleteBCQuyetToanNienDo(Guid iId)
//        {
//            return _vdtService.DeleteBaoCaoQuyetToan(iId);
//        }

//        public List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNam(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon, int iCoQuanThanhToan)
//        {
//            return _vdtService.GetQuyetToanVonNam(iIDQuyetToan, iIdMaDonVi, iNamKeHoach, iIdNguonVon, iCoQuanThanhToan);
//        }

//        public List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUng(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon, int iCoQuanThanhToan)
//        {
//            return _vdtService.GetQuyetToanVonUng(iIDQuyetToan, iIdMaDonVi, iNamKeHoach, iIdNguonVon, iCoQuanThanhToan);
//        }
//    }
//}