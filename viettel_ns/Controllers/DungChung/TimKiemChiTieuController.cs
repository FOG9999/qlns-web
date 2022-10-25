using System.Web.Mvc;
using VIETTEL.Models;
using Viettel.Services;
using VIETTEL.Models.TimKiemChiTieu;

namespace VIETTEL.Models.TimKiemChiTieu
{
    public class TimKiemChiTieuViewModel
    {
        public string iNamLamViec { get; set; }
        public SelectList DonViList { get; set; }
    }
}


namespace VIETTEL.Controllers.TimKiemChiTieu
{
    public class TimKiemChiTieuController : AppController
    {
        private string _viewPath = "~/Views/DungChung/TimKiemChiTieu/";

        private readonly INganSachService _nganSachService = NganSachService.Default;

        [Authorize]
        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".aspx";
            var iNamLamViec = PhienLamViec.iNamLamViec;

            var dtDvi = TimKiemChiTieuModels.LayDSDvi(iNamLamViec, Username);

            var vm = new TimKiemChiTieuViewModel
            {
                iNamLamViec = iNamLamViec,
                DonViList = dtDvi.ToSelectList("iID_MaDonVi", "iID_MaDonVi", null, "-- Chọn đơn vị --"),
            };

            return View(view, vm);
        }

        [Authorize]
        public ActionResult Edit()
        {
            if (_nganSachService.IsUserRoleType(Username, Viettel.Domain.DomainModel.UserRoleType.TroLyTongHop))
            {
                return View(_viewPath + "TimKiemChiTieu_Edit.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        [Authorize]
        public ActionResult TimKiemChiTieu_Frame()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                return View(_viewPath + "TimKiemChiTieu_Edit_Frame.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
    }
}
