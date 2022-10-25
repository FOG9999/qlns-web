SELECT 
ct1.iID_DeNghiThanhToan_ChiTietID,
ct1.iID_NganhID,
 ct1.iID_DuAnID, 
 (case when ct1.iID_HopDongID is null and ct1.iID_NhaThauID is null then null else (
 select top 1 iID_DeNghiThanhToan_ChiTietID from VDT_TT_DeNghiThanhToan_ChiTiet where iID_DuAnID = ct1.iID_DuAnID AND iID_HopDongID is null AND iID_NhaThauID is null and
 iID_DeNghiThanhToanID IN (
SELECT iID_DeNghiThanhToanID from VDT_TT_DeNghiThanhToan WHERE iNamKeHoach = @iNamThongTri AND iID_DonViQuanLyID = @sMaDonVi AND dNgayDeNghi <= @dNgayTaoThongTri
AND (@dNgayLapGanNhat IS NULL OR dNgayDeNghi > @dNgayLapGanNhat) AND iID_NguonVonID = @iNguonVon
)
 ) end) as iID_Parent,
 ct1.iID_HopDongID, 
 ct1.iID_NhaThauID, 
 ct1.fGiaTriThanhToan, 
 ct1.fGiaTriTamUng, 
 ct1.fGiaTriThuHoi 
 into #temptable
 FROM VDT_TT_DeNghiThanhToan_ChiTiet  ct1
WHERE ct1.iID_DeNghiThanhToanID IN (
SELECT iID_DeNghiThanhToanID from VDT_TT_DeNghiThanhToan WHERE iNamKeHoach = @iNamThongTri AND iID_DonViQuanLyID = @sMaDonVi AND dNgayDeNghi <= @dNgayTaoThongTri
AND (@dNgayLapGanNhat IS NULL OR dNgayDeNghi > @dNgayLapGanNhat) AND iID_NguonVonID = @iNguonVon
)

SELECT DISTINCT
g.iID_DeNghiThanhToan_ChiTietID,
g.iID_NganhID,
 g.iID_DuAnID, 
 g.iID_Parent,
 g.iID_HopDongID, 
 g.iID_NhaThauID, 
 g.fGiaTriThanhToan, 
 g.fGiaTriTamUng, 
 g.fGiaTriThuHoi,

(CASE WHEN c.iID_Parent IS NULL THEN 0 ELSE 1 END) as bHasChild
into #temptable1
 FROM #temptable as g 
 LEFT JOIN #temptable as c ON g.iID_DeNghiThanhToan_ChiTietID = c.iID_Parent;


WITH cte(iID_DeNghiThanhToan_ChiTietID, iID_NganhID, iID_DuAnID, iID_Parent, iID_HopDongID, iID_NhaThauID, fGiaTriThanhToan, fGiaTriTamUng, fGiaTriThuHoi, sLevelTab,bHasChild, location)
AS
(
	SELECT iID_DeNghiThanhToan_ChiTietID, iID_NganhID, iID_DuAnID, iID_Parent, iID_HopDongID, iID_NhaThauID, fGiaTriThanhToan, fGiaTriTamUng, fGiaTriThuHoi, CAST('' as nvarchar(250)) as sLevelTab, bHasChild, 
			CAST(iID_DeNghiThanhToan_ChiTietID as nvarchar(max)) as location
	FROM #temptable1 WHERE iID_Parent IS NULL
	UNION ALL
	SELECT cd.iID_DeNghiThanhToan_ChiTietID, cd.iID_NganhID, cd.iID_DuAnID, cd.iID_Parent, cd.iID_HopDongID, cd.iID_NhaThauID, cd.fGiaTriThanhToan, cd.fGiaTriTamUng, cd.fGiaTriThuHoi, CAST(CONCAT(pr.sLevelTab,' ') as nvarchar(250)) as sLevelTab, cd.bHasChild, 
			CAST(CONCAT(pr.location, CAST(cd.iID_DeNghiThanhToan_ChiTietID as nvarchar(max))) as nvarchar(max)) as location
	FROM cte as pr, #temptable1 as cd
	WHERE cd.iID_Parent = pr.iID_DeNghiThanhToan_ChiTietID
)
SELECT cte.*, 
(case when cte.iID_Parent is null then mlns.sM else '' end) AS sM,
(case when cte.iID_Parent is null then mlns.sTM else '' end) AS sTM,
(case when cte.iID_Parent is null then mlns.sTTM else '' end) AS sTTM,
(case when cte.iID_Parent is null then mlns.sNG else '' end) AS sNG,
(case when cte.iID_Parent is null then duan.sTenDuAn else nhathau.sTenNhaThau end) AS sNoiDung,
duan.iID_LoaiCongTrinhID, loai_congtrinh.sTenLoaiCongTrinh
FROM cte 
LEFT JOIN NS_MucLucNganSach mlns ON mlns.iID_MaMucLucNganSach = cte.iID_NganhID 
LEFT JOIN VDT_DM_NhaThau nhathau ON nhathau.iID_NhaThauID = cte.iID_NhaThauID
LEFT JOIN VDT_DA_DuAn duan ON duan.iID_DuAnID = cte.iID_DuAnID
LEFT JOIN VDT_DM_LoaiCongTrinh loai_congtrinh ON loai_congtrinh.iID_LoaiCongTrinh = duan.iID_LoaiCongTrinhID
--WHERE cte.iID_Parent is not null or (cte.iID_Parent is null and cte.bHasChild = 1)
ORDER BY location

DROP TABLE #temptable
DROP TABLE #temptable1
