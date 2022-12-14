USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_qltthopdong_paging]    Script Date: 15/11/2022 4:42:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






ALTER PROC [dbo].[proc_get_all_qltthopdong_paging]
        @sUserName NVARCHAR(MAX),
        @iNamLamViec INT,
		@sSoHopDong NVARCHAR(100),
		@fTienHopDongTu FLOAT,
		@fTienHopDongDen FLOAT,
		@dHopDongTuNgay DATETIME,
        @dHopDongDenNgay DATETIME,
        @sTenDuAn NVARCHAR(MAX),
		@iID_MaDonVi NVARCHAR(MAX),
		@sId_CDT NVARCHAR(MAX),
        @CurrentPage INT,
        @ItemsPerPage INT,
        @iToTalItem INT OUTPUT,
		@sTenHopDong NVARCHAR(MAX)
AS
BEGIN
SET @iToTalItem = (SELECT 
                         COUNT(hd.iID_HopDongID)
                   FROM 
                         VDT_DA_TT_HopDong hd
                   LEFT JOIN 
                         VDT_DA_DuAn da 
                         ON hd.iID_DuAnID = da.iID_DuAnID
                   LEFT JOIN 
                         NS_DonVi dv 
                         ON da.iID_DonViQuanLyID = dv.iID_Ma
				   LEFT JOIN
						 VDT_DA_GoiThau gt
						 ON hd.iID_GoiThauID = gt.iID_GoiThauID
				   LEFT JOIN (select * FROM DM_ChuDauTu where iNamLamViec = @iNamLamViec and iTrangThai = 1) chudautu 
						 ON da.iID_MaCDT = chudautu.sId_CDT  
                   WHERE
                         da.iID_DonViQuanLyID IN
                         (
                         SELECT
								b.iID_Ma 
                         FROM
								NS_NguoiDung_DonVi a,
								ns_donvi b 
                         WHERE
							    a.iID_MaDonVi = b.iID_MaDonVi 
						        AND a.iNamLamViec = @iNamLamViec 
					            AND b.iNamLamViec_DonVi = @iNamLamViec 
				                AND a.sMaNguoiDung = @sUserName 
			                    AND a.iTrangThai = 1 
		                        AND b.iTrangThai = 1 )
						 AND ( ISNULL( @sSoHopDong, '' ) = '' OR hd.sSoHopDong LIKE CONCAT ( N'%',@sSoHopDong, N'%' ) )
						 AND ( @fTienHopDongTu IS NULL OR ( hd.fTienHopDong >= @fTienHopDongTu ) )
						 AND ( @fTienHopDongDen IS NULL OR ( hd.fTienHopDong <= @fTienHopDongDen ) )
						 AND ( @dHopDongTuNgay IS NULL OR ( hd.dNgayHopDong >= @dHopDongTuNgay ) ) 
                         AND ( @dHopDongDenNgay IS NULL OR ( hd.dNgayHopDong <= @dHopDongDenNgay ) )
                         AND ( ISNULL( @sTenDuAn, '' ) = '' OR da.sTenDuAn LIKE CONCAT ( N'%',@sTenDuAn, N'%' ) )
						 AND ( ISNULL( @iID_MaDonVi, '' ) = '' OR dv.iID_MaDonVi = @iID_MaDonVi)
						 AND ( ISNULL( @sId_CDT, '' ) = '' OR chudautu.sId_CDT = @sId_CDT)
						 AND ( ISNULL( @sTenHopDong, '' ) = '' OR hd.sTenHopDong LIKE CONCAT ( N'%',@sTenHopDong, N'%' ) )

                         --AND (hd.bActive = 1)
                         --AND (da.bIsDeleted = 1)
                                        )
SELECT 
        hd.*,
		da.sTenDuAn,
		gt.sTenGoiThau,
		nhathau.sTenNhaThau,
		loaihopdong.sTenLoaiHopDong,
		dv.sTen as TenDonVi,
		chudautu.sTenCDT as ChuDauTu
		INTO #tmp
FROM
        VDT_DA_TT_HopDong hd
LEFT JOIN 
        VDT_DA_DuAn da 
        ON hd.iID_DuAnID = da.iID_DuAnID
LEFT JOIN 
        NS_DonVi dv 
        ON da.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN
		VDT_DA_GoiThau gt
		ON hd.iID_GoiThauID = gt.iID_GoiThauID
LEFT JOIN (select * FROM VDT_DM_NhaThau) nhathau 
		ON hd.iID_NhaThauThucHienID = nhathau.iID_NhaThauID
LEFT JOIN (select * FROM VDT_DM_LoaiHopDong) loaihopdong
		ON hd.iID_LoaiHopDongID = loaihopdong.iID_LoaiHopDongID
LEFT JOIN (select * FROM DM_ChuDauTu where iNamLamViec = @iNamLamViec and iTrangThai = 1) chudautu 
		ON da.iID_MaCDT = chudautu.sId_CDT       
WHERE
        da.iID_DonViQuanLyID IN
        (
        SELECT
                b.iID_Ma 
        FROM
                NS_NguoiDung_DonVi a,
                ns_donvi b 
        WHERE
                a.iID_MaDonVi = b.iID_MaDonVi 
                AND a.iNamLamViec = @iNamLamViec 
                AND b.iNamLamViec_DonVi = @iNamLamViec 
                AND a.sMaNguoiDung = @sUserName 
                AND a.iTrangThai = 1 
                AND b.iTrangThai = 1 )
        AND ( ISNULL( @sSoHopDong, '' ) = '' OR hd.sSoHopDong LIKE CONCAT ( N'%',@sSoHopDong, N'%' ) )
		AND ( @fTienHopDongTu IS NULL OR ( hd.fTienHopDong >= @fTienHopDongTu ) )
		AND ( @fTienHopDongDen IS NULL OR ( hd.fTienHopDong <= @fTienHopDongDen ) )
		AND ( @dHopDongTuNgay IS NULL OR ( hd.dNgayHopDong >= @dHopDongTuNgay ) ) 
        AND ( @dHopDongDenNgay IS NULL OR ( hd.dNgayHopDong <= @dHopDongDenNgay ) )
        AND ( ISNULL( @sTenDuAn, '' ) = '' OR da.sTenDuAn LIKE CONCAT ( N'%',@sTenDuAn, N'%' ) )
		AND ( ISNULL( @iID_MaDonVi, '' ) = '' OR dv.iID_MaDonVi = @iID_MaDonVi)
		AND ( ISNULL( @sId_CDT, '' ) = '' OR chudautu.sId_CDT = @sId_CDT )
		AND ( ISNULL( @sTenHopDong, '' ) = '' OR hd.sTenHopDong LIKE CONCAT ( N'%',@sTenHopDong, N'%' ) )

        --AND (hd.bActive = 1)
        --AND (da.bIsDeleted = 1)

SELECT tmp.iID_HopDongID, SUM((CASE WHEN tmp.bIsGoc = 1 THEN tmp.fTienHopDong ELSE cd.fTienHopDong END)) AS fGiaTriSauDieuChinh INTO #tmpGiaTriSauDC
FROM #tmp AS tmp
LEFT JOIN VDT_DA_TT_HopDong AS cd ON tmp.bIsGoc = 0 AND tmp.iID_HopDongGocID = cd.iID_HopDongGocID
GROUP BY tmp.iID_HopDongID;

WITH temp(iID_HopDongID, row_num)
AS
(
	SELECT iID_HopDongID, CAST(0 as int) as row_num
	FROM VDT_DA_TT_HopDong 
	WHERE iID_ParentID IS NULL
	UNION ALL
	SELECT cd.iID_HopDongID, (pr.row_num + 1)
	FROM VDT_DA_TT_HopDong as cd, temp as pr
	WHERE cd.iID_ParentID = pr.iID_HopDongID
)
SELECT * INTO #tmpDieuChinh
FROM temp


SELECT  tmp.*, tmpGiaTriSauDC.fGiaTriSauDieuChinh, tmpDieuChinh.row_num as iTongSoDieuChinh
FROM #tmp AS tmp
LEFT JOIN #tmpGiaTriSauDC AS tmpGiaTriSauDC ON tmp.iID_HopDongID = tmpGiaTriSauDC.iID_HopDongID
LEFT JOIN #tmpDieuChinh AS tmpDieuChinh ON tmp.iID_HopDongID = tmpDieuChinh.iID_HopDongID
ORDER BY tmp.dNgayHopDong desc, tmp.sSoHopDong
OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

DROP TABLE #tmp
DROP TABLE #tmpGiaTriSauDC
DROP TABLE #tmpDieuChinh

END

