using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class BenhNhanBHXHTempViewModel : BHXH_BenhNhanTemp
    {
        public int iThangBN { get; set; }
    }

    public class BenhNhanBaoHiemQuery
    {
        public string sSTT { get; set; }
        public string sHoTen { get; set; }
        public string sNgaySinh { get; set; }
        public string sGioiTinh { get; set; }
        public string sCapBac { get; set; }
        public string sChucVu { get; set; }
        public string sMaThe { get; set; }
        public string sNgayVaoVien { get; set; }
        public string sNgayRaVien { get; set; }
        public string sSoNgayDieuTri { get; set; }
        public string sMaBV { get; set; }
        public string sTenBV { get; set; }
        public string sMaDV { get; set; }
        public string sTenDonVi { get; set; }
        public string sGhiChu { get; set; }
        public Guid? iID_ImportID { get; set; }
        public int? iLine { get; set; }

    }
}
