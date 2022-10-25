using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnThongTinDuToanModel
    {
        public Guid iID_DuToanID { get; set; }
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
        public string sCoQuanPheDuyet { get; set; }
        public double? fTongDuToanPheDuyet { get; set; }
        public double? fTongDuToanPheDuyetCuoi { get; set; }
        public virtual string sNguoiKy { get; set; }
    }
}