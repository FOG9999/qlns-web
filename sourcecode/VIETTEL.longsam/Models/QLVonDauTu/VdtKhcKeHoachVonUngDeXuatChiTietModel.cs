using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtKhcKeHoachVonUngDeXuatChiTietModel : VDT_KHV_KeHoachVonUng_DX_ChiTiet
    {
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sGiaTriDeNghi
        {
            get
            {
                return this.fGiaTriDeNghi == null ? string.Empty : this.fGiaTriDeNghi.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fTongMucDauTu { get; set; }
        public string sTongMucDauTu
        {
            get
            {
                return this.fTongMucDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public bool isDelete { get; set; }
    }
}
