using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLVonDauTu.Model.QuyetToan
{
    public class ReportTongHopQuyetToanDuAnHoanThanhModel
    {
        public string Stt { get; set; }
        public string MaNguonVon { get; set; }
        public string NoiDung { get; set; }
        public double DieuChinhCuoi { get; set; }
        public double KeHoach { get; set; }
        public double DaThanhToan { get; set; }
        public bool IsHangCha { get; set; }
        public int IdNguonVon { get; set; }
        public string MaDuAn { get; set; }
        public string TenDuAn { get; set; }
    }
}