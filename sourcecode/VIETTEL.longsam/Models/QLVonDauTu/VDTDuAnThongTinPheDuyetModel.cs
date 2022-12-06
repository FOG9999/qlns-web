using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnThongTinPheDuyetModel
    {
        public Guid iID_QDDauTuID { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sNgayQuyetDinh
        {
            get
            {
                return (dNgayQuyetDinh == null ? "" : dNgayQuyetDinh.Value.ToString("dd/MM/yyyy"));
            }
        }
        public string sSoToTrinh { get; set; }
        public string sSoThamDinh { get; set; }
        public string sCoQuanPheDuyet { get; set; }
        public double? fTongMucDauTuPheDuyet { get; set; }
        public double? fTongMucDauTuPheDuyetCuoi { get; set; }
        public int iSoLanDieuChinh { get; set; }
        public string sNguoiKy { get; set; }
    }
}