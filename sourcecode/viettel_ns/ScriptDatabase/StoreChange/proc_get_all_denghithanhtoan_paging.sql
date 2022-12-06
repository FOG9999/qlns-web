ALTER PROC [dbo].[proc_get_all_denghithanhtoan_paging]
@iID_DonVi uniqueidentifier,
@sSoDeNghi nvarchar(50),
@dNgayDeNghi datetime,
@iLoaiNoiDungChi int,
@iLoaiDeNghi int,
@iID_ChuDauTuID uniqueidentifier,
@iID_KHCTBQP_NhiemVuChiID uniqueidentifier,
@iNamKeHoach int, 
@iQuyKeHoach int, 
@iNamNganSach int,
@iCoQuanThanhToan int, 
@iID_NhaThauID uniqueidentifier,
@iTrangThai int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS

BEGIN
	SELECT tt.*, CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen) AS sTen,cdt.sTenCDT,ct.sTenNhiemVuChi, nt.sTenNhaThau,
			fTongCTDeNghiCapKyNay_USD, 
			fTongCTDeNghiCapKyNay_VND, 
			fTongCtPheDuyetCapKyNay_USD, 
			fTongCTPheDuyetCapKyNay_VND,
			sTenDuAn,
			sTenHopDong
	INTO #temp
	FROM NH_TT_ThanhToan as tt 
			left join NS_DonVi as dv on  tt.iID_DonVi = dv.iID_Ma
			left join NH_KHChiTietBQP_NhiemVuChi as ct on ct.ID = tt.iID_KHCTBQP_NhiemVuChiID
			left join DM_ChuDauTu as cdt on cdt.ID = tt.iID_ChuDauTuID
			left join NH_DM_NhaThau as nt on nt.ID= tt.iID_NhaThauID
			left join NH_DA_DuAn as da on da.ID = tt.iID_DuAnID
			left join NH_DA_HopDong as hd on hd.ID = tt.iID_HopDongID
			left join (
				select 
				Sum(fDeNghiCapKyNay_USD) as fTongCTDeNghiCapKyNay_USD, 
				Sum(fDeNghiCapKyNay_VND)   as fTongCTDeNghiCapKyNay_VND, 
				Sum(fPheDuyetCapKyNay_USD)  as fTongCtPheDuyetCapKyNay_USD, 
				Sum(fPheDuyetCapKyNay_VND)  as fTongCTPheDuyetCapKyNay_VND,
				tt_ct.iID_ThanhToanID
				from NH_TT_ThanhToan_ChiTiet as tt_ct
				group by tt_ct.iID_ThanhToanID
			) as chitiet on chitiet.iID_ThanhToanID = tt.ID
	WHERE 
			(@iID_DonVi is null or tt.iID_DonVi = @iID_DonVi)
			AND (@sSoDeNghi is null or tt.sSoDeNghi = @sSoDeNghi)
			AND (@dNgayDeNghi is null or tt.dNgayDeNghi = @dNgayDeNghi)
			AND (@iLoaiNoiDungChi is null or tt.iLoaiNoiDungChi = @iLoaiNoiDungChi)
			AND (@iLoaiDeNghi is null or tt.iLoaiDeNghi = @iLoaiDeNghi)
			AND (@iID_ChuDauTuID is null or tt.iID_ChuDauTuID = @iID_ChuDauTuID)
			AND (@iID_KHCTBQP_NhiemVuChiID is null or tt.iID_KHCTBQP_NhiemVuChiID = @iID_KHCTBQP_NhiemVuChiID)
			AND (@iQuyKeHoach = 0 or @iQuyKeHoach is null or tt.iQuyKeHoach = @iQuyKeHoach)
			AND (@iNamKeHoach is null or tt.iNamKeHoach = @iNamKeHoach)
			AND (@iNamNganSach is null or tt.iNamNganSach = @iNamNganSach)
			AND (@iCoQuanThanhToan is null or tt.iCoQuanThanhToan = @iCoQuanThanhToan)
			AND (@iID_NhaThauID is null or tt.iID_NhaThauID = @iID_NhaThauID)
			AND (@iTrangThai is null or tt.iTrangThai = @iTrangThai)
		
		SET @iToTalItem = (SELECT COUNT(*) FROM #temp);
		
		SELECT * FROM #temp
		ORDER BY dNgayTao DESC
		OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
		FETCH NEXT @ItemsPerPage ROWS ONLY;
		
		drop table #temp;
END