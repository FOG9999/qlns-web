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

    public class MucLucSKTController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;

        // GET: Admin/MLNS
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan == "11")
            {
                var vm = new MucLucSKTViewModel()
                {   
                };
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult SheetFrame(string filters = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new MucLucSKTSheetTable(Request.QueryString, true) : 
             new MucLucSKTSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new MucLucSKTViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MucLucSKT", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "MucLucSKT", new { area = "SKT" })
                    ),
            };

            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {            
            var rows = vm.Rows.ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var columns = new MucLucSKTSheetTable().Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];

                        if (r.IsDeleted)
                        {
                            #region delete
                            if (!string.IsNullOrWhiteSpace(id_mucluc) || !string.IsNullOrEmpty(id_mucluc)) { 
                                var entity = conn.Get<SKT_MLSKTNT>(id_mucluc);
                                if (entity != null)
                                {
                                    conn.Delete(entity);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var isNew = string.IsNullOrWhiteSpace(id_mucluc);
                            if (isNew)
                            {
                                #region create

                                var entity = new SKT_MLSKTNT()
                                {
                                    Id = Guid.NewGuid(),
                                    NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),

                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };
                                
                                entity.MapFrom(changes);                                

                                var temp = _sKTService.UpdateParentId("SKT_MLSKTNT", PhienLamViec.iNamLamViec, entity.KyHieu, entity.KyHieuCha);
                                entity.Id_Parent = new Guid(temp["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(temp["IsParent"]);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<SKT_MLSKTNT>(id_mucluc);
                                entity.MapFrom(changes);                               

                                var temp = _sKTService.UpdateParentId("SKT_MLSKTNT", PhienLamViec.iNamLamViec, entity.KyHieu, entity.KyHieuCha);
                                entity.Id_Parent = new Guid(temp["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(temp["IsParent"]);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;
                                entity.IpModified = Request.UserHostAddress;

                                conn.Update(entity);

                                #endregion
                            }                            
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new {filters = vm.FiltersString });
        }

        #region private methods

        private DataTable getTable(Dictionary<string, string> filters)
        {

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_mlsktsheet", conn))
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
