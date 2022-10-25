using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;

namespace VIETTEL.Mvc
{
    /// <summary>
    /// Base class for all views in Abp system.
    /// </summary>
    /// <typeparam name="TModel">Type of the View Model</typeparam>
    public abstract class AppWebViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// Gets the root path of the application.
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                var appPath = HttpContext.Current.Request.ApplicationPath;
                if (appPath == null)
                {
                    return "/";
                }
                //appPath = appPath.EnsureEndsWith('/');

                return appPath;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AppWebViewPage()
        {
            //using (_unitOfWorkManager.NewUnitOfWork())
            //{
            //    LoggedOnReadOnlyUser = UserIsAuthenticated ? _membershipService.GetUser(Username, true) : null;
            //    UsersRole = LoggedOnReadOnlyUser == null ? _roleService.GetRole(AppConstants.GuestRoleName, true) : LoggedOnReadOnlyUser.Roles.FirstOrDefault();
            //    CurrentSetting = _settingService.GetSettings(true);
            //}
        }


        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        //protected bool UserIsAdmin => User.IsInRole(AppConstants.AdminRoleName);

        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;



        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationService.Default.Translate(name);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        public string L(string name, params object[] args)
        {
            //return string.Format(_localizationSource.GetResourceString(name), args);
            return LocalizationService.Default.Translate(name, args);
        }

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

        private PhienLamViecViewModel ChangePhienLamViec()
        {
            var vm = PhienLamViecViewModel.Create(Username);
            Session["NS_PhienLamViec"] = vm;
            return vm;
        }

    }

    public abstract class AppWebViewPage : AppWebViewPage<dynamic>
    {
        //internal string ControllerName => TempData[AppConstants.CurrentController].ToString();
        //internal string ActionName => TempData[AppConstants.CurrentAction].ToString();
        //internal string TagName => TempData[AppConstants.CurrentTag].ToString();

        //public ControllerViewModel ControllerViewModel => new ControllerViewModel()
        //{
        //    Controller = TempData[AppConstants.CurrentController].ToString(),
        //    Action = TempData[AppConstants.CurrentAction].ToString(),
        //    Tag = TempData[AppConstants.CurrentTag].ToString(),
        //};
    }
}

//namespace MVCForum.Website.Application.ExtensionMethods
//{
//    public static class ViewEngineExtension
//    {
//        public static string MenuClass(this AppWebViewPage page, string controller, string action, string tag = "")
//        {
//            var c = page.ControllerName == controller &&
//                    page.ActionName == action &&
//                    (string.IsNullOrWhiteSpace(tag) || page.TagName == tag)
//                ? "active"
//                : "";
//            return c;
//        }

//        public static string MenuClass(this AppWebViewPage page, ControllerViewModel vm)
//        {
//            var c = page.ControllerName == vm.Controller &&
//                    page.ActionName == vm.Action &&
//                    (string.IsNullOrWhiteSpace(vm.Tag) || page.TagName == vm.Tag)
//                ? "active"
//                : "";
//            return c;
//        }


//    }
//}
