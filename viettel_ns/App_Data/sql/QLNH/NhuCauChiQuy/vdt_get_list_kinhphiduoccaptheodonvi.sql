--declare @tabTable int = 1 , @iQuyList int = 0 , @iNam int = 2022 , @iTuNam int = 2022 , @iDenNam int = 2022 , @iDonvi uniqueidentifier = null

Select distinct ttct.*, tt.iNamKeHoach, tt.iID_DonVi , CONCAT (dv.sTen , ' - ', dv.sMoTa) as sTenDonVi, nvc.ID as IDNhiemVuChi, da.ID as IDDuAn, tt.iLoaiNoiDungChi,hd.ID as IDHopDong,
nvc.sTenNhiemVuChi , da.sTenDuAn , hd.sTenHopDong,tt.iLoaiNoiDungChi, dmCDT.sTenCDT,
hd.fGiaTriUSD as HopDongUSD ,  hd.fGiaTriVND as HopDongVND, da.fGiaTriUSD as DuAnUSD ,  da.fGiaTriVND as DuAnVND,
nvcttcp.fGiaTri as NCVTTCP ,nvc.fGiaTriUSD as NhiemVuChi , 
QTND.KinhPhiUSD as KinhPhiUSD , QTND.KinhPhiVND as KinhPhiVND,
QTNDToY.KinhPhiToYUSD as KinhPhiToYUSD ,QTNDToY.KinhPhiToYVND as KinhPhiToYVND,
KPDC.KinhPhiDaChiUSD as KinhPhiDaChiUSD , KPDCVND.KinhPhiDaChiVND as KinhPhiDaChiVND , 
KPDCToY.KinhPhiDaChiUSD as KinhPhiDaChiToYUSD , KPDCVNDToY.KinhPhiDaChiVND as KinhPhiDaChiToYVND,
QTNDCT.fLuyKeKinhPhiDuocCap_USD , QTNDCT.fLuyKeKinhPhiDuocCap_VND,
TTCP.iGiaiDoanDen , TTCP.iGiaiDoanTu
from NH_TT_ThanhToan_ChiTiet ttct
left join NH_TT_ThanhToan tt on ttct.iID_ThanhToanID = tt.ID 
left join NS_DonVi dv on dv.iID_Ma = tt.iID_DonVi
left join NH_DA_HopDong hd on hd.ID = tt.iID_HopDongID
left join NH_DA_DuAn da on da.ID = tt.iID_DuAnID
left join DM_ChuDauTu dmCDT on da.iID_ChuDauTuID = dmCDT.ID
left join NH_KHChiTietBQP_NhiemVuChi nvc on nvc.ID = tt.iID_KHCTBQP_NhiemVuChiID
left join NH_KHTongTheTTCP_NhiemVuChi nvcTTCP on nvc.iID_KHTTTTCP_NhiemVuChiID = nvcTTCP.ID
left join NH_KHTongTheTTCP TTCP on TTCP.ID = nvcTTCP.iID_KHTongTheID
left join NH_QT_QuyetToanNienDo_ChiTiet QTNDCT on QTNDCT.iID_ThanhToan_ChiTietID = ttct.ID
left join (
		Select NDCT.iID_HopDongID, Sum(NDCT.fQTKinhPhiDuocCap_TongSo_USD) as KinhPhiUSD, sum(NDCT.fQTKinhPhiDuocCap_TongSo_VND) as KinhPhiVND
		from NH_QT_QuyetToanNienDo_ChiTiet NDCT group by NDCT.iID_HopDongID)
	QTND on QTND.iID_HopDongID = hd.ID
left join (
		select ThanhToan.iLoaiDeNghi , ThanhToan.iID_HopDongID , sum(ThanhToan.fChuyenKhoan_BangSo) as KinhPhiDaChiUSD from NH_TT_ThanhToan ThanhToan
		join NH_TT_ThanhToan_ChiTiet b 
		on ThanhToan.ID = b.iID_ThanhToanID 
		where ThanhToan.iLoaiNoiDungChi = 1 and ThanhToan.dNgayDeNghi > convert(datetime,(concat(year(GETDATE()),'-1-1 00:00:00.000'))) and 
			((ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-9-30 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 4)
			or (ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-6-30 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 3)
			or (ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-3-31 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 2)
			)
		Group BY ThanhToan.iLoaiDeNghi, ThanhToan.iID_HopDongID having ThanhToan.iLoaiDeNghi = 2 or ThanhToan.iLoaiDeNghi = 3
	) KPDCToY on KPDCToY.iID_HopDongID = hd.ID
left join (
		select ThanhToan.iLoaiDeNghi , ThanhToan.iID_HopDongID , sum(ThanhToan.fChuyenKhoan_BangSo) as KinhPhiDaChiVND from NH_TT_ThanhToan ThanhToan
		join NH_TT_ThanhToan_ChiTiet b 
		on ThanhToan.ID = b.iID_ThanhToanID 
		where ThanhToan.iLoaiNoiDungChi = 2 and ThanhToan.dNgayDeNghi > convert(datetime,(concat(year(GETDATE()),'-1-1 00:00:00.000'))) and 
			((ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-9-30 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 4)
			or (ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-6-30 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 3)
			or (ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE()),'-3-31 00:00:00.000'))) and DATEPART(quarter,GETDATE()) = 2)
			)
		Group BY ThanhToan.iLoaiDeNghi, ThanhToan.iID_HopDongID having ThanhToan.iLoaiDeNghi = 2 or ThanhToan.iLoaiDeNghi = 3
	) KPDCVNDToY on KPDCVNDToY.iID_HopDongID = hd.ID
left join (
		select ThanhToan.iLoaiDeNghi , ThanhToan.iID_HopDongID , sum(ThanhToan.fChuyenKhoan_BangSo) as KinhPhiDaChiUSD from NH_TT_ThanhToan ThanhToan
		join NH_TT_ThanhToan_ChiTiet b 
		on ThanhToan.ID = b.iID_ThanhToanID 
		where ThanhToan.iLoaiNoiDungChi = 1 and ThanhToan.iNamKeHoach = DATEPART(YEAR,GETDATE()) and ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE())-1,'-12-31 00:00:00.000')))
		Group BY ThanhToan.iLoaiDeNghi, ThanhToan.iID_HopDongID having ThanhToan.iLoaiDeNghi = 2 or ThanhToan.iLoaiDeNghi = 3
	) KPDC on KPDC.iID_HopDongID = hd.ID
left join (
		select ThanhToan.iLoaiDeNghi , ThanhToan.iID_HopDongID , sum(ThanhToan.fChuyenKhoan_BangSo) as KinhPhiDaChiVND from NH_TT_ThanhToan ThanhToan
		join NH_TT_ThanhToan_ChiTiet b 
		on ThanhToan.ID = b.iID_ThanhToanID 
		where ThanhToan.iLoaiNoiDungChi = 2 and ThanhToan.iNamKeHoach = DATEPART(YEAR,GETDATE()) and ThanhToan.dNgayDeNghi < convert(datetime,(concat(year(GETDATE())-1,'-12-31 00:00:00.000')))
		Group BY ThanhToan.iLoaiDeNghi, ThanhToan.iID_HopDongID having ThanhToan.iLoaiDeNghi = 2 or ThanhToan.iLoaiDeNghi = 3
	) KPDCVND on KPDCVND.iID_HopDongID = hd.ID
left join (
	Select NDCT.iID_HopDongID, Sum(NDCT.fQTKinhPhiDuocCap_TongSo_USD) as KinhPhiToYUSD, sum(NDCT.fQTKinhPhiDuocCap_TongSo_VND) as KinhPhiToYVND
		from NH_QT_QuyetToanNienDo_ChiTiet NDCT
		left join NH_QT_QuyetToanNienDo NDCTParent on NDCTParent.ID = NDCT.iID_QuyetToanNienDoID
		Where NDCTParent.iNamKeHoach = DATEPART(YEAR,GETDATE())
		group by NDCT.iID_HopDongID) 
		QTNDToY on QTNDToY.iID_HopDongID = hd.ID

Where(tt.dNgayDeNghi >= @dTuNgay or @dTuNgay is null or @dTuNgay = '') and (tt.dNgayDeNghi <= @dDenNgay or @dDenNgay is null or @dDenNgay = '') and (tt.iID_DonVi = @iDonvi or @iDonvi is null or @iDonvi = '00000000-0000-0000-0000-000000000000')
Order by tt.iID_DonVi , nvc.ID desc , nvc.sTenNhiemVuChi , da.ID desc , da.sTenDuAn, tt.iLoaiNoiDungChi ,hd.ID , hd.sTenHopDong


