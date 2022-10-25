using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class DeNghiQuyetToanNienDoTroLyChiTietViewModel: VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet
    {
        public string sTenDuAn { get; set; }
        public string sXauNoiChuoi { get; set; }
        public decimal fChiTieuNganSachNam { get; set; }
        public decimal fCapPhatVonNamNay { get; set; }
        public decimal fTongMucDauTuPheDuyet { get; set; }

        public string sChiTieuNganSachNam
        {
            get
            {
                return this.fChiTieuNganSachNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sCapPhatVonNamNay {
            get
            {
                return this.fCapPhatVonNamNay.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sTongMucDauTuPheDuyet {
            get
            {
                return this.fTongMucDauTuPheDuyet.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }
}
