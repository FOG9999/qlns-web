using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Common;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class QLKeHoachVonNamModel
    {
        private readonly INganSachNewService _nsService = NganSachNewService.Default;
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

        public List<SelectListItem> GetDataDropdownDonViQuanLy(string sMaNguoiDung)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = NganSach_HamChungModels.DSDonViCuaNguoiDung(sMaNguoiDung);
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem() { Text = Convert.ToString(dr["TenHT"]), Value = Convert.ToString(dr["iId_Ma"]) });
            }
            return lstData;
        }

        public List<SelectListItem> GetDataDropdownNguonNganSach()
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = DanhMucModels.NS_NguonNganSach();
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem() { Text = Convert.ToString(dr["sTen"]), Value = Convert.ToString(dr["iID_MaNguonNganSach"]) });
            }
            return lstData;
        }

        public List<SelectListItem> GetDataDropdownLoaiNganSach(int iNamLamViec)
        {
            List<SelectListItem> lstDataResult = new List<SelectListItem>();
            lstDataResult.Add(new SelectListItem()
            {
                Text = Constants.CHON,
                Value = string.Empty
            });
            var data = _nsService.GetDataDropdownMucLucNganSach(iNamLamViec);
            List<SelectListItem> lstData = new List<SelectListItem>();
            foreach (NS_MucLucNganSach item in data)
            {
                lstData.Add(new SelectListItem()
                {
                    Text = item.sMoTa,
                    Value = item.sLNS + "|" + item.iID_MaMucLucNganSach.ToString()
                });
            }
            lstData.Sort(delegate (SelectListItem x, SelectListItem y)
            {
                if (x.Text == null && y.Text == null) return 0;
                else if (x.Text == null) return -1;
                else if (y.Text == null) return 1;
                else return x.Text.CompareTo(y.Text);
            });
            //var result = (from lns in data
            //              select new SelectListItem()
            //              {
            //                  Text = lns.sMoTa,
            //                  Value = lns.sLNS + "|" + lns.iID_MaMucLucNganSach.ToString()
            //              }).OrderBy(n => n.Text).ToList();
            lstDataResult.AddRange(lstData);
            return lstDataResult;
        }

        public List<SelectListItem> GetDataDropdownLoaiAndKhoanByLoaiNganSach(string sLNS, int iNamKeHoach)
        {
            List<SelectListItem> lstData = new List<SelectListItem>();
            DataTable dt = _nsService.GetDataDropdownLoaiAndKhoanByLoaiNganSach(sLNS, iNamKeHoach);
            lstData.Add(new SelectListItem()
            {
                Text = Constants.CHON,
                Value = string.Empty
            });
            foreach (DataRow dr in dt.Rows)
            {
                lstData.Add(new SelectListItem()
                {
                    Text = string.Format("Loại {0} - Khoản {1}", Convert.ToString(dr["sL"]), Convert.ToString(dr["sK"])),
                    Value = string.Format("{0}|{1}", Convert.ToString(dr["sL"]), Convert.ToString(dr["sK"]))
                });
            }
            return lstData;
        }

        public List<SelectListItem> GetDataThongTinChiTietLoaiNganSach(string sLNS, string sLoai, string sKhoan, int iNamKeHoach)
        {
            IEnumerable<NS_MucLucNganSach> data = _nsService.GetDataThongTinChiTietLoaiNganSach(sLNS, sLoai, sKhoan, iNamKeHoach);
            List<SelectListItem> lstData = new List<SelectListItem>();
            lstData.Add(new SelectListItem()
            {
                Text = Constants.CHON,
                Value = string.Empty
            });
            foreach (NS_MucLucNganSach item in data)
            {
                lstData.Add(new SelectListItem()
                {
                    Text = string.Format("{0} - {1} - {2} - {3}", item.sM, item.sTM, item.sTTM, item.sNG),
                    Value = item.iID_MaMucLucNganSach.ToString()
                });
            }
            //return (from lns in data
            //        select new SelectListItem()
            //        {
            //            Text = string.Format("{0} - {1} - {2} - {3}", lns.sM, lns.sTM, lns.sTTM, lns.sNG),
            //            Value = lns.iID_MaMucLucNganSach.ToString()
            //        }).Distinct().ToList();
            return lstData;
        }

        public List<SelectListItem> GetDataThongTinChiTietLoaiNganSachByNganh(Guid iId_Nganh, int iNamKeHoach)
        {
            IEnumerable<NS_MucLucNganSach> data = _nsService.GetDataDropdownLoaiAndKhoanByNganh(iId_Nganh, iNamKeHoach);
            return (from lns in data
                    select new SelectListItem()
                    {
                        Text = string.Format("{0} - {1} - {2} - {3}", lns.sM, lns.sTM, lns.sTTM, lns.sNG),
                        Value = lns.iID_MaMucLucNganSach.ToString()
                    }).Distinct().ToList();
        }

        public List<SelectListItem> GetDataDropDownDuAnByLoaiCongTrinh(Guid iId)
        {
            var data = _vdtService.GetVDTDADuAnByLoaiCongTrinhID(iId);
            List<SelectListItem> lstData = new List<SelectListItem>();
            lstData.Add(new SelectListItem()
            {
                Text = Constants.CHON,
                Value = string.Empty
            });
            foreach (VDT_DA_DuAn item in data)
            {
                lstData.Add(new SelectListItem()
                {
                    Text = item.sTenDuAn,
                    Value = item.iID_DuAnID.ToString() + "|" + item.iID_CapPheDuyetID
                });
            }
            return lstData;

            //var data = _vdtService.GetVDTDADuAnByLoaiCongTrinhID(iId);
            //return (from da in data
            //        select new SelectListItem()
            //        {
            //            Text = da.sTenDuAn,
            //            Value = da.iID_DuAnID.ToString() + "|" + da.iID_CapPheDuyetID
            //        }).ToList();
        }

        public List<SelectListItem> GetDataDropdownCapPheDuyet()
        {
            var data = _vdtService.LayPhanCapDuAn();
            return (from da in data
                    select new SelectListItem()
                    {
                        Text = da.sTen,
                        Value = da.iID_PhanCapID.ToString()
                    }).ToList();
        }

        public VDT_DA_DuAn GetVDT_DA_DuAn(Guid iId)
        {
            return _vdtService.LayThongTinChiTietDuAn(iId);
        }

        public List<KeHoachVonNamViewModel> GetDataGridViewDefault()
        {
            List<KeHoachVonNamViewModel> lstData = new List<KeHoachVonNamViewModel>();
            lstData.Add(new KeHoachVonNamViewModel() { sTenDuAn = "Dự án chuyển tiếp" });
            lstData.Add(new KeHoachVonNamViewModel() { sTenDuAn = "Dự án khởi công mới" });
            lstData.Add(new KeHoachVonNamViewModel() { sTenDuAn = "Dự án kết thúc đầu tư" });
            return lstData;
        }

        public IEnumerable<KeHoachVonNamViewModel> TimKiemDuLieu(int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, DateTime? dNgayLap)
        {
            return _vdtService.GetKeHoachVonNamView(iNamKeHoach, iDonViQuanLy, iNguonVon, iLoaiNganSach, dNgayLap);
        }

        public KeHoachVonNamViewModel GetDataByChooseDuAn(Guid iIdDuAn, int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, Guid iNganh, DateTime? dNgayLap)
        {
            return _vdtService.GetThongTinChiTietByMaDuAnId(iIdDuAn, iNamKeHoach, iDonViQuanLy, iNguonVon, iLoaiNganSach, iNganh, dNgayLap);
        }

        public bool InsertVdtKhvPhanBoVon(ref VDT_KHV_PhanBoVon data, string userLogin)
        {
            data.bActive = true;
            data.bIsGoc = true;
            data.dDateCreate = DateTime.Now;
            data.sUserCreate = userLogin;

            return _vdtService.InsertVdtKhvPhanBoVon(ref data);
        }

        public bool InsertVdtKhvPhanBoVonChiTiet(ref List<VDT_KHV_PhanBoVon_ChiTiet> lstData)
        {
            return _vdtService.InsertPhanBoVonChiTiet(ref lstData);
        }

        public bool UpdateVdtKhvPhanBoVon(VDT_KHV_PhanBoVon dataUpdate, string sUserName)
        {
            var data = _vdtService.GetPhanBoVonByID(dataUpdate.iID_PhanBoVonID);
            if (data == null) return false;
            data.sSoQuyetDinh = dataUpdate.sSoQuyetDinh;
            data.dNgayQuyetDinh = dataUpdate.dNgayQuyetDinh;
            data.dDateUpdate = DateTime.Now;
            data.sUserUpdate = sUserName;
            if (_vdtService.UpdateVdtKhvPhanBoVon(data)) return true;
            return false;
        }

        public bool DeleteVdtKhvPhanBoVon(Guid id, string sUserName)
        {
            var data = _vdtService.GetPhanBoVonByID(id);
            if (data == null) return false;
            data.dDateDelete = DateTime.Now;
            data.sUserDelete = sUserName;
            data.bActive = false;
            if (!_vdtService.UpdateVdtKhvPhanBoVon(data)) return false;
            return true;
        }

        public bool ChinhSuaPhanBoVon(VDT_KHV_PhanBoVon data, List<VDT_KHV_PhanBoVon_ChiTiet> lstDetail, string sUserName)
        {
            // update data gốc
            var objDataGoc = _vdtService.GetPhanBoVonByID(data.iID_PhanBoVonID);
            objDataGoc.bActive = false;
            objDataGoc.bIsGoc = true;
            _vdtService.UpdateVdtKhvPhanBoVon(objDataGoc);
            double fTotalMoneyDetailNew = lstDetail.Sum(n => n.fGiaTrPhanBo ?? 0);
            // setup data child
            VDT_KHV_PhanBoVon dataNew = new VDT_KHV_PhanBoVon()
            {
                bActive = true,
                bIsGoc = false,
                dDateCreate = DateTime.Now,
                dNgayQuyetDinh = data.dNgayQuyetDinh,
                fGiaTrDeNghi = objDataGoc.fGiaTrDeNghi,
                fGiaTriThuHoi = objDataGoc.fGiaTriThuHoi,
                fTiGia = objDataGoc.fTiGia,
                fTiGiaDonVi = objDataGoc.fTiGiaDonVi,
                iID_DonViQuanLyID = objDataGoc.iID_DonViQuanLyID,
                iID_DonViTienTeID = objDataGoc.iID_DonViTienTeID,
                iID_KhoanNganSachID = objDataGoc.iID_KhoanNganSachID,
                iID_LoaiNguonVonID = objDataGoc.iID_LoaiNguonVonID,
                iID_NguonVonID = objDataGoc.iID_NguonVonID,
                iID_ParentId = objDataGoc.iID_PhanBoVonID,
                iID_TienTeID = objDataGoc.iID_TienTeID,
                iNamKeHoach = objDataGoc.iNamKeHoach,
                sLoaiDieuChinh = data.sLoaiDieuChinh,
                sSoQuyetDinh = data.sSoQuyetDinh,
                fGiaTrPhanBo = fTotalMoneyDetailNew + objDataGoc.fGiaTrPhanBo ?? 0,
                sUserCreate = sUserName
            };
            if (!_vdtService.InsertVdtKhvPhanBoVon(ref dataNew)) return false;

            lstDetail = lstDetail.Select(n => { n.iID_PhanBoVonID = dataNew.iID_PhanBoVonID; return n; }).ToList();
            if (!_vdtService.InsertPhanBoVonChiTiet(ref lstDetail)) return false;
            return true;
        }
    }
}