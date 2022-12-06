
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_rpt_tinh_hinh_thuc_hien_du_an]    Script Date: 03/11/2022 2:22:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_vdt_rpt_tinh_hinh_thuc_hien_du_an]
	@iID_DonViQuanLyID uniqueidentifier,
	@iID_DuAn uniqueidentifier,
	@denngay datetime 
AS 
BEGIN
	select  ROW_NUMBER() OVER(ORDER BY dntt.iID_DeNghiThanhToanID) as STT,    dntt.iID_DeNghiThanhToanID as iID_DeNghiTT,
	case when dntt.iLoaiThanhToan = 1 then  mlns.sMoTa else null end as sMLNS,    dm_nt.sTenNhaThau as sTenNhaThau,
	da_hd.sSoHopDong as sSoHopDong, da_hd.iThoiGianThucHien as iThoiGianThucHien,isnull(da_hd.fTienHopDong, 0) as fTienHopDong ,
	dntt.dNgayPheDuyet as dNgayThanhToan,    case when dntt.iLoaiThanhToan = 1 then isnull(fGiaTriThanhToan,0) else 0 end fSoThanhToan,
	case when dntt.iLoaiThanhToan = 2 then isnull(fGiaTriThanhToan,0) else 0 end fSoTamUng,    isnull(fThuHoiTamUng,0) as fThuHoiTamUng,
	(isnull(fGiaTriThanhToan,0) - isnull(fThuHoiTamUng,0)) as fTongCongGiaiNgan,
	case when sSoKHVU > 0 then dntt.dNgayPheDuyet else null end dNgayDaCapUng,
	isnull(fSoDaCapUng,0) as fSoDaCapUng
	from VDT_TT_DeNghiThanhToan as dntt
	left join 
	(     select (Sum(fGiaTriThanhToanNN) + Sum(fGiaTriThanhToanTN)) as fGiaTriThanhToan, Sum(fGiaTriThuHoiNamNayNN + fGiaTriThuHoiNamNayTN + fGiaTriThuHoiNamTruocNN + fGiaTriThuHoiNamTruocTN) as fThuHoiTamUng,
	iID_DeNghiThanhToanID,      iID_NganhID     
	from VDT_TT_PheDuyetThanhToan_ChiTiet     
	group by iID_DeNghiThanhToanID, iID_NganhID    ) as pqtt_ct on pqtt_ct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID   
	left join VDT_DM_NhaThau as dm_nt on dm_nt.iID_NhaThauID = dntt.iID_NhaThauId    
	left join VDT_DA_TT_HopDong as da_hd on da_hd.iID_HopDongID = dntt.iID_HopDongId    
	left join NS_MucLucNganSach  as mlns on mlns.iID_MaMucLucNganSach = pqtt_ct.iID_NganhID    
	left join    (     select Sum(fGiaTriUng) as fSoDaCapUng, count(iID_KeHoachVonID) as sSoKHVU,  iID_DeNghiThanhToanID     
	from VDT_TT_DeNghiThanhToan_KHV as dn_khvu     inner join VDT_KHV_KeHoachVonUng  as khvn on dn_khvu.iID_KeHoachVonID = khvn.Id     
	where iLoai = 2     group by iID_DeNghiThanhToanID      ) as khv_dn on  khv_dn.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID      
	where (@iID_DonViQuanLyID is null or dntt.iID_DonViQuanLyID = @iID_DonViQuanLyID)
	and (@iID_DuAn is null or dntt.iID_DuAnId = @iID_DuAn)
	and (@denngay is null or   CAST(dntt.dNgayPheDuyet as DATE) < CAST(@denngay as DATE));
END