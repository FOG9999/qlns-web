using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQLKeHoachVonUngDuocDuyetViewModel
    {
        public VDTKhvkeHoachVonUngViewModel dataKHVU { get; set; }
        public List<VdtKhvKeHoachVonUngChiTietModel> listKHVUChiTiet { get; set; }

        public double fSumTongMucDauTu
        {
            get
            {
                double sum = 0;
                if (listKHVUChiTiet != null && listKHVUChiTiet.Any())
                    sum = listKHVUChiTiet.Sum(x => x.fTongMucDauTu);
                return sum;
            }
        }
        public string sSumTongMucDauTu
        {
            get
            {
                return this.fSumTongMucDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fSumCapPhatTaiKhoBac
        {
            get
            {
                double sum = 0;
                if (listKHVUChiTiet != null && listKHVUChiTiet.Any())
                    sum = listKHVUChiTiet.Sum(x => x.fCapPhatTaiKhoBac);
                return sum;
            }
        }
        public string sSumCapPhatTaiKhoBac
        {
            get
            {
                return this.fSumCapPhatTaiKhoBac.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fSumCapPhatBangLenhChi
        {
            get
            {
                double sum = 0;
                if (listKHVUChiTiet != null && listKHVUChiTiet.Any())
                    sum = listKHVUChiTiet.Sum(x => x.fCapPhatBangLenhChi);
                return sum;
            }
        }
        public string sSumCapPhatBangLenhChi
        {
            get
            {
                return this.fSumCapPhatBangLenhChi.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class VDTKhvkeHoachVonUngViewModel : VDT_KHV_KeHoachVonUng
    {
        public string sTenDonViQuanLy { get; set; }
        public string sTenNguonVon { get; set; }
        public string sTenNhomQuanLy { get; set; }
        public string sSoDeNghi_KHVUDX { get; set; }

        public string sNgayQuyetDinh
        {
            get
            {
                return dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

    }
}
