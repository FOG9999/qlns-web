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
using Viettel.Models.CPNH;

namespace Viettel.Services
{
    public interface ICPNHService : IServiceBase
    {
        IEnumerable<CPNHNhuCauChiQuy_Model> getListNhuCauChiQuyModels(ref PagingInfo _paging,
            string sSoDeNghi = "", DateTime? dNgayDeNghi = null, Guid? iID_BQuanLyID = null,
            Guid? iID_DonViID = null, int? iQuy = 0, int? iNamKeHoach = 0, int? tabIndex = 0);
        IEnumerable<CPNHThucHienNganSach_Model> getListThucHienNganSachModels(int tabTable, int iTuNam, int iDenNam, Guid? iDonvi, int iQuyList, int iNam, int slbDonViUSD, int slbDonViVND);
        IEnumerable<CPNHThucHienNganSach_Model> getListKinhPhiDuocCapTheoDonViModels(int? iQuy, int? iNam, Guid? iDonvi);
        IEnumerable<CPNHNhuCauChiQuy_Model> getListNhuCauChiQuyModelsBaoCao(ref PagingInfo _paging,
            string sSoDeNghi = "", DateTime? dNgayDeNghi = null, Guid? iID_BQuanLyID = null,
            Guid? iID_DonViID = null, int? iQuy = 0, int? iNamKeHoach = 0, int? tabIndex = 0);
        CPNHNhuCauChiQuy_Model GetNhuCauChiQuyId(Guid iId);
        CPNHNhuCauChiQuy_View_Model GetNhuCauChiQuyChitiet(Guid iId);
        
        IEnumerable<NS_DonVi> GetDonviListByYear(int namLamViec = 0);
        IEnumerable<NH_DM_TiGia> GetNHDMTiGiaList(Guid? id = null);
        IEnumerable<NH_DM_TiGia_ChiTiet> GetNHDMTiGiaChiTietList(Guid? iDTiGia, bool isMaNgoaiTeKhac = true);
        IEnumerable<CPNHNhuCauChiQuy_Model> GetListNhuCauChiQuy(string sSodenghi = "", DateTime? dNgaydenghi = null, Guid? iBQuanly = null, Guid? iDonvi = null, int? iQuy = 0, string iNam = "");
        IEnumerable<NH_DM_TiGia> GetDonviListTyGia();
        IEnumerable<NH_DM_TiGia_ChiTiet> GetDonviListMangoaitekhac(Guid? iDTiGia, bool isMaNgoaiTeKhac = true);
        IEnumerable<NS_PhongBan> GetBQuanlyList();
        NH_NhuCauChiQuy NhuCauChiQuySave(NH_NhuCauChiQuy data, string dieuchinh ,string sUsername);
        NH_NhuCauChiQuy NhuCauChiQuyTongHopSave(NH_NhuCauChiQuy data, List<CPNHNhuCauChiQuy_Model> lstItem, string sUsername);
        Boolean DeleteNhuCauChiQuy(Guid iId);
        NS_PhongBan GetPhongBanID(Guid? ID);
        IEnumerable<NH_DA_HopDong> GetListHopDong(Guid? iID_DonViID, Guid? iID_BQuanLyID);
        IEnumerable<CPNHNhuCauChiQuy_ChiTiet_Model> GetListNhucauchiquyChitiet(Guid? iDonvi , int iQuy , int iNam);
        IEnumerable<CPNHNhuCauChiQuy_ChiTiet_Model> GetListNhucauchiquyChitietBaoCao2(int iUSD , int iVND, Guid? iDonvi, int iQuy, int iNam);
        bool LockOrUnLockNhuCauChiQuy(Guid id, bool isLockOrUnLock);
        string GetSTTLAMA(int ID);
        IEnumerable<ThucHienNganSach_GiaiDoan_Model> GetListGiaiDoan(int toY, int fromY);
    }
    public class CPNHService: ICPNHService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILocalizationService _languageService;
        private readonly ICacheService _cacheService;
        private static ICPNHService _default;

        public CPNHService(
            IConnectionFactory connectionFactory = null,
            ILocalizationService languageService = null,
            ICacheService cacheService = null)
        {
            _connectionFactory = connectionFactory ?? new ConnectionFactory();
            _languageService = new LocalizationService();
            _cacheService = CacheService.Default;
        }

        public static ICPNHService Default
        {
            get { return _default ?? (_default = new CPNHService()); }
        }
        public IEnumerable<CPNHNhuCauChiQuy_Model> getListNhuCauChiQuyModels(ref PagingInfo _paging, 
            string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iID_BQuanLyID,
            Guid? iID_DonViID, int? iQuy, int? iNamKeHoach, int? tabIndex)
        {
            if(iID_BQuanLyID == Guid.Empty)
            {
                iID_BQuanLyID = null;
            }
            if (iID_DonViID == Guid.Empty)
            {
                iID_DonViID = null;
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sSoDeNghi", sSoDeNghi);
                lstPrams.Add("dNgayDeNghi", dNgayDeNghi);
                lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID);
                lstPrams.Add("iID_DonViID", iID_DonViID);
                lstPrams.Add("iQuy", iQuy); 
                lstPrams.Add("iNamKeHoach", iNamKeHoach); 
                lstPrams.Add("tabIndex", tabIndex);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", _paging.ItemsPerPage);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<CPNHNhuCauChiQuy_Model>("proc_get_all_cpnhNhucauchiquy_paging", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public IEnumerable<CPNHThucHienNganSach_Model> getListThucHienNganSachModels(int tabTable, int iTuNam, int iDenNam, Guid? iDonvi, int iQuyList, int iNam, int slbDonViUSD, int slbDonViVND)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("tabTable", tabTable);
                lstPrams.Add("iTuNam", iTuNam);
                lstPrams.Add("iDenNam", iDenNam);
                lstPrams.Add("iDonvi", iDonvi);
                lstPrams.Add("iQuyList", iQuyList);
                lstPrams.Add("iNam", iNam);
                lstPrams.Add("iTUSD", slbDonViUSD);
                lstPrams.Add("iTVND", slbDonViVND);

                var items = conn.Query<CPNHThucHienNganSach_Model>("proc_get_all_cpnh_Thuchienngansach", lstPrams, commandType: CommandType.StoredProcedure);
                return items;
            }
        }

        public IEnumerable<CPNHThucHienNganSach_Model> getListKinhPhiDuocCapTheoDonViModels(int? iQuy, int? iNam, Guid? iDonvi)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("iQuyList", iQuy);
                lstPrams.Add("iNam", iNam);
                lstPrams.Add("iDonvi", iDonvi);

                var items = conn.Query<CPNHThucHienNganSach_Model>("proc_get_all_cpnh_Kinhphiduoccaptheodinvi", lstPrams, commandType: CommandType.StoredProcedure);
                return items;
            }
        }


        

        public IEnumerable<CPNHNhuCauChiQuy_Model> getListNhuCauChiQuyModelsBaoCao(ref PagingInfo _paging,
            string sSoDeNghi, DateTime? dNgayDeNghi, Guid? iID_BQuanLyID,
            Guid? iID_DonViID, int? iQuy, int? iNamKeHoach, int? tabIndex)
        {
            if (iID_BQuanLyID == Guid.Empty)
            {
                iID_BQuanLyID = null;
            }
            if (iID_DonViID == Guid.Empty)
            {
                iID_DonViID = null;
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("sSoDeNghi", sSoDeNghi);
                lstPrams.Add("dNgayDeNghi", dNgayDeNghi);
                lstPrams.Add("iID_BQuanLyID", iID_BQuanLyID);
                lstPrams.Add("iID_DonViID", iID_DonViID);
                lstPrams.Add("iQuy", iQuy);
                lstPrams.Add("iNamKeHoach", iNamKeHoach);
                lstPrams.Add("tabIndex", tabIndex);
                lstPrams.Add("CurrentPage", _paging.CurrentPage);
                lstPrams.Add("ItemsPerPage", 99999);
                lstPrams.Add("iToTalItem", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var items = conn.Query<CPNHNhuCauChiQuy_Model>("proc_get_all_cpnhNhucauchiquy_paging", lstPrams, commandType: CommandType.StoredProcedure);
                _paging.TotalItems = lstPrams.Get<int>("iToTalItem");
                return items;
            }
        }

        public CPNHNhuCauChiQuy_Model GetNhuCauChiQuyId(Guid iId)
        {
            var sql = "select NH_NhuCauChiQuy.*, concat (NS_PhongBan.sTen, ' - ', NS_PhongBan.sMoTa) as BPhongBan, concat (NS_DonVi.sTen, ' - ', NS_DonVi.sMoTa) as BQuanLy, " +
                "NH_DM_TiGia.sTenTiGia as sTenTiGia, NH_DM_TiGia_ChiTiet.fTiGia as fTiGiaChiTiet " +
                "from NH_NhuCauChiQuy " +
                "left join NS_PhongBan  on NH_NhuCauChiQuy.iID_BQuanLyID = NS_PhongBan.iID_MaPhongBan " +
                "left join NH_DM_TiGia_ChiTiet  on NH_NhuCauChiQuy.iID_TiGiaID = NH_DM_TiGia_ChiTiet.iID_TiGiaID and NH_NhuCauChiQuy.sMaNgoaiTeKhac COLLATE SQL_Latin1_General_CP1_CI_AS = NH_DM_TiGia_ChiTiet.sMaTienTeQuyDoi " +
                "left join NH_DM_TiGia on NH_NhuCauChiQuy.iID_TiGiaID = NH_DM_TiGia.ID " +
                "left join NS_DonVi on NH_NhuCauChiQuy.iID_DonViID = NS_DonVi.iID_Ma and NH_NhuCauChiQuy.iID_MaDonVi COLLATE SQL_Latin1_General_CP1_CI_AS = NS_DonVi.iID_MaDonVi " +
                " where NH_NhuCauChiQuy.ID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<CPNHNhuCauChiQuy_Model>(sql, param: new { iId }, commandType:CommandType.Text);
                return item;
            }
        }

        public CPNHNhuCauChiQuy_View_Model GetNhuCauChiQuyChitiet(Guid iId)
        {
            var sql = "select NH_NhuCauChiQuy.*, concat (NS_PhongBan.sTen, ' - ', NS_PhongBan.sMoTa) as BQuanLy, concat (NS_DonVi.sTen, ' - ', NS_DonVi.sMoTa) as BPhongBan, " +
                "NH_DM_TiGia.sTenTiGia as sTenTiGia, NH_DM_TiGia_ChiTiet.fTiGia as fTiGiaChiTiet " +
                "from NH_NhuCauChiQuy " +
                "left join NS_PhongBan  on NH_NhuCauChiQuy.iID_BQuanLyID = NS_PhongBan.iID_MaPhongBan " +
                "left join NH_DM_TiGia_ChiTiet  on NH_NhuCauChiQuy.iID_TiGiaID = NH_DM_TiGia_ChiTiet.iID_TiGiaID and NH_NhuCauChiQuy.sMaNgoaiTeKhac COLLATE SQL_Latin1_General_CP1_CI_AS = NH_DM_TiGia_ChiTiet.sMaTienTeQuyDoi " +
                "left join NH_DM_TiGia on NH_NhuCauChiQuy.iID_TiGiaID = NH_DM_TiGia.ID " +
                "left join NS_DonVi on NH_NhuCauChiQuy.iID_DonViID = NS_DonVi.iID_Ma and NH_NhuCauChiQuy.iID_MaDonVi COLLATE SQL_Latin1_General_CP1_CI_AS = NS_DonVi.iID_MaDonVi " +
                " where NH_NhuCauChiQuy.ID = @iId";

            var sqlChitiet = "select NH_NhuCauChiQuy_ChiTiet.* from NH_NhuCauChiQuy_ChiTiet " +
                "where NH_NhuCauChiQuy_ChiTiet.iID_NhuCauChiQuyID = @iId";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<CPNHNhuCauChiQuy_View_Model>(sql, param: new { iId }, commandType: CommandType.Text);
                var listchitiet = conn.Query<CPNHNhuCauChiQuy_ChiTiet_Model>(sqlChitiet, param: new { iId });

                if (listchitiet != null && listchitiet.Any())
                {
                    var listNV = new List<CPNHNhuCauChiQuy_ChiTiet_Model>();
                    foreach (var nv in listchitiet)
                    {
                        var chitiet = new CPNHNhuCauChiQuy_ChiTiet_Model();
                        chitiet.fChiNgoaiTeUSD = nv.fChiNgoaiTeUSD;
                        chitiet.fChiNgoaiTeVND = nv.fChiNgoaiTeVND;
                        chitiet.fChiTrongNuocUSD = nv.fChiTrongNuocUSD;
                        chitiet.fChiTrongNuocVND = nv.fChiTrongNuocVND;
                        chitiet.iID_HopDongID = nv.iID_HopDongID;
                        chitiet.sTenHopDong = nv.sTenHopDong;
                        chitiet.iID_NhuCauChiQuyID = nv.iID_NhuCauChiQuyID;
                        chitiet.sNoiDung = nv.sNoiDung;
                        chitiet.ID = nv.ID;
                        listNV.Add(chitiet);
                    }

                    if (listNV.Any())
                    {
                        item.ListNCCQChiTiet = listNV.AsEnumerable();
                    }
                }
                return item;

            }
           
        }

        public IEnumerable<CPNHNhuCauChiQuy_Model> GetListNhuCauChiQuy(string sSodenghi, DateTime? dNgaydenghi, Guid? iBQuanly, Guid? iDonvi, int? iQuy, string iNam)
        {
            if (iBQuanly == Guid.Empty)
            {
                iBQuanly = null;
            }
            if (iDonvi == Guid.Empty)
            {
                iDonvi = null;
            }
            var sSoDeNghi = sSodenghi;
            var dNgayDeNghi = dNgaydenghi;
            var iID_BQuanLyID = iBQuanly;
            var iID_DonViID = iDonvi;
            var iNamKeHoach = iNam;
            var sql =
                @"
                SELECT DISTINCT NCCQ.ID, NCCQ.sSoDeNghi, NCCQ.dNgayDeNghi , NCCQ.iID_BQuanLyID, NCCQ.iID_DonViID, NCCQ.iNamKeHoach, NCCQ.iQuy, NCCQ.iID_TiGiaID, 
                NCCQ.fTongChiNgoaiTeUSD, NCCQ.fTongChiNgoaiTeVND, NCCQ.fTongChiTrongNuocVND, NCCQ.fTongChiTrongNuocUSD, NCCQ.iID_ParentAdjustID, NCCQ.iID_GocID,
                NCCQ.iLanDieuChinh,NCCQ.bIsKhoa,NCCQ.iID_TongHopID,NCCQ.sTongHop,
                concat ('Quý ',NCCQ.iQuy, ' Năm ',NCCQ.iNamKeHoach) as sQuyNam ,
                concat (b.sTen, ' - ', b.sMoTa) as BPhongBan, concat (c.sTen, ' - ', c.sMoTa) as BQuanLy, TG.sTenTiGia as sTenTiGia
                INTO #tmp
                from NH_NhuCauChiQuy as NCCQ  
                left join NH_NhuCauChiQuy as cd on NCCQ.ID = cd.iID_TongHopID
                left join NH_NhuCauChiQuy as cha on cha.ID = NCCQ.iID_TongHopID
                inner join NS_PhongBan  b on NCCQ.iID_BQuanLyID = b.iID_MaPhongBan
                left join NH_DM_TiGia  TG on NCCQ.iID_TiGiaID = TG.ID
                left join NS_DonVi c on NCCQ.iID_DonViID = c.iID_Ma and NCCQ.iID_MaDonVi COLLATE SQL_Latin1_General_CP1_CI_AS = c.iID_MaDonVi
                where ((ISNULL(@sSoDeNghi,'') = '' or NCCQ.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%')) or (ISNULL(@sSoDeNghi,'') = '' or cha.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%')))
	            and ((@dNgayDeNghi is null or (NCCQ.dNgayDeNghi >= @dNgayDeNghi and NCCQ.dNgayDeNghi < DATEADD(day, 1, @dNgayDeNghi))) or (@dNgayDeNghi is null or (NCCQ.dNgayDeNghi >= @dNgayDeNghi and NCCQ.dNgayDeNghi < DATEADD(day, 1, @dNgayDeNghi))))
	            and ((@iID_BQuanLyID is null or NCCQ.iID_BQuanLyID = @iID_BQuanLyID) or (@iID_BQuanLyID is null or NCCQ.iID_BQuanLyID = @iID_BQuanLyID))
	            and ((@iID_DonViID is null or NCCQ.iID_DonViID = @iID_DonViID) or (@iID_DonViID is null or cha.iID_DonViID = @iID_DonViID) )
	            and ((ISNULL(@iQuy,'') = '' or @iQuy = 0 or NCCQ.iQuy = @iQuy) or (ISNULL(@iQuy,'') = '' or @iQuy = 0 or cha.iQuy = @iQuy) )
	            and ((ISNULL(@iNamKeHoach,'') = '' or NCCQ.iNamKeHoach = @iNamKeHoach) or (ISNULL(@iNamKeHoach,'') = '' or cha.iNamKeHoach = @iNamKeHoach))
                ;
                WITH cte(ID, sSoDeNghi, dNgayDeNghi, iID_BQuanLyID, iID_DonViID, iNamKeHoach, iQuy, iID_TiGiaID, fTongChiNgoaiTeUSD, fTongChiNgoaiTeVND,
                 fTongChiTrongNuocVND, fTongChiTrongNuocUSD, iID_ParentAdjustID, iID_GocID, iLanDieuChinh,bIsKhoa,iID_TongHopID,sTongHop, sQuyNam)
                AS
                (
	                SELECT lct.ID, lct.sSoDeNghi, lct.dNgayDeNghi , lct.iID_BQuanLyID, lct.iID_DonViID, lct.iNamKeHoach, lct.iQuy, lct.iID_TiGiaID, 
                lct.fTongChiNgoaiTeUSD, lct.fTongChiNgoaiTeVND, lct.fTongChiTrongNuocVND, lct.fTongChiTrongNuocUSD, lct.iID_ParentAdjustID, lct.iID_GocID,
                lct.iLanDieuChinh,lct.bIsKhoa,lct.iID_TongHopID,lct.sTongHop,concat ('Quý ',lct.iQuy, ' Năm ',lct.iNamKeHoach) as sQuyNam
	                FROM NH_NhuCauChiQuy lct , #tmp tmp
	                WHERE lct.ID  = tmp.iID_TongHopID
	                UNION ALL
	                SELECT cd.ID, cd.sSoDeNghi, cd.dNgayDeNghi , cd.iID_BQuanLyID, cd.iID_DonViID, cd.iNamKeHoach, cd.iQuy, cd.iID_TiGiaID, 
                cd.fTongChiNgoaiTeUSD, cd.fTongChiNgoaiTeVND, cd.fTongChiTrongNuocVND, cd.fTongChiTrongNuocUSD, cd.iID_ParentAdjustID, cd.iID_GocID,
                cd.iLanDieuChinh,cd.bIsKhoa,cd.iID_TongHopID,cd.sTongHop, concat ('Quý ',cd.iQuy, ' Năm ',cd.iNamKeHoach) as sQuyNam
	                FROM cte as NCCQ, #tmp as cd
	                WHERE cd.iID_TongHopID = NCCQ.ID
                )
                SELECT DISTINCT cte.ID, cte.sSoDeNghi, dNgayDeNghi, iID_BQuanLyID, iID_DonViID, iNamKeHoach, iQuy, iID_TiGiaID, fTongChiNgoaiTeUSD, fTongChiNgoaiTeVND,
                 fTongChiTrongNuocVND, fTongChiTrongNuocUSD, iID_ParentAdjustID, iID_GocID, iLanDieuChinh,bIsKhoa,iID_TongHopID,sTongHop,concat ('Quý ',cte.iQuy, ' Năm ',cte.iNamKeHoach) as sQuyNam  , 
                 concat (b.sTen, ' - ', b.sMoTa) as BPhongBan, concat (c.sTen, ' - ', c.sMoTa) as BQuanLy, TG.sTenTiGia as sTenTiGia INTO #db
                 FROM cte 
                  inner join NS_PhongBan  b on cte.iID_BQuanLyID = b.iID_MaPhongBan
                left join NH_DM_TiGia  TG on cte.iID_TiGiaID = TG.ID
                left join NS_DonVi c on cte.iID_DonViID = c.iID_Ma 
                UNION ALL
                SELECT DISTINCT NCCQ.ID, NCCQ.sSoDeNghi, NCCQ.dNgayDeNghi , NCCQ.iID_BQuanLyID, NCCQ.iID_DonViID, NCCQ.iNamKeHoach, NCCQ.iQuy, NCCQ.iID_TiGiaID, 
                NCCQ.fTongChiNgoaiTeUSD, NCCQ.fTongChiNgoaiTeVND, NCCQ.fTongChiTrongNuocVND, NCCQ.fTongChiTrongNuocUSD, NCCQ.iID_ParentAdjustID, NCCQ.iID_GocID,
                NCCQ.iLanDieuChinh,NCCQ.bIsKhoa,NCCQ.iID_TongHopID,NCCQ.sTongHop,
                concat ('Quý ',NCCQ.iQuy, ' Năm ',NCCQ.iNamKeHoach) as sQuyNam ,
                concat (b.sTen, ' - ', b.sMoTa) as BPhongBan, concat (c.sTen, ' - ', c.sMoTa) as BQuanLy, TG.sTenTiGia as sTenTiGia
                from NH_NhuCauChiQuy as NCCQ  
                inner join NH_NhuCauChiQuy as cd on NCCQ.ID = cd.iID_TongHopID
                inner join NS_PhongBan  b on NCCQ.iID_BQuanLyID = b.iID_MaPhongBan
                left join NH_DM_TiGia  TG on NCCQ.iID_TiGiaID = TG.ID
                left join NS_DonVi c on NCCQ.iID_DonViID = c.iID_Ma and NCCQ.iID_MaDonVi COLLATE SQL_Latin1_General_CP1_CI_AS = c.iID_MaDonVi
                where (ISNULL(@sSoDeNghi,'') = '' or NCCQ.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%'))
	            and (@dNgayDeNghi is null or (NCCQ.dNgayDeNghi >= @dNgayDeNghi and NCCQ.dNgayDeNghi < DATEADD(day, 1, @dNgayDeNghi)))
	            and (@iID_BQuanLyID is null or NCCQ.iID_BQuanLyID = @iID_BQuanLyID)
	            and (@iID_DonViID is null or NCCQ.iID_DonViID = @iID_DonViID) 
	            and (ISNULL(@iQuy,'') = '' or @iQuy = 0 or NCCQ.iQuy = @iQuy)
	            and (ISNULL(@iNamKeHoach,'') = '' or NCCQ.iNamKeHoach = @iNamKeHoach) and  NCCQ.iID_TongHopID is null and ( NCCQ.sTongHop is null or NCCQ.sTongHop = '')
                Order by cte.iID_TongHopID

                Select db.* ,ROW_NUMBER() OVER (ORDER BY db.iID_TongHopID) AS sSTT from  #db  db
                DROP TABLE #tmp
                DROP TABLE #db";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<CPNHNhuCauChiQuy_Model>(sql,
                    param: new
                     {
                         sSoDeNghi,
                         dNgayDeNghi,
                         iID_BQuanLyID,
                         iID_DonViID,
                         iQuy,
                         iNamKeHoach
                     },
                     commandType: CommandType.Text
                 );

                return items;
            }
        }

        public IEnumerable<NS_DonVi> GetDonviListByYear(int namLamViec = 0)
        {
            var sql =
                @"SELECT DISTINCT b.iID_MaDonVi, b.sTen, CONCAT(b.iID_MaDonVi, ' - ', b.sTen) AS sMoTa, b.iID_Ma
                FROM ns_donvi b
                WHERE b.iNamLamViec_DonVi = @namLamViec AND b.iTrangThai = 1
                ORDER BY iID_MaDonVi";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_DonVi>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                         namLamViec
                     },
                     commandType: CommandType.Text
                 ));

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
                query.AppendLine("AND UPPER(tgct.sMaTienTeQuyDoi) IN ('USD', 'VND') ");
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
        private NS_DonVi GetDonviByID(Guid iID_Ma)
        {
            var sql = "SELECT DISTINCT b.iID_MaDonVi, b.sTen, (b.iID_MaDonVi + ' - ' + b.sTen) as sMoTa, " +
                "b.iID_Ma FROM ns_donvi b WHERE b.iID_Ma = @iID_Ma";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NS_DonVi>(sql, param: new { iID_Ma }, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<NS_PhongBan> GetBQuanlyList()
        {
            var sql =
                @"SELECT DISTINCT b.iID_MaPhongBan, b.sTen, (b.sTen + ' - ' + b.sMoTa) as sMoTa
                FROM NS_PhongBan b
                ORDER BY iID_MaPhongBan";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NS_PhongBan>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }
        public IEnumerable<NH_DM_TiGia> GetDonviListTyGia()
        {
            var sql =
                @"SELECT DISTINCT b.ID, b.sTenTiGia , b.sMaTienTeGoc
                FROM NH_DM_TiGia b where b.sMaTienTeGoc = UPPER('USD') or b.sMaTienTeGoc = UPPER('VND')
                ORDER BY ID";

            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DM_TiGia>(new CommandDefinition(
                     commandText: sql,
                     parameters: new
                     {
                     },
                     commandType: CommandType.Text
                 ));

                return items;
            }
        }
        public IEnumerable<NH_DM_TiGia_ChiTiet> GetDonviListMangoaitekhac(Guid? iDTiGia, bool isMaNgoaiTeKhac = true)
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
        public NH_NhuCauChiQuy NhuCauChiQuySave(NH_NhuCauChiQuy data, string dieuchinh ,string sUsername)
        {
            try
            {
                NS_DonVi Donvi = new NS_DonVi();
                NH_NhuCauChiQuy NhuCauChiQuy = new NH_NhuCauChiQuy();
                Donvi = GetDonviByID((Guid)data.iID_DonViID);
                NhuCauChiQuy = GetNhuCauChiQuyId((Guid)data.ID);
                if (NhuCauChiQuy != null)
                {
                    data.fTongChiNgoaiTeUSD = NhuCauChiQuy.fTongChiNgoaiTeUSD;
                    data.fTongChiNgoaiTeVND = NhuCauChiQuy.fTongChiNgoaiTeVND;
                    data.fTongChiTrongNuocUSD = NhuCauChiQuy.fTongChiTrongNuocUSD;
                    data.fTongChiTrongNuocVND = NhuCauChiQuy.fTongChiTrongNuocVND;
                    data.bIsKhoa = NhuCauChiQuy.bIsKhoa;
                    data.sTongHop = NhuCauChiQuy.sTongHop;
                    data.iID_TongHopID = NhuCauChiQuy.iID_TongHopID;
                }
                if (Donvi != null)
                {
                    data.iID_MaDonVi = Donvi.iID_MaDonVi;
                }
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        data.dNgayTao = DateTime.Now;
                        data.sNguoiTao = sUsername;

                        conn.Insert<NH_NhuCauChiQuy>(data, trans);
                    }
                    else
                    {
                        data.dNgaySua = DateTime.Now;
                        data.sNguoiSua = sUsername;

                        if (dieuchinh == "1")
                        {
                            if (NhuCauChiQuy.iID_GocID != null)
                            {
                                data.iID_GocID = NhuCauChiQuy.iID_GocID;
                            }
                            else
                            {
                                data.iID_GocID = NhuCauChiQuy.ID;
                            }
                            data.iLanDieuChinh = NhuCauChiQuy.iLanDieuChinh + 1;
                            data.iID_ParentAdjustID = NhuCauChiQuy.ID;
                            data.ID = Guid.Empty;
                            conn.Insert<NH_NhuCauChiQuy>(data, trans);
                            trans.Commit();
                            trans = conn.BeginTransaction();

                            NhuCauChiQuy.bIsActive = true;
                            conn.Update<NH_NhuCauChiQuy>(NhuCauChiQuy, trans);
                        }
                        else
                        {
                            conn.Update<NH_NhuCauChiQuy>(data, trans);
                        }
                    }

                    trans.Commit();
                    return data;
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return data;
        }
        
        public NH_NhuCauChiQuy NhuCauChiQuyTongHopSave(NH_NhuCauChiQuy data, List<CPNHNhuCauChiQuy_Model> lstItem , string sUsername)
        {
            try
            {
                var getIDTongHop = 0;
                NH_NhuCauChiQuy NhuCauChiQuy = new NH_NhuCauChiQuy();
                NhuCauChiQuy = GetNhuCauChiQuyId((Guid)data.ID);
                if (data.ID == Guid.Empty && lstItem != null)
                {
                    NH_NhuCauChiQuy GetDataTongHop = GetNhuCauChiQuyId(lstItem[0].ID);
                    var sTongHop = "";
                    for (int n = 0; n < lstItem.Count; n++)
                    {
                        if (n == lstItem.Count - 1)
                        {
                            sTongHop += lstItem[n].ID;
                        }
                        else
                        {
                            sTongHop += lstItem[n].ID + ",";
                        }
                    }
                    data.sTongHop = sTongHop;
                    data.iID_BQuanLyID = GetDataTongHop.iID_BQuanLyID;
                }

                if (NhuCauChiQuy != null)
                {
                    NhuCauChiQuy.sSoDeNghi = data.sSoDeNghi;
                    NhuCauChiQuy.dNgayDeNghi = data.dNgayDeNghi;
                }
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    if (data.ID == Guid.Empty)
                    {
                        data.dNgayTao = DateTime.Now;
                        data.sNguoiTao = sUsername;
                        getIDTongHop = 1;
                        conn.Insert<NH_NhuCauChiQuy>(data, trans);
                    }
                    else
                    {
                        NhuCauChiQuy.dNgaySua = DateTime.Now;
                        NhuCauChiQuy.sNguoiSua = sUsername;

                        conn.Update<NH_NhuCauChiQuy>(NhuCauChiQuy, trans);
                    }

                    trans.Commit();
                    if(getIDTongHop == 1 && lstItem != null)
                    {   
                        foreach (var item in lstItem)
                        {
                            var NHCQ = GetNhuCauChiQuyId(item.ID);
                            NHCQ.iID_TongHopID = data.ID;
                            NHCQ.dNgaySua = DateTime.Now;
                            NHCQ.sNguoiSua = sUsername;

                            conn.Update<NH_NhuCauChiQuy>(NHCQ, trans);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return data;
        }


        public bool DeleteNhuCauChiQuy(Guid iId)
        {
            try
            {
                
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    
                    var checkExist =
                        conn.QueryFirstOrDefault<NH_NhuCauChiQuy>("select * from NH_NhuCauChiQuy where ID = @iId", param: new { iId },trans);
                    if (checkExist != null)
                    {
                        var sql = @"Update NH_NhuCauChiQuy set iID_TongHopID = null where iID_TongHopID = @iId";
                        conn.Execute(sql, param: new { iId }, trans, commandType: CommandType.Text);

                        var sqlDieuChinh = $"Update NH_NhuCauChiQuy set bIsActive = 0 where ID = @iId";
                        conn.Execute(sqlDieuChinh, param: new { iId = checkExist.iID_ParentAdjustID }, trans , commandType: CommandType.Text);

                        var sqlDeleteCT = $"Delete from NH_NhuCauChiQuy_ChiTiet where iID_NhuCauChiQuyID = @iId";
                        conn.Execute(sqlDeleteCT, param: new { iId }, trans, commandType: CommandType.Text);

                        conn.Delete<NH_NhuCauChiQuy>(checkExist, trans);
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

        public IEnumerable<NH_DA_HopDong> GetListHopDong(Guid? iID_DonViID, Guid? iID_BQuanLyID)
        {
            var sql = "SELECT * FROM NH_DA_HopDong Where iID_BQuanLyID = @iID_BQuanLyID and iID_DonViID = @iID_DonViID ORDER BY sTenHopDong;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<NH_DA_HopDong>(sql, param: new { iID_BQuanLyID, iID_DonViID });
                return items;
            }
        }
        public NS_PhongBan GetPhongBanID(Guid? ID = null)
        {
            var sql = "SELECT * FROM NS_PhongBan Where iID_MaPhongBan = @ID;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<NS_PhongBan>(sql, param: new { ID }, commandType: CommandType.Text);
                return item;
            }
        }
        public IEnumerable<CPNHNhuCauChiQuy_ChiTiet_Model> GetListNhucauchiquyChitiet(Guid? iDonvi, int iQuy, int iNam)
        {
            var iID_DonViID = iDonvi == Guid.Empty ? null : iDonvi;
            var sql = "SELECT NH_NhuCauChiQuy_ChiTiet.*, NH_NhuCauChiQuy.iID_DonViID," +
                "Case When NH_NhuCauChiQuy_ChiTiet.iID_HopDongID is not null then NH_DA_HopDong.sTenHopDong " +
                "Else NH_NhuCauChiQuy_ChiTiet.sNoiDung " +
                "End as sTenHopDong , " +
                "NS_DonVi.sTen as sTenDonvi, NH_KHChiTietBQP_NhiemVuChi.ID as ID_NhiemVuChi," +
                "Case When NH_KHChiTietBQP_NhiemVuChi.sTenNhiemVuChi is not null then NH_KHChiTietBQP_NhiemVuChi.sTenNhiemVuChi " +
                "Else N'Chưa có chương trình' End as sTenNhiemVuChi " +
                "FROM NH_NhuCauChiQuy_ChiTiet " +
                "left join NH_NhuCauChiQuy on NH_NhuCauChiQuy.ID = NH_NhuCauChiQuy_ChiTiet.iID_NhuCauChiQuyID  " +
                "left join NH_DA_HopDong on NH_DA_HopDong.ID = NH_NhuCauChiQuy_ChiTiet.iID_HopDongID " +
                "left join NS_DonVi on NS_DonVi.iID_Ma = NH_NhuCauChiQuy.iID_DonViID " +
                "left join NH_KHChiTietBQP_NhiemVuChi on NH_KHChiTietBQP_NhiemVuChi.ID = NH_DA_HopDong.iID_KHCTBQP_ChuongTrinhID " +
                "Where (NH_NhuCauChiQuy.iID_BQuanLyID = @iID_DonViID or @iID_DonViID is null) and (NH_NhuCauChiQuy.iQuy = @iQuy or @iQuy = 0 or @iQuy is null) " +
                "and (NH_NhuCauChiQuy.iNamKeHoach = @iNam or @iNam is null or @iNam = 0) " +
                "ORDER BY ID_NhiemVuChi desc, sTenNhiemVuChi, sTenDonvi, iID_NhuCauChiQuyID ";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<CPNHNhuCauChiQuy_ChiTiet_Model>(sql, param: new { iID_DonViID, iQuy, iNam }, commandType: CommandType.Text); ;
                return items;
            }
        }
        public IEnumerable<CPNHNhuCauChiQuy_ChiTiet_Model> GetListNhucauchiquyChitietBaoCao2(int iUSD, int iVND, Guid? iDonvi, int iQuy, int iNam)
        {
            var iID_DonViID = iDonvi == Guid.Empty ? null : iDonvi;
            var fUSD = 1;
            var fVND = 1;
            if (iUSD == 1){fUSD = 1000;}
            else if (iUSD == 2){fUSD = 1000000; }
            else if (iUSD == 3){fUSD = 1000000000; }
            else { fUSD = 1; }

            if (iVND == 1) { fVND = 1000; }
            else if (iVND == 2) { fVND = 1000000; }
            else if (iVND == 3) { fVND = 1000000000; }
            else { fVND = 1; }

            using (var conn = _connectionFactory.GetConnection())
            {
                DynamicParameters lstPrams = new DynamicParameters();
                lstPrams.Add("fUSD", fUSD);
                lstPrams.Add("fVND", fVND);
                lstPrams.Add("iID_DonViID", iID_DonViID);
                lstPrams.Add("iQuy", iQuy);
                lstPrams.Add("iNam", iNam);

                var items = conn.Query<CPNHNhuCauChiQuy_ChiTiet_Model>("proc_get_all_cpnh_Nhucauchiquy_baocao", lstPrams, commandType: CommandType.StoredProcedure);
                return items;
            }
        }
        public IEnumerable<ThucHienNganSach_GiaiDoan_Model> GetListGiaiDoan(int toY = 0 , int fromY = 0)
        {
            var sql = @"select distinct TTCP.* from NH_TT_ThanhToan_ChiTiet ttct
                    inner join NH_TT_ThanhToan tt on ttct.iID_ThanhToanID = tt.ID 
                    inner join NH_KHChiTietBQP_NhiemVuChi nvc on nvc.ID = tt.iID_KHCTBQP_NhiemVuChiID
                    inner join NH_KHTongTheTTCP_NhiemVuChi nvcTTCP on nvc.iID_KHTTTTCP_NhiemVuChiID = nvcTTCP.ID
                    inner join NH_KHTongTheTTCP TTCP on TTCP.ID = nvcTTCP.iID_KHTongTheID
                    and (tt.dNgayDeNghi >=  convert(datetime,(concat(@toY,'-01-01 00:00:00.000'))) or @toY = 0 ) 
                    and (tt.dNgayDeNghi <=  convert(datetime,(concat(@fromY,'-12-31 00:00:00.000'))) or @fromY = 0 )
                    Order by TTCP.iGiaiDoanTu";
            using (var conn = _connectionFactory.GetConnection())
            {
                var items = conn.Query<ThucHienNganSach_GiaiDoan_Model>(
                    sql, 
                    param: new {
                        toY,
                        fromY
                    }, commandType: CommandType.Text); ;
                return items;
            }
        }
        public string GetSTTLAMA(int STT = 1)
        {
            var sql = "SELECT dbo.ToRoman(@STT) as STT;";
            using (var conn = _connectionFactory.GetConnection())
            {
                var item = conn.QueryFirstOrDefault<string>(sql, param: new { STT }, commandType: CommandType.Text);

                return item;

            }
        }
        public bool LockOrUnLockNhuCauChiQuy(Guid id, bool isLockOrUnLock)
        {
            try
            {
                using (var conn = _connectionFactory.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    var entity = conn.Get<NH_NhuCauChiQuy>(id, trans);
                    if (entity == null) return false;
                    entity.bIsKhoa = isLockOrUnLock;

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
    }
}
