using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_ThongTriExportDataModel
    {
        public VDT_ThongTriExportDataModel()
        {
            ListDetail = new List<VDT_ThongTri_ChiTietViewModel>();
            ListMaster = new List<VDT_ThongTriViewModel>();
        }
        public List<VDT_ThongTriViewModel> ListMaster { get; set; }
        public List<VDT_ThongTri_ChiTietViewModel> ListDetail { get; set; }
    }

    public class VDT_ThongTri_ChiTietViewModel : VDT_ThongTri_ChiTiet
    {
        public int STT { get; set; }
        public string TenLoaiCongTrinh { get; set; }
        public string MaLoaiCongTrinh { get; set; }
        public string TenMuc { get; set; }
        public string TieuMuc { get; set; }
        public string TietMuc { get; set; }
        public string Nganh { get; set; }
        public string TenDuAn { get; set; }
        public string TenChuNhaThau { get; set; }
    }
}
