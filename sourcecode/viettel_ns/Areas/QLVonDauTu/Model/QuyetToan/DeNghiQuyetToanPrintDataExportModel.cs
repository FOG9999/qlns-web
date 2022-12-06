using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLVonDauTu.Model.QuyetToan
{
    public class DeNghiQuyetToanPrintDataExportModel
    {
        public string iID_DeNghiQuyetToanID { get; set; }
        public int iItemLoaiBC { get; set; }
        public string txt_TieuDe { get; set; }
        public string txt_DiaDiem { get; set; }
        public string txt_KinhGui { get; set; }
        public DateTime? dNgayChungTu { get; set; }
        public string txt_TinhHinh { get; set; }
        public string txt_NhanXet { get; set; }
        public string txt_KienNghi { get; set; }
        public int? fDonViTinh { get; set; }
        public string sDonViTinh { get; set; }
    }
}