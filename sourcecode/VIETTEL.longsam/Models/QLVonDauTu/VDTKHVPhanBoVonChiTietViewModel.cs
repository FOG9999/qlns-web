using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKHVPhanBoVonChiTietViewModel: VDT_KHV_PhanBoVon_ChiTiet
    {
        public string sXauNoiMa { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sTrangThaiDuAn { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public string sMaKetNoi { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string sTenCapPheDuyet { get; set; }
        public double fGiaTriDauTu { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }
        public Guid? iID_CapPheDuyetID { get; set; }
        public double fVonDaBoTri { get; set; }
        public double fVonConLai
        {
            get
            {
                return fGiaTriDauTu - fVonDaBoTri;
            }
        }
        public double fChiTieuBoXungTrongNam { get; set; }
        public double fNamTruocChuyenSang { get; set; }
        public double? fThuUngXDCB { get; set; }
        public double? fChiTieuNganSach { get; set; }
        public double? fChiTieuGoc { get; set; }
    }
}
