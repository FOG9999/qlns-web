DECLARE @iNamKeHoach int set @iNamKeHoach = 2019
DECLARE @iIdMaDonVi nvarchar(50) set @iIdMaDonVi = '21'
DECLARE @iIdNguonVon int set @iIdNguonVon = 2
DECLARE @iQuy int set @iQuy = 1

--###--

DECLARE @fKeHoachQuyTruoc float
DECLARE @fKeHoachQuyNay float
DECLARE @fThanhToanQuyTruoc float
DECLARE @fThanhToanQuyNay float
-- Quy truoc
SET @fKeHoachQuyTruoc = (SELECT ISNULL(SUM(ISNULL(dt.fGiaTriDeNghi, 0)), 0)
							FROM VDT_NC_NhuCauChi as tbl
							INNER JOIN VDT_NC_NhuCauChi_ChiTiet as dt on tbl.Id = dt.iID_NhuCauChiId
							WHERE iNamKeHoach = @iNamKeHoach 
								AND iID_MaDonViQuanLy = @iIdMaDonVi
								AND tbl.iID_NguonVonID = @iIdNguonVon
								AND ((@iQuy = 1 AND tbl.iQuy = 4 AND iNamKeHoach = (@iNamKeHoach - 1))
									OR (tbl.iQuy = (@iQuy - 1) AND iNamKeHoach = @iNamKeHoach)))

SET @fThanhToanQuyTruoc = (SELECT SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0))
							FROM VDT_TT_DeNghiThanhToan as tbl
							INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
							WHERE iLoaiThanhToan in (1,2)
								AND iNamKeHoach = @iNamKeHoach 
								AND iID_MaDonViQuanLy = @iIdMaDonVi
								AND tbl.iID_NguonVonID = @iIdNguonVon
								AND ((@iQuy = 1 AND MONTH(tbl.dNgayDeNghi) IN (10,11,12) AND tbl.iNamKeHoach = @iNamKeHoach - 1)
									OR (@iQuy = 2 AND MONTH(tbl.dNgayDeNghi) IN (1,2,3) AND tbl.iNamKeHoach = @iNamKeHoach)
									OR (@iQuy = 3 AND MONTH(tbl.dNgayDeNghi) IN (4,5,6) AND tbl.iNamKeHoach = @iNamKeHoach)
									OR (@iQuy = 4 AND MONTH(tbl.dNgayDeNghi) IN (7,8,9)) AND tbl.iNamKeHoach = @iNamKeHoach))

-- Quy nay
SET @fKeHoachQuyNay = (SELECT ISNULL(SUM(ISNULL(dt.fGiaTriDeNghi, 0)), 0)
						FROM VDT_NC_NhuCauChi as tbl
						INNER JOIN VDT_NC_NhuCauChi_ChiTiet as dt on tbl.Id = dt.iID_NhuCauChiId
						WHERE iNamKeHoach = @iNamKeHoach 
							AND iID_MaDonViQuanLy = @iIdMaDonVi
							AND tbl.iID_NguonVonID = @iIdNguonVon
							AND tbl.iQuy = @iQuy)

SET @fThanhToanQuyNay = (SELECT SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0))
							FROM VDT_TT_DeNghiThanhToan as tbl
							INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
							WHERE iLoaiThanhToan in (1,2)
								AND iNamKeHoach = @iNamKeHoach 
								AND iID_MaDonViQuanLy = @iIdMaDonVi
								AND tbl.iID_NguonVonID = @iIdNguonVon
								AND ((@iQuy = 1 AND MONTH(tbl.dNgayDeNghi) IN (1,2,3))
									OR (@iQuy = 2 AND MONTH(tbl.dNgayDeNghi) IN (4,5,6))
									OR (@iQuy = 3 AND MONTH(tbl.dNgayDeNghi) IN (7,8,9))
									OR (@iQuy = 4 AND MONTH(tbl.dNgayDeNghi) IN (10,11,12))))

SELECT (ISNULL(@fKeHoachQuyTruoc,0) - ISNULL(@fThanhToanQuyTruoc, 0)) as fQuyTruocChuaGiaiNgan,
		(ISNULL(@fKeHoachQuyNay, 0)) as fQuyNayDuocCap,
		(ISNULL(@fThanhToanQuyNay, 0)) as fGiaiNganQuyNay
