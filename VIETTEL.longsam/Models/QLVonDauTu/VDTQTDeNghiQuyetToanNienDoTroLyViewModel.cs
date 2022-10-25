using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQTDeNghiQuyetToanNienDoTroLyViewModel: VDT_QT_DeNghiQuyetToanNienDo_TroLy
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public string sTenLoaiNguonVon { get; set; }
        public decimal fGiaTriQuyetToanNamTruoc { get; set; }
        public decimal fGiaTriQuyetToanNamNay { get; set; }
        public string sGiaTriQuyetToanNamTruoc { 
            get 
            { 
                return this.fGiaTriQuyetToanNamTruoc.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            } 
        }
        public string sGiaTriQuyetToanNamNay { 
            get
            {
                return this.fGiaTriQuyetToanNamNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

    }
}
