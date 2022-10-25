using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQuanLyDuAnModel
    {
        public VDT_DA_DuAn duAn { get; set; }
        public IEnumerable<VDT_DA_DuAn_NguonVon> listChuTruongDauTuNguonVon { get; set; }
        public IEnumerable<VDT_DA_DuAn_HangMuc> listDuAnHangMuc { get; set; }
        public IEnumerable<VDT_DA_QDDauTu_ChiPhi> listQDDauTuChiPhi { get; set; }
        public IEnumerable<TL_TaiLieu> listTaiLieu { get; set; }
        public VDT_DA_ChuTruongDauTu chuTruongDauTu { get; set; }
        public VDT_DA_QDDauTu quyetDinhDauTu { get; set; }
    }
}
