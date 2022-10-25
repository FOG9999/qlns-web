using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.CapPhat.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.CapPhat.Controllers
{
    #region Models


    #endregion

    [Authorize]
    public class ChungTuChiTietController : AppController
    {
        public ActionResult Index(string id_chungtu)
        {
            var vm = new ChungTuChiTietViewModel
            {
                Id_ChungTu = id_chungtu,
                Entity = CapPhatService.Default.GetChungTu(Guid.Parse(id_chungtu)),
                Filter = 1,
                FilterOptions = CapPhat_ChungTu_SheetTableFilterType.Items,
            };

            return View(vm);
        }

        public ActionResult SheetFrame(string id, int option = 0, string filters = null)
        {
            var sheet = getSheetTable(id, option, filters);
            var vm = new ChungTuChiTietViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    option: option,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "ChungTuChiTiet", new { area = "CapPhat", id, option })
                    ),
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
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    #region crud
                    var columns = new CapPhat_ChungTu_SheetTable().Columns.Where(x => !x.IsReadonly);

                    var chungtu = conn.Get<CP_CapPhat>(vm.Id, trans);
                    var ngansachService = NganSachService.Default;
                    var mlnsList = ngansachService.GetMLNS_All(PhienLamViec.iNamLamViec);
                    rows.ForEach(r =>
                    {
                        var values = r.Id.ToList("_", true);
                        var id_chungtuchitiet = values[0];
                        var id_mucluc = values[1];
                        var id_donvi = values[2];

                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (!string.IsNullOrWhiteSpace(id_chungtuchitiet)
                        && (r.IsDeleted || (r.Columns.ContainsKey("rTuChi") && r.Columns["rTuChi"] == "0")))
                        {
                            #region delete

                            var entity = conn.Get<CP_CapPhatChiTiet>(id_chungtuchitiet, trans);
                            if (entity != null)
                            {
                                conn.Delete(entity, trans);
                            }

                            #endregion
                        }
                        else if (changes.Any())
                        {
                            var isNew = string.IsNullOrWhiteSpace(id_chungtuchitiet);
                            if (isNew)
                            {
                                #region create

                                var mucluc = mlnsList.FirstOrDefault(x => x.sXauNoiMa == id_mucluc);
                                var entity = new CP_CapPhatChiTiet()
                                {
                                    iID_MaCapPhat = Guid.Parse(vm.Id),
                                    iTrangThai = 1,
                                    iNamLamViec = chungtu.iNamLamViec,
                                    iID_MaNamNganSach = chungtu.iID_MaNamNganSach,
                                    iID_MaNguonNganSach = chungtu.iID_MaNguonNganSach,
                                    iID_MaDonVi = id_donvi,
                                    iID_MaPhongBan = chungtu.iID_MaPhongBan,

                                    dNgayCapPhat = chungtu.dNgayCapPhat,
                                    dNgayTao = DateTime.Now,
                                    sID_MaNguoiDungTao = Username,

                                    sLNS = mucluc.sLNS,
                                    sL = mucluc.sL,
                                    sK = mucluc.sK,
                                    sM = mucluc.sM,
                                    sTM = mucluc.sTM,
                                    sTTM = mucluc.sTTM,
                                    sNG = mucluc.sNG,
                                    sTNG = string.Empty,
                                    sMoTa = mucluc.sMoTa,
                                    sXauNoiMa = mucluc.sXauNoiMa,

                                };
                                entity.MapFrom(changes);
                                conn.Insert(entity, trans);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<CP_CapPhatChiTiet>(id_chungtuchitiet, trans);
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                entity.dNgayCapPhat = chungtu.dNgayCapPhat;

                                entity.MapFrom(changes);
                                conn.Update(entity, trans);

                                #endregion
                            }
                        }
                    });

                    #endregion

                    // commit to db
                    trans.Commit();
                }
            }

            // clear cache

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filters = vm.FiltersString });
        }

        private SheetTable getSheetTable(string id, int option = 0, string filter = null)
        {
            var entity = CapPhatService.Default.GetChungTu(Guid.Parse(id));

            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);

            if (entity.sLoai == "sNG")
                return new CapPhat_ChungTu_SheetTable_NG(id, option, filters);
            else
            if (entity.sLoai == "sTM")
                return new CapPhat_ChungTu_SheetTable(id, option, filters);
            else
                return new CapPhat_ChungTu_SheetTable_M(id, option, filters);
        }

    }
}
