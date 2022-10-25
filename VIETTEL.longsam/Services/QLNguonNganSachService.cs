using Dapper;
using Viettel.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Data;
using Viettel.Domain.Interfaces.Services;
using Viettel.Models.QLNguonNganSach;
using VIETTEL.Helpers;
using DomainModel;
using DapperExtensions;
using VIETTEL.Areas.QLNguonNganSach.Models;
using Newtonsoft.Json;
//using DomainModel;

namespace Viettel.Services
{
    public interface IQLNguonNganSachService : IServiceBase
    {
        #region Common

        #endregion

        #region QL Giao du toan cho don vi
        /// <summary>
        /// Get NNS Giao du toan cho don vi
        /// </summary>
        /// <param name="sSoChungTu">So chung tu</param>
        /// <param name="sNoiDung">Noi dung</param>
        /// <param name="sUserName">UserName</param>
        /// <returns></returns>
        IEnumerable<NNS_DuToan> GetAllNNSGiaoDuToanChoDV(ref PagingInfo _paging, string sSoChungTu = "", string sNoiDung = "", string sMaLoaiDuToan = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sSoCongVan = "", DateTime? dNgayCongVanTu = null, DateTime? dNgayCongVanDen = null, string sUserName = "");
        /// <summary>
        /// Lay Danh muc Loai du toan
        /// </summary>
        /// <returns></returns>
        List<DM_LoaiDuToan> GetAllLoaiDuToan(string sMaLoaiDuToan = "", string sUserName = "");
        /// <summary>
        /// Lay NNS Giao du toan cho don vi theo id du toan
        /// </summary>
        /// <param name="iID_DuToan">id du toan</param>
        /// <returns></returns>
        NNS_DuToan GetNNSGiaoDuToanByID(Guid iID_DuToan);

        /// <summary>
        /// Insert database
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="listDotGom">listDotGom</param>
        /// <param name="sUserLogin">user login</param>
        /// <returns></returns>
        bool InsertNNSDuToanGiaoDV(ref NNS_DuToan data, string listDotGomAsString, string listChungTuAsString, string ipSua, string sUserLogin);

        /// <summary>
        /// id du toan
        /// </summary>
        /// <param name="idDuToan">id Du toan</param>
        /// <returns></returns>

        NNS_DotNhan Get_NNS_DuToan_DotNhan_ById(Guid id);

        bool DeleteNNSGiaoDuToan(Guid idDuToan);

        /// <summary>
        /// Get max index giao du toan cho don vi
        /// </summary>
        /// <returns></returns>
        int GetMaxIndexNNSGiaoDuToan(string sUserName);

        List<NNSDuToanChiTietDataTableViewModel> GetListChiTietDuToanNew(string idDuToan, string sUserName, bool isSaoChep, List<NNSDuToanChiTietGetSoTienByDonViModel> aListXauNoiXa, string iID_NhiemVu, Dictionary<string, string> filters);

        /// <summary>
        /// cap nhat tong so tien cua du toan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool UpdateSumDuToan(string id);

        /// <summary>
        /// Proc lay du lieu tinh tong tien giao du toan cho don vi
        /// </summary>
        /// <param name="sUserName"></param>
        /// <returns></returns>

        List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDV(string sUserName);

        List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDauNamTruocChuyenSang(string sUserName);

        /// <summary>
        /// Proc lay du lieu tinh tong tien giao du toan cho don vi truoc va sau 30-9
        /// </summary>
        /// <param name="sUserName"></param>
        /// <param name="iID_DuToan"></param>
        /// <returns></returns>

        List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDVTruocVaSauBaMuoiThangChin(string sUserName, string iID_DuToan);

        /// <summary>
        /// Proc lay tong tien du toan chi tiet
        /// </summary>
        /// <param name="iID_DuToan"></param>
        /// <param name="sUserName"></param>
        /// <param name="sMaLoaiDuToan"></param>
        /// <returns></returns>

        List<NNSDuToanChiTietTongTienModel> GetTongTienDuToanChiTiet(string iID_DuToan, string sUserName, string sMaLoaiDuToan, string iID_NhiemVu = null);

        /// <summary>
        /// Check them moi loai du toan dau nam
        /// </summary>
        /// <param name="sUserName"></param>
        /// <param name="sMaLoaiDuToan"></param>
        /// <returns></returns>
        bool CheckCreateLoaiDuToanDauNam(string sUserName, string sMaLoaiDuToan);

        List<NNSDuToanExportDataModel> ExportData(int iNamLamViec, string sSoChungTu = "", string sNoiDung = "", string sMaLoaiDuToan = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sSoCongVan = "", DateTime? dNgayCongVanTu = null, DateTime? dNgayCongVanDen = null, string sUserName = "");

        #endregion

        #region QL phân nguồn BTC theo nội dung chi BQP
        /// <summary>
        /// Get NNS phân nguồn BTC theo nội dung chi BQP
        /// </summary>
        /// <param name="sSoChungTu">So chung tu</param>
        /// <param name="sNoiDung">Noi dung</param>
        /// <returns></returns>
        IEnumerable<NNS_PhanNguon> GetAllNNSPhanNguonBTCTheoNDChiBQP(ref PagingInfo _paging, string sSoChungTu, string sNoiDung);


        /// <summary>
        /// Get NNS_PhanNguon by ID
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        /// <returns></returns>
        NNS_PhanNguon GetNNSPhanNguonByID(Guid iId);

        /// <summary>
        /// Insert NNS_PhanNguon
        /// </summary>
        /// <param name="sNoiDung">sNoiDung</param>
        /// <param name="sSoChungTu">sSoChungTu</param>
        /// <param name="dNgayChungTu">dNgayChungTu</param>
        /// <param name="sSoQuyetDinh">dNgayChungTu</param>
        /// <param name="dNgayQuyetDinh">dNgayChungTu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        /// <returns></returns>
        bool InsertNNSPhanNguon(string sNoiDung, string sSoChungTu, DateTime? dNgayChungTu, string sSoQuyetDinh, DateTime? dNgayQuyetDinh, string sUserLogin);

        /// <summary>
        /// Update NNS_PhanNguon
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        /// <param name="sNoiDung">sNoiDung</param>
        /// <param name="sSoChungTu">sSoChungTu</param>
        /// <param name="dNgayChungTu">dNgayChungTu</param>
        /// <param name="sSoQuyetDinh">dNgayChungTu</param>
        /// <param name="dNgayQuyetDinh">dNgayChungTu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        bool UpdateNNSPhanNguon(Guid iId, string sNoiDung, string sSoChungTu, DateTime? dNgayChungTu, string sSoQuyetDinh, DateTime? dNgayQuyetDinh, string sIPSua, string sUserLogin);

        /// <summary>
        /// Delete NNS_PhanNguon by iId
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        bool deleteNNSPhanNguon(Guid iId);

        bool deleteNNSPhanNguonNDChi(Guid iIdPhanNguon);

        IEnumerable<NNSDMNguonViewModel> GetAllDMNguonBTCCap(ref PagingInfo _paging, DateTime? dNgayChungTu, string sUserLogin, Guid? iIdPhanNguon);

        /// <summary>
        /// Get all DM_NoiDungChi by condition
        /// </summary>
        /// <param name="code">sMaNoiDungChi</param>
        /// <param name="name">sTenNoiDungChi</param>
        /// <returns></returns>
        IEnumerable<NNSDMNoiDungChiViewModel> GetAllDMNoiDungChiBQP(Guid? iIdNguon, Guid? iIdPhanNguon, string sUserLogin);

        Decimal getSoTienCoTheChi(Guid? iIdNguon, Guid? iIdPhanNguon, decimal rSoTienConLai, string sUserLogin, string sTenNoiDungChi);

        DataTable GetAllDMNoiDungChiBQP2(Guid? iIdNguon, Guid? iIdPhanNguon, decimal rSoTienConLai, string sUserLogin, Dictionary<string, string> _filters);

        /// <summary>
        /// Insert NNS_PhanNguon
        /// </summary>
        /// <param name="sNoiDung">sNoiDung</param>
        /// <param name="sSoChungTu">sSoChungTu</param>
        /// <param name="dNgayChungTu">dNgayChungTu</param>
        /// <param name="sSoQuyetDinh">dNgayChungTu</param>
        /// <param name="dNgayQuyetDinh">dNgayChungTu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        /// <returns></returns>
        bool SaveNNSPhanNguonNDChi(IEnumerable<NNSPhanNguonNDChiTempViewModel> dataNew, IEnumerable<NNSPhanNguonNDChiTempViewModel> dataEdit, string sUserLogin, String sIPSua);

        int GetMaxiIndexNNSPhanNguon();

        IEnumerable<NNSPhanNguonNDChiTempViewModel> getNNSPhanNguonNDChiByIds(Guid? iIdNguon, Guid? iIdPhanNguon, Guid? iID_NoiDungChi, string sUserLogin);

        #endregion

        #region Báo cáo
        /// <summary>
        /// Lấy tổng số nguồn ngân sách BQP theo năm làm việc
        /// </summary>
        /// <param name="iNamLamViec">NNS_PhanNguon_NDChi.iNamLamViec</param>
        /// <returns></returns>
        IEnumerable<NNSNganSachTheoNamLamViec> GetTotalNganSachBQPByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, int dvt = 1);

        /// <summary>
        /// Lấy tổng số tiền dự toán theo năm làm việc
        /// </summary>
        /// <param name="iNamLamViec">NNS_DuToan.iNamLamViec</param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        IEnumerable<NNSDuToanTheoNamLamViecModel> GetTotalDuToanByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);

        /// <summary>
        /// Lấy dự toán chi tiết theo năm làm việc
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDetailDuToanByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// get chi tiet du toan theo noi dung chi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDetailDuToanTheoNoiDungChi(Guid? iNguonNganSach, int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// lay chi tiet du toan theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDuToanTheoNhiemVu(int iNamLamViec, int? iLoaiNganSach, string iSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);

        /// <summary>
        /// lay chi tiet du toan phan cap dau nam theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDuToanPhanCapDauNam(int iNamLamViec, int? iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);

        /// <summary>
        /// lay chi tiet du toan phan cap dau nam theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDuToanPhanCapDauNamTheoSoQuyetDinh(int iNamLamViec, int? iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);

        /// <summary>
        /// lay chi tiet du toan theo so quyet dinh va danh muc nguon
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        IEnumerable<RptNNSDuToanChiTietModel> GetDuToanTheoNhiemVuVaDanhMucNguon(Guid? iNguonNganSach, int iNamLamViec, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);

        /// <summary>
        /// lay danh sach tong hop du toan NS theo don vi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// lay danh sach tong hop du toan NS theo don vi bql
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoDonViBQL(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// lay danh sach tong hop du toan NS theo bql don vi 
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoBQLDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// lay danh sach tong hop qd bs du toan NS theo don vi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<RptChiTietTongHopQDBSDuToanNSNamModel> GetTongHopQDBSDuToanNSNamTheoDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);

        /// <summary>
        /// lay danh sach tong hop qd bs du toan NS theo don vi bql
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<RptChiTietTongHopQDBSDuToanNSNamModel> GetTongHopQDBSDuToanNSNamTheoDonViBQL(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1);
        #endregion

        #region QL đợt nhận
        /// <summary>
        /// Lấy đợt nhận theo ID
        /// </summary>
        /// <param name="iId">ID đợt nhận</param>
        /// <returns></returns>
        NNS_DotNhan GetDotNhanNguonNS(Guid iId);
        /// <summary>
        /// Lấy danh sách đợt nhận theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging info</param>
        /// <param name="sSoChungTu">Số chứng từ</param>
        /// <param name="sNoiDung">Nội dung</param>
        /// <param name="sMaLoaiDuToan">Mã loại dự toán</param>
        /// <returns></returns>
        IEnumerable<DotNhanViewModel> GetAllDotNhanNguonNS(ref PagingInfo _paging, int iNamLamViec, string sSoChungTu, string sNoiDung, string sMaLoaiDuToan, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo);
        /// <summary>
        /// Insert or Update đợt nhận
        /// </summary>
        /// <param name="data">data insert/update</param>
        /// <param name="sUserLogin">User login</param>
        /// <returns></returns>
        bool InsertDotNhanNguonNS(ref NNS_DotNhan data, string sUserLogin);

        List<DM_LoaiDuToan> GetAllDMLoaiDuToan(int iNamLamViec);
        void UpdateDotNhanChiTiet(NNS_DotNhanChiTiet data, string sUserLogin);
        List<BaoCaoTongHopNguonViewModel> BaoCaoTongHopNguon(Guid? iNguonNganSach, int iNamLamViec, int iSoCotBoSung, DateTime? dNgayFrom, DateTime? dNgayTo, int dvt = 1);
        DataTable GetChiTietDotNhanVaDotBoSung(Guid? iNguonNganSach, int iNamLamViec, DateTime? dNgayFrom, DateTime? dNgayTo, int dvt = 1, string sTenCot = "");
        bool DeleteDotNhanNguon(Guid id);
        IEnumerable<DotNhanBoSungTrongNamViewModel> GetAllDotNhanBoSungNam(Guid? iNguonNganSach, int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo);
        IEnumerable<DotNhanBoSungTrongNamViewModel> GetAllDotDuToan(Guid? iNguonNganSach, int iNamLamViec, int iSoCotBoSung, DateTime? dDateFrom, DateTime? dDateTo);
        NNS_DotNhanChiTiet GetDotNhanChiTiet(Guid iId);
        void InsertNNSDotNhanChiTiet(NNS_DotNhanChiTiet data, string sUserLogin);
        DataTable GetListDotNhanNguonChiTietDatatable(string iId, int iNamLamViec, Dictionary<string, string> filters);
        TinhTongSoTienBaoCaoDotNhanViewModel GetTongSoTienBaoCao(List<BaoCaoTongHopNguonViewModel> dataBaoCaoTongHop, IEnumerable<DotNhanBoSungTrongNamViewModel> dotNhanBoSungNam, IEnumerable<DotNhanBoSungTrongNamViewModel> dotDuToanNam);
        /// <summary>
        /// Get max index đợt nhận
        /// </summary>
        /// <returns></returns>
        int GetMaxIndexNNSDotNhan(int iNamLamViec);
        DataTable GetAllDMNoiDungChiBQPTheoDotNhan(Guid? iID_DotNhanChiTiet, string sMaLoaiDuToan, string sUserLogin, Dictionary<string, string> _filters);
        NNS_DotNhanChiTiet_NDChi GetDotNhanChiTietNDChi(Guid iID_NoiDungChi, Guid iID_DotNhanChiTiet);
        IEnumerable<SoKiemTraDotNhanChiTietNDChiModel> GetSoKiemTraDnctNDC(int iNamLamViec);
        bool CheckExistLoaiDuToan(int iNamLamViec, string sMaLoaiDuToan, Guid? iID_DotNhan);
        NNS_DotNhanChiTiet GetDotNhanChiTietByIdDotNhanIdNguon(Guid iID_DotNhan, Guid iID_Nguon, int iNamLamViec);

        List<NNSDotNhanExportDataModel> ExportData(string sMaLoaiDuToan = "", string sSoChungTu = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sNoiDung = "", string sUserName = "");

        #endregion

        #region Dự toán nhiệm vụ
        List<NNS_DuToan_NhiemVu_ViewGridModel> GetListDuToan_NhiemVu_ByIdDuToan(string iID_DuToan, string sUserName);
        //List<NNS_DuToan_NhiemVu_ChungTuViewModel> GetListChungTu(string sUserName, string idDuToan);
        /// <summary>
        /// get list chung tu chi tiet
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        List<NNS_DuToan_NhiemVu_ChungTuViewModel> GetListChungTuChiTiet(int iNamLamViec);
        bool CreateNNSDuToanNhiemVu(string idDuToan, string listIdChungTu, string sUserName, string sIP);
        List<NS_PhongBan> GetListDanhSachPhongBan();
        /// <summary>
        /// get list danh muc noi dung chi con
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        List<DMNoiDungChiViewModel> GetListDanhSachNoiDungChi(int iNamLamViec);
        List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuSoTienNoiDungChiTheoIdMaChungTu(string sUserName, string iID_NhiemVu);

        /// <summary>
        /// get so tien theo ma chung tu
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <returns></returns>
        List<NNSDuToanChiTietGetSoTienByDonViModel> GetSoTienTheoMaChungTu(Guid iID_MaChungTu);
        NNS_DuToan_NhiemVu GetNhiemVuById(string iID_NhiemVu);
        bool DeleteDuToanChiTietTheoNhiemVu(List<string> aListIdNhiemVu);
        bool DeleteDuToanChiTietDuToanID(string iIDDuToanID);

        #endregion

        #region Bao cao tong hop giao du toan theo LNS
        /// <summary>
        /// get số tổng dự toán
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<TongTien> GetTongDuToanLNS(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);
        /// <summary>
        /// get số tổng phân nguồn
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<TongTien> GetTongPhanNguonLNS(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);
        /// <summary>
        /// get chi tiet du toan theo noi dung chi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        List<ChiTietBoSung> GetChiTietBoSung(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1);
        #endregion

    }
    public class QLNguonNganSachService : IQLNguonNganSachService
    {
        #region Private
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IQLNguonNganSachService _default;
        private readonly INganSachService _nganSachService;
        private readonly IDanhMucService _dmService;
        #endregion

        public QLNguonNganSachService(
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

        public static IQLNguonNganSachService Default
        {
            get { return _default ?? (_default = new QLNguonNganSachService()); }
        }

        #region QL Giao du toan cho don vi

        public IEnumerable<NNS_DuToan> GetAllNNSGiaoDuToanChoDV(ref PagingInfo _paging, string sSoChungTu = "", string sNoiDung = "", string sMaLoaiDuToan = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sSoCongVan = "", DateTime? dNgayCongVanTu = null, DateTime? dNgayCongVanDen = null, string sUserName = "")
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("sSoChungTu", sSoChungTu);
                    lstParam.Add("sNoiDung", sNoiDung);
                    lstParam.Add("sMaLoaiDuToan", sMaLoaiDuToan);
                    lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                    lstParam.Add("dNgayQuyetDinhTu", dNgayQuyetDinhTu);
                    lstParam.Add("dNgayQuyetDinhDen", dNgayQuyetDinhDen);
                    lstParam.Add("sSoCongVan", sSoCongVan);
                    lstParam.Add("dNgayCongVanTu", dNgayCongVanTu);
                    lstParam.Add("dNgayCongVanDen", dNgayCongVanDen);
                    lstParam.Add("iNamLamViec", config.iNamLamViec);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var items = conn.Query<NNS_DuToan>("proc_get_all_giaodutoanchodv_paging", lstParam, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                    return items;

                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        public List<DM_LoaiDuToan> GetAllLoaiDuToan(string sMaLoaiDuToan = "", string sUserName = "")
        {
            var result = new List<DM_LoaiDuToan>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var listResult = conn.Query<DM_LoaiDuToan>($"SELECT * FROM DM_LoaiDuToan WHERE iTrangThai = 1", null, commandType: CommandType.Text);
                    if (listResult != null && listResult.Any())
                    {
                        if (!string.IsNullOrEmpty(sMaLoaiDuToan))
                        {
                            result = listResult.Where(x => x.sMaLoaiDuToan == sMaLoaiDuToan).ToList();
                        }
                        else
                        {
                            result = listResult.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;

        }

        public NNS_DuToan GetNNSGiaoDuToanByID(Guid iID_DuToan)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_by_id_giaodutoanchodonvi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<NNS_DuToan>(sql,
                        param: new
                        {
                            iID_DuToan
                        },
                        commandType: CommandType.Text);

                    return item.ToList().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool DeleteNNSGiaoDuToan(Guid idDuToan)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_delete_giaodutoanchodonvi.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            idDuToan
                        },
                        commandType: CommandType.Text);
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public int GetMaxIndexNNSGiaoDuToan(string sUserName)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                var sql = new StringBuilder();
                sql.AppendFormat("SELECT ISNULL(MAX(iIndex),0) FROM NNS_DuToan WHERE iNamLamViec = '{0}'", config.iNamLamViec);
                using (var conn = _connectionFactory.GetConnection())
                {

                    var entity = conn.QueryFirst<int>(
                        sql.ToString(),
                        commandType: CommandType.Text);

                    return entity;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return -1;
        }

        public NNS_DotNhan Get_NNS_DuToan_DotNhan_ById(Guid id)
        {
            var result = new NNS_DotNhan();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sqlDuToanDotNhan = new StringBuilder();
                    sqlDuToanDotNhan.AppendFormat("SELECT * FROM NNS_DuToan_DotNhan WHERE iID_DuToan = '{0}'", id);
                    var items = conn.Query<NNS_DuToan_DotNhan>(sqlDuToanDotNhan.ToString(), null, commandType: CommandType.Text);
                    if (items != null && items.Any())
                    {
                        var idDotNhan = items.FirstOrDefault().iID_DotNhan;
                        var sqlDotNhan = new StringBuilder();
                        sqlDotNhan.AppendFormat("SELECT * FROM NNS_DotNhan WHERE iID_DotNhan = '{0}'", idDotNhan);
                        var listDotNhan = conn.Query<NNS_DotNhan>(sqlDotNhan.ToString());
                        if (listDotNhan != null && listDotNhan.Any())
                        {
                            result = listDotNhan.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        public bool InsertNNSDuToanGiaoDV(ref NNS_DuToan data, string listDotGomAsString, string listChungTuAsString, string ipSua, string sUserLogin)
        {
            if (data == null)
            {
                return false;
            }
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                var listDuToanVaTLTHCreate = new List<NNS_DuToan_NS_DTBS_TLTH>();
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.iID_DuToan == null || data.iID_DuToan == Guid.Empty)
                    {
                        var iIndex = conn.QueryFirstOrDefault<int>("SELECT ISNULL(MAX(iIndex),0) FROM NNS_DuToan", null, trans, commandType: CommandType.Text);
                        data.iID_DuToan = Guid.NewGuid();
                        //data.sTenLoaiDuToan = "Dự toán ABC";
                        data.dNgayTao = DateTime.Now;
                        data.dNgaySua = DateTime.Now;
                        data.sID_MaNguoiDungTao = sUserLogin;
                        data.sID_MaNguoiDungSua = sUserLogin;
                        data.iNamLamViec = config.iNamLamViec;
                        data.iIndex = iIndex + 1;
                        data.sIPSua = ipSua;

                        conn.Insert(data, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NNS_DuToan>(data.iID_DuToan, trans);
                        if (entity == null)
                        {
                            return false;
                        }

                        entity.sMaLoaiDuToan = data.sMaLoaiDuToan;
                        entity.sTenLoaiDuToan = data.sTenLoaiDuToan;
                        //data.sTenLoaiDuToan = "Dự toán ABC";
                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.sNoiDung = data.sNoiDung;
                        entity.dNgayChungTu = data.dNgayChungTu;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.iSoLanSua = entity.iSoLanSua != null ? entity.iSoLanSua + 1 : 0;
                        entity.sID_MaNguoiDungSua = sUserLogin;
                        entity.dNgaySua = DateTime.Now;
                        entity.sSoCongVan = data.sSoCongVan;
                        entity.dNgayCongVan = data.dNgayCongVan;
                        entity.sIPSua = ipSua;

                        conn.Update<NNS_DuToan>(entity, trans);
                    }
                    trans.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public List<NNSDuToanExportDataModel> ExportData(int iNamLamViec, string sSoChungTu = "", string sNoiDung = "", string sMaLoaiDuToan = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sSoCongVan = "", DateTime? dNgayCongVanTu = null, DateTime? dNgayCongVanDen = null, string sUserName = "")
        {
            var result = new List<NNSDuToanExportDataModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = new StringBuilder();
                    sql.AppendFormat("SELECT * FROM NNS_DuToan WHERE iNamLamViec = {0}", iNamLamViec);
                    var items = conn.Query<NNS_DuToan>(sql.ToString());
                    if (items != null && items.Any())
                    {

                        if (!string.IsNullOrEmpty(sSoChungTu))
                        {
                            items = items.Where(x => x.sSoChungTu != null && x.sSoChungTu.Contains(sSoChungTu)).ToList();
                        }

                        if (!string.IsNullOrEmpty(sNoiDung))
                        {
                            items = items.Where(x => x.sNoiDung != null && x.sNoiDung.Contains(sNoiDung)).ToList();
                        }

                        if (!string.IsNullOrEmpty(sMaLoaiDuToan))
                        {
                            items = items.Where(x => x.sMaLoaiDuToan != null && x.sMaLoaiDuToan == sMaLoaiDuToan).ToList();
                        }

                        if (!string.IsNullOrEmpty(sSoQuyetDinh))
                        {
                            items = items.Where(x => x.sSoQuyetDinh != null && x.sSoQuyetDinh.Contains(sSoQuyetDinh)).ToList();
                        }

                        if (dNgayQuyetDinhTu.HasValue)
                        {
                            items = items.Where(x => x.dNgayQuyetDinh != null && x.dNgayQuyetDinh >= dNgayQuyetDinhTu.Value).ToList();
                        }

                        if (dNgayQuyetDinhDen.HasValue)
                        {
                            items = items.Where(x => x.dNgayQuyetDinh != null && x.dNgayQuyetDinh <= dNgayQuyetDinhDen.Value).ToList();
                        }

                        if (!string.IsNullOrEmpty(sSoCongVan))
                        {
                            items = items.Where(x => x.sSoCongVan != null && x.sSoCongVan.Contains(sSoCongVan)).ToList();
                        }

                        if (dNgayCongVanTu.HasValue)
                        {
                            items = items.Where(x => x.dNgayCongVan != null && x.dNgayCongVan >= dNgayCongVanTu.Value).ToList();
                        }

                        if (dNgayCongVanDen.HasValue)
                        {
                            items = items.Where(x => x.dNgayCongVan != null && x.dNgayCongVan <= dNgayCongVanDen.Value).ToList();
                        }

                        var count = 1;
                        foreach (var item in items.OrderBy(x => x.sSoChungTu))
                        {
                            var model = new NNSDuToanExportDataModel();
                            model.STT = count;
                            model.sMaLoaiDuToan = item.sMaLoaiDuToan;
                            model.sTenLoaiDuToan = item.sTenLoaiDuToan;
                            model.sSoChungTu = item.sSoChungTu;
                            model.sSoQuyetDinh = item.sSoQuyetDinh;
                            if (item.dNgayQuyetDinh.HasValue)
                            {
                                model.dNgayQuyetDinh = item.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                            }
                            model.sSoCongVan = item.sSoCongVan;
                            if (item.dNgayCongVan.HasValue)
                            {
                                model.dNgayCongVan = item.dNgayCongVan.Value.ToString("dd/MM/yyyy");
                            }
                            model.sNoiDung = item.sNoiDung;
                            if (item.rTongSoTien.HasValue)
                            {
                                //model.sSoTien = string.Format("{0:0,0}", item.rTongSoTien.Value);
                                model.sSoTien = item.rTongSoTien.Value;
                            }

                            result.Add(model);
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        #region Chi tiet du toan
        public List<NNSDuToanChiTietDataTableViewModel> GetListChiTietDuToanNew(string idDuToan, string sUserName, bool isSaoChep, List<NNSDuToanChiTietGetSoTienByDonViModel> aListXauNoiXa, string iID_NhiemVu, Dictionary<string, string> filters)
        {
            var result = new List<NNSDuToanChiTietDataTableViewModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    string sTenNoiDungChi = string.Empty;
                    string TenDonVi = string.Empty;
                    string sTenPhongBan = string.Empty;
                    if (filters.ContainsKey("sTenNoiDungChi"))
                        sTenNoiDungChi = filters["sTenNoiDungChi"].Trim();
                    if (filters.ContainsKey("TenDonVi"))
                        TenDonVi = filters["TenDonVi"].Trim();
                    if (filters.ContainsKey("sTenPhongBan"))
                        sTenPhongBan = filters["sTenPhongBan"].Trim();

                    var sqlAllNoiDungChi = new StringBuilder();
                    sqlAllNoiDungChi.Append("SELECT ndc.*, tree.depth, tree.bLaHangCha, tree.rootParent, tree.location FROM DM_NoiDungChi ndc ");
                    sqlAllNoiDungChi.Append("INNER JOIN orderedTree_view_dmnoidungchi tree ON ndc.iID_NoiDungChi = tree.iID_NoiDungChi ");
                    sqlAllNoiDungChi.AppendFormat("WHERE ndc.iNamLamViec = {0} ", config.iNamLamViec);
                    if (!string.IsNullOrEmpty(sTenNoiDungChi))
                        sqlAllNoiDungChi.AppendFormat("AND ndc.sTenNoiDungChi LIKE N'%{0}%' ", sTenNoiDungChi);
                    sqlAllNoiDungChi.Append("ORDER BY cast('/' + replace(ndc.iSTT, '.', '/') + '/' as hierarchyid); ");

                    var listAllDMNoiDungChi = conn.Query<DMNoiDungChiViewModel>(sqlAllNoiDungChi.ToString());
                    var listDuToanChiTiet = new List<NNS_DuToanChiTiet>();
                    var sqlDuToanChiTiet = new StringBuilder();
                    if (!string.IsNullOrEmpty(iID_NhiemVu))
                        sqlDuToanChiTiet.AppendFormat("SELECT * FROM NNS_DuToanChiTiet WHERE iID_DuToan = '{0}' AND iID_NhiemVu = '{1}'", idDuToan, iID_NhiemVu);
                    else
                        sqlDuToanChiTiet.AppendFormat("SELECT * FROM NNS_DuToanChiTiet WHERE iID_DuToan = '{0}'", idDuToan);

                    if (!string.IsNullOrEmpty(TenDonVi))
                        sqlDuToanChiTiet.AppendFormat(" AND (TenDonVi LIKE N'%{0}%' OR iID_MaDonVi LIKE N'%{0}%') ", TenDonVi);
                    if (!string.IsNullOrEmpty(sTenPhongBan))
                        sqlDuToanChiTiet.AppendFormat(" AND (sTenPhongBan LIKE N'%{0}%' OR sMaPhongBan LIKE N'%{0}%') ", sTenPhongBan);
                    sqlDuToanChiTiet.Append(" ORDER BY iID_MaDonVi, sMaPhongBan ");
                    var items = conn.Query<NNS_DuToanChiTiet>(sqlDuToanChiTiet.ToString());
                    if (items != null && items.Any())
                        listDuToanChiTiet = items.ToList();

                    var nhiemVu = new NNS_DuToan_NhiemVu();
                    if (!string.IsNullOrEmpty(iID_NhiemVu))
                    {
                        var sqlNhiemVu = new StringBuilder();
                        sqlNhiemVu.AppendFormat("SELECT * FROM NNS_DuToan_NhiemVu WHERE iID_NhiemVu = '{0}'", iID_NhiemVu);
                        nhiemVu = conn.QueryFirstOrDefault<NNS_DuToan_NhiemVu>(sqlNhiemVu.ToString());
                    }
                    if (listAllDMNoiDungChi != null && listAllDMNoiDungChi.Any())
                    {
                        foreach (var item in listAllDMNoiDungChi)
                        {
                            var model = new NNSDuToanChiTietDataTableViewModel();
                            model.sMaNoiDungChi = item.sMaNoiDungChi;
                            model.sTenNoiDungChi = item.sTenNoiDungChi;
                            model.bLaHangCha = true;
                            model.iID_NoiDungChi = item.iID_NoiDungChi;
                            model.iID_NoiDungChiCha = item.iID_Parent;
                            model.location = item.location;
                            model.depth = item.depth;
                            model.rootParent = item.rootParent;
                            if (!item.bLaHangCha)
                            {
                                model.bConLai = true;
                            }

                            result.Add(model);

                            if (!isSaoChep && listDuToanChiTiet != null && listDuToanChiTiet.Any())
                            {
                                var listChiTiet = listDuToanChiTiet.Where(x => x.sMaNoiDungChi == item.sMaNoiDungChi).ToList();
                                if (listChiTiet != null && listChiTiet.Any())
                                {
                                    foreach (var ct in listChiTiet)
                                    {
                                        var chiTiet = new NNSDuToanChiTietDataTableViewModel();
                                        chiTiet.iID_DuToanChiTiet = ct.iID_DuToanChiTiet.ToString();
                                        chiTiet.sMaNoiDungChi = item.sMaNoiDungChi;
                                        chiTiet.sTenNoiDungChi = "";
                                        chiTiet.iID_MaDonVi = ct.iID_MaDonVi;
                                        chiTiet.TenDonVi = string.Format("{0} - {1}", ct.iID_MaDonVi, ct.TenDonVi);
                                        chiTiet.sMaPhongBan = ct.sMaPhongBan;
                                        chiTiet.sTenPhongBan = string.Format("{0} - {1}", ct.sMaPhongBan, ct.sTenPhongBan);
                                        chiTiet.SoTien = ct.SoTien.Value;
                                        chiTiet.bConLai = false;
                                        chiTiet.bLaHangCha = false;
                                        chiTiet.GhiChu = ct.GhiChu;
                                        if (ct.iID_NhiemVu.HasValue)
                                        {
                                            chiTiet.iID_NhiemVu = ct.iID_NhiemVu.Value.ToString();
                                        }
                                        chiTiet.iID_NoiDungChiCha = item.iID_NoiDungChi;
                                        chiTiet.depth = (int.Parse(item.depth) + 1).ToString();
                                        chiTiet.rootParent = item.rootParent;
                                        chiTiet.location = item.location + "." + item.sMaNoiDungChi;
                                        result.Add(chiTiet);
                                    }
                                }
                            }
                        }

                        if (isSaoChep == true)
                        {
                            if (aListXauNoiXa != null && aListXauNoiXa.Any())
                            {
                                foreach (var item in aListXauNoiXa)
                                {
                                    var objNoiDungChi = listAllDMNoiDungChi.FirstOrDefault(x => x.sMaNoiDungChi == item.sMaNoiDungChi);
                                    if (result != null && result.Any())
                                    {
                                        var xau = new NNSDuToanChiTietDataTableViewModel();
                                        var modelCheck = result.FirstOrDefault(x => x.sMaNoiDungChi == item.sMaNoiDungChi && x.iID_MaDonVi == item.iID_MaDonVi && x.sMaPhongBan == item.sMaPhongBan);
                                        if (modelCheck != null)
                                        {
                                            result.Remove(modelCheck);
                                            xau.iID_DuToanChiTiet = modelCheck.iID_DuToanChiTiet;
                                        }

                                        if (objNoiDungChi != null)
                                        {
                                            xau.sMaNoiDungChi = item.sMaNoiDungChi;
                                            xau.sTenNoiDungChi = "";
                                            xau.iID_MaDonVi = item.iID_MaDonVi;
                                            xau.TenDonVi = item.TenDonVi;
                                            xau.sMaPhongBan = item.sMaPhongBan;
                                            xau.sTenPhongBan = item.sTenPhongBan;
                                            xau.SoTien = item.SoTienXauNoiMa;
                                            xau.bConLai = false;
                                            xau.bLaHangCha = false;
                                            xau.iThayDoi = true;
                                            if (nhiemVu != null)
                                            {
                                                if (nhiemVu.iID_MaChungTu.HasValue)
                                                {
                                                    xau.iID_MaChungTu = nhiemVu.iID_MaChungTu.Value.ToString();
                                                }
                                            }
                                            xau.iID_NoiDungChiCha = objNoiDungChi.iID_NoiDungChi;
                                            xau.depth = (int.Parse(objNoiDungChi.depth) + 1).ToString();
                                            xau.rootParent = objNoiDungChi.rootParent;
                                            xau.location = objNoiDungChi.location + "." + objNoiDungChi.sMaNoiDungChi;

                                            // get index of parent
                                            int index = result.FindIndex(x => x.bLaHangCha == true && x.sMaNoiDungChi == item.sMaNoiDungChi);
                                            if (index > -1)
                                                result.Insert(index, xau);
                                            //result.Add(xau);
                                        }
                                    }
                                    else
                                    {
                                        if (objNoiDungChi != null)
                                        {
                                            var xau = new NNSDuToanChiTietDataTableViewModel();
                                            xau.sMaNoiDungChi = item.sMaNoiDungChi;
                                            xau.sTenNoiDungChi = "";
                                            xau.iID_MaDonVi = item.iID_MaDonVi;
                                            xau.TenDonVi = item.TenDonVi;
                                            xau.sMaPhongBan = item.sMaPhongBan;
                                            xau.sTenPhongBan = item.sTenPhongBan;
                                            xau.SoTien = item.SoTienXauNoiMa;
                                            xau.bConLai = false;
                                            xau.bLaHangCha = false;
                                            xau.iThayDoi = true;
                                            if (nhiemVu != null)
                                            {
                                                if (nhiemVu.iID_MaChungTu.HasValue)
                                                {
                                                    xau.iID_MaChungTu = nhiemVu.iID_MaChungTu.Value.ToString();
                                                }
                                            }
                                            xau.iID_NoiDungChiCha = objNoiDungChi.iID_NoiDungChi;
                                            xau.depth = (int.Parse(objNoiDungChi.depth) + 1).ToString();
                                            xau.rootParent = objNoiDungChi.rootParent;
                                            xau.location = objNoiDungChi.location + "." + objNoiDungChi.sMaNoiDungChi;
                                            result.Add(xau);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result.ToList();
        }

        /// <summary>
        /// cap nhat tong so tien cua du toan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateSumDuToan(string iID_DuToan)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_update_tongsotien_dutoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(sql,
                        param: new
                        {
                            iID_DuToan
                        },
                        commandType: CommandType.Text);
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDV(string sUserName)
        {
            var result = new List<NNSDuToanChiTietGetSoTienByDonViModel>();

            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {

                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iNamLamViec", config.iNamLamViec);
                    lstParam.Add("@sUserName", sUserName);

                    var items = conn.Query<NNSDuToanChiTietGetSoTienByDonViModel>("proc_getdutoangiaosotienbydonvi", lstParam, commandType: CommandType.StoredProcedure);
                    if (items != null && items.Any())
                    {
                        result.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        public List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDauNamTruocChuyenSang(string sUserName)
        {
            var result = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iNamLamViec", config.iNamLamViec);

                    var items = conn.Query<NNSDuToanChiTietGetSoTienByDonViModel>("proc_get_nns_dutoan_dau_nam_truoc_chuyen_sang", lstParam, commandType: CommandType.StoredProcedure);
                    if (items != null && items.Any())
                    {
                        result.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        public List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuTinhTongTienDuToanDVTruocVaSauBaMuoiThangChin(string sUserName, string iID_DuToan)
        {
            var result = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iNamLamViec", config.iNamLamViec);
                    lstParam.Add("@sUserName", sUserName);
                    lstParam.Add("@iID_DuToan", iID_DuToan);

                    var items = conn.Query<NNSDuToanChiTietGetSoTienByDonViModel>("proc_get_nns_dutoan_bosung", lstParam, commandType: CommandType.StoredProcedure);
                    if (items != null && items.Any())
                    {
                        result.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        public List<NNSDuToanChiTietTongTienModel> GetTongTienDuToanChiTiet(string iID_DuToan, string sUserName, string sMaLoaiDuToan, string iID_NhiemVu = null)
        {
            var result = new List<NNSDuToanChiTietTongTienModel>();

            if (string.IsNullOrEmpty(iID_DuToan) || string.IsNullOrEmpty(sMaLoaiDuToan))
            {
                return result;
            }

            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var listAllDotNhanChiTiet = conn.Query<NNS_DotNhanChiTiet>("SELECT * FROM NNS_DotNhanChiTiet");
                    var listAllDotNhanChiTietNDChi = conn.Query<NNS_DotNhanChiTiet_NDChi>("SELECT * FROM NNS_DotNhanChiTiet_NDChi");
                    var listAllDMNoiDungChi = conn.Query<DM_NoiDungChi>("SELECT * FROM DM_NoiDungChi WHERE iTrangThai = 1");

                    var sqlAllDuToan = new StringBuilder();
                    sqlAllDuToan.AppendFormat("SELECT * FROM NNS_DuToan WHERE iNamLamViec = '{0}'", config.iNamLamViec);
                    var listAllDuToan = conn.Query<NNS_DuToan>(sqlAllDuToan.ToString());

                    var listAllDuToanChiTiet = conn.Query<NNS_DuToanChiTiet>($"SELECT * FROM NNS_DuToanChiTiet");

                    var sqlAllDotNhan = new StringBuilder();
                    sqlAllDotNhan.AppendFormat("SELECT * FROM NNS_DotNhan WHERE iNamLamViec = '{0}'", config.iNamLamViec);
                    var listAllDotNhan = conn.Query<NNS_DotNhan>(sqlAllDotNhan.ToString());

                    if (listAllDuToan == null && !listAllDuToan.Any())
                    {
                        return result;
                    }

                    var duToanNow = listAllDuToan.FirstOrDefault(x => x.iID_DuToan == Guid.Parse(iID_DuToan));
                    if (duToanNow == null)
                    {
                        return result;
                    }

                    var dictSoTienNow = new Dictionary<string, decimal>();
                    var dictSoTienOld = new Dictionary<string, decimal>();
                    var listIdDotNhanNow = new List<Guid>();
                    if (listAllDotNhan != null && listAllDotNhan.Any())
                    {
                        if (duToanNow.dNgayQuyetDinh != null && duToanNow.dNgayQuyetDinh.HasValue)
                        {
                            var listDotNhanQT = listAllDotNhan.Where(x => x.dNgayQuyetDinh <= duToanNow.dNgayQuyetDinh).ToList();
                            if (listDotNhanQT != null && listDotNhanQT.Any())
                            {
                                var listIdDotNhanQT = listDotNhanQT.Select(x => x.iID_DotNhan).ToList();
                                if (listIdDotNhanQT != null && listIdDotNhanQT.Any())
                                {
                                    listIdDotNhanNow.AddRange(listIdDotNhanQT);
                                }
                            }
                        }
                        else
                        {
                            var listDotNhanCV = listAllDotNhan.Where(x => x.dNgayQuyetDinh <= duToanNow.dNgayCongVan).ToList();
                            if (listDotNhanCV != null && listDotNhanCV.Any())
                            {
                                var listIdDotNhanCV = listDotNhanCV.Select(x => x.iID_DotNhan).ToList();
                                if (listIdDotNhanCV != null && listIdDotNhanCV.Any())
                                {
                                    listIdDotNhanNow.AddRange(listIdDotNhanCV);
                                }
                            }
                        }
                    }

                    var listIdDotNhanChiTietNow = new List<Guid>();
                    if (listAllDotNhanChiTiet != null && listAllDotNhanChiTiet.Any())
                    {
                        if (listIdDotNhanNow != null && listIdDotNhanNow.Any())
                        {
                            var listDotNhanChiTiet = listAllDotNhanChiTiet.Where(x => listIdDotNhanNow.Contains(x.iID_DotNhan)).ToList();
                            if (listDotNhanChiTiet != null && listDotNhanChiTiet.Any())
                            {
                                listIdDotNhanChiTietNow = listDotNhanChiTiet.Select(x => x.iID_DotNhanChiTiet).ToList();
                            }
                        }
                    }

                    var listDotNhanChiTietNDChiNow = new List<NNS_DotNhanChiTiet_NDChi>();
                    if (listAllDotNhanChiTietNDChi != null && listAllDotNhanChiTietNDChi.Any())
                    {
                        if (listIdDotNhanChiTietNow != null && listIdDotNhanChiTietNow.Any())
                        {
                            listDotNhanChiTietNDChiNow = listAllDotNhanChiTietNDChi.Where(x => listIdDotNhanChiTietNow.Contains(x.iID_DotNhanChiTiet.Value)).ToList();
                        }
                    }

                    if (listDotNhanChiTietNDChiNow != null && listDotNhanChiTietNDChiNow.Any())
                    {
                        foreach (var item in listDotNhanChiTietNDChiNow)
                        {
                            var sMaNoiDungChi = "";
                            if (listAllDMNoiDungChi != null && listAllDMNoiDungChi.Any())
                            {
                                var dmNDChi = listAllDMNoiDungChi.FirstOrDefault(x => x.iID_NoiDungChi == item.iID_NoiDungChi);
                                if (dmNDChi != null)
                                {
                                    sMaNoiDungChi = dmNDChi.sMaNoiDungChi;
                                }
                            }
                            if (!dictSoTienNow.ContainsKey(sMaNoiDungChi))
                            {
                                dictSoTienNow.Add(sMaNoiDungChi, item.SoTien.Value);
                                continue;
                            }
                            if (dictSoTienNow.ContainsKey(sMaNoiDungChi))
                            {
                                dictSoTienNow[sMaNoiDungChi] += item.SoTien.Value;
                            }
                        }
                    }
                    var listDuToanOld = new List<NNS_DuToan>();
                    foreach (var item in listAllDuToan)
                    {
                        if (duToanNow != null)
                        {
                            if (item.iID_DuToan == duToanNow.iID_DuToan)
                            {
                                continue;
                            }
                            if (duToanNow.dNgayQuyetDinh != null && duToanNow.dNgayQuyetDinh.HasValue)
                            {
                                if (item.dNgayQuyetDinh != null && item.dNgayQuyetDinh.HasValue)
                                {
                                    if (item.dNgayQuyetDinh <= duToanNow.dNgayQuyetDinh)
                                    {
                                        listDuToanOld.Add(item);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (item.dNgayCongVan <= duToanNow.dNgayQuyetDinh)
                                    {
                                        listDuToanOld.Add(item);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if (item.dNgayQuyetDinh != null && item.dNgayQuyetDinh.HasValue)
                                {
                                    if (item.dNgayQuyetDinh <= duToanNow.dNgayCongVan)
                                    {
                                        listDuToanOld.Add(item);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (item.dNgayCongVan <= duToanNow.dNgayCongVan)
                                    {
                                        listDuToanOld.Add(item);
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    var listIdDuToanOld = new List<Guid>();
                    if (listDuToanOld != null && listDuToanOld.Any())
                    {
                        listIdDuToanOld = listDuToanOld.Select(x => x.iID_DuToan).ToList();
                    }

                    var listDuToanChiTietOld = new List<NNS_DuToanChiTiet>();
                    if (listIdDuToanOld != null && listIdDuToanOld.Any())
                    {
                        if (listAllDuToanChiTiet != null && listAllDuToanChiTiet.Any())
                        {
                            listDuToanChiTietOld = listAllDuToanChiTiet.Where(x => listIdDuToanOld.Contains(x.iID_DuToan)).ToList();
                        }
                    }

                    if (listDuToanChiTietOld != null && listDuToanChiTietOld.Any())
                    {
                        foreach (var item in listDuToanChiTietOld)
                        {
                            if (!dictSoTienOld.ContainsKey(item.sMaNoiDungChi))
                            {
                                dictSoTienOld.Add(item.sMaNoiDungChi, item.SoTien.Value);
                                continue;
                            }

                            if (dictSoTienOld.ContainsKey(item.sMaNoiDungChi))
                            {
                                dictSoTienOld[item.sMaNoiDungChi] = dictSoTienOld[item.sMaNoiDungChi] + item.SoTien.Value;
                            }
                        }
                    }

                    if (dictSoTienNow != null)
                    {
                        List<NNSDuToanChiTietTongTienModel> lstTongTienNhiemVuTruoc = new List<NNSDuToanChiTietTongTienModel>();
                        // tru tien da giao o nhiem vu truoc
                        if (!string.IsNullOrEmpty(iID_NhiemVu))
                            lstTongTienNhiemVuTruoc = GetListDuToanChiTietPrevious(iID_DuToan, iID_NhiemVu);

                        foreach (var item in dictSoTienNow)
                        {
                            var model = new NNSDuToanChiTietTongTienModel();
                            model.sMaNoiDungChi = item.Key;
                            if (dictSoTienOld.ContainsKey(item.Key))
                                model.SoTien = item.Value - dictSoTienOld[item.Key];
                            else
                                model.SoTien = item.Value;

                            NNSDuToanChiTietTongTienModel objTienNhiemVuTruoc = lstTongTienNhiemVuTruoc.Where(x => x.sMaNoiDungChi == item.Key).FirstOrDefault();
                            if (objTienNhiemVuTruoc != null)
                                model.SoTien = model.SoTien - objTienNhiemVuTruoc.SoTien;
                            result.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// get list du toan chi tiet cua nhiem vu tao truoc
        /// </summary>
        /// <param name="iID_DuToan"></param>
        /// <param name="iID_NhiemVu"></param>
        /// <returns></returns>
        public List<NNSDuToanChiTietTongTienModel> GetListDuToanChiTietPrevious(string iID_DuToan, string iID_NhiemVu)
        {
            List<NNSDuToanChiTietTongTienModel> result = new List<NNSDuToanChiTietTongTienModel>();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = new StringBuilder();
                    sql.Append("SELECT sMaNoiDungChi, sum(SoTien) as SoTien FROM NNS_DuToanChiTiet ");
                    sql.AppendFormat("WHERE iID_NhiemVu IN(select iID_NhiemVu from NNS_DuToan_NhiemVu WHERE iID_DuToan = '{0}' " +
                        "AND dNgayTao < (select dNgayTao from NNS_DuToan_NhiemVu where iID_NhiemVu = '{1}')) GROUP BY sMaNoiDungChi", iID_DuToan, iID_NhiemVu);
                    result = conn.Query<NNSDuToanChiTietTongTienModel>(sql.ToString(), null, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        public bool CheckCreateLoaiDuToanDauNam(string sUserName, string sMaLoaiDuToan)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = new StringBuilder();
                    sql.AppendFormat("SELECT * FROM NNS_DuToan WHERE sMaLoaiDuToan = '{0}' AND iNamLamViec = {1}", sMaLoaiDuToan, config.iNamLamViec);
                    var items = conn.Query<NNS_DuToan>(sql.ToString(), null, commandType: CommandType.Text);
                    if (items != null && items.Any())
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        #endregion
        #endregion

        #region QL phân nguồn BTC theo nội dung chi BQP
        /// <summary>
        /// Get NNS phân nguồn BTC theo nội dung chi BQP
        /// </summary>
        /// <param name="sSoChungTu">So chung tu</param>
        /// <param name="sNoiDung">Noi dung</param>
        /// <returns></returns>
        public IEnumerable<NNS_PhanNguon> GetAllNNSPhanNguonBTCTheoNDChiBQP(ref PagingInfo _paging, string sSoChungTu, string sNoiDung)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoChungTu", sSoChungTu);
                    lstParam.Add("sNoiDung", sNoiDung);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<NNS_PhanNguon>("proc_get_all_nnsphannguon_paging", lstParam, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Get NNS_PhanNguon by ID
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        /// <returns></returns>
        public NNS_PhanNguon GetNNSPhanNguonByID(Guid iId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_by_id_phannguon.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirst<NNS_PhanNguon>(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Insert NNS_PhanNguon
        /// </summary>
        /// <param name="sNoiDung">sNoiDung</param>
        /// <param name="sSoChungTu">sSoChungTu</param>
        /// <param name="dNgayChungTu">dNgayChungTu</param>
        /// <param name="sSoQuyetDinh">dNgayChungTu</param>
        /// <param name="dNgayQuyetDinh">dNgayChungTu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        /// <returns></returns>
        public bool InsertNNSPhanNguon(string sNoiDung, string sSoChungTu, DateTime? dNgayChungTu, string sSoQuyetDinh, DateTime? dNgayQuyetDinh, string sUserLogin)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;
                var sql = FileHelpers.GetSqlQuery("nns_insert_phannguon.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            sNoiDung,
                            sSoChungTu,
                            dNgayChungTu,
                            sSoQuyetDinh,
                            dNgayQuyetDinh,
                            iNamLamViec,
                            iID_MaNguonNganSach,
                            iID_MaNamNganSach,
                            sUserLogin
                        },
                        commandType: CommandType.Text);

                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Update NNS_PhanNguon
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        /// <param name="sNoiDung">sNoiDung</param>
        /// <param name="sSoChungTu">sSoChungTu</param>
        /// <param name="dNgayChungTu">dNgayChungTu</param>
        /// <param name="sSoQuyetDinh">dNgayChungTu</param>
        /// <param name="dNgayQuyetDinh">dNgayChungTu</param>
        /// <param name="sUserLogin">sID_MaNguoiDungTao</param>
        public bool UpdateNNSPhanNguon(Guid iId, string sNoiDung, string sSoChungTu, DateTime? dNgayChungTu, string sSoQuyetDinh, DateTime? dNgayQuyetDinh, string sIPSua, string sUserLogin)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;
                var sql = FileHelpers.GetSqlQuery("nns_update_phannguon.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iId,
                            sNoiDung,
                            sSoChungTu,
                            dNgayChungTu,
                            sSoQuyetDinh,
                            dNgayQuyetDinh,
                            iNamLamViec,
                            iID_MaNguonNganSach,
                            iID_MaNamNganSach,
                            sIPSua,
                            sUserLogin
                        },
                        commandType: CommandType.Text);

                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Delete NNS_PhanNguon by iId
        /// </summary>
        /// <param name="iId">iID_PhanNguon</param>
        public bool deleteNNSPhanNguon(Guid iId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_delete_phannguon.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool deleteNNSPhanNguonNDChi(Guid iIdPhanNguon)
        {
            try
            {
                var sql = "DELETE NNS_PhanNguon_NDChi WHERE iID_PhanNguon = @iIdPhanNguon";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iIdPhanNguon
                        },
                        commandType: CommandType.Text);

                    return r >= 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<NNSDMNguonViewModel> GetAllDMNguonBTCCap(ref PagingInfo _paging, DateTime? dNgayChungTu, string sUserLogin, Guid? iIdPhanNguon)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("dNgayChungTu", dNgayChungTu);
                    lstParam.Add("iNamLamViec", iNamLamViec);
                    lstParam.Add("iIdPhanNguon", iIdPhanNguon);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<NNSDMNguonViewModel>("proc_get_all_dmnguon_btccap_paging", lstParam, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<NNSDMNoiDungChiViewModel> GetAllDMNoiDungChiBQP(Guid? iIdNguon, Guid? iIdPhanNguon, string sUserLogin)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;
                var sql = FileHelpers.GetSqlQuery("nns_get_all_dmnoidungchi_bqp.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NNSDMNoiDungChiViewModel>(sql,
                        param: new
                        {
                            iIdNguon,
                            iIdPhanNguon,
                            iNamLamViec,
                            iID_MaNguonNganSach,
                            iID_MaNamNganSach
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public Decimal getSoTienCoTheChi(Guid? iIdNguon, Guid? iIdPhanNguon, decimal rSoTienConLai, string sUserLogin, string sTenNoiDungChi)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;
                var sql = FileHelpers.GetSqlQuery("nns_get_sotiencothechi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@iIdNguon", iIdNguon);
                        cmd.Parameters.AddWithValue("@iIdPhanNguon", iIdPhanNguon);
                        cmd.Parameters.AddWithValue("@rSoTienConLai", rSoTienConLai);
                        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                        cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                        cmd.Parameters.AddWithValue("@sTenNoiDungChi", sTenNoiDungChi);
                        return Convert.ToDecimal(Connection.GetValue(cmd, 0));
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return 0;
        }

        public DataTable GetAllDMNoiDungChiBQP2(Guid? iIdNguon, Guid? iIdPhanNguon, decimal rSoTienConLai, string sUserLogin, Dictionary<string, string> _filters)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;
                var sql = FileHelpers.GetSqlQuery("nns_get_all_dmnoidungchi_bqp.sql");
                string sTenNoiDungChi = "";
                if (_filters.ContainsKey("sTenNoiDungChi"))
                {
                    sTenNoiDungChi = _filters["sTenNoiDungChi"];
                }

                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@iIdNguon", iIdNguon);
                        cmd.Parameters.AddWithValue("@iIdPhanNguon", iIdPhanNguon);
                        cmd.Parameters.AddWithValue("@rSoTienConLai", rSoTienConLai);
                        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                        cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                        cmd.Parameters.AddWithValue("@sTenNoiDungChi", sTenNoiDungChi);
                        return cmd.GetTable();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }
        public bool SaveNNSPhanNguonNDChi(IEnumerable<NNSPhanNguonNDChiTempViewModel> dataNew, IEnumerable<NNSPhanNguonNDChiTempViewModel> dataEdit, string sUserLogin, String sIPSua)
        {
            try
            {
                var datatableNew = ToDataTable(dataNew);
                var datatableEdit = ToDataTable(dataEdit);
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;
                int iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                int iID_MaNamNganSach = config.iID_MaNamNganSach;

                using (var conn = _connectionFactory.GetConnection())
                {
                    var trans = conn.BeginTransaction();
                    using (var cmd1 = new SqlCommand("InsertNNSPhanNguonNDChi", conn, trans))
                    {
                        cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd1.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                        cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                        cmd1.Parameters.AddWithValue("@sUserLogin", sUserLogin);
                        cmd1.Parameters.AddWithValue("@sIPSua", sIPSua);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@nnsPhanNguonNDChiNew", datatableNew);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.NNS_PhanNguon_NDChi_temp";
                        SqlParameter tvpParam2 = cmd1.Parameters.AddWithValue("@nnsPhanNguonNDChiEdit", datatableEdit);
                        tvpParam2.SqlDbType = SqlDbType.Structured;
                        tvpParam2.TypeName = "dbo.NNS_PhanNguon_NDChi_temp";

                        conn.Open();
                        cmd1.ExecuteNonQuery();
                        conn.Close();
                    }
                    trans.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;

        }
        public DataTable ToDataTable<NNSPhanNguonNDChiTempViewModel>(IEnumerable<NNSPhanNguonNDChiTempViewModel> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(NNSPhanNguonNDChiTempViewModel));
            DataTable table = new DataTable();

            try
            {
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (NNSPhanNguonNDChiTempViewModel item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return table;
        }

        public int GetMaxiIndexNNSPhanNguon()
        {
            try
            {
                var sql = "SELECT ISNULL(MAX(iIndex),0) FROM NNS_PhanNguon";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var entity = conn.QueryFirst<int>(
                        sql,
                        commandType: CommandType.Text);

                    return entity;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return 0;
        }

        public IEnumerable<NNSPhanNguonNDChiTempViewModel> getNNSPhanNguonNDChiByIds(Guid? iIdNguon, Guid? iIdPhanNguon, Guid? iID_NoiDungChi, string sUserLogin)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_phannguonndchi_by_ids.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NNSPhanNguonNDChiTempViewModel>(sql,
                        param: new
                        {
                            iIdNguon,
                            iIdPhanNguon,
                            iID_NoiDungChi
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        #endregion

        #region Báo cáo
        public IEnumerable<NNSNganSachTheoNamLamViec> GetTotalNganSachBQPByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_nguonngansachtheonamlamviec.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<NNSNganSachTheoNamLamViec>(sql,
                        param: new
                        {
                            iNamLamViec,
                            iLoaiNganSach,
                            iNguonNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<NNSDuToanTheoNamLamViecModel> GetTotalDuToanByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_dutoantheonamlamviec.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<NNSDuToanTheoNamLamViecModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iLoaiNganSach,
                            iNguonNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<RptNNSDuToanChiTietModel> GetDetailDuToanByNamLamViec(int iNamLamViec, int? iLoaiNganSach, Guid? iNguonNganSach, DateTime? dDateFrom, 
            DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_dutoanchitiettheonamlamviec.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            sSoQuyetDinh,
                            iLoaiNganSach,
                            iNguonNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// get chi tiet du toan theo noi dung chi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public IEnumerable<RptNNSDuToanChiTietModel> GetDetailDuToanTheoNoiDungChi(Guid? iNguonNganSach, int iNamLamViec, DateTime? dDateFrom, 
            DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_dutoanchitiettheonamlamviec_theo_noidungchi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay chi tiet du toan theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        public IEnumerable<RptNNSDuToanChiTietModel> GetDuToanTheoNhiemVu(int iNamLamViec, int? iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_dutoan_theo_nhiemvu.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            sSoQuyetDinh,
                            iLoaiNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay chi tiet du toan phan cap dau nam theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        public IEnumerable<RptNNSDuToanChiTietModel> GetDuToanPhanCapDauNam(int iNamLamViec, int? iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_dutoan_phancap_daunam.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            sSoQuyetDinh,
                            iLoaiNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay chi tiet du toan phan cap dau nam theo so quyet dinh
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        public IEnumerable<RptNNSDuToanChiTietModel> GetDuToanPhanCapDauNamTheoSoQuyetDinh(int iNamLamViec, int? iLoaiNganSach, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_dutoan_phancap_daunam_theo_soquyetdinh.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            sSoQuyetDinh,
                            iLoaiNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay chi tiet du toan theo so quyet dinh va danh muc nguon
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <param name="iSoQuyetDinh"></param>
        /// <returns></returns>
        public IEnumerable<RptNNSDuToanChiTietModel> GetDuToanTheoNhiemVuVaDanhMucNguon(Guid? iNguonNganSach, int iNamLamViec, string sSoQuyetDinh, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_dutoan_theo_nhiemvu_va_id_nguon.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<RptNNSDuToanChiTietModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            sSoQuyetDinh,
                            iNguonNganSach,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach tong hop du toan NS theo don vi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_rpt_tonghopdutoannsnam_theodonvi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<RptChiTietTongHopDuToanNganSachNamModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach tong hop du toan NS theo don vi bql
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoDonViBQL(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_rpt_tonghopdutoannsnam_theodonvibql.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<RptChiTietTongHopDuToanNganSachNamModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach tong hop du toan NS theo bql don vi 
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<RptChiTietTongHopDuToanNganSachNamModel> GetTongHopDuToanNSTheoBQLDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_rpt_tonghopdutoannsnam_theobqldonvi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<RptChiTietTongHopDuToanNganSachNamModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach tong hop qd bs du toan NS theo don vi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<RptChiTietTongHopQDBSDuToanNSNamModel> GetTongHopQDBSDuToanNSNamTheoDonVi(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_rpt_tonghopqdbsdutoannsnam_theodonvi.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<RptChiTietTongHopQDBSDuToanNSNamModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach tong hop qd bs du toan NS theo don vi bql
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<RptChiTietTongHopQDBSDuToanNSNamModel> GetTongHopQDBSDuToanNSNamTheoDonViBQL(int iNamLamViec, Guid? iNguonNganSach, DateTime? dDateFrom, DateTime? dDateTo, string sSoQuyetDinh = null, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_rpt_tonghopqdbsdutoannsnam_theodonvibql.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<RptChiTietTongHopQDBSDuToanNSNamModel>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iNguonNganSach,
                            sSoQuyetDinh,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }
        #endregion

        #region QL đợt nhận
        public IEnumerable<DotNhanViewModel> GetAllDotNhanNguonNS(ref PagingInfo _paging, int iNamLamViec, string sSoChungTu, string sNoiDung, string sMaLoaiDuToan, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("sSoChungTu", sSoChungTu);
                    lstParam.Add("sNoiDung", sNoiDung);
                    lstParam.Add("sMaLoaiDuToan", sMaLoaiDuToan);
                    lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                    lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                    lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                    lstParam.Add("iNamLamViec", iNamLamViec);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<DotNhanViewModel>("proc_get_all_dotnhannguonNS_paging", lstParam, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public NNS_DotNhan GetDotNhanNguonNS(Guid iId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("dm_get_by_id_dotnhanNguonNS.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirst<NNS_DotNhan>(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool InsertDotNhanNguonNS(ref NNS_DotNhan data, string sUserLogin)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.iID_DotNhan == null || data.iID_DotNhan == Guid.Empty)
                    {
                        data.iID_DotNhan = Guid.NewGuid();
                        data.iNamLamViec = config.iNamLamViec;
                        data.iID_MaNguonNganSach = config.iID_MaNguonNganSach;
                        data.iID_MaNamNganSach = config.iID_MaNamNganSach;
                        data.iIndex = GetMaxIndexNNSDotNhan(config.iNamLamViec) + 1;
                        data.sID_MaNguoiDungTao = sUserLogin;
                        data.dNgayTao = DateTime.Now;
                        conn.Insert(data, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NNS_DotNhan>(data.iID_DotNhan, trans);
                        if (entity == null)
                            return false;
                        data.iSoLanSua = entity.iSoLanSua != null ? entity.iSoLanSua : 0;
                        entity.sMaLoaiDuToan = data.sMaLoaiDuToan;
                        entity.sTenLoaiDuToan = data.sTenLoaiDuToan;
                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.sNoiDung = data.sNoiDung;
                        entity.dNgayChungTu = data.dNgayChungTu;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.iSoLanSua = data.iSoLanSua + 1;
                        entity.sID_MaNguoiDungSua = sUserLogin;
                        entity.dNgaySua = DateTime.Now;
                        conn.Update(entity, trans);
                    }
                    // commit to db
                    trans.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public List<DM_LoaiDuToan> GetAllDMLoaiDuToan(int iNamLamViec)
        {
            try
            {
                List<DM_LoaiDuToan> listLoaiDuToan = _dmService.GetAllDMLoaiDuToan(iNamLamViec).ToList();
                listLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = "", sTenLoaiDuToan = "--Tất cả--" });
                return listLoaiDuToan;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public void InsertNNSDotNhanChiTiet(NNS_DotNhanChiTiet data, string sUserLogin)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                var sql = FileHelpers.GetSqlQuery("dm_insert_dotnhanChiTiet.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_DotNhan = data.iID_DotNhan,
                            iID_Nguon = data.iID_Nguon,
                            SoTien = data.SoTien,
                            GhiChu = data.GhiChu,
                            iNamLamViec = config.iNamLamViec,
                            iID_MaNguonNganSach = config.iID_MaNguonNganSach,
                            iID_MaNamNganSach = config.iID_MaNamNganSach,
                            sUserLogin,
                        },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void UpdateDotNhanChiTiet(NNS_DotNhanChiTiet data, string sUserLogin)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("dm_update_dotnhanChiTiet.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_DotNhanChiTiet = data.iID_DotNhanChiTiet,
                            SoTien = data.SoTien,
                            GhiChu = data.GhiChu,
                            sUserLogin
                        },
                        commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public List<BaoCaoTongHopNguonViewModel> BaoCaoTongHopNguon(Guid? iNguonNganSach, int iNamLamViec, int iSoCotBoSung, DateTime? dNgayFrom, DateTime? dNgayTo, int dvt = 1)
        {
            try
            {
                List<BaoCaoTongHopNguonViewModel> listData = new List<BaoCaoTongHopNguonViewModel>();
                var sql = FileHelpers.GetSqlQuery("dm_get_data_baocaoTongHopNguon.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    listData = conn.Query<BaoCaoTongHopNguonViewModel>(sql,
                        param: new
                        {
                            iNguonNganSach,
                            iNamLamViec,
                            dNgayFrom,
                            dNgayTo,
                            dvt
                        },
                       commandType: CommandType.Text).ToList();
                    return listData;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public DataTable GetChiTietDotNhanVaDotBoSung(Guid? iNguonNganSach, int iNamLamViec, DateTime? dNgayFrom, DateTime? dNgayTo, int dvt = 1, string sTenCot = "")
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_getallchitietbaocao1.sql");
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@dNgayFrom", dNgayFrom ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dNgayTo", dNgayTo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dvt", dvt);
                cmd.Parameters.AddWithValue("@sTenCot", sTenCot);
                return Connection.GetDataTable(cmd);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public TinhTongSoTienBaoCaoDotNhanViewModel GetTongSoTienBaoCao(List<BaoCaoTongHopNguonViewModel> dataBaoCaoTongHop, IEnumerable<DotNhanBoSungTrongNamViewModel> dotNhanBoSungNam, IEnumerable<DotNhanBoSungTrongNamViewModel> dotDuToanNam)
        {
            TinhTongSoTienBaoCaoDotNhanViewModel data = new TinhTongSoTienBaoCaoDotNhanViewModel();
            try
            {
                var listDateDotNhanBoSung = dotNhanBoSungNam.Select(x => x.dNgayQuyetDinh).ToList();
                var listDateDotDuToan = dotDuToanNam.Select(x => x.dNgayQuyetDinh).ToList();
                data.TongDauNam = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dTDauNam);
                data.TongChuyenSang = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dtChuyenSang);
                data.TongBoSung = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.NhaNuocBosung);
                data.TongDuToanAll = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.TongDuToan);

                data.DaGiaoTongDauNam = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dagiaoDauNam);
                data.DaGiaoTongChuyenSang = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dagiaoChuyenSang);
                data.DaGiaoTongDuToan = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dagiaoDuToan);
                data.DaGiaoTongDuToanAll = dataBaoCaoTongHop.Where(x => x.depth == "0").Sum(y => y.dagiaoTongDuToan);
                data.ConLai = data.TongDuToanAll - data.DaGiaoTongDuToanAll;

                data.TongCacDotBoSung = new List<decimal>();
                data.DaGiaoTongCacDotDuToan = new List<decimal>();
                foreach (var item in listDateDotNhanBoSung)
                {
                    decimal SoTienTongTungDot = dataBaoCaoTongHop.Where(x => x.depth == "0" && x.dNgayQuyetDinh.Date == item.Date).Sum(y => y.NhaNuocBosung);
                    data.TongCacDotBoSung.Add(SoTienTongTungDot);
                }
                foreach (var item in listDateDotDuToan)
                {
                    decimal SoTienTongTungDot = dataBaoCaoTongHop.Where(x => x.depth == "0" && x.dNgayQuyetDinh.Date == item.Date).Sum(y => y.dagiaoDuToan);
                    data.DaGiaoTongCacDotDuToan.Add(SoTienTongTungDot);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return data;
        }

        public IEnumerable<DotNhanBoSungTrongNamViewModel> GetAllDotNhanBoSungNam(Guid? iNguonNganSach, int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_all_dotnhanbosung.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<DotNhanBoSungTrongNamViewModel>(sql,
                        param: new
                        {
                            iNguonNganSach,
                            iNamLamViec,
                            dDateFrom,
                            dDateTo
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay list dot du toan
        /// </summary>
        /// <param name="iNguonNganSach"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iSoCotBoSung"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public IEnumerable<DotNhanBoSungTrongNamViewModel> GetAllDotDuToan(Guid? iNguonNganSach, int iNamLamViec, int iSoCotBoSung, DateTime? dDateFrom, DateTime? dDateTo)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_all_dotdutoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<DotNhanBoSungTrongNamViewModel>(sql,
                        param: new
                        {
                            iNguonNganSach,
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            iSoCotBoSung
                        },
                        commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool DeleteDotNhanNguon(Guid id)
        {
            try
            {
                NNS_DotNhan objDotNhanNguon = GetDotNhanNguonNS(id);
                var sql = FileHelpers.GetSqlQuery("dm_delete_dotnhannguon.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iId = objDotNhanNguon.iID_DotNhan
                        },
                        commandType: CommandType.Text);

                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public DataTable GetListDotNhanNguonChiTietDatatable(string iId, int iNamLamViec, Dictionary<string, string> filters)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("dm_get_by_id_dotNhanChiTietViewModel.sql");
                string maCTMT = string.Empty;
                string loai = string.Empty;
                string khoan = string.Empty;
                string noiDung = string.Empty;
                if (filters.ContainsKey("sMaCTMT"))
                {
                    maCTMT = filters["sMaCTMT"];
                }
                if (filters.ContainsKey("sLoai"))
                {
                    loai = filters["sLoai"];
                }
                if (filters.ContainsKey("sKhoan"))
                {
                    khoan = filters["sKhoan"];
                }
                if (filters.ContainsKey("sNoiDung"))
                {
                    noiDung = filters["sNoiDung"];
                }

                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd.Parameters.AddWithValue("@iId", iId);
                        cmd.Parameters.AddWithValue("@maCTMT", maCTMT);
                        cmd.Parameters.AddWithValue("@loai", loai);
                        cmd.Parameters.AddWithValue("@khoan", khoan);
                        cmd.Parameters.AddWithValue("@noiDung", noiDung);
                        return cmd.GetTable();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public NNS_DotNhanChiTiet GetDotNhanChiTiet(Guid iId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("dm_get_by_id_dotNhanChiTiet.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirst<NNS_DotNhanChiTiet>(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public int GetMaxIndexNNSDotNhan(int iNamLamViec)
        {
            try
            {
                var sql = "SELECT ISNULL(MAX(iIndex),0) FROM NNS_DotNhan WHERE iNamLamViec = @iNamLamViec";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var entity = conn.QueryFirstOrDefault<int>(
                        sql,
                        param: new
                        {
                            iNamLamViec
                        },
                        commandType: CommandType.Text);

                    return entity;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return -1;
        }

        public DataTable GetAllDMNoiDungChiBQPTheoDotNhan(Guid? iID_DotNhanChiTiet, string sMaLoaiDuToan, string sUserLogin, Dictionary<string, string> _filters)
        {
            try
            {
                var config = _nganSachService.GetCauHinh(sUserLogin);
                int iNamLamViec = config.iNamLamViec;

                var strProc = "";
                if (sMaLoaiDuToan == "002")
                {
                    strProc = "proc_get_tree_all_dmnoidungchi_bqp_002";
                }
                else
                {
                    strProc = "proc_get_tree_all_dmnoidungchi_bqp_001";
                }
                string sTenNoiDungChi = "";
                if (_filters.ContainsKey("sTenNoiDungChi"))
                {
                    sTenNoiDungChi = _filters["sTenNoiDungChi"];
                }
                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(strProc, conn))
                    {
                        cmd.Parameters.AddWithValue("@iID_DotNhanChiTiet", iID_DotNhanChiTiet);
                        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd.Parameters.AddWithValue("@sTenNoiDungChi", sTenNoiDungChi);
                        cmd.CommandType = CommandType.StoredProcedure;
                        return cmd.GetTable();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public NNS_DotNhanChiTiet_NDChi GetDotNhanChiTietNDChi(Guid iID_NoiDungChi, Guid iID_DotNhanChiTiet)
        {
            try
            {
                var sql = "Select * from NNS_DotNhanChiTiet_NDChi where iID_NoiDungChi = @iID_NoiDungChi and iID_DotNhanChiTiet = @iID_DotNhanChiTiet";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<NNS_DotNhanChiTiet_NDChi>(sql,
                        param: new
                        {
                            iID_NoiDungChi,
                            iID_DotNhanChiTiet
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<SoKiemTraDotNhanChiTietNDChiModel> GetSoKiemTraDnctNDC(int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iNamLamViec", iNamLamViec);
                    var items = conn.Query<SoKiemTraDotNhanChiTietNDChiModel>("proc_get_all_skt_dotnhanct_ndc", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool CheckExistLoaiDuToan(int iNamLamViec, string sMaLoaiDuToan, Guid? iID_DotNhan)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_check_exist_loaidutoan.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iCountId = conn.QueryFirst<int>(sql,
                        param: new
                        {
                            iNamLamViec,
                            sMaLoaiDuToan,
                            @iID_DotNhan
                        },
                        commandType: CommandType.Text);

                    return iCountId == 0 ? false : true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public NNS_DotNhanChiTiet GetDotNhanChiTietByIdDotNhanIdNguon(Guid iID_DotNhan, Guid iID_Nguon, int iNamLamViec)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_dotnhanchitiet_by_iddotnhan_idnguon.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.QueryFirstOrDefault<NNS_DotNhanChiTiet>(sql,
                        param: new
                        {
                            iID_DotNhan,
                            iID_Nguon,
                            iNamLamViec
                        },
                        commandType: CommandType.Text);

                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<NNSDotNhanExportDataModel> ExportData(string sMaLoaiDuToan = "", string sSoChungTu = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sNoiDung = "", string sUserName = "")
        {
            var result = new List<NNSDotNhanExportDataModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = new StringBuilder();
                    sql.AppendFormat("SELECT * FROM NNS_DotNhan WHERE iNamLamViec = {0}", config.iNamLamViec);
                    var items = conn.Query<NNS_DotNhan>(sql.ToString());

                    var sqlDotNhanChiTiet = new StringBuilder();
                    sqlDotNhanChiTiet.AppendFormat("SELECT * FROM NNS_DotNhanChiTiet WHERE iNamLamViec = {0}", config.iNamLamViec);
                    var listChiTiet = conn.Query<NNS_DotNhanChiTiet>(sqlDotNhanChiTiet.ToString());

                    var sqlDMNguon = new StringBuilder();
                    sqlDMNguon.AppendFormat("SELECT * FROM v_tree_dmNguon WHERE depth = 0");
                    var listDMNguon = conn.Query<v_tree_dmNguon>(sqlDMNguon.ToString());
                    if (listDMNguon != null && listDMNguon.Any())
                    {
                        var listId = listDMNguon.Select(x => x.iID_Nguon).ToList();
                        if (listId != null && listId.Any())
                        {
                            listChiTiet = listChiTiet.Where(x => listId.Contains(x.iID_Nguon)).ToList();
                        }
                    }

                    if (items != null && items.Any())
                    {
                        if (!string.IsNullOrEmpty(sMaLoaiDuToan))
                        {
                            items = items.Where(x => x.sMaLoaiDuToan != null && x.sMaLoaiDuToan == sMaLoaiDuToan).ToList();
                        }

                        if (!string.IsNullOrEmpty(sSoChungTu))
                        {
                            items = items.Where(x => x.sSoChungTu != null && x.sSoChungTu.Contains(sSoChungTu)).ToList();
                        }

                        if (!string.IsNullOrEmpty(sSoQuyetDinh))
                        {
                            items = items.Where(x => x.sSoQuyetDinh != null && x.sSoQuyetDinh.Contains(sSoQuyetDinh)).ToList();
                        }

                        if (dNgayQuyetDinhTu.HasValue)
                        {
                            items = items.Where(x => x.dNgayQuyetDinh != null && x.dNgayQuyetDinh >= dNgayQuyetDinhTu.Value).ToList();
                        }

                        if (dNgayQuyetDinhDen.HasValue)
                        {
                            items = items.Where(x => x.dNgayQuyetDinh != null && x.dNgayQuyetDinh <= dNgayQuyetDinhDen.Value).ToList();
                        }

                        if (!string.IsNullOrEmpty(sNoiDung))
                        {
                            items = items.Where(x => x.sNoiDung != null && x.sNoiDung.Contains(sNoiDung)).ToList();
                        }

                        var count = 1;
                        foreach (var item in items.OrderBy(x => x.sSoChungTu))
                        {
                            var model = new NNSDotNhanExportDataModel();
                            model.STT = count;
                            model.MaLoaiDuToan = item.sMaLoaiDuToan;
                            model.TenLoaiDuToan = item.sTenLoaiDuToan;
                            model.SoChungTu = item.sSoChungTu;
                            model.SoQuyetDinh = item.sSoQuyetDinh;
                            if (item.dNgayQuyetDinh.HasValue)
                            {
                                model.NgayQuyetDinh = item.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy");
                            }
                            model.NoiDung = item.sNoiDung;
                            if (listChiTiet != null && listChiTiet.Any())
                            {
                                var listCT = listChiTiet.Where(x => x.iID_DotNhan == item.iID_DotNhan && x.SoTien != null).ToList();
                                if (listCT != null && listCT.Any())
                                {
                                    var soTien = listCT.Sum(x => x.SoTien);
                                    if (soTien.HasValue)
                                    {
                                        model.SoTien = soTien.Value;
                                    }
                                }
                            }

                            result.Add(model);
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return result;
        }
        #endregion

        #region Dự toán nhiệm vụ

        public List<NNS_DuToan_NhiemVu_ViewGridModel> GetListDuToan_NhiemVu_ByIdDuToan(string iID_DuToan, string sUserName)
        {
            var result = new List<NNS_DuToan_NhiemVu_ViewGridModel>();
            if (string.IsNullOrEmpty(iID_DuToan))
            {
                return result;
            }
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sqlNhiemVu = new StringBuilder();
                    sqlNhiemVu.AppendFormat("SELECT * FROM NNS_DuToan_NhiemVu WHERE iID_DuToan = '{0}'", iID_DuToan);
                    var items = conn.Query<NNS_DuToan_NhiemVu>(sqlNhiemVu.ToString());
                    var listDuToanChiTiet = conn.Query<NNS_DuToanChiTiet>("SELECT * FROM NNS_DuToanChiTiet");
                    var sqlNganSach = new StringBuilder();
                    sqlNganSach.AppendFormat("SELECT * FROM NNS_NDChi_MLNS WHERE NamLamViec = '{0}'", config.iNamLamViec);
                    var listNganSach = conn.Query<NNS_NDChi_MLNS>(sqlNganSach.ToString());
                    if (items != null && items.Any())
                    {
                        foreach (var item in items)
                        {
                            var model = new NNS_DuToan_NhiemVu_ViewGridModel();
                            if (item.iID_MaChungTu.HasValue)
                            {
                                model.iID_MaChungTu = item.iID_MaChungTu.Value.ToString();
                            }
                            model.iID_DuToan = iID_DuToan;
                            model.iID_NhiemVu = item.iID_NhiemVu.ToString();
                            model.sNhiemVu = item.sNhiemVu;

                            if (listDuToanChiTiet != null && listDuToanChiTiet.Any())
                            {
                                var listChiTiet = listDuToanChiTiet.Where(x => x.iID_NhiemVu == item.iID_NhiemVu && x.SoTien != null).ToList();
                                if (listChiTiet != null && listChiTiet.Any())
                                {
                                    model.sSoTien = listChiTiet.Sum(x => x.SoTien.Value);
                                }
                            }
                            result.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// get list chung tu chi tiet
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public List<NNS_DuToan_NhiemVu_ChungTuViewModel> GetListChungTuChiTiet(int iNamLamViec)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_chungtuchitiet.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<NNS_DuToan_NhiemVu_ChungTuViewModel>(sql,
                        param: new
                        {
                            iNamLamViec
                        },
                        commandType: CommandType.Text);

                    return iTotal.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool CreateNNSDuToanNhiemVu(string idDuToan, string listIdChungTu, string sUserName, string sIP)
        {
            if (string.IsNullOrEmpty(idDuToan) || string.IsNullOrWhiteSpace(listIdChungTu))
            {
                return false;
            }
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var trans = conn.BeginTransaction();
                    var sqlChungTu = new StringBuilder();
                    sqlChungTu.AppendFormat("SELECT * FROM DTBS_ChungTu WHERE iNamLamViec = '{0}' AND iTrangThai = 1", config.iNamLamViec);
                    var listChungTu = conn.Query<DTBS_ChungTu>(sqlChungTu.ToString());
                    //var listChungTuChiTiet = conn.Query<DTBS_ChungTu>
                    var listId = listIdChungTu.Split(',').ToList();
                    if (listId != null && listId.Any())
                    {
                        foreach (var item in listId)
                        {
                            var model = new NNS_DuToan_NhiemVu();
                            model.iID_NhiemVu = Guid.NewGuid();
                            model.iID_DuToan = Guid.Parse(idDuToan);
                            if (!string.IsNullOrEmpty(item))
                            {
                                model.iID_MaChungTu = Guid.Parse(item);
                            }
                            model.dNgaySua = DateTime.Now;
                            model.dNgayTao = DateTime.Now;
                            model.sID_MaNguoiDungSua = sUserName;
                            model.sID_MaNguoiDungTao = sUserName;
                            model.sIPSua = sIP;
                            if (listChungTu != null && listChungTu.Any())
                            {
                                var chungTu = listChungTu.FirstOrDefault(x => x.iID_MaChungTu == Guid.Parse(item));
                                if (chungTu != null)
                                {
                                    model.sNhiemVu = chungTu.sNoiDung;
                                }
                            }

                            conn.Insert<NNS_DuToan_NhiemVu>(model, trans);
                        }
                    }
                    trans.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public List<NS_PhongBan> GetListDanhSachPhongBan()
        {
            var result = new List<NS_PhongBan>();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NS_PhongBan>("SELECT * FROM NS_PhongBan WHERE iTrangThai = 1 ORDER BY sKyHieu");
                    if (items != null && items.Any())
                    {
                        result.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// get list danh muc noi dung chi con
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public List<DMNoiDungChiViewModel> GetListDanhSachNoiDungChi(int iNamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("nns_get_noidungchi_sotienconlai.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<DMNoiDungChiViewModel>(sql, 
                        param: new
                        {
                            iNamLamViec
                        },
                        commandType: CommandType.Text);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<NNSDuToanChiTietGetSoTienByDonViModel> GetDuLieuSoTienNoiDungChiTheoIdMaChungTu(string sUserName, string iID_NhiemVu)
        {
            var result = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            try
            {
                var config = _nganSachService.GetCauHinh(sUserName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iNamLamViec", config.iNamLamViec);
                    lstParam.Add("@iID_NhiemVu", iID_NhiemVu);
                    var items = conn.Query<NNSDuToanChiTietGetSoTienByDonViModel>("proc_get_nns_dutoan_bosung_tu_chungtu_nhiemvu", lstParam, commandType: CommandType.StoredProcedure);
                    if (items != null && items.Any())
                    {
                        result.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// get so tien theo ma chung tu
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <returns></returns>
        public List<NNSDuToanChiTietGetSoTienByDonViModel> GetSoTienTheoMaChungTu(Guid iID_MaChungTu)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sotien_theo_machungtu.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<NNSDuToanChiTietGetSoTienByDonViModel>(sql,
                        param: new
                        {
                            iID_MaChungTu
                        },
                        commandType: CommandType.Text);

                    return iTotal.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public NNS_DuToan_NhiemVu GetNhiemVuById(string iID_NhiemVu)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = new StringBuilder();
                    sql.AppendFormat("SELECT * FROM NNS_DuToan_NhiemVu WHERE iID_NhiemVu = '{0}'", iID_NhiemVu);
                    var model = conn.QueryFirstOrDefault<NNS_DuToan_NhiemVu>(sql.ToString());
                    return model;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool DeleteDuToanChiTietTheoNhiemVu(List<string> aListIdNhiemVu)
        {
            if (aListIdNhiemVu == null || !aListIdNhiemVu.Any())
            {
                return false;
            }
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    foreach (var item in aListIdNhiemVu)
                    {
                        conn.Execute(string.Format("DELETE NNS_DuToanChiTiet WHERE iID_NhiemVu = '{0}'", item));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool DeleteDuToanChiTietDuToanID(string iIDDuToanID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Execute(string.Format("DELETE NNS_DuToanChiTiet WHERE iID_DuToan = '{0}'", iIDDuToanID));
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        #endregion

        #region Bao cao tong hop giao du toan theo LNS
        /// <summary>
        /// get số tổng dự toán
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<TongTien> GetTongDuToanLNS(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_dutoan_theo_LNS.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<TongTien>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return iTotal.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// get số tổng phân nguồn
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<TongTien> GetTongPhanNguonLNS(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_sum_phannguon_theo_LNS.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var iTotal = conn.Query<TongTien>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            dvt

                        },
                        commandType: CommandType.Text);

                    return iTotal.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// get chi tiet du toan theo noi dung chi
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="dDateFrom"></param>
        /// <param name="dDateTo"></param>
        /// <returns></returns>
        public List<ChiTietBoSung> GetChiTietBoSung(int iNamLamViec, DateTime? dDateFrom, DateTime? dDateTo, int dvt = 1)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("nns_get_chitiet_tien_bosung_LNS.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstData = conn.Query<ChiTietBoSung>(sql,
                        param: new
                        {
                            iNamLamViec,
                            dDateFrom,
                            dDateTo,
                            dvt
                        },
                        commandType: CommandType.Text);

                    return lstData.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }
        #endregion
    }
}
