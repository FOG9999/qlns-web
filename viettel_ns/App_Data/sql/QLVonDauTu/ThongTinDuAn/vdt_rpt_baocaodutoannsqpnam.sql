DECLARE @sMaDonVi nvarchar(200) SET @sMaDonVi = '13'
DECLARE @iNamKeHoach int SET @iNamKeHoach = 2019

--#DECLARE#--

DECLARE @tmpAll TABLE (
	iCT int,
	sM nvarchar(50), 
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sTenDuAn nvarchar(50),
	iLoaiDuAn int,
	bIsHangCha bit,
	fGiaTrPhanBo float, 
	iID nvarchar(50), 
	iID_Parent nvarchar(50)
)

Insert into @tmpAll
select distinct
	lct.iThuTu AS iCT,
	null as sM,
	null as sTM,
	null as sTTM,
	null as sNG,
	lct.sTenLoaiCongTrinh as sTenDuAn,
	0 as iLoaiDuAn,
	(cast(1 as bit))as bIsHangCha,
	CAST(0 AS float) as fGiaTrPhanBo,
	cast(lct.iID_LoaiCongTrinh as nvarchar(50)) AS iID,
	null AS iID_Parent
from VDT_KHV_PhanBoVon_ChiTiet ct
INNER JOIN VDT_DA_DuAn da ON da.iID_DuAnID = ct.iID_DuAnID
INNER JOIN VDT_KHV_PhanBoVon pbv ON pbv.iID_PhanBoVonID = ct.iID_PhanBoVonID
INNER JOIN NS_DonVi donvi ON pbv.iID_DonViQuanLyID = donvi.iID_Ma
INNER JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = da.iID_LoaiCongTrinhID
WHERE pbv.iNamKeHoach = @iNamKeHoach AND donvi.iID_MaDonVi = @sMaDonVi

union all

select distinct
	lct.iThuTu AS iCT,
	null as sM,
	null as sTM,
	null as sTTM,
	null as sNG,
	N'Dự án công trình chuyển tiếp' as sTenDuAn,
	1 as iLoaiDuAn,
	(cast(1 as bit))as bIsHangCha,
	CAST(0 AS float) as fGiaTrPhanBo,
	cast(concat(lct.iThuTu, lct.iID_LoaiCongTrinh) as nvarchar(50)) as iID,
	cast(lct.iID_LoaiCongTrinh as nvarchar(50))  as iID_Parent
from VDT_KHV_PhanBoVon_ChiTiet ct
INNER JOIN VDT_DA_DuAn da ON da.iID_DuAnID = ct.iID_DuAnID
INNER JOIN VDT_KHV_PhanBoVon pbv ON pbv.iID_PhanBoVonID = ct.iID_PhanBoVonID
INNER JOIN NS_DonVi donvi ON pbv.iID_DonViQuanLyID = donvi.iID_Ma
INNER JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = da.iID_LoaiCongTrinhID
WHERE pbv.iNamKeHoach = @iNamKeHoach AND donvi.iID_MaDonVi = @sMaDonVi


 SELECT * FROM
 (
 SELECT aa.iCT,
			aa.sM,
			aa.sTM,
			aa.sTTM,
			aa.sNG,
			aa.sTenDuAn,
			aa.iLoaiDuAn,
			aa.bIsHangCha,  
			ISNULL(SUM(aa.fGiaTrPhanBo),0) as fGiaTrPhanBo,
			aa.iID,
			aa.iID_Parent
 FROM (
		SELECT 
			lct.iThuTu AS iCT,
			mlns.sM,
			mlns.sTM,
			mlns.sTTM,
			mlns.sNG,
			da.sTenDuAn,
			1 as iLoaiDuAn,
			(cast(0 as bit))as bIsHangCha,
			--ISNULL(SUM(ct.fGiaTrPhanBo),0) as fGiaTrPhanBo,
			ct.fGiaTrPhanBo,
			cast(da.iID_DuAnID as nvarchar(50)) as iID,
			concat(lct.iThuTu, lct.iID_LoaiCongTrinh) as iID_Parent
		FROM VDT_KHV_PhanBoVon_ChiTiet ct
		LEFT JOIN NS_MucLucNganSach mlns ON mlns.iID_MaMucLucNganSach = ct.iID_NganhID 
		INNER JOIN VDT_DA_DuAn da ON da.iID_DuAnID = ct.iID_DuAnID
		INNER JOIN VDT_KHV_PhanBoVon pbv ON pbv.iID_PhanBoVonID = ct.iID_PhanBoVonID
		INNER JOIN NS_DonVi donvi ON pbv.iID_DonViQuanLyID = donvi.iID_Ma
		INNER JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = da.iID_LoaiCongTrinhID
		WHERE pbv.iNamKeHoach = @iNamKeHoach AND donvi.iID_MaDonVi = @sMaDonVi
		) aa 
	
		GROUP BY sM,sTM,sTTM,sNG, sTenDuAn, iLoaiDuAn, bIsHangCha, iCT, iID, iID_Parent
		union ALL SELECT *  FROM @tmpAll
 ) pbct ORDER BY	iCT, bIsHangCha desc, iLoaiDuAn