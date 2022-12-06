using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper.Extensions;
using Dapper;
using DapperExtensions;
using Newtonsoft.Json;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QuyetToan.Models;
using VIETTEL.Areas.z.Models;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.z.Controllers
{
    public class QT_ChungTu_Result
    {
        public IList<QTA_ChungTuChiTiet> Rows { get; set; }
        public IList<QTA_ChungTuChiTiet> Rows_Error { get; set; }

        public decimal Sum { get; set; }

        public QTA_ChungTu ChungTu { get; set; }
    }


    public abstract class ChungTuController : AppController
    {
        // GET: z/ChungTu
        public ActionResult Index()
        {
            return View();
        }


        public virtual ActionResult Excel(string id_chungtu)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public abstract ActionResult Excel(ChungTuViewModel_QuyetToan vm);
    }

    public class QTController : ChungTuController
    {
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;


        public override ActionResult Excel(string id_chungtu)
        {
            var vm = _quyetToanService.GetChungTu(Guid.Parse(id_chungtu)).MapTo<ChungTuViewModel_QuyetToan>();
            vm.sTenDonVi = NganSachService.Default.GetDonVi(PhienLamViec.iNamLamViec, vm.iID_MaDonVi)?.sMoTa;
            vm.FileList = getFileList(PhienLamViec.iID_MaPhongBan, vm.iID_MaDonVi,
                zHelper.GetKey(PhienLamViec.NamLamViec, (int)zType.QuyetToan, vm.iThang_Quy, 1)).ToSelectList();

            return View(vm);
        }


        public override ActionResult Excel(ChungTuViewModel_QuyetToan vm)
        {
            if ((vm.FileUpload == null || vm.FileUpload.ContentLength == 0) && vm.File_Data.IsEmpty())
                return RedirectToAction("Excel", new { id_chungtu = vm.iID_MaChungTu });

            QT_ChungTu_Result result = null;
            var is_excel = vm.FileUpload != null;
            if (is_excel)
            {
                var data = getBytes(vm.FileUpload);
                var cach_key = getFile_CacheKey(vm.FileUpload.FileName, data);
                result = CacheService.Default.CachePerRequest(cach_key, () =>
                {
                    var dt = ExcelHelpers.LoadExcelDataTable(data);
                    var r = excel_result(vm.iID_MaChungTu, dt);
                    return r;
                });
            }
            else
            {
                var file_path = Path.Combine(zHelper.GetFolderSync(PhienLamViec.NamLamViec), vm.File_List);
                var entity = JsonConvert.DeserializeObject<Z_ChungTu>(System.IO.File.ReadAllText(file_path, Encoding.UTF8));

                var file = entity.Z_File.First(x => x.Id.ToString() == vm.File_Data);
                var data = file.FileData;
                var cach_key = getFile_CacheKey(file.FileName, data);
                result = CacheService.Default.CachePerRequest(cach_key, () =>
                {
                    var dt = ExcelHelpers.LoadExcelDataTable(data);
                    var r = excel_result(vm.iID_MaChungTu, dt);
                    return r;
                });
            }



            //excel_import(vm.iID_MaChungTu, dt);
            excel_import(vm.iID_MaChungTu, result.Rows);


            return RedirectToAction("Index", "QuyetToan_ChungTuChiTiet", new { vm.iID_MaChungTu, area = "" });
        }


        private void excel_import(Guid id_chungtu, IEnumerable<QTA_ChungTuChiTiet> rows)
        {
            var entity = _quyetToanService.GetChungTu(id_chungtu);
            #region insert to db


            // can kiem tra mlns xem co bi sai hay ko truoc khi day
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                try
                {
                    // xoa tat ca dong thuoc chung tu dang co
                    var sql = "delete QTA_ChungTuChiTiet where iID_MaChungTu=@iID_MaChungTu";
                    conn.Execute(sql, new { entity.iID_MaChungTu }, trans);

                    foreach (var r in rows)
                    {
                        conn.Insert(r, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    throw (ex);
                }
            }

            #endregion
        }

        private void excel_import(Guid id_chungtu, DataTable dt)
        {
            var entity = _quyetToanService.GetChungTu(id_chungtu);

            var items = dt.AsEnumerable().Where(x => x.Field<string>(2) == "n");

            var rows = new List<QTA_ChungTuChiTiet>();
            foreach (DataRow r in items)
            {
                var xauNoiMa = r.Field<string>(0);
                if (xauNoiMa.IsEmpty() || xauNoiMa.Length < 10) continue;

                var mlns = xauNoiMa.ToList("-");
                var e = new QTA_ChungTuChiTiet
                {
                    sXauNoiMa = xauNoiMa,
                    sLNS = mlns[0],
                    sL = mlns[1],
                    sK = mlns[2],
                    sM = mlns[3],
                    sTM = mlns[4],
                    sTTM = mlns[5],
                    sNG = mlns[6],
                    sTNG = mlns.Count > 7 ? mlns[7] : string.Empty,

                    //rNgay = r.Field<int>(10),
                    //rSoNguoi = r.Field<int>(11),
                    rTuChi = Convert.ToDecimal(r.Field<string>(15)),


                    iID_MaChungTu = entity.iID_MaChungTu,
                    iID_MaDonVi = entity.iID_MaDonVi,
                    sTenDonVi = entity.iID_MaDonVi,
                    iID_MaPhongBan = entity.iID_MaPhongBan,
                    iID_MaTrangThaiDuyet = entity.iID_MaTrangThaiDuyet,

                    iThang_Quy = entity.iThang_Quy,

                    iTrangThai = 1,
                    iNamLamViec = entity.iNamLamViec,
                    iID_MaNamNganSach = entity.iID_MaNamNganSach,
                    iID_MaNguonNganSach = entity.iID_MaNguonNganSach,
                    bChiNganSach = true,

                    dNgayTao = DateTime.Now,
                    dNgaySua = DateTime.Now,
                    sID_MaNguoiDungTao = Username,
                    sID_MaNguoiDungSua = Username,
                };

                e.rDonViDeNghi = e.rTuChi;
                rows.Add(e);
            }

            #region insert to db

            var mlns_list = NganSachService.Default.MLNS_GetAll(PhienLamViec.iNamLamViec)
                .Where(x => x.sNG.IsNotEmpty() && (entity.sDSLNS.IsEmpty() || entity.sDSLNS.Contains(x.sLNS)))
                .ToList();
            // can kiem tra mlns xem co bi sai hay ko truoc khi day
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                try
                {
                    // xoa tat ca dong thuoc chung tu dang co
                    var sql = "delete QTA_ChungTuChiTiet where iID_MaChungTu=@iID_MaChungTu";
                    conn.Execute(sql, new { entity.iID_MaChungTu }, trans);

                    foreach (var r in rows)
                    {
                        var mlns = mlns_list.FirstOrDefault(x => x.sXauNoiMa == r.sXauNoiMa);
                        if (mlns == null) continue;


                        // cap nhat mlns
                        r.iID_MaMucLucNganSach = mlns.iID_MaMucLucNganSach;
                        r.iID_MaMucLucNganSach_Cha = mlns.iID_MaMucLucNganSach_Cha;
                        r.sMoTa = mlns.sMoTa;

                        conn.Insert(r, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    throw (ex);
                }
            }

            #endregion


        }


        private QT_ChungTu_Result excel_result(Guid id_chungtu, DataTable dt)
        {
            var entity = _quyetToanService.GetChungTu(id_chungtu);
            var items = dt.AsEnumerable().Where(x => x.Field<string>(2) == "n");

            var rows = new List<QTA_ChungTuChiTiet>();
            foreach (DataRow r in items)
            {
                var xauNoiMa = r.Field<string>(0);
                if (xauNoiMa.IsEmpty() || xauNoiMa.Length < 10 || Convert.ToDecimal(r.Field<string>(15)) == 0) continue;

                var mlns = xauNoiMa.ToList("-");
                var e = new QTA_ChungTuChiTiet
                {
                    sXauNoiMa = xauNoiMa,
                    sLNS = mlns[0],
                    sL = mlns[1],
                    sK = mlns[2],
                    sM = mlns[3],
                    sTM = mlns[4],
                    sTTM = mlns[5],
                    sNG = mlns[6],
                    sTNG = mlns.Count > 7 ? mlns[7] : string.Empty,
                    sMoTa = r.Field<string>(9),

                    //rNgay = r.Field<int>(10),
                    //rSoNguoi = r.Field<int>(11),
                    rTuChi = Convert.ToDecimal(r.Field<string>(15)),


                    iID_MaChungTu = entity.iID_MaChungTu,
                    iID_MaDonVi = entity.iID_MaDonVi,
                    sTenDonVi = entity.iID_MaDonVi,
                    iID_MaPhongBan = entity.iID_MaPhongBan,
                    iID_MaTrangThaiDuyet = entity.iID_MaTrangThaiDuyet,

                    iThang_Quy = entity.iThang_Quy,

                    iTrangThai = 1,
                    iNamLamViec = entity.iNamLamViec,
                    iID_MaNamNganSach = entity.iID_MaNamNganSach,
                    iID_MaNguonNganSach = entity.iID_MaNguonNganSach,
                    bChiNganSach = true,

                    dNgayTao = DateTime.Now,
                    dNgaySua = DateTime.Now,
                    sID_MaNguoiDungTao = Username,
                    sID_MaNguoiDungSua = Username,
                };

                e.rDonViDeNghi = e.rTuChi;

                rows.Add(e);
            }

            #region insert to db

            var mlns_list = NganSachService.Default.MLNS_GetAll(PhienLamViec.iNamLamViec)
                .Where(x => x.sNG.IsNotEmpty() && (entity.sDSLNS.IsEmpty() || entity.sDSLNS.Contains(x.sLNS)))
                .ToList();

            // can kiem tra mlns xem co bi sai hay ko truoc khi day

            var Rows = new List<QTA_ChungTuChiTiet>();
            var Rows_Error = new List<QTA_ChungTuChiTiet>();
            foreach (var r in rows)
            {
                var mlns = mlns_list.FirstOrDefault(x => x.sXauNoiMa == r.sXauNoiMa);
                if (mlns == null)
                {
                    Rows_Error.Add(r);
                }
                else
                {
                    // cap nhat mlns
                    r.iID_MaMucLucNganSach = mlns.iID_MaMucLucNganSach;
                    r.iID_MaMucLucNganSach_Cha = mlns.iID_MaMucLucNganSach_Cha;
                    r.sMoTa = mlns.sMoTa;

                    Rows.Add(r);
                }
            }

            #endregion

            var result = new QT_ChungTu_Result()
            {
                ChungTu = entity,
                Rows = Rows,
                Rows_Error = Rows_Error,
                Sum = Rows.Sum(r => r.rTuChi)
            };

            return result;
        }


        /// <summary>
        /// Kiểm tra dữ liệu trước khi nhận vào
        /// </summary>
        /// <param name="id_chungtu"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult excel_check(Guid id_chungtu, HttpPostedFileBase file)
        {
            var file_data = getBytes(file);
            var cache_key = getFile_CacheKey(file.FileName, file_data);


            var dt = ExcelHelpers.LoadExcelDataTable(file_data);
            var result = CacheService.Default.CachePerRequest(cache_key, () => excel_result(id_chungtu, dt), 30);

            // kiem tra mlns bi sai hoac thieu
            var msg =
                $@"- Filename: {file.FileName}
- Tổng cộng quyết toán:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Sum.ToString("###,###")}</b>
- Số bản ghi hợp lệ:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Rows.Count}</b>
- Số bản ghi ko hợp lệ:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Rows_Error.Count}</b>
 {result.Rows_Error.Take(10).Select(x => $"&nbsp;&nbsp;&nbsp;+ {x.sXauNoiMa}({x.sMoTa.PadRight(50)}): <b>{x.rTuChi.ToString("###,##0")}</b>").Join(Environment.NewLine)}
<br>
";
            return Json(new
            {
                id_chungtu,
                success = true,
                msg = msg.ToList("\n").Join("<br>"),
                count = result.Rows.Count,
                count_error = result.Rows_Error.Count,
            }, JsonRequestBehavior.AllowGet);
        }



        public PartialViewResult excel_file_select(string file)
        {
            var file_path = Path.Combine(zHelper.GetFolderSync(PhienLamViec.NamLamViec), file);
            var entity = JsonConvert.DeserializeObject<Z_ChungTu>(System.IO.File.ReadAllText(file_path, Encoding.UTF8));
            return PartialView(entity);
        }

        /// <summary>
        /// Kiểm tra dữ liệu trước khi nhận vào
        /// </summary>
        /// <param name="id_chungtu"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult excel_check_file(Guid id_chungtu, string file_name, string z_file)
        {
            var file_path = Path.Combine(zHelper.GetFolderSync(PhienLamViec.NamLamViec), file_name);
            var entity = JsonConvert.DeserializeObject<Z_ChungTu>(System.IO.File.ReadAllText(file_path, Encoding.UTF8));

            var file = entity.Z_File.First(x => x.Id.ToString() == z_file);
            //var cache_key = getFile_CacheKey(file_path + z_file, file_data);
            var cache_key = getFile_CacheKey(file.FileName, file.FileData);

            var dt = ExcelHelpers.LoadExcelDataTable(file.FileData);
            var result = CacheService.Default.CachePerRequest(cache_key, () => excel_result(id_chungtu, dt), 30);

            // kiem tra mlns bi sai hoac thieu
            var msg =
                $@"- Filename: {file_name}
- Tổng cộng quyết toán:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Sum.ToString("###,###")}</b>
- Số bản ghi hợp lệ:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Rows.Count}</b>
- Số bản ghi ko hợp lệ:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{result.Rows_Error.Count}</b>
 {result.Rows_Error.Take(10).Select(x => $"&nbsp;&nbsp;&nbsp;+ {x.sXauNoiMa}({x.sMoTa.PadRight(50)}): <b>{x.rTuChi.ToString("###,##0")}</b>").Join(Environment.NewLine)}
<br>
";
            return Json(new
            {
                success = true,
                msg = msg.ToList("\r\n").Join("<br>"),
                count = result.Rows.Count,
                count_error = result.Rows_Error.Count,
            }, JsonRequestBehavior.AllowGet);
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private string getFile_CacheKey(string name, Byte[] data)
        {
            var hash = string.Empty;
            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(data))
                {
                    hash = Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }

            var cache_key = $"{this.ControllerName()}___{name}___hash_{hash}";
            return cache_key;
        }

        private Dictionary<string, string> getFileList(string iID_MaPhongBan, string iID_MaDonVi, string key)
        {
            var folder = zHelper.GetFolderSync(PhienLamViec.NamLamViec);
            var files = Directory.GetFiles(folder);

            var r = new Dictionary<string, string>()
            {
                {"","---" }
            };
            foreach (var file in files)
            {
                var file_info = new FileInfo(file);
                var entity = zHelper.GetChungTu_FromFileName(file_info.Name);
                if (entity.Id == Guid.Empty || !file_info.Name.Contains(key) || entity.Id_DonVi != iID_MaDonVi || entity.Id_PhongBan != iID_MaPhongBan) continue;

                r.Add(file_info.Name, file_info.Name);

            }

            return r;
        }


    }



}