using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class BcquyetToanNienDoVonNamChiTietViewModel
    {
        public string iStt { get; set; }
        public bool BIsChuyenTiep { get; set; }
        public bool IsHangCha { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sMaDuAn { get; set; }
        public string sDiaDiem { get; set; }
        public string sTenDuAn { get; set; }
        public double fTongMucDauTu { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public bool bIsShow { get; set; }

        // column 6
        public double fLuyKeThanhToanNamTruoc { get; set; }
        // column 7
        public double fTamUngTheoCheDoChuaThuHoiNamTruoc
        {
            get
            {
                return fTamUngChuaThuHoiNamTruoc - fTamUngDaThuHoiNamTruoc;
            }
        }
        // column 8*
        public double fGiaTriTamUngDieuChinhGiam { get; set; }
        // column 9
        public double fTamUngNamTruocThuHoiNamNay { get; set; }
        // column 10
        public double fKHVNamTruocChuyenNamNay { get; set; }
        // column 11
        public double fTongThanhToanVonKeoDaiNamNay
        {
            get
            {
                return fTongThanhToanSuDungVonNamTruoc + fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay;
            }
        }
        // column 12
        public double fTongThanhToanSuDungVonNamTruoc { get; set; }
        // column 13
        public double fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay
        {
            get
            {
                return fTamUngNamNayDungVonNamTruoc - fThuHoiTamUngNamNayDungVonNamTruoc;
            }
        }
        // column 14*
        public double fGiaTriNamTruocChuyenNamSau { get; set; }
        // column 15
        public double fVonConLaiHuyBoKeoDaiNamNay
        {
            get
            {
                return fKHVNamTruocChuyenNamNay - fTongThanhToanVonKeoDaiNamNay - fGiaTriNamTruocChuyenNamSau;
            }
        }
        // column 16
        public double fKHVNamNay { get; set; }
        // column 17
        public double fTongKeHoachThanhToanVonNamNay
        {
            get
            {
                return fTongThanhToanSuDungVonNamNay + fTamUngTheoCheDoChuaThuHoiNamNay;
            }
        }
        // column 18
        public double fTongThanhToanSuDungVonNamNay { get; set; }
        // column 19
        public double fTamUngTheoCheDoChuaThuHoiNamNay
        {
            get
            {
                return fTongTamUngNamNay - fTongThuHoiTamUngNamNay;
            }
        }
        // column 20*
        public double fGiaTriNamNayChuyenNamSau { get; set; }
        // column 21
        public double fVonConLaiHuyBoNamNay
        {
            get
            {
                return fKHVNamNay - fTongKeHoachThanhToanVonNamNay - fGiaTriNamNayChuyenNamSau;
            }
        }
        // column 22
        public double fTongVonThanhToanNamNay
        {
            get
            {
                return fTamUngNamTruocThuHoiNamNay + fTongThanhToanSuDungVonNamTruoc + fTongThanhToanSuDungVonNamNay;
            }
        }
        // column 23
        public double fLuyKeTamUngChuaThuHoiChuyenSangNam
        {
            get
            {
                return fTamUngTheoCheDoChuaThuHoiNamTruoc - fGiaTriTamUngDieuChinhGiam - fTamUngNamTruocThuHoiNamNay
                    + fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay + fTamUngTheoCheDoChuaThuHoiNamNay;
            }     
        }
        // column 24
        public double fLuyKeConDaThanhToanHetNamNay
        {
            get
            {
                return fLuyKeThanhToanNamTruoc + fTongThanhToanVonKeoDaiNamNay + fTongKeHoachThanhToanVonNamNay;
            }
        }
        public double fThuHoiTamUngNamNayDungVonNamTruoc { get; set; }
        public double fTongTamUngNamNay { get; set; }
        public double fTamUngNamNayDungVonNamTruoc { get; set; }
        public double fTongThuHoiTamUngNamNay { get; set; }
        public double fTamUngChuaThuHoiNamTruoc { get; set; }
        public double fTamUngDaThuHoiNamTruoc { get; set; }
    }
}
