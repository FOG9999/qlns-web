using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class KHVNDXExportExcelModel
    {
        public string STT { get; set; }
        public Guid iID_KeHoachVonNam_DeXuatID { get; set; }
        public string STenDuAn { get; set; }
        public string SMaDuAn { get; set; }
        public string SChuDauTu { get; set; }
        public string SMaChuDauTu{ get; set; }
        public string SDonViQuanLy { get; set; }
        public string SMaDonViQuanLy { get; set; }
        public double? TongMucDauTuDuocDuyet { get; set; }
        public double? LuyKeVonThucHienTruocNam { get; set; }
        public double? TongSoKeHoachVon { get; set; }
        public double? KeHoachVonDuocGiao { get; set; }
        public double? VonKeoDaiCacNamTruoc { get; set; }
        public double? UocThucHien { get; set; }
        public double? LuyKeVonDaBoTriHetNam { get; set; }
        public double? TongNhuCauVonNamSau { get; set; }
        public double? ThuHoiVonUngTruoc { get; set; }
        public double? ThanhToan { get; set; }
        public Guid? IIdLoaiCongTrinh { get; set; }
        public Guid? IIdLoaiCongTrinhParent { get; set; }
        public string INamKeHoach { get; set; }
    }
}
