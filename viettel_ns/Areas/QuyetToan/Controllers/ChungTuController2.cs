using AutoMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Areas.QuyetToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QuyetToan.Controllers
{
    public class ChungTu2Controller : AppController
    {
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;

        // GET: QuyetToan/ChungTu
        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                id = LNSType.NSQP.ToString();

            return index(id);
        }

        #region index

        public ActionResult NSQP()
        {
            return index(LNSType.NSQP.ToString());
        }

        public ActionResult NSNN()
        {
            return index(LNSType.NSNN.ToString());
        }

        public ActionResult NSK()
        {
            return index(LNSType.NSK.ToString());
        }

        private ActionResult index(string id)
        {
            var vm = new ChungTuListViewModel()
            {
                Type = id,
                Items = getChungTuList(id.ParseEnum<LNSType>().ToString()),
            };

            TempData["Type"] = id;
            return View("list", vm);
        }

        #endregion

        public ActionResult Edit(Guid id, string type)
        {
            var vm = _quyetToanService.GetChungTu(id).ToViewModel();

            vm.DonViList = PhienLamViec.DonViList.ToSelectList();
            vm.LoaiNganSachList = PhienLamViec.LNSList.ToSelectList();
            vm.QuyList = DataHelper.GetQuys().ToSelectList();

            TempData["Type"] = type;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(ChungTuViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var type = TempData["Type"];
            return RedirectToAction("Index", type);
        }

        public ActionResult Delete(string id)
        {
            return View("Index");
        }

        #region create

        public ActionResult Create(string id)
        {
            var vm = createViewModel(id);
            return View(vm);
        }

        #endregion

        #region private methods

        private ChungTuViewModel createViewModel(string lns)
        {

            var tiento = PhanHeModels.LayTienToChungTu(PhanHeModels.iID_MaPhanHeQuyetToan);
            var iSoChungTu = _quyetToanService.GetMax() + 1;

            var donviList = PhienLamViec.DonViList.ToSelectList("-1", "-- Chọn đơn vị --");

            var vm = new ChungTuViewModel()
            {
                iSoChungTu = iSoChungTu,
                sTienToChungTu = tiento,
                dNgayChungTu = DateTime.Now.Date,


                //LoaiNganSachList = _ngansachService.GetLNS(PhienLamViec.iID_MaPhongBan, PhienLamViec.iNamLamViec, lns)
                QuyList = DataHelper.GetQuys().ToSelectList(),
                DonViList = donviList,
            };
            return vm;
        }


        private IEnumerable<ChungTuViewModel> getChungTuList(string lns)
        {
            //var items = _quyetToanService.GetAll(PhienLamViec.iNamLamViec, Username, lns)
            //     .MapToList<QTA_ChungTu, ChungTuViewModel>();
            var items = _quyetToanService.GetAll(PhienLamViec.iNamLamViec, Username, PhienLamViec.iID_MaPhongBan, lns)
              .Select(x => ((object)x).MapTo<ChungTuViewModel>())
              .ToList();

            return items;
        }


        #endregion

    }
}
