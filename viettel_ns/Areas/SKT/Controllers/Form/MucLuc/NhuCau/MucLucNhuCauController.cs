using DapperExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class MucLucNhuCauController : AppController
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
                var vm = new MucLucNhuCauViewModel()
                {   
                };
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult SheetFrame(string filters = null, string message = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new MucLucNhuCauSheetTable(Request.QueryString, true) : 
             new MucLucNhuCauSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(sheet.Filters));

            var vm = new MucLucNhuCauViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MucLucNhuCau", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "MucLucNhuCau", new { area = "SKT" }),
                    message: message
                    ),
            };            
                                  
            return View("_sheetFrame", vm.Sheet );
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {            
            var rows = vm.Rows.ToList();
            var message = "";
            var messagedel = "";
            var messageadd = "";
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var columns = new MucLucNhuCauSheetTable().Columns.Where(x => !x.IsReadonly);
                    var ngansachService = NganSachService.Default;                    
                    rows.ForEach(r =>
                    {
                        var id_mucluc = r.Id.Split('_')[0];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id_mucluc) || !string.IsNullOrEmpty(id_mucluc))
                            {
                                var entity = conn.Get<SKT_MLNhuCau>(id_mucluc);
                                if (entity != null)
                                {
                                    try { 
                                        conn.Delete(entity);
                                    }                                   
                                    catch (Exception ex)
                                    {                                       
                                        if (messagedel == "")
                                        {
                                            messagedel += entity.MoTa;
                                        }
                                        else
                                        {
                                            messagedel += ", " + entity.MoTa;
                                        }
                                    }
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

                                var entity = new SKT_MLNhuCau()
                                {
                                    Id = Guid.NewGuid(),
                                    NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),

                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };

                                entity.MapFrom(changes);
                                var temp = _sKTService.UpdateParentId("SKT_MLNhuCau", PhienLamViec.iNamLamViec, entity.KyHieu, entity.KyHieuCha);
                                entity.Id_Parent = new Guid(temp["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(temp["IsParent"]);

                                try
                                {
                                    conn.Insert(entity);
                                }
                                catch (Exception ex)
                                {
                                    if (messageadd == "")
                                    {
                                        messageadd += entity.MoTa;
                                    }
                                    else
                                    {
                                        messageadd += ", " + entity.MoTa;
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<SKT_MLNhuCau>(id_mucluc);
                                entity.MapFrom(changes);

                                var temp = _sKTService.UpdateParent("SKT_MLNhuCau", PhienLamViec.iNamLamViec, entity.KyHieu, entity.KyHieuCha);
                                entity.Id_Parent = new Guid(temp["Id_Parent"]);
                                entity.IsParent = Convert.ToBoolean(temp["IsParent"]);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;
                                entity.IpModified = Request.UserHostAddress;

                                conn.Update(entity);

                                var sql = @"update  SKT_EXCLUDE
                                            set     KyHieu = (select top(1) KyHieu from SKT_MLNhuCau where Id = Id_MLNC)";
                                conn.Open();
                                using (var cmd = new SqlCommand(sql, conn))
                                {                                    
                                    var result = cmd.ExecuteNonQuery();
                                }
                                sql = @"update  SKT_MLNS
                                        set     KyHieu = (select top(1) KyHieu from SKT_MLNhuCau where Id = Id_MLNhuCau)";
                                using (var cmd = new SqlCommand(sql, conn))
                                {
                                    var result = cmd.ExecuteNonQuery();
                                }
                                sql = @"update  SKT_NCSKT
                                        set     KyHieu = (select top(1) KyHieu from SKT_MLNhuCau where Id = Id_MLNhuCau)";
                                using (var cmd = new SqlCommand(sql, conn))
                                {                                    
                                    var result = cmd.ExecuteNonQuery();
                                }
                                conn.Close();
                                #endregion
                            }                            
                        }

                    });                    
                }
            }
            if (messagedel != "" && messageadd == "")
            {
                message = "Mục lục nhu cầu " + messagedel + " đã có dữ liệu chi tiêt! Không được xóa mục này!";
            }
            return RedirectToAction("SheetFrame", new {filters = vm.FiltersString, message = message });
        }

        #region private methods

        private DataTable getTable(Dictionary<string, string> filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_mlncsheet", conn))
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
                conn.Open();
                var dt = cmd.GetTable();
                conn.Close();

                return dt;
            }

        }        
        #endregion
    }
}
