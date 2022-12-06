using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtKhvVonNamDonViPheDuyetNguonVonDieuChinhQuery
    {
        [NotMapped]
        public string STT { get; set; }
        public string STenDuAn { get; set; }
        public string DiaDiemXayDung { get; set; }
        public string DiaDiemMoTaiKhoanDuAn { get; set; }
        public string ChuDauTu { get; set; }
        public string MaSoDuAnDauTu { get; set; }
        public string MaNganhKinhTe { get; set; }
        public string NangLucThietKe { get; set; }
        public string ThoiGianThucHien { get; set; }
        public string SSoQuyetDinh { get; set; }
        public DateTime? DNgayQuyetDinh { get; set; }
        [NotMapped]
        public string SoNgayThangNam
        {
            get => !IsHangCha ? string.Format("{0} - {1}", SSoQuyetDinh, DNgayQuyetDinh.HasValue ? DNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty;
        }
        public double? TongSoVonDauTu { get; set; }
        [NotMapped]
        public double? TongSoVonDauTuTrongNuoc { get; set; }
        public double? KeHoachVonDauTuGiaiDoan { get; set; }
        public double? VonThanhToanLuyKe { get; set; }
        public double? TongSoKeHoachVonNam { get; set; }
        public double? ThuHoiVonDaUngTruoc { get; set; }
        public double? VonThucHienTuDauNamDenNay { get; set; }
        public double? TongSoVonNamDieuChinh { get; set; }
        public double? ThuHoiVonDaUngTruocDieuChinh { get; set; }
        public double? TraNoXDCB { get; set; }
        public string SGhiChu { get; set; }
        public bool IsHangCha { get; set; }
        public int? Loai { get; set; }
        public Guid? IdNhomDuAn { get; set; }
    }
}
