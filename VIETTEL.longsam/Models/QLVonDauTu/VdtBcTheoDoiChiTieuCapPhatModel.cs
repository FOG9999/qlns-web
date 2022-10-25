using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtBcTheoDoiChiTieuCapPhatModel
    {
        public int iStt { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sTenDuAn { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sTienDo { get; set; }
        /// <summary>
        /// col 6
        /// </summary>
        public double fTmdtNsqp { get; set; }
        /// <summary>
        /// col 7
        /// </summary>
        public double fTmdtNsnn { get; set; }
        /// <summary>
        /// col 8
        /// </summary>
        public double fTmdtKhac { get; set; }
        /// <summary>
        /// col 9
        /// </summary>
        public double fTongMucDauTu
        {
            get
            {
                return fTmdtNsnn + fTmdtNsqp + fTmdtKhac;
            }
        }
        /// <summary>
        /// col 10
        /// </summary>
        public double fChiTieuNamTruoc { get; set; }
        /// <summary>
        /// col 11
        /// </summary>
        public double fChiTieuNamNay { get; set; }
        /// <summary>
        /// col 12
        /// </summary>
        public double fThanhToan { get; set; }
        /// <summary>
        /// col 13
        /// </summary>
        public double fTamUng { get; set; }
        /// <summary>
        /// col 14
        /// </summary>
        public double fThuHoiUng { get; set; }
        /// <summary>
        /// col 15
        /// </summary>
        public double fKeHoachVonNam
        {
            get
            {
                return fCapPhatKhoBac + fCapPhatCQTC;
            }
        }
        /// <summary>
        /// col 16
        /// </summary>
        public double fCapPhatKhoBac { get; set; }
        /// <summary>
        /// col 17
        /// </summary>
        public double fCapPhatCQTC { get; set; }
        /// <summary>
        /// col 19
        /// </summary>
        public double fChiTieuConLaiChuaCapPhat
        {
            get
            {
                return fChiTieuNamNay - fThanhToan - fTamUng + fThuHoiUng;
            }
        }
        /// <summary>
        /// col 20
        /// </summary>
        public double fVonPhaiBoTriTiep
        {
            get
            {
                return fTongMucDauTu - fChiTieuNamTruoc - fChiTieuNamNay - fKeHoachVonNam;
            }
        }
        // --- String bao cao
        /// <summary>
        /// col 6
        /// </summary>
        public string sTmdtNsqp { get { return fTmdtNsqp.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 7
        /// </summary>
        public string sTmdtNsnn { get { return fTmdtNsnn.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 8
        /// </summary>
        public string sTmdtKhac { get { return fTmdtKhac.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 9
        /// </summary>
        public string sTongMucDauTu { get { return fTongMucDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 10
        /// </summary>
        public string sChiTieuNamTruoc { get { return fChiTieuNamTruoc.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 11
        /// </summary>
        public string sChiTieuNamNay { get { return fChiTieuNamNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 12
        /// </summary>
        public string sThanhToan { get { return fThanhToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 13
        /// </summary>
        public string sTamUng { get { return fTamUng.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 14
        /// </summary>
        public string sThuHoiUng { get { return fThuHoiUng.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 15
        /// </summary>
        public string sKeHoachVonNam { get { return fKeHoachVonNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 16
        /// </summary>
        public string sCapPhatKhoBac { get { return fCapPhatKhoBac.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 17
        /// </summary>
        public string sCapPhatCQTC { get { return fCapPhatCQTC.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 19
        /// </summary>
        public string sChiTieuConLaiChuaCapPhat { get { return fChiTieuConLaiChuaCapPhat.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
        /// <summary>
        /// col 20
        /// </summary>
        public string sVonPhaiBoTriTiep { get { return fVonPhaiBoTriTiep.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")); } }
    }
}
