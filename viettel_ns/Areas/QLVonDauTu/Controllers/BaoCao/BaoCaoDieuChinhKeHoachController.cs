using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.BaoCao
{
    public class BaoCaoDieuChinhKeHoachController : AppController
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;
        // GET: QLVonDauTu/BaoCaoDieuChinhKeHoach
        public ActionResult Index()
        {
            var listNguonVon = _qLVonDauTuService.LayNguonVon();
            ViewBag.ListNguonVon = listNguonVon.ToSelectList("iID_MaNguonNganSach", "sTen");
            var listLoaiNguonVon = _qLVonDauTuService.LayLoaiNguonVon(PhienLamViec.NamLamViec);
            ViewBag.ListLoaiNguonVon = listLoaiNguonVon.ToSelectList("sLNS", "sMoTa");
            return View();
        }

        [HttpPost]
        public ActionResult LayBaoCaoDieuChinhKeHoach(int? iID_NguonVon, string sLNS, int? iNamThucHien)
        {
            IEnumerable<VDTBaoCaoDieuChinhKeHoachViewModel> lstBaoCao = _qLVonDauTuService.LayBaoCaoDieuChinhKeHoach(iID_NguonVon, sLNS, iNamThucHien, Username);
            List<VDTBaoCaoDieuChinhKeHoachViewModel> data = lstBaoCao.ToList();
            return PartialView("_baoCaoDieuChinhKeHoach", data);
        }
    }
}