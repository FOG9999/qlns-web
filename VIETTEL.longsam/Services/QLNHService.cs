using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Interfaces.Services;
using VIETTEL.Helpers;
using Viettel.Models.QLNH;
using System.Reflection;
using Viettel.Models.KeHoachChiTietBQP;
using Viettel.Models.Shared;
using Viettel.Models.QLNH.QuyetToan;
using System.Data.SqlClient;
using System.Globalization;
using DomainModel;
using Viettel.Models.QLNH.QuyetToan.ChuyenQuyetToan;
using Viettel.Models.QLNH.KhoiTao;
using System.Web;

namespace Viettel.Services
{
    public interface IQLNHService : IServiceBase
    {
        #region QLNH - Danh Mục tỉ giá hối đoái
        IEnumerable<DanhmucNgoaiHoi_TiGiaModel> GetAllTiGiaPaging(ref PagingInfo _paging, DateTime? dNgayLap,
            string sMaTiGia = "", string sTenTiGia = "", string sMoTa = "", string sMaTienTeGoc = "");
        DanhmucNgoaiHoi_TiGiaModel GetTyGiaById(Guid iId);
        bool SaveTyGia(NH_DM_TiGia data, List<NH_DM_TiGia_ChiTiet> dataTiGiaChiTiet, string sUsername);
        bool DeleteTyGia(Guid iId);
        IEnumerable<NH_DM_LoaiTienTe> GetNHDMLoaiTienTeList(Guid? id = null, List<Guid?> excludeIds = null);
        #endregion

        #region QLNH - Danh Mục Nhà Thầu
        IEnumerable<NHDMNhaThauModel> GetAllDanhMucNhaThau(ref PagingInfo _paging, string sMaNhaThau = "",
            string sTenNhaThau = "", string sDiaChi = "", string sDaiDien = "", string sChucVu = "", string sDienThoai = "", string sSoFax = "",
            string sEmail = "", string sWebsite = "", int? iLoai = 0);

        NHDMNhaThauModel GetDanhMucNhaThauById(Guid Id);
        bool DeleteNhaThau(Guid Id);
        bool SaveNhaThau(NH_DM_NhaThau data, string username);
        bool SaveImportNhaThau(List<NH_DM_NhaThau> contractList);
        #endregion

        #region QLNH - Thông tin hợp đồng
        IEnumerable<NH_DA_HopDongModel> GetAllNHThongTinHopDong(ref PagingInfo _paging, DateTime? dNgayHopDong,
            Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, Guid? iLoaiHopDong, string sTenHopDong, string sSoHopDong);

        IEnumerable<NH_DA_HopDongModel> GetAllNHThongTinHopDongById(Guid Id, DateTime? dNgayHopDong,
Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, Guid? iLoaiHopDong, string sTenHopDong, string sSoHopDong);
        NH_DA_HopDongModel GetThongTinHopDongById(Guid iId);
        bool SaveThongTinHopDong(NH_DA_HopDong data, bool isDieuChinh, string userName);
        bool DeleteThongTinHopDong(Guid iId);
        IEnumerable<NH_DM_LoaiHopDong> GetNHDMLoaiHopDongList(Guid? id = null);
        IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetNHKeHoachChiTietBQPNhiemVuChiList(Guid? id = null);
        IEnumerable<NH_DA_DuAn> GetNHDADuAnList(Guid? id = null);
        IEnumerable<NH_DM_NhaThau> GetNHDMNhaThauList(Guid? id = null, int? iLoai = null);
        IEnumerable<NH_DM_TiGia> GetNHDMTiGiaList(Guid? id = null);
        IEnumerable<NH_DM_TiGia_ChiTiet> GetNHDMTiGiaChiTietList(Guid? iDTiGia, bool isMaNgoaiTeKhac = true);
        IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetNHNhiemVuChiTietTheoDonViId(string maDonVi, Guid? donViID);
        IEnumerable<NH_DA_DuAn> GetNHDuAnTheoKHCTBQPChuongTrinhId(Guid? chuongTrinhID);
        IEnumerable<NH_DA_GoiThau> GetGoiThauTheoCT(Guid? donViID = null);
        bool SaveImportThongTinHopDong(List<NH_DA_HopDong> contractList);
        #endregion

        #region QLNH - Phân cấp phê duyệt
        IEnumerable<DanhmucNgoaiHoi_PhanCapPheDuyetModel> getListPhanCapPheDuyetModels(ref PagingInfo _paging,
            string sMa = "", string sTenVietTat = "", string sMoTa = "", string sTen = "");
        DanhmucNgoaiHoi_PhanCapPheDuyetModel GetPhanCapPheDuyetById(Guid iId);
        bool SavePhanCapPheDuyet(NH_DM_PhanCapPheDuyet data, string username);
        bool DeletePhanCapPheDuyet(Guid iId);
        IEnumerable<NH_DM_PhanCapPheDuyet> GetNHDMPhanCapPheDuyetList(Guid? id = null);
        #endregion

        #region QLNH - Loại hợp đồng
        IEnumerable<DanhmucNgoaiHoi_LoaiHopDongModel> getListLoaiHopDongModels(ref PagingInfo _paging,
            string sMaLoaiHopDong = "", string sTenVietTat = "", string sTenLoaiHopDong = "", string sMoTa = "");
        DanhmucNgoaiHoi_LoaiHopDongModel GetLoaiHopDongById(Guid iId);
        bool SaveLoaiHopDong(NH_DM_LoaiHopDong data);
        bool DeleteLoaiHopDong(Guid iId);
        #endregion

        #region QLNH - Kế hoạch chi tiết Bộ Quốc phòng
        NH_KHChiTietBQPViewModel getListKHChiTietBQP(PagingInfo _paging, string sSoKeHoach, DateTime? dNgayBanHanh, int? from, int? to);
        IEnumerable<LookupDto<Guid, string>> getLookupKHBQP();
        IEnumerable<LookupKHTTCP> getLookupKHTTCP();
        IEnumerable<LookupDto<Guid, string>> getLookupPhongBan();
        Boolean SaveKHBQP(List<NH_KHChiTietBQP_NhiemVuChiCreateDto> lstNhiemVuChis, NH_KHChiTietBQP khct, string state);
        NH_KHChiTietBQPModel GetKeHoachChiTietBQPById(Guid id);
        Boolean DeleteKHBQP(Guid id);
        NH_KHChiTietBQP_NVCViewModel GetDetailKeHoachChiTietBQP(string state, Guid? KHTTCP_ID, Guid? KHBQP_ID, Guid? iID_BQuanLyID, Guid? iID_DonViID, string sTenNhiemVuChi, string sTenPhongBan, string sTenDonVi, bool isUseLastTTCP = false);

        List<NH_DM_TiGia_ChiTiet_ViewModel> GetTiGiaChiTietByTiGiaId(Guid KHBQP_ID);

        IEnumerable<NS_DonVi> GetDonviListByYear(int namLamViec = 0);

        IEnumerable<NH_KHChiTietBQP_NVCModel> GetListBQPNhiemVuChiById(Guid id, string sTenNhiemVuChi, Guid? iID_BQuanLyID, Guid? iID_DonViID);

        IEnumerable<LookupKHBQP> getLookupKHBQPByKHTTCPId(Guid id);
        #endregion

        #region QLNH - Quyết toán niên độ
        IEnumerable<NH_QT_QuyetToanNienDoData> GetListQuyetToanNienDo(ref PagingInfo _paging, string sSoDeNghi,
            DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamKeHoach, int? tabIndex);

        IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetDetailQuyetToanNienDoCreate(int? iNamKeHoach, Guid? iIDDonVi, int? donViTinhUSD, int? donViTinhVND);
        IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetDetailQuyetToanNienDoDetail(int? iNamKeHoach, Guid? iIDDonVi, Guid? iIDQuyetToan, int? donViTinhUSD, int? donViTinhVND);


        NH_QT_QuyetToanNienDoData GetThongTinQuyetToanNienDoById(Guid iId);
        NH_QT_QuyetToanNienDoReturnData SaveQuyetToanNienDo(NH_QT_QuyetToanNienDo data, string userName);
        bool SaveTongHop(NH_QT_QuyetToanNienDo data, string userName, string listId);
        IEnumerable<NS_DonVi> GetDonviList(int? iNamLamViec);
        NH_QT_QuyetToanNienDoChiTietReturnData SaveQuyetToanNienDoDetail(List<NH_QT_QuyetToanNienDo_ChiTiet> data, string userName, bool isAddOrUpdate, Guid idQuyetToan);

        bool DeleteQuyetToanNienDo(Guid iId);
        bool LockOrUnLockQuyetToanNienDo(Guid id, bool isLockOrUnLock);
        IEnumerable<NH_QT_QuyetToanNienDoData> GetListTongHopQuyetToanNienDo(string sSodenghi = "", DateTime? dNgaydenghi = null, Guid? iDonvi = null, int? iNamKeHoach = null);
        IEnumerable<NH_DM_TiGia> GetTiGiaQuyetToan(Guid? id = null);
        #endregion

        #region QLNH - Danh mục tài sản
        IEnumerable<DanhmucNgoaiHoi_TaiSanModel> getListDanhMucTaiSanModels(ref PagingInfo _paging, string sMaLoaiTaiSan = "", string sTenLoaiTaiSan = "", string sMoTa = "");
        DanhmucNgoaiHoi_TaiSanModel GetTaiDanhMucSanById(Guid iId);
        bool SaveTaiSan(NH_DM_LoaiTaiSan data, string username);
        bool DeleteDanhMucTaiSan(Guid iId);
        IEnumerable<NH_DM_LoaiTaiSan> GetNHDMLoaiTaiSanList(Guid? id = null);
        #endregion

        #region QLNH - Quyết toán - Tài sản
        IEnumerable<NH_QT_TaiSanViewModel> GetListTaiSanModels(Guid? chungTuId);
        QuyetToan_ChungTuModelPaging GetListChungTuTaiSanModels(ref PagingInfo _paging, string sTenChungTu = "", string sSoChungTu = "", DateTime? dNgayChungTu = null);
        NH_QT_ChungTuTaiSan GetChungTuTaiSanById(Guid iId);
        List<GetTaiSan> getListTaiSan();
        bool DeleteChungTuTaiSan(Guid iId);
        bool DeleteTaiSan(Guid iId);
        bool SaveChungTuTaiSan(List<NH_QT_TaiSan> datats, NH_QT_ChungTuTaiSan datactts);
        bool SaveListLoaiTaiSan(List<NH_DM_LoaiTaiSan> data);
        IEnumerable<NH_DA_DuAn> GetLookupDuAn(Guid? ID);
        IEnumerable<NH_DA_HopDong> GetLookupHopDong(Guid? ID);
        #endregion

        #region QLNH - Đề nghị thanh toán
        IEnumerable<NS_DonViViewModel> GetAllNSDonVi(int nam);
        IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetAllNhiemVuChiByDonVi(Guid? iID_MaDonVi = null);
        IEnumerable<DM_ChuDauTuViewModel> GetAllDMChuDauTu();
        IEnumerable<NH_DA_DuAn> GetDADuAn(Guid? iID_NhiemVuChi, Guid? iID_ChuDauTu);
        IEnumerable<NH_DA_HopDong> GetThongTinHopDong(Guid? iID_NhiemVuChi);
        IEnumerable<NH_DM_TiGia> GetThongTinTyGia();
        IEnumerable<MucLucNganSachViewModel> GetAllMucLucNganSach(ref PagingInfo _paging, string username, int namlamviec, ThongTinThanhToanSearchModel conditionSearch);
        IEnumerable<NH_DM_NhaThau> GetAllDMNhaThau();
        IEnumerable<ThongTinThanhToanModel> GetAllThongTinThanhToanPaging(ref PagingInfo _paging, Guid? iID_DonVi,
            string sSoDeNghi, DateTime? dNgayDeNghi, int? iLoaiNoiDungChi, int? iLoaiDeNghi,
            Guid? iID_ChuDauTuID, Guid? iID_KHCTBQP_NhiemVuChiID, int? iQuyKeHoach, int? iNamKeHoach, int? iNamNganSach,
            int? iCoQuanThanhToan, Guid? iID_NhaThauID, int? iTrangThai);
        ThongTinThanhToanModel GetThongTinThanhToanByID(Guid? id);
        IEnumerable<ThanhToanChiTietViewModel> GetThongTinThanhToanChiTietById(Guid? iDThanhToan);
        NH_TT_ThanhToan_ChiTiet GetThongTinThanhToanChiTiet(Guid id_chitiet);
        bool DeleteDeNghiThanhToan(Guid id);
        IEnumerable<NS_PhongBan> GetAllNSPhongBan();
        IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoChiThanhToan(string listIDThanhToan, int thang, int quy, int nam);
        double ChuyenDoiTyGia(Guid? matygia, float sotiennhap, int loaitiennhap);
        IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoThongBaoCapKinhPhiChiNgoaiTe(string listIDThanhToan, DateTime tungay, DateTime denngay);
        IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoThongBaoCapKinhPhiChiTrongNuoc(string listIDThanhToan, DateTime tungay, DateTime denngay);
        IEnumerable<NH_TT_ThanhToan> GetThanhToanGanNhat(DateTime dngaytao, Guid? idonvi, Guid? inhiemvuchi, Guid? ichudautu);
        Boolean CheckTrungMaDeNghi(string sodenghi, int type_action, Guid? idenghi);
        #endregion

        #region QLNH - Kế hoạch tổng thể TTCP
        NH_KHTongTheTTCP Get_KHTT_TTCP_ById(Guid iId);
        IEnumerable<NH_KHTongTheTTCPModel> Get_KHTT_TTCP_ListActive();
        bool SaveKeHoachTTCP(NH_KHTongTheTTCP data, string sUsername);
        bool DeleteKeHoachTTCP(Guid id);
        NH_KHTongTheTTCPViewModel getListKHTongTheTTCP(PagingInfo _paging, string sSoKeHoach, DateTime? dNgayBanHanh, int? from, int? to);
        IEnumerable<LookupKHTTCP> getLookupKHTTCPByStage();
        NH_KHTongTheTTCP_NVCViewModel GetDetailKeHoachTongTheTTCP(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan);
        #endregion

        #region QLNH - Kế hoạch chi tiết TTCP
        Boolean SaveKHTongTheTTCP(List<NH_KHTongTheTTCP_NhiemVuChiDto> lstNhiemVuChis, NH_KHTongTheTTCP khtt, string state);
        IEnumerable<NH_KHTongTheTTCP_NhiemVuChi_Parent> Get_KHCT_TTCP_GetListOfParent(Guid khtt_id, Guid program_id);
        IEnumerable<NH_KHTongTheTTCP_NhiemVuChi> Get_KHCT_TTCP_GetListProgramByPlanID(Guid khtt_id);//Lấy ds chương trình
        IEnumerable<NH_KHTongTheTTCP_NhiemVuChi> Get_KHCT_TTCP_GetListMissionByPlanIdAndProgramId(Guid khtt_id, Guid program_id);//Lấy ds nhiệm vụ chi
        NH_KHTongTheTTCP_NhiemVuChi GetNhiemVuChiById(Guid ID);
        IEnumerable<NH_KHTongTheTTCP_BQL> GetListDM_BQL();
        IEnumerable<NH_KHTongTheTTCP_SoKeHoach> GetListKHTT_ActiveWithNumber();
        bool SaveKeHoachTTCP_NVC(NH_KHTongTheTTCP_NhiemVuChi data, string sUsername);
        bool? CheckKHTongTheTTCPIsActive(Guid id);
        NH_KHTongTheTTCP FindParentTTCPActive(Guid id);
        #endregion

        #region QLNH - Thông tin dự án
        IEnumerable<NHDAThongTinDuAnModel> getListThongTinDuAnModels(ref PagingInfo _paging, string sMaDuAn = "", string sTenDuAn = "",
            Guid? iID_BQuanLyID = null,
            Guid? iID_ChuDauTuID = null,
            Guid? iID_DonViID = null, Guid? iID_CapPheDuyetID = null);
        NHDAThongTinDuAnModel GetThongTinDuAnById(Guid iId);

        bool DeleteThongTinDuAn(Guid iId);
        List<NH_DA_DuAn_ChiPhiModel> getListChiPhiTTDuAn(Guid? ID, string state);
        IEnumerable<NH_DM_PhanCapPheDuyet> GetLookupThongTinDuAn();
        IEnumerable<NS_PhongBan> GetLookupQuanLy();
        IEnumerable<NS_DonVi> GetLookupThongTinDonVi();
        IEnumerable<DM_ChuDauTu> GetLookupChuDauTu();
        IEnumerable<NH_DA_DuAn> GetThongTinDuAnDuAnList(Guid? id = null);
        List<NH_DA_DuAn> GetMaDuAn(string iID_MaDonVi, string sMaChuDauTu);
        IEnumerable<DM_ChuDauTu> GetLookupChuDauTuu();
        IEnumerable<NH_DM_ChiPhi> GetLookupChiPhi();
        IEnumerable<NH_KHChiTietBQP_NhiemVuChiModel> GetNHKeHoachChiTietBQPNhiemVuChiListDuAn(Guid? id = null);
        IEnumerable<NS_DonVi> GetListDonViToBQP(Guid? id = null);
        IEnumerable<NHDAThongTinDuAnModel> GetListBQPToNHC(Guid? id = null);
        //IEnumerable<NH_DA_DuAn> GetMaDuAn(Guid? IdDonVi = null, Guid? IdCDT = null);
        IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetListCTbyDV(Guid? id = null, Guid? idBQP = null);
        bool SaveThongTinDuAn(NHDAThongTinDuAnModel data, string username, string state, List<NH_DA_DuAn_ChiPhiDto> dataTableChiPhi, Guid? oldId);
        IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetListChuongTrinh();
        bool SaveImportThongTinDuAn(List<NH_DA_DuAn> contractList);
        #endregion

        #region QLNH - Chênh lệch tỉ giá hối đoái
        IEnumerable<ChenhLechTiGiaModel> GetAllChenhLechTiGia(ref PagingInfo _paging, Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong);
        IEnumerable<NS_DonVi> GetDonViList(bool hasChuongTrinh = false);
        IEnumerable<NH_DA_HopDong> GetNHDAHopDongList(Guid? chuongTrinhID);
        IEnumerable<ChenhLechTiGiaModel> GetDataExportChenhLechTiGia(Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong);
        #endregion

        #region QLNH - Tổng hợp dự án
        IEnumerable<NHDAThongTinDuAnModel> getListTongHopDuAnModels(ref PagingInfo _paging, Guid? iID_BQuanLyID = null, Guid? iID_DonViID = null);
        List<NHDAThongTinDuAnModel> getListTongHopDuAn_BaoCaoModels(Guid? iID_BQuanLyID = null, Guid? iID_DonViID = null);


        //NHDAThongTinDuAnModel GetThongTinDuAnById(Guid iId);

        //bool DeleteThongTinDuAn(Guid iId);
        //IEnumerable<NH_DM_PhanCapPheDuyet> GetLookupThongTinDuAn();
        //IEnumerable<NS_PhongBan> GetLookupQuanLy();
        //IEnumerable<NS_DonVi> GetLookupThongTinDonVi();
        //IEnumerable<DM_ChuDauTu> GetLookupChuDauTu();
        //IEnumerable<NH_DM_ChiPhi> GetLookupChiPhi();
        //bool SaveThongTinDuAn(NH_DA_DuAn data, string userName, string state);
        #endregion

        #region QLNH - Báo cáo tài sản
        IEnumerable<BaoCaoTaiSanModel> getListBaoCaoTaiSanModels(ref PagingInfo _paging,
        Guid? iID_DonViID = null,
        Guid? iID_DuAnID = null,
        Guid? iID_HopDongID = null);
        List<BaoCaoTaiSanModel2> getListBaoCaoTaiSanModelstb2(ref PagingInfo _paging,
        Guid? iID_DonViID = null);
        BaoCaoTaiSanModel GetBaoCaoTaiSanById(Guid iId);
        IEnumerable<NS_DonViModel> GetLookupDonViTaiSan();
        IEnumerable<NH_DA_DuAn> GetLookupDuAnTaiSan();
        IEnumerable<NH_DA_HopDong> GetLookupHopDongTaiSan();
        #endregion

        #region QLNH - Báo cáo tình hình thực hiện dự án

        BaoCaoTHTHDuAnModel GetBaoCaoTinhHinhThucHienDuAn(ref PagingInfo _paging, DateTime? dBatDau = null, DateTime? dKetThuc = null, Guid? iID_DuAnID = null);
        NH_DA_DuAnViewModel GetDuAnById(Guid? iId);
        IEnumerable<NH_DA_DuAn> GetListDuAnByDonViID(Guid iID);
        BaoCaoTHTHDuAnModel GetDataExportBaoCaoTinhHinhThucHienDuAn(DateTime? dBatDau = null, DateTime? dKetThuc = null, Guid? iID_DuAnID = null);
        #endregion

        #region QLNH - Thông tri cấp phát
        IEnumerable<ThanhToan_ThongTriModel> GetListThanhToanTaoThongTri(ref PagingInfo _paging, Guid? iThongTri, Guid? iDonVi, int? iNam, int? iLoaiThongTri, int? iLoaiNoiDung, int? iTypeAction = 0);
        Boolean CheckTrungMaThongTri(string mathongtri, int type_action, Guid? imathongtri);
        bool SaveDanhSachPheDuyetThanhToan(Guid? iThongTri, string lstDeNghiThanhToan, int type_action);
        IEnumerable<ThongTriCapPhatModel> GetListThongTriCapPhatPaging(ref PagingInfo _paging, Guid? iDonVi, string sMaThongTri, DateTime? dNgayLap, int? iNam);

        ThongTriCapPhatModel GetThongTriByID(Guid? IdThongTri);
        IEnumerable<NH_TT_ThongTriCapPhat_ChiTiet> GetListhongTriChiTietByID(Guid? IdThongTri);
        bool DeleteThongTriCapPhat(Guid? id);

        IEnumerable<ThongTriBaoCaoModel> ExportBaoCaoThongTriCapPhat(Guid? idThongTri);
        #endregion

        #region QLNH - Quyết toán dự án hoành thành
        IEnumerable<NH_QT_QuyetToanDAHTData> GetListQuyetToanDuAnHT(ref PagingInfo _paging, string sSoDeNghi,
           DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamBaoCaoTu, int? iNamBaoCaoDen, int? tabIndex);
        NH_QT_QuyetToanDAHTData GetThongTinQuyetToanDuAnHTById(Guid iId);
        NH_QT_QuyetToanDuAnHTReturnData SaveQuyetToanDuAnHT(NH_QT_QuyetToanDAHT data, string userName);
        bool DeleteQuyetToanDuAnHT(Guid iId);
        bool LockOrUnLockQuyetToanDuAnHT(Guid id, bool isLockOrUnLock);
        bool SaveTongHopQuyetToanDAHT(NH_QT_QuyetToanDAHT data, string userName, string listId);
        IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> GetDetailQuyetToanDuAnDetail(int? iNamBaoCaoTu, int? iNamBaoCaoDen, Guid? iIDDonVi, Guid? iIDQuyetToan, int? donViTinhUSD, int? donViTinhVND);
        IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> GetDetailQuyetToanDuAnCreate(int? iNamBaoCaoTu, int? iNamBaoCaoDen, Guid? iIDDonVi, int? donViTinhUSD, int? donViTinhVND);
        NH_QT_QuyetToanDAHTChiTietReturnData SaveQuyetToanDuAnDetail(List<NH_QT_QuyetToanDAHT_ChiTiet> data, string userName);
        IEnumerable<NH_QT_QuyetToanDAHTData> GetListTongHopQuyetToanDAHT(string sSodenghi = "", DateTime? dNgaydenghi = null, Guid? iDonvi = null, int? iNamBaoCaoTu = null, int? iNamBaoCaoDen = null);
        #endregion

        #region QLNH - Báo cáo chi tiết số chuyển năm sau
        IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetBaoCaoChiTietSoChuyenNamSauDetail(int? iNamKeHoach, Guid? iIDDonVi);
        #endregion

        #region QLNH - Danh mục chương trình

        NH_KHChiTietBQPViewModel getListDanhMucChuongTrinh(PagingInfo _paging, string sTenNhiemVuChi, Guid? iID_BQuanLyID, Guid? iID_DonViID);

        #endregion

        #region QLNH - Báo cáo tổng hợp số chuyển năm sau
        IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetBaoCaoTongHopSoChuyenNamSauDetail(int? iNamKeHoach);
        #endregion

        #region QLNH - Danh mục nội dung chi

        IEnumerable<DanhmucNgoaiHoi_NoiDungChiModel> GetListDanhMucNoiDungChiModels(ref PagingInfo _paging, string sMaNoiDungChi = "", string sTenNoiDungChi = "", string sMoTa = "");

        DanhmucNgoaiHoi_NoiDungChiModel GetDanhMucNoiDungChiById(Guid iId);
        IEnumerable<NH_DM_NoiDungChi> GetNHDMNoiDungChiList(Guid? id = null);
        bool SaveNoiDungChi(NH_DM_NoiDungChi data);
        bool DeleteDanhMucNoiDungChi(Guid iId);
        #endregion

        #region QLNH - Hình thức chọn nhà thầu

        IEnumerable<DanhmucNgoaiHoi_HinhThucChonNhaThauModel> GetListDanhMucHinhThucChonNhaThauPaging(ref PagingInfo _paging, string sMaHinhThuc = "", string sTenVietTat = "", string sTenHinhThuc = "", string sMoTa = "");

        DanhmucNgoaiHoi_HinhThucChonNhaThauModel GetDanhMucHinhThucChonNhaThauById(Guid iId);
        bool SaveHinhThucChonNhaThau(NH_DM_HinhThucChonNhaThau data, int? soThuTu);
        Boolean DeleteDanhMucHinhThucChonNhaThau(Guid iId);
        #endregion

        #region QLNH - Phương thức chọn nhà thầu

        IEnumerable<DanhmucNgoaiHoi_PhuongThucChonNhaThauModel> GetListDanhMucPhuongThucChonNhaThauPaging(ref PagingInfo _paging, string sMaPhuongThuc = "", string sTenVietTat = "", string sTenPhuongThuc = "", string sMoTa = "");

        DanhmucNgoaiHoi_PhuongThucChonNhaThauModel GetDanhMucPhuongThucChonNhaThauById(Guid iId);
        bool SavePhuongThucChonNhaThau(NH_DM_PhuongThucChonNhaThau data, int? soThuTu);
        bool DeleteDanhMucPhuongThucChonNhaThau(Guid iId);
        #endregion

        #region QLNH - Thông tin gói thầu
        IEnumerable<NH_DA_GoiThauModel> GetAllNHThongTinGoiThau(ref PagingInfo _paging, string sTenGoiThau,
            Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien);
        IEnumerable<NH_DA_GoiThauModel> GetAllNHThongTinGoiThauById(Guid id, string sTenGoiThau,
        Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien);
        NH_DA_GoiThauModel GetThongTinGoiThauById(Guid id);
        IEnumerable<NH_DM_LoaiTienTe> GetNHDMLoaiTienTeByCode(string maTienTe);
        IEnumerable<NH_DM_HinhThucChonNhaThau> GetNHDMHinhThucChonNhaThauList(Guid? id = null);
        IEnumerable<NH_DM_PhuongThucChonNhaThau> GetNHDMPhuongThucChonNhaThauList(Guid? id = null);
        bool SaveThongTinGoiThau(NH_DA_GoiThau data, bool isDieuChinh, string userName);
        bool DeleteThongTinGoiThau(Guid iId);
        bool SaveImportThongTinGoiThau(List<NH_DA_GoiThau> packageList);
        #endregion

        #region QLNH - Thông tri quyết toán
        NH_QT_ThongTriQuyetToanViewModel GetListThongTriQuyetToan(PagingInfo _paging, NH_QT_ThongTriQuyetToanFilter filter);
        IEnumerable<NH_QT_ThongTriQuyetToan_ChiTietModel> GetChiTietThongTriQuyetToan(Guid? iID_DonViID, Guid? iID_KHCTBQP_ChuongTrinhID, int? iNamThucHien);
        IEnumerable<LookupDto<Guid, string>> GetLookupBQPNhiemVuChiByDonViId(Guid? iID_DonViID);
        bool SaveThongTriQuyetToan(NH_QT_ThongTriQuyetToanCreateDto input, string state);
        bool DeleteThongTriQuyetToan(Guid id);
        NH_QT_ThongTriQuyetToanModel GetThongTinQuyetToanById(Guid id);
        IEnumerable<NH_QT_ThongTriQuyetToan_ChiTietModel> GetListThongTriQuyetToanChiTietByTTQTId(Guid id);
        #endregion

        #region QLNH - Nguồn ngân sách
        IEnumerable<DanhmucNgoaiHoi_NguonNganSachModel> GetListDanhMucNguonNganSachPaging(ref PagingInfo _paging, string MaNguonNganSach, string sTenNguonNganSach, int? iTrangThai);

        DanhmucNgoaiHoi_NguonNganSachModel GetDanhMucNguonNganSachById(int? iId);
        NH_DM_NguonNganSachReturnData SaveNguonNganSach(DanhmucNgoaiHoi_NguonNganSachModel data, string sIPSua, string username);
        bool DeleteDanhMucNguonNganSach(int iId);
        #endregion

        #region QLNH - Báo cáo kết luân quyết toán
        IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> getListBaoCaoKetLuanQuyetToanModels(DateTime? dTuNgay, DateTime? dDenNgay, Guid? iDonvi);

        //DanhmucNgoaiHoi_NguonNganSachModel GetDanhMucNguonNganSachById(int? iId);
        //NH_DM_NguonNganSachReturnData SaveNguonNganSach(DanhmucNgoaiHoi_NguonNganSachModel data);
        //Boolean DeleteDanhMucNguonNganSach(int iId);
        #endregion

        #region QLNH - Chuyển dữ liệu quyết toán
        DataTable Get_dtMucLucNganSach(int Trang = 1, int SoBanGhi = 0, String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "");
        int Get_MucLucNganSach_Count(String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "");
        bool Insert_FirstTimeData(int namLamViec, Guid? iChuyenQuyetToan);

        NH_QT_ChuyenQuyetToanReturnData SaveChuyenQuyetToan(NH_QT_ChuyenQuyetToan data, string userName, int namLamViec);
        IEnumerable<NH_QT_ChuyenQuyetToanData> GetListChuyenQuyetToan(ref PagingInfo _paging, string sSoChungTu,
        DateTime? dNgayChungTu, Guid? iDonVi, int? iLoaiThoiGian, int? iThoiGian);

        NH_QT_ChuyenQuyetToanData GetThongTinChuyenQuyetToanById(Guid iId);

        bool DeleteChuyenQuyetToan(Guid iId);

        #endregion

        #region QLNH - Khởi tạo cấp phát
        IEnumerable<NH_KT_KhoiTaoCapPhatData> GetListKhoiTaoCapPhat(ref PagingInfo _paging, DateTime? dNgayKhoiTao, Guid? iDonVi, int? iNamKhoiTao);

        IEnumerable<NH_KT_KhoiTaoCapPhat_ChiTietData> GetListKhoiTaoCapPhatChiTiet(Guid iId);
        NH_KT_KhoiTaoCapPhatData GetThongTinKhoiTaoCapPhatById(Guid iId);
        NH_KT_KhoiTaoCapPhatReturnData SaveKhoiTaoCapPhat(NH_KT_KhoiTaoCapPhat data, string userName);

        bool DeleteKhoiTaoCapPhat(Guid iId);
        IEnumerable<NH_DM_TiGia_ChiTiet> GetNHDMTiGiaChiTiet(Guid? iDTiGia, bool isMaNgoaiTeKhac = true);
        IEnumerable<NH_DA_HopDong> GetListHopDong();
        NH_KT_KhoiTaoCapPhat_ChiTietReturnData SaveKhoiTaoCapPhatDetail(List<NH_KT_KhoiTaoCapPhat_ChiTiet> data, List<NH_KT_KhoiTaoCapPhat_ChiTiet> dataDelete, string userName);
        #endregion

        #region QLNH - Báo cáo quyết toán thuộc các nguồn chi đặc biệt
        IEnumerable<NH_QTND_NguonChiDacBietReport> GetBaoCaoQuyetToanNguonChiDacBiet(int? iNamKeHoach);
        #endregion

        #region Tổng hợp
        void InsertNHTongHop_Tang(SqlConnection conn, SqlTransaction trans, string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld = null);
        void InsertNHTongHop_Giam(SqlConnection conn, SqlTransaction trans, string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld = null);
        void DeleteNHTongHop_Giam(SqlConnection conn, string sLoai, Guid iIdQuyetDinh);
        void InsertNHTongHop(Guid iIDChungTu, string sLoai, List<NHTHTongHopQuery> lstData);
        #endregion
    }

    public class QLNHService : IQLNHService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static IQLNHService _default;

        public QLNHService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }

        public static IQLNHService Default
        {
            get { return _default ?? (_default = new QLNHService()); }
        }

        #region QLNH - Danh Mục tỉ giá hối đoái
        public IEnumerable<DanhmucNgoaiHoi_TiGiaModel> GetAllTiGiaPaging(ref PagingInfo _paging, DateTime? dNgayLap, string sMaTiGia, string sTenTiGia, string sMoTa, string sMaTienTeGoc)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaTiGia", sMaTiGia);
                lstPrams.Add("sTenTiGia", sTenTiGia);
                lstPrams.Add("sMoTaTiGia", sMoTa);
                lstPrams.Add("sMaTienTeGoc", sMaTienTeGoc);
                lstPrams.Add("dNgayLap", dNgayLap);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_TiGiaModel>("proc_get_all_danhmuctigia_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }
        public DanhmucNgoaiHoi_TiGiaModel GetTyGiaById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_TiGia WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_TiGiaModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveTyGia(NH_DM_TiGia data, List<NH_DM_TiGia_ChiTiet> dataTiGiaChiTiet, string sUsername)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    bool result = false;
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        data.dNgayTao = DateTime.Now;
                        data.sNguoiTao = sUsername;

                        Guid idNew = conn.Insert<NH_DM_TiGia>(data, trans);

                        foreach (var item in dataTiGiaChiTiet)
                        {
                            item.iID_TiGiaID = idNew;
                        }

                        conn.Insert<NH_DM_TiGia_ChiTiet>(dataTiGiaChiTiet, trans);
                        result = idNew != Guid.Empty;
                    }
                    else
                    {
                        data.dNgaySua = DateTime.Now;
                        data.sNguoiSua = sUsername;

                        result = conn.Update<NH_DM_TiGia>(data, trans);

                        foreach (var item in dataTiGiaChiTiet)
                        {
                            if (!result) break;
                            if (item.ID == Guid.Empty)
                            {
                                Guid idCTNew = conn.Insert<NH_DM_TiGia_ChiTiet>(item, trans);
                                result &= idCTNew != Guid.Empty;
                            }
                            else
                            {
                                result &= conn.Update<NH_DM_TiGia_ChiTiet>(item, trans);
                            }
                        }
                    }

                    if (result)
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                if (trans != null) trans.Rollback();
            }

            return false;
        }
        public bool DeleteTyGia(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_TiGia>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_TiGia>(checkExist, trans);
                        conn.Execute("DELETE FROM NH_DM_TiGia_ChiTiet WHERE iID_TiGiaID = @iId", new { iId = iId }, trans);
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
        public IEnumerable<NH_DM_LoaiTienTe> GetNHDMLoaiTienTeList(Guid? id = null, List<Guid?> excludeIds = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DM_LoaiTienTe ");
            query.Append("WHERE 1=1 ");
            if (id != null && id != Guid.Empty)
            {
                query.AppendLine("AND ID = @ID ");
            }
            else if (excludeIds != null && excludeIds.Count() > 0)
            {
                query.AppendLine("AND ID NOT IN ( ");
                for (int i = 0; i < excludeIds.Count(); i++)
                {
                    query.Append("@ExcludeId" + i);
                    if (i < excludeIds.Count() - 1) query.Append(",");
                }

                query.Append(") ");
            }

            query.Append("ORDER BY sMaTienTe");
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters dymParam = new DynamicParameters();
                if (id != null && id != Guid.Empty)
                {
                    dymParam.Add("ID", id);
                }
                else if (excludeIds != null && excludeIds.Count() > 0)
                {
                    for (int i = 0; i < excludeIds.Count(); i++)
                    {
                        dymParam.Add("ExcludeId" + i, excludeIds[i]);
                    }
                }

                var items = conn.Query<NH_DM_LoaiTienTe>(query.ToString(), dymParam, commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Danh Mục Nhà Thầu
        public IEnumerable<NHDMNhaThauModel> GetAllDanhMucNhaThau(ref PagingInfo _paging,
            string sMaNhaThau, string sTenNhaThau, string sDiaChi, string sDaiDien, string sChucVu, string sDienThoai,
            string sSoFax, string sEmail, string sWebsite, int? iLoai)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iLoai", iLoai);
                lstParam.Add("sMaNhaThau", sMaNhaThau);
                lstParam.Add("sTenNhaThau", sTenNhaThau);
                lstParam.Add("sDiaChi", sDiaChi);
                lstParam.Add("sDaiDien", sDaiDien);
                lstParam.Add("sChucVu", sChucVu);
                lstParam.Add("sDienThoai", sDienThoai);
                lstParam.Add("sSoFax", sSoFax);
                lstParam.Add("sEmail", sEmail);
                lstParam.Add("sWebsite", sWebsite);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NHDMNhaThauModel>("proc_get_all_nhdanhmucnhathau_paging", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }
        public NHDMNhaThauModel GetDanhMucNhaThauById(Guid Id)
        {
            string sql = "SELECT * FROM NH_DM_NhaThau WHERE Id = @Id";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NHDMNhaThauModel>(sql,
                    param: new { Id },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public bool DeleteNhaThau(Guid Id)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var entity = conn.Get<NH_DM_NhaThau>(Id, trans);
                    if (entity != null)
                    {
                        conn.Delete<NH_DM_NhaThau>(entity, trans);
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

        public bool SaveNhaThau(NH_DM_NhaThau data, string username)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.Id == null || data.Id == Guid.Empty)
                    {
                        var entity = new NH_DM_NhaThau();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = username;
                        conn.Insert<NH_DM_NhaThau>(entity, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NH_DM_NhaThau>(data.Id, trans);
                        if (entity != null)
                        {
                            data.dNgayTao = entity.dNgayTao;
                            data.sNguoiTao = entity.sNguoiTao;
                        }
                        conn.Update<NH_DM_NhaThau>(data, trans);
                    }

                    trans.Commit();
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

        public bool SaveImportNhaThau(List<NH_DM_NhaThau> contractList)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    conn.Insert<NH_DM_NhaThau>(contractList, trans);
                    trans.Commit();
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
        #endregion

        #region QLNH - Thông tin hợp đồng
        public IEnumerable<NH_DA_HopDongModel> GetAllNHThongTinHopDong(ref PagingInfo _paging, DateTime? dNgayHopDong,
            Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, Guid? iLoaiHopDong, string sTenHopDong, string sSoHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("dNgayHopDong", dNgayHopDong);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", iChuongTrinh);
                lstParam.Add("iDuAn", iDuAn);
                lstParam.Add("iLoaiHopDong", iLoaiHopDong);
                lstParam.Add("sTenHopDong", sTenHopDong);
                lstParam.Add("sSoHopDong", sSoHopDong);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_DA_HopDongModel>("proc_get_all_nh_da_tthopdong_paging_demo", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<NH_DA_HopDongModel> GetAllNHThongTinHopDongById(Guid Id, DateTime? dNgayHopDong,
    Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, Guid? iLoaiHopDong, string sTenHopDong, string sSoHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("dNgayHopDong", dNgayHopDong);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", Id);
                lstParam.Add("iDuAn", iDuAn);
                lstParam.Add("iLoaiHopDong", iLoaiHopDong);
                lstParam.Add("sTenHopDong", sTenHopDong);
                lstParam.Add("sSoHopDong", sSoHopDong);

                var items = conn.Query<NH_DA_HopDongModel>("proc_get_all_nh_da_tthopdong", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public NH_DA_HopDongModel GetThongTinHopDongById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DA_HopDong WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_DA_HopDongModel>(query.ToString(), param: new { iId = iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NH_DA_GoiThau> GetGoiThauTheoCT(Guid? ID = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT DISTINCT GT.* FROM NH_DA_GoiThau GT ");
            DynamicParameters lstPrams = new DynamicParameters();
            if (ID.HasValue)
            {
                query.AppendLine("JOIN NH_DA_HopDong hd ON hd.iID_DonViID = GT.iID_DonViID");
                query.AppendLine("WHERE hd.iID_DonViID = @ID ");
                lstPrams.Add("ID", ID.Value);
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_GoiThau>(query.ToString(),
                  param: lstPrams,
                  commandType: CommandType.Text);
                return items;
            }
        }
        public bool SaveThongTinHopDong(NH_DA_HopDong data, bool isDieuChinh, string userName)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (!isDieuChinh)
                    {
                        if (data.ID == null || data.ID == Guid.Empty)
                        {
                            var entity = new NH_DA_HopDong();
                            entity.MapFrom(data);
                            entity.dNgayTao = DateTime.Now;
                            entity.sNguoiTao = userName;
                            entity.bIsActive = true;
                            entity.bIsGoc = true;
                            conn.Insert(entity, trans);
                        }
                        else
                        {
                            var entity = conn.Get<NH_DA_HopDong>(data.ID, trans);
                            if (entity == null) return false;
                            entity.iID_DonViID = data.iID_DonViID;
                            entity.iID_MaDonVi = data.iID_MaDonVi;
                            entity.iID_KHCTBQP_ChuongTrinhID = data.iID_KHCTBQP_ChuongTrinhID;
                            entity.iPhanLoai = data.iPhanLoai;
                            entity.iID_DuAnID = data.iID_DuAnID;
                            entity.iID_BQuanLyID = data.iID_BQuanLyID;
                            entity.sSoHopDong = data.sSoHopDong;
                            entity.dNgayHopDong = data.dNgayHopDong;
                            entity.sTenHopDong = data.sTenHopDong;
                            entity.iID_LoaiHopDongID = data.iID_LoaiHopDongID;
                            entity.dKhoiCongDuKien = data.dKhoiCongDuKien;
                            entity.dKetThucDuKien = data.dKetThucDuKien;
                            entity.iThoiGianThucHien = data.iThoiGianThucHien;
                            entity.iID_NhaThauThucHienID = data.iID_NhaThauThucHienID;
                            entity.iID_TiGiaID = data.iID_TiGiaID;
                            entity.iID_TiGia_ChiTietID = data.iID_TiGia_ChiTietID;
                            entity.sMaNgoaiTeKhac = data.sMaNgoaiTeKhac;
                            entity.fGiaTriVND = data.fGiaTriVND;
                            entity.fGiaTriUSD = data.fGiaTriUSD;
                            entity.fGiaTriEUR = data.fGiaTriEUR;
                            entity.fGiaTriNgoaiTeKhac = data.fGiaTriNgoaiTeKhac;
                            entity.dNgaySua = DateTime.Now;
                            entity.sNguoiSua = userName;
                            conn.Update(entity, trans);
                        }
                    }
                    else
                    {
                        var entity = new NH_DA_HopDong();
                        entity.MapFrom(data);
                        entity.ID = Guid.NewGuid();
                        if (data.iID_HopDongGocID == null || data.iID_HopDongGocID == Guid.Empty)
                            entity.iID_HopDongGocID = data.ID;
                        entity.bIsActive = true;
                        entity.bIsGoc = false;
                        entity.iLanDieuChinh = data.iLanDieuChinh + 1;
                        entity.iID_ParentAdjustID = data.ID;
                        entity.sNguoiTao = userName;
                        entity.dNgayTao = DateTime.Now;

                        var entityGoc = conn.Get<NH_DA_HopDong>(data.ID, trans);
                        entityGoc.bIsActive = false;
                        entityGoc.sNguoiSua = userName;
                        entityGoc.dNgaySua = DateTime.Now;

                        conn.Update(entityGoc, trans);
                        conn.Insert(entity, trans);
                    }

                    trans.Commit();
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

        public bool DeleteThongTinHopDong(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NH_DA_HopDong SET bIsActive = 1 WHERE ID = (SELECT iID_ParentAdjustID FROM NH_DA_HopDong WHERE ID = @iId);");
                    query.AppendLine("DELETE NH_DA_HopDong WHERE ID = @iId");
                    conn.Execute(query.ToString(), param: new { iId = iId }, trans, commandType: CommandType.Text);

                    trans.Commit();
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

        public IEnumerable<NH_DM_LoaiHopDong> GetNHDMLoaiHopDongList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DM_LoaiHopDong ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.Append("ORDER BY iThuTu");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_LoaiHopDong>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetNHKeHoachChiTietBQPNhiemVuChiList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_KHChiTietBQP_NhiemVuChi nvc ");
            query.AppendLine("WHERE 1=1 ");
            if (id != null)
            {
                query.AppendLine("AND ID = @ID ");
            }
            query.AppendLine("AND NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi nv WHERE nvc.ID = nv.iID_ParentID) ");

            query.Append("ORDER BY sMaThuTu");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_KHChiTietBQP_NhiemVuChi>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DA_DuAn> GetNHDADuAnList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DA_DuAn ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY sTenDuAn");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_DuAn>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_NhaThau> GetNHDMNhaThauList(Guid? id = null, int? iLoai = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_NhaThau ");
            query.AppendLine("WHERE 1=1 ");
            if (id != null)
            {
                query.AppendLine("AND Id = @ID ");
            }

            if (iLoai != null && iLoai.HasValue)
            {
                switch (iLoai.Value)
                {
                    case 1:
                    case 2:
                        query.AppendLine("AND iLoai = 1 ");
                        break;
                    case 3:
                    case 4:
                        query.AppendLine("AND iLoai = 2 ");
                        break;
                    default:
                        break;
                }
            }

            query.AppendLine("ORDER BY sTenNhaThau");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_NhaThau>(query.ToString(),
                    param: (id != null ? new { Id = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_TiGia> GetNHDMTiGiaList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_TiGia ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY sMaTiGia");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_TiGia>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_TiGia_ChiTiet> GetNHDMTiGiaChiTietList(Guid? iDTiGia, bool isMaNgoaiTeKhac = true)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT tgct.* FROM NH_DM_TiGia_ChiTiet tgct ");
            query.AppendLine("INNER JOIN NH_DM_TiGia tg ON tgct.iID_TiGiaID = tg.ID ");
            query.AppendLine("WHERE 1 = 1 ");
            if (isMaNgoaiTeKhac)
            {
                query.AppendLine("AND UPPER(tgct.sMaTienTeQuyDoi) NOT IN ('USD', 'VND', 'EUR') ");
            }

            if (iDTiGia != null && iDTiGia != Guid.Empty)
            {
                query.AppendLine(" AND tgct.iID_TiGiaID = @iDTiGia");
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_TiGia_ChiTiet>(query.ToString(),
                    param: iDTiGia != null ? new { iDTiGia = iDTiGia } : null,
                    commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<NH_DA_HopDong> GetListHopDong()
        {
            var sql = "SELECT * FROM NH_DA_HopDong ORDER BY ID;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_HopDong>(sql);
                return items;
            }
        }
        public IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetNHNhiemVuChiTietTheoDonViId(string maDonVi, Guid? donViID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT nvc.* FROM NH_KHChiTietBQP_NhiemVuChi nvc ");
            if (!string.IsNullOrEmpty(maDonVi) && donViID != null && donViID != Guid.Empty)
            {
                query.AppendLine("INNER JOIN NS_DonVi dv ON dv.iID_MaDonVi = nvc.iID_MaDonVi AND dv.iID_Ma = nvc.iID_DonViID ");
                query.AppendLine("WHERE dv.iID_Ma = @donViID AND dv.iID_MaDonVi = @maDonVi ");
                query.AppendLine("AND NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi nv WHERE nvc.ID = nv.iID_ParentID) ");
            }
            else
            {
                query.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi nv WHERE nvc.ID = nv.iID_ParentID) ");
            }
            query.AppendLine("ORDER BY nvc.sMaThuTu");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_KHChiTietBQP_NhiemVuChi>(
                    query.ToString(),
                    param: (!string.IsNullOrEmpty(maDonVi) && donViID != null && donViID != Guid.Empty)
                        ? new { donViID = donViID, maDonVi = maDonVi }
                        : null,
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DA_DuAn> GetNHDuAnTheoKHCTBQPChuongTrinhId(Guid? chuongTrinhID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT da.* FROM NH_DA_DuAn da ");
            if (chuongTrinhID != null && chuongTrinhID != Guid.Empty)
            {
                query.AppendLine("INNER JOIN NH_KHChiTietBQP_NhiemVuChi nvc ON nvc.ID = da.iID_KHCTBQP_ChuongTrinhID ");
                query.AppendLine("WHERE nvc.ID = @chuongTrinhID ");
            }

            query.AppendLine("ORDER BY da.sTenDuAn");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_DuAn>(
                    query.ToString(),
                    param: (chuongTrinhID != null && chuongTrinhID != Guid.Empty)
                        ? new { chuongTrinhID = chuongTrinhID }
                        : null,
                    commandType: CommandType.Text);
                return items;
            }
        }

        public bool SaveImportThongTinHopDong(List<NH_DA_HopDong> contractList)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    conn.Insert<NH_DA_HopDong>(contractList, trans);
                    trans.Commit();
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
        #endregion

        #region QLNH - Phân cấp phê duyệt
        public IEnumerable<DanhmucNgoaiHoi_PhanCapPheDuyetModel> getListPhanCapPheDuyetModels(ref PagingInfo _paging,
            string sMa, string sTenVietTat,
            string sMoTa, string sTen)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMa", sMa);
                lstPrams.Add("sTenVietTat", sTenVietTat);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("sTen", sTen);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_PhanCapPheDuyetModel>("proc_get_all_phancappheduyet_paging",
                    lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public bool SavePhanCapPheDuyet(NH_DM_PhanCapPheDuyet data, string username)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_PhanCapPheDuyet();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = username;
                        conn.Insert<NH_DM_PhanCapPheDuyet>(entity, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NH_DM_PhanCapPheDuyet>(data.ID, trans);
                        if (entity != null)
                        {
                            data.dNgayTao = entity.dNgayTao;
                            data.sNguoiTao = entity.sNguoiTao;
                            data.iThuTu = entity.iThuTu;
                        }
                        conn.Update<NH_DM_PhanCapPheDuyet>(data, trans);
                    }

                    trans.Commit();
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

        public DanhmucNgoaiHoi_PhanCapPheDuyetModel GetPhanCapPheDuyetById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_PhanCapPheDuyet WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_PhanCapPheDuyetModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public bool DeletePhanCapPheDuyet(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_PhanCapPheDuyet>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_PhanCapPheDuyet>(checkExist, trans);
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

        public IEnumerable<NH_DM_PhanCapPheDuyet> GetNHDMPhanCapPheDuyetList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_PhanCapPheDuyet ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY sMa");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_PhanCapPheDuyet>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Loại hợp đồng
        public IEnumerable<DanhmucNgoaiHoi_LoaiHopDongModel> getListLoaiHopDongModels(ref PagingInfo _paging,
            string sMaLoaiHopDong, string sTenVietTat, string sTenLoaiHopDong, string sMoTa)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaLoaiHopDong", sMaLoaiHopDong);
                lstPrams.Add("sTenVietTat", sTenVietTat);
                lstPrams.Add("sTenLoaiHopDong", sTenLoaiHopDong);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_LoaiHopDongModel>("proc_get_all_danhmucloaiHopDong_paging",
                    lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhmucNgoaiHoi_LoaiHopDongModel GetLoaiHopDongById(Guid iId)
        {
            var sql = "select * from NH_DM_LoaiHopDong where ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_LoaiHopDongModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveLoaiHopDong(NH_DM_LoaiHopDong data)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_LoaiHopDong();
                        entity.MapFrom(data);
                        conn.Insert<NH_DM_LoaiHopDong>(entity, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NH_DM_LoaiHopDong>(data.ID, trans);
                        if (entity != null)
                        {
                            data.dNgayTao = entity.dNgayTao;
                            data.sNguoiTao = entity.sNguoiTao;
                        }
                        conn.Update<NH_DM_LoaiHopDong>(data, trans);
                    }

                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                if (trans != null) trans.Rollback();
            }

            return false;
        }

        public bool DeleteLoaiHopDong(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_LoaiHopDong>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_LoaiHopDong>(checkExist, trans);
                        trans.Commit();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                if (trans != null) trans.Rollback();
            }

            return false;
        }
        #endregion

        #region QLNH - Kế hoạch tổng thể TTCP
        // Get danh sách kế hoạch tổng thể TTCP
        public NH_KHTongTheTTCPViewModel getListKHTongTheTTCP(PagingInfo _paging, string sSoKeHoach, DateTime? dNgayBanHanh, int? from, int? to)
        {
            var result = new NH_KHTongTheTTCPViewModel();
            using (var connection = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("From", from);
                lstPrams.Add("To", to);
                lstPrams.Add("sSoKeHoach", sSoKeHoach);
                lstPrams.Add("dNgayBanHanh", dNgayBanHanh);
                lstPrams.Add("SkipCount", (_paging.CurrentPage - 1) * _paging.ItemsPerPage);
                lstPrams.Add("MaxResultCount", _paging.ItemsPerPage);
                lstPrams.Add("TotalItems", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = connection.Query<NH_KHTongTheTTCPModel>("sp_get_all_KeHoachTongTheTTCP", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("TotalItems");
                result._paging = _paging;
                result.Items = items.ToList();
            }

            return result;
        }

        // Get lookup kế hoạch tổng thể TTCP loại = 1 (theo giai đoạn)
        public IEnumerable<LookupKHTTCP> getLookupKHTTCPByStage()
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT IIF(iLoai = 1, 
			                            CONCAT('KHTT ', iGiaiDoanTu, ' - ', iGiaiDoanDen, N' -  Số KH: ', sSoKeHoach), 
			                            CONCAT('KHTT ', iNamKeHoach, N' -  Số KH:', sSoKeHoach)) AS DisplayName, ID AS Id, iGiaiDoanTu, iGiaiDoanDen
                            FROM NH_KHTongTheTTCP WHERE iLoai = 1 AND bIsActive = 1";
                return connection.Query<LookupKHTTCP>(query, commandType: CommandType.Text);
            }
        }

        // Lấy danh sách nhiệm vụ chi
        public NH_KHTongTheTTCP_NVCViewModel GetDetailKeHoachTongTheTTCP(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                NH_KHTongTheTTCP_NVCViewModel result = new NH_KHTongTheTTCP_NVCViewModel();
                result.Items = new List<NH_KHTongTheTTCP_NVCModel>();

                if (state == "CREATE" && KHTTCP_ID.HasValue && KHTTCP_ID != Guid.Empty)
                {
                    // Lấy thêm các nhiệm vụ chi của TTCP trong DB
                    var lstPrams = new DynamicParameters();
                    lstPrams.Add("KHTTCP_ID", KHTTCP_ID.Value);
                    result.Items = conn.Query<NH_KHTongTheTTCP_NVCModel>("sp_get_create_KeHoachTongTheTTCP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                }
                else if (state == "DETAIL" && KHTTCP_ID.HasValue && KHTTCP_ID != Guid.Empty)
                {
                    // Nếu là update thì lấy toàn bộ data trong DB
                    var queryInfo = @"
                    SELECT
		                TTCP.ID AS ID,
		                TTCP.iGiaiDoanTu AS iGiaiDoanTu,
		                TTCP.iGiaiDoanDen AS iGiaiDoanDen,
		                TTCP.iNamKeHoach AS iNamKeHoach,
		                TTCP.sSoKeHoach AS sSoKeHoach,
		                TTCP.dNgayKeHoach AS dNgayKeHoach,
		                TTCP.iLoai AS iLoai
                    FROM NH_KHTongTheTTCP TTCP
                    WHERE TTCP.ID = @Id";
                    result = conn.QueryFirstOrDefault<NH_KHTongTheTTCP_NVCViewModel>(queryInfo, new { Id = KHTTCP_ID.Value }, commandType: CommandType.Text);

                    // Lấy các nhiệm vụ chi TTCP cha
                    var lstPrams = new DynamicParameters();
                    lstPrams.Add("KHTTCP_ID", KHTTCP_ID.Value);
                    lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID);
                    lstPrams.Add("sTenNhiemVuChi", sTenNhiemVuChi);
                    lstPrams.Add("sTenPhongBan", sTenPhongBan);
                    result.Items = conn.Query<NH_KHTongTheTTCP_NVCModel>("sp_get_detail_KeHoachTongTheTTCP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                }
                else if ((state == "UPDATE" || state == "ADJUST") && KHTTCP_ID.HasValue && KHTTCP_ID != Guid.Empty)
                {
                    // Lấy các nhiệm vụ chi TTCP cha
                    var lstPrams = new DynamicParameters();
                    lstPrams.Add("KHTTCP_ID", KHTTCP_ID.Value);
                    lstPrams.Add("sTenNhiemVuChi", sTenNhiemVuChi);
                    lstPrams.Add("sTenPhongBan", sTenPhongBan);
                    result.Items = conn.Query<NH_KHTongTheTTCP_NVCModel>("sp_get_detail_KeHoachTongTheTTCP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                }

                return result;
            }
        }

        // Lấy thông tin kế hoạch tổng thể TTCP theo id
        public NH_KHTongTheTTCP Get_KHTT_TTCP_ById(Guid iId)
        {
            var sql = "SELECT * FROM NH_KHTongTheTTCP WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_KHTongTheTTCP>(sql, param: new { iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<NH_KHTongTheTTCPModel> Get_KHTT_TTCP_ListActive()
        {
            var sql = "select * from NH_KHTongTheTTCP where bIsActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCPModel>(sql, commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveKeHoachTTCP(NH_KHTongTheTTCP data, string sUsername)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    bool result = false;
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        data.dNgayTao = DateTime.Now;
                        data.sNguoiTao = sUsername;
                        data.bIsActive = true;
                        Guid idNew = conn.Insert<NH_KHTongTheTTCP>(data, trans);
                        result = idNew != Guid.Empty;
                    }
                    else
                    {
                        data.dNgaySua = DateTime.Now;
                        data.sNguoiSua = sUsername;

                        result = conn.Update<NH_KHTongTheTTCP>(data, trans);
                    }

                    if (result)
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        // Xóa kế hoạch TTCP + Nhiệm vụ chi
        public bool DeleteKeHoachTTCP(Guid id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    // Xóa nhiệm vụ chi
                    var deleteNVC = @"DELETE FROM NH_KHTongTheTTCP_NhiemVuChi WHERE iID_KHTongTheID = @Id";
                    conn.Execute(deleteNVC, new { Id = id }, trans);

                    // Update active và xóa kế hoạch chi tiêt BQP
                    StringBuilder query = new StringBuilder(@"UPDATE NH_KHTongTheTTCP SET bIsActive = 1 WHERE ID = (SELECT iID_ParentAdjustID FROM NH_KHTongTheTTCP WHERE ID = @Id);");
                    query.Append(@"DELETE NH_KHTongTheTTCP WHERE ID = @Id");
                    conn.Execute(query.ToString(), new { Id = id }, trans, commandType: CommandType.Text);

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        // Kiểm tra xem thằng TTCP hiện tại có đang active không?
        public bool? CheckKHTongTheTTCPIsActive(Guid id)
        {
            // Return true thì sẽ disable update lên bản mới nhất.
            try
            {
                var sql = "SELECT bIsActive FROM NH_KHTongTheTTCP WHERE ID = @id";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var check = conn.QueryFirstOrDefault<bool?>(sql, new { id = id }, commandType: CommandType.Text);
                    return check.HasValue ? check.Value : (bool?)null;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return (bool?)null;
            }
        }

        // Tìm thằng TTCP đang active dựa vào 1 thằng TTCP inactive
        public NH_KHTongTheTTCP FindParentTTCPActive(Guid id)
        {
            var query = @"SELECT * FROM NH_KHTongTheTTCP
                            WHERE iID_GocID = (SELECT IIF(bIsGoc = 1, ID, iID_GocID) AS ID FROM NH_KHTongTheTTCP WHERE ID = @id)
                            AND bIsActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                return conn.QueryFirstOrDefault<NH_KHTongTheTTCP>(query, new { id = id }, commandType: CommandType.Text);
            }
        }
        #endregion

        #region QLNH - Kế hoạch chỉ tiết TTCP

        // Lưu Kế hoạch
        public Boolean SaveKHTongTheTTCP(List<NH_KHTongTheTTCP_NhiemVuChiDto> lstNhiemVuChis, NH_KHTongTheTTCP khtt, string state)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    if (state == "CREATE" || state == "ADJUST")
                    {
                        if (state == "ADJUST")
                        {
                            // Update bản ghi cha
                            var queryKHCTOld = @"SELECT * FROM NH_KHTongTheTTCP WHERE ID = @Id";
                            var khttOld = conn.QueryFirstOrDefault<NH_KHTongTheTTCP>(queryKHCTOld, new { Id = khtt.iID_ParentAdjustID }, trans);
                            khttOld.bIsActive = false;
                            conn.Update(khttOld, trans);

                            // Check để lấy GocID
                            if (khttOld.bIsGoc)
                            {
                                khtt.iID_GocID = khttOld.ID;
                            }
                            else
                            {
                                khtt.iID_GocID = khttOld.iID_GocID;
                            }
                        }

                        // Insert
                        khtt.ID = Guid.Empty;
                        conn.Insert(khtt, trans);

                        // Convert data nhiệm vụ chi
                        var lstNVCInserts = new List<NH_KHTongTheTTCP_NhiemVuChi>();
                        foreach (var nvcDto in lstNhiemVuChis)
                        {
                            var nvc = new NH_KHTongTheTTCP_NhiemVuChi();
                            nvc.iID_KHTongTheID = khtt.ID;
                            nvc.sTenNhiemVuChi = HttpUtility.HtmlDecode(nvcDto.sTenNhiemVuChi);
                            nvc.iID_BQuanLyID = nvcDto.iID_BQuanLyID;
                            nvc.fGiaTri = double.TryParse(nvcDto.sGiaTri, NumberStyles.Float, new CultureInfo("en-US"), out double gtusd) ? gtusd : (double?)null;
                            nvc.sMaThuTu = nvcDto.sMaThuTu;
                            nvc.iID_ParentAdjustID = (state == "ADJUST" && nvcDto.ID != Guid.Empty) ? nvcDto.ID : (Guid?)null;

                            lstNVCInserts.Add(nvc);
                        }

                        // Exec insert data nhiệm vụ chi
                        //lstNVCInserts.OrderBy(x => x.sMaThuTu).ToList();
                        foreach (var nvc in lstNVCInserts)
                        {
                            // Nếu chưa được insert thì insert và đã có parentID thì insert luôn
                            if (nvc.ID == Guid.Empty && nvc.iID_ParentID.HasValue)
                            {
                                conn.Insert(nvc, trans);
                            }
                            else if (nvc.ID == Guid.Empty && !nvc.iID_ParentID.HasValue)
                            {
                                // Nếu chưa được insert và chưa có parentId luôn thì tìm thằng cha để insert thằng cha trước
                                var indexOfDot = nvc.sMaThuTu.LastIndexOf(".");
                                if (indexOfDot == -1)
                                {
                                    // Nếu không có thằng cha thì insert luôn.
                                    conn.Insert(nvc, trans);
                                }
                                else
                                {
                                    // Lấy mã thứ tự của bản ghi cha.
                                    var sttParent = nvc.sMaThuTu.Substring(0, indexOfDot);
                                    // Tìm bản ghi cha
                                    var parent = lstNVCInserts.FirstOrDefault(x => x.sMaThuTu == sttParent);
                                    // Nếu tìm không thấy thằng cha thì insert luôn
                                    if (parent == null)
                                    {
                                        conn.Insert(nvc, trans);
                                    }
                                    else
                                    {
                                        // Nếu tìm thấy thằng cha thì ném vào đệ quy để check xem nó đã được insert hay chưa rồi lấy id của thằng cha.
                                        nvc.iID_ParentID = GetIdKHTHTTCPNhiemVuChiParent(conn, trans, parent, ref lstNVCInserts);
                                        conn.Insert(nvc, trans);
                                    }
                                }
                            }
                        }

                        // Sau khi insert thì tính tổng giá trị của BQP
                        double fTongGiaTri = 0;
                        foreach (var nvc in lstNVCInserts)
                        {
                            if (!nvc.iID_ParentID.HasValue)
                            {
                                fTongGiaTri += nvc.fGiaTri.HasValue ? nvc.fGiaTri.Value : 0;
                            }
                        }

                        khtt.fTongGiaTri = fTongGiaTri;
                        conn.Update(khtt, trans);
                    }
                    else if (state == "UPDATE")
                    {
                        // Update KH Chi tiết.
                        var queryKhctGoc = @"SELECT * FROM NH_KHTongTheTTCP WHERE ID = @Id";
                        var khctGoc = conn.QueryFirstOrDefault<NH_KHTongTheTTCP>(queryKhctGoc, new { Id = khtt.ID }, trans);

                        khctGoc.iLoai = khtt.iLoai;
                        khctGoc.iGiaiDoanTu = khtt.iGiaiDoanTu;
                        khctGoc.iGiaiDoanDen = khtt.iGiaiDoanDen;
                        khctGoc.iNamKeHoach = khtt.iNamKeHoach;
                        khctGoc.iID_ParentID = khtt.iID_ParentID;
                        khctGoc.sSoKeHoach = HttpUtility.HtmlDecode(khtt.sSoKeHoach);
                        khctGoc.dNgayKeHoach = khtt.dNgayKeHoach;
                        khctGoc.sMoTaChiTiet = khtt.sMoTaChiTiet;
                        khctGoc.dNgaySua = khtt.dNgaySua;
                        khctGoc.sNguoiSua = khtt.sNguoiSua;

                        conn.Update(khctGoc, trans);

                        // Update nhiệm vụ chi
                        var queryListIdNVC = @"SELECT ID 
                            FROM NH_KHTongTheTTCP_NhiemVuChi 
                            WHERE iID_KHTongTheID = @Id";
                        var lstIdNVC = conn.Query<Guid>(queryListIdNVC, new { Id = khctGoc.ID }, trans).ToList();

                        // Convert data nhiệm vụ chi
                        var lstNVCUpdate = new List<NH_KHTongTheTTCP_NhiemVuChi>();
                        var lstNVCAdd = new List<NH_KHTongTheTTCP_NhiemVuChi>();
                        double fTongGiaTri = 0;
                        foreach (var nvcDto in lstNhiemVuChis)
                        {
                            var nvc = new NH_KHTongTheTTCP_NhiemVuChi();
                            nvc.ID = nvcDto.ID;
                            nvc.iID_KHTongTheID = khctGoc.ID;
                            nvc.sTenNhiemVuChi = HttpUtility.HtmlDecode(nvcDto.sTenNhiemVuChi);
                            nvc.iID_BQuanLyID = nvcDto.iID_BQuanLyID;
                            nvc.fGiaTri = double.TryParse(nvcDto.sGiaTri, NumberStyles.Float, new CultureInfo("en-US"), out double gtusd) ? gtusd : (double?)null;
                            nvc.sMaThuTu = nvcDto.sMaThuTu;
                            nvc.iID_ParentID = nvcDto.iID_ParentID;
                            if (!nvcDto.iID_ParentID.HasValue)
                            {
                                fTongGiaTri += nvc.fGiaTri.HasValue ? nvc.fGiaTri.Value : 0;
                            }
                            if (nvcDto.isAdd)
                            {
                                conn.Insert(nvc, trans);
                                //lstNVCAdd.Add(nvc);
                            }
                            else
                            {
                                conn.Update(nvc, trans);
                                lstIdNVC.Remove(nvc.ID);
                            }
                        }
                        khctGoc.fTongGiaTri = fTongGiaTri;
                        conn.Update(khctGoc, trans);
                        //if (lstNVCAdd.Count > 0)
                        //{
                        //    conn.Insert(lstNVCAdd, trans);
                        //}
                        //if (lstNVCUpdate.Count > 0)
                        //{
                        //    conn.Update(lstNVCUpdate, trans);
                        //}

                        //// Check có ID thì update, ko có ID thì insert vào.
                        //foreach (var nvc in lstNVCUpdate)
                        //{
                        //    if (nvc.ID != Guid.Empty)
                        //    {
                        //        conn.Update(nvc, trans);
                        //        lstIdNVC.Remove(nvc.ID);
                        //    }
                        //    else
                        //    {
                        //        // Nếu đã có parentID thì insert luôn
                        //        if (nvc.iID_ParentID.HasValue)
                        //        {
                        //            conn.Insert(nvc, trans);
                        //        }
                        //        else if (!nvc.iID_ParentID.HasValue)
                        //        {
                        //            // Nếu chưa có parentId thì tìm thằng cha để insert thằng cha trước
                        //            var indexOfDot = nvc.sMaThuTu.LastIndexOf(".");
                        //            if (indexOfDot == -1)
                        //            {
                        //                // Nếu không có thằng cha thì insert luôn.
                        //                conn.Insert(nvc, trans);
                        //            }
                        //            else
                        //            {
                        //                // Lấy mã thứ tự của bản ghi cha.
                        //                var sttParentNvc = nvc.sMaThuTu.Substring(0, indexOfDot);
                        //                // Tìm bản ghi cha
                        //                var parent = lstNVCUpdate.FirstOrDefault(x => x.sMaThuTu == sttParentNvc);
                        //                // Nếu tìm không thấy thằng cha thì insert luôn
                        //                if (parent == null)
                        //                {
                        //                    conn.Insert(nvc, trans);
                        //                }
                        //                else
                        //                {
                        //                    // Nếu tìm thấy thằng cha thì ném vào đệ quy để check xem nó đã được insert hay chưa rồi lấy id của thằng cha.
                        //                    nvc.iID_ParentID = GetIdKHTHTTCPNhiemVuChiParent(conn, trans, parent, ref lstNVCUpdate);
                        //                    conn.Insert(nvc, trans);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        // Còn những thằng nào dư ra thì delete
                        foreach (var idDelete in lstIdNVC)
                        {
                            var nvcTemp = new NH_KHTongTheTTCP_NhiemVuChi();
                            nvcTemp.ID = idDelete;
                            conn.Delete(nvcTemp, trans);
                        }

                        // Sau khi insert thì tính tổng giá trị của BQP
                        //double fTongGiaTri = 0;
                        //foreach (var nvc in lstNVCUpdate)
                        //{
                        //    if (!nvc.iID_ParentID.HasValue)
                        //    {
                        //        fTongGiaTri += nvc.fGiaTri.HasValue ? nvc.fGiaTri.Value : 0;
                        //    }
                        //}

                        //khctGoc.fTongGiaTri = fTongGiaTri;
                        //conn.Update(khctGoc, trans);
                    }

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChi_Parent> Get_KHCT_TTCP_GetListOfParent(Guid khtt_id, Guid program_id)
        {
            var sql = @"select  nvc.sMaThuTu, nvc.ID, nvc.sTenNhiemVuChi, 
                                nvc.iID_BQuanLyID, nvc.fGiaTri , nvc.iID_ParentID,
		                        pb.iID_MaPhongBan as BQuanLyID, concat(pb.sTen, ' - ', pb.sMoTa) BQuanLy
                        from NH_KHTongTheTTCP_NhiemVuChi nvc
                        join NS_PhongBan pb on nvc.iID_BQuanLyID = pb.iID_MaPhongBan
                        where nvc.iID_KHTongTheID = @khtt_id and iID_ParentID = @program_id order by 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCP_NhiemVuChi_Parent>(sql, param: new { khtt_id, program_id }, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChi> Get_KHCT_TTCP_GetListProgramByPlanID(Guid khtt_id)//Lấy ds chương trình
        {
            var sql = @"SELECT *
                          FROM NH_KHTongTheTTCP_NhiemVuChi
                          where iID_KHTongTheID = @khtt_id order by sMaThuTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCP_NhiemVuChi>(sql, param: new { khtt_id }, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NH_KHTongTheTTCP_NhiemVuChi> Get_KHCT_TTCP_GetListMissionByPlanIdAndProgramId(Guid khtt_id, Guid program_id)//Lấy ds nhiệm vụ chi
        {
            var sql = @"SELECT *
                          FROM NH_KHTongTheTTCP_NhiemVuChi
                          where iID_KHTongTheID = @khtt_id and iID_ParentID = @show_id order by sMaThuTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCP_NhiemVuChi>(sql, param: new { khtt_id, program_id }, commandType: CommandType.Text);
                return item;
            }
        }
        public NH_KHTongTheTTCP_NhiemVuChi GetNhiemVuChiById(Guid ID)
        {
            try
            {
                var sql = "SELECT * FROM NH_KHTongTheTTCP_NhiemVuChi WHERE ID = @ID";
                using (var conn = _connectionFactory.GetConnection())
                {
                    var item = conn.QueryFirstOrDefault<NH_KHTongTheTTCP_NhiemVuChi>(sql, param: new { ID },
                        commandType: CommandType.Text);
                    return item;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }
        public IEnumerable<NH_KHTongTheTTCP_BQL> GetListDM_BQL()
        {
            var sql = @"select iID_MaPhongBan as BQuanLyID, 
                               concat (sTen, ' - ', sMoTa) BQuanLy 
                        from NS_PhongBan 
                        where iTrangThai = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCP_BQL>(sql, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NH_KHTongTheTTCP_SoKeHoach> GetListKHTT_ActiveWithNumber()
        {
            var sql = @"select ID KHTTCP_ID, 
                               concat (sSoKeHoach, ' - ', FORMAT(dNgayKeHoach,'dd/MM/yyyy')) KHTTCP 
                        from NH_KHTongTheTTCP 
                        where bIsActive = 1";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KHTongTheTTCP_SoKeHoach>(sql, commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveKeHoachTTCP_NVC(NH_KHTongTheTTCP_NhiemVuChi data, string sUsername)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    bool result = false;
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        Guid idNew = conn.Insert<NH_KHTongTheTTCP_NhiemVuChi>(data, trans);
                        result = idNew != Guid.Empty;
                    }
                    else
                    {
                        result = conn.Update<NH_KHTongTheTTCP_NhiemVuChi>(data, trans);
                    }

                    if (result)
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        // Insert nhiệm vụ chi và ném ra ID của thằng cha
        private Guid? GetIdKHTHTTCPNhiemVuChiParent(SqlConnection conn, SqlTransaction trans, NH_KHTongTheTTCP_NhiemVuChi nvc, ref List<NH_KHTongTheTTCP_NhiemVuChi> lstNhiemVuChis)
        {
            // Nếu thằng cha này đã insert thì ném ra ID của thằng cha, chưa insert thì check tiếp.
            if (nvc.ID == Guid.Empty)
            {
                // Nếu thằng cha đã có ParentID thì insert luôn, ko thì check tiếp.
                if (!nvc.iID_ParentID.HasValue)
                {
                    // Tìm bản ghi cha dựa vào mã thứ tự
                    var indexOfDot = nvc.sMaThuTu.LastIndexOf(".");
                    if (indexOfDot == -1)
                    {
                        // Nếu không có parent thì insert luôn.
                        conn.Insert(nvc, trans);
                    }
                    else
                    {
                        // Lấy mã thứ tự của bản ghi cha.
                        var sttParent = nvc.sMaThuTu.Substring(0, indexOfDot);
                        // Tìm bản ghi cha
                        var parent = lstNhiemVuChis.FirstOrDefault(x => x.sMaThuTu == sttParent);
                        // Nếu tìm ko ra thằng cha thì insert luôn
                        if (parent == null)
                        {
                            conn.Insert(nvc, trans);
                        }
                        else
                        {
                            // Nếu tìm thấy thì ném lại vào đệ quy để check lại, nếu đã insert thì return ra id thằng cha đó luôn, nếu chưa thì check tiếp.
                            return GetIdKHTHTTCPNhiemVuChiParent(conn, trans, parent, ref lstNhiemVuChis);
                        }
                    }
                }
                else
                {
                    conn.Insert(nvc, trans);
                }
            }

            // Sau khi đã thực hiện N thao tác thì đã insert được thằng cha, nên giờ chỉ cần ném Id của nó ra thôi.
            return nvc.ID;
        }
        #endregion

        #region QLNH - Quyết toán niên độ
        public IEnumerable<NH_QT_QuyetToanNienDoData> GetListQuyetToanNienDo(ref PagingInfo _paging, string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamKeHoach, int? tabIndex)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoDeNghi", sSoDeNghi);
                lstParam.Add("dNgayDeNghi", dNgayDeNghi);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("tabIndex", tabIndex);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_QT_QuyetToanNienDoData>("proc_get_all_quyettoanniendo_paging", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }
        public IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetDetailQuyetToanNienDoCreate(int? iNamKeHoach, Guid? iIDDonVi, int? donViTinhUSD, int? donViTinhVND)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIDDonVi", iIDDonVi);
                lstParam.Add("devideDonViUSD", donViTinhUSD);
                lstParam.Add("devideDonViVND", donViTinhVND);

                var items = conn.Query<NH_QT_QuyetToanNienDo_ChiTietData>("proc_get_all_nh_baocaoquyettoanniendo_paging", lstParam,
                    commandType: CommandType.StoredProcedure).OrderBy(x => x.iID_DonVi);
                return items;
            }
        }
        public IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetDetailQuyetToanNienDoDetail(int? iNamKeHoach, Guid? iIDDonVi, Guid? iIDQuyetToan, int? donViTinhUSD, int? donViTinhVND)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIDDonVi", iIDDonVi);
                lstParam.Add("iIDQuyetToan", iIDQuyetToan);
                lstParam.Add("devideDonViUSD", donViTinhUSD);
                lstParam.Add("devideDonViVND", donViTinhVND);

                var items = conn.Query<NH_QT_QuyetToanNienDo_ChiTietData>("proc_get_all_nh_baocaoquyettoanniendo_detail", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
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

        public NH_QT_QuyetToanNienDoData GetThongTinQuyetToanNienDoById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_QT_QuyetToanNienDo WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_QT_QuyetToanNienDoData>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public NH_QT_QuyetToanNienDoReturnData SaveQuyetToanNienDo(NH_QT_QuyetToanNienDo data, string userName)
        {
            NH_QT_QuyetToanNienDoReturnData dt = new NH_QT_QuyetToanNienDoReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NH_QT_QuyetToanNienDo> item = new List<NH_QT_QuyetToanNienDo>();
                    if (data.iID_DonViID != null)
                    {
                        StringBuilder query = new StringBuilder();
                        query.Append("select * from NH_QT_QuyetToanNienDo where iNamKeHoach = " + data.iNamKeHoach + " and iID_DonViID ='" + data.iID_DonViID + "' and ID <> '" + data.ID + "'");
                        item = conn.Query<NH_QT_QuyetToanNienDo>(query.ToString(), commandType: CommandType.Text).ToList();
                    }

                    var trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_QT_QuyetToanNienDo();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = userName;
                        entity.bIsKhoa = false;
                        entity.bIsXoa = false;


                        if (item.Any())
                        {
                            dt.IsReturn = false;
                            dt.errorMess = "Đã tồn quyết toán niên độ của đơn vị trong năm!";
                            return dt;
                        }
                        conn.Insert(entity, trans);
                        dt.QuyetToanNienDoData = entity;
                    }
                    else
                    {
                        var entity = conn.Get<NH_QT_QuyetToanNienDo>(data.ID, trans);
                        if (entity == null)
                        {
                            dt.IsReturn = false;
                            return dt;
                        }

                        entity.iID_DonViID = data.iID_DonViID;
                        entity.iID_MaDonVi = data.iID_MaDonVi;
                        entity.iID_TiGiaID = data.iID_TiGiaID;
                        entity.iNamKeHoach = data.iNamKeHoach;
                        entity.iLoaiQuyetToan = data.iLoaiQuyetToan;
                        entity.sSoDeNghi = data.sSoDeNghi;
                        entity.sMoTa = data.sMoTa;
                        entity.dNgayDeNghi = data.dNgayDeNghi;
                        entity.dNgaySua = DateTime.Now;
                        entity.sNguoiSua = userName;
                        conn.Update(entity, trans);
                        dt.QuyetToanNienDoData = entity;

                    }

                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }

        public NH_QT_QuyetToanNienDoChiTietReturnData SaveQuyetToanNienDoDetail(List<NH_QT_QuyetToanNienDo_ChiTiet> listData, string userName, bool isAddOrUpdate, Guid idQuyetToan)
        {
            NH_QT_QuyetToanNienDoChiTietReturnData dt = new NH_QT_QuyetToanNienDoChiTietReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    foreach (var data in listData)
                    {

                        if (data.ID == null || data.ID == Guid.Empty)
                        {
                            var entity = new NH_QT_QuyetToanNienDo_ChiTiet();
                            entity.MapFrom(data);
                            conn.Insert(entity, trans);
                            dt.QuyetToanNienDoChiTietData = entity;
                        }
                        else
                        {
                            var entity = conn.Get<NH_QT_QuyetToanNienDo_ChiTiet>(data.ID, trans);
                            if (entity == null)
                            {
                                dt.IsReturn = false;
                                return dt;
                            }
                            entity.MapFrom(data);
                            conn.Update(entity, trans);
                            dt.QuyetToanNienDoChiTietData = entity;
                        }
                    }

                    dt.IsReturn = true;
                    trans.Commit();

                    InsertNHTongHop_Tang(conn, trans, "QUYET_TOAN", isAddOrUpdate ? 2 : 1, idQuyetToan, null);
                    InsertNHTongHop_Giam(conn, trans, "QUYET_TOAN", isAddOrUpdate ? 2 : 1, idQuyetToan, null);

                    InsertNHTongHop_Giam(conn, trans, "QTND", isAddOrUpdate ? 2 : 1, idQuyetToan, null);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }
        public bool SaveTongHop(NH_QT_QuyetToanNienDo data, string userName, string listId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = new NH_QT_QuyetToanNienDo();
                    entity.MapFrom(data);
                    entity.sTongHop = listId;
                    entity.dNgayTao = DateTime.Now;
                    entity.sNguoiTao = userName;
                    entity.bIsKhoa = false;
                    entity.bIsXoa = false;
                    conn.Insert(entity, trans);

                    foreach (var id in listId.Split(','))
                    {
                        var entityCon = conn.Get<NH_QT_QuyetToanNienDo>(id, trans);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = entity.ID;
                        entityCon.dNgaySua = DateTime.Now;
                        entityCon.sNguoiSua = userName;
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

        public bool DeleteQuyetToanNienDo(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DELETE NH_QT_QuyetToanNienDo WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                DeleteNHTongHop_Giam(conn, "QUYET_TOAN", iId);
                DeleteNHTongHop_Giam(conn, "QTND", iId);

                var entity = conn.Get<NH_QT_QuyetToanNienDo>(iId);
                if (entity.sTongHop != null)
                {
                    foreach (var id in entity.sTongHop.Split(','))
                    {
                        var entityCon = conn.Get<NH_QT_QuyetToanNienDo>(id);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = null;
                        conn.Update(entityCon);
                    }
                }
                conn.Execute("DELETE NH_QT_QuyetToanNienDo_ChiTiet WHERE iID_QuyetToanNienDoID = @iId", param: new { iId = iId }, commandType: CommandType.Text);
                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool LockOrUnLockQuyetToanNienDo(Guid id, bool isLockOrUnLock)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var entity = conn.Get<NH_QT_QuyetToanNienDo>(id, trans);
                if (entity == null) return false;
                entity.bIsKhoa = isLockOrUnLock;
                conn.Update(entity, trans);

                trans.Commit();
                conn.Close();

                return true;
            }
        }

        public IEnumerable<NH_QT_QuyetToanNienDoData> GetListTongHopQuyetToanNienDo(string sSodenghi, DateTime? dNgaydenghi, Guid? iDonvi, int? iNam)
        {
            if (iDonvi == Guid.Empty)
            {
                iDonvi = null;
            }
            var sSoDeNghi = sSodenghi;
            var dNgayDeNghi = dNgaydenghi;
            var iID_DonViID = iDonvi;
            var iNamKeHoach = iNam;
            var sql =
                @"
                 SELECT DISTINCT qtnd.ID, qtnd.sSoDeNghi, qtnd.dNgayDeNghi , qtnd.iID_DonViID, qtnd.iNamKeHoach, qtnd.iID_TiGiaID
               ,concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi,
               qtnd.bIsKhoa,qtnd.iID_TongHopID,qtnd.sTongHop
                INTO #tmp
                from NH_QT_QuyetToanNienDo as qtnd  
                left join NS_DonVi dv on qtnd.iID_DonViID = dv.iID_Ma
                 WHERE
				(ISNULL(@sSoDeNghi,'') = '' OR qtnd.sSoDeNghi like CONCAT(N'%', @sSoDeNghi, N'%')) 
				 AND (ISNULL(@iNamKeHoach,0) = 0 OR iNamKeHoach = @iNamKeHoach)
				AND (@dNgayDeNghi is   null or qtnd.dNgayDeNghi = @dNgayDeNghi)
				AND (@iID_DonViID IS NULL OR qtnd.iID_DonViID = @iID_DonViID)
                ;
                WITH cte(ID, sSoDeNghi, dNgayDeNghi, iID_DonViID, iNamKeHoach, iID_TiGiaID,bIsKhoa,iID_TongHopID,sTongHop)
                AS
                (
	                SELECT 
					  lct.ID
					, lct.sSoDeNghi
					, lct.dNgayDeNghi 
					, lct.iID_DonViID
					, lct.iNamKeHoach
					, lct.iID_TiGiaID
					, lct.bIsKhoa
					, lct.iID_TongHopID
					, lct.sTongHop
	                FROM NH_QT_QuyetToanNienDo lct , #tmp tmp
	                WHERE lct.ID  = tmp.iID_TongHopID
	                UNION ALL
	                SELECT 
					  cd.ID
					, cd.sSoDeNghi
					, cd.dNgayDeNghi 
					, cd.iID_DonViID
					, cd.iNamKeHoach
					, cd.iID_TiGiaID
					, cd.bIsKhoa
					, cd.iID_TongHopID
					, cd.sTongHop
	                FROM cte as NCCQ, #tmp as cd
	                WHERE cd.iID_TongHopID = NCCQ.ID
                )
                SELECT DISTINCT 
				  cte.ID
				, cte.sSoDeNghi
				, dNgayDeNghi
				, iID_DonViID
				, iNamKeHoach
				, iID_TiGiaID
                , bIsKhoa
				, iID_TongHopID
				, sTongHop
				, concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi
				 INTO #db
                 FROM cte 
                left join NS_DonVi dv on cte.iID_DonViID = dv.iID_Ma 
                UNION ALL
                SELECT DISTINCT 
				  qtnd.ID
				, qtnd.sSoDeNghi
				, qtnd.dNgayDeNghi
				, qtnd.iID_DonViID
				, qtnd.iNamKeHoach
				, qtnd.iID_TiGiaID
				, qtnd.bIsKhoa
				, qtnd.iID_TongHopID
				, qtnd.sTongHop
				, concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi
                from NH_QT_QuyetToanNienDo as qtnd  
                inner join NH_QT_QuyetToanNienDo as cd on qtnd.ID = cd.iID_TongHopID
                left join NS_DonVi dv on qtnd.iID_DonViID = dv.iID_Ma
                where (ISNULL(@sSoDeNghi,'') = '' or qtnd.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%'))
	            and (@iID_DonViID is null or qtnd.iID_DonViID = @iID_DonViID) 
	            and (ISNULL(@iNamKeHoach,'') = '' or qtnd.iNamKeHoach = @iNamKeHoach) 
				and  qtnd.iID_TongHopID is null and ( qtnd.sTongHop is null or qtnd.sTongHop = '')
                Order by cte.iID_TongHopID

                Select db.* ,ROW_NUMBER() OVER (ORDER BY db.iID_TongHopID) AS sSTT from  #db  db
                DROP TABLE #tmp
                DROP TABLE #db


";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_QT_QuyetToanNienDoData>(sql,
                    param: new
                    {
                        sSoDeNghi,
                        dNgayDeNghi,
                        iID_DonViID,
                        iNamKeHoach
                    },
                     commandType: CommandType.Text
                 );

                return items;
            }
        }
        #endregion

        #region QLNH - Quyết toán tài sản
        public QuyetToan_ChungTuModelPaging GetListChungTuTaiSanModels(ref PagingInfo _paging, string sTenChungTu, string sSoChungTu, DateTime? dNgayChungTu)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sTenChungTu", sTenChungTu);
                lstPrams.Add("sSoChungTu", sSoChungTu);
                lstPrams.Add("dNgayChungTu", dNgayChungTu);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var items = conn.Query<NH_QT_ChungTuTaiSan>("proc_get_all_quyettoan_chungtutaisan_paging", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                QuyetToan_ChungTuModelPaging pagingItems = new QuyetToan_ChungTuModelPaging();
                pagingItems.Items = items;
                return pagingItems;
            }
        }

        public IEnumerable<NH_QT_TaiSanViewModel> GetListTaiSanModels(Guid? chungTuId)
        {
            if (!chungTuId.HasValue || chungTuId == Guid.Empty)
            {
                chungTuId = null;
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("IDChungTu", chungTuId);

                var items = conn.Query<NH_QT_TaiSanViewModel>("proc_get_all_quyettoan_taisan_paging", lstPrams, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public NH_QT_ChungTuTaiSan GetChungTuTaiSanById(Guid iId)
        {
            var sql = "SELECT * FROM NH_QT_ChungTuTaiSan WHERE ID=@iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_QT_ChungTuTaiSan>(sql, param: new { iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public List<GetTaiSan> getListTaiSan()
        {
            var sql = "SELECT ROW_NUMBER() OVER(ORDER BY dNgayTao) AS STT, * FROM NH_DM_LoaiTaiSan";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<GetTaiSan>(sql, param: new { }, commandType: CommandType.Text);
                return item.ToList();
            }
        }

        public bool DeleteChungTuTaiSan(Guid iId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var taiSanList = conn.Query<NH_QT_TaiSan>(@"SELECT * FROM NH_QT_TaiSan WHERE iID_ChungTuTaiSanID=@iId", new { iId = iId }, trans);
                    var chungTuTaiSan = conn.QueryFirstOrDefault<NH_QT_ChungTuTaiSan>(@"SELECT * FROM NH_QT_ChungTuTaiSan WHERE ID=@iId", new { iId = iId }, trans);
                    if (taiSanList.Any())
                    {
                        foreach (var item in taiSanList)
                        {
                            conn.Delete<NH_QT_TaiSan>(item, trans);
                        }
                    }
                    conn.Delete<NH_QT_ChungTuTaiSan>(chungTuTaiSan, trans);
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

        public bool DeleteTaiSan(Guid iId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var checkExist = conn.QueryFirstOrDefault<NH_QT_TaiSan>(@"SELECT * FROM NH_QT_TaiSan WHERE ID = @iId", new { iId = iId }, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_QT_TaiSan>(checkExist, trans);
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

        public bool SaveChungTuTaiSan(List<NH_QT_TaiSan> datats, NH_QT_ChungTuTaiSan datactts)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (datactts.ID == Guid.Empty || datactts.ID == null)
                    {
                        var id = Guid.NewGuid();
                        datactts.ID = id;
                        datactts.dNgayTao = DateTime.Now;
                        conn.Insert(datactts, trans);
                        if (datats != null)
                        {
                            foreach (var item in datats)
                            {
                                item.iID_ChungTuTaiSanID = id;
                                conn.Insert<NH_QT_TaiSan>(item, trans);
                            }
                        }
                    }
                    else
                    {
                        conn.Update<NH_QT_ChungTuTaiSan>(datactts, trans);
                        var query = @"SELECT ID FROM NH_QT_TaiSan WHERE iID_ChungTuTaiSanID = @Id";
                        List<Guid> taiSanIds = conn.Query<Guid>(query, new { Id = datactts.ID }, trans).ToList();
                        //conn.Update<NH_QT_ChungTuTaiSan>(datactts, trans);
                        if (datats != null)
                        {
                            foreach (var ts in datats)
                            {
                                ts.iID_ChungTuTaiSanID = datactts.ID;
                                if (ts.ID != Guid.Empty)
                                {
                                    var index = taiSanIds.FindIndex(x => x == ts.ID);
                                    conn.Update<NH_QT_TaiSan>(ts, trans);
                                    if (index != -1)
                                    {
                                        taiSanIds.RemoveAt(index);
                                    }
                                }
                                else
                                {
                                    conn.Insert<NH_QT_TaiSan>(ts, trans);
                                }
                            }
                        }

                        foreach (var item in taiSanIds)
                        {
                            var tsQuery = @"SELECT * FROM NH_QT_TaiSan WHERE ID = @Id";
                            var taiSan = conn.QueryFirstOrDefault<NH_QT_TaiSan>(tsQuery, new { Id = item }, trans);
                            conn.Delete(taiSan, trans);
                        }
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

        public bool SaveListLoaiTaiSan(List<NH_DM_LoaiTaiSan> data)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    List<Guid> taiSanIds = new List<Guid>();
                    var query = @"SELECT ID FROM NH_DM_LoaiTaiSan";
                    taiSanIds = conn.Query<Guid>(query, null, trans).ToList();

                    foreach (var item in data)
                    {
                        if (item.ID != Guid.Empty)
                        {
                            var index = taiSanIds.FindIndex(x => x == item.ID);
                            conn.Update<NH_DM_LoaiTaiSan>(item, trans);
                            if (index != -1)
                            {
                                taiSanIds.RemoveAt(index);
                            }
                        }
                        else
                        {
                            item.dNgayTao = DateTime.Now;
                            conn.Insert<NH_DM_LoaiTaiSan>(item, trans);
                        }
                    }
                    foreach (var itemts in taiSanIds)
                    {
                        var tsQuery = @"SELECT * FROM NH_DM_LoaiTaiSan WHERE ID = @Id";
                        var taiSan = conn.QueryFirstOrDefault<NH_DM_LoaiTaiSan>(tsQuery, new { Id = itemts }, trans);
                        conn.Delete(taiSan, trans);
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

        public IEnumerable<NH_DA_DuAn> GetLookupDuAn(Guid? id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NH_DA_DuAn> duAnList = new List<NH_DA_DuAn>();
                    if (id == null || id == Guid.Empty)
                    {
                        duAnList = conn.Query<NH_DA_DuAn>("SELECT ID, sTenDuAn FROM NH_DA_DuAn WHERE bIsActive = 1").ToList();
                    }
                    else
                    {
                        duAnList = conn.Query<NH_DA_DuAn>("SELECT ID, sTenDuAn FROM NH_DA_DuAn WHERE bIsActive = 1 AND iID_DonViID=@ID", new { ID = id }).ToList();
                    }
                    return duAnList;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<NH_DA_HopDong> GetLookupHopDong(Guid? id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NH_DA_HopDong> hopDongList = new List<NH_DA_HopDong>();
                    if (id == null || id == Guid.Empty)
                    {
                        hopDongList = conn.Query<NH_DA_HopDong>("SELECT ID, sTenHopDong FROM NH_DA_HopDong WHERE bIsActive = 1").ToList();
                    }
                    else
                    {
                        hopDongList = conn.Query<NH_DA_HopDong>("SELECT ID, sTenHopDong FROM NH_DA_HopDong WHERE bIsActive = 1 AND iID_DonViID=@ID", new { ID = id }).ToList();
                    }
                    return hopDongList;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }
        #endregion

        #region QLNH - Danh mục tài sản
        public IEnumerable<DanhmucNgoaiHoi_TaiSanModel> getListDanhMucTaiSanModels(ref PagingInfo _paging, string sMaLoaiTaiSan, string sTenLoaiTaiSan, string sMoTa)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaLoaiTaiSan", sMaLoaiTaiSan);
                lstPrams.Add("sTenLoaiTaiSan", sTenLoaiTaiSan);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_TaiSanModel>("proc_get_all_DanhMucTaiSan_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }
        public DanhmucNgoaiHoi_TaiSanModel GetTaiDanhMucSanById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_LoaiTaiSan WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_TaiSanModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public bool SaveTaiSan(NH_DM_LoaiTaiSan data, string username)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_LoaiTaiSan();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = username;
                        conn.Insert<NH_DM_LoaiTaiSan>(entity, trans);
                    }
                    else
                    {
                        var entity = conn.Get<NH_DM_LoaiTaiSan>(data.ID, trans);
                        if (entity != null)
                        {
                            data.dNgayTao = entity.dNgayTao;
                            data.sNguoiTao = entity.sNguoiTao;
                        }
                        conn.Update<NH_DM_LoaiTaiSan>(data, trans);
                    }

                    trans.Commit();
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

        public bool DeleteDanhMucTaiSan(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_LoaiTaiSan>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_LoaiTaiSan>(checkExist, trans);
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

        public IEnumerable<NH_DM_LoaiTaiSan> GetNHDMLoaiTaiSanList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_LoaiTaiSan ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY sMaLoaiTaiSan");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_LoaiTaiSan>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Báo cáo tài sản
        public IEnumerable<BaoCaoTaiSanModel> getListBaoCaoTaiSanModels(ref PagingInfo _paging, Guid? iID_DonViID = null, Guid? iID_DuAnID = null, Guid? iID_HopDongID = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("iID_DonViID", iID_DonViID == Guid.Empty ? null : (object)iID_DonViID);
                    lstPrams.Add("iID_DuAnID", iID_DuAnID == Guid.Empty ? null : (object)iID_DuAnID);
                    lstPrams.Add("iID_HopDongID", iID_HopDongID == Guid.Empty ? null : (object)iID_HopDongID);
                    lstPrams.Add("CurrentPage", _paging.CurrentPage);
                    lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var items = conn.Query<BaoCaoTaiSanModel>("proc_get_all_baocaodanhsachtaisan_paging", lstPrams, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }

        }

        public List<BaoCaoTaiSanModel2> getListBaoCaoTaiSanModelstb2(ref PagingInfo _paging, Guid? iID_DonViID = null)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("iID_DonViID", iID_DonViID == Guid.Empty ? null : (object)iID_DonViID);
                    lstPrams.Add("CurrentPage", _paging.CurrentPage);
                    lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var items = conn.Query<BaoCaoTaiSanModel2>("proc_get_all_baocaotaisanchitiet_paging", lstPrams, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                    return (List<BaoCaoTaiSanModel2>)(items.Any() ? items : new List<BaoCaoTaiSanModel2>());
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }

        }
        public BaoCaoTaiSanModel GetBaoCaoTaiSanById(Guid iId)
        {
            var sql = "SELECT * FROM NH_QT_TaiSan WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<BaoCaoTaiSanModel>(sql, param: new { iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<NS_DonViModel> GetLookupDonViTaiSan()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NS_DonViModel>(@"select b.iID_Ma, concat(b.iID_MaDonVi, ' - ', b.sTen) as sDonVi from NS_DonVi b ");
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }

        public IEnumerable<NH_DA_DuAn> GetLookupDuAnTaiSan()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NH_DA_DuAn>(@"select a.ID , a.sTenDuAn from NH_DA_DuAn a ");
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }
        public IEnumerable<NH_DA_HopDong> GetLookupHopDongTaiSan()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NH_DA_HopDong>(@"select  a.ID , a.sTenHopDong from NH_DA_HopDong a  ");
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }
        #endregion

        #region QLNH - Đề nghị thanh toán
        /// <summary>
        /// Lấy tất cả danh sách đơn vị theo năm gần nhất
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NS_DonViViewModel> GetAllNSDonVi(int nam)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM NS_DonVi where iNamLamViec_DonVi= @nam";
                var items = conn.Query<NS_DonViViewModel>(sql, param: new { nam }, commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy danh sách nhiệm vụ chi(Tên chương trình) theo đơn vị được chọn
        /// </summary>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <returns></returns>
        public IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetAllNhiemVuChiByDonVi(Guid? iID_MaDonVi = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT nvc.* FROM NH_KHChiTietBQP_NhiemVuChi nvc ");
            if (iID_MaDonVi != null && iID_MaDonVi != Guid.Empty)
            {
                query.AppendLine("INNER JOIN NS_DonVi dv ON dv.iID_MaDonVi = nvc.iID_MaDonVi AND dv.iID_Ma = nvc.iID_DonViID ");
                query.AppendLine("WHERE dv.iID_Ma = @donViID ");
                query.AppendLine("AND NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi nv WHERE nvc.ID = nv.iID_ParentID) ");
            }
            else
            {
                query.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi nv WHERE nvc.ID = nv.iID_ParentID) ");
            }
            query.AppendLine("ORDER BY nvc.sMaThuTu");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_KHChiTietBQP_NhiemVuChi>(
                    query.ToString(),
                    param: (iID_MaDonVi != null && iID_MaDonVi != Guid.Empty)
                        ? new { donViID = iID_MaDonVi }
                        : null,
                    commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy danh sách danh mục chủ đầu tư
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DM_ChuDauTuViewModel> GetAllDMChuDauTu()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM DM_ChuDauTu";
                var items = conn.Query<DM_ChuDauTuViewModel>(sql, commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy danh sách dự án theo nhiệm vụ chi và chủ đầu tư
        /// </summary>
        /// <param name="iID_NhiemVuChi"></param>
        /// <param name="iID_ChuDauTu"></param>
        /// <returns></returns>
        public IEnumerable<NH_DA_DuAn> GetDADuAn(Guid? iID_NhiemVuChi, Guid? iID_ChuDauTu)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM NH_DA_DuAn WHERE iID_KHCTBQP_ChuongTrinhID = @iID_NhiemVuChi AND iID_ChuDauTuID = @iID_ChuDauTu ";
                var items = conn.Query<NH_DA_DuAn>(sql, param: new { iID_NhiemVuChi, iID_ChuDauTu },
                    commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy danh sách hợp đồng theo nhiệm vụ chi
        /// </summary>
        /// <param name="iID_NhiemVuChi"></param>
        /// <returns></returns>
        public IEnumerable<NH_DA_HopDong> GetThongTinHopDong(Guid? iID_NhiemVuChi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT hd.ID, sTenHopDong FROM NH_DA_HopDong hd ");
                query.AppendLine("INNER JOIN NH_KHChiTietBQP_NhiemVuChi nvc ON hd.iID_KHCTBQP_ChuongTrinhID = nvc.ID ");
                query.AppendLine("WHERE hd.iID_KHCTBQP_ChuongTrinhID = @iID_NhiemVuChi ");
                var items = conn.Query<NH_DA_HopDong>(query.ToString(), param: new { iID_NhiemVuChi },
                    commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy danh sách thông tin tỉ giá
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NH_DM_TiGia> GetThongTinTyGia()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * FROM NH_DM_TiGia";
                var items = conn.Query<NH_DM_TiGia>(sql, commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy thông tin mục lục ngân sách theo năm kế hoạch
        /// </summary>
        /// <param name="namthuchien"></param>
        /// <returns></returns>
        public IEnumerable<NS_MucLucNganSach> GetThongTinMucLucNganSach(int namthuchien, int sotrang, int sobanghi)
        {
            var sql = FileHelpers.GetSqlQuery("get_mucluc_ngansach.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NS_MucLucNganSach>(sql,
                        param: new
                        {
                            namthuchien,
                            sotrang,
                            sobanghi,
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

        public IEnumerable<MucLucNganSachViewModel> GetAllMucLucNganSach(ref PagingInfo _paging, string username, int namlamviec, ThongTinThanhToanSearchModel conditionSearch)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("username", username);
                    lstParam.Add("namlamviec", namlamviec);
                    lstParam.Add("sLNS", conditionSearch != null ? conditionSearch.sLNS : string.Empty);
                    lstParam.Add("sL", conditionSearch != null ? conditionSearch.sL : string.Empty);
                    lstParam.Add("sK", conditionSearch != null ? conditionSearch.sK : string.Empty);
                    lstParam.Add("sM", conditionSearch != null ? conditionSearch.sM : string.Empty);
                    lstParam.Add("sTM", conditionSearch != null ? conditionSearch.sTM : string.Empty);
                    lstParam.Add("sTTM", conditionSearch != null ? conditionSearch.sTTM : string.Empty);
                    lstParam.Add("sNG", conditionSearch != null ? conditionSearch.sNG : string.Empty);
                    lstParam.Add("sTNG", conditionSearch != null ? conditionSearch.sTNG : string.Empty);
                    lstParam.Add("sNoiDung", conditionSearch != null ? conditionSearch.sNoiDung : string.Empty);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<MucLucNganSachViewModel>("proc_get_all_muclucngansach_paging", lstParam,
                        commandType: CommandType.StoredProcedure);
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


        public IEnumerable<ThongTinThanhToanModel> GetAllThongTinThanhToanPaging(ref PagingInfo _paging,
            Guid? iID_DonVi, string sSoDeNghi, DateTime? dNgayDeNghi, int? iLoaiNoiDungChi, int? iLoaiDeNghi,
            Guid? iID_ChuDauTuID, Guid? iID_KHCTBQP_NhiemVuChiID, int? iQuyKeHoach, int? iNamKeHoach, int? iNamNganSach,
            int? iCoQuanThanhToan, Guid? iID_NhaThauID, int? iTrangThai)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("iID_DonVi", iID_DonVi);
                    lstParam.Add("sSoDeNghi", sSoDeNghi);
                    lstParam.Add("dNgayDeNghi", dNgayDeNghi);
                    lstParam.Add("iLoaiNoiDungChi", iLoaiNoiDungChi);
                    lstParam.Add("iLoaiDeNghi", iLoaiDeNghi);
                    lstParam.Add("iID_ChuDauTuID", iID_ChuDauTuID);
                    lstParam.Add("iID_KHCTBQP_NhiemVuChiID", iID_KHCTBQP_NhiemVuChiID);
                    lstParam.Add("iQuyKeHoach", iQuyKeHoach);
                    lstParam.Add("iNamKeHoach", iNamKeHoach);
                    lstParam.Add("iNamNganSach", iNamNganSach);
                    lstParam.Add("iCoQuanThanhToan", iCoQuanThanhToan);
                    lstParam.Add("iID_NhaThauID", iID_NhaThauID);
                    lstParam.Add("iTrangThai", iTrangThai);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<ThongTinThanhToanModel>("proc_get_all_denghithanhtoan_paging", lstParam,
                        commandType: CommandType.StoredProcedure);
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
        /// Lấy tất cả danh mục nhà thầu
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NH_DM_NhaThau> GetAllDMNhaThau()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * from NH_DM_NhaThau ";
                var items = conn.Query<NH_DM_NhaThau>(sql, commandType: CommandType.Text);
                return items;
            }
        }

        /// <summary>
        /// Lấy thông tin thanh toán
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ThongTinThanhToanModel GetThongTinThanhToanByID(Guid? id)
        {
            var sql = FileHelpers.GetSqlQuery("get_thongtin_thanhtoan_byid.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<ThongTinThanhToanModel>(sql,
                        param: new
                        {
                            id
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

        public IEnumerable<ThanhToanChiTietViewModel> GetThongTinThanhToanChiTietById(Guid? iDThanhToan)
        {
            var sql = FileHelpers.GetSqlQuery("get_thanhtoan_chitiet_byidthanhtoan.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<ThanhToanChiTietViewModel>(sql,
                        param: new
                        {
                            iDThanhToan
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

        public NH_TT_ThanhToan_ChiTiet GetThongTinThanhToanChiTiet(Guid id_chitiet)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * from NH_TT_ThanhToan_ChiTiet where ID = @id_chitiet ";
                var items = conn.QueryFirstOrDefault<NH_TT_ThanhToan_ChiTiet>(sql, param: new { id_chitiet },
                    commandType: CommandType.Text);
                return items;
            }
        }
        public bool DeleteDeNghiThanhToan(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE NH_TT_ThanhToan_ChiTiet WHERE iID_ThanhToanID = '{0}'; ", id);
                query.AppendFormat("DELETE NH_TT_ThanhToan WHERE ID = '{0}'; ", id);

                var r = conn.Execute(query.ToString(), commandType: CommandType.Text, transaction: trans);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }

        public IEnumerable<NS_PhongBan> GetAllNSPhongBan()
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "SELECT * from NS_PhongBan ";
                var items = conn.Query<NS_PhongBan>(sql, commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoChiThanhToan(string listIDThanhToan, int thang, int quy, int nam)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("lstIDThanhToan", listIDThanhToan);
                    lstParam.Add("thang", thang);
                    lstParam.Add("quy", quy);
                    lstParam.Add("nam", nam);

                    var items = conn.Query<ThanhToanBaoCaoModel>("sp_export_baocao_thongbaochingansach_BM01", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }
        public double ChuyenDoiTyGia(Guid? matygia, float sotiennhap, int loaitiennhap)
        {
            var sql = FileHelpers.GetSqlQuery("chuyendoitygia.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<double>(sql,
                        param: new
                        {
                            matygia,
                            sotiennhap,
                            loaitiennhap,
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return 0;
        }

        public IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoThongBaoCapKinhPhiChiNgoaiTe(string listIDThanhToan, DateTime tungay, DateTime denngay)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("lstIDThanhToan", listIDThanhToan);
                    lstParam.Add("tungay", tungay);
                    lstParam.Add("denngay", denngay);

                    var items = conn.Query<ThanhToanBaoCaoModel>("sp_export_baocao_thongbaocapkinhphingoaite_BM05", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<ThanhToanBaoCaoModel> ExportBaoCaoThongBaoCapKinhPhiChiTrongNuoc(string listIDThanhToan, DateTime tungay, DateTime denngay)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("lstIDThanhToan", listIDThanhToan);
                    lstParam.Add("tungay", tungay);
                    lstParam.Add("denngay", denngay);

                    var items = conn.Query<ThanhToanBaoCaoModel>("sp_export_baocao_thongbaocapkinhphitrongnuoc_BM06", lstParam, commandType: CommandType.StoredProcedure);
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public IEnumerable<NH_TT_ThanhToan> GetThanhToanGanNhat(DateTime dngaytao, Guid? idonvi, Guid? inhiemvuchi, Guid? ichudautu)
        {
            var sql = FileHelpers.GetSqlQuery("thanhtoanluyke.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.Query<NH_TT_ThanhToan>(sql,
                        param: new
                        {
                            idonvi,
                            inhiemvuchi,
                            ichudautu,
                            dngaytao
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

        public Boolean CheckTrungMaDeNghi(string sodenghi, int type_action, Guid? imadenghi)
        {
            var sql = FileHelpers.GetSqlQuery("checktrungmadenghithanhtoan.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<Boolean>(sql,
                        param: new
                        {
                            sodenghi,
                            type_action,
                            imadenghi,
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return true;
        }

        public bool SaveImportThongTinDuAn(List<NH_DA_DuAn> contractList)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    conn.Insert<NH_DA_DuAn>(contractList, trans);
                    trans.Commit();
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

        public IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetListChuongTrinh()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NH_KHChiTietBQP_NhiemVuChi>(@"SELECT * FROM NH_KHChiTietBQP_NhiemVuChi");
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }

        #endregion

        #region QLNH - Thông tin dự án
        public IEnumerable<NHDAThongTinDuAnModel> getListThongTinDuAnModels(ref PagingInfo _paging, string sMaDuAn, string sTenDuAn, Guid? iID_BQuanLyID, Guid? iID_ChuDauTuID, Guid? iID_DonViID, Guid? iID_CapPheDuyetID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("sMaDuAn", sMaDuAn);
                    lstPrams.Add("sTenDuAn", sTenDuAn);
                    lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID == Guid.Empty ? null : (object)iID_BQuanLyID);
                    lstPrams.Add("iID_ChuDauTuID", iID_ChuDauTuID == Guid.Empty ? null : (object)iID_ChuDauTuID);
                    lstPrams.Add("iID_DonViID", iID_DonViID == Guid.Empty ? null : (object)iID_DonViID);
                    lstPrams.Add("iID_CapPheDuyetID", iID_CapPheDuyetID == Guid.Empty ? null : (object)iID_CapPheDuyetID);
                    lstPrams.Add("CurrentPage", _paging.CurrentPage);
                    lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var items = conn.Query<NHDAThongTinDuAnModel>("proc_get_all_nhdaDuAn_paging", lstPrams, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstPrams.Get<int>("iToTalItem");

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        public IEnumerable<NH_DM_PhanCapPheDuyet> GetLookupThongTinDuAn()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NH_DM_PhanCapPheDuyet>(
                        string.Format(@"SELECT * FROM NH_DM_PhanCapPheDuyet"));
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }
        public IEnumerable<NH_KHChiTietBQP_NhiemVuChi> GetListCTbyDV(Guid? id = null, Guid? idBQP = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT DISTINCT a.ID,a.sTenNhiemVuChi,a.iID_DonViID,a.iID_KHChiTietID FROM NH_KHChiTietBQP_NhiemVuChi a ");
            DynamicParameters lstPrams = new DynamicParameters();
            if (id.HasValue)
            {
                query.AppendLine("WHERE a.iID_DonViID = @ID ");
                lstPrams.Add("ID", id.Value);
                if (idBQP.HasValue)
                {
                    query.Append(" AND a.iID_KHChiTietID = @IDBQP ");
                    lstPrams.Add("IDBQP", idBQP.Value);
                }
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_KHChiTietBQP_NhiemVuChi>(query.ToString(),
                    param: lstPrams,
                    commandType: CommandType.Text);
                return items;
            }
        }
        public List<NH_DA_DuAn> GetMaDuAn(string iID_MaDonVi, string sMaChuDauTu)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("iID_MaDonVi", iID_MaDonVi);
                    lstPrams.Add("sMaChuDauTu", sMaChuDauTu);
                    var items = conn.Query<NH_DA_DuAn>("proc_get_all_getMaDuAn_paging", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }

        }
        public IEnumerable<NH_DA_DuAn> GetThongTinDuAnDuAnList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DA_DuAn ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY sMaDuAn");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_DuAn>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_TiGia_ChiTiet> GetNHDMTiGiaChiTiet(Guid? iDTiGia, bool isMaNgoaiTeKhac = true)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT tgct.* FROM NH_DM_TiGia_ChiTiet tgct ");
            query.AppendLine("INNER JOIN NH_DM_TiGia tg ON tgct.iID_TiGiaID = tg.ID ");
            query.AppendLine("WHERE 1 = 1 ");
            if (isMaNgoaiTeKhac)
            {
                query.AppendLine("AND tgct.sMaTienTeQuyDoi NOT IN ('USD', 'VND', 'EUR') ");
            }

            if (iDTiGia != null && iDTiGia != Guid.Empty)
            {
                query.AppendLine(" AND tgct.iID_TiGiaID = @iDTiGia");
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_TiGia_ChiTiet>(query.ToString(),
                    param: iDTiGia != null ? new { iDTiGia = iDTiGia } : null,
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NS_DonVi> GetLookupThongTinDonVi()
        {
            StringBuilder query = new StringBuilder();
            query.Append(" select dv.iID_Ma ,dv.iID_MaDonVi,concat(iID_MaDonVi, ' - ', sTen) as sTen from NS_DonVi dv");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public NHDAThongTinDuAnModel GetThongTinDuAnById(Guid id)
        {
            var sql = "proc_getall_ChiTietTTDA";
            using (var conn = _connectionFactory.GetConnection())
            {
                var a = new NHDAThongTinDuAnModel();

                a = conn.QueryFirstOrDefault<NHDAThongTinDuAnModel>(sql, param: new { id }, commandType: CommandType.StoredProcedure);
                var sqltb = @"select cp.sTenChiPhi, da.fGiaTriUSD,da.fGiaTriVND,da.fGiaTriEUR,da.fGiaTriNgoaiTeKhac from NH_DA_DuAn_ChiPhi da left join NH_DM_ChiPhi cp on da.iID_ChiPhiID=cp.ID where iID_DuAnID=@id";
                a.TableChiPhis = conn.Query<TableChiPhi>(sqltb, param: new { id }, commandType: CommandType.Text).ToList();
                return a;
            }
        }

        public IEnumerable<NH_KHChiTietBQP_NhiemVuChiModel> GetNHKeHoachChiTietBQPNhiemVuChiListDuAn(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("select distinct BQP.* from ( select a.ID, concat(N'KHTT', iGiaiDoanTu, ' - ', iGiaiDoanDen, N' -  Số KH:', sSoKeHoach) as sSoKeHoachBQP from NH_KHChiTietBQP a" +
                " inner join NH_KHChiTietBQP_NhiemVuChi b on a.ID = b.iID_KHChiTietID where a.iLoai = 1 and a.bIsActive = 1 " +
                "UNION ALL " +
                "select a.ID, concat(N'KHTT', iNamKeHoach, ' -  NSố KH:', sSoKeHoach) as sSoKeHoachBQP from NH_KHChiTietBQP a inner join NH_KHChiTietBQP_NhiemVuChi b on a.ID = b.iID_KHChiTietID where a.iLoai = 2 and a.bIsActive = 1) as BQP ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_KHChiTietBQP_NhiemVuChiModel>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NS_DonVi> GetListDonViToBQP(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("select distinct dv.iID_Ma , dv.iID_MaDonVi,dv.sTen,dv.iID_MaDonVi from NS_DonVi dv inner join NH_KHChiTietBQP_NhiemVuChi ct on ct.iID_DonViID = dv.iID_Ma where ct.iID_KHChiTietID = @ID");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(query.ToString(),
                    param: new { ID = id ?? new Guid() },
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NHDAThongTinDuAnModel> GetListBQPToNHC(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("select nvc.iID_KHChiTietID from NH_DA_DuAn da left join NH_KHChiTietBQP_NhiemVuChi nvc on nvc.ID = da.iID_KHCTBQP_ChuongTrinhID where iID_KHChiTietID = @ID ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NHDAThongTinDuAnModel>(query.ToString(),
                    param: new { ID = id ?? new Guid() },
                    commandType: CommandType.Text);
                return items;
            }
        }
        public List<NH_DA_DuAn_ChiPhiModel> getListChiPhiTTDuAn(Guid? ID, string state)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    if (state == "DETAIL")
                    {
                        return new List<NH_DA_DuAn_ChiPhiModel>();
                    }
                    conn.Open();
                    var query = conn.Query<NH_DA_DuAn_ChiPhiModel>(
                        @"SELECT DACP.*, CP.sTenChiPhi FROM NH_DA_DuAn_ChiPhi AS DACP
                          LEFT JOIN NH_DM_ChiPhi AS CP ON DACP.iID_ChiPhiID = CP.ID WHERE DACP.iID_DuAnID=@ID", new { ID = ID }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool DeleteThongTinDuAn(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE NH_DA_DuAn SET bIsActive = 1 WHERE ID = (SELECT iID_ParentAdjustID FROM NH_DA_DuAn WHERE ID = @iId);");
            query.Append("DELETE NH_DA_DuAn WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                return r >= 0;
            }
        }

        public IEnumerable<NS_PhongBan> GetLookupQuanLy()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT DISTINCT cdt.iID_MaPhongBan ,concat(cdt.sTen, ' - ' ,cdt.sMoTa) AS sTen FROM NS_PhongBan cdt");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_PhongBan>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<NH_DM_ChiPhi> GetLookupChiPhi()
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var query = conn.Query<NH_DM_ChiPhi>(@"SELECT * FROM NH_DM_ChiPhi");
                    return query;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return null;
        }

        public bool SaveThongTinDuAn(NHDAThongTinDuAnModel data, string userName, string state, List<NH_DA_DuAn_ChiPhiDto> dataTableChiPhi, Guid? oldId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    if (state == "CREATE")
                    {
                        var entity = new NH_DA_DuAn();
                        entity.MapFrom(data);
                        entity.fGiaTriEUR = double.TryParse(data.sGiaTriEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feurParent) ? feurParent : (double?)null;
                        entity.fGiaTriVND = double.TryParse(data.sGiaTriVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvndParent) ? fvndParent : (double?)null;
                        entity.fGiaTriUSD = double.TryParse(data.sGiaTriUSD, NumberStyles.Float, new CultureInfo("en-US"), out double feusdParent) ? feusdParent : (double?)null;
                        entity.fGiaTriNgoaiTeKhac = double.TryParse(data.sGiaTriNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fgtkParent) ? fgtkParent : (double?)null;
                        entity.bIsActive = true;
                        entity.bIsGoc = true;
                        entity.sNguoiTao = userName;
                        entity.dNgayTao = DateTime.Now;
                        entity.iLanDieuChinh = 0;
                        conn.Insert(entity, trans);

                        var listDaCpIds = conn.Query<Guid>(@"select da.ID from NH_DA_DuAn da join NH_DA_DuAn_ChiPhi cp on da.ID = cp.iID_DuAnID", null, trans).ToList();
                        foreach (var item in dataTableChiPhi)
                        {
                            var newCp = new NH_DA_DuAn_ChiPhi();
                            
                            newCp.iID_DuAnID = entity.ID;
                            newCp.iID_ChiPhiID = item.iID_ChiPhiID;
                            newCp.fGiaTriEUR = double.TryParse(item.HopDongEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feur) ? feur : (double?)null;
                            newCp.fGiaTriNgoaiTeKhac = double.TryParse(item.HopDongNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fntk) ? fntk : (double?)null;
                            newCp.fGiaTriUSD = double.TryParse(item.HopDongUSD, NumberStyles.Float, new CultureInfo("en-US"), out double fusd) ? fusd : (double?)null;
                            newCp.fGiaTriVND = double.TryParse(item.HopDongVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvnd) ? fvnd : (double?)null;
                            conn.Insert(newCp, trans);
                        }
                        foreach (var id in listDaCpIds)
                        {
                            var newCp = new NH_DA_DuAn_ChiPhi();
                            newCp.ID = id;
                            conn.Delete(newCp, trans);
                        }
                    }
                    else if (state == "UPDATE")
                    {
                        var query = @"SELECT * FROM NH_DA_DuAn WHERE ID = @Id";
                        var da = conn.QueryFirstOrDefault<NH_DA_DuAn>(query, new { Id = data.ID }, trans, commandType: CommandType.Text);
                        da.iID_BQuanLyID = data.iID_BQuanLyID;
                        da.iID_MaDonVi = data.iID_MaDonVi;
                        da.iID_DonViID = data.iID_DonViID;
                        da.iID_KHCTBQP_ChuongTrinhID = data.iID_KHCTBQP_ChuongTrinhID;
                        da.sMaDuAn = data.sMaDuAn;
                        da.sTenDuAn = data.sTenDuAn;
                        da.sSoChuTruongDauTu = data.sSoChuTruongDauTu;
                        da.dNgayChuTruongDauTu = data.dNgayChuTruongDauTu;
                        da.sSoQuyetDinhDauTu = data.sSoQuyetDinhDauTu;
                        da.dNgayQuyetDinhDauTu = data.dNgayQuyetDinhDauTu;
                        da.sSoDuToan = data.sSoDuToan;
                        da.dNgayDuToan = data.dNgayDuToan;
                        da.iID_ChuDauTuID = data.iID_ChuDauTuID;
                        da.sMaChuDauTu = data.sMaChuDauTu;
                        da.iID_CapPheDuyetID = data.iID_CapPheDuyetID;
                        da.sKhoiCong = data.sKhoiCong;
                        da.sKetThuc = data.sKetThuc;
                        da.iID_TiGiaID = data.iID_TiGiaID;
                        da.sMaNgoaiTeKhac = data.sMaNgoaiTeKhac;
                        da.fGiaTriEUR = double.TryParse(data.sGiaTriEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feurParent) ? feurParent : (double?)null;
                        da.fGiaTriVND = double.TryParse(data.sGiaTriVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvndParent) ? fvndParent : (double?)null;
                        da.fGiaTriUSD = double.TryParse(data.sGiaTriUSD, NumberStyles.Float, new CultureInfo("en-US"), out double feusdParent) ? feusdParent : (double?)null;
                        da.fGiaTriNgoaiTeKhac = double.TryParse(data.sGiaTriNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fgtkParent) ? fgtkParent : (double?)null;
                        da.dNgaySua = data.dNgaySua;
                        da.sNguoiSua = data.sNguoiSua;
                        da.dNgayXoa = data.dNgayXoa;
                        da.sNguoiXoa = data.sNguoiXoa;
                        conn.Update<NH_DA_DuAn>(da, trans);

                        var listDaCpIds = conn.Query<Guid>(@"SELECT ID FROM NH_DA_DuAn_ChiPhi", null, trans).ToList();
                        foreach (var item in dataTableChiPhi)
                        {
                            if (item.ID != Guid.Empty)
                            {
                                var newCp = new NH_DA_DuAn_ChiPhi();
                                newCp.iID_DuAnID = da.ID;
                                newCp.ID = item.ID;
                                newCp.iID_ChiPhiID = item.iID_ChiPhiID;
                                newCp.fGiaTriEUR = double.TryParse(item.HopDongEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feur) ? feur : (double?)null;
                                newCp.fGiaTriNgoaiTeKhac = double.TryParse(item.HopDongNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fntk) ? fntk : (double?)null;
                                newCp.fGiaTriUSD = double.TryParse(item.HopDongUSD, NumberStyles.Float, new CultureInfo("en-US"), out double fusd) ? fusd : (double?)null;
                                newCp.fGiaTriVND = double.TryParse(item.HopDongVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvnd) ? fvnd : (double?)null;
                                conn.Update(newCp, trans);
                                listDaCpIds.Remove(item.ID);
                            }
                            else
                            {
                                var newCp = new NH_DA_DuAn_ChiPhi();
                                newCp.iID_DuAnID = da.ID;
                                newCp.iID_ChiPhiID = item.iID_ChiPhiID;
                                newCp.fGiaTriEUR = double.TryParse(item.HopDongEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feur) ? feur : (double?)null;
                                newCp.fGiaTriNgoaiTeKhac = double.TryParse(item.HopDongNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fntk) ? fntk : (double?)null;
                                newCp.fGiaTriUSD = double.TryParse(item.HopDongUSD, NumberStyles.Float, new CultureInfo("en-US"), out double fusd) ? fusd : (double?)null;
                                newCp.fGiaTriVND = double.TryParse(item.HopDongVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvnd) ? fvnd : (double?)null;
                                conn.Insert(newCp, trans);
                            }
                        }
                        foreach (var id in listDaCpIds)
                        {
                            var newCp = new NH_DA_DuAn_ChiPhi();
                            newCp.ID = id;
                        }
                    }
                    else
                    {
                        var queryNH_DA_DuAn = @"SELECT * FROM NH_DA_DuAn WHERE ID = @oldID";
                        var daOld = conn.QueryFirstOrDefault<NH_DA_DuAn>(queryNH_DA_DuAn, new { oldId = oldId }, trans);
                        daOld.bIsActive = false;
                        conn.Update(daOld, trans);
                        var entity = new NH_DA_DuAn();
                        entity.MapFrom(data);
                        entity.fGiaTriEUR = double.TryParse(data.sGiaTriEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feurParent) ? feurParent : (double?)null;
                        entity.fGiaTriVND = double.TryParse(data.sGiaTriVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvndParent) ? fvndParent : (double?)null;
                        entity.fGiaTriUSD = double.TryParse(data.sGiaTriUSD, NumberStyles.Float, new CultureInfo("en-US"), out double feusdParent) ? feusdParent : (double?)null;
                        entity.fGiaTriNgoaiTeKhac = double.TryParse(data.sGiaTriNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fgtkParent) ? fgtkParent : (double?)null;
                        entity.ID = new Guid();
                        entity.bIsActive = true;
                        entity.iID_ParentAdjustID = daOld.ID;
                        entity.bIsGoc = false;
                        entity.sNguoiTao = userName;
                        entity.dNgayTao = DateTime.Now;
                        entity.iLanDieuChinh = daOld.iLanDieuChinh + 1;
                        conn.Insert(entity, trans);

                        var listDaCpIds = conn.Query<Guid>(@"SELECT ID FROM NH_DA_DuAn_ChiPhi", null, trans).ToList();
                        foreach (var item in dataTableChiPhi)
                        {
                            if (item.ID != Guid.Empty)
                            {
                                var newCp = new NH_DA_DuAn_ChiPhi();
                                newCp.iID_DuAnID = entity.ID;
                                newCp.ID = item.ID;
                                newCp.iID_ChiPhiID = item.iID_ChiPhiID;
                                newCp.fGiaTriEUR = double.TryParse(item.HopDongEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feur) ? feur : (double?)null;
                                newCp.fGiaTriNgoaiTeKhac = double.TryParse(item.HopDongNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fntk) ? fntk : (double?)null;
                                newCp.fGiaTriUSD = double.TryParse(item.HopDongUSD, NumberStyles.Float, new CultureInfo("en-US"), out double fusd) ? fusd : (double?)null;
                                newCp.fGiaTriVND = double.TryParse(item.HopDongVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvnd) ? fvnd : (double?)null;
                                conn.Update(newCp, trans);

                                listDaCpIds.Remove(item.ID);
                            }
                            else
                            {
                                var newCp = new NH_DA_DuAn_ChiPhi();
                                newCp.iID_DuAnID = entity.ID;
                                newCp.iID_ChiPhiID = item.iID_ChiPhiID;
                                newCp.fGiaTriEUR = double.TryParse(item.HopDongEUR, NumberStyles.Float, new CultureInfo("en-US"), out double feur) ? feur : (double?)null;
                                newCp.fGiaTriNgoaiTeKhac = double.TryParse(item.HopDongNgoaiTeKhac, NumberStyles.Float, new CultureInfo("en-US"), out double fntk) ? fntk : (double?)null;
                                newCp.fGiaTriUSD = double.TryParse(item.HopDongUSD, NumberStyles.Float, new CultureInfo("en-US"), out double fusd) ? fusd : (double?)null;
                                newCp.fGiaTriVND = double.TryParse(item.HopDongVND, NumberStyles.Float, new CultureInfo("en-US"), out double fvnd) ? fvnd : (double?)null;
                                conn.Insert(newCp, trans);
                            }
                        }
                        foreach (var id in listDaCpIds)
                        {
                            var newCp = new NH_DA_DuAn_ChiPhi();
                            newCp.ID = id;
                            //conn.Delete(newCp, trans);
                        }
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

        public IEnumerable<DM_ChuDauTu> GetLookupChuDauTu()
        {
            StringBuilder query = new StringBuilder();
            query.Append("select distinct cdt.ID,cdt.sId_CDT,concat(cdt.sId_CDT, ' - ', cdt.sTenCDT) as sTenCDT from DM_ChuDauTu cdt left join NH_DA_DuAn da on cdt.ID = da.iID_ChuDauTuID");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_ChuDauTu>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<DM_ChuDauTu> GetLookupChuDauTuu()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM DM_ChuDauTu ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<DM_ChuDauTu>(query.ToString(), commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Tổng hợp dự án
        public IEnumerable<NHDAThongTinDuAnModel> getListTongHopDuAnModels(ref PagingInfo _paging, Guid? iID_BQuanLyID, Guid? iID_DonViID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID == Guid.Empty ? null : (object)iID_BQuanLyID);
                    lstPrams.Add("iID_DonViID", iID_DonViID == Guid.Empty ? null : (object)iID_DonViID);
                    lstPrams.Add("CurrentPage", _paging.CurrentPage);
                    lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<NHDAThongTinDuAnModel>("proc_get_all_tonghop_duan_paging", lstPrams, commandType: CommandType.StoredProcedure);
                    _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }

        }

        public List<NHDAThongTinDuAnModel> getListTongHopDuAn_BaoCaoModels(Guid? iID_BQuanLyID, Guid? iID_DonViID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstPrams = new DynamicParameters();
                    lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID == Guid.Empty ? null : (object)iID_BQuanLyID);
                    lstPrams.Add("iID_DonViID", iID_DonViID == Guid.Empty ? null : (object)iID_DonViID);
                    return conn.Query<NHDAThongTinDuAnModel>("proc_get_all_tonghop_duan", lstPrams, commandType: CommandType.StoredProcedure).ToList(); ;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }

        }
        #endregion

        #region QLNH - Chênh lệch tỉ giá hối đoái
        public IEnumerable<ChenhLechTiGiaModel> GetAllChenhLechTiGia(ref PagingInfo _paging, Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", iChuongTrinh);
                lstParam.Add("iHopDong", iHopDong);
                //lstParam.Add("CurrentPage", _paging.CurrentPage);
                //lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                //lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<ChenhLechTiGiaModel>("proc_get_all_nh_da_chenhlechtigia_paging", lstParam, commandType: CommandType.StoredProcedure);
                //_paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }
        public IEnumerable<NS_DonVi> GetDonViList(bool hasChuongTrinh = false)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT a.iID_MaDonVi, a.sTen, CONCAT(a.iID_MaDonVi,' - ',a.sTen) AS sMoTa, a.iID_Ma ");
            sql.AppendLine("FROM NS_DonVi a ");
            if (hasChuongTrinh)
            {
                sql.AppendLine("INNER JOIN NH_KHChiTietBQP_NhiemVuChi c ON a.iID_Ma = c.iID_DonViID AND a.iID_MaDonVi = c.iID_MaDonVi ");
            }
            sql.AppendLine("ORDER BY a.iID_MaDonVi ");

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(new CommandDefinition(commandText: sql.ToString(), commandType: CommandType.Text));
                return items;
            }
        }
        public IEnumerable<NH_DA_HopDong> GetNHDAHopDongList(Guid? chuongTrinhID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT hd.* FROM NH_DA_HopDong hd ");
            if (chuongTrinhID != null && chuongTrinhID != Guid.Empty)
            {
                query.AppendLine("INNER JOIN NH_KHChiTietBQP_NhiemVuChi nvc ON nvc.ID = hd.iID_KHCTBQP_ChuongTrinhID ");
            }
            query.AppendLine("WHERE hd.bIsActive = 1 ");
            if (chuongTrinhID != null && chuongTrinhID != Guid.Empty)
            {
                query.AppendLine("AND nvc.ID = @chuongTrinhID ");
            }
            query.AppendLine("ORDER BY hd.sTenHopDong ");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_HopDong>(
                    query.ToString(),
                    param: (chuongTrinhID != null && chuongTrinhID != Guid.Empty) ? new { chuongTrinhID = chuongTrinhID } : null,
                    commandType: CommandType.Text);
                return items;
            }
        }
        public IEnumerable<ChenhLechTiGiaModel> GetDataExportChenhLechTiGia(Guid? iDonVi, Guid? iChuongTrinh, Guid? iHopDong)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", iChuongTrinh);
                lstParam.Add("iHopDong", iHopDong);

                var items = conn.Query<ChenhLechTiGiaModel>("sp_export_baocao_chenhlechtigia", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region QLNH - Kế hoạch chi tiết Bộ Quốc phòng
        public NH_KHChiTietBQPViewModel getListKHChiTietBQP(PagingInfo _paging, string sSoKeHoach, DateTime? dNgayBanHanh, int? from, int? to)
        {
            var result = new NH_KHChiTietBQPViewModel();
            using (var connection = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("From", from);
                lstPrams.Add("To", to);
                lstPrams.Add("sSoKeHoach", sSoKeHoach);
                lstPrams.Add("dNgayBanHanh", dNgayBanHanh);
                lstPrams.Add("SkipCount", (_paging.CurrentPage - 1) * _paging.ItemsPerPage);
                lstPrams.Add("MaxResultCount", _paging.ItemsPerPage);
                lstPrams.Add("TotalItems", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = connection.Query<NH_KHChiTietBQPModel>("sp_get_all_KeHoachChiTietBQP", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("TotalItems");
                result._paging = _paging;
                result.Items = items.ToList();
            }

            return result;
        }

        public IEnumerable<LookupDto<Guid, string>> getLookupKHBQP()
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT IIF(iLoai = 1, 
			                            CONCAT('KHTT ', iGiaiDoanTu, ' - ', iGiaiDoanDen, N' -  Số KH: ', sSoKeHoach), 
			                            CONCAT('KHTT ', iNamKeHoach, N' -  Số KH:', sSoKeHoach)) AS DisplayName, ID AS Id
                            FROM NH_KHChiTietBQP";
                return connection.Query<LookupDto<Guid, string>>(query, commandType: CommandType.Text);
            }
        }

        public IEnumerable<LookupKHTTCP> getLookupKHTTCP()
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT IIF(iLoai = 1, 
			                            CONCAT('KHTT ', iGiaiDoanTu, ' - ', iGiaiDoanDen, N' -  Số KH: ', sSoKeHoach), 
			                            CONCAT('KHTT ', iNamKeHoach, N' -  Số KH:', sSoKeHoach)) AS DisplayName, ID AS Id, iGiaiDoanTu, iGiaiDoanDen, iNamKeHoach, iLoai
                            FROM NH_KHTongTheTTCP WHERE bIsActive = 1";
                return connection.Query<LookupKHTTCP>(query, commandType: CommandType.Text);
            }
        }

        public Boolean SaveKHBQP(List<NH_KHChiTietBQP_NhiemVuChiCreateDto> lstNhiemVuChis, NH_KHChiTietBQP khct, string state)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    if (state == "CREATE" || state == "ADJUST")
                    {
                        if (state == "ADJUST")
                        {
                            // Update bản ghi cha
                            var queryKHCTOld = @"SELECT * FROM NH_KHChiTietBQP WHERE ID = @Id";
                            var khctOld = conn.QueryFirstOrDefault<NH_KHChiTietBQP>(queryKHCTOld, new { Id = khct.iID_ParentAdjustID }, trans);
                            khctOld.bIsActive = false;
                            conn.Update(khctOld, trans);

                            // Check để lấy GocID
                            if (khctOld.bIsGoc)
                            {
                                khct.iID_GocID = khctOld.ID;
                            }
                            else
                            {
                                khct.iID_GocID = khctOld.iID_GocID;
                            }
                        }

                        // Insert
                        khct.ID = Guid.Empty;
                        conn.Insert(khct, trans);

                        // Convert data nhiệm vụ chi
                        var lstNVCInserts = new List<NH_KHChiTietBQP_NhiemVuChi>();
                        foreach (var nvcDto in lstNhiemVuChis)
                        {
                            var nvc = new NH_KHChiTietBQP_NhiemVuChi();
                            nvc.iID_KHChiTietID = khct.ID;
                            nvc.sTenNhiemVuChi = HttpUtility.HtmlDecode(nvcDto.sTenNhiemVuChi);
                            nvc.iID_BQuanLyID = nvcDto.iID_BQuanLyID;
                            nvc.iID_MaDonVi = HttpUtility.HtmlDecode(nvcDto.iID_MaDonVi);
                            nvc.iID_DonViID = nvcDto.iID_DonViID;
                            nvc.fGiaTriUSD = double.TryParse(nvcDto.fGiaTriUSD, NumberStyles.Float, new CultureInfo("en-US"), out double gtusd) ? gtusd : (double?)null;
                            nvc.fGiaTriVND = double.TryParse(nvcDto.fGiaTriVND, NumberStyles.Float, new CultureInfo("en-US"), out double gtvnd) ? gtvnd : (double?)null;
                            nvc.sMaThuTu = nvcDto.sMaThuTu;
                            nvc.iID_KHTTTTCP_NhiemVuChiID = nvcDto.iID_KHTTTTCP_NhiemVuChiID;
                            nvc.bIsTTCP = nvcDto.bIsTTCP;
                            //nvc.iID_ParentID = nvcDto.iID_ParentID;
                            nvc.iID_ParentAdjustID = (state == "ADJUST" && nvcDto.ID != Guid.Empty) ? nvcDto.ID : (Guid?)null;

                            lstNVCInserts.Add(nvc);
                        }

                        // Exec insert data nhiệm vụ chi
                        //lstNVCInserts.OrderBy(x => x.sMaThuTu).ToList();
                        foreach (var nvc in lstNVCInserts)
                        {
                            // Nếu chưa được insert thì insert và đã có parentID thì insert luôn
                            if (nvc.ID == Guid.Empty && nvc.iID_ParentID.HasValue)
                            {
                                conn.Insert(nvc, trans);
                            }
                            else if (nvc.ID == Guid.Empty && !nvc.iID_ParentID.HasValue)
                            {
                                // Nếu chưa được insert và chưa có parentId luôn thì tìm thằng cha để insert thằng cha trước
                                var indexOfDot = nvc.sMaThuTu.LastIndexOf(".");
                                if (indexOfDot == -1)
                                {
                                    // Nếu không có thằng cha thì insert luôn.
                                    conn.Insert(nvc, trans);
                                }
                                else
                                {
                                    // Lấy mã thứ tự của bản ghi cha.
                                    var sttParent = nvc.sMaThuTu.Substring(0, indexOfDot);
                                    // Tìm bản ghi cha
                                    var parent = lstNVCInserts.FirstOrDefault(x => x.sMaThuTu == sttParent);
                                    // Nếu tìm không thấy thằng cha thì insert luôn
                                    if (parent == null)
                                    {
                                        conn.Insert(nvc, trans);
                                    }
                                    else
                                    {
                                        // Nếu tìm thấy thằng cha thì ném vào đệ quy để check xem nó đã được insert hay chưa rồi lấy id của thằng cha.
                                        nvc.iID_ParentID = GetIdKHBQPNhiemVuChiParent(conn, trans, parent, ref lstNVCInserts);
                                        conn.Insert(nvc, trans);
                                    }
                                }
                            }
                        }

                        // Sau khi insert thì tính tổng giá trị của BQP
                        double fTongGiaTriUSD = 0;
                        double fTongGiaTriVND = 0;
                        foreach (var nvc in lstNVCInserts)
                        {
                            if (!nvc.iID_ParentID.HasValue)
                            {
                                fTongGiaTriUSD += nvc.fGiaTriUSD.HasValue ? nvc.fGiaTriUSD.Value : 0;
                                fTongGiaTriVND += nvc.fGiaTriVND.HasValue ? nvc.fGiaTriVND.Value : 0;
                            }
                        }

                        khct.fTongGiaTriUSD = fTongGiaTriUSD;
                        khct.fTongGiaTriVND = fTongGiaTriVND;
                        conn.Update(khct, trans);
                    }
                    else if (state == "UPDATE")
                    {
                        // Update KH Chi tiết.
                        var queryKhctGoc = @"SELECT * FROM NH_KHChiTietBQP WHERE ID = @Id";
                        var khctGoc = conn.QueryFirstOrDefault<NH_KHChiTietBQP>(queryKhctGoc, new { Id = khct.ID }, trans);

                        khctGoc.iLoai = khct.iLoai;
                        khctGoc.iGiaiDoanTu = khct.iGiaiDoanTu;
                        khctGoc.iGiaiDoanDen = khct.iGiaiDoanDen;
                        khctGoc.iNamKeHoach = khct.iNamKeHoach;
                        khctGoc.iID_ParentID = khct.iID_ParentID;
                        khctGoc.iID_KHTongTheTTCPID = khct.iID_KHTongTheTTCPID;
                        khctGoc.iID_TiGiaID = khct.iID_TiGiaID;
                        khctGoc.sSoKeHoach = HttpUtility.HtmlDecode(khct.sSoKeHoach);
                        khctGoc.dNgayKeHoach = khct.dNgayKeHoach;
                        khctGoc.sMoTaChiTiet = khct.sMoTaChiTiet;
                        khctGoc.dNgaySua = khct.dNgaySua;
                        khctGoc.sNguoiSua = khct.sNguoiSua;

                        conn.Update(khctGoc, trans);

                        // Update nhiệm vụ chi
                        var queryListIdNVC = @"SELECT ID 
                            FROM NH_KHChiTietBQP_NhiemVuChi 
                            WHERE iID_KHChiTietID = @Id";
                        var lstIdNVC = conn.Query<Guid>(queryListIdNVC, new { Id = khctGoc.ID }, trans).ToList();

                        // Convert data nhiệm vụ chi
                        var lstNVCUpdate = new List<NH_KHChiTietBQP_NhiemVuChi>();
                        var lstNVCAdd = new List<NH_KHTongTheTTCP_NhiemVuChi>();
                        double fTongGiaTriUSD = 0;
                        double fTongGiaTriVND = 0;

                        foreach (var nvcDto in lstNhiemVuChis)
                        {
                            var nvc = new NH_KHChiTietBQP_NhiemVuChi();
                            nvc.ID = nvcDto.ID;
                            nvc.iID_KHChiTietID = khctGoc.ID;
                            nvc.sTenNhiemVuChi = HttpUtility.HtmlDecode(nvcDto.sTenNhiemVuChi);
                            nvc.iID_BQuanLyID = nvcDto.iID_BQuanLyID;
                            nvc.iID_MaDonVi = HttpUtility.HtmlDecode(nvcDto.iID_MaDonVi);
                            nvc.iID_DonViID = nvcDto.iID_DonViID;
                            nvc.fGiaTriUSD = double.TryParse(nvcDto.fGiaTriUSD, NumberStyles.Float, new CultureInfo("en-US"), out double gtusd) ? gtusd : (double?)null;
                            nvc.fGiaTriVND = double.TryParse(nvcDto.fGiaTriVND, NumberStyles.Float, new CultureInfo("en-US"), out double gtvnd) ? gtvnd : (double?)null;
                            nvc.sMaThuTu = nvcDto.sMaThuTu;
                            nvc.iID_KHTTTTCP_NhiemVuChiID = nvcDto.iID_KHTTTTCP_NhiemVuChiID;
                            nvc.bIsTTCP = nvcDto.bIsTTCP;
                            nvc.iID_ParentID = nvcDto.iID_ParentID;

                            if (!nvcDto.iID_ParentID.HasValue)
                            {
                                fTongGiaTriUSD += nvc.fGiaTriUSD.HasValue ? nvc.fGiaTriUSD.Value : 0;
                                fTongGiaTriVND += nvc.fGiaTriVND.HasValue ? nvc.fGiaTriVND.Value : 0;
                            }
                            if (nvcDto.isAdd)
                            {
                                conn.Insert(nvc, trans);
                                //lstNVCAdd.Add(nvc);
                            }
                            else
                            {
                                conn.Update(nvc, trans);
                                lstIdNVC.Remove(nvc.ID);
                            }
                            //lstNVCUpdate.Add(nvc);
                        }

                        // Check có ID thì update, ko có ID thì insert vào.
                        //foreach (var nvc in lstNVCUpdate)
                        //{
                        //    if (nvc.ID != Guid.Empty)
                        //    {
                        //        conn.Update(nvc, trans);
                        //        lstIdNVC.Remove(nvc.ID);
                        //    }
                        //    else
                        //    {
                        //        if (!nvc.iID_ParentID.HasValue)
                        //        {
                        //            nvc.iID_ParentID = GetIdKHBQPNhiemVuChiParent(conn, trans, nvc, ref lstNVCUpdate);
                        //        }
                        //        conn.Insert(nvc, trans);
                        //    }
                        //}

                        // Còn những thằng nào dư ra thì delete
                        foreach (var idDelete in lstIdNVC)
                        {
                            var nvcTemp = new NH_KHChiTietBQP_NhiemVuChi();
                            nvcTemp.ID = idDelete;
                            conn.Delete(nvcTemp, trans);
                        }

                        // Sau khi insert thì tính tổng giá trị của BQP
                        //double fTongGiaTriUSD = 0;
                        //double fTongGiaTriVND = 0;
                        //foreach (var nvc in lstNVCUpdate)
                        //{
                        //    if (!nvc.iID_ParentID.HasValue)
                        //    {
                        //        fTongGiaTriUSD += nvc.fGiaTriUSD.HasValue ? nvc.fGiaTriUSD.Value : 0;
                        //        fTongGiaTriVND += nvc.fGiaTriVND.HasValue ? nvc.fGiaTriVND.Value : 0;
                        //    }
                        //}

                        khctGoc.fTongGiaTriUSD = fTongGiaTriUSD;
                        khctGoc.fTongGiaTriVND = fTongGiaTriVND;
                        conn.Update(khctGoc, trans);
                    }

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public NH_KHChiTietBQPModel GetKeHoachChiTietBQPById(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var query = @"SELECT * FROM NH_KHChiTietBQP WHERE ID = @Id";
                return conn.QueryFirstOrDefault<NH_KHChiTietBQPModel>(query, new { Id = id }, commandType: CommandType.Text);
            }
        }

        public Boolean DeleteKHBQP(Guid id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    // Xóa nhiệm vụ chi
                    var deleteNVC = @"DELETE FROM NH_KHChiTietBQP_NhiemVuChi WHERE iID_KHChiTietID = @Id";
                    conn.Execute(deleteNVC, new { Id = id }, trans);

                    // Update active và xóa kế hoạch chi tiêt BQP
                    StringBuilder query = new StringBuilder(@"UPDATE NH_KHChiTietBQP SET bIsActive = 1 WHERE ID = (SELECT iID_ParentAdjustID FROM NH_KHChiTietBQP WHERE ID = @Id);");
                    query.Append(@"DELETE NH_KHChiTietBQP WHERE ID = @Id");
                    conn.Execute(query.ToString(), new { Id = id }, trans, commandType: CommandType.Text);

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public NH_KHChiTietBQP_NVCViewModel GetDetailKeHoachChiTietBQP(string state, Guid? KHTTCP_ID, Guid? KHBQP_ID, Guid? iID_BQuanLyID, Guid? iID_DonViID, string sTenNhiemVuChi, string sTenPhongBan, string sTenDonVi, bool isUseLastTTCP)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                NH_KHChiTietBQP_NVCViewModel result = new NH_KHChiTietBQP_NVCViewModel();
                result.Items = new List<NH_KHChiTietBQP_NVCModel>();

                if (state == "CREATE" && KHTTCP_ID.HasValue && KHTTCP_ID != Guid.Empty)
                {
                    // Nếu là tạo mới thì lấy các trường của TTCP
                    var queryInfo = @"
                    SELECT
                        TTCP.sSoKeHoach AS sSoKeHoachTTCP,
                        TTCP.dNgayKeHoach AS dNgayKeHoachTTCP
                    FROM NH_KHTongTheTTCP AS TTCP
                    WHERE TTCP.ID = @Id";
                    result = conn.QueryFirstOrDefault<NH_KHChiTietBQP_NVCViewModel>(queryInfo, new { Id = KHTTCP_ID.Value }, commandType: CommandType.Text);

                    // Lấy thêm các nhiệm vụ chi của TTCP trong DB
                    var lstPrams = new DynamicParameters();
                    lstPrams.Add("KHTTCP_ID", KHTTCP_ID.Value);
                    result.Items = conn.Query<NH_KHChiTietBQP_NVCModel>("sp_get_create_KeHoachChiTietBQP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                }
                else if (state == "DETAIL" && KHBQP_ID.HasValue && KHBQP_ID != Guid.Empty)
                {
                    // Nếu là update thì lấy toàn bộ data trong DB
                    var queryInfo = @"
                    SELECT
                        BQP.ID AS ID,
                        BQP.iGiaiDoanTu AS iGiaiDoanTu,
                        BQP.iGiaiDoanDen AS iGiaiDoanDen,
                        BQP.iNamKeHoach AS iNamKeHoach,
                        BQP.iID_TiGiaID AS iID_TiGiaID,
                        BQP.sSoKeHoach AS sSoKeHoachBQP,
                        BQP.dNgayKeHoach AS dNgayKeHoachBQP,
                        TTCP.sSoKeHoach AS sSoKeHoachTTCP,
                        TTCP.dNgayKeHoach AS dNgayKeHoachTTCP,
                        BQP.iLoai AS iLoai
                    FROM NH_KHChiTietBQP BQP
                    LEFT JOIN NH_KHTongTheTTCP AS TTCP ON BQP.iID_KHTongTheTTCPID = TTCP.ID
                    WHERE BQP.ID = @Id";
                    result = conn.QueryFirstOrDefault<NH_KHChiTietBQP_NVCViewModel>(queryInfo, new { Id = KHBQP_ID.Value }, commandType: CommandType.Text);

                    // Lấy các nhiệm vụ chi BQP đã có trong DB.
                    var lstPrams = new DynamicParameters();
                    lstPrams.Add("KHBQP_ID", KHBQP_ID.Value);
                    lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID);
                    lstPrams.Add("iID_DonViID", iID_DonViID);
                    lstPrams.Add("sTenNhiemVuChi", sTenNhiemVuChi);
                    lstPrams.Add("sTenPhongBan", sTenPhongBan);
                    lstPrams.Add("sTenDonVi", sTenDonVi);

                    result.Items = conn.Query<NH_KHChiTietBQP_NVCModel>("sp_get_detail_KeHoachChiTietBQP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                }
                else if ((state == "UPDATE" || state == "ADJUST") && KHTTCP_ID.HasValue && KHBQP_ID.HasValue && KHTTCP_ID != Guid.Empty && KHBQP_ID != Guid.Empty)
                {
                    // Nếu là edit hoặc điều chỉnh thì lấy các trường của TTCP
                    var queryInfo = @"
                    SELECT
                        TTCP.sSoKeHoach AS sSoKeHoachTTCP,
                        TTCP.dNgayKeHoach AS dNgayKeHoachTTCP
                    FROM NH_KHTongTheTTCP AS TTCP
                    WHERE TTCP.ID = @Id";
                    result = conn.QueryFirstOrDefault<NH_KHChiTietBQP_NVCViewModel>(queryInfo, new { Id = KHTTCP_ID.Value }, commandType: CommandType.Text);

                    // Lấy các nhiệm vụ chi BQP đã có trong DB.
                    if (state == "ADJUST" && isUseLastTTCP)
                    {
                        var lstPrams = new DynamicParameters();
                        lstPrams.Add("KHTTCP_ID", KHTTCP_ID.Value);
                        result.Items = conn.Query<NH_KHChiTietBQP_NVCModel>("sp_get_create_KeHoachChiTietBQP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                    }
                    else
                    {
                        var lstPrams = new DynamicParameters();
                        lstPrams.Add("KHBQP_ID", KHBQP_ID.Value);
                        lstPrams.Add("sTenNhiemVuChi", sTenNhiemVuChi);
                        lstPrams.Add("sTenPhongBan", sTenPhongBan);
                        lstPrams.Add("sTenDonVi", sTenDonVi);
                        result.Items = conn.Query<NH_KHChiTietBQP_NVCModel>("sp_get_detail_KeHoachChiTietBQP", lstPrams, commandType: CommandType.StoredProcedure).ToList();
                    }
                }

                return result;
            }
        }

        public IEnumerable<LookupDto<Guid, string>> getLookupPhongBan()
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT iID_MaPhongBan AS Id, CONCAT(sTen, IIF(sMoTa = '', '', CONCAT(' - ', sMoTa))) AS DisplayName
                            FROM NS_PhongBan";
                return connection.Query<LookupDto<Guid, string>>(query, commandType: CommandType.Text);
            }
        }

        public List<NH_DM_TiGia_ChiTiet_ViewModel> GetTiGiaChiTietByTiGiaId(Guid iID_TiGiaID)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT TG.ID AS iID_TiGiaID, TG.sMaTienTeGoc, TGCT.ID AS iID_TiGiaChiTietID, TGCT.sMaTienTeQuyDoi, TGCT.fTiGia
                    FROM NH_DM_TiGia TG
                    LEFT JOIN NH_DM_TiGia_ChiTiet TGCT ON TG.ID = TGCT.iID_TiGiaID
                    WHERE TG.ID = @Id";
                return connection.Query<NH_DM_TiGia_ChiTiet_ViewModel>(query, new { Id = iID_TiGiaID }, commandType: CommandType.Text).ToList();
            }
        }

        public IEnumerable<NH_KHChiTietBQP_NVCModel> GetListBQPNhiemVuChiById(Guid id, string sTenNhiemVuChi, Guid? iID_BQuanLyID, Guid? iID_DonViID)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var lstPrams = new DynamicParameters();
                lstPrams.Add("KHBQP_ID", id);
                lstPrams.Add("sTenNhiemVuChi", sTenNhiemVuChi);
                lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID);
                lstPrams.Add("iID_DonViID", iID_DonViID);

                return connection.Query<NH_KHChiTietBQP_NVCModel>("sp_get_all_BQPNhiemVuChiById", lstPrams, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<LookupKHBQP> getLookupKHBQPByKHTTCPId(Guid id)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"SELECT IIF(iLoai = 1, 
			                            CONCAT('KHTT ', iGiaiDoanTu, ' - ', iGiaiDoanDen, N' -  Số KH: ', sSoKeHoach), 
			                            CONCAT('KHTT ', iNamKeHoach, N' -  Số KH:', sSoKeHoach)) AS DisplayName, 
                                    ID AS Id, iGiaiDoanTu, iGiaiDoanDen, iNamKeHoach, iLoai
                            FROM NH_KHChiTietBQP WHERE iLoai = 1 AND iID_KHTongTheTTCPID = @Id";
                return connection.Query<LookupKHBQP>(query, new { Id = id }, commandType: CommandType.Text);
            }
        }

        // Insert và ném ra ID của thằng cha
        private Guid? GetIdKHBQPNhiemVuChiParent(SqlConnection conn, SqlTransaction trans, NH_KHChiTietBQP_NhiemVuChi nvc, ref List<NH_KHChiTietBQP_NhiemVuChi> lstNhiemVuChis)
        {
            // Nếu thằng cha này đã insert thì ném ra ID của thằng cha, chưa insert thì check tiếp.
            if (nvc.ID == Guid.Empty)
            {
                // Nếu thằng cha đã có ParentID thì insert luôn, ko thì check tiếp.
                if (!nvc.iID_ParentID.HasValue)
                {
                    // Tìm bản ghi cha dựa vào mã thứ tự
                    var indexOfDot = nvc.sMaThuTu.LastIndexOf(".");
                    if (indexOfDot == -1)
                    {
                        // Nếu không có parent thì insert luôn.
                        conn.Insert(nvc, trans);
                    }
                    else
                    {
                        // Lấy mã thứ tự của bản ghi cha.
                        var sttParent = nvc.sMaThuTu.Substring(0, indexOfDot);
                        // Tìm bản ghi cha
                        var parent = lstNhiemVuChis.FirstOrDefault(x => x.sMaThuTu == sttParent);
                        // Nếu tìm ko ra thằng cha thì insert luôn
                        if (parent == null)
                        {
                            conn.Insert(nvc, trans);
                        }
                        else
                        {
                            // Nếu tìm thấy thì ném lại vào đệ quy để check lại, nếu đã insert thì return ra id thằng cha đó luôn, nếu chưa thì check tiếp.
                            return GetIdKHBQPNhiemVuChiParent(conn, trans, parent, ref lstNhiemVuChis);
                        }
                    }
                }
                else
                {
                    conn.Insert(nvc, trans);
                }
            }

            // Sau khi đã thực hiện N thao tác thì đã insert được thằng cha, nên giờ chỉ cần ném Id của nó ra thôi.
            return nvc.ID;
        }
        #endregion

        #region QLNH - Báo cáo tình hình thực hiện dự án
        public IEnumerable<NS_DonVi> GetDonviListByYear(int namLamViec = 0)
        {
            var sql =
                @"SELECT DISTINCT b.iID_MaDonVi, b.sTen, (b.iID_MaDonVi + ' - ' + b.sTen) as sMoTa, b.iID_Ma
                FROM ns_donvi b
                WHERE b.iNamLamViec_DonVi = @namLamViec AND b.iTrangThai = 1
                ORDER BY iID_MaDonVi";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(new CommandDefinition(
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

        public NH_DA_DuAnViewModel GetDuAnById(Guid? iId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT da.*, cdt.sTenCDT AS SChuDauTu, pcpd.sTen AS STen FROM NH_DA_DuAn da ");
            sb.AppendLine("LEFT JOIN DM_ChuDauTu cdt ON da.iID_ChuDauTuID = cdt.ID ");
            sb.AppendLine("LEFT JOIN NH_DM_PhanCapPheDuyet pcpd on da.iID_CapPheDuyetID = pcpd.ID ");
            if (iId.HasValue)
            {
                sb.AppendLine("WHERE da.ID = @iId");
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_DA_DuAnViewModel>(sb.ToString(), param: iId.HasValue ? new { iId } : null, commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<NH_DA_DuAn> GetListDuAnByDonViID(Guid iID)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT da.ID, da.sTenDuAn FROM NH_DA_DuAn da ");
                    sb.AppendLine("INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViID ");
                    if (iID != null && !iID.Equals(Guid.Empty))
                    {
                        sb.AppendLine("WHERE da.iID_DonViID=@iID ");
                    }
                    return conn.Query<NH_DA_DuAn>(sb.ToString(),
                        param: (iID != null && !iID.Equals(Guid.Empty) ? new { iID } : null));
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return null;
        }

        public BaoCaoTHTHDuAnModel GetBaoCaoTinhHinhThucHienDuAn(ref PagingInfo _paging, DateTime? dBatDau = null, DateTime? dKetThuc = null, Guid? iID_DuAnID = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("dBatDau", dBatDau);
                lstPrams.Add("dKetThuc", dKetThuc);
                lstPrams.Add("iID_DuAnID", iID_DuAnID);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_TT_ThanhToanViewModel>("proc_get_all_baocao_tinhhinhthuchien_duan_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                double Sum = 0;
                double Sumgn = 0;
                if (items.Any())
                {
                    Sum = items.FirstOrDefault().fGiaTriDuocCap_USD;
                    Sumgn = items.FirstOrDefault().fGiaTriTTTU_USD;
                }

                //foreach (var item in items)
                //{
                //    if (item.iCoQuanThanhToan.HasValue)
                //    {
                //        if (item.iCoQuanThanhToan.Value == 1)
                //        {
                //            Sum += item.fTongPheDuyet_USD ?? 0d;
                //        }
                //        else if (item.iCoQuanThanhToan.Value == 2)
                //        {
                //            if (item.iLoaiDeNghi.HasValue)
                //            {
                //                if (item.iLoaiDeNghi.Value == 1)
                //                {
                //                    Sum += item.fTongPheDuyet_USD ?? 0d;
                //                }
                //                else if (item.iLoaiDeNghi.Value != 1)
                //                {
                //                    Sumgn += item.fTongPheDuyet_USD ?? 0d;
                //                }
                //            }
                //        }
                //    }
                //}
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");

                BaoCaoTHTHDuAnModel baoCaoTHTHModel = new BaoCaoTHTHDuAnModel
                {
                    Items = items,
                    Sum = Sum,
                    Sumgn = Sumgn
                };
                return baoCaoTHTHModel;
            }
        }

        public BaoCaoTHTHDuAnModel GetDataExportBaoCaoTinhHinhThucHienDuAn(DateTime? dBatDau = null, DateTime? dKetThuc = null, Guid? iID_DuAnID = null)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("dBatDau", dBatDau);
                lstPrams.Add("dKetThuc", dKetThuc);
                lstPrams.Add("iID_DuAnID", iID_DuAnID);

                var items = conn.Query<NH_TT_ThanhToanViewModel>("sp_export_baocao_tinhhinhthuchien_duan", lstPrams, commandType: CommandType.StoredProcedure);

                double Sum = items.FirstOrDefault().fGiaTriDuocCap_USD;
                double Sumgn = items.FirstOrDefault().fGiaTriTTTU_USD;
                //foreach (var item in items)
                //{
                //    if (item.iCoQuanThanhToan.HasValue)
                //    {
                //        if (item.iCoQuanThanhToan.Value == 1)
                //        {
                //            Sum += item.fTongPheDuyet_USD ?? 0d;
                //        }
                //        else if (item.iCoQuanThanhToan.Value == 2)
                //        {
                //            if (item.iLoaiDeNghi.HasValue)
                //            {
                //                if (item.iLoaiDeNghi.Value == 1)
                //                {
                //                    Sum += item.fTongPheDuyet_USD ?? 0d;
                //                }
                //                else if (item.iLoaiDeNghi.Value != 1)
                //                {
                //                    Sumgn += item.fTongPheDuyet_USD ?? 0d;
                //                }
                //            }
                //        }
                //    }
                //}

                BaoCaoTHTHDuAnModel baoCaoTHTHModel = new BaoCaoTHTHDuAnModel
                {
                    Items = items,
                    Sum = Sum,
                    Sumgn = Sumgn
                };
                return baoCaoTHTHModel;
            }
        }
        #endregion

        #region QLNH - Thông tri cấp phát
        public IEnumerable<ThanhToan_ThongTriModel> GetListThanhToanTaoThongTri(ref PagingInfo _paging, Guid? iThongTri, Guid? iDonVi, int? iNam, int? iLoaiThongTri, int? iLoaiNoiDung, int? iTypeAction = 0)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("iThongTri", iThongTri);
                    lstParam.Add("iDonVi", iDonVi);
                    lstParam.Add("iNam", iNam);
                    lstParam.Add("iLoaiThongTri", iLoaiThongTri);
                    lstParam.Add("iLoaiNoiDung", iLoaiNoiDung);
                    lstParam.Add("iTypeAction", iTypeAction);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<ThanhToan_ThongTriModel>("proc_get_all_de_thanhtoan_capphat_paging", lstParam,
                        commandType: CommandType.StoredProcedure);
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

        public Boolean CheckTrungMaThongTri(string mathongtri, int type_action, Guid? imathongtri)
        {
            var sql = FileHelpers.GetSqlQuery("checktrungmathongtricapphat.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<Boolean>(sql,
                        param: new
                        {
                            mathongtri,
                            type_action,
                            imathongtri,
                        },
                        commandType: CommandType.Text);

                    return items;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return true;
        }

        public bool SaveDanhSachPheDuyetThanhToan(Guid? iThongTri, string lstDeNghiThanhToan, int iTypeAction)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iThongTri", iThongTri);
                lstParam.Add("lstDeNghiThanhToan", lstDeNghiThanhToan);
                lstParam.Add("iTypeAction", iTypeAction);
                var r = conn.Execute("proc_create_chitiet_thongtricapphat", lstParam, commandType: CommandType.StoredProcedure, transaction: trans);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }

        public IEnumerable<ThongTriCapPhatModel> GetListThongTriCapPhatPaging(ref PagingInfo _paging, Guid? iDonVi, string sMaThongTri, DateTime? dNgayLap, int? iNam)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    DynamicParameters lstParam = new DynamicParameters();
                    lstParam.Add("iDonVi", iDonVi);
                    lstParam.Add("sMaThongTri", sMaThongTri);
                    lstParam.Add("dNgayLap", dNgayLap);
                    lstParam.Add("iNam", iNam);
                    lstParam.Add("CurrentPage", _paging.CurrentPage);
                    lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                    lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var items = conn.Query<ThongTriCapPhatModel>("proc_get_all_thongtri_capphat_paging", lstParam,
                        commandType: CommandType.StoredProcedure);
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

        public ThongTriCapPhatModel GetThongTriByID(Guid? IdThongTri)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "select tt.ID as ID, tt.sSoThongTri as sSoThongTri, tt.dNgayLap as dNgayLap, tt.dNgayTao as dNgayTao , CONCAT(dv.iID_MaDonVi, '-', dv.sTen) as sTenDonvi, dv.sTen,tt.iNamThongTri as iNamThongTri,"
                    + " tt.fThongTri_USD as fThongTri_USD, tt.fThongTri_VND as fThongTri_VND, tt.iID_DonViID as iID_DonViID, tt.iLoaiThongTri as iLoaiThongTri, tt.iLoaiNoiDungChi as iLoaiNoiDungChi,  "
                    + " case when tt.iLoaiThongTri  = 1 then N'Thông tri cấp kinh phí'"
                    + " when tt.iLoaiThongTri  = 2 then N'Thông tri thanh toán'"
                    + " when tt.iLoaiThongTri  = 3 then N'Thông tri tạm ứng'"
                    + " when tt.iLoaiThongTri  = 4 then N'Thông tri giảm cấp kinh phí'"
                    + " end as sLoaiThongTri,"
                    + " case when tt.iLoaiNoiDungChi  = 1 then N'Chi ngoại tệ'"
                    + " when tt.iLoaiNoiDungChi  = 2 then N'Chi trong nước'"
                    + " end as sLoaiNoiDung"
                    + " from NH_TT_ThongTriCapPhat as tt"
                    + " inner join NS_DonVi as dv on tt.iID_DonViID = dv.iID_Ma"
                    + " where tt.ID = @IdThongTri";
                var items = conn.QueryFirstOrDefault<ThongTriCapPhatModel>(sql, param: new { IdThongTri },
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_TT_ThongTriCapPhat_ChiTiet> GetListhongTriChiTietByID(Guid? IdThongTri)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "Select * from NH_TT_ThongTriCapPhat_ChiTiet where iID_ThongTriCapPhatID = @IdThongTri";
                var items = conn.Query<NH_TT_ThongTriCapPhat_ChiTiet>(sql, param: new { IdThongTri },
                    commandType: CommandType.Text);
                return items;
            }
        }

        public bool DeleteThongTriCapPhat(Guid? id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE NH_TT_ThongTriCapPhat_ChiTiet WHERE iID_ThongTriCapPhatID = '{0}'; ", id);
                query.AppendFormat("DELETE NH_TT_ThongTriCapPhat WHERE ID = '{0}'; ", id);

                var r = conn.Execute(query.ToString(), commandType: CommandType.Text, transaction: trans);

                trans.Commit();
                conn.Close();
                return r >= 0;
            }
        }
        public IEnumerable<ThongTriBaoCaoModel> ExportBaoCaoThongTriCapPhat(Guid? idThongTri)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "select ROW_NUMBER() OVER(ORDER BY ct.ID) as STT, sM, sTM, sTTM, sNG, ct.sTenNoiDungChi, ct.fPheDuyetCapKyNay_USD, ct.fPheDuyetCapKyNay_VND from NH_TT_ThanhToan_ChiTiet as ct"
                    + " inner join NH_TT_ThanhToan  as tt on ct.iID_ThanhToanID = tt.ID"
                    + " inner join NH_TT_ThongTriCapPhat_ChiTiet as cp_ct on cp_ct.iID_ThanhToanID =  tt.ID"
                    + " left join NS_MucLucNganSach as ns on ns.iID_MaMucLucNganSach = ct.iID_MucLucNganSachID"
                    + " where cp_ct.iID_ThongTriCapPhatID = @idThongTri";
                var items = conn.Query<ThongTriBaoCaoModel>(sql, param: new { idThongTri },
                    commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Quyết toán dự án hoàn thành

        public IEnumerable<NH_QT_QuyetToanDAHTData> GetListQuyetToanDuAnHT(ref PagingInfo _paging, string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iDonVi, int? iNamBaoCaoTu, int? iNamBaoCaoDen, int? tabIndex)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoDeNghi", sSoDeNghi);
                lstParam.Add("dNgayDeNghi", dNgayDeNghi);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iNamBaoCaoTu", iNamBaoCaoTu);
                lstParam.Add("iNamBaoCaoDen", iNamBaoCaoDen);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("tabIndex", tabIndex);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_QT_QuyetToanDAHTData>("proc_get_all_quyettoanduan_paging", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public NH_QT_QuyetToanDAHTData GetThongTinQuyetToanDuAnHTById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_QT_QuyetToanDAHT WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_QT_QuyetToanDAHTData>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }
        public bool DeleteQuyetToanDuAnHT(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DELETE NH_QT_QuyetToanDAHT WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<NH_QT_QuyetToanDAHT>(iId);
                if (entity.sTongHop != null)
                {
                    foreach (var id in entity.sTongHop.Split(','))
                    {
                        var entityCon = conn.Get<NH_QT_QuyetToanDAHT>(id);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = null;
                        conn.Update(entityCon);
                    }
                }
                var e = conn.Execute("delete  NH_QT_QuyetToanDAHT_ChiTiet where iID_DeNghiQuyetToanDAHT_ID =  @iId", param: new { iId = iId }, commandType: CommandType.Text);
                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                return r > 0;
            }
        }

        public bool LockOrUnLockQuyetToanDuAnHT(Guid id, bool isLockOrUnLock)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                var entity = conn.Get<NH_QT_QuyetToanDAHT>(id, trans);
                if (entity == null) return false;
                entity.bIsKhoa = isLockOrUnLock;
                conn.Update(entity, trans);

                trans.Commit();
                conn.Close();

                return true;
            }
        }
        public NH_QT_QuyetToanDuAnHTReturnData SaveQuyetToanDuAnHT(NH_QT_QuyetToanDAHT data, string userName)
        {
            NH_QT_QuyetToanDuAnHTReturnData dt = new NH_QT_QuyetToanDuAnHTReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_QT_QuyetToanDAHT();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = userName;
                        entity.bIsKhoa = false;
                        entity.bIsXoa = false;
                        conn.Insert(entity, trans);
                        dt.QuyetToanDuAnHTData = entity;
                    }
                    else
                    {
                        var entity = conn.Get<NH_QT_QuyetToanDAHT>(data.ID, trans);
                        if (entity == null)
                        {
                            dt.IsReturn = false;
                            return dt;
                        }

                        entity.iID_DonViID = data.iID_DonViID;
                        entity.iID_MaDonVi = data.iID_MaDonVi;
                        entity.iID_TiGiaID = data.iID_TiGiaID;
                        entity.iNamBaoCaoTu = data.iNamBaoCaoTu;
                        entity.iNamBaoCaoDen = data.iNamBaoCaoDen;
                        entity.iTrangThai = data.iTrangThai;
                        entity.sSoDeNghi = data.sSoDeNghi;
                        entity.sMoTa = data.sMoTa;
                        entity.dNgayDeNghi = data.dNgayDeNghi;
                        entity.dNgaySua = DateTime.Now;
                        entity.sNguoiSua = userName;
                        conn.Update(entity, trans);
                        dt.QuyetToanDuAnHTData = entity;
                    }

                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }
        public bool SaveTongHopQuyetToanDAHT(NH_QT_QuyetToanDAHT data, string userName, string listId)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = new NH_QT_QuyetToanDAHT();
                    entity.MapFrom(data);
                    if (entity.sTongHop != null)
                    {
                        entity.sTongHop += "," + listId;
                    }
                    else
                    {
                        entity.sTongHop = listId;
                    }

                    entity.dNgayTao = DateTime.Now;
                    entity.sNguoiTao = userName;
                    entity.bIsKhoa = false;
                    entity.bIsXoa = false;
                    conn.Insert(entity, trans);

                    foreach (var id in listId.Split(','))
                    {
                        var entityCon = conn.Get<NH_QT_QuyetToanDAHT>(id, trans);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = entity.ID;
                        entityCon.dNgaySua = DateTime.Now;
                        entityCon.sNguoiSua = userName;
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

        public IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> GetDetailQuyetToanDuAnDetail(int? iNamBaoCaoTu, int? iNamBaoCaoDen, Guid? iIDDonVi, Guid? iIDQuyetToan, int? donViTinhUSD, int? donViTinhVND)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamBaoCaoTu", iNamBaoCaoTu);
                lstParam.Add("iNamBaoCaoDen", iNamBaoCaoDen);
                lstParam.Add("iIDDonVi", iIDDonVi);
                lstParam.Add("iIDQuyetToan", iIDQuyetToan);
                lstParam.Add("devideDonViUSD", donViTinhUSD);
                lstParam.Add("devideDonViVND", donViTinhVND);


                var items = conn.Query<NH_QT_QuyetToanDAHT_ChiTietData>("proc_get_all_nh_baocao_quyettoanduan_detail", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        public IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> GetDetailQuyetToanDuAnCreate(int? iNamBaoCaoTu, int? iNamBaoCaoDen, Guid? iIDDonVi, int? donViTinhUSD, int? donViTinhVND)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamBaoCaoTu", iNamBaoCaoTu);
                lstParam.Add("iNamBaoCaoDen", iNamBaoCaoDen);
                lstParam.Add("iIDDonVi", iIDDonVi);
                lstParam.Add("devideDonViUSD", donViTinhUSD);
                lstParam.Add("devideDonViVND", donViTinhVND);



                var items = conn.Query<NH_QT_QuyetToanDAHT_ChiTietData>("proc_get_all_nh_baocao_quyettoanduan_create", lstParam,
                    commandType: CommandType.StoredProcedure).OrderBy(x => x.iID_DonVi);
                return items;
            }
        }

        public NH_QT_QuyetToanDAHTChiTietReturnData SaveQuyetToanDuAnDetail(List<NH_QT_QuyetToanDAHT_ChiTiet> listData, string userName)
        {
            NH_QT_QuyetToanDAHTChiTietReturnData dt = new NH_QT_QuyetToanDAHTChiTietReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    foreach (var data in listData)
                    {

                        if (data.ID == null || data.ID == Guid.Empty)
                        {
                            var entity = new NH_QT_QuyetToanDAHT_ChiTiet();
                            entity.MapFrom(data);
                            conn.Insert(entity, trans);
                            dt.QuyetToanDuAnChiTietData = entity;
                        }
                        else
                        {
                            var entity = conn.Get<NH_QT_QuyetToanDAHT_ChiTiet>(data.ID, trans);
                            if (entity == null)
                            {
                                dt.IsReturn = false;
                                return dt;
                            }
                            entity.MapFrom(data);
                            conn.Update(entity, trans);
                            dt.QuyetToanDuAnChiTietData = entity;
                        }
                    }

                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }
        public IEnumerable<NH_QT_QuyetToanDAHTData> GetListTongHopQuyetToanDAHT(string sSodenghi, DateTime? dNgaydenghi, Guid? iDonvi, int? iNamTu, int? iNamDen)
        {
            if (iDonvi == Guid.Empty)
            {
                iDonvi = null;
            }
            var sSoDeNghi = sSodenghi;
            var dNgayDeNghi = dNgaydenghi;
            var iID_DonViID = iDonvi;
            var iNamBaoCaoTu = iNamTu;
            var iNamBaoCaoDen = iNamDen;

            var sql =
                @"
                SELECT DISTINCT qtda.ID, qtda.sSoDeNghi,qtda.iTrangThai, qtda.dNgayDeNghi , qtda.iID_DonViID, qtda.iNamBaoCaoTu,iNamBaoCaoDen, qtda.iID_TiGiaID
               ,concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi,
               qtda.bIsKhoa,qtda.iID_TongHopID,qtda.sTongHop
                INTO #tmp
                from NH_QT_QuyetToanDAHT as qtda  
                left join NS_DonVi dv on qtda.iID_DonViID = dv.iID_Ma
                 WHERE
				(ISNULL(@sSoDeNghi,'') = '' OR qtda.sSoDeNghi like CONCAT(N'%', @sSoDeNghi, N'%')) 
				 AND (ISNULL(@iNamBaoCaoTu,0) = 0 OR qtda.iNamBaoCaoTu  = @iNamBaoCaoTu)
                 AND (ISNULL(@iNamBaoCaoDen,0) = 0 OR qtda.iNamBaoCaoDen  = @iNamBaoCaoDen)
				AND (@dNgayDeNghi is   null or qtda.dNgayDeNghi = @dNgayDeNghi)
				AND (@iID_DonViID IS NULL OR qtda.iID_DonViID = @iID_DonViID)
                ;
                WITH cte(ID, sSoDeNghi,iTrangThai, dNgayDeNghi, iID_DonViID, iNamBaoCaoTu,iNamBaoCaoDen, iID_TiGiaID,bIsKhoa,iID_TongHopID,sTongHop)
                AS
                (
	                SELECT 
					  lct.ID
					, lct.sSoDeNghi
					, lct.iTrangThai
					, lct.dNgayDeNghi 
					, lct.iID_DonViID
					, lct.iNamBaoCaoTu
					, lct.iNamBaoCaoDen
					, lct.iID_TiGiaID
					, lct.bIsKhoa
					, lct.iID_TongHopID
					, lct.sTongHop
	                FROM NH_QT_QuyetToanDAHT lct , #tmp tmp
	                WHERE lct.ID  = tmp.iID_TongHopID
	                UNION ALL
	                SELECT 
					  cd.ID
					, cd.sSoDeNghi
					, cd.iTrangThai
					, cd.dNgayDeNghi 
					, cd.iID_DonViID
					, cd.iNamBaoCaoTu
					, cd.iNamBaoCaoDen
					, cd.iID_TiGiaID
					, cd.bIsKhoa
					, cd.iID_TongHopID
					, cd.sTongHop
	                FROM cte as NCCQ, #tmp as cd
	                WHERE cd.iID_TongHopID = NCCQ.ID
                )
                SELECT DISTINCT 
				  cte.ID
				, cte.sSoDeNghi
				, cte.iTrangThai
				, dNgayDeNghi
				, iID_DonViID
				, iNamBaoCaoTu
				, iNamBaoCaoDen
				, iID_TiGiaID
                , bIsKhoa
				, iID_TongHopID
				, sTongHop
				, concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi
				 INTO #db
                 FROM cte 
                left join NS_DonVi dv on cte.iID_DonViID = dv.iID_Ma 
                UNION ALL
                SELECT DISTINCT 
				  qtda.ID
				, qtda.sSoDeNghi
				, qtda.iTrangThai
				, qtda.dNgayDeNghi
				, qtda.iID_DonViID
				, qtda.iNamBaoCaoTu
				, qtda.iNamBaoCaoDen
				, qtda.iID_TiGiaID
				, qtda.bIsKhoa
				, qtda.iID_TongHopID
				, qtda.sTongHop
				, concat(dv.iID_MaDonVi,'-',dv.sTen) as sTenDonVi
                from NH_QT_QuyetToanDAHT as qtda  
                inner join NH_QT_QuyetToanDAHT as cd on qtda.ID = cd.iID_TongHopID
                left join NS_DonVi dv on qtda.iID_DonViID = dv.iID_Ma
                where (ISNULL(@sSoDeNghi,'') = '' or qtda.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%'))
	            and (@iID_DonViID is null or qtda.iID_DonViID = @iID_DonViID) 
	            AND (ISNULL(@iNamBaoCaoTu,0) = 0 OR qtda.iNamBaoCaoTu = @iNamBaoCaoTu)
                AND (ISNULL(@iNamBaoCaoDen,0) = 0 OR qtda.iNamBaoCaoDen = @iNamBaoCaoDen)
				and  qtda.iID_TongHopID is null and ( qtda.sTongHop is null or qtda.sTongHop = '')
                Order by cte.iID_TongHopID

                Select db.* ,ROW_NUMBER() OVER (ORDER BY db.iID_TongHopID) AS sSTT from  #db  db
                DROP TABLE #tmp
                DROP TABLE #db

";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_QT_QuyetToanDAHTData>(sql,
                    param: new
                    {
                        sSoDeNghi,
                        dNgayDeNghi,
                        iID_DonViID,
                        iNamBaoCaoTu,
                        iNamBaoCaoDen
                    },
                     commandType: CommandType.Text
                 );

                return items;
            }
        }

        public IEnumerable<NH_DM_TiGia> GetTiGiaQuyetToan(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("select a.* from NH_DM_TiGia a left join NH_DM_TiGia_ChiTiet b on a.ID = b.iID_TiGiaID where a.sMaTienTeGoc in (UPPER('USD'),UPPER('VND')) and b.sMaTienTeQuyDoi in (UPPER('USD'),UPPER('VND')) ");
            if (id != null)
            {
                query.AppendLine("AND a.ID = @ID ");
            }

            query.AppendLine("ORDER BY sMaTiGia");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_TiGia>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }
        #endregion

        #region QLNH - Nội dung chi
        public IEnumerable<DanhmucNgoaiHoi_NoiDungChiModel> GetListDanhMucNoiDungChiModels(ref PagingInfo _paging,
          string sMaNoiDungChi, string sTenNoiDungChi, string sMoTa)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaNoiDungChi", sMaNoiDungChi);
                lstPrams.Add("sTenNoiDungChi", sTenNoiDungChi);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_NoiDungChiModel>("proc_get_all_DanhMucNoiDungChi_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhmucNgoaiHoi_NoiDungChiModel GetDanhMucNoiDungChiById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_NoiDungChi WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_NoiDungChiModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public IEnumerable<NH_DM_NoiDungChi> GetNHDMNoiDungChiList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DM_NoiDungChi ");
            if (id != null)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.Append("ORDER BY sMaNoiDungChi");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_NoiDungChi>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public bool SaveNoiDungChi(NH_DM_NoiDungChi data)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_NoiDungChi();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        conn.Insert<NH_DM_NoiDungChi>(entity, trans);
                    }
                    else
                    {
                        conn.Update<NH_DM_NoiDungChi>(data, trans);
                    }

                    trans.Commit();
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

        public bool DeleteDanhMucNoiDungChi(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_NoiDungChi>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_NoiDungChi>(checkExist, trans);
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
        #endregion

        #region QLNH - Hình thức chọn nhà thầu
        public IEnumerable<DanhmucNgoaiHoi_HinhThucChonNhaThauModel> GetListDanhMucHinhThucChonNhaThauPaging(ref PagingInfo _paging,
          string sMaHinhThuc, string sTenVietTat, string sTenHinhThuc, string sMoTa)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaHinhThuc", sMaHinhThuc);
                lstPrams.Add("sTenVietTat", sTenVietTat);
                lstPrams.Add("sTenHinhThuc", sTenHinhThuc);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_HinhThucChonNhaThauModel>("proc_get_all_HinhThucChonNhaThau_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhmucNgoaiHoi_HinhThucChonNhaThauModel GetDanhMucHinhThucChonNhaThauById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_HinhThucChonNhaThau WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_HinhThucChonNhaThauModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public bool SaveHinhThucChonNhaThau(NH_DM_HinhThucChonNhaThau data, int? soThuTu)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_HinhThucChonNhaThau();
                        data.iThuTu = soThuTu.HasValue ? (soThuTu.Value + 1) : 1;
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        conn.Insert<NH_DM_HinhThucChonNhaThau>(entity, trans);
                    }
                    else
                    {
                        var checkExist = conn.Get<NH_DM_HinhThucChonNhaThau>(data.ID, trans);
                        if (checkExist != null)
                        {
                            data.iThuTu = soThuTu;
                            data.dNgayTao = checkExist.dNgayTao;
                            conn.Update<NH_DM_HinhThucChonNhaThau>(data, trans);
                        }
                    }

                    trans.Commit();
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

        public bool DeleteDanhMucHinhThucChonNhaThau(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_HinhThucChonNhaThau>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_HinhThucChonNhaThau>(checkExist, trans);
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
        #endregion

        #region QLNH - Phương thức chọn nhà thầu
        public IEnumerable<DanhmucNgoaiHoi_PhuongThucChonNhaThauModel> GetListDanhMucPhuongThucChonNhaThauPaging(ref PagingInfo _paging,
          string sMaPhuongThuc, string sTenVietTat, string sTenPhuongThuc, string sMoTa)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sMaPhuongThuc", sMaPhuongThuc);
                lstPrams.Add("sTenVietTat", sTenVietTat);
                lstPrams.Add("sTenPhuongThuc", sTenPhuongThuc);
                lstPrams.Add("sMoTa", sMoTa);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_PhuongThucChonNhaThauModel>("proc_get_all_PhuongThucChonNhaThau_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhmucNgoaiHoi_PhuongThucChonNhaThauModel GetDanhMucPhuongThucChonNhaThauById(Guid iId)
        {
            var sql = "SELECT * FROM NH_DM_PhuongThucChonNhaThau WHERE ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_PhuongThucChonNhaThauModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }

        public bool SavePhuongThucChonNhaThau(NH_DM_PhuongThucChonNhaThau data, int? soThuTu)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_DM_PhuongThucChonNhaThau();
                        data.iThuTu = soThuTu.HasValue ? (soThuTu.Value + 1) : 1;
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        conn.Insert<NH_DM_PhuongThucChonNhaThau>(entity, trans);
                    }
                    else
                    {
                        var checkExist = conn.Get<NH_DM_PhuongThucChonNhaThau>(data.ID, trans);
                        if (checkExist != null)
                        {
                            data.iThuTu = soThuTu;
                            data.dNgayTao = checkExist.dNgayTao;
                            conn.Update<NH_DM_PhuongThucChonNhaThau>(data, trans);
                        }
                    }

                    trans.Commit();
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

        public bool DeleteDanhMucPhuongThucChonNhaThau(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    var checkExist = conn.Get<NH_DM_PhuongThucChonNhaThau>(iId, trans);
                    if (checkExist != null)
                    {
                        conn.Delete<NH_DM_PhuongThucChonNhaThau>(checkExist, trans);
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
        #endregion

        #region  QLNH - Báo cáo chi tiết số chuyển năm sau
        public IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetBaoCaoChiTietSoChuyenNamSauDetail(int? iNamKeHoach, Guid? iIDDonVi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("iNamKeHoach", iNamKeHoach);
                lstParam.Add("iIDDonVi", iIDDonVi);

                var items = conn.Query<NH_QT_QuyetToanNienDo_ChiTietData>("proc_get_all_nh_baocao_chitiet_sochuyennamsau", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region QLNH - Danh mục chương trình
        // Lấy danh sách chương trình
        public NH_KHChiTietBQPViewModel getListDanhMucChuongTrinh(PagingInfo _paging, string sTenNhiemVuChi, Guid? iID_BQuanLyID, Guid? iID_DonViID)
        {
            var result = new NH_KHChiTietBQPViewModel();
            using (var connection = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();

                lstPrams.Add("TenNVCFilter", sTenNhiemVuChi);
                lstPrams.Add("BQuanLyFilter", iID_BQuanLyID.HasValue ? iID_BQuanLyID.Value : (Guid?)null);
                lstPrams.Add("DonViFilter", iID_DonViID.HasValue ? iID_DonViID.Value : (Guid?)null);
                lstPrams.Add("SkipCount", (_paging.CurrentPage - 1) * _paging.ItemsPerPage);
                lstPrams.Add("MaxResultCount", _paging.ItemsPerPage);
                lstPrams.Add("TotalItems", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = connection.Query<NH_KHChiTietBQPModel>("sp_get_all_DanhMucChuongTrinh", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("TotalItems");
                result._paging = _paging;
                result.Items = items.ToList();
            }

            return result;
        }
        #endregion

        #region  QLNH - Báo cáo tổng hợp số chuyển năm sau
        public IEnumerable<NH_QT_QuyetToanNienDo_ChiTietData> GetBaoCaoTongHopSoChuyenNamSauDetail(int? iNamKeHoach)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamKeHoach", iNamKeHoach);

                var items = conn.Query<NH_QT_QuyetToanNienDo_ChiTietData>("proc_get_all_nh_baocao_tonghop_sochuyennamsau", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region QLNH - Nguồn Ngân Sách
        public IEnumerable<DanhmucNgoaiHoi_NguonNganSachModel> GetListDanhMucNguonNganSachPaging(ref PagingInfo _paging, string MaNguonNganSach, string sTenNguonNganSach, int? iTrangThai)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("MaNguonNganSach", MaNguonNganSach);
                lstPrams.Add("sTenNguonNganSach", sTenNguonNganSach);
                lstPrams.Add("iTrangThai", iTrangThai);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<DanhmucNgoaiHoi_NguonNganSachModel>("proc_get_all_danhmucnguonngansach_paging", lstPrams,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public DanhmucNgoaiHoi_NguonNganSachModel GetDanhMucNguonNganSachById(int? iId)
        {
            var sql = "SELECT * FROM NS_NguonNganSach WHERE iID_MaNguonNganSach = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<DanhmucNgoaiHoi_NguonNganSachModel>(sql, param: new { iId },
                    commandType: CommandType.Text);
                return item;
            }
        }
        public NH_DM_NguonNganSachReturnData SaveNguonNganSach(DanhmucNgoaiHoi_NguonNganSachModel data, string sIPSua, string username)
        {
            NH_DM_NguonNganSachReturnData dt = new NH_DM_NguonNganSachReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NS_NguonNganSach> itemsNS = conn.Query<NS_NguonNganSach>("SELECT * FROM NS_NguonNganSach", commandType: CommandType.Text).ToList();
                    var checkExistMaNganSach = itemsNS.FirstOrDefault(x => x.iID_MaNguonNganSach == data.iID_MaNguonNganSach && x.iID_MaNguonNganSach != data.iID_NguonNganSach);
                    if (checkExistMaNganSach != null)
                    {
                        dt.IsReturn = false;
                        dt.errorMess = "Mã nguồn ngân sách đã tồn tại!";
                        return dt;
                    }

                    var checkExistSTT = itemsNS.FirstOrDefault(x => x.iSTT.HasValue && data.iSTT.HasValue && x.iSTT.Value == data.iSTT.Value && x.iID_MaNguonNganSach != data.iID_NguonNganSach);
                    if (checkExistSTT != null)
                    {
                        dt.IsReturn = false;
                        dt.errorMess = "Đã tồn tại nguồn ngân sách có số thứ tự: " + data.iSTT.Value + "!";
                        return dt;
                    }
                    var trans = conn.BeginTransaction();
                    NS_NguonNganSach entity = new NS_NguonNganSach();
                    entity.MapFrom(data);
                    if (data.iID_NguonNganSach == null || data.iID_NguonNganSach.Value == 0)
                    {
                        entity.dNgayTao = DateTime.Now;
                        entity.sID_MaNguoiDungTao = username;
                        entity.sIPSua = sIPSua;
                        conn.Insert<NS_NguonNganSach>(entity, trans);
                        dt.LoaiNguonNganSachData = entity;
                    }
                    else
                    {
                        var nguonNganSach = itemsNS.FirstOrDefault(x => x.iID_MaNguonNganSach == entity.iID_MaNguonNganSach);
                        if (nguonNganSach != null)
                        {
                            if (nguonNganSach.iSoLanSua.HasValue)
                            {
                                entity.iSoLanSua += 1;
                            }
                            else
                            {
                                entity.iSoLanSua = 1;
                            }
                            entity.dNgaySua = DateTime.Now;
                            entity.sID_MaNguoiDungSua = username;
                            entity.sIPSua = sIPSua;
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("UPDATE NS_NguonNganSach SET ");
                            sb.AppendLine("sTen=@sTen,iTrangThai=@iTrangThai,iSTT=@iSTT,iSoLanSua=@iSoLanSua, ");
                            sb.AppendLine("dNgaySua=@dNgaySua,sID_MaNguoiDungSua=@sID_MaNguoiDungSua,sIPSua=@sIPSua ");
                            sb.AppendLine("WHERE iID_MaNguonNganSach=@iID_MaNguonNganSach ");
                            conn.Execute(sb.ToString(),
                                new
                                {
                                    sTen = entity.sTen,
                                    iTrangThai = entity.iTrangThai,
                                    iSTT = entity.iSTT,
                                    iSoLanSua = entity.iSoLanSua,
                                    dNgaySua = entity.dNgaySua,
                                    sID_MaNguoiDungSua = entity.sID_MaNguoiDungSua,
                                    sIPSua = entity.sIPSua,
                                    iID_MaNguonNganSach = entity.iID_MaNguonNganSach
                                }, trans, commandType: CommandType.Text);
                        }
                    }

                    trans.Commit();
                    dt.IsReturn = true;
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                dt.errorMess = ex.Message;
            }

            return dt;
        }

        public bool DeleteDanhMucNguonNganSach(int iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    string sql = "DELETE FROM NS_NguonNganSach WHERE iID_MaNguonNganSach = @iId";
                    conn.Execute(sql, new { iId }, trans, commandType: CommandType.Text);
                    trans.Commit();
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
        #endregion

        #region QLNH - Thông tin gói thầu
        public IEnumerable<NH_DA_GoiThauModel> GetAllNHThongTinGoiThau(ref PagingInfo _paging, string sTenGoiThau,
            Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sTenGoiThau", sTenGoiThau);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", iChuongTrinh);
                lstParam.Add("iDuAn", iDuAn);
                lstParam.Add("iLoai", iLoai);
                lstParam.Add("iThoiGianThucHien", iThoiGianThucHien);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_DA_GoiThauModel>("proc_get_all_nh_da_ttgoithau_paging_demo", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }
        public IEnumerable<NH_DA_GoiThauModel> GetAllNHThongTinGoiThauById(Guid id, string sTenGoiThau,
            Guid? iDonVi, Guid? iChuongTrinh, Guid? iDuAn, int? iLoai, int? iThoiGianThucHien)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sTenGoiThau", sTenGoiThau);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iChuongTrinh", id);
                lstParam.Add("iDuAn", iDuAn);
                lstParam.Add("iLoai", iLoai);
                lstParam.Add("iThoiGianThucHien", iThoiGianThucHien);

                var items = conn.Query<NH_DA_GoiThauModel>("proc_get_all_nh_da_ttgoithau", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public NH_DA_GoiThauModel GetThongTinGoiThauById(Guid id)
        {
            var sql = FileHelpers.GetSqlQuery("get_thongtin_goithau_byid.sql");
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var items = conn.QueryFirstOrDefault<NH_DA_GoiThauModel>(sql,
                        param: new
                        {
                            id
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

        public IEnumerable<NH_DM_LoaiTienTe> GetNHDMLoaiTienTeByCode(string maTienTe)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_DM_LoaiTienTe ");
            query.Append("WHERE 1=1 ");
            if (!string.IsNullOrEmpty(maTienTe))
            {
                query.AppendLine("AND sMaTienTe = @maTienTe ");
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_LoaiTienTe>(query.ToString()
                    , param: (!string.IsNullOrEmpty(maTienTe) ? new { maTienTe = maTienTe } : null)
                    , commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_HinhThucChonNhaThau> GetNHDMHinhThucChonNhaThauList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_HinhThucChonNhaThau ");
            if (id != null && id != Guid.Empty)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY iThuTu DESC");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_HinhThucChonNhaThau>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public IEnumerable<NH_DM_PhuongThucChonNhaThau> GetNHDMPhuongThucChonNhaThauList(Guid? id = null)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM NH_DM_PhuongThucChonNhaThau ");
            if (id != null && id != Guid.Empty)
            {
                query.AppendLine("WHERE ID = @ID ");
            }

            query.AppendLine("ORDER BY iThuTu DESC");
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_PhuongThucChonNhaThau>(query.ToString(),
                    param: (id != null ? new { ID = id } : null),
                    commandType: CommandType.Text);
                return items;
            }
        }

        public bool SaveThongTinGoiThau(NH_DA_GoiThau data, bool isDieuChinh, string userName)
        {
            bool isSuccess = false;
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    if (!isDieuChinh)
                    {
                        if (data.ID == null || data.ID == Guid.Empty)
                        {
                            var entity = new NH_DA_GoiThau();
                            entity.MapFrom(data);
                            entity.dNgayTao = DateTime.Now;
                            entity.sNguoiTao = userName;
                            entity.bIsActive = true;
                            entity.bIsGoc = true;
                            Guid idNew = conn.Insert(entity, trans);
                            isSuccess = idNew != Guid.Empty;
                        }
                        else
                        {
                            var entity = conn.Get<NH_DA_GoiThau>(data.ID, trans);
                            if (entity == null) return false;
                            entity.iID_DonViID = data.iID_DonViID;
                            entity.iID_MaDonVi = data.iID_MaDonVi;
                            entity.iID_KHCTBQP_ChuongTrinhID = data.iID_KHCTBQP_ChuongTrinhID;
                            entity.iPhanLoai = data.iPhanLoai;
                            entity.iID_DuAnID = data.iID_DuAnID;
                            entity.sSoKeHoachLCNT = data.sSoKeHoachLCNT;
                            entity.dNgayKeHoachLCNT = data.dNgayKeHoachLCNT;
                            entity.sSoKetQuaLCNT = data.sSoKetQuaLCNT;
                            entity.dNgayKetQuaLCNT = data.dNgayKetQuaLCNT;
                            entity.sSoPANK = data.sSoPANK;
                            entity.dNgayPANK = data.dNgayPANK;
                            entity.sSoKetQuaDamPhan = data.sSoKetQuaDamPhan;
                            entity.dNgayKetQuaDamPhan = data.dNgayKetQuaDamPhan;
                            entity.iID_HinhThucChonNhaThauID = data.iID_HinhThucChonNhaThauID;
                            entity.iID_PhuongThucChonNhaThauID = data.iID_PhuongThucChonNhaThauID;
                            entity.sTenGoiThau = data.sTenGoiThau;
                            entity.sThanhToanBang = data.sThanhToanBang;
                            entity.iID_LoaiHopDongID = data.iID_LoaiHopDongID;
                            entity.iThoiGianThucHien = data.iThoiGianThucHien;
                            entity.iID_NhaThauThucHienID = data.iID_NhaThauThucHienID;
                            entity.iID_TiGiaID = data.iID_TiGiaID;
                            entity.iID_TiGia_ChiTietID = data.iID_TiGia_ChiTietID;
                            entity.sMaNgoaiTeKhac = data.sMaNgoaiTeKhac;
                            entity.fGiaTriVND = data.fGiaTriVND;
                            entity.fGiaTriUSD = data.fGiaTriUSD;
                            entity.fGiaTriEUR = data.fGiaTriEUR;
                            entity.fGiaTriNgoaiTeKhac = data.fGiaTriNgoaiTeKhac;
                            entity.dNgaySua = DateTime.Now;
                            entity.sNguoiSua = userName;
                            isSuccess = conn.Update(entity, trans);
                        }
                    }
                    else
                    {
                        var entity = new NH_DA_GoiThau();
                        entity.MapFrom(data);
                        entity.ID = Guid.NewGuid();
                        if (data.iID_GoiThauGocID == null || data.iID_GoiThauGocID == Guid.Empty)
                            entity.iID_GoiThauGocID = data.ID;
                        entity.bIsActive = true;
                        entity.bIsGoc = false;
                        entity.iLanDieuChinh = data.iLanDieuChinh + 1;
                        entity.iID_ParentAdjustID = data.ID;
                        entity.sNguoiTao = userName;
                        entity.dNgayTao = DateTime.Now;

                        var entityGoc = conn.Get<NH_DA_GoiThau>(data.ID, trans);
                        entityGoc.bIsActive = false;
                        entityGoc.sNguoiSua = userName;
                        entityGoc.dNgaySua = DateTime.Now;

                        isSuccess = conn.Update(entityGoc, trans);
                        Guid idGoiThau = conn.Insert(entity, trans);
                        isSuccess &= idGoiThau != Guid.Empty;
                    }

                    if (!isSuccess)
                    {
                        trans.Rollback();
                    }
                    else
                    {
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                if (trans != null) trans.Rollback();
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool DeleteThongTinGoiThau(Guid iId)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE NH_DA_GoiThau SET bIsActive = 1 WHERE ID = (SELECT iID_ParentAdjustID FROM NH_DA_GoiThau WHERE ID = @iId);");
                    query.AppendLine("DELETE NH_DA_GoiThau WHERE ID = @iId");
                    conn.Execute(query.ToString(), param: new { iId = iId }, trans, commandType: CommandType.Text);

                    trans.Commit();
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

        public bool SaveImportThongTinGoiThau(List<NH_DA_GoiThau> packageList)
        {
            SqlTransaction trans = null;
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    conn.Insert<NH_DA_GoiThau>(packageList, trans);
                    trans.Commit();
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
        #endregion

        #region QLNH - Thông tri quyết toán

        public NH_QT_ThongTriQuyetToanViewModel GetListThongTriQuyetToan(PagingInfo _paging, NH_QT_ThongTriQuyetToanFilter filter)
        {
            var result = new NH_QT_ThongTriQuyetToanViewModel();
            using (var connection = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("iID_DonViID", filter.iID_DonViID);
                lstPrams.Add("iID_ChuongTrinhID", filter.iID_KHCTBQP_ChuongTrinhID);
                lstPrams.Add("sSoThongTri", filter.sSoThongTri);
                lstPrams.Add("dNgayLap", filter.dNgayLap);
                lstPrams.Add("iNamThucHien", filter.iNamThucHien);
                lstPrams.Add("iLoaiThongTri", filter.iLoaiThongTri);
                lstPrams.Add("iLoaiNoiDungChi", filter.iLoaiNoiDungChi);
                lstPrams.Add("SkipCount", (_paging.CurrentPage - 1) * _paging.ItemsPerPage);
                lstPrams.Add("MaxResultCount", _paging.ItemsPerPage);
                lstPrams.Add("TotalItems", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = connection.Query<NH_QT_ThongTriQuyetToanModel>("sp_get_all_ThongTriQuyetToan", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("TotalItems");
                result._paging = _paging;
                result.Items = items.ToList();
            }

            return result;
        }

        public IEnumerable<NH_QT_ThongTriQuyetToan_ChiTietModel> GetChiTietThongTriQuyetToan(Guid? iID_DonViID, Guid? iID_KHCTBQP_ChuongTrinhID, int? iNamThucHien)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamThucHien", iNamThucHien);
                lstParam.Add("iID_DonViID", iID_DonViID);
                lstParam.Add("iID_KHCTBQP_ChuongTrinhID", iID_KHCTBQP_ChuongTrinhID);

                var items = conn.Query<NH_QT_ThongTriQuyetToan_ChiTietModel>("sp_get_create_ThongTriQuyetToan_ChiTiet", lstParam, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<LookupDto<Guid, string>> GetLookupBQPNhiemVuChiByDonViId(Guid? iID_DonViID)
        {
            var result = new List<LookupDto<Guid, string>>();
            using (var connection = _connectionFactory.GetConnection())
            {
                if (iID_DonViID.HasValue && iID_DonViID != Guid.Empty)
                {
                    var query = @"SELECT DISTINCT ID AS Id, sTenNhiemVuChi AS DisplayName FROM NH_KHChiTietBQP_NhiemVuChi WHERE iID_DonViID = @idDonVi";
                    var items = connection.Query<LookupDto<Guid, string>>(query, new { idDonVi = iID_DonViID }, commandType: CommandType.Text);
                    result = items.ToList();
                    result.Insert(0, new LookupDto<Guid, string> { Id = Guid.Empty, DisplayName = "-- Chọn chương trình --" });
                }
                else
                {
                    result.Add(new LookupDto<Guid, string> { Id = Guid.Empty, DisplayName = "-- Chọn chương trình --" });
                }
            }

            return result;
        }

        public bool SaveThongTriQuyetToan(NH_QT_ThongTriQuyetToanCreateDto input, string state)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    if (state == "CREATE")
                    {
                        // Insert thông tri quyết toán
                        var tt = new NH_QT_ThongTriQuyetToan();
                        tt.sSoThongTri = input.ThongTriQuyetToan.sSoThongTri;
                        tt.dNgayLap = input.ThongTriQuyetToan.dNgayLap;
                        tt.iID_KHTT_NhiemVuChiID = input.ThongTriQuyetToan.iID_KHTT_NhiemVuChiID;
                        tt.iID_DonViID = input.ThongTriQuyetToan.iID_DonViID;
                        tt.iID_MaDonVi = input.ThongTriQuyetToan.iID_MaDonVi;
                        tt.iNamThongTri = input.ThongTriQuyetToan.iNamThongTri;
                        tt.iLoaiThongTri = input.ThongTriQuyetToan.iLoaiThongTri;
                        tt.iLoaiNoiDungChi = input.ThongTriQuyetToan.iLoaiNoiDungChi;
                        tt.fThongTri_USD = double.TryParse(input.ThongTriQuyetToan.sThongTri_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fttUSD) ? fttUSD : 0;
                        tt.fThongTri_VND = double.TryParse(input.ThongTriQuyetToan.sThongTri_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fttVND) ? fttVND : 0;
                        conn.Insert(tt, trans);

                        // Insert thông tri quyết toán chi tiết
                        foreach (var item in input.ThongTriQuyetToan_ChiTiet)
                        {
                            var ttct = new NH_QT_ThongTriQuyetToan_ChiTiet();
                            ttct.iID_ThongTriQuyetToanID = tt.ID;
                            ttct.iID_DuAnID = item.iID_DuAnID;
                            ttct.iID_HopDongID = item.iID_HopDongID;
                            ttct.iID_ThanhToan_ChiTietID = item.iID_ThanhToan_ChiTietID;
                            ttct.fDeNghiQuyetToanNam_USD = double.TryParse(item.sDeNghiQuyetToanNam_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fDNQT_USD) ? fDNQT_USD : 0;
                            ttct.fDeNghiQuyetToanNam_VND = double.TryParse(item.sDeNghiQuyetToanNam_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fDNQT_VND) ? fDNQT_VND : 0;
                            ttct.fThuaNopTraNSNN_USD = double.TryParse(item.sThuaNopTraNSNN_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fTNTNSNN_USD) ? fTNTNSNN_USD : 0;
                            ttct.fThuaNopTraNSNN_VND = double.TryParse(item.sThuaNopTraNSNN_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fTNTNSNN_VND) ? fTNTNSNN_VND : 0;
                            ttct.sMaThuTu = item.sMaThuTu;
                            ttct.sTenNoiDungChi = item.sTenNoiDungChi;
                            conn.Insert(ttct, trans);
                        }
                    }

                    if (state == "UPDATE")
                    {
                        // Get thông tin thông tri cũ
                        var queryTT = @"SELECT * FROM NH_QT_ThongTriQuyetToan WHERE ID = @Id";
                        var ttOld = conn.QueryFirstOrDefault<NH_QT_ThongTriQuyetToan>(queryTT, new { Id = input.ThongTriQuyetToan.ID }, trans, commandType: CommandType.Text);

                        // Update
                        ttOld.sSoThongTri = input.ThongTriQuyetToan.sSoThongTri;
                        ttOld.dNgayLap = input.ThongTriQuyetToan.dNgayLap;
                        ttOld.iID_KHTT_NhiemVuChiID = input.ThongTriQuyetToan.iID_KHTT_NhiemVuChiID;
                        ttOld.iID_DonViID = input.ThongTriQuyetToan.iID_DonViID;
                        ttOld.iID_MaDonVi = input.ThongTriQuyetToan.iID_MaDonVi;
                        ttOld.iNamThongTri = input.ThongTriQuyetToan.iNamThongTri;
                        ttOld.iLoaiThongTri = input.ThongTriQuyetToan.iLoaiThongTri;
                        ttOld.iLoaiNoiDungChi = input.ThongTriQuyetToan.iLoaiNoiDungChi;
                        ttOld.fThongTri_USD = double.TryParse(input.ThongTriQuyetToan.sThongTri_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fttUSD) ? fttUSD : 0;
                        ttOld.fThongTri_VND = double.TryParse(input.ThongTriQuyetToan.sThongTri_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fttVND) ? fttVND : 0;
                        conn.Update(ttOld, trans);

                        // Xóa toàn bộ chi tiết rồi insert lại
                        var queryDelete = @"DELETE NH_QT_ThongTriQuyetToan_ChiTiet WHERE iID_ThongTriQuyetToanID = @Id";
                        conn.Execute(queryDelete, new { Id = input.ThongTriQuyetToan.ID }, trans, commandType: CommandType.Text);

                        // Insert lại chi tiết
                        foreach (var item in input.ThongTriQuyetToan_ChiTiet)
                        {
                            var ttct = new NH_QT_ThongTriQuyetToan_ChiTiet();
                            ttct.iID_ThongTriQuyetToanID = ttOld.ID;
                            ttct.iID_DuAnID = item.iID_DuAnID;
                            ttct.iID_HopDongID = item.iID_HopDongID;
                            ttct.iID_ThanhToan_ChiTietID = item.iID_ThanhToan_ChiTietID;
                            ttct.fDeNghiQuyetToanNam_USD = double.TryParse(item.sDeNghiQuyetToanNam_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fDNQT_USD) ? fDNQT_USD : 0;
                            ttct.fDeNghiQuyetToanNam_VND = double.TryParse(item.sDeNghiQuyetToanNam_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fDNQT_VND) ? fDNQT_VND : 0;
                            ttct.fThuaNopTraNSNN_USD = double.TryParse(item.sThuaNopTraNSNN_USD, NumberStyles.Float, new CultureInfo("en-US"), out double fTNTNSNN_USD) ? fTNTNSNN_USD : 0;
                            ttct.fThuaNopTraNSNN_VND = double.TryParse(item.sThuaNopTraNSNN_VND, NumberStyles.Float, new CultureInfo("en-US"), out double fTNTNSNN_VND) ? fTNTNSNN_VND : 0;
                            ttct.sMaThuTu = item.sMaThuTu;
                            ttct.sTenNoiDungChi = item.sTenNoiDungChi;
                            conn.Insert(ttct, trans);
                        }
                    }

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public bool DeleteThongTriQuyetToan(Guid id)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    // Xóa thông tri quyết toán chi tiết
                    var deleteTTCT = @"DELETE FROM NH_QT_ThongTriQuyetToan_ChiTiet WHERE iID_ThongTriQuyetToanID = @Id";
                    conn.Execute(deleteTTCT, new { Id = id }, trans);

                    // Xóa thông tri quyết toán
                    var ttDelete = new NH_QT_ThongTriQuyetToan
                    {
                        ID = id
                    };
                    conn.Delete(ttDelete, trans);

                    trans.Commit();
                    trans.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        public NH_QT_ThongTriQuyetToanModel GetThongTinQuyetToanById(Guid id)
        {
            var result = new NH_QT_ThongTriQuyetToanModel();
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"
                    SELECT TTQT.*, BQP_NVC.sTenNhiemVuChi, CONCAT(DV.iID_MaDonVi, ' - ', DV.sTen) AS sTenDonVi
                    FROM NH_QT_ThongTriQuyetToan TTQT
                    LEFT JOIN NH_KHChiTietBQP_NhiemVuChi BQP_NVC ON TTQT.iID_KHTT_NhiemVuChiID = BQP_NVC.ID
                    LEFT JOIN NS_DonVi DV ON TTQT.iID_DonViID = DV.iID_Ma AND TTQT.iID_MaDonVi = DV.iID_MaDonVi
                    WHERE TTQT.ID = @Id";

                result = connection.QueryFirstOrDefault<NH_QT_ThongTriQuyetToanModel>(query, new { Id = id }, commandType: CommandType.Text);
            }

            return result;
        }

        public IEnumerable<NH_QT_ThongTriQuyetToan_ChiTietModel> GetListThongTriQuyetToanChiTietByTTQTId(Guid id)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var query = @"
                    SELECT TTQT_CT.*, MLNS.sM, MLNS.sTM, MLNS.sTTM, MLNS.sNG
                    FROM NH_QT_ThongTriQuyetToan_ChiTiet TTQT_CT
                    LEFT JOIN NH_TT_ThanhToan_ChiTiet TTCT ON TTQT_CT.iID_ThanhToan_ChiTietID = TTCT.ID
                    LEFT JOIN NS_MucLucNganSach MLNS ON TTCT.iID_MucLucNganSachID = MLNS.iID_MaMucLucNganSach
                    WHERE iID_ThongTriQuyetToanID = @Id";

                return connection.Query<NH_QT_ThongTriQuyetToan_ChiTietModel>(query, new { Id = id }, commandType: CommandType.Text);
            }
        }
        #endregion

        #region QLNH - Chuyển dữ liệu quyết toán

        public DataTable Get_dtMucLucNganSach(int Trang = 1, int SoBanGhi = 0, String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "")
        {
            String SQL = "SELECT * FROM NS_MucLucNganSach {0} ";
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(sL) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sL=@sL";
                cmd.Parameters.AddWithValue("@sL", sL);
            }
            if (String.IsNullOrEmpty(sK) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sK=@sK";
                cmd.Parameters.AddWithValue("@sK", sK);
            }
            if (String.IsNullOrEmpty(sM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (String.IsNullOrEmpty(sTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (String.IsNullOrEmpty(sTTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }
            if (String.IsNullOrEmpty(sTNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTNG=@sTNG";
                cmd.Parameters.AddWithValue("@sTNG", sTNG);
            }

            #endregion Điều kiện

            if (String.IsNullOrEmpty(DK) == false) DK = " WHERE " + DK;
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            DataTable vR = CommonFunction.dtData(cmd, "iSTT", Trang, SoBanGhi);
            cmd.Dispose();
            return vR;
        }
        public int Get_MucLucNganSach_Count(String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "")
        {
            String SQL = "SELECT COUNT(*) FROM NS_MucLucNganSach {0} ";
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(sL) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sL=@sL";
                cmd.Parameters.AddWithValue("@sL", sL);
            }
            if (String.IsNullOrEmpty(sK) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sK=@sK";
                cmd.Parameters.AddWithValue("@sK", sK);
            }
            if (String.IsNullOrEmpty(sM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (String.IsNullOrEmpty(sTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (String.IsNullOrEmpty(sTTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }
            if (String.IsNullOrEmpty(sTNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTNG=@sTNG";
                cmd.Parameters.AddWithValue("@sTNG", sTNG);
            }

            #endregion Điều kiện

            if (String.IsNullOrEmpty(DK) == false) DK = " WHERE " + DK;
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            int vR = Convert.ToInt32(Connection.GetValue(cmd, 0));
            cmd.Dispose();

            return vR;
        }

        public bool Insert_FirstTimeData(int namKeHoach, Guid? idChuyenQuyetToan)
        {
            String SQL = @"insert into NH_QT_ChuyenQuyetToan_ChiTiet ("
    + "iID_ChuyenQuyetToanChiTietID , iID_ChuyenQuyetToanID, iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sLNS"
    + ", sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,sChuong,iNamLamViec,bsMaCongTrinh,bsTenCongTrinh,brKhauHaoTSCD,brNgay"
    + ", brTienThu,brNgay_DonVi,brSoNguoi_DonVi,brChiTaiKhoBac_DonVi,brTonKho_DonVi,brTuChi_DonVi,brChiTapTrung_DonVi,brHangNhap_DonVi,brHangMua_DonVi,brHienVat_DonVi"
    + ", brDuPhong_DonVi,brPhanCap_DonVi,brTienThu_DonVi,brTongThu,brTienLuong,brQTNSKhac,brChiPhiKhac,brChenhLech,brNopNSQP,brNopNSNN"
    + ", brNopCapTren,brBoSungKinhPhi,brTrichQuyDonVi,brSoChuaPhanPhoi,brDuToanDuocDuyet,brThucHien,brSoXacNhan,bsGhiChu,sNhapTheoTruong,iID_MaTaiKhoan_No"
    + ", iID_MaTaiKhoan_Co,iSTT,iTrangThai,bPublic,iID_MaNhomNguoiDung_Public,iID_MaNhomNguoiDung_DuocGiao,sID_MaNguoiDung_DuocGiao"
    + ", dNgayTao,sID_MaNguoiDungTao,iSoLanSua,dNgaySua,sIPSua,sID_MaNguoiDungSua,bKhongHienThi,fGiaTriUSD ) "
    + "select NEWID() as iID_ChuyenQuyetToanChiTietID ,'" + idChuyenQuyetToan.ToString() + "' as iID_ChuyenQuyetToanID, b.iID_MaMucLucNganSach,b.iID_MaMucLucNganSach_Cha,b.bLaHangCha,b.sXauNoiMa,b.sLNS"
    + ",b.sL,b.sK,b.sM,b.sTM,b.sTTM,b.sNG,b.sTNG,b.sMoTa,b.sChuong,b.iNamLamViec,b.bsMaCongTrinh,b.bsTenCongTrinh,b.brKhauHaoTSCD,b.brNgay"
    + ", b.brTienThu,b.brNgay_DonVi,b.brSoNguoi_DonVi,b.brChiTaiKhoBac_DonVi,b.brTonKho_DonVi,b.brTuChi_DonVi,b.brChiTapTrung_DonVi,b.brHangNhap_DonVi,b.brHangMua_DonVi,b.brHienVat_DonVi"
    + ", b.brDuPhong_DonVi,b.brPhanCap_DonVi,b.brTienThu_DonVi,b.brTongThu,b.brTienLuong,b.brQTNSKhac,b.brChiPhiKhac,b.brChenhLech,b.brNopNSQP,b.brNopNSNN"
    + ", b.brNopCapTren,b.brBoSungKinhPhi,b.brTrichQuyDonVi,b.brSoChuaPhanPhoi,b.brDuToanDuocDuyet,b.brThucHien,b.brSoXacNhan,b.bsGhiChu,b.sNhapTheoTruong,b.iID_MaTaiKhoan_No"
    + ", b.iID_MaTaiKhoan_Co,b.iSTT,b.iTrangThai,b.bPublic,b.iID_MaNhomNguoiDung_Public,b.iID_MaNhomNguoiDung_DuocGiao,b.sID_MaNguoiDung_DuocGiao"
    + ", b.dNgayTao,b.sID_MaNguoiDungTao,b.iSoLanSua,b.dNgaySua,b.sIPSua,b.sID_MaNguoiDungSua,b.bKhongHienThi,null "
    + "from NS_MucLucNganSach b WHERE b.iNamLamViec=" + namKeHoach + " AND b.iTrangThai=1 AND (1 = 1)AND b.sLNS <> '' AND b.sLNS like '3%'  ORDER BY b.sXauNoiMa";
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(
                    SQL,

                    commandType: CommandType.Text);
                return r > 0;

            }
        }


        public IEnumerable<NH_QT_ChuyenQuyetToanData> GetListChuyenQuyetToan(ref PagingInfo _paging, string sSoChungTu, DateTime? dNgayChungTu, Guid? iDonVi, int? iLoaiThoiGian, int? iThoiGian)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("sSoChungTu", sSoChungTu);
                lstParam.Add("dNgayChungTu", dNgayChungTu);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iLoaiThoiGian", iLoaiThoiGian);
                lstParam.Add("iThoiGian", iThoiGian);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_QT_ChuyenQuyetToanData>("proc_get_all_chuyenquyettoan_paging", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public NH_QT_ChuyenQuyetToanReturnData SaveChuyenQuyetToan(NH_QT_ChuyenQuyetToan data, string userName, int namLamViec)
        {
            NH_QT_ChuyenQuyetToanReturnData dt = new NH_QT_ChuyenQuyetToanReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NH_QT_ChuyenQuyetToan> item = new List<NH_QT_ChuyenQuyetToan>();
                    if (data.iID_DonViID != null)
                    {
                        StringBuilder query = new StringBuilder();
                        query.Append("select * from NH_QT_ChuyenQuyetToan where iLoaiThoiGian = " + data.iLoaiThoiGian + " and iThoiGian = " + data.iThoiGian + "  and iID_DonViID ='" + data.iID_DonViID + "' and ID <> '" + data.ID + "'");
                        item = conn.Query<NH_QT_ChuyenQuyetToan>(query.ToString(), commandType: CommandType.Text).ToList();
                    }

                    var trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_QT_ChuyenQuyetToan();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = userName;


                        if (item.Any())
                        {
                            dt.IsReturn = false;
                            dt.errorMess = "Đã tồn chuyển quyết toán với khoảng thời gian đã chọn!";
                            return dt;
                        }
                        conn.Insert(entity, trans);
                        dt.ChuyenQuyetToanData = entity;
                        var saveChiTiet = Insert_FirstTimeData(namLamViec, entity.ID);
                    }
                    else
                    {
                        var entity = conn.Get<NH_QT_ChuyenQuyetToan>(data.ID, trans);
                        if (entity == null)
                        {
                            dt.IsReturn = false;
                            return dt;
                        }

                        entity.iID_DonViID = data.iID_DonViID;
                        entity.iID_MaDonVi = data.iID_MaDonVi;
                        entity.iThoiGian = data.iThoiGian;
                        entity.iLoaiThoiGian = data.iLoaiThoiGian;
                        entity.sSoChungTu = data.sSoChungTu;
                        entity.sMoTa = data.sMoTa;
                        entity.dNgayChungTu = data.dNgayChungTu;
                        entity.dNgaySua = DateTime.Now;
                        entity.sNguoiSua = userName;
                        conn.Update(entity, trans);
                        dt.ChuyenQuyetToanData = entity;

                    }

                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }


        public NH_QT_ChuyenQuyetToanData GetThongTinChuyenQuyetToanById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_QT_ChuyenQuyetToan WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_QT_ChuyenQuyetToanData>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }


        public bool DeleteChuyenQuyetToan(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DELETE NH_QT_ChuyenQuyetToan WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                return r > 0;
            }
        }


        #endregion

        #region QLNH - Báo cáo quyết toán thuộc các nguồn chi đặc biệt
        public IEnumerable<NH_QTND_NguonChiDacBietReport> GetBaoCaoQuyetToanNguonChiDacBiet(int? iNamKeHoach)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();

                lstParam.Add("iNamKeHoach", iNamKeHoach);

                var items = conn.Query<NH_QTND_NguonChiDacBietReport>("proc_get_all_nh_baocao_quyettoan_nguonchidacbiet", lstParam,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region QLNH -  Khởi tạo cấp phát
        public IEnumerable<NH_KT_KhoiTaoCapPhatData> GetListKhoiTaoCapPhat(ref PagingInfo _paging, DateTime? dNgayKhoiTao, Guid? iDonVi, int? iNamKhoiTao)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstParam = new DynamicParameters();
                lstParam.Add("dNgayKhoiTao", dNgayKhoiTao);
                lstParam.Add("iDonVi", iDonVi);
                lstParam.Add("iNamKhoiTao", iNamKhoiTao);
                lstParam.Add("CurrentPage", _paging.CurrentPage);
                lstParam.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstParam.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<NH_KT_KhoiTaoCapPhatData>("proc_get_all_khoitaocapphat_paging", lstParam,
                    commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstParam.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<NH_KT_KhoiTaoCapPhat_ChiTietData> GetListKhoiTaoCapPhatChiTiet(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ktcpct.*,da.sMaDuAn + ' - ' + da.sTenDuAn as sTenDuAn , hd.sSoHopDong + ' - ' + hd.sTenHopDong as sTenHopDong " +
                "FROM NH_KT_KhoiTaoCapPhat_ChiTiet ktcpct" +
                " left join NH_DA_DuAn da on ktcpct.iID_DuAnID = da.ID" +
                " left join NH_DA_HopDong hd on ktcpct.iID_HopDongID = hd.ID WHERE iID_KhoiTaoCapPhatID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.Query<NH_KT_KhoiTaoCapPhat_ChiTietData>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public NH_KT_KhoiTaoCapPhatData GetThongTinKhoiTaoCapPhatById(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM NH_KT_KhoiTaoCapPhat WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NH_KT_KhoiTaoCapPhatData>(query.ToString(),
                    param: new { iId = iId }, commandType: CommandType.Text);
                return item;
            }
        }

        public NH_KT_KhoiTaoCapPhatReturnData SaveKhoiTaoCapPhat(NH_KT_KhoiTaoCapPhat data, string userName)
        {
            NH_KT_KhoiTaoCapPhatReturnData dt = new NH_KT_KhoiTaoCapPhatReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    List<NH_KT_KhoiTaoCapPhat> item = new List<NH_KT_KhoiTaoCapPhat>();
                    if (data.iID_DonViID != null)
                    {
                        StringBuilder query = new StringBuilder();
                        query.Append("select * from NH_KT_KhoiTaoCapPhat where iNamKhoiTao = " + data.iNamKhoiTao + " and iID_DonViID ='" + data.iID_DonViID + "' and ID <> '" + data.ID + "'");
                        item = conn.Query<NH_KT_KhoiTaoCapPhat>(query.ToString(), commandType: CommandType.Text).ToList();
                    }

                    var trans = conn.BeginTransaction();
                    if (data.ID == null || data.ID == Guid.Empty)
                    {
                        var entity = new NH_KT_KhoiTaoCapPhat();
                        entity.MapFrom(data);
                        entity.dNgayTao = DateTime.Now;
                        entity.sNguoiTao = userName;
                        entity.bIsKhoa = false;
                        entity.bIsXoa = false;


                        if (item.Any())
                        {
                            dt.IsReturn = false;
                            dt.errorMess = "Đã tồn khởi tạo cấp phát của đơn vị trong năm!";
                            return dt;
                        }
                        conn.Insert(entity, trans);
                        dt.KhoiTaoCapPhatData = entity;
                    }
                    else
                    {
                        var entity = conn.Get<NH_KT_KhoiTaoCapPhat>(data.ID, trans);
                        if (entity == null)
                        {
                            dt.IsReturn = false;
                            return dt;
                        }

                        entity.iID_DonViID = data.iID_DonViID;
                        entity.iID_MaDonVi = data.iID_MaDonVi;
                        entity.iID_TiGiaID = data.iID_TiGiaID;
                        entity.iNamKhoiTao = data.iNamKhoiTao;
                        entity.sMoTa = data.sMoTa;
                        entity.dNgayKhoiTao = data.dNgayKhoiTao;
                        entity.dNgaySua = DateTime.Now;
                        entity.sNguoiSua = userName;
                        conn.Update(entity, trans);
                        dt.KhoiTaoCapPhatData = entity;

                    }

                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }

        public bool DeleteKhoiTaoCapPhat(Guid iId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DELETE NH_KT_KhoiTaoCapPhat WHERE ID = @iId");
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<NH_KT_KhoiTaoCapPhat>(iId);
                if (entity.sTongHop != null)
                {
                    foreach (var id in entity.sTongHop.Split(','))
                    {
                        var entityCon = conn.Get<NH_KT_KhoiTaoCapPhat>(id);
                        if (entityCon == null) return false;
                        entityCon.iID_TongHopID = null;
                        conn.Update(entityCon);
                    }
                }

                var r = conn.Execute(query.ToString(), param: new { iId = iId }, commandType: CommandType.Text);
                DeleteNHTongHop_Giam(conn, "KHOI_TAO", iId);
                return r > 0;
            }
        }

        public NH_KT_KhoiTaoCapPhat_ChiTietReturnData SaveKhoiTaoCapPhatDetail(List<NH_KT_KhoiTaoCapPhat_ChiTiet> listData, List<NH_KT_KhoiTaoCapPhat_ChiTiet> listDataDelete, string userName)
        {
            NH_KT_KhoiTaoCapPhat_ChiTietReturnData dt = new NH_KT_KhoiTaoCapPhat_ChiTietReturnData();
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    foreach (var data in listData)
                    {
                        if (data.ID == null || data.ID == Guid.Empty)
                        {
                            var entity = new NH_KT_KhoiTaoCapPhat_ChiTiet();
                            entity.MapFrom(data);

                            if (entity.iID_HopDongID != null)
                            {
                                var hopDong = conn.Get<NH_DA_HopDong>(entity.iID_HopDongID, trans);
                                if (hopDong.iID_DuAnID != null)
                                {
                                    entity.iID_DuAnID = hopDong.iID_DuAnID;
                                }
                            }

                            conn.Insert(entity, trans);
                            dt.KhoiTaoCapPhatChiTietData = entity;
                            InsertNHTongHop_Tang(conn, trans, "KHOI_TAO", 1, (Guid)entity.iID_KhoiTaoCapPhatID, null);
                            InsertNHTongHop_Giam(conn, trans, "KHOI_TAO", 1, (Guid)entity.iID_KhoiTaoCapPhatID, null);
                        }
                        else
                        {
                            var entity = conn.Get<NH_KT_KhoiTaoCapPhat_ChiTiet>(data.ID, trans);
                            if (entity == null)
                            {
                                dt.IsReturn = false;
                                return dt;
                            }

                            if (entity.iID_DuAnID != data.iID_DuAnID
                                || entity.iID_HopDongID != data.iID_HopDongID
                                || entity.fLuyKeKinhPhiDuocCap_USD != data.fLuyKeKinhPhiDuocCap_USD
                                || entity.fLuyKeKinhPhiDuocCap_VND != data.fLuyKeKinhPhiDuocCap_VND
                                || entity.fQTKinhPhiDuyetCacNamTruoc_USD != data.fQTKinhPhiDuyetCacNamTruoc_USD
                                || entity.fQTKinhPhiDuyetCacNamTruoc_VND != data.fQTKinhPhiDuyetCacNamTruoc_VND
                                || entity.fDeNghiQTNamNay_USD != data.fDeNghiQTNamNay_USD
                                || entity.fDeNghiQTNamNay_VND != data.fDeNghiQTNamNay_VND)
                            {
                                InsertNHTongHop_Tang(conn, trans, "KHOI_TAO", 2, (Guid)entity.iID_KhoiTaoCapPhatID, null);
                                InsertNHTongHop_Giam(conn, trans, "KHOI_TAO", 2, (Guid)entity.iID_KhoiTaoCapPhatID, null);
                            }

                            entity.MapFrom(data);

                            if (entity.iID_HopDongID != null)
                            {
                                var hopDong = conn.Get<NH_DA_HopDong>(entity.iID_HopDongID, trans);
                                if (hopDong.iID_DuAnID != null)
                                {
                                    entity.iID_DuAnID = hopDong.iID_DuAnID;
                                }
                            }

                            conn.Update(entity, trans);
                            dt.KhoiTaoCapPhatChiTietData = entity;

                        }
                    }

                    if (listDataDelete != null && listDataDelete.Any())
                    {
                        foreach (var data in listDataDelete)
                        {
                            conn.Delete(data, trans);
                        }
                    }
                    dt.IsReturn = true;
                    trans.Commit();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            dt.IsReturn = false;
            return dt;
        }

        #endregion

        #region QLNH - Báo cáo kết luận quyết toán
        public IEnumerable<NH_QT_QuyetToanDAHT_ChiTietData> getListBaoCaoKetLuanQuyetToanModels(DateTime? dTuNgay, DateTime? dDenNgay, Guid? iDonvi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("dTuNgay", dTuNgay);
                lstPrams.Add("dDenNgay", dDenNgay);
                lstPrams.Add("iDonvi", iDonvi);

                var items = conn.Query<NH_QT_QuyetToanDAHT_ChiTietData>("proc_get_all_nh_baocao_ketluanbaocao", lstPrams,
                    commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        #endregion

        #region Tổng hợp
        public void InsertNHTongHop_Tang(SqlConnection conn, SqlTransaction trans, string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld)
        {
            try
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@sLoai", sLoai);
                lstParam.Add("@iTypeExecute", iTypeExecute);
                lstParam.Add("@uIdQuyetDinh", iIdQuyetDinh);
                lstParam.Add("@iIDQuyetDinhOld", iIDQuyetDinhOld);

                conn.Execute("sp_insert_nh_tonghop_tang", lstParam, trans, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void InsertNHTongHop_Giam(SqlConnection conn, SqlTransaction trans, string sLoai, int iTypeExecute, Guid iIdQuyetDinh, Guid? iIDQuyetDinhOld)
        {
            try
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@sLoai", sLoai);
                lstParam.Add("@iTypeExecute", iTypeExecute);
                lstParam.Add("@uIdQuyetDinh", iIdQuyetDinh);
                lstParam.Add("@iIDQuyetDinhOld", iIDQuyetDinhOld);

                conn.Execute("sp_insert_nh_tonghop_giam", lstParam, trans, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void DeleteNHTongHop_Giam(SqlConnection conn, string sLoai, Guid iIdQuyetDinh)
        {
            try
            {
                var lstParam = new DynamicParameters();
                lstParam.Add("@sLoai", sLoai);
                lstParam.Add("@uIdQuyetDinh", iIdQuyetDinh);

                conn.Execute("sp_delete_nh_tonghop_giam", lstParam, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void InsertNHTongHop(Guid iIDChungTu, string sLoai, List<NHTHTongHopQuery> lstData)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    var data = ConvertDataToTableDefined("t_tbl_nh_tonghop", lstData);

                    using (var cmd1 = new SqlCommand("sp_insert_nhtonghop", conn))
                    {
                        cmd1.Parameters.AddWithValue("@iIDChungTu", iIDChungTu);
                        cmd1.Parameters.AddWithValue("@sLoai", sLoai);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@data", data);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.t_tbl_nh_tonghop";

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
        #endregion
    }

}
