using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class BcquyetToanNienDoVonNamPhanTichChiTietViewModel
    {
        public string SSoThuTu { get; set; }
        public Guid IIDDuAnID { get; set; }
        public string STenDuAn { get; set; }
        // col 1
        public double FDuToanCnsChuaGiaiNganTaiKbNamTruoc { get; set; }
        // col 2
        public double FDuToanCnsChuaGiaiNganTaiDvNamTruoc { get; set; }
        // col 3
        public double FDuToanCnsChuaGiaiNganTaiCucNamTruoc { get; set; }
        // col 4 
        public double FDaTamUngTheoCheDoChuaThuHoi
        {
            get 
            {
                return FTuChuaThuHoiTaiCuc + FTuChuaThuHoiTaiDonVi;
            } 
        }
        // col 5
        public double FSumDuToanDuocGiaoNamTruocChuyenSang
        {
            get
            {
                return FDuToanCnsChuaGiaiNganTaiKbNamTruoc + FDuToanCnsChuaGiaiNganTaiDvNamTruoc + FDuToanCnsChuaGiaiNganTaiCucNamTruoc + FDaTamUngTheoCheDoChuaThuHoi;
            }
        }
        // col 6
        public double FChiTieuNamNayKb { get; set; }
        // col 7
        public double FChiTieuNamNayLc { get; set; }
        // col 8 
        public double FSumDuToanDuocGiaoNamNay
        {
            get
            {
                return FChiTieuNamNayKb + FChiTieuNamNayLc;
            }
        }
        // col 9
        public double FSumDuToanDuocGiao
        {
            get
            {
                return FSumDuToanDuocGiaoNamTruocChuyenSang + FSumDuToanDuocGiaoNamNay;
            }
        }
        // col 10
        public double FSoCapNamTrcCs { get; set; }
        // col 11
        public double FSoCapNamNay { get; set; }
        // col 12
        public double FSumSoDuocCap
        {
            get
            {
                return FSoCapNamTrcCs + FSoCapNamNay;
            }
        }
        // col 13
        public double FDnQuyetToanNamTrc { get; set; }
        // col 14
        public double FDnQuyetToanNamNay { get; set; }
        // col 15
        public double FSumSoDeNghiQuyetToan 
        {
            get
            {
                return FDnQuyetToanNamTrc + FDnQuyetToanNamNay;
            }
        }
        // col 16 
        public double FSumKeHoachVonDauTuChuaQuyetToanChuyenNamSau
        {
            get
            {
                return FSumTamUngCheDoChuaThuHoi + FSumDuToanConDuChuaGiaiNgan;
            }
        }
        // col 17
        public double FSumTamUngCheDoChuaThuHoi
        {
            get
            {
                return FTuChuaThuHoiTaiCuc + FTuChuaThuHoiTaiDonVi;
            }
        }
        // col 18
        public double FTuChuaThuHoiTaiCuc { get; set; }
        // col 19
        public double FTuChuaThuHoiTaiDonVi { get; set; }
        // col 20
        public double FSumDuToanConDuChuaGiaiNgan
        {
            get
            {
                return FDuToanCnsChuaGiaiNganTaiCuc + FDuToanCnsChuaGiaiNganTaiDv + FDuToanCnsChuaGiaiNganTaiKb;
            }
        }
        // col 21
        public double FDuToanCnsChuaGiaiNganTaiCuc { get; set; }
        // col 22
        public double FDuToanCnsChuaGiaiNganTaiDv { get; set; }
        // col 23
        public double FDuToanCnsChuaGiaiNganTaiKb { get; set; }
        // col 24
        public double FDuToanThuHoi { get; set; }

    }
}
