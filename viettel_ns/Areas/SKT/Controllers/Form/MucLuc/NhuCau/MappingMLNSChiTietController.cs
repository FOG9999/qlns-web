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
    public class MappingMLNSChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        // GET: Admin/MLNS
        public ActionResult Index(Guid id, int loai = 1)
        {
            SKT_MLNhuCau nhucauViewModel = null;
            using (var conn = _connectionFactory.GetConnection())
            {
                nhucauViewModel = conn.Get<SKT_MLNhuCau>(id);                
            }
            var vm = new MappingMLNSChiTietViewModel()
            {
                Id = id,
                NhuCauViewModel = nhucauViewModel,
                Loai = loai,
            };
            return View(vm);            
        }

        public ActionResult SheetFrame(Guid id, int loai = 1, string filters = null)
        {
            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new MappingMLNSChiTietSheetTable(Request.QueryString, true) :
             new MappingMLNSChiTietSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), true);

            sheet.FillSheet(getTable(id,loai,sheet.Filters));

            var vm = new MappingMLNSChiTietViewModel
            {
                Id = id,
                Loai = loai,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "MappingMLNSChiTiet", new { area = "SKT", id = id, loai = loai }),
                    urlGet: Url.Action("SheetFrame", "MappingMLNSChiTiet", new { area = "SKT" , id = id, loai = loai})
                    ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>
                {
                    {"F2" , "Chọn tất cả" },
                    {"F3" , "Bỏ chọn tất cả" },
                    {"F10" , "Lưu" },
                    {"Del" , "Xóa" },
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
                    var columns = new MappingMLNSChiTietSheetTable().Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id_mapping = r.Id.Split('_')[0];
                        var xau = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete
                            if (!string.IsNullOrWhiteSpace(id_mapping))
                            {
                                var entity = conn.Get<SKT_MLNS>(id_mapping);
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
                            var isNew = string.IsNullOrWhiteSpace(id_mapping);
                            if (isNew)
                            {
                                #region create
                                if (r.Columns["Map"] != "Không chọn")
                                {
                                    var entity = new SKT_MLNS()
                                    {
                                        Id = Guid.NewGuid(),
                                        NamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec),
                                        Id_MLNhuCau = new Guid(vm.Id),
                                        KyHieu = conn.Get<SKT_MLNhuCau>(vm.Id).KyHieu,
                                        Xau = xau,
                                        Lns = "",
                                        L = "",
                                        K = "",
                                        M = "",
                                        TM = "",
                                        TTM = "",
                                        NG = "",
                                        
                                        DateCreated = DateTime.Now,
                                        UserCreator = Username,
                                    };

                                    entity.MapFrom(changes);

                                    conn.Insert(entity);

                                    conn.Open();
                                    using (var cmd = new SqlCommand("sp_ncskt_update_skt_mlns", conn))
                                    {
                                        cmd.AddParams(new
                                        {
                                            id = entity.Id
                                        });
                                        
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.ExecuteNonQuery();
                                    }
                                    conn.Close();

                                }
                                #endregion
                            }
                            else
                            {
                                #region edit
                                if (r.Columns["Map"] == "Không chọn") { 
                                    var entity = conn.Get<SKT_MLNS>(id_mapping);
                                    conn.Delete(entity);
                                }
                                #endregion
                            }
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new { id = vm.Id, filters = vm.FiltersString });
        }

        #region private methods

        private DataTable getTable(Guid id, int loai, Dictionary<string, string> filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_ncskt_mapncmlns_chitietsheet", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParams(new
                {
                    Id_MLNhuCau = id,
                    nam = PhienLamViec.iNamLamViec,
                    loai,
                });

                filters.ToList().ForEach(x =>
                {
                    cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                });
                cmd.CommandType = CommandType.StoredProcedure;

                var dt = cmd.GetTable();

                return dt;
            }

        }
        #endregion
    }
}
