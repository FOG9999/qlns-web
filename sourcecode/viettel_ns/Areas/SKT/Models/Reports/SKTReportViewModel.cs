using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class SKTReportViewModel
    {
        public string loai { get; set; }
    }

    public class rptSKT_M03ViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
        public string TieuDe { get; set; }
        public ChecklistModel DonViList { get; set; }
    }

    public class rptKiem_CauHinhNCMLNSViewModel
    {
        public SelectList DonViList { get; set; }
    }

    public class rptKiem_K2ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }

    public class rptKiem_K4ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList LoaiList { get; set; }
    }
    public class rptKiem_K4DViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList LoaiList { get; set; }
    }
    public class rptKiem_K5ViewModel
    {
        public SelectList NganhList { get; set; }
    }
    public class rptTrinhKy_K1ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptTrinhKy_K1NViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_BC01ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList LoaiList { get; set; }
        public SelectList PBList { get; set; }
    }
    public class rptSKT_BC02ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_BC03ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_BC04ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList LoaiList { get; set; }
    }
    public class rptSKT_BC05ViewModel
    {
        public SelectList PhongBanList { get; set; }
    }
    public class rptSKT_BCTHNTViewModel
    {
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_BCTHNT_SSViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_PL01ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptSKT_PL02ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
    }
    public class rptSKT_PL03ViewModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList NganhList { get; set; }
        public SelectList MucList { get; set; }
        public int  LoaiBC { get; set; }
    }
    public class rptSKT_PL04ViewModel
    {       
        public int LoaiBC { get; set; }
        public SelectList LoaiList { get; set; }
        public SelectList MucList { get; set; }
    }
    public class rptSKT_ReportModel
    {
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
        public SelectList NganhList { get; set; }
    }
}
