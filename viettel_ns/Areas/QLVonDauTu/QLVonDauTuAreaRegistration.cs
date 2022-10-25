using System.Web.Mvc;

namespace VIETTEL.Areas.QLVonDauTu
{
    public class QLVonDauTuAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QLVonDauTu";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QLVonDauTu_default",
                "QLVonDauTu/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}