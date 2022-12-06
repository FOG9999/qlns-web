using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{

    public class MappingMLNSController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {            
            var vm = new MappingMLNSViewModel()
            {   
            };
            return View(vm);           
        }

        public ActionResult SheetFrame(string filters = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new MappingMLNSSheetTable(Request.QueryString, true) : 
             new MappingMLNSSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new MappingMLNSViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MappingMLNS", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "MappingMLNS", new { area = "SKT" })
                    ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F2" , "Danh sách mục lục ngân sách chưa cấu hình" },
                    {"F3" , "Danh sách mục lục ngân sách đã cấu hình" },
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
                };
            return View("_sheetFrame", vm.Sheet);
        }       

        #region private methods

        private DataTable getTable(Dictionary<string, string> filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_mapncmlns_sheet", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.iNamLamViec,
                });

                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                var dt = cmd.GetTable();

                return dt;
            }

        }        
        #endregion
    }
}
