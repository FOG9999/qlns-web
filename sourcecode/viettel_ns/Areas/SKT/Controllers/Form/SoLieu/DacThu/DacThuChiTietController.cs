using AutoMapper.Extensions;
using DapperExtensions;
using Microsoft.Ajax.Utilities;
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
    public class DacThuChiTietController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;
        private int _dvt = 1000;

        // GET: Admin/MLNS
        public ActionResult Index(Guid id)
        {
            DacThuChungTuDetailsViewModel chungtuViewModel = null;
            var isReadonly = false;
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_DacThuChungTu>(id);
                chungtuViewModel = entity.MapTo<DacThuChungTuDetailsViewModel>();

                isReadonly = entity.UserCreator != Username ||
                    _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_Nganh, PhienLamViec.iID_MaPhongBan, Username) ||
                    entity.Locked;
            }
            var vm = new SKTDacThuSheetViewModel()
            {
                Id = id,
                ChungTuViewModel = chungtuViewModel,
                Sheet = new SheetViewModel(isReadonly: isReadonly),
            };

            return View(vm);
        }

        public ActionResult SheetFrame(Guid id, string filters = null)
        {
            var isReadonly = false;
            var entity = new SKT_DacThuChungTu();
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                entity = conn.Get<SKT_DacThuChungTu>(id);                
                isReadonly = entity.UserCreator != Username ||
                    _sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_Nganh, PhienLamViec.iID_MaPhongBan, Username) ||
                    entity.Locked;                
            }

            var sheet = string.IsNullOrWhiteSpace(filters) ?
            new DacThuSheetTable(Request.QueryString, entity, isReadonly, false) :
             new DacThuSheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters), entity, isReadonly, false);
            
            sheet.FillSheet(getTable(id, sheet.Filters));

            var vm = new SKTDacThuSheetViewModel
            {
                Id = id,
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id.ToString(),
                    filters: sheet.Filters,
                    urlPost: isReadonly ? "" : Url.Action("Save", "DacThuChiTiet", new { area = "SKT" }),
                    urlGet: Url.Action("SheetFrame", "DacThuChiTiet", new { area = "SKT" }),
                    dvt: _dvt),

            };

            return View("_sheetFrame", vm.Sheet);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    var chungtu = conn.Get<SKT_DacThuChungTu>(vm.Id);                    
                    var columns = new DacThuSheetTable(chungtu).Columns.Where(x => !x.IsReadonly);
                    rows.ForEach(r =>
                    {
                        var id = r.Id.Split('_')[0];
                        var id_mlns = r.Id.Split('_')[1];

                        if (r.IsDeleted)
                        {
                            #region delete

                            if (!string.IsNullOrWhiteSpace(id))
                            {
                                var entity = conn.Get<SKT_DacThuChiTiet>(id);
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
                            var isNew = string.IsNullOrWhiteSpace(id);
                            if (isNew)
                            {
                                #region create

                                var mucluc = conn.Get<NS_MucLucNganSach>(id_mlns);
                                var entity = new SKT_DacThuChiTiet()
                                {
                                    Id = Guid.NewGuid(),
                                    Id_ChungTu = chungtu.Id,
                                    Id_MLNS = mucluc.iID_MaMucLucNganSach,

                                    DateCreated = DateTime.Now,
                                    UserCreator = Username,
                                };
                                
                                entity.MapFrom(changes, 1000);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<SKT_DacThuChiTiet>(id);
                                entity.MapFrom(changes, 1000);

                                entity.DateModified = DateTime.Now;
                                entity.UserModifier = Username;

                                conn.Update(entity);

                                #endregion
                            }
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new { id = vm.Id, filters = vm.FiltersString });
        }

        #region private methods
        private DataTable getTable(Guid id_chungtu, Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("skt_dacthuchitietsheet.sql");

            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                var chungTu = conn.Get<SKT_DacThuChungTu>(id_chungtu);
                var nganhList = _ngansachService.Nganh_Get(chungTu.NamLamViec.ToString(), chungTu.Id_Nganh);
                cmd.AddParams(new
                {
                    NamLamViec = PhienLamViec.NamLamViec - 1,
                    nganh = nganhList.iID_MaNganhMLNS.ToParamString(),
                    id_chungtu,
                    dvt = _dvt,
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
