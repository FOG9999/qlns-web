using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class QLPhanNguonBTCTheoNDChiBQPController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        // GET: QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP
        public ActionResult Index()
        {
            QLPhanNguonPagingModel data = new QLPhanNguonPagingModel();
            data._paging.CurrentPage = 1;
            data.lstData = _qLNguonNSService.GetAllNNSPhanNguonBTCTheoNDChiBQP(ref data._paging, string.Empty, string.Empty);
            return View(data);
        }

        [HttpPost]
        public ActionResult TimKiemNNSPhanNguon(PagingInfo _paging, string soChungTu, string noiDung)
        {
            QLPhanNguonPagingModel data = new QLPhanNguonPagingModel();
            data._paging = _paging;
            data.lstData = _qLNguonNSService.GetAllNNSPhanNguonBTCTheoNDChiBQP(ref data._paging, soChungTu, noiDung);

            return PartialView("_partialListNNSPhanNguon", data);
        }

        public ActionResult Update(Guid? id)
        {
            NNS_PhanNguon data = new NNS_PhanNguon();
            if (id.HasValue)
            {
                data = _qLNguonNSService.GetNNSPhanNguonByID(id.Value);
            }
            else
            {
                int iIndexMax = _qLNguonNSService.GetMaxiIndexNNSPhanNguon();
                string sSochungTuNew = formatSoChungTu(iIndexMax + 1);
                data.sSoChungTu = sSochungTuNew;
            }
            return View(data);
        }

        public ActionResult ViewDetail(Guid? id)
        {
            NNS_PhanNguon data = new NNS_PhanNguon();
            if (id.HasValue)
            {
                data = _qLNguonNSService.GetNNSPhanNguonByID(id.Value);
            }
            return PartialView("_partialViewDetail", data);
        }

        [HttpPost]
        public bool NNSPhanNguonSave(NNS_PhanNguon data)
        {
            String sIPSua = Request.UserHostAddress;
            if (data.iID_PhanNguon == new Guid())
            {
                if (!_qLNguonNSService.InsertNNSPhanNguon(data.sNoiDung, data.sSoChungTu, data.dNgayChungTu, data.sSoQuyetDinh, data.dNgayQuyetDinh, Username)) return false;
            }
            else
            {
                if (!_qLNguonNSService.UpdateNNSPhanNguon(data.iID_PhanNguon, data.sNoiDung, data.sSoChungTu, data.dNgayChungTu, data.sSoQuyetDinh, data.dNgayQuyetDinh, sIPSua, Username)) return false;
            }
            return true;
        }

        [HttpPost]
        public bool NNSPhanNguonDelete(Guid id)
        {
            if (!_qLNguonNSService.deleteNNSPhanNguon(id)) return false;
            if (!_qLNguonNSService.deleteNNSPhanNguonNDChi(id)) return false;
            return true;
        }

        public ActionResult ViewDMNguonBTCCap(Guid? id)
        {
            QLPhanNguonNDChiPagingModel data = new QLPhanNguonNDChiPagingModel();
            data._paging.CurrentPage = 1;
            data.iIdPhanNguon = id;
            NNS_PhanNguon nnsPhanNguon = _qLNguonNSService.GetNNSPhanNguonByID(id.Value);
            data.lstData = _qLNguonNSService.GetAllDMNguonBTCCap(ref data._paging, nnsPhanNguon.dNgayChungTu, Username, id);
            return View(data);
        }

        public ActionResult TimKiemDMNguonBTCCap(Guid? iIdPhanNguon, int iCurrentPage)
        {
            QLPhanNguonNDChiPagingModel data = new QLPhanNguonNDChiPagingModel();
            data._paging.CurrentPage = iCurrentPage;
            data.iIdPhanNguon = iIdPhanNguon;
            NNS_PhanNguon nnsPhanNguon = _qLNguonNSService.GetNNSPhanNguonByID(iIdPhanNguon.Value);
            data.lstData = _qLNguonNSService.GetAllDMNguonBTCCap(ref data._paging, nnsPhanNguon.dNgayChungTu, Username, iIdPhanNguon);
            return PartialView("_partialListDMNguonBTCCap", data);
        }

        public ActionResult ViewPhanNguonBTCTheoNDChi(Guid iIdNguon, Guid iIdPhanNguon, decimal rSoTienBTCCap, decimal rSoTienConLai)
        {
            IEnumerable<NNSDMNoiDungChiViewModel> lstDMNoiDungChi = _qLNguonNSService.GetAllDMNoiDungChiBQP(iIdNguon, iIdPhanNguon, Username);
            decimal? rSoTienCoTheChi = rSoTienConLai;
            foreach (NNSDMNoiDungChiViewModel item in lstDMNoiDungChi)
            {
                rSoTienCoTheChi += item.SoTien;
            }

            NNSPhanNguonBTCTheoNDChiViewModel nnsPhanNguonBTCTheoNDChiViewModel = new NNSPhanNguonBTCTheoNDChiViewModel();
            nnsPhanNguonBTCTheoNDChiViewModel.lstDMNoiDungChi = lstDMNoiDungChi;
            nnsPhanNguonBTCTheoNDChiViewModel.rSoTienBTCCap = rSoTienBTCCap;
            nnsPhanNguonBTCTheoNDChiViewModel.rSoTienConLai = rSoTienConLai;
            nnsPhanNguonBTCTheoNDChiViewModel.rSoTienCoTheChi = rSoTienCoTheChi;
            nnsPhanNguonBTCTheoNDChiViewModel.iID_Nguon = iIdNguon;
            nnsPhanNguonBTCTheoNDChiViewModel.iIdPhanNguon = iIdPhanNguon;
            return View(nnsPhanNguonBTCTheoNDChiViewModel);
        }

        [HttpPost]
        public bool NNSPhanNguonNDChiChiTietSave(IEnumerable<NNSPhanNguonNDChiTempViewModel> data)
        {
            //if (!_qLNguonNSService.SaveNNSPhanNguonNDChi(data, data, Username)) return false;
            return true;
        }

        public string formatSoChungTu(int iIndex)
        {
            if (iIndex < 10)
            {
                return "PN-000" + iIndex;
            }
            else if (iIndex < 100)
            {
                return "PN-00" + iIndex;
            }
            else if (iIndex < 1000)
            {
                return "PN-0" + iIndex;
            }
            return "PN-" + iIndex;
        }

        #region modal phan nguon btc
        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NNS_PhanNguon data = new NNS_PhanNguon();
            if (id.HasValue)
            {
                data = _qLNguonNSService.GetNNSPhanNguonByID(id.Value);
            }
            else
            {
                int iIndexMax = _qLNguonNSService.GetMaxiIndexNNSPhanNguon();
                string sSochungTuNew = formatSoChungTu(iIndexMax + 1);
                data.sSoChungTu = sSochungTuNew;
            }
            return PartialView("_partialUpdate", data);
        }
        #endregion

        #region data grid moi
        public ActionResult ViewPhanNguonBTCTheoNDChi1(Guid iIdNguon, Guid iIdPhanNguon, decimal rSoTienBTCCap, decimal rSoTienConLai, string sNoiDung)
        {
            Decimal rSoTienCoTheChi = _qLNguonNSService.getSoTienCoTheChi(iIdNguon, iIdPhanNguon, rSoTienConLai, Username, "");
            var vm = new QLPhanNguonNDChiViewModel
            {
                iID_Nguon = iIdNguon,
                iIdPhanNguon = iIdPhanNguon,
                rSoTienBTCCap = rSoTienBTCCap,
                rSoTienConLai = rSoTienConLai,
                rSoTienCoTheChi = rSoTienCoTheChi,
                sNoiDung = sNoiDung
            };

            return View(vm);
        }

        public ActionResult SheetFrame(string id, string filters = null)
        {
            var filtersJson = filters == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filters);
            var arrParam = id.Split('_');
            Guid iIdNguon = Guid.Parse(arrParam[0]);
            Guid iIdPhanNguon = Guid.Parse(arrParam[1]);
            decimal rSoTienConLai = Decimal.Parse(arrParam[2]); ;
            decimal rSoTienCoTheChi = Decimal.Parse(arrParam[3]);
            if (filtersJson.ContainsKey("sTenNoiDungChi"))
            {
                string sTenNoiDungChi = filtersJson["sTenNoiDungChi"];
                Decimal rSoTienCoTheChiNew = _qLNguonNSService.getSoTienCoTheChi(iIdNguon, iIdPhanNguon, rSoTienConLai, Username, sTenNoiDungChi);
                rSoTienCoTheChi = rSoTienCoTheChiNew;
                id = iIdNguon.ToString() + "_" + iIdPhanNguon.ToString() + "_" + rSoTienConLai.ToString() + "_" + rSoTienCoTheChi.ToString();
            }

            var sheet = new QLPhanNguon_NDChi_SheetTable(iIdNguon, iIdPhanNguon, rSoTienConLai, Username, filtersJson);
            var vm = new QLPhanNguonNDChiViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "QLPhanNguonBTCTheoNDChiBQP", new { area = "QLNguonNganSach" }),
                    urlGet: Url.Action("SheetFrame", "QLPhanNguonBTCTheoNDChiBQP", new { area = "QLNguonNganSach" })
                    ),
                rSoTienCoTheChi = rSoTienCoTheChi,
            };

            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            List<NNSPhanNguonNDChiTempViewModel> dataSaveNew = new List<NNSPhanNguonNDChiTempViewModel>();
            List<NNSPhanNguonNDChiTempViewModel> dataSaveEdit = new List<NNSPhanNguonNDChiTempViewModel>();
            var arrParam = (vm.Id).Split('_');
            Guid iIdNguon = Guid.Parse(arrParam[0]);
            Guid iIdPhanNguon = Guid.Parse(arrParam[1]);
            decimal rSoTienCoTheChi = Decimal.Parse(arrParam[2]);
            decimal rSoTienConLai = Decimal.Parse(arrParam[3]);
            var rowsParent = vm.Rows.Where(x => x.IsParent).ToList();
            if (rowsParent.Count > 0)
            {
                var rowCount = vm.Rows.ToList()[0];
                rSoTienConLai = Decimal.Parse(rowCount.Columns["SoTien"]);
                string idNew = iIdNguon.ToString() + "_" + iIdPhanNguon.ToString() + "_" + rSoTienConLai.ToString() + "_" + rSoTienCoTheChi.ToString();
                vm.Id = idNew;
            }

            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                rows.ForEach(r =>
                {
                    Guid iID_NoiDungChi = Guid.Parse(r.Id);
                    IEnumerable<NNSPhanNguonNDChiTempViewModel> nnsPhanNguonNDChis = _qLNguonNSService.getNNSPhanNguonNDChiByIds(iIdNguon, iIdPhanNguon, iID_NoiDungChi, Username);
                    if(nnsPhanNguonNDChis.Count() > 0)
                    {
                        NNSPhanNguonNDChiTempViewModel dataItemEdit = nnsPhanNguonNDChis.First();
                        if (r.Columns.ContainsKey("GhiChu"))
                        {
                            dataItemEdit.GhiChu = r.Columns["GhiChu"];
                        }
                        if (r.Columns.ContainsKey("SoTien"))
                        {
                            dataItemEdit.SoTien = Decimal.Parse(r.Columns["SoTien"]);
                        }
                        dataSaveEdit.Add(dataItemEdit);
                    }
                    else
                    {
                        NNSPhanNguonNDChiTempViewModel dataItem = new NNSPhanNguonNDChiTempViewModel();
                        var columns = new QLPhanNguon_NDChi_SheetTable().Columns;
                        string GhiChu = "";
                        decimal SoTien = 0;
                        if (r.Columns.ContainsKey("GhiChu"))
                        {
                            GhiChu = r.Columns["GhiChu"];
                        }
                        dataItem.iID_NoiDungChi = Guid.Parse(r.Id);
                        dataItem.iID_Nguon = iIdNguon;
                        dataItem.iID_PhanNguon = iIdPhanNguon;
                        if (r.Columns.ContainsKey("SoTien"))
                        {
                            SoTien = Decimal.Parse(r.Columns["SoTien"]);
                        }
                        dataItem.SoTien = SoTien;
                        dataItem.GhiChu = GhiChu;
                        dataSaveNew.Add(dataItem);
                    }
                    
                });
            }

            IEnumerable<NNSPhanNguonNDChiTempViewModel> dataIenumNew = dataSaveNew.AsEnumerable();
            IEnumerable<NNSPhanNguonNDChiTempViewModel> dataIenumEdit = dataSaveEdit.AsEnumerable();
            String sIPSua = Request.UserHostAddress;
            _qLNguonNSService.SaveNNSPhanNguonNDChi(dataIenumNew, dataIenumEdit, Username, sIPSua);
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }
        #endregion


    }
}