using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIETTEL.Areas.QLNH
{
    public class QLNHAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QLNH",
                "QLNH/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }

        public override string AreaName
        {
            get
            {
                return "QLNH";
            }
        }
    }
}