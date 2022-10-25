using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using Viettel.Models.BaoHiemXaHoi;
using VIETTEL.Helpers;

namespace Viettel.Services
{
    public interface IBaoHiemXaHoiService : IServiceBase
    {
        #region Danh muc don vi Bao hiem xa hoi
        /// <summary>
        /// Lấy danh sách danh mục đơn vị BHXH theo tiêu chí tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging info</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <returns></returns>
        IEnumerable<DanhMucBHXHDonViViewModel> GetAllDanhMucBHXHDonVi(ref PagingInfo _paging, int iNamLamViec, string sMaDonVi = "", string sTenDonVi = "", Guid? iID_BHXH_DonVi_ParentID = null, string iID_MaDonVi_NS = "");
        /// <summary>
        /// Lấy thông tin đơn vị bảo hiểm xã hội theo ID
        /// </summary>
        /// <param name="iID_BHXH_DonViID">ID đơn vị BHXH</param>
        /// <returns></returns>
        DanhMucBHXHDonViViewModel GetDanhMucBHXHDonViById(Guid iID_BHXH_DonViID);
        /// <summary>
        /// Lấy danh sách đơn vị bảo hiểm xã hội theo năm làm việc
        /// </summary>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="iID_BHXH_DonViID">ID đơn vị BHXH</param>
        /// <returns></returns>
        IEnumerable<BHXH_DonVi> GetListDonViBHXH(int iNamLamViec, string sUserName, Guid? iID_BHXH_DonViID = null);
        /// <summary>
        /// get list dv bhxh by dv ns
        /// </summary>
        /// <param name="iID_NS_DonVi"></param>
        /// <returns></returns>
        IEnumerable<BHXH_DonVi> GetListDonViBHXHByDonViNS(string iID_NS_DonVi);
        /// <summary>
        /// Lấy danh sách đơn vị Ngân sách theo năm làm việc
        /// </summary>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="iID_MaDonViBHXH">ID đơn vị BHXH</param>
        /// <returns></returns>
        IEnumerable<NS_DonVi> GetListDonViNS(int iNamLamViec, Guid? iID_MaDonViBHXH);
        /// <summary>
        /// Check trùng mã đơn vị BHXH
        /// </summary>
        /// <param name="iID_MaDonViBHXH">Mã đơn vị BHXH</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="iID_BHXH_DonViID">ID đơn vị BHXH</param>
        /// <returns></returns>
        bool CheckExistMaDonVi(string iID_MaDonViBHXH, int iNamLamViec, Guid? iID_BHXH_DonViID);
        /// <summary>
        /// Insert đơn vị bảo hiểm xã hội
        /// </summary>
        /// <param name="data">Data đơn vị BHXH</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="UserLogin">User login</param>
        /// <returns></returns>
        bool InsertDonViBHXH(BHXH_DonVi data, int iNamLamViec, string UserLogin);
        /// <summary>
        /// Xóa đơn vị bảo hiểm xã hội
        /// </summary>
        /// <param name="iID_BHXH_DonViID">ID đơn vị bảo hiểm xã hội</param>
        /// <returns></returns>
        bool DeleteDonViBHXH(Guid iID_BHXH_DonViID);
        /// <summary>
        /// Lấy danh sách đơn vị ngân sách theo năm làm việc (dùng cho tìm kiếm)
        /// </summary>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <returns></returns>
        IEnumerable<NS_DonVi> GetListAllDonViNS(int iNamLamViec);
        #endregion

        #region Quản lý bệnh nhân điều trị BHXH
        /// <summary>
        /// Lấy danh sách bệnh nhân điều trị bảo hiểm xã hội theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging info</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="iThang">Tháng làm việc</param>
        /// <param name="iID_MaDonViBHXH">Mã đơn vị BHXH</param>
        /// <param name="iID_MaDonViNS">Mã đơn vị Ngân sách</param>
        /// <returns></returns>
        IEnumerable<QLBenhNhanBHXHViewModel> GetAllBenhNhanDieuTriBHXH(ref PagingInfo _paging, int iNamLamViec, int? iThangFrom, int? iThangTo, string iID_MaDonViBHXH, string iID_MaDonViNS, int? iSoNgayDieuTri, string sHoTen, string sMaThe);
        /// <summary>
        /// Save bệnh nhân BHXH
        /// </summary>
        /// <param name="danhSach">Danh sách bệnh nhân</param>
        /// <param name="iID_MaDonViBHXH">Mã đơn vị BHXH</param>
        /// <param name="iThang">Tháng</param>
        /// <param name="sMoTa">Mô tả</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="UserName">User login</param>
        /// <returns></returns>
        //bool SaveBenhNhanBHXH(List<BenhNhanBaoHiemQuery> lstBenhNhan, List<TepBenhNhanBHXHViewModel> lstTepBenhNhan/*, string iID_MaDonViBHXH, int iThang, string sMoTa*/, int iNamLamViec, string UserName);
        bool SaveBenhNhanBHXH(List<BenhNhanBaoHiemQuery> lstBenhNhanUpDate, string lstIdImport, string UserName, ref int countError);

        IEnumerable<NS_DonVi> GetDonviNSByThang(int iThang, int iNamLamViec);

        IEnumerable<BHXH_DonVi> GetDonviBHXHByDonViNS(string sMaDonViNS, int iNamLamViec);

        IEnumerable<TepBenhNhanBHXHViewModel> GetAllFileBenhNhanBHXH(ref PagingInfo _paging, int iNamLamViec, int? iThang, string sMoTa, string sTenFile);

        bool CheckExistBenhNhan(string maBH, string ngayVaoVien, string ngayRaVien);

        bool InsertFileDonViBHXH(List<DanhMucBHXHDonViViewModel> lstDonVi, int iNamLamViec, string UserLogin);

        bool SaveFileAndImportBenhNhan(int iNamLamViec, List<TepBenhNhanBHXHViewModel> lstTepBenhNhan, List<ImportBHXHModel> lstImport);
        bool SaveBenhNhanTempAndError(int iNamLamViec, int? iThang, Guid iID_ImportID, DataTable tableBNTemp, DataTable tableBNError, ref bool bIsErrorMaDonVi);

        IEnumerable<BenhNhanBHXHTempViewModel> GetBenhNhanImportTemp(List<Guid> lstImport, int iPage = 1);
        List<BenhNhanBHXHTempViewModel> GetBenhNhanUpdateImportTemp(List<Guid> lstImport, List<BenhNhanBaoHiemQuery> lstUpdate, int iPage = 1);
        IEnumerable<BHXH_BenhNhanError> GetErrorByLine(List<TTblImportLine> lstLine);
        IEnumerable<NS_DonVi> GetAllDonViNS(int iNamLamViec);
        #endregion

        #region Báo cáo bệnh nhân điều trị nội trú BHXH
        //IEnumerable<QLBenhNhanBHXHViewModel> GetBaoCaoBenhNhanNoiTru(int iNamLamViec, int? iThang, string sMaDonViNS, string sMaDonViBHXH);
        IEnumerable<QLBenhNhanBHXHViewModel> GetBaoCaoBenhNhanNoiTru(int iNamLamViec, int? iThangBatDau, int? iThangKetThuc, string sMaDonViBHXHParent);
        #endregion
        List<TongHopBenhNhanDonVi> GetTongHopBenhNhanTheoDonVi(int iNamLamViec, string lstDonViBHXH);
        List<BHXHDonViTree> GetTreeBHXHDonVi(string sMaDonViNS = null, string sMaDonViBHXH = null, string strDonviBHXH = null);
        List<BHXHDonViTree> GetDataBaoCaoTongHop(int iNamLamViec, string lstDonViBHXH);
    }
    public class BaoHiemXaHoiService : IBaoHiemXaHoiService
    {
        #region Khoi tao
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IBaoHiemXaHoiService _default;
        private readonly INganSachService _nganSachService;
        private readonly IDanhMucService _dmService;
        public BaoHiemXaHoiService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
            _nganSachService = NganSachService.Default;
            _dmService = DanhMucService.Default;
        }

        public static IBaoHiemXaHoiService Default
        {
            get { return _default ?? (_default = new BaoHiemXaHoiService()); }
        }
        #endregion

        #region Danh muc don vi Bao hiem xa hoi
        public IEnumerable<DanhMucBHXHDonViViewModel> GetAllDanhMucBHXHDonVi(ref PagingInfo _paging, int iNamLamViec, string sMaDonVi = "", string sTenDonVi = "", Guid? iID_BHXH_DonVi_ParentID = null, string iID_MaDonVi_NS = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("sTenDonVi", sTenDonVi);
                lstParam.Add("iID_BHXH_DonVi_ParentID", iID_BHXH_DonVi_ParentID);
                lstParam.Add("iID_MaDonVi_NS", iID_MaDonVi_NS);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhMucBHXHDonViViewModel>("proc_get_all_danhmuc_donvi_bhxh_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhMucBHXHDonViViewModel GetDanhMucBHXHDonViById(Guid iID_BHXH_DonViID)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT bhxh.*, parent.sTen AS sTenBHXHDonViParent, nsdv.sTen AS sTenNSDonViMapping FROM BHXH_DonVi bhxh ");
            query.Append("LEFT JOIN BHXH_DonVi parent ON parent.iID_BHXH_DonViID = bhxh.iID_ParentID ");
            query.Append("LEFT JOIN NS_DonVi nsdv ON nsdv.iID_MaDonVi = bhxh.iID_NS_MaDonVi ");
            query.AppendFormat("WHERE bhxh.iID_BHXH_DonViID = '{0}'", iID_BHXH_DonViID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhMucBHXHDonViViewModel>(query.ToString(),
                   commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<BHXH_DonVi> GetListDonViBHXH(int iNamLamViec, string sUserName, Guid? iID_BHXH_DonViID = null)
        {
            StringBuilder query = new StringBuilder();
            //query.AppendFormat("SELECT * FROM BHXH_DonVi WHERE iNamLamViec = {0} ", iNamLamViec);
            //query.Append("SELECT * FROM BHXH_DonVi ");
            //if (iID_BHXH_DonViID != null)
            //    query.AppendFormat("WHERE iID_BHXH_DonViID <> '{0}' ", iID_BHXH_DonViID);
            //query.Append("ORDER BY sTen");
            query.Append("SELECT * FROM BHXH_DonVi WHERE iID_NS_MaDonViGoc IN ");
            query.Append("(SELECT DISTINCT b.iID_MaDonVi FROM NS_NguoiDung_DonVi a, ns_donvi b WHERE a.iID_MaDonVi = b.iID_MaDonVi and ");
            query.AppendFormat("a.iNamLamViec = {0} and b.iNamLamViec_DonVi = {1} and a.sMaNguoiDung = '{2}' and ", iNamLamViec, iNamLamViec, sUserName);
            query.Append("a.iTrangThai = 1 and b.iTrangThai = 1) ");
            if (iID_BHXH_DonViID != null)
                query.AppendFormat("AND iID_BHXH_DonViID <> '{0}' ", iID_BHXH_DonViID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXH_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<BHXH_DonVi> GetListDonViBHXHByDonViNS(string iID_NS_DonVi)
        {
            var sql = FileHelpers.GetSqlQuery("bhxh_get_donvi_bhxh_by_donvi_ns.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXH_DonVi>(sql,
                    param: new
                    {
                        iID_NS_DonVi
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<NS_DonVi> GetListDonViNS(int iNamLamViec, Guid? iID_MaDonViBHXH)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT nsdv.* FROM NS_DonVi nsdv WHERE nsdv.iNamLamViec_DonVi = {0} ", iNamLamViec);
            query.AppendFormat("AND nsdv.iID_MaDonVi NOT IN(SELECT ISNULL(bhxhdv.iID_NS_MaDonVi,'') FROM BHXH_DonVi bhxhdv WHERE bhxhdv.iNamLamViec = {0} ", iNamLamViec);
            if (iID_MaDonViBHXH != null && iID_MaDonViBHXH != Guid.Empty)
                query.AppendFormat("AND bhxhdv.iID_BHXH_DonViID <> '{0}' ", iID_MaDonViBHXH);
            query.Append(") ORDER BY nsdv.sTen");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }
        public bool CheckExistMaDonVi(string iID_MaDonViBHXH, int iNamLamViec, Guid? iID_BHXH_DonViID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM BHXH_DonVi WHERE iNamLamViec = {0} AND iID_MaDonViBHXH = '{1}' ", iNamLamViec, iID_MaDonViBHXH);
            if(iID_BHXH_DonViID != null && iID_BHXH_DonViID != Guid.Empty)
                query.AppendFormat("AND iID_BHXH_DonViID <> '{0}'", iID_BHXH_DonViID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXH_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items.ToList().Count() > 0;
            }
        }

        public bool InsertDonViBHXH(BHXH_DonVi data, int iNamLamViec, string UserLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                //conn.Open();
                //var trans = conn.BeginTransaction();
                if (data.iID_BHXH_DonViID == null || data.iID_BHXH_DonViID == Guid.Empty)
                {
                    if(data.iID_ParentID == null || data.iID_ParentID == Guid.Empty)
                    {
                        data.iID_NS_MaDonViGoc = data.iID_NS_MaDonVi;
                    }
                    else
                    {
                        var donViParent = conn.Get<BHXH_DonVi>(data.iID_ParentID/*, trans*/);
                        if (donViParent == null)
                        {
                            return false;
                        }
                        data.iID_NS_MaDonViGoc = donViParent.iID_NS_MaDonViGoc;
                    }
                    data.iNamLamViec = iNamLamViec;
                    data.dNgayTao = DateTime.Now;
                    data.sID_MaNguoiDungTao = UserLogin;

                    conn.Insert<BHXH_DonVi>(data/*, trans*/);
                }
                else
                {
                    var entity = conn.Get<BHXH_DonVi>(data.iID_BHXH_DonViID/*, trans*/);
                    if (entity == null)
                    {
                        return false;
                    }

                    if(entity.iID_NS_MaDonVi != data.iID_NS_MaDonVi)
                    {
                        if (!string.IsNullOrEmpty(data.iID_NS_MaDonVi))
                        {
                            entity.iID_NS_MaDonViGoc = data.iID_NS_MaDonVi;
                        }
                        else if (data.iID_ParentID != null && data.iID_ParentID != Guid.Empty)
                        {
                            var donViParent = conn.Get<BHXH_DonVi>(data.iID_ParentID/*, trans*/);
                            if (donViParent == null)
                            {
                                return false;
                            }
                            data.iID_NS_MaDonViGoc = donViParent.iID_NS_MaDonViGoc;
                        }
                        StringBuilder queryDelete = new StringBuilder();
                        queryDelete.AppendFormat("UPDATE BHXH_DonVi SET iID_NS_MaDonViGoc = '{0}' WHERE iID_NS_MaDonVi = '{1}' AND iNamLamViec = {2};", data.iID_NS_MaDonVi, entity.iID_NS_MaDonVi, iNamLamViec);
                        var excute = conn.Execute(queryDelete.ToString(), /*trans,*/
                                            commandType: CommandType.Text);
                    }
                    entity.iID_MaDonViBHXH = data.iID_MaDonViBHXH;
                    entity.sTen = data.sTen;
                    entity.iID_ParentID = data.iID_ParentID;
                    entity.iID_NS_MaDonVi = data.iID_NS_MaDonVi;
                    entity.bDoanhNghiep = data.bDoanhNghiep;
                    entity.sID_MaNguoiDungSua = UserLogin;
                    entity.dNgaySua = DateTime.Now;
                    if (entity.iSoLanSua == null)
                        entity.iSoLanSua = 1;
                    else
                        entity.iSoLanSua += 1;

                    conn.Update<BHXH_DonVi>(entity/*, trans*/);
                }
                // commit to db
                //trans.Commit();
            }

            return true;
        }

        public bool DeleteDonViBHXH(Guid iID_BHXH_DonViID)
        {
            StringBuilder queryCheckChild = new StringBuilder();
            queryCheckChild.AppendFormat("SELECT * FROM BHXH_DonVi WHERE iID_ParentID = '{0}'", iID_BHXH_DonViID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var excuteCheckChild = conn.Query(queryCheckChild.ToString(),
                   commandType: CommandType.Text);
                if (excuteCheckChild.ToList().Count() > 0)
                    return false;
            }
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE BHXH_DonVi WHERE iID_BHXH_DonViID = '{0}'", iID_BHXH_DonViID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var excute = conn.Execute(query.ToString(),
                   commandType: CommandType.Text);
                return excute >= 0;
            }
        }

        public IEnumerable<NS_DonVi> GetListAllDonViNS(int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT nsdv.* FROM NS_DonVi nsdv WHERE nsdv.iNamLamViec_DonVi = {0} ORDER BY nsdv.sTen", iNamLamViec);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public bool InsertFileDonViBHXH(List<DanhMucBHXHDonViViewModel> lstDonVi, int iNamLamViec, string UserLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                //conn.Open();
                //var trans = conn.BeginTransaction();
                foreach(var item in lstDonVi)
                {
                    var entity = new BHXH_DonVi();
                    entity.iID_BHXH_DonViID = item.iID_BHXH_DonViID;
                    entity.iID_MaDonViBHXH = item.iID_MaDonViBHXH;
                    entity.sTen = item.sTen;
                    entity.iID_ParentID = item.iID_ParentID;
                    entity.iNamLamViec = iNamLamViec;
                    entity.dNgayTao = DateTime.Now;
                    entity.sID_MaNguoiDungTao = UserLogin;
                    conn.Insert<BHXH_DonVi>(entity/*, trans*/);
                    //thiếu Mã đơn vị gốc, Mã đơn vị Mapping
                } 
                // commit to db
                //trans.Commit();
            }

            return true;
        }
        #endregion

        #region Quản lý bệnh nhân điều trị BHXH
        public IEnumerable<QLBenhNhanBHXHViewModel> GetAllBenhNhanDieuTriBHXH(ref PagingInfo _paging, int iNamLamViec, int? iThangFrom, int? iThangTo, string iID_MaDonViBHXH, string iID_MaDonViNS, int? iSoNgayDieuTri, string sHoTen, string sMaThe)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("iThangFrom", iThangFrom);
                lstParam.Add("iThangTo", iThangTo);
                lstParam.Add("sMaDonViBHXH", iID_MaDonViBHXH);
                lstParam.Add("sMaDonViNS", iID_MaDonViNS);
                lstParam.Add("iSoNgayDieuTri", iSoNgayDieuTri);
                lstParam.Add("sHoTen", sHoTen);
                lstParam.Add("sMaThe", sMaThe);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<QLBenhNhanBHXHViewModel>("proc_get_all_benhnhan_dieutri_bhxh_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool SaveBenhNhanBHXH(List<BenhNhanBaoHiemQuery> lstBenhNhanUpDate, string lstIdImport, string UserName, ref int countError)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstImportIds = QLVonDauTuService.ConvertDataToTableTypeDefined("t_tbl_Ids", "iId", lstIdImport.Split(',').Select(n => Guid.Parse(n)).ToList());
                if (lstBenhNhanUpDate == null)
                    lstBenhNhanUpDate = new List<BenhNhanBaoHiemQuery>();
                var lstBenhNhanUpdate = QLVonDauTuService.ConvertDataToTableDefined("t_tbl_benhnhanbaohiem", lstBenhNhanUpDate);

                conn.Execute("sp_bhxh_insert_benhnhanbhxh_by_import",
                    new
                    {
                        sUserCreate = UserName,
                        lstImportId = lstImportIds.AsTableValuedParameter("t_tbl_Ids"),
                        lstDataUpdate = lstBenhNhanUpdate.AsTableValuedParameter("t_tbl_benhnhanbaohiem")
                    }, commandType: CommandType.StoredProcedure);

                StringBuilder queryCountError = new StringBuilder();
                queryCountError.AppendFormat("SELECT COUNT(*) FROM (SELECT iLine, iID_ImportID FROM BHXH_BenhNhanError WHERE iID_ImportID IN ({0}) GROUP BY iLine, iID_ImportID) as tbl", 
                    string.Join(",", lstIdImport.Split(',').Select(n => string.Format("'{0}'", n))));
                countError = conn.QueryFirstOrDefault<int>(queryCountError.ToString(), commandType: CommandType.Text);
            }
            return true;
        }

        public IEnumerable<NS_DonVi> GetDonviNSByThang(int iThang, int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT(nsdv.iID_MaDonVi), nsdv.iID_Ma, nsdv.sTen FROM BHXH_BenhNhan bn ");
            query.Append("INNER JOIN BHXH_DonVi bhxhdv ON bhxhdv.iID_MaDonViBHXH = bn.iID_MaDonVi ");
            query.Append("INNER JOIN NS_DonVi nsdv ON nsdv.iID_MaDonVi = bhxhdv.iID_NS_MaDonViGoc ");
            query.AppendFormat("WHERE bn.iThang = {0} AND bn.iNamLamViec = {1} ", iThang, iNamLamViec);
            query.AppendFormat("AND bhxhdv.iNamLamViec = {0} AND nsdv.iNamLamViec_DonVi = {1} ", iNamLamViec, iNamLamViec);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<BHXH_DonVi> GetDonviBHXHByDonViNS(string sMaDonViNS, int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM BHXH_DonVi WHERE iNamLamViec = {0} AND iID_NS_MaDonViGoc = '{1}'", iNamLamViec, sMaDonViNS);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXH_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<TepBenhNhanBHXHViewModel> GetAllFileBenhNhanBHXH(ref PagingInfo _paging, int iNamLamViec, int? iThang, string sMoTa, string sTenFile)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("iThang", iThang);
                lstParam.Add("sMoTa", sMoTa);
                lstParam.Add("sTenFile", sTenFile);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<TepBenhNhanBHXHViewModel>("proc_get_all_file_benhnhan_bhxh_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool CheckExistBenhNhan(string maBH, string ngayVaoVien, string ngayRaVien)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT COUNT(*) FROM BHXH_BenhNhan WHERE sMaThe = '{0}' AND sNgayVaoVien = '{1}' AND sNgayRaVien = '{2}';", maBH, ngayVaoVien, ngayRaVien);
            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.Execute(query.ToString(),
                   commandType: CommandType.Text);
                return count > 0;
            }
        }

        public bool SaveFileAndImportBenhNhan(int iNamLamViec, List<TepBenhNhanBHXHViewModel> lstTepBenhNhan, List<ImportBHXHModel> lstImport)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                //conn.Open();
                //var trans = conn.BeginTransaction();
                string lstThang = string.Join(",", lstTepBenhNhan.Select(x => x.iThang).ToList());
                StringBuilder queryDelete = new StringBuilder();
                queryDelete.AppendFormat("DELETE BHXH_TepBenhNhan WHERE iNamLamViec = {0} AND iThang IN ({1});", iNamLamViec, lstThang);
                queryDelete.AppendFormat("DELETE BHXH_BenhNhanTempImport WHERE iNamLamViec = {0} AND iThang IN ({1});", iNamLamViec, lstThang);
                var excute = conn.Execute(queryDelete.ToString(), /*trans,*/
                   commandType: CommandType.Text);

                conn.Insert<BHXH_TepBenhNhan>(lstTepBenhNhan/*, trans*/);
                conn.Insert<BHXH_BenhNhanTempImport>(lstImport/*, trans*/);
            }
            return true;
        }

        public bool SaveBenhNhanTempAndError(int iNamLamViec, int? iThang, Guid iID_ImportID, DataTable tableBNTemp, DataTable tableBNError, ref bool bIsErrorMaDonVi)
        {
            var sqlCheckExistDVBHXH = FileHelpers.GetSqlQuery("check_khong_ton_tai_dv_bhxh.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();

                if(iThang != null)
                {
                    StringBuilder queryDelete = new StringBuilder();
                    queryDelete.AppendFormat("DELETE BHXH_BenhNhanTemp WHERE iID_ImportID IN (SELECT iID_ImportID FROM BHXH_BenhNhanTempImport WHERE iThang = {0} AND iNamLamViec = {1});", iThang.Value, iNamLamViec);
                    queryDelete.AppendFormat("DELETE BHXH_BenhNhanError WHERE iID_ImportID IN (SELECT iID_ImportID FROM BHXH_BenhNhanTempImport WHERE iThang = {0} AND iNamLamViec = {1});", iThang.Value, iNamLamViec);

                    var excute = conn.Execute(queryDelete.ToString(), /*trans,*/
                       commandType: CommandType.Text);
                }
                
                using (var bulkInsert = new SqlBulkCopy(conn))
                {
                    //thieu phan delete du lieu da ton tai
                    bulkInsert.DestinationTableName = tableBNTemp.TableName;
                    bulkInsert.ColumnMappings.Add("sSTT", "sSTT");
                    bulkInsert.ColumnMappings.Add("sHoTen", "sHoTen");
                    bulkInsert.ColumnMappings.Add("sNgaySinh", "sNgaySinh");
                    bulkInsert.ColumnMappings.Add("sGioiTinh", "sGioiTinh");
                    bulkInsert.ColumnMappings.Add("sCapBac", "sCapBac");
                    bulkInsert.ColumnMappings.Add("sMaThe", "sMaThe");
                    bulkInsert.ColumnMappings.Add("sNgayVaoVien", "sNgayVaoVien");
                    bulkInsert.ColumnMappings.Add("sNgayRaVien", "sNgayRaVien");
                    bulkInsert.ColumnMappings.Add("sSoNgayDieuTri", "sSoNgayDieuTri");
                    bulkInsert.ColumnMappings.Add("sMaBV", "sMaBV");
                    bulkInsert.ColumnMappings.Add("sTenBV", "sTenBV");
                    bulkInsert.ColumnMappings.Add("sMaDV", "sMaDV");
                    bulkInsert.ColumnMappings.Add("sTenDonVi", "sTenDonVi");
                    bulkInsert.ColumnMappings.Add("sGhiChu", "sGhiChu");
                    bulkInsert.ColumnMappings.Add("bError", "bError");
                    bulkInsert.ColumnMappings.Add("iID_ImportID", "iID_ImportID");
                    bulkInsert.ColumnMappings.Add("iLine", "iLine");

                    bulkInsert.WriteToServer(tableBNTemp);
                }

                using (var bulkInsert = new SqlBulkCopy(conn))
                {
                    //thieu phan delete du lieu da ton tai

                    bulkInsert.DestinationTableName = tableBNError.TableName;
                    bulkInsert.ColumnMappings.Add("iID_ImportID", "iID_ImportID");
                    bulkInsert.ColumnMappings.Add("iLine", "iLine");
                    bulkInsert.ColumnMappings.Add("sPropertyName", "sPropertyName");
                    bulkInsert.ColumnMappings.Add("sMessage", "sMessage");

                    bulkInsert.WriteToServer(tableBNError);
                }

                var result = conn.ExecuteScalar<int>(
                    sqlCheckExistDVBHXH,
                    param: new
                    {
                        iIDImportID = iID_ImportID
                    },
                    commandType: CommandType.Text);
                bIsErrorMaDonVi = result > 0;
            }
            return true;
        }

        public IEnumerable<BenhNhanBHXHTempViewModel> GetBenhNhanImportTemp(List<Guid> lstImport, int iPage = 1)
        {
            string lstThang = string.Join(",", lstImport.Select(x => string.Format("'{0}'", x.ToString())));
            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT bnt.*, tip.iThang AS iThangBN "
                            + " FROM BHXH_BenhNhanTemp bnt "
                            + " LEFT JOIN BHXH_BenhNhanTempImport tip ON tip.iID_ImportID = bnt.iID_ImportID "
                            + " WHERE bnt.iID_ImportID IN (" + lstThang + ")"
                            + " ORDER BY iLine "
                            + " OFFSET " + ((iPage - 1) * 50) + " ROWS FETCH NEXT 50 ROWS ONLY; ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BenhNhanBHXHTempViewModel>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public List<BenhNhanBHXHTempViewModel> GetBenhNhanUpdateImportTemp(List<Guid> lstImport, List<BenhNhanBaoHiemQuery> lstUpdate, int iPage = 1)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstImportIds = QLVonDauTuService.ConvertDataToTableTypeDefined("t_tbl_Ids", "iId", lstImport);
                if (lstUpdate == null)
                    lstUpdate = new List<BenhNhanBaoHiemQuery>();
                var lstBenhNhanUpdate = QLVonDauTuService.ConvertDataToTableDefined("t_tbl_benhnhanbaohiem", lstUpdate);

                var data = conn.Query<BenhNhanBHXHTempViewModel>("sp_bhxh_get_benhnhanimporterror",
                    new
                    {
                        iPage = iPage,
                        lstImportId = lstImportIds.AsTableValuedParameter("t_tbl_Ids"),
                        lstDataUpdate = lstBenhNhanUpdate.AsTableValuedParameter("t_tbl_benhnhanbaohiem")
                    }, commandType: CommandType.StoredProcedure);
                if(data != null) return data.ToList();
            }
            return new List<BenhNhanBHXHTempViewModel>();
        }

        public IEnumerable<BHXH_BenhNhanError> GetErrorByLine(List<TTblImportLine> lstLine)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var conditions = QLVonDauTuService.ConvertDataToTableDefined("t_tbl_importline", lstLine);

                SqlParameter dtDetailParam = new SqlParameter();
                dtDetailParam.ParameterName = "data";
                dtDetailParam.Value = conditions;
                dtDetailParam.TypeName = "t_tbl_importline";
                dtDetailParam.SqlDbType = SqlDbType.Structured;

                var data = conn.Query<BHXH_BenhNhanError>("sp_get_importerror_by_baohiem", new { data = conditions.AsTableValuedParameter("t_tbl_importline") }, commandType: CommandType.StoredProcedure);
                if (data == null) return new List<BHXH_BenhNhanError>();
                return data.ToList();
            }
        }

        public IEnumerable<NS_DonVi> GetAllDonViNS(int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT nsdv.* FROM NS_DonVi nsdv WHERE nsdv.iNamLamViec_DonVi = {0} ", iNamLamViec);
            query.Append(" ORDER BY nsdv.sTen");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Báo cáo bệnh nhân điều trị nội trú BHXH

        public IEnumerable<QLBenhNhanBHXHViewModel> GetBaoCaoBenhNhanNoiTru(int iNamLamViec, int? iThangBatDau, int? iThangKetThuc, string sMaDonViBHXHParent)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT bn.* FROM BHXH_BenhNhan bn ");
            query.Append("LEFT JOIN BHXH_DonVi bhxhdv ON bhxhdv.iID_MaDonViBHXH = bn.iID_MaDonVi ");
            query.AppendFormat("WHERE bn.iNamLamViec = {0} ", iNamLamViec);
            if (iThangBatDau != null)
                query.AppendFormat("AND bn.iThang >= {0} ", iThangBatDau);
            if (iThangKetThuc != null)
                query.AppendFormat("AND bn.iThang <= {0} ", iThangKetThuc);
            if (!string.IsNullOrEmpty(sMaDonViBHXHParent))
                query.AppendFormat("AND bhxhdv.iID_MaDonViBHXH LIKE CONCAT('{0}',N'%') ", sMaDonViBHXHParent);
            query.Append("ORDER BY bn.iThang ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<QLBenhNhanBHXHViewModel>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Báo cáo tổng hợp
        public List<TongHopBenhNhanDonVi> GetTongHopBenhNhanTheoDonVi(int iNamLamViec, string lstDonViBHXH)
        {
            var sql = FileHelpers.GetSqlQuery("bhxh_count_luotdieutri_songaydieutri_theo_donvi");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<TongHopBenhNhanDonVi>(sql,
                    param: new
                    {
                        iNamLamViec,
                        lstDonViBHXH
                    },
                    commandType: CommandType.Text).ToList();

                return items;
            }
        }

        public List<BHXHDonViTree> GetTreeBHXHDonVi(string sMaDonViNS, string sMaDonViBHXH, string strDonviBHXH)
        {
            var sql = FileHelpers.GetSqlQuery("bhxh_get_tree_donvi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXHDonViTree>(sql,
                    param: new {
                        sMaDonViNS,
                        sMaDonViBHXH,
                        strDonviBHXH
                    },
                    commandType: CommandType.Text).ToList();

                return items;
            }
        }

        public List<BHXHDonViTree> GetDataBaoCaoTongHop(int iNamLamViec, string lstDonViBHXH)
        {
            var sql = FileHelpers.GetSqlQuery("bhxh_get_data_baocao_tonghop.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<BHXHDonViTree>(sql,
                    param: new
                    {
                        iNamLamViec,
                        lstDonViBHXH
                    },
                    commandType: CommandType.Text).ToList();

                return items;
            }
        }
        #endregion
    }
}
