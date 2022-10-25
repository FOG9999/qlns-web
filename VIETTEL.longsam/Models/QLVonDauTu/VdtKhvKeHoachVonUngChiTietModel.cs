using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtKhvKeHoachVonUngChiTietModel
    {
        public int STT { get; set; }
        public string sXauNoiMa { get; set; }
        public Guid id { get; set; }
        public Guid iID_DuAnID { get; set; }
        public Guid? iID_MucID { get; set; }
        public Guid? iID_TieuMucID { get; set; }
        public Guid? iID_TietMucID { get; set; }
        public Guid? iID_NganhID { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sTrangThaiDuAnDangKy { get; set; }
        public string sLNS { get; set; }
        public string sL { get; set; }
        public string sK { get; set; }
        public string sM { get; set; }
        public string sTM { get; set; }
        public string sTTM { get; set; }
        public string sNG { get; set; }
        public double fGiaTriDeNghi { get; set; }
        public string sGiaTriDeNghi
        {
            get
            {
                return this.fGiaTriDeNghi.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
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
        public double fCapPhatTaiKhoBac { get; set; }
        public string sCapPhatTaiKhoBac
        {
            get
            {
                return this.fCapPhatTaiKhoBac.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fCapPhatBangLenhChi { get; set; }
        public string sCapPhatBangLenhChi
        {
            get
            {
                return this.fCapPhatBangLenhChi.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGhiChu { get; set; }
        public bool isDelete { get; set; }
        public Guid? iID_KeHoachUngID { get; set; }
    }
}
