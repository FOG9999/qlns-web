using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{


    public class BaseController : Controller
    {
        protected readonly ILocalizationService _languageService;
        protected readonly ISettingService _settingService;
        public BaseController()
        {
            _languageService = new LocalizationService();
            _settingService = new SettingService();
        }

        public string Username
        {
            get { return Request.IsAuthenticated ? User.Identity.Name : string.Empty; }
        }

        public string L(string key)
        {
            return _languageService.Translate(key);
        }

        public string L(string key, params object[] values)
        {
            return string.Format(_languageService.Translate(key), values);
        }
    }

    [Authorize]
    public class AppController : BaseController
    {
        public PhienLamViecViewModel PhienLamViec
        {
            get
            {
                var phienLamViec = Session["NS_PhienLamViec"] != null ?
                    (PhienLamViecViewModel)Session["NS_PhienLamViec"] :
                    ChangePhienLamViec();

                return phienLamViec;
            }
        }

        protected PhienLamViecViewModel ChangePhienLamViec()
        {
            var vm = PhienLamViecViewModel.Create(Username);
            Session["NS_PhienLamViec"] = vm;
            return vm;
        }

        protected void ClearPhienLamViec()
        {
            Session["NS_PhienLamViec"] = null;

        }


        #region ui

        protected virtual JsonResult ToCheckboxList(ChecklistModel vm)
        {
            var result = this.RenderRazorViewToString("_CheckboxList", vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        protected virtual JsonResult ToDropdownList(ChecklistModel vm)
        {
            var result = vm.List
                .Select(x => string.Format("<option value='{0}'>{1}</option>", x.Value, x.Text))
                .Join("");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected virtual JsonResult ToDropdownList(SelectList list)
        {
            var result = list
                .Select(x => string.Format("<option value='{0}'>{1}</option>", x.Value, x.Text))
                .Join("");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Request.IsAjaxRequest())
            {

            }
            base.OnActionExecuting(filterContext);
        }
    }

}
