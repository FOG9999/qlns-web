using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class DeNghiQuyetToanChiTietModel: VDT_QT_DeNghiQuyetToan_ChiTiet
    {
        public Guid ChiPhiId { get; set; }

        public Guid HangMucId { get; set; }

        public Guid IdHangMucParent { get; set; }

        public string MaOrDer { get; set; }

        public int PhanCap { get; set; }

        public double? GiaTriPheDuyet { get; set; }

        public string TenChiPhi { get; set; }

        public Guid? IdChiPhiDuAnParent { get; set; }

        public bool IsShow { get; set; }

        public int Stt { get; set; }

        public double? FGiaTriKiemToan { get; set; }

        public double FGiaTriDeNghiQuyetToan { get; set; }

        public double FGiaTriAB { get; set; }

        public bool IsChiPhi { get; set; }

        public string TenLoai => IsChiPhi ? "Chi phí" : "Hạng mục";

        public List<DeNghiQuyetToanChiTietModel> ListHangMuc { get; set; }

        public string MaOrderDb { get; set; }

        public Guid? ChiPhiIdParentOfHangMuc { get; set; }

        public string MaChiPhi { get; set; }

        public string MaHangMuc { get; set; }
        public bool IsHangCha { get; set; }
        public Guid? iID_ChiPhi { get; set; }
        public double? GiaTriPheDuyetQDDT { get; set; }
        public string sGiaTriKiemToan
        {
            get 
            {
                return this.FGiaTriKiemToan.HasValue ? this.FGiaTriKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "0"; 
            }
        }
        public string sGiaTriDeNghiQuyetToan
        {
            get 
            {
                return this.fGiaTriDeNghiQuyetToan != 0 ? this.FGiaTriDeNghiQuyetToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "0"; 
            }
        }
        public string sGiaTriPheDuyet
        {
            get 
            {
                return this.GiaTriPheDuyet.HasValue ? this.GiaTriPheDuyet.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "0";
            }
        }
        public string sGiaTriPheDuyetQDDT
        {
            get 
            {
                return this.GiaTriPheDuyetQDDT.HasValue ? this.GiaTriPheDuyetQDDT.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "0";
            }
        }
    }
}
