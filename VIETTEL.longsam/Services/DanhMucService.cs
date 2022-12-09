using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using VIETTEL.Helpers;
using System.Linq;
using System.Text;
using DapperExtensions;
using Viettel.Models.QLVonDauTu;
using System.ComponentModel;
using Viettel.Models.QLNH;
using System.Reflection;
using System.Diagnostics;

namespace Viettel.Services
{
    public interface IDanhMucService : IServiceBase
    {
        #region Danh mục Nguồn ngân sách - Loại dự toán
        /// <summary>
        /// Get DM_LoaiDuToan by ID
        /// </summary>
        /// <param name="iId">iID_LoaiDuToan</param>
        /// <returns></returns>
        DM_LoaiDuToan GetDMLoaiDuToanByID(Guid iId);

        /// <summary>
        /// Get all DM_LoaiDuToan by condition
        /// </summary>
        /// <param name="code">sMaLoaiDuToan</param>
        /// <param name="name">sTenLoaiDuToan</param>
        /// <returns></returns>
        IEnumerable<DM_LoaiDuToan> GetAllDMLoaiDuToan(int iNamLamViec, string sCode = "", string sName = "");

        /// <summary>
        /// Get Paging all DM_LoaiDuToan by condition
        /// </summary>
        /// <param name="code">sMaLoaiDuToan</param>
        /// <param name="name">sTenLoaiDuToan</param>
        /// <returns></returns>
        IEnumerable<DM_LoaiDuToan> GetAllDMLoaiDuToanPaging(ref PagingInfo _paging, int iNamLamViec, string sCode = "", string sName = "");

        /// <summary>
        /// Insert DM_LoaiDuToan
        /// </summary>
        /// <param name="sCode">sMaLoaiDuToan</param>
        /// <param name="sName">sTenLoaiDuToan</param>
        /// <param name="sNote">sGhiChu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        /// <returns></returns>
        bool InsertDMLoaiDuToan(string sCode, string sName, string sNote, string sUserLogin, int iNamLamViec);

        /// <summary>
        /// Update DM_NoiDungChiBQP
        /// </summary>
        /// <param name="iId">iID_LoaiDuToan</param>
        /// <param name="sCode">sMaLoaiDuToan</param>
        /// <param name="sName">sTenLoaiDuToan</param>
        /// <param name="sNote">sGhiChu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungSua</param>
        /// <param name="bPublic">bPublic</param>
        bool UpdateDMLoaiDuToan(Guid iId, string sCode, string sName, string sNote, string sUserLogin, bool bPublic = true);

        /// <summary>
        /// CheckExistMaDMLoaiDuAn
        /// </summary>
        /// <param name="iId">iID_LoaiDuToan</param>
        /// <param name="sCode">sMaLoaiDuToan</param>
        bool CheckExistMaDMLoaiDuToan(Guid? iId, string sCode, int iNamLamViec);

        List<DM_LoaiDuToan> GetListDMLoaiDuToan(string sUserName);

        #endregion

        #region Danh mục Nguồn ngân sách - Nội dung chi
        /// <summary>
        /// Get DM_NoiDungChi by ID
        /// </summary>
        /// <param name="iId">iID_NoiDungChi</param>
        /// <returns></returns>
        DM_NoiDungChi GetDMNoiDungChiByID(Guid iId);

        /// <summary>
        /// Check tồn tại mã nội dung chi
        /// </summary>
        /// <param name="sMaLoaiDuToan">sMaNoiDungChi</param>
        /// <param name="iId">iID_NoiDungChi</param>
        /// <returns></returns>
        bool CheckExistMaNoiDungchi(string sMaNoiDungChi, int iNamLamViec, Guid? iId = null);

        /// <summary>
        /// Get all DM_NoiDungChi by condition
        /// </summary>
        /// <returns></returns>
        IEnumerable<DM_NoiDungChi> GetAllDMNoiDungChiBQP(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach);

        /// <summary>
        /// Get paging all DM_NoiDungChi by condition
        /// </summary>
        /// <param name="_paging">paging</param>
        /// <param name="code">sMaNoiDungChi</param>
        /// <param name="name">sTenNoiDungChi</param>
        IEnumerable<DMNoiDungChiViewModel> GetAllDMNoiDungChiBQPPaging(ref PagingInfo _paging, int iNamLamViec, string sCode = "", string sName = "");

        /// <summary>
        /// Insert DM_NoiDungChi
        /// </summary>
        /// <param name="sCode">sMaNoiDungChi</param>
        /// <param name="sName">sTenNoiDungChi</param>
        /// <param name="sNote">sGhiChu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        /// <returns></returns>
        bool InsertDMNoiDungChiBQP(string sCode, string sName, Guid? iID_Parent, string sNote, string sUserLogin, int iNamLamViec);

        /// <summary>
        /// Update DM_NoiDungChiBQP
        /// </summary>
        /// <param name="iId">iID_NoiDungChi</param>
        /// <param name="sCode">sMaNoiDungChi</param>
        /// <param name="sName">sTenNoiDungChi</param>
        /// <param name="sNote">sGhiChu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungSua</param>
        /// <param name="bPublic">bPublic</param>
        bool UpdateDMNoiDungChiBQP(Guid iId, string sCode, string sName, Guid? iID_Parent, string sNote, string sUserLogin, bool bPublic = true);
        IEnumerable<DM_NoiDungChi> GetNoiDungChiCha(int iNamLamViec, Guid? iID_NoiDungChi);
        DMNoiDungChiViewModel GetDMNoiDungChiByIDForDetail(Guid iId);
        IEnumerable<DMNoiDungChiViewModel> GetTreeAllDMNoiDungChiBQPPaging(ref PagingInfo _paging, int iNamLamViec);

        IEnumerable<DMNoiDungChiViewModel> GetTreeAllDMNoiDungChiForReport(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach);
        DataTable GetAllDMNoiDungChiByNamLamViec(int iNamLamViec, Dictionary<string, string> _filters);
        IEnumerable<DM_NoiDungChi> GetAllDMNoiDungChiForCheck(int iNamLamViec);
        DM_NoiDungChi GetDMNoiDungChiChaByMaNDCCha(string sMaNoiDungChiCha, int iNamLamViec);
        bool DeleteDuToanChiTiet(List<string> aListIdUpdate, List<string> aListIdDelete);

        /// <summary>
        /// kiem tra trang thai xoa noi dung chi
        /// </summary>
        /// <param name="iID_NoiDungChi"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        bool CheckCanDeleteNDChi(Guid iID_NoiDungChi, int iNamLamViec);
        #endregion

        #region Danh mục Nguồn ngân sách - Nguồn BTC cấp

        /// <summary>
        /// Lấy list danh mục nguồn
        /// <returns></returns>
        IEnumerable<DM_Nguon> GetAllDMNguon();

        /// <summary>
        /// Lấy danh mục nguồn BTC cấp theo cha con
        /// </summary>
        /// <param name="_paging">Pagin info</param>
        /// <returns></returns>
        IEnumerable<DanhMucNguonViewModel> GetTreeAllDMNguonPaging(ref PagingInfo _paging, int iNamLamViec);

        /// <summary>
        /// Lấy danh mục nguồn BTC cấp theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sMaNguon"></param>
        /// <param name="sNoiDung"></param>
        /// <param name="iID_NguonCha"></param>
        /// <returns></returns>
        IEnumerable<DanhMucNguonViewModel> GetAllDMNguonPaging(ref PagingInfo _paging, string sMaNguon, string sNoiDung, Guid? iID_NguonCha, int iNamLamViec);

        /// <summary>
        /// Check tồn tại mã nguồn con
        /// </summary>
        /// <param name="iID_Nguon"></param>
        /// <returns></returns>
        bool CheckHasChild(Guid iID_Nguon);

        /// <summary>
        /// Thêm mới danh mục nguồn
        /// </summary>
        /// <param name="sMaCTMT">Mã chương trình mục tiêu</param>
        /// <param name="sMaNguon">Mã nguồn</param>
        /// <param name="sLoai">Loại</param>
        /// <param name="sKhoan">Khoản</param>
        /// <param name="sNoiDung">Nội dung</param>
        /// <param name="iID_NguonCha">ID Mã nguồn cha</param>
        /// <param name="sUserLogin">User đăng nhập</param>
        /// <returns></returns>
        bool InsertDMNguon(string sMaCTMT, string sMaNguon, string sLoai, string sKhoan, string sNoiDung, Guid? iID_NguonCha, string sUserLogin);

        /// <summary>
        /// Lấy danh mục nguồn theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DM_Nguon GetDMNguon(Guid id);

        /// <summary>
        /// Lấy danh mục nguồn View model
        /// </summary>
        /// <param name="id">ID danh mục nguồn</param>
        /// <returns></returns>
        DanhMucNguonViewModel DMNguonViewMoDel(Guid id);

        /// <summary>
        /// Update danh mục nguồn
        /// </summary>
        /// <param name="iId">ID danh mục nguồn</param>
        /// <param name="sMaCTMT">Mã chương trình mục tiêu</param>
        /// <param name="sMaNguon">Mã nguồn</param>
        /// <param name="sLoai">Loại</param>
        /// <param name="sKhoan">Khoản</param>
        /// <param name="sNoiDung">Nội dung</param>
        /// <param name="iID_NguonCha">ID mã nguồn cha</param>
        /// <param name="sUserLogin">User đăng nhập</param>
        /// <param name="bPublic">Trạng thái</param>
        /// <returns></returns>
        bool UpdateDMNguon(Guid iId, string sMaCTMT, string sMaNguon, string sLoai, string sKhoan, string sNoiDung, Guid? iID_NguonCha, string sUserLogin, bool bPublic = true);

        /// <summary>
        /// Lấy mã nguồn cha
        /// </summary>
        /// <returns></returns>
        List<DM_Nguon> GetAllMaNguonCha(Guid? iID_Nguon_Parent, int iNamLamViec);

        IEnumerable<NNSDMNguonViewModel> GetAllDMNguonBaoCao(int iNamLamViec, int? iLoaiNganSach = null);

        DataTable GetAllDMNguonByNamLamViec(int iNamLamViec, Dictionary<string, string> _filters);
        DM_Nguon GetDMNguonByMaNguon(string sMaNguon, int iNamLamViec);
        IEnumerable<DM_Nguon> GetAllDMNguonForCheck(int iNamLamViec);

        /// <summary>
        /// kiem tra trang thai xoa DM nguon
        /// </summary>
        /// <param name="iID_Nguon"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        bool CheckCanDeleteDMNguon(Guid iID_Nguon, int iNamLamViec);
        #endregion

        #region Danh mục Nguồn ngân sách - Khối đơn vị quản lý
        /// <summary>
        /// Lấy danh mục khối đơn vị QL theo ID
        /// </summary>
        /// <param name="iId">ID danh mục khối ĐVQL</param>
        /// <returns></returns>
        DM_KhoiDonViQuanLy GetDMKhoiDonViQL(Guid iId);

        /// <summary>
        /// Lấy danh mục khối đơn vị QL theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging Info</param>
        /// <param name="sMaKhoi">Mã khối</param>
        /// <param name="sTenKhoi">Tên khối</param>
        /// <returns></returns>
        IEnumerable<DM_KhoiDonViQuanLy> GetAllDMKhoiDonviQL(ref PagingInfo _paging, int iNamLamViec, string sMaKhoi = "", string sTenKhoi = "");

        /// <summary>
        /// Thêm Danh mục khối ĐV QL
        /// </summary>
        /// <param name="sMaKhoi">Mã khối</param>
        /// <param name="sTenKhoi">Tên khối</param>
        /// <param name="sNote">Note</param>
        /// <param name="sUserLogin">User đăng nhập</param>
        /// <returns></returns>
        bool InsertDMKhoiDonviQL(string sMaKhoi, string sTenKhoi, string sNote, string sUserLogin);

        /// <summary>
        /// Update danh mục khối đơn vị QL
        /// </summary>
        /// <param name="iId">ID danh mục</param>
        /// <param name="sMaKhoi">Mã khối</param>
        /// <param name="sTenKhoi">Tên khối</param>
        /// <param name="sNote">Note</param>
        /// <param name="sUserLogin">User đăng nhập</param>
        /// <param name="bPublic">Trạng thái</param>
        /// <returns></returns>
        bool UpdateDMKhoiDonviQL(Guid iId, string sMaKhoi, string sTenKhoi, string sNote, string sUserLogin, bool bPublic = true);

        /// <summary>
        /// CheckExist mã danh mục khối đơn vị quản lí
        /// </summary>
        /// <param name="iId">ID danh mục</param>
        /// <param name="sMaKhoi">Mã khối</param>
        /// <returns></returns>
        bool CheckExistDMKhoiDonVIQL(Guid iId, string sMaKhoi, int iNamLamViec);
        #endregion

        #region Danh mục loại chi phí

        IEnumerable<VDT_DM_ChiPhi_ViewModel> GetAllDanhMucLoaiChiPhi(ref PagingInfo _paging, string sMaChiPhi = "", string sTenVietTat = "", string sTenChiPhi = "");
        VDT_DM_ChiPhi_ViewModel GetDanhMucLoaiChiPhiById(Guid iID_ChiPhiID);
        bool DeleteLoaiChiPhi(Guid iID_ChiPhiID);
        bool SaveLoaiChiPhi(VDT_DM_ChiPhi data, string sUserName);
        bool CheckExistMaLoaiChiPhi(string sMaLoaiChiPhi, Guid iID_ChiPhi);
        bool CheckExistIthuTu(int iThuTu, Guid iID_ChiPhi);
        #endregion

        #region Danh mục loại công trình

        /// <summary>
        /// Lấy danh sách loại công trình để hiển thị trên partial View
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaicongTrinhInPartial(string sTenLoaiCongTrinh = "");
        IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaiCongTrinhByName(string sTenLoaiCongTrinh, string sTenVietTat, string sMaLoaiCongTrinh, int? iThuTu, string sMoTa);

        /// <summary>
        /// Get VDT_DM_LoaiCongTrinh By id
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinh</param>
        /// <returns></returns>
        VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhById(Guid iId);

        /// <summary>
        /// Get data choose parent when edit or create data
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinh</param>
        /// <returns></returns>
        IEnumerable<VDTDMLoaiCongTrinhViewModel> GetComboboxParent(Guid? iId);

        /// <summary>
        /// Check data have child when delete data
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinh</param>
        /// <returns></returns>
        bool CheckLoaiCongTrinhHaveChild(Guid iId);

        /// <summary>
        /// Insert VDT_DM_LoaiCongTrinh
        /// </summary>
        /// <param name="data">VDT_DM_LoaiCongTrinh</param>
        /// <returns></returns>
        bool InsertDMLoaiCongTrinh(VDT_DM_LoaiCongTrinh data, string sUserLogin);

        /// <summary>
        /// Update VDT_DM_LoaiCongTrinh
        /// </summary>
        /// <param name="data">VDT_DM_LoaiCongTrinh</param>
        /// <returns></returns>
        bool UpdateDMLoaiCongTrinh(VDT_DM_LoaiCongTrinh data, string sUserLogin, bool bPublic = true);

        IEnumerable<dynamic> GetAllDMLoaiCongTrinh();

        bool DeleteLoaiCongTrinh(VDT_DM_LoaiCongTrinh sModel, string iIPSua, string sUserLogin);

        #endregion

        #region Mapping Nội dung chi - ML Nhu cầu - NinhNV
        DataTable GetAllDMNoiDungChiMappingMLNC(int iNamLamViec, Dictionary<string, string> _filters);
        DataTable GetAllMLNCChiTietMapping(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> _filters);
        bool deleteAllNNSNDChiMLNC(List<Guid> lstIdMap);
        bool insertAllNNSNDChiMLNC(Guid? iID_NoiDungChi, List<Guid> lstIdMLNC, string sUserLogin, int iNamLamViec);
        #endregion

        #region Mapping Nội dung chi - ML Ngân sách - NinhNV
        DataTable GetAllDMNoiDungChiMappingMLNS(int iNamLamViec, Dictionary<string, string> _filters);
        DataTable GetAllMLNSChiTietMapping(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> _filters);
        bool deleteAllNNSNDChiMLNS(List<Guid> lstIdMap);
        bool InsertAllNNSNdcMLNS(Guid? iID_NoiDungChi, IEnumerable<MappingNdcMLNSModel> dataIenumDelete, IEnumerable<MappingNdcMLNSModel> dataIenumNew, string sUserLogin, int iNamLamViec);
        #endregion

        #region Danh mục VĐT - Nhà Thầu
        IEnumerable<VDT_DM_NhaThau_ViewModel> GetAllDanhMucNhaThau(ref PagingInfo _paging, string sMaNhaThau = "", string sTenNhaThau = "", string sDaiDien = "", string sChucVu = "", string sDiaChi = "", string sDienThoai = "", string sFax = "", string sEmail = "", string sWebsite = "", string sSoTaiKhoan = "", string sNganHang = "", string sMaSoThue = "", string sNguoiLienHe = "", string sDienThoaiLienHe = "", string sMaNganHang = "");
        VDT_DM_NhaThau_ViewModel GetDanhMucNhaThauById(Guid iID_NhaThau);
        bool DeleteNhaThau(Guid iID_NhaThau);
        bool SaveNhaThau(VDT_DM_NhaThau data);
        #endregion

        #region Danh mục VĐT - Đơn vị thực hiện dự án
        IEnumerable<VDT_DM_DonViThucHienDuAn_ViewModel> GetAllDanhMucDonViThucHienDuAn(ref PagingInfo _paging, string sMaDonViTHDA = "", string sTenDonViTHDA = "");
        VDT_DM_DonViThucHienDuAn_ViewModel GetDanhMucDonViThucHienDuAnById(Guid iID_DonVi);
        bool DeleteDonViThucHienDuAn(Guid iID_DonVi);
        bool SaveDonViThucHienDuAn(VDT_DM_DonViThucHienDuAn data);
        IEnumerable<VDT_DM_DonViThucHienDuAn> GetListDonViThucHienDuAnCha(Guid? iID_DonVi);
        IEnumerable<VDT_DM_DonViThucHienDuAn> GetListDonViThucHienDuAn();
        VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAn(string iID_Ma_DonVi);
        VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnById(Guid iID_DonVi);
        NS_DonVi GetNSDonViById(Guid iID_DonVi);
        #endregion

        #region Danh mục VĐT - Chủ đầu tư
        IEnumerable<VDT_DM_ChuDauTu_ViewModel> GetAllDanhMucChuDauTu(ref PagingInfo _paging, int iNamLamViec, string sMaChuDauTu = "", string sTenChuDauTu = "");
        VDT_DM_ChuDauTu_ViewModel GetDanhMucChuDauTuById(Guid iID_ChuDauTu);
        VDT_DM_ChuDauTu_ViewModel GetDanhMucChuDauTuDetailById(Guid iID_ChuDauTu);
        bool DeleteChuDauTu(Guid iID_ChuDauTu, string sUserName);
        bool SaveChuDauTu(DM_ChuDauTu data, int iNamLamViec, string sUserName);
        IEnumerable<VDT_DM_ChuDauTu_ViewModel> GetListChuDauTuCha(Guid? iID_ChuDauTu, int iNamLamViec);
        #endregion

        #region Danh mục Ngoại hối - Loại đơn vị tiền tệ
        IEnumerable<NH_DM_LoaiTienTeModel> GetAllDanhMucDonViTienTe(ref PagingInfo _paging, string sMaTienTe = "", string sTenTienTe = "", string sMoTaChiTiet = "");
        NH_DM_LoaiTienTeModel GetDanhMucDonViTienTeByID(Guid id);
        bool DeleteDonViTienTe(Guid id);
        bool SaveDonViTienTe(NH_DM_LoaiTienTe data);
        IEnumerable<NH_DM_LoaiTienTe> GetListTienTe(Guid? id);
        #endregion
    }

    public class DanhMucService : IDanhMucService
    {
        #region Private
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IDanhMucService _default;
        private readonly INganSachService _nganSachService;
        #endregion

        public DanhMucService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
            _nganSachService = NganSachService.Default;
        }
        public static IDanhMucService Default
        {
            get { return _default ?? (_default = new DanhMucService()); }
        }

        #region Danh mục Nguồn ngân sách - Loại dự toán
        public DM_LoaiDuToan GetDMLoaiDuToanByID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_by_id_loaidutoan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<DM_LoaiDuToan>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<DM_LoaiDuToan> GetAllDMLoaiDuToan(int iNamLamViec, string sCode, string sName)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_loaidutoan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_LoaiDuToan>(sql,
                    param: new
                    {
                        iNamLamViec,
                        sCode = sCode,
                        sName = sName,
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<DM_LoaiDuToan> GetAllDMLoaiDuToanPaging(ref PagingInfo _paging, int iNamLamViec, string sCode, string sName)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sCode", sCode);
                lstParam.Add("sName", sName);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DM_LoaiDuToan>("proc_get_all_loaidutoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool InsertDMLoaiDuToan(string sCode, string sName, string sNote, string sUserLogin, int iNamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("dm_insert_loaidutoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        sCode,
                        sName,
                        sNote,
                        sUserLogin,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool UpdateDMLoaiDuToan(Guid iId, string sCode, string sName, string sNote, string sUserLogin, bool bPublic = true)
        {
            var sql = FileHelpers.GetSqlQuery("dm_update_loaidutoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId,
                        sCode,
                        sName,
                        sNote,
                        sUserLogin,
                        bPublic
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool CheckExistMaDMLoaiDuToan(Guid? iId, string sCode, int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            if (iId == null)
            {
                query.Append($"SELECT * FROM DM_LoaiDuToan WHERE sMaLoaiDuToan = '{sCode}' and bPublic = 1 and iNamLamViec = '{iNamLamViec}'");
            }
            if (iId != null)
            {
                query.Append($"SELECT * FROM DM_LoaiDuToan WHERE sMaLoaiDuToan = '{sCode}' and iID_LoaiDuToan != '{iId}' and bPublic = 1 and iNamLamViec = '{iNamLamViec}'");
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_LoaiDuToan>(query.ToString(),

                    commandType: CommandType.Text);

                if (items.Any())
                {
                    return false;
                }
            }

            return true;
        }

        public List<DM_LoaiDuToan> GetListDMLoaiDuToan(string sUserName)
        {
            var result = new List<DM_LoaiDuToan>();

            var config = _nganSachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_LoaiDuToan>($"SELECT * FROM DM_LoaiDuToan WHERE iTrangThai = 1");
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }

            return result.OrderBy(x => x.sMaLoaiDuToan).ToList();
        }
        #endregion

        #region Danh mục Nguồn ngân sách - Nội dung chi
        public DM_NoiDungChi GetDMNoiDungChiByID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_by_id_noidungchibqp.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<DM_NoiDungChi>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public bool CheckExistMaNoiDungchi(string sMaNoiDungChi, int iNamLamViec, Guid? iId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_check_exist_manoidungchi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        sMaNoiDungChi,
                        iId,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public IEnumerable<DM_NoiDungChi> GetAllDMNoiDungChiBQP(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_noidungchibqp.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_NoiDungChi>(sql,
                    param: new
                    {
                        iNamLamViec,
                        iLoaiNganSach,
                        iNguonNganSach
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<DMNoiDungChiViewModel> GetAllDMNoiDungChiBQPPaging(ref PagingInfo _paging, int iNamLamViec, string sCode, string sName)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sCode", sCode);
                lstParam.Add("sName", sName);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DMNoiDungChiViewModel>("proc_get_all_noidungchibqp_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool InsertDMNoiDungChiBQP(string sCode, string sName, Guid? iID_Parent, string sNote, string sUserLogin, int iNamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("dm_insert_noidungchibqp.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        sCode,
                        sName,
                        iID_Parent,
                        sNote,
                        sUserLogin,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool UpdateDMNoiDungChiBQP(Guid iId, string sCode, string sName, Guid? iID_Parent, string sNote, string sUserLogin, bool bPublic = true)
        {
            var sql = FileHelpers.GetSqlQuery("dm_update_noidungchibqp.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId,
                        sCode,
                        sName,
                        iID_Parent,
                        sNote,
                        sUserLogin,
                        bPublic
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public IEnumerable<DM_NoiDungChi> GetNoiDungChiCha(int iNamLamViec, Guid? iID_NoiDungChi)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_noidungchicha.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_NoiDungChi>(sql,
                    param: new
                    {
                        iNamLamViec,
                        iID_NoiDungChi
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public DMNoiDungChiViewModel GetDMNoiDungChiByIDForDetail(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_by_id_noidungchibqp_fordetail.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<DMNoiDungChiViewModel>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<DMNoiDungChiViewModel> GetTreeAllDMNoiDungChiBQPPaging(ref PagingInfo _paging, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DMNoiDungChiViewModel>("proc_get_tree_all_noidungchibqp_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<DMNoiDungChiViewModel> GetTreeAllDMNoiDungChiForReport(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_tree_all_dmnoidungchi_report.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DMNoiDungChiViewModel>(sql,
                    param: new
                    {
                        iNamLamViec,
                        iLoaiNganSach,
                        iNguonNganSach
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public DataTable GetAllDMNoiDungChiByNamLamViec(int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_dm_noidungchi_bqp.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }

        public IEnumerable<DM_NoiDungChi> GetAllDMNoiDungChiForCheck(int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM DM_NoiDungChi where iTrangThai = 1 and iNamLamViec = @iNamLamViec ";
                var items = conn.Query<DM_NoiDungChi>(sql,
                    param: new
                    {
                        iNamLamViec
                    },
                    commandType: CommandType.Text);
                return items;
            }
        }

        public DM_NoiDungChi GetDMNoiDungChiChaByMaNDCCha(string sMaNoiDungChiCha, int iNamLamViec)
        {
            var sql = $"SELECT * FROM DM_NoiDungChi WHERE iTrangThai = 1 and sMaNoiDungChi = @sMaNoiDungChiCha and iNamLamViec = @iNamLamViec ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<DM_NoiDungChi>(
                    sql,
                    param: new
                    {
                        sMaNoiDungChiCha,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        /// <summary>
        /// kiem tra trang thai xoa noi dung chi
        /// </summary>
        /// <param name="iID_NoiDungChi"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public bool CheckCanDeleteNDChi(Guid iID_NoiDungChi, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    int countDotNhanChiTiet = conn.QueryFirst<int>("SELECT COUNT(iID_DotNhanChiTiet_NDChi) as count FROM NNS_DotNhanChiTiet_NDChi WHERE iID_NoiDungChi = @iID_NoiDungChi AND iNamLamViec = @iNamLamViec", param: new
                    {
                        iID_NoiDungChi,
                        iNamLamViec
                    },
                                                commandType: CommandType.Text);
                    int countDuToanChiTiet = conn.QueryFirst<int>("SELECT COUNT(iID_DuToanChiTiet) as count FROM NNS_DuToanChiTiet WHERE sMaNoiDungChi = (select sMaNoiDungChi FROM DM_NoiDungChi WHERE iID_NoiDungChi = @iID_NoiDungChi and iNamLamViec = @iNamLamViec) AND iNamLamViec = @iNamLamViec", param: new
                    {
                        iID_NoiDungChi,
                        iNamLamViec
                    },
                                                commandType: CommandType.Text);

                    if (countDotNhanChiTiet == 0 && countDuToanChiTiet == 0)
                        return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool DeleteDuToanChiTiet(List<string> aListIdUpdate, List<string> aListIdDelete)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var listDMNoiDung = conn.Query<DM_NoiDungChi>("SELECT * FROM DM_NoiDungChi");
                var listDuToanChiTiet = conn.Query<NNS_DuToanChiTiet>("SELECT * FROM NNS_DuToanChiTiet");

                var listMaNoiDung = new List<string>();
                if (listDMNoiDung != null && listDMNoiDung.Any())
                {
                    var listUpdate = new List<string>();
                    if (aListIdUpdate != null && aListIdUpdate.Any())
                    {
                        foreach (var item in aListIdUpdate)
                        {
                            var model = listDMNoiDung.Where(x => x.iID_Parent == Guid.Parse(item)).ToList();
                            if (model == null || !model.Any())
                            {
                                listUpdate.Add(item);
                            }
                        }
                    }

                    //listMaNoiDung = listDMNoiDung.Where(x => x.iID_Parent == null && !listUpdate.Contains(x.iID_NoiDungChi.ToString())).Select(x => x.sMaNoiDungChi).ToList();

                    if (aListIdDelete != null && aListIdDelete.Any())
                    {
                        var listMaNoiDungDelete = listDMNoiDung.Where(x => aListIdDelete.Contains(x.iID_NoiDungChi.ToString()) && !listUpdate.Contains(x.iID_NoiDungChi.ToString())).Select(x => x.sMaNoiDungChi).ToList();
                        if (listMaNoiDungDelete != null && listMaNoiDungDelete.Any())
                        {
                            listMaNoiDung.AddRange(listMaNoiDungDelete);
                        }
                    }
                }
                if (listDuToanChiTiet != null && listDuToanChiTiet.Any())
                {
                    var listDelete = listDuToanChiTiet.Where(x => listMaNoiDung.Contains(x.sMaNoiDungChi)).ToList();
                    if (listDelete != null && listDelete.Any())
                    {
                        foreach (var item in listDelete)
                        {
                            conn.Execute(string.Format("DELETE NNS_DuToanChiTiet WHERE iID_DuToanChiTiet = '{0}'", item.iID_DuToanChiTiet));
                        }
                    }
                }

                // delete NNS_NDChi_MLNhuCau, NNS_NDChi_MLNS
                var sql = "DELETE FROM NNS_NDChi_MLNhuCau WHERE iID_NoiDungChi in @aListIdDelete; DELETE FROM NNS_NDChi_MLNS WHERE iID_NoiDungChi in @aListIdDelete;";
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        aListIdDelete
                    },
                    commandType: CommandType.Text);
            }

            return true;
        }
        #endregion

        #region Danh mục Nguồn ngân sách - Nguồn BTC cấp

        public IEnumerable<DM_Nguon> GetAllDMNguon()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM DM_Nguon";
                var items = conn.Query<DM_Nguon>(sql, commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<DanhMucNguonViewModel> GetTreeAllDMNguonPaging(ref PagingInfo _paging, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhMucNguonViewModel>("proc_get_all_danhmucnguon_tree_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<DanhMucNguonViewModel> GetAllDMNguonPaging(ref PagingInfo _paging, string sMaNguon, string sNoiDung, Guid? iID_NguonCha, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sMaNguon", sMaNguon);
                lstParam.Add("sNoiDung", sNoiDung);
                lstParam.Add("iID_NguonCha", iID_NguonCha);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhMucNguonViewModel>("proc_get_all_danhmucnguon_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool InsertDMNguon(string sMaCTMT, string sMaNguon, string sLoai, string sKhoan, string sNoiDung, Guid? iID_NguonCha, string sUserLogin)
        {
            var config = _nganSachService.GetCauHinh(sUserLogin);
            var sql = FileHelpers.GetSqlQuery("dm_insert_danhmucnguon.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        sMaCTMT,
                        sMaNguon,
                        sLoai,
                        sKhoan,
                        sNoiDung,
                        iID_NguonCha,
                        iNamLamViec = config.iNamLamViec,
                        sUserLogin
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public DM_Nguon GetDMNguon(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return null;
            }
            var sql = $"SELECT * FROM DM_Nguon WHERE iID_Nguon = '{id}'";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<DM_Nguon>(
                    sql,
                    param: new
                    {
                        iID_Nguon = id,
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public DanhMucNguonViewModel DMNguonViewMoDel(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_by_id_dmnguonViewModel.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<DanhMucNguonViewModel>(
                    sql,
                    param: new
                    {
                        iID_Nguon = id,
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public bool UpdateDMNguon(Guid iId, string sMaCTMT, string sMaNguon, string sLoai, string sKhoan, string sNoiDung, Guid? iID_NguonCha, string sUserLogin, bool bPublic = true)
        {
            var sql = FileHelpers.GetSqlQuery("dm_update_danhmucnguon.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId,
                        sMaCTMT,
                        sMaNguon,
                        sLoai,
                        sKhoan,
                        sNoiDung,
                        sUserLogin,
                        iID_NguonCha,
                        bPublic
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool CheckHasChild(Guid iID_Nguon)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT COUNT(iID_Nguon) FROM DM_Nguon WHERE iID_NguonCha = '{0}' AND bPublic = 1;", iID_Nguon);

            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirstOrDefault<int>(query.ToString(),

                    commandType: CommandType.Text);

                return count > 0;
            }
        }

        public List<DM_Nguon> GetAllMaNguonCha(Guid? iID_Nguon_Parent, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iId", iID_Nguon_Parent, dbType: DbType.Guid);
                lstParam.Add("iNamLamViec", iNamLamViec);
                var items = conn.Query<DM_Nguon>("get_nns_danhmucnguon_manguoncha", lstParam, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        public IEnumerable<NNSDMNguonViewModel> GetAllDMNguonBaoCao(int iNamLamViec, int? iLoaiNganSach = null)
        {
            var sql = FileHelpers.GetSqlQuery("dm_nguon_get_by_loaingansach.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("iLoaiNganSach", iLoaiNganSach);
                var items = conn.Query<NNSDMNguonViewModel>(sql, lstParam, commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public DataTable GetAllDMNguonByNamLamViec(int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_dm_nguon_btccap.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }

        public DM_Nguon GetDMNguonByMaNguon(string sMaNguon, int iNamLamViec)
        {
            var sql = $"SELECT * FROM DM_Nguon WHERE iTrangThai = 1 and sMaNguon = @sMaNguon and iNamLamViec = @iNamLamViec ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<DM_Nguon>(
                    sql,
                    param: new
                    {
                        sMaNguon,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public IEnumerable<DM_Nguon> GetAllDMNguonForCheck(int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM DM_Nguon where iTrangThai = 1 and iNamLamViec = @iNamLamViec ";
                var items = conn.Query<DM_Nguon>(sql,
                    param: new
                    {
                        iNamLamViec
                    },
                    commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// kiem tra trang thai xoa DM nguon
        /// </summary>
        /// <param name="iID_Nguon"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public bool CheckCanDeleteDMNguon(Guid iID_Nguon, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    int countDotNhanChiTiet = conn.QueryFirst<int>("SELECT count(iID_DotNhanChiTiet) FROM NNS_DotNhanChiTiet WHERE iID_Nguon = @iID_Nguon AND iNamLamViec = @iNamLamViec", param: new
                    {
                        iID_Nguon,
                        iNamLamViec
                    },
                                                commandType: CommandType.Text);
                    int countDMNoiDungChi = conn.QueryFirst<int>("SELECT count(iID_NoiDungChi) FROM DM_NoiDungChi WHERE iTrangThai = 1 AND iID_Nguon = @iID_Nguon AND iNamLamViec = @iNamLamViec", param: new
                    {
                        iID_Nguon,
                        iNamLamViec
                    },
                                                commandType: CommandType.Text);
                    if (countDotNhanChiTiet == 0 && countDMNoiDungChi == 0)
                        return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        #endregion

        #region Danh mục Nguồn ngân sách - Khối đơn vị quản lý
        public IEnumerable<DM_KhoiDonViQuanLy> GetAllDMKhoiDonviQL(ref PagingInfo _paging, int iNamLamViec, string sMaKhoi = "", string sTenKhoi = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sMaKhoi", sMaKhoi);
                lstParam.Add("sTenKhoi", sTenKhoi);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DM_KhoiDonViQuanLy>("proc_get_all_khoidonviql_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public DM_KhoiDonViQuanLy GetDMKhoiDonViQL(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_by_id_khoidonviql.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<DM_KhoiDonViQuanLy>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public bool InsertDMKhoiDonviQL(string sMaKhoi, string sTenKhoi, string sNote, string sUserLogin)
        {
            var config = _nganSachService.GetCauHinh(sUserLogin);
            var sql = FileHelpers.GetSqlQuery("dm_insert_khoidonviql.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        sMaKhoi,
                        sTenKhoi,
                        sNote,
                        sUserLogin,
                        iNamLamViec = config.iNamLamViec
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool UpdateDMKhoiDonviQL(Guid iId, string sMaKhoi, string sTenKhoi, string sNote, string sUserLogin, bool bPublic = true)
        {
            var sql = FileHelpers.GetSqlQuery("dm_update_khoidonviql.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId,
                        sMaKhoi,
                        sTenKhoi,
                        sNote,
                        sUserLogin,
                        bPublic
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public bool CheckExistDMKhoiDonVIQL(Guid iId, string sMaKhoi, int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            if (iId == null || iId == Guid.Empty)
            {
                query.Append($"SELECT * FROM DM_KhoiDonViQuanLy WHERE sMaKhoi = '{sMaKhoi}' and bPublic = 1 and iNamLamViec = '{iNamLamViec}'");
            }
            if (iId != null)
            {
                query.Append($"SELECT * FROM DM_KhoiDonViQuanLy WHERE sMaKhoi = '{sMaKhoi}' and iID_Khoi != '{iId}' and bPublic = 1 and iNamLamViec = '{iNamLamViec}'");
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_KhoiDonViQuanLy>(query.ToString(),

                    commandType: CommandType.Text);

                if (items.Any())
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Danh mục loại chi phí
        public IEnumerable<VDT_DM_ChiPhi_ViewModel> GetAllDanhMucLoaiChiPhi(ref PagingInfo _paging, string sMaChiPhi = "", string sTenVietTat = "", string sTenChiPhi = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaChiPhi", sMaChiPhi);
                lstParam.Add("sTenVietTat", sTenVietTat);
                lstParam.Add("sTenChiPhi", sTenChiPhi);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_DM_ChiPhi_ViewModel>("proc_get_all_loaichiphi_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_DM_ChiPhi_ViewModel GetDanhMucLoaiChiPhiById(Guid iID_ChiPhiID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_DM_ChiPhi WHERE iID_ChiPhi = '{0}'", iID_ChiPhiID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DM_ChiPhi_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeleteLoaiChiPhi(Guid iID_ChiPhiID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE VDT_DM_ChiPhi WHERE iID_ChiPhi =  '{0}'", iID_ChiPhiID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool SaveLoaiChiPhi(VDT_DM_ChiPhi data, string sUserName)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_ChiPhi == null || data.iID_ChiPhi == Guid.Empty)
                {
                    var entity = new VDT_DM_ChiPhi();
                    entity.MapFrom(data);
                    entity.sID_MaNguoiDungTao = sUserName;
                    entity.dNgayTao = DateTime.Now;
                    conn.Insert(entity, trans);
                }
                else
                {
                    var entity = conn.Get<VDT_DM_ChiPhi>(data.iID_ChiPhi, trans);
                    if (entity == null)
                        return false;
                    entity.sMaChiPhi = data.sMaChiPhi;
                    entity.sTenVietTat = data.sTenVietTat;
                    entity.sTenChiPhi = data.sTenChiPhi;
                    entity.sMoTa = data.sMoTa;
                    entity.iThuTu = data.iThuTu;
                    if (entity.iSoLanSua == null)
                        entity.iSoLanSua = 1;
                    else
                        entity.iSoLanSua += 1;
                    entity.sID_MaNguoiDungSua = sUserName;
                    entity.dNgaySua = DateTime.Now;
                    conn.Update(entity, trans);
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public bool CheckExistMaLoaiChiPhi(string sMaLoaiChiPhi, Guid iID_ChiPhi)
        {
            try
            {
                var sql = "SELECT * FROM VDT_DM_ChiPhi WHERE (@iID_ChiPhi IS NULL OR iID_ChiPhi != @iID_ChiPhi) and sMaChiPhi = @sMaLoaiChiPhi\r\n";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_DM_ChiPhi>(
                        sql,
                        param: new
                        {
                            iID_ChiPhi,
                            sMaLoaiChiPhi
                        },
                        commandType: CommandType.Text);

                    return item.ToList().Count > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }


        public bool CheckExistIthuTu(int iThuTu, Guid iID_ChiPhi)
        {
            try
            {
                var sql = "SELECT * FROM VDT_DM_ChiPhi WHERE (@iID_ChiPhi IS NULL OR iID_ChiPhi != @iID_ChiPhi) and iThuTu = @iThuTu\r\n";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_DM_ChiPhi>(
                        sql,
                        param: new
                        {
                            iID_ChiPhi,
                            iThuTu
                        },
                        commandType: CommandType.Text);

                    return item.ToList().Count > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        #endregion

        #region Danh mục loại công trình

        public IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaicongTrinhInPartial(string sTenLoaiCongTrinh = "")
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_loaicongtrinh_view.sql");


            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTDMLoaiCongTrinhViewModel>(sql,
                        param: new
                        {
                            sTenLoaiCongTrinh
                        },
                        commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaiCongTrinhByName(string sTenLoaiCongTrinh, string sTenVietTat, string sMaLoaiCongTrinh, int? iThuTu, string sMoTa)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_loaicongtrinh_by_name.sql");


            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTDMLoaiCongTrinhViewModel>(sql,
                        param: new
                        {
                            sTenLoaiCongTrinh,
                            sTenVietTat,
                            sMaLoaiCongTrinh,
                            iThuTu,
                            sMoTa
                        },
                        commandType: CommandType.Text);
                return items;
            }
        }

        public VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhById(Guid iId)
        {
            var sql = "SELECT * FROM VDT_DM_LoaiCongTrinh WHERE iID_LoaiCongTrinh = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<VDT_DM_LoaiCongTrinh>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDTDMLoaiCongTrinhViewModel> GetComboboxParent(Guid? iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_comboboxparentloaicongtrinh.sql");

            if (!iId.HasValue) iId = Guid.NewGuid();


            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTDMLoaiCongTrinhViewModel>(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);
                return items;
            }
        }

        public bool CheckLoaiCongTrinhHaveChild(Guid iId)
        {
            var sql = "SELECT COUNT(iID_LoaiCongTrinh) FROM VDT_DM_LoaiCongTrinh WHERE iID_Parent = @iId AND bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<int>(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                return items == 0 ? false : true;
            }
        }

        public bool InsertDMLoaiCongTrinh(VDT_DM_LoaiCongTrinh data, string sUserLogin)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    if (data.iID_LoaiCongTrinh == null || data.iID_LoaiCongTrinh == new Guid())
                    {
                        VDT_DM_LoaiCongTrinh entity = new VDT_DM_LoaiCongTrinh();
                        entity.MapFrom(data);
                        entity.bActive = true;
                        entity.dNgayTao = DateTime.Now;
                        entity.sID_MaNguoiDungSua = sUserLogin;
                        entity.iID_LoaiCongTrinh = Guid.NewGuid();
                        conn.Insert(entity);
                        conn.Close();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool UpdateDMLoaiCongTrinh(VDT_DM_LoaiCongTrinh data, string sUserLogin, bool bPublic)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_update_loaicongtrinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId = data.iID_LoaiCongTrinh,
                        data.sMaLoaiCongTrinh,
                        data.sTenLoaiCongTrinh,
                        data.sTenVietTat,
                        iParentID = data.iID_Parent,
                        data.iThuTu,
                        data.sMoTa,
                        sUserLogin,
                        bPublic,
                        data.L,
                        data.LNS,
                        data.K,
                        data.M,
                        data.TM,
                        data.TTM,
                        data.NG,
                        data.TNG1,
                        data.TNG2
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public IEnumerable<dynamic> GetAllDMLoaiCongTrinh()
        {
            var sql = "SELECT * FROM VDT_DM_LoaiCongTrinh";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(sql);

                return items;
            }
        }

        public bool DeleteLoaiCongTrinh(VDT_DM_LoaiCongTrinh sModel, string iIPSua, string sUserLogin)
        {
            var sql = $"Update VDT_DM_LoaiCongTrinh set bActive = 0 ,dNgaySua = GETDATE() ,sID_MaNguoiDungSua = '{sUserLogin}' , sIPSua = '{iIPSua}' where iID_LoaiCongTrinh = '{sModel.iID_LoaiCongTrinh}'";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Execute(sql, null, null, null, commandType: CommandType.Text) > 0;
            }
        }

        #endregion

        #region Mapping Nội dung chi - ML Nhu cầu - NinhNV
        public DataTable GetAllDMNoiDungChiMappingMLNC(int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_tree_all_dmnoidungchi_mapping.sql");
            string sTenNoiDungChi = "";
            string sMaNoiDungChi = "";
            string sGhiChu = "";
            int? iMap = null;
            if (_filters.ContainsKey("sTenNoiDungChi"))
            {
                sTenNoiDungChi = _filters["sTenNoiDungChi"];
                sMaNoiDungChi = _filters["sMaNoiDungChi"];
                sGhiChu = _filters["sGhiChu"];
                iMap = string.IsNullOrEmpty(_filters["sMap"]) ? (int?)null : int.Parse(_filters["sMap"]);
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@sTenNoiDungChi", sTenNoiDungChi);
                    cmd.Parameters.AddWithValue("@sMaNoiDungChi", sMaNoiDungChi);
                    cmd.Parameters.AddWithValue("@sGhiChu", sGhiChu);
                    cmd.Parameters.AddWithValue("@iMap", iMap ?? (object)DBNull.Value);
                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetAllMLNCChiTietMapping(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get__all_muclucnhucau_mapping.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    int? iMap = null;
                    if (_filters.ContainsKey("isMap"))
                        iMap = string.IsNullOrEmpty(_filters["isMap"]) ? (int?)null : int.Parse(_filters["isMap"]);
                    _filters.ToList().ForEach(x =>
                    {
                        if (x.Key != "isMap")
                            cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_NoiDungChi", iID_NoiDungChi);
                    cmd.Parameters.AddWithValue("@iMap", iMap ?? (object)DBNull.Value);
                    return cmd.GetTable();
                }
            }
        }

        public bool deleteAllNNSNDChiMLNC(List<Guid> lstIdMap)
        {
            var sql = "DELETE FROM NNS_NDChi_MLNhuCau WHERE Id in @lstIdMap";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        lstIdMap = lstIdMap
                    },
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }

        public bool insertAllNNSNDChiMLNC(Guid? iID_NoiDungChi, List<Guid> lstIdMLNC, string sUserLogin, int iNamLamViec)
        {
            var sql = "INSERT INTO NNS_NDChi_MLNhuCau(iID_NoiDungChi, iID_MLNhuCau, NamLamViec, UserCreator, DateCreated) " +
                "SELECT @iID_NoiDungChi, Id, @iNamLamViec, @sUserLogin, GETDATE() FROM SKT_MLNhuCau where Id in @lstIdMLNC";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_NoiDungChi = iID_NoiDungChi,
                        lstIdMLNC = lstIdMLNC,
                        sUserLogin = sUserLogin,
                        iNamLamViec = iNamLamViec
                    },
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }
        #endregion

        #region Mapping Nội dung chi - ML Ngân sách - NinhNV
        public DataTable GetAllDMNoiDungChiMappingMLNS(int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_tree_all_dmnoidungchi_mapping_mlns.sql");
            string sTenNoiDungChi = "";
            string sMaNoiDungChi = "";
            string sGhiChu = "";
            int? iMap = null;

            if (_filters.ContainsKey("sTenNoiDungChi"))
            {
                sTenNoiDungChi = _filters["sTenNoiDungChi"];
                sMaNoiDungChi = _filters["sMaNoiDungChi"];
                sGhiChu = _filters["sGhiChu"];
                iMap = string.IsNullOrEmpty(_filters["sMap"]) ? (int?)null : int.Parse(_filters["sMap"]);
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@sTenNoiDungChi", sTenNoiDungChi);
                    cmd.Parameters.AddWithValue("@sMaNoiDungChi", sMaNoiDungChi);
                    cmd.Parameters.AddWithValue("@sGhiChu", sGhiChu);
                    cmd.Parameters.AddWithValue("@iMap", iMap ?? (object)DBNull.Value);
                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetAllMLNSChiTietMapping(Guid? iID_NoiDungChi, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get__all_muclucngansach_mapping.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    int? iMap = null;
                    if (_filters.ContainsKey("isMap"))
                        iMap = string.IsNullOrEmpty(_filters["isMap"]) ? (int?)null : int.Parse(_filters["isMap"]);

                    _filters.ToList().ForEach(x =>
                    {
                        if (x.Key != "isMap")
                            cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@iID_NoiDungChi", iID_NoiDungChi);
                    cmd.Parameters.AddWithValue("@iMap", iMap ?? (object)DBNull.Value);
                    return cmd.GetTable();
                }
            }
        }

        public bool deleteAllNNSNDChiMLNS(List<Guid> lstIdMap)
        {
            var sql = "DELETE FROM NNS_NDChi_MLNS WHERE Id in @lstIdMap";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        lstIdMap = lstIdMap
                    },
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }
        public bool InsertAllNNSNdcMLNS(Guid? iID_NoiDungChi, IEnumerable<MappingNdcMLNSModel> dataIenumDelete, IEnumerable<MappingNdcMLNSModel> dataIenumNew, string sUserLogin, int iNamLamViec)
        {
            var datatableDelete = ToDataTable(dataIenumDelete);
            var datatableNew = ToDataTable(dataIenumNew);

            using (var conn = _connectionFactory.GetConnection())
            {

                using (var cmd1 = new SqlCommand("proc_insert_all_nns_ndc_mlns", conn))
                {
                    cmd1.Parameters.AddWithValue("@iID_NoiDungChi", iID_NoiDungChi);
                    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd1.Parameters.AddWithValue("@sUserLogin", sUserLogin);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@tempDelNNSNdcMLNS", datatableDelete);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.TempIdNSMLNS";
                    SqlParameter tvpParam2 = cmd1.Parameters.AddWithValue("@tempIdNSMLNS", datatableNew);
                    tvpParam2.SqlDbType = SqlDbType.Structured;
                    tvpParam2.TypeName = "dbo.TempIdNSMLNS";

                    conn.Open();
                    cmd1.ExecuteNonQuery();
                    conn.Close();
                }

            }
            return true;
        }

        public DataTable ToDataTable<MappingNdcMLNSModel>(IEnumerable<MappingNdcMLNSModel> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(MappingNdcMLNSModel));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (MappingNdcMLNSModel item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        #endregion

        #region Danh mục VĐT - Nhà Thầu
        public IEnumerable<VDT_DM_NhaThau_ViewModel> GetAllDanhMucNhaThau(ref PagingInfo _paging, string sMaNhaThau = "", string sTenNhaThau = "", string sDaiDien = "", string sChucVu = "", string sDiaChi = "",
            string sDienThoai = "", string sFax = "", string sEmail = "", string sWebsite = "", string sSoTaiKhoan = "", string sNganHang = "", string sMaSoThue = "", string sNguoiLienHe = "", string sDienThoaiLienHe = "", string sMaNganHang = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaNhaThau", sMaNhaThau);
                lstParam.Add("sTenNhaThau", sTenNhaThau);
                lstParam.Add("sDaiDien", sDaiDien);
                lstParam.Add("sChucVu", sChucVu);
                lstParam.Add("sDiaChi", sDiaChi);
                lstParam.Add("sDienThoai", sDienThoai);
                lstParam.Add("sFax", sFax);
                lstParam.Add("sEmail", sEmail);
                lstParam.Add("sWebsite", sWebsite);
                lstParam.Add("sSoTaiKhoan", sSoTaiKhoan);
                lstParam.Add("sNganHang", sNganHang);
                lstParam.Add("sMaSoThue", sMaSoThue);
                lstParam.Add("sNguoiLienHe", sNguoiLienHe);
                lstParam.Add("sDienThoaiLienHe", sDienThoaiLienHe);
                lstParam.Add("sMaNganHang", sMaNganHang);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_DM_NhaThau_ViewModel>("proc_get_all_danhmucnhathau_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_DM_NhaThau_ViewModel GetDanhMucNhaThauById(Guid iID_NhaThau)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_DM_NhaThau WHERE iID_NhaThauID = '{0}'", iID_NhaThau);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DM_NhaThau_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeleteNhaThau(Guid iID_NhaThau)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE VDT_DM_NhaThau WHERE iID_NhaThauID =  '{0}'", iID_NhaThau);
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool SaveNhaThau(VDT_DM_NhaThau data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_NhaThauID == null || data.iID_NhaThauID == Guid.Empty)
                {
                    var entity = new VDT_DM_NhaThau();
                    entity.MapFrom(data);
                    conn.Insert(entity, trans);
                }
                else
                {
                    var entity = conn.Get<VDT_DM_NhaThau>(data.iID_NhaThauID, trans);
                    if (entity == null)
                        return false;
                    entity.sMaNhaThau = data.sMaNhaThau;
                    entity.sTenNhaThau = data.sTenNhaThau;
                    entity.sDiaChi = data.sDiaChi;
                    entity.sDaiDien = data.sDaiDien;
                    entity.sChucVu = data.sChucVu;
                    entity.sDienThoai = data.sDienThoai;
                    entity.sFax = data.sFax;
                    entity.sEmail = data.sEmail;
                    entity.sWebsite = data.sWebsite;
                    entity.sSoTaiKhoan = data.sSoTaiKhoan;
                    entity.sMaSoThue = data.sMaSoThue;
                    entity.sNganHang = data.sNganHang;
                    entity.sNguoiLienHe = data.sNguoiLienHe;
                    entity.sDienThoaiLienHe = data.sDienThoaiLienHe;
                    entity.sMaNganHang = data.sMaNganHang;
                    conn.Update(entity, trans);
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }
        #endregion

        #region Danh mục VĐT - Đơn vị thực hiện dự án
        public IEnumerable<VDT_DM_DonViThucHienDuAn_ViewModel> GetAllDanhMucDonViThucHienDuAn(ref PagingInfo _paging, string sMaDonViTHDA = "", string sTenDonViTHDA = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaDonViTHDA", sMaDonViTHDA);
                lstParam.Add("sTenDonViTHDA", sTenDonViTHDA);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_DM_DonViThucHienDuAn_ViewModel>("proc_get_all_danhmucdonvithuchienduan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_DM_DonViThucHienDuAn_ViewModel GetDanhMucDonViThucHienDuAnById(Guid iID_DonVi)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT dvthda.*, parent.sTenDonVi AS sTenDonViCha FROM VDT_DM_DonViThucHienDuAn dvthda ");
            query.Append("LEFT JOIN VDT_DM_DonViThucHienDuAn parent ON parent.iID_DonVi = dvthda.iID_DonViCha ");
            query.AppendFormat("WHERE dvthda.iID_DonVi = '{0}' ", iID_DonVi);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DM_DonViThucHienDuAn_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeleteDonViThucHienDuAn(Guid iID_DonVi)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE VDT_DM_DonViThucHienDuAn WHERE iID_DonVi =  '{0}'", iID_DonVi);
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool SaveDonViThucHienDuAn(VDT_DM_DonViThucHienDuAn data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_DonViCha == Guid.Empty)
                    data.iID_DonViCha = null;
                if (data.iID_DonVi == null || data.iID_DonVi == Guid.Empty)
                {
                    var entity = new VDT_DM_DonViThucHienDuAn();
                    data.iID_DonVi = Guid.NewGuid();
                    if (data.iID_DonViCha == null)
                        data.BHangCha = false;
                    else
                        data.BHangCha = true;
                    entity.MapFrom(data);
                    conn.Insert(entity, trans);
                }
                else
                {
                    var entity = conn.Get<VDT_DM_DonViThucHienDuAn>(data.iID_DonVi, trans);
                    if (entity == null)
                        return false;
                    entity.iID_MaDonVi = data.iID_MaDonVi;
                    entity.sTenDonVi = data.sTenDonVi;
                    entity.iID_DonViCha = data.iID_DonViCha;
                    entity.sDiaChi = data.sDiaChi;
                    if (data.iID_DonViCha == null || data.iID_DonViCha == Guid.Empty)
                        entity.BHangCha = false;
                    else
                        entity.BHangCha = true;
                    //entity.BHangCha = data.BHangCha;
                    entity.iCapDonVi = data.iCapDonVi;
                    conn.Update(entity, trans);
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAn(string iID_Ma_DonVi)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM VDT_DM_DonViThucHienDuAn ");
            if (!string.IsNullOrEmpty(iID_Ma_DonVi))
                query.AppendFormat("WHERE iID_MaDonVi = '{0}' ", iID_Ma_DonVi);
            query.Append("ORDER BY sTenDonVi");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<VDT_DM_DonViThucHienDuAn>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }

        public VDT_DM_DonViThucHienDuAn GetDonViThucHienDuAnById(Guid iID_DonVi)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT * FROM VDT_DM_DonViThucHienDuAn ");
                if (iID_DonVi != null)
                    query.AppendFormat("WHERE iID_DonVi = '{0}' ", iID_DonVi);
                query.Append("ORDER BY sTenDonVi");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirst<VDT_DM_DonViThucHienDuAn>(query.ToString(), commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public NS_DonVi GetNSDonViById(Guid iID_DonVi)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT * FROM NS_DONVI ");
                if (iID_DonVi != null)
                    query.AppendFormat("WHERE iID_Ma = '{0}' ", iID_DonVi);
                query.Append("ORDER BY sTen");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirst<NS_DonVi>(query.ToString(), commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<VDT_DM_DonViThucHienDuAn> GetListDonViThucHienDuAn()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM VDT_DM_DonViThucHienDuAn ");
            query.Append("ORDER BY sTenDonVi");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_DonViThucHienDuAn>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DM_DonViThucHienDuAn> GetListDonViThucHienDuAnCha(Guid? iID_DonVi)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM VDT_DM_DonViThucHienDuAn ");
            if (iID_DonVi != null && iID_DonVi != Guid.Empty)
                query.AppendFormat("WHERE iID_DonVi <> '{0}' ", iID_DonVi);
            query.Append("ORDER BY sTenDonVi");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_DonViThucHienDuAn>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Danh mục VĐT - Chủ đầu tư
        public IEnumerable<VDT_DM_ChuDauTu_ViewModel> GetAllDanhMucChuDauTu(ref PagingInfo _paging, int iNamLamViec, string sMaChuDauTu = "", string sTenChuDauTu = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaChuDauTu", sMaChuDauTu);
                lstParam.Add("sTenChuDauTu", sTenChuDauTu);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_DM_ChuDauTu_ViewModel>("proc_get_all_DanhMucnhaThau_paging_of_thin", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_DM_ChuDauTu_ViewModel GetDanhMucChuDauTuById(Guid iID_ChuDauTu)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM DM_ChuDauTu WHERE ID = '{0}'", iID_ChuDauTu);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DM_ChuDauTu_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }
        public VDT_DM_ChuDauTu_ViewModel GetDanhMucChuDauTuDetailById(Guid iID_ChuDauTu)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("select cdt.*, (select sTenCDT from DM_ChuDauTu where ID = cdt.Id_Parent ) as sTenChuDauTuCha from DM_ChuDauTu cdt where ID = '{0}'", iID_ChuDauTu);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DM_ChuDauTu_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeleteChuDauTu(Guid iID_ChuDauTu, string sUserName)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<DM_ChuDauTu>(iID_ChuDauTu, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<DM_ChuDauTu>(checkExist, trans);
                        trans.Commit();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                if (trans != null) trans.Rollback();
            }

            return false;
        }

        public bool SaveChuDauTu(DM_ChuDauTu data, int iNamLamViec, string sUserName)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.ID == null || data.ID == Guid.Empty)
                {
                    var entity = new DM_ChuDauTu();
                    entity.MapFrom(data);
                    entity.iNamLamViec = iNamLamViec;
                    entity.iTrangThai = 1;
                    entity.sUserCreator = sUserName;
                    entity.dDateCreated = DateTime.Now;
                    conn.Insert(entity, trans);
                }
                else
                {
                    var entity = conn.Get<DM_ChuDauTu>(data.ID, trans);
                    if (entity == null)
                        return false;
                    entity.sId_CDT = data.sId_CDT;
                    entity.sTenCDT = data.sTenCDT;
                    entity.sKyHieu = data.sKyHieu;
                    entity.sMoTa = data.sMoTa;
                    entity.sLoai = data.sLoai;
                    entity.Id_Parent = data.Id_Parent;
                    entity.sUserModifier = sUserName;
                    entity.dDateModified = DateTime.Now;
                    entity.STKNuocNgoai = data.STKNuocNgoai;
                    entity.STKTrongNuoc = data.STKTrongNuoc;
                    entity.ChiNhanhNuocNgoai = data.ChiNhanhNuocNgoai;
                    entity.ChiNhanhTrongNuoc = data.ChiNhanhTrongNuoc;
                    conn.Update(entity, trans);
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<VDT_DM_ChuDauTu_ViewModel> GetListChuDauTuCha(Guid? iID_ChuDauTu, int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ID, CONCAT(sId_CDT, ' - ', sTenCDT) as sTenChuDauTuCha,sTenCDT, sId_CDT FROM DM_ChuDauTu ");
            query.AppendFormat("WHERE iTrangThai = 1 AND iNamLamViec = {0} ", iNamLamViec);
            if (iID_ChuDauTu != null && iID_ChuDauTu != Guid.Empty)
                query.AppendFormat("AND ID <> '{0}' ", iID_ChuDauTu);
            query.Append("ORDER BY sTenCDT");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_ChuDauTu_ViewModel>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Danh mục Ngoại hối - Loại đơn vị tiền tệ
        public IEnumerable<NH_DM_LoaiTienTeModel> GetAllDanhMucDonViTienTe(ref PagingInfo _paging, string sMaTienTe = "", string sTenTienTe = "", string sMoTaChiTiet = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaTienTe", sMaTienTe);
                lstParam.Add("sTenTienTe", sTenTienTe);
                lstParam.Add("sMoTaChiTiet", sMoTaChiTiet);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_DM_LoaiTienTeModel>("proc_get_all_danhmucdonvitiente_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public NH_DM_LoaiTienTeModel GetDanhMucDonViTienTeByID(Guid id)
        {
            string query = "SELECT * FROM NH_DM_LoaiTienTe WHERE ID = @ID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_DM_LoaiTienTeModel>(query.ToString(), param: new { ID = id }, commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeleteDonViTienTe(Guid id)
        {
            string query = "DELETE NH_DM_LoaiTienTe WHERE ID = @ID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), param: new { ID = id }, commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool SaveDonViTienTe(NH_DM_LoaiTienTe data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.ID == null || data.ID == Guid.Empty)
                {
                    var entity = new NH_DM_LoaiTienTe();
                    entity.MapFrom(data);
                    conn.Insert(entity, trans);
                }
                else
                {
                    var entity = conn.Get<NH_DM_LoaiTienTe>(data.ID, trans);
                    if (entity == null) return false;
                    entity.sMaTienTe = data.sMaTienTe;
                    entity.sTenTienTe = data.sTenTienTe;
                    entity.sMoTaChiTiet = data.sMoTaChiTiet;
                    conn.Update(entity, trans);
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<NH_DM_LoaiTienTe> GetListTienTe(Guid? id)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DM_LoaiTienTe ");
            query.Append("ORDER BY sTenTienTe");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_LoaiTienTe>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }
        #endregion
    }
}

