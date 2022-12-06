SELECT 
ct1.iID_ThongTriChiTietID,
ct1.iID_NganhID,
 ct1.iID_DuAnID, 
 (case when ct1.iID_NhaThauID is null then null else (
 select top 1 iID_ThongTriChiTietID from VDT_ThongTri_ChiTiet WHERE iID_DuAnID = ct1.iID_DuAnID AND iID_NhaThauID is null and ct1.iID_ThongTriID = @iID_ThongTriID AND iID_KieuThongTriID = @iID_KieuThongTriID
 ) end) as iID_Parent,
 ct1.iID_NhaThauID, 
 ct1.fSoTien
 into #temptable
 FROM VDT_ThongTri_ChiTiet  ct1
WHERE ct1.iID_ThongTriID = @iID_ThongTriID AND iID_KieuThongTriID = @iID_KieuThongTriID

SELECT DISTINCT
g.iID_ThongTriChiTietID,
g.iID_NganhID,
 g.iID_DuAnID, 
 g.iID_Parent,
 g.iID_NhaThauID, 
 g.fSoTien,
(CASE WHEN c.iID_Parent IS NULL THEN 0 ELSE 1 END) as bHasChild
into #temptable1
 FROM #temptable as g 
 LEFT JOIN #temptable as c ON g.iID_ThongTriChiTietID = c.iID_Parent;


WITH cte(iID_ThongTriChiTietID, iID_NganhID, iID_DuAnID, iID_Parent, iID_NhaThauID, fSoTien, sLevelTab,bHasChild, location)
AS
(
	SELECT iID_ThongTriChiTietID, iID_NganhID, iID_DuAnID, iID_Parent, iID_NhaThauID, fSoTien, CAST('' as nvarchar(250)) as sLevelTab, bHasChild, 
			CAST(iID_ThongTriChiTietID as nvarchar(max)) as location
	FROM #temptable1 WHERE iID_Parent IS NULL
	UNION ALL
	SELECT cd.iID_ThongTriChiTietID, cd.iID_NganhID, cd.iID_DuAnID, cd.iID_Parent, cd.iID_NhaThauID, cd.fSoTien, CAST(CONCAT(pr.sLevelTab,' ') as nvarchar(250)) as sLevelTab, cd.bHasChild, 
			CAST(CONCAT(pr.location, CAST(cd.iID_ThongTriChiTietID as nvarchar(max))) as nvarchar(max)) as location
	FROM cte as pr, #temptable1 as cd
	WHERE cd.iID_Parent = pr.iID_ThongTriChiTietID
)
SELECT cte.*, 
(case when cte.iID_Parent is null then mlns.sM else '' end) AS sM,
(case when cte.iID_Parent is null then mlns.sTM else '' end) AS sTM,
(case when cte.iID_Parent is null then mlns.sTTM else '' end) AS sTTM,
(case when cte.iID_Parent is null then mlns.sNG else '' end) AS sNG,
--(case when cte.iID_Parent is null then duan.sTenDuAn else nhathau.sTenNhaThau end) AS sNoiDung,
(case when duan.sTenDuAn is not null then duan.sTenDuAn else nhathau.sTenNhaThau end) AS sNoiDung,
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