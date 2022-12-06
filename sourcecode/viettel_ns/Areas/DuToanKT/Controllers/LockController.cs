using AutoMapper.Extensions;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Json;
using Viettel.Services;
using VIETTEL.Areas.DuToanKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;
using System.Data;
using VIETTEL.Helpers;
using System.Data.SqlClient;
using Viettel.Data;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class LockController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IDuToanKTService _dtktService = DuToanKTService.Default;

        #region def
        // GET: DuToanKT/Lock
        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.QuanTri
                || _ngansachService.GetUserRoleType(Username) == (int)UserRoleType.ThuTruong
                || PhienLamViec.iID_MaPhongBan.IsContains("02,11"))
            {
                var vm = getListViewModel(int.Parse(PhienLamViec.iNamLamViec));
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
        public PartialViewResult Editor(string id_donvi)
        {
            var vm = getLockViewModel(PhienLamViec.iNamLamViec.ToValue<int>(), id_donvi);
            vm.PhongBanList = _ngansachService.GetPhongBanQuanLyNS().ToSelectList("sKyHieu", "sMoTa", vm.Id_PhongBan);
            return PartialView("_editor", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DTKTLockViewModel vm)
        {
            var success = false;
            var msg = "";
            if (!ModelState.IsValid)
            {
            }

            var entity = _dtktService.GetLockById(PhienLamViec.iNamLamViec.ToValue<int>(), vm.Id_DonVi);
            var isnew = false;
            if (entity == null)
            {
                isnew = true;
                entity = new DTKT_Lock()
                {
                    DateCreated = DateTime.Now,
                    UserCreator = Username,
                    iTrangThai = 1,
                    NamLamViec = PhienLamViec.iNamLamViec.ToValue<int>(),
                };

            }
            else
            {
                entity.DateModified = DateTime.Now;
                entity.UserModifier = Username;
            }

            entity.MapFrom(vm);

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    if (isnew)
                    {
                        var id = conn.Insert(entity);
                        success = id > 0;
                    }
                    else
                    {
                        success = conn.Update(entity);
                    }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                msg = ex.Message;
#else
                msg = L("Msg.Save.Failed");
#endif
            }

            if (success)
            {
                msg = L("Msg.Save.Success");

            }

            return Json(new js_msg()
            {
                success = success,
                text = msg,
                title = success ? L("Msg.Success") : L("Msg.Failed"),
            });
        }
        public PartialViewResult LockOrOpen()
        {
            var vm = getLockIdViewModel();
            return PartialView("_editor_id", vm);
        }
        [HttpPost]
        public ActionResult UpdateLockId(
            int nam,
            int id_lock,
            string iloais = null,
            string ireqs = null,
            string phongbans = null,
            string users = null,
            string donvis = null,
            string ilans = null)
        {

            var success = false;
            var msg = "";
            if (!ModelState.IsValid)
            {
            }

            if (id_lock != -1)
            {
                try
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        var sql_update = FileHelpers.GetSqlQuery("dtkt_lockoropen_update.sql");
                        using (var cmd = new SqlCommand(sql_update, conn))
                        {
                            cmd.AddParams(new
                            {
                                nam,
                                id_lock,
                                iloais = iloais.ToParamString(),
                                ireqs = ireqs.ToParamString(),
                                phongbans = phongbans.ToParamString(),
                                users = users.ToParamString(),
                                donvis = donvis.ToParamString(),
                                ilans = ilans.ToParamString()
                            });

                            var result = cmd.ExecuteNonQuery();
                            success = result > 0;
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    msg = ex.Message;
#else
                            msg = L("Msg.Save.Failed");
#endif
                }
            }

            if (success)
            {
                msg = "Đang thực hiện, xin chờ trong giây lát!";

            }

            return Json(new js_msg()
            {
                success = success,
                text = msg,
                title = success ? (id_lock == 1 ? "Đã khóa thành công!" : "Đã mở thành công!") : "Không thực hiện thành công",
            });
        }
        #endregion

        #region public
        public JsonResult Ds_iRequest(
            int nam,
            int id_lock,
            string group = null,
            string iloais = null,
            string ireqs = null,
            string phongbans = null,
            string users = null,
            string donvis = null)
        {
            var data = getTable_Cache(nam, id_lock, group, iloais, ireqs, phongbans, users, donvis);
            if (string.IsNullOrEmpty(group) || group == "undefined")
            {
                var dt = data.SelectDistinct("Loai", "iLoai,sLoai");
                return ToCheckboxList(new ChecklistModel("iLoai", dt.ToSelectList("iLoai", "sLoai")));
            }
            else if (group == "iLoai")
            {
                var dt = data.SelectDistinct("Request", "iRequest,Request");
                return ToCheckboxList(new ChecklistModel("Request", dt.ToSelectList("iRequest", "Request")));
            }
            else if (group == "Request")
            {
                var dt = data.SelectDistinct("PhongBan", "Id_PhongBan,TenPB");
                return ToCheckboxList(new ChecklistModel("PhongBan", dt.ToSelectList("Id_PhongBan", "TenPB")));
            }
            else if (group == "PhongBan")
            {
                var dt = data.SelectDistinct("NguoiDung", "id_User,sID_User");
                return ToCheckboxList(new ChecklistModel("NguoiDung", dt.ToSelectList("id_User", "sID_User")));
            }
            else if (group == "User")
            {
                var dt = data.SelectDistinct("Donvi", "Id_DonVi,TenDonVi");
                return ToCheckboxList(new ChecklistModel("DonVi", dt.ToSelectList("Id_DonVi", "TenDonVi")));
            }
            else if (group == "DonVi")
            {
                var dt = data.SelectDistinct("Lan", "iLan,sLan");
                return ToCheckboxList(new ChecklistModel("Lan", dt.ToSelectList("iLan", "sLan")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region private methods

        private DTKTLockListViewModel getListViewModel(int nam)
        {
            var donvis = PhienLamViec.DonViList.ToList();
            var items = new List<DTKTLockViewModel>();
            var list = _dtktService.GetAllLock(nam);
            donvis.ForEach(x =>
            {
                DTKTLockViewModel item = null;
                var entity = list.FirstOrDefault(e => e.Id_DonVi == x.Key);
                if (entity == null)
                {
                    item = new DTKTLockViewModel()
                    {
                        Id_DonVi = x.Key,
                        TenDonVi = x.Value,
                    };
                }
                else
                {
                    item = entity.MapTo<DTKTLockViewModel>();
                }
                item.TenDonVi = x.Value;
                items.Add(item);
            });

            return new DTKTLockListViewModel() { Items = items };
        }
        private DTKTLockViewModel getLockViewModel(int nam, string id_donvi)
        {
            DTKTLockViewModel vm = null;
            var entity = _dtktService.GetLockById(nam, id_donvi);
            vm = entity == null
                    ? vm = new DTKTLockViewModel()
                    {
                        Id_DonVi = id_donvi,
                    }
                    : entity.MapTo<DTKTLockViewModel>();

            vm.TenDonVi = $"{id_donvi} - {_ngansachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen}";

            return vm;
        }
        private DTKTLockIdViewModel getLockIdViewModel()
        {
            var lockList = new Dictionary<string, string>
            {
                { "0", "Mở chứng từ"},
                { "1", "Khóa chứng từ" },
            };
            var vm = new DTKTLockIdViewModel
            {
                LockList = lockList.ToSelectList("-1", "----Chọn mở hay khóa----")
            };


            return vm;
        }
        private DataTable getTable_Cache(
            int nam,
            int id_lock,
            string group = null,
            string iloais = null,
            string ireqs = null,
            string phongbans = null,
            string users = null,
            string donvis = null)
        {
            //#if DEBUG
            //return getTable(id_chungtu, filters, type);
            //#endif

            var cachekey = getCacheKey(nam.ToString() + id_lock.ToString() + group + iloais + ireqs + phongbans + users + donvis);
            return CacheService.Default.CachePerRequest(cachekey,
                () => getTable(nam, id_lock, group, iloais, ireqs, phongbans, users, donvis),
                CacheTimes.OneMinute);
        }

        private DataTable getTable(
            int nam,
            int id_lock,
            string group = null,
            string iloais = null,
            string ireqs = null,
            string phongbans = null,
            string users = null,
            string donvis = null)
        {
            return _dtktService.GetChungTu_Lock(nam, id_lock, iloais, ireqs, phongbans, users, donvis);
        }

        private string getCacheKey(string cachename)
        {
            var cacheKey = $"{Username}_dtkt_lock_{cachename}";
            return cacheKey;
        }
        #endregion
    }
}
