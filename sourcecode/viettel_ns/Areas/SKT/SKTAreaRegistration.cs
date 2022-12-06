using System.Web.Mvc;

namespace VIETTEL.Areas.SKT
{
    public class SKTAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SKT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SKT_default",
                "SKT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.SKT.Controllers" }
            );

            //context.MapRoute(
            //    null, // Route name
            //    "{ext}/{id_phongban}/{id_nganh}/{dvt}", // URL with parameters
            //    new
            //    {
            //        controller = "rptNCSKT_TH02",
            //        action = "Print",
            //        ext = UrlParameter.Optional,
            //        id_phongban = UrlParameter.Optional,
            //        id_nganh = UrlParameter.Optional,
            //        dvt = UrlParameter.Optional,
            //        area = "SKT"
            //    },
            //    namespaces: new[] { "VIETTEL.Areas.SKT.Controllers" }
            //);
        }
    }
}
