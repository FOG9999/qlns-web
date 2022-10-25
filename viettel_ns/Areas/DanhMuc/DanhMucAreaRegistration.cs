using System.Web.Mvc;

namespace VIETTEL.Areas.DanhMuc
{
    public class DanhMucAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DanhMuc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DanhMuc_default",
                "DanhMuc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.DanhMuc.Controllers" }
            );
        }
    }
}