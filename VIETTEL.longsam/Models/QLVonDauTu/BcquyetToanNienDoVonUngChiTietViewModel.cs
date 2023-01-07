using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class BcquyetToanNienDoVonUngChiTietViewModel
    {
        public bool BIsChuyenTiep { get; set; }
        public bool IsHangCha { get; set; }
        public string iStt { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sMaDuAn { get; set; }
        public string sDiaDiem { get; set; }
        public string sTenDuAn { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public int iCoQuanThanhToan { get; set; }

        // col 6 *
        public double fGiaTriThuHoiTheoGiaiNganThucTe { get; set; }
        // col 1
        public double fUngTruocChuaThuHoiNamTruoc { get; set; }
        // col 2
        public double fLuyKeThanhToanNamTruoc { get; set; }

        //// col 3
        public double fKeHoachVonDuocKeoDai { get; set; }
        //// col 4
        public double fVonKeoDaiDaThanhToanNamNay { get; set; }

        // col 5
        public double fThuHoiVonNamNay { get; set; }
        // col 7
        public double fKHVUNamNay { get; set; }

        //// col 8
        public double fVonDaThanhToanNamNay { get; set; }
        //// col 9
        public double fKHVUChuaThuHoiChuyenNamSau { get; set; }
        //// col 10
        public double fTongSoVonDaThanhToanThuHoi { get; set; }

        // col 4 - ThanhToanKLHTNamTruocChuyenSang
        public double fThanhToanKLHTNamTruocChuyenSang { get; set; }
        // col 4 - ThanhToanUngNamTruocChuyenSang
        public double fThanhToanUngNamTruocChuyenSang { get; set; }
        // col 4 - Thu hoi tam ung nam nay dung von nam truoc
        public double fThuHoiTamUngNamNayVonNamTruoc { get; set; }
        // col 4 - Thu hoi tam ung nam truoc dung von nam truoc
        public double fThuHoiTamUngNamTruocVonNamTruoc { get; set; }

        // col 8 - ThanhToanKLHTNamNay
        public double fThanhToanKLHTTamUngNamNay { get; set; }
        // col 8 - ThanhToanUngNamNay
        public double fThanhToanUngNamNay { get; set; }
        // col 8 - Thu hoi tam ung nam nay dung von nam nay
        public double fThuHoiTamUngNamNay { get; set; }
        // col 8 - Thu hoi tam ung nam nay dung von nam truoc
        public double fThuHoiTamUngNamTruoc { get; set; }

        // --- column bo xung
        public double fLuyKeUngNamTruoc { get; set; }

        public double fKeHoachVonDuocKeoDaiView
        {
            get
            {
                return fUngTruocChuaThuHoiNamTruoc - fLuyKeThanhToanNamTruoc;
            }
        }

        public double fVonKeoDaiDaThanhToanNamNayView
        {
            get
            {
                return (fThanhToanKLHTNamTruocChuyenSang + fThanhToanUngNamTruocChuyenSang)
                    - (fThuHoiTamUngNamNayVonNamTruoc + fThuHoiTamUngNamTruocVonNamTruoc);
            }
        }

        public double fVonDaThanhToanNamNayView
        {
            get
            {
                return (fThanhToanKLHTTamUngNamNay + fThanhToanUngNamNay)
                    - (fThuHoiTamUngNamNay + fThuHoiTamUngNamTruoc);
            }
        }

        public double fKHVUChuaThuHoiChuyenNamSauView
        {
            get
            {
                return fUngTruocChuaThuHoiNamTruoc - fThuHoiVonNamNay + fKHVUNamNay;
            }
        }

        public double fTongSoVonDaThanhToanThuHoiView
        {
            get
            {
                return fLuyKeThanhToanNamTruoc
                + ((fThanhToanKLHTNamTruocChuyenSang + fThanhToanUngNamTruocChuyenSang) - (fThuHoiTamUngNamNayVonNamTruoc + fThuHoiTamUngNamTruocVonNamTruoc))
                + ((fThanhToanKLHTTamUngNamNay + fThanhToanUngNamNay) - (fThuHoiTamUngNamNay + fThuHoiTamUngNamTruoc));
            }
        }
    }
}
