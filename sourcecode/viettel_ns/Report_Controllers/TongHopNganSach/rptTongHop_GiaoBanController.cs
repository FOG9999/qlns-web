using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Models
{
    public class rptTongHop_GiaoBanViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList PhongBanList { get; set; }
    }
}

namespace VIETTEL.Report_Controllers.TongHopNganSach
{
    public class rptTongHop_GiaoBanController : AppController
    {
        private const string _viewPath = "~/Views/Report_Views/TongHopNganSach/";

        private INganSachService _ngansachService = NganSachService.Default;

        public ActionResult Index()
        {
            var iNamLamViec = ReportModels.LayNamLamViec(Username);


            var vm = new rptTongHop_GiaoBanViewModel()
            {
                iNamLamViec = ReportModels.LayNamLamViec(Username),
                //PhongBanList = new SelectList(new List<dynamic>()
                //       {
                //            new { Text = "-- TẤT CẢ --", Value= "-1"},
                //            new { Text = "B7", Value= "07"},
                //            new { Text = "B10", Value= "10"},
                //        }, "value", "text")
                PhongBanList = _ngansachService.GetPhongBanQuanLyNS(_ngansachService.CheckParam_PhongBan(PhienLamViec.iID_MaPhongBan))
                .ToSelectList("sKyHieu", "sMota", "-1", "-- Toàn cục --")
            };

            var view = _viewPath + this.ControllerName() + ".cshtml";
            return View(view, vm);
        }


    }
}
