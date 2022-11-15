using Dapper;
using DapperExtensions;
using DomainModel;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Util;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using Viettel.Extensions;
using Viettel.Models.QLThongTinHopDong;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Helpers;
using static Dapper.SqlMapper;
using static Viettel.Extensions.Constants;
using Attachment = Viettel.Domain.DomainModel.Attachment;

namespace Viettel.Services
{
    public interface IQLVonDauTuService : IServiceBase
    {
        #region Danh mục Loại Công Trình
        /// <summary>
        /// Lấy danh sách loại công trình để hiển thị trên partial View
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaicongTrinhInPartial(string sTenLoaiCongTrinh = "");

        /// <summary>
        /// Get VDT_DM_LoaiCongTrinh By id
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinh</param>
        /// <returns></returns>
        VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhById(Guid iId);

        /// <summary>
        /// Get Thong chi chi tiet 
        /// </summary>
        /// <param name="iIdQuyetToanId"></param>
        /// <returns></returns>
        IEnumerable<VDTGetThongTriChiTietViewModel> GetThongTriChiTiet(Guid iIdQuyetToanId);
        /// <summary>
        /// Get ThongTri QuyetToan detail by ThongTri_Id
        /// </summary>
        /// <param name="iIdThongTriId"></param>
        /// <returns></returns>
        IEnumerable<VDTGetThongTriChiTietViewModel> GetThongTriQuyetToanChiTietByThongTriId(Guid iIdThongTriId);
        /// <summary>
        /// Get data choose parent when edit or create data
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinh</param>
        /// <returns></returns>
        NS_NguonNganSach GetNganSachByMa(string iID_NguonVonID);
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
        #endregion

        #region thong tin du an
        /// <summary>
        /// lay list du an chua co chu truong dau tu
        /// </summary>
        /// <param name="sUserLogin"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iIDChuTruongDauTuSua"></param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn> LayDuAnLapKeHoachTrungHanDuocDuyet();

        IEnumerable<VDT_DA_DuAn> LayDuAnLapKeHoachTrungHanDuocDuyetTheoDonVi(string maDonViThucHienDuAn);

        /// <summary>
        /// lay danh sach du an theo don vi quan ly
        /// </summary>
        /// <param name="iIDChuTruongDauTuSua">
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn> LayDanhSachDuAnTheoDonViQuanLy(Guid iID_DonViQuanLyID);

        /// <summary>
        /// Lay danh sach du an theo loai cong trinh
        /// </summary>
        /// <param name="iId">iID_LoaiCongTrinhID</param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn> GetVDTDADuAnByLoaiCongTrinhID(Guid iId);

        /// <summary>
        /// lay thong tin chi tiet du an theo id
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        VDT_DA_DuAn LayThongTinChiTietDuAn(Guid iId);

        /// <summary>
        /// lay danh sach chu dau tu
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        IEnumerable<DM_ChuDauTu> LayChuDauTu(int iNamLamViec);

        /// <summary>
        /// lay danh muc chu dau tu
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        IEnumerable<DM_ChuDauTu> LayDanhMucChuDauTu(int iNamLamViec);

        /// <summary>
        /// lay danh sach nhom du an
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_NhomDuAn> LayNhomDuAn();

        /// <summary>
        /// lay danh sach hinh thuc quan ly
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_HinhThucQuanLy> LayHinhThucQuanLy();

        /// <summary>
        /// lay danh sach phan cap du an
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_PhanCapDuAn> LayPhanCapDuAn();

        /// <summary>
        /// lay danh sach chi phi
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_ChiPhi> LayChiPhi();

        /// <summary>
        /// lay danh sach nguon ngan sach
        /// </summary>
        /// <returns></returns>
        IEnumerable<NS_NguonNganSach> LayNguonVon();

        /// <summary>
        /// lay danh sach chung tu
        /// </summary>
        /// <param name="iIdMaDonVi">Don Vi</param>
        /// <param name="iNamThucHien">Nam</param>
        /// <param name="iIdNguonVon">Nguon von</param>
        /// <returns></returns>
        IEnumerable<VDT_QT_BCQuyetToanNienDo> GetLicense(string iIdMaDonVi, int iNamThucHien, int iIdNguonVon);

        IEnumerable<VDT_QT_BCQuyetToanNienDo> GetQuyetToanNienDoInThongTri(Guid iIdThongTri, string iIdMaDonVi, int iNamThongTri, int iIdNguonVon);
        /// <summary>
        /// Lấy danh sách loại nguồn vốn
        /// </summary>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <returns></returns>
        IEnumerable<NS_MucLucNganSach> LayLoaiNguonVon(int iNamLamViec);
        #endregion

        #region Thông tin dự án - QL phê duyệt TKTC & TDT
        /// <summary>
        /// Lấy phê duyệt TKTC và TDT theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging Info</param>
        /// <param name="sUserLogin">User đăng nhập</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="bIsTongDuToan">Loại dự toán</param>
        /// <param name="sTenDuAn">Tên dự án</param>
        /// <param name="sSoQuyetDinh">Số quyết định</param>
        /// <param name="dPheDuyetTuNgay">Phê duyệt từ ngày</param>
        /// <param name="dPheDuyetDenNgay">Phê duyệt đến ngày</param>
        /// <param name="sMaDonViQL">Mã đơn vị</param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuToan_ViewModel> GetAllVDTPheDuyetTKTCVaTDT(ref PagingInfo _paging, string sUserLogin, int iNamLamViec, byte bIsTongDuToan, string sTenDuAn = "", string sSoQuyetDinh = "", DateTime? dPheDuyetTuNgay = null,
            DateTime? dPheDuyetDenNgay = null, Guid? sMaDonViQL = null, string sTenDuToan = "");

        /// <summary>
        /// Lấy danh sách dự án theo đơn vị và loại quyết định
        /// </summary>
        /// <param name="iID_DonViQuanLyID">ID đơn vị quản lý</param>
        /// <param name="loaiQuyetDinh">Loại quyết định</param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn> LayDuAnByDonViQLVaLoaiQuyetDinh(string iID_MaDonViQuanLyID, int loaiQuyetDinh);

        /// <summary>
        /// Xóa QL Phê duyệt TKTC và TDT
        /// </summary>
        /// <param name="iID_DuToanID">id dự toán TKTC và TDT</param>
        /// <returns></returns>
        bool XoaQLPheDuyetTKTCvaTDT(Guid iID_DuToanID);

        /// <summary>
        /// Xóa danh sách TKTC và TDT chi phí, nguồn vốn đầu tư
        /// </summary>
        /// <param name="iID_DuToanID">id dự toán TKTC và TDT</param>
        /// <returns></returns>
        bool XoaDanhSachTKTCvaTDTChiPhiNguonVonCu(Guid iID_DuToanID, SqlConnection _connection = null, SqlTransaction _transaction = null);

        /// <summary>
        /// Kiểm tra trùng số QĐ QL TKTC và TDT
        /// </summary>
        /// <param name="sSoQuyetDinh">Số quyết định</param>
        /// <param name="iID_DuToanID">ID dự toán TKTC và TDT</param>
        /// <returns></returns>
        bool KiemTraTrungSoQuyetDinhQLTKTCvaTDT(string sSoQuyetDinh, string iID_DuToanID = "");

        /// <summary>
        /// Lấy thông tin chi tiết Phê duyệt TKTC và TDT
        /// </summary>
        /// <param name="iID_DuToanID">ID TKTC và TDT</param>
        /// <returns></returns>
        VDT_DA_DuToan_ViewModel GetPheDuyetTKTCvaTDTByID(Guid iID_DuToanID);
        VDT_DA_DuToan_ViewModel GetPheDuyetTKTCvaTDTByIDFix(Guid iID_DuToanID, int loaiChungTu);
        List<VDT_DA_DuToan_Nguonvon_ViewModel> GetNguonVonTKTCTDTByDuAnId(Guid iIdDuAnId);

        IEnumerable<VDT_DA_DuToan_ChiPhi_ByPheDuyetDuAn> GetListChiPhiTheoPheDuyetDuAn(Guid? idDuAn);

        List<VDT_DA_DuAnToan_NguonVon_ByPheDuyetDuAn> GetListNguonVonTheoPheDuyetDuAn(Guid? idDuAn);

        List<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoPheDuyetDuAn(Guid? idDuAn);
        List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoTKTC(Guid duToanId);
        List<VDT_DA_DuToan_ChiPhi_ViewModel> GetChiPhiTKTCTDTByDuAnId(Guid iIdDuAnId);
        IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoTKTC(Guid duToanId);
        IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoTKTC(Guid duToanId);
        List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoQDDT(Guid duToanId);
        List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoCTDT(Guid duToanId);
        IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoQDDT(Guid duToanId);
        IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoCTDT(Guid duToanId);
        IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoQDDT(Guid duToanId);
        IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoCTDT(Guid duToanId);
        IEnumerable<VDT_DA_DuAn> GetDuAnByIdDuToan(Guid? iIdDuToanId);
        IEnumerable<VDTQuyetDinhDauTuChiPhiModel> GetListChiPhiQDDTByIdDuToan(Guid duToanId);
        #endregion

        #region QL Thông Tin Gói Thầu
        IEnumerable<ThongTinGoiThauViewModel> GetAllThongTinGoiThau(string tenDuAn = "", string tenGoiThau = "", int giaTriMin = 0, int giaTriMax = 0);
        IEnumerable<VDT_DM_NhaThau> GetAllNhaThau();
        VDT_DA_QDDauTu LayThongTinQDDauTu(Guid iId);
        void DeleteDataTablesOld(Guid iId);
        bool DeleteGoiThau(Guid iId, string sUserLogin);

        DuAnViewModel GetThongTinDuAnByGoiThauId(Guid iId);
        IEnumerable<VDT_DA_DuAn> ListDuAnTheoDonViQuanLy(Guid iID_DonViQuanLyID);
        IEnumerable<VDT_DM_ChiPhi> ListChiPhiByDuAn(Guid iID_DuAnID, DateTime dNgayLap);
        IEnumerable<NS_NguonNganSach> ListNguonVonByDuAn(Guid iID_DuAnID, DateTime dNgayLap);
        IEnumerable<VDT_DA_DuAn_HangMuc> ListHangMucByDuAn(Guid iID_DuAnID, DateTime dNgayLap);
        float? GetTongMucDauTuChiPhi(Guid iId, Guid iID_DuAnID, DateTime dNgayLap);
        float? GetTongMucDauTuNguonVon(int iId, Guid iID_DuAnID, DateTime dNgayLap);
        float? GetTongMucDauTuHangMuc(Guid iId, Guid iID_DuAnID, DateTime dNgayLap);
        IEnumerable<GoiThauChiPhiViewModel> GetListChiPhiDieuChinh(Guid iId, DateTime dNgayLap);
        IEnumerable<GoiThauNguonVonViewModel> GetListNguonVonDieuChinh(Guid iId, DateTime dNgayLap);
        IEnumerable<GoiThauHangMucViewModel> GetListHangMucDieuChinh(Guid iId, DateTime dNgayLap);
        IEnumerable<GoiThauNguonVonViewModel> GetListNguonVonChuaDieuChinhByGoiThau(Guid iId);
        IEnumerable<GoiThauChiPhiViewModel> GetListChiPhiChuaDieuChinhByGoiThau(Guid iID_GoiThauID);
        IEnumerable<GoiThauHangMucViewModel> GetListHangMucChuaDieuChinhByGoiThau(Guid iId);
        IEnumerable<ThongTinHopDongModel> GetThongTinHopDong(Guid goiThauId);
        IEnumerable<GoiThauInfoModel> GetThongTinGoiThauByDuAnIdAndHopDongId(Guid duAnId, Guid hopDongId);
        IEnumerable<ChiPhiInfoModel> GetThongTinChiPhiByGoiThauId(Guid goiThauId);
        IEnumerable<HangMucInfoModel> GetThongTinHangMucAll(Guid iIdHopDongId, List<Guid> listGoiThauId);
        IEnumerable<ChiPhiInfoModel> GetThongTinChiPhiByHopDongId(Guid hopDongId);
        IEnumerable<VDT_QDDT_KHLCNhaThau> GetThongTinKHLCNhaThauByIdKHLC(Guid iD_KHLCNThau);
        IEnumerable<GoiThauInfoModel> GetThongTinGoiThauByHopDongId(Guid hopDongId);
        IEnumerable<HangMucInfoModel> GetThongTinHangMucAllByHopDongId(Guid iIdHopDongId);
        IEnumerable<HangMucInfoModel> GetThongTinDieuChinhHangMucAllByHopDongId(Guid iIdHopDongId);
        void DeleteHopDongDetail(Guid iIdHopDongID);
        string GetSoTaiKhoanNhaThauByIdNhaThau(Guid? iID_NhaThauID);
        #endregion

        #region thong tin du an
        /// <summary>
        /// lay list du an chua co chu truong dau tu
        /// </summary>
        /// <param name="sUserLogin"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>

        ThongTinGoiThauViewModel GetThongTinGoiThau(Guid iId);
        IEnumerable<VDT_DA_DuAn_HangMuc> LayHangMucDauTu();
        #endregion

        #region Phe duyet chu truong dau tu
        /// <summary>
        /// lay danh sach chu truong dau tu
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sNoiDung"></param>
        /// <param name="fTongMucDauTuFrom"></param>
        /// <param name="fTongMucDauTuTo"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        IEnumerable<VDTChuTruongDauTuViewModel> LayDanhSachChuTruongDauTu(ref PagingInfo _paging, int iNamLamViec, string sUserName, string sSoQuyetDinh = "", string sNoiDung = "",
                                                float? fTongMucDauTuFrom = null, float? fTongMucDauTuTo = null, DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, string sMaDonVi = "");
        /// <summary>
        /// kiem tra trung so quyet dinh
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <returns></returns>
        bool KiemTraTrungSoQuyetDinhChuTruongDauTu(string sSoQuyetDinh, string iID_ChuTruongDauTuID = "");

        /// <summary>
        /// lay thong tin chi tiet chu truong dau tu
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        VDTChuTruongDauTuViewModel LayThongTinChiTietChuTruongDauTu(Guid iIDChuTruongDauTuId);

        /// <summary>
        /// lay danh sach chu truong dau tu chi phi theo chu truong dau tu id
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        IEnumerable<VDTChuTruongDauTuChiPhiModel> LayDanhSachChuTruongDauTuChiPhi(Guid iID_ChuTruongDauTuID);
        VDT_DA_QDDauTu GetListQDDauTuByIdChuTruongDauTuID(Guid iID_ChuTruongDauTuID);

        /// <summary>
        /// lay danh sach chu truong dau tu nguon von theo chu truong dau tu id
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        IEnumerable<VDTChuTruongDauTuNguonVonModel> LayDanhSachChuTruongDauTuNguonVon(Guid iID_ChuTruongDauTuID);

        /// <summary>
        /// xoa danh sach chi phi va nguon von cu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        bool XoaDanhSachChiPhiNguonVonCu(Guid iID_ChuTruongDauTuID, Guid? iID_DuAnID);

        /// <summary>
        /// xoa chu truong dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        bool XoaChuTruongDauTu(Guid iID_ChuTruongDauTuID);

        List<VDT_DA_DuAn_HangMuc> GetDataDMDuAnHangMuc(Guid iID_DuAnID);


        /// <summary>
        /// lay listnguonVon,Listhangmuc chu truong dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        /// 
        IEnumerable<VDTChuTruongDauTuNguonVonModel> GetListNguonVonTheoCTDauTuId(Guid iID_ChuTruongDauTuID);
        IEnumerable<VDTDADuAnHangMucModel> GetListHangMucTheoCTDauTuId(Guid iID_ChuTruongDauTuID);

        bool CheckDuAnQuyetDinhDauTu(VDT_DA_ChuTruongDauTuCreateModel model, string sUserName);
        #endregion

        #region Phe duyet du an
        /// <summary>
        /// lay danh sach phe duyet du an
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sMaDonVi"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuViewModel> LayDanhSachPheDuyetDuAn(ref PagingInfo _paging, int iNamLamViec, string sUserName, string sSoQuyetDinh = "",
            string sTenDuAn = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, string sMaDonVi = "");

        /// kiem tra trung so quyet dinh
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <returns></returns>
        bool KiemTraTrungSoQuyetDinhQuyetDinhDauTu(string sSoQuyetDinh, string iID_QDDauTuID = "");

        /// <summary>
        /// lay thong tin chi tiet quyet dinh dau tu
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        VDTQuyetDinhDauTuViewModel LayThongTinChiTietQuyetDinhDauTu(Guid iID_QDDauTuID);

        /// <summary>
        /// lay danh sach quyet dinh dau tu chi phi theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuChiPhiModel> LayDanhSachQuyetDinhDauTuChiPhi(Guid iID_QDDauTuID);

        /// <summary>
        /// lay danh sach quyet dinh dau tu chi phi dieu chinh
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuChiPhiModel> LayDanhSachQuyetDinhDauTuChiPhiDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet);

        /// <summary>
        /// lay danh sach quyet dinh dau tu nguonvon dieu chinh
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuNguonVonModel> LayDanhSachQuyetDinhDauTuNguonVonDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet);

        /// <summary>
        /// lay danh sach quyet dinh dau tu nguon von theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuNguonVonModel> LayDanhSachQuyetDinhDauTuNguonVon(Guid iID_QDDauTuID);

        /// <summary>
        /// lay danh sach hang muc theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuHangMucModel> LayDanhSachQuyetDinhDauTuHangMuc(Guid iID_QDDauTuID);

        /// <summary>
        /// lay danh sach hang muc theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        IEnumerable<VDTQuyetDinhDauTuHangMucModel> LayDanhSachQuyetDinhDauTuHangMucDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet);

        /// <summary>
        /// lay danh sach hang muc theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn_HangMuc> LayDanhSachHangMucTheoDuAnId(Guid iID_DuAnID);

        IEnumerable<ChuTruongHangMucModel> LayDanhSachHangMucTheoChuTruongId(Guid chuTruongId);

        /// <summary>
        /// lay danh sach nguon von theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        IEnumerable<VDTDADSNguonVonTheoIDDuAnModel> LayDanhSachNguonVonTheoDuAnId(Guid iID_DuAnID);
        IEnumerable<NS_NguonNganSach> LayDanhSachNguonVonTheoDuAnInQDDauTu(Guid iID_DuAnID);

        /// <summary>
        /// lay ma du an theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        string LayMaDuAnTheoDuAnId(Guid? iID_DuAnID);

        /// <summary>
        /// xoa danh sach chi phi va nguon von cu cua quyet dinh dau tu
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        bool XoaDanhSachQuyetDinhDauTuChiPhiNguonVonCu(Guid iID_QDDauTuID, SqlConnection _connection = null, SqlTransaction _transaction = null);

        /// <summary>
        /// xoa quyet dinh dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        bool XoaQuyetDinhDauTu(Guid iID_QDDauTuID);

        IEnumerable<VDT_DA_DuToan> GetListDuToanByQDDT(Guid iiDQDDT);

        IEnumerable<VDT_DA_DuToan> GetListDuToanByIdDuAn(Guid idDuAn);

        VDT_DA_ChuTruongDauTu FindChuTruongDauTuByDuAnId(Guid id);

        double GetTMPDTheoChuTruongDauTu(Guid? iIDDuAn);

        IEnumerable<VDTQuyetDinhDauTuNguonVonByChuTruongDauTu> GetListNguonVonTheoChuTruongDauTu(Guid idDuAn);

        IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListHangMucTheoChuTruongDauTu(Guid? idDuAn);
        IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListHangMucTheoQDDauTu(Guid qdDauTuId);
        IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListQuyetDinhDauTuHangMuc(Guid? iID_QDDauTuID, Guid? iID_DuAn_ChiPhi);

        IEnumerable<VDT_DA_DuAn> LayDuAnTaoMoiPheDuyetDuAn(Guid iID_DonViQuanLyID);
        DuAnViewModel GetThongTinDuAnByDuAnId(Guid iId);
        IEnumerable<VDTQuyetDinhDauTuNguonVonModel> GetListNguonVonTheoQDDauTuDauTu(Guid qdDauTuId);
        IEnumerable<VDTQuyetDinhDauTuChiPhiCreateModel> GetListChiPhiTheoQDDauTuDauTu(Guid qdDauTuId);
        VDT_DA_QDDauTu FindQDDauTuByDuAnId(Guid duanId);

        #endregion

        #region Quản lý dự án - NinhNV
        IEnumerable<VDTDuAnInfoModel> GetAllDuAnTheoTrangThai(ref PagingInfo _paging, string sTenDuAn, string sKhoiCong, string sKetThuc, Guid? iID_DonViQuanLyID, Guid? iID_CapPheDuyetID, Guid? iID_LoaiCongTrinhID, int iTrangThai);
        VDTDuAnInfoModel GetDuAnById(Guid? iID_DuAnID);
        VDT_DA_ChuTruongDauTu GetVDTChuTruongDauTu(Guid? iID_DuAnID);
        VDTDuAnThongTinPheDuyetModel GetVDTQDDauTu(Guid? iID_DuAnID);
        VDTDuAnThongTinDuToanModel GetVDTDuToan(Guid? iID_DuAnID);
        VDT_QT_QuyetToan GetVDTQuyetToan(Guid? iID_DuAnID);
        IEnumerable<VDTDuAnListCTDTChiPhiModel> GetListCTDTChiPhi(Guid iID_ChuTruongDauTuID);
        IEnumerable<VDT_DA_ChuTruongDauTu> GetListCTDTByIdCTDT(Guid iID_ChuTruongDauTuID);
        IEnumerable<VDTDuAnListCTDTNguonVonModel> GetListCTDTNguonVon(Guid iID_ChuTruongDauTuID);
        bool deleteVDTDuAn(Guid iID_DuAnID);
        IEnumerable<VDTDuAnListQDDTChiPhiModel> GetListQDDTChiPhi(Guid iID_QDDauTuID, Guid iID_DuAnID);
        IEnumerable<VDTDuAnListQDDTNguonVonModel> GetListQDDTNguonVon(Guid iID_QDDauTuID, Guid iID_DuAnID);
        IEnumerable<VDTDuAnListQDDTHangMucModel> GetListQDDTHangMuc(Guid iID_QDDauTuID, Guid iID_DuAnID);
        IEnumerable<VDTDuAnListDuToanChiPhiModel> GetListDuToanChiPhi(Guid iID_DuToanID, Guid iID_DuAnID);
        IEnumerable<VDTDuAnListDuToanNguonVonModel> GetListDuToanNguonVon(Guid iID_DuToanID, Guid iID_DuAnID);
        IEnumerable<VDT_DA_DuToan> GetListDuToanDieuChinh(Guid iID_DuAnID);
        VDT_DM_PhanCapDuAn GetPhanCapDuanByChuTruongDauTu(Guid? capPheDuyetID);
        bool CheckExistMaDuAn(Guid iID_DuAnID, string sMaDuAn);
        DataTable GetDuAnInQuyetToanNienDo(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID);
        VDT_QT_BCQuyetToanNienDo GetDataBCQuyetToanNienDo(int iLoaiThanhToan, Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach);
        List<VDTQtBcquyetToanNienDoExportModel> GetDataBCQuyetToanNienDoDetail(Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach);
        List<VDTQtBcquyetToanNienDoExportModel> GetQuyetToanNienDoVonUngByParentId(Guid iID_BCQuyetToanNienDoID);
        bool CheckExistDuAnInQDDT(Guid iID_DuAnID);
        bool CheckExistDuAnInCTDT(Guid iID_DuAnID);
        Guid? getQuyetToanID(Guid? iID_DuAnID);

        /// <summary>
        /// Lấy danh sách nguồn vốn được tạo ở màn hình thêm mới dự án
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        IEnumerable<VDTDuAnListNguonVonTTDuAnModel> GetListDuAnNguonVonTTDuAn(Guid? iID_DuAnID);

        /// <summary>
        /// Lấy danh sách hạng mục được tạo ở màn thêm mới dự án
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        IEnumerable<VDT_DA_DuAn_HangMucModel> GetListDuAnHangMucTTDuAn(Guid? iID_DuAnID);


        /// <summary>
        /// get iMaDuAnIndex
        /// </summary>
        /// <returns></returns>
        int GetiMaDuAnIndex();

        /// <summary>
        /// get indexMaHangMuc
        /// </summary>
        /// <returns></returns>
        int GetIndexMaHangMuc();

        #endregion

        ///không dùng
        #region Kế hoạch vốn - LongDT
        /// <summary>
        /// Get data GridView in kế hoạch vốn năm 
        /// </summary>
        /// <param name="iNamKeHoac"></param>
        /// <param name="iDonViQuanLy"></param>
        /// <param name="iNguonVon"></param>
        /// <param name="iLoaiNganSach"></param>
        /// <param name="dNgayLap"></param>
        /// <returns></returns>
        IEnumerable<KeHoachVonNamViewModel> GetKeHoachVonNamView(int iNamKeHoac, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, DateTime? dNgayLap);

        KeHoachVonNamViewModel GetThongTinChiTietByMaDuAnId(Guid iIdDuAn, int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, Guid iNganh, DateTime? dNgayLap);
        IEnumerable<VDTKHVPhanBoVonChiTietViewModel> GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID(Guid iId);

        /// <summary>
        /// get paging VDT_KHV_PhanBoVon
        /// </summary>
        /// <param name="_paging">paging</param>
        /// <param name="sSoKeHoach">sSoQuyetDinh</param>
        /// <param name="iNamKeHoach">iNamKeHoach</param>
        /// <param name="dNgayLapFrom">dNgayQuyetDinh from</param>
        /// <param name="dNgayLapTo">dNgayQuyetDinh To</param>
        /// <param name="DonViQuanLy">iID_DonViQuanLyID</param>
        /// <returns></returns>
        IEnumerable<VDTKHVPhanBoVonViewModel> GetAllPhanBoVonPaging(ref PagingInfo _paging, string sSoKeHoach = "", int? iNamKeHoach = null, DateTime? dNgayLapFrom = null, DateTime? dNgayLapTo = null, Guid? sDonViQuanLy = null);

        /// <summary>
        /// Get VDT_KHV_PhanBoVon by id
        /// </summary>
        /// <param name="iId">iID_PhanBoVonID</param>
        /// <returns></returns>
        VDT_KHV_PhanBoVon GetPhanBoVonByID(Guid iId);

        /// <summary>
        /// Insert VDT_KHV_PhanBoVon
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool InsertVdtKhvPhanBoVon(ref VDT_KHV_PhanBoVon data);

        /// <summary>
        /// Update UpdateVdtKhvPhanBoVon
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateVdtKhvPhanBoVon(VDT_KHV_PhanBoVon data);

        /// <summary>
        /// Insert list VDT_KHV_PhanBoVon_ChiTiet
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        bool InsertPhanBoVonChiTiet(ref List<VDT_KHV_PhanBoVon_ChiTiet> lstData, bool bChinhSua = false);

        bool DeleteKeHoachNamChiTietByKeHoachNamId(Guid iId);

        VDT_KHV_PhanBoVon GetPhanBoVonDuplicate(Guid iDonViQuanLy, int iNguonVonId, int iNamKeHoach);

        //NinhNV Phan Bo Von start
        bool CheckDupKeHoachNam(Guid iID_PhanBoVonID, Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach);
        bool CheckDupSoKeHoach(Guid iID_PhanBoVonID, Guid iID_DonViQuanLyID, string sSoQuyetDinh);
        bool deleteKeHoachVonNam(Guid iID_PhanBoVonID);
        //NinhNV end

        #endregion

        #region Thong tri thanh toan

        void UpdateThongTriById(VDT_ThongTri data);

        /// <summary>
        /// lay danh sach thong tri thanh toan
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <returns></returns>
        IEnumerable<VDTThongTriModel> LayDanhSachThongTri(ref PagingInfo _paging, int iNamLamViec, string sUserName, bool bIsThanhToanView = false, string sMaDonVi = "", string sMaThongTri = "",
            int? iNamThongTri = null, DateTime? dNgayThongTri = null, bool isXuatDanhSach = false);

        /// <summary>
        /// Lay danh sach thong tri quyet toan
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <returns></returns>
        IEnumerable<VDTThongTriModel> GetThongTriQuyetToanPaging(ref PagingInfo _paging, string sMaDonVi = null, string sMaThongTri = null, int? iNamThongTri = null, DateTime? dNgayThongTri = null);

        /// <summary>
        /// lay chi tiet de nghi thanh toan
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="iNguonVon"></param>
        /// <param name="dNgayLapGanNhat"></param>
        /// <param name="dNgayTaoThongTri"></param>
        /// <returns></returns>
        IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTiet(string sMaDonViQuanLy, int iNamThongTri, int iNguonVon, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri);
        IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTiet_v1(string sMaDonViQuanLy, int iNamThongTri, int iNguonVon, DateTime? dNgayTaoThongTri);
        /// <summary>
        /// lay chi tiet de nghi thanh toan ung
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="sMaNhomQuanLy"></param>
        /// <param name="dNgayLapGanNhat"></param>
        /// <param name="dNgayTaoThongTri"></param>
        /// <returns></returns>
        IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTietUng(string sMaDonVi, int iNamThongTri, string iNguonVon, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri);

        /// <summary>
        /// lay ngay lap gan nhat theo don vi, nam thong tri, ma nguon von
        /// </summary>
        /// <param name="iIDDonViId"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="iNguonVon"></param>
        /// <returns></returns>
        string LayNgayLapGanNhat(string iIDDonViId, int iNamThongTri, int iNguonVon, string iID_ThongTriID = "");

        /// kiem tra trung ma thong tri
        /// </summary>
        /// <param name="sMaThongTri"></param>
        /// <returns></returns>
        bool KiemTraTrungMaThongTri(string sMaThongTri, string iID_ThongTriID);

        /// <summary>
        ///  lay danh sach kieu thong tri
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_KieuThongTri> LayDanhSachKieuThongTri();

        /// <summary>
        /// lay thong tin chi tiet cua thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <returns></returns>
        VDTThongTriModel LayChiTietThongTri(string iID_ThongTriID);

        /// <summary>
        /// luu thong tin thong tri
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lstDetail"></param>
        /// <returns></returns>
        bool LuuThongTinThongTriQuyetToan(VDT_ThongTri model, List<VDT_ThongTri_ChiTiet> lstDetail, string sUserLogin);

        //bool SaveThongTriChiTiet()

        /// <summary>
        /// lay thong tin chi tiet cua thong tri chi tiet theo kieu thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <param name="iID_KieuThongTriID"></param>
        /// <returns></returns>
        IEnumerable<VDTThongTriChiTiet> LayThongTriChiTietTheoKieuThongTri(string iID_ThongTriID, string iID_KieuThongTriID);

        /// <summary>
        /// Get all DM_LoaiThongTri
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_LoaiThongTri> GetLoaiThongTri();

        /// <summary>
        /// xoa thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <returns></returns>
        bool XoaThongTri(Guid iID_ThongTriID);

        /// <summary>
        /// lay danh sach thong tri thanh toan
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <returns></returns>
        IEnumerable<VDTThongTriModel> LayDanhSachThongTriXuatFile(int iNamLamViec, string sUserName, string sMaDonVi, string sMaThongTri,
            int? iNamThongTri, DateTime? dNgayThongTri);
        //IEnumerable<VDTGetThongTriChiTietViewModel> LayDanhSachThongTriXuatFile_v1(int iNamLamViec, string sUserName, string sMaDonVi, string sMaThongTri,
        //    int? iNamThongTri, DateTime? dNgayThongTri);
        #endregion

        /// <summary>
        /// lay data bao cao du toan nsqp
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="sMaDonVi"></param>
        /// <returns></returns>
        List<RptDuToanNSQPNam> LayDataBaoCaoDuToanNSQPNam(int iNamKeHoach, string sMaDonVi);
        #region Giải ngân - Thanh Toán
        /// <summary>
        /// Get VDT_DA_DuAn in GiaiNganThanhToan sreen
        /// </summary>
        /// <param name="iID_DonViQuanLyID"></param>
        /// <param name="iID_NguonVonID"></param>
        /// <param name="iID_LoaiNguonVonID"></param>
        /// <param name="dNgayQuyetDinh"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iID_NganhID"></param>
        /// <returns></returns>
        DataTable GetDuAnInGiaiNganThanhToan(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID);
        bool UpdateDeNghiThanhToan(VDT_TT_DeNghiThanhToan data);
        bool DeletePheDuyetThanhToanChiTietByThanhToanId(Guid iId);
        bool DeleteDeNghiThanhToan(Guid iId);
        bool checkExistDonViVonNamPheDuyetChiTiet(List<Guid> listVonNamPheDuyetChiTietIds);
        bool KiemTraTrung(string sTable, string sColumn, string sMaThongTri);

        VDT_TT_DeNghiThanhToan GetDeNghiThanhToanByID(Guid iId);
        IEnumerable<GiaiNganThanhToanChiTietViewModel> GetDeNghiThanhToanChiTietByDeNghiThanhToanID(Guid iId);
        IEnumerable<GiaiNganThanhToanViewModel> GetAllGiaiNganThanhToanPaging(ref PagingInfo _paging, string sSoDeNghi = "", int? iNamKeHoach = null, DateTime? dNgayLapFrom = null, DateTime? dNgayLapTo = null, Guid? sDonViQuanLy = null);
        IEnumerable<GiaiNganThanhToanViewModel> LoadVDTTTDeNghiThanhToanIndex(ref PagingInfo _paging, int workYear, string maNguoiDung, int? iNamKeHoach = null, DateTime? dNgayDeNghiFrom = null, DateTime? dNgayDeNghiTo = null, string iidDuAn = "", string iLoaiThanhToan = "", string sMaDonVi = "", string sSoDeNghi = "");

        /// <summary>
        /// get list mlns by du an
        /// </summary>
        /// <param name="iIdDuAn"></param>
        /// <param name="iIdMaDonViQuanLy"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="dNgayLap"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        IEnumerable<VDTKHVPhanBoVonChiTietViewModel> GetAllMucLucNganSachByDuAnId(Guid iIdDuAn, string iIdMaDonViQuanLy, int iIdNguonVon, DateTime? dNgayLap, int iNamLamViec);
        #endregion

        #region Quản lý kế hoạch vốn ứng được duyệt - NinhNV
        IEnumerable<VDT_DM_NhomQuanLy> GetNhomQuanLyList();
        IEnumerable<VDT_DA_DuAn> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh);
        double GetTongMucDauTuTheoIdDuan(Guid iID_DuAnID, DateTime? dNgayQuyetDinh);
        IEnumerable<VDTKeHoachVonUngInfoModel> GetAllKHVUDuocDuyet(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh = null, DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iIdNguonVon = null, string iID_MaDonViQuanLyID = null);
        IEnumerable<VdtKhvKeHoachVonUngChiTietModel> GetKHVUChiTietList(Guid? iID_KeHoachUngID);
        bool deleteKHVUChiTiet(Guid iID_KeHoachUngID);
        bool deleteKHVU(Guid iID_KeHoachUngID, string username);
        VDTKeHoachVonUngDuocDuyetViewModel GetKHVUById(Guid iID_KeHoachUngID, int iNamLamViec);
        bool CheckExistSoQuyetDinh(Guid? iID_KeHoachUngID, string sSoQuyetDinh);
        IEnumerable<VDT_KHV_KeHoachVonUng_DX> GetKeHoachVonUngDeXuatInVonUngDuocDuyetScreen(string iIdMaDonVi, int iNamKeHoach, DateTime dNgayLap);
        IEnumerable<VdtKhvKeHoachVonUngChiTietModel> GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(Guid? iID_KeHoachUngID, List<Guid> listDuaAnIds, Guid? iId_KHVUDX_ID);
        VDTKhvkeHoachVonUngViewModel GetKHVUDuocDuyetById(Guid? iID_KeHoachUngID);
        Guid CheckExistXauNoiMaAndGetIdNganh(int iNamLamViec, string sXauNoiMa);
        bool CheckDuplicateXauNoiMa(int iNamLamViec, string sXauNoiMa);
        bool KehoachVonUngDuocDuyetCtSave(List<VdtKhvKeHoachVonUngChiTietModel> listData, bool isUpdate, Guid? iId_KeHoachUngID, bool isDieuChinh = false);
        IEnumerable<VdtKhvKeHoachVonUngChiTietModel> KehoachVonUngDuocDuyetChiTietExport(Guid? iIDKeHoachVonUngID);

        #endregion

        #region Quản lý phê duyệt vốn ứng ngoài chỉ tiêu - NinhNV
        IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhKHVU(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh);
        IEnumerable<VDT_DA_TT_HopDong> LayDanhSachHopDongTheoDuAn(Guid iID_DuAnID);
        double GetLuyKeKHVUDuocDuyet(Guid iID_DuAnID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime? dNgayQuyetDinh);
        VDTPheDuyetVonUngNgoaiCTViewModel GetThongTinHopDong(Guid iID_HopDongID, DateTime? dNgayQuyetDinh);
        VDTPheDuyetVonUngNgoaiCTViewModel GetLuyKeUng(Guid iID_DuAnID, Guid? iID_HopDongID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime? dNgayQuyetDinh);
        IEnumerable<VDTPheDuyetVonUngNgoaiChiTieuInfoModel> GetAllPheDuyetVonUngNCT(ref PagingInfo _paging, Guid? iID_DonViQuanLyID, string sSoDeNghi, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo);
        bool deletePDVUNCTChiTiet(Guid iID_DeNghiThanhToanID);
        bool deletePDVUNCT(Guid iID_DeNghiThanhToanID);
        IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> GetPDVUNCTChiTietList(Guid? iID_DeNghiThanhToanID);
        VDTPheDuyetVonUngNgoaiCTViewModel GetPDVUNCTById(Guid iID_DeNghiThanhToanID);
        bool CheckExistSoDeNghi(Guid iID_DeNghiThanhToanID, string sSoDeNghi);
        #endregion

        #region Kế hoạch 5 năm
        IEnumerable<VDT_DA_DuAn> GetVDTDADuAn();
        bool InsertKeHoach5Nam(ref VDT_KHV_KeHoach5Nam data);
        bool InsertKeHoach5NamChiTiet(ref List<VDT_KHV_KeHoach5Nam_ChiTiet> lstData);
        IEnumerable<KeHoach5NamViewModel> LayDanhSachKeHoach5Nam(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null,
            int? iGiaiDoanTu = null, int? iGiaiDoanDen = null, string sMaDonVi = "");
        KeHoach5NamViewModel Get_KeHoach5Nam(Guid id);
        IEnumerable<KeHoach5NamChiTietViewModel> ListKeHoach5NamChiTiet(Guid iID_KeHoach5NamID, int iGiaiDoanTu, int iGiaiDoanDen, DateTime? dNgayLap = null);
        void DeleteKeHoachChiTiet(Guid iId);
        bool deleteKH5Nam(Guid iID, string sUserLogin);
        IEnumerable<VDT_DA_DuAn> ListDuAnTheoDonViQLAndNgayLap(Guid iID_DonViQuanLyID, DateTime dNgayLap);
        bool CheckDuplicate(Guid iDDonVi, int giaiDoanTu, int giaiDoanDen, string soKeHoach, out byte error);
        #endregion

        #region Thông tin dự án - QL thông tin hợp đồng
        /// <summary>
        /// Lấy danh sách quản lý thông tin hợp đồng theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="_paging">Paging info</param>
        /// <param name="sUserLogin">User login</param>
        /// <param name="iNamLamViec">Năm làm việc</param>
        /// <param name="sSoHopDong">Số hợp đồng</param>
        /// <param name="fTienHopDongTu">Giá trị hợp đồng nhỏ nhất tìm kiếm</param>
        /// <param name="fTienHopDongDen">Giá trị hợp đồng lớn nhất tìm kiếm</param>
        /// <param name="dHopDongTuNgay">Hợp đồng từ ngày</param>
        /// <param name="dHopDongDenNgay">Hợp đồng đến ngày</param>
        /// <param name="sTenDuAn">Tên dự án</param>
        /// <returns></returns>
        IEnumerable<VDT_DA_TT_HopDong_ViewModel> GetAllVDTQuanLyTTHopDong(ref PagingInfo _paging, string sUserLogin, int iNamLamViec, string sSoHopDong = "", double? fTienHopDongTu = null,
            double? fTienHopDongDen = null, DateTime? dHopDongTuNgay = null, DateTime? dHopDongDenNgay = null, string sTenDuAn = null, string sTenDonVi = null, string sChuDauTu = null);

        /// <summary>
        /// Lấy danh mục loại hợp đồng
        /// </summary>
        /// <returns></returns>
        IEnumerable<VDT_DM_LoaiHopDong> GetDMPhanLoaiHopDong();

        /// <summary>
        /// Lấy danh sách gói thầu theo dự án
        /// </summary>
        /// <param name="iID_DuAnID">ID dự án</param>
        /// <returns></returns>
        IEnumerable<VDT_DA_GoiThau> LayGoiThauTheoDuAnId(Guid iID_DuAnID);

        /// <summary>
        /// Lấy chi tiết gói thầu theo ID
        /// </summary>
        /// <param name="iID_GoiThauID">ID gói thầu</param>
        /// <returns></returns>
        VDT_DA_GoiThau LayThongTinChiTietGoiThau(Guid iID_GoiThauID);

        /// <summary>
        /// Xóa Quản lý thông tin hợp đồng
        /// </summary>
        /// <param name="iID_HopDongID"></param>
        /// <returns></returns>
        bool XoaQLThongTinHopDong(Guid iID_HopDongID);

        /// <summary>
        /// Lay thong tin  goi thau trong ke hoach lua chon nha thau
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        IEnumerable<VDT_DA_GoiThau> getListGoiThauKHLCNhaThau(Guid iId_GoiThauID);

        VDT_DA_TT_HopDong_ViewModel LayChiTietThongTinHopDong(Guid iID_HopDongID);

        IEnumerable<VDT_DA_TT_HopDong> GetHopDongByThanhToanDuAnId(Guid guid);

        HopDongDetailModel GetDetailHopDongByDuAnId(Guid iID_DuAnID, Guid iID_HopDongID, DateTime dNgayDeNghi, int iNamKeHoach, int iID_NguonVonID, Guid iID_LoaiNguonVonID, Guid iID_NganhID);
        VDT_DA_TT_HopDong_ViewModel LayThongTinChiTietDuAnTheoId(Guid iID_DuAnID);

        /// <summary>
        /// lay gia tri truoc dieu chinh cua hop dong
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dNgayHopDong"></param>
        /// <returns></returns>
        double? LayGiaTriTruocDieuChinhHopDong(Guid id, DateTime dNgayHopDong);
        #endregion
        #region Chủ đầu tư

        IEnumerable<DM_ChuDauTu> GetChuDTListByNamLamViec(int namLamViec);

        #endregion

        #region Báo cáo quyết toán niên độ 
        IEnumerable<VdtQtBcQuyetToanNienDoViewModel> GetAllBaoCaoQuyetToanPaging(ref PagingInfo _paging, string iIdMaDonViQuanLy = null, int? iIdNguonVon = null, DateTime? dNgayDeNghiFrom = null, DateTime? dNgayDeNghiTo = null, int? iNamKeHoach = null, string txtSoChungTu = null);
        VDT_QT_BCQuyetToanNienDo GetBaoCaoQuyetToanById(Guid iId);
        VdtQtBcQuyetToanNienDoViewModel GetBCQuyetToanNienDoById(Guid iId);
        IEnumerable<NS_NguonNganSach> GetNguonNSList();
        IEnumerable<NS_DonVi> GetDonviList(int? iNamLamViec);
        bool SaveTongHop(VDT_QT_BCQuyetToanNienDo data, string userName, string listId);
        //List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNam(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon, int iCoQuanThanhToan);
        //List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTich(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon, int iCoQuanThanhToan);
        //List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUng(Guid? iIDQuyetToan, string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon, int iCoQuanThanhToan);
        List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNam(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon);
        List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNamById(Guid? iIDQuyetToan);
        List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTich(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon);
        List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTichById(Guid? iIDQuyetToan);
        List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUng(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon);
        List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUngById(Guid? iIDQuyetToan);
        bool InsertVdtQtBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data);
        bool UpdateVdtQtBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data);
        bool UpdateQuyetToanNienDoChiTiet(Guid iId, List<VDT_QT_BCQuyetToanNienDo_ChiTiet_01> lstData);
        bool UpdateQuyetToanNienDoChiTietPhanTich(Guid iId, List<VDT_QT_BCQuyetToanNienDo_PhanTich> lstData);
        bool DeleteBaoCaoQuyetToan(Guid iId);
        bool DeleteQuyetToanNienDoTongHop(Guid iId);
        IEnumerable<VdtQtBcQuyetToanNienDoViewModel> GetListTongHopQuyetToanNienDo(string sSodenghi = "", DateTime? dNgaydenghi = null, int? iLoaiThanhtoan = null, Guid? iDonvi = null, int? iNamKeHoach = null, string sTensTennguonvon = "");

        VdtQtBcQuyetToanNienDoViewModel GetQuyetToanNienDoById(Guid ID);
        NS_NguonNganSach GetTenNguonVonByID(int ID);
        VdtQtBcQuyetToanNienDoReturnData SaveQTNDTongHop(VDT_QT_BCQuyetToanNienDo data);
        #endregion

        #region Quyết toán niên độ của trợ lý : LongDT
        IEnumerable<VDTQTDeNghiQuyetToanNienDoTroLyViewModel> GetAllQuyetToanNienDoPaging(ref PagingInfo _paging, Guid? iIdDonViQuanLy = null, int? iIdNguonVon = null, DateTime? dNgayDeNghiFrom = null, DateTime? dNgayDeNghiTo = null, int? iNamKeHoach = null);
        VDT_QT_DeNghiQuyetToanNienDo_TroLy GetQuyetToanNienDoTroLyById(Guid iId);
        decimal GetChiTieuNganSachNamInQTNienDoByDuAn(Guid iIdDuAnId, Guid iIdNganh, int iNamKeHoach, DateTime dNgayQuyetDinh, Guid iIdDonViQuanLyId, int iIdNguonVonId);
        decimal GetCapPhatVonNamNayInQTNienDoByDuAn(Guid iIdDuAnId, Guid iIdNganh, int iNamKeHoach, DateTime dNgayQuyetDinh, Guid iIdDonViQuanLyId, int iIdNguonVonId, Guid iIdLoaiNguonVon);
        decimal GetTongMucDauTuInQTNienDoByDuAn(Guid iIdDuAnId, DateTime dNgayQuyetDinh, int iIdNguonVonId);
        IEnumerable<DeNghiQuyetToanNienDoTroLyChiTietViewModel> GetQuyetToanNienDoTroLyChiTietInGridUpdate(Guid iId);
        bool InsertQuyetToanNienDoTroLy(ref VDT_QT_DeNghiQuyetToanNienDo_TroLy data);
        bool InsertQuyetToanNienDoTroLyChiTiet(List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstData);
        bool RemoveQuyetToanNienDoTroLy(Guid iId);
        bool DeleteQuyetToanNienDoTroLy(Guid iId, string userLogin);
        bool UpdateQuyetToanNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data);
        bool RemoveQuyetToanNienDoTroLyChiTietByParentId(Guid iId);

        /// <summary>
        /// Xóa VDT_QT_DeNghiQuyetToanNienDo_TroLy theo id
        /// </summary>
        /// <param name="iId">iID_DeNghiQuyetToanNienDo_ChiTietID</param>
        /// <param name="userLogin">userLogin</param>
        /// <returns></returns>
        bool RemoveQuyetToanNienDoTroLyChiTiet(Guid iId);

        #endregion

        #region Vốn đầu tư - Quyết toán - Đề nghị quyết toán
        IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> GetAllDeNghiQuyetToanPaging(ref PagingInfo _paging, string sSoBaoCao = "", decimal? sGiaDeNghiTu = null, decimal? sGiaDeNghiDen = null, string sTenDuAn = "", string sMaDuAn = "", string sUserName = "");
        VDT_QT_DeNghiQuyetToan Get_VDT_QT_DeNghiQuyetToanById(Guid id);
        List<VDT_DA_DuAn> GetListAllDuAn(string idDonViQuanLy, string iIdDeNghiQuyetToanId);
        List<NS_DonVi> GetListAllDonVi(string sUserName);
        List<NS_DonVi> GetListAllDonViByCurrentYear(string sUserName, string iNamLamViec);
        bool VDT_QT_DeNghiQuyetToan_SaveData(VDT_QT_DeNghiQuyetToanViewModel sModel, string sUserName);
        VDT_QT_DeNghiQuyetToanGetDuAnModel GetDuLieuDuAnById(string idDuAn, string sUserName);
        bool VDT_QT_DeNghiQuyetToan_Delete(string id, string sUserName);
        NS_DonVi GetDuLieuDonViQuanLyByIdDuAn(string idDuAn, string sUserName);
        VDT_QT_DeNghiQuyetToanViewModel GetDeNghiQuyetToanDetail(string id, string sUserName);
        List<VDT_QT_DeNghiQuyetToanViewModel> ExportData(string sSoBaoCao, decimal? sGiaDeNghiTu, decimal? sGiaDeNghiDen, string sTenDuAn, string sMaDuAn, string sUserName);
        List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToanChiTietById(Guid id);
        List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToan_VonDauTuById(Guid id);
        List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToan_TaiSanById(Guid id);
        List<VDT_QT_DeNghiQuyetToanChiTietViewModel> Get_VDT_QT_DeNghiQuyetToan_ChiPhiById(Guid id);
        #endregion

        #region Quản lý phê duyệt quyết toán DA hoàn thành - NinhNV
        IEnumerable<VDTQLPheDuyetQuyetToanViewModel> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhDNQT(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh);
        VDTQLPheDuyetQuyetToanViewModel GetThongTinDuAn(Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh);
        double GetGiaTriDuToan(Guid iID_DuAnID, Guid iID_ChiPhiID, DateTime? dNgayQuyetDinh);
        double GetGiaTriDuToanNguonVon(Guid iID_DuAnID, int iID_MaNguonNganSach, DateTime? dNgayQuyetDinh);
        double GetTongGiaTriPhanBo(Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh);
        IEnumerable<VDTNguonVonDauTuViewModel> GetLstNoiDungQuyetToan(IEnumerable<VDTNguonVonDauTuTableModel> arrNguonVon, Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh, int iNamLamViec);
        IEnumerable<VDTQLPheDuyetQuyetToanViewModel> GetAllPheDuyetQuyetToan(ref PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sTenDuAn, double? fTienQuyetToanPheDuyetFrom, double? fTienQuyetToanPheDuyetTo);
        VDTQLPheDuyetQuyetToanViewModel GetVdtQuyetToanById(Guid? iID_QuyetToanID);
        IEnumerable<VDTChiPhiDauTuModel> GetLstChiPhiDauTu(Guid? iID_QuyetToanID);
        IEnumerable<VDTNguonVonDauTuModel> GetLstNguonVonDauTu(Guid? iID_QuyetToanID);
        bool deleteQTChiPhiQTNguonVonQTChenhLech(Guid iID_QuyetToanID);
        bool deleteVDTQTDA(Guid iID_QuyetToanID);
        #endregion

        #region Tổng hợp thông tin dự án - NinhNV
        IEnumerable<NS_DonVi> GetDanhSachDonVi(int iNamLamViec);

        IEnumerable<VDTTongHopThongTinDuAnViewModel> GetThongTinTongHopDuAn(Guid iID_DonViQuanLyID, string sTenDuAn, int? iNamKeHoach, int iNamKhoiTao);

        //Tổng hợp thông tin dự án tho yêu cầu mới
        IEnumerable<VDTTongHopThongTinDuAnViewModel> GetThongTinTongHopDuAn_v2(Guid iID_DonViQuanLyID, int? iNamKeHoach);

        //Báo cáo cấp phát
        IEnumerable<VDTBaoCaoCapPhatViewModel> GetThongTinBaoCaoCapPhat(Guid? iIDMaDonVi, int? iIDNguonNganSach, int? iNamKeHoach, int iNamLamViec);
        //Báo cáo tình hình thực hiện dự án
        IEnumerable<VDTTinhHinhDuAnViewModel> GetThongTinTinhHinhThucHienDuAn(Guid? iID_DonViQuanLyID, Guid? iID_DuAn, DateTime? denngay);
        VDTBaoCaoTinhHinhDATTChungViewModel GetThongTinChungBCTTDA(Guid? iID_DuAn, int? nam);
        IEnumerable<VDT_DA_DuAn> GetDuAnByIdDonVi(Guid? iID_DonViQuanLyID);

        #endregion

        #region Kết quả giải ngân vốn đầu tư nguồn vốn NSQP
        IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> GetKetQuaGiaiNganChiKinhPhiDauTu(Guid? iID_DonViQuanLyID, int iID_NguonVonID, int? iNamKeHoach);
        #endregion
        #region Vốn đầu tư - Quyết toán - Tổng hợp số liệu
        IEnumerable<VDT_QT_TongHopSoLieuViewModel> GetAllTongHopSoLieuPaging(ref PagingInfo _paging, Guid? iID_DonViQuanLy, int? iID_NguonVon, int? iNamKeHoach);
        List<NS_NguonNganSach> GetListDataNguonNganSach();
        VDT_QT_TongHopSoLieuDetailViewModel GetDetailTongHopSoLieu(string id);
        VDT_QT_TongHopSoLieu GetTongHopSoLieuById(Guid id);
        List<VDT_QT_TongHopSoLieu_ChiTiet> GetListDataTongHopSoLieuChiTiet(string id);
        List<VDT_QT_TongHopSoLieuViewModel> GetListDataTongHopSoLieu();
        bool ChangeStatusTongHopSoLieu(Guid? id, string typeChange);
        List<NS_DonVi> GetListDataDonVi(string sUserName);
        VDT_QT_TongHopSoLieuCreateViewModel TongHopSoLieu(int? iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap);
        bool TongHopSoLieuSave(Guid? iID_TongHopSoLieu, int? iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap, string typeSave, string sUserName);
        bool DeleteTongHopSoLieu(string id);
        #endregion

        #region Vốn đầu tư - Thông tri
        List<NS_DonVi> GetListDataDonViByUser(string sUserName);
        IEnumerable<VDT_ThongTriViewModel> GetAllThongTriPaging(ref PagingInfo _paging, string sUserName, Guid? iID_DonViQuanLy = null, string sMaThongTri = "", DateTime? dNgayTaoThongTri = null, int? iNamThucHien = null, string sNguoiLap = "", string sTruongPhong = "", string sThuTruongDonVi = "");
        List<NS_NguonNganSach> GetListAllNguonNganSach();
        List<VDT_DM_LoaiCongTrinh> GetListDMLoaiCongTrinh();
        VDT_ThongTriFilterResultModel GetDataThongTriQuyetToanTheoFilter(VDT_ThongTriFilterModel model, string sUserName);
        bool SaveThongTri(VDT_ThongTriFilterModel aModel, string sUserName);

        Dictionary<bool,Guid> InsertThongTriThanhToan(VDTThongTriModel model, string sUserName, List<Guid> lstGuidChecked);
        VDT_ThongTriViewModel GetThongTriById(string id);
        bool DeleteThongTri(string id);
        bool CheckExistMaThongTri(string id, string sMaThongTri);
        VDT_ThongTriExportDataModel ExportData(string sUserName, Guid? iID_DonViQuanLy, string sMaThongTri, DateTime? dNgayTaoThongTri, int? iNamThucHien, string sNguoiLap, string sTruongPhong, string sThuTruongDonVi);

        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTiet(Guid iIdThongTri, string sMaDonVi, int iLoaiThongTri, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon, DateTime? dNgayLapGanNhat, string sMaLoaiCongTrinh = "");
        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietByParentId(Guid iId);
        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTiet(Guid iIdThongTri, string sMaDonVi, int iLoaiThongTri, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon);
        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietById(Guid iId);
        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriQuyetToanChiTiet(string sMaDonVi, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon, DateTime? dNgayLapGanNhat, string sMaLoaiCongTrinh);
        void InsertListThongTriChiTiet(Guid iIdThongTri, List<VDT_ThongTri_ChiTiet> lstData);
        VDT_ThongTri FindThongTriById(Guid iIdThongTri);

        IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietByListIdDeNghiThanhToan(List<Guid> iIds);

        #endregion

        #region Vốn đầu tư - Khởi tạo dự án chuyển tiếp
        /// <summary>
        /// luu data khoi tao
        /// </summary>
        /// <param name="objKhoiTao"></param>
        /// <param name="lstKhoiTaoChiTiet"></param>
        /// <param name="objDuToan"></param>
        /// <param name="objQDDT"></param>
        /// <param name="objDuAn"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        bool LuuKhoiTao(VDT_KT_KhoiTao objKhoiTao, List<VDTKTKhoiTaoChiTietModel> lstKhoiTaoChiTiet, VDT_DA_DuToan objDuToan, VDT_DA_QDDauTu objQDDT,
                        VDT_DA_DuAn objDuAn, List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThau, string sUserName);

        /// <summary>
        /// luu data khoi tao
        /// </summary>
        /// <param name="objKhoiTao"></param>
        /// <param name="lstKhoiTaoChiTiet"></param>
        /// <param name="objDuToan"></param>
        /// <param name="objQDDT"></param>
        /// <param name="objDuAn"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        bool CapNhatKhoiTao(VDT_KT_KhoiTao objKhoiTao, List<VDTKTKhoiTaoChiTietModel> lstKhoiTaoChiTiet, VDT_DA_DuToan objDuToan, VDT_DA_QDDauTu objQDDT,
                            VDT_DA_DuAn objDuAn, List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThau, string sUserName);

        /// <summary>
        /// get data khoi tao du an chuyen tiep theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="iNamKhoiTao"></param>
        /// <returns></returns>
        IEnumerable<VDTQLKhoiTaoDuAnViewModel> GetAllQLKhoiTaoDuAnPaging(ref PagingInfo _paging, string sTenDuAn = "", int? iNamKhoiTao = null);

        /// <summary>
        /// get data khoi tao du an chuyen tiep theo dieu kien
        /// </summary>
        /// <param name="sTenDuAn"></param>
        /// <param name="iNamKhoiTao"></param>
        /// <returns></returns>
        IEnumerable<VDTQLKhoiTaoDuAnViewModel> GetAllQLKhoiTaoDuAnExport(string sTenDuAn, int? iNamKhoiTao);

        /// <summary>
        /// get chi tiet khoi tao du an
        /// </summary>
        /// <param name="iID_KhoiTaoID"></param>
        /// <returns></returns>
        VDTQLKhoiTaoDuAnViewModel GetDetailQLKhoiTaoDuAn(Guid iID_KhoiTaoID);

        #region New
        IEnumerable<VDT_KT_KhoiTao_DuLieu_ViewModel> GetAllKhoiTaoThongTinDuAn(ref PagingInfo _paging, int? iNamKhoiTao = null, string sTenDonVi = "");
        IEnumerable<NS_DonVi> GetListDonViByNamLamViec(int iNamLamViec);
        VDT_KT_KhoiTao_DuLieu_ViewModel GetKhoiTaoTTDAById(Guid iID_KhoiTaoID);
        bool SaveKhoiTaoTTDA(ref Guid iID_KhoiTao, /*ref Guid iID_DonViQL*/ ref string sMaDonVi, VDT_KT_KhoiTao_DuLieu data, string sUserName);
        bool DeleteKhoiTaoTTDA(Guid iID_KhoiTaoID);
        IEnumerable<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel> GetKhoiTaoTTDAChiTietByIdKhoiTao(Guid iID_KhoiTaoID);
        IEnumerable<VDT_DA_DuAn> GetDuAnByMaDonViQL(/*Guid iID_DonViQLID*/ string sMaDonVi);
        bool SaveChiTietKhoiTaoTTDA(List<VDT_KT_KhoiTao_DuLieu_ChiTiet> data, List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstHopDong);
        IEnumerable<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> GetListHopDongKhoiTaoTTDAByKhoiTaoID(Guid iID_KhoiTaoID);
        #endregion

        #endregion

        #region Báo cáo Điều chỉnh kế hoạch năm
        IEnumerable<VDTBaoCaoDieuChinhKeHoachViewModel> LayBaoCaoDieuChinhKeHoach(int? iID_NguonVon, string sLNS, int? iNamThucHien, string UserLogin);
        #endregion

        #region Kế hoạch trung hạn đề xuất - NinhNV
        IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetAllKeHoachTrungHanDeXuatByCondition(int iNamLamViec, int iGiaiDoanTu, int iGiaiDoanDen, Guid? iID_DonViQuanLyID);
        bool LockOrUnLockKeHoach5NamDeXuat(Guid id, bool isLockOrUnLock);
        IEnumerable<VdtKhvKeHoachTrungHanDeXuatChiTietModel> GetAllKH5NamDeXuatDieuChinhChiTiet(Guid idKhthDx);
        IEnumerable<VdtKhvKeHoachTrungHanDeXuatChiTietModel> GetAllKH5NamDeXuatChiTiet(Guid idKhthDx);
        IEnumerable<DuAnKeHoach5Nam> GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(Guid iID_KeHoach5Nam_DeXuatID);
        bool SaveKeHoach5NamDeXuat(ref VDT_KHV_KeHoach5Nam_DeXuat data, string sUserLogin, int iNamLamViec, bool isModified, bool isAggregate, List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> details);
        IEnumerable<KeHoach5NamDeXuatModel> GetAllKeHoach5NamDeXuat(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int? iLoai, int? isTongHop);
        bool deleteKeHoach5NamDeXuat(Guid Id);
        VDT_KHV_KeHoach5Nam_DeXuat GetKeHoach5NamDeXuatById(Guid? Id);
        IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetKeHoach5NamDeXuatByCondition();
        IEnumerable<DuAnKeHoach5Nam> GetAllDuAnChuyenTiep(string iIdMaDonVi);
        KeHoach5NamDeXuatModel GetKeHoach5NamDeXuatByIdForDetail(Guid Id);
        bool CheckExistSoKeHoach(string sSoQuyetDinh, int iNamLamViec, Guid? iId);
        DataTable GetListKH5NamDeXuatChiTietById(string Id, int iNamLamViec, Dictionary<string, string> _filters);
        DataTable GetListKH5NamDeXuatChuyenTiepChiTietById(string Id, Dictionary<string, string> _filters);
        IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> GetAllKH5NamDeXuatChiTiet(int iNamLamViec);
        string GetMaxSTTDuAn(string Id);
        int GetNumchild(string Id);
        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet GetKH5NamDeXuatChaBySTTDuAn(string iID_KeHoach5NamID, string sttDuAnCha);
        int GetMaxIndexCode(string Id, Guid IdParent);
        VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhByMa(string sMaLoaiCongTrinh);
        bool UpdateIsParent(string iID_KeHoach5NamID);
        bool SaveKeHoach5NamDeXuatChiTiet(IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> data);
        bool UpdateGiaTriKeHoach(string iID_KeHoach5NamID);
        bool CheckExistGiaiDoanKeHoach(int iGiaiDoanTu, int iGiaiDoanDen, int iNamLamViec, Guid? iID_DonViQuanLyID, Guid? iId);
        bool CheckExistDonVi_LoaiDuAn_GiaiDoanKeHoach(int iGiaiDoanTu, int iGiaiDoanDen, int iNamLamViec, Guid? iID_DonViQuanLyID, int? iLoai, Guid? iId);
        bool CheckPeriodValid(int iGiaiDoanTu, int iNamLamViec, Guid? iID_DonViQuanLyID);
        VDT_KHV_KeHoach5Nam_DeXuat GetGiaiDoanTuDenDaTrung(int iGiaiDoanTu, int iNamLamViec, Guid? iID_DonViQuanLyID);
        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet GetKH5NamDeXuatChiTietById(Guid? Id);
        IEnumerable<NS_NguonNganSach> GetListDMNguonNganSach();
        IEnumerable<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> FindByReportKeHoachTrungHanDeXuatChuyenTiep(string lstId, string lstBudget, string lstLoaiCongTrinh, string lstUnit, int type, double donViTinh);
        IEnumerable<KH5NDXPrintDataDieuChinhExportModel> FindSuggestionDcReport(int type, string lstId, string lstDonVi, double menhGiaTienTe);
        IEnumerable<KH5NDXPrintDataExportModel> GetDataReportKH5NDeXuat(string id, string lct, string lstNguonVon, string lstMaDonViQL, int type, double donViTinh, int iNamLamViec);
        IEnumerable<PhanBoVonDonViDieuChinhNSQPReport> GetPhanBoVonDieuChinhReport(string lstId, string lstLct, int yearPlan, int type, double donViTienTe);
        IEnumerable<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> GetPhanBoVonDieuChinhNguonVon(int type, string lstId, string lstLct, string lstNguonVon, double donViTienTe);
        IEnumerable<KHVNDXExportModel> GetReportKeHoachVonNam(int type, string theLoaiCongTrinh, string lstId, string lstLct, double donViTinh);
        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetVoucherVonNamDeXuatByCondition(int idNguonVon, List<string> arrDonVi, List<string> arrNguonVon, int iNamLamViec, string isStatus);
        IEnumerable<string> GetVoucherVonNamDeXuatIdByCondition(int idNguonVon, List<string> arrDonVi, List<string> arrNguonVon, int iNamLamViec, string isStatus);
        IEnumerable<string> GetLstIdChungTuDeXuat(int iGiaiDoanTu, int iGiaiDoanDen, bool isModified, bool isCt, int iNamLanViec);
        List<VDT_DM_LoaiCongTrinh> GetListParentDMLoaiCongTrinh();
        #endregion

        #region Kế hoạch vốn năm đề xuất
        IEnumerable<KeHoachVonNamChiTietViewModel> GetAllVonNamDeXuatByIdDx(Guid? iIDVonNamDeXuat);
        IEnumerable<KeHoachVonNamChiTietViewModel> GetAllKHVonNamDeXuatDieuChinhChiTiet(Guid iID_KeHoachVonNamDeXuatID);
        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> GetAllKeHoachVonNamDeXuat(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iID_NguonVonID = null, Guid? iID_DonViQuanLyID = null, int? isTongHop = null);
        VDT_KHV_KeHoachVonNam_DeXuat GetKeHoachVonNamDeXuatById(Guid? iID_KeHoachVonNamDeXuatID);
        VDT_KHV_KeHoachVonNam_DeXuat_ViewModel GetKeHoachVonNamDeXuatViewModelById(Guid? iID_KeHoachVonNamDeXuatID);
        bool CheckExistSoKeHoachVonNamDeXuat(string sSoQuyetDinh, int iNamKeHoach, Guid? iID_KeHoachVonNamDeXuatID);
        bool SaveKeHoachVonNamDeXuat(ref Guid iID, VDT_KHV_KeHoachVonNam_DeXuat data, List<KeHoachVonNamChiTietViewModel> lstDetail, string sUserName, bool isDieuChinh, bool isTongHop, bool bIsDetail);
        DataTable GetListKHVonNamDeXuatChiTietById(string iID_KeHoachVonNamDeXuatID, string lstDuAnID, Dictionary<string, string> _filters);
        List<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet> GetKHVNDeXuatChiTietByParentId(Guid iIdParentID);
        bool SaveKeHoachVonNamDeXuatChiTiet(List<KeHoachVonNamChiTietViewModel> aListModel, List<KeHoachVonNamChiTietViewModel> listDeleted);
        IEnumerable<VdtKhvKeHoach5NamDeXuatExportModel> GetDataExportKeHoachTrungHanDeXuat(Guid iID);
        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetListSoChungTuVonNamDeXuat(Guid? iID_DonViQuanLyID, int iNamLamViec, string isStatus);
        IEnumerable<KHVNDXPrintDataExportModel> GetDataReportKHVNDeXuat(string arrIdKHVNDX, string iID_LoaiCongTrinhs, string iID_NguonVonIDs, int type);
        VDT_KHV_KeHoachVonNam_DeXuat_ViewModel GetKeHoachVonNamDeXuatByIdDetail(Guid iID_KeHoachVonNamDeXuatID);
        List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> GetSTongHopKeHoachVonNamDeXuat(Guid? iID_KeHoachVonNamDeXuatID);
        bool DeleteKeHoachVonNamDeXuat(Guid iID_KeHoachVonNamDeXuatID);
        bool LockOrUnLockKeHoachVonNamDeXuat(Guid id, bool isLockOrUnLock);
        IEnumerable<VDT_DA_DuAn_HangMuc> GetDuAnHangMucByListDuAnID(string iID_KeHoachVonNamDeXuatID, string lstDuAnID);
        IEnumerable<VDTDuAnViewModel> GetDuAnKHVNDXByDonVi(Guid iID_DonViID, int iNamKeHoach, int iID_NguonVonID);
        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetListChungTuTH(Guid iID_DonViQL, int iNamKeHoach);
        IEnumerable<KHVNDXExportExcelModel> GetKeHoachVonNamExcel(string id);

        #endregion

        #region Kế hoạch vốn năm được duyệt
        bool DeleteKeHoachVonNamDuocDuyet(Guid id);
        IEnumerable<KeHoachVonNamDuocDuyetExportDieuChinh> GetKeHoachVonNamDuocDuyetDieuChinhReport(string lstId, double DonViTienTe);
        IEnumerable<VDT_KHV_KeHoachVonNam_DuocDuyet> GetKeHoachVonNamDuocDuyetDieuChinh(string donViQuanLy, string namKeHoach);
        NS_DonVi GetDonViQuanLyById(Guid id);
        IEnumerable<VDTKeHoachVonNamDuocDuyetExport> GetKeHoachVonNamDuocDuyetReport(string lstId, string lstLct, int type, double donViTinh);
        IEnumerable<VDT_KHV_KeHoachVonNam_DuocDuyet> GetKeHoachVonNamDuocDuyetByCondition(string namKeHoach, string donViQuanLy, string nguonVon);
        DataTable GetListKHVonNamDuocDuyetChiTietById(string iIDPhanBoVonID, string iIdPhanBoVonDeXuat, int iNamLamViec, Dictionary<string, string> _filters);
        DataTable GetListKHVonNamPhanBoVonDonViPheDuyetChiTietById(string iIDPhanBoVonID, string iIdPhanBoVonDeXuat, int iNamLamViec, Dictionary<string, string> _filters);
        IEnumerable<VDTKHVPhanBoVonDuocDuyetViewModel> GetAllKeHoachVonNamDuocDuyet(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iID_NguonVonID = null, Guid? iID_DonViQuanLyID = null);

        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetKeHoachVonNamDeXuatTongHopByCondition(int iNamLamViec, Guid? iIdDonViQuanLy, int? iID_NguonVonID);
        VDT_KHV_KeHoachVonNam_DuocDuyet GetKeHoachVonNamDuocDuyetById(Guid? idPhanBoVon);
        VDT_KHV_PhanBoVon_DonVi_PheDuyet GetKeHoachVonNamPhanBoVonDonViPheDuyetById(Guid? idPhanBoVon);
        IEnumerable<NS_MucLucNganSach> GetAllMucLucNganSachByNamLamViec(int yearOfWork);
        bool CheckExistXauNoiMa(int iNamLamViec, string sXauNoiMa);
        bool SaveKeHoachVonNamDuocDuyetChiTiet(List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> data, List<NS_MucLucNganSach> lsMucLucNganSach, Guid iID_KeHoachVonNamDeXuatID, Guid iID_KeHoachVonNamDuocDuyetID);

        /// <summary>
        /// update fCapPhatTaiKhoBac, fCapPhatBangLenhChi cua KHV nam duoc duyet
        /// </summary>
        /// <param name="iID_KeHoachVonNam_DuocDuyetID"></param>
        /// <returns></returns>
        bool UpdateGiaTriCapPhatKHVnamDuocDuyet(Guid iID_KeHoachVonNam_DuocDuyetID);
        IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetAllKeHoachVonNamDeXuatByIdDonVi(Guid? idDonViQuanLy);
        bool SaveKeHoachVonNamDuocDuyet(ref Guid iID, VDT_KHV_KeHoachVonNam_DuocDuyet data, string sUserName, bool isModified);
        bool CheckExistSoKeHoachVonNamDuocDuyet(string sSoQuyetDinh, int iNamKeHoach, Guid? idPhanBoVon);
        bool CheckExistKeHoachVonNamDuocDuyet(string iIdMaDonVi, int iIdNguonVon, int iNamKeHoach, Guid iId);
        IEnumerable<VDTKHVPhanBoVonDuocDuyetViewModel> GetKeHoachVonNamExport(string id);
        IEnumerable<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> GetKeHoachVonNamChiTietExport(string iiDKeHoachVonNam);
        #endregion

        #region Kế hoạch vốn năm phôn bổ đơn vị phê duyệt
        IEnumerable<VDTKHVPhanBoVonDonViPheDuyetViewModel> GetAllKeHoachVonNamDonViPheDuyet(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iID_NguonVonID = null, Guid? iID_DonViQuanLyID = null);
        bool CheckExistSoKeHoachVonNamPhanBoVonDVPheDuyet(string sSoQuyetDinh, int iNamKeHoach, Guid? idPhanBoVon);
        bool CheckExistKeHoachVonNamPhanBoVonDVPheDuyet(string iIdDonViQuanLy, int iIdNguonVon, int iNamKeHoach, Guid Id);
        bool SaveKeHoachVonNamPhanBoVonDVPheDuyet(ref Guid iID, VDT_KHV_PhanBoVon_DonVi_PheDuyet data, string sUserName, bool isModified);
        bool DeleteKeHoachVonNamPhanBoVonDVPheDuyet(Guid id);
        bool SaveKHVonNamDonViPheDuyetChiTiet(List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> listKHVonNamDonViDuocDuyetChiTiet, Guid? iID_KeHoachVonNamDeXuatID);
        VDT_KHV_PhanBoVon_DonVi_PheDuyet GetKeHoachVonPBVDonViPheDuyetById(Guid? idPhanBoVon);

        VDTKHVPhanBoVonDonViPheDuyetViewModel GetKeHoachVonPBVDonViPheDuyetByIdDetail(Guid iID_KeHoachPBV);
        #endregion

        #region Kế hoạch trung hạn được duyệt - NinhNV
        IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetLstChungTuDeXuat(Guid? iID_DonViQuanLyID, int iGiaiDoanTu, int iGiaiDoanden, int iNamLamViec, bool isModified, bool isCt);
        IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> LayDanhSachChungTuDeXuatTheoDonViQuanLy(Guid iID_DonViQuanLyID, int iNamLamViec);
        bool SaveKeHoach5NamDuocDuyet(ref VDT_KHV_KeHoach5Nam data, string sUserLogin, int iNamLamViec, bool isDieuChinh, List<VDT_KHV_KeHoach5Nam_ChiTiet> lstDetail);
        bool UpdateGiaTriKeHoachDuocDuyet(string iID_KeHoach5NamID);
        IEnumerable<KH5NDDPrintDataChuyenTiepExportModel> FindByReportKeHoachTrungHanChuyenTiep(string lstId, string lstBudget, string lstUnit, int type, double donViTinh);
        IEnumerable<VdtKhvKeHoach5NamExportModel> GetDataExportKeHoachTrungHan(string id);
        IEnumerable<KH5NDDPrintDataExportModel> FindByReportKeHoachTrungHan(string id, string lct, int idNguonVon, int type, double donViTinh, string lstDonViThucHienDuAn);
        IEnumerable<KeHoach5NamDuocDuyetModel> GetAllKeHoach5NamDuocDuyet(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int iLoai);
        VDT_KHV_KeHoach5Nam GetKeHoach5NamDuocDuyetById(Guid? Id);
        bool deleteKeHoach5NamDuocDuyet(Guid Id);
        IEnumerable<string> GetLstIdChungTuDuocDuyet(int iGiaiDoanTu, int iGiaiDoanDen, bool isCt, int iNamLanViec);
        KeHoach5NamDuocDuyetModel GetKeHoach5NamDuocDuyetByIdForDetail(Guid iID_KeHoach5NamID);
        IEnumerable<VDT_KHV_KeHoach5Nam> GetLstChungTuDuocDuyet(Guid? iID_DonViQuanLyID, int iNamLamViec, bool isCt);
        bool CheckExistSoKeHoachKH5NDD(string sSoQuyetDinh, int iNamLamViec, Guid? iID_KeHoach5NamID);
        IEnumerable<VDT_KHV_KeHoach5Nam_ChiTiet> GetListKH5NamByIdKHDD(Guid iID_KeHoach5NamID);
        DataTable GetListKH5NamDuocDuyetChiTietById(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> _filters);
        IEnumerable<VDT_KHV_KeHoach5Nam_ChiTiet> GetListKH5NamDuocDuyetChiTietByIdInList(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> _filters);
        DataTable GetListDuAnKH5NamDeXuatChiTietById(string Id, int iNamLamViec, Dictionary<string, string> _filters);
        VDT_DA_DuAn GetVDTDuAnByIdKH5NDXCT(string iID_DuAnKHTHDeXuatID);
        int GetMaxMaDuAnIndex();
        int GetMaxIndexMaHangMuc();
        VDT_DA_DuAn_HangMuc GetVDTDuAnHangMuc(Guid iID_DuAnID, Guid iID_LoaiCongTrinhID, int iID_NguonVonID);
        VDT_DA_DuAn_NguonVon GetVDTDuAnNguonVon(Guid iID_DuAnID, int iID_NguonVonID);
        DataTable GetListDMDuAnKH5NDD(int iNamLamViec, Dictionary<string, string> _filters);
        DM_ChuDauTu GetChuDauTuByMaCDT(int iNamLamViec, string sId_CDT);
        VDT_DA_DuAn GetDuAnByIdDuAn(Guid? iID_DuAnID);
        VDT_DA_DuAn_HangMuc GetVDTDuAnHangMucById(Guid iID_DuAn_HangMucID);
        double GetTongHanMucDauTuNguonVon(Guid iID_DuAnID, int iID_NguonVonID);
        void updateHanMucDauTuDuAn(Guid iID_DuAnID);

        NS_DonVi GetNameDonViQLByMaDV(string sMaDV);
        #endregion

        #region KH lựa chọn nhà thầu
        /// <summary>
        /// get list du toan by du an id
        /// </summary>
        /// <param name="iIdDuAn"></param>
        /// <returns></returns>
        List<VDTDADuToanModel> GetDuToanByDuAnId(Guid iIdDuAn, int loaiChungTu);
        List<VDTDADuToanModel> GetDuToanDieuChinhByDuAnId(Guid iIdDuAn);

        /// <summary>
        /// get danh sach ke hoach lua chon nha thau theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sDonViQuanLy"></param>
        /// <returns></returns>
        IEnumerable<VDTKHLuaChonNhaThauViewModel> GetAllKHLuaChonNhaThauPaging(ref PagingInfo _paging, string sSoQuyetDinh, string sTenDuAn, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? sDonViQuanLy);

        /// <summary>
        /// delete ke hoach lua chon nha thau
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteKHLuaChonNhaThau(Guid id);

        /// <summary>
        /// delete child ke hoach lua chon nha thau: goi thau, goi thau nguon von, goi thau chi phi, goi thau hang muc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteChildGoiThau(Guid id);

        /// <summary>
        /// get detail ke hoach lua chon nha thau
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        VDTKHLuaChonNhaThauViewModel GetDetailKHLuaChonNhaThau(Guid id);

        /// <summary>
        /// get list goi thau by kh lua chon nha thau
        /// </summary>
        /// <param name="iID"></param>
        /// <returns></returns>
        List<VDT_DA_GoiThau> GetListGoiThauByKHLuaChonNhaThauID(Guid? iID);

        /// <summary>
        /// get list goi thau nguon von by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        List<VDT_DA_GoiThau_NguonVon> GetListGoiThauNguonVonByGoiThauID(Guid iID_GoiThauID);

        /// <summary>
        /// get list goi thau chi phi by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        List<VDT_DA_GoiThau_ChiPhi> GetListGoiThauChiPhiByGoiThauID(Guid iID_GoiThauID);

        /// <summary>
        /// get list goi thau hang muc by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        List<VDT_DA_GoiThau_HangMuc> GetListGoiThauHangMucByGoiThauID(Guid iID_GoiThauID);
        IEnumerable<VDT_DA_DuAn> LayDuAnByIdMaDonViQuanLY(Guid iID_DonViQuanLyID);
        IEnumerable<VDT_DA_DuAn> LayDuAnTaoMoiKHLCNT(string iIdMaDonViQuanLy, int iLoaiChungTu);
        IEnumerable<VDTKHLCNhaThauChungTuViewModel> GetChungTuByDuAnAndLoaiChungTu(Guid iIdDuAnId, int iLoaiChungTu, string iID_KHLCNT);
        List<VDTKHLCNTDetailViewModel> GetChungTuDetailByListChungTuId(List<Guid> lstChungTuId, int iLoaiChungTu);
        List<VDTKHLCNTDetailViewModel> GetChungTuDetailByKHLCNTId(Guid iId);
        List<VDT_DM_DuAn_ChiPhi> GetListDuAnChiPhis(Guid? iId_ChiPhiID);


        /// <summary>
        /// Check trung so quyet dinh ke hoach lua chon nha thau
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckExitsSoQuyetDinhKHLCNT(string sSoQuyetDinh, Guid? id);
        #endregion

        #region Cap phat thanh toan
        /// <summary>
        /// lay danh sach du an theo chu dau tu
        /// </summary>
        /// <param name="iID_ChuDauTuID"></param>
        /// <returns></returns>
        List<VDT_DA_DuAn> LayDanhSachDuAnTheoChuDauTu(string sId_CDT, string sId_DonViQuanLy);

        /// <summary>
        /// lay danh sach nguon von theo du an
        /// </summary>
        /// <param name="sId_CDT"></param>
        /// <returns></returns>

        List<NS_NguonNganSach> LayDanhSachNguonVonTheoDuAn(Guid iID_DuAnID);

        /// <summary>
        /// load gia tri thanh toan theo hop dong
        /// </summary>
        /// <param name="iCoQuanThanhToan"></param>
        /// <param name="ngayDeNghi"></param>
        /// <param name="bThanhToanTheoHopDong"></param>
        /// <param name="iIdChungTu"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="thanhToanTN"></param>
        /// <param name="thanhToanNN"></param>
        /// <param name="tamUngTN"></param>
        /// <param name="tamUngNN"></param>
        /// <param name="luyKeTUUngTruocTN"></param>
        /// <param name="luyKeTUUngTruocNN"></param>
        void LoadGiaTriThanhToan(int iCoQuanThanhToan, DateTime ngayDeNghi, bool bThanhToanTheoHopDong, string iIdChungTu, int nguonVonId, int namKeHoach, ref double thanhToanTN, ref double thanhToanNN, ref double tamUngTN, ref double tamUngNN, ref double luyKeTUUngTruocTN, ref double luyKeTUUngTruocNN);
        void LoadGiaTriThanhToanNew(int iCoQuanThanhToan, DateTime ngayDeNghi, bool bThanhToanTheoHopDong, string iIdChungTu, int nguonVonId, int namKeHoach, int? loaiCoQuanTaiChinh, ref double thanhToanTN, ref double thanhToanNN, ref double tamUngTN, ref double tamUngNN, ref double luyKeTUUngTruocTN, ref double luyKeTUUngTruocNN, ref double sumTN, ref double sumNN, int? keHoachVon);

        /// <summary>
        /// get de nghi thanh toan KHV by iid de nghi thanh toan
        /// </summary>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <returns></returns>
        List<VDT_TT_DeNghiThanhToan_KHV> FindDeNghiThanhToanKHVByDeNghiThanhToanID(Guid iID_DeNghiThanhToanID);

        /// <summary>
        /// lay danh sach khv 
        /// </summary>
        /// <param name="duAnId"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <returns></returns>
        List<KeHoachVonModel> GetKeHoachVonCapPhatThanhToan(string duAnId, int nguonVonId, DateTime dNgayDeNghi, int namKeHoach, int iCoQuanThanhToan, Guid iID_DeNghiThanhToanID);

        /// <summary>
        /// load ke hoach von thanh toan
        /// </summary>
        /// <param name="iIdDuAnId"></param>
        /// <param name="iIdNguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <param name="iIdPheDuyet"></param>
        /// <returns></returns>
        List<VdtTtKeHoachVonQuery> LoadKeHoachVonThanhToan(string iIdDuAnId, int iIdNguonVonId, DateTime dNgayDeNghi, int iNamKeHoach, int iCoQuanThanhToan, Guid? iIdPheDuyet = null);

        /// <summary>
        /// lay danh sach de nghi tam ung
        /// </summary>
        /// <param name="duAnId"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <returns></returns>
        List<KeHoachVonModel> GetDeNghiTamUngCapPhatThanhToan(string duAnId, int nguonVonId, DateTime dNgayDeNghi, int namKeHoach, int iCoQuanThanhToan, Guid iID_DeNghiThanhToanID);

        /// <summary>
        /// update phe duyet thanh toan chi tiet
        /// </summary>
        /// <param name="lstData"></param>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <param name="sUserLogin"></param>
        /// <returns></returns>
        bool UpdatePheDuyetThanhToanChiTiet(List<PheDuyetThanhToanChiTiet> lstData, Guid iID_DeNghiThanhToanID, string sUserLogin, int iNamLamViec);

        /// <summary>
        /// luu thanh toan
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        bool LuuThanhToan(GiaiNganThanhToanViewModel data, string userLogin);

        bool LuuPheDuyetThanhToanChiTiet(Guid iIdDeNghiThanhToanId, List<PheDuyetThanhToanChiTiet> data, string userLogin, int iNamLamViec, DateTime dNgayPheDuyet, double? fThueGiaTriGiaTangDuocDuyet, double? fChuyenTienBaoHanhDuocDuyet);

        /// <summary>
        /// get thong tin chi tiet de nghi thanh toan
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        GiaiNganThanhToanViewModel GetDeNghiThanhToanDetailByID(Guid iId);

        /// <summary>
        /// get danh sanh phe duyet thanh toan chi tiet by de nghi thanh toan id
        /// </summary>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <returns></returns>
        List<PheDuyetThanhToanChiTiet> GetListPheDuyetChiTietByDeNghiId(Guid iID_DeNghiThanhToanID);
        List<PheDuyetThanhToanChiTiet> GetListPheDuyetChiTietDetail(Guid iID_DeNghiThanhToanID);
        List<VdtTtDeNghiThanhToanChiPhiQuery> GetChiPhiInDenghiThanhToanScreen(DateTime dNgayDeNghi, Guid iIdDuAnId);
        List<VDT_DM_NhaThau> GetNhaThauByHopDong(Guid iIdHopDongId);
        CapPhatThanhToanReportQuery GetThongTinPhanGhiCoQuanTaiChinh(Guid id, int iNamLamViec);

        DeNghiThanhToanValueQuery LoadGiaTriPheDuyetThanhToanByParentId(Guid id);
        /// <summary>
        /// get mlns by ke hoach von
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="lstCondition"></param>
        /// <returns></returns>
        List<MlnsByKeHoachVonModel> GetMucLucNganSachByKeHoachVon(int iNamLamViec, List<TongHopNguonNSDauTuQuery> lstCondition);
        List<VdtBcTheoDoiChiTieuCapPhatModel> GetBcTheoDoiChiTieuCapPhatBySearch(string iIdMaDonVi, int iNamLamViec);
        #endregion

        #region KH chi quy
        /// <summary>
        /// get kinh phi cuc tai chinh cap
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iIdMaDonVi"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        KinhPhiCucTaiChinhCap GetKinhPhiCucTaiChinhCap(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy);

        /// <summary>
        /// get nhu cau chi tiet
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iIdMaDonVi"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        List<NcNhuCauChi_ChiTiet> GetNhuCauChiChiTiet(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy, long iTiGia = 1);

        /// <summary>
        /// get list nhu cau chi chi tiet by nhu cau chi ID
        /// </summary>
        /// <param name="iID_NhuCauChiID"></param>
        /// <returns></returns>
        List<VDT_NC_NhuCauChi_ChiTiet> GetNhuCauChiChiTietByNhuCauChiID(Guid iID_NhuCauChiID);

        /// <summary>
        /// luu nhu cau chi 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lstChiTiet"></param>
        /// <param name="sUserLogin"></param>
        /// <returns></returns>
        bool LuuNhuCauChi(VDT_NC_NhuCauChi data, List<VDT_NC_NhuCauChi_ChiTiet> lstChiTiet, string sUserLogin, bool? isUpdate);

        /// <summary>
        /// xoa nhu cau chi
        /// </summary>
        /// <param name="iID_NhuCauChiID"></param>
        /// <returns></returns>
        bool XoaNhuCauChi(Guid iID_NhuCauChiID);

        /// <summary>
        /// get danh sach nhu cau chi quy theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoDeNghi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="dNgayDeNghiFrom"></param>
        /// <param name="dNgayDeNghiTo"></param>
        /// <param name="iIDMaDonViQuanLy"></param>
        /// <param name="iIDNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        IEnumerable<NCNhuCauChi> GetAllKHNhuCauChiPaging(ref PagingInfo _paging, string sSoDeNghi = "", int? iNamKeHoach = null, DateTime? dNgayDeNghiFrom = null,
            DateTime? dNgayDeNghiTo = null, string iIDMaDonViQuanLy = null, int? iIDNguonVon = null, int? iQuy = null);

        /// <summary>
        /// get thong tin nhu cau chi theo iNamKeHoach,iIdMaDonVi,iIdNguonVon,iQuy
        /// </summary>
        /// <param name="iNamKeHoach,iIdMaDonVi,iIdNguonVon,iQuy"></param>
        /// <returns></returns>
        VDT_NC_NhuCauChi GetNhuCauChiByiNamKeHoachMaDonVi(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy);

        DataTable GetDataDropdownDonViQuanLy(string sMaNguoiDung, int iNamLamViec);
        /// <summary>
        /// get nhu cau chi tiet
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iIdMaDonVi"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>

        NCNhuCauChi GetNhuCauChiByID(Guid iId);

        #endregion
        #region Trao đổi dữ liệu

        /// <summary>
        /// get danh sach trao đổi dữ liệu theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoChungTu"></param>       
        /// <param name="dNgayChungtu"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="sLoaiTraoDoi"></param>
        /// <param name="TrangThai"></param>
        /// <returns></returns>
        IEnumerable<VDT_NS_TraoDoiDuLieuViewModel> GetAllTraoDoiDuLieuPaging(ref PagingInfo _paging, string sSoChungTu = "", int? iNamKeHoach = null, DateTime? dNgayChungtu = null,
            string sMaDonVi = null, int? sLoaiTraoDoi = null, int? TrangThai = null);
        bool CheckExistSoChungTuTraoDoiDL(string sSoChungTu, int iNamLamViec, Guid? id);

        bool SaveTraoDoiDuDLieu(ref VDT_NS_TraoDoiDuLieuViewModel data, string sUserLogin, int iNamLamViec, bool isUpdate, List<VDT_NS_TraoDoiDuLieu_ChiTiet> lstDataChiTiet);

        // bool SaveChiTietTraoDoiDL(ref List<VDT_NS_TraoDoiDuLieuChiTietViewModel> lstDataChiTiet, bool isUpdate);
        IEnumerable<NS_MucLucNganSach> GetAlTraoDoiDuLieuChiTiet(int iNamLamViec);
        IEnumerable<VDT_NS_TraoDoiDuLieu_ChiTiet> GetAlTraoDoiDuLieuChiTietById(Guid? ID);

        /// <summary>
        /// xoa nhu trao doi du lieu
        /// </summary>
        /// <param name="iID_TraoDoiDuLieuID"></param>
        /// <returns></returns>
        bool XoaTraoDoiDuLieu(Guid iID_TraoDoiDuLieuID);

        /// <summary>
        /// xoa nhu trao doi du lieu chi tiet
        /// </summary>
        /// <param name="iID_TraoDoiDuLieuID"></param>
        /// <returns></returns>
        bool XoaTraoDoiDuLieuChiTiet(Guid iID_TraoDoiDuLieuID);

        /// <summary>
        /// Get nhu trao doi du lieu theo id
        /// </summary>
        /// <param name="iID_TraoDoiDuLieuID"></param>
        /// <returns></returns>
        VDT_NS_Traodoidulieu GetVDTraoDoiDuLieuByID(Guid? iID_TraoDoiDuLieuID);

        VDT_NS_TraoDoiDuLieuViewModel GetDetailVDTraoDoiDuLieuByID(Guid iID_TraoDoiDuLieuID);

        /// <summary>
        /// Get nhu trao doi du lieu theo lstid,inamLamViec
        /// </summary>
        /// <param name="iID_TraoDoiDuLieuID"></param>
        /// <returns></returns>
        IEnumerable<VDT_NS_Traodoidulieu> GetListVDTraoDoiDuLieuByIds(List<Guid> lstId, int iNamLamViec);
        bool ChuyenTrangThaiLstTraoDoiDuLieu(List<VDT_NS_Traodoidulieu> lstData, int iNamLamViec);


        #endregion

        #region Kế hoạch vốn ứng đề xuất
        IEnumerable<VDTKeHoachVonUngDeXuatModel> GetKeHoachVonUngDeXuatByCondition
            (ref PagingInfo _paging, string sSoQuyetDinh = null, DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null,
            int? iNamKeHoach = null, int? iIdNguonVon = null, string iID_MaDonViQuanLyID = null);
        bool CheckExistSoDeNghiKHVNDX(Guid? iID_KeHoachUngID, string sSoDeNghi);
        bool deleteKHVUDX(Guid iID_KeHoachUngID, string username);
        bool deleteKHVUDXChiTiet(Guid iID_KeHoachUngID);
        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetDuAnInVonUngDeXuatByCondition(string sMaDonVi = null, DateTime? dNgayDeNghi = null, string sTongHop = null);
        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetDuAnInVonUngDeXuatByIdDonVi(string sMaDonVi = null);
        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatDetailById(Guid iID);
        VdtKhvuDXChiTietModel GetKeHoachVonUngChiTietById(Guid iId, int iNamLamViec);
        bool LockOrUnLockKeHoachVonUngDeXuat(Guid id);

        /// <summary>
        /// Get kế hoạch vốn ứng đề xuất chi tiét theo lstidDuAn,idKHVU
        /// </summary>
        /// <param name="idKHVU"></param>
        /// <param name="lstidDuAn"></param>
        /// <returns></returns>
        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(Guid? id, List<Guid> lstIdDuAn);

        /// <summary>
        /// Luu  kế hoạch vốn ứng đề xuất chi tiét theo lstidDuAn
        /// </summary>
        /// <param name="isUpdate"> flag phan biet la them moi hay edit </param>
        /// <param name="lstidDuAn"></param>
        /// <returns></returns>
        bool SaveKHVNCT(List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listData, bool isUpdate, Guid iID_KeHoachUngID, bool isDieuChinh = false);

        /// <summary>
        /// GEt kế hoạch vốn ứng đề xuất  theo IDKHVUDX
        /// </summary>
        /// <param name="iId_KHVUDX"></param>
        /// <param name="INamLamViec"></param>
        /// <returns></returns>
        VDTKeHoachVonUngDeXuatModel GetKHVonUngDXByID(Guid? iId_KHVUDX);
        IEnumerable<VDTKeHoachVonUngDeXuatModel> GetKeHoachVonUngDeXuatTongHopByListKhvIds(List<Guid> listKhvIds);
        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatChiTietTongHopById(Guid? id);
        IEnumerable<VDT_DA_DuAn> GetListidDuAnByListMaDuAn(List<string> ListMaDuAns);
        bool KhvuImportSave(VdtKhvuDXChiTietModel data, List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listData, string username);
        IEnumerable<VdtKhvuDXChiTietModel> GetDataReportKHVUDeXuatById(Guid? id);

        #endregion

        #region Luu bang tong hop
        void InsertTongHopNguonDauTu_Tang(string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld = null);
        #endregion

        #region Báo cáo quyết toán dự án hoàn thành
        /// <summary>
        /// get list du toan nguon von by du an
        /// </summary>
        /// <param name="iIdDuAnId"></param>
        /// <returns></returns>
        List<VDTDuToanNguonVonModel> GetListDuToanNguonVonByDuAn(string iIdDuAnId);
        /// <summary>
        /// get list du toan nguon von by du toan
        /// </summary>
        /// <param name="iIdDuToanId"></param>
        /// <returns></returns>
        List<VDTDuToanNguonVonModel> GetListDuToanNguonVonByDuToanId(string iIdDuToanId);
        VDT_DA_DuToan GetDuToanIdByDuAnId(Guid iID_DuAnID);
        IEnumerable<VDT_DA_DuToan> GetAllDuToanIdByDuAnId(Guid iID_DuAnID);
        List<VDT_QT_DeNghiQuyetToan_ChiTiet> GetDeNghiQuyetToanChiTiet(Guid iIdDeNghiQuyetToanId);
        IEnumerable<VDT_QT_DeNghiQuyetToan> GetDeNghiQuyetToan(string sUserName = "");
        IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> FindDeNghiQuyetToan(string userName = "");
        double GetGiaTriDuToanIdByDuAnId(Guid? duanId);
        List<NguonVonQuyetToanKeHoachQuery> GetNguonVonByDuToanId(string duToanId);
        List<VdtDaDuToanChiPhiDataQuery> FindListDuToanChiPhiByDuAn(Guid? duanId);
        List<VDT_QT_DeNghiQuyetToan_ChiTiet> FindByDeNghiQuyetToanId(Guid? deNghiQuyetToanId);
        List<VDT_QT_QuyetToan_ChiTiet> GetQuyetToanChiTiet(Guid? iID_QuyetToanID);
        IEnumerable<VDTQLPheDuyetQuyetToanViewModel> GetDataExportQuyetToan(Guid? iID_QuyetToanID);
        IEnumerable<VDTNguonVonDauTuViewModel> GetDataChiPhiExportQuyetToan(Guid? iID_QuyetToanID);
        #endregion
        #region FTP file
        void InsertFtpRoot(VDT_FtpRoot ftpRoot);
        void InsertFtpFile(VDT_FtpFile ftpFile);
        VDT_FtpRoot GetFtpRoot(string sMaDonVi, string sIpAddress, string sFolderRoot);
        #endregion

        #region Attachment
        Attachment GetAttachment(Guid id);
        IEnumerable<Attachment> GetAllAttachmentByID(Guid id);
        bool SaveAttachment(Attachment attachment);
        bool SaveListAttachment(List<Attachment> attachment);
        bool DeleteAttachment(Guid id);
        bool UpdateAttachment(Attachment attachment);
        #endregion

        /// <summary>
        /// hàm dùng chung để check trùng số quyết định
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool CheckExistSoQuyetDinhShare(string propertyName, string tableName, string checkVal);
        int FindCurrentPeriod(int namLamViec);
    }


    public class QLVonDauTuService : IQLVonDauTuService
    {
        #region Private
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IQLVonDauTuService _default;

        public QLVonDauTuService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }
        public static IQLVonDauTuService Default
        {
            get { return _default ?? (_default = new QLVonDauTuService()); }
        }
        #endregion

        #region Danh mục Loại Công Trình
        public IEnumerable<VDTDMLoaiCongTrinhViewModel> GetListLoaicongTrinhInPartial(string sTenLoaiCongTrinh = "")
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_loaicongtrinh_view.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTDMLoaiCongTrinhViewModel>(sql, commandType: CommandType.Text);

                return items;
            }
        }

        public VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhById(Guid iId)
        {
            var sql = "SELECT * FROM VDT_DM_LoaiCongTrinh WHERE iID_LoaiCongTrinh = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_DM_LoaiCongTrinh>(sql, param: new
                {
                    iId
                },
                                                                        commandType: CommandType.Text);

                return items;
            }
        }
        #region Lay thong tri chi tiet
        public IEnumerable<VDTGetThongTriChiTietViewModel> GetThongTriChiTiet(Guid iIdQuyetToanId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdQuyetToanId", iIdQuyetToanId);

                    var items = conn.Query<VDTGetThongTriChiTietViewModel>("sp_vdt_getthongtrichitiet", lstParam, commandType: CommandType.StoredProcedure);
                    if (items == null) return new List<VDTGetThongTriChiTietViewModel>();
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDTGetThongTriChiTietViewModel> GetThongTriQuyetToanChiTietByThongTriId(Guid iIdThongTriId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdThongTriId", iIdThongTriId);

                    var items = conn.Query<VDTGetThongTriChiTietViewModel>("sp_vdt_getthongtriquyettoan_chitiet_by_thongtriid", lstParam, commandType: CommandType.StoredProcedure);
                    if (items == null) return new List<VDTGetThongTriChiTietViewModel>();
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
        #region Lay Quyet Toan nien do trong man hinh thong tri
        public IEnumerable<VDT_QT_BCQuyetToanNienDo> GetQuyetToanNienDoInThongTri(Guid iIdThongTri, string iIdMaDonVi, int iNamThongTri, int iIdNguonVon)
        {

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdThongTri", iIdThongTri);
                    lstParam.Add("iIdMaDonVi", iIdMaDonVi);
                    lstParam.Add("iNamThongTri", iNamThongTri);
                    lstParam.Add("iIdNguonVon", iIdNguonVon);

                    var items = conn.Query<VDT_QT_BCQuyetToanNienDo>("sp_vdt_qt_get_quyettoanniendo_in_thongtriscreen", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return new List<VDT_QT_BCQuyetToanNienDo>();
        }
        #endregion
        public NS_NguonNganSach GetNganSachByMa(string iID_NguonVonID)
        {
            var sql = "SELECT * FROM NS_NguonNganSach WHERE iID_MaNguonNganSach = @iID_NguonVonID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<NS_NguonNganSach>(sql, param: new
                {
                    iID_NguonVonID
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
            var sql = FileHelpers.GetSqlQuery("vdt_insert_loaicongtrinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        data.sMaLoaiCongTrinh,
                        data.sTenLoaiCongTrinh,
                        data.sTenVietTat,
                        iParentID = data.iID_Parent,
                        data.iThuTu,
                        data.sMoTa,
                        sUserLogin
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
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
                        bPublic
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        public IEnumerable<dynamic> GetAllDMLoaiCongTrinh()
        {
            var sql = "SELECT * FROM VDT_DM_LoaiCongTrinh WHERE bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<dynamic>(sql);

                return items;
            }
        }
        #endregion

        #region Lay data do vao dropdownlist
        /// <summary>
        /// lay list du an chua co chu truong dau tu
        /// </summary>
        /// <param name="sUserLogin"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iIDChuTruongDauTuSua">
        /// <returns></returns>
        public IEnumerable<VDT_DA_DuAn> LayDuAnLapKeHoachTrungHanDuocDuyet()
        {
            var sql = "SELECT  da.* FROM VDT_DA_DuAn da " +
                        "WHERE da.iID_DuAnID in" +
                        "(  SELECT chitiet.iID_DuAnID FROM VDT_KHV_KeHoach5Nam_ChiTiet chitiet)" +
                        "AND da.iID_DuAnID NOT IN" +
                        "(SELECT ct.iID_DuAnID from VDT_DA_ChuTruongDauTu ct)";

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDT_DA_DuAn>(sql);
            }
        }
        public IEnumerable<NS_NguonNganSach> LayDanhSachNguonVonTheoDuAnInQDDauTu(Guid iIDDuAnId)
        {
            var sql = "SELECT dm.* " +
                        "FROM VDT_DA_QDDauTu as tbl " +
                        "INNER JOIN VDT_DA_QDDauTu_NguonVon as nv on tbl.iID_QDDauTuID = nv.iID_QDDauTuID " +
                        "INNER JOIN NS_NguonNganSach as dm on nv.iID_NguonVonID = dm.iID_MaNguonNganSach " +
                        "WHERE tbl.iID_DuAnID = @iIDDuAnId AND tbl.bActive = 1 " +
                        "ORDER BY dm.iID_MaNguonNganSach ";

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<NS_NguonNganSach>(sql,
                    param: new
                    {
                        iIDDuAnId
                    },
                    commandType: System.Data.CommandType.Text);
            }
        }

        public IEnumerable<VDT_DA_DuAn> LayDuAnLapKeHoachTrungHanDuocDuyetTheoDonVi(string maDonViThucHienDuAn)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_duan_by_donvi_chutruongdt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDT_DA_DuAn>(sql,
                    param: new
                    {
                        maDonViThucHienDuAn
                    },
                    commandType: System.Data.CommandType.Text);
            }
        }

        public IEnumerable<VDT_DA_DuAn> LayDuAnTaoMoiPheDuyetDuAn(Guid donViQLId)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_duan_by_donvi_pheduyetduan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDT_DA_DuAn>(sql,
                    param: new
                    {
                        donViQLId
                    },
                    commandType: System.Data.CommandType.Text);
            }
        }

        /// <summary>
        /// lay danh sach du an theo don vi quan ly
        /// </summary>
        /// <param name="iIDChuTruongDauTuSua">
        /// <returns></returns>
        public IEnumerable<VDT_DA_DuAn> LayDanhSachDuAnTheoDonViQuanLy(Guid iID_DonViQuanLyID)
        {
            var sql = "SELECT * FROM VDT_DA_DuAn duan WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID " +
               "AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_DA_QDDauTu dautu WHERE duan.iID_DuAnID = dautu.iID_DuAnID)";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn> GetVDTDADuAnByLoaiCongTrinhID(Guid iId)
        {
            string sSql = "SELECT * FROM VDT_DA_DuAn WHERE iID_LoaiCongTrinhID = @iId ORDER BY sTenDuAn";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sSql,
                   param: new
                   {
                       iId
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay thong tin chi tiet du an theo id
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        public VDT_DA_DuAn LayThongTinChiTietDuAn(Guid iId)
        {
            var sql = "SELECT da.* FROM VDT_DA_DuAn da " +
                        "WHERE da.iID_DuAnID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_DA_DuAn>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);
                return items;
            }
        }

        public DuAnViewModel GetThongTinDuAnByDuAnId(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_pheduyet_get_thongtinduan_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<DuAnViewModel>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);

                return items;
            }
        }

        /// <summary>
        /// lay danh sach chu dau tu
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public IEnumerable<DM_ChuDauTu> LayChuDauTu(int iNamLamViec)
        {
            var sql = "SELECT * FROM DM_ChuDauTu WHERE  iNamLamViec = @iNamLamViec";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_ChuDauTu>(sql,
                   param: new
                   {
                       iNamLamViec
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach chu dau tu
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public IEnumerable<DM_ChuDauTu> LayDanhMucChuDauTu(int iNamLamViec)
        {
            var sql = "SELECT * FROM DM_ChuDauTu WHERE iTrangThai = 1 AND iNamLamViec = @iNamLamViec";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_ChuDauTu>(sql,
                   param: new
                   {
                       iNamLamViec
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach nhom du an
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VDT_DM_NhomDuAn> LayNhomDuAn()
        {
            var sql = "SELECT * FROM VDT_DM_NhomDuAn ORDER BY iThuTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_NhomDuAn>(sql);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach hinh thuc quan ly
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VDT_DM_HinhThucQuanLy> LayHinhThucQuanLy()
        {
            var sql = "SELECT * FROM VDT_DM_HinhThucQuanLy ORDER BY iThuTu;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_HinhThucQuanLy>(sql);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach phan cap du an
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VDT_DM_PhanCapDuAn> LayPhanCapDuAn()
        {
            var sql = "SELECT * FROM VDT_DM_PhanCapDuAn WHERE bActive = 1 ORDER BY iThuTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_PhanCapDuAn>(sql);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach chi phi
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VDT_DM_ChiPhi> LayChiPhi()
        {
            var sql = "SELECT * FROM VDT_DM_ChiPhi ORDER BY iThuTu;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_ChiPhi>(sql);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach nguon ngan sach
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NS_NguonNganSach> LayNguonVon()
        {
            var sql = "SELECT * FROM NS_NguonNganSach WHERE iTrangThai = 1 ORDER BY iSTT;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_NguonNganSach>(sql);
                return items;
            }
        }
        public IEnumerable<VDT_QT_BCQuyetToanNienDo> GetLicense(string iIdMaDonVi, int iNamThucHien, int iIdNguonVon)
        {
            var sql = "SELECT * FROM VDT_QT_BCQuyetToanNienDo WHERE iID_MaDonViQuanLy = @iId_MaDonVi AND iNamKeHoach = @i_NamThucHien AND iID_NguonVonID = @iId_NguonVon";
            using (var conn = _connectionFactory.GetConnection())
            {

                var items = conn.Query<VDT_QT_BCQuyetToanNienDo>(sql, param: new
                {
                    iId_MaDonVi = iIdMaDonVi,
                    i_NamThucHien = iNamThucHien,
                    iId_NguonVon = iIdNguonVon
                },
                commandType: System.Data.CommandType.Text);
                return items;
            }
        }
        public IEnumerable<NS_MucLucNganSach> LayLoaiNguonVon(int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM NS_MucLucNganSach WHERE sL = '' AND sK = '' AND sM = '' AND sTM = '' AND sTTM = '' AND sNG = '' AND iNamLamViec = {0}", iNamLamViec);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(query.ToString());
                return items;
            }

        }
        #endregion

        #region QL Thông Tin Gói Thầu
        public IEnumerable<ThongTinGoiThauViewModel> GetAllThongTinGoiThau(string tenDuAn, string tenGoiThau, int giaTriMin, int giaTriMax)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_thongtingoithau.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<ThongTinGoiThauViewModel>(sql,
                    param: new
                    {
                        tenDuAn,
                        tenGoiThau,
                        giaTriMin,
                        giaTriMax
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<ThongTinHopDongModel> GetThongTinHopDong(Guid goiThauId)
        {
            var sql = FileHelpers.GetSqlQuery("get_thongtin_hopdong.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<ThongTinHopDongModel>(sql,
                    param: new
                    {
                        goiThauId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public ThongTinGoiThauViewModel GetThongTinGoiThau(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_id_thongtingoithau.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<ThongTinGoiThauViewModel>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);
                items.listChiPhi = GetListChiPhiChuaDieuChinhByGoiThau(iId);
                items.listNguonVon = GetListNguonVonChuaDieuChinhByGoiThau(iId);
                items.listHangMuc = GetListHangMucChuaDieuChinhByGoiThau(iId);
                return items;
            }

        }

        public DuAnViewModel GetThongTinDuAnByGoiThauId(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_idgoithau_thongtinduan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<DuAnViewModel>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<GoiThauChiPhiViewModel> GetListChiPhiDieuChinh(Guid iId, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_goithau_chiphi.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauChiPhiViewModel>(sql,
                    param: new
                    {
                        iId,
                        dNgayLap
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<GoiThauChiPhiViewModel> GetListChiPhiChuaDieuChinhByGoiThau(Guid iID_GoiThauID)
        {
            var sql = FileHelpers.GetSqlQuery("get_list_goithau_chiphi_chuadieuchinh.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauChiPhiViewModel>(sql,
                    param: new
                    {
                        iID_GoiThauID
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }
        public IEnumerable<GoiThauNguonVonViewModel> GetListNguonVonDieuChinh(Guid iId, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_goithau_nguonvon.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauNguonVonViewModel>(sql,
                    param: new
                    {
                        iId,
                        dNgayLap
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<GoiThauNguonVonViewModel> GetListNguonVonChuaDieuChinhByGoiThau(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("get_goithau_nguonvon_chuadieuchinh.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauNguonVonViewModel>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }
        public IEnumerable<GoiThauHangMucViewModel> GetListHangMucDieuChinh(Guid iId, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_goithau_hangmuc.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauHangMucViewModel>(sql,
                    param: new
                    {
                        iId,
                        dNgayLap
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<GoiThauHangMucViewModel> GetListHangMucChuaDieuChinhByGoiThau(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("get_goithau_hangmuc_chuadieuchinh.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<GoiThauHangMucViewModel>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public void DeleteDataTablesOld(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_delete_chiphi_nguonvon_hangmuc.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

            }
        }

        public bool DeleteGoiThau(Guid iId, string sUserLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                var goiThau = conn.Get<VDT_DA_GoiThau>(iId, trans);
                if (goiThau == null)
                    return false;
                if (goiThau.iID_ParentID != null)
                {
                    var goiThauCha = conn.Get<VDT_DA_GoiThau>(goiThau.iID_ParentID, trans);
                    if (goiThauCha != null)
                    {
                        goiThauCha.bActive = true;
                        goiThauCha.sUserUpdate = sUserLogin;
                        goiThauCha.dDateUpdate = DateTime.Now;
                        conn.Update(goiThauCha, trans);
                    }
                }

                conn.Delete(goiThau, trans);

                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE VDT_DA_GoiThau_ChiPhi WHERE iID_GoiThauID = '{0}';", iId);
                query.AppendFormat("DELETE VDT_DA_GoiThau_NguonVon WHERE iID_GoiThauID = '{0}';", iId);
                query.AppendFormat("DELETE VDT_DA_GoiThau_HangMuc WHERE iID_GoiThauID = '{0}';", iId);
                conn.Execute(query.ToString(), transaction: trans, commandType: CommandType.Text);

                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<VDT_DM_NhaThau> GetAllNhaThau()
        {
            var sql = "SELECT * FROM VDT_DM_NhaThau;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_NhaThau>(sql);
                return items;
            }
        }

        public VDT_DA_QDDauTu LayThongTinQDDauTu(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("get_QDDauTu_By_DuAnId.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_DA_QDDauTu>(sql, param: new
                {
                    iId
                },
                commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn_HangMuc> LayHangMucDauTu()
        {
            var sql = "select * from VDT_DA_DuAn_HangMuc;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMuc>(sql);
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn> ListDuAnTheoDonViQuanLy(Guid iID_DonViQuanLyID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_duan_by_donvi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DM_ChiPhi> ListChiPhiByDuAn(Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_list_chiphi_by_duan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_ChiPhi>(sql,
                   param: new
                   {
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NS_NguonNganSach> ListNguonVonByDuAn(Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_list_nguonvon_by_duan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_NguonNganSach>(sql,
                   param: new
                   {
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn_HangMuc> ListHangMucByDuAn(Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_list_hangmuc_by_duan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMuc>(sql,
                   param: new
                   {
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public float? GetTongMucDauTuChiPhi(Guid iId, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_tongmucdautuchiphi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var tongMucDauTuChiPhi = conn.QueryFirstOrDefault<float?>(sql,
                   param: new
                   {
                       iId,
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return tongMucDauTuChiPhi;
            }
        }

        public float? GetTongMucDauTuNguonVon(int iId, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_tongmucdautunguonvon.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var tongMucDauTuChiPhi = conn.QueryFirstOrDefault<float?>(sql,
                   param: new
                   {
                       iId,
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return tongMucDauTuChiPhi;
            }
        }

        public float? GetTongMucDauTuHangMuc(Guid iId, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_tongmucdautuhangmuc.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var tongMucDauTuChiPhi = conn.QueryFirstOrDefault<float?>(sql,
                   param: new
                   {
                       iId,
                       iID_DuAnID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return tongMucDauTuChiPhi;
            }
        }



        #endregion

        #region Thông tin dự án - QL phê duyệt TKTC & TDT
        public IEnumerable<VDT_DA_DuToan_ViewModel> GetAllVDTPheDuyetTKTCVaTDT(ref PagingInfo _paging, string sUserLogin, int iNamLamViec, byte bIsTongDuToan, string sTenDuAn = "", string sSoQuyetDinh = "", DateTime? dPheDuyetTuNgay = null,
            DateTime? dPheDuyetDenNgay = null, Guid? sMaDonViQL = null, string sTenDuToan = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sUserName", sUserLogin);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sTenDuAn", sTenDuAn);
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dPheDuyetTuNgay", dPheDuyetTuNgay);
                lstParam.Add("dPheDuyetDenNgay", dPheDuyetDenNgay);
                lstParam.Add("sMaDonViQL", sMaDonViQL);
                lstParam.Add("bIsTongDuToan", bIsTongDuToan);
                lstParam.Add("sTenDuToan", sTenDuToan);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //var items = conn.Query<VDT_DA_DuToan_ViewModel>("proc_get_all_thietkethicong_tongdutoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                var items = conn.Query<VDT_DA_DuToan_ViewModel>("proc_get_all_thietkethicong_tongdutoan_paging_test", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn> LayDuAnByDonViQLVaLoaiQuyetDinh(string iID_MaDonViQuanLyID, int loaiQuyetDinh)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                if (loaiQuyetDinh == Constants.TONG_DU_TOAN)
                {
                    query.Append("SELECT da.* FROM VDT_DA_DuAn da WHERE da.iID_DuAnID NOT IN");
                    //query.Append(" (SELECT dt.iID_DuAnID FROM VDT_DA_DuToan dt WHERE dt.bActive = 1) AND ");
                    query.Append(" (SELECT dt.iID_DuAnID FROM VDT_DA_DuToan dt WHERE dt.bActive = 1) AND da.iID_DuAnID IN (SELECT qddt.iID_DuAnID FROM VDT_DA_QDDauTu qddt) AND");
                }
                else
                {
                    query.Append("SELECT da.* FROM VDT_DA_DuAn da WHERE da.iID_DuAnID NOT IN");
                    //query.AppendFormat(" (SELECT dt.iID_DuAnID FROM VDT_DA_DuToan dt WHERE dt.bLaTongDuToan = {0} AND dt.bActive = 1) AND", Constants.TONG_DU_TOAN);
                    query.AppendFormat(" (SELECT dt.iID_DuAnID FROM VDT_DA_DuToan dt WHERE dt.bLaTongDuToan = {0} AND dt.bActive = 1) AND da.iID_DuAnID IN (SELECT qddt.iID_DuAnID FROM VDT_DA_QDDauTu qddt) AND", Constants.TONG_DU_TOAN);
                }
                query.Append(" da.iID_MaDonViThucHienDuAnID = @iID_MaDonViQuanLyID");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_DA_DuAn>(query.ToString(),
                       param: new
                       {
                           iID_MaDonViQuanLyID
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

        public IEnumerable<VDT_DA_DuAn> GetDuAnByIdDuToan(Guid? iIdDuToanId)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT da.* FROM VDT_DA_DuAn da JOIN VDT_DA_DuToan dt on da.iID_DuAnID = dt.iID_DuAnID");
                query.Append(" WHERE dt.iID_DuToanID = @iIdDuToanId");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_DA_DuAn>(query.ToString(),
                       param: new
                       {
                           iIdDuToanId
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

        public bool XoaQLPheDuyetTKTCvaTDT(Guid iID_DuToanID)
        {
            try
            {
                var sql = "DELETE VDT_DA_DuToan WHERE iID_DuToanID = @iID_DuToanID;";
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var entity = conn.Get<VDT_DA_DuToan>(iID_DuToanID, trans);
                    if (entity == null) return false;

                    if (entity.iID_ParentID != null && entity.iID_ParentID.HasValue && entity.iID_ParentID.Value != Guid.Empty)
                    {
                        var itemParent = conn.Get<VDT_DA_DuToan>(entity.iID_ParentID, trans);
                        if (itemParent != null)
                        {
                            itemParent.bActive = true;
                            itemParent.bIsGoc = false;
                            conn.Update(itemParent, trans);
                        }
                    }
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_DuToanID
                        },
                        transaction: trans,
                        commandType: CommandType.Text);

                    if (r > 0)
                        XoaDanhSachTKTCvaTDTChiPhiNguonVonCu(iID_DuToanID);
                    trans.Commit();
                    conn.Close();
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool XoaDanhSachTKTCvaTDTChiPhiNguonVonCu(Guid iID_DuToanID, SqlConnection _connection = null, SqlTransaction _transaction = null)
        {
            try
            {
                var sql = @"DELETE VDT_DA_DuToan_ChiPhi WHERE iID_DuToanID = @iID_DuToanID; 

                        DELETE VDT_DA_DuToan_Nguonvon WHERE iID_DuToanID = @iID_DuToanID;

                        DELETE hm FROM VDT_DA_DuToan_HangMuc as hm
                            INNER JOIN VDT_DA_DuToan_DM_HangMuc as dm on dm.Id = hm.iID_HangMucID
                            WHERE hm.iID_DuToanID = @iID_DuToanID;

                        DELETE VDT_DA_DuToan_HangMuc WHERE iID_DuToanID = @iID_DuToanID;";
                if (_connection == null)
                {
                    using (var conn = _connectionFactory.GetConnection())
                    {
                        var r = conn.Execute(
                            sql,
                            param: new
                            {
                                iID_DuToanID
                            },
                            commandType: CommandType.Text);

                        return r >= 0;
                    }
                }
                else
                {
                    var r = _connection.Execute(
                           sql,
                           param: new
                           {
                               iID_DuToanID
                           },
                           _transaction,
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

        public bool KiemTraTrungSoQuyetDinhQLTKTCvaTDT(string sSoQuyetDinh, string iID_DuToanID = "")
        {
            try
            {
                var sql = "SELECT COUNT(iID_DuToanID) FROM VDT_DA_DuToan WHERE sSoQuyetDinh = @sSoQuyetDinh AND bActive = 1";
                if (!string.IsNullOrEmpty(iID_DuToanID))
                    sql += " AND iID_DuToanID <> @iID_DuToanID ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var count = conn.QueryFirst<int>(sql,
                       param: new
                       {
                           sSoQuyetDinh,
                           iID_DuToanID = string.IsNullOrEmpty(iID_DuToanID) ? Guid.Empty : Guid.Parse(iID_DuToanID)
                       },
                       commandType: System.Data.CommandType.Text);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public VDT_DA_DuToan_ViewModel GetPheDuyetTKTCvaTDTByID(Guid iID_DuToanID)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_pheduyet_tktcvatdt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.QueryFirstOrDefault<VDT_DA_DuToan_ViewModel>(sql,
                       param: new
                       {
                           iID_DuToanID
                       },
                       commandType: CommandType.Text);
                    if (item == null)
                    {
                        return null;
                    }
                    item.ListNguonVon = GetListNguonVonTheoTKTC(iID_DuToanID);
                    item.ListChiPhi = GetListChiPhiTheoTKTC(iID_DuToanID);
                    item.ListHangMuc = GetListHangMucTheoTKTC(iID_DuToanID);

                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public VDT_DA_DuToan_ViewModel GetPheDuyetTKTCvaTDTByIDFix(Guid iID_DuToanID, int loaiChungTu)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_pheduyet_tktcvatdt_byid.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.QueryFirstOrDefault<VDT_DA_DuToan_ViewModel>(sql,
                       param: new
                       {
                           iID_DuToanID,
                           loaiChungTu
                       },
                       commandType: CommandType.Text);
                    if (item == null)
                    {
                        return null;
                    }
                    if (loaiChungTu == 1)
                    {
                        item.ListNguonVon = GetListNguonVonTheoTKTC(iID_DuToanID);
                        item.ListChiPhi = GetListChiPhiTheoTKTC(iID_DuToanID);
                        item.ListHangMuc = GetListHangMucTheoTKTC(iID_DuToanID);
                    }
                    else if (loaiChungTu == 2)
                    {
                        item.ListNguonVon = GetListNguonVonTheoQDDT(iID_DuToanID);
                        item.ListChiPhi = GetListChiPhiTheoQDDT(iID_DuToanID);
                        item.ListHangMuc = GetListHangMucTheoQDDT(iID_DuToanID);
                    }
                    else
                    {
                        item.ListNguonVon = GetListNguonVonTheoCTDT(iID_DuToanID);
                        item.ListChiPhi = GetListChiPhiTheoCTDT(iID_DuToanID);
                        item.ListHangMuc = GetListHangMucTheoCTDT(iID_DuToanID);
                    }

                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDT_DA_DuToan_ChiPhi_ByPheDuyetDuAn> GetListChiPhiTheoPheDuyetDuAn(Guid? idDuAn)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_qddautu_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var quyetDinhDauTu = conn.QueryFirstOrDefault<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE bActive =1 and iID_DuAnID = '{0}'", idDuAn));
                    if (quyetDinhDauTu == null)
                    {
                        return null;
                    }
                    var item = conn.Query<VDT_DA_DuToan_ChiPhi_ByPheDuyetDuAn>(sql,
                        param: new
                        {
                            quyetDinhDauTu.iID_QDDauTuID
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

        public VDT_DA_QDDauTu FindQDDauTuByDuAnId(Guid duanId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    VDT_DA_QDDauTu quyetDinhDauTu = conn.QueryFirstOrDefault<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE bActive =1 and iID_DuAnID = '{0}'", duanId));
                    return quyetDinhDauTu;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuAnToan_NguonVon_ByPheDuyetDuAn> GetListNguonVonTheoPheDuyetDuAn(Guid? idDuAn)
        {
            var result = new List<VDT_DA_DuAnToan_NguonVon_ByPheDuyetDuAn>();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var quyetDinhDauTu = conn.QueryFirstOrDefault<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE iID_DuAnID = '{0}'", idDuAn));
                    if (quyetDinhDauTu != null)
                    {
                        var listDMNguonVon = conn.Query<NS_NguonNganSach>("SELECT * FROM NS_NguonNganSach");
                        var listNguonVon = conn.Query<VDT_DA_QDDauTu_NguonVon>(string.Format("SELECT * FROM VDT_DA_QDDauTu_NguonVon WHERE iID_QDDauTuID = '{0}'", quyetDinhDauTu.iID_QDDauTuID));
                        if (listNguonVon != null && listNguonVon.Any())
                        {
                            foreach (var item in listNguonVon)
                            {
                                var nguonVon = new VDT_DA_DuAnToan_NguonVon_ByPheDuyetDuAn();
                                nguonVon.iID_NguonVonID = item.iID_NguonVonID;
                                nguonVon.fTienPheDuyet = item.fTienPheDuyet;

                                if (listDMNguonVon != null && listDMNguonVon.Any())
                                {
                                    var dmNguonVon = listDMNguonVon.FirstOrDefault(x => x.iID_MaNguonNganSach == item.iID_NguonVonID);
                                    if (dmNguonVon != null)
                                    {
                                        nguonVon.sTenNguonVon = dmNguonVon.sTen;
                                    }
                                }

                                result.Add(nguonVon);
                            }
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

        public List<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoPheDuyetDuAn(Guid? idDuAn)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_hangmuc_qddautu_by_duanid.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_HangMuc_ViewModel>(sql,
                        param: new
                        {
                            iIdDuAnId = idDuAn
                        },
                        commandType: CommandType.Text).ToList();

                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuToan_Nguonvon_ViewModel> GetNguonVonTKTCTDTByDuAnId(Guid iIdDuAnId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_tktctdt_get_nguonvon_by_duan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_DA_DuToan_Nguonvon_ViewModel>(sql,
                       param: new
                       {
                           iIdDuAnId
                       },
                       commandType: CommandType.Text).ToList();
                    if (item == null) return new List<VDT_DA_DuToan_Nguonvon_ViewModel>();
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoTKTC(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listnguonvon_dutoan_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_Nguonvon_ViewModel>(sql,
                        param: new
                        {
                            duToanId
                        },
                        commandType: CommandType.Text).ToList();
                    if (item == null) return new List<VDT_DA_DuToan_Nguonvon_ViewModel>();
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoQDDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listnguonvon_qddt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_Nguonvon_ViewModel>(sql,
                        param: new
                        {
                            duToanId
                        },
                        commandType: CommandType.Text).ToList();
                    if (item == null) return new List<VDT_DA_DuToan_Nguonvon_ViewModel>();
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuToan_Nguonvon_ViewModel> GetListNguonVonTheoCTDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listnguonvon_ctdt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_Nguonvon_ViewModel>(sql,
                        param: new
                        {
                            duToanId
                        },
                        commandType: CommandType.Text).ToList();
                    if (item == null) return new List<VDT_DA_DuToan_Nguonvon_ViewModel>();
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_DA_DuToan_ChiPhi_ViewModel> GetChiPhiTKTCTDTByDuAnId(Guid iIdDuAnId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_tktctdt_get_chiphi_by_duan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_DA_DuToan_ChiPhi_ViewModel>(sql,
                       param: new
                       {
                           iIdDuAnId
                       },
                       commandType: CommandType.Text).ToList();
                    if (item == null) return new List<VDT_DA_DuToan_ChiPhi_ViewModel>();
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoTKTC(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_dutoan_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_ChiPhi_ViewModel>(sql,
                        param: new
                        {
                            duToanId
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

        public IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoQDDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_qddt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_ChiPhi_ViewModel>(sql,
                        param: new
                        {
                            duToanId
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

        public IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> GetListChiPhiTheoCTDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_ctdt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_ChiPhi_ViewModel>(sql,
                        param: new { },
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

        public IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoTKTC(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listhangmuc_dutoan_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_HangMuc_ViewModel>(sql,
                        param: new
                        {
                            duToanId
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

        public IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoQDDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listhangmuc_qddt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_HangMuc_ViewModel>(sql,
                        param: new
                        {
                            duToanId
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

        public IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> GetListHangMucTheoCTDT(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listhangmuc_ctdt_by_id.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDT_DA_DuToan_HangMuc_ViewModel>(sql,
                        param: new
                        {
                            duToanId
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

        public IEnumerable<VDTQuyetDinhDauTuChiPhiModel> GetListChiPhiQDDTByIdDuToan(Guid duToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_qddt_by_iddutoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDTQuyetDinhDauTuChiPhiModel>(sql,
                        param: new
                        {
                            duToanId
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


        #endregion

        #region Phe duyet chu truong dau tu
        /// <summary>
        /// lay danh sach chu truong dau tu
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sNoiDung"></param>
        /// <param name="fTongMucDauTuFrom"></param>
        /// <param name="fTongMucDauTuTo"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        public IEnumerable<VDTChuTruongDauTuViewModel> LayDanhSachChuTruongDauTu(ref PagingInfo _paging, int iNamLamViec, string sUserName, string sSoQuyetDinh, string sNoiDung,
            float? fTongMucDauTuFrom, float? fTongMucDauTuTo, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sMaDonVi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("sNoiDung", sNoiDung);
                lstParam.Add("fTongMucDauTuFrom", fTongMucDauTuFrom);
                lstParam.Add("fTongMucDauTuTo", fTongMucDauTuTo);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sUserName", sUserName);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<VDTChuTruongDauTuViewModel>("proc_get_all_chutruongdautu_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        // lấy phân câp dự án theo chủ trương đầu tư
        public VDT_DM_PhanCapDuAn GetPhanCapDuanByChuTruongDauTu(Guid? capPheDuyetID)
        {
            var sql = $"select * from VDT_DM_PhanCapDuAn where iID_PhanCapID = '{capPheDuyetID}'";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_PhanCapDuAn>(sql, param:
                    new
                    {
                        capPheDuyetID
                    }, commandType: System.Data.CommandType.Text);
                return items.FirstOrDefault();
            }
        }

        /// <summary>
        /// kiem tra trung so quyet dinh
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <returns></returns>
        public bool KiemTraTrungSoQuyetDinhChuTruongDauTu(string sSoQuyetDinh, string iID_ChuTruongDauTuID)
        {
            if (string.IsNullOrEmpty(sSoQuyetDinh))
            {
                return false;
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_ChuTruongDauTu>("SELECT * FROM VDT_DA_ChuTruongDauTu");
                if (items == null || !items.Any())
                {
                    return true;
                }

                if (string.IsNullOrEmpty(iID_ChuTruongDauTuID))
                {
                    var check = items.FirstOrDefault(x => x.sSoQuyetDinh == sSoQuyetDinh);
                    if (check != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var check = items.FirstOrDefault(x => x.sSoQuyetDinh == sSoQuyetDinh && x.iID_ChuTruongDauTuID != Guid.Parse(iID_ChuTruongDauTuID));
                    if (check != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// lay thong tin chi tiet chu truong dau tu
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        public VDTChuTruongDauTuViewModel LayThongTinChiTietChuTruongDauTu(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_laychitietchutruongpheduyet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDTChuTruongDauTuViewModel>(sql,
                   param: new
                   {
                       iID_ChuTruongDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                if (item != null)
                {
                    var listNguonNganSach = conn.Query<NS_NguonNganSach>("SELECT * FROM NS_NguonNganSach");
                    var listNguonVon = conn.Query<VDT_DA_ChuTruongDauTu_NguonVon>(string.Format("SELECT * FROM VDT_DA_ChuTruongDauTu_NguonVon WHERE iID_ChuTruongDauTuID = '{0}'", item.iID_ChuTruongDauTuID));
                    if (listNguonVon != null && listNguonVon.Any())
                    {
                        var listNV = new List<VDTChuTruongDauTuNguonVonModel>();
                        foreach (var nv in listNguonVon)
                        {
                            var nguonVon = new VDTChuTruongDauTuNguonVonModel();
                            nguonVon.iID_ChuTruongDauTuID = nv.iID_ChuTruongDauTuID;
                            nguonVon.iID_ChuTruongDauTu_NguonVonID = nv.iID_ChuTruongDauTu_NguonVonID;
                            nguonVon.fTienToTrinh = nv.fTienToTrinh;
                            nguonVon.fTienThamDinh = nv.fTienThamDinh;
                            nguonVon.fGiaTriTruocDieuChinh = nv.fTienPheDuyet - (nv.fGiaTriDieuChinh ?? 0);
                            nguonVon.fTienPheDuyet = nv.fTienPheDuyet;
                            nguonVon.iID_NguonVonID = nv.iID_NguonVonID;
                            //nguonVon.id = 
                            if (nv.iID_NguonVonID != null && listNguonNganSach != null && listNguonNganSach.Any())
                            {
                                var nguonNganSach = listNguonNganSach.FirstOrDefault(x => x.iID_MaNguonNganSach == nv.iID_NguonVonID);
                                if (nguonNganSach != null)
                                {
                                    nguonVon.sTenNguonVon = nguonNganSach.sTen;
                                }
                            }
                            listNV.Add(nguonVon);
                        }

                        if (listNV.Any())
                        {
                            item.listChuTruongDauTuNguonVon = listNV.AsEnumerable();
                        }
                    }
                    var listHangMuc = conn.Query<VDT_DA_DuAn_HangMuc>(string.Format("SELECT * FROM VDT_DA_DuAn_HangMuc WHERE iID_DuAnid = '{0}'", item.iID_DuAnID));
                    if (listHangMuc != null && listHangMuc.Any())
                    {
                        var listHM = new List<VDTDADuAnHangMucModel>();
                        foreach (var hm in listHangMuc)
                        {
                            var hangMuc = new VDTDADuAnHangMucModel();
                            hangMuc.iID_DuAn_HangMucID = hm.iID_DuAn_HangMucID;
                            hangMuc.iID_ParentID = hm.iID_ParentID;
                            hangMuc.sMaHangMuc = hm.sMaHangMuc;
                            hangMuc.sTenHangMuc = hm.sTenHangMuc;

                            if (hangMuc.iID_ParentID.HasValue)
                            {
                                var parent = listHangMuc.FirstOrDefault(x => x.iID_DuAn_HangMucID == hangMuc.iID_ParentID.Value);
                                if (parent != null)
                                {
                                    hangMuc.sHangMucCha = parent.sMaHangMuc;
                                    hangMuc.sTenHangMucCha = parent.sTenHangMuc;
                                }
                            }

                            if (!string.IsNullOrEmpty(hangMuc.sMaHangMuc))
                            {
                                listHM.Add(hangMuc);
                            }
                        }
                        item.ListHangMuc = listHM.AsEnumerable();
                    }
                }
                return item;
            }
        }

        /// <summary>
        /// lay danh sach chu truong dau tu chi phi theo chu truong dau tu id
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        public IEnumerable<VDTChuTruongDauTuChiPhiModel> LayDanhSachChuTruongDauTuChiPhi(Guid iID_ChuTruongDauTuID)
        {
            var sql = "SELECT cp.*, dm_cp.sTenChiPhi AS sTenChiPhi FROM VDT_DA_ChuTruongDauTu_ChiPhi cp " +
                    "INNER JOIN VDT_DM_ChiPhi dm_cp ON cp.iID_ChiPhiID = dm_cp.iID_ChiPhi " +
                    "WHERE cp.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTChuTruongDauTuChiPhiModel>(sql,
                   param: new
                   {
                       iID_ChuTruongDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach chu truong dau tu nguon von theo chu truong dau tu id
        /// </summary>
        /// <param name="iIDChuTruongDauTuId"></param>
        /// <returns></returns>
        public IEnumerable<VDTChuTruongDauTuNguonVonModel> LayDanhSachChuTruongDauTuNguonVon(Guid iID_ChuTruongDauTuID)
        {
            var sql = "SELECT nv.*, dm_nv.sTen AS sTenNguonVon FROM VDT_DA_ChuTruongDauTu_NguonVon nv " +
                    "INNER JOIN NS_NguonNganSach dm_nv ON nv.iID_NguonVonID = dm_nv.iID_MaNguonNganSach " +
                    "WHERE nv.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTChuTruongDauTuNguonVonModel>(sql,
                   param: new
                   {
                       iID_ChuTruongDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// xoa danh sach chi phi va nguon von cu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        public bool XoaDanhSachChiPhiNguonVonCu(Guid iID_ChuTruongDauTuID, Guid? parentId)
        {

            using (var conn = _connectionFactory.GetConnection())
            {
                var chuTruongDT = conn.Get<VDT_DA_ChuTruongDauTu>(iID_ChuTruongDauTuID);
                if (chuTruongDT != null)
                {
                    var sql = FileHelpers.GetSqlQuery("vdt_delete_chutruongdautu.sql");
                    var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID,
                        parentId
                    },
                    commandType: CommandType.Text);
                    return r >= 0;
                }
            }
            return false;
        }
        public VDT_DA_QDDauTu GetListQDDauTuByIdChuTruongDauTuID(Guid iID_ChuTruongDauTuID)
        {
            var sql = "select * from VDT_DA_QDDauTu where iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_QDDauTu>(sql,
                   param: new
                   {
                       iID_ChuTruongDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// lay listnguonVon,Listhangmuc chu truong dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<VDTChuTruongDauTuNguonVonModel> GetListNguonVonTheoCTDauTuId(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_ListNguonVon_ctDauTu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTChuTruongDauTuNguonVonModel>(sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDTDADuAnHangMucModel> GetListHangMucTheoCTDauTuId(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_ListHangMuc_ctDauTu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTDADuAnHangMucModel>(sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        /// <summary>
        /// xoa chu truong dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        public bool XoaChuTruongDauTu(Guid iID_ChuTruongDauTuID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var model = conn.Get<VDT_DA_ChuTruongDauTu>(iID_ChuTruongDauTuID);
                var model2 = GetListQDDauTuByIdChuTruongDauTuID(iID_ChuTruongDauTuID);
                if (model2 == null && model != null)
                {
                    //nếu là bản ghi điều chỉnh thì update bản ghi gốc bactive = 1
                    if (model.iID_ParentID != null && model.bIsGoc == false)
                    {
                        VDT_DA_ChuTruongDauTu parent = conn.Get<VDT_DA_ChuTruongDauTu>(model.iID_ParentID);
                        if (parent != null)
                        {
                            parent.bActive = true;
                            conn.Update<VDT_DA_ChuTruongDauTu>(parent);
                        }
                    }
                    XoaDanhSachChiPhiNguonVonCu(model.iID_ChuTruongDauTuID, model.iID_DuAnID);
                    conn.Delete<VDT_DA_ChuTruongDauTu>(model);
                    return true;
                }
                return false;

            }

        }

        public List<VDT_DA_DuAn_HangMuc> GetDataDMDuAnHangMuc(Guid iID_DuAnID)
        {
            var result = new List<VDT_DA_DuAn_HangMuc>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMuc>(string.Format("SELECT * FROM VDT_DA_DuAn_HangMuc WHERE iID_DuAnID = '{0}'", iID_DuAnID));
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }

            return result;
        }

        public bool CheckDuAnQuyetDinhDauTu(VDT_DA_ChuTruongDauTuCreateModel model, string sUserName)
        {
            if (model == null)
            {
                return false;
            }

            if (model.iID_DuAnID == null || model.iID_DuAnID == Guid.Empty)
            {
                return false;
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var quyetDinhDauTu = conn.QueryFirstOrDefault<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE iID_DuAnID = '{0}'", model.iID_DuAnID));
                if (quyetDinhDauTu == null)
                {
                    var duAn = conn.QueryFirstOrDefault<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn WHERE iID_DuAnID = '{0}'", model.iID_DuAnID));
                    if (duAn != null)
                    {
                        duAn.iID_DonViQuanLyID = model.iID_DonViQuanLyID;
                        duAn.iID_ChuDauTuID = model.iID_ChuDauTuID;
                        duAn.sMucTieu = model.sMucTieu;
                        duAn.sQuyMo = model.sQuyMo;
                        duAn.sDiaDiem = model.sDiaDiem;
                        duAn.iID_NhomDuAnID = model.iID_NhomDuAnID;
                        duAn.iID_NhomQuanLyID = model.iID_NhomQuanLyID;
                        //duAn.iID_LoaiCongTrinhID = model.iID_LoaiCongTrinhID;
                        duAn.iID_CapPheDuyetID = model.iID_CapPheDuyetID;
                        duAn.sKhoiCong = model.sKhoiCong;
                        duAn.sKetThuc = model.sHoanThanh;
                        duAn.sUserDelete = sUserName;
                        duAn.dDateUpdate = DateTime.Now;

                        conn.Update<VDT_DA_DuAn>(duAn);
                    }
                }
            }

            return true;
        }

        public IEnumerable<ChuTruongHangMucModel> LayDanhSachHangMucTheoChuTruongId(Guid chuTruongId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_chutruonghangmuc_update.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<ChuTruongHangMucModel>(sql,
                   param: new
                   {
                       chuTruongId
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Phe duyet du an
        /// <summary>
        /// lay danh sach phe duyet du an
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sMaDonVi"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuViewModel> LayDanhSachPheDuyetDuAn(ref PagingInfo _paging, int iNamLamViec, string sUserName, string sSoQuyetDinh,
            string sTenDuAn, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sMaDonVi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("sTenDuAn", sTenDuAn);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sUserName", sUserName);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<VDTQuyetDinhDauTuViewModel>("proc_get_all_pheduyetduan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        /// kiem tra trung so quyet dinh
        /// </summary>
        /// <param name="sSoQuyetDinh"></param>
        /// <returns></returns>
        public bool KiemTraTrungSoQuyetDinhQuyetDinhDauTu(string sSoQuyetDinh, string iID_QDDauTuID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                if (iID_QDDauTuID == Guid.Empty.ToString())
                {
                    var model = conn.Query<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE sSoQuyetDinh = '{0}'", sSoQuyetDinh));
                    if (model != null && model.Any())
                    {
                        return false;
                    }
                }
                else
                {
                    var model = conn.Query<VDT_DA_QDDauTu>(string.Format("SELECT * FROM VDT_DA_QDDauTu WHERE sSoQuyetDinh = '{0}' AND iID_QDDauTuID != '{1}'", sSoQuyetDinh, iID_QDDauTuID));
                    if (model != null && model.Any())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// lay thong tin chi tiet quyet dinh dau tu
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public VDTQuyetDinhDauTuViewModel LayThongTinChiTietQuyetDinhDauTu(Guid iID_QDDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_qddautu_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTQuyetDinhDauTuViewModel>(sql,
                    param: new
                    {
                        iID_QDDauTuID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }

        }

        /// <summary>
        /// lay danh sach quyet dinh dau tu chi phi theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuChiPhiModel> LayDanhSachQuyetDinhDauTuChiPhi(Guid iID_QDDauTuID)
        {
            var sql = "SELECT cp.*, dm_cp.sTenChiPhi AS sTenChiPhi FROM VDT_DA_QDDauTu_ChiPhi cp " +
                    "INNER JOIN VDT_DM_ChiPhi dm_cp ON cp.iID_ChiPhiID = dm_cp.iID_ChiPhi " +
                    "WHERE cp.iID_QDDauTuID = @iID_QDDauTuID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuChiPhiModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach quyet dinh dau tu chi phi dieu chinh
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuChiPhiModel> LayDanhSachQuyetDinhDauTuChiPhiDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddtchiphi_dieuchinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuChiPhiModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID,
                       dNgayPheDuyet
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach quyet dinh dau tu nguonvon dieu chinh
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuNguonVonModel> LayDanhSachQuyetDinhDauTuNguonVonDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddtnguonvon_dieuchinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuNguonVonModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID,
                       dNgayPheDuyet
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach quyet dinh dau tu nguon von theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuNguonVonModel> LayDanhSachQuyetDinhDauTuNguonVon(Guid iID_QDDauTuID)
        {
            var sql = "SELECT nv.*, dm_nv.sTen AS sTenNguonVon FROM VDT_DA_QDDauTu_NguonVon nv " +
                    "INNER JOIN NS_NguonNganSach dm_nv ON nv.iID_NguonVonID = dm_nv.iID_MaNguonNganSach " +
                    "WHERE nv.iID_QDDauTuID = @iID_QDDauTuID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuNguonVonModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }



        /// <summary>
        /// lay danh sach hang muc theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuHangMucModel> LayDanhSachQuyetDinhDauTuHangMucDieuChinh(Guid iID_QDDauTuID, DateTime dNgayPheDuyet)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddthangmuc_dieuchinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuHangMucModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID,
                       dNgayPheDuyet
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay danh sach hang muc theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public IEnumerable<VDT_DA_DuAn_HangMuc> LayDanhSachHangMucTheoDuAnId(Guid iID_DuAnID)
        {
            var sql = "SELECT * FROM VDT_DA_DuAn_HangMuc " +
                   "WHERE iID_DuAnID = @iID_DuAnID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMuc>(sql,
                   param: new
                   {
                       iID_DuAnID
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        /// <summary>
        /// lay danh sach nguon von theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public IEnumerable<VDTDADSNguonVonTheoIDDuAnModel> LayDanhSachNguonVonTheoDuAnId(Guid iID_DuAnID)
        {
            var sql = "SELECT da.iID_DuAnID,chitiet.iID_NguonVonID,ns.sTen,sum(chitiet.fHanMucDauTu) as 'TongHanMucDauTu' " +
                "FROM VDT_KHV_KeHoach5Nam_ChiTiet chitiet " +
                "INNER JOIN VDT_DA_DuAn da on da.iID_DuAnID = chitiet.iID_DuAnID " +
                "INNER JOIN NS_NguonNganSach ns on ns.iID_MaNguonNganSach = chitiet.iID_NguonVonID " +
                "WHERE da.iID_DuAnID = @iID_DuAnID " +
                "GROUP BY da.iID_DuAnID,chitiet.iID_NguonVonID,ns.sTen";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTDADSNguonVonTheoIDDuAnModel>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        /// <summary>
        /// lay ma du an theo du an Id
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public string LayMaDuAnTheoDuAnId(Guid? iID_DuAnID)
        {
            var sql = "SELECT sMaDuAn FROM VDT_DA_DuAn WHERE iID_DuAnID = @iID_DuAnID";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<string>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.FirstOrDefault().ToString();
            }

        }

        /// <summary>
        /// lay danh sach hang muc theo quyet dinh dau tu id
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public IEnumerable<VDTQuyetDinhDauTuHangMucModel> LayDanhSachQuyetDinhDauTuHangMuc(Guid iID_QDDauTuID)
        {
            var sql = "SELECT hm.iID_HangMucID, da_hm.sTenHangMuc, hm.fTienPheDuyet FROM VDT_DA_QDDauTu_HangMuc hm " +
                    "INNER JOIN VDT_DA_DuAn_HangMuc da_hm ON hm.iID_HangMucID = da_hm.iID_DuAn_HangMucID " +
                    "WHERE iID_QDDauTuID = @iID_QDDauTuID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQuyetDinhDauTuHangMucModel>(sql,
                   param: new
                   {
                       iID_QDDauTuID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// xoa danh sach chi phi va nguon von cu cua quyet dinh dau tu
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public bool XoaDanhSachQuyetDinhDauTuChiPhiNguonVonCu(Guid iID_QDDauTuID, SqlConnection _connection = null, SqlTransaction _transaction = null)
        {
            var sql = "DELETE VDT_DA_QDDauTu_ChiPhi WHERE iID_QDDauTuID = @iID_QDDauTuID; " +
                       "DELETE VDT_DA_QDDauTu_NguonVon WHERE iID_QDDauTuID = @iID_QDDauTuID; " +
                       "DELETE VDT_DA_QDDauTu_HangMuc WHERE iID_QDDauTuID = @iID_QDDauTuID;" +
                       "DELETE VDT_DA_QDDauTu_DM_HangMuc WHERE QDDauTuId = @iID_QDDauTuID;" +
                       "DELETE VDT_DM_DuAn_ChiPhi WHERE iID_QDDauTuID = @iID_QDDauTuID;";
            if (_connection == null)
            {
                using (_connection = _connectionFactory.GetConnection())
                {
                    var r = _connection.Execute(
                    sql,
                    param: new
                    {
                        iID_QDDauTuID
                    },
                    commandType: CommandType.Text);
                    return r > 0;
                }
            }
            else
            {
                var r = _connection.Execute(
                    sql,
                    param: new
                    {
                        iID_QDDauTuID
                    },
                    _transaction,
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }

        /// <summary>
        /// xoa quyet dinh dau tu
        /// </summary>
        /// <param name="iID_ChuTruongDauTuID"></param>
        /// <returns></returns>
        public bool XoaQuyetDinhDauTu(Guid iID_QDDauTuID)
        {
            //var sql = "DELETE VDT_DA_QDDauTu WHERE iID_QDDauTuID = @iID_QDDauTuID;";
            //using (var conn = _connectionFactory.GetConnection())
            //{
            //    var r = conn.Execute(
            //        sql,
            //        param: new
            //        {
            //            iID_QDDauTuID
            //        },
            //        commandType: CommandType.Text);
            //    

            //    if (r > 0)
            //        XoaDanhSachQuyetDinhDauTuChiPhiNguonVonCu(iID_QDDauTuID);
            //    return r > 0;
            //}

            var sql = FileHelpers.GetSqlQuery("vdt_delete_pheduyetduan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var qdDauTu = conn.Get<VDT_DA_QDDauTu>(iID_QDDauTuID);
                //nếu là xóa bản ghi điều chỉnh thì update bản ghi cha bactive = 1

                if (qdDauTu == null)
                {
                    return false;
                }
                if (qdDauTu.iID_ParentID != null)
                {
                    var qdDauTuParent = conn.Get<VDT_DA_QDDauTu>(qdDauTu.iID_ParentID);
                    if (qdDauTuParent != null)
                    {
                        qdDauTuParent.bActive = true;
                        conn.Update<VDT_DA_QDDauTu>(qdDauTuParent);
                    }
                }
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_QDDauTuID,
                        qdDauTu.iID_ParentID
                    },
                    commandType: CommandType.Text);

                return r >= 0;

            }
        }

        public IEnumerable<VDT_DA_DuToan> GetListDuToanByQDDT(Guid iiDQDDT)
        {
            var sql = "SELECT * FROM VDT_DA_DuToan WHERE iID_QDDauTuID = @iiDQDDT";
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDT_DA_DuToan>(sql,
                    param: new
                    {
                        iiDQDDT
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDT_DA_DuToan> GetListDuToanByIdDuAn(Guid idDuAn)
        {
            var sql = "SELECT * FROM VDT_DA_DuToan WHERE iID_DuAnID = @idDuAn";
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDT_DA_DuToan>(sql,
                    param: new
                    {
                        idDuAn
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_DA_ChuTruongDauTu FindChuTruongDauTuByDuAnId(Guid idDuAn)
        {
            var sql = "SELECT * FROM VDT_DA_ChuTruongDauTu WHERE iID_DuAnID = @idDuAn AND bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.QueryFirstOrDefault<VDT_DA_ChuTruongDauTu>(sql,
                    param: new
                    {
                        idDuAn
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public double GetTMPDTheoChuTruongDauTu(Guid? iIDDuAn)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var chuTruongDauTu = conn.QueryFirstOrDefault<VDT_DA_ChuTruongDauTu>(string.Format("SELECT * FROM VDT_DA_ChuTruongDauTu WHERE iID_DuAnID = '{0}' AND bActive = 1", iIDDuAn));
                if (chuTruongDauTu != null)
                {
                    if (chuTruongDauTu.fTMDTDuKienPheDuyet.HasValue)
                    {
                        return chuTruongDauTu.fTMDTDuKienPheDuyet.Value;
                    }
                }
            }

            return 0;
        }

        public IEnumerable<VDTQuyetDinhDauTuNguonVonByChuTruongDauTu> GetListNguonVonTheoChuTruongDauTu(Guid idDuAn)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_listnguonvon_chutruong_by_duanid.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTQuyetDinhDauTuNguonVonByChuTruongDauTu>(sql,
                    param: new
                    {
                        idDuAn
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTQuyetDinhDauTuNguonVonModel> GetListNguonVonTheoQDDauTuDauTu(Guid qdDauTuId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_listnguonvon_qddautu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTQuyetDinhDauTuNguonVonModel>(sql,
                    param: new
                    {
                        qdDauTuId
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTQuyetDinhDauTuChiPhiCreateModel> GetListChiPhiTheoQDDauTuDauTu(Guid qdDauTuId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_listchiphi_qddautu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTQuyetDinhDauTuChiPhiCreateModel>(sql,
                    param: new
                    {
                        qdDauTuId
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListHangMucTheoChuTruongDauTu(Guid? idDuAn)
        {
            var chutruongDT = GetVDTChuTruongDauTu(idDuAn);
            if (chutruongDT != null)
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_listhangmuc_chutruong_by_duanid.sql");
                using (var conn = _connectionFactory.GetConnection())
                {

                    var item = conn.Query<VDTQuyetDinhDauTuDMHangMucModel>(sql,
                        param: new
                        {
                            chutruongDT.iID_ChuTruongDauTuID
                        },
                        commandType: CommandType.Text);

                    return item;
                }
            }
            return null;

        }

        public IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListHangMucTheoQDDauTu(Guid qdDauTuId)
        {
            //var chutruongDT = GetVDTChuTruongDauTu(idDuAn);

            var sql = FileHelpers.GetSqlQuery("vdt_get_listhangmuc_qddautu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTQuyetDinhDauTuDMHangMucModel>(sql,
                    param: new
                    {
                        qdDauTuId
                    },
                    commandType: CommandType.Text);

                return item;
            }

        }

        public IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> GetListQuyetDinhDauTuHangMuc(Guid? iID_QDDauTuID, Guid? iID_DuAn_ChiPhi)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_quyet_dinh_dautu_hangmuc.sql");
            using (var conn = _connectionFactory.GetConnection())
            {

                var item = conn.Query<VDTQuyetDinhDauTuDMHangMucModel>(sql,
                    param: new
                    {
                        iID_QDDauTuID,
                        iID_DuAn_ChiPhi
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        #endregion

        #region Quản lý dự án - NinhNV
        public IEnumerable<VDTDuAnInfoModel> GetAllDuAnTheoTrangThai(ref PagingInfo _paging, string sTenDuAn, string sKhoiCong, string sKetThuc, Guid? iID_DonViQuanLyID, Guid? iID_CapPheDuyetID, Guid? iID_LoaiCongTrinhID, int iTrangThai)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sTenDuAn", sTenDuAn);
                lstParam.Add("sKhoiCong", sKhoiCong);
                lstParam.Add("sKetThuc", sKetThuc);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("iID_CapPheDuyetID", iID_CapPheDuyetID);
                lstParam.Add("iID_LoaiCongTrinhID", iID_LoaiCongTrinhID);
                lstParam.Add("iTrangThai", iTrangThai);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTDuAnInfoModel>("proc_get_all_duantheotrangthai_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_DA_ChuTruongDauTu GetVDTChuTruongDauTu(Guid? iID_DuAnID)
        {
            var sql = "SELECT * FROM VDT_DA_ChuTruongDauTu WHERE iID_DuAnID = @iID_DuAnID and bActive = 1";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_ChuTruongDauTu>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }
        }

        public VDTDuAnThongTinPheDuyetModel GetVDTQDDauTu(Guid? iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_qddutu_by_idduan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnThongTinPheDuyetModel>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }
        }

        public VDTDuAnThongTinDuToanModel GetVDTDuToan(Guid? iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_dutoan_by_idduan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnThongTinDuToanModel>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }
        }

        public VDT_QT_QuyetToan GetVDTQuyetToan(Guid? iID_DuAnID)
        {
            var sql = "SELECT * FROM VDT_QT_QuyetToan WHERE iID_DuAnID = @iID_DuAnID";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_QT_QuyetToan>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }
        }

        public VDTDuAnInfoModel GetDuAnById(Guid? iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_duan_by_idduan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnInfoModel>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().FirstOrDefault();
            }
        }
        public IEnumerable<VDTDuAnListCTDTChiPhiModel> GetListCTDTChiPhi(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_ctdtchiphi_by_idctdt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListCTDTChiPhiModel>(sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDT_DA_ChuTruongDauTu> GetListCTDTByIdCTDT(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_ctdt_by_idctdt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_ChuTruongDauTu>(sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTDuAnListCTDTNguonVonModel> GetListCTDTNguonVon(Guid iID_ChuTruongDauTuID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_ctdtnguonvon_by_idctdt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListCTDTNguonVonModel>(sql,
                    param: new
                    {
                        iID_ChuTruongDauTuID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public bool deleteVDTDuAn(Guid iID_DuAnID)
        {
            var sql = "DELETE VDT_DA_DuAn WHERE iID_DuAnID = @iID_DuAnID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }
        public IEnumerable<VDTDuAnListQDDTChiPhiModel> GetListQDDTChiPhi(Guid iID_QDDauTuID, Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddtchiphi_by_idqddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListQDDTChiPhiModel>(sql,
                    param: new
                    {
                        iID_QDDauTuID,
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDTDuAnListQDDTNguonVonModel> GetListQDDTNguonVon(Guid iID_QDDauTuID, Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddtnguonvon_by_idqddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListQDDTNguonVonModel>(sql,
                    param: new
                    {
                        iID_QDDauTuID,
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDTDuAnListQDDTHangMucModel> GetListQDDTHangMuc(Guid iID_QDDauTuID, Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_qddthangmuc_by_idqddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListQDDTHangMucModel>(sql,
                    param: new
                    {
                        iID_QDDauTuID,
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDTDuAnListDuToanChiPhiModel> GetListDuToanChiPhi(Guid iID_DuToanID, Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_dutoanchiphi_by_iddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListDuToanChiPhiModel>(sql,
                    param: new
                    {
                        iID_DuToanID,
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }
        public IEnumerable<VDTDuAnListDuToanNguonVonModel> GetListDuToanNguonVon(Guid iID_DuToanID, Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_dutoannguonvon_by_iddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListDuToanNguonVonModel>(sql,
                    param: new
                    {
                        iID_DuToanID,
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        /// <summary>
        /// lấy danh sách nguồn vốn được tạo ở màn thêm mới thông tin dự án
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public IEnumerable<VDTDuAnListNguonVonTTDuAnModel> GetListDuAnNguonVonTTDuAn(Guid? iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_danhsachnguonvon_ttduan_idduan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTDuAnListNguonVonTTDuAnModel>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        /// <summary>
        /// Lấy danh sách hạng mục được tạo ở màn thêm mới dự án
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public IEnumerable<VDT_DA_DuAn_HangMucModel> GetListDuAnHangMucTTDuAn(Guid? iID_DuAnID)
        {

            var sql = "select * from VDT_DA_DuAn_HangMuc where iID_DuAnID = @iID_DuAnID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMucModel>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iID_DuAnID,
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }

        }

        public IEnumerable<VDT_DA_DuToan> GetListDuToanDieuChinh(Guid iID_DuAnID)
        {
            var sql = "select * from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID and bLaTongDuToan = 0";
            //var sql = FileHelpers.GetSqlQuery("vdt_get_all_dutoannguonvon_by_iddt.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_DuToan>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public DataTable GetDuAnInQuyetToanNienDo(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            String SQL = FileHelpers.GetSqlQuery("vdt_get_all_duan_in_quyettoanniendo.sql");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@donViQuanLyId", iID_DonViQuanLyID);
            cmd.Parameters.AddWithValue("@nguonVonID", iID_NguonVonID);
            cmd.Parameters.AddWithValue("@loaiNguonVonID", iID_LoaiNguonVonID);
            cmd.Parameters.AddWithValue("@ngayLap", dNgayQuyetDinh);
            cmd.Parameters.AddWithValue("@year", iNamKeHoach);
            cmd.Parameters.AddWithValue("@nganhId", iID_NganhID);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public VDT_QT_BCQuyetToanNienDo GetDataBCQuyetToanNienDo(int iLoaiThanhToan, Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach)
        {
            var sql = "SELECT * FROM VDT_QT_BCQuyetToanNienDo WHERE iLoaiThanhToan = @iLoaiThanhToan and iID_DonViQuanLyID = @iID_DonViQuanLyID and iID_NguonVonID = @iID_NguonVonID and iNamKeHoach = @iNamKeHoach";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_QT_BCQuyetToanNienDo>(
                    sql,
                    param: new
                    {
                        iLoaiThanhToan,
                        iID_DonViQuanLyID,
                        iID_NguonVonID,
                        iNamKeHoach
                    },
                    commandType: CommandType.Text);

                return item.FirstOrDefault();
            }
        }

        public List<VDTQtBcquyetToanNienDoExportModel> GetDataBCQuyetToanNienDoDetail(Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdMaDonVi", iID_DonViQuanLyID);
                lstParam.Add("iIdNguonVon", iID_NguonVonID);
                lstParam.Add("iNamKeHoach", iNamKeHoach);

                var items = conn.Query<VDTQtBcquyetToanNienDoExportModel>("sp_vdt_get_baocaodquyettoanniendo_vonung", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }

        public List<VDTQtBcquyetToanNienDoExportModel> GetQuyetToanNienDoVonUngByParentId(Guid iID_BCQuyetToanNienDoID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdQuyetToanId", iID_BCQuyetToanNienDoID);

                var items = conn.Query<VDTQtBcquyetToanNienDoExportModel>("sp_vdt_get_quyettoanniendovonung_by_parentid", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }



        public bool CheckExistMaDuAn(Guid iID_DuAnID, string sMaDuAn)
        {
            var sql = "SELECT * FROM VDT_DA_DuAn WHERE iID_DuAnID != @iID_DuAnID and sMaDuAn = @sMaDuAn";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_DuAn>(
                    sql,
                    param: new
                    {
                        iID_DuAnID,
                        sMaDuAn
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }

        public bool CheckExistDuAnInQDDT(Guid iID_DuAnID)
        {
            var sql = "SELECT * FROM VDT_DA_QDDauTu WHERE iID_DuAnID = @iID_DuAnID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_QDDauTu>(
                    sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }

        public bool CheckExistDuAnInCTDT(Guid iID_DuAnID)
        {
            var sql = "SELECT * FROM VDT_DA_ChuTruongDauTu WHERE iID_DuAnID = @iID_DuAnID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_ChuTruongDauTu>(
                    sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }

        public Guid? getQuyetToanID(Guid? iID_DuAnID)
        {
            var sql = "SELECT iID_QuyetToanID FROM VDT_QT_QuyetToan WHERE iID_DuAnID = @iID_DuAnID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<Guid>(
                    sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        /// <summary>
        /// get iMaDuAnIndex
        /// </summary>
        /// <returns></returns>
        public int GetiMaDuAnIndex()
        {
            var sql = "SELECT TOP 1 iMaDuAnIndex FROM VDT_DA_DuAn ORDER BY iMaDuAnIndex DESC";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<int?>(
                    sql,
                    commandType: CommandType.Text);

                return item == null ? 0 : item.Value;
            }
        }

        /// <summary>
        /// get indexMaHangMuc
        /// </summary>
        /// <returns></returns>
        public int GetIndexMaHangMuc()
        {
            var sql = "SELECT TOP 1 indexMaHangMuc FROM VDT_DA_DuAn_HangMuc ORDER BY indexMaHangMuc DESC";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<int?>(
                    sql,
                    commandType: CommandType.Text);

                return item == null ? 0 : item.Value;
            }
        }
        #endregion

        ///không dùng
        #region Kế hoạch vốn - LongDT
        public IEnumerable<KeHoachVonNamViewModel> GetKeHoachVonNamView(int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, DateTime? dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_detailkehoachvonnambyloaingansach.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<KeHoachVonNamViewModel>(sql,
                        param: new
                        {
                            iNamKeHoach,
                            iDonViQuanLy,
                            iNguonVon,
                            iLoaiNganSach,
                            dNgayLap
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

        public IEnumerable<VDTKHVPhanBoVonViewModel> GetAllPhanBoVonPaging(ref PagingInfo _paging, string sSoKeHoach, int? iNamKeHoach, DateTime? dNgayLapFrom, DateTime? dNgayLapTo, Guid? sDonViQuanLy)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoKeHoach", sSoKeHoach);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("dNgayLapFrom", dNgayLapFrom);
                    lstParam.Add("dNgayLapTo", dNgayLapTo);
                    lstParam.Add("sDonViQuanLy", sDonViQuanLy, dbType: DbType.Guid);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<VDTKHVPhanBoVonViewModel>("proc_get_kehoachvonnam_paging", lstParam, commandType: CommandType.StoredProcedure);
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

        public KeHoachVonNamViewModel GetThongTinChiTietByMaDuAnId(Guid iIdDuAn, int iNamKeHoach, Guid iDonViQuanLy, int iNguonVon, Guid iLoaiNganSach, Guid iNganh, DateTime? dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_findthongtinchitietduan.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<KeHoachVonNamViewModel>(sql,
                        param: new
                        {
                            iIdDuAn,
                            iNamKeHoach,
                            iDonViQuanLy,
                            iNguonVon,
                            iLoaiNguonVon = iLoaiNganSach,
                            dNgayLap,
                            iNganh
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

        public VDT_KHV_PhanBoVon GetPhanBoVonDuplicate(Guid iDonViQuanLy, int iNguonVonId, int iNamKeHoach)
        {
            var sql = @"SELECT * FROM VDT_KHV_PhanBoVon WHERE iID_DonViQuanLyID = @iDonViQuanLy AND iID_NguonVonID = @iNguonVonId AND iNamKeHoach = @iNamKeHoach";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_KHV_PhanBoVon>(sql,
                    param: new
                    {
                        iDonViQuanLy,
                        iNguonVonId,
                        iNamKeHoach
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDTKHVPhanBoVonChiTietViewModel> GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_phanbovonchitiet_gridview.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDTKHVPhanBoVonChiTietViewModel>(sql,
                        param: new
                        {
                            iID_PhanBoVon = iId
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

        public bool InsertVdtKhvPhanBoVon(ref VDT_KHV_PhanBoVon data)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Insert(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool UpdateVdtKhvPhanBoVon(VDT_KHV_PhanBoVon data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Update(data);
            }
            if (data.dDateDelete.HasValue)
            {
                UpdateStatusDuAn(data.iID_PhanBoVonID);
            }
            return true;
        }

        public bool InsertPhanBoVonChiTiet(ref List<VDT_KHV_PhanBoVon_ChiTiet> lstData, bool bChinhSua)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Insert<VDT_KHV_PhanBoVon_ChiTiet>(lstData);
                    if (!bChinhSua)
                    {
                        var PhanBoVon = conn.Get<VDT_KHV_PhanBoVon>(lstData.FirstOrDefault().iID_PhanBoVonID);
                        PhanBoVon.fGiaTrPhanBo = lstData.Sum(n => n.fGiaTrPhanBo ?? 0);
                        conn.Update(PhanBoVon);
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

        public VDT_KHV_PhanBoVon GetPhanBoVonByID(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Get<VDT_KHV_PhanBoVon>(iId);
            }
        }

        private bool UpdateStatusDuAn(Guid iId)
        {
            string sql = @"UPDATE da
                            SET
                            sTrangThaiDuAn = N'THUC_HIEN'
                            FROM VDT_DA_DuAn as da
                            INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on da.iID_DuAnID = dt.iID_DuAnID
                            WHERE dt.iID_PhanBoVonID = @iId AND sTrangThaiDuAn = 'KhoiTao'";
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Execute(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);

                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool DeleteKeHoachNamChiTietByKeHoachNamId(Guid iId)
        {
            try
            {
                string sql = @"DELETE FROM VDT_KHV_PhanBoVon_ChiTiet WHERE iID_PhanBoVonID = @iId";
                IEnumerable<VDT_KHV_PhanBoVon_ChiTiet> lstData;
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Execute(sql,
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;

        }

        //NinhNV Phân bổ vốn start
        public bool CheckDupKeHoachNam(Guid iID_PhanBoVonID, Guid iID_DonViQuanLyID, int iID_NguonVonID, int iNamKeHoach)
        {
            var sql = @"SELECT * FROM VDT_KHV_PhanBoVon WHERE iID_PhanBoVonID != @iID_PhanBoVonID AND iID_DonViQuanLyID = @iID_DonViQuanLyID 
                                                            AND iID_NguonVonID = @iID_NguonVonID AND iNamKeHoach = @iNamKeHoach";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_KHV_PhanBoVon>(
                    sql,
                    param: new
                    {
                        iID_PhanBoVonID,
                        iID_DonViQuanLyID,
                        iID_NguonVonID,
                        iNamKeHoach
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }

        public bool CheckDupSoKeHoach(Guid iID_PhanBoVonID, Guid iID_DonViQuanLyID, string sSoQuyetDinh)
        {
            var sql = @"SELECT * FROM VDT_KHV_PhanBoVon WHERE iID_PhanBoVonID != @iID_PhanBoVonID AND iID_DonViQuanLyID = @iID_DonViQuanLyID 
                                                            AND sSoQuyetDinh = @sSoQuyetDinh";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_KHV_PhanBoVon>(
                    sql,
                    param: new
                    {
                        iID_PhanBoVonID,
                        iID_DonViQuanLyID,
                        sSoQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }

        public bool deleteKeHoachVonNam(Guid iID_PhanBoVonID)
        {
            var sql = "DELETE VDT_KHV_PhanBoVon_ChiTiet WHERE iID_PhanBoVonID = @iID_PhanBoVonID; " +
                " DELETE VDT_KHV_PhanBoVon WHERE iID_PhanBoVonID = @iID_PhanBoVonID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_PhanBoVonID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        //NinhNV end

        #endregion
        #region Chủ đầu tư

        public IEnumerable<DM_ChuDauTu> GetChuDTListByNamLamViec(int namLamViec)
        {
            var sql =
                @" select distinct cdt.sId_CDT, cdt.sTenCDT ,cdt.iNamLamViec from DM_ChuDauTu as cdt 
	                            where cdt.iNamLamViec = @namLamViec";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_ChuDauTu>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         namLamViec,
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        #endregion

        #region Thong tri thanh toan
        public void UpdateThongTriById(VDT_ThongTri data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var entity = conn.Get<VDT_ThongTri>(data.iID_ThongTriID);
                entity.sMaThongTri = data.sMaThongTri;
                entity.sMoTa = data.sMoTa;
                conn.Update(entity);
            }
        }

        /// <summary>
        /// lay danh sach thong tri thanh toan
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <returns></returns>
        public IEnumerable<VDTThongTriModel> LayDanhSachThongTri(ref PagingInfo _paging, int iNamLamViec, string sUserName, bool bIsThanhToanView = false, string sMaDonVi = null, string sMaThongTri = null,
            int? iNamThongTri = null, DateTime? dNgayThongTri = null, bool isXuatDanhSach = false)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("sMaThongTri", sMaThongTri);
                lstParam.Add("iNamThongTri", iNamThongTri);
                lstParam.Add("dNgayThongTri", dNgayThongTri);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sUserName", sUserName);
                lstParam.Add("bIsThanhToanView", bIsThanhToanView);
                lstParam.Add("isXuatDanhSach", isXuatDanhSach);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<VDTThongTriModel>("proc_get_all_thongtri_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<VDTThongTriModel> GetThongTriQuyetToanPaging(ref PagingInfo _paging, string sMaDonVi = null, string sMaThongTri = null, int? iNamThongTri = null, DateTime? dNgayThongTri = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sMaThongTri", sMaThongTri);
                lstParam.Add("iNamThongTri", iNamThongTri);
                lstParam.Add("dNgayThongTri", dNgayThongTri);
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<VDTThongTriModel>("proc_get_all_thongtri_quyettoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        #region VDT_TT_DeNghiThanhToan
        /// <summary>
        /// lay chi tiet de nghi thanh toan
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="iNguonVon"></param>
        /// <param name="dNgayLapGanNhat"></param>
        /// <param name="dNgayTaoThongTri"></param>
        /// <returns></returns>
        public IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTiet(string sMaDonViQuanLy, int iNamThongTri, int iNguonVon, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_thongtri_denghithanhtoan.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTTTDeNghiThanhToanChiTiet>(sql, param: new
                {
                    sMaDonVi = sMaDonViQuanLy,
                    iNamThongTri,
                    iNguonVon,
                    dNgayLapGanNhat,
                    dNgayTaoThongTri
                }, commandType: CommandType.Text);
                if (items.Count() > 0)
                {
                    List<VDTTTDeNghiThanhToanChiTiet> listItems = items.ToList();
                    listItems.ForEach(item =>
                    {
                        if (item.iID_Parent == null && item.iID_LoaiCongTrinhID != null)
                        {
                            if (listItems.Where(x => x.iID_DeNghiThanhToan_ChiTietID == item.iID_LoaiCongTrinhID.Value).Count() == 0)
                            {
                                listItems.Add(new VDTTTDeNghiThanhToanChiTiet
                                {
                                    iID_DeNghiThanhToan_ChiTietID = item.iID_LoaiCongTrinhID.Value,
                                    sNoiDung = item.sTenLoaiCongTrinh,
                                    bHasChild = true
                                });
                            }

                            item.iID_Parent = item.iID_LoaiCongTrinhID.Value;
                        }
                    });

                    foreach (var topLevelNode in listItems.Where(x => x.iID_Parent == null))
                    {
                        TinhGiaTriThanhToan(listItems, topLevelNode);
                        TinhGiaTriTamUng(listItems, topLevelNode);
                        TinhGiaTriThuHoi(listItems, topLevelNode);
                    };
                    return listItems;
                }
                return null;
            }
        }
        public IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTiet_v1(string sMaDonViQuanLy, int iNamThongTri, int iNguonVon, DateTime? dNgayTaoThongTri)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_thongtri_denghithanhtoan_v1.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTTTDeNghiThanhToanChiTiet>(sql, param: new
                {
                    sMaDonVi = sMaDonViQuanLy,
                    iNamThongTri,
                    iNguonVon,
                    dNgayTaoThongTri
                }, commandType: CommandType.Text);
                if (items.Count() > 0)
                {
                    List<VDTTTDeNghiThanhToanChiTiet> listItems = items.ToList();
                    listItems.ForEach(item =>
                    {
                        if (item.iID_Parent == null && item.iID_LoaiCongTrinhID != null)
                        {
                            if (listItems.Where(x => x.iID_DeNghiThanhToan_ChiTietID == item.iID_LoaiCongTrinhID.Value).Count() == 0)
                            {
                                listItems.Add(new VDTTTDeNghiThanhToanChiTiet
                                {
                                    iID_DeNghiThanhToan_ChiTietID = item.iID_LoaiCongTrinhID.Value,
                                    sNoiDung = item.sTenLoaiCongTrinh,
                                    bHasChild = true
                                });
                            }

                            item.iID_Parent = item.iID_LoaiCongTrinhID.Value;
                        }
                    });

                    foreach (var topLevelNode in listItems.Where(x => x.iID_Parent == null))
                    {
                        TinhGiaTriThanhToan(listItems, topLevelNode);
                        TinhGiaTriTamUng(listItems, topLevelNode);
                        TinhGiaTriThuHoi(listItems, topLevelNode);
                    };
                    return listItems;
                }
                return new List<VDTTTDeNghiThanhToanChiTiet>();
            }
        }

        /// <summary>
        /// tinh lai gia tri thanh toan cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriThanhToan(IEnumerable<VDTTTDeNghiThanhToanChiTiet> tiers, VDTTTDeNghiThanhToanChiTiet current)
        {
            if (current.bHasChild)
                current.fGiaTriThanhToan = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_DeNghiThanhToan_ChiTietID);
            foreach (var child in children)
                current.fGiaTriThanhToan += TinhGiaTriThanhToan(tiers, child);
            return current.fGiaTriThanhToan;
        }

        /// <summary>
        /// tinh lai gia tri tam ung cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriTamUng(IEnumerable<VDTTTDeNghiThanhToanChiTiet> tiers, VDTTTDeNghiThanhToanChiTiet current)
        {
            if (current.bHasChild)
                current.fGiaTriTamUng = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_DeNghiThanhToan_ChiTietID);
            foreach (var child in children)
                current.fGiaTriTamUng += TinhGiaTriTamUng(tiers, child);
            return current.fGiaTriTamUng;
        }

        /// <summary>
        /// tinh lại gia tri thu hoi cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriThuHoi(IEnumerable<VDTTTDeNghiThanhToanChiTiet> tiers, VDTTTDeNghiThanhToanChiTiet current)
        {
            if (current.bHasChild)
                current.fGiaTriThuHoi = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_DeNghiThanhToan_ChiTietID);
            foreach (var child in children)
                current.fGiaTriThuHoi += TinhGiaTriThuHoi(tiers, child);
            return current.fGiaTriThuHoi;
        }
        #endregion

        #region VDT_TT_DeNghiThanhToanUng
        /// <summary>
        /// lay chi tiet de nghi thanh toan ung
        /// </summary>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="sMaNhomQuanLy"></param>
        /// <param name="dNgayLapGanNhat"></param>
        /// <param name="dNgayTaoThongTri"></param>
        /// <returns></returns>
        public IEnumerable<VDTTTDeNghiThanhToanChiTiet> GetDeNghiThanhToanChiTietUng(string sMaDonVi, int iNamThongTri, string sMaNhomQuanLy, DateTime? dNgayLapGanNhat, DateTime? dNgayTaoThongTri)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_thongtri_denghithanhtoanung.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTTTDeNghiThanhToanChiTiet>(sql, param: new
                {
                    sMaDonVi,
                    iNamThongTri,
                    sMaNhomQuanLy,
                    dNgayLapGanNhat,
                    dNgayTaoThongTri
                }, commandType: CommandType.Text);
                if (items.Count() > 0)
                {
                    List<VDTTTDeNghiThanhToanChiTiet> listItems = items.ToList();
                    listItems.ForEach(item =>
                    {
                        if (item.iID_Parent == null && item.iID_LoaiCongTrinhID != null)
                        {
                            if (listItems.Where(x => x.iID_DeNghiThanhToan_ChiTietID == item.iID_LoaiCongTrinhID.Value).Count() == 0)
                            {
                                listItems.Add(new VDTTTDeNghiThanhToanChiTiet
                                {
                                    iID_DeNghiThanhToan_ChiTietID = item.iID_LoaiCongTrinhID.Value,
                                    sNoiDung = item.sTenLoaiCongTrinh,
                                    bHasChild = true
                                });
                            }

                            item.iID_Parent = item.iID_LoaiCongTrinhID.Value;
                        }
                    });

                    foreach (var topLevelNode in listItems.Where(x => x.iID_Parent == null))
                    {
                        TinhGiaTriThanhToanUng(listItems, topLevelNode);
                        TinhGiaTriThuHoiUngNgoaiChiTieu(listItems, topLevelNode);
                    };
                    return listItems;
                }
                return null;
            }
        }

        /// <summary>
        /// tinh lai gia tri thanh toan ung cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriThanhToanUng(IEnumerable<VDTTTDeNghiThanhToanChiTiet> tiers, VDTTTDeNghiThanhToanChiTiet current)
        {
            if (current.bHasChild)
                current.fGiaTriThanhToan = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_DeNghiThanhToan_ChiTietID);
            foreach (var child in children)
                current.fGiaTriThanhToan += TinhGiaTriThanhToanUng(tiers, child);
            return current.fGiaTriThanhToan;
        }

        /// <summary>
        /// tinh lai gia tri thu hoi cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriThuHoiUngNgoaiChiTieu(IEnumerable<VDTTTDeNghiThanhToanChiTiet> tiers, VDTTTDeNghiThanhToanChiTiet current)
        {
            if (current.bHasChild)
                current.fGiaTriThuHoiUngNgoaiChiTieu = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_DeNghiThanhToan_ChiTietID);
            foreach (var child in children)
                current.fGiaTriThuHoiUngNgoaiChiTieu += TinhGiaTriThuHoiUngNgoaiChiTieu(tiers, child);
            return current.fGiaTriThuHoiUngNgoaiChiTieu;
        }
        #endregion

        /// <summary>
        /// lay ngay lap gan nhat theo don vi, nam thong tri, ma nguon von
        /// </summary>
        /// <param name="iIDDonViId"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="iNguonVon"></param>
        /// <returns></returns>
        public string LayNgayLapGanNhat(string iIDDonViId, int iNamThongTri, int iNguonVon, string iID_ThongTriID)
        {
            var sql = "SELECT TOP 1 CONVERT(varchar, dNgayThongTri, 103) FROM VDT_ThongTri WHERE iID_DonViID = @iIDDonViId AND iNamThongTri = @iNamThongTri AND sMaNguonVon = @iNguonVon {0} ORDER BY dNgayThongTri DESC ";
            if (!string.IsNullOrEmpty(iID_ThongTriID))
                sql = string.Format(sql, "AND dDateCreate < (SELECT dDateCreate FROM VDT_ThongTri WHERE iID_ThongTriID = @iID_ThongTriID) ");
            else
                sql = string.Format(sql, "");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<string>(sql,
                    param: new
                    {
                        iIDDonViId,
                        iNamThongTri,
                        iNguonVon,
                        iID_ThongTriID
                    }, commandType: CommandType.Text);
                return item;
            }
        }

        /// kiem tra trung ma thong tri
        /// </summary>
        /// <param name="sMaThongTri"></param>
        /// <returns></returns>
        public bool KiemTraTrungMaThongTri(string sMaThongTri, string iID_ThongTriID)
        {
            var sql = "SELECT COUNT(iID_ThongTriID) FROM VDT_ThongTri WHERE sMaThongTri = @sMaThongTri ";
            if (!string.IsNullOrEmpty(iID_ThongTriID))
                sql += " AND iID_ThongTriID <> @iID_ThongTriID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirst<int>(sql,
                   param: new
                   {
                       sMaThongTri,
                       iID_ThongTriID = string.IsNullOrEmpty(iID_ThongTriID) ? Guid.Empty : Guid.Parse(iID_ThongTriID)
                   },
                   commandType: System.Data.CommandType.Text);
                return count > 0;
            }
        }

        /// <summary>
        ///  lay danh sach kieu thong tri
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VDT_DM_KieuThongTri> LayDanhSachKieuThongTri()
        {
            var sql = "SELECT * FROM VDT_DM_KieuThongTri";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_KieuThongTri>(sql, commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// lay loai thong tri theo kieu loai thong tri
        /// </summary>
        /// <param name="iKieuLoaiThongTri"></param>
        /// <returns></returns>
        public VDT_DM_LoaiThongTri LayLoaiThongTriTheoKieuThongTri(int iKieuLoaiThongTri)
        {
            var sql = "SELECT * FROM VDT_DM_LoaiThongTri WHERE iKieuLoaiThongTri = @iKieuLoaiThongTri";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_DM_LoaiThongTri>(sql, param: new { iKieuLoaiThongTri }, commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// luu thong tin thong tri
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sUserName"></param>
        /// <param name="bReloadChiTiet"></param>
        /// <returns></returns>
        public bool LuuThongTinThongTriQuyetToan(VDT_ThongTri model, List<VDT_ThongTri_ChiTiet> lstDetail, string sUserLogin)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    VDT_ThongTri modelNew = new VDT_ThongTri();
                    if (model.iID_ThongTriID == Guid.Empty)
                    {
                        #region Them moi VDT_ThongTri
                        var entity = new VDT_ThongTri();
                        entity.sMaThongTri = model.sMaThongTri;
                        entity.dNgayThongTri = model.dNgayThongTri;
                        entity.iNamThongTri = model.iNamThongTri;
                        entity.sNguoiLap = model.sNguoiLap;
                        entity.iID_LoaiThongTriID = model.iID_LoaiThongTriID;
                        entity.sMaNguonVon = model.sMaNguonVon;
                        entity.iID_DonViID = model.iID_DonViID;
                        entity.bThanhToan = model.bThanhToan;
                        entity.bIsCanBoDuyet = true;
                        entity.iNamNganSach = model.iNamNganSach;
                        entity.sUserCreate = sUserLogin;
                        entity.dDateCreate = DateTime.Now;
                        entity.iLoaiThongTri = model.iLoaiThongTri;
                        entity.sMoTa = model.sMoTa;
                        entity.iID_BCQuyetToanNienDo = model.iID_BCQuyetToanNienDo;
                        conn.Insert(entity, trans);
                        model.iID_ThongTriID = entity.iID_ThongTriID;
                        #endregion
                    }
                    else
                    {
                        #region Sua VDT_ThongTri
                        var entity = conn.Get<VDT_ThongTri>(model.iID_ThongTriID, trans);
                        if (entity == null)
                            return false;
                        entity.sMaThongTri = model.sMaThongTri;
                        entity.sNguoiLap = model.sNguoiLap;
                        entity.sUserUpdate = sUserLogin;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);

                        modelNew = entity;
                        #endregion
                    }
                    // commit to db
                    trans.Commit();
                }
                XoaThongTriChiTiet(model.iID_ThongTriID);
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    lstDetail = lstDetail.Select(n => { n.iID_ThongTriID = model.iID_ThongTriID; return n; }).ToList();
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    conn.Insert<VDT_ThongTri_ChiTiet>(lstDetail, trans);
                    trans.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public IEnumerable<VDT_DM_LoaiThongTri> GetLoaiThongTri()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var data = conn.GetList<VDT_DM_LoaiThongTri>();
                if (data == null) return new List<VDT_DM_LoaiThongTri>();
                return data.ToList();
            }
        }

        /// <summary>
        /// lay thong tin chi tiet cua thong tri chi tiet theo kieu thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <param name="iID_KieuThongTriID"></param>
        /// <returns></returns>
        public IEnumerable<VDTThongTriChiTiet> LayThongTriChiTietTheoKieuThongTri(string iID_ThongTriID, string iID_KieuThongTriID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_thongtri_get_thongchichitiet_theokieuthongtri.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTThongTriChiTiet>(sql, param: new
                {
                    iID_ThongTriID,
                    iID_KieuThongTriID
                }, commandType: CommandType.Text);
                if (items.Count() > 0)
                {
                    List<VDTThongTriChiTiet> listItems = items.ToList();
                    listItems.ForEach(item =>
                    {
                        if (item.iID_Parent == null && item.iID_LoaiCongTrinhID != null)
                        {
                            if (listItems.Where(x => x.iID_ThongTriChiTietID == item.iID_LoaiCongTrinhID.Value).Count() == 0)
                            {
                                listItems.Add(new VDTThongTriChiTiet
                                {
                                    iID_ThongTriChiTietID = item.iID_LoaiCongTrinhID.Value,
                                    sNoiDung = item.sTenLoaiCongTrinh,
                                    bHasChild = true
                                });
                            }

                            item.iID_Parent = item.iID_LoaiCongTrinhID.Value;
                        }
                    });

                    foreach (var topLevelNode in listItems.Where(x => x.iID_Parent == null))
                    {
                        TinhGiaTriSoTien(listItems, topLevelNode);
                    };
                    return listItems;
                }
                return null;
            }
        }

        /// <summary>
        /// tinh lại so tien cho hang cha
        /// </summary>
        /// <param name="tiers"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private double? TinhGiaTriSoTien(IEnumerable<VDTThongTriChiTiet> tiers, VDTThongTriChiTiet current)
        {
            if (current.bHasChild || current.fSoTien == null)
                current.fSoTien = 0;
            var children = tiers.Where(x => x.iID_Parent == current.iID_ThongTriChiTietID);
            foreach (var child in children)
                current.fSoTien += TinhGiaTriSoTien(tiers, child);
            return current.fSoTien;
        }

        /// <summary>
        /// lay thong tin chi tiet cua thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <returns></returns>
        public VDTThongTriModel LayChiTietThongTri(string iID_ThongTriID)
        {
            var sql = "SELECT *, donvi.sTen AS sTenDonVi, nguonns.sTen AS sTenNguonNganSach FROM VDT_ThongTri thongtri " +
                    "INNER JOIN NS_DonVi donvi ON donvi.iID_Ma = thongtri.iID_DonViID " +
                    "LEFT JOIN NS_NguonNganSach nguonns ON nguonns.sMoTa = thongtri.sMaNguonVon " +
                    "WHERE thongtri.iID_ThongTriID = @iID_ThongTriID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var item = conn.QueryFirstOrDefault<VDTThongTriModel>(sql, param: new
                {
                    iID_ThongTriID
                }, commandType: CommandType.Text);
                if (item != null)
                {
                    string sNgayLapGanNhat = LayNgayLapGanNhat(item.iID_DonViID.ToString(), item.iNamThongTri.Value, int.Parse(item.sMaNguonVon), iID_ThongTriID);
                    if (!string.IsNullOrEmpty(sNgayLapGanNhat))
                        item.dNgayLapGanNhat = DateTime.ParseExact(sNgayLapGanNhat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (item == null) return new VDTThongTriModel();
                return item;
            }
        }

        /// <summary>
        /// xoa thong tri
        /// </summary>
        /// <param name="iID_ThongTriID"></param>
        /// <returns></returns>
        public bool XoaThongTri(Guid iID_ThongTriID)
        {
            var sql = "DELETE VDT_ThongTri WHERE iID_ThongTriID = @iID_ThongTriID; " +
                    "DELETE VDT_ThongTri_ChiTiet WHERE iID_ThongTriID = @iID_ThongTriID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_ThongTriID
                    },
                    commandType: CommandType.Text);
                if (r > 0)
                    XoaThongTriChiTiet(iID_ThongTriID);
                return r > 0;
            }
        }

        public bool XoaThongTriChiTiet(Guid iID_ThongTriID)
        {
            var sql = "DELETE VDT_ThongTri_ChiTiet WHERE iID_ThongTriID = @iID_ThongTriID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                try
                {
                    var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_ThongTriID
                    },
                    commandType: CommandType.Text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// lay danh sach thong tri thanh toan
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="sUserName"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="sMaThongTri"></param>
        /// <param name="iNamThongTri"></param>
        /// <param name="dNgayThongTri"></param>
        /// <returns></returns>
        public IEnumerable<VDTThongTriModel> LayDanhSachThongTriXuatFile(int iNamLamViec, string sUserName, string sMaDonVi, string sMaThongTri,
            int? iNamThongTri, DateTime? dNgayThongTri)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_thongtri_get_all_thongtri_export.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTThongTriModel>(sql,
                            param: new
                            {
                                sMaThongTri,
                                iNamThongTri,
                                dNgayThongTri,
                                sMaDonVi,
                                iNamLamViec,
                                sUserName
                            },
                            commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Giải ngân - Thanh toán

        public DataTable GetDuAnInGiaiNganThanhToan(Guid iID_DonViQuanLyID, int iID_NguonVonID, Guid iID_LoaiNguonVonID, DateTime dNgayQuyetDinh, int iNamKeHoach, Guid iID_NganhID)
        {
            String SQL = @"sp_SelectDuAnForComBo_ThanhToan @donViQuanLyId, @nguonVonID, @loaiNguonVonID, @ngayLap, @year, @nganhId";

            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@donViQuanLyId", iID_DonViQuanLyID);
            cmd.Parameters.AddWithValue("@nguonVonID", iID_NguonVonID);
            cmd.Parameters.AddWithValue("@loaiNguonVonID", iID_LoaiNguonVonID);
            cmd.Parameters.AddWithValue("@ngayLap", dNgayQuyetDinh);
            cmd.Parameters.AddWithValue("@year", iNamKeHoach);
            cmd.Parameters.AddWithValue("@nganhId", iID_NganhID);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        /// <summary>
        /// luu thanh toan
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public bool LuuThanhToan(GiaiNganThanhToanViewModel data, string userLogin)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    // insert VDT_TT_DeNghiThanhToan
                    if (string.IsNullOrEmpty(data.iID_MaDonViQuanLy))
                    {
                        var objDonViQuanLy = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                        if (objDonViQuanLy != null)
                            data.iID_MaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;
                    }
                    data.sUserCreate = userLogin;
                    data.dDateCreate = DateTime.Now;
                    conn.Insert<VDT_TT_DeNghiThanhToan>(data, trans);

                    // insert VDT_TT_DeNghiThanhToan_KHV
                    if (data.listKeHoachVon != null && data.listKeHoachVon.Count > 0)
                    {
                        List<VDT_TT_DeNghiThanhToan_KHV> lstKHV = new List<VDT_TT_DeNghiThanhToan_KHV>();
                        foreach (var item in data.listKeHoachVon)
                        {
                            lstKHV.Add(new VDT_TT_DeNghiThanhToan_KHV()
                            {
                                iID_DeNghiThanhToanID = data.iID_DeNghiThanhToanID,
                                iID_KeHoachVonID = item.Id,
                                iLoai = item.iPhanLoai
                            });
                        }

                        conn.Insert<VDT_TT_DeNghiThanhToan_KHV>(lstKHV, trans);
                    }

                    // insert VDT_TT_PheDuyetThanhToan_ChiTiet
                    //if (data.listPheDuyetChiTiet != null && data.listPheDuyetChiTiet.Count > 0)
                    //{
                    //    List<VDT_TT_PheDuyetThanhToan_ChiTiet> lstPheDuyetChiTiet = new List<VDT_TT_PheDuyetThanhToan_ChiTiet>();
                    //    foreach (var item in data.listPheDuyetChiTiet)
                    //    {
                    //        lstPheDuyetChiTiet.Add(ConvertDataPheDuyetThanhToanChiTiet(item, data.iID_DeNghiThanhToanID, userLogin));
                    //    }

                    //    conn.Insert<VDT_TT_PheDuyetThanhToan_ChiTiet>(lstPheDuyetChiTiet, trans);
                    //}
                    trans.Commit();

                    // insert bang tong hop
                    //XuLySoLieuBangTongHop(data);
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// luu phe duyet thanh toan chi tiet
        /// </summary>
        /// <param name="iIdDeNghiThanhToanId"></param>
        /// <param name="data"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public bool LuuPheDuyetThanhToanChiTiet(Guid iIdDeNghiThanhToanId, List<PheDuyetThanhToanChiTiet> data, string userLogin, int iNamLamViec, DateTime dNgayPheDuyet, double? fThueGiaTriGiaTangDuocDuyet, double? fChuyenTienBaoHanhDuocDuyet)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    VDT_TT_DeNghiThanhToan objDeNghiThanhToan = GetDeNghiThanhToanByID(iIdDeNghiThanhToanId);
                    // update các giá trị phê duyệt cho đntt
                    objDeNghiThanhToan.dNgayPheDuyet = dNgayPheDuyet;
                    objDeNghiThanhToan.fThueGiaTriGiaTangDuocDuyet = fThueGiaTriGiaTangDuocDuyet;
                    objDeNghiThanhToan.fChuyenTienBaoHanhDuocDuyet = fChuyenTienBaoHanhDuocDuyet;
                    conn.Update<VDT_TT_DeNghiThanhToan>(objDeNghiThanhToan, trans);

                    // insert VDT_TT_PheDuyetThanhToan_ChiTiet
                    if (data != null && data.Count > 0)
                    {
                        List<VDT_TT_PheDuyetThanhToan_ChiTiet> lstPheDuyetChiTiet = new List<VDT_TT_PheDuyetThanhToan_ChiTiet>();
                        foreach (var item in data)
                        {
                            if (!item.isDelete)
                                lstPheDuyetChiTiet.Add(ConvertDataPheDuyetThanhToanChiTiet(item, iIdDeNghiThanhToanId, userLogin, iNamLamViec));
                        }

                        conn.Insert<VDT_TT_PheDuyetThanhToan_ChiTiet>(lstPheDuyetChiTiet, trans);
                    }
                    trans.Commit();

                    // insert bang tong hop
                    XuLySoLieuBangTongHop(data, objDeNghiThanhToan);
                    return true;
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public VDT_TT_PheDuyetThanhToan_ChiTiet ConvertDataPheDuyetThanhToanChiTiet(PheDuyetThanhToanChiTiet item, Guid iID_DeNghiThanhToanID, string sUserLogin, int iNamLamViec)
        {
            ConvertMucLucNganSach(item, iNamLamViec);

            VDT_TT_PheDuyetThanhToan_ChiTiet model = new VDT_TT_PheDuyetThanhToan_ChiTiet();
            model.iID_DeNghiThanhToanID = iID_DeNghiThanhToanID;
            model.iID_PheDuyetThanhToan_ChiTietID = item.iID_PheDuyetThanhToan_ChiTietID;
            model.iID_NganhID = item.iID_NganhID;
            model.sM = item.sM;
            model.sTM = item.sTM;
            model.sTTM = item.sTTM;
            model.sNG = item.sNG;

            if (item.iID_NganhID.HasValue && item.iID_NganhID.Value != Guid.Empty)
                model.iID_NganhID = item.iID_NganhID;
            else if (item.iID_TietMucID.HasValue && item.iID_TietMucID.Value != Guid.Empty)
                model.iID_TietMucID = item.iID_TietMucID;
            else if (item.iID_TieuMucID.HasValue && item.iID_TieuMucID.Value != Guid.Empty)
                model.iID_TieuMucID = item.iID_TieuMucID;
            else if (item.iID_MucID.HasValue && item.iID_MucID.Value != Guid.Empty)
                model.iID_MucID = item.iID_MucID;

            model.sGhiChu = item.sGhiChu;
            model.iID_KeHoachVonID = item.iID_KeHoachVonID;
            model.iLoaiKeHoachVon = item.iLoaiKeHoachVon;

            if (item.iLoaiNamKH == (int)NamKeHoachEnum.Type.NAM_TRUOC && item.iLoaiKeHoachVon <= 2)
            {
                model.iLoaiKeHoachVon = item.iLoaiKeHoachVon + 2;
            }
            else
            {
                model.iLoaiKeHoachVon = item.iLoaiKeHoachVon;
            }

            model.iID_KeHoachVonID = item.iID_KeHoachVonID;
            if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THANH_TOAN || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.TAM_UNG)
            {
                model.fGiaTriThanhToanNN = item.fGiaTriNgoaiNuoc;
                model.fGiaTriThanhToanTN = item.fGiaTriTrongNuoc;
            }
            else
            {
                if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY)
                {
                    model.fGiaTriThuHoiUngTruocNamNayNN = item.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiUngTruocNamNayTN = item.fGiaTriTrongNuoc;
                }
                if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                {
                    model.fGiaTriThuHoiUngTruocNamTruocNN = item.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiUngTruocNamTruocTN = item.fGiaTriTrongNuoc;
                }
                else if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_NAY)
                {
                    model.fGiaTriThuHoiNamNayNN = item.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiNamNayTN = item.fGiaTriTrongNuoc;
                }
                else if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC)
                {
                    model.fGiaTriThuHoiNamTruocNN = item.fGiaTriNgoaiNuoc;
                    model.fGiaTriThuHoiNamTruocTN = item.fGiaTriTrongNuoc;
                }
            }
            return model;
        }

        private bool ConvertMucLucNganSach(PheDuyetThanhToanChiTiet item, int iNamLamViec)
        {
            var lstMlns = GetAllMucLucNganSachByNamLamViec(iNamLamViec);
            if (lstMlns == null) return false;
            Dictionary<string, Guid> dicMlns = new Dictionary<string, Guid>();
            foreach (var itemMlns in lstMlns)
            {
                if (!dicMlns.ContainsKey(itemMlns.sXauNoiMa))
                    dicMlns.Add(itemMlns.sXauNoiMa, itemMlns.iID_MaMucLucNganSach);
            }

            if (!string.IsNullOrEmpty(item.sXauNoiMa))
            {
                if (!dicMlns.ContainsKey(item.sXauNoiMa)) return false;
                Guid iIdMaMucLucNganSach = dicMlns[item.sXauNoiMa];
                if (!string.IsNullOrEmpty(item.sNG))
                    item.iID_NganhID = iIdMaMucLucNganSach;
                else if (!string.IsNullOrEmpty(item.sTTM))
                    item.iID_TieuMucID = iIdMaMucLucNganSach;
                else if (!string.IsNullOrEmpty(item.sTM))
                    item.iID_TietMucID = iIdMaMucLucNganSach;
                else if (!string.IsNullOrEmpty(item.sM))
                    item.iID_MucID = iIdMaMucLucNganSach;
            }

            return true;
        }
        public bool KiemTraTrung(string sTable, string sColumn, string val)
        {
            var sql = "SELECT COUNT(*) FROM " + sTable + " WHERE " + sColumn + " = @val";
            //if (!string.IsNullOrEmpty(iID_ThongTriID))
            //    sql += " AND iID_ThongTriID <> @iID_ThongTriID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirst<int>(sql,
                   param: new
                   {
                       val
                       //iID_ThongTriID = string.IsNullOrEmpty(iID_ThongTriID) ? Guid.Empty : Guid.Parse(iID_ThongTriID)
                   },
                   commandType: System.Data.CommandType.Text);
                return count > 0;
            }
        }


        public VDT_TT_DeNghiThanhToan GetDeNghiThanhToanByID(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Get<VDT_TT_DeNghiThanhToan>(iId);
            }
        }

        public bool UpdateDeNghiThanhToan(VDT_TT_DeNghiThanhToan data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Update(data);
            }
            return true;
        }

        public IEnumerable<GiaiNganThanhToanViewModel> GetAllGiaiNganThanhToanPaging(ref PagingInfo _paging, string sSoDeNghi, int? iNamKeHoach, DateTime? dNgayLapFrom, DateTime? dNgayLapTo, Guid? sDonViQuanLy)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoDeNghi", sSoDeNghi);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("dNgayLapFrom", dNgayLapFrom);
                    lstParam.Add("dNgayLapTo", dNgayLapTo);
                    lstParam.Add("sDonViQuanLy", sDonViQuanLy, dbType: DbType.Guid);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<GiaiNganThanhToanViewModel>("proc_get_giainganthanhtoan_paging", lstParam, commandType: CommandType.StoredProcedure);
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

        public IEnumerable<GiaiNganThanhToanViewModel> LoadVDTTTDeNghiThanhToanIndex(ref PagingInfo _paging, int workYear, string maNguoiDung, int? iNamKeHoach, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, string iidDuAn = "", string iLoaiThanhToan = "", string sMaDonVi = "", string sSoDeNghi = "")
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("dNgayDeNghiFrom", dNgayDeNghiFrom);
                    lstParam.Add("dNgayDeNghiTo", dNgayDeNghiTo);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("sSoDeNghi", sSoDeNghi);
                    lstParam.Add("YearOfWork", workYear);
                    lstParam.Add("UserName", maNguoiDung);
                    lstParam.Add("iidDuAn", iidDuAn == "" ? null : iidDuAn);
                    lstParam.Add("iLoaiThanhToan", iLoaiThanhToan == "" ? null : iLoaiThanhToan);
                    lstParam.Add("sMaDonVi", sMaDonVi == "" ? null : sMaDonVi);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var item = conn.Query<GiaiNganThanhToanViewModel>("sp_vdt_denghithanhtoan_index_2", lstParam, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                    return item == null ? new List<GiaiNganThanhToanViewModel>() : item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<GiaiNganThanhToanChiTietViewModel> GetDeNghiThanhToanChiTietByDeNghiThanhToanID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_denghithanhtoanchitiet_by_denghithanhtoanid.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<GiaiNganThanhToanChiTietViewModel>(sql,
                        param: new
                        {
                            iID_DeNghiThanhToanID = iId
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

        public bool DeleteDeNghiThanhToan(Guid iId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Execute("DELETE VDT_TT_DeNghiThanhToan WHERE iID_DeNghiThanhToanID = @iId",
                        param: new
                        {
                            iId
                        },
                        commandType: CommandType.Text);
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool DeletePheDuyetThanhToanChiTietByThanhToanId(Guid iId)
        {
            string sql = @"SELECT * FROM VDT_TT_PheDuyetThanhToan_ChiTiet WHERE iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID";
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    IEnumerable<VDT_TT_PheDuyetThanhToan_ChiTiet> lstData = conn.Query<VDT_TT_PheDuyetThanhToan_ChiTiet>(sql,
                        param: new
                        {
                            iID_DeNghiThanhToanID = iId
                        },
                        commandType: CommandType.Text);

                    if (lstData != null && lstData.Any())
                    {
                        foreach (var item in lstData)
                            conn.Delete<VDT_TT_PheDuyetThanhToan_ChiTiet>(item);
                    }

                    // update bang tong hop
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sLoai", LOAI_CHUNG_TU.CAP_THANH_TOAN);
                    lstParam.Add("uIdQuyetDinh", iId);

                    conn.Execute("sp_delete_tonghopnguonnsdautu_giam", lstParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return true;
        }

        public NS_MucLucNganSach GetKhoanByNganhID(Guid iId)
        {
            string sql = @"DECLARE @sLNS nvarchar(100)
	                        DECLARE @sL nvarchar(100)
	                        DECLARE @sk nvarchar(100)
	                        SELECT @sLNS = sLNS, @sL = sL, @sk = sK FROM NS_MucLucNganSach WHERE iID_MaMucLucNganSach = @iId
	                        SELECT TOP(1) * FROM NS_MucLucNganSach WHERE sLNS = @sLNS AND sL = @sL AND sK = @sk AND sM = '' AND sTM = '' AND sTTM = '' AND sNG = '' AND sTNG = ''";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<NS_MucLucNganSach>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

                return items;
            }
        }

        /// <summary>
        /// get list mlns by du an
        /// </summary>
        /// <param name="iIdDuAn"></param>
        /// <param name="iIdMaDonViQuanLy"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="dNgayLap"></param>
        /// <param name="iNamLamViec"></param>
        /// <returns></returns>
        public IEnumerable<VDTKHVPhanBoVonChiTietViewModel> GetAllMucLucNganSachByDuAnId(Guid iIdDuAn, string iIdMaDonViQuanLy, int iIdNguonVon, DateTime? dNgayLap, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("uIdDuAn", iIdDuAn);
                    lstParam.Add("sMaDonVi", iIdMaDonViQuanLy);
                    lstParam.Add("iNguonVon", iIdNguonVon);
                    lstParam.Add("dNgayLap", dNgayLap);
                    lstParam.Add("iNamLamViec", iNamLamViec);

                    var items = conn.Query<VDTKHVPhanBoVonChiTietViewModel>("sp_pbv_get_muclucngansach_by_duan_khvnam", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        #endregion

        #region Quản lý kế hoạch vốn ứng được duyệt - NinhNV
        public IEnumerable<VDT_DM_NhomQuanLy> GetNhomQuanLyList()
        {
            var sql = "select * from VDT_DM_NhomQuanLy where sMaNhomQuanLy in ('CTC', 'CKHDT') ORDER BY iThuTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_NhomQuanLy>(sql);
                return items;
            }
        }

        public IEnumerable<VDT_DA_DuAn> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_duan_by_dvql_vangayqd.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID,
                       dNgayQuyetDinh
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public double GetTongMucDauTuTheoIdDuan(Guid iID_DuAnID, DateTime? dNgayQuyetDinh)
        {
            var sql = "select ISNULL(sum(fTongMucDauTuPheDuyet), 0) as fTongMucDauTuPheDuyet from VDT_DA_QDDauTu where iID_DuAnID = @iID_DuAnID " +
                " and dNgayQuyetDinh <= @dNgayQuyetDinh ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTKeHoachVonUngInfoModel> GetAllKHVUDuocDuyet(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh = null, DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iIdNguonVon = null, string iID_MaDonViQuanLyID = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iID_MaDonViQuanLyID", iID_MaDonViQuanLyID);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIdNguonVon", iIdNguonVon);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTKeHoachVonUngInfoModel>("proc_get_all_vdt_kehoachvonung_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<VdtKhvKeHoachVonUngChiTietModel> GetKHVUChiTietList(Guid? iID_KeHoachUngID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_khvuchitiet_by_khvuid.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VdtKhvKeHoachVonUngChiTietModel>(sql,
                   param: new
                   {
                       iID_KeHoachUngID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public bool deleteKHVUChiTiet(Guid iID_KeHoachUngID)
        {
            var sql = "delete VDT_KHV_KeHoachVonUng_ChiTiet where iID_KeHoachUngID = @iID_KeHoachUngID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_KeHoachUngID
                    },
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }
        public bool deleteKHVU(Guid iID_KeHoachUngID, string username)
        {

            try
            {
                var sql = "delete VDT_KHV_KeHoachVonUng where Id = @iID_KeHoachUngID ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<VDT_KHV_KeHoachVonUng>(iID_KeHoachUngID, trans);
                    if (entity != null && entity.iID_ParentID != null)
                    {
                        var entityChild = conn.Get<VDT_KHV_KeHoachVonUng>(entity.iID_ParentID, trans);
                        entityChild.bActive = true;
                        entityChild.sUserCreate = username;
                        entityChild.dDateUpdate = DateTime.Now;
                        conn.Update<VDT_KHV_KeHoachVonUng>(entityChild, trans);
                    }

                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_KeHoachUngID
                        },
                        commandType: CommandType.Text, transaction: trans);
                    trans.Commit();
                    conn.Close();

                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }
        public VDTKeHoachVonUngDuocDuyetViewModel GetKHVUById(Guid iID_KeHoachUngID, int iNamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_khvu_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTKeHoachVonUngDuocDuyetViewModel>(sql,
                    param: new
                    {
                        iID_KeHoachUngID,
                        iNamLamViec
                    },
                    commandType: CommandType.Text).FirstOrDefault();
                if (item != null)
                    item.listChiTietDuAn = GetKHVUChiTietList(iID_KeHoachUngID).ToList();
                return item;
            }
        }

        public bool CheckExistSoQuyetDinh(Guid? iID_KeHoachUngID, string sSoQuyetDinh)
        {
            var sql = "SELECT * FROM VDT_KHV_KeHoachVonUng WHERE (@iID_KeHoachUngID IS NULL OR Id != @iID_KeHoachUngID) and sSoQuyetDinh = @sSoQuyetDinh";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_KHV_KeHoachVonUng>(
                    sql,
                    param: new
                    {
                        iID_KeHoachUngID,
                        sSoQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }
        #endregion

        #region Quản lý phê duyệt vốn ứng ngoài chỉ tiêu - NinhNV
        public IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhKHVU(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_duan_by_dvql_va_ngayqd_khvu.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTPheDuyetVonUngNgoaiCTViewModel>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID,
                       dNgayQuyetDinh
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }
        public IEnumerable<VDT_DA_TT_HopDong> LayDanhSachHopDongTheoDuAn(Guid iID_DuAnID)
        {
            var sql = "select * from VDT_DA_TT_HopDong where bActive = 1 and iID_DuAnID = @iID_DuAnID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_TT_HopDong>(sql,
                   param: new
                   {
                       iID_DuAnID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public double GetLuyKeKHVUDuocDuyet(Guid iID_DuAnID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_luyke_khvu_duocduyet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_DonViQuanLyID,
                        iID_NhomQuanLyID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public VDTPheDuyetVonUngNgoaiCTViewModel GetThongTinHopDong(Guid iID_HopDongID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_hopdong_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<VDTPheDuyetVonUngNgoaiCTViewModel>(sql,
                    param: new
                    {
                        iID_HopDongID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public VDTPheDuyetVonUngNgoaiCTViewModel GetLuyKeUng(Guid iID_DuAnID, Guid? iID_HopDongID, Guid iID_DonViQuanLyID, Guid iID_NhomQuanLyID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_luyke_ung.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<VDTPheDuyetVonUngNgoaiCTViewModel>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_HopDongID,
                        iID_DonViQuanLyID,
                        iID_NhomQuanLyID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<VDTPheDuyetVonUngNgoaiChiTieuInfoModel> GetAllPheDuyetVonUngNCT(ref PagingInfo _paging, Guid? iID_DonViQuanLyID, string sSoDeNghi, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sSoDeNghi", sSoDeNghi);
                lstParam.Add("dNgayDeNghiFrom", dNgayDeNghiFrom);
                lstParam.Add("dNgayDeNghiTo", dNgayDeNghiTo);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTPheDuyetVonUngNgoaiChiTieuInfoModel>("proc_get_all_vdt_pheduyetvonung_nct_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool deletePDVUNCTChiTiet(Guid iID_DeNghiThanhToanID)
        {
            var sql = "delete VDT_TT_DeNghiThanhToanUng_ChiTiet where iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_DeNghiThanhToanID
                    },
                    commandType: CommandType.Text);
                return r >= 0;
            }
        }
        public bool deletePDVUNCT(Guid iID_DeNghiThanhToanID)
        {
            var sql = "delete dbo.VDT_TT_DeNghiThanhToanUng where iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_DeNghiThanhToanID
                    },
                    commandType: CommandType.Text);
                return r > 0;
            }
        }

        public IEnumerable<VDTPheDuyetVonUngNgoaiCTViewModel> GetPDVUNCTChiTietList(Guid? iID_DeNghiThanhToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_dnttuchitiet_by_dnttuid.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTPheDuyetVonUngNgoaiCTViewModel>(sql,
                   param: new
                   {
                       iID_DeNghiThanhToanID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public VDTPheDuyetVonUngNgoaiCTViewModel GetPDVUNCTById(Guid iID_DeNghiThanhToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_dnttu_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDTPheDuyetVonUngNgoaiCTViewModel>(sql,
                    param: new
                    {
                        iID_DeNghiThanhToanID
                    },
                    commandType: CommandType.Text);
                return item.ToList().FirstOrDefault();
            }
        }

        public bool CheckExistSoDeNghi(Guid iID_DeNghiThanhToanID, string sSoDeNghi)
        {
            var sql = "SELECT * FROM VDT_TT_DeNghiThanhToanUng WHERE iID_DeNghiThanhToanID != @iID_DeNghiThanhToanID and sSoDeNghi = @sSoDeNghi";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_TT_DeNghiThanhToanUng>(
                    sql,
                    param: new
                    {
                        iID_DeNghiThanhToanID,
                        sSoDeNghi
                    },
                    commandType: CommandType.Text);

                return item.ToList().Count > 0;
            }
        }
        #endregion

        #region Kế hoạch 5 năm
        public IEnumerable<VDT_DA_DuAn> GetVDTDADuAn()
        {
            string sSql = "SELECT * FROM VDT_DA_DuAn";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sSql,
                //param: new
                //{
                //    iId
                //},

                commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public KeHoach5NamViewModel Get_KeHoach5Nam(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("get_by_id_kehoach5nam.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<KeHoach5NamViewModel>(sql, param: new
                {
                    id
                },
                commandType: CommandType.Text);
                if (items != null)
                {
                    items.listChiTiet = ListKeHoach5NamChiTietChuaDieuChinh(items.iID_KeHoach5NamID).ToList();
                }
                return items;
            }
        }

        public bool InsertKeHoach5Nam(ref VDT_KHV_KeHoach5Nam data)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Insert(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool InsertKeHoach5NamChiTiet(ref List<VDT_KHV_KeHoach5Nam_ChiTiet> lstData)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Insert<VDT_KHV_KeHoach5Nam_ChiTiet>(lstData);
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<KeHoach5NamViewModel> LayDanhSachKeHoach5Nam(ref PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iGiaiDoanTu, int? iGiaiDoanDen, string sMaDonVi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iGiaiDoanTu", iGiaiDoanTu);
                lstParam.Add("iGiaiDoanDen", iGiaiDoanDen);
                lstParam.Add("sMaDonVi", sMaDonVi);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<KeHoach5NamViewModel>("proc_get_all_kehoach5nam_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<KeHoach5NamChiTietViewModel> ListKeHoach5NamChiTiet(Guid iID_KeHoach5NamID, int iGiaiDoanTu, int iGiaiDoanDen, DateTime? dNgayLap)
        {
            List<KeHoach5NamChiTietViewModel> listDataReturn = new List<KeHoach5NamChiTietViewModel>();
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 1, sTenDuAn = "Dự án chuyển tiếp", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 2, sTenDuAn = "Dự án khởi công mới", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 3, sTenDuAn = "Dự án kết thúc đầu tư", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_kehoach5namchitiet_by_kh5nam.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<KeHoach5NamChiTietViewModel>(sql,
                   param: new
                   {
                       iID_KeHoach5NamID,
                       iGiaiDoanTu,
                       iGiaiDoanDen,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                foreach (var item in items)
                {
                    item.iId = listDataReturn.Count + 1;
                    item.iParentId = 1;
                    item.fGiaTriSauDieuChinh = item.fGiaTriKeHoach;
                    listDataReturn.Add(item);
                }
                return listDataReturn;
            }
        }

        public IEnumerable<KeHoach5NamChiTietViewModel> ListKeHoach5NamChiTietChuaDieuChinh(Guid iID_KeHoach5NamID)
        {
            List<KeHoach5NamChiTietViewModel> listDataReturn = new List<KeHoach5NamChiTietViewModel>();
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 1, sTenDuAn = "Dự án chuyển tiếp", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 2, sTenDuAn = "Dự án khởi công mới", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            listDataReturn.Add(new KeHoach5NamChiTietViewModel { iId = 3, sTenDuAn = "Dự án kết thúc đầu tư", fGiaTriKeHoach = 0, fGiaTriDieuChinh = 0, fGiaTriSauDieuChinh = 0 });
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_kehoach5namchitiet_chuadieuchinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<KeHoach5NamChiTietViewModel>(sql,
                   param: new
                   {
                       iID_KeHoach5NamID
                   },
                   commandType: System.Data.CommandType.Text);
                foreach (var item in items)
                {
                    item.iId = listDataReturn.Count + 1;
                    item.iParentId = 1;
                    item.fGiaTriSauDieuChinh = item.fGiaTriKeHoach;
                    listDataReturn.Add(item);
                }
                return listDataReturn;
            }
        }

        public void DeleteKeHoachChiTiet(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_delete_kehoach5namchitiet_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Execute(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);

            }
        }

        public bool deleteKH5Nam(Guid iID, string sUserLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                var keHoach5Nam = conn.Get<VDT_KHV_KeHoach5Nam>(iID, trans);
                if (keHoach5Nam == null)
                    return false;
                if (keHoach5Nam.iID_ParentId != null)
                {
                    var keHoach5NamCha = conn.Get<VDT_KHV_KeHoach5Nam>(keHoach5Nam.iID_ParentId, trans);
                    if (keHoach5NamCha != null)
                    {
                        keHoach5NamCha.bActive = true;
                        keHoach5NamCha.sUserUpdate = sUserLogin;
                        keHoach5NamCha.dDateUpdate = DateTime.Now;
                        conn.Update(keHoach5NamCha, trans);
                    }
                }

                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE VDT_KHV_KeHoach5Nam_ChiTiet WHERE iID_KeHoach5NamID = '{0}';", iID);
                conn.Execute(query.ToString(), transaction: trans, commandType: CommandType.Text);

                conn.Delete(keHoach5Nam, trans);

                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<VDT_DA_DuAn> ListDuAnTheoDonViQLAndNgayLap(Guid iID_DonViQuanLyID, DateTime dNgayLap)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_duan_by_donvivangaylap.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID,
                       dNgayLap
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public bool CheckDuplicate(Guid iDDonVi, int giaiDoanTu, int giaiDoanDen, string soKeHoach, out byte error)
        {
            StringBuilder queryGiaiDoan = new StringBuilder();
            queryGiaiDoan.Append("SELECT CASE WHEN EXISTS ");
            queryGiaiDoan.AppendFormat("(SELECT 1 FROM VDT_KHV_KeHoach5Nam WHERE  iID_DonViQuanLyID = '{0}' AND iGiaiDoanTu = {1} AND iGiaiDoanDen = {2}) ", iDDonVi, giaiDoanTu, giaiDoanDen);
            queryGiaiDoan.Append("THEN 1 ELSE 0 END");

            StringBuilder querySoKeHoach = new StringBuilder();
            querySoKeHoach.AppendFormat("SELECT * FROM VDT_KHV_KeHoach5Nam WHERE sSoQuyetDinh = '{0}'", soKeHoach);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_KHV_KeHoach5Nam>(querySoKeHoach.ToString(), commandType: CommandType.Text);
                var item = conn.QueryFirstOrDefault<int>(queryGiaiDoan.ToString(), commandType: CommandType.Text);
                if (items != null)
                {
                    error = (byte)Constants.CHECK_TRUNG.SO_KE_HOACH;
                    return true;
                }
                else if (item == 1)
                {
                    error = (byte)Constants.CHECK_TRUNG.GIAI_DOAN;
                    return true;
                }
            }
            error = 2;
            return false;
        }

        #endregion

        #region Thông tin dự án - QL thông tin hợp đồng
        public IEnumerable<VDT_DA_TT_HopDong_ViewModel> GetAllVDTQuanLyTTHopDong(ref PagingInfo _paging, string sUserLogin, int iNamLamViec, string sSoHopDong = "", double? fTienHopDongTu = null,
            double? fTienHopDongDen = null, DateTime? dHopDongTuNgay = null, DateTime? dHopDongDenNgay = null, string sTenDuAn = null, string iID_MaDonVi = null, string sId_CDT = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("sUserName", sUserLogin);
                    lstParam.Add("iNamLamViec", iNamLamViec);
                    lstParam.Add("sSoHopDong", sSoHopDong);
                    lstParam.Add("fTienHopDongTu", fTienHopDongTu);
                    lstParam.Add("fTienHopDongDen", fTienHopDongDen);
                    lstParam.Add("dHopDongTuNgay", dHopDongTuNgay);
                    lstParam.Add("dHopDongDenNgay", dHopDongDenNgay);
                    lstParam.Add("sTenDuAn", sTenDuAn);
                    lstParam.Add("iID_MaDonVi", iID_MaDonVi);
                    lstParam.Add("sId_CDT", sId_CDT);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var items = conn.Query<VDT_DA_TT_HopDong_ViewModel>("proc_get_all_qltthopdong_paging", lstParam, commandType: CommandType.StoredProcedure);
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

        public IEnumerable<VDT_DM_LoaiHopDong> GetDMPhanLoaiHopDong()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT iID_LoaiHopDongID, sTenLoaiHopDong FROM VDT_DM_LoaiHopDong ORDER BY iThuTu");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_LoaiHopDong>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_DA_GoiThau> LayGoiThauTheoDuAnId(Guid iID_DuAnID)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT iID_GoiThauID, sTenGoiThau, * FROM VDT_DA_GoiThau");
            query.Append(" WHERE iID_GoiThauID NOT IN (SELECT iID_GoiThauID FROM VDT_DA_TT_HopDong WHERE iID_GoiThauID IS NOT NULL AND bActive = 1)");
            query.AppendFormat(" AND iID_DuAnID = '{0}'", iID_DuAnID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_GoiThau>(query.ToString(),
                   commandType: CommandType.Text);
                return items;
            }
        }

        public VDT_DA_GoiThau LayThongTinChiTietGoiThau(Guid iID_GoiThauID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_goithau_by_idduan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DA_GoiThau>(sql,
                   param: new
                   {
                       iID_GoiThauID
                   },
                   commandType: CommandType.Text);
                return item;
            }
        }

        public bool XoaQLThongTinHopDong(Guid iID_HopDongID)
        {
            StringBuilder query = new StringBuilder();

            query.AppendFormat("DELETE VDT_DA_TT_HopDong WHERE iID_HopDongID = '{0}' ", iID_HopDongID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var hd = conn.Get<VDT_DA_TT_HopDong>(iID_HopDongID);
                if (hd != null)
                {
                    // Trường hợp xóa bản ghi có active = 1, isGoc = 0
                    if (hd.bIsGoc.HasValue && !hd.bIsGoc.Value && hd.bActive.HasValue && hd.bActive.Value)
                        query.AppendFormat("UPDATE VDT_DA_TT_HopDong SET bActive = 1 WHERE iID_HopDongID = '{0}' ", hd.iID_ParentID);

                    var delete = conn.Execute(query.ToString(),
                    commandType: CommandType.Text);

                    return delete >= 0;
                }

                return false;
            }
        }

        public VDT_DA_TT_HopDong_ViewModel LayChiTietThongTinHopDong(Guid iID_HopDongID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT hd.*, da.sMaDuAn, da.sTenDuAn, dv.sTen AS sTenDonViQL, da.sKhoiCong, da.sKetThuc, da.fTongMucDauTu," +
                //"da.fTongMucDauTu, " +
                //" (select ISNULL(sum(fTienPheDuyet), 0) from VDT_DA_QDDauTu_ChiPhi where iID_QDDauTuID in (select iID_QDDauTuID from VDT_DA_QDDauTu where iID_DuAnID = " +
                //" (select iID_DuAnID from VDT_DA_TT_HopDong where iID_HopDongID = '{0}'))) as fTongMucDauTu, " +
                //"da.sDiaDiem, da.sSuCanThietDauTu, da.sMucTieu, ", iID_HopDongID);
                "da.sDiaDiem, da.sSuCanThietDauTu, da.sMucTieu, ", iID_HopDongID);
            query.Append("da.sDienTichSuDungDat, da.sNguonGocSuDungDat, da.sQuyMo, nhomda.sTenNhomDuAn, hinhthucQL.sTenHinhThucQuanLy, lhd.sTenLoaiHopDong, nt.sTenNhaThau, gt.fTienTrungThau ");
            query.Append("FROM VDT_DA_TT_HopDong hd ");
            query.Append("LEFT JOIN VDT_DA_DuAn da ON hd.iID_DuAnID = da.iID_DuAnID ");
            query.Append("LEFT JOIN VDT_DM_NhomDuAn nhomda ON nhomda.iID_NhomDuAnID = da.iID_NhomDuAnID ");
            query.Append("LEFT JOIN VDT_DM_HinhThucQuanLy hinhthucQL ON hinhthucQL.iID_HinhThucQuanLyID = da.iID_HinhThucQuanLyID ");
            query.Append("LEFT JOIN NS_DonVi dv ON da.iID_DonViQuanLyID = dv.iID_Ma ");
            query.Append("LEFT JOIN VDT_DM_LoaiHopDong lhd ON hd.iID_LoaiHopDongID = lhd.iID_LoaiHopDongID ");
            query.Append("LEFT JOIN VDT_DM_NhaThau nt ON hd.iID_NhaThauThucHienID = nt.iID_NhaThauID ");
            query.Append("LEFT JOIN VDT_DA_GoiThau gt ON hd.iID_GoiThauID = gt.iID_GoiThauID ");
            query.AppendFormat("WHERE hd.iID_HopDongID = '{0}'", iID_HopDongID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DA_TT_HopDong_ViewModel>(query.ToString(),
                   commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<VDT_DA_TT_HopDong> GetHopDongByThanhToanDuAnId(Guid guid)
        {
            var sql = @"SELECT *
                        FROM VDT_DA_TT_HopDong as hd
                        WHERE hd.iID_DuAnID = @iID_DuAnID AND hd.bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<VDT_DA_TT_HopDong>(
                    sql,
                    param: new
                    {
                        iID_DuAnID = guid,
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public IEnumerable<GoiThauInfoModel> GetThongTinGoiThauByDuAnIdAndHopDongId(Guid duAnId, Guid hopDongId)
        {
            var sql = @"SELECT iID_GoiThauID , iID_DuAnID , sMaGoiThau , sTenGoiThau, iID_GoiThauGocID, fTienTrungThau INTO #tmp 
                        FROM VDT_DA_GoiThau WHERE iID_DuAnID = @DuAnId AND  bActive = 1 
                        SELECT iID_GoiThauID, iID_NhaThauID, fGiaTri, iID_HopDongID, ISNULL(FGiaTriTrungThau, 0) as FGiaTriTrungThau INTO #tmpGiaTri 
                        FROM VDT_DA_HopDong_GoiThau_NhaThau WHERE iID_HopDongID = @iIdHopDong 
                        SELECT null as IdGoiThauNhaThau, NEWID() as Id, tmp.iID_GoiThauID as IIDGoiThauID, 
                        tmp.iID_DuAnID as IIDDuAnID, tmp.SMaGoiThau, tmp.STenGoiThau, tmp.iID_GoiThauGocID as IIDGoiThauGocID, dt.iID_HopDongID, 
                        ISNULL(tmp.FTienTrungThau, 0) as FTienTrungThau, dt.iID_NhaThauID as IIdNhaThauId, ISNULL(dt.fGiaTri, 0) as fGiaTriGoiThau, 
                        ISNULL(dt.FGiaTriTrungThau, 0) as FGiaTriTrungThau, 
                        (CASE WHEN dt.iID_GoiThauID IS NULL THEN CAST(0 as bit) ELSE CAST(1 as bit) END) as IsChecked 
                        FROM #tmp as tmp LEFT JOIN #tmpGiaTri as dt on tmp.iID_GoiThauID = dt.iID_GoiThauID 

                        DROP TABLE #tmpGiaTri 
                        DROP TABLE #tmp";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<GoiThauInfoModel>(
                    sql,
                    param: new
                    {
                        DuAnId = duAnId,
                        iIdHopDong = hopDongId
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public IEnumerable<GoiThauInfoModel> GetThongTinGoiThauByHopDongId(Guid hopDongId)
        {
            var sql = @"SELECT 
		                    hdnt.Id as IdGoiThauNhaThau, 
		                    hdnt.Id as Id, 
		                    hdnt.iID_GoiThauID as IIDGoiThauID, 
		                    gt.iID_DuAnID as IIDDuAnID, 
		                    gt.SMaGoiThau, 
		                    gt.STenGoiThau, 
		                    gt.iID_GoiThauGocID as IIDGoiThauGocID, 
		                    gt.FTienTrungThau, 
                            gt.iID_KHLCNhaThau,
                            hdnt.fGiaTriHD,
		                    hdnt.iID_NhaThauID as IIdNhaThauId,  
		                    hdnt.fGiaTri as FGiaTriGoiThau, 
		                    ISNULL(hdnt.FGiaTriTrungThau, 0) as FGiaTriTrungThau, 
		                    CAST(1 as bit)  as IsChecked 
	                    FROM  VDT_DA_HopDong_GoiThau_NhaThau hdnt 
	                    LEFT JOIN VDT_DA_GoiThau gt ON gt.iID_GoiThauID = hdnt.iID_GoiThauID 
	                    where hdnt.iID_HopDongID = @iIdHopDong";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<GoiThauInfoModel>(
                    sql,
                    param: new
                    {
                        iIdHopDong = hopDongId
                    },
                    commandType: CommandType.Text);
                return entity;
            }
        }

        public IEnumerable<HangMucInfoModel> GetThongTinHangMucAll(Guid iIdHopDongId, List<Guid> listGoiThauId)
        {
            var sql = @"    select Item AS Id into #tmpTable FROM f_split(@listId) 
                            select  
                         distinct gthm.iID_HangMucID as IIDHangMucID,
                         null as Id, 
                         gthm.iID_GoiThauID  as IIDGoiThauID, 
                         gthm.iID_ChiPhiID as IIDChiPhiID, 
                         null as IdGoiThauNhaThau, 
                         null as IIDHopDongID, 
                          
                         null as IIDNhaThauID, 
                         cast(0 as bit) as IsChecked, 
                         null as STenChiPhi, 
                         (CAST(0 AS float)) as FTienGoiThau, 
                         null as IThuTu, 
                         null as IdChiPhiDuAnParent, 
                         dmhm.iID_ParentID as HangMucParentId, 
                         dmhm.smaOrder AS MaOrDer, 
                         dmhm.sTenHangMuc as STenHangMuc, 
                         gthm.fTienGoiThau as FGiaTriDuocDuyet, 
                         gthm.fTienGoiThau as FGiaTriConLai, " +
                      // "isnull(cast(case when parentId.iID_ParentID is not null or dthm.iID_ParentID is null then 1 else 0 end as bit),0)  as IsHangCha INTO #tmp "+
                      "isnull(cast(case when parentId.iID_ParentID is not null or dmhm.iID_ParentID is null then 1 else 0 end as bit),0)  as IsHangCha " +
                      "from VDT_DA_GoiThau_HangMuc gthm " +

                      // "inner join VDT_DA_DuToan_DM_HangMuc dthm ON dthm.Id = gthm.iID_HangMucID " +
                      "inner join VDT_DA_QDDauTu_DM_HangMuc dmhm ON dmhm.iID_QDDauTu_DM_HangMucID = gthm.iID_HangMucID  " +
                      "inner join VDT_DA_GoiThau gt on gt.iID_DuAnID = dmhm.iID_DuAnID " +
                      "inner join #tmpTable tbl ON tbl.Id = gthm.iID_GoiThauID " +
                      "left join " +
                      "(" +
                      "select distinct tb2.iID_ParentID from VDT_DA_GoiThau_HangMuc tb1 " +
                      "inner join VDT_DA_DuToan_DM_HangMuc tb2 ON tb1.iID_HangMucID = tb2.Id  and tb2.iID_ParentID is not null " +

                      ") as parentId ON parentId.iID_ParentID = gthm.iID_HangMucID  " +
                      "order by MaOrDer ";

            if (iIdHopDongId != Guid.Empty)
            {
                sql += "SELECT tbl.iID_GoiThauID, dt.iID_ChiPhiID, dt.iID_HangMucID INTO #tmpExclude " +
                       "FROM VDT_DA_HopDong_GoiThau_NhaThau as tbl " +
                       "INNER JOIN VDT_DA_HopDong_GoiThau_HangMuc as dt on tbl.Id = dt.iID_HopDongGoiThauNhaThauID " +
                       "INNER JOIN #tmpTable as gt on tbl.iID_GoiThauID = gt.Id " +
                       (iIdHopDongId != Guid.Empty ? "WHERE tbl.iID_HopDongID <> @iIdHopDongId" : "") +

                       "SELECT distinct tmp.*" +
                       "FROM #tmp as tmp " +
                       "LEFT JOIN #tmpExclude as ex on tmp.IIDGoiThauID = ex.iID_GoiThauID AND tmp.IIDChiPhiID = ex.iID_ChiPhiID AND tmp.IIDHangMucID = ex.iID_HangMucID " +
                       "WHERE ex.iID_GoiThauID IS NULL OR ex.iID_ChiPhiID IS NULL OR ex.iID_HangMucID IS NULL " +

                       "DROP TABLE #tmp " +
                       "DROP TABLE #tmpExclude  " +
                       "  DROP TABLE #tmpTable";
            }
            else
            {
                sql +=
                       "\nDROP TABLE #tmpTable";
            }



            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<HangMucInfoModel>(
                    sql,
                    param: new
                    {
                        iIdHopDongId = iIdHopDongId,
                        listId = string.Join(",", listGoiThauId.Select(n => n.ToString()).ToList())
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public void DeleteHopDongDetail(Guid iIdHopDongID)
        {
            //using (var conn = _connectionFactory.GetConnection())
            //{
            //    var lstParam = new DynamicParameters();
            //    lstParam.Add("@iIdHopDongID", iIdHopDongId);
            //    var items = conn.Query<HangMucInfoModel>("sp_vdt_delete_hopdong_detail", lstParam, commandType: CommandType.StoredProcedure);
            //}
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Execute(@"DELETE dt FROM VDT_DA_HopDong_GoiThau_NhaThau as tbl 
                               INNER JOIN VDT_DA_HopDong_GoiThau_HangMuc as dt on tbl.Id = dt.iID_HopDongGoiThauNhaThauID 
                               WHERE tbl.iID_HopDongID = @iIdHopDongID 

                               DELETE gtcp FROM VDT_DA_HopDong_GoiThau_ChiPhi gtcp 
	                           INNER JOIN VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.id = gtcp.iID_HopDongGoiThauNhaThauID 
	                           where gtnt.iID_HopDongID = @iIdHopDongID 

                               DELETE VDT_DA_HopDong_GoiThau_NhaThau WHERE iID_HopDongID = @iIdHopDongID",
                    param: new
                    {
                        iIdHopDongID
                    }, commandType: CommandType.Text);

                conn.Delete<VDT_QT_BCQuyetToanNienDo>(iIdHopDongID);
                //return true;
            }
        }

        public IEnumerable<HangMucInfoModel> GetThongTinHangMucAllByHopDongId(Guid iIdHopDongId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@iIdHopDong", iIdHopDongId);
                var items = conn.Query<HangMucInfoModel>("sp_vdt_get_hangmuc_goithau_hopdong_by_hopdong", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<HangMucInfoModel> GetThongTinDieuChinhHangMucAllByHopDongId(Guid iIdHopDongId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@iIdHopDong", iIdHopDongId);
                var items = conn.Query<HangMucInfoModel>("sp_vdt_get_hangmuc_goithau_hopdong_by_hopdong_dieuchinh", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<ChiPhiInfoModel> GetThongTinChiPhiByGoiThauId(Guid goiThauId)
        {
            // query bi loi lay ca chi phi cha va chi phi con khi ben khlcnt chi luu chi phi con vao db
            /*var sql = @";WITH  ChiPhiTreeCTE 
                        AS 
                        ( select 
                        gtcp.iID_GoiThauID as IIDGoiThauID, 
		                gtcp.iID_ChiPhiID as IIDChiPhiID, 
		                null as IIDHopDongID, 
		                null as IIDHangMucID, 
		                null as IIDNhaThauID, 
		                cast(0 as bit) as IsChecked, 
		                dacp.sTenChiPhi as STenChiPhi, 
		                gtcp.fTienGoiThau as FGiaTriDuocDuyet , 
		                dacp.iThuTu as IThuTu, 
		                dacp.iID_ChiPhi_Parent as IdChiPhiDuAnParent, 
		                null as HangMucParentId, 
		                CAST((dacp.iID_DuAn_ChiPhi) AS VARCHAR(MAX)) AS MaOrDer 
		                from VDT_DA_GoiThau_ChiPhi gtcp 
		                inner join VDT_DM_DuAn_ChiPhi dacp ON gtcp.iID_ChiPhiID = dacp.iID_DuAn_ChiPhi 
		                where gtcp.iID_GoiThauID = @iIdGoiThau AND  
		                dacp.iID_ChiPhi_Parent is null 
	                union all 
	                select 
		                gtcp2.iID_GoiThauID as IIDGoiThauID, 
		                gtcp2.iID_ChiPhiID as IIDChiPhiID, 
		                null as IIDHopDongID, 
		                null as IIDHangMucID, 
		                null as IIDNhaThauID, 
		                cast(0 as bit) as IsChecked, 
		                dacp2.sTenChiPhi as STenChiPhi, 
		                gtcp2.fTienGoiThau as FGiaTriDuocDuyet, 
		                dacp2.iThuTu as IThuTu, 
		                dacp2.iID_ChiPhi_Parent as IdChiPhiDuAnParent, 
		                null as HangMucParentId, 
		                CAST((M.MaOrDer + '-' + CAST(dacp2.iID_DuAn_ChiPhi AS VARCHAR(max)) ) AS VARCHAR(MAX)) AS MaOrDer 
	                from VDT_DA_GoiThau_ChiPhi gtcp2 
		                inner join VDT_DM_DuAn_ChiPhi dacp2 ON gtcp2.iID_ChiPhiID = dacp2.iID_DuAn_ChiPhi 
			                inner join ChiPhiTreeCTE M ON dacp2.iID_ChiPhi_Parent = M.IIDChiPhiID 
			                where gtcp2.iID_GoiThauID = @iIdGoiThau 
                            ) 
                            select tbl.* , 
                            null as Id, 
                            null as IdGoiThauNhaThau, 
                            null as STenHangMuc, 
                            null as FTienGoiThau, 
                            tbl.FGiaTriDuocDuyet as FGiaTriConLai, 
                            isnull(cast(case when parentId.iID_ChiPhi_Parent is not null or tbl.IdChiPhiDuAnParent is null then 1 else 0 end as bit),0)  as IsHangCha 
                            from ChiPhiTreeCTE tbl 
                            left join 
		                            ( 
			                            select distinct tb2.iID_ChiPhi_Parent from VDT_DA_GoiThau_ChiPhi tb1 
			                            inner join VDT_DM_DuAn_ChiPhi tb2 ON tb1.iID_ChiPhiID = tb2.iID_DuAn_ChiPhi AND tb1.iID_GoiThauID = @iIdGoiThau 
			                            and tb2.iID_ChiPhi_Parent is not null 
		                            ) as parentId ON parentId.iID_ChiPhi_Parent = tbl.IIDChiPhiID 
		                            order by IThuTu, MaOrDer";
            */
            var sql = @";select 
                        gtcp.iID_GoiThauID as IIDGoiThauID, 
		                gtcp.iID_ChiPhiID as IIDChiPhiID, 
		                null as IIDHopDongID, 
		                null as IIDHangMucID, 
		                null as IIDNhaThauID, 
		                cast(0 as bit) as IsChecked, 
		                dacp.sTenChiPhi as STenChiPhi, 
		                gtcp.fTienGoiThau as FGiaTriDuocDuyet , 
		                dacp.iThuTu as IThuTu, 
		                dacp.iID_ChiPhi_Parent as IdChiPhiDuAnParent, 
		                null as HangMucParentId, 
		                CAST((dacp.iID_DuAn_ChiPhi) AS VARCHAR(MAX)) AS MaOrDer 
		                from VDT_DA_GoiThau_ChiPhi gtcp 
		                inner join VDT_DM_DuAn_ChiPhi dacp ON gtcp.iID_ChiPhiID = dacp.iID_DuAn_ChiPhi 
		                where gtcp.iID_GoiThauID = @iIdGoiThau AND  
		                dacp.iID_ChiPhi_Parent is null
		                order by IThuTu, MaOrDer";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<ChiPhiInfoModel>(
                    sql,
                    param: new
                    {
                        iIdGoiThau = goiThauId
                    },
                    commandType: CommandType.Text);
                return entity;
            }
        }

        public IEnumerable<ChiPhiInfoModel> GetThongTinChiPhiByHopDongId(Guid hopDongId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@iIdHopDong", hopDongId);
                var items = conn.Query<ChiPhiInfoModel>("sp_vdt_get_chiphi_goithau_hopdong_by_hopdong", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<VDT_QDDT_KHLCNhaThau> GetThongTinKHLCNhaThauByIdKHLC(Guid iD_KHLCNThau)
        {

            var sql = "	select nt.iID_DuToanID, nt.iID_QDDauTuID, nt.iID_ChuTruongDauTuID FROM VDT_QDDT_KHLCNhaThau as nt" +
                " where nt.id = @iD_KHLCNThau";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Query<VDT_QDDT_KHLCNhaThau>(
                    sql,
                    param: new
                    {
                        iD_KHLCNThau = iD_KHLCNThau
                    },
                    commandType: CommandType.Text);
                return entity;
            }
        }

        public VDT_DA_TT_HopDong_ViewModel LayThongTinChiTietDuAnTheoId(Guid iID_DuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_thongtinduan_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<VDT_DA_TT_HopDong_ViewModel>(sql, param: new
                {
                    iID_DuAnID
                },
                commandType: CommandType.Text);
                return items;
            }
        }

        public HopDongDetailModel GetDetailHopDongByDuAnId(Guid iID_DuAnID, Guid iID_HopDongID, DateTime dNgayDeNghi, int iNamKeHoach, int iID_NguonVonID, Guid iID_LoaiNguonVonID, Guid iID_NganhID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_detailhopdong_in_thanhtoansreen.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirst<HopDongDetailModel>(
                    sql,
                    param: new
                    {
                        iID_HopDongID,
                        dNgayDeNghi,
                        iNamKeHoach,
                        iID_NguonVonID,
                        iID_LoaiNguonVonID,
                        iID_DuAnID,
                        iID_NganhID
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        /// <summary>
        /// lay gia tri truoc dieu chinh cua hop dong
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dNgayHopDong"></param>
        /// <returns></returns>
        public double? LayGiaTriTruocDieuChinhHopDong(Guid id, DateTime dNgayHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var hd = conn.Get<VDT_DA_TT_HopDong>(id);
                if (hd == null)
                    return null;
                if (hd.bIsGoc.HasValue)
                {
                    if (hd.bIsGoc.Value)
                        return hd.fTienHopDong;

                    // bIsGoc = false
                    var sql = "SELECT ISNULL(SUM(fTienHopDong), 0)  FROM VDT_DA_TT_HopDong WHERE iID_HopDongGocID = @iID_HopDongGocID AND dNgayHopDong <= @dNgayHopDong;";
                    var item = conn.QueryFirstOrDefault<double>(sql,
                        param: new
                        {
                            iID_HopDongGocID = hd.iID_HopDongGocID,
                            dNgayHopDong
                        },
                        commandType: CommandType.Text
                    );
                    return item;
                }
                return null;
            }
        }

        public string GetSoTaiKhoanNhaThauByIdNhaThau(Guid? iID_NhaThauID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {

                    var sql = "SELECT sSoTaiKhoan FROM VDT_DM_NhaThau WHERE iID_NhaThauID = @iID_NhaThauID;";
                    var item = conn.QueryFirstOrDefault<string>(sql,
                        param: new
                        {
                            iID_NhaThauID = iID_NhaThauID,

                        },
                        commandType: CommandType.Text
                    );
                    return string.IsNullOrEmpty(item) ? string.Empty : item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return string.Empty;
        }

        #endregion

        #region Báo cáo quyết toán niên độ 
        public IEnumerable<VdtQtBcQuyetToanNienDoViewModel> GetAllBaoCaoQuyetToanPaging(ref PagingInfo _paging, string iIdMaDonViQuanLy, int? iIdNguonVon, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, int? iNamKeHoach, string txtSoChungTu)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                if (iIdMaDonViQuanLy == "")
                {
                    iIdMaDonViQuanLy = null;

                }
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdMaDonViQuanLy", iIdMaDonViQuanLy);
                lstParam.Add("iIdNguonVon", iIdNguonVon);
                lstParam.Add("dNgayDeNghiFrom", dNgayDeNghiFrom);
                lstParam.Add("dNgayDeNghiTo", dNgayDeNghiTo);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                lstParam.Add("txtSoChungTu", txtSoChungTu);

                var items = conn.Query<VdtQtBcQuyetToanNienDoViewModel>("proc_get_all_baocaoquyettoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_QT_BCQuyetToanNienDo GetBaoCaoQuyetToanById(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Get<VDT_QT_BCQuyetToanNienDo>(iId);
            }
        }
        public VdtQtBcQuyetToanNienDoViewModel GetBCQuyetToanNienDoById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VdtQtBcQuyetToanNienDoViewModel>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NS_NguonNganSach> GetNguonNSList()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT * FROM NS_NguonNganSach");
                var item = conn.Query<NS_NguonNganSach>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NS_DonVi> GetDonviList(int? iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT * FROM NS_DonVi where NS_DonVi.iNamLamViec_DonVi = " + iNamLamViec);
                var item = conn.Query<NS_DonVi>(query.ToString(), commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveTongHop(VDT_QT_BCQuyetToanNienDo data, string userName, string listId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = new VDT_QT_BCQuyetToanNienDo();
                    entity.MapFrom(data);
                    entity.sTongHop = listId;
                    conn.Insert(entity, trans);

                    foreach (var id in listId.Split(','))
                    {
                        var entityCon = conn.Get<VDT_QT_BCQuyetToanNienDo>(id, trans);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = entity.iID_BCQuyetToanNienDoID;
                        conn.Update(entityCon, trans);
                    }
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }
        public bool InsertVdtQtBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Insert(data);
            }
            return true;
        }

        public bool UpdateVdtQtBcQuyetToanNienDo(VDT_QT_BCQuyetToanNienDo data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Update(data);
            }
            return true;
        }

        public bool UpdateQuyetToanNienDoChiTiet(Guid iId, List<VDT_QT_BCQuyetToanNienDo_ChiTiet_01> lstData)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Execute("DELETE VDT_QT_BCQuyetToanNienDo_ChiTiet_01 WHERE iID_BCQuyetToanNienDo = @iId",
                    param: new
                    {
                        iId
                    }, commandType: CommandType.Text);
                conn.Insert<VDT_QT_BCQuyetToanNienDo_ChiTiet_01>(lstData);
            }

            return true;
        }

        public bool UpdateQuyetToanNienDoChiTietPhanTich(Guid iId, List<VDT_QT_BCQuyetToanNienDo_PhanTich> lstData)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Execute("DELETE VDT_QT_BCQuyetToanNienDo_PhanTich WHERE iID_BCQuyetToanNienDo = @iId",
                    param: new
                    {
                        iId
                    }, commandType: CommandType.Text);
                conn.Insert<VDT_QT_BCQuyetToanNienDo_PhanTich>(lstData);
            }
            return true;
        }

        public bool DeleteBaoCaoQuyetToan(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Execute(@"DELETE VDT_QT_BCQuyetToanNienDo_ChiTiet_01 WHERE iID_BCQuyetToanNienDo = @iId;
                                DELETE VDT_QT_BCQuyetToanNienDo_PhanTich WHERE iID_BCQuyetToanNienDo = @iId;
                                DELETE VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iId",
                    param: new
                    {
                        iId
                    }, commandType: CommandType.Text);

                //conn.Delete<VDT_QT_BCQuyetToanNienDo>(iId);
                return true;
            }
        }

        public List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNam(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdMaDonVi", iIdMaDonVi);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("iIdNguonVon", iIdNguonVon);

                    var items = conn.Query<BcquyetToanNienDoVonNamChiTietViewModel>("sp_vdt_get_baocaodquyettoanniendo_vonnam", lstParam, commandType: CommandType.StoredProcedure).ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {

            }
            return new List<BcquyetToanNienDoVonNamChiTietViewModel>();
        }

        public List<BcquyetToanNienDoVonNamChiTietViewModel> GetQuyetToanVonNamById(Guid? iIDQuyetToan)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdBCQuyetToanNienDo", iIDQuyetToan);

                    var items = conn.Query<BcquyetToanNienDoVonNamChiTietViewModel>("sp_vdt_get_baocaodquyettoanniendo_vonnam_by_id", lstParam, commandType: CommandType.StoredProcedure).ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {

            }
            return new List<BcquyetToanNienDoVonNamChiTietViewModel>();
        }

        public List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTich(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdMaDonVi", iIdMaDonVi);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIdNguonVon", iIdNguonVon);

                var items = conn.Query<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>("sp_vdt_qt_baocaoquyettoanniendo_phantich", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }

        public List<BcquyetToanNienDoVonNamPhanTichChiTietViewModel> GetQuyetToanVonNam_PhanTichById(Guid? iIDQuyetToan)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIDQuyetToanId", iIDQuyetToan);

                var items = conn.Query<BcquyetToanNienDoVonNamPhanTichChiTietViewModel>("sp_vdt_qt_baocaoquyettoanniendo_phantich_by_id", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }

        public List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUng(string iIdMaDonVi, int iNamKeHoach, int iIdNguonVon)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdMaDonVi", iIdMaDonVi);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIdNguonVon", iIdNguonVon);

                var items = conn.Query<BcquyetToanNienDoVonUngChiTietViewModel>("sp_vdt_get_baocaodquyettoanniendo_vonung", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }
        public IEnumerable<VdtQtBcQuyetToanNienDoViewModel> GetListTongHopQuyetToanNienDo(string sSodenghi, DateTime? dNgaydenghi, int? iLoaiThanhtoan, Guid? iDonvi, int? iNam, string sTennguonvon)
        {
            if (iDonvi == Guid.Empty)
            {
                iDonvi = null;
            }
            var sSoDeNghi = sSodenghi;
            var dNgayDeNghi = dNgaydenghi;
            var iLoaiThanhToan = iLoaiThanhtoan;
            //var sThanhToan = "";
            var iID_DonViQuanLyID = iDonvi;
            var iNamKeHoach = iNam;
            var iID_NguonVonID = sTennguonvon;
            var sql =
                @"
                
                SELECT DISTINCT qtnd.*,ns.sTen as sTenNguonVon,dv.sTen as sTenDonVi
                INTO #tmp
                from VDT_QT_BCQuyetToanNienDo as qtnd  
                left join NS_NguonNganSach ns on qtnd.iID_NguonVonID = ns.iID_MaNguonNganSach
				left join NS_DonVi dv on qtnd.iID_DonViQuanLyID = dv.iID_Ma
                 WHERE
				(ISNULL(@sSoDeNghi,'') = '' OR qtnd.sSoDeNghi like CONCAT(N'%', @sSoDeNghi, N'%')) 
				 AND (ISNULL(@iNamKeHoach,0) = 0 OR iNamKeHoach = @iNamKeHoach)
				AND (@dNgayDeNghi is   null or qtnd.dNgayDeNghi = @dNgayDeNghi)
				AND (@iLoaiThanhToan is   null or qtnd.iLoaiThanhToan = @iLoaiThanhToan)
				AND (@iID_DonViQuanLyID IS NULL OR qtnd.iID_DonViQuanLyID = @iID_DonViQuanLyID)
				AND (@iID_NguonVonID IS NULL OR qtnd.iID_NguonVonID = @iID_NguonVonID)
                ;
                WITH cte( iID_BCQuyetToanNienDoID,sSoDeNghi, dNgayDeNghi,iLoaiThanhToan, iNamKeHoach,iID_DonViQuanLyID,iID_NguonVonID ,iID_TongHopID,sTongHop)
                AS
                (
	                SELECT 
					  lct.iID_BCQuyetToanNienDoID
					, lct.sSoDeNghi
					, lct.dNgayDeNghi 
					, lct.iLoaiThanhToan 
					, lct.iNamKeHoach
					, lct.iID_DonViQuanLyID
					, lct.iID_NguonVonID
					, lct.iID_TongHopID
					, lct.sTongHop
	                FROM VDT_QT_BCQuyetToanNienDo lct , #tmp tmp
	                WHERE lct.iID_BCQuyetToanNienDoID  = tmp.iID_TongHopID
	                UNION ALL
	                SELECT 
					  cd.iID_BCQuyetToanNienDoID
					, cd.sSoDeNghi
					, cd.dNgayDeNghi 
					, cd.iLoaiThanhToan 
					, cd.iNamKeHoach
					, cd.iID_DonViQuanLyID
					, cd.iID_NguonVonID
					, cd.iID_TongHopID
					, cd.sTongHop
	                FROM cte as NCCQ, #tmp as cd
	                WHERE cd.iID_TongHopID = NCCQ.iID_BCQuyetToanNienDoID
                )
                SELECT DISTINCT 
				  cte.iID_BCQuyetToanNienDoID
				, cte.sSoDeNghi
				, dNgayDeNghi
				, iLoaiThanhToan
				, iNamKeHoach
				, ns.sTen  as sTenNguonVon
				, dv.sTen as sTenDonVi
				, iID_TongHopID
				, sTongHop
				 INTO #db
                 FROM cte 
                 left join NS_DonVi dv on cte.iID_DonViQuanLyID = dv.iID_Ma 
				 left join NS_NguonNganSach ns on cte.iID_NguonVonID = ns.iID_MaNguonNganSach
                UNION ALL
                SELECT DISTINCT 
				  qtnd.iID_BCQuyetToanNienDoID
				, qtnd.sSoDeNghi
				, qtnd.dNgayDeNghi
				, qtnd.iLoaiThanhToan
				, qtnd.iNamKeHoach
				, ns.sTen  as sTenNguonVon
				, dv.sTen as sTenDonVi
				, qtnd.iID_TongHopID
				, qtnd.sTongHop
				
                from VDT_QT_BCQuyetToanNienDo as qtnd  
                inner join VDT_QT_BCQuyetToanNienDo as cd on qtnd.iID_BCQuyetToanNienDoID = cd.iID_TongHopID
                left join NS_DonVi dv on qtnd.iID_DonViQuanLyID = dv.iID_Ma
				left join NS_NguonNganSach ns on qtnd.iID_NguonVonID = ns.iID_MaNguonNganSach
                where (ISNULL(@sSoDeNghi,'') = '' or qtnd.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%'))
	            and (@iID_DonViQuanLyID is null or qtnd.iID_DonViQuanLyID = @iID_DonViQuanLyID) 
				 and (@iID_NguonVonID is null or qtnd.iID_NguonVonID = @iID_NguonVonID) 
	            and (ISNULL(@iNamKeHoach,'') = '' or qtnd.iNamKeHoach = @iNamKeHoach) 
	            and (ISNULL(@iLoaiThanhToan,'') = '' or qtnd.iLoaiThanhToan = @iLoaiThanhToan) 
				and  qtnd.iID_TongHopID is null and ( qtnd.sTongHop is null or qtnd.sTongHop = '')
                Order by cte.iID_TongHopID

                Select db.* ,ROW_NUMBER() OVER (ORDER BY db.iID_TongHopID) AS sSTT from  #db  db
                DROP TABLE #tmp
                DROP TABLE #db

";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VdtQtBcQuyetToanNienDoViewModel>(sql,
                    param: new
                    {
                        sSoDeNghi,
                        dNgayDeNghi,
                        iLoaiThanhToan,
                        iID_DonViQuanLyID,
                        iNamKeHoach,
                        iID_NguonVonID,
                        sTennguonvon,

                    },
                     commandType: CommandType.Text

                 );
                return items;
            }
        }
        public bool DeleteQuyetToanNienDoTongHop(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DELETE VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<VDT_QT_BCQuyetToanNienDo>(iId);
                if (entity.sTongHop != null)
                {
                    foreach (var id in entity.sTongHop.Split(','))
                    {
                        var entityCon = conn.Get<VDT_QT_BCQuyetToanNienDo>(id);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = null;
                        conn.Update(entityCon);
                    }
                }

                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                return r > 0;
            }
        }

        public List<BcquyetToanNienDoVonUngChiTietViewModel> GetQuyetToanVonUngById(Guid? iIDQuyetToan)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdQuyetToanId", iIDQuyetToan);

                var items = conn.Query<BcquyetToanNienDoVonUngChiTietViewModel>("sp_vdt_get_quyettoanniendovonung_by_parentid", lstParam, commandType: CommandType.StoredProcedure).ToList();
                return items;
            }
        }
        public VdtQtBcQuyetToanNienDoViewModel GetQuyetToanNienDoById(Guid iId)
        {
            var sql = "select * FROM VDT_QT_BCQuyetToanNienDo  where iID_BCQuyetToanNienDoID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VdtQtBcQuyetToanNienDoViewModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;

            }
        }
        public NS_NguonNganSach GetTenNguonVonByID(int iId)
        {
            var sql = "select * FROM NS_NguonNganSach  where iID_MaNguonNganSach = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NS_NguonNganSach>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public VdtQtBcQuyetToanNienDoReturnData SaveQTNDTongHop(VDT_QT_BCQuyetToanNienDo data)
        {
            VdtQtBcQuyetToanNienDoReturnData dt = new VdtQtBcQuyetToanNienDoReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<VDT_QT_BCQuyetToanNienDo> item = new List<VDT_QT_BCQuyetToanNienDo>();
                    StringBuilder query = new StringBuilder();
                    query.Append("select * from VDT_QT_BCQuyetToanNienDo");
                    item = conn.Query<VDT_QT_BCQuyetToanNienDo>(query.ToString(), commandType: CommandType.Text).ToList();
                    var trans = conn.BeginTransaction();
                    if (data.iID_BCQuyetToanNienDoID == null || data.iID_BCQuyetToanNienDoID == Guid.Empty)
                    {
                        var entity = new VDT_QT_BCQuyetToanNienDo();
                        entity.MapFrom(data);
                        if (item.Where(x => x.sSoDeNghi.ToLower() == entity.sSoDeNghi.ToLower()).Any())
                        {
                            dt.IsReturn = false;
                            dt.errorMess = "Đã tồn tại số đề nghị!";
                            return dt;
                        }
                        conn.Insert<VDT_QT_BCQuyetToanNienDo>(data, trans);
                        dt.VDT_QT_BCQuyetToanNienDoData = entity;
                    }
                    else
                    {
                        var entity = new VDT_QT_BCQuyetToanNienDo();
                        entity.MapFrom(data);
                        if (item.Where(x => x.sSoDeNghi.ToLower() == entity.sSoDeNghi.ToLower()).Any())
                        {
                            dt.IsReturn = false;
                            dt.errorMess = "Đã tồn tại  số đề nghị!";
                            return dt;
                        }
                        else
                        {
                            conn.Update<VDT_QT_BCQuyetToanNienDo>(data, trans);
                        }
                    }

                    trans.Commit();
                    dt.IsReturn = true;
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return dt;
        }

        #endregion

        #region Quyết toán niên độ của trợ lý : LongDT
        public IEnumerable<VDTQTDeNghiQuyetToanNienDoTroLyViewModel> GetAllQuyetToanNienDoPaging(ref PagingInfo _paging, Guid? iIdDonViQuanLy, int? iIdNguonVon, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, int? iNamKeHoach)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iIdDonViQuanLy", iIdDonViQuanLy);
                lstParam.Add("iIdNguonVon", iIdNguonVon);
                lstParam.Add("dNgayDeNghiFrom", dNgayDeNghiFrom);
                lstParam.Add("dNgayDeNghiTo", dNgayDeNghiTo);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTQTDeNghiQuyetToanNienDoTroLyViewModel>("proc_get_all_quyettoanniendotroly_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_QT_DeNghiQuyetToanNienDo_TroLy GetQuyetToanNienDoTroLyById(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Get<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(iId);
            }
        }

        public decimal GetChiTieuNganSachNamInQTNienDoByDuAn(Guid iIdDuAnId, Guid iIdNganh, int iNamKeHoach, DateTime dNgayQuyetDinh, Guid iIdDonViQuanLyId, int iIdNguonVonId)
        {
            string sQuery = @"SELECT ISNULL(SUM(ISNULL(dt.fGiaTrPhanBo,0)) ,0)
                                FROM VDT_KHV_PhanBoVon as tbl
                                INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
											                                AND dt.iID_DuAnID = @iIdDuAnId
											                                AND iID_NganhID = @iIdNganh
                                WHERE tbl.iNamKeHoach = @iNamKeHoach
	                                AND tbl.dNgayQuyetDinh <= @dNgayQuyetDinh
	                                AND tbl.iID_DonViQuanLyID = @iIdDonViQuanLyId
	                                AND tbl.iID_NguonVonID = @iIdNguonVonId";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.QueryFirstOrDefault<decimal>(sQuery.ToString(),
                    param: new
                    {
                        iIdDuAnId,
                        iIdNganh,
                        iNamKeHoach,
                        dNgayQuyetDinh,
                        iIdDonViQuanLyId,
                        iIdNguonVonId
                    },
                   commandType: CommandType.Text);
            }
        }

        public decimal GetCapPhatVonNamNayInQTNienDoByDuAn(Guid iIdDuAnId, Guid iIdNganh, int iNamKeHoach, DateTime dNgayQuyetDinh, Guid iIdDonViQuanLyId, int iIdNguonVonId, Guid iIdLoaiNguonVon)
        {
            string sQuery = @"SELECT ISNULL(SUM(ISNULL(dt.fGiaTriThanhToan,0)),0)
                                FROM VDT_TT_DeNghiThanhToan as tbl
                                INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet  as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
											                                AND dt.iID_DuAnID = @iIdDuAnId
											                                AND iID_NganhID = @iIdNganh
                                WHERE tbl.iNamKeHoach = @iNamKeHoach
	                                AND tbl.dNgayDeNghi <= @dNgayQuyetDinh
	                                AND tbl.iID_DonViQuanLyID = @iIdDonViQuanLyId
	                                AND tbl.iID_NguonVonID = @iIdNguonVonId
	                                AND tbl.iID_LoaiNguonVonID = @iIdLoaiNguonVon";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.QueryFirstOrDefault<decimal>(sQuery.ToString(),
                    param: new
                    {
                        iIdDuAnId,
                        iIdNganh,
                        iNamKeHoach,
                        dNgayQuyetDinh,
                        iIdDonViQuanLyId,
                        iIdNguonVonId,
                        iIdLoaiNguonVon
                    },
                   commandType: CommandType.Text);
            }
        }

        public decimal GetTongMucDauTuInQTNienDoByDuAn(Guid iIdDuAnId, DateTime dNgayQuyetDinh, int iIdNguonVonId)
        {
            string sQuery = @"SELECT ISNULL(SUM(ISNULL(fTongMucDauTuPheDuyet,0)),0)
                                FROM
                                (SELECT SUM(fTongMucDauTuPheDuyet * ISNULL(fTiGia,1) * ISNULL(fTiGiaDonVi,1)) as fTongMucDauTuPheDuyet
                                FROM VDT_DA_QDDauTu
                                WHERE dNgayQuyetDinh <= @dNgayLap
	                                AND iID_DuAnID = @iIdDuAnId
	                                AND ISNULL(fTongMucDauTuPheDuyet,0) <> 0
                                UNION
                                SELECT SUM(ISNULL(dt.fTienPheDuyet,0) * ISNULL(dt.fTiGia,1) * ISNULL(dt.fTiGiaDonVi,1) ) as fTongMucDauTuPheDuyet
                                FROM VDT_DA_QDDauTu as tbl
                                INNER JOIN VDT_DA_QDDauTu_NguonVon as dt on tbl.iID_QDDauTuID = dt.iID_QDDauTuID
                                WHERE ISNULL(tbl.fTongMucDauTuPheDuyet,0) = 0 
	                                AND tbl.iID_DuAnID = @iIdDuAnId
	                                AND tbl.dNgayQuyetDinh <= @dNgayLap
	                                AND dt.iID_NguonVonID = @iIdNguonVonId) as tbl";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.QueryFirstOrDefault<decimal>(sQuery.ToString(),
                    param: new
                    {
                        iIdDuAnId,
                        dNgayLap = dNgayQuyetDinh,
                        iIdNguonVonId
                    },
                   commandType: CommandType.Text);
            }
        }

        public IEnumerable<DeNghiQuyetToanNienDoTroLyChiTietViewModel> GetQuyetToanNienDoTroLyChiTietInGridUpdate(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_quyettoanniendotrolychitiet_gridview.sql");



            using (var conn = _connectionFactory.GetConnection())
            {
                var parentData = conn.Get<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(iId);

                var items = conn.Query<DeNghiQuyetToanNienDoTroLyChiTietViewModel>(sql,
                    param: new
                    {
                        iNamKeHoach = parentData.iNamKeHoach,
                        dNgayQuyetDinh = parentData.dNgayDeNghi,
                        iIdDonViQuanLyId = parentData.iID_DonViDeNghiID,
                        iIdNguonVonId = parentData.iID_NguonVonID,
                        iIdLoaiNguonVon = parentData.iID_LoaiNguonVonID,
                        dNgayLap = parentData.dNgayDeNghi,
                        iID = iId
                    },
                    commandType: CommandType.Text);

                return items;
            }

        }

        public bool InsertQuyetToanNienDoTroLy(ref VDT_QT_DeNghiQuyetToanNienDo_TroLy data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Insert(data);
            }
            return true;
        }

        public bool InsertQuyetToanNienDoTroLyChiTiet(List<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstData)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Insert<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>(lstData);
            }
            return true;
        }

        public bool UpdateQuyetToanNienDoTroLy(VDT_QT_DeNghiQuyetToanNienDo_TroLy data)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Update(data);
            }
            return true;
        }

        public bool RemoveQuyetToanNienDoTroLy(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var data = conn.Get<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(iId);
                conn.Delete(data);
            }
            return true;
        }
        public bool DeleteQuyetToanNienDoTroLy(Guid iId, string userLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var data = conn.Get<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(iId);
                data.dDateDelete = DateTime.Now;
                data.sUserDelete = userLogin;
                conn.Update(data);
            }
            return true;
        }

        public bool RemoveQuyetToanNienDoTroLyChiTietByParentId(Guid iId)
        {

            string sql = @"SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet WHERE iID_DeNghiQuyetToanNienDoID = @iId";
            IEnumerable<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet> lstData;
            using (var conn = _connectionFactory.GetConnection())
            {
                lstData = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>(sql,
                    param: new
                    {
                        iId
                    },
                    commandType: CommandType.Text);
            }
            if (lstData != null && lstData.Any())
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Delete<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>(lstData);
                }
            }
            return true;
        }

        public bool RemoveQuyetToanNienDoTroLyChiTiet(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var data = conn.Get<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>(iId);
                conn.Delete(data);
            }
            return true;
        }

        #endregion

        #region Vốn đầu tư - Quyết toán - Đề nghị quyết toán

        public IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> GetAllDeNghiQuyetToanPaging(ref PagingInfo _paging, string sSoBaoCao = "", decimal? sGiaDeNghiTu = null, decimal? sGiaDeNghiDen = null, string sTenDuAn = "", string sMaDuAn = "", string sUserName = "")
        {
            if (string.IsNullOrEmpty(sSoBaoCao))
            {
                sSoBaoCao = null;
            }
            if (string.IsNullOrEmpty(sTenDuAn))
            {
                sTenDuAn = null;
            }
            if (string.IsNullOrEmpty(sMaDuAn))
            {
                sMaDuAn = null;
            }
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sSoBaoCao", sSoBaoCao);
                lstParam.Add("sGiaDeNghiTu", sGiaDeNghiTu);
                lstParam.Add("sGiaDeNghiDen", sGiaDeNghiDen);
                lstParam.Add("sTenDuAn", sTenDuAn);
                lstParam.Add("sMaDuAn", sMaDuAn);
                lstParam.Add("sUserName", sUserName);
                lstParam.Add("iNamLamViec", config.iNamLamViec);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_QT_DeNghiQuyetToanViewModel>("proc_get_all_vdt_qt_denghiquyettoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_QT_DeNghiQuyetToan Get_VDT_QT_DeNghiQuyetToanById(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.QueryFirstOrDefault<VDT_QT_DeNghiQuyetToan>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan WHERE iID_DeNghiQuyetToanID = '{0}'", id), null, commandType: CommandType.Text);
            }
        }
        public List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToan_VonDauTuById(Guid id)
        {
            string sql = @"SELECT dnqt.*,da.sTenDuAn, da.sMaDuAn ,nns.sTen  ,nns.iID_MaNguonNganSach, nv.fTienPheDuyet, ddct.fCapPhatBangLenhChi,ddct.fCapPhatTaiKhoBac
                FROM VDT_QT_DeNghiQuyetToan dnqt
                left join VDT_DA_DuAn da on da.iID_DuAnID = dnqt.iID_DuAnID 
                left join VDT_DA_QDDauTu dqdt on da.iID_DuAnID = dqdt.iID_DuAnID 
                left join VDT_DA_QDDauTu_NguonVon nv on nv.iID_QDDauTuID = dqdt.iID_QDDauTuID 
                left join NS_NguonNganSach nns on nv.iID_NguonVonID = nns.iID_MaNguonNganSach 
                left join VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ddct on da.iID_DuAnID = ddct.iID_DuAnID
                left join VDT_KHV_KeHoachVonNam_DuocDuyet dd on ddct.iID_KeHoachVonNam_DuocDuyetID = dd.iID_KeHoachVonNam_DuocDuyetID and dd.iID_NguonVonID = nv.iID_NguonVonID
				where iID_DeNghiQuyetToanID=@id";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_QT_DeNghiQuyetToanViewModel>(sql, param: new { id },
                    commandType: CommandType.Text);
                var SumKeHoachVon = item.ToList().Sum(x => x.fCapPhatBangLenhChi + x.fCapPhatTaiKhoBac);
                foreach (var sum in item)
                {
                    sum.SumKeHoachVon = sum.fCapPhatBangLenhChi + sum.fCapPhatTaiKhoBac;
                }
                return item.ToList();
            }
        }
        public List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToan_TaiSanById(Guid id)
        {
            string sql = @"SELECT dnqt.*,da.sTenDuAn, da.sMaDuAn, nda.sTenNhomDuAn
                FROM VDT_QT_DeNghiQuyetToan dnqt
                left join VDT_DA_DuAn da on da.iID_DuAnID = dnqt.iID_DuAnID 
				left join VDT_DM_NhomDuAn nda on nda.iID_NhomDuAnID = da.iID_NhomDuAnID 
                where iID_DeNghiQuyetToanID=@id";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_QT_DeNghiQuyetToanViewModel>(sql, param: new { id },
                    commandType: CommandType.Text);
                foreach (var sum in item)
                {
                    sum.SumCDTQuanLy = sum.fTaiSanDaiHanThuocCDTQuanLy + sum.fTaiSanNganHanThuocCDTQuanLy;
                    sum.SumKhacQuanLy = sum.fTaiSanDaiHanDonViKhacQuanLy + sum.fTaiSanNganHanDonViKhacQuanLy;
                }
                return item.ToList();
            }
        }
        public List<VDT_QT_DeNghiQuyetToanChiTietViewModel> Get_VDT_QT_DeNghiQuyetToan_ChiPhiById(Guid id)
        {
            string sql = @"SELECT ROW_NUMBER() OVER (ORDER BY id) AS STT,dnqtct.*,da.sTenDuAn, da.sMaDuAn,dacp.sTenChiPhi,dacp.sMaChiPhi,dt.fTongDuToanPheDuyet, dnqt.fChiPhiKhongTaoNenTaiSan,dnqt.fChiPhiThietHai
                FROM VDT_QT_DeNghiQuyetToan_ChiTiet dnqtct
				left join VDT_QT_DeNghiQuyetToan dnqt on dnqtct.iID_DeNghiQuyetToanID=dnqt.iID_DeNghiQuyetToanID
                left join VDT_DA_DuAn da on da.iID_DuAnID = dnqt.iID_DuAnID 
				left join VDT_DM_DuAn_ChiPhi dacp on dacp.iID_DuAn_ChiPhi=dnqtct.iID_ChiPhiId 
				left join VDT_DA_DuToan dt on dt.iID_DuAnID=dnqt.iID_DuAnID
				where dnqtct.iID_ChiPhiId is not null and dnqtct.iID_DeNghiQuyetToanID=@id";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_QT_DeNghiQuyetToanChiTietViewModel>(sql, param: new { id },
                    commandType: CommandType.Text);
                foreach (var itemdeviant in item)
                {
                    itemdeviant.deviant = itemdeviant.fGiaTriKiemToan - itemdeviant.fTongDuToanPheDuyet;
                }
                return item.ToList();
            }
        }
        public List<VDT_QT_DeNghiQuyetToanViewModel> Get_VDT_QT_DeNghiQuyetToanChiTietById(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDT_QT_DeNghiQuyetToanViewModel>(string.Format("SELECT *,dv.sTen,da.sTenDuAn, da.sMaDuAn  FROM VDT_QT_DeNghiQuyetToan dnqt inner join NS_DonVi dv  on dnqt.iID_DonViID = dv.iID_Ma  inner join VDT_DA_DuAn da on da.iID_DuAnID=dnqt.iID_DuAnID WHERE iID_DeNghiQuyetToanID = '{0}'", id), null, commandType: CommandType.Text).ToList();
            }
        }

        public List<VDT_DA_DuAn> GetListAllDuAn(string idDonViQuanLy, string iIdDeNghiQuyetToanId)
        {
            var result = new List<VDT_DA_DuAn>();

            using (var conn = _connectionFactory.GetConnection())
            {
                string sql = "SELECT * FROM VDT_QT_DeNghiQuyetToan";
                if (!string.IsNullOrEmpty(iIdDeNghiQuyetToanId))
                    sql += " WHERE iID_DeNghiQuyetToanID <> '" + iIdDeNghiQuyetToanId + "'";
                var listId = new List<Guid?>();
                var listModel = conn.Query<VDT_QT_DeNghiQuyetToan>(sql, null, commandType: CommandType.Text);
                if (listModel != null && listModel.Any())
                {
                    listId = listModel.Where(x => x.iID_DuAnID != null).Select(x => x.iID_DuAnID).ToList();
                }
                var items = conn.Query<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn WHERE iID_DonViQuanLyID = '{0}'", idDonViQuanLy), null, commandType: CommandType.Text);
                if (items != null && items.Any())
                {
                    if (listId != null && listId.Any())
                    {
                        items = items.Where(x => !listId.Contains(x.iID_DuAnID)).ToList();
                    }
                    result.AddRange(items);
                }
            }

            return result;
        }

        // get đơn vị theo năm làm việc của user login
        public List<NS_DonVi> GetListAllDonVi(string sUserName)
        {
            var result = new List<NS_DonVi>();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listPhongBanDonVi = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE sID_MaNguoiDung_DuocGiao = '{0}' AND iNamLamViec = '{1}' AND iTrangThai = 1", sUserName, config.iNamLamViec), null, commandType: CommandType.Text);
                if (listPhongBanDonVi == null || !listPhongBanDonVi.Any())
                {
                    return result;
                }

                var listDonVi = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = '{0}' AND iTrangThai = 1", config.iNamLamViec));
                var listIDMaDonVi = listPhongBanDonVi.Select(x => x.iID_MaDonVi).ToList();
                if (listIDMaDonVi != null && listIDMaDonVi.Any())
                {
                    if (listDonVi != null && listDonVi.Any())
                    {
                        result = listDonVi.Where(x => listIDMaDonVi.Contains(x.iID_MaDonVi)).ToList();
                    }
                }
            }

            return result;
        }

        // get đơn vị theo năm hiện tại
        public List<NS_DonVi> GetListAllDonViByCurrentYear(string sUserName, string iNamLamViec)
        {
            var result = new List<NS_DonVi>();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listPhongBanDonVi = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE iNamLamViec = '{1}' AND iTrangThai = 1", sUserName, iNamLamViec), null, commandType: CommandType.Text);
                if (listPhongBanDonVi == null || !listPhongBanDonVi.Any())
                {
                    return result;
                }

                var listDonVi = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = '{0}' AND iTrangThai = 1", iNamLamViec));
                var listIDMaDonVi = listPhongBanDonVi.Select(x => x.iID_MaDonVi).ToList();
                if (listIDMaDonVi != null && listIDMaDonVi.Any())
                {
                    if (listDonVi != null && listDonVi.Any())
                    {
                        result = listDonVi.Where(x => listIDMaDonVi.Contains(x.iID_MaDonVi)).ToList();
                    }
                }
            }

            return result;
        }

        public bool VDT_QT_DeNghiQuyetToan_SaveData(VDT_QT_DeNghiQuyetToanViewModel sModel, string sUserName)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (sModel.iID_DeNghiQuyetToanID == null || sModel.iID_DeNghiQuyetToanID == Guid.Empty)
                    {
                        sModel.iID_DeNghiQuyetToanID = Guid.NewGuid();
                        sModel.sUserCreate = sUserName;
                        sModel.dDateCreate = DateTime.Now;
                        sModel.sUserUpdate = sUserName;
                        sModel.dDateUpdate = DateTime.Now;

                        conn.Insert<VDT_QT_DeNghiQuyetToan>(sModel, trans);
                    }
                    else
                    {
                        sModel.sUserUpdate = sUserName;
                        sModel.dDateUpdate = DateTime.Now;

                        conn.Update<VDT_QT_DeNghiQuyetToan>(sModel, trans);
                    }

                    // delete VDT_QT_DeNghiQuyetToan_NguonVon cu
                    var listNguonVonCu = conn.Query<VDT_QT_DeNghiQuyetToan_Nguonvon>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan_Nguonvon WHERE iID_DeNghiQuyetToanID = '{0}'", sModel.iID_DeNghiQuyetToanID), null, trans);
                    if (listNguonVonCu != null && listNguonVonCu.Any())
                    {
                        foreach (var item in listNguonVonCu)
                        {
                            conn.Delete(item, trans);
                        }
                    }

                    // insert VDT_QT_DeNghiQuyetToan_NguonVon
                    if (sModel.listNguonVon != null && sModel.listNguonVon.Any())
                    {
                        foreach (VDTDuToanNguonVonModel item in sModel.listNguonVon)
                        {
                            VDT_QT_DeNghiQuyetToan_Nguonvon objQuyetToanNguonVon = new VDT_QT_DeNghiQuyetToan_Nguonvon();
                            objQuyetToanNguonVon.iID_DeNghiQuyetToanID = sModel.iID_DeNghiQuyetToanID;
                            objQuyetToanNguonVon.fTienToTrinh = item.fTienToTrinh;
                            objQuyetToanNguonVon.iID_NguonVonID = item.iID_NguonVonID;

                            conn.Insert<VDT_QT_DeNghiQuyetToan_Nguonvon>(objQuyetToanNguonVon, trans);
                        }
                    }

                    // delete VDT_QD_DeNghiQuyetToan_ChiTiet cu
                    var listChiTietCu = conn.Query<VDT_QT_DeNghiQuyetToan_ChiTiet>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan_ChiTiet WHERE iID_DeNghiQuyetToanID = '{0}'", sModel.iID_DeNghiQuyetToanID), null, trans);
                    if (listChiTietCu != null && listChiTietCu.Any())
                    {
                        foreach (var item in listChiTietCu)
                        {
                            conn.Delete(item, trans);
                        }
                    }

                    // insert VDT_QD_DeNghiQuyetToan_ChiTiet
                    if (sModel.listChiPhi != null && sModel.listChiPhi.Any())
                    {
                        foreach (VDT_DA_DuToan_ChiPhi_ViewModel itemCp in sModel.listChiPhi)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objChiTiet = new VDT_QT_DeNghiQuyetToan_ChiTiet();
                            objChiTiet.iID_DeNghiQuyetToanID = sModel.iID_DeNghiQuyetToanID;
                            objChiTiet.iID_ChiPhiId = itemCp.iID_DuAn_ChiPhi;
                            objChiTiet.fGiaTriKiemToan = itemCp.fGiaTriKiemToan;
                            objChiTiet.fGiaTriDeNghiQuyetToan = itemCp.fGiaTriDeNghiQuyetToan;
                            objChiTiet.fGiaTriQuyetToanAB = itemCp.fGiaTriQuyetToanAB;
                            //objChiTiet.sMaOrder = itemCp.iThuTu;

                            conn.Insert<VDT_QT_DeNghiQuyetToan_ChiTiet>(objChiTiet, trans);
                        }
                    }

                    if (sModel.listHangMuc != null && sModel.listHangMuc.Any())
                    {
                        foreach (VDT_DA_DuToan_HangMuc_ViewModel itemHm in sModel.listHangMuc)
                        {
                            VDT_QT_DeNghiQuyetToan_ChiTiet objChiTiet = new VDT_QT_DeNghiQuyetToan_ChiTiet();
                            objChiTiet.iID_DeNghiQuyetToanID = sModel.iID_DeNghiQuyetToanID;
                            objChiTiet.iID_HangMucId = itemHm.iID_HangMucID;
                            objChiTiet.fGiaTriKiemToan = itemHm.fGiaTriKiemToan;
                            objChiTiet.fGiaTriDeNghiQuyetToan = itemHm.fGiaTriDeNghiQuyetToan;
                            objChiTiet.fGiaTriQuyetToanAB = itemHm.fGiaTriQuyetToanAB;
                            //objChiTiet.sMaOrder = itemCp.iThuTu;

                            conn.Insert<VDT_QT_DeNghiQuyetToan_ChiTiet>(objChiTiet, trans);
                        }
                    }

                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public VDT_QT_DeNghiQuyetToanGetDuAnModel GetDuLieuDuAnById(string idDuAn, string sUserName)
        {
            var result = new VDT_QT_DeNghiQuyetToanGetDuAnModel();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var duAn = conn.QueryFirstOrDefault<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn WHERE iID_DuAnID = '{0}'", idDuAn));
                if (duAn != null && duAn.iID_ChuDauTuID != null)
                {
                    var donVi = conn.QueryFirstOrDefault<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iID_Ma = '{0}' AND iNamLamViec_DonVi = '{1}' AND iTrangThai = 1", duAn.iID_ChuDauTuID, config.iNamLamViec));
                    if (donVi != null)
                    {
                        result.sTenDuAn = duAn.sTenDuAn;
                        result.sTenChuDauTu = donVi.sTen;
                    }
                }
            }

            return result;
        }

        public NS_DonVi GetDuLieuDonViQuanLyByIdDuAn(string idDuAn, string sUserName)
        {
            var result = new NS_DonVi();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var duAn = conn.QueryFirstOrDefault<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn WHERE iID_DuAnID = '{0}'", idDuAn));
                if (duAn != null)
                {
                    result = conn.QueryFirstOrDefault<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iID_Ma = '{0}' AND iNamLamViec_DonVi = '{1}' AND iTrangThai = 1", duAn.iID_DonViQuanLyID, config.iNamLamViec));
                }
            }

            return result;
        }

        public bool VDT_QT_DeNghiQuyetToan_Delete(string id, string sUserName)
        {
            //using (var conn = _connectionFactory.GetConnection())
            //{
            //    var item = conn.QueryFirstOrDefault<VDT_QT_DeNghiQuyetToan>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan WHERE iID_DeNghiQuyetToanID = '{0}'", id));
            //    if (item == null)
            //    {
            //        return false;
            //    }

            //    return conn.Delete<VDT_QT_DeNghiQuyetToan>(item);
            //}
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var sql = @"DELETE VDT_QT_DeNghiQuyetToan_ChiTiet WHERE iID_DeNghiQuyetToanID = @id; " +
                " DELETE VDT_QT_DeNghiQuyetToan WHERE iID_DeNghiQuyetToanID = @id; ";

                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        id
                    },
                    transaction: trans,
                    commandType: CommandType.Text);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }

        public VDT_QT_DeNghiQuyetToanViewModel GetDeNghiQuyetToanDetail(string id, string sUserName)
        {
            var result = new VDT_QT_DeNghiQuyetToanViewModel();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var model = conn.QueryFirstOrDefault<VDT_QT_DeNghiQuyetToan>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan WHERE iID_DeNghiQuyetToanID = '{0}'", id), null, commandType: CommandType.Text);
                if (model != null)
                {
                    result.sSoBaoCao = model.sSoBaoCao;
                    result.dThoiGianLapBaoCao = model.dThoiGianLapBaoCao;
                    result.dThoiGianNhanBaoCao = model.dThoiGianNhanBaoCao;
                    result.dThoiGianKhoiCong = model.dThoiGianKhoiCong;
                    result.dThoiGianHoanThanh = model.dThoiGianHoanThanh;
                    result.sNguoiNhan = model.sNguoiNhan;
                    result.sNguoiLap = model.sNguoiLap;
                    result.fGiaTriDeNghiQuyetToan = model.fGiaTriDeNghiQuyetToan;
                    result.sMoTa = model.sMoTa;
                    result.iID_DeNghiQuyetToanID = model.iID_DeNghiQuyetToanID;
                    result.iID_DuAnID = model.iID_DuAnID;
                    result.iID_DonViID = model.iID_DonViID;
                    result.iID_DuToanID = model.iID_DuToanID;

                    result.fChiPhiThietHai = model.fChiPhiThietHai;
                    result.fChiPhiKhongTaoNenTaiSan = model.fChiPhiKhongTaoNenTaiSan;
                    result.fTaiSanDaiHanThuocCDTQuanLy = model.fTaiSanDaiHanThuocCDTQuanLy;
                    result.fTaiSanDaiHanDonViKhacQuanLy = model.fTaiSanDaiHanDonViKhacQuanLy;
                    result.fTaiSanNganHanThuocCDTQuanLy = model.fTaiSanNganHanThuocCDTQuanLy;
                    result.fTaiSanNganHanDonViKhacQuanLy = model.fTaiSanNganHanDonViKhacQuanLy;

                    var duAn = conn.QueryFirstOrDefault<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn WHERE iID_DuAnID = '{0}'", model.iID_DuAnID), null, commandType: CommandType.Text);
                    if (duAn != null)
                    {
                        result.sTenDuAn = duAn.sTenDuAn;
                        var donViQuanLy = conn.QueryFirstOrDefault<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iID_Ma = '{0}' AND iNamLamViec_DonVi = '{1}' AND iTrangThai = 1", duAn.iID_DonViQuanLyID, config.iNamLamViec), null, commandType: CommandType.Text);
                        var chuDauTu = conn.QueryFirstOrDefault<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iID_Ma = '{0}' AND iNamLamViec_DonVi = '{1}' AND iTrangThai = 1", duAn.iID_ChuDauTuID, config.iNamLamViec), null, commandType: CommandType.Text);
                        if (chuDauTu != null)
                        {
                            result.sTenChuDauTu = chuDauTu.sTen;
                        }
                    }

                    // get list nguon von
                    List<VDTDuToanNguonVonModel> lstNguonVonByDuAn = GetListDuToanNguonVonByDuAn(result.iID_DuAnID.ToString());
                    if (lstNguonVonByDuAn != null && lstNguonVonByDuAn.Any())
                    {
                        List<VDT_QT_DeNghiQuyetToan_Nguonvon> lstQuyetToanNguonVon = conn.Query<VDT_QT_DeNghiQuyetToan_Nguonvon>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToan_Nguonvon WHERE iID_DeNghiQuyetToanID = '{0}'", result.iID_DeNghiQuyetToanID), null, commandType: CommandType.Text).ToList();
                        if (lstQuyetToanNguonVon != null && lstQuyetToanNguonVon.Any())
                        {
                            foreach (VDTDuToanNguonVonModel item in lstNguonVonByDuAn)
                            {
                                VDT_QT_DeNghiQuyetToan_Nguonvon objQuyetToanNguonVon = lstQuyetToanNguonVon.Where(x => x.iID_NguonVonID == item.iID_NguonVonID).FirstOrDefault();
                                if (objQuyetToanNguonVon != null)
                                    item.fTienToTrinh = objQuyetToanNguonVon.fTienToTrinh;
                            }
                        }
                    }
                    result.listNguonVon = lstNguonVonByDuAn;
                }
            }

            return result;
        }

        public List<VDT_QT_DeNghiQuyetToanViewModel> ExportData(string sSoBaoCao, decimal? sGiaDeNghiTu, decimal? sGiaDeNghiDen, string sTenDuAn, string sMaDuAn, string sUserName)
        {
            var result = new List<VDT_QT_DeNghiQuyetToanViewModel>();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listModel = conn.Query<VDT_QT_DeNghiQuyetToan>("SELECT * FROM VDT_QT_DeNghiQuyetToan", null, commandType: CommandType.Text);
                if (listModel == null || !listModel.Any())
                {
                    return result;
                }
                var listDonVi = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = '{0}' AND iTrangThai = 1", config.iNamLamViec));
                var listPhongBan = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE sID_MaNguoiDung_DuocGiao = '{0}' AND iNamLamViec = '{1}' AND iTrangThai = 1", sUserName, config.iNamLamViec));
                var listIdMaDonVi = new List<string>();
                if (listPhongBan != null && listPhongBan.Any())
                {
                    listIdMaDonVi = listPhongBan.Where(x => x.iID_MaDonVi != null).Select(x => x.iID_MaDonVi).ToList();
                }
                var listIdDonViQuanLy = new List<Guid>();
                if (listDonVi != null && listDonVi.Any())
                {
                    listIdDonViQuanLy = listDonVi.Where(x => listIdMaDonVi.Contains(x.iID_MaDonVi)).Select(x => x.iID_Ma).ToList();
                }
                var listIdDuAn = new List<Guid>();
                var listDuAn = conn.Query<VDT_DA_DuAn>($"SELECT * FROM VDT_DA_DuAn");
                if (listIdDonViQuanLy != null && listIdDonViQuanLy.Any())
                {
                    listIdDuAn = listDuAn.Where(x => x.iID_DonViQuanLyID != null && listIdDonViQuanLy.Contains(x.iID_DonViQuanLyID.Value)).Select(x => x.iID_DuAnID).ToList();
                }
                if (listIdDuAn == null || !listIdDuAn.Any())
                {
                    return result;
                }
                listModel = listModel.Where(x => x.iID_DuAnID != null && listIdDuAn.Contains(x.iID_DuAnID.Value)).ToList();
                if (listModel != null && listModel.Any())
                {
                    foreach (var item in listModel.OrderBy(x => x.dThoiGianKhoiCong))
                    {
                        var model = new VDT_QT_DeNghiQuyetToanViewModel();
                        model.iID_DeNghiQuyetToanID = item.iID_DeNghiQuyetToanID;
                        model.iID_DonViTienTeID = item.iID_DonViTienTeID;
                        model.iID_DuAnID = item.iID_DuAnID;
                        model.iID_TienTeID = item.iID_TienTeID;
                        model.sSoBaoCao = item.sSoBaoCao;
                        if (item.dThoiGianKhoiCong.HasValue)
                        {
                            model.dThoiGianKhoiCong = item.dThoiGianKhoiCong;
                            model.dThoiGianKhoiCongAsString = item.dThoiGianKhoiCong.Value.ToString("dd/MM/yyyy");
                        }
                        if (item.dThoiGianHoanThanh.HasValue)
                        {
                            model.dThoiGianHoanThanh = item.dThoiGianHoanThanh;
                            model.dThoiGianHoanThanhAsString = item.dThoiGianHoanThanh.Value.ToString("dd/MM/yyyy");
                        }
                        model.fGiaTriDeNghiQuyetToan = item.fGiaTriDeNghiQuyetToan;
                        if (item.iID_DuAnID != null)
                        {
                            if (listDuAn != null && listDuAn.Any())
                            {
                                var duAn = listDuAn.FirstOrDefault(x => x.iID_DuAnID == item.iID_DuAnID);
                                if (duAn != null)
                                {
                                    model.sMaDuAn = duAn.sMaDuAn;
                                    model.sTenDuAn = duAn.sTenDuAn;

                                    if (duAn.iID_ChuDauTuID != null)
                                    {
                                        if (listDonVi != null && listDonVi.Any())
                                        {
                                            var chuDauTu = listDonVi.FirstOrDefault(x => x.iID_Ma == duAn.iID_ChuDauTuID);
                                            if (chuDauTu != null)
                                            {
                                                model.sTenChuDauTu = chuDauTu.sTen;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        result.Add(model);
                    }
                }
            }

            if (result != null && result.Any())
            {
                if (!string.IsNullOrEmpty(sSoBaoCao))
                {
                    result = result.Where(x => x.sSoBaoCao.Contains(sSoBaoCao)).ToList();
                }

                if (sGiaDeNghiTu != null)
                {
                    result = result.Where(x => x.fGiaTriDeNghiQuyetToan >= double.Parse(sGiaDeNghiTu.Value.ToString())).ToList();
                }

                if (sGiaDeNghiDen != null)
                {
                    result = result.Where(x => x.fGiaTriDeNghiQuyetToan <= double.Parse(sGiaDeNghiDen.Value.ToString())).ToList();
                }

                if (!string.IsNullOrEmpty(sMaDuAn))
                {
                    result = result.Where(x => x.sMaDuAn.Contains(sMaDuAn)).ToList();
                }

                if (!string.IsNullOrEmpty(sTenDuAn))
                {
                    result = result.Where(x => x.sTenDuAn.Contains(sTenDuAn)).ToList();
                }

            }

            if (result != null && result.Any())
            {
                var count = 1;
                foreach (var item in result.OrderBy(x => x.dThoiGianKhoiCong))
                {
                    item.STT = count;
                    count++;
                }
            }

            return result;
        }

        #endregion

        #region Quản lý phê duyệt quyết toán DA hoàn thành - NinhNV
        public IEnumerable<VDTQLPheDuyetQuyetToanViewModel> LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhDNQT(Guid iID_DonViQuanLyID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_duan_denghiquyettoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTQLPheDuyetQuyetToanViewModel>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID,
                       dNgayQuyetDinh
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public VDTQLPheDuyetQuyetToanViewModel GetThongTinDuAn(Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_duan_info_by_id_ngayquyetdinh.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<VDTQLPheDuyetQuyetToanViewModel>(sql,
                    param: new
                    {
                        iID_DonViQuanLyID,
                        iID_DuAnID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public double GetGiaTriDuToan(Guid iID_DuAnID, Guid iID_ChiPhiID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_giatridutoan_pdqt.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_ChiPhiID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public double GetGiaTriDuToanNguonVon(Guid iID_DuAnID, int iID_MaNguonNganSach, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_giatridutoan_nguonvon_pdqt.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_MaNguonNganSach,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public double GetTongGiaTriPhanBo(Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_tonggiatriphanbo.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DonViQuanLyID,
                        iID_DuAnID,
                        dNgayQuyetDinh
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public IEnumerable<VDTNguonVonDauTuViewModel> GetLstNoiDungQuyetToan(IEnumerable<VDTNguonVonDauTuTableModel> arrNguonVon, Guid iID_DonViQuanLyID, Guid iID_DuAnID, DateTime? dNgayQuyetDinh, int iNamLamViec)
        {
            var datatableNguonVon = ToDataTable(arrNguonVon);
            using (var conn = _connectionFactory.GetConnection())
            {

                using (var cmd = new SqlCommand("proc_get_danhsach_quyettoan", conn))
                {
                    cmd.Parameters.AddWithValue("@iID_DonViQuanLyID", iID_DonViQuanLyID);
                    cmd.Parameters.AddWithValue("@iID_DuAnID", iID_DuAnID);
                    cmd.Parameters.AddWithValue("@dNgayQuyetDinh", dNgayQuyetDinh);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@arrNguonVon", datatableNguonVon);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.VDT_PhanNguon_temp";

                    DataTable dt = cmd.GetTable();
                    return ConvertToViewModels(dt);
                }

            }

        }

        private IEnumerable<VDTNguonVonDauTuViewModel> ConvertToViewModels(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(row => new VDTNguonVonDauTuViewModel
            {
                iID_MaNguonNganSach = Convert.ToInt32(row["iID_MaNguonNganSach"].ToString()),
                sTen = row["sTen"].ToString(),
                fGiaTriDuToan = Convert.ToDouble(row["fGiaTriDuToan"]),
                fGiaTriQuyetToan = Convert.ToDouble(row["fGiaTriQuyetToan"]),
                fGiaTriChenhLech = Convert.ToDouble(row["fGiaTriChenhLech"]),
                sChenhLech = row["sChenhLech"].ToString()
            });
        }

        public DataTable ToDataTable<VDTNguonVonDauTuTableModel>(IEnumerable<VDTNguonVonDauTuTableModel> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(VDTNguonVonDauTuTableModel));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (VDTNguonVonDauTuTableModel item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public IEnumerable<VDTQLPheDuyetQuyetToanViewModel> GetAllPheDuyetQuyetToan(ref PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, string sTenDuAn, double? fTienQuyetToanPheDuyetFrom, double? fTienQuyetToanPheDuyetTo)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("sTenDuAn", sTenDuAn);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("fTienQuyetToanPheDuyetFrom", fTienQuyetToanPheDuyetFrom);
                lstParam.Add("fTienQuyetToanPheDuyetTo", fTienQuyetToanPheDuyetTo);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTQLPheDuyetQuyetToanViewModel>("proc_get_all_pheduyet_quyettoan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDTQLPheDuyetQuyetToanViewModel GetVdtQuyetToanById(Guid? iID_QuyetToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_pheduyetquyettoan_info_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDTQLPheDuyetQuyetToanViewModel>(sql,
                   param: new
                   {
                       iID_QuyetToanID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDTChiPhiDauTuModel> GetLstChiPhiDauTu(Guid? iID_QuyetToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_chiphidautu_by_idquyettoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTChiPhiDauTuModel>(sql,
                   param: new
                   {
                       iID_QuyetToanID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDTNguonVonDauTuModel> GetLstNguonVonDauTu(Guid? iID_QuyetToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_nguonvondautu_by_idquyettoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTNguonVonDauTuModel>(sql,
                   param: new
                   {
                       iID_QuyetToanID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public bool deleteQTChiPhiQTNguonVonQTChenhLech(Guid iID_QuyetToanID)
        {
            var sql = "DELETE VDT_QT_QuyetToan_ChiPhi WHERE iID_QuyetToanID = @iID_QuyetToanID; " +
                " DELETE VDT_QT_QuyetToan_Nguonvon WHERE iID_QuyetToanID = @iID_QuyetToanID; " +
                " DELETE VDT_QT_QuyetToan_NguonVon_ChenhLech WHERE iID_QuyetToanID = @iID_QuyetToanID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_QuyetToanID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public bool deleteVDTQTDA(Guid iID_QuyetToanID)
        {
            var sql = "DELETE VDT_QT_QuyetToan WHERE iID_QuyetToanID = @iID_QuyetToanID; " +
                " DELETE VDT_QT_QuyetToan_ChiTiet WHERE iID_QuyetToanID = @iID_QuyetToanID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_QuyetToanID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }
        #endregion

        #region Vốn đầu tư - quyết toán - tổng hợp số liệu

        public IEnumerable<VDT_QT_TongHopSoLieuViewModel> GetAllTongHopSoLieuPaging(ref PagingInfo _paging, Guid? iID_DonViQuanLy, int? iID_NguonVon, int? iNamKeHoach)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iID_DonViQuanLy", iID_DonViQuanLy);
                lstParam.Add("iID_NguonVon", iID_NguonVon);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_QT_TongHopSoLieuViewModel>("proc_get_all_vdt_tonghopsolieu_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        #region Tổng hợp thông tin dự án - NinhNV
        public IEnumerable<NS_DonVi> GetDanhSachDonVi(int iNamLamViec)
        {
            var sql = "select * from NS_DonVi where iNamLamViec_DonVi = @iNamLamViec and iTrangThai =1  ";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(sql,
                   param: new
                   {
                       iNamLamViec
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDTTongHopThongTinDuAnViewModel> GetThongTinTongHopDuAn(Guid iID_DonViQuanLyID, string sTenDuAn, int? iNamKeHoach, int iNamKhoiTao)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                    lstParam.Add("sTenDuAn", sTenDuAn);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("iNamKhoiTao", iNamKhoiTao);

                    var items = conn.Query<VDTTongHopThongTinDuAnViewModel>("sp_vdt_rpt_tong_hop_thong_tin_du_an", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDTTongHopThongTinDuAnViewModel> GetThongTinTongHopDuAn_v2(Guid iID_DonViQuanLyID, int? iNamKeHoach)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);

                    var items = conn.Query<VDTTongHopThongTinDuAnViewModel>("sp_vdt_rpt_tong_hop_thong_tin_du_an_v2", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDTTinhHinhDuAnViewModel> GetThongTinTinhHinhThucHienDuAn(Guid? iID_DonViQuanLyID, Guid? iID_DuAn, DateTime? denngay)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                    lstParam.Add("iID_DuAn", iID_DuAn);
                    lstParam.Add("denngay", denngay);

                    var items = conn.Query<VDTTinhHinhDuAnViewModel>("sp_vdt_rpt_tinh_hinh_thuc_hien_du_an", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public VDTBaoCaoTinhHinhDATTChungViewModel GetThongTinChungBCTTDA(Guid? iID_DuAn, int? Nam)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_baocao_thuc_hien_du_an_ttchung.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<VDTBaoCaoTinhHinhDATTChungViewModel>(sql,
                        param: new
                        {
                            iID_DuAn,
                            Nam,
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

        public IEnumerable<VDT_DA_DuAn> GetDuAnByIdDonVi(Guid? iID_DonViQuanLyID)
        {
            var sql = "SELECT * FROM VDT_DA_DuAn duan WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID ";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDTBaoCaoCapPhatViewModel> GetThongTinBaoCaoCapPhat(Guid? iIDMaDonVi, int? iIDNguonNganSach, int? iNamKeHoach, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIDMaDonVi", iIDMaDonVi);
                    lstParam.Add("iIDNguonNganSach", iIDNguonNganSach);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("iNamLamViec", iNamLamViec);

                    var items = conn.Query<VDTBaoCaoCapPhatViewModel>("sp_vdt_rpt_tong_hop_cap_phat", lstParam, commandType: CommandType.StoredProcedure);
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
        #region Kết quả giải ngân chi phí đầu tư
        public IEnumerable<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2> GetKetQuaGiaiNganChiKinhPhiDauTu(Guid? iID_DonViQuanLyID, int iID_NguonVonID, int? iNamKeHoach)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("iIdMaDonVi", iID_DonViQuanLyID);
                    lstParam.Add("iIdNguonVon", iID_NguonVonID);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);

                    var items = conn.Query<VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2>("sp_vdt_get_baocaoketquagiainganchikinhphidautu", lstParam, commandType: CommandType.StoredProcedure);
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

        public List<NS_NguonNganSach> GetListDataNguonNganSach()
        {
            var result = new List<NS_NguonNganSach>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_NguonNganSach>(string.Format("SELECT * FROM NS_NguonNganSach"));
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }

            return result;
        }
        public VDT_QT_TongHopSoLieuDetailViewModel GetDetailTongHopSoLieu(string id)
        {
            var model = new VDT_QT_TongHopSoLieuDetailViewModel();
            using (var conn = _connectionFactory.GetConnection())
            {
                var master = conn.QueryFirstOrDefault<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (master != null)
                {
                    model.Master = master;
                }

                var listDetail = conn.Query<VDT_QT_TongHopSoLieu_ChiTiet>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu_ChiTiet WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (listDetail != null && listDetail.Any())
                {
                    model.ListDetail = listDetail.ToList();
                }

                var listSoLieu = conn.Query<VDT_QT_XuLySoLieu>(string.Format("SELECT * FROM VDT_QT_XuLySoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (listSoLieu != null && listSoLieu.Any())
                {
                    model.ListSoLieu = listSoLieu.ToList();
                }
            }
            return model;
        }

        public VDT_QT_TongHopSoLieu GetTongHopSoLieuById(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                return item;
            }
        }

        public List<VDT_QT_TongHopSoLieu_ChiTiet> GetListDataTongHopSoLieuChiTiet(string id)
        {
            var result = new List<VDT_QT_TongHopSoLieu_ChiTiet>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_TongHopSoLieu_ChiTiet>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu_ChiTiet WHERE iID_TongHopSoLieuID = {0}", id));
                if (items != null)
                {
                    result.AddRange(items);
                }
            }

            return result;
        }

        public List<VDT_QT_TongHopSoLieuViewModel> GetListDataTongHopSoLieu()
        {
            var result = new List<VDT_QT_TongHopSoLieuViewModel>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_TongHopSoLieu>("SELECT * FROM VDT_QT_TongHopSoLieu");
                var listDonViQuanLy = conn.Query<NS_DonVi>("SELECT * FROM NS_DonVi");
                var listNguonNganSach = conn.Query<NS_NguonNganSach>("SELECT * FROM NS_NguonNganSach");
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var model = new VDT_QT_TongHopSoLieuViewModel();
                        model.iID_TongHopSoLieuID = item.iID_TongHopSoLieuID;
                        model.iNamKeHoach = item.iNamKeHoach;
                        model.dNgayLap = item.dNgayLap;
                        if (listDonViQuanLy != null && listDonViQuanLy.Any())
                        {
                            var donVi = listDonViQuanLy.FirstOrDefault(x => x.iID_Ma == item.iID_DonViQuanLyID);
                            if (donVi != null)
                            {
                                model.iID_DonViQuanLyID = item.iID_DonViQuanLyID;
                                model.TenDonViQuanLy = donVi.sTen;
                            }
                        }

                        if (listNguonNganSach != null && listNguonNganSach.Any())
                        {
                            if (item.iID_NguonVonID.HasValue)
                            {
                                var nguonVon = listNguonNganSach.FirstOrDefault(x => x.iID_MaNguonNganSach == int.Parse(item.iID_NguonVonID.Value.ToString()));
                                if (nguonVon != null)
                                {
                                    model.TenNguonVon = nguonVon.sTen;
                                    model.iID_NguonVonID = item.iID_NguonVonID;
                                }
                            }
                        }
                        result.Add(model);
                    }
                }
            }

            return result;
        }

        public bool ChangeStatusTongHopSoLieu(Guid? id, string typeChange)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var model = conn.QueryFirstOrDefault<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (model != null)
                {
                    if (typeChange == "bIsCanBoDuyet")
                    {
                        if (model.bIsCanBoDuyet == null)
                        {
                            model.bIsCanBoDuyet = true;
                        }
                        else
                        {
                            model.bIsCanBoDuyet = !model.bIsCanBoDuyet;
                        }
                    }
                    else
                    {
                        if (model.bIsDuyet == null)
                        {
                            model.bIsDuyet = true;
                        }
                        else
                        {
                            model.bIsDuyet = !model.bIsDuyet;
                        }
                    }
                }

                conn.Update<VDT_QT_TongHopSoLieu>(model);
            }
            return true;
        }

        public List<NS_DonVi> GetListDataDonVi(string sUserName)
        {
            var result = new List<NS_DonVi>();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listPhongBan = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE iNamLamViec = '{0}' AND iTrangThai = 1 AND sID_MaNguoiDung_DuocGiao = '{1}'", config.iNamLamViec, sUserName));
                var items = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = '{0}' AND iTrangThai = 1", config.iNamLamViec));
                var listMaDonVi = new List<string>();
                if (listPhongBan != null && listPhongBan.Any())
                {
                    listMaDonVi = listPhongBan.Select(x => x.iID_MaDonVi).ToList();
                }
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        if (listMaDonVi.Contains(item.iID_MaDonVi))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public VDT_QT_TongHopSoLieuCreateViewModel TongHopSoLieu(int? iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap)
        {
            var result = new VDT_QT_TongHopSoLieuCreateViewModel();

            using (var conn = _connectionFactory.GetConnection())
            {
                var listTongHopSoLieuAll = conn.Query<VDT_QT_TongHopSoLieu>("SELECT * FROM VDT_QT_TongHopSoLieu");

                var listXuLySoLieuAll = conn.Query<VDT_QT_XuLySoLieu>("SELECT * FROM VDT_QT_XuLySoLieu");
                var listPhanBoVonAll = conn.Query<VDT_KHV_PhanBoVon>("SELECT * FROM VDT_KHV_PhanBoVon");
                var listPhanBoVonChiTietAll = conn.Query<VDT_KHV_PhanBoVon_ChiTiet>("SELECT * FROM VDT_KHV_PhanBoVon_ChiTiet");
                var listDeNghiThanhToanAll = conn.Query<VDT_TT_DeNghiThanhToan>("SELECT * FROM VDT_TT_DeNghiThanhToan");
                var listDeNghiQuyetToanNienDoTroLyAll = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy>("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy");
                var listDeNghiQuyetToanNienDoTroLyChiTietAll = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet");

                var tongHopSoLieuResult = new TongHopSoLieuChiTietViewModel();
                var xuLySoLieuNamTruocResult = new VDT_QT_XuLySoLieuViewModel();
                var xuLySoLieuNamNayResult = new VDT_QT_XuLySoLieuViewModel();
                var tongHopSoLieuNamTruoc = new VDT_QT_TongHopSoLieu();
                var tongHopSoLieuNamNay = new VDT_QT_TongHopSoLieu();
                if (listTongHopSoLieuAll != null && listTongHopSoLieuAll.Any())
                {
                    if (iID_DonViQuanLy.HasValue && iNamThucHien.HasValue)
                    {
                        tongHopSoLieuNamTruoc = listTongHopSoLieuAll.FirstOrDefault(x => x.iID_DonViQuanLyID == iID_DonViQuanLy && x.iNamKeHoach == iNamThucHien - 1);
                        tongHopSoLieuNamNay = listTongHopSoLieuAll.FirstOrDefault(x => x.iID_DonViQuanLyID == iID_DonViQuanLy && x.iNamKeHoach == iNamThucHien);
                    }
                }

                if (listXuLySoLieuAll != null && listXuLySoLieuAll.Any())
                {
                    if (tongHopSoLieuNamTruoc != null)
                    {
                        var xuLySoLieuNamTruoc = listXuLySoLieuAll.FirstOrDefault(x => x.iID_TongHopSoLieuID == tongHopSoLieuNamTruoc.iID_TongHopSoLieuID);
                        if (xuLySoLieuNamTruoc != null)
                        {
                            result.XuLySoLieuNamTruoc = new VDT_QT_XuLySoLieuViewModel();
                            result.TongHopSoLieu = new TongHopSoLieuChiTietViewModel();

                            xuLySoLieuNamTruocResult.iID_XuLySoLieu_ChiTietID = xuLySoLieuNamTruoc.iID_XuLySoLieu_ChiTietID;
                            xuLySoLieuNamTruocResult.iID_TongHopSoLieuID = tongHopSoLieuNamTruoc.iID_TongHopSoLieuID;
                            if (xuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap.HasValue)
                            {
                                result.TongHopSoLieu.ChiTieuNamTruoc = xuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap.Value;
                            }
                            else
                            {
                                result.TongHopSoLieu.ChiTieuNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruoc.fGiaTriChuyenNamSauDaCap.HasValue)
                            {
                                result.TongHopSoLieu.ChiTieuNamTruoc += xuLySoLieuNamTruoc.fGiaTriChuyenNamSauDaCap.Value;
                            }

                            if (xuLySoLieuNamTruoc.fChiTieuNamTruoc.HasValue)
                            {
                                result.TongHopSoLieu.CapPhatNamTruoc = xuLySoLieuNamTruoc.fChiTieuNamTruoc.Value;
                            }
                            else
                            {
                                result.TongHopSoLieu.CapPhatNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruoc.fChiTieuNamTruoc.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fChiTieu = xuLySoLieuNamTruoc.fChiTieuNamTruoc.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fChiTieu = 0;
                            }

                            if (xuLySoLieuNamTruoc.fCapPhatNamTruoc.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fCapPhat = xuLySoLieuNamTruoc.fCapPhatNamTruoc.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fCapPhat = 0;
                            }

                            if (xuLySoLieuNamTruoc.fQuyetToanNamTruoc.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fQuyetToan = xuLySoLieuNamTruoc.fQuyetToanNamTruoc.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fQuyetToan = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fThuaNamTruoc = xuLySoLieuNamTruocResult.fChiTieu.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fThuaNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fThuaNamTruoc -= xuLySoLieuNamTruocResult.fQuyetToan.Value;
                            }

                            if (xuLySoLieuNamTruocResult.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fThieuNamTruoc = xuLySoLieuNamTruocResult.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fThieuNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fThieuNamTruoc -= xuLySoLieuNamTruocResult.fChiTieu.Value;
                            }

                            if (xuLySoLieuNamTruoc.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fTrongChiTieuNamTruoc = xuLySoLieuNamTruoc.fChiTieu.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fTrongChiTieuNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruoc = xuLySoLieuNamTruocResult.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruoc = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruoc -= xuLySoLieuNamTruocResult.fChiTieu.Value;
                            }

                            if (xuLySoLieuNamTruocResult.fCapPhat.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap = xuLySoLieuNamTruocResult.fCapPhat.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap = 0;
                            }

                            if (xuLySoLieuNamTruocResult.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap -= xuLySoLieuNamTruocResult.fQuyetToan.Value;
                            }

                            if (xuLySoLieuNamTruocResult.fThuThanhKhoan.HasValue)
                            {
                                result.XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap -= xuLySoLieuNamTruocResult.fThuThanhKhoan.Value;
                            }

                            result.XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauChuaCap = 0;
                        }
                    }

                    if (tongHopSoLieuNamNay != null)
                    {
                        var xuLySoLieuNamNay = listXuLySoLieuAll.FirstOrDefault(x => x.iID_TongHopSoLieuID == tongHopSoLieuNamNay.iID_TongHopSoLieuID);
                        if (xuLySoLieuNamNay != null)
                        {
                            xuLySoLieuNamNayResult.iID_XuLySoLieu_ChiTietID = xuLySoLieuNamNay.iID_XuLySoLieu_ChiTietID;
                            xuLySoLieuNamNayResult.iID_TongHopSoLieuID = xuLySoLieuNamNay.iID_TongHopSoLieuID;

                            result.XuLySoLieuNamSau = new VDT_QT_XuLySoLieuViewModel();
                            if (xuLySoLieuNamNay.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fChiTieu = xuLySoLieuNamNay.fChiTieu.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fChiTieu = 0;
                            }

                            if (xuLySoLieuNamNay.fCapPhat.HasValue)
                            {
                                result.XuLySoLieuNamSau.fCapPhat = xuLySoLieuNamNay.fCapPhat.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fCapPhat = 0;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fQuyetToan = xuLySoLieuNamNay.fQuyetToan.Value;

                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fQuyetToan = 0;
                            }

                            if (xuLySoLieuNamNay.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fThua = xuLySoLieuNamNay.fChiTieu.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fThua = 0;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fThua -= xuLySoLieuNamNay.fQuyetToan.Value;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fThieu = xuLySoLieuNamNay.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fThieu = 0;
                            }

                            if (xuLySoLieuNamNay.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fThieu -= xuLySoLieuNamNay.fChiTieu.Value;
                            }

                            if (xuLySoLieuNamNay.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fTrongChiTieu = xuLySoLieuNamNay.fChiTieu.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fTrongChiTieu = 0;
                            }

                            if (xuLySoLieuNamNay.fDuocBu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fTrongChiTieu -= xuLySoLieuNamNay.fDuocBu.Value;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fNgoaiChiTieu = xuLySoLieuNamNay.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fNgoaiChiTieu = 0;
                            }

                            if (xuLySoLieuNamNay.fChiTieu.HasValue)
                            {
                                result.XuLySoLieuNamSau.fNgoaiChiTieu -= xuLySoLieuNamNay.fChiTieu.Value;
                            }

                            if (xuLySoLieuNamNay.fCapPhat.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap = xuLySoLieuNamNay.fCapPhat.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap = 0;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap -= xuLySoLieuNamNay.fQuyetToan.Value;
                            }

                            if (xuLySoLieuNamNay.fCapThanhKhoan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap += xuLySoLieuNamNay.fCapThanhKhoan.Value;
                            }

                            if (xuLySoLieuNamNay.fThuThanhKhoan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap -= xuLySoLieuNamNay.fThuThanhKhoan.Value;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap = xuLySoLieuNamNay.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap = 0;
                            }

                            if (xuLySoLieuNamNay.fCapPhat.HasValue)
                            {
                                result.XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap -= xuLySoLieuNamNay.fCapPhat.Value;
                            }

                            if (xuLySoLieuNamNay.fQuyetToan.HasValue)
                            {
                                result.XuLySoLieuNamSau.fCapThanhKhoan = xuLySoLieuNamNay.fQuyetToan.Value;
                            }
                            else
                            {
                                result.XuLySoLieuNamSau.fCapThanhKhoan = 0;
                            }

                            if (xuLySoLieuNamNay.fCapPhat.HasValue)
                            {
                                result.XuLySoLieuNamSau.fCapThanhKhoan -= xuLySoLieuNamNay.fCapPhat.Value;
                            }
                        }
                    }

                }

                if (listPhanBoVonAll != null && listPhanBoVonAll.Any())
                {
                    var listPhanBoVon = listPhanBoVonAll.Where(x => x.iID_DonViQuanLyID == iID_DonViQuanLy && x.iNamKeHoach == iNamThucHien).ToList();
                    if (listPhanBoVon != null && listPhanBoVon.Any())
                    {
                        var listIdPhanBoVon = listPhanBoVon.Select(x => x.iID_PhanBoVonID).ToList();

                        if (listPhanBoVonChiTietAll != null && listPhanBoVonChiTietAll.Any())
                        {
                            var listPhanBoVonChiTiet = listPhanBoVonChiTietAll.Where(x => listIdPhanBoVon.Contains(x.iID_PhanBoVonID.Value) && x.fGiaTrPhanBo != null).ToList();
                            if (listPhanBoVonChiTiet != null && listPhanBoVonChiTiet.Any())
                            {
                                if (result.TongHopSoLieu == null)
                                {
                                    result.TongHopSoLieu = new TongHopSoLieuChiTietViewModel();
                                }
                                if (result.XuLySoLieuNamSau == null)
                                {
                                    result.XuLySoLieuNamSau = new VDT_QT_XuLySoLieuViewModel();
                                }
                                result.TongHopSoLieu.ChiTieuNamNay = listPhanBoVonChiTiet.Sum(x => x.fGiaTrPhanBo.Value);
                                result.XuLySoLieuNamSau.fChiTieu = listPhanBoVonChiTiet.Sum(x => x.fGiaTrPhanBo.Value);
                            }
                        }
                    }
                }

                if (listDeNghiThanhToanAll != null && listDeNghiThanhToanAll.Any())
                {
                    var listDeNghiThanhToanNamTruoc = listDeNghiThanhToanAll.Where(x => x.iID_DonViQuanLyID == iID_DonViQuanLy && iNamThucHien == iNamThucHien.Value).ToList(); // đang bị lỗi sửa tạm để chạy
                    if (listDeNghiThanhToanNamTruoc != null && listDeNghiThanhToanNamTruoc.Any())
                    {
                        if (result.TongHopSoLieu == null)
                        {
                            result.TongHopSoLieu = new TongHopSoLieuChiTietViewModel();
                        }
                        if (result.XuLySoLieuNamSau == null)
                        {
                            result.XuLySoLieuNamSau = new VDT_QT_XuLySoLieuViewModel();
                        }
                        //result.TongHopSoLieu.CapPhatNamNay = listDeNghiThanhToanNamTruoc.Sum(x => x.fGiaTriThanhToan.Value);// comment tạm để chạy
                        //result.XuLySoLieuNamSau.fCapPhat = listDeNghiThanhToanNamTruoc.Sum(x => x.fGiaTriThanhToan.Value);
                    }
                }

                if (listDeNghiQuyetToanNienDoTroLyAll != null && listDeNghiQuyetToanNienDoTroLyAll.Any())
                {
                    var deNghiQuyetToanNienDoTroLy = listDeNghiQuyetToanNienDoTroLyAll.FirstOrDefault(x => x.iID_DonViDeNghiID == iID_DonViQuanLy && x.iNamKeHoach == iNamThucHien.Value);

                    if (deNghiQuyetToanNienDoTroLy != null)
                    {
                        if (listDeNghiQuyetToanNienDoTroLyChiTietAll != null && listDeNghiQuyetToanNienDoTroLyChiTietAll.Any())
                        {
                            var listDeNghiQuyetToanNienDoTroLyChiTiet = listDeNghiQuyetToanNienDoTroLyChiTietAll.Where(x => x.iID_DeNghiQuyetToanNienDoID == deNghiQuyetToanNienDoTroLy.iID_DeNghiQuyetToanNienDoID);

                            if (listDeNghiQuyetToanNienDoTroLyChiTiet != null && listDeNghiQuyetToanNienDoTroLyChiTiet.Any())
                            {
                                if (result.TongHopSoLieu == null)
                                {
                                    result.TongHopSoLieu = new TongHopSoLieuChiTietViewModel();
                                }
                                var fDonViiDeNghiQuyetToanNamTruoc = listDeNghiQuyetToanNienDoTroLyChiTiet.Where(x => x.fGiaTriQuyetToanNamTruocDonVi != null).Sum(x => x.fGiaTriQuyetToanNamTruocDonVi);
                                if (fDonViiDeNghiQuyetToanNamTruoc.HasValue)
                                {
                                    result.TongHopSoLieu.fDonViiDeNghiQuyetToanNamTruoc = fDonViiDeNghiQuyetToanNamTruoc.Value;
                                }
                                else
                                {
                                    result.TongHopSoLieu.fDonViiDeNghiQuyetToanNamTruoc = 0;
                                }

                                var fDonViiDeNghiQuyetToanNamNay = listDeNghiQuyetToanNienDoTroLyChiTiet.Where(x => x.fGiaTriQuyetToanNamNayDonVi != null).Sum(x => x.fGiaTriQuyetToanNamNayDonVi);
                                if (fDonViiDeNghiQuyetToanNamNay.HasValue)
                                {
                                    result.TongHopSoLieu.fDonViiDeNghiQuyetToanNamNay = fDonViiDeNghiQuyetToanNamNay.Value;
                                }
                                else
                                {
                                    result.TongHopSoLieu.fDonViiDeNghiQuyetToanNamNay = 0;
                                }

                                var fTroLyDeNghiQuyetToanNamTruoc = listDeNghiQuyetToanNienDoTroLyChiTiet.Where(x => x.fGiaTriQuyetToanNamTruoc != null).Sum(x => x.fGiaTriQuyetToanNamTruoc);
                                if (fTroLyDeNghiQuyetToanNamTruoc.HasValue)
                                {
                                    result.TongHopSoLieu.fTroLyDeNghiQuyetToanNamTruoc = fTroLyDeNghiQuyetToanNamTruoc.Value;
                                }
                                else
                                {
                                    result.TongHopSoLieu.fTroLyDeNghiQuyetToanNamTruoc = 0;
                                }

                                var fTroLyDeNghiQuyetToanNamNay = listDeNghiQuyetToanNienDoTroLyChiTiet.Where(x => x.fGiaTriQuyetToanNamNay != null).Sum(x => x.fGiaTriQuyetToanNamNay);
                                if (fTroLyDeNghiQuyetToanNamNay.HasValue)
                                {
                                    result.TongHopSoLieu.fTroLyDeNghiQuyetToanNamNay = fTroLyDeNghiQuyetToanNamNay.Value;
                                }
                                else
                                {
                                    result.TongHopSoLieu.fTroLyDeNghiQuyetToanNamNay = 0;
                                }

                                var fXuLySoLieuNamNayQuyetToan = listDeNghiQuyetToanNienDoTroLyChiTiet.Where(x => x.fGiaTriQuyetToanNamNay != null).Sum(x => x.fGiaTriQuyetToanNamNay);
                                if (fXuLySoLieuNamNayQuyetToan.HasValue)
                                {
                                    if (result.XuLySoLieuNamSau == null)
                                    {
                                        result.XuLySoLieuNamSau = new VDT_QT_XuLySoLieuViewModel();
                                    }
                                    result.XuLySoLieuNamSau.fQuyetToan = fXuLySoLieuNamNayQuyetToan.Value;
                                }
                            }
                        }
                    }
                }
            }

            result.FormatNumber();

            return result;
        }

        public bool TongHopSoLieuSave(Guid? iID_TongHopSoLieu, int? iNamThucHien, Guid? iID_DonViQuanLy, int? iID_NguonVon, DateTime? dNgayLap, string typeSave, string sUserName)
        {
            var model = TongHopSoLieu(iNamThucHien, iID_DonViQuanLy, iID_NguonVon, dNgayLap);

            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                if (iID_TongHopSoLieu == null)
                {
                    var master = new VDT_QT_TongHopSoLieu();
                    master.iID_TongHopSoLieuID = Guid.NewGuid();
                    master.dDateCreate = DateTime.Now;
                    master.dDateUpdate = DateTime.Now;
                    master.sUserCreate = sUserName;
                    master.sUserUpdate = sUserName;
                    master.iNamKeHoach = iNamThucHien;
                    master.iID_DonViQuanLyID = iID_DonViQuanLy;
                    master.iID_NguonVonID = iID_NguonVon;
                    master.dNgayLap = dNgayLap;
                    conn.Insert<VDT_QT_TongHopSoLieu>(master, trans);

                    if (model.XuLySoLieuNamSau != null)
                    {
                        var xuLySoLieuNamSau = new VDT_QT_XuLySoLieu();
                        xuLySoLieuNamSau.iID_XuLySoLieu_ChiTietID = Guid.NewGuid();
                        xuLySoLieuNamSau.iID_TongHopSoLieuID = master.iID_TongHopSoLieuID;
                        xuLySoLieuNamSau.iID_NguonVonID = model.XuLySoLieuNamSau.iID_NguonVonID;
                        xuLySoLieuNamSau.iID_LoaiNguonVonID = model.XuLySoLieuNamSau.iID_LoaiNguonVonID;
                        xuLySoLieuNamSau.iID_DuAnID = model.XuLySoLieuNamSau.iID_DuAnID;
                        xuLySoLieuNamSau.iID_MucID = model.XuLySoLieuNamSau.iID_MucID;
                        xuLySoLieuNamSau.iID_TieuMucID = model.XuLySoLieuNamSau.iID_TieuMucID;
                        xuLySoLieuNamSau.iID_TietMucID = model.XuLySoLieuNamSau.iID_TietMucID;
                        xuLySoLieuNamSau.iID_NganhID = model.XuLySoLieuNamSau.iID_NganhID;
                        xuLySoLieuNamSau.fChiTieu = model.XuLySoLieuNamSau.fChiTieu;
                        xuLySoLieuNamSau.fCapPhat = model.XuLySoLieuNamSau.fCapPhat;
                        xuLySoLieuNamSau.fQuyetToan = model.XuLySoLieuNamSau.fQuyetToan;
                        xuLySoLieuNamSau.fThua = model.XuLySoLieuNamSau.fThua;
                        xuLySoLieuNamSau.fThieu = model.XuLySoLieuNamSau.fThieu;
                        xuLySoLieuNamSau.fTrongChiTieu = model.XuLySoLieuNamSau.fTrongChiTieu;
                        xuLySoLieuNamSau.fNgoaiChiTieu = model.XuLySoLieuNamSau.fNgoaiChiTieu;
                        xuLySoLieuNamSau.fCapThanhKhoan = model.XuLySoLieuNamSau.fCapThanhKhoan;
                        xuLySoLieuNamSau.fThuThanhKhoan = model.XuLySoLieuNamSau.fThuThanhKhoan;
                        xuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap = model.XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap;
                        xuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap = model.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap;
                        xuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap = model.XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap;
                        xuLySoLieuNamSau.sTrangThaiDuAnDangKy = model.XuLySoLieuNamSau.sTrangThaiDuAnDangKy;
                        xuLySoLieuNamSau.fTrongChiTieuNamTruoc = model.XuLySoLieuNamSau.fTrongChiTieuNamTruoc;
                        xuLySoLieuNamSau.fThuaNamTruoc = model.XuLySoLieuNamSau.fThuaNamTruoc;
                        xuLySoLieuNamSau.fNgoaiChiTieuNamTruoc = model.XuLySoLieuNamSau.fNgoaiChiTieuNamTruoc;
                        xuLySoLieuNamSau.fQuyetToanNamTruoc = model.XuLySoLieuNamSau.fQuyetToanNamTruoc;
                        xuLySoLieuNamSau.fChiTieuNamTruoc = model.XuLySoLieuNamSau.fChiTieuNamTruoc;
                        xuLySoLieuNamSau.fCapPhatNamTruoc = model.XuLySoLieuNamSau.fCapPhatNamTruoc;
                        xuLySoLieuNamSau.dDateCreate = DateTime.Now;
                        xuLySoLieuNamSau.dDateUpdate = DateTime.Now;
                        xuLySoLieuNamSau.sUserCreate = sUserName;
                        xuLySoLieuNamSau.sUserUpdate = sUserName;

                        if (typeSave == "SAVE")
                        {
                            xuLySoLieuNamSau.sTrangThaiDuAnDangKy = "1";
                            master.bIsCanBoDuyet = false;
                        }

                        if (typeSave == "APPROVESAVE")
                        {
                            xuLySoLieuNamSau.sTrangThaiDuAnDangKy = "2";
                            master.bIsCanBoDuyet = true;
                        }

                        conn.Insert<VDT_QT_XuLySoLieu>(xuLySoLieuNamSau, trans);
                    }
                }
                else
                {
                    var master = conn.QueryFirstOrDefault<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_TongHopSoLieuID = '{0}'", iID_TongHopSoLieu), null, trans);
                    if (master != null)
                    {
                        master.dDateUpdate = DateTime.Now;
                        master.sUserUpdate = sUserName;
                        master.iID_DonViQuanLyID = iID_DonViQuanLy;
                        master.iID_NguonVonID = iID_NguonVon;
                        master.iNamKeHoach = iNamThucHien;
                        master.dNgayLap = dNgayLap;
                        if (typeSave == "SAVE")
                        {
                            master.bIsCanBoDuyet = false;
                        }
                        if (typeSave == "APPROVESAVE")
                        {
                            master.bIsCanBoDuyet = true;
                        }
                        conn.Update<VDT_QT_TongHopSoLieu>(master, trans);
                    }

                    if (model.XuLySoLieuNamSau != null)
                    {
                        conn.Execute(string.Format("DELETE FROM VDT_QT_XuLySoLieu WHERE iID_TongHopSoLieuID = '{0}'", master.iID_TongHopSoLieuID), null, trans);
                        var xuLySoLieuNamSau = new VDT_QT_XuLySoLieu();
                        xuLySoLieuNamSau.iID_XuLySoLieu_ChiTietID = Guid.NewGuid();
                        xuLySoLieuNamSau.iID_TongHopSoLieuID = master.iID_TongHopSoLieuID;
                        xuLySoLieuNamSau.iID_NguonVonID = model.XuLySoLieuNamSau.iID_NguonVonID;
                        xuLySoLieuNamSau.iID_LoaiNguonVonID = model.XuLySoLieuNamSau.iID_LoaiNguonVonID;
                        xuLySoLieuNamSau.iID_DuAnID = model.XuLySoLieuNamSau.iID_DuAnID;
                        xuLySoLieuNamSau.iID_MucID = model.XuLySoLieuNamSau.iID_MucID;
                        xuLySoLieuNamSau.iID_TieuMucID = model.XuLySoLieuNamSau.iID_TieuMucID;
                        xuLySoLieuNamSau.iID_TietMucID = model.XuLySoLieuNamSau.iID_TietMucID;
                        xuLySoLieuNamSau.iID_NganhID = model.XuLySoLieuNamSau.iID_NganhID;
                        xuLySoLieuNamSau.fChiTieu = model.XuLySoLieuNamSau.fChiTieu;
                        xuLySoLieuNamSau.fCapPhat = model.XuLySoLieuNamSau.fCapPhat;
                        xuLySoLieuNamSau.fQuyetToan = model.XuLySoLieuNamSau.fQuyetToan;
                        xuLySoLieuNamSau.fThua = model.XuLySoLieuNamSau.fThua;
                        xuLySoLieuNamSau.fThieu = model.XuLySoLieuNamSau.fThieu;
                        xuLySoLieuNamSau.fTrongChiTieu = model.XuLySoLieuNamSau.fTrongChiTieu;
                        xuLySoLieuNamSau.fNgoaiChiTieu = model.XuLySoLieuNamSau.fNgoaiChiTieu;
                        xuLySoLieuNamSau.fCapThanhKhoan = model.XuLySoLieuNamSau.fCapThanhKhoan;
                        xuLySoLieuNamSau.fThuThanhKhoan = model.XuLySoLieuNamSau.fThuThanhKhoan;
                        xuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap = model.XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap;
                        xuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap = model.XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap;
                        xuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap = model.XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap;
                        xuLySoLieuNamSau.sTrangThaiDuAnDangKy = model.XuLySoLieuNamSau.sTrangThaiDuAnDangKy;
                        xuLySoLieuNamSau.fTrongChiTieuNamTruoc = model.XuLySoLieuNamSau.fTrongChiTieuNamTruoc;
                        xuLySoLieuNamSau.fThuaNamTruoc = model.XuLySoLieuNamSau.fThuaNamTruoc;
                        xuLySoLieuNamSau.fNgoaiChiTieuNamTruoc = model.XuLySoLieuNamSau.fNgoaiChiTieuNamTruoc;
                        xuLySoLieuNamSau.fQuyetToanNamTruoc = model.XuLySoLieuNamSau.fQuyetToanNamTruoc;
                        xuLySoLieuNamSau.fChiTieuNamTruoc = model.XuLySoLieuNamSau.fChiTieuNamTruoc;
                        xuLySoLieuNamSau.fCapPhatNamTruoc = model.XuLySoLieuNamSau.fCapPhatNamTruoc;
                        xuLySoLieuNamSau.dDateCreate = DateTime.Now;
                        xuLySoLieuNamSau.dDateUpdate = DateTime.Now;
                        xuLySoLieuNamSau.sUserCreate = sUserName;
                        xuLySoLieuNamSau.sUserUpdate = sUserName;

                        if (typeSave == "SAVE")
                        {
                            xuLySoLieuNamSau.sTrangThaiDuAnDangKy = "1";
                            master.bIsCanBoDuyet = false;
                        }

                        if (typeSave == "APPROVESAVE")
                        {
                            xuLySoLieuNamSau.sTrangThaiDuAnDangKy = "2";
                            master.bIsCanBoDuyet = true;
                        }
                        conn.Insert<VDT_QT_XuLySoLieu>(xuLySoLieuNamSau, trans);
                    }
                }

                trans.Commit();
                conn.Close();
            }

            return true;
        }

        public bool DeleteTongHopSoLieu(string id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var model = conn.QueryFirstOrDefault<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (model != null)
                {
                    conn.Delete(model);
                }

                var listChiTiet = conn.Query<VDT_QT_TongHopSoLieu_ChiTiet>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu_ChiTiet WHERE iID_TongHopSoLieuID = '{0}'"), id);
                if (listChiTiet != null && listChiTiet.Any())
                {
                    foreach (var item in listChiTiet)
                    {
                        conn.Delete(item);
                    }
                }

                var listXuLySoLieu = conn.Query<VDT_QT_XuLySoLieu>(string.Format("SELECT * FROM VDT_QT_XuLySoLieu WHERE iID_TongHopSoLieuID = '{0}'", id));
                if (listXuLySoLieu != null && listXuLySoLieu.Any())
                {
                    foreach (var item in listXuLySoLieu)
                    {
                        conn.Delete(item);
                    }
                }
            }

            return true;
        }
        #endregion

        #region Vốn đầu từ - quyết toán - thông tri
        public List<NS_DonVi> GetListDataDonViByUser(string sUserName)
        {
            var result = new List<NS_DonVi>();
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listPhongBanDonVi = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE sID_MaNguoiDung_DuocGiao = '{0}' AND iNamLamViec = '{1}' AND iTrangThai = 1", sUserName, config.iNamLamViec), null, commandType: CommandType.Text);
                if (listPhongBanDonVi == null || !listPhongBanDonVi.Any())
                {
                    return result;
                }

                var listDonVi = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = '{0}' AND iTrangThai = 1", config.iNamLamViec));
                var listIDMaDonVi = listPhongBanDonVi.Select(x => x.iID_MaDonVi).ToList();
                if (listIDMaDonVi != null && listIDMaDonVi.Any())
                {
                    if (listDonVi != null && listDonVi.Any())
                    {
                        result = listDonVi.Where(x => listIDMaDonVi.Contains(x.iID_MaDonVi)).ToList();
                    }
                }
            }

            return result;
        }

        public IEnumerable<VDT_ThongTriViewModel> GetAllThongTriPaging(ref PagingInfo _paging, string sUserName, Guid? iID_DonViQuanLy = null, string sMaThongTri = "", DateTime? dNgayTaoThongTri = null, int? iNamThucHien = null, string sNguoiLap = "", string sTruongPhong = "", string sThuTruongDonVi = "")
        {
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("iID_DonViQuanLy", iID_DonViQuanLy);
                lstParam.Add("sMaThongTri", sMaThongTri);
                lstParam.Add("dNgayTaoThongTri", dNgayTaoThongTri);
                lstParam.Add("iNamThucHien", iNamThucHien);
                lstParam.Add("sNguoiLap", sNguoiLap);
                lstParam.Add("sTruongPhong", sTruongPhong);
                lstParam.Add("sThuTruongDonVi", sThuTruongDonVi);
                lstParam.Add("iNamLamViec", config.iNamLamViec);
                lstParam.Add("sUserName", sUserName);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_ThongTriViewModel>("proc_get_all_vdt_qt_thongtri_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }
        public List<NS_NguonNganSach> GetListAllNguonNganSach()
        {
            var result = new List<NS_NguonNganSach>();
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_NguonNganSach>("SELECT * FROM NS_NguonNganSach");
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }
            return result;
        }
        public List<VDT_DM_LoaiCongTrinh> GetListDMLoaiCongTrinh()
        {
            var result = new List<VDT_DM_LoaiCongTrinh>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_LoaiCongTrinh>("SELECT * FROM VDT_DM_LoaiCongTrinh");
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }

            return result;
        }

        public VDT_ThongTriFilterResultModel GetDataThongTriQuyetToanTheoFilter(VDT_ThongTriFilterModel model, string sUserName)
        {
            var result = new VDT_ThongTriFilterResultModel();

            if (model.iID_DonViQuanLy == null || model.iID_DonViQuanLy == Guid.Empty || model.NamThucHien == null || string.IsNullOrEmpty(model.iID_NguonVon))
            {
                return result;
            }

            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listTroLy = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy WHERE iID_DonViDeNghiID = '{0}' AND iID_NguonVonID = '{1}' AND iNamKeHoach = '{2}'", model.iID_DonViQuanLy, model.iID_NguonVon, model.NamThucHien));
                var listTroLyChiTiet = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet");
                var listMucLucNganSach = conn.Query<NS_MucLucNganSach>(string.Format("SELECT * FROM NS_MucLucNganSach WHERE iTrangThai = 1 AND iNamLamViec = '{0}'", config.iNamLamViec));
                var listLoaiCongTrinh = conn.Query<VDT_DM_LoaiCongTrinh>("SELECT * FROM VDT_DM_LoaiCongTrinh");
                var listDuAn = conn.Query<VDT_DA_DuAn>("SELECT * FROM VDT_DA_DuAn");
                var listXuLySoLieu = new List<VDT_QT_XuLySoLieu>();
                var listXuLySoLieuAll = conn.Query<VDT_QT_XuLySoLieu>("SELECT * FROM VDT_QT_XuLySoLieu");
                var listTongHopSoLieu = conn.Query<VDT_QT_TongHopSoLieu>(string.Format("SELECT * FROM VDT_QT_TongHopSoLieu WHERE iID_DonViQuanLyID = '{0}' AND iNamKeHoach = '{1}' AND iID_NguonVonID = '{2}'", model.iID_DonViQuanLy, model.NamThucHien, model.iID_NguonVon));
                var listKhoiTaoChiTietAll = conn.Query<VDT_KT_KhoiTao_ChiTiet>("SELECT * FROM VDT_KT_KhoiTao_ChiTiet");
                var listKhoiTaoAll = conn.Query<VDT_KT_KhoiTao>("SELECT * FROM VDT_KT_KhoiTao");
                if (listTongHopSoLieu != null && listTongHopSoLieu.Any())
                {
                    var listIdTongHopSoLieu = listTongHopSoLieu.Select(x => x.iID_TongHopSoLieuID).ToList();
                    if (listXuLySoLieuAll != null && listXuLySoLieuAll.Any())
                    {
                        listXuLySoLieu = listXuLySoLieuAll.Where(x => x.iID_TongHopSoLieuID != null && listIdTongHopSoLieu.Contains(x.iID_TongHopSoLieuID.Value)).ToList();
                    }
                }
                var listIdTroLy = new List<Guid>();
                if (listTroLy != null && listTroLy.Any())
                {
                    listIdTroLy = listTroLy.Select(x => x.iID_DeNghiQuyetToanNienDoID).ToList();
                }
                if (listTroLyChiTiet != null && listTroLyChiTiet.Any())
                {
                    var listChiTiet = listTroLyChiTiet.Where(x => x.iID_DeNghiQuyetToanNienDoID != null && listIdTroLy.Contains(x.iID_DeNghiQuyetToanNienDoID.Value)).ToList();
                    if (listChiTiet != null && listChiTiet.Any())
                    {
                        foreach (var item in listChiTiet)
                        {
                            var quyetToanKinhPhiNamTruoc = new VDT_ThongTriFilterChiTiet();
                            var quyetToanKinhPhiNamSau = new VDT_ThongTriFilterChiTiet();
                            var thuKinhPhiChuyenNamSau = new VDT_ThongTriFilterChiTiet();
                            var capThanhKhoan = new VDT_ThongTriFilterChiTiet();
                            var thuThanhKhoan = new VDT_ThongTriFilterChiTiet();
                            var capKinhPhiChuyenSang = new VDT_ThongTriFilterChiTiet();
                            var thuNopNganSach = new VDT_ThongTriFilterChiTiet();
                            if (listMucLucNganSach != null && listMucLucNganSach.Any())
                            {
                                var mucLuc = listMucLucNganSach.FirstOrDefault(x => x.iID_MaMucLucNganSach == item.iID_NganhID);
                                if (mucLuc != null)
                                {
                                    quyetToanKinhPhiNamTruoc.Muc = mucLuc.sM;
                                    quyetToanKinhPhiNamTruoc.TieuMuc = mucLuc.sTM;
                                    quyetToanKinhPhiNamTruoc.TietMuc = mucLuc.sTTM;
                                    quyetToanKinhPhiNamTruoc.Nganh = mucLuc.sNG;

                                    quyetToanKinhPhiNamSau.Muc = mucLuc.sM;
                                    quyetToanKinhPhiNamSau.TieuMuc = mucLuc.sTM;
                                    quyetToanKinhPhiNamSau.TietMuc = mucLuc.sTTM;
                                    quyetToanKinhPhiNamSau.Nganh = mucLuc.sNG;

                                    thuKinhPhiChuyenNamSau.Muc = mucLuc.sM;
                                    thuKinhPhiChuyenNamSau.TieuMuc = mucLuc.sTM;
                                    thuKinhPhiChuyenNamSau.TietMuc = mucLuc.sTTM;
                                    thuKinhPhiChuyenNamSau.Nganh = mucLuc.sNG;

                                    capThanhKhoan.Muc = mucLuc.sM;
                                    capThanhKhoan.TieuMuc = mucLuc.sTM;
                                    capThanhKhoan.TietMuc = mucLuc.sTTM;
                                    capThanhKhoan.Nganh = mucLuc.sNG;

                                    thuThanhKhoan.Muc = mucLuc.sM;
                                    thuThanhKhoan.TieuMuc = mucLuc.sTM;
                                    thuThanhKhoan.TietMuc = mucLuc.sTTM;
                                    thuThanhKhoan.Nganh = mucLuc.sNG;

                                    capKinhPhiChuyenSang.Muc = mucLuc.sM;
                                    capKinhPhiChuyenSang.TieuMuc = mucLuc.sTM;
                                    capKinhPhiChuyenSang.TietMuc = mucLuc.sTTM;
                                    capKinhPhiChuyenSang.Nganh = mucLuc.sNG;

                                    thuNopNganSach.Muc = mucLuc.sM;
                                    thuNopNganSach.TieuMuc = mucLuc.sTM;
                                    thuNopNganSach.TietMuc = mucLuc.sTTM;
                                    thuNopNganSach.Nganh = mucLuc.sNG;
                                }
                            }

                            if (listDuAn != null && listDuAn.Any())
                            {
                                var duAn = listDuAn.FirstOrDefault(x => x.iID_DuAnID == item.iID_DuAnID);
                                if (duAn != null)
                                {
                                    quyetToanKinhPhiNamTruoc.iID_DuAn = duAn.iID_DuAnID;
                                    quyetToanKinhPhiNamTruoc.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    quyetToanKinhPhiNamSau.iID_DuAn = duAn.iID_DuAnID;
                                    quyetToanKinhPhiNamSau.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    thuKinhPhiChuyenNamSau.iID_DuAn = duAn.iID_DuAnID;
                                    thuKinhPhiChuyenNamSau.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    capThanhKhoan.iID_DuAn = duAn.iID_DuAnID;
                                    capThanhKhoan.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    thuThanhKhoan.iID_DuAn = duAn.iID_DuAnID;
                                    thuThanhKhoan.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    capKinhPhiChuyenSang.iID_DuAn = duAn.iID_DuAnID;
                                    capKinhPhiChuyenSang.iID_NhaThau = duAn.iID_ChuDauTuID;
                                    thuNopNganSach.iID_DuAn = duAn.iID_DuAnID;
                                    thuNopNganSach.iID_NhaThau = duAn.iID_ChuDauTuID;

                                    if (listLoaiCongTrinh != null && listLoaiCongTrinh.Any())
                                    {
                                        var loaiCongTrinh = listLoaiCongTrinh.FirstOrDefault(x => x.iID_LoaiCongTrinh == duAn.iID_LoaiCongTrinhID);
                                        if (loaiCongTrinh != null)
                                        {
                                            quyetToanKinhPhiNamTruoc.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            quyetToanKinhPhiNamSau.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            thuKinhPhiChuyenNamSau.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            capThanhKhoan.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            thuThanhKhoan.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            capKinhPhiChuyenSang.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;
                                            thuNopNganSach.iID_LoaiCongTrinh = loaiCongTrinh.iID_LoaiCongTrinh;

                                            var loaiNguonVon = new NS_MucLucNganSach();
                                            if (listMucLucNganSach != null && listMucLucNganSach.Any())
                                            {
                                                if (listTroLy != null && listTroLy.Any())
                                                {
                                                    var troLy = listTroLy.FirstOrDefault(x => x.iID_DeNghiQuyetToanNienDoID == item.iID_DeNghiQuyetToanNienDoID);
                                                    if (troLy != null && troLy.iID_LoaiNguonVonID.HasValue)
                                                    {
                                                        loaiNguonVon = listMucLucNganSach.FirstOrDefault(x => x.iID_MaMucLucNganSach == troLy.iID_LoaiNguonVonID);
                                                    }
                                                }
                                            }

                                            if (loaiNguonVon == null || string.IsNullOrEmpty(loaiNguonVon.sMoTa))
                                            {
                                                quyetToanKinhPhiNamTruoc.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                quyetToanKinhPhiNamSau.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                thuKinhPhiChuyenNamSau.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                capThanhKhoan.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n-  Dự án : {duAn.sTenDuAn}";
                                                thuThanhKhoan.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                capKinhPhiChuyenSang.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                thuNopNganSach.NoiDung = $"- Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} /n- Dự án : {duAn.sTenDuAn}";
                                            }
                                            else
                                            {
                                                quyetToanKinhPhiNamTruoc.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                quyetToanKinhPhiNamSau.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                thuKinhPhiChuyenNamSau.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                capThanhKhoan.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n-  Dự án : {duAn.sTenDuAn}";
                                                thuThanhKhoan.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                capKinhPhiChuyenSang.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} \n- Dự án : {duAn.sTenDuAn}";
                                                thuNopNganSach.NoiDung = $"- {loaiNguonVon.sMoTa} - Công trình : {loaiCongTrinh.sTenLoaiCongTrinh} /n- Dự án : {duAn.sTenDuAn}";
                                            }
                                        }
                                    }

                                    if (config.iNamLamViec == (model.NamThucHien + 1))
                                    {
                                        if (listKhoiTaoAll != null && listKhoiTaoAll.Any())
                                        {
                                            var listKhoiTao = listKhoiTaoAll.Where(x => x.iID_DuAnID == item.iID_DuAnID).ToList();
                                            if (listKhoiTao != null && listKhoiTao.Any())
                                            {
                                                var listIdKhoiTao = listKhoiTao.Select(x => x.iID_KhoiTaoID).ToList();
                                                if (listKhoiTaoChiTietAll != null && listKhoiTaoChiTietAll.Any())
                                                {
                                                    var listKhoiTaoChiTiet = listKhoiTaoChiTietAll.Where(x => listIdKhoiTao.Contains(x.iID_KhoiTaoID) &&
                                                                                                              x.iID_NganhID == item.iID_NganhID &&
                                                                                                              x.iID_NguonVonID == int.Parse(model.iID_NguonVon)).ToList();
                                                    if (listKhoiTaoChiTiet != null && listKhoiTaoChiTiet.Any())
                                                    {
                                                        capKinhPhiChuyenSang.SoTien = listKhoiTaoChiTiet.Sum(x => x.fLuyKeThanhToanKLHT);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var tongHopSoLieu = listTongHopSoLieu.FirstOrDefault(x => x.iID_DonViQuanLyID == model.iID_DonViQuanLy &&
                                                                                                  x.iNamKeHoach == model.NamThucHien - 1 &&
                                                                                                  x.iID_NguonVonID == int.Parse(model.iID_NguonVon));
                                        if (tongHopSoLieu != null)
                                        {
                                            if (listXuLySoLieuAll != null && listXuLySoLieuAll.Any())
                                            {
                                                var listXuLySoLieuCapKinhPhiChuyenSang = listXuLySoLieuAll.Where(x => x.iID_TongHopSoLieuID == tongHopSoLieu.iID_TongHopSoLieuID &&
                                                                                                                      x.iID_DuAnID == item.iID_DuAnID &&
                                                                                                                      x.iID_NganhID == item.iID_NganhID).ToList();

                                                if (listXuLySoLieuCapKinhPhiChuyenSang != null && listXuLySoLieuCapKinhPhiChuyenSang.Any())
                                                {
                                                    capKinhPhiChuyenSang.SoTien = listXuLySoLieuCapKinhPhiChuyenSang.Sum(x => x.fGiaTriChuyenNamSauDaCap + x.fGiaTriNamTruocChuyenNamSauDaCap);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (item.fGiaTriQuyetToanNamTruoc.HasValue)
                            {
                                quyetToanKinhPhiNamTruoc.SoTien = item.fGiaTriQuyetToanNamTruoc;
                                quyetToanKinhPhiNamSau.SoTien = item.fGiaTriQuyetToanNamNay;

                                if (thuKinhPhiChuyenNamSau.iID_DuAn != null)
                                {
                                    if (listXuLySoLieu.Any())
                                    {
                                        var xuLySoLieu = listXuLySoLieu.FirstOrDefault(x => x.iID_DuAnID == thuKinhPhiChuyenNamSau.iID_DuAn);
                                        if (xuLySoLieu != null)
                                        {
                                            thuKinhPhiChuyenNamSau.SoTien = xuLySoLieu.fGiaTriChuyenNamSauDaCap + xuLySoLieu.fGiaTriChuyenNamSauChuaCap;
                                            capThanhKhoan.SoTien = xuLySoLieu.fCapThanhKhoan;
                                            thuThanhKhoan.SoTien = xuLySoLieu.fThuThanhKhoan + xuLySoLieu.fThuThanhKhoanNamTruoc;
                                            thuNopNganSach.SoTien = xuLySoLieu.fThuLaiKeHoachNamTruoc + xuLySoLieu.fThuLaiKeHoachNamNay;
                                        }
                                    }
                                }
                            }

                            result.ListQuyetToanKinhPhiNamTruoc.Add(quyetToanKinhPhiNamTruoc);
                            result.ListQuyetToanKinhPhiNamSau.Add(quyetToanKinhPhiNamSau);
                            result.ListThuKinhPhiChuyenNamSau.Add(thuKinhPhiChuyenNamSau);
                            result.ListCapThanhKhoan.Add(capThanhKhoan);
                            result.ListThuThanhKhoan.Add(thuThanhKhoan);
                            result.ListCapKinhPhiChuyenSang.Add(capKinhPhiChuyenSang);
                            result.ListThuNopNganSach.Add(thuNopNganSach);
                        }
                    }
                }
            }

            if (result.ListQuyetToanKinhPhiNamTruoc.Any())
            {
                result.TongTienQuyetToanKinhPhiNamTruoc = result.ListQuyetToanKinhPhiNamTruoc.Sum(x => x.SoTien);
                result.TongTienQuyetToanKinhPhiNamSau = result.ListQuyetToanKinhPhiNamSau.Sum(x => x.SoTien);
                result.TongTienThuKinhPhiChuyenNamSau = result.ListThuKinhPhiChuyenNamSau.Sum(x => x.SoTien);
                result.TongTienCapThanhKhoan = result.ListCapThanhKhoan.Sum(x => x.SoTien);
                result.TongTienThuThanhKhoan = result.ListThuThanhKhoan.Sum(x => x.SoTien);
                result.TongTienCapKinhPhiChuyenSang = result.ListCapKinhPhiChuyenSang.Sum(x => x.SoTien);
                result.TongTienThuNopNganSach = result.ListThuNopNganSach.Sum(x => x.SoTien);
            }

            result.FormatNumber();

            return result;
        }

        public VDT_ThongTri FindThongTriById(Guid iIdThongTri)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Get<VDT_ThongTri>(iIdThongTri);
            }
        }

        public VDT_ThongTriViewModel GetThongTriById(string id)
        {
            var result = new VDT_ThongTriViewModel();

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri WHERE iID_ThongTriID = '{0}'", id));
                if (item != null)
                {
                    result.iID_ThongTriID = item.iID_ThongTriID;
                    result.iNamThongTri = item.iNamThongTri;
                    result.iID_DonViID = item.iID_DonViID;
                    if (item.iID_DonViID.HasValue)
                    {
                        var donVi = conn.QueryFirstOrDefault<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iID_Ma = '{0}'", item.iID_DonViID));
                        if (donVi != null)
                        {
                            result.TenDonVi = donVi.sTen;
                        }
                    }
                    result.dNgayThongTri = item.dNgayThongTri;
                    result.sMaLoaiCongTrinh = item.sMaLoaiCongTrinh;
                    if (!string.IsNullOrEmpty(item.sMaLoaiCongTrinh))
                    {
                        var congtrinh = conn.QueryFirstOrDefault<VDT_DM_LoaiCongTrinh>(string.Format("SELECT * FROM VDT_DM_LoaiCongTrinh WHERE sMaLoaiCongTrinh = '{0}'", item.sMaLoaiCongTrinh));
                        if (congtrinh != null)
                        {
                            result.iID_LoaiCongTrinh = congtrinh.iID_LoaiCongTrinh;
                            result.sTenLoaiCongTrinh = congtrinh.sTenLoaiCongTrinh;
                        }
                    }
                    result.sMaNguonVon = item.sMaNguonVon;
                    if (!string.IsNullOrEmpty(item.sMaNguonVon))
                    {
                        var iID_NguonVon = int.Parse(item.sMaNguonVon);
                        var sql = new StringBuilder();
                        sql.AppendFormat("SELECT * FROM NS_NguonNganSach WHERE iID_MaNguonNganSach = {0}", iID_NguonVon);
                        var nguonVon = conn.QueryFirstOrDefault<NS_NguonNganSach>(sql.ToString());
                        if (nguonVon != null)
                        {
                            result.TenNguonVon = nguonVon.sTen;
                        }
                    }
                    result.sMaThongTri = item.sMaThongTri;
                    result.sNguoiLap = item.sNguoiLap;
                    result.sThuTruongDonVi = item.sThuTruongDonVi;
                    result.sTruongPhong = item.sTruongPhong;
                }
            }

            return result;
        }

        public bool SaveThongTri(VDT_ThongTriFilterModel aModel, string sUserName)
        {
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listTroLy = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy>(string.Format("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy WHERE iID_DonViDeNghiID = '{0}' AND iID_NguonVonID = '{1}' AND iNamKeHoach = '{2}'", aModel.iID_DonViQuanLy, aModel.iID_NguonVon, aModel.NamThucHien));
                var listTroLyChiTietAll = conn.Query<VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet>("SELECT * FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet");
                var listMucLucNganSach = conn.Query<NS_MucLucNganSach>(string.Format("SELECT * FROM NS_MucLucNganSach WHERE iTrangThai = 1 AND iNamLamViec = '{0}'", config.iNamLamViec));
                var listLoaiCongTrinh = conn.Query<VDT_DM_LoaiCongTrinh>("SELECT * FROM VDT_DM_LoaiCongTrinh");
                var listDuAn = conn.Query<VDT_DA_DuAn>("SELECT * FROM VDT_DA_DuAn");
                var listLoaiThongTri = conn.Query<VDT_DM_LoaiThongTri>("SELECT * FROM VDT_DM_LoaiThongTri");

                var congTrinh = new VDT_DM_LoaiCongTrinh();
                if (!string.IsNullOrEmpty(aModel.iID_LoaiCongTrinh))
                {
                    congTrinh = conn.QueryFirstOrDefault<VDT_DM_LoaiCongTrinh>(string.Format("SELECT * FROM VDT_DM_LoaiCongTrinh WHERE iID_LoaiCongTrinh = '{0}'", aModel.iID_LoaiCongTrinh));
                }
                var nguonVon = new NS_NguonNganSach();
                if (!string.IsNullOrEmpty(aModel.iID_NguonVon))
                {
                    var iID_NguonVon = int.Parse(aModel.iID_NguonVon);
                    var sql = new StringBuilder();
                    sql.AppendFormat("SELECT * FROM NS_NguonNganSach WHERE iID_MaNguonNganSach = {0}", iID_NguonVon);
                    nguonVon = conn.QueryFirstOrDefault<NS_NguonNganSach>(sql.ToString());
                }
                var listCongTrinh = conn.Query<VDT_DM_LoaiCongTrinh>("SELECT * FROM VDT_DM_LoaiCongTrinh");
                var modelFilterChiTiet = GetDataThongTriQuyetToanTheoFilter(aModel, sUserName);
                var loaiThongTri = new VDT_DM_LoaiThongTri();
                if (listLoaiThongTri != null && listLoaiThongTri.Any())
                {
                    loaiThongTri = listLoaiThongTri.FirstOrDefault(x => x.iKieuLoaiThongTri == aModel.KieuThongTri);
                }
                var listKieuThongTri = conn.Query<VDT_DM_KieuThongTri>("SELECT * FROM VDT_DM_KieuThongTri");
                if (string.IsNullOrEmpty(aModel.iID_ThongTriID))
                {
                    var model = new VDT_ThongTri();
                    model.iID_ThongTriID = Guid.NewGuid();
                    model.sMaThongTri = aModel.MaThongTri;
                    if (loaiThongTri != null)
                    {
                        model.iID_LoaiThongTriID = loaiThongTri.iID_LoaiThongTriID;
                    }
                    model.dNgayThongTri = aModel.NgayTaoThongTri;
                    model.iNamThongTri = aModel.NamThucHien;
                    model.sNguoiLap = aModel.NguoiLapThongTri;
                    model.sTruongPhong = aModel.TruongPhongTaiChinh;
                    model.sThuTruongDonVi = aModel.TruongPhongTaiChinh;
                    model.sMaNguonVon = nguonVon.iID_MaNguonNganSach.ToString();
                    model.iID_DonViID = aModel.iID_DonViQuanLy;
                    model.sMaLoaiCongTrinh = congTrinh.sMaLoaiCongTrinh;
                    model.dDateCreate = DateTime.Now;
                    model.dDateUpdate = DateTime.Now;
                    model.sUserCreate = sUserName;
                    model.sUserUpdate = sUserName;

                    var listChiTiet = new List<VDT_ThongTri_ChiTiet>();
                    if (modelFilterChiTiet != null)
                    {
                        if (modelFilterChiTiet.ListCapKinhPhiChuyenSang != null && modelFilterChiTiet.ListCapKinhPhiChuyenSang.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListCapKinhPhiChuyenSang)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListCapThanhKhoan != null && modelFilterChiTiet.ListCapThanhKhoan.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListCapThanhKhoan)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListQuyetToanKinhPhiNamSau != null && modelFilterChiTiet.ListQuyetToanKinhPhiNamSau.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListQuyetToanKinhPhiNamSau)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc != null && modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuKinhPhiChuyenNamSau != null && modelFilterChiTiet.ListThuKinhPhiChuyenNamSau.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListThuKinhPhiChuyenNamSau)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuNopNganSach != null && modelFilterChiTiet.ListThuNopNganSach.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListThuNopNganSach)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuThanhKhoan != null && modelFilterChiTiet.ListThuThanhKhoan.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListThuThanhKhoan)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTiet.Add(chiTiet);
                            }
                        }
                    }
                    if (listChiTiet.Any())
                    {
                        conn.Insert<VDT_ThongTri_ChiTiet>(listChiTiet);
                    }

                    conn.Insert<VDT_ThongTri>(model);
                }
                else
                {
                    var model = conn.QueryFirstOrDefault<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri WHERE iID_ThongTriID = '{0}'", aModel.iID_ThongTriID));
                    if (model != null)
                    {
                        model.sMaThongTri = aModel.MaThongTri;
                        model.dNgayThongTri = aModel.NgayTaoThongTri;
                        model.iNamThongTri = aModel.NamThucHien;
                        model.sNguoiLap = aModel.NguoiLapThongTri;
                        model.sTruongPhong = aModel.TruongPhongTaiChinh;
                        model.sThuTruongDonVi = aModel.ThuTruongDonVi;
                        model.iID_DonViID = aModel.iID_DonViQuanLy;
                        model.sMaLoaiCongTrinh = congTrinh.sMaLoaiCongTrinh;
                        model.sMaNguonVon = nguonVon.iID_MaNguonNganSach.ToString();
                        model.dDateUpdate = DateTime.Now;
                        model.sUserUpdate = sUserName;

                        conn.Update<VDT_ThongTri>(model);
                    }

                    var listThongTriChiTietEdit = conn.Query<VDT_ThongTri_ChiTiet>(string.Format("SELECT * FROM VDT_ThongTri_ChiTiet WHERE iID_ThongTriID = '{0}'", aModel.iID_ThongTriID));
                    var listChiTietInsert = new List<VDT_ThongTri_ChiTiet>();
                    if (modelFilterChiTiet != null)
                    {
                        if (modelFilterChiTiet.ListCapKinhPhiChuyenSang != null && modelFilterChiTiet.ListCapKinhPhiChuyenSang.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListCapKinhPhiChuyenSang)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListCapThanhKhoan != null && modelFilterChiTiet.ListCapThanhKhoan.Any())
                        {
                            var kieuThongTri = new VDT_DM_KieuThongTri();
                            if (listKieuThongTri != null && listKieuThongTri.Any())
                            {
                                kieuThongTri = listKieuThongTri.FirstOrDefault(x => x.sMaKieuThongTri == "CTK");
                            }
                            foreach (var item in modelFilterChiTiet.ListCapThanhKhoan)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                if (kieuThongTri != null)
                                {
                                    chiTiet.iID_KieuThongTriID = kieuThongTri.iID_KieuThongTriID;
                                }
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListQuyetToanKinhPhiNamSau != null && modelFilterChiTiet.ListQuyetToanKinhPhiNamSau.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListQuyetToanKinhPhiNamSau)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc != null && modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListQuyetToanKinhPhiNamTruoc)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuKinhPhiChuyenNamSau != null && modelFilterChiTiet.ListThuKinhPhiChuyenNamSau.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListThuKinhPhiChuyenNamSau)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuNopNganSach != null && modelFilterChiTiet.ListThuNopNganSach.Any())
                        {
                            foreach (var item in modelFilterChiTiet.ListThuNopNganSach)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (modelFilterChiTiet.ListThuThanhKhoan != null && modelFilterChiTiet.ListThuThanhKhoan.Any())
                        {
                            var kieuThongTri = new VDT_DM_KieuThongTri();
                            if (listKieuThongTri != null && listKieuThongTri.Any())
                            {
                                kieuThongTri = listKieuThongTri.FirstOrDefault(x => x.sMaKieuThongTri == "TTK");
                            }
                            foreach (var item in modelFilterChiTiet.ListThuThanhKhoan)
                            {
                                var chiTiet = new VDT_ThongTri_ChiTiet();
                                chiTiet.iID_ThongTriChiTietID = Guid.NewGuid();
                                chiTiet.iID_ThongTriID = model.iID_ThongTriID;
                                chiTiet.sSoThongTri = model.sMaThongTri;
                                chiTiet.fSoTien = item.SoTien;
                                chiTiet.iID_LoaiCongTrinhID = item.iID_LoaiCongTrinh;
                                chiTiet.iID_DuAnID = item.iID_DuAn;
                                chiTiet.iID_NhaThauID = item.iID_NhaThau;
                                if (kieuThongTri != null)
                                {
                                    chiTiet.iID_KieuThongTriID = kieuThongTri.iID_KieuThongTriID;
                                }
                                listChiTietInsert.Add(chiTiet);
                            }
                        }

                        if (listChiTietInsert.Any())
                        {
                            conn.Insert<VDT_ThongTri_ChiTiet>(listChiTietInsert);
                        }

                        if (listThongTriChiTietEdit != null && listThongTriChiTietEdit.Any())
                        {
                            conn.Delete<VDT_ThongTri_ChiTiet>(listThongTriChiTietEdit);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// luu thong tri thanh toan
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        public Dictionary<bool,Guid> InsertThongTriThanhToan(VDTThongTriModel model, string sUserName, List<Guid> lstGuidChecked)
        {
            VDT_ThongTri modelNew;
            var iID = Guid.Empty;

            Dictionary<bool, Guid> result = new Dictionary<bool, Guid>();
            var status = false;

            result.Add(status, iID);

            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    modelNew = new VDT_ThongTri();
                    if (model.iID_ThongTriID == Guid.Empty)
                    {

                        var loaiThongTri = LayLoaiThongTriTheoKieuThongTri((int)Constants.KIEU_LOAI_THONG_TRI.THANH_TOAN);
                        if (loaiThongTri == null)
                            return result;

                        string sMaNhomQuanLy = string.Empty;
                        if (int.Parse(model.sMaNguonVon) == (int)Constants.NS_NGUON_NGAN_SACH.NS_QUOC_PHONG)
                            sMaNhomQuanLy = "CTC";
                        else if (int.Parse(model.sMaNguonVon) == (int)Constants.NS_NGUON_NGAN_SACH.NS_NHA_NUOC
                            || int.Parse(model.sMaNguonVon) == (int)Constants.NS_NGUON_NGAN_SACH.NS_DAC_BIET
                            || int.Parse(model.sMaNguonVon) == (int)Constants.NS_NGUON_NGAN_SACH.NS_KHAC)
                            sMaNhomQuanLy = "CKHDT";

                        //VDT_DM_NhomQuanLy objNhomQuanLy = GetNhomQuanLyBysMaNhom(sMaNhomQuanLy);
                        //if (objNhomQuanLy == null)
                        //    return false;


                        #region Them moi VDT_ThongTri
                        var entity = new VDT_ThongTri();
                        entity.sMaThongTri = model.sMaThongTri;
                        entity.dNgayThongTri = model.dNgayThongTri;
                        entity.iNamThongTri = model.iNamThongTri;
                        entity.sNguoiLap = model.sNguoiLap;
                        entity.sTruongPhong = model.sTruongPhong;
                        entity.sThuTruongDonVi = model.sThuTruongDonVi;
                        entity.sMaNguonVon = model.sMaNguonVon;
                        entity.iID_DonViID = model.iID_DonViID;
                        entity.iID_LoaiThongTriID = loaiThongTri.iID_LoaiThongTriID;
                        entity.bThanhToan = model.bThanhToan;
                        entity.bIsCanBoDuyet = true;
                        //entity.iID_NhomQuanLyID = objNhomQuanLy.iID_NhomQuanLyID;
                        entity.sUserCreate = sUserName;
                        entity.dDateCreate = DateTime.Now;

                        entity.iLoaiThongTri = model.iLoaiThongTri;
                        entity.sMoTa = model.sMoTa;


                        conn.Insert(entity, trans);
                        modelNew = entity;
                        iID = entity.iID_ThongTriID;
                        #endregion
                    }
                    else
                    {
                        #region Sua VDT_ThongTri
                        var entity = conn.Get<VDT_ThongTri>(model.iID_ThongTriID, trans);
                        if (entity == null)
                            return result;
                        entity.sMaThongTri = model.sMaThongTri;
                        entity.sNguoiLap = model.sNguoiLap;
                        entity.sTruongPhong = model.sTruongPhong;
                        entity.sThuTruongDonVi = model.sThuTruongDonVi;

                        entity.sUserUpdate = sUserName;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);

                        modelNew = entity;
                        iID = entity.iID_ThongTriID;
                        #endregion
                    }

                    int iThanhToan = modelNew.bThanhToan.Value ? 1 : 0;
                    // commit to db
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return result;
            }

            if (modelNew != null)
            {

                try
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        var trans = conn.BeginTransaction();

                        lstGuidChecked.ForEach((deNgheThanhToanId) =>
                        {
                            var entity = conn.Get<VDT_TT_DeNghiThanhToan>(deNgheThanhToanId, trans);
                            entity.iID_ThongTriThanhToanID = modelNew.iID_ThongTriID;
                            conn.Update(entity, trans);
                        });
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return result;
                }
            }

            result.Remove(status);
            result.Add(true, iID);
            return result;
        }



        public bool DeleteThongTri(string id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var model = conn.QueryFirstOrDefault<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri WHERE iID_ThongTriID = '{0}'", id));
                if (model != null)
                {
                    var listChiTiet = conn.Query<VDT_ThongTri_ChiTiet>(string.Format("SELECT * FROM VDT_ThongTri_ChiTiet WHERE iID_ThongTriID = '{0}'", id));
                    if (listChiTiet != null && listChiTiet.Any())
                    {
                        foreach (var item in listChiTiet)
                        {
                            conn.Delete<VDT_ThongTri_ChiTiet>(item);
                        }
                    }
                    conn.Delete<VDT_ThongTri>(model);
                }
            }

            return true;
        }

        public bool CheckExistMaThongTri(string id, string sMaThongTri)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                if (string.IsNullOrEmpty(id))
                {
                    var model = conn.Query<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri WHERE sMaThongTri = '{0}'", sMaThongTri));
                    if (model != null && model.Any())
                    {
                        return false;
                    }
                }
                else
                {
                    var model = conn.Query<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri WHERE iID_ThongTriID != '{0}' AND sMaThongTri = '{1}'", id, sMaThongTri));
                    if (model != null && model.Any())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public VDT_ThongTriExportDataModel ExportData(string sUserName, Guid? iID_DonViQuanLy, string sMaThongTri, DateTime? dNgayTaoThongTri, int? iNamThucHien, string sNguoiLap, string sTruongPhong, string sThuTruongDonVi)
        {
            var result = new VDT_ThongTriExportDataModel();

            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var listThongTri = conn.Query<VDT_ThongTri>(string.Format("SELECT * FROM VDT_ThongTri"));
                var listThongTriChiTiet = conn.Query<VDT_ThongTri_ChiTiet>(string.Format("SELECT * FROM VDT_ThongTri_ChiTiet"));
                var listDonViAll = conn.Query<NS_DonVi>(string.Format("SELECT * FROM NS_DonVi WHERE iTrangThai = 1 AND iNamLamViec_DonVi = '{0}'", config.iNamLamViec));
                var listPhongBanDonVi = conn.Query<NS_PhongBan_DonVi>(string.Format("SELECT * FROM NS_PhongBan_DonVi WHERE iNamLamViec = '{0}' AND iTrangThai = 1 AND sID_MaNguoiDung_DuocGiao = '{1}'", config.iNamLamViec, sUserName));

                if (listDonViAll == null || !listDonViAll.Any() || listPhongBanDonVi == null || !listPhongBanDonVi.Any())
                {
                    return result;
                }

                var listIdPhongBan = listPhongBanDonVi.Select(x => x.iID_MaDonVi).ToList();
                var listDonViByUser = listDonViAll.Where(x => listIdPhongBan.Contains(x.iID_MaDonVi)).ToList();
                if (listDonViByUser == null && !listDonViByUser.Any())
                {
                    return result;
                }

                var listIdDonViByUser = listDonViByUser.Select(x => x.iID_Ma).ToList();

                listThongTri = listThongTri.Where(x => x.iID_DonViID != null && listIdDonViByUser.Contains(x.iID_DonViID.Value));

                if (listThongTri == null || !listThongTri.Any())
                {
                    return result;
                }

                var listNguonNganSach = conn.Query<NS_NguonNganSach>(string.Format("SELECT * FROM NS_NguonNganSach WHERE iTrangThai = 1"));
                var listMucLucNganSach = conn.Query<NS_MucLucNganSach>(string.Format("SELECT * FROM NS_MucLucNganSach WHERE iTrangThai = 1 AND iNamLamViec = '{0}'", config.iNamLamViec));
                var listCongTrinh = conn.Query<VDT_DM_LoaiCongTrinh>(string.Format("SELECT * FROM VDT_DM_LoaiCongTrinh"));
                var listDuAn = conn.Query<VDT_DA_DuAn>(string.Format("SELECT * FROM VDT_DA_DuAn"));

                if (iID_DonViQuanLy.HasValue)
                {
                    listThongTri = listThongTri.Where(x => x.iID_DonViID != null && x.iID_DonViID.Value == iID_DonViQuanLy.Value).ToList();
                }

                if (!string.IsNullOrEmpty(sMaThongTri))
                {
                    listThongTri = listThongTri.Where(x => x.sMaThongTri.ToLower().Contains(sMaThongTri.ToLower())).ToList();
                }

                if (dNgayTaoThongTri.HasValue)
                {
                    listThongTri = listThongTri.Where(x => x.dNgayThongTri != null && x.dNgayThongTri.Value <= dNgayTaoThongTri.Value && x.bIsCanBoDuyet == true).ToList();
                }

                if (iNamThucHien != null)
                {
                    listThongTri = listThongTri.Where(x => x.iNamThongTri == iNamThucHien).ToList();
                }

                if (!string.IsNullOrEmpty(sNguoiLap))
                {
                    listThongTri = listThongTri.Where(x => x.sNguoiLap.ToLower().Contains(sNguoiLap.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(sTruongPhong))
                {
                    listThongTri = listThongTri.Where(x => x.sTruongPhong.ToLower().Contains(sTruongPhong.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(sThuTruongDonVi))
                {
                    listThongTri = listThongTri.Where(x => x.sThuTruongDonVi.ToLower().Contains(sThuTruongDonVi.ToLower())).ToList();
                }

                if (listThongTri != null && listThongTri.Any())
                {
                    var count = 1;
                    foreach (var item in listThongTri.OrderBy(x => x.dNgayThongTri))
                    {
                        var model = new VDT_ThongTriViewModel();
                        model.STT = count;
                        model.iID_ThongTriID = item.iID_ThongTriID;
                        model.iID_DonViID = item.iID_DonViID;
                        if (listDonViAll != null && listDonViAll.Any())
                        {
                            if (item.iID_DonViID.HasValue)
                            {
                                var donVi = listDonViAll.FirstOrDefault(x => x.iID_Ma == item.iID_DonViID.Value);
                                if (donVi != null)
                                {
                                    model.TenDonVi = donVi.sTen;
                                }
                            }
                        }
                        model.sMaNguonVon = item.sMaNguonVon;
                        if (!string.IsNullOrEmpty(model.sMaNguonVon))
                        {
                            if (listNguonNganSach != null && listNguonNganSach.Any())
                            {
                                var nguonVon = listNguonNganSach.FirstOrDefault(x => x.iID_MaNguonNganSach == int.Parse(model.sMaNguonVon));
                                if (nguonVon != null)
                                {
                                    model.TenNguonVon = nguonVon.sTen;
                                }
                            }
                        }

                        model.sMaLoaiCongTrinh = item.sMaLoaiCongTrinh;
                        if (!string.IsNullOrEmpty(model.sMaLoaiCongTrinh))
                        {
                            if (listCongTrinh != null && listCongTrinh.Any())
                            {
                                var congtrinh = listCongTrinh.FirstOrDefault(x => x.sMaLoaiCongTrinh == model.sMaLoaiCongTrinh);
                                if (congtrinh != null)
                                {
                                    model.sTenLoaiCongTrinh = congtrinh.sTenLoaiCongTrinh;
                                }
                            }
                        }

                        model.sMaThongTri = item.sMaThongTri;
                        model.dNgayThongTri = item.dNgayThongTri;
                        model.iNamThongTri = item.iNamThongTri;
                        model.sNguoiLap = item.sNguoiLap;
                        model.sTruongPhong = item.sTruongPhong;
                        model.sThuTruongDonVi = item.sThuTruongDonVi;

                        result.ListMaster.Add(model);
                        count++;
                    }
                }

                if (listThongTriChiTiet != null && listThongTriChiTiet.Any())
                {
                    var listIdThongTri = listThongTri.Select(x => x.iID_ThongTriID).ToList();
                    if (listIdThongTri != null)
                    {
                        listThongTriChiTiet = listThongTriChiTiet.Where(x => listIdThongTri.Contains(x.iID_ThongTriID));
                    }
                }

                if (listThongTriChiTiet != null && listThongTriChiTiet.Any())
                {
                    var count = 1;
                    foreach (var ct in listThongTriChiTiet.OrderBy(x => x.sSoThongTri))
                    {
                        var chitiet = new VDT_ThongTri_ChiTietViewModel();
                        chitiet.STT = count;
                        chitiet.iID_ThongTriID = ct.iID_ThongTriID;
                        chitiet.iID_MucID = ct.iID_MucID;
                        chitiet.iID_TieuMucID = ct.iID_TieuMucID;
                        chitiet.iID_TietMucID = ct.iID_TietMucID;
                        chitiet.iID_NganhID = ct.iID_NganhID;
                        chitiet.fSoTien = ct.fSoTien;
                        chitiet.sSoThongTri = ct.sSoThongTri;
                        chitiet.iID_DuAnID = ct.iID_DuAnID;

                        if (ct.iID_DuAnID.HasValue)
                        {
                            if (listDuAn != null && listDuAn.Any())
                            {
                                var duAn = listDuAn.FirstOrDefault(x => x.iID_DuAnID == ct.iID_DuAnID.Value);
                                if (duAn != null)
                                {
                                    chitiet.TenDuAn = duAn.sTenDuAn;

                                    if (listDonViAll != null && listDonViAll.Any())
                                    {
                                        var chuDauTu = listDonViAll.FirstOrDefault(x => x.iID_Ma == duAn.iID_ChuDauTuID);
                                        if (chuDauTu != null)
                                        {
                                            chitiet.TenChuNhaThau = chuDauTu.sTen;
                                        }
                                    }
                                }
                            }
                        }
                        chitiet.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinhID;
                        if (ct.iID_LoaiCongTrinhID.HasValue)
                        {
                            if (listCongTrinh != null && listCongTrinh.Any())
                            {
                                var congTrinh = listCongTrinh.FirstOrDefault(x => x.iID_LoaiCongTrinh == ct.iID_LoaiCongTrinhID);
                                if (congTrinh != null)
                                {
                                    chitiet.MaLoaiCongTrinh = congTrinh.sMaLoaiCongTrinh;
                                    chitiet.TenLoaiCongTrinh = congTrinh.sTenLoaiCongTrinh;
                                }
                            }
                        }

                        result.ListDetail.Add(chitiet);
                        count++;
                    }
                }
            }

            return result;
        }

        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTiet(Guid iIdThongTri, string sMaDonVi, int iLoaiThongTri, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon, DateTime? dNgayLapGanNhat, string sMaLoaiCongTrinh)
        {
            if (string.IsNullOrEmpty(sMaLoaiCongTrinh))
            {
                return this.GetVdtThongTriChiTiet(iIdThongTri, sMaDonVi, iLoaiThongTri, iNamKeHoach, dNgayThongTri, sMaNguonVon);
            }
            else
            {
                return this.GetVdtThongTriQuyetToanChiTiet(sMaDonVi, iNamKeHoach, dNgayThongTri, sMaNguonVon, dNgayLapGanNhat, sMaLoaiCongTrinh);
            }
        }
        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietByParentId(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VdtThongTriChiTietQuery>(@"EXECUTE dbo.sp_vdt_thongtri_detail_by_id @iId",
                new SqlParameter("iId", iId)).ToList();
            }
        }

        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTiet(Guid iIdThongTri, string sMaDonVi, int iLoaiThongTri, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                string sql = @"EXECUTE dbo.sp_vdt_thongtri_detail @iIdThongTriId, @sMaDonViQuanLy, @iLoaiThongTri, @iNamkeHoach, @dNgayThongTri, @sMaNguonVon";
                var parameters = new DynamicParameters();
                parameters.Add("iIdThongTriId", iIdThongTri);
                parameters.Add("sMaDonViQuanLy", sMaDonVi);
                parameters.Add("iLoaiThongTri", iLoaiThongTri);
                parameters.Add("iNamkeHoach", iNamKeHoach);
                parameters.Add("dNgayThongTri", dNgayThongTri);
                parameters.Add("sMaNguonVon", sMaNguonVon);
                var data = conn.Query<VdtThongTriChiTietQuery>(sql, parameters);
                if (data == null) return new List<VdtThongTriChiTietQuery>();
                return data.ToList();
            }
        }
        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietById(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("iId", iId);
                string sql = @"EXECUTE dbo.sp_vdt_thongtri_detail_by_id @iId";
                var data = conn.Query<VdtThongTriChiTietQuery>(sql, parameters);
                if (data == null) return new List<VdtThongTriChiTietQuery>();
                return data.ToList();
            }
        }


        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriQuyetToanChiTiet(string sMaDonVi, int iNamKeHoach, DateTime dNgayThongTri,
            string sMaNguonVon, DateTime? dNgayLapGanNhat, string sMaLoaiCongTrinh)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                string sql = @"EXECUTE dbo.sp_vdt_thongtri_quyettoan_detail @sMaDonViQuanLy, @iNamkeHoach, @dNgayThongTri, @sMaNguonVon, @dNgayLapGanNhat, @sMaLoaiCongTrinh";
                var parameters = new DynamicParameters();
                parameters.Add("sMaDonViQuanLy", sMaDonVi);
                parameters.Add("iNamkeHoach", iNamKeHoach);
                parameters.Add("dNgayThongTri", dNgayThongTri);
                parameters.Add("sMaNguonVon", sMaNguonVon);
                parameters.Add("dNgayLapGanNhat", dNgayLapGanNhat);
                parameters.Add("sMaLoaiCongTrinh", sMaLoaiCongTrinh);

                //return conn.Query<VdtThongTriChiTietQuery>(sql, parameters).ToList();
                var data = conn.Query<VdtThongTriChiTietQuery>(sql, parameters);
                if (data == null) return new List<VdtThongTriChiTietQuery>();
                return data.ToList();
            }
        }

        public void InsertListThongTriChiTiet(Guid iIdThongTri, List<VDT_ThongTri_ChiTiet> lstData)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                conn.Execute(String.Format("DELETE VDT_ThongTri_ChiTiet WHERE iID_ThongTriID = '{0}'", iIdThongTri));
                conn.Insert<VDT_ThongTri_ChiTiet>(lstData);
            }
        }
        #endregion

        /// <summary>
        /// get nhom quan ly theo ma nhom quan ly
        /// </summary>
        /// <param name="sMaNhomQuanLy"></param>
        /// <returns></returns>
        public VDT_DM_NhomQuanLy GetNhomQuanLyBysMaNhom(string sMaNhomQuanLy)
        {
            var sql = "select * from VDT_DM_NhomQuanLy where sMaNhomQuanLy = '" + sMaNhomQuanLy + "'";
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var items = conn.QueryFirstOrDefault<VDT_DM_NhomQuanLy>(sql);
                return items;
            }
        }

        public IEnumerable<VdtThongTriChiTietQuery> GetVdtThongTriChiTietByListIdDeNghiThanhToan(List<Guid> iIds)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("listIds", string.Join(",", iIds.Select(n => n.ToString())));
                string sql = @"EXECUTE dbo.proc_vdt_getthongtrithanhtoan_byiddenghithanhtoan @listIds";
                var data = conn.Query<VdtThongTriChiTietQuery>(sql, parameters);
                if (data == null) return new List<VdtThongTriChiTietQuery>();
                return data.ToList();
            }

        }


        #region Khoitao
        /// <summary>
        /// luu data khoi tao
        /// </summary>
        /// <param name="objKhoiTao"></param>
        /// <param name="lstKhoiTaoChiTiet"></param>
        /// <param name="objDuToan"></param>
        /// <param name="objQDDT"></param>
        /// <param name="objDuAn"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        public bool LuuKhoiTao(VDT_KT_KhoiTao objKhoiTao, List<VDTKTKhoiTaoChiTietModel> lstKhoiTaoChiTiet, VDT_DA_DuToan objDuToan, VDT_DA_QDDauTu objQDDT,
            VDT_DA_DuAn objDuAn, List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThau, string sUserName)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    Guid? iID_DuToanID = null;
                    Guid iID_QDDauTuID = Guid.Empty;
                    Guid iID_DuAnId = Guid.Empty;
                    Guid iID_KhoiTaoID = Guid.Empty;

                    if (!objKhoiTao.bIsDuAnCu.HasValue || (objKhoiTao.bIsDuAnCu.HasValue && !objKhoiTao.bIsDuAnCu.Value))
                    {
                        // VDT_DA_DuAn
                        objDuAn.sUserCreate = sUserName;
                        objDuAn.dDateCreate = DateTime.Now;
                        conn.Insert(objDuAn, trans);
                        iID_DuAnId = objDuAn.iID_DuAnID;

                        // VDT_DA_DuToan
                        if (!string.IsNullOrEmpty(objDuToan.sSoQuyetDinh))
                        {
                            objDuToan.iID_DuAnID = iID_DuAnId;
                            objDuToan.sUserCreate = sUserName;
                            objDuToan.dDateCreate = DateTime.Now;
                            conn.Insert(objDuToan, trans);

                            iID_DuToanID = objDuToan.iID_DuToanID;
                        }

                        // VDT_DA_QDDauTu
                        objQDDT.iID_DuAnID = iID_DuAnId;
                        objQDDT.sUserCreate = sUserName;
                        objQDDT.dDateCreate = DateTime.Now;
                        conn.Insert(objQDDT, trans);

                        iID_QDDauTuID = objQDDT.iID_QDDauTuID;
                    }
                    else
                    {
                        iID_DuAnId = objDuAn.iID_DuAnID;
                        iID_DuToanID = objDuToan.iID_DuToanID == Guid.Empty ? (Guid?)null : objDuToan.iID_DuToanID;
                        iID_QDDauTuID = objQDDT.iID_QDDauTuID;
                    }

                    // VDT_KT_KhoiTao
                    objKhoiTao.iID_DuAnID = iID_DuAnId;
                    objKhoiTao.iID_QDDauTuID = iID_QDDauTuID;
                    objKhoiTao.iID_DuToanID = iID_DuToanID;
                    objKhoiTao.sUserCreate = sUserName;
                    objKhoiTao.dDateCreate = DateTime.Now;
                    conn.Insert(objKhoiTao, trans);

                    iID_KhoiTaoID = objKhoiTao.iID_KhoiTaoID;

                    foreach (VDTKTKhoiTaoChiTietModel ktChiTiet in lstKhoiTaoChiTiet)
                    {
                        // VDT_KT_KhoiTao_ChiTiet
                        VDT_KT_KhoiTao_ChiTiet objKhoiTaoCt = new VDT_KT_KhoiTao_ChiTiet();
                        objKhoiTaoCt.MapFrom(ktChiTiet);
                        objKhoiTaoCt.iID_KhoiTaoID = iID_KhoiTaoID;
                        conn.Insert(objKhoiTaoCt, trans);

                        // VDT_DA_QDDauTu_NguonVon
                        VDT_DA_QDDauTu_NguonVon objQDDTNguonVon = new VDT_DA_QDDauTu_NguonVon();
                        objQDDTNguonVon.iID_QDDauTuID = iID_QDDauTuID;
                        objQDDTNguonVon.iID_NguonVonID = ktChiTiet.iID_NguonVonID;
                        objQDDTNguonVon.fTienPheDuyet = ktChiTiet.fTongMucDauTu;
                        conn.Insert(objQDDTNguonVon, trans);

                        // VDT_DA_DuToan_NguonVon
                        if (iID_DuToanID != null && iID_DuToanID != Guid.Empty)
                        {
                            VDT_DA_DuToan_Nguonvon objDTNguonVon = new VDT_DA_DuToan_Nguonvon();
                            objDTNguonVon.iID_DuToanID = iID_DuToanID.Value;
                            objDTNguonVon.iID_NguonVonID = ktChiTiet.iID_NguonVonID;
                            objDTNguonVon.fTienPheDuyet = ktChiTiet.fGiaTriDuToan;
                            conn.Insert(objDTNguonVon, trans);
                        }

                        List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThauByKT = lstNhaThau.Where(x => x.iID_NganhID == objKhoiTaoCt.iID_NganhID
                                                                                    && x.iID_LoaiNguonVonID == objKhoiTaoCt.iID_LoaiNguonVonID
                                                                                    && x.iID_NguonVonId == objKhoiTaoCt.iID_NguonVonID).ToList();
                        foreach (VDTKTKhoiTaoChiTietNhaThau itemNhaThau in lstNhaThauByKT)
                        {
                            VDT_KT_KhoiTao_ChiTiet_NhaThau objNhaThau = new VDT_KT_KhoiTao_ChiTiet_NhaThau();
                            objNhaThau.MapFrom(itemNhaThau);
                            objNhaThau.iID_KhoiTaoID = objKhoiTao.iID_KhoiTaoID;
                            objNhaThau.iID_KhoiTao_ChiTietID = objKhoiTaoCt.iID_KhoiTao_ChiTietID;
                            conn.Insert(objNhaThau, trans);
                        }
                    }

                    // commit to db
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// luu data khoi tao
        /// </summary>
        /// <param name="objKhoiTao"></param>
        /// <param name="lstKhoiTaoChiTiet"></param>
        /// <param name="objDuToan"></param>
        /// <param name="objQDDT"></param>
        /// <param name="objDuAn"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        public bool CapNhatKhoiTao(VDT_KT_KhoiTao objKhoiTao, List<VDTKTKhoiTaoChiTietModel> lstKhoiTaoChiTiet, VDT_DA_DuToan objDuToan, VDT_DA_QDDauTu objQDDT,
            VDT_DA_DuAn objDuAn, List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThau, string sUserName)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    Guid? iID_DuToanID = null;
                    Guid iID_QDDauTuID = Guid.Empty;
                    Guid iID_DuAnId = Guid.Empty;
                    Guid iID_KhoiTaoID = Guid.Empty;

                    // VDT_KT_KhoiTao
                    var objKhoiTaoOld = conn.Get<VDT_KT_KhoiTao>(objKhoiTao.iID_KhoiTaoID, trans);
                    if (objKhoiTaoOld == null)
                        return false;

                    if (!objKhoiTaoOld.bIsDuAnCu.HasValue || (objKhoiTaoOld.bIsDuAnCu.HasValue && !objKhoiTaoOld.bIsDuAnCu.Value))
                    {
                        // VDT_DA_DuAn
                        if (objDuAn.iID_DuAnID != Guid.Empty)
                        {
                            var objDuAnOld = conn.Get<VDT_DA_DuAn>(objDuAn.iID_DuAnID, trans);
                            if (objDuAnOld == null)
                                return false;
                            objDuAnOld.sTenDuAn = objDuAn.sTenDuAn;
                            objDuAnOld.sMaDuAn = objDuAn.sMaDuAn;
                            objDuAnOld.iID_ChuDauTuID = objDuAn.iID_ChuDauTuID;
                            objDuAnOld.iID_CapPheDuyetID = objDuAn.iID_CapPheDuyetID;
                            objDuAnOld.iID_LoaiCongTrinhID = objDuAn.iID_LoaiCongTrinhID;
                            objDuAnOld.iID_DonViQuanLyID = objDuAn.iID_DonViQuanLyID;
                            objDuAnOld.sKhoiCong = objDuAn.sKhoiCong;
                            objDuAnOld.sKetThuc = objDuAn.sKetThuc;
                            objDuAnOld.sUserUpdate = sUserName;
                            objDuAnOld.dDateUpdate = DateTime.Now;
                            conn.Update(objDuAnOld, trans);

                        }
                        iID_DuAnId = objDuAn.iID_DuAnID;

                        // VDT_DA_DuToan
                        if (!string.IsNullOrEmpty(objDuToan.sSoQuyetDinh))
                        {
                            if (objDuToan.iID_DuToanID != Guid.Empty)
                            {
                                var objDuToanOld = conn.Get<VDT_DA_DuToan>(objDuToan.iID_DuToanID, trans);
                                if (objDuToanOld == null)
                                    return false;
                                objDuToanOld.sSoQuyetDinh = objDuToan.sSoQuyetDinh;
                                objDuToanOld.dNgayQuyetDinh = objDuToan.dNgayQuyetDinh;
                                objDuToanOld.sCoQuanPheDuyet = objDuToan.sCoQuanPheDuyet;
                                objDuToanOld.sNguoiKy = objDuToan.sNguoiKy;
                                objDuToanOld.fTongDuToanPheDuyet = objDuToan.fTongDuToanPheDuyet;

                                objDuToanOld.sUserUpdate = sUserName;
                                objDuToanOld.dDateUpdate = DateTime.Now;

                                conn.Update(objDuToanOld, trans);
                            }
                            else
                            {
                                objDuToan.iID_DuAnID = iID_DuAnId;
                                objDuToan.sUserCreate = sUserName;
                                objDuToan.dDateCreate = DateTime.Now;
                                conn.Insert(objDuToan, trans);
                            }

                            iID_DuToanID = objDuToan.iID_DuToanID;
                        }
                        else
                        {
                            // neu da co du toan, nhung xoa sSoQuyetDinh du toan -> xoa du toan cu
                            if (objKhoiTaoOld.iID_DuToanID.HasValue && objKhoiTaoOld.iID_DuToanID != Guid.Empty)
                            {
                                var objDuToanOld = conn.Get<VDT_DA_DuToan>(objDuToan.iID_DuToanID, trans);
                                if (objDuToanOld == null)
                                    return false;
                                conn.Delete(objDuToanOld, trans);

                                // xoa du toan nv cu
                                XoaDuToanNguonVon(objKhoiTaoOld.iID_DuToanID.Value);
                            }
                        }

                        // VDT_DA_QDDauTu
                        var objQDDTOld = conn.Get<VDT_DA_QDDauTu>(objQDDT.iID_QDDauTuID, trans);
                        objQDDTOld.sSoQuyetDinh = objQDDT.sSoQuyetDinh;
                        objQDDTOld.dNgayQuyetDinh = objQDDT.dNgayQuyetDinh;
                        objQDDTOld.sCoQuanPheDuyet = objQDDT.sCoQuanPheDuyet;
                        objQDDTOld.sNguoiKy = objQDDT.sNguoiKy;
                        objQDDTOld.fTongMucDauTuPheDuyet = objQDDT.fTongMucDauTuPheDuyet;

                        objQDDTOld.sUserUpdate = sUserName;
                        objQDDTOld.dDateUpdate = DateTime.Now;

                        conn.Update(objQDDTOld, trans);

                        iID_QDDauTuID = objQDDT.iID_QDDauTuID;
                    }
                    else
                    {
                        iID_DuAnId = objDuAn.iID_DuAnID;
                        iID_DuToanID = objDuToan.iID_DuToanID == Guid.Empty ? (Guid?)null : objDuToan.iID_DuToanID;
                        iID_QDDauTuID = objQDDT.iID_QDDauTuID;
                    }

                    objKhoiTaoOld.iID_DuAnID = iID_DuAnId;
                    objKhoiTaoOld.iID_QDDauTuID = iID_QDDauTuID;
                    objKhoiTaoOld.iID_DuToanID = iID_DuToanID;

                    objKhoiTaoOld.fKHVonUng = objKhoiTao.fKHVonUng;
                    objKhoiTaoOld.fVonUngDaCap = objKhoiTao.fVonUngDaCap;
                    objKhoiTaoOld.fVonUngDaThuHoi = objKhoiTao.fVonUngDaThuHoi;
                    objKhoiTaoOld.fGiaTriConPhaiUng = objKhoiTao.fGiaTriConPhaiUng;
                    objKhoiTaoOld.iID_DonViID = objKhoiTao.iID_DonViID;

                    objKhoiTaoOld.sUserCreate = sUserName;
                    objKhoiTaoOld.dDateCreate = DateTime.Now;
                    conn.Update(objKhoiTaoOld, trans);

                    iID_KhoiTaoID = objKhoiTaoOld.iID_KhoiTaoID;

                    // xoa data khoi tao chi tiet/ khoi tao chi tiet nha thau/ quyet dinh dau tu nv/ du toan nv cu
                    XoaKhoiTaoChiTietChiTietNhaThau(iID_KhoiTaoID);
                    XoaQuyetDinhDauTuNguonVon(iID_QDDauTuID);
                    if (iID_DuToanID.HasValue && iID_DuToanID != Guid.Empty)
                        XoaDuToanNguonVon(iID_DuToanID.Value);

                    foreach (VDTKTKhoiTaoChiTietModel ktChiTiet in lstKhoiTaoChiTiet)
                    {
                        // VDT_KT_KhoiTao_ChiTiet
                        VDT_KT_KhoiTao_ChiTiet objKhoiTaoCt = new VDT_KT_KhoiTao_ChiTiet();
                        objKhoiTaoCt.MapFrom(ktChiTiet);
                        objKhoiTaoCt.iID_KhoiTaoID = iID_KhoiTaoID;
                        conn.Insert(objKhoiTaoCt, trans);

                        // VDT_DA_QDDauTu_NguonVon
                        VDT_DA_QDDauTu_NguonVon objQDDTNguonVon = new VDT_DA_QDDauTu_NguonVon();
                        objQDDTNguonVon.iID_QDDauTuID = iID_QDDauTuID;
                        objQDDTNguonVon.iID_NguonVonID = ktChiTiet.iID_NguonVonID;
                        objQDDTNguonVon.fTienPheDuyet = ktChiTiet.fTongMucDauTu;
                        conn.Insert(objQDDTNguonVon, trans);

                        // VDT_DA_DuToan_NguonVon
                        if (iID_DuToanID != null && iID_DuToanID != Guid.Empty)
                        {
                            VDT_DA_DuToan_Nguonvon objDTNguonVon = new VDT_DA_DuToan_Nguonvon();
                            objDTNguonVon.iID_DuToanID = iID_DuToanID.Value;
                            objDTNguonVon.iID_NguonVonID = ktChiTiet.iID_NguonVonID;
                            objDTNguonVon.fTienPheDuyet = ktChiTiet.fGiaTriDuToan;
                            conn.Insert(objDTNguonVon, trans);
                        }

                        if (lstNhaThau != null && lstNhaThau.Count > 0)
                        {
                            List<VDTKTKhoiTaoChiTietNhaThau> lstNhaThauByKT = lstNhaThau.Where(x => x.iID_NganhID == objKhoiTaoCt.iID_NganhID
                                                                                    && x.iID_LoaiNguonVonID == objKhoiTaoCt.iID_LoaiNguonVonID
                                                                                    && x.iID_NguonVonId == objKhoiTaoCt.iID_NguonVonID).ToList();
                            foreach (VDTKTKhoiTaoChiTietNhaThau itemNhaThau in lstNhaThauByKT)
                            {
                                VDT_KT_KhoiTao_ChiTiet_NhaThau objNhaThau = new VDT_KT_KhoiTao_ChiTiet_NhaThau();
                                objNhaThau.MapFrom(itemNhaThau);
                                objNhaThau.iID_KhoiTaoID = objKhoiTao.iID_KhoiTaoID;
                                objNhaThau.iID_KhoiTao_ChiTietID = objKhoiTaoCt.iID_KhoiTao_ChiTietID;
                                conn.Insert(objNhaThau, trans);
                            }
                        }
                    }

                    // commit to db
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// get data khoi tao du an chuyen tiep theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="iNamKhoiTao"></param>
        /// <returns></returns>
        public IEnumerable<VDTQLKhoiTaoDuAnViewModel> GetAllQLKhoiTaoDuAnPaging(ref PagingInfo _paging, string sTenDuAn, int? iNamKhoiTao)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sTenDuAn", sTenDuAn);
                    lstParam.Add("iNamKhoiTao", iNamKhoiTao);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<VDTQLKhoiTaoDuAnViewModel>("proc_get_all_vdt_khoitaoduan_paging", lstParam, commandType: CommandType.StoredProcedure);
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
        /// get data khoi tao du an chuyen tiep theo dieu kien
        /// </summary>
        /// <param name="sTenDuAn"></param>
        /// <param name="iNamKhoiTao"></param>
        /// <returns></returns>
        public IEnumerable<VDTQLKhoiTaoDuAnViewModel> GetAllQLKhoiTaoDuAnExport(string sTenDuAn, int? iNamKhoiTao)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sTenDuAn", sTenDuAn);
                    lstParam.Add("iNamKhoiTao", iNamKhoiTao);

                    var items = conn.Query<VDTQLKhoiTaoDuAnViewModel>("proc_get_all_vdt_khoitaoduan_export", lstParam, commandType: CommandType.StoredProcedure);
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
        /// get chi tiet khoi tao du an
        /// </summary>
        /// <param name="iID_KhoiTaoID"></param>
        /// <returns></returns>
        public VDTQLKhoiTaoDuAnViewModel GetDetailQLKhoiTaoDuAn(Guid iID_KhoiTaoID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_chitiet_khoitaoduan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<VDTQLKhoiTaoDuAnViewModel>(sql, param: new
                {
                    iID_KhoiTaoID
                },
                commandType: CommandType.Text);
                if (items != null)
                {
                    items.lstKTChiTiet = GetListKhoiTaoChiTiet(iID_KhoiTaoID).ToList();
                    items.lstKTChiTietNhaThau = GetListKhoiTaoChiTietNhaThau(iID_KhoiTaoID).ToList();
                }
                return items;
            }
        }

        /// <summary>
        /// get list khoi tao chi tiet
        /// </summary>
        /// <param name="iID_KhoiTaoID"></param>
        /// <returns></returns>
        public List<VDTKTKhoiTaoChiTietModel> GetListKhoiTaoChiTiet(Guid iID_KhoiTaoID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_khoitaoduan_chitiet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKTKhoiTaoChiTietModel>(sql, param: new
                {
                    iID_KhoiTaoID
                },
                commandType: CommandType.Text);

                return items.ToList();
            }
        }

        /// <summary>
        /// get list nha thau
        /// </summary>
        /// <param name="iID_KhoiTaoID"></param>
        /// <returns></returns>
        public List<VDTKTKhoiTaoChiTietNhaThau> GetListKhoiTaoChiTietNhaThau(Guid iID_KhoiTaoID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_khoitaoduan_nhathau.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKTKhoiTaoChiTietNhaThau>(sql, param: new
                {
                    iID_KhoiTaoID
                },
                commandType: CommandType.Text);

                return items.ToList();
            }
        }

        /// <summary>
        /// xoa danh sach khoi tao chi tiet, khoi tao chi tiet nha thau
        /// </summary>
        /// <param name="iID_KhoiTaoID"></param>
        /// <returns></returns>
        public bool XoaKhoiTaoChiTietChiTietNhaThau(Guid iID_KhoiTaoID)
        {
            var sql = "DELETE VDT_KT_KhoiTao_ChiTiet_NhaThau WHERE iID_KhoiTaoID = @iID_KhoiTaoID; " +
                      "DELETE VDT_KT_KhoiTao_ChiTiet WHERE iID_KhoiTaoID = @iID_KhoiTaoID;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_KhoiTaoID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        /// <summary>
        /// xoa danh sach quyet dinh dau tu nguon von
        /// </summary>
        /// <param name="iID_QDDauTuID"></param>
        /// <returns></returns>
        public bool XoaQuyetDinhDauTuNguonVon(Guid iID_QDDauTuID)
        {
            var sql = "DELETE VDT_DA_QDDauTu_NguonVon WHERE iID_QDDauTuID = @iID_QDDauTuID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_QDDauTuID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        /// <summary>
        /// xoa danh sach du toan nguon von
        /// </summary>
        /// <param name="iID_DuToanID"></param>
        /// <returns></returns>
        public bool XoaDuToanNguonVon(Guid iID_DuToanID)
        {
            var sql = "DELETE VDT_DA_DuToan_Nguonvon WHERE iID_DuToanID = @iID_DuToanID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_DuToanID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        #region New
        public IEnumerable<VDT_KT_KhoiTao_DuLieu_ViewModel> GetAllKhoiTaoThongTinDuAn(ref PagingInfo _paging, int? iNamKhoiTao = null, string sTenDonVi = "")
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iNamKhoiTao", iNamKhoiTao);
                lstParam.Add("sTenDonViQL", sTenDonVi);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDT_KT_KhoiTao_DuLieu_ViewModel>("proc_get_all_khoitaothongtinduan_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<NS_DonVi> GetListDonViByNamLamViec(int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM NS_DonVi WHERE iNamLamViec_DonVi = {0} ORDER BY iID_MaDonVi; ", iNamLamViec);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public VDT_KT_KhoiTao_DuLieu_ViewModel GetKhoiTaoTTDAById(Guid iID_KhoiTaoID)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT kt.*, dv.sTen AS sTenDonVi FROM VDT_KT_KhoiTao_DuLieu kt ");
            query.Append("LEFT JOIN NS_DonVi dv ON dv.iID_Ma = kt.iID_DonViID ");
            query.AppendFormat("WHERE kt.iID_KhoiTaoID = '{0}' ", iID_KhoiTaoID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_KT_KhoiTao_DuLieu_ViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }

        public bool SaveKhoiTaoTTDA(ref Guid iID_KhoiTao, ref string sMaDonVi, VDT_KT_KhoiTao_DuLieu data, string sUserName)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                data.iID_MaDonVi = GetDonViQuanLyById(data.iID_DonViID.Value).iID_MaDonVi;
                sMaDonVi = data.iID_MaDonVi;
                if (data.iID_KhoiTaoID == null || data.iID_KhoiTaoID == Guid.Empty)
                {
                    var entity = new VDT_KT_KhoiTao_DuLieu();
                    entity.MapFrom(data);
                    entity.sUserCreate = sUserName;
                    entity.dDateCreate = DateTime.Now;
                    conn.Insert(entity, trans);
                    iID_KhoiTao = entity.iID_KhoiTaoID;
                }
                else
                {
                    var entity = conn.Get<VDT_KT_KhoiTao_DuLieu>(data.iID_KhoiTaoID, trans);
                    if (entity == null)
                        return false;
                    entity.iNamKhoiTao = data.iNamKhoiTao;
                    entity.iID_DonViID = data.iID_DonViID;
                    entity.iID_MaDonVi = data.iID_MaDonVi;
                    entity.dNgayKhoiTao = data.dNgayKhoiTao;
                    entity.sUserUpdate = sUserName;
                    entity.dDateUpdate = DateTime.Now;
                    conn.Update(entity, trans);
                    iID_KhoiTao = entity.iID_KhoiTaoID;
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public bool DeleteKhoiTaoTTDA(Guid iID_KhoiTaoID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE VDT_KT_KhoiTao_DuLieu WHERE iID_KhoiTaoID =  '{0}';", iID_KhoiTaoID);
            query.AppendFormat("DELETE VDT_KT_KhoiTao_DuLieu_ChiTiet WHERE iID_KhoiTaoDuLieuID =  '{0}';", iID_KhoiTaoID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), commandType: CommandType.Text);
                return r > 0;
            }
        }

        public IEnumerable<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel> GetKhoiTaoTTDAChiTietByIdKhoiTao(Guid iID_KhoiTaoID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT ROW_NUMBER() OVER (ORDER BY sMaDuAn) AS STT, ktct.* FROM VDT_KT_KhoiTao_DuLieu_ChiTiet ktct WHERE ktct.iID_KhoiTaoDuLieuID = '{0}' ", iID_KhoiTaoID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }

        public IEnumerable<VDT_DA_DuAn> GetDuAnByMaDonViQL(string sMaDonVi)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT da.* FROM VDT_DA_DuAn da WHERE da.iID_MaDonViThucHienDuAnID = '{0}' ", sMaDonVi);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public bool SaveChiTietKhoiTaoTTDA(List<VDT_KT_KhoiTao_DuLieu_ChiTiet> data, List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> lstHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                //conn.Open();
                //var trans = conn.BeginTransaction();
                Guid iID_KhoiTaoID = data.FirstOrDefault().iID_KhoiTaoDuLieuID;
                StringBuilder query = new StringBuilder();
                query.Append("DELETE VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan WHERE iId_KhoiTaoDuLieuChiTietId IN ");
                query.AppendFormat("(SELECT ct.iID_KhoiTao_ChiTietID FROM VDT_KT_KhoiTao_DuLieu_ChiTiet ct WHERE ct.iID_KhoiTaoDuLieuID = '{0}');", iID_KhoiTaoID);
                query.AppendFormat("DELETE VDT_KT_KhoiTao_DuLieu_ChiTiet WHERE iID_KhoiTaoDuLieuID = '{0}'; ", iID_KhoiTaoID);
                var excute = conn.Execute(query.ToString(), /*trans,*/
                   commandType: CommandType.Text);

                conn.Insert<VDT_KT_KhoiTao_DuLieu_ChiTiet>(data/*, trans*/);
                conn.Insert<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan>(lstHopDong/*, trans*/);

                //insert data bang VDT_TongHop_NguonNSDauTu
                List<VDT_TongHop_NguonNSDauTu> lstDataTongHop = ConvertDataTongHopFromKhoiTaoChiTiet(data);

                conn.Insert<VDT_TongHop_NguonNSDauTu>(lstDataTongHop/*, trans*/);

                // commit to db
                //trans.Commit();
            }
            return true;
        }

        private List<VDT_TongHop_NguonNSDauTu> ConvertDataTongHopFromKhoiTaoChiTiet(List<VDT_KT_KhoiTao_DuLieu_ChiTiet> lstKhoiTaoChiTiet)
        {
            List<VDT_TongHop_NguonNSDauTu> lstDataTongHop = new List<VDT_TongHop_NguonNSDauTu>();

            foreach (var item in lstKhoiTaoChiTiet)
            {
                if ((item.fKHVN_VonBoTriHetNamTruoc ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVN_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVN_LENHCHI,
                        sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        fGiaTri = item.fKHVN_VonBoTriHetNamTruoc,
                    });
                }

                if ((item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU,
                        sMaDich = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_TT_KHVN_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_TT_KHVN_LENHCHI,
                        sMaNguonCha = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVN_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVN_LENHCHI,
                        fGiaTri = item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc,
                    });
                }

                if ((item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU,
                        sMaDich = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_TU_CHUATH_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_TU_CHUATH_LENHCHI,
                        sMaNguonCha = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVN_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVN_LENHCHI,
                        fGiaTri = item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi,
                    });
                }

                if ((item.fKHVN_KeHoachVonKeoDaiSangNam ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_LENHCHI_CHUYENNAMTRUOC,
                        sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        fGiaTri = item.fKHVN_KeHoachVonKeoDaiSangNam,
                    });
                }

                if ((item.fKHUT_VonBoTriHetNamTruoc ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVU_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVU_LENHCHI,
                        sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        fGiaTri = item.fKHUT_VonBoTriHetNamTruoc,
                    });
                }

                if ((item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU,
                        sMaDich = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_TT_KHVU_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_TT_KHVU_LENHCHI,
                        sMaNguonCha = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVU_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVU_LENHCHI,
                        fGiaTri = item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc,
                    });
                }

                if ((item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU,
                        sMaDich = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_TU_CHUATH_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_TU_CHUATH_LENHCHI,
                        sMaNguonCha = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVU_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVU_LENHCHI,
                        fGiaTri = item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi,
                    });
                }

                if ((item.fKHUT_KeHoachUngTruocKeoDaiSangNam ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_UNG_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_UNG_LENHCHI_CHUYENNAMTRUOC,
                        sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        fGiaTri = item.fKHUT_KeHoachUngTruocKeoDaiSangNam,
                    });
                }

                if ((item.fKHUT_KeHoachUngTruocChuaThuHoi ?? 0) != 0)
                {
                    lstDataTongHop.Add(new VDT_TongHop_NguonNSDauTu()
                    {
                        iID_ChungTu = item.iID_KhoiTao_ChiTietID,
                        iID_DuAnID = item.iID_DuAnID.Value,
                        sMaNguon = (item.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC) ? LOAI_CHUNG_TU.QT_LUYKE_KHVU_KHOBAC : LOAI_CHUNG_TU.QT_LUYKE_KHVU_LENHCHI,
                        sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        fGiaTri = item.fKHUT_KeHoachUngTruocChuaThuHoi,
                    });
                }
            }

            return lstDataTongHop;
        }

        public IEnumerable<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel> GetListHopDongKhoiTaoTTDAByKhoiTaoID(Guid iID_KhoiTaoID)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ROW_NUMBER() OVER (ORDER BY hd.sSoHopDong) AS STT, cttt.*, hd.sSoHopDong AS sTenHopDong, nt.sTenNhaThau AS sTenNhaThau, hd.fTienHopDong AS sGiaTriHD  ");
            query.Append("FROM VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan cttt ");
            query.Append("LEFT JOIN VDT_DA_TT_HopDong hd ON hd.iID_HopDongID = cttt.iID_HopDongId ");
            query.Append("LEFT JOIN VDT_DM_NhaThau nt ON nt.iID_NhaThauID = hd.iID_NhaThauThucHienID ");
            query.Append("WHERE cttt.iId_KhoiTaoDuLieuChiTietId IN ");
            query.AppendFormat("(SELECT ct.iID_KhoiTao_ChiTietID FROM VDT_KT_KhoiTao_DuLieu_ChiTiet ct WHERE ct.iID_KhoiTaoDuLieuID = '{0}')", iID_KhoiTaoID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan_ViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }
        #endregion

        #endregion

        #region Báo cáo dự toán NSQP năm
        /// <summary>
        /// lay data bao cao du toan nsqp
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="sMaDonVi"></param>
        /// <returns></returns>
        public List<RptDuToanNSQPNam> LayDataBaoCaoDuToanNSQPNam(int iNamKeHoach, string sMaDonVi)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_rpt_baocaodutoannsqpnam.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<RptDuToanNSQPNam>(sql,
                            param:
                            new
                            {
                                iNamKeHoach,
                                sMaDonVi
                            },
                            commandType: CommandType.Text);

                foreach (var item in items)
                {
                    if ((bool)item.bIsHangCha)
                        item.fGiaTrPhanBo = items.Where(x => !(bool)x.bIsHangCha && x.iCT == item.iCT).Select(x => x.fGiaTrPhanBo).Sum();
                }
                return items.ToList();
            }
        }
        #endregion

        #region Báo cáo Điều chỉnh kế hoạch năm
        public IEnumerable<VDTBaoCaoDieuChinhKeHoachViewModel> LayBaoCaoDieuChinhKeHoach(int? iID_NguonVon, string sLNS, int? iNamThucHien, string UserLogin)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iIdNguonVonId", iID_NguonVon);
                lstParam.Add("sLNS", sLNS);
                lstParam.Add("iNamKeHoach", iNamThucHien);
                lstParam.Add("userName", UserLogin);
                var items = conn.Query<VDTBaoCaoDieuChinhKeHoachViewModel>("proc_vdt_baocao_dieuchinhkehoach", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region Kế hoạch trung hạn đề xuất - NinhNV
        public bool SaveKeHoach5NamDeXuat(ref VDT_KHV_KeHoach5Nam_DeXuat data, string sUserLogin, int iNamLamViec, bool isModified, bool isAggregate, List<VdtKhvKeHoachTrungHanDeXuatChiTietModel> details)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_KeHoach5Nam_DeXuatID == null || data.iID_KeHoach5Nam_DeXuatID == Guid.Empty)
                {
                    if (isAggregate)
                    {
                        var entity = new VDT_KHV_KeHoach5Nam_DeXuat();

                        var lstItemTongHop = details.GroupBy(x => x.iID_KeHoach5Nam_DeXuatID).Select(grp => grp.FirstOrDefault());

                        entity.MapFrom(data);
                        entity.iID_KeHoach5Nam_DeXuatID = Guid.NewGuid();
                        entity.bActive = true;
                        entity.bIsGoc = true;
                        entity.iKhoa = false;
                        entity.iNamLamViec = iNamLamViec;
                        entity.sUserCreate = sUserLogin;
                        entity.dDateCreate = DateTime.Now;

                        NS_DonVi itemDonVi = conn.Get<NS_DonVi>(entity.iID_DonViQuanLyID, trans);
                        if (itemDonVi != null)
                        {
                            entity.iID_MaDonViQuanLy = itemDonVi.iID_MaDonVi;
                        }

                        foreach (var item in lstItemTongHop)
                        {
                            var entityUpdate = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(item.iID_KeHoach5Nam_DeXuatID, trans);
                            if (entityUpdate != null)
                            {
                                entityUpdate.iID_TongHopParent = entity.iID_KeHoach5Nam_DeXuatID;

                                conn.Update(entityUpdate, trans);
                            }
                        }

                        details = details.Select(x =>
                        {
                            x.IdParentNew = x.iID_KeHoach5Nam_DeXuat_ChiTietID;
                            x.iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.NewGuid();
                            return x;
                        }).ToList();

                        var refDictionary = details.ToDictionary(x => x.IdParentNew, x => x.iID_KeHoach5Nam_DeXuat_ChiTietID);

                        details.Select(item =>
                        {
                            if (item.iIDReference != null) item.iIDReference = refDictionary[item.iIDReference];
                            if (item.iID_ParentID != null) item.iID_ParentID = refDictionary[item.iID_ParentID];
                            item.iID_KeHoach5Nam_DeXuatID = entity.iID_KeHoach5Nam_DeXuatID;
                            return item;
                        }).ToList();

                        data = entity;
                        conn.Insert(entity, trans);
                        conn.Insert<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(details, trans);
                    }
                    else
                    {
                        var entity = new VDT_KHV_KeHoach5Nam_DeXuat();
                        entity.MapFrom(data);

                        NS_DonVi itemDonVi = conn.Get<NS_DonVi>(entity.iID_DonViQuanLyID, trans);
                        if (itemDonVi != null)
                        {
                            entity.iID_MaDonViQuanLy = itemDonVi.iID_MaDonVi;
                        }

                        entity.bActive = true;
                        entity.bIsGoc = true;
                        entity.iKhoa = false;
                        entity.iNamLamViec = iNamLamViec;
                        entity.sUserCreate = sUserLogin;
                        entity.dDateCreate = DateTime.Now;

                        data = entity;
                        conn.Insert(entity, trans);
                    }
                }
                else
                {
                    if (isModified)
                    {
                        var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(data.iID_KeHoach5Nam_DeXuatID, trans);
                        if (entity == null)
                            return false;

                        entity.bActive = false;
                        conn.Update(entity, trans);

                        var newItem = new VDT_KHV_KeHoach5Nam_DeXuat();
                        newItem.MapFrom(entity);
                        newItem.iID_ParentId = entity.iID_KeHoach5Nam_DeXuatID;
                        newItem.iID_KeHoach5Nam_DeXuatID = Guid.NewGuid();
                        newItem.iID_DonViQuanLyID = entity.iID_DonViQuanLyID;
                        newItem.iID_MaDonViQuanLy = entity.iID_MaDonViQuanLy;
                        newItem.sSoQuyetDinh = data.sSoQuyetDinh;
                        newItem.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        newItem.iGiaiDoanTu = data.iGiaiDoanTu;
                        newItem.iGiaiDoanDen = data.iGiaiDoanDen;
                        newItem.sMoTaChiTiet = data.sMoTaChiTiet;
                        newItem.sUserCreate = sUserLogin;
                        newItem.dDateCreate = DateTime.Now;
                        newItem.bActive = true;
                        newItem.bIsGoc = false;
                        newItem.iKhoa = false;
                        newItem.iID_TongHopParent = null;

                        conn.Insert(newItem, trans);

                        details = details.Select(x =>
                        {
                            x.IdParentNew = x.iID_KeHoach5Nam_DeXuat_ChiTietID;
                            x.iID_ParentModified = x.iID_KeHoach5Nam_DeXuat_ChiTietID;
                            x.iID_KeHoach5Nam_DeXuat_ChiTietID = Guid.NewGuid();
                            x.iID_KeHoach5Nam_DeXuatID = newItem.iID_KeHoach5Nam_DeXuatID;
                            x.sGhiChu = string.Empty;
                            return x;
                        }).OrderBy(x => x.sMaOrder).ToList();

                        var refDictionary = details.ToDictionary(x => x.IdParentNew, x => x.iID_KeHoach5Nam_DeXuat_ChiTietID);

                        details.Select(item =>
                        {
                            // Cập nhật IdReference, IdParent
                            if (item.iIDReference != null) item.iIDReference = refDictionary[item.iIDReference];
                            //if (item.iID_ParentID != null) item.iID_ParentID = refDictionary[item.iID_ParentID];          // khi duyệt để điều chỉnh sẽ không còn quan hệ cha con trong chi tiết KHTH
                            item.iID_ParentID = null;                                                                       // nên iID_ParentID của các chi tiết trong KHTH điều chỉnh sẽ = null

                            return item;
                        }).ToList();

                        data = newItem;

                        conn.Insert<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(details, trans);
                    }
                    else
                    {
                        var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(data.iID_KeHoach5Nam_DeXuatID, trans);
                        if (entity == null)
                            return false;

                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        NS_DonVi itemDonVi = conn.Get<NS_DonVi>(entity.iID_DonViQuanLyID, trans);
                        if (itemDonVi != null)
                        {
                            entity.iID_MaDonViQuanLy = itemDonVi.iID_MaDonVi;
                        }

                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.iGiaiDoanTu = data.iGiaiDoanTu;
                        entity.iGiaiDoanDen = data.iGiaiDoanDen;
                        entity.sMoTaChiTiet = data.sMoTaChiTiet;
                        entity.sUserUpdate = sUserLogin;
                        entity.dDateUpdate = DateTime.Now;

                        conn.Update(entity, trans);
                    }
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }
        public NS_DonVi GetNameDonViQLByMaDV(string sMaDV)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT DISTINCT sTen FROM NS_DonVi where iID_MaDonVi = '{0}'", sMaDV);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NS_DonVi>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }

        public IEnumerable<KeHoach5NamDeXuatModel> GetAllKeHoach5NamDeXuat(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int? iLoai, int? isTongHop)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("sMoTaChiTiet", sMoTaChiTiet);
                lstParam.Add("iGiaiDoanTu", iGiaiDoanTu);
                lstParam.Add("iGiaiDoanDen", iGiaiDoanDen);
                lstParam.Add("iLoai", iLoai);
                lstParam.Add("isTongHop", isTongHop);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                IEnumerable<KeHoach5NamDeXuatModel> items;
                // chứng từ 
                if (isTongHop == 1)
                {
                    items = conn.Query<KeHoach5NamDeXuatModel>("proc_get_all_kehoach5namdexuat_paging1", lstParam, commandType: CommandType.StoredProcedure);
                }
                // chứng từ tổng hợp
                else
                {
                    items = conn.Query<KeHoach5NamDeXuatModel>("proc_get_all_kehoach5namdexuat_paging2", lstParam, commandType: CommandType.StoredProcedure);
                }
                // var items = conn.Query<KeHoach5NamDeXuatModel>("proc_get_all_kehoach5namdexuat_paging1", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool deleteKeHoach5NamDeXuat(Guid Id)
        {

            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var sql = @"DELETE VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet WHERE iID_KeHoach5Nam_DeXuatID = @Id; " +
                " DELETE VDT_KHV_KeHoach5Nam_DeXuat WHERE iID_KeHoach5Nam_DeXuatID = @Id; " +
                " UPDATE  VDT_KHV_KeHoach5Nam_DeXuat set iID_TongHopParent = null WHERE iID_KeHoach5Nam_DeXuatID IN(" +
                " SELECT iID_KeHoach5Nam_DeXuatID FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE iID_TongHopParent = @Id );";

                var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(Id, trans);
                if (entity == null) return false;

                if (entity.iID_ParentId != null && entity.iID_ParentId.HasValue && entity.iID_ParentId != Guid.Empty)
                {
                    var itemParent = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(entity.iID_ParentId, trans);
                    if (itemParent != null)
                    {
                        itemParent.bActive = true;
                        itemParent.bIsGoc = false;
                        conn.Update(itemParent, trans);
                    }
                }

                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        Id
                    },
                    transaction: trans,
                    commandType: CommandType.Text);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }

        public IEnumerable<DuAnKeHoach5Nam> GetAllDuAnChuyenTiep(string iID_MaDonViThucHienDuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_du_an_chuyentiep.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var itemQuery = conn.Query<DuAnKeHoach5Nam>(sql,
                    param: new
                    {
                        iID_MaDonViThucHienDuAnID
                    },
                    commandType: CommandType.Text);

                return itemQuery;
            }
        }

        public VDT_KHV_KeHoach5Nam_DeXuat GetKeHoach5NamDeXuatById(Guid? Id)
        {
            VDT_KHV_KeHoach5Nam_DeXuat entity = new VDT_KHV_KeHoach5Nam_DeXuat();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(Id, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetKeHoach5NamDeXuatByCondition()
        {
            var sql = @"select dx.iID_KeHoach5Nam_DeXuatID, CONCAT(dx.sSoQuyetDinh, '-', dx.iGiaiDoanTu, '-', dx.iGiaiDoanDen) as sSoQuyetDinh
                            from VDT_KHV_KeHoach5Nam_DeXuat dx where bActive = 1 and sTongHop is null";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoach5Nam_DeXuat>(sql);
                return items;
            }
        }

        public KeHoach5NamDeXuatModel GetKeHoach5NamDeXuatByIdForDetail(Guid Id)
        {
            var sql = @"SELECT kh5nam.*, donVi.sTen as sTenDonvVi " +
                "FROM VDT_KHV_KeHoach5Nam_DeXuat kh5nam " +
                "INNER JOIN NS_DonVi as donVi on kh5nam.iID_DonViQuanLyID = donVi.iID_Ma " +
                "where kh5nam.iID_KeHoach5Nam_DeXuatID = '" + Id + "'";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<KeHoach5NamDeXuatModel>(sql);
                return items;
            }
        }

        public bool CheckExistSoKeHoach(string sSoQuyetDinh, int iNamLamViec, Guid? iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_exist_sokehoach_kh5namdx.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        sSoQuyetDinh,
                        iId,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public DataTable GetListKH5NamDeXuatChiTietById(string Id, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_kh5namdexuat_chitiet_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iId", Id);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }

        public DataTable GetListKH5NamDeXuatChuyenTiepChiTietById(string Id, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_kh5namdexuat_chuyentiep_chitiet_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iID_KeHoach5Nam_DeXuatID", Id);
                    return cmd.GetTable();
                }
            }
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> GetAllKH5NamDeXuatChiTiet(int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet ";
                var items = conn.Query<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(sql,
                    param: new
                    {
                        iNamLamViec
                    },
                    commandType: CommandType.Text);
                return items;
            }
        }

        public string GetMaxSTTDuAn(string Id)
        {
            var sql = "select TOP 1 sSTT from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet " +
                "where iID_ParentID is null " +
                "AND iID_KeHoach5Nam_DeXuatID = @Id " +
                "ORDER BY sSTT DESC ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<string>(sql,
                    param: new
                    {
                        Id
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public int GetNumchild(string Id)
        {
            var sql = "select count(*) from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet " +
                "where iID_ParentID is null " +
                "AND iID_KeHoach5Nam_DeXuatID = @Id ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        Id
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet GetKH5NamDeXuatChaBySTTDuAn(string iID_KeHoach5NamID, string sttDuAnCha)
        {
            var sql = @"select * from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet " +
                "where sSTT = @sttDuAnCha " +
                "AND iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(sql,
                    param: new
                    {
                        sttDuAnCha,
                        iID_KeHoach5NamID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public int GetMaxIndexCode(string Id, Guid IdParent)
        {
            var sql = @"Select ISNULL(MAX(iIndexCode), 0) from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet " +
                "where iID_ParentID = @IdParent " +
                "AND iID_KeHoach5Nam_DeXuatID = @Id ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        IdParent,
                        Id
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_DM_LoaiCongTrinh GetDMLoaiCongTrinhByMa(string sMaLoaiCongTrinh)
        {
            var sql = "SELECT * FROM VDT_DM_LoaiCongTrinh WHERE sMaLoaiCongTrinh = @sMaLoaiCongTrinh and bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<VDT_DM_LoaiCongTrinh>(sql, param: new
                {
                    sMaLoaiCongTrinh
                },
                commandType: CommandType.Text);

                return items;
            }
        }

        public bool UpdateIsParent(string iID_KeHoach5NamID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_update_isparent_kh5namdexuat_chitiet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_KeHoach5NamID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public bool SaveKeHoach5NamDeXuatChiTiet(IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet> data)
        {
            foreach (var item in data)
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(item.iID_KeHoach5Nam_DeXuat_ChiTietID, trans);
                    if (entity == null)
                        return false;
                    entity.fHanMucDauTu = item.fHanMucDauTu;
                    entity.fGiaTriNamThuNhat = item.fGiaTriNamThuNhat;
                    entity.fGiaTriNamThuHai = item.fGiaTriNamThuHai;
                    entity.fGiaTriNamThuBa = item.fGiaTriNamThuBa;
                    entity.fGiaTriNamThuTu = item.fGiaTriNamThuTu;
                    entity.fGiaTriNamThuNam = item.fGiaTriNamThuNam;
                    entity.fGiaTriBoTri = item.fGiaTriBoTri;

                    conn.Update(entity, trans);
                    // commit to db
                    trans.Commit();
                }
            }
            return true;
        }

        public bool UpdateGiaTriKeHoach(string iID_KeHoach5NamID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_update_giatrikehoach_kh5namdexuat.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_KeHoach5NamID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public bool CheckExistGiaiDoanKeHoach(int iGiaiDoanTu, int iGiaiDoanDen, int iNamLamViec, Guid? iID_DonViQuanLyID, Guid? iID_KeHoach5Nam_DeXuatID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_exist_giaidoan_kh5namdx.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        iGiaiDoanTu,
                        iGiaiDoanDen,
                        iID_DonViQuanLyID,
                        iID_KeHoach5Nam_DeXuatID,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public bool CheckExistDonVi_LoaiDuAn_GiaiDoanKeHoach(int iGiaiDoanTu, int iGiaiDoanDen, int iNamLamViec, Guid? iID_DonViQuanLyID, int? iLoai, Guid? iID_KeHoach5Nam_DeXuatID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_exist_donvi_giaidoan_loaiduan_kh5namdx.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        iGiaiDoanTu,
                        iGiaiDoanDen,
                        iID_DonViQuanLyID,
                        iID_KeHoach5Nam_DeXuatID,
                        iLoai,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public bool CheckPeriodValid(int iGiaiDoanTu, int iNamLamViec, Guid? iID_DonViQuanLyID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_period_valid_suggesstion_medium.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        iGiaiDoanTu,
                        iNamLamViec,
                        iID_DonViQuanLyID
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public VDT_KHV_KeHoach5Nam_DeXuat GetGiaiDoanTuDenDaTrung(int iGiaiDoanTu, int iNamLamViec, Guid? iID_DonViQuanLyID)
        {
            var sql = "SELECT DISTINCT iGiaiDoanTu, iGiaiDoanDen from VDT_KHV_KeHoach5Nam_DeXuat khthdx " +
                "where khthdx.iGiaiDoanTu <= @iGiaiDoanTu and khthdx.iGiaiDoanDen >= @iGiaiDoanTu " +
                "and khthdx.iNamLamViec = @iNamLamViec and khthdx.iID_DonViQuanLyID = @iID_DonViQuanLyID;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<VDT_KHV_KeHoach5Nam_DeXuat>(sql,
                    param: new
                    {
                        iGiaiDoanTu,
                        iNamLamViec,
                        iID_DonViQuanLyID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet GetKH5NamDeXuatChiTietById(Guid? Id)
        {
            VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet entity = new VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet>(Id, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public IEnumerable<NS_NguonNganSach> GetListDMNguonNganSach()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM NS_NguonNganSach WHERE iTrangThai=1 ORDER BY iSTT ";
                var items = conn.Query<NS_NguonNganSach>(sql,
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetVoucherVonNamDeXuatByCondition(int idNguonVon, List<string> arrDonVi, List<string> arrNguonVon, int iNamLamViec, string isStatus)
        {
            var sql =
                @"SELECT 
	                * 
                FROM 
	                VDT_KHV_KeHoachVonNam_DeXuat 
                WHERE 
	                iID_NguonVonID = @idNguonVon
	                AND iNamKeHoach = @iNamLamViec";

            if (isStatus.Equals("3"))
            {

            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         idNguonVon,
                         iNamLamViec
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }
        public IEnumerable<string> GetVoucherVonNamDeXuatIdByCondition(int idNguonVon, List<string> arrDonVi, List<string> arrNguonVon, int iNamLamViec, string isStatus)
        {
            var sql =
                @"SELECT convert(nvarchar(50), iID_KeHoachVonNamDeXuatID)
                FROM 
	                VDT_KHV_KeHoachVonNam_DeXuat 
                WHERE 
	                iID_NguonVonID = @idNguonVon
	                AND iNamKeHoach = @iNamLamViec
                    AND iID_MaDonViQuanLy IN (SELECT * FROM dbo.f_split(@lstMaDV))";
            var lstMaDV = string.Join(",", arrDonVi.Select(n => n.ToString()));

            if (isStatus.Equals("3"))
            {

            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<string>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         idNguonVon,
                         iNamLamViec,
                         lstMaDV
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public IEnumerable<KHVNDXExportModel> GetReportKeHoachVonNam(int type, string theLoaiCongTrinh, string lstId, string lstLct, double donViTinh)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("type", type);
                lstParam.Add("theLoaiCongTrinh", theLoaiCongTrinh);
                lstParam.Add("lstId", lstId);
                lstParam.Add("lstLct", lstLct);
                lstParam.Add("DonViTienTe", donViTinh);
                var items = conn.Query<KHVNDXExportModel>("sp_export_baocao_kehoachvonnam_donvi", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel> GetPhanBoVonDieuChinhNguonVon(int type, string lstId, string lstLct, string lstNguonVon, double donViTienTe)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("type", type);
                lstParam.Add("lstId", lstId);
                lstParam.Add("lstLct", lstLct);
                lstParam.Add("lstNguonVon", lstNguonVon);
                lstParam.Add("DonViTienTe", donViTienTe);
                var items = conn.Query<VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel>("sp_vdt_get_phan_bo_von_don_vi_dieu_chinh_nguonvon_report", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<PhanBoVonDonViDieuChinhNSQPReport> GetPhanBoVonDieuChinhReport(string lstId, string lstLct, int yearPlan, int type, double donViTienTe)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("lstId", lstId);
                lstParam.Add("lstLct", lstLct);
                lstParam.Add("YearPlan", yearPlan);
                lstParam.Add("type", type);
                lstParam.Add("DonViTienTe", donViTienTe);
                var items = conn.Query<PhanBoVonDonViDieuChinhNSQPReport>("sp_vdt_get_phan_bo_von_don_vi_dieu_chinh_report", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<KH5NDXPrintDataExportModel> GetDataReportKH5NDeXuat(string id, string lct, string lstNguonVon, string lstMaDonViQL, int type, double donViTinh, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("Id", id);
                lstParam.Add("lct", lct);
                lstParam.Add("IdNguonVon", lstNguonVon);
                lstParam.Add("IdMaDVQL", lstMaDonViQL);
                lstParam.Add("type", type);
                lstParam.Add("MenhGiaTienTe", donViTinh);
                lstParam.Add("iNamLamViec", iNamLamViec);
                var items = conn.Query<KH5NDXPrintDataExportModel>("sp_vdt_khv_kehoach_5_nam_de_xuat_export_clone", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel> FindByReportKeHoachTrungHanDeXuatChuyenTiep(string lstId, string lstBudget, string lstLoaiCongTrinh, string lstUnit, int type, double donViTinh)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("lstId", lstId);
                lstParam.Add("lstBudget", lstBudget);
                lstParam.Add("lstLoaiCongTrinh", lstLoaiCongTrinh);
                lstParam.Add("lstUnit", lstUnit);
                lstParam.Add("type", type);
                lstParam.Add("DonViTienTe", donViTinh);
                //var items = conn.Query<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel>("sp_vdt_kehoach5nam_dexuat_chitiet_chuyentiep_report", lstParam, commandType: CommandType.StoredProcedure);
                var items = conn.Query<VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel>("sp_vdt_kehoach5nam_dexuat_chitiet_chuyentiep_report_test", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<KH5NDXPrintDataDieuChinhExportModel> FindSuggestionDcReport(int type, string lstId, string lstDonVi, double menhGiaTienTe)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("type", type);
                lstParam.Add("VoucherId", lstId);
                lstParam.Add("lstDonVi", lstDonVi);
                lstParam.Add("MenhGiaTienTe", menhGiaTienTe);
                var items = conn.Query<KH5NDXPrintDataDieuChinhExportModel>("sp_khv_khth_dexuat_dieuchinh_report", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<string> GetLstIdChungTuDeXuat(int iGiaiDoanTu, int iGiaiDoanDen, bool isModified, bool isCt, int iNamLamViec)
        {
            var sql =
                @"SELECT convert(nvarchar(50), iID_KeHoach5Nam_DeXuatID)
                FROM VDT_KHV_KeHoach5Nam_DeXuat 
                where iGiaiDoanTu = @iGiaiDoanTu and iGiaiDoanDen = @iGiaiDoanDen
                and bActive = 1 and bIsGoc = 1 and iLoai = 1 and sTongHop is null
                ";

            if (isModified && !isCt)
            {
                sql =
                @"SELECT convert(nvarchar(50), iID_KeHoach5Nam_DeXuatID)
                FROM VDT_KHV_KeHoach5Nam_DeXuat 
                where iGiaiDoanTu = @iGiaiDoanTu and iGiaiDoanDen = @iGiaiDoanDen
                and bActive = 1 and bIsGoc = 0 and iLoai = 1 and sTongHop is null
                ";
            }
            else if (!isModified && isCt)
            {
                sql =
               @"SELECT convert(nvarchar(50), iID_KeHoach5Nam_DeXuatID)
                FROM VDT_KHV_KeHoach5Nam_DeXuat 
                where iGiaiDoanTu = @iGiaiDoanTu and iGiaiDoanDen = @iGiaiDoanDen
                and bActive = 1 and bIsGoc = 1 and iLoai = 2 and sTongHop is null";
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<string>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iGiaiDoanTu,
                         iGiaiDoanDen,
                         iNamLamViec
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public List<VDT_DM_LoaiCongTrinh> GetListParentDMLoaiCongTrinh()
        {
            var result = new List<VDT_DM_LoaiCongTrinh>();

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_LoaiCongTrinh>("SELECT * FROM VDT_DM_LoaiCongTrinh where iID_Parent is null and bActive = 1");
                if (items != null && items.Any())
                {
                    result.AddRange(items);
                }
            }

            return result;
        }
        #endregion

        #region Kế hoạch vốn năm đề xuất
        public IEnumerable<KeHoachVonNamChiTietViewModel> GetAllVonNamDeXuatByIdDx(Guid? iIDVonNamDeXuat)
        {
            var sql = "SELECT null as IdParentNew,* FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet where iID_KeHoachVonNamDeXuatID = @iIDVonNamDeXuat";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<KeHoachVonNamChiTietViewModel>(sql,
                   param: new
                   {
                       iIDVonNamDeXuat,
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        public IEnumerable<KeHoachVonNamChiTietViewModel> GetAllKHVonNamDeXuatDieuChinhChiTiet(Guid iID_KeHoachVonNamDeXuatID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_khVonnamdexuat_dieuchinh_chitiet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<KeHoachVonNamChiTietViewModel>(sql,
                    param: new
                    {
                        iID_KeHoachVonNamDeXuatID
                    },
                    commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> GetAllKeHoachVonNamDeXuat(ref PagingInfo _paging, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, int? iNamKeHoach, int? iID_NguonVonID, Guid? iID_DonViQuanLyID, int? isTongHop)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iID_NguonVonID", iID_NguonVonID);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("isTongHop", isTongHop);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> items;
                if (isTongHop == 1)
                {
                    items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>("proc_get_all_kehoachvonnamdexuat_paging1", lstParam, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> allItems = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>("proc_get_all_kehoachvonnamdexuat_paging2", lstParam, commandType: CommandType.StoredProcedure);
                    items = new List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>();
                    foreach (VDT_KHV_KeHoachVonNam_DeXuat_ViewModel oneitem in allItems)
                    {
                        if (oneitem.sTongHop != null)
                        {
                            items.AsList().Add(oneitem);
                            List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> children = allItems.Where(item => item.iID_TongHopParent == oneitem.iID_KeHoachVonNamDeXuatID).ToList();
                            items.AsList().AddRange(children);
                        }
                    }
                }
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_KHV_KeHoachVonNam_DeXuat GetKeHoachVonNamDeXuatById(Guid? iID_KeHoachVonNamDeXuatID)
        {
            VDT_KHV_KeHoachVonNam_DeXuat entity = new VDT_KHV_KeHoachVonNam_DeXuat();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(iID_KeHoachVonNamDeXuatID, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public VDT_KHV_KeHoachVonNam_DeXuat_ViewModel GetKeHoachVonNamDeXuatViewModelById(Guid? iID_KeHoachVonNamDeXuatID)
        {

            StringBuilder query = new StringBuilder();
            query.Append("SELECT vndx.*, donVi.sTen AS sTenDonvi, nv.sTen AS sTenNguonVon FROM VDT_KHV_KeHoachVonNam_DeXuat vndx ");
            query.Append("LEFT JOIN NS_DonVi donVi ON vndx.iID_DonViQuanLyID = donVi.iID_Ma ");
            query.Append("LEFT JOIN NS_NguonNganSach nv ON vndx.iID_NguonVonID = nv.iID_MaNguonNganSach ");
            query.AppendFormat("WHERE vndx.iID_KeHoachVonNamDeXuatID = '{0}' ", iID_KeHoachVonNamDeXuatID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }
        public bool CheckExistSoKeHoachVonNamDeXuat(string sSoQuyetDinh, int iNamKeHoach, Guid? iID_KeHoachVonNamDeXuatID)
        {
            StringBuilder query = new StringBuilder();
            if (iID_KeHoachVonNamDeXuatID != null)
            {
                query.AppendFormat("SELECT COUNT(iID_KeHoachVonNamDeXuatID) FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE sSoQuyetDinh = '{0}' AND iID_KeHoachVonNamDeXuatID <> '{1}' AND iNamKeHoach = {2} ",
                    sSoQuyetDinh, iID_KeHoachVonNamDeXuatID, iNamKeHoach);
            }
            else
                query.AppendFormat("SELECT COUNT(iID_KeHoachVonNamDeXuatID) FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE sSoQuyetDinh = '{0}' AND iNamKeHoach = {1} ",
                    sSoQuyetDinh, iNamKeHoach);

            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirstOrDefault<int>(query.ToString(), commandType: CommandType.Text);
                return count > 0;
            }
        }

        public bool SaveKeHoachVonNamDeXuat(ref Guid iID, VDT_KHV_KeHoachVonNam_DeXuat data, List<KeHoachVonNamChiTietViewModel> lstDetail, string sUserName, bool isDieuChinh, bool isTongHop, bool bIsDetail)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_KeHoachVonNamDeXuatID == null || data.iID_KeHoachVonNamDeXuatID == Guid.Empty)
                {
                    var entity = new VDT_KHV_KeHoachVonNam_DeXuat();
                    entity.MapFrom(data);
                    var donvi = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                    if (donvi == null)
                        return false;
                    entity.iID_KeHoachVonNamDeXuatID = Guid.NewGuid();
                    entity.iID_MaDonViQuanLy = donvi.iID_MaDonVi;
                    entity.bActive = true;
                    entity.sUserCreate = sUserName;
                    entity.dDateCreate = DateTime.Now;
                    entity.iID_KeHoachVonNam_DeXuatGocID = entity.iID_KeHoachVonNamDeXuatID;


                    if (isTongHop)
                    {
                        var lstItemTongHop = lstDetail.GroupBy(x => x.iID_KeHoachVonNamDeXuatID).Select(grp => grp.FirstOrDefault());
                        foreach (var item in lstItemTongHop)
                        {
                            var entityUpdate = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(item.iID_KeHoachVonNamDeXuatID, trans);
                            if (entityUpdate != null)
                            {
                                entityUpdate.iID_TongHopParent = entity.iID_KeHoachVonNamDeXuatID;

                                conn.Update(entityUpdate, trans);
                            }
                        }
                        lstDetail.Select(item =>
                        {
                            item.iID_KeHoachVonNamDeXuatChiTietID = Guid.NewGuid();
                            item.iID_KeHoachVonNamDeXuatID = entity.iID_KeHoachVonNamDeXuatID;
                            return item;
                        }).ToList();

                        entity.fThuHoiVonUngTruoc = lstDetail.Sum(x => x.fThuHoiVonUngTruoc);
                        entity.fThanhToan = lstDetail.Sum(x => x.fThanhToan);
                        conn.Insert<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet>(lstDetail, trans);
                    }
                    iID = entity.iID_KeHoachVonNamDeXuatID;
                    conn.Insert(entity, trans);


                }
                else
                {
                    if (isDieuChinh)
                    {
                        var entityGoc = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(data.iID_KeHoachVonNamDeXuatID, trans);
                        var entity = new VDT_KHV_KeHoachVonNam_DeXuat();
                        entity.MapFrom(data);
                        entity.iID_KeHoachVonNamDeXuatID = Guid.Empty;
                        entity.iID_KeHoachVonNam_DeXuatGocID = data.iID_KeHoachVonNamDeXuatID;
                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        entity.iID_MaDonViQuanLy = entityGoc.iID_MaDonViQuanLy;
                        entity.iNamKeHoach = entityGoc.iNamKeHoach;
                        entity.iID_NguonVonID = entityGoc.iID_NguonVonID;
                        entity.iID_ParentId = data.iID_KeHoachVonNamDeXuatID;
                        entity.bActive = true;
                        entity.bIsGoc = false;
                        entity.sUserCreate = sUserName;
                        entity.dDateCreate = DateTime.Now;
                        conn.Insert(entity, trans);


                        entityGoc.bActive = false;
                        entityGoc.sUserUpdate = sUserName;
                        entityGoc.dDateUpdate = DateTime.Now;
                        conn.Update(entityGoc, trans);

                        iID = entity.iID_KeHoachVonNamDeXuatID;
                    }
                    else
                    {
                        iID = data.iID_KeHoachVonNamDeXuatID;
                        var entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(data.iID_KeHoachVonNamDeXuatID, trans);
                        if (entity == null)
                            return false;
                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        var donvi = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                        if (donvi == null)
                            return false;
                        entity.iID_MaDonViQuanLy = donvi.iID_MaDonVi;
                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.iID_NguonVonID = data.iID_NguonVonID;
                        entity.iNamKeHoach = data.iNamKeHoach;
                        entity.sNguoiLap = data.sNguoiLap;
                        entity.sTruongPhong = data.sTruongPhong;
                        entity.sUserUpdate = sUserName;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);
                    }

                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public List<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet> GetKHVNDeXuatChiTietByParentId(Guid iIdParentID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet WHERE iID_KeHoachVonNamDeXuatID = '{0}'", iIdParentID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return items.ToList();
            }
        }

        public DataTable GetListKHVonNamDeXuatChiTietById(string iID_KeHoachVonNamDeXuatID, string lstDuAnID, Dictionary<string, string> _filters)
        {
            //DataTable dt = new DataTable();
            //return dt;

            //var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamdexuat_chitiet_by_id.sql");
            //var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamdexuat_chitiet_by_id.sql");
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamdexuat_chitiet_by_id_V2.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        _filters.ToList().ForEach(x =>
                        {
                            cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                        });
                        cmd.Parameters.AddWithValue("@iIDKHVNDeXuatId", iID_KeHoachVonNamDeXuatID);
                        cmd.Parameters.AddWithValue("@lstDuAnID", lstDuAnID);
                        return cmd.GetTable();
                    }
                }

            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return new DataTable();
        }

        public IEnumerable<VdtKhvKeHoach5NamDeXuatExportModel> GetDataExportKeHoachTrungHanDeXuat(Guid iID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iId", iID);
                var items = conn.Query<VdtKhvKeHoach5NamDeXuatExportModel>("sp_vdt_kehoachtrunghan_dexuat_export", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public bool SaveKeHoachVonNamDeXuatChiTiet(List<KeHoachVonNamChiTietViewModel> aListModel, List<KeHoachVonNamChiTietViewModel> listDeleted)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    List<KeHoachVonNamChiTietViewModel> lstInsert = new List<KeHoachVonNamChiTietViewModel>();
                    List<KeHoachVonNamChiTietViewModel> lstUpdate = new List<KeHoachVonNamChiTietViewModel>();
                    lstInsert = aListModel.Where(x => x.iID_KeHoachVonNamDeXuatChiTietID == null || x.iID_KeHoachVonNamDeXuatChiTietID == Guid.Empty).ToList();
                    lstUpdate = aListModel.Where(x => x.iID_KeHoachVonNamDeXuatChiTietID != null && x.iID_KeHoachVonNamDeXuatChiTietID != Guid.Empty).ToList();
                    conn.Insert<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet>(lstInsert);
                    foreach (var item in lstUpdate)
                    {
                        var entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet>(item.iID_KeHoachVonNamDeXuatChiTietID);
                        entity.fUocThucHien = item.fUocThucHien;
                        entity.fThuHoiVonUngTruoc = item.fThuHoiVonUngTruoc;
                        entity.fThanhToan = item.fThanhToan;
                        entity.iLoaiDuAn = item.iLoaiDuAn;
                        conn.Update(entity);
                    }
                    if (listDeleted != null)
                    {
                        foreach (var item in listDeleted)
                        {
                            var entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet>(item.iID_KeHoachVonNamDeXuatChiTietID);
                            entity.fUocThucHien = item.fUocThucHien;
                            entity.fThuHoiVonUngTruoc = item.fThuHoiVonUngTruoc;
                            entity.fThanhToan = item.fThanhToan;
                            conn.Delete(entity);
                        }
                    }
                    var entityGoc = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(aListModel.FirstOrDefault().iID_KeHoachVonNamDeXuatID);
                    entityGoc.fThuHoiVonUngTruoc = aListModel.Sum(x => x.fThuHoiVonUngTruoc);
                    entityGoc.fThanhToan = aListModel.Sum(x => x.fThanhToan);
                    conn.Update(entityGoc);

                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetListSoChungTuVonNamDeXuat(Guid? iID_DonViQuanLyID, int iNamLamViec, string isStatus)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT vndx.iID_KeHoachVonNamDeXuatID, CONCAT(vndx.sSoQuyetDinh, ' (', vndx.iNamKeHoach, ')') AS sSoQuyetDinh ");
            query.Append("FROM VDT_KHV_KeHoachVonNam_DeXuat vndx ORDER BY vndx.sSoQuyetDinh");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public IEnumerable<KHVNDXPrintDataExportModel> GetDataReportKHVNDeXuat(string arrIdKHVNDX, string iID_LoaiCongTrinhs, string iID_NguonVonIDs, int type)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("Id", arrIdKHVNDX);
                lstParam.Add("lct", iID_LoaiCongTrinhs);
                lstParam.Add("IdNguonVon", iID_NguonVonIDs);
                lstParam.Add("type", type);
                var items = conn.Query<KHVNDXPrintDataExportModel>("sp_vdt_khv_kehoach_5_nam_de_xuat_export", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public VDT_KHV_KeHoachVonNam_DeXuat_ViewModel GetKeHoachVonNamDeXuatByIdDetail(Guid iID_KeHoachVonNamDeXuatID)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT vndx.*, donVi.sTen AS sTenDonvi, nv.sTen AS sTenNguonVon FROM VDT_KHV_KeHoachVonNam_DeXuat vndx ");
            query.Append("LEFT JOIN NS_DonVi donVi ON vndx.iID_DonViQuanLyID = donVi.iID_Ma ");
            query.Append("LEFT JOIN NS_NguonNganSach nv ON vndx.iID_NguonVonID = nv.iID_MaNguonNganSach ");
            query.AppendFormat("WHERE vndx.iID_KeHoachVonNamDeXuatID = '{0}' ", iID_KeHoachVonNamDeXuatID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }

        public VDTKHVPhanBoVonDonViPheDuyetViewModel GetKeHoachVonPBVDonViPheDuyetByIdDetail(Guid iID_KeHoachPBV)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT vpb.*, donVi.sTen AS sTenDonvi, nv.sTen AS sTenNguonVon FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet vpb ");
            query.Append("LEFT JOIN NS_DonVi donVi ON vpb.iID_DonViQuanLyID = donVi.iID_Ma ");
            query.Append("LEFT JOIN NS_NguonNganSach nv ON vpb.iID_NguonVonID = nv.iID_MaNguonNganSach ");
            query.AppendFormat("WHERE vpb.Id = '{0}' ", iID_KeHoachPBV);
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDTKHVPhanBoVonDonViPheDuyetViewModel>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                return item;
            }
        }

        public List<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> GetSTongHopKeHoachVonNamDeXuat(Guid? iID_KeHoachVonNamDeXuatID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_khv_get_thongtinchungtutonghop_by_idkhvn.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel>(sql,
                            param:
                            new
                            {
                                iID_KeHoachVonNamDeXuatID
                            },
                            commandType: CommandType.Text);


                return items.ToList();
            }
        }

        public bool DeleteKeHoachVonNamDeXuat(Guid iID_KeHoachVonNamDeXuatID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet WHERE iID_KeHoachVonNamDeXuatID = '{0}'; ", iID_KeHoachVonNamDeXuatID);
                query.AppendFormat("DELETE VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_KeHoachVonNamDeXuatID = '{0}'; ", iID_KeHoachVonNamDeXuatID);
                query.Append("UPDATE VDT_KHV_KeHoachVonNam_DeXuat SET iID_TongHopParent = NULL WHERE iID_KeHoachVonNamDeXuatID IN ");
                query.AppendFormat("(SELECT iID_KeHoachVonNamDeXuatID FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_TongHopParent = '{0}');", iID_KeHoachVonNamDeXuatID);

                var entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(iID_KeHoachVonNamDeXuatID, trans);
                if (entity == null) return false;

                if (entity.iID_ParentId != null && entity.iID_ParentId.HasValue && entity.iID_ParentId != Guid.Empty)
                {
                    var itemParent = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(entity.iID_ParentId, trans);
                    if (itemParent != null)
                    {
                        itemParent.bActive = true;
                        itemParent.bIsGoc = false;
                        conn.Update(itemParent, trans);
                    }
                }

                var r = conn.Execute(query.ToString(), commandType: CommandType.Text, transaction: trans);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }
        public bool LockOrUnLockKeHoachVonNamDeXuat(Guid id, bool isLockOrUnLock)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var entity = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(id, trans);
                if (entity == null) return false;
                entity.iKhoa = isLockOrUnLock;

                conn.Update(entity, trans);

                trans.Commit();
                conn.Close();

                return true;
            }
        }

        public IEnumerable<VDT_DA_DuAn_HangMuc> GetDuAnHangMucByListDuAnID(string iID_KeHoachVonNamDeXuatID, string lstDuAnID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_hangmuc_by_duan_khvndx.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn_HangMuc>(sql,
                            param:
                            new
                            {
                                iIDKHVNDeXuatId = Guid.Parse(iID_KeHoachVonNamDeXuatID),
                                lstDuAnID = lstDuAnID
                            },
                            commandType: CommandType.Text);


                return items.ToList();
            }
        }

        public IEnumerable<VDTDuAnViewModel> GetDuAnKHVNDXByDonVi(Guid iID_DonViID, int iNamKeHoach, int iID_NguonVonID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT dv.* FROM NS_DonVi dv WHERE dv.iID_Ma = '{0}' ", iID_DonViID);
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamdexuat_by_donvi.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NS_DonVi>(new CommandDefinition(
                     commandText: query.ToString(),
                     commandType: CommandType.Text
                 ));

                var items = conn.Query<VDTDuAnViewModel>(sql,
                            param:
                            new
                            {
                                iIdMaDonViQuanLy = item.iID_MaDonVi,
                                iNamKeHoach = iNamKeHoach
                            },
                            commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region Kế hoạch vốn năm được duyệt
        public bool DeleteKeHoachVonNamDuocDuyet(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                VDT_KHV_KeHoachVonNam_DuocDuyet itemDelete = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(id, trans);
                if (itemDelete != null)
                {
                    if (itemDelete.iID_ParentId.HasValue && itemDelete.iID_ParentId != null)
                    {
                        VDT_KHV_KeHoachVonNam_DuocDuyet itemUpdate = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(itemDelete.iID_ParentId, trans);
                        if (itemUpdate != null)
                        {
                            itemUpdate.bActive = true;
                            conn.Update(itemUpdate, trans);
                        }
                    }

                    var idPhanBoVon = itemDelete.iID_KeHoachVonNam_DuocDuyetID;
                    var sql = "select * from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet where iID_KeHoachVonNam_DuocDuyetID = @idPhanBoVon";
                    var items = conn.Query<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet>(sql,
                    param: new
                    {
                        idPhanBoVon
                    },
                    commandType: System.Data.CommandType.Text,
                    transaction: trans);

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            conn.Delete(item, trans);
                        }
                    }

                    conn.Delete(itemDelete, trans);
                }

                trans.Commit();
            }

            return true;
        }
        public bool DeleteKeHoachVonNamPhanBoVonDVPheDuyet(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                VDT_KHV_PhanBoVon_DonVi_PheDuyet itemDelete = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(id, trans);
                if (itemDelete != null)
                {
                    if (itemDelete.iID_ParentId.HasValue && itemDelete.iID_ParentId != null)
                    {
                        VDT_KHV_PhanBoVon_DonVi_PheDuyet itemUpdate = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(itemDelete.iID_ParentId, trans);
                        if (itemUpdate != null)
                        {
                            itemUpdate.bActive = true;
                            conn.Update(itemUpdate, trans);
                        }
                    }

                    var idPhanBoVon = itemDelete.Id;
                    var sql = "select * from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet where iID_PhanBoVon_DonVi_PheDuyet_ID = @idPhanBoVon";
                    var items = conn.Query<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>(sql,
                    param: new
                    {
                        idPhanBoVon
                    },
                    commandType: System.Data.CommandType.Text,
                    transaction: trans);

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            conn.Delete(item, trans);
                        }
                    }

                    conn.Delete(itemDelete, trans);
                }

                trans.Commit();
            }

            return true;
        }

        // kiểm tra xem các item đang gửi về để edit có tồn tại hay không
        public bool checkExistDonViVonNamPheDuyetChiTiet(List<Guid> listVonNamPheDuyetChiTietIds)
        {
            try
            {
                if (listVonNamPheDuyetChiTietIds.Count > 0)
                {
                    var arrIds = "(";
                    listVonNamPheDuyetChiTietIds.ForEach(item =>
                    {
                        arrIds += $"'{item.ToString()}',";
                    });
                    arrIds = arrIds.Substring(0, arrIds.Length - 1) + ')';
                    var sql = "select * from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet where Id in " + arrIds;
                    using (var conn = _connectionFactory.GetConnection())
                    {
                        var items = conn.Query<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>(sql,
                            commandType: System.Data.CommandType.Text);

                        return items.ToList().Count > 0;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SaveKHVonNamDonViPheDuyetChiTiet(List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> listkhChiTiet, Guid? iID_KeHoachVonNamDeXuatID)
        {
            try
            {
                var listDuAnIds = string.Join(",", listkhChiTiet.Select(i => i.iID_DuAnID).ToList().Select(n=>n));
                List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> listDataCopy = new List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>();
                listDataCopy.AddRange(listkhChiTiet);
                List <VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> lstUpdate = listkhChiTiet.Where(x => (x.Id != null && x.Id != Guid.Empty)).ToList();
                List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet> lstAdd = listkhChiTiet.Where(x => (x.Id == null || x.Id == Guid.Empty)).ToList();
                var sql = "select DISTINCT iID_DuAnID from VDT_KHV_KeHoachVonNam_DeXuat dx\r\ninner join VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct on dx.iID_KeHoachVonNamDeXuatID = ct.iID_KeHoachVonNamDeXuatID\r\nwhere ct.iID_DuAnID in (Select * from f_split(@listDuAnIds)) and ct.iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID";

                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (iID_KeHoachVonNamDeXuatID != null && iID_KeHoachVonNamDeXuatID != Guid.Empty)
                    {
                        var entityKHDX = conn.Get<VDT_KHV_KeHoachVonNam_DeXuat>(iID_KeHoachVonNamDeXuatID, trans);
                        if (entityKHDX != null)
                        {
                            var items = conn.Query<Guid>(sql,
                                      param: new
                                      {

                                          listDuAnIds,
                                          iID_KeHoachVonNamDeXuatID
                                      },
                                      transaction: trans,
                                      commandType: System.Data.CommandType.Text);

                            if (items.Any())
                            {
                                foreach(var item in items)
                                {
                                    foreach (var item2 in listkhChiTiet)
                                    {
                                        if(item != item2.iID_DuAnID)
                                        {
                                            listDataCopy.RemoveAll(i => i.iID_DuAnID == item2.iID_DuAnID);
                                        }
                                    }
                                }
                            }
                            lstUpdate = new List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>();
                            lstAdd = new List<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>();
                            lstUpdate = listDataCopy.Where(x => (x.Id != null && x.Id != Guid.Empty)).ToList();
                            lstAdd = listDataCopy.Where(x => (x.Id == null || x.Id == Guid.Empty)).ToList();
                        }
                    }

                        if (lstAdd.Count() > 0)
                    {
                        foreach (var item in lstAdd)
                        {
                            item.Id = Guid.NewGuid();
                            conn.Insert(item, trans);

                            var entityDuAn = conn.Get<VDT_DA_DuAn>(item.iID_DuAnID, trans);
                            if (entityDuAn != null && entityDuAn.sTrangThaiDuAn.Equals("KhoiTao"))
                            {
                                entityDuAn.sTrangThaiDuAn = "THUC_HIEN";
                                conn.Update(entityDuAn, trans);
                            }
                        }
                    }

                    if (lstUpdate.Count() > 0)
                    {
                        foreach (var item in lstUpdate)
                        {
                            var entityDetail = conn.Get<VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet>(item.Id, trans);
                            if (entityDetail != null)
                            {
                                conn.Update(item, trans);
                            }

                            var entityDuAn = conn.Get<VDT_DA_DuAn>(item.iID_DuAnID, trans);
                            if (entityDuAn != null && entityDuAn.sTrangThaiDuAn.Equals("KhoiTao"))
                            {
                                entityDuAn.sTrangThaiDuAn = "THUC_HIEN";
                                conn.Update(entityDuAn, trans);
                            }
                        }
                    }
                    //Update iD KHVN DX
                    var keHoachVNDVDuocDuyet = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(listkhChiTiet.FirstOrDefault().iID_PhanBoVon_DonVi_PheDuyet_ID, trans);
                    if (keHoachVNDVDuocDuyet != null)
                    {
                        var tongGiaTriPhanBo = listkhChiTiet.Sum(x => x.fGiaTriPhanBo);
                        keHoachVNDVDuocDuyet.iID_VonNamDeXuatID = iID_KeHoachVonNamDeXuatID;
                        keHoachVNDVDuocDuyet.fGiaTrPhanBo = tongGiaTriPhanBo;
                        conn.Update(keHoachVNDVDuocDuyet, trans);
                    }

                    //Kết thúc lưu bảng tổng hợp
                    trans.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public IEnumerable<KeHoachVonNamDuocDuyetExportDieuChinh> GetKeHoachVonNamDuocDuyetDieuChinhReport(string lstId, double DonViTienTe)
        {
            var sql = FileHelpers.GetSqlQuery("sp_vdt_phan_bo_von_dieu_chinh_report.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<KeHoachVonNamDuocDuyetExportDieuChinh>(sql,
                   param: new
                   {
                       lstId,
                       DonViTienTe
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDT_KHV_KeHoachVonNam_DuocDuyet> GetKeHoachVonNamDuocDuyetDieuChinh(string donViQuanLy, string namKeHoach)
        {
            var sql = "SELECT khvn.* from VDT_KHV_KeHoachVonNam_DuocDuyet khvn where khvn.iNamKeHoach = @namKeHoach " +
                        "and khvn.iID_DonViQuanLyID in (select * from dbo.f_split(@donViQuanLy)) and bIsGoc = 0 and bActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DuocDuyet>(sql,
                   param: new
                   {
                       namKeHoach,
                       donViQuanLy
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDTKeHoachVonNamDuocDuyetExport> GetKeHoachVonNamDuocDuyetReport(string lstId, string lct, int type, double MenhGiaTienTe)
        {
            var sql = FileHelpers.GetSqlQuery("sp_vdt_khv_kehoach_von_nam_duoc_duyet_export.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKeHoachVonNamDuocDuyetExport>(sql,
                   param: new
                   {
                       lstId,
                       lct,
                       type,
                       MenhGiaTienTe
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        public NS_DonVi GetDonViQuanLyById(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var item = conn.Get<NS_DonVi>(id, trans);

                trans.Commit();

                return item;
            }
        }
        public IEnumerable<VDT_KHV_KeHoachVonNam_DuocDuyet> GetKeHoachVonNamDuocDuyetByCondition(string namKeHoach, string donViQuanLy, string nguonVon)
        {
            var sql = "SELECT khvn.* from VDT_KHV_KeHoachVonNam_DuocDuyet khvn where khvn.iID_NguonVonID = @nguonVon and khvn.iNamKeHoach = @namKeHoach " +
                        "and khvn.iID_DonViQuanLyID in (select * from dbo.f_split(@donViQuanLy)) and bIsGoc = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DuocDuyet>(sql,
                   param: new
                   {
                       namKeHoach,
                       donViQuanLy,
                       nguonVon
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }
        public IEnumerable<KHVNDXExportExcelModel> GetKeHoachVonNamExcel(string id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("Id", id);
                var items = conn.Query<KHVNDXExportExcelModel>("sp_vdt_get_ke_hoach_von_nam_Excel", lstParam, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        public IEnumerable<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> GetKeHoachVonNamChiTietExport(string iiDKeHoachVonNam)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_ke_hoach_von_nam_duoc_duyet_chi_tiet_export.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKHVPhanBoVonDuocDuyetChiTietViewModel>(sql,
                   param: new
                   {
                       iiDKeHoachVonNam
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }

        public IEnumerable<VDTKHVPhanBoVonDuocDuyetViewModel> GetKeHoachVonNamExport(string id)
        {
            var sql = "SELECT khvn.*, '' as STT from VDT_KHV_KeHoachVonNam_DuocDuyet khvn where khvn.iID_KeHoachVonNam_DuocDuyetID = @id";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKHVPhanBoVonDuocDuyetViewModel>(sql,
                   param: new
                   {
                       id
                   },
                   commandType: System.Data.CommandType.Text);

                return items;
            }
        }
        public DataTable GetListKHVonNamDuocDuyetChiTietById(string iIDPhanBoVonID, string iIdPhanBoVonDeXuat, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamduocduyet_chitiet_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@phanBoVonId", iIDPhanBoVonID);
                    cmd.Parameters.AddWithValue("@iIdPhanBoVonDeXuat", iIdPhanBoVonDeXuat);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }
        public DataTable GetListKHVonNamPhanBoVonDonViPheDuyetChiTietById(string iIDPhanBoVonID, string iIdPhanBoVonDeXuat, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_khvonnamduocduyet_chitiet_by_id_beta.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        //_filters.ToList().ForEach(x =>
                        //{
                        //    cmd.Parameters.AddWithValue($"@{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                        //});
                        cmd.Parameters.AddWithValue("@phanBoVonId", Guid.Parse(iIDPhanBoVonID));
                        cmd.Parameters.AddWithValue("@iIdPhanBoVonDeXuat", Guid.Parse(iIdPhanBoVonDeXuat));
                        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        return cmd.GetTable();
                    }
                }

            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return new DataTable();
        }

        public bool SaveKeHoachVonNamDuocDuyetChiTiet(List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> data, List<NS_MucLucNganSach> lsMucLucNganSach, Guid iID_KeHoachVonNamDeXuatID, Guid iID_KeHoachVonNamDuocDuyetID)
        {
            Dictionary<string, Guid> dicMucLucNganSach = new Dictionary<string, Guid>();
            foreach (var n in lsMucLucNganSach)
            {
                string sKey = n.sLNS + "-" + n.sL + "-" + n.sK + "-" + n.sM + "-" + n.sTM + "-" + n.sTTM + "-" + n.sNG;
                if (!dicMucLucNganSach.ContainsKey(sKey))
                    dicMucLucNganSach.Add(sKey, n.iID_MaMucLucNganSach);
            }
            data.Select(item =>
            {
                item.L = (item.L == null ? string.Empty : item.L);
                item.K = (item.K == null ? string.Empty : item.K);
                item.M = (item.M == null ? string.Empty : item.M);
                item.TM = (item.TM == null ? string.Empty : item.TM);
                item.TTM = (item.TTM == null ? string.Empty : item.TTM);
                item.NG = (item.NG == null ? string.Empty : item.NG);

                var itemMucLucNs = lsMucLucNganSach.Where(x => x.sLNS == item.LNS && x.sL == item.L && x.sK == item.K && x.sM == item.M && x.sTM == item.TM && x.sTTM == item.TTM && x.sNG == item.NG).FirstOrDefault();
                if (itemMucLucNs != null)
                {
                    if (item.TM == item.TTM && item.TM == item.NG && string.IsNullOrEmpty(item.TM))
                    {
                        item.iID_MucID = itemMucLucNs.iID_MaMucLucNganSach;
                        item.iID_TieuMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_NganhID = null;
                    }
                    else if (!string.IsNullOrEmpty(item.TM) && item.TTM == item.NG)
                    {
                        item.iID_MucID = null;
                        item.iID_TieuMucID = itemMucLucNs.iID_MaMucLucNganSach;
                        item.iID_TietMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_NganhID = null;
                    }
                    else if (!string.IsNullOrEmpty(item.TM) && !string.IsNullOrEmpty(item.TTM) && string.IsNullOrEmpty(item.NG))
                    {
                        item.iID_MucID = null;
                        item.iID_TieuMucID = null;
                        item.iID_TietMucID = itemMucLucNs.iID_MaMucLucNganSach;
                        item.iID_TietMucID = null;
                        item.iID_NganhID = null;
                    }
                    else if (!string.IsNullOrEmpty(item.TM) && !string.IsNullOrEmpty(item.TTM) && string.IsNullOrEmpty(item.NG))
                    {
                        item.iID_MucID = null;
                        item.iID_TieuMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_TietMucID = itemMucLucNs.iID_MaMucLucNganSach;
                        item.iID_NganhID = null;
                    }
                    else if (!string.IsNullOrEmpty(item.TM) && !string.IsNullOrEmpty(item.TTM) && !string.IsNullOrEmpty(item.NG))
                    {
                        item.iID_MucID = null;
                        item.iID_TieuMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_TietMucID = null;
                        item.iID_NganhID = itemMucLucNs.iID_MaMucLucNganSach;
                    }
                }

                return item;
            }).ToList();

            List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> lstUpdate = data.Where(x => (x.iID_KeHoachVonNam_DuocDuyet_ChiTietID != null && x.iID_KeHoachVonNam_DuocDuyet_ChiTietID != Guid.Empty)).ToList();
            List<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet> lstAdd = data.Where(x => (x.iID_KeHoachVonNam_DuocDuyet_ChiTietID == null || x.iID_KeHoachVonNam_DuocDuyet_ChiTietID == Guid.Empty)).ToList();

            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                if (lstAdd.Count() > 0)
                {
                    foreach (var item in lstAdd)
                    {
                        item.iID_KeHoachVonNam_DuocDuyet_ChiTietID = Guid.NewGuid();
                        item.iID_KeHoachVonNam_DuocDuyetID = iID_KeHoachVonNamDuocDuyetID;

                        conn.Insert(item, trans);

                        var entityDuAn = conn.Get<VDT_DA_DuAn>(item.iID_DuAnID, trans);
                        if (entityDuAn != null && entityDuAn.sTrangThaiDuAn.Equals("KhoiTao"))
                        {
                            entityDuAn.sTrangThaiDuAn = "THUC_HIEN";
                            conn.Update(entityDuAn, trans);
                        }
                    }
                }

                if (lstUpdate.Count() > 0)
                {
                    foreach (var item in lstUpdate)
                    {
                        var entityDetail = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet>(item.iID_KeHoachVonNam_DuocDuyet_ChiTietID, trans);
                        if (entityDetail != null)
                        {
                            conn.Update(item, trans);
                        }

                        var entityDuAn = conn.Get<VDT_DA_DuAn>(item.iID_DuAnID, trans);
                        if (entityDuAn != null && entityDuAn.sTrangThaiDuAn.Equals("KhoiTao"))
                        {
                            entityDuAn.sTrangThaiDuAn = "THUC_HIEN";
                            conn.Update(entityDuAn, trans);
                        }
                    }
                }
                //Update iD KHVN DX
                var keHoachVNDuocDuyet = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(iID_KeHoachVonNamDuocDuyetID, trans);
                if (keHoachVNDuocDuyet != null)
                {
                    keHoachVNDuocDuyet.iID_KeHoachVonNamDeXuatID = iID_KeHoachVonNamDeXuatID;
                    conn.Update(keHoachVNDuocDuyet, trans);
                }
                //Kết thúc lưu bảng tổng hợp
                trans.Commit();
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                //Lưu bảng tổng hợp
                List<TongHopNguonNSDauTuQuery> lstDataInsert = new List<TongHopNguonNSDauTuQuery>();
                var entityVonNamDX = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(iID_KeHoachVonNamDuocDuyetID, trans);
                if (entityVonNamDX.iID_ParentId != null && entityVonNamDX.iID_ParentId != Guid.Empty)
                {
                    InsertTongHopNguonDauTu_Tang(LOAI_CHUNG_TU.KE_HOACH_VON_NAM, (int)TypeExecute.Adjust, entityVonNamDX.iID_KeHoachVonNam_DuocDuyetID, entityVonNamDX.iID_ParentId);
                }
                else
                {
                    InsertTongHopNguonDauTu_Tang(LOAI_CHUNG_TU.KE_HOACH_VON_NAM, (int)TypeExecute.Adjust, entityVonNamDX.iID_KeHoachVonNam_DuocDuyetID, Guid.NewGuid());
                }

                foreach (var item in
                    data.Where(n => ((n.fGiaTriThuHoiNamTruocKhoBac ?? 0) != 0 || (n.fGiaTriThuHoiNamTruocLenhChi ?? 0) != 0)))
                {
                    if ((item.fGiaTriThuHoiNamTruocKhoBac ?? 0) != 0)
                    {
                        TongHopNguonNSDauTuQuery data1 = GetMucLucNganSachByPhanBoVon(item, dicMucLucNganSach);
                        data1.iID_ChungTu = entityVonNamDX.iID_KeHoachVonNam_DuocDuyetID;
                        data1.fGiaTri = (item.fGiaTriThuHoiNamTruocKhoBac ?? 0);
                        data1.bIsLog = false;
                        data1.iID_DuAnID = item.iID_DuAnID.Value;
                        data1.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                        data1.sMaDich = LOAI_CHUNG_TU.KHVU_KHOBAC;
                        data1.sMaNguonCha = LOAI_CHUNG_TU.KHVN_KHOBAC;
                        data1.bKeHoach = true;
                        lstDataInsert.Add(data1);
                    }
                    if ((item.fGiaTriThuHoiNamTruocLenhChi ?? 0) != 0)
                    {
                        TongHopNguonNSDauTuQuery data2 = GetMucLucNganSachByPhanBoVon(item, dicMucLucNganSach);
                        data2.iID_ChungTu = entityVonNamDX.iID_KeHoachVonNam_DuocDuyetID;
                        data2.fGiaTri = (item.fGiaTriThuHoiNamTruocLenhChi ?? 0);
                        data2.bIsLog = false;
                        data2.iID_DuAnID = item.iID_DuAnID.Value;
                        data2.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                        data2.sMaDich = LOAI_CHUNG_TU.KHVU_LENHCHI;
                        data2.sMaNguonCha = LOAI_CHUNG_TU.KHVN_LENHCHI;
                        data2.bKeHoach = true;
                        lstDataInsert.Add(data2);
                    }
                }
                if (lstDataInsert.Count != 0)
                {
                    InsertTongHopNguonDauTu_KHVN_Giam(entityVonNamDX.iNamKeHoach ?? 0, LOAI_CHUNG_TU.KE_HOACH_VON_NAM, (int)TypeExecute.Update, entityVonNamDX.iID_KeHoachVonNam_DuocDuyetID, lstDataInsert);
                }
                trans.Commit();
            }

            return true;
        }

        public IEnumerable<VDT_DA_GoiThau> getListGoiThauKHLCNhaThau(Guid iID_GoiThauID)
        {
            var sql = "select * from VDT_DA_GoiThau where iID_GoiThauID = @iID_GoiThauID";
            using (var conn = _connectionFactory.GetConnection())
            {
                IEnumerable<VDT_DA_GoiThau> listGoiThau = conn.Query<VDT_DA_GoiThau>(sql, param: new { iID_GoiThauID }, commandType: CommandType.Text);
                return listGoiThau;
            }
        }

        /// <summary>
        /// update fCapPhatTaiKhoBac, fCapPhatBangLenhChi cua KHV nam duoc duyet
        /// </summary>
        /// <param name="iID_KeHoachVonNam_DuocDuyetID"></param>
        /// <returns></returns>
        public bool UpdateGiaTriCapPhatKHVnamDuocDuyet(Guid iID_KeHoachVonNam_DuocDuyetID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_update_giatricapphat_khvonnamduocduyet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        @iID_KeHoachVonNam_DuocDuyetID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public IEnumerable<NS_MucLucNganSach> GetAllMucLucNganSachByNamLamViec(int yearOfWork)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM NS_MucLucNganSach WHERE iNamLamViec = {0}", yearOfWork);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(query.ToString());
                return items;
            }
        }

        public bool CheckExistXauNoiMa(int iNamLamViec, string sXauNoiMa)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT COUNT(iID_MaMucLucNganSach) FROM NS_MucLucNganSach WHERE iNamLamViec = {0} AND sXauNoiMa = '{1}'", iNamLamViec, sXauNoiMa);
            using (var conn = _connectionFactory.GetConnection())
            {
                int count = conn.Query<int>(query.ToString()).FirstOrDefault();
                return count > 0;
            }
        }

        public bool CheckExistSoKeHoachVonNamDuocDuyet(string sSoQuyetDinh, int iNamKeHoach, Guid? idPhanBoVon)
        {
            StringBuilder query = new StringBuilder();
            if (idPhanBoVon != null && idPhanBoVon != Guid.Empty)
            {
                query.AppendFormat("SELECT COUNT(iID_KeHoachVonNam_DuocDuyetID) FROM VDT_KHV_KeHoachVonNam_DuocDuyet WHERE sSoQuyetDinh = '{0}' AND iID_KeHoachVonNam_DuocDuyetID <> '{1}' AND iNamKeHoach = {2} AND iLoai = 1",
                    sSoQuyetDinh, idPhanBoVon, iNamKeHoach);
            }
            else
                query.AppendFormat("SELECT COUNT(iID_KeHoachVonNam_DuocDuyetID) FROM VDT_KHV_KeHoachVonNam_DuocDuyet WHERE sSoQuyetDinh = '{0}' AND iNamKeHoach = {1} AND iLoai = 1",
                    sSoQuyetDinh, iNamKeHoach);

            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirstOrDefault<int>(query.ToString(), commandType: CommandType.Text);
                return count > 0;
            }
        }

        public bool CheckExistSoKeHoachVonNamPhanBoVonDVPheDuyet(string sSoQuyetDinh, int iNamKeHoach, Guid? idPhanBoVon)
        {
            StringBuilder query = new StringBuilder();
            if (idPhanBoVon != null && idPhanBoVon != Guid.Empty)
            {
                query.AppendFormat("SELECT COUNT(Id) FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet WHERE sSoQuyetDinh = '{0}' AND Id <> '{1}' AND iNamKeHoach = {2} AND iLoai = 1",
                    sSoQuyetDinh, idPhanBoVon, iNamKeHoach);
            }
            else
                query.AppendFormat("SELECT COUNT(Id) FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet WHERE sSoQuyetDinh = '{0}' AND iNamKeHoach = {1} AND iLoai = 1",
                    sSoQuyetDinh, iNamKeHoach);

            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirstOrDefault<int>(query.ToString(), commandType: CommandType.Text);
                return count > 0;
            }
        }

        public bool CheckExistKeHoachVonNamDuocDuyet(string iIdDonViQuanLy, int iIdNguonVon, int iNamKeHoach, Guid iId)
        {
            var sql =
                @"SELECT *
                FROM VDT_KHV_KeHoachVonNam_DuocDuyet 
                WHERE iID_DonViQuanLyID = @iIdDonViQuanLy AND iID_NguonVonID = @iIdNguonVon
	                AND iNamKeHoach = @iNamKeHoach AND iID_KeHoachVonNam_DuocDuyetID <> @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DuocDuyet>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iIdDonViQuanLy,
                         iIdNguonVon,
                         iNamKeHoach,
                         iId
                     },
                     commandType: CommandType.Text
                 ));

                return items.Count() != 0;
            }
        }
        public bool CheckExistKeHoachVonNamPhanBoVonDVPheDuyet(string iIdDonViQuanLy, int iIdNguonVon, int iNamKeHoach, Guid Id)
        {
            var sql =
                @"SELECT *
                FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet 
                WHERE iID_DonViQuanLyID = @iIdDonViQuanLy AND iID_NguonVonID = @iIdNguonVon
	                AND iNamKeHoach = @iNamKeHoach AND Id <> @Id";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iIdDonViQuanLy,
                         iIdNguonVon,
                         iNamKeHoach,
                         Id
                     },
                     commandType: CommandType.Text
                 ));

                return items.Count() != 0;
            }
        }

        public bool SaveKeHoachVonNamDuocDuyet(ref Guid iID, VDT_KHV_KeHoachVonNam_DuocDuyet data, string sUserName, bool isModified = false)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_KeHoachVonNam_DuocDuyetID == null || data.iID_KeHoachVonNam_DuocDuyetID == Guid.Empty)
                {
                    var entity = new VDT_KHV_KeHoachVonNam_DuocDuyet();
                    entity.MapFrom(data);
                    var donvi = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                    if (donvi == null)
                        return false;
                    entity.iID_MaDonViQuanLy = donvi.iID_MaDonVi;
                    entity.iID_KeHoachVonNam_DuocDuyetID = Guid.NewGuid();
                    entity.bActive = true;
                    entity.sUserCreate = sUserName;
                    entity.dDateCreate = DateTime.Now;
                    entity.bActive = true;
                    entity.bIsGoc = true;
                    entity.iNamKeHoach = data.iNamKeHoach;
                    entity.iID_NguonVonID = data.iID_NguonVonID;
                    entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                    entity.iID_KeHoachVonNamGocID = entity.iID_KeHoachVonNam_DuocDuyetID;
                    entity.iID_ParentId = null;
                    iID = entity.iID_KeHoachVonNam_DuocDuyetID;
                    conn.Insert(entity, trans);
                }
                else
                {
                    if (isModified)
                    {
                        var entity = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(data.iID_KeHoachVonNam_DuocDuyetID, trans);

                        var entityModified = new VDT_KHV_KeHoachVonNam_DuocDuyet();
                        entityModified.iID_KeHoachVonNam_DuocDuyetID = Guid.NewGuid();
                        entityModified.sSoQuyetDinh = data.sSoQuyetDinh;
                        entityModified.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entityModified.iNamKeHoach = entity.iNamKeHoach;
                        entityModified.iID_DonViQuanLyID = entity.iID_DonViQuanLyID;
                        entityModified.iID_ParentId = entity.iID_KeHoachVonNam_DuocDuyetID;
                        entityModified.bActive = true;
                        entityModified.bIsGoc = false;
                        entityModified.sUserCreate = sUserName;
                        entityModified.dDateCreate = DateTime.Now;
                        entityModified.iID_NguonVonID = entity.iID_NguonVonID;
                        entityModified.iID_MaDonViQuanLy = entity.iID_MaDonViQuanLy;
                        entityModified.iID_KeHoachVonNamGocID = entity.iID_KeHoachVonNamGocID;
                        iID = entityModified.iID_KeHoachVonNam_DuocDuyetID;
                        conn.Insert(entityModified, trans);

                        var entityUpdate = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(data.iID_KeHoachVonNam_DuocDuyetID, trans);
                        if (entityUpdate != null)
                        {
                            entityUpdate.bActive = false;

                            conn.Update(entityUpdate, trans);
                        }

                        DynamicParameters lstParam = new DynamicParameters();
                        var iID_KePhanBoVonDonViF = data.iID_KeHoachVonNam_DuocDuyetID.ToString();
                        var iID_KePhanBoVonDonViL = entityModified.iID_KeHoachVonNam_DuocDuyetID.ToString();

                        var sql = FileHelpers.GetSqlQuery("proc_vdt_create_phan_bo_von_duoc_duyet_chi_tiet.sql");

                        conn.Execute(
                                sql,
                                param: new
                                {
                                    iID_KePhanBoVonDonViL,
                                    iID_KePhanBoVonDonViF,
                                },
                                transaction: trans,
                                commandType: CommandType.Text);
                    }
                    else
                    {
                        iID = data.iID_KeHoachVonNam_DuocDuyetID;
                        var entity = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(data.iID_KeHoachVonNam_DuocDuyetID, trans);
                        if (entity == null)
                            return false;

                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.sUserUpdate = sUserName;
                        entity.dDateUpdate = DateTime.Now;

                        conn.Update(entity, trans);
                    }
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }
        public bool SaveKeHoachVonNamPhanBoVonDVPheDuyet(ref Guid iID, VDT_KHV_PhanBoVon_DonVi_PheDuyet data, string sUserName, bool isModified = false)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.Id == null || data.Id == Guid.Empty)
                {
                    var entity = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
                    entity.MapFrom(data);
                    var donvi = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                    if (donvi == null)
                        return false;
                    entity.iID_MaDonViQuanLy = donvi.iID_MaDonVi;
                    entity.Id = Guid.NewGuid();
                    entity.bActive = true;
                    entity.sUserCreate = sUserName;
                    entity.dDateCreate = DateTime.Now;
                    entity.bActive = true;
                    entity.bIsGoc = true;
                    entity.iNamKeHoach = data.iNamKeHoach;
                    entity.iID_NguonVonID = data.iID_NguonVonID;
                    entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                    entity.iID_PhanBoGocID = entity.Id;
                    entity.iID_ParentId = null;
                    iID = entity.Id;
                    conn.Insert(entity, trans);
                }
                else
                {
                    if (isModified)
                    {
                        var entity = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(data.Id, trans);

                        var entityModified = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
                        entityModified.Id = Guid.NewGuid();
                        entityModified.sSoQuyetDinh = data.sSoQuyetDinh;
                        entityModified.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entityModified.iNamKeHoach = entity.iNamKeHoach;
                        entityModified.iID_DonViQuanLyID = entity.iID_DonViQuanLyID;
                        entityModified.iID_ParentId = entity.Id;
                        entityModified.bActive = true;
                        entityModified.bIsGoc = false;
                        entityModified.sUserCreate = sUserName;
                        entityModified.dDateCreate = DateTime.Now;
                        entityModified.iID_NguonVonID = entity.iID_NguonVonID;
                        entityModified.iID_MaDonViQuanLy = entity.iID_MaDonViQuanLy;
                        entityModified.iID_PhanBoGocID = entity.iID_PhanBoGocID;
                        iID = entityModified.Id;
                        conn.Insert(entityModified, trans);

                        var entityUpdate = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(data.Id, trans);
                        if (entityUpdate != null)
                        {
                            entityUpdate.bActive = false;

                            conn.Update(entityUpdate, trans);
                        }

                        DynamicParameters lstParam = new DynamicParameters();
                        var iID_KePhanBoVonDonViF = data.Id.ToString();
                        var iID_KePhanBoVonDonViL = entityModified.Id.ToString();

                        var sql = FileHelpers.GetSqlQuery("proc_vdt_create_phan_bo_von_duoc_phe_duyet_chi_tiet.sql");

                        conn.Execute(
                                sql,
                                param: new
                                {
                                    iID_KePhanBoVonDonViL,
                                    iID_KePhanBoVonDonViF
                                },
                                transaction: trans,
                                commandType: CommandType.Text);
                    }
                    else
                    {
                        iID = data.Id;
                        var entity = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(data.Id, trans);
                        ///if (entity == null)
                        //     return false;
                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.sUserUpdate = sUserName;
                        entity.dDateUpdate = DateTime.Now;

                        conn.Update(entity, trans);
                    }
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }
        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetAllKeHoachVonNamDeXuatByIdDonVi(Guid? idDonViQuanLy)
        {
            List<VDT_KHV_KeHoachVonNam_DeXuat> lstKeHoachNamDeXuat = new List<VDT_KHV_KeHoachVonNam_DeXuat>();

            if (idDonViQuanLy.HasValue)
            {
                var sql = @"select * from VDT_KHV_KeHoachVonNam_DeXuat " +
                "where iID_DonViQuanLyID = @idDonViQuanLy ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat>(sql,
                        param: new
                        {
                            idDonViQuanLy
                        },
                        commandType: CommandType.Text);

                    lstKeHoachNamDeXuat = item.ToList();
                }
            }

            return lstKeHoachNamDeXuat;
        }

        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetKeHoachVonNamDeXuatTongHopByCondition(int iNamLamViec, Guid? iIdDonViQuanLy, int? iID_NguonVonID)
        {
            var sql =
                @"SELECT 
                    dx.iID_KeHoachVonNamDeXuatID,
	                CONCAT(dx.sSoQuyetDinh, '-', dx.iNamKeHoach) as sSoQuyetDinh
                FROM
                    VDT_KHV_KeHoachVonNam_DeXuat dx
                WHERE
                    dx.sTongHop is not null
                    AND dx.iID_DonViQuanLyID = @iIdDonViQuanLy
                    AND dx.iNamKeHoach = @iNamLamViec
					AND dx.iID_NguonVonID = @iID_NguonVonID  ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iIdDonViQuanLy,
                         iNamLamViec,
                         iID_NguonVonID
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public VDT_KHV_KeHoachVonNam_DuocDuyet GetKeHoachVonNamDuocDuyetById(Guid? idPhanBoVon)
        {
            VDT_KHV_KeHoachVonNam_DuocDuyet entity = new VDT_KHV_KeHoachVonNam_DuocDuyet();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_KeHoachVonNam_DuocDuyet>(idPhanBoVon, trans);
                // commit to db
                trans.Commit();
            }

            if (entity == null)
            {
                return new VDT_KHV_KeHoachVonNam_DuocDuyet();
            }

            return entity;
        }

        public VDT_KHV_PhanBoVon_DonVi_PheDuyet GetKeHoachVonPBVDonViPheDuyetById(Guid? idPhanBoVon)
        {
            VDT_KHV_PhanBoVon_DonVi_PheDuyet entity = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(idPhanBoVon, trans);
                // commit to db
                trans.Commit();
            }

            if (entity == null)
            {
                return new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            }

            return entity;
        }
        public VDT_KHV_PhanBoVon_DonVi_PheDuyet GetKeHoachVonNamPhanBoVonDonViPheDuyetById(Guid? idPhanBoVon)
        {
            VDT_KHV_PhanBoVon_DonVi_PheDuyet entity = new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_PhanBoVon_DonVi_PheDuyet>(idPhanBoVon, trans);
                // commit to db
                trans.Commit();
            }

            if (entity == null)
            {
                return new VDT_KHV_PhanBoVon_DonVi_PheDuyet();
            }

            return entity;
        }


        public IEnumerable<VDTKHVPhanBoVonDuocDuyetViewModel> GetAllKeHoachVonNamDuocDuyet(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iID_NguonVonID = null, Guid? iID_DonViQuanLyID = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iID_NguonVonID", iID_NguonVonID);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTKHVPhanBoVonDuocDuyetViewModel>("proc_get_all_kehoachvonnamduocduyet_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<VDTKHVPhanBoVonDonViPheDuyetViewModel> GetAllKeHoachVonNamDonViPheDuyet(ref PagingInfo _paging, string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null, int? iNamKeHoach = null, int? iID_NguonVonID = null, Guid? iID_DonViQuanLyID = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iID_NguonVonID", iID_NguonVonID);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<VDTKHVPhanBoVonDonViPheDuyetViewModel>("proc_get_all_kehoachvonnamphanbovondonvipheduyet_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat> GetListChungTuTH(Guid iID_DonViQL, int iNamKeHoach)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_DonViQuanLyID = '{0}' AND iNamKeHoach = {1} AND sTongHop IS NOT NULL ", iID_DonViQL, iNamKeHoach);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoachVonNam_DeXuat>(query.ToString());
                return items;
            }
        }
        #endregion

        #region Kế hoạch trung hạn được duyệt - NinhNV
        public IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetLstChungTuDeXuat(Guid? iID_DonViQuanLyID, int iGiaiDoanTu, int iGiaiDoanDen, int iNamLamViec, bool isModified, bool isCt)
        {
            // Chung tu bao cao Mo moi
            var sql =
                @"SELECT iID_KeHoach5Nam_DeXuatID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam_DeXuat tblGoc
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND sTongHop is not null
                AND iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanden
                AND (SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat dx WHERE tblGoc.sTongHop LIKE CONCAT('%', dx.iID_KeHoach5Nam_DeXuatID, '%') AND dx.bIsGoc = 0) = 0
                ORDER BY sSoQuyetDinh";
            //AND bIsGoc = 1

            // Chung tu bao cao mo moi Dieu Chinh
            if (isModified)
            {
                /*sql =
                 @"SELECT iID_KeHoach5Nam_DeXuatID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam_DeXuat tblGoc
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND sTongHop is null
                AND iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanden
                AND bIsGoc = 1
                AND (SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat dx WHERE tblGoc.sTongHop LIKE CONCAT('%', dx.iID_KeHoach5Nam_DeXuatID, '%') AND dx.bIsGoc = 0) > 0
                ORDER BY sSoQuyetDinh";*/
                sql =
                 @"SELECT iID_KeHoach5Nam_DeXuatID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam_DeXuat tblGoc
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID --AND sTongHop is null
                AND iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanden
                AND bIsGoc = 1
                AND (SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat dx WHERE tblGoc.sTongHop LIKE CONCAT('%', dx.iID_KeHoach5Nam_DeXuatID, '%') AND dx.bIsGoc = 0) > 0
                ORDER BY sSoQuyetDinh";
            }

            //Chung tu bao cao Chuyen tiep
            if (!isModified && isCt)
            {
                sql =
                @"SELECT iID_KeHoach5Nam_DeXuatID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam_DeXuat tblGoc
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND sTongHop is not null AND iLoai = 2
                AND iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanden
                ORDER BY sSoQuyetDinh";
                //AND bIsGoc = 1 AND iLoai = 2
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoach5Nam_DeXuat>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iID_DonViQuanLyID,
                         iGiaiDoanTu,
                         iGiaiDoanDen,
                         iNamLamViec
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> LayDanhSachChungTuDeXuatTheoDonViQuanLy(Guid iID_DonViQuanLyID, int iNamLamViec)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_all_chungtudexuat_by_dvql.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoach5Nam_DeXuat>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID,
                       iNamLamViec
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public bool UpdateGiaTriKeHoachDuocDuyet(string iID_KeHoach5NamID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_update_giatrikehoach_kh5namduocduyet.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_KeHoach5NamID
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public bool SaveKeHoach5NamDuocDuyet(ref VDT_KHV_KeHoach5Nam data, string sUserLogin, int iNamLamViec, bool isDieuChinh, List<VDT_KHV_KeHoach5Nam_ChiTiet> lstDetails)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.iID_KeHoach5NamID == null || data.iID_KeHoach5NamID == Guid.Empty)
                {
                    var entity = new VDT_KHV_KeHoach5Nam();
                    entity.MapFrom(data);
                    entity.iNamLamViec = iNamLamViec;
                    entity.bActive = true;
                    entity.bIsGoc = true;
                    entity.iLoai = data.iLoai;
                    entity.sUserCreate = sUserLogin;
                    entity.dDateCreate = DateTime.Now;
                    conn.Insert(entity, trans);

                    data.iID_KeHoach5NamID = entity.iID_KeHoach5NamID;
                }
                else
                {
                    if (isDieuChinh)
                    {
                        var entity = new VDT_KHV_KeHoach5Nam();
                        entity.MapFrom(data);
                        entity.iID_KeHoach5NamID = Guid.NewGuid();
                        entity.iID_ParentId = data.iID_KeHoach5NamID;
                        entity.iNamLamViec = iNamLamViec;
                        entity.bActive = true;
                        entity.bIsGoc = false;
                        entity.sUserCreate = sUserLogin;
                        entity.dDateCreate = DateTime.Now;

                        var entityGoc = conn.Get<VDT_KHV_KeHoach5Nam>(data.iID_KeHoach5NamID, trans);
                        entityGoc.bActive = false;
                        entityGoc.sUserUpdate = sUserLogin;
                        entityGoc.dDateUpdate = DateTime.Now;
                        conn.Update(entityGoc, trans);

                        lstDetails.Select(item =>
                        {
                            item.iID_KeHoach5NamID = entity.iID_KeHoach5NamID;
                            item.iID_ParentID = item.iID_KeHoach5Nam_ChiTietID;
                            item.iID_KeHoach5Nam_ChiTietID = Guid.NewGuid();
                            return item;
                        }).ToList();

                        if (lstDetails != null && lstDetails.Count() > 0)
                        {
                            entity.fGiaTriDuocDuyet = lstDetails.Where(item => item != null && item.fVonBoTriTuNamDenNam != null).Sum(item => item.fVonBoTriTuNamDenNam);
                        }

                        conn.Insert(entity, trans);
                        conn.Insert<VDT_KHV_KeHoach5Nam_ChiTiet>(lstDetails, trans);

                        data.iID_KeHoach5NamID = entity.iID_KeHoach5NamID;
                    }
                    else
                    {
                        var entity = conn.Get<VDT_KHV_KeHoach5Nam>(data.iID_KeHoach5NamID, trans);
                        if (entity == null)
                            return false;
                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        entity.iID_KeHoach5NamDeXuatID = data.iID_KeHoach5NamDeXuatID;
                        entity.sSoQuyetDinh = data.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = data.dNgayQuyetDinh;
                        entity.iGiaiDoanTu = data.iGiaiDoanTu;
                        entity.iGiaiDoanDen = data.iGiaiDoanDen;
                        entity.sMoTaChiTiet = data.sMoTaChiTiet;
                        entity.iLoai = data.iLoai;
                        entity.sUserUpdate = sUserLogin;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);
                    }
                }
                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<VdtKhvKeHoach5NamExportModel> GetDataExportKeHoachTrungHan(string id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("id", id);
                var items = conn.Query<VdtKhvKeHoach5NamExportModel>("sp_vdt_kehoachtrunghan_export", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<KH5NDDPrintDataExportModel> FindByReportKeHoachTrungHan(string id, string lct, int idNguonVon, int type, double donViTinh, string lstDonViThucHienDuAn)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("Id", id);
                lstParam.Add("lct", lct);
                lstParam.Add("IdNguonVon", idNguonVon);
                lstParam.Add("type", type);
                lstParam.Add("MenhGiaTienTe", donViTinh);
                lstParam.Add("lstDonViThucHienDuAn", lstDonViThucHienDuAn);

                var items = conn.Query<KH5NDDPrintDataExportModel>("sp_vdt_khv_kehoach_5_nam_duoc_duyet_export", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<KH5NDDPrintDataChuyenTiepExportModel> FindByReportKeHoachTrungHanChuyenTiep(string lstId, string lstBudget, string lstUnit, int type, double donViTinh)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("lstId", lstId);
                lstParam.Add("lstBudget", lstBudget);
                lstParam.Add("lstUnit", lstUnit);
                lstParam.Add("type", type);
                lstParam.Add("DonViTienTe", donViTinh);

                var items = conn.Query<KH5NDDPrintDataChuyenTiepExportModel>("sp_vdt_kehoach5nam_chitiet_chuyentiep_report", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<KeHoach5NamDuocDuyetModel> GetAllKeHoach5NamDuocDuyet(ref PagingInfo _paging, int iNamLamViec, string sSoQuyetDinh, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? iID_DonViQuanLyID, string sMoTaChiTiet, int? iGiaiDoanTu, int? iGiaiDoanDen, int iLoai)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                lstParam.Add("iNamLamViec", iNamLamViec);
                lstParam.Add("iID_DonViQuanLyID", iID_DonViQuanLyID);
                lstParam.Add("sMoTaChiTiet", sMoTaChiTiet);
                lstParam.Add("iGiaiDoanTu", iGiaiDoanTu);
                lstParam.Add("iGiaiDoanDen", iGiaiDoanDen);
                lstParam.Add("iLoai", iLoai);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<KeHoach5NamDuocDuyetModel>("proc_get_all_kehoach5namduocduyet_paging", lstParam, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public VDT_KHV_KeHoach5Nam GetKeHoach5NamDuocDuyetById(Guid? Id)
        {
            VDT_KHV_KeHoach5Nam entity = new VDT_KHV_KeHoach5Nam();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_KHV_KeHoach5Nam>(Id, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public bool deleteKeHoach5NamDuocDuyet(Guid Id)
        {
            var sql = @"update VDT_KHV_KeHoach5Nam set bActive = 1 where iID_KeHoach5NamID = (Select iID_ParentId from VDT_KHV_KeHoach5Nam where iID_KeHoach5NamID = @Id); " +
                " DELETE VDT_KHV_KeHoach5Nam_ChiTiet WHERE iID_KeHoach5NamID = @Id; " +
                " DELETE VDT_KHV_KeHoach5Nam WHERE iID_KeHoach5NamID = @Id; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        Id
                    },
                    commandType: CommandType.Text);

                return r >= 0;
            }
        }

        public IEnumerable<string> GetLstIdChungTuDuocDuyet(int iGiaiDoanTu, int iGiaiDoanDen, bool isCt, int iNamLamViec)
        {
            var sql =
                @"SELECT convert(nvarchar(50), iID_KeHoach5NamID)
                FROM VDT_KHV_KeHoach5Nam
                WHERE iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND iNamLamViec = @iNamLamViec
                AND iLoai = 1
                ";

            if (isCt)
            {
                sql =
                @"SELECT convert(nvarchar(50), iID_KeHoach5NamID)
                FROM VDT_KHV_KeHoach5Nam
                WHERE iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND iNamLamViec = @iNamLamViec
                AND iLoai = 2
                ";
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<string>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iGiaiDoanTu,
                         iGiaiDoanDen,
                         iNamLamViec
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam> GetLstChungTuDuocDuyet(Guid? iID_DonViQuanLyID, int iNamLamViec, bool isCt)
        {
            var sql =
                @"SELECT iID_KeHoach5NamID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam 
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND bIsGoc = 1
                ORDER BY sSoQuyetDinh";
            if (isCt)
            {
                sql =
                @"SELECT iID_KeHoach5NamID, CONCAT(sSoQuyetDinh, ' (', iGiaiDoanTu, ' - ', iGiaiDoanDen, ')') as sSoQuyetDinh
                FROM VDT_KHV_KeHoach5Nam 
                WHERE iNamLamViec = @iNamLamViec AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND bIsGoc = 1 AND iLoai = 2
                ORDER BY sSoQuyetDinh";
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_KHV_KeHoach5Nam>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         iID_DonViQuanLyID,
                         iNamLamViec,
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }

        public KeHoach5NamDuocDuyetModel GetKeHoach5NamDuocDuyetByIdForDetail(Guid iID_KeHoach5NamID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_detail_kh5nam_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirstOrDefault<KeHoach5NamDuocDuyetModel>(sql,
                   param: new
                   {
                       iID_KeHoach5NamID
                   },
                   commandType: System.Data.CommandType.Text);
                return items;
            }
        }

        public bool CheckExistSoKeHoachKH5NDD(string sSoQuyetDinh, int iNamLamViec, Guid? iID_KeHoach5NamID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_exist_sokehoach_kh5namdd.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var iCountId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        sSoQuyetDinh,
                        iID_KeHoach5NamID,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return iCountId == 0 ? false : true;
            }
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam_ChiTiet> GetListKH5NamByIdKHDD(Guid iID_KeHoach5NamID)
        {
            var sql = "SELECT * FROM VDT_KHV_KeHoach5Nam_ChiTiet khthct where khthct.iID_KeHoach5NamID = @iID_KeHoach5NamID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_KHV_KeHoach5Nam_ChiTiet>(sql,
                    param: new
                    {
                        iID_KeHoach5NamID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        // result in DataTable
        public DataTable GetListKH5NamDuocDuyetChiTietById(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_kh5namduocduyet_chitiet_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iID_KeHoach5NamID", iID_KeHoach5NamID);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }

        // result in List
        public IEnumerable<VDT_KHV_KeHoach5Nam_ChiTiet> GetListKH5NamDuocDuyetChiTietByIdInList(string iID_KeHoach5NamID, int iNamLamViec, Dictionary<string, string> _filters)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                _filters.ToList().ForEach(x =>
                {
                    lstParam.Add($"{x.Key}", x.Value);
                });
                lstParam.Add("@iID_KeHoach5NamID", iID_KeHoach5NamID);
                lstParam.Add("@iNamLamViec", iNamLamViec);
                var items = conn.Query<VDT_KHV_KeHoach5Nam_ChiTiet>("proc_get_list_kehoach5nam_chitiet_by_id", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public DataTable GetListDuAnKH5NamDeXuatChiTietById(string Id, int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_duan_kh5namdexuat_chitiet_by_id.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    _filters.ToList().ForEach(x =>
                    {
                        cmd.Parameters.AddWithValue($"{x.Key}", string.IsNullOrWhiteSpace(x.Value) ? x.Value.ToParamString() : $"%{x.Value}%");
                    });
                    cmd.Parameters.AddWithValue("@iId", Id);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    return cmd.GetTable();
                }
            }
        }

        public VDT_DA_DuAn GetVDTDuAnByIdKH5NDXCT(string iID_DuAnKHTHDeXuatID)
        {
            var sql = @"select * from VDT_DA_DuAn " +
                "where iID_DuAnKHTHDeXuatID = @iID_DuAnKHTHDeXuatID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DA_DuAn>(sql,
                    param: new
                    {
                        iID_DuAnKHTHDeXuatID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public int GetMaxMaDuAnIndex()
        {
            var sql = @"select ISNULL(max(iMaDuAnIndex), 0) from VDT_DA_DuAn ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<int>(sql,
                    commandType: CommandType.Text);

                return item;
            }
        }

        public int GetMaxIndexMaHangMuc()
        {
            var sql = @"select ISNULL(max(indexMaHangMuc), 0) from VDT_DA_DuAn_HangMuc ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<int>(sql,
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_DA_DuAn_HangMuc GetVDTDuAnHangMuc(Guid iID_DuAnID, Guid iID_LoaiCongTrinhID, int iID_NguonVonID)
        {
            var sql = @"select * from VDT_DA_DuAn_HangMuc " +
                "where iID_DuAnID = @iID_DuAnID AND iID_LoaiCongTrinhID = @iID_LoaiCongTrinhID AND iID_NguonVonID = @iID_NguonVonID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DA_DuAn_HangMuc>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_LoaiCongTrinhID,
                        iID_NguonVonID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public VDT_DA_DuAn_NguonVon GetVDTDuAnNguonVon(Guid iID_DuAnID, int iID_NguonVonID)
        {
            var sql = @"select * from VDT_DA_DuAn_NguonVon " +
                "where iID_DuAn = @iID_DuAnID AND iID_NguonVonID = @iID_NguonVonID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<VDT_DA_DuAn_NguonVon>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_NguonVonID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public DataTable GetListDMDuAnKH5NDD(int iNamLamViec, Dictionary<string, string> _filters)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_dm_duan_kh5namduocduyet.sql");

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

        public DM_ChuDauTu GetChuDauTuByMaCDT(int iNamLamViec, string sId_CDT)
        {
            var sql = @"
            select  *
            from    DM_ChuDauTu
            where   iTrangThai = 1
                    and sId_CDT = @sId_CDT
                    and iNamLamViec = @iNamLamViec
            ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<DM_ChuDauTu>(sql,
                    new
                    {
                        iNamLamViec,
                        sId_CDT
                    },
                    commandType: CommandType.Text);

                return entity;
            }
        }

        public VDT_DA_DuAn GetDuAnByIdDuAn(Guid? iID_DuAnID)
        {
            VDT_DA_DuAn entity = new VDT_DA_DuAn();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_DA_DuAn>(iID_DuAnID, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public VDT_DA_DuAn_HangMuc GetVDTDuAnHangMucById(Guid iID_DuAn_HangMucID)
        {
            VDT_DA_DuAn_HangMuc entity = new VDT_DA_DuAn_HangMuc();
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                entity = conn.Get<VDT_DA_DuAn_HangMuc>(iID_DuAn_HangMucID, trans);
                // commit to db
                trans.Commit();
            }
            return entity;
        }

        public double GetTongHanMucDauTuNguonVon(Guid iID_DuAnID, int iID_NguonVonID)
        {
            var sql = @" SELECT SUM(ISNULL(fHanMucDauTu, 0)) FROM VDT_DA_DuAn_HangMuc
                         where iID_DuAnID = @iID_DuAnID
                         AND iID_NguonVonID = @iID_NguonVonID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirst<double>(sql,
                    param: new
                    {
                        iID_DuAnID,
                        iID_NguonVonID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public void updateHanMucDauTuDuAn(Guid iID_DuAnID)
        {
            var sql = @"UPDATE VDT_DA_DuAn set fHanMucDauTu = (
		                        SELECT SUM(ISNULL(fHanMucDauTu, 0)) 
		                        FROM VDT_DA_DuAn_HangMuc
		                        where iID_DuAnID = @iID_DuAnID
	                        )
                        WHERE iID_DuAnID = @iID_DuAnID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

            }
        }
        #endregion

        #region KH lựa chọn nhà thầu
        /// <summary>
        /// get list du toan by du an id
        /// </summary>
        /// <param name="iIdDuAn"></param>
        /// <returns></returns>
        public List<VDTDADuToanModel> GetDuToanByDuAnId(Guid iIdDuAn, int loaiChungTu)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("@iID_DuAnID", iIdDuAn);
                lstParam.Add("@loaiChungTu", loaiChungTu);
                var items = conn.Query<VDTDADuToanModel>("sp_vdt_get_dutoan_by_duanid", lstParam, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        public List<VDTDADuToanModel> GetDuToanDieuChinhByDuAnId(Guid iIdDuAn)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("@iID_DuAnID", iIdDuAn);
                var items = conn.Query<VDTDADuToanModel>("sp_vdt_get_dutoan_dieuchinh_by_duanid", lstParam, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        /// <summary>
        /// get danh sach ke hoach lua chon nha thau theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoQuyetDinh"></param>
        /// <param name="sTenDuAn"></param>
        /// <param name="dNgayQuyetDinhFrom"></param>
        /// <param name="dNgayQuyetDinhTo"></param>
        /// <param name="sDonViQuanLy"></param>
        /// <returns></returns>
        public IEnumerable<VDTKHLuaChonNhaThauViewModel> GetAllKHLuaChonNhaThauPaging(ref PagingInfo _paging, string sSoQuyetDinh, string sTenDuAn, DateTime? dNgayQuyetDinhFrom, DateTime? dNgayQuyetDinhTo, Guid? sDonViQuanLy)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                    lstParam.Add("sTenDuAn", sTenDuAn);
                    lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                    lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                    lstParam.Add("sDonViQuanLy", sDonViQuanLy, dbType: DbType.Guid);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<VDTKHLuaChonNhaThauViewModel>("proc_get_khluachonnhathau_paging", lstParam, commandType: CommandType.StoredProcedure);
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
        /// delete ke hoach lua chon nha thau
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteKHLuaChonNhaThau(Guid id)
        {
            var sql = "DELETE VDT_QDDT_KHLCNhaThau WHERE Id = @id";
            using (var conn = _connectionFactory.GetConnection())
            {
                var objLuaChonNhaThau = conn.Get<VDT_QDDT_KHLCNhaThau>(id);
                //nếu xóa bản ghi điều chỉnh thì update bản ghi cha bactive = 1
                if (objLuaChonNhaThau != null && objLuaChonNhaThau.iID_ParentID != null)
                {
                    var objParent = conn.Get<VDT_QDDT_KHLCNhaThau>(objLuaChonNhaThau.iID_ParentID);
                    if (objParent != null)
                    {
                        objParent.bActive = true;
                        conn.Update(objParent);
                    }
                }
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        id
                    },
                    commandType: CommandType.Text);

                if (r > 0)
                {

                    return DeleteChildGoiThau(id);
                }

                return false;
            }
        }

        /// <summary>
        /// delete child ke hoach lua chon nha thau: goi thau, goi thau nguon von, goi thau chi phi, goi thau hang muc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteChildGoiThau(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_delete_child_goithau.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    sql,
                    param: new
                    {
                        id
                    },
                    commandType: CommandType.Text);

                return r > 0;
            }
        }

        /// <summary>
        /// get detail ke hoach lua chon nha thau
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VDTKHLuaChonNhaThauViewModel GetDetailKHLuaChonNhaThau(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_detail_khluachonnhathau_by_id.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.QueryFirst<VDTKHLuaChonNhaThauViewModel>(sql, param: new
                {
                    id
                },
                commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// get list goi thau by kh lua chon nha thau
        /// </summary>
        /// <param name="iID"></param>
        /// <returns></returns>
        public List<VDT_DA_GoiThau> GetListGoiThauByKHLuaChonNhaThauID(Guid? iID)
        {
            var sql = "SELECT * FROM VDT_DA_GoiThau WHERE iID_KHLCNhaThau = @iID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_GoiThau>(sql,
                   param: new
                   {
                       iID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }

        /// <summary>
        /// get list goi thau nguon von by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        public List<VDT_DA_GoiThau_NguonVon> GetListGoiThauNguonVonByGoiThauID(Guid iID_GoiThauID)
        {
            var sql = "SELECT * FROM VDT_DA_GoiThau_NguonVon WHERE iID_GoiThauID = @iID_GoiThauID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_GoiThau_NguonVon>(sql,
                   param: new
                   {
                       iID_GoiThauID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }

        /// <summary>
        /// get list goi thau chi phi by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        public List<VDT_DA_GoiThau_ChiPhi> GetListGoiThauChiPhiByGoiThauID(Guid iID_GoiThauID)
        {
            var sql = "SELECT * FROM VDT_DA_GoiThau_ChiPhi WHERE iID_GoiThauID = @iID_GoiThauID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_GoiThau_ChiPhi>(sql,
                   param: new
                   {
                       iID_GoiThauID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }

        // get list du an chi phi co iID_DuAn_ChiPhi = VDT_DA_GoiThau_ChiPhi.iID_ChiPhiID
        public List<VDT_DM_DuAn_ChiPhi> GetListDuAnChiPhis(Guid? iId_ChiPhiID)
        {
            var sql = "select * from VDT_DM_DuAn_ChiPhi where iID_DuAn_ChiPhi = @iId_ChiPhiID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DM_DuAn_ChiPhi>(sql,
                    param: new
                    {
                        iId_ChiPhiID
                    },
                    commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }

        /// <summary>
        /// get list goi thau hang muc by goi thau id
        /// </summary>
        /// <param name="iID_GoiThauID"></param>
        /// <returns></returns>
        public List<VDT_DA_GoiThau_HangMuc> GetListGoiThauHangMucByGoiThauID(Guid iID_GoiThauID)
        {
            var sql = "SELECT * FROM VDT_DA_GoiThau_HangMuc WHERE iID_GoiThauID = @iID_GoiThauID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_GoiThau_HangMuc>(sql,
                   param: new
                   {
                       iID_GoiThauID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }

        public IEnumerable<VDT_DA_DuAn> LayDuAnByIdMaDonViQuanLY(Guid iID_DonViQuanLyID)
        {
            var sql = "select * from VDT_DA_DuAn da " +
                                  " where da.iID_DuAnID not in (select ctdt.iID_DuAnID from VDT_DA_ChuTruongDauTu ctdt " +
                                  " inner join VDT_DA_DuAn da on da.iID_DuAnID = ctdt.iID_DuAnID) and da.iID_DonViQuanLyID = @iID_DonViQuanLyID";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_DA_DuAn>(sql,
                   param: new
                   {
                       iID_DonViQuanLyID
                   },
                   commandType: System.Data.CommandType.Text);
                return items.ToList();
            }
        }


        public IEnumerable<VDT_DA_DuAn> LayDuAnTaoMoiKHLCNT(string iIdMaDonViQuanLy, int iLoaiChungTu)
        {
            var sql = FileHelpers.GetSqlQuery("dm_get_all_duan_by_donvi_khluachonnhathau.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDT_DA_DuAn>(sql,
                    param: new
                    {
                        iIdMaDonViQuanLy,
                        iLoaiChungTu
                    },
                    commandType: System.Data.CommandType.Text);
            }
        }

        public IEnumerable<VDTKHLCNhaThauChungTuViewModel> GetChungTuByDuAnAndLoaiChungTu(Guid iIdDuAnId, int iLoaiChungTu, string iID_KHLCNT)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_khlcnt_get_chungtu_by_duan.sql");
            Guid idKHLCNT = iID_KHLCNT == "" ? Guid.Empty : Guid.Parse(iID_KHLCNT);

            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.Query<VDTKHLCNhaThauChungTuViewModel>(sql,
                    param: new
                    {
                        iIdDuAnId,
                        iLoaiChungTu,
                        idKHLCNT
                    },
                    commandType: System.Data.CommandType.Text);
            }
        }

        public List<VDTKHLCNTDetailViewModel> GetChungTuDetailByListChungTuId(List<Guid> lstChungTuId, int iLoaiChungTu)
        {
            var lstId = ConvertDataToTableTypeDefined("t_tbl_Ids", "iId", lstChungTuId);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKHLCNTDetailViewModel>("sp_vdt_khlcnt_get_goithau_detail_by_chungtu_2",
                    param: new
                    {
                        iLoaiChungTu,
                        lstId = lstId.AsTableValuedParameter("t_tbl_Ids")
                    },
                    commandType: System.Data.CommandType.StoredProcedure);
                if (items == null)
                    return new List<VDTKHLCNTDetailViewModel>();
                return items.ToList();
            }
        }

        public List<VDTKHLCNTDetailViewModel> GetChungTuDetailByKHLCNTId(Guid iId)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDTKHLCNTDetailViewModel>("sp_vdt_khlcnt_get_goithau_detail_by_khlcnt",
                    param: new
                    {
                        iId
                    },
                    commandType: System.Data.CommandType.StoredProcedure);
                if (items == null)
                    return new List<VDTKHLCNTDetailViewModel>();
                return items.ToList();
            }
        }

        public bool CheckExitsSoQuyetDinhKHLCNT(string sSoQuyetDinh, Guid? id)
        {
            var sql = "select count(*) from VDT_QDDT_KHLCNhaThau where (@id is null or Id != @id) and sSoQuyetDinh = @sSoQuyetDinh ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var count = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        sSoQuyetDinh,
                        id
                    },
                    commandType: CommandType.Text);
                return count == 0 ? false : true;
            }
        }

        #endregion

        #region Cap phat thanh toan
        /// <summary>
        /// lay danh sach du an theo chu dau tu
        /// </summary>
        /// <param name="iID_ChuDauTuID"></param>
        /// <returns></returns>
        public List<VDT_DA_DuAn> LayDanhSachDuAnTheoChuDauTu(string sId_CDT, string sId_DonViQuanLy)
        {
            try
            {
                var sql = "SELECT * FROM VDT_DA_DuAn WHERE iID_MaCDT = @sId_CDT AND (@sId_DonViQuanLy is null or iID_DonViQuanLyID=@sId_DonViQuanLy)";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_DA_DuAn>(sql,
                       param: new
                       {
                           sId_CDT,
                           sId_DonViQuanLy
                       },
                       commandType: System.Data.CommandType.Text);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach nguon von theo du an
        /// </summary>
        /// <param name="iID_DuAnID"></param>
        /// <returns></returns>
        public List<NS_NguonNganSach> LayDanhSachNguonVonTheoDuAn(Guid iID_DuAnID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@DuAnId", iID_DuAnID);
                    var items = conn.Query<NS_NguonNganSach>("sp_vdt_get_nguon_ngan_sach_by_du_an_id", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// load gia tri thanh toan theo hop dong
        /// </summary>
        /// <param name="iCoQuanThanhToan"></param>
        /// <param name="ngayDeNghi"></param>
        /// <param name="bThanhToanTheoHopDong"></param>
        /// <param name="iIdChungTu"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="thanhToanTN"></param>
        /// <param name="thanhToanNN"></param>
        /// <param name="tamUngTN"></param>
        /// <param name="tamUngNN"></param>
        /// <param name="luyKeTUUngTruocTN"></param>
        /// <param name="luyKeTUUngTruocNN"></param>
        public void LoadGiaTriThanhToan(int iCoQuanThanhToan, DateTime ngayDeNghi, bool bThanhToanTheoHopDong, string iIdChungTu, int nguonVonId, int namKeHoach, ref double thanhToanTN, ref double thanhToanNN, ref double tamUngTN, ref double tamUngNN, ref double luyKeTUUngTruocTN, ref double luyKeTUUngTruocNN)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("sp_vdt_laygiatridenghithanhtoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@bThanhToanTheoHopDong", bThanhToanTheoHopDong);
                    lstParam.Add("@iIdChungTu", iIdChungTu);
                    lstParam.Add("@NgayDeNghi", ngayDeNghi);
                    lstParam.Add("@NguonVonId", nguonVonId);
                    lstParam.Add("@NamKeHoach", namKeHoach);
                    lstParam.Add("@iCoQuanThanhToan", iCoQuanThanhToan);

                    var items = conn.Query<DeNghiThanhToanValueQuery>(sql,
                      param: lstParam,
                      commandType: System.Data.CommandType.Text);

                    //List<DeNghiThanhToanValueQuery> list = conn.Query<DeNghiThanhToanValueQuery>("sp_vdt_lay_gia_tri_denghi_thanh_toan", lstParam, commandType: CommandType.StoredProcedure).ToList();
                    DeNghiThanhToanValueQuery item = items != null && items.ToList().Count > 0 ? items.ToList().FirstOrDefault() : null;
                    if (item != null)
                    {
                        thanhToanTN = item.ThanhToanTN;
                        thanhToanNN = item.ThanhToanNN;
                        tamUngTN = item.TamUngTN - item.ThuHoiUngTN;
                        tamUngNN = item.TamUngNN - item.ThuHoiUngNN;
                        luyKeTUUngTruocTN = item.TamUngUngTruocTN - item.ThuHoiUngUngTruocTN;
                        luyKeTUUngTruocNN = item.TamUngUngTruocNN - item.ThuHoiUngUngTruocNN;
                    }
                    else
                    {
                        thanhToanTN = 0;
                        thanhToanNN = 0;
                        tamUngTN = 0;
                        tamUngNN = 0;
                        luyKeTUUngTruocTN = 0;
                        luyKeTUUngTruocNN = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        // hàm lấy danh sách đề nghị thanh toán mới, update theo desktop
        public void LoadGiaTriThanhToanNew(int iCoQuanThanhToan, DateTime ngayDeNghi, bool bThanhToanTheoHopDong, string iIdChungTu, int nguonVonId, int namKeHoach, int? loaiCoQuanTaiChinh, ref double thanhToanTN, ref double thanhToanNN, ref double tamUngTN, ref double tamUngNN, ref double luyKeTUUngTruocTN, ref double luyKeTUUngTruocNN, ref double sumTN, ref double sumNN, int? keHoachVon)
        {
            try
            {
                var sql = "";
                using (var conn = _connectionFactory.GetConnection())
                {
                    sql = "EXECUTE dbo.sp_vdt_lay_gia_tri_denghi_thanh_toan_edit_3 @bThanhToanTheoHopDong, @iIdChungTu, @NgayDeNghi, @NguonVonId, @NamKeHoach, @iCoQuanThanhToan, @loaiCoQuanTaiChinh, @loaiKhv";
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@bThanhToanTheoHopDong", bThanhToanTheoHopDong);
                    lstParam.Add("@iIdChungTu", iIdChungTu);
                    lstParam.Add("@NgayDeNghi", ngayDeNghi);
                    lstParam.Add("@NguonVonId", nguonVonId);
                    lstParam.Add("@NamKeHoach", namKeHoach);
                    lstParam.Add("@iCoQuanThanhToan", iCoQuanThanhToan);
                    lstParam.Add("@loaiCoQuanTaiChinh", loaiCoQuanTaiChinh == null ? 0 : loaiCoQuanTaiChinh);
                    lstParam.Add("@loaiKhv", keHoachVon == null ? keHoachVon : 0);
                    List<DeNghiThanhToanValueQuery> list = conn.Query<DeNghiThanhToanValueQuery>(sql, param: lstParam, commandType: CommandType.Text).ToList();
                    DeNghiThanhToanValueQuery item = list != null && list.Count > 0 ? list.FirstOrDefault() : null;
                    if (item != null)
                    {
                        thanhToanTN = item.ThanhToanTN;
                        sumTN = item.ThanhToanTN + item.TamUngTN + item.ThuHoiUngTN;
                        thanhToanNN = item.ThanhToanNN;
                        sumNN = item.ThanhToanNN + item.TamUngNN + item.ThuHoiUngNN;
                        tamUngTN = item.TamUngTN - item.ThuHoiUngTN;
                        tamUngNN = item.TamUngNN - item.ThuHoiUngNN;
                        luyKeTUUngTruocTN = item.TamUngUngTruocTN - item.ThuHoiUngUngTruocTN;
                        luyKeTUUngTruocNN = item.TamUngUngTruocNN - item.ThuHoiUngUngTruocNN;
                    }
                    else
                    {
                        thanhToanTN = 0;
                        thanhToanNN = 0;
                        tamUngTN = 0;
                        tamUngNN = 0;
                        luyKeTUUngTruocTN = 0;
                        luyKeTUUngTruocNN = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// get de nghi thanh toan KHV by iid de nghi thanh toan
        /// </summary>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <returns></returns>
        public List<VDT_TT_DeNghiThanhToan_KHV> FindDeNghiThanhToanKHVByDeNghiThanhToanID(Guid iID_DeNghiThanhToanID)
        {
            try
            {
                var sql = "SELECT * FROM VDT_TT_DeNghiThanhToan_KHV WHERE iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_TT_DeNghiThanhToan_KHV>(sql,
                       param: new
                       {
                           iID_DeNghiThanhToanID
                       },
                       commandType: System.Data.CommandType.Text);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach khv 
        /// </summary>
        /// <param name="duAnId"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <returns></returns>
        public List<KeHoachVonModel> GetKeHoachVonCapPhatThanhToan(string duAnId, int nguonVonId, DateTime dNgayDeNghi, int namKeHoach, int iCoQuanThanhToan, Guid iID_DeNghiThanhToanID)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("sp_vdt_get_kehoachvon_capphatthanhtoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@DuAnId", duAnId);
                    lstParam.Add("@NguonVonId", nguonVonId);
                    lstParam.Add("@dNgayDeNghi", dNgayDeNghi);
                    lstParam.Add("@NamKeHoach", namKeHoach);
                    lstParam.Add("@iCoQuanThanhToan", iCoQuanThanhToan);
                    lstParam.Add("@iIdPheDuyet", iID_DeNghiThanhToanID);

                    var items = conn.Query<KeHoachVonModel>(sql,
                       param: lstParam,
                       commandType: System.Data.CommandType.Text);

                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return new List<KeHoachVonModel>();
        }

        /// <summary>
        /// load ke hoach von thanh toan
        /// </summary>
        /// <param name="iIdDuAnId"></param>
        /// <param name="iIdNguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <param name="iIdPheDuyet"></param>
        /// <returns></returns>
        public List<VdtTtKeHoachVonQuery> LoadKeHoachVonThanhToan(string iIdDuAnId, int iIdNguonVonId, DateTime dNgayDeNghi, int iNamKeHoach, int iCoQuanThanhToan, Guid? iIdPheDuyet)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("sp_vdt_tt_get_khvthanhtoan.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iIdDuAnId", iIdDuAnId);
                    lstParam.Add("@iIdNguonVonId", iIdNguonVonId);
                    lstParam.Add("@dNgayDeNghi", dNgayDeNghi);
                    lstParam.Add("@iNamKeHoach", iNamKeHoach);
                    lstParam.Add("@iCoQuanThanhToan", iCoQuanThanhToan);
                    lstParam.Add("@iIdPheDuyet", iIdPheDuyet);

                    var items = conn.Query<VdtTtKeHoachVonQuery>(sql,
                       param: lstParam,
                       commandType: System.Data.CommandType.Text);

                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lay danh sach de nghi tam ung
        /// </summary>
        /// <param name="duAnId"></param>
        /// <param name="nguonVonId"></param>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="namKeHoach"></param>
        /// <param name="iCoQuanThanhToan"></param>
        /// <returns></returns>
        public List<KeHoachVonModel> GetDeNghiTamUngCapPhatThanhToan(string duAnId, int nguonVonId, DateTime dNgayDeNghi, int namKeHoach, int iCoQuanThanhToan, Guid iID_DeNghiThanhToanID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@DuAnId", duAnId);
                    lstParam.Add("@NguonVonId", nguonVonId);
                    lstParam.Add("@dNgayDeNghi", dNgayDeNghi);
                    lstParam.Add("@NamKeHoach", namKeHoach);
                    lstParam.Add("@iCoQuanThanhToan", iCoQuanThanhToan);
                    lstParam.Add("@iIdPheDuyet", iID_DeNghiThanhToanID);
                    var items = conn.Query<KeHoachVonModel>("sp_vdt_get_denghitamung_capphatthanhtoan", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// update phe duyet thanh toan chi tiet
        /// </summary>
        /// <param name="lstData"></param>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <param name="sUserLogin"></param>
        /// <returns></returns>
        public bool UpdatePheDuyetThanhToanChiTiet(List<PheDuyetThanhToanChiTiet> lstData, Guid iID_DeNghiThanhToanID, string sUserLogin, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    if (DeletePheDuyetThanhToanChiTietByThanhToanId(iID_DeNghiThanhToanID))
                    {
                        conn.Open();
                        var trans = conn.BeginTransaction();

                        VDT_TT_DeNghiThanhToan objDeNghiThanhToan = GetDeNghiThanhToanByID(iID_DeNghiThanhToanID);
                        // insert VDT_TT_PheDuyetThanhToan_ChiTiet
                        if (lstData != null && lstData.Count > 0)
                        {
                            List<VDT_TT_PheDuyetThanhToan_ChiTiet> lstPheDuyetChiTiet = new List<VDT_TT_PheDuyetThanhToan_ChiTiet>();
                            foreach (var item in lstData)
                            {
                                if (!item.isDelete)
                                    lstPheDuyetChiTiet.Add(ConvertDataPheDuyetThanhToanChiTiet(item, iID_DeNghiThanhToanID, sUserLogin, iNamLamViec));
                            }

                            conn.Insert<VDT_TT_PheDuyetThanhToan_ChiTiet>(lstPheDuyetChiTiet, trans);
                        }
                        trans.Commit();

                        // insert bang tong hop
                        XuLySoLieuBangTongHop(lstData, objDeNghiThanhToan);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// get thong tin chi tiet de nghi thanh toan
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        public GiaiNganThanhToanViewModel GetDeNghiThanhToanDetailByID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_chitiet_denghithanhtoan.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<GiaiNganThanhToanViewModel>(sql,
                        param: new
                        {
                            iID_DeNghiThanhToanID = iId
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
        /// get danh sanh phe duyet thanh toan chi tiet by de nghi thanh toan id
        /// </summary>
        /// <param name="iID_DeNghiThanhToanID"></param>
        /// <returns></returns>
        public List<PheDuyetThanhToanChiTiet> GetListPheDuyetChiTietByDeNghiId(Guid iID_DeNghiThanhToanID)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_list_pheduyetthanhtoan_chitiet_by_iid_denghithanhtoan.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<PheDuyetThanhToanChiTiet>(sql,
                        param: new
                        {
                            iID_DeNghiThanhToanID = iID_DeNghiThanhToanID
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

        public List<PheDuyetThanhToanChiTiet> GetListPheDuyetChiTietDetail(Guid iID_DeNghiThanhToanID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@uIdPheDuyet", iID_DeNghiThanhToanID);
                    var items = conn.Query<PheDuyetThanhToanChiTiet>("sp_tt_get_pheduyetthanhtoanchitiet_by_parentid", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        private void XuLySoLieuBangTongHop(List<PheDuyetThanhToanChiTiet> data, VDT_TT_DeNghiThanhToan objDeNghiThanhToan)
        {
            List<TongHopNguonNSDauTuQuery> lstDataInsert = new List<TongHopNguonNSDauTuQuery>();
            List<TongHopNguonNSDauTuQuery> lstChungTuNguon = new List<TongHopNguonNSDauTuQuery>();
            List<TongHopNguonNSDauTuQuery> lstChungTuAppend = new List<TongHopNguonNSDauTuQuery>();

            if (data != null && data.Count > 0)
            {
                foreach (var item in
                data.Where(n => !n.isDelete && (n.fGiaTriNgoaiNuoc != 0 || n.fGiaTriTrongNuoc != 0)
                && n.iID_KeHoachVonID != null))
                {
                    TongHopNguonNSDauTuQuery objTongHop = new TongHopNguonNSDauTuQuery();
                    var lstAppend = ConvertDataTongHop(item, objDeNghiThanhToan, ref objTongHop);
                    lstDataInsert.Add(objTongHop);
                    if (lstAppend != null && lstAppend.Count != 0)
                        lstChungTuAppend.AddRange(lstAppend);

                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                    {
                        lstChungTuNguon.Add(new TongHopNguonNSDauTuQuery()
                        {
                            iID_ChungTu = item.iID_KeHoachVonID.Value,
                            sMaDich = objTongHop.sMaNguon,
                            sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU
                        });
                    }
                    else
                    {
                        lstChungTuNguon.Add(new TongHopNguonNSDauTuQuery()
                        {
                            iID_ChungTu = item.iID_KeHoachVonID.Value,
                            sMaNguon = GetMaNguonChaByKeHoachVon(item),
                            sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU,
                        });
                    }
                }

                if (lstDataInsert.Count != 0)
                {
                    InsertTongHopNguonDauTu_ThanhToan_Giam(LOAI_CHUNG_TU.CAP_THANH_TOAN, (int)LoaiXuLy.CapNhat, objDeNghiThanhToan.iID_DeNghiThanhToanID, lstDataInsert, lstChungTuNguon, lstChungTuAppend);
                }
            }
        }

        public List<TongHopNguonNSDauTuQuery> ConvertDataTongHop(PheDuyetThanhToanChiTiet item, VDT_TT_DeNghiThanhToan objDeNghiThanhToan, ref TongHopNguonNSDauTuQuery data)
        {
            List<TongHopNguonNSDauTuQuery> results = new List<TongHopNguonNSDauTuQuery>();
            data.iID_ChungTu = objDeNghiThanhToan.iID_DeNghiThanhToanID;
            data.fGiaTri = item.fGiaTriTrongNuoc + item.fGiaTriNgoaiNuoc;
            data.bIsLog = false;
            data.iID_DuAnID = objDeNghiThanhToan.iID_DuAnId ?? Guid.Empty;
            data.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
            data.IIDMucID = item.iID_MucID;
            data.IIDTieuMucID = item.iID_TieuMucID;
            data.IIDTietMucID = item.iID_TietMucID;
            data.IIDNganhID = item.iID_NganhID;
            if (item.iID_KeHoachVonID != null)
            {
                data.iId_MaNguonCha = item.iID_KeHoachVonID;
                data.sMaNguonCha = GetMaNguonChaByKeHoachVon(item);

            }
            switch (objDeNghiThanhToan.iCoQuanThanhToan)
            {
                case (int)CoQuanThanhToan.Type.KHO_BAC:
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THANH_TOAN)
                        data.sMaDich = LOAI_CHUNG_TU.TT_THANHTOAN_KHOBAC;
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.TAM_UNG)
                        data.sMaDich = LOAI_CHUNG_TU.TT_UNG_KHOBAC;
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                    {
                        data.sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU;
                        data.sMaNguon = LOAI_CHUNG_TU.TT_UNG_KHOBAC;
                        data.iId_MaNguonCha = item.iID_KeHoachVonID;
                        data.sMaNguonCha = GetMaNguonChaByKeHoachVon(item);
                        if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC
                            || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                        {
                            data.iThuHoiTUCheDo = 1;
                        }
                        else
                        {
                            data.iThuHoiTUCheDo = 2;
                        }
                        if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC
                            || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY)
                            data.ILoaiUng = 2;
                        else

                            data.ILoaiUng = 1;
                    }
                    break;

                case (int)CoQuanThanhToan.Type.CQTC:
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THANH_TOAN) data.sMaDich = LOAI_CHUNG_TU.TT_THANHTOAN_LENHCHI;
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.TAM_UNG) data.sMaDich = LOAI_CHUNG_TU.TT_UNG_LENHCHI;
                    if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY
                        || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                    {
                        data.sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU;
                        data.sMaNguon = LOAI_CHUNG_TU.TT_UNG_LENHCHI;
                        data.iId_MaNguonCha = item.iID_KeHoachVonID;
                        data.sMaNguonCha = GetMaNguonChaByKeHoachVon(item);
                        if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_NAM_TRUOC
                            || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC)
                        {
                            data.iThuHoiTUCheDo = 1;
                        }
                        else
                        {
                            data.iThuHoiTUCheDo = 2;
                        }

                        if (item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_TRUOC
                            || item.iLoaiDeNghi == (int)PaymentTypeEnum.Type.THU_HOI_UNG_TRUOC_NAM_NAY)
                            data.ILoaiUng = 2;
                        else

                            data.ILoaiUng = 1;
                    }
                    break;
            }

            if (objDeNghiThanhToan.bHoanTraUngTruoc.HasValue && objDeNghiThanhToan.bHoanTraUngTruoc.Value)
            {
                TongHopNguonNSDauTuQuery childThuHoiUng = new TongHopNguonNSDauTuQuery();
                TongHopNguonNSDauTuQuery childThuHoiKhoBac = new TongHopNguonNSDauTuQuery();
                childThuHoiUng.iID_ChungTu = objDeNghiThanhToan.iID_DeNghiThanhToanID;
                childThuHoiUng.fGiaTri = item.fGiaTriTrongNuoc + item.fGiaTriNgoaiNuoc;
                childThuHoiUng.bIsLog = false;
                childThuHoiUng.iID_DuAnID = objDeNghiThanhToan.iID_DuAnId ?? Guid.Empty;
                childThuHoiUng.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                childThuHoiUng.IIDMucID = item.iID_MucID;
                childThuHoiUng.IIDTieuMucID = item.iID_TieuMucID;
                childThuHoiUng.IIDTietMucID = item.iID_TietMucID;
                childThuHoiUng.IIDNganhID = item.iID_NganhID;

                childThuHoiKhoBac.iID_ChungTu = objDeNghiThanhToan.iID_DeNghiThanhToanID;
                childThuHoiKhoBac.fGiaTri = item.fGiaTriTrongNuoc + item.fGiaTriNgoaiNuoc;
                childThuHoiKhoBac.bIsLog = false;
                childThuHoiKhoBac.iID_DuAnID = objDeNghiThanhToan.iID_DuAnId ?? Guid.Empty;
                childThuHoiKhoBac.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                childThuHoiKhoBac.IIDMucID = item.iID_MucID;
                childThuHoiKhoBac.IIDTieuMucID = item.iID_TieuMucID;
                childThuHoiKhoBac.IIDTietMucID = item.iID_TietMucID;
                childThuHoiKhoBac.IIDNganhID = item.iID_NganhID;
                if (objDeNghiThanhToan.iCoQuanThanhToan == (int)CoQuanThanhToan.Type.KHO_BAC)
                {
                    childThuHoiUng.sMaNguon = LOAI_CHUNG_TU.TT_UNG_KHOBAC;
                    childThuHoiUng.sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU;
                    childThuHoiUng.sMaNguonCha = LOAI_CHUNG_TU.KHVN_KHOBAC;
                    childThuHoiUng.ILoaiUng = 2;
                    childThuHoiUng.iThuHoiTUCheDo = 1;


                    childThuHoiKhoBac.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                    childThuHoiKhoBac.sMaDich = LOAI_CHUNG_TU.KHVU_KHOBAC;
                    childThuHoiKhoBac.sMaNguonCha = LOAI_CHUNG_TU.KHVN_KHOBAC;
                }
                else
                {
                    childThuHoiUng.sMaNguon = LOAI_CHUNG_TU.TT_UNG_LENHCHI;
                    childThuHoiUng.sMaDich = LOAI_CHUNG_TU.CHU_DAU_TU;
                    childThuHoiUng.sMaNguonCha = LOAI_CHUNG_TU.KHVN_LENHCHI;
                    childThuHoiUng.ILoaiUng = 2;
                    childThuHoiUng.iThuHoiTUCheDo = 1;

                    childThuHoiKhoBac.sMaNguon = LOAI_CHUNG_TU.CHU_DAU_TU;
                    childThuHoiKhoBac.sMaDich = LOAI_CHUNG_TU.KHVU_LENHCHI;
                    childThuHoiKhoBac.sMaNguonCha = LOAI_CHUNG_TU.KHVN_LENHCHI;
                }
                results.Add(childThuHoiKhoBac);
                results.Add(childThuHoiUng);
            }
            return results;
        }

        private string GetMaNguonChaByKeHoachVon(PheDuyetThanhToanChiTiet item)
        {
            if (item.iLoaiKeHoachVon == 1)
            {
                if (item.iLoaiNamKH == (int)NamKeHoachEnum.Type.NAM_TRUOC)
                {
                    return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.QT_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_LENHCHI_CHUYENNAMTRUOC;
                }
                else
                {
                    return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.KHVN_KHOBAC : LOAI_CHUNG_TU.KHVN_LENHCHI;
                }
            }
            if (item.iLoaiKeHoachVon == 2)
            {
                if (item.iLoaiNamKH == (int)NamKeHoachEnum.Type.NAM_TRUOC)
                {
                    return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.QT_UNG_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_UNG_LENHCHI_CHUYENNAMTRUOC;
                }
                else
                {
                    return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.KHVU_KHOBAC : LOAI_CHUNG_TU.KHVU_LENHCHI;
                }
            }
            if (item.iLoaiKeHoachVon == 3)
            {
                return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.QT_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_LENHCHI_CHUYENNAMTRUOC;
            }
            if (item.iLoaiKeHoachVon == 4)
            {
                return item.iCoQuanThanhToanKHV == (int)CoQuanThanhToan.Type.KHO_BAC ? LOAI_CHUNG_TU.QT_UNG_KHOBAC_CHUYENNAMTRUOC : LOAI_CHUNG_TU.QT_UNG_LENHCHI_CHUYENNAMTRUOC;
            }
            return string.Empty;
        }

        public void InsertTongHopNguonDauTu_ThanhToan_Giam(string sLoai, int iTypeExecute, Guid iIdQuyetDinh, List<TongHopNguonNSDauTuQuery> lstChungTu, List<TongHopNguonNSDauTuQuery> lstNguon, List<TongHopNguonNSDauTuQuery> lstChungTuAppend = null)
        {
            if (lstChungTu == null || lstChungTu.Count == 0) return;
            List<TongHopNguonNSDauTuQuery> lstChungTuUpdate = new List<TongHopNguonNSDauTuQuery>();
            List<TongHopNguonNSDauTuQuery> lstNguonVon = GetNguonVonTongHopNguonDauTuByCondition(lstNguon);
            Dictionary<Guid, double> dicNguonVon = new Dictionary<Guid, double>();

            foreach (var objChungTu in lstChungTu)
            {
                var lstIdRemove = new List<Guid>();
                double fThanhToan = objChungTu.fGiaTri ?? 0;
                foreach (var objNguonVon in lstNguonVon)
                {
                    if (!dicNguonVon.ContainsKey(objNguonVon.Id))
                        dicNguonVon.Add(objNguonVon.Id, objNguonVon.fGiaTri ?? 0);

                    if (objNguonVon.iID_DuAnID == objChungTu.iID_DuAnID
                        && objNguonVon.iID_ChungTu == objChungTu.iId_MaNguonCha)
                    {
                        if (objChungTu.sMaDich == LOAI_CHUNG_TU.CHU_DAU_TU)
                        {
                            lstChungTuUpdate.Add(new TongHopNguonNSDauTuQuery()
                            {
                                fGiaTri = objChungTu.fGiaTri,
                                iID_ChungTu = objChungTu.iID_ChungTu,
                                iID_DuAnID = objChungTu.iID_DuAnID,
                                sMaDich = objChungTu.sMaDich,
                                sMaNguon = objChungTu.sMaNguon,
                                sMaNguonCha = objChungTu.sMaNguonCha,
                                iId_MaNguonCha = objChungTu.iId_MaNguonCha,
                                iThuHoiTUCheDo = objChungTu.iThuHoiTUCheDo,
                                ILoaiUng = objChungTu.ILoaiUng,
                                iStatus = (int)NguonVonStatus.DaSuDung,
                                IIDMucID = objChungTu.IIDMucID,
                                IIDTieuMucID = objChungTu.IIDTieuMucID,
                                IIDTietMucID = objChungTu.IIDTietMucID,
                                IIDNganhID = objChungTu.IIDNganhID
                            });
                            break;
                        }

                        if (fThanhToan >= dicNguonVon[objNguonVon.Id])
                        {
                            lstChungTuUpdate.Add(new TongHopNguonNSDauTuQuery()
                            {
                                fGiaTri = dicNguonVon[objNguonVon.Id],
                                iID_ChungTu = objChungTu.iID_ChungTu,
                                iID_DuAnID = objChungTu.iID_DuAnID,
                                sMaDich = objChungTu.sMaDich,
                                sMaNguon = objChungTu.sMaNguon,
                                sMaNguonCha = objChungTu.sMaNguonCha,
                                iId_MaNguonCha = objChungTu.iId_MaNguonCha,
                                ILoaiUng = objChungTu.ILoaiUng,
                                iStatus = (int)NguonVonStatus.DaSuDung,
                                IIDMucID = objChungTu.IIDMucID,
                                IIDTieuMucID = objChungTu.IIDTieuMucID,
                                IIDTietMucID = objChungTu.IIDTietMucID,
                                IIDNganhID = objChungTu.IIDNganhID
                            });
                            fThanhToan = fThanhToan - dicNguonVon[objNguonVon.Id];
                            objNguonVon.iStatus = (int)NguonVonStatus.DaSuDung;
                            lstIdRemove.Add(objNguonVon.Id);
                        }
                        else
                        {
                            lstChungTuUpdate.Add(new TongHopNguonNSDauTuQuery()
                            {
                                fGiaTri = fThanhToan,
                                iID_ChungTu = objChungTu.iID_ChungTu,
                                iID_DuAnID = objChungTu.iID_DuAnID,
                                sMaDich = objChungTu.sMaDich,
                                sMaNguon = objChungTu.sMaNguon,
                                sMaNguonCha = objChungTu.sMaNguonCha,
                                iId_MaNguonCha = objChungTu.iId_MaNguonCha,
                                ILoaiUng = objChungTu.ILoaiUng,
                                iStatus = (int)NguonVonStatus.DaSuDung,
                                IIDMucID = objChungTu.IIDMucID,
                                IIDTieuMucID = objChungTu.IIDTieuMucID,
                                IIDTietMucID = objChungTu.IIDTietMucID,
                                IIDNganhID = objChungTu.IIDNganhID
                            });
                            dicNguonVon[objNguonVon.Id] = dicNguonVon[objNguonVon.Id] - fThanhToan;
                            break;
                        }
                    }
                }
                lstNguonVon = lstNguonVon.Where(n => !lstIdRemove.Contains(n.Id)).ToList();
            }

            if (lstChungTuAppend != null)
            {
                lstChungTuUpdate.AddRange(lstChungTuAppend);
            }

            if (lstChungTuUpdate.Count != 0)
            {
                InsertTongHopNguonDauTu(iIdQuyetDinh, sLoai, lstChungTuUpdate);
            }
        }

        /// <summary>
        /// get nguon von tong hop nguon dau tu KHVN
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <returns></returns>
        public List<TongHopNguonNSDauTuQuery> GetNguonVonTongHopNguonDauTuByCondition(List<TongHopNguonNSDauTuQuery> lstCondition)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var conditions = ConvertDataToTableDefined("t_tbl_tonghopdautu", lstCondition);

                    var data = conn.Query<TongHopNguonNSDauTuQuery>("sp_get_nguonvon_tonghopnguondautu_by_condition", new { data = conditions.AsTableValuedParameter("t_tbl_tonghopdautu") }, commandType: CommandType.StoredProcedure);
                    if (data == null) return new List<TongHopNguonNSDauTuQuery>();
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// get mlns by ke hoach von
        /// </summary>
        /// <param name="iNamLamViec"></param>
        /// <param name="lstCondition"></param>
        /// <returns></returns>
        public List<MlnsByKeHoachVonModel> GetMucLucNganSachByKeHoachVon(int iNamLamViec, List<TongHopNguonNSDauTuQuery> lstCondition)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var conditions = ConvertDataToTableDefined("t_tbl_tonghopdautu", lstCondition);

                    SqlParameter dtDetailParam = new SqlParameter();
                    dtDetailParam.ParameterName = "data";
                    dtDetailParam.Value = conditions;
                    dtDetailParam.TypeName = "t_tbl_tonghopdautu";
                    dtDetailParam.SqlDbType = SqlDbType.Structured;

                    var data = conn.Query<MlnsByKeHoachVonModel>("sp_vdt_tt_get_mlns_by_khv", new { data = conditions.AsTableValuedParameter("t_tbl_tonghopdautu"), iNamLamViec = iNamLamViec }, commandType: CommandType.StoredProcedure);
                    if (data == null) return new List<MlnsByKeHoachVonModel>();
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VdtBcTheoDoiChiTieuCapPhatModel> GetBcTheoDoiChiTieuCapPhatBySearch(string iIdMaDonVi, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iIdMaDonVi", iIdMaDonVi);
                    lstParam.Add("@iNamLamViec", iNamLamViec);
                    var data = conn.Query<VdtBcTheoDoiChiTieuCapPhatModel>("sp_VDT_BC_TheoDoiChiTieuCapPhatDuAn", lstParam, commandType: CommandType.StoredProcedure);
                    if (data == null) return new List<VdtBcTheoDoiChiTieuCapPhatModel>();
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return new List<VdtBcTheoDoiChiTieuCapPhatModel>();
        }

        /// <summary>
        /// luu data vao bang tong hop
        /// </summary>
        /// <param name="iIDChungTu"></param>
        /// <param name="sLoai"></param>
        /// <param name="lstData"></param>
        public void InsertTongHopNguonDauTu(Guid iIDChungTu, string sLoai, List<TongHopNguonNSDauTuQuery> lstData)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var data = ConvertDataToTableDefined("t_tbl_tonghopdautu", lstData);

                    using (var cmd1 = new SqlCommand("sp_insert_tonghopnguondautu", conn))
                    {
                        cmd1.Parameters.AddWithValue("@sLoai", sLoai);
                        cmd1.Parameters.AddWithValue("@iIDChungTu", iIDChungTu);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@data", data);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.t_tbl_tonghopdautu";

                        conn.Open();
                        cmd1.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// convert list to table defined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sTypeName"></param>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public static DataTable ConvertDataToTableDefined<T>(string sTypeName, List<T> lstData) where T : class
        {
            DataTable dt = new DataTable();
            dt.TableName = sTypeName;
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in properties)
            {
                if (item.CustomAttributes != null && ((IEnumerable<CustomAttributeData>)item.CustomAttributes).Any()) continue;
                dt.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
            }
            foreach (T item in lstData)
            {
                DataRow dr = dt.NewRow();
                foreach (var prop in properties)
                {
                    if (dr.Table.Columns.IndexOf(prop.Name) == -1) continue;
                    dr[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static DataTable ConvertDataToTableTypeDefined<T>(string sTypeName, string sPropertyname, List<T> lstData)
        {
            DataTable dt = new DataTable();
            dt.TableName = sTypeName;
            dt.Columns.Add(sPropertyname, typeof(T));
            if (lstData != null)
            {
                foreach (T item in lstData)
                {
                    DataRow dr = dt.NewRow();
                    dr[sPropertyname] = item;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>
        /// lấy thông tin chi phí cho màn đề nghị thanh toán
        /// </summary>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="iIdDuAnId"></param>
        /// <returns></returns>
        public List<VdtTtDeNghiThanhToanChiPhiQuery> GetChiPhiInDenghiThanhToanScreen(DateTime dNgayDeNghi, Guid iIdDuAnId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@dNgayDeNghi", dNgayDeNghi);
                    lstParam.Add("@iIdDuAnId", iIdDuAnId);
                    var items = conn.Query<VdtTtDeNghiThanhToanChiPhiQuery>("sp_vdt_tt_getchiphi_in_denghithanhtoan_screen", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// lấy danh sách nhà thầu theo hợp đồng
        /// </summary>
        /// <param name="dNgayDeNghi"></param>
        /// <param name="iIdDuAnId"></param>
        /// <returns></returns>
        public List<VDT_DM_NhaThau> GetNhaThauByHopDong(Guid iIdHopDongId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iIdHopDongId", iIdHopDongId);
                    var items = conn.Query<VDT_DM_NhaThau>("sp_vdt_get_nhathau_by_hopdong", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public CapPhatThanhToanReportQuery GetThongTinPhanGhiCoQuanTaiChinh(Guid id, int iNamLamViec)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@Id", id);
                    lstParam.Add("@NamLamViec", iNamLamViec);
                    var item = conn.QueryFirstOrDefault<CapPhatThanhToanReportQuery>("sp_vdt_report_cap_phat_thanh_toan", lstParam, commandType: CommandType.StoredProcedure);
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public DeNghiThanhToanValueQuery LoadGiaTriPheDuyetThanhToanByParentId(Guid id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("@iIdPheDuyetId", id);
                    var item = conn.QueryFirstOrDefault<DeNghiThanhToanValueQuery>("sp_tt_get_giatrithanhtoanthuctepheduyetthanhtoan_by_parentid", lstParam, commandType: CommandType.StoredProcedure);
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        #endregion

        #region KH chi quy
        /// <summary>
        /// get kinh phi cuc tai chinh cap
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iIdMaDonVi"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        public KinhPhiCucTaiChinhCap GetKinhPhiCucTaiChinhCap(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_kinhphi_ctc_cap.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<KinhPhiCucTaiChinhCap>(sql,
                        param: new
                        {
                            iNamKeHoach = iNamKeHoach,
                            iIdMaDonVi = iIdMaDonVi,
                            iIdNguonVon = iIdNguonVon,
                            iQuy = iQuy
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
        /// get nhu cau chi tiet
        /// </summary>
        /// <param name="iNamKeHoach"></param>
        /// <param name="iIdMaDonVi"></param>
        /// <param name="iIdNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        public List<NcNhuCauChi_ChiTiet> GetNhuCauChiChiTiet(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy, long iTiGia = 1)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_nhucauchi_chitiet.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NcNhuCauChi_ChiTiet>(sql,
                        param: new
                        {
                            iNamKeHoach = iNamKeHoach,
                            iIdMaDonVi = iIdMaDonVi,
                            iIdNguonVon = iIdNguonVon,
                            iQuy = iQuy,
                            iTiGia = iTiGia
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

        /// <summary>
        /// get list nhu cau chi chi tiet by nhu cau chi ID
        /// </summary>
        /// <param name="iID_NhuCauChiID"></param>
        /// <returns></returns>
        public List<VDT_NC_NhuCauChi_ChiTiet> GetNhuCauChiChiTietByNhuCauChiID(Guid iID_NhuCauChiID)
        {
            try
            {
                var sql = "SELECT * FROM VDT_NC_NhuCauChi_ChiTiet WHERE iID_NhuCauChiID = @iID_NhuCauChiID";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_NC_NhuCauChi_ChiTiet>(sql,
                       param: new
                       {
                           iID_NhuCauChiID
                       },
                       commandType: System.Data.CommandType.Text);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// luu nhu cau chi 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lstChiTiet"></param>
        /// <param name="sUserLogin"></param>
        /// <returns></returns>
        public bool LuuNhuCauChi(VDT_NC_NhuCauChi data, List<VDT_NC_NhuCauChi_ChiTiet> lstChiTiet, string sUserLogin, bool? isUpdate)
        {
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.Id == Guid.Empty)
                    {

                        #region tao moi
                        data.Id = Guid.NewGuid();
                        data.dDateCreate = DateTime.Now;
                        data.sUserCreate = sUserLogin;
                        if (string.IsNullOrEmpty(data.iID_MaDonViQuanLy))
                        {
                            var objDonViQuanLy = conn.Get<NS_DonVi>(data.iID_DonViQuanLyID, trans);
                            if (objDonViQuanLy != null)
                                data.iID_MaDonViQuanLy = objDonViQuanLy.iID_MaDonVi;
                        }

                        conn.Insert(data, trans);
                        #endregion
                    }
                    else
                    {
                        #region sua
                        var entity = conn.Get<VDT_NC_NhuCauChi>(data.Id, trans);
                        if (entity == null)
                            return false;
                        entity.sSoDeNghi = data.sSoDeNghi;
                        entity.sNguoiLap = data.sNguoiLap;

                        entity.sUserUpdate = sUserLogin;
                        entity.dDateUpdate = DateTime.Now;
                        conn.Update(entity, trans);

                        #endregion
                        if (XoaNhuCauChiChiTiet(data.Id) == false)
                            return false;

                    }


                    #region VDT_NC_NhuCauChi_ChiTiet
                    if (lstChiTiet != null && lstChiTiet.Count > 0)
                    {
                        if (isUpdate != null && isUpdate.Equals("true"))
                        {
                            foreach (VDT_NC_NhuCauChi_ChiTiet item in lstChiTiet)
                            {
                                item.iID_NhuCauChiId = data.Id;
                                item.dDateCreate = DateTime.Now;
                                item.sUserCreate = sUserLogin;
                                conn.Update<VDT_NC_NhuCauChi_ChiTiet>(item, trans);
                            }
                        }
                        else
                        {
                            foreach (VDT_NC_NhuCauChi_ChiTiet item in lstChiTiet)
                            {
                                item.Id = new Guid();
                                item.iID_NhuCauChiId = data.Id;
                                item.dDateCreate = DateTime.Now;
                                item.sUserCreate = sUserLogin;
                            }

                            conn.Insert<VDT_NC_NhuCauChi_ChiTiet>(lstChiTiet, trans);
                        }

                    }

                    #endregion
                    // commit to db
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// xoa nhu cau chi
        /// </summary>
        /// <param name="iID_NhuCauChiID"></param>
        /// <returns></returns>
        public bool XoaNhuCauChi(Guid iID_NhuCauChiID)
        {
            try
            {
                var sql = "DELETE VDT_NC_NhuCauChi WHERE Id =  @iID_NhuCauChiID; ";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_NhuCauChiID
                        },
                        commandType: CommandType.Text);
                    if (r > 0)
                        XoaNhuCauChiChiTiet(iID_NhuCauChiID);
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool XoaNhuCauChiChiTiet(Guid iID_NhuCauChiID)
        {
            var sql = "DELETE VDT_NC_NhuCauChi_ChiTiet WHERE iID_NhuCauChiId =  @iID_NhuCauChiID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                try
                {
                    var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_NhuCauChiID
                    },
                    commandType: CommandType.Text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// get danh sach nhu cau chi quy theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoDeNghi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="dNgayDeNghiFrom"></param>
        /// <param name="dNgayDeNghiTo"></param>
        /// <param name="iIDMaDonViQuanLy"></param>
        /// <param name="iIDNguonVon"></param>
        /// <param name="iQuy"></param>
        /// <returns></returns>
        public IEnumerable<NCNhuCauChi> GetAllKHNhuCauChiPaging(ref PagingInfo _paging, string sSoDeNghi, int? iNamKeHoach, DateTime? dNgayDeNghiFrom, DateTime? dNgayDeNghiTo, string iIDMaDonViQuanLy, int? iIDNguonVon, int? iQuy)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoDeNghi", sSoDeNghi);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("dNgayDeNghiFrom", dNgayDeNghiFrom);
                    lstParam.Add("dNgayDeNghiTo", dNgayDeNghiTo);
                    lstParam.Add("iIDMaDonViQuanLy", iIDMaDonViQuanLy);
                    lstParam.Add("iIDNguonVon", iIDNguonVon);
                    lstParam.Add("iQuy", iQuy);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<NCNhuCauChi>("proc_get_all_khnhucauchiquy_paging", lstParam, commandType: CommandType.StoredProcedure);
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
        /// get thong tin nhu cau chi
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        public NCNhuCauChi GetNhuCauChiByID(Guid iId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_chitiet_nhucauchi.sql");

            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<NCNhuCauChi>(sql,
                        param: new
                        {
                            iID_NhuCauChiID = iId
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

        public DataTable GetDataDropdownDonViQuanLy(string sMaNguoiDung, int iNamLamViec)
        {
            String SQL = @"SELECT DISTINCT NS_DonVi.iID_MaDonVi+' - '+NS_DonVi.sTen as TenHT, NS_DonVi.* FROM (SELECT * FROM NS_NguoiDung_DonVi
                          WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec_DonVi AND sMaNguoiDung=@sMaNguoiDung) AS NS_NguoiDung_DonVi 
                          INNER JOIN (SELECT * FROM NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec_DonVi) NS_DonVi ON (NS_NguoiDung_DonVi.iID_MaDonVi=NS_DonVi.iID_MaDonVi) WHERE NS_NguoiDung_DonVi.iTrangThai=1 AND NS_NguoiDung_DonVi.sMaNguoiDung=@sMaNguoiDung ORDER BY NS_DonVi.iID_MaDonVi";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sMaNguoiDung", sMaNguoiDung);
            cmd.Parameters.AddWithValue("@iNamLamViec_DonVi", iNamLamViec);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }


        public VDT_NC_NhuCauChi GetNhuCauChiByiNamKeHoachMaDonVi(int iNamKeHoach, string iIdMaDonVi, int iIdNguonVon, int iQuy)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_NC_NhuCauChi WHERE iID_NguonVonID = {0} AND iID_MaDonViQuanLy = {1} AND iQuy = {2} AND iNamKeHoach = {3}", iIdNguonVon, iIdMaDonVi, iQuy, iNamKeHoach);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_NC_NhuCauChi>(query.ToString()).FirstOrDefault();
                return items;
            }
        }

        #endregion

        #region Kế hoạch vốn ứng đề xuất
        public IEnumerable<VDTKeHoachVonUngDeXuatModel> GetKeHoachVonUngDeXuatByCondition
            (ref PagingInfo _paging, string sSoQuyetDinh = null, DateTime? dNgayQuyetDinhFrom = null, DateTime? dNgayQuyetDinhTo = null,
            int? iNamKeHoach = null, int? iIdNguonVon = null, string iID_MaDonViQuanLyID = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoQuyetDinh", sSoQuyetDinh);
                    lstParam.Add("dNgayQuyetDinhFrom", dNgayQuyetDinhFrom);
                    lstParam.Add("dNgayQuyetDinhTo", dNgayQuyetDinhTo);
                    lstParam.Add("iID_MaDonViQuanLyID", iID_MaDonViQuanLyID);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("iIdNguonVon", iIdNguonVon);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<VDTKeHoachVonUngDeXuatModel>("proc_get_all_vdt_kehoachvonungdx_paging", lstParam, commandType: CommandType.StoredProcedure);
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

        public bool CheckExistSoDeNghiKHVNDX(Guid? iID_KeHoachUngID, string sSoDeNghi)
        {
            try
            {
                var sql = "SELECT * FROM VDT_KHV_KeHoachVonUng_DX WHERE (@iID_KeHoachUngID IS NULL OR Id != @iID_KeHoachUngID) and sSoDeNghi = @sSoDeNghi";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_KHV_KeHoachVonUng>(
                        sql,
                        param: new
                        {
                            iID_KeHoachUngID,
                            sSoDeNghi
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

        public bool deleteKHVUDX(Guid iID_KeHoachUngID, string username)
        {
            try
            {
                var sql = "delete VDT_KHV_KeHoachVonUng_DX where Id = @iID_KeHoachUngID ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX>(iID_KeHoachUngID, trans);
                    if (entity != null && entity.iID_ParentID != null)
                    {
                        var entityChild = conn.Get<VDT_KHV_KeHoachVonUng_DX>(entity.iID_ParentID, trans);
                        entityChild.bActive = true;
                        entityChild.sUserCreate = "username";
                        entityChild.dDateUpdate = DateTime.Now;
                        conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityChild, trans);
                    }

                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_KeHoachUngID
                        },
                        commandType: CommandType.Text, transaction: trans);
                    trans.Commit();

                    return r >= 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool deleteKHVUDXChiTiet(Guid iID_KeHoachUngID)
        {
            try
            {

                var sql = "delete VDT_KHV_KeHoachVonUng_DX_ChiTiet where iID_KeHoachUngID = @iID_KeHoachUngID ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Get<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(iID_KeHoachUngID);
                    if (item != null)
                    {
                        var r = conn.Execute(
                                sql,
                                param: new
                                {
                                    iID_KeHoachUngID
                                },
                                commandType: CommandType.Text) ;
                        return r > 0;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetDuAnInVonUngDeXuatByCondition(string sMaDonVi = null, DateTime? dNgayDeNghi = null, string sTongHop = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@sTongHop", sTongHop);
                    lstParam.Add("@iIdDonViQuanLy", sMaDonVi);
                    lstParam.Add("@dNgayLap", dNgayDeNghi);

                    var items = conn.Query<VdtKhcKeHoachVonUngDeXuatChiTietModel>("sp_vdt_get_duan_kehoachvonungdx_detail", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public VdtKhvuDXChiTietModel GetKeHoachVonUngChiTietById(Guid iId, int iNamLamViec)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_khvu_dx_getchitiettheoid.sql");
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VdtKhvuDXChiTietModel>(sql,
                        param: new
                        {
                            iId,
                            iNamLamViec
                        },
                        commandType: CommandType.Text).FirstOrDefault();

                    if (item != null)
                    {
                        IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> eChiTiet = GetKeHoachVonUngDeXuatDetailById(item.Id);
                        if (eChiTiet != null)
                            item.listKhvuChiTiet = eChiTiet.ToList();

                        if (!string.IsNullOrEmpty(item.sTongHop))
                        {
                            item.listChungTuChild = new List<VdtKhvuDXChiTietModel>();
                            List<string> listIdChild = item.sTongHop.Split(',').ToList();
                            foreach (string idChild in listIdChild)
                            {
                                VdtKhvuDXChiTietModel objChild = GetKeHoachVonUngChiTietById(Guid.Parse(idChild), iNamLamViec);
                                if (objChild != null)
                                    item.listChungTuChild.Add(objChild);
                            }
                        }
                    }

                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool LockOrUnLockKeHoachVonUngDeXuat(Guid id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX>(id, trans);
                    if (entity == null) return false;
                    bool isLockOrUnlock = entity.bKhoa.HasValue ? entity.bKhoa.Value : false;
                    entity.bKhoa = !isLockOrUnlock;

                    conn.Update(entity, trans);

                    trans.Commit();
                    conn.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatDetailById(Guid iID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@iIdKeHoachVonUng", iID);

                    var items = conn.Query<VdtKhcKeHoachVonUngDeXuatChiTietModel>("sp_vdt_get_kehoachvonungdxchitiet_detail", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDT_KHV_KeHoachVonUng_DX> GetKeHoachVonUngDeXuatInVonUngDuocDuyetScreen(string iIdMaDonVi, int iNamKeHoach, DateTime dNgayLap)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@iIdMaDonVi", iIdMaDonVi);
                    lstParam.Add("@iNamKeHoach", iNamKeHoach);
                    lstParam.Add("@dNgayLap", dNgayLap);

                    var items = conn.Query<VDT_KHV_KeHoachVonUng_DX>("sp_vdt_khvu_get_kehoachvonungdexuat_in_duocduyet", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VdtKhvKeHoachTrungHanDeXuatChiTietModel> GetAllKH5NamDeXuatDieuChinhChiTiet(Guid iID_KeHoach5Nam_DeXuatID)
        {
            try
            {
                //var items = new List<VdtKhvKeHoachTrungHanDeXuatChiTietModel>();

                var sql = FileHelpers.GetSqlQuery("vdt_get_list_kh5namdexuat_dieuchinh_chitiet.sql");
                /* var conn = _connectionFactory.GetConnection();
                 DataTable dt = new DataTable();
                 using (var cmd = new SqlCommand(sql, conn))
                 {
                     Console.WriteLine("command created successfuly");

                     SqlDataAdapter adapt = new SqlDataAdapter(cmd);

                     conn.Open();
                     Console.WriteLine("connection opened successfuly");
                     adapt.Fill(dt);
                     conn.Close();
                     Console.WriteLine("connection closed successfuly");
                 }

                 return items;*/
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@iID_KeHoach5Nam_DeXuatIDPara", iID_KeHoach5Nam_DeXuatID);
                    // lstParam.Add("@iID_KeHoach5Nam_DeXuatIDPara", "e96d037e-0554-4362-89f1-aebe008b2930");
                    // lstParam.Add("@iID_KeHoach5Nam_DeXuatIDPara", "9c83fdd7-b383-4a23-8589-aec80118e6e4");

                    var items = conn.Query<VdtKhvKeHoachTrungHanDeXuatChiTietModel>(sql, lstParam, commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VdtKhvKeHoachTrungHanDeXuatChiTietModel> GetAllKH5NamDeXuatChiTiet(Guid iID_KeHoach5Nam_DeXuatID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = "SELECT null as IdParentNew, * FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet WHERE iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatID";
                    var items = conn.Query<VdtKhvKeHoachTrungHanDeXuatChiTietModel>(sql,
                        param: new
                        {
                            iID_KeHoach5Nam_DeXuatID
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

        public IEnumerable<DuAnKeHoach5Nam> GetIDDuAnKHTHDeXuatChiTietByIDDeXuat(Guid iID_KeHoach5Nam_DeXuatID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var sql = "SELECT iID_DuAnID as IDDuAnID FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet WHERE iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatID";
                    var items = conn.Query<DuAnKeHoach5Nam>(sql,
                        param: new
                        {
                            iID_KeHoach5Nam_DeXuatID
                        },
                        commandType: CommandType.Text);
                    foreach (var item in items)
                    {
                        item.IsChecked = true;
                    }
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool LockOrUnLockKeHoach5NamDeXuat(Guid id, bool isLockOrUnLock)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<VDT_KHV_KeHoach5Nam_DeXuat>(id, trans);
                    if (entity == null) return false;
                    entity.iKhoa = isLockOrUnLock;

                    conn.Update(entity, trans);

                    trans.Commit();
                    conn.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public IEnumerable<VDT_KHV_KeHoach5Nam_DeXuat> GetAllKeHoachTrungHanDeXuatByCondition(int iNamLamViec, int iGiaiDoanTu, int iGiaiDoanDen, Guid? iID_DonViQuanLyID)
        {
            try
            {
                var sql = "SELECT * from VDT_KHV_KeHoach5Nam_DeXuat khthdx where khthdx.iNamLamViec = @iNamLamViec AND khthdx.iGiaiDoanTu = @iGiaiDoanTu" +
                           " AND khthdx.iGiaiDoanDen = @iGiaiDoanDen AND khthdx.iID_DonViQuanLyID = @iID_DonViQuanLyID AND khthdx.sTongHop is not null";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDT_KHV_KeHoach5Nam_DeXuat>(sql,
                        param: new
                        {
                            iNamLamViec,
                            iGiaiDoanTu,
                            iGiaiDoanDen,
                            iID_DonViQuanLyID
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
        /// Get kế hoạch vốn ứng đề xuất chi tiét theo lstidDuAn,idKHVU
        /// </summary>
        /// <param name="idKHVU"></param>
        /// <param name="lstidDuAn"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatChiTietByIdAndListIDDuAn(Guid? id, List<Guid> lstIdDuAn)
        {
            try
            {

                var sql = FileHelpers.GetSqlQuery("vdt_khvu_dx_chiTiet_theo_idkhvu_dx.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@iId", id);
                    lstParam.Add("@listIdDuAn", string.Join(",", lstIdDuAn.Select(n => n.ToString())));


                    var items = conn.Query<VdtKhcKeHoachVonUngDeXuatChiTietModel>(sql, lstParam, commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);

            }
            return null;
        }

        public bool SaveKHVNCT(List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listData, bool isUpdate, Guid iID_KeHoachUngID, bool isDieuChinh = false)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var listDataChiTiet = new List<VDT_KHV_KeHoachVonUng_DX_ChiTiet>();
                    var listDataChiTietIsDelete = new List<VDT_KHV_KeHoachVonUng_DX_ChiTiet>();
                    var entityKHV = conn.Get<VDT_KHV_KeHoachVonUng_DX>(iID_KeHoachUngID, trans);
                    if (entityKHV == null) return false;
                    entityKHV.fGiaTriUng = 0;

                    #region Chứng từ điều chỉnh
                    if (isDieuChinh)
                    {
                        if (listData.Any())
                        {
                            foreach (var item in listData)
                            {
                                if (item.isDelete == false)
                                {
                                    var data = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                                    data.Id = Guid.NewGuid();
                                    data.iID_DuAnID = item.iID_DuAnID;
                                    if (item.fGiaTriDeNghi == null && item.fGiaTriDeNghiDC.HasValue)
                                    {
                                        data.fGiaTriDeNghi = item.fGiaTriDeNghiDC;
                                        data.fGiaTriDeNghiDC = item.fGiaTriDeNghiDC;

                                    }
                                    else
                                    {
                                        data.fGiaTriDeNghi = item.fGiaTriDeNghi;
                                        data.fGiaTriDeNghiDC = item.fGiaTriDeNghiDC;
                                    }

                                    data.iID_KeHoachUngID = iID_KeHoachUngID;
                                    data.sGhiChu = item.sGhiChu;
                                    listDataChiTiet.Add(data);
                                }

                            }
                            //listDataChiTiet.MapFrom(listData.Where(x => x.isDelete = false));
                            conn.Insert<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(listDataChiTiet, trans);
                            // update fGiatriUng to KHVUngDeXuat
                            entityKHV.fGiaTriUng = listDataChiTiet.Sum(x => x.fGiaTriDeNghiDC);
                            conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityKHV, trans);
                        }
                    }
                    #endregion
                    else
                    {
                        #region Thêm mới chi tiết.
                        if (!isUpdate)
                        {
                            if (listData.Any())
                            {
                                foreach (var item in listData)
                                {
                                    if (item.isDelete == false)
                                    {
                                        var data = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                                        data.Id = Guid.NewGuid();
                                        data.iID_DuAnID = item.iID_DuAnID;
                                        data.fGiaTriDeNghi = item.fGiaTriDeNghi;
                                        data.iID_KeHoachUngID = iID_KeHoachUngID;
                                        data.sGhiChu = item.sGhiChu;
                                        listDataChiTiet.Add(data);
                                    }

                                }
                                //listDataChiTiet.MapFrom(listData.Where(x => x.isDelete = false));
                                conn.Insert<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(listDataChiTiet, trans);
                                // update fGiatriUng to KHVUngDeXuat
                                entityKHV.fGiaTriUng = listDataChiTiet.Sum(x => x.fGiaTriDeNghi);
                                conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityKHV, trans);

                            }
                        }
                        #endregion

                        #region update chi tiết
                        else
                        {
                            // update dieu chinh
                            if (entityKHV != null && entityKHV.bIsGoc == false)
                            {
                                if (listData.Any())
                                {
                                    //listDataChiTiet.MapFrom(listData.Where(x => x.isDelete = false));
                                    //listDataChiTietIsDelete.MapFrom(listData.Where(x => x.isDelete = true));

                                    // save nhung dataChiTiet
                                    foreach (var item in listData)
                                    {
                                        if (item.Id == new Guid())
                                        {
                                            if (item.isDelete == false)
                                            {
                                                var data = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                                                data.Id = Guid.NewGuid();
                                                data.iID_DuAnID = item.iID_DuAnID;
                                                data.fGiaTriDeNghi = item.fGiaTriDeNghi;
                                                data.iID_KeHoachUngID = iID_KeHoachUngID;
                                                data.sGhiChu = item.sGhiChu;
                                                listDataChiTiet.Add(data);
                                                entityKHV.fGiaTriUng += item.fGiaTriDeNghi;
                                            }
                                            conn.Insert<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(listDataChiTiet, trans);
                                        }
                                        else
                                        {
                                            if (item.isDelete == true)
                                            {
                                                var entityDelete = conn.Get<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(item.Id, trans);
                                                conn.Delete<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(entityDelete, trans);
                                                //entityKHV.fGiaTriUng -= item.fGiaTriDeNghi;
                                            }
                                            else
                                            {
                                                var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(item.Id, trans);
                                                entity.fGiaTriDeNghiDC = item.fGiaTriDeNghi;
                                                entity.sGhiChu = item.sGhiChu;

                                                conn.Update<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(entity, trans);
                                                entityKHV.fGiaTriUng += item.fGiaTriDeNghi;
                                            }

                                        }
                                        conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityKHV, trans);
                                    }
                                }
                            }
                            else
                            {
                                if (listData.Any())
                                {
                                    //listDataChiTiet.MapFrom(listData.Where(x => x.isDelete = false));
                                    //listDataChiTietIsDelete.MapFrom(listData.Where(x => x.isDelete = true));

                                    // save nhung dataChiTiet
                                    foreach (var item in listData)
                                    {
                                        if (item.Id == new Guid())
                                        {
                                            if (item.isDelete == false)
                                            {
                                                var data = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                                                data.Id = Guid.NewGuid();
                                                data.iID_DuAnID = item.iID_DuAnID;
                                                data.fGiaTriDeNghi = item.fGiaTriDeNghi;
                                                data.iID_KeHoachUngID = iID_KeHoachUngID;
                                                data.sGhiChu = item.sGhiChu;
                                                listDataChiTiet.Add(data);
                                                entityKHV.fGiaTriUng += item.fGiaTriDeNghi;
                                            }
                                            conn.Insert<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(listDataChiTiet, trans);
                                        }
                                        else
                                        {
                                            if (item.isDelete == true)
                                            {
                                                var entityDelete = conn.Get<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(item.Id, trans);
                                                conn.Delete<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(entityDelete, trans);
                                                //entityKHV.fGiaTriUng -= item.fGiaTriDeNghi;
                                            }
                                            else
                                            {
                                                var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(item.Id, trans);
                                                entity.fGiaTriDeNghi = item.fGiaTriDeNghi;
                                                entity.sGhiChu = item.sGhiChu;

                                                conn.Update<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(item, trans);
                                                entityKHV.fGiaTriUng += item.fGiaTriDeNghi;
                                            }

                                        }
                                        conn.Update<VDT_KHV_KeHoachVonUng_DX>(entityKHV, trans);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public VDTKeHoachVonUngDeXuatModel GetKHVonUngDXByID(Guid? iId_KHVUDX)
        {
            try
            {
                var sql = "SELECT khvudx.*,nv.sTen as sTenNguonVon, dv.sTen as sTenDonVi  FROM VDT_KHV_KeHoachVonUng_DX khvudx" +
                    " INNER JOIN NS_NguonNganSach as nv ON khvudx.iID_NguonVonID = nv.iID_MaNguonNganSach" +
                    " INNER JOIN NS_DonVi as dv ON dv.iID_Ma = khvudx.iID_DonViQuanLyID" +
                    " WHERE khvudx.Id = @iId_KHVUDX ";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDTKeHoachVonUngDeXuatModel>(sql,
                        param: new
                        {
                            iId_KHVUDX
                        },
                        commandType: CommandType.Text);
                    return item.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;

        }

        public IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetDuAnInVonUngDeXuatByIdDonVi(string iID_MaDonViQuanLy = null)
        {
            try
            {
                var sql = "select DISTINCT da.iID_DuAnID,da.sMaDuAn, da.sTenDuAn,da.iID_DonViQuanLyID, da.iID_ChuDauTuID,da.fTongMucDauTu,da.sTrangThaiDuAn as sTrangThaiDuAnDangKy from  VDT_DA_DuAn da " +
                    " inner join VDT_DA_ChuTruongDauTu ctdt on ctdt.iID_DuAnID = da.iID_DuAnID" +
                    " where ctdt.iID_MaDonViQuanLy = @iID_MaDonViQuanLy";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VdtKhcKeHoachVonUngDeXuatChiTietModel>(sql,
                       param: new
                       {
                           iID_MaDonViQuanLy
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

        public IEnumerable<VDT_DA_DuAn> GetListidDuAnByListMaDuAn(List<string> ListMaDuAns)
        {
            try
            {
                var sql = "select distinct da.iID_DuAnID,da.sTrangThaiDuAn,da.sMaDuAn from VDT_DA_DuAn da\r\n" +
                          "inner join VDT_DA_ChuTruongDauTu dt on dt.iID_DuAnID = da.iID_DuAnID\r\n" +
                          "where da.sMaDuAn in (select * from dbo.f_split(@ListMaDuAns))";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var results = conn.Query<VDT_DA_DuAn>(sql,
                        param: new
                        {
                            ListMaDuAns
                        },
                        commandType: CommandType.Text);
                    return results;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool KhvuImportSave(VdtKhvuDXChiTietModel data, List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listData, string userName)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var iId_khvUng = new Guid();
                    var entityParent = new VDT_KHV_KeHoachVonUng_DX();
                    entityParent.MapFrom(data);

                    entityParent.sUserCreate = userName;
                    entityParent.dDateCreate = DateTime.Now;
                    entityParent.Id = Guid.NewGuid();
                    entityParent.fGiaTriUng = listData.Sum(x => x.fGiaTriDeNghi);
                    conn.Insert<VDT_KHV_KeHoachVonUng_DX>(entityParent, trans);
                    iId_khvUng = entityParent.Id;

                    // Save KHVU chi tiet
                    var listDetailsEntity = new List<VDT_KHV_KeHoachVonUng_DX_ChiTiet>();
                    foreach (var item in listData)
                    {
                        var detailEntity = new VDT_KHV_KeHoachVonUng_DX_ChiTiet();
                        detailEntity.MapFrom(item);
                        detailEntity.iID_KeHoachUngID = iId_khvUng;
                        listDetailsEntity.Add(detailEntity);
                    }
                    conn.Insert<VDT_KHV_KeHoachVonUng_DX_ChiTiet>(listDetailsEntity, trans);
                    trans.Commit();
                    return true;
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);

            }
            return false;
        }

        public IEnumerable<VdtKhvuDXChiTietModel> GetDataReportKHVUDeXuatById(Guid? id)
        {
            try
            {
                var sql = "select ROW_NUMBER()  over( order by ct.iID_KeHoachUngID ) as sSTT, ct.*,da.sMaDuAn, dv.sTen as sTenDonVi,nns.sTen as sTenNguonVon,da.sTenDuAn from VDT_KHV_KeHoachVonUng_DX khvu\r\n" +
                          "left join VDT_KHV_KeHoachVonUng_DX_ChiTiet ct on ct.iID_KeHoachUngID = khvu.Id\r\n" +
                          "inner join NS_NguonNganSach nns on nns.iID_MaNguonNganSach = khvu.iID_NguonVonID\r\n" +
                          "inner join NS_DonVi dv on dv.iID_Ma = khvu.iID_DonViQuanLyID\r\n" +
                          "inner join VDT_DA_DuAn da on da.iID_DuAnID = ct.iID_DuAnID\r\n" +
                          "where khvu.Id = @id";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var results = conn.Query<VdtKhvuDXChiTietModel>(sql,
                        param: new
                        {
                            id
                        },
                        commandType: CommandType.Text);
                    return results;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }



        #endregion


        #region Kế hoạch vốn ứng được duyệt

        /// <summary>
        /// Get kế hoạch vốn ứng được duyệt chi tiét theo lstidDuAn,idKHVU
        /// </summary>
        /// <param name="idKHVU"></param>
        /// <param name="lstidDuAn"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<VdtKhvKeHoachVonUngChiTietModel> GetKHVUDuocDuyetChiTietByIdAndListDuAnIds(Guid? iID_KeHoachUngID, List<Guid> listDuaAnIds, Guid? iId_KHVUDX_ID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@iID_KeHoachVDuocDuyetId", iID_KeHoachUngID);
                    lstParam.Add("@iId_KHVUDX_ID", iId_KHVUDX_ID);
                    lstParam.Add("@listDuAnIds", string.Join(",", listDuaAnIds.Select(n => n.ToString())));

                    var items = conn.Query<VdtKhvKeHoachVonUngChiTietModel>("proc_vdt_khv_getdatakhvuduocduyet_chitiet", lstParam, commandType: CommandType.StoredProcedure);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<VDTKeHoachVonUngDeXuatModel> GetKeHoachVonUngDeXuatTongHopByListKhvIds(List<Guid> listKhvIds)
        {
            try
            {

                var sql = FileHelpers.GetSqlQuery("vdt_khv_getdata_kehoach_von_ung_tonghop_by_listkhvungids.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@listKhvIds", string.Join(",", listKhvIds.Select(n => n.ToString())));

                    var items = conn.Query<VDTKeHoachVonUngDeXuatModel>(sql, lstParam, commandType: CommandType.Text);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public VDTKhvkeHoachVonUngViewModel GetKHVUDuocDuyetById(Guid? iID_KeHoachUngID)
        {
            try
            {
                var sql = "select khvu.*, dv.sTen as sTenDonViQuanLy, nns.sTen as sTenNguonVon,khvu_dx.sSoDeNghi as sSoDeNghi_KHVUDX " +
                    " FROM VDT_KHV_KeHoachVonUng khvu" +
                    " INNER JOIN NS_DonVi dv on khvu.iID_DonViQuanLyID = dv.iID_Ma" +
                    " INNER JOIN NS_NguonNganSach nns on khvu.iID_NguonVonID = nns.iID_MaNguonNganSach" +
                    " INNER JOIN VDT_KHV_KeHoachVonUng_DX khvu_dx ON khvu_dx.Id = khvu.iID_KeHoachVonUngDeXuatID" +
                    " where khvu.Id = @iID_KeHoachUngID";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDTKhvkeHoachVonUngViewModel>(sql,
                        param: new
                        {
                            iID_KeHoachUngID
                        },
                        commandType: CommandType.Text);
                    return item.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);

            }
            return null;

        }

        public IEnumerable<VdtKhcKeHoachVonUngDeXuatChiTietModel> GetKeHoachVonUngDeXuatChiTietTongHopById(Guid? id)
        {
            try
            {
                var sql = "SELECT khvuct.*, da.sMaDuAn as sMaDuAn, da.sTenDuAn as sTenDuAn, da.fTongMucDauTu as fTongMucDauTu\r\n\t\t" +
                    "FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet khvuct\r\n\t\t" +
                    "INNER JOIN VDT_DA_DuAn da on da.iID_DuAnID = khvuct.iID_DuAnID\r\n\t\t" +
                    "WHERE khvuct.iID_KeHoachUngID in (select * from dbo.f_split(@sTonghop))";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var entity = conn.Get<VDT_KHV_KeHoachVonUng_DX>(id);
                    var sTongHop = entity.sTongHop;
                    var item = conn.Query<VdtKhcKeHoachVonUngDeXuatChiTietModel>(sql,
                        param: new
                        {
                            sTongHop

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

        public Guid CheckExistXauNoiMaAndGetIdNganh(int iNamLamViec, string sXauNoiMa)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE iNamLamViec = {0} AND sXauNoiMa = '{1}'", iNamLamViec, sXauNoiMa);
            using (var conn = _connectionFactory.GetConnection())
            {
                Guid iId_NganhID = conn.Query<Guid>(query.ToString()).FirstOrDefault();
                return iId_NganhID;
            }
        }
        public bool CheckDuplicateXauNoiMa(int iNamLamViec, string sXauNoiMa)
        {
            try
            {
                var sql = "select count(*) from VDT_KHV_KeHoachVonUng_ChiTiet where (Id is null or Id != @id) and iID_NganhID = @iiID_NganhID";
                using (var conn = _connectionFactory.GetConnection())
                {
                    int count = conn.Query<int>(sql).FirstOrDefault();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool KehoachVonUngDuocDuyetCtSave(List<VdtKhvKeHoachVonUngChiTietModel> listData, bool isUpdate, Guid? iId_KeHoachUngID, bool isDieuChinh = false)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var listDataChiTiet = new List<VDT_KHV_KeHoachVonUng_ChiTiet>();
                    var entityParent = new VDT_KHV_KeHoachVonUng();
                    if ((listData != null && listData.Count() > 0) && iId_KeHoachUngID != null)
                    {
                        entityParent = conn.Get<VDT_KHV_KeHoachVonUng>(iId_KeHoachUngID, trans);

                        if (isDieuChinh)
                        {
                            entityParent.fGiaTriUng = listData.Where(x => x.isDelete == false).Sum(x => x.fCapPhatBangLenhChiDC) + listData.Where(x => x.isDelete == false).Sum(x => x.fCapPhatTaiKhoBacDC);
                        }
                        else
                        {
                            entityParent.fGiaTriUng = listData.Where(x => x.isDelete == false).Sum(x => x.fCapPhatBangLenhChi) + listData.Where(x => x.isDelete == false).Sum(x => x.fCapPhatTaiKhoBac);
                        }
                        conn.Update<VDT_KHV_KeHoachVonUng>(entityParent, trans);
                    }

                    #region Dieu chinh ke hoach von ung duoc duyet 
                    if (isDieuChinh)
                    {
                        if (listData.Any())
                        {
                            foreach (var item in listData)
                            {
                                if (item.isDelete == false)
                                {
                                    var entity = new VDT_KHV_KeHoachVonUng_ChiTiet();
                                    entity.Id = Guid.NewGuid();
                                    entity.iID_DuAnID = item.iID_DuAnID;
                                    entity.iID_MucID = item.iID_NganhID;
                                    entity.iID_TietMucID = item.iID_NganhID;
                                    entity.iID_TieuMucID = item.iID_NganhID;
                                    entity.iID_NganhID = item.iID_NganhID;
                                    entity.iID_KeHoachUngID = item.iID_KeHoachUngID;
                                    entity.fCapPhatTaiKhoBacDC = item.fCapPhatTaiKhoBacDC;
                                    entity.fCapPhatBangLenhChiDC = item.fCapPhatBangLenhChiDC;
                                    entity.fCapPhatTaiKhoBac = item.fCapPhatTaiKhoBac;
                                    entity.fCapPhatBangLenhChi = item.fCapPhatBangLenhChi;
                                    entity.LNS = item.sLNS;
                                    entity.L = item.sL;
                                    entity.K = item.sK;
                                    entity.M = item.sM;
                                    entity.TM = item.sTM;
                                    entity.TTM = item.sTTM;
                                    entity.NG = item.sNG;
                                    entity.sGhiChu = item.sGhiChu;
                                    entity.sTrangThaiDuAnDangKy = item.sTrangThaiDuAnDangKy;
                                    listDataChiTiet.Add(entity);
                                }
                            }

                            //Save dataChiTiet
                            conn.Insert<VDT_KHV_KeHoachVonUng_ChiTiet>(listDataChiTiet, trans);

                        }
                    }
                    #endregion
                    else
                    {
                        if (isUpdate)
                        {
                            #region sua dieu chinh ke hoach von ung duoc duyet chi tiet
                            if (entityParent != null && entityParent.bIsGoc == false)
                            {
                                foreach (var item in listData)
                                {
                                    if (item.id == new Guid())
                                    {
                                        if (item.isDelete == false)
                                        {
                                            var entity = new VDT_KHV_KeHoachVonUng_ChiTiet();
                                            entity.Id = Guid.NewGuid();
                                            entity.iID_DuAnID = item.iID_DuAnID;
                                            entity.iID_MucID = item.iID_NganhID;
                                            entity.iID_TietMucID = item.iID_NganhID;
                                            entity.iID_TieuMucID = item.iID_NganhID;
                                            entity.iID_NganhID = item.iID_NganhID;
                                            entity.iID_KeHoachUngID = item.iID_KeHoachUngID;
                                            entity.fCapPhatTaiKhoBacDC = item.fCapPhatTaiKhoBac;
                                            entity.fCapPhatBangLenhChiDC = item.fCapPhatBangLenhChi;
                                            entity.LNS = item.sLNS;
                                            entity.L = item.sL;
                                            entity.K = item.sK;
                                            entity.M = item.sM;
                                            entity.TM = item.sTM;
                                            entity.TTM = item.sTTM;
                                            entity.NG = item.sNG;
                                            entity.sGhiChu = item.sGhiChu;
                                            entity.sTrangThaiDuAnDangKy = item.sTrangThaiDuAnDangKy;
                                            listDataChiTiet.Add(entity);
                                        }
                                        conn.Insert<VDT_KHV_KeHoachVonUng_ChiTiet>(listDataChiTiet, trans);

                                    }
                                    else
                                    {
                                        if (item.isDelete == true)
                                        {
                                            conn.Delete<VDT_KHV_KeHoachVonUng_ChiTiet>(item.id, trans);
                                        }
                                        else
                                        {
                                            var entityOld = conn.Get<VDT_KHV_KeHoachVonUng_ChiTiet>(item.id, trans);
                                            entityOld.iID_MucID = item.iID_NganhID;
                                            entityOld.iID_TietMucID = item.iID_NganhID;
                                            entityOld.iID_TieuMucID = item.iID_NganhID;
                                            entityOld.iID_NganhID = item.iID_NganhID;
                                            entityOld.fCapPhatTaiKhoBacDC = item.fCapPhatTaiKhoBac;
                                            entityOld.fCapPhatBangLenhChiDC = item.fCapPhatBangLenhChi;
                                            entityOld.LNS = item.sLNS;
                                            entityOld.L = item.sL;
                                            entityOld.K = item.sK;
                                            entityOld.M = item.sM;
                                            entityOld.TM = item.sTM;
                                            entityOld.TTM = item.sTTM;
                                            entityOld.NG = item.sNG;
                                            entityOld.sGhiChu = item.sGhiChu;
                                            conn.Update<VDT_KHV_KeHoachVonUng_ChiTiet>(entityOld, trans);

                                        }
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                                #region sua ke hoach von ung duoc duyet chi tiet
                                foreach (var item in listData)
                                {
                                    if (item.id == new Guid())
                                    {
                                        if (item.isDelete == false)
                                        {
                                            var entity = new VDT_KHV_KeHoachVonUng_ChiTiet();
                                            entity.Id = Guid.NewGuid();
                                            entity.iID_DuAnID = item.iID_DuAnID;
                                            entity.iID_MucID = item.iID_NganhID;
                                            entity.iID_TietMucID = item.iID_NganhID;
                                            entity.iID_TieuMucID = item.iID_NganhID;
                                            entity.iID_NganhID = item.iID_NganhID;
                                            entity.iID_KeHoachUngID = item.iID_KeHoachUngID;
                                            entity.fCapPhatTaiKhoBac = item.fCapPhatTaiKhoBac;
                                            entity.fCapPhatBangLenhChi = item.fCapPhatBangLenhChi;
                                            entity.LNS = item.sLNS;
                                            entity.L = item.sL;
                                            entity.K = item.sK;
                                            entity.M = item.sM;
                                            entity.TM = item.sTM;
                                            entity.TTM = item.sTTM;
                                            entity.NG = item.sNG;
                                            entity.sGhiChu = item.sGhiChu;
                                            entity.sTrangThaiDuAnDangKy = item.sTrangThaiDuAnDangKy;
                                            listDataChiTiet.Add(entity);
                                        }
                                        conn.Insert<VDT_KHV_KeHoachVonUng_ChiTiet>(listDataChiTiet, trans);

                                    }
                                    else
                                    {
                                        if (item.isDelete == true)
                                        {
                                            conn.Delete<VDT_KHV_KeHoachVonUng_ChiTiet>(item.id, trans);
                                        }
                                        else
                                        {
                                            var entityOld = conn.Get<VDT_KHV_KeHoachVonUng_ChiTiet>(item.id, trans);
                                            entityOld.iID_MucID = item.iID_NganhID;
                                            entityOld.iID_TietMucID = item.iID_NganhID;
                                            entityOld.iID_TieuMucID = item.iID_NganhID;
                                            entityOld.iID_NganhID = item.iID_NganhID;
                                            entityOld.fCapPhatTaiKhoBac = item.fCapPhatTaiKhoBac;
                                            entityOld.fCapPhatBangLenhChi = item.fCapPhatBangLenhChi;
                                            entityOld.LNS = item.sLNS;
                                            entityOld.L = item.sL;
                                            entityOld.K = item.sK;
                                            entityOld.M = item.sM;
                                            entityOld.TM = item.sTM;
                                            entityOld.TTM = item.sTTM;
                                            entityOld.NG = item.sNG;
                                            entityOld.sGhiChu = item.sGhiChu;
                                            conn.Update<VDT_KHV_KeHoachVonUng_ChiTiet>(entityOld, trans);

                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region Them moi ke hoach von ung duoc duyet chi tiet
                            if (listData.Any())
                            {
                                foreach (var item in listData)
                                {
                                    if (item.isDelete == false)
                                    {
                                        var entity = new VDT_KHV_KeHoachVonUng_ChiTiet();
                                        entity.Id = Guid.NewGuid();
                                        entity.iID_DuAnID = item.iID_DuAnID;
                                        entity.iID_MucID = item.iID_NganhID;
                                        entity.iID_TietMucID = item.iID_NganhID;
                                        entity.iID_TieuMucID = item.iID_NganhID;
                                        entity.iID_NganhID = item.iID_NganhID;
                                        entity.iID_KeHoachUngID = item.iID_KeHoachUngID;
                                        entity.fCapPhatTaiKhoBac = item.fCapPhatTaiKhoBac;
                                        entity.fCapPhatBangLenhChi = item.fCapPhatBangLenhChi;
                                        entity.LNS = item.sLNS;
                                        entity.L = item.sL;
                                        entity.K = item.sK;
                                        entity.M = item.sM;
                                        entity.TM = item.sTM;
                                        entity.TTM = item.sTTM;
                                        entity.NG = item.sNG;
                                        entity.sGhiChu = item.sGhiChu;
                                        entity.sTrangThaiDuAnDangKy = item.sTrangThaiDuAnDangKy;
                                        listDataChiTiet.Add(entity);
                                    }
                                }

                                //Save dataChiTiet
                                conn.Insert<VDT_KHV_KeHoachVonUng_ChiTiet>(listDataChiTiet, trans);
                            }
                            #endregion
                        }
                    }
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public IEnumerable<VdtKhvKeHoachVonUngChiTietModel> KehoachVonUngDuocDuyetChiTietExport(Guid? iIDKeHoachVonUngID)
        {
            try
            {
                var sql = "select  ROW_NUMBER()  over( order by ct.iID_KeHoachUngID ) as STT, da.sTenDuAn, da.sMaDuAn, ct.LNS as sLNS, ct.L as SL, ct.K as sK, ct.M as sM, ct.TM as sTM, ct.TTM as sTTM, ct.NG as sNG,\r\n" +
                    "qddt.fTongMucDauTuPheDuyet as fTongMucDauTu,dx.fGiaTriDeNghi, ct.fCapPhatTaiKhoBac, ct.fCapPhatBangLenhChi,ct.sGhiChu, qddt.bActive\r\n" +
                    "from VDT_KHV_KeHoachVonUng_ChiTiet ct\r\n" +
                    "inner join VDT_KHV_KeHoachVonUng khv on khv.Id = ct.iID_KeHoachUngID\r\n" +
                    "inner join VDT_KHV_KeHoachVonUng_DX_ChiTiet dx on dx.iID_KeHoachUngID = khv.iID_KeHoachVonUngDeXuatID\r\n" +
                    "left join VDT_DA_DuAn da on da.iID_DuAnID = ct.iID_DuAnID\r\n" +
                    "inner join VDT_DA_QDDauTu  qddt on qddt.iID_DuAnID = da.iID_DuAnID\r\n" +
                    "where ct.iID_KeHoachUngID = @iIDKeHoachVonUngID and qddt.bActive = 1";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VdtKhvKeHoachVonUngChiTietModel>(sql,
                        param: new
                        {
                            iIDKeHoachVonUngID
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




        #endregion

        #region Luu bang tong hop
        public void InsertTongHopNguonDauTu_Tang(string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("@sLoai", sLoai);
                    lstParam.Add("@iTypeExecute", iTypeExecute);
                    lstParam.Add("@uIdQuyetDinh", iIdQuyetDinh);
                    lstParam.Add("@iIDQuyetDinhOld", iIDQuyetDinhOld);

                    var items = conn.Execute("sp_insert_tonghopnguonnsdautu_tang", lstParam, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private TongHopNguonNSDauTuQuery GetMucLucNganSachByPhanBoVon(VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet item, Dictionary<string, Guid> _dicMucLucNganSach)
        {
            TongHopNguonNSDauTuQuery data = new TongHopNguonNSDauTuQuery();
            string sKey = item.LNS + "-" + item.L + "-" + item.K + "-" + item.M + "-" + item.TM + "-" + item.TTM + "-" + item.NG;
            if (_dicMucLucNganSach.ContainsKey(sKey))
            {
                if (!string.IsNullOrEmpty(item.NG))
                {
                    data.IIDNganhID = _dicMucLucNganSach[sKey];
                }
                else if (!string.IsNullOrEmpty(item.TTM))
                {
                    data.IIDTietMucID = _dicMucLucNganSach[sKey];
                }
                else if (!string.IsNullOrEmpty(item.TM))
                {
                    data.IIDTieuMucID = _dicMucLucNganSach[sKey];
                }
                else if (!string.IsNullOrEmpty(item.M))
                {
                    data.IIDMucID = _dicMucLucNganSach[sKey];
                }
            }
            return data;
        }

        private void InsertTongHopNguonDauTu_KHVN_Giam(int iNamKeHoach, string sLoai, int iTypeExecute, Guid iIdQuyetDinh, List<TongHopNguonNSDauTuQuery> lstChungTu)
        {
            if (lstChungTu == null || lstChungTu.Count == 0) return;
            List<TongHopNguonNSDauTuQuery> lstChungTuUpdate = new List<TongHopNguonNSDauTuQuery>();
            List<TongHopNguonNSDauTuQuery> lstNguonVon = GetNguonVonTongHopNguonDauTuKHVN(iNamKeHoach);
            Dictionary<Guid, double> dicNguonVon = new Dictionary<Guid, double>();

            foreach (var objChungTu in lstChungTu)
            {
                var lstIdRemove = new List<Guid>();
                double fThanhToan = objChungTu.fGiaTri ?? 0;
                foreach (var objNguonVon in lstNguonVon)
                {
                    if (!dicNguonVon.ContainsKey(objNguonVon.Id))
                        dicNguonVon.Add(objNguonVon.Id, objNguonVon.fGiaTri ?? 0);

                    if (objNguonVon.iID_DuAnID == objChungTu.iID_DuAnID && objNguonVon.sMaNguon == objChungTu.sMaDich)
                    {
                        if (fThanhToan >= dicNguonVon[objNguonVon.Id])
                        {
                            lstChungTuUpdate.Add(new TongHopNguonNSDauTuQuery()
                            {
                                fGiaTri = dicNguonVon[objNguonVon.Id],
                                iID_ChungTu = objChungTu.iID_ChungTu,
                                iID_DuAnID = objChungTu.iID_DuAnID,
                                sMaDich = objChungTu.sMaDich,
                                sMaNguon = objChungTu.sMaNguon,
                                sMaNguonCha = objChungTu.sMaNguonCha,
                                iId_MaNguonCha = objNguonVon.iID_ChungTu,
                                iStatus = (int)NguonVonStatus.DaSuDung,
                                IIDMucID = objChungTu.IIDMucID,
                                IIDTieuMucID = objChungTu.IIDTieuMucID,
                                IIDTietMucID = objChungTu.IIDTietMucID,
                                IIDNganhID = objChungTu.IIDNganhID
                            });
                            fThanhToan = fThanhToan - dicNguonVon[objNguonVon.Id];
                            objNguonVon.iStatus = (int)NguonVonStatus.DaSuDung;
                            lstIdRemove.Add(objNguonVon.Id);
                        }
                        else
                        {
                            lstChungTuUpdate.Add(new TongHopNguonNSDauTuQuery()
                            {
                                fGiaTri = fThanhToan,
                                iID_ChungTu = objChungTu.iID_ChungTu,
                                iID_DuAnID = objChungTu.iID_DuAnID,
                                sMaDich = objChungTu.sMaDich,
                                sMaNguon = objChungTu.sMaNguon,
                                sMaNguonCha = objChungTu.sMaNguonCha,
                                iId_MaNguonCha = objNguonVon.iID_ChungTu,
                                iStatus = (int)NguonVonStatus.DaSuDung,
                                IIDMucID = objChungTu.IIDMucID,
                                IIDTieuMucID = objChungTu.IIDTieuMucID,
                                IIDTietMucID = objChungTu.IIDTietMucID,
                                IIDNganhID = objChungTu.IIDNganhID
                            });
                            dicNguonVon[objNguonVon.Id] = dicNguonVon[objNguonVon.Id] - fThanhToan;
                            break;
                        }
                    }
                }
                lstNguonVon = lstNguonVon.Where(n => !lstIdRemove.Contains(n.Id)).ToList();
            }

            if (lstChungTuUpdate.Count != 0)
            {
                InsertTongHopNguonDauTu(iIdQuyetDinh, sLoai, lstChungTuUpdate);
            }
        }

        private List<TongHopNguonNSDauTuQuery> GetNguonVonTongHopNguonDauTuKHVN(int iNamKeHoach)
        {

            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("@iNamKeHoach", @iNamKeHoach);
                var items = conn.Query<TongHopNguonNSDauTuQuery>("sp_get_nguonvon_tonghopnguondautu_khvn", lstParam, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }
        #endregion

        #region Báo cáo quyết toán dự án hoàn thành
        /// <summary>
        /// get list du toan nguon von by du an
        /// </summary>
        /// <param name="iIdDuAnId"></param>
        /// <returns></returns>
        public List<VDTDuToanNguonVonModel> GetListDuToanNguonVonByDuAn(string iIdDuAnId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_dutoan_nguonvon_by_duan.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDTDuToanNguonVonModel>(sql,
                    param: new
                    {
                        iIdDuAnId
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

        public List<VDTDuToanNguonVonModel> GetListDuToanNguonVonByDuToanId(string iIdDuToanId)
        {
            try
            {
                var sql = FileHelpers.GetSqlQuery("vdt_get_dutoan_nguonvon_by_dutoanid.sql");

                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<VDTDuToanNguonVonModel>(sql,
                    param: new
                    {
                        iIdDuToanId
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

        public VDT_DA_DuToan GetDuToanIdByDuAnId(Guid iID_DuAnID)
        {
            var sql = "select * from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID and bActive = 1";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_DuToan>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text).FirstOrDefault();

                return item;
            }
        }

        public IEnumerable<VDT_DA_DuToan> GetAllDuToanIdByDuAnId(Guid iID_DuAnID)
        {
            var sql = "select * from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID and bActive = 1 order by dDateCreate desc";

            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<VDT_DA_DuToan>(sql,
                    param: new
                    {
                        iID_DuAnID
                    },
                    commandType: CommandType.Text);

                return item;
            }
        }

        public List<VDT_QT_DeNghiQuyetToan_ChiTiet> GetDeNghiQuyetToanChiTiet(Guid iIdDeNghiQuyetToanId)
        {
            var sql = "select * from VDT_QT_DeNghiQuyetToan_ChiTiet where iID_DeNghiQuyetToanID = @iIdDeNghiQuyetToanId";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_DeNghiQuyetToan_ChiTiet>(sql,
                    param: new
                    {
                        iIdDeNghiQuyetToanId
                    },
                    commandType: CommandType.Text).ToList();

                return items;
            }
        }

        public IEnumerable<VDT_QT_DeNghiQuyetToan> GetDeNghiQuyetToan(string sUserName = "")
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_denghithanhtoan_by_user_and_namlamviec.sql");
            var config = _ngansachService.GetCauHinh(sUserName);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_DeNghiQuyetToan>(sql,
                param: new
                {
                    sUserName,
                    config.iNamLamViec
                },
                commandType: CommandType.Text);
                return items.ToList();
            }
        }
        public IEnumerable<VDT_QT_DeNghiQuyetToanViewModel> FindDeNghiQuyetToan(string userName = "")
        {
            try
            {
                var config = _ngansachService.GetCauHinh(userName);
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("NamLamViec", config.iNamLamViec);
                    lstParam.Add("UserName", userName);

                    var items = conn.Query<VDT_QT_DeNghiQuyetToanViewModel>("sp_vdt_dnqt_get_denghiquyetoan", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public double GetGiaTriDuToanIdByDuAnId(Guid? duanId)
        {
            try
            {
                var sql = "select * from VDT_DA_DuToan where iID_DuAnID = @duanId and bActive = 1";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.Query<VDT_DA_DuToan>(sql,
                        param: new
                        {
                            duanId,
                        }, commandType: CommandType.Text).ToList();
                    if (item != null)
                    {
                        return item.Sum(n => n.fTongDuToanPheDuyet.HasValue ? n.fTongDuToanPheDuyet.Value : 0);
                    }
                    else
                    {
                        var itemTongDuToan = conn.Query<VDT_DA_DuToan>(sql,
                            param: new
                            {
                                duanId,
                            }, commandType: CommandType.Text).FirstOrDefault();
                        if (itemTongDuToan != null)
                        {
                            return itemTongDuToan.fTongDuToanPheDuyet.HasValue ? itemTongDuToan.fTongDuToanPheDuyet.Value : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return 0;
        }

        public List<NguonVonQuyetToanKeHoachQuery> GetNguonVonByDuToanId(string duToanId)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_get_nguonvon_by_iddutoan.sql");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NguonVonQuyetToanKeHoachQuery>(sql,
                param: new
                {
                    duToanId,
                },
                commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public List<VdtDaDuToanChiPhiDataQuery> FindListDuToanChiPhiByDuAn(Guid? duanId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("duAnId", duanId);

                    var items = conn.Query<VdtDaDuToanChiPhiDataQuery>("sp_vdt_get_all_dutoan_chiphi_by_duan_quyettoanchitiet", lstParam, commandType: CommandType.StoredProcedure).ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public List<VDT_QT_DeNghiQuyetToan_ChiTiet> FindByDeNghiQuyetToanId(Guid? deNghiQuyetToanId)
        {
            var sql = "select * from VDT_QT_DeNghiQuyetToan_ChiTiet where iID_DeNghiQuyetToanID = @deNghiQuyetToanId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_DeNghiQuyetToan_ChiTiet>(sql,
                        param: new
                        {
                            deNghiQuyetToanId,
                        }, commandType: CommandType.Text).ToList();
                return items;
            }
        }

        public List<VDT_QT_QuyetToan_ChiTiet> GetQuyetToanChiTiet(Guid? iID_QuyetToanID)
        {
            var sql = "select * from VDT_QT_QuyetToan_ChiTiet where iID_QuyetToanID = @iID_QuyetToanID";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_QT_QuyetToan_ChiTiet>(sql,
                    param: new
                    {
                        iID_QuyetToanID
                    },
                    commandType: CommandType.Text).ToList();

                return items;
            }
        }

        public IEnumerable<VDTQLPheDuyetQuyetToanViewModel> GetDataExportQuyetToan(Guid? iID_QuyetToanID)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iID_QuyetToanID", iID_QuyetToanID);
                var items = conn.Query<VDTQLPheDuyetQuyetToanViewModel>("sp_vdt_quyettoan_export", lstParam, commandType: CommandType.StoredProcedure);

                return items;
            }
        }
        public IEnumerable<VDTNguonVonDauTuViewModel> GetDataChiPhiExportQuyetToan(Guid? iID_QuyetToanID)
        {
            int count = 1;
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iID_QuyetToanID", iID_QuyetToanID);
                var items = conn.Query<VDTNguonVonDauTuViewModel>("sp_vdt_quyettoan_export", lstParam, commandType: CommandType.StoredProcedure);
                foreach (var item in items)
                {
                    item.Stt = count;
                    count++;
                }

                return items;
            }
        }
        #endregion

        #region Trao đổi dữ liệu

        /// <summary>
        /// get danh sach trao đổi dữ liệu theo dieu kien
        /// </summary>
        /// <param name="_paging"></param>
        /// <param name="sSoChungTu"></param>       
        /// <param name="dNgayChungtu"></param>
        /// <param name="sMaDonVi"></param>
        /// <param name="iNamKeHoach"></param>
        /// <param name="sLoaiTraoDoi"></param>
        /// <param name="TrangThai"></param>
        /// <returns></returns>
        public IEnumerable<VDT_NS_TraoDoiDuLieuViewModel> GetAllTraoDoiDuLieuPaging(ref PagingInfo _paging, string sSoChungTu = "", int? iNamKeHoach = null, DateTime? dNgayChungtu = null,
            string sMaDonVi = null, int? sLoaiTraoDoi = null, int? TrangThai = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var lstParam = new DynamicParameters();
                    lstParam.Add("sSoChungTu", sSoChungTu);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("dNgayChungtu", dNgayChungtu);
                    lstParam.Add("sMaDonVi", sMaDonVi);
                    lstParam.Add("sLoaiTraoDoi", sLoaiTraoDoi);
                    lstParam.Add("TrangThai", TrangThai);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<VDT_NS_TraoDoiDuLieuViewModel>("proc_get_all_traodoidulieu_paging", lstParam, commandType: CommandType.StoredProcedure);
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
        #endregion

        public bool CheckExistSoChungTuTraoDoiDL(string sSoChungTu, int iNamLamViec, Guid? id)
        {
            var sql = FileHelpers.GetSqlQuery("vdt_check_exist_sochungtu_tddl.sql");

            using (var conn = _connectionFactory.GetConnection())
            {
                var countId = conn.QueryFirst<int>(sql,
                    param: new
                    {
                        sSoChungTu,
                        id,
                        iNamLamViec
                    },
                    commandType: CommandType.Text);

                return countId == 0 ? false : true;
            }

        }

        public bool SaveTraoDoiDuDLieu(ref VDT_NS_TraoDoiDuLieuViewModel data, string sUserLogin, int iNamLamViec, bool isUpdate, List<VDT_NS_TraoDoiDuLieu_ChiTiet> lstDataChiTiet)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                if (data.ID == null || data.ID == Guid.Empty)
                {
                    var entity = new VDT_NS_Traodoidulieu();
                    entity.MapFrom(data);
                    entity.iNamLamViec = iNamLamViec;
                    entity.sSoChungTu = data.sSoChungTu;
                    entity.dNgayChungTu = data.dNgayChungTu;
                    entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                    entity.iThoiGian = data.iThoiGian;
                    entity.iID_NguonVonID = data.iID_NguonVonID;
                    entity.iLoaiTraoDoi = data.iLoaiTraoDoi;
                    entity.iLoaiThongtri = data.iLoaiThongtri;
                    entity.iLoaiDuToan = data.iLoaiDuToan;
                    entity.iTrangThai = 1;
                    entity.dNgayTao = DateTime.Now;
                    entity.sNguoiTao = sUserLogin;

                    if (lstDataChiTiet.Any())
                    {
                        entity.fSoTien = lstDataChiTiet.Sum(x => x.fSoTien);
                    }

                    conn.Insert(entity, trans);

                    data.ID = entity.ID;
                }
                else
                {
                    if (isUpdate)
                    {
                        var entity = conn.Get<VDT_NS_Traodoidulieu>(data.ID, trans);
                        if (entity == null)
                            return false;
                        entity.iID_DonViQuanLyID = data.iID_DonViQuanLyID;
                        entity.iID_MaDonVi = data.iID_MaDonVi;
                        entity.sSoChungTu = data.sSoChungTu;
                        entity.dNgayChungTu = data.dNgayChungTu;
                        entity.iThoiGian = data.iThoiGian;
                        entity.iID_NguonVonID = data.iID_NguonVonID;
                        entity.iLoaiDuToan = data.iLoaiDuToan;
                        entity.iLoaiThongtri = data.iLoaiThongtri;
                        entity.iLoaiTraoDoi = data.iLoaiTraoDoi;
                        entity.iNamLamViec = data.iNamLamViec;
                        entity.sNguoiSua = sUserLogin;
                        entity.dNgaySua = DateTime.Now;
                        if (lstDataChiTiet.Any())
                        {
                            entity.fSoTien = lstDataChiTiet.Sum(x => x.fSoTien);
                        }

                        conn.Update(entity, trans);
                    }
                }

                #region Save data chi tiet

                if (lstDataChiTiet.Any())
                {
                    if (!isUpdate)
                    {

                        foreach (var item in lstDataChiTiet)
                        {
                            item.iID_TraodoiDuLieuChiTietID = new Guid();
                            item.iID_TraodoidulieuID = data.ID;
                            item.iTrangThai = 1;
                            item.dNgayTao = DateTime.Now;
                            item.sID_MaNguoiDungTao = sUserLogin;
                        }
                        conn.Insert<VDT_NS_TraoDoiDuLieu_ChiTiet>(lstDataChiTiet, trans);
                    }
                    else
                    {
                        foreach (VDT_NS_TraoDoiDuLieu_ChiTiet item in lstDataChiTiet)
                        {
                            item.dNgaySua = DateTime.Now;
                            item.sID_MaNguoiDungSua = sUserLogin;
                            item.iTrangThai = 1;
                            item.iID_TraodoidulieuID = data.ID;
                            if (item.iID_TraodoiDuLieuChiTietID == new Guid())
                            {
                                conn.Insert<VDT_NS_TraoDoiDuLieu_ChiTiet>(item, trans);
                            }
                            conn.Update<VDT_NS_TraoDoiDuLieu_ChiTiet>(item, trans);
                        }

                    }
                }
                #endregion

                // commit to db
                trans.Commit();
            }
            return true;
        }

        public IEnumerable<NS_MucLucNganSach> GetAlTraoDoiDuLieuChiTiet(int iNamLamViec)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM NS_MucLucNganSach WHERE sLNS = '1030100' AND sL = '010' AND sK = '011' AND sM = '9300' AND sTM = '9301' AND iID_MaMucLucNganSach_Cha IS NULL ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_MucLucNganSach>(query.ToString());
                return items;
            }
        }


        /// <summary>
        /// xoa nhu cau chi
        /// </summary>
        /// <param name="iID_NhuCauChiID"></param>
        /// <returns></returns>
        public bool XoaTraoDoiDuLieu(Guid iID_TraoDoiDuLieuID)
        {
            try
            {
                var sql = "DELETE VDT_NS_Traodoidulieu WHERE ID =  @iID_TraoDoiDuLieuID; ";

                using (var conn = _connectionFactory.GetConnection())
                {
                    var r = conn.Execute(
                        sql,
                        param: new
                        {
                            iID_TraoDoiDuLieuID
                        },
                        commandType: CommandType.Text);
                    if (r > 0)
                        XoaTraoDoiDuLieuChiTiet(iID_TraoDoiDuLieuID);
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return false;
        }

        public bool XoaTraoDoiDuLieuChiTiet(Guid iID_TraoDoiDuLieuID)
        {
            var sql = "DELETE VDT_NS_TraoDoiDuLieu_ChiTiet WHERE iID_TraodoidulieuID =  @iID_TraoDoiDuLieuID; ";
            using (var conn = _connectionFactory.GetConnection())
            {
                try
                {
                    var r = conn.Execute(
                    sql,
                    param: new
                    {
                        iID_TraoDoiDuLieuID
                    },
                    commandType: CommandType.Text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get nhu trao doi du lieu theo id
        /// </summary>
        /// <param name="iID_TraoDoiDuLieuID"></param>
        /// <returns></returns>
        public VDT_NS_Traodoidulieu GetVDTraoDoiDuLieuByID(Guid? iID_TraoDoiDuLieuID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_NS_Traodoidulieu WHERE ID = '{0}' ", iID_TraoDoiDuLieuID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_NS_Traodoidulieu>(query.ToString());
                return items.FirstOrDefault();
            }
        }

        public IEnumerable<VDT_NS_TraoDoiDuLieu_ChiTiet> GetAlTraoDoiDuLieuChiTietById(Guid? iID_TraoDoiDuLieuID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM VDT_NS_TraoDoiDuLieu_ChiTiet WHERE iID_TraodoidulieuID = '{0}' ", iID_TraoDoiDuLieuID);
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<VDT_NS_TraoDoiDuLieu_ChiTiet>(query.ToString());
                return items;
            }
        }

        public VDT_NS_TraoDoiDuLieuViewModel GetDetailVDTraoDoiDuLieuByID(Guid iID_TraoDoiDuLieuID)
        {
            var sql = "select dl.*, nv.sTen as sNguonVon,  concat (DV.iID_MaDonVi, ' - ', DV.sTen) AS sDonViQuanLy from  VDT_NS_Traodoidulieu dl " +
                " inner join NS_NguonNganSach nv on nv.iID_MaNguonNganSach = dl.iID_NguonVonID" +
                " inner join NS_DonVi dv on dv.iID_Ma = dl.iID_DonViQuanLyID where dl.ID = @iID_TraoDoiDuLieuID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                try
                {
                    var items = conn.Query<VDT_NS_TraoDoiDuLieuViewModel>(sql,
                       param: new
                       {
                           iID_TraoDoiDuLieuID
                       },
                       commandType: System.Data.CommandType.Text);
                    return items.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
                return null;
            }
        }

        // getlist by list id
        public IEnumerable<VDT_NS_Traodoidulieu> GetListVDTraoDoiDuLieuByIds(List<Guid> lstId, int iNamLamViec)
        {
            string sLstId = string.Join(",", lstId.Select(n => string.Format("'{0}'", n.ToString())));
            var sql = "select * from VDT_NS_Traodoidulieu where ID in (" + sLstId + ") and iNamLamViec = @iNamLamViec";

            using (var conn = _connectionFactory.GetConnection())
            {

                try
                {
                    var items = conn.Query<VDT_NS_Traodoidulieu>(sql,
                       param: new
                       {
                           iNamLamViec
                       },
                       commandType: System.Data.CommandType.Text);
                    return items;
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
                return null;
            }

        }

        //Chuyen trang thai
        public bool ChuyenTrangThaiLstTraoDoiDuLieu(List<VDT_NS_Traodoidulieu> lstData, int iNamLamViec)
        {
            using (var conn = _connectionFactory.GetConnection())
            {

                conn.Open();
                var trans = conn.BeginTransaction();
                foreach (var item in lstData)
                {
                    if (item.iTrangThai == 1)
                    {
                        item.iTrangThai = item.iTrangThai + 1;
                        conn.Update(item, trans);
                    }

                }
                trans.Commit();

                return true;
            }


        }

        #region File FTP
        public VDT_FtpRoot GetFtpRoot(string sMaDonVi, string sIpAddress, string sFolderRoot)
        {
            var sql = "select * from VDT_FtpRoot where sMaDonVi = @sMaDonVi AND sIpAddress = @sIpAddress AND sFolderRoot = @sFolderRoot";

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.QueryFirstOrDefault<VDT_FtpRoot>(sql,
                    new
                    {
                        sMaDonVi = sMaDonVi,
                        sIpAddress = sIpAddress,
                        sFolderRoot = sFolderRoot
                    }, null, null, CommandType.Text);

                return entity;
            }
        }

        public void InsertFtpRoot(VDT_FtpRoot ftpRoot)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@Id", ftpRoot.Id);
                lstParam.Add("@sMaDonVi", ftpRoot.sMaDonVi);
                lstParam.Add("@sIpAddress", ftpRoot.sIpAddress);
                lstParam.Add("@sFolderRoot", ftpRoot.sFolderRoot);
                lstParam.Add("@sNguoiTao", ftpRoot.sNguoiTao);
                lstParam.Add("@dNgayTao", ftpRoot.dNgayTao);
                var items = conn.Execute("sp_vdt_insert_vdt_ftproot", lstParam, commandType: CommandType.StoredProcedure);
            }
        }

        public void InsertFtpFile(VDT_FtpFile ftpFile)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@Id", ftpFile.Id);
                lstParam.Add("@iID_FtpRoot", ftpFile.iID_FtpRoot);
                lstParam.Add("@sFileName", ftpFile.sFileName);
                lstParam.Add("@sRootPath", ftpFile.sRootPath);
                lstParam.Add("@sNguoiTao", ftpFile.sNguoiTao);
                lstParam.Add("@dNgayTao", ftpFile.dNgayTao);
                var items = conn.Execute("sp_vdt_insert_vdt_ftpfile", lstParam, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion

        #region Attachment
        public Attachment GetAttachment(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                return conn.Get<Attachment>(id, trans);
            }
        }
        public IEnumerable<Attachment> GetAllAttachmentByID(Guid id)
        {
            var sql = "SELECT * FROM Attachment WHERE ObjectId = @id";
            using (var conn = _connectionFactory.GetConnection())
            {
                //conn.Open();
                var items = conn.Query<Attachment>(sql,
                    param: new
                    {
                        id
                    },
                    commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public bool SaveAttachment(Attachment attachment)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    conn.Insert(attachment, trans);
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveListAttachment(List<Attachment> attachment)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    conn.Insert<Attachment>(attachment, trans);
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public bool DeleteAttachment(Guid id)
        {
            try
            {
                var sql = "DELETE FROM Attachment WHERE Id = @id";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<Attachment>(sql,
                       param: new
                       {
                           id
                       },
                       commandType: CommandType.Text);
                    return true;
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
        public bool UpdateAttachment(Attachment attachment)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    conn.Update<Attachment>(attachment, trans);
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;

        }

        public bool CheckExistSoQuyetDinhShare(string propertyName, string tableName, string checkVal)
        {
            try
            {
                var sql = $"select {propertyName} from {tableName} where upper({propertyName}) = upper('{checkVal}')";
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var items = conn.Query<string>(sql);
                    if (items.ToList().Count > 0)
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public int FindCurrentPeriod(int namLamViec)
        {
            int phanDu = namLamViec % 5;
            int phanNguyen = namLamViec / 5;
            if (phanDu == 0)
            {
                return (phanNguyen - 1) * 5 + 1;
            }
            else
            {
                return phanNguyen * 5 + 1;
            }
        }
        #endregion
    }

}
