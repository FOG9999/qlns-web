using DapperExtensions;
using DomainModel;
using DomainModel.Abstract;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QuyetToan.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QuyetToan.Controllers
{
    [Authorize]
    public class ChungTuChiTietController : AppController
    {
        // GET: QuyetToan/ChungTuChiTiet
        public ActionResult Index(string id)
        {
            var vm = new ChungTuChiTietViewModel
            {
                Id = id,
                Filter = 1,
                FilterOptions = QuyetToan_ChungTu_SheetTableFilterType.Items,
            };
            return View(vm);
        }

        public ActionResult SheetFrame(string id, int option = 0)
        {
            var sheet = new QuyetToan_ChungTu_SheetTable(id, option, Username, Request.QueryString);
            var vm = new ChungTuChiTietViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    option: option,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "ChungTuChiTiet", new { area = "QuyetToan", id, option })
                    ),

                Id = id,
                Filter = option,
            };

            return View("_sheetFrame", vm.Sheet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var id = vm.Id;
            var option = vm.Option;

            var row = vm.Rows.ToList();
            if (row.Count > 0)
            {
                var qtService = QuyetToanService.Default;
                var nsService = NganSachService.Default;
                var conn = ConnectionFactory.Default.GetConnection();

                var columns = new QuyetToan_ChungTu_SheetTable().Columns.Where(x => !x.IsReadonly);
                row.ForEach(r =>
                {
                    var idChungTu = r.Id.Split('_')[0];
                    var idMLNS = r.Id.Split('_')[1];
                    if (r.IsDeleted)
                    {
                        if (!string.IsNullOrWhiteSpace(idChungTu))
                        {
                            qtService.DeleteChungTuChiTiet(idChungTu);
                        }
                    }
                    else
                    {
                        var chungtu = qtService.GetChungTu(Guid.Parse(vm.Id));
                        var mlns = nsService.GetMLNS(PhienLamViec.iNamLamViec, Guid.Parse(idMLNS));

                        var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                        // tao moi chung tu chi tiet
                        if (string.IsNullOrWhiteSpace(idChungTu))
                        {
                            #region new

                            var entity = new QTA_ChungTuChiTiet()
                            {
                                iID_MaChungTu = chungtu.iID_MaChungTu,
                                iID_MaPhongBan = chungtu.iID_MaPhongBan,
                                sTenPhongBan = chungtu.sTenPhongBan,
                                iID_MaDonVi = chungtu.iID_MaDonVi,
                                sTenDonVi = nsService.GetDonVi(PhienLamViec.iNamLamViec, chungtu.iID_MaDonVi).sTen,
                                iID_MaTrangThaiDuyet = chungtu.iID_MaTrangThaiDuyet,

                                iID_MaNamNganSach = chungtu.iID_MaNamNganSach,
                                iID_MaNguonNganSach = chungtu.iID_MaNguonNganSach,
                                iNamLamViec = chungtu.iNamLamViec,
                                iThang_Quy = chungtu.iThang_Quy,
                                bLoaiThang_Quy = chungtu.bLoaiThang_Quy,
                                bChiNganSach = chungtu.bChiNganSach,

                                //mlns
                                sLNS = mlns.sLNS,
                                sL = mlns.sL,
                                sK = mlns.sK,
                                sM = mlns.sM,
                                sTM = mlns.sTM,
                                sTTM = mlns.sTTM,
                                sNG = mlns.sNG,
                                sTNG = mlns.sTNG,
                                sXauNoiMa = mlns.sXauNoiMa,
                                sMoTa = mlns.sMoTa,
                                iID_MaMucLucNganSach = mlns.iID_MaMucLucNganSach,
                                iID_MaMucLucNganSach_Cha = mlns.iID_MaMucLucNganSach_Cha,


                                // common values
                                iTrangThai = 1,
                                dNgayTao = DateTime.Now,
                                sID_MaNguoiDungTao = Username,
                                sIPSua = Request.UserHostAddress,
                            };

                            entity.MapFrom(changes);

                            //conn.InsertIdentity(entity);
                            conn.Insert(entity);

                            #endregion
                        }
                        else
                        {
                            #region update

                            var entity = conn.Get<QTA_ChungTuChiTiet>(idChungTu);
                            entity.MapFrom(changes);

                            entity.iID_MaChungTuChiTiet = int.Parse(idChungTu);
                            conn.Update(entity);

                            #endregion
                        }
                    }

                });

                conn.Dispose();

            }

            // clear cache
            var cacheKey = $"quyetoan_{id}";
            CacheService.Default.ClearStartsWith(cacheKey);

            return RedirectToAction("SheetFrame", new { id, option });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save1(string id, int filter)
        {
            String TenBangChiTiet = "QTA_ChungTuChiTiet";

            String MaND = User.Identity.Name;
            String IPSua = Request.UserHostAddress;
            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauLaHangCha = Request.Form["idXauLaHangCha"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];


            String[] arrLaHangCha = idXauLaHangCha.Split(',');
            String[] arrMaHang = idXauMaCacHang.Split(',');
            String[] arrMaCot = idXauMaCacCot.Split(',');
            String[] arrHangDaXoa = idXauCacHangDaXoa.Split(',');
            String[] arrHangGiaTri = idXauGiaTriChiTiet.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);
            String[] arrHangThayDoi = idXauDuLieuThayDoi.Split(new string[] { BangDuLieu.DauCachHang }, StringSplitOptions.None);

            DataTable dtChungTu = QuyetToan_ChungTuModels.GetChungTu(id);
            String sTenDonVi = DonViModels.Get_TenDonVi(Convert.ToString(dtChungTu.Rows[0]["iID_MaDonVi"]), MaND);
            for (int i = 0; i < arrMaHang.Length; i++)
            {
                if (arrMaHang[i] != "")
                {
                    String maChungTuChiTiet = arrMaHang[i].Split('_')[0];
                    //Trường hợp delete.
                    if (arrHangDaXoa[i] == "1")
                    {

                        //Lưu các hàng đã xóa
                        if (maChungTuChiTiet != "")
                        {
                            //Dữ liệu đã có
                            var bang = new Bang(TenBangChiTiet);
                            bang.DuLieuMoi = false;
                            bang.GiaTriKhoa = maChungTuChiTiet;
                            bang.CmdParams.Parameters.AddWithValue("@iTrangThai", 0);
                            bang.MaNguoiDungSua = MaND;
                            bang.IPSua = Request.UserHostAddress; ;
                            bang.Save();
                        }
                    }
                    else
                    {
                        String[] arrGiaTri = arrHangGiaTri[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                        String[] arrThayDoi = arrHangThayDoi[i].Split(new string[] { BangDuLieu.DauCachO }, StringSplitOptions.None);
                        Boolean okCoThayDoi = false;
                        for (int j = 0; j < arrMaCot.Length; j++)
                        {
                            if (arrThayDoi[j] == "1")
                            {
                                okCoThayDoi = true;
                                break;
                            }
                        }
                        if (okCoThayDoi)
                        {
                            Bang bang = new Bang(TenBangChiTiet)
                            {
                                MaNguoiDungSua = User.Identity.Name,
                                IPSua = User.Identity.Name,
                                TruongKhoaKieuSo = true,
                            };
                            String MaChungTuChiTiet = arrMaHang[i].Split('_')[0];

                            if (MaChungTuChiTiet == "")
                            {
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", id);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", dtChungTu.Rows[0]["iID_MaPhongBan"]);
                                bang.CmdParams.Parameters.AddWithValue("@sTenPhongBan", dtChungTu.Rows[0]["sTenPhongBan"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", dtChungTu.Rows[0]["iID_MaTrangThaiDuyet"]);
                                bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", dtChungTu.Rows[0]["iNamLamViec"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", dtChungTu.Rows[0]["iID_MaNguonNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", dtChungTu.Rows[0]["iID_MaNamNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", dtChungTu.Rows[0]["bChiNganSach"]);
                                bang.CmdParams.Parameters.AddWithValue("@iThang_Quy", dtChungTu.Rows[0]["iThang_Quy"]);
                                bang.CmdParams.Parameters.AddWithValue("@bLoaiThang_Quy", dtChungTu.Rows[0]["bLoaiThang_Quy"]);
                                bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", dtChungTu.Rows[0]["iID_MaDonVi"]);
                                bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", sTenDonVi);
                                String iID_MaMucLucNganSach = arrMaHang[i].Split('_')[1];

                                DataTable dtMucLuc = MucLucNganSachModels.dt_ChiTietMucLucNganSach(iID_MaMucLucNganSach);
                                //Dien thong tin cua Muc luc ngan sach
                                NganSach_HamChungModels.ThemThongTinCuaMucLucNganSach(dtMucLuc.Rows[0], bang.CmdParams.Parameters);
                                //Xet rieng ngan sach thuong xuyen
                                dtMucLuc.Dispose();
                            }
                            else
                            {
                                bang.GiaTriKhoa = MaChungTuChiTiet;
                                bang.DuLieuMoi = false;
                            }

                            //Them tham so
                            for (int j = 0; j < arrMaCot.Length; j++)
                            {
                                if (arrThayDoi[j] == "1")
                                {
                                    if (arrMaCot[j].EndsWith("_ConLai") == false)
                                    {
                                        String Truong = "@" + arrMaCot[j];
                                        if (arrMaCot[j].StartsWith("b"))
                                        {
                                            //Nhap Kieu checkbox
                                            if (arrGiaTri[j] == "1")
                                            {
                                                bang.CmdParams.Parameters.AddWithValue(Truong, true);
                                            }
                                            else
                                            {
                                                bang.CmdParams.Parameters.AddWithValue(Truong, false);
                                            }
                                        }
                                        else if (arrMaCot[j].StartsWith("r") || arrMaCot[j].StartsWith("i"))
                                        {
                                            //Nhap Kieu so
                                            if (CommonFunction.IsNumeric(arrGiaTri[j]))
                                            {
                                                bang.CmdParams.Parameters.AddWithValue(Truong, Convert.ToDouble(arrGiaTri[j]));
                                            }
                                        }
                                        else
                                        {
                                            //Nhap kieu xau
                                            bang.CmdParams.Parameters.AddWithValue(Truong, arrGiaTri[j]);
                                        }
                                    }
                                }
                            }

                            bang.Save();
                        }
                    }
                }
            }

            dtChungTu.Dispose();
            string idAction = Request.Form["idAction"];
            if (idAction == "1")
            {
                return RedirectToAction("TuChoi", "QuyetToan_ChungTu", new { iID_MaChungTu = id });
            }
            else if (idAction == "2")
            {
                return RedirectToAction("TrinhDuyet", "QuyetToan_ChungTu", new { iID_MaChungTu = id });
            }
            ViewData["LoadLai"] = "";
            ViewData["MaLoai"] = filter;
            //return View(sViewPath + "QuyetToan_ChungTuChiTiet_Index_DanhSach_Frame.aspx");

            return RedirectToAction("SheetFrame", new { id, filter });
        }

        public ActionResult BaocaoThongTri(string id)
        {
            var dt = DanhMucModels.DT_DanhMuc_All("TenThongTriQuyetToan");
            var iID_MaDanhMuc = "";
            var sGhiChu = Report_Controllers.ThuNop.rptQuyetToan_ThongTri_ChungTuController.DsGhiChu(id);
            string[] arrGhiChu = sGhiChu.Split(',');

            if (arrGhiChu.Count() > 0)
            {
                sGhiChu = arrGhiChu[0];

                if (arrGhiChu.Count() > 1)
                {
                    iID_MaDanhMuc = arrGhiChu[1];
                }
            }

            if (string.IsNullOrEmpty(iID_MaDanhMuc) && dt.Rows.Count > 0)
            {
                iID_MaDanhMuc = dt.Rows[0]["iID_MaDanhMuc"].ToString();
            }

            var vm = new BaocaoThongTriViewModel()
            {
                iID_MaChungTu = id,
                iID_MaDanhMuc = iID_MaDanhMuc,
                sGhiChu = sGhiChu,

                Items = dt
                .AsEnumerable()
                .Select(x => new SelectListItem()
                {
                    Value = x.Field<string>("iID_MaDanhMuc"),
                    Text = x.Field<string>("sTen")
                })
            };

            return PartialView("_baocaoThongTri", vm);
        }

        #region private methods


        #endregion
    }
}
