using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLThongTinHopDong;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using static VIETTEL.Common.Constants;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class GiaiNganThanhToanModel
    {
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

        public List<SelectListItem> GetDataDropdownDuAn(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = _vdtService.GetDuAnInGiaiNganThanhToan(iID_DonViQuanLyID, iID_NguonVonID, iID_LoaiNguonVonID, dNgayQuyetDinh, iNamKeHoach, iID_NganhID);
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem() { Text = Convert.ToString(dr["sTenDuAn"]), Value = Convert.ToString(dr["iID_DuAnID"]) + "|" + Convert.ToString(dr["TenPhanCap"]) });
            }
            return lstData;
        }

        public List<SelectListItem> GetDataDropdownHopDong(Guid iIdDuAn)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            IEnumerable<VDT_DA_TT_HopDong> data = _vdtService.GetHopDongByThanhToanDuAnId(iIdDuAn);
            return (from dt in data
                    select new SelectListItem
                    {
                        Text = dt.sSoHopDong,
                        Value = dt.iID_HopDongID.ToString()
                    }).ToList();
        }

        public HopDongDetailModel GetDetailHopDongDuAn(Guid iID_DuAnID, Guid iID_HopDongID, DateTime dNgayDeNghi, int iNamKeHoach, int iID_NguonVonID, Guid iID_LoaiNguonVonID, Guid iID_NganhID)
        {
            return _vdtService.GetDetailHopDongByDuAnId(iID_DuAnID, iID_HopDongID, dNgayDeNghi, iNamKeHoach, iID_NguonVonID, iID_LoaiNguonVonID, iID_NganhID);
        }

        public VDT_TT_PheDuyetThanhToan_ChiTiet ConvertDataPheDuyetThanhToanChiTiet(PheDuyetThanhToanChiTiet data, string sUserLogin)
        {
            VDT_TT_PheDuyetThanhToan_ChiTiet model = new VDT_TT_PheDuyetThanhToan_ChiTiet();
            model.iID_DeNghiThanhToanID = data.iID_DeNghiThanhToanID;
            model.iID_PheDuyetThanhToan_ChiTietID = data.iID_PheDuyetThanhToan_ChiTietID;
            model.iID_NganhID = data.iID_NganhID;
            model.sM = data.sM;
            model.sTM = data.sTM;
            model.sTTM = data.sTTM;
            model.sNG = data.sNG;
            model.sGhiChu = data.sGhiChu;

            if (data.iLoai == (int)LoaiThanhToan.Type.THANH_TOAN || data.iLoai == (int)LoaiThanhToan.Type.TAM_UNG)
            {
                model.iID_KeHoachVonID = data.iID_KeHoachVonID;
                model.iLoaiKeHoachVon = data.iLoaiKeHoachVon;

                model.fGiaTriThanhToanNN = data.fGiaTriNgoaiNuoc;
                model.fGiaTriThanhToanTN = data.fGiaTriTrongNuoc;
            }
            else
            {
                if (data.iLoaiNamKH == (int)LoaiNamKeHoach.Type.NAM_NAY)
                {
                    model.fGiaTriThuHoiNamNayNN = data.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiNamNayTN = data.fGiaTriTrongNuoc;
                }
                else if (data.iLoaiNamKH == (int)LoaiNamKeHoach.Type.NAM_TRUOC)
                {
                    model.fGiaTriThuHoiNamTruocNN = data.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiNamTruocTN = data.fGiaTriTrongNuoc;
                }
                model.iId_DeNghiTamUng = data.iID_KeHoachVonID;
            }
            return model;
        }

        public bool UpdateDeNghiThanhToan(VDT_TT_DeNghiThanhToan dataUpdate, string sUserLogin)
        {
            var data = _vdtService.GetDeNghiThanhToanByID(dataUpdate.iID_DeNghiThanhToanID);
            data.sSoDeNghi = dataUpdate.sSoDeNghi;
            data.sSoBangKLHT = dataUpdate.sSoBangKLHT;
            data.dNgayBangKLHT = dataUpdate.dNgayBangKLHT;
            data.fLuyKeGiaTriNghiemThuKLHT = dataUpdate.fLuyKeGiaTriNghiemThuKLHT;
            data.sNguoiLap = dataUpdate.sNguoiLap;

            data.fGiaTriThanhToanTN = dataUpdate.fGiaTriThanhToanTN;
            data.fGiaTriThanhToanNN = dataUpdate.fGiaTriThanhToanNN;
            data.fGiaTriThuHoiTN = dataUpdate.fGiaTriThuHoiTN;
            data.fGiaTriThuHoiNN = dataUpdate.fGiaTriThuHoiNN;

            data.fGiaTriThuHoiUngTruocTN = dataUpdate.fGiaTriThuHoiUngTruocTN;
            data.fGiaTriThuHoiUngTruocNN = dataUpdate.fGiaTriThuHoiUngTruocNN;

            data.fThueGiaTriGiaTang = dataUpdate.fThueGiaTriGiaTang;
            data.fChuyenTienBaoHanh = dataUpdate.fChuyenTienBaoHanh;
            data.sGhiChu = dataUpdate.sGhiChu;

            data.sUserUpdate = sUserLogin;
            data.dDateUpdate = DateTime.Now;
            data.sThongTinCanCu = dataUpdate.sThongTinCanCu;
            return _vdtService.UpdateDeNghiThanhToan(data);
        }

        public bool DeleteDeNghiThanhToan(Guid id, string sUserName)
        {
            if (!_vdtService.DeleteDeNghiThanhToan(id)) return false;
            if (!_vdtService.DeletePheDuyetThanhToanChiTietByThanhToanId(id)) return false;
            return true;
        }
    }
}