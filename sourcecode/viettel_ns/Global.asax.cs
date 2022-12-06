using DomainModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using VIETTEL.Models;
using VIETTEL.Mvc;

namespace VIETTEL
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, area = "" },
                namespaces: new[] { "VIETTEL.Controllers", "VIETTEL.Report_Controllers.*" }
            );
        }

        protected void Application_Start()
        {
            this.ConfigViewEngine();
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("vi-VN"); // đặt trang hiện tại là tiếng việt

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Lấy xâu kết nối
            Connection.ConnectionString = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            RegisterRoutes(RouteTable.Routes);
            HamRiengModels.SSODomain = WebConfigurationManager.AppSettings["SSODomain"];
            HamRiengModels.SSOTimeout = Convert.ToInt32(WebConfigurationManager.AppSettings["SSOTimeout"]);

            MapperConfig.Config();
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly>() { typeof(Viettel.Data.QTA_ChungTuChiTietMapper).Assembly });

            //Lấy số bản ghi trong một trang
            Globals.PageSize = 20;
        }

        protected void Session_Start()
        {
            Session.Timeout = 120;
            //Session.Timeout = 500000;
            //Session.Abandon();
        }
    }
}
