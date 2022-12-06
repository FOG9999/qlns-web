using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_ThongTriFilterResultModel
    {
        public VDT_ThongTriFilterResultModel()
        {
            ListQuyetToanKinhPhiNamTruoc = new List<VDT_ThongTriFilterChiTiet>();
            ListQuyetToanKinhPhiNamSau = new List<VDT_ThongTriFilterChiTiet>();
            ListThuKinhPhiChuyenNamSau = new List<VDT_ThongTriFilterChiTiet>();
            ListCapThanhKhoan = new List<VDT_ThongTriFilterChiTiet>();
            ListThuThanhKhoan = new List<VDT_ThongTriFilterChiTiet>();
            ListCapKinhPhiChuyenSang = new List<VDT_ThongTriFilterChiTiet>();
            ListThuNopNganSach = new List<VDT_ThongTriFilterChiTiet>();

            TongTienQuyetToanKinhPhiNamTruocAsString = "0";
            TongTienQuyetToanKinhPhiNamSauAsString = "0";
            TongTienThuKinhPhiChuyenNamSauAsString = "0";
            TongTienCapThanhKhoanAsString = "0";
            TongTienThuThanhKhoanAsString = "0";
            TongTienCapKinhPhiChuyenSangAsString = "0";
            TongTienThuNopNganSachAsString = "0";

        }

        public void FormatNumber()
        {
            if (ListCapKinhPhiChuyenSang != null)
            {
                foreach (var item in ListCapKinhPhiChuyenSang)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListCapThanhKhoan != null)
            {
                foreach (var item in ListCapThanhKhoan)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListQuyetToanKinhPhiNamSau != null)
            {
                foreach (var item in ListQuyetToanKinhPhiNamSau)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListQuyetToanKinhPhiNamTruoc != null)
            {
                foreach (var item in ListQuyetToanKinhPhiNamTruoc)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListThuKinhPhiChuyenNamSau != null)
            {
                foreach (var item in ListThuKinhPhiChuyenNamSau)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListThuNopNganSach != null)
            {
                foreach (var item in ListThuNopNganSach)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (ListThuThanhKhoan != null)
            {
                foreach (var item in ListThuThanhKhoan)
                {
                    if (item.SoTien != 0)
                    {
                        item.SoTienAsString = string.Format("{0:0,0}", item.SoTien);
                    }
                }
            }

            if (TongTienCapKinhPhiChuyenSang != 0)
            {
                TongTienCapKinhPhiChuyenSangAsString = string.Format("{0:0,0}", TongTienCapKinhPhiChuyenSang);
            }

            if (TongTienCapThanhKhoan != 0)
            {
                TongTienCapThanhKhoanAsString = string.Format("{0:0,0}", TongTienCapThanhKhoan);
            }

            if (TongTienQuyetToanKinhPhiNamSau != 0)
            {
                TongTienQuyetToanKinhPhiNamSauAsString = string.Format("{0:0,0}", TongTienQuyetToanKinhPhiNamSau);
            }

            if (TongTienQuyetToanKinhPhiNamTruoc != 0)
            {
                TongTienQuyetToanKinhPhiNamTruocAsString = string.Format("{0:0,0}", TongTienQuyetToanKinhPhiNamTruoc);
            }

            if (TongTienThuKinhPhiChuyenNamSau != 0)
            {
                TongTienThuKinhPhiChuyenNamSauAsString = string.Format("{0:0,0}", TongTienThuKinhPhiChuyenNamSau);
            }

            if (TongTienThuNopNganSach != 0)
            {
                TongTienThuNopNganSachAsString = string.Format("{0:0,0}", TongTienThuNopNganSach);
            }

            if (TongTienThuThanhKhoan != 0)
            {
                TongTienThuThanhKhoanAsString = string.Format("{0:0,0}", TongTienThuThanhKhoan);
            }
        }


        public List<VDT_ThongTriFilterChiTiet> ListQuyetToanKinhPhiNamTruoc { get; set; }
        public double? TongTienQuyetToanKinhPhiNamTruoc { get; set; }
        public string TongTienQuyetToanKinhPhiNamTruocAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListQuyetToanKinhPhiNamSau { get; set; }
        public double? TongTienQuyetToanKinhPhiNamSau { get; set; }
        public string TongTienQuyetToanKinhPhiNamSauAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListThuKinhPhiChuyenNamSau { get; set; }
        public double? TongTienThuKinhPhiChuyenNamSau { get; set; }
        public string TongTienThuKinhPhiChuyenNamSauAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListCapThanhKhoan { get; set; }
        public double? TongTienCapThanhKhoan { get; set; }
        public string TongTienCapThanhKhoanAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListThuThanhKhoan { get; set; }
        public double? TongTienThuThanhKhoan { get; set; }
        public string TongTienThuThanhKhoanAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListCapKinhPhiChuyenSang { get; set; }
        public double? TongTienCapKinhPhiChuyenSang { get; set; }
        public string TongTienCapKinhPhiChuyenSangAsString { get; set; }
        public List<VDT_ThongTriFilterChiTiet> ListThuNopNganSach { get; set; }
        public double? TongTienThuNopNganSach { get; set; }
        public string TongTienThuNopNganSachAsString { get; set; }
    }

    public class VDT_ThongTriFilterChiTiet
    {
        public VDT_ThongTriFilterChiTiet()
        {
            Muc = "";
            TieuMuc = "";
            TietMuc = "";
            Nganh = "";
            NoiDung = "";
            SoTienAsString = "0";
        }
        public string Muc { get; set; }
        public string TieuMuc { get; set; }
        public string TietMuc { get; set; }
        public string Nganh { get; set; }
        public string NoiDung { get; set; }
        public double? SoTien { get; set; }
        public string SoTienAsString { get; set; }
        public Guid? iID_DuAn { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public Guid? iID_NhaThau { get; set; }
    }
}
