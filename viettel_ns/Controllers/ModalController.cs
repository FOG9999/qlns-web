using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public class ModalController : Controller
    {
        // GET: Modal
        [HttpPost]
        public ActionResult OpenModal(ModalModels model)
        {
            model.Title = HttpUtility.HtmlDecode(model.Title);
            for (int i = 0; i < model.Messages.Count(); i++)
            {
                model.Messages[i] = HttpUtility.HtmlDecode(model.Messages[i]);
            }
            return PartialView("~/Views/Shared/Modal/_modalConfirm.cshtml", model);
        }
    }
}