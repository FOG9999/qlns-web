--#DECLARE#--

/*

danh sách đợt nhận chi tiết

*/

-- Luỹ kế Nguồn nhận từ BTC
Select iID_Nguon, ISNULL(sum(SoTien), 0) as fLuyKeNguonNhan into #TEMP_nguonnhan
from NNS_DotNhanChiTiet dnct
inner join NNS_DotNhan dn on dnct.iID_DotNhan = dn.iID_DotNhan and dn.iID_DotNhan != @iId
where dn.iNamLamViec = @iNamLamViec
and (dn.dNgayQuyetDinh <= (select dNgayQuyetDinh from NNS_DotNhan where iID_DotNhan = @iId) 
		or (dn.dNgayQuyetDinh = (select dNgayQuyetDinh from NNS_DotNhan where iID_DotNhan = @iId) and dn.iIndex < (select iIndex from NNS_DotNhan where iID_DotNhan = @iId)))
GROUP BY dnct.iID_Nguon;

-- Luỹ kế Nguồn đã chi theo NDC
Select dnct.iID_Nguon, ISNULL(sum(dnct_ndc.SoTien), 0) as fLuyKeDaPhanNDC into #TEMP_nguonchi
from NNS_DotNhanChiTiet_NDChi dnct_ndc
inner join NNS_DotNhanChiTiet dnct on dnct_ndc.iID_DotNhanChiTiet = dnct.iID_DotNhanChiTiet
INNER JOIN DM_NoiDungChi dm_ndc on dnct.iID_Nguon = dm_ndc.iID_Nguon and dm_ndc.iID_NoiDungChi = dnct_ndc.iID_NoiDungChi and dm_ndc.iTrangThai = 1
inner join NNS_DotNhan dn on dnct.iID_DotNhan = dn.iID_DotNhan and dn.iID_DotNhan != @iId
where dn.iNamLamViec = @iNamLamViec
and (dn.dNgayQuyetDinh <= (select dNgayQuyetDinh from NNS_DotNhan where iID_DotNhan = @iId) 
		or (dn.dNgayQuyetDinh = (select dNgayQuyetDinh from NNS_DotNhan where iID_DotNhan = @iId) and dn.iIndex < (select iIndex from NNS_DotNhan where iID_DotNhan = @iId)))
GROUP BY dnct.iID_Nguon;

-- Nguồn đã chi - đợt nhận này
select dnct.iID_DotNhanChiTiet, sum(dnct_ndc.SoTien) as fSoTien into #TEMP_chi_dotnhannay
from NNS_DotNhanChiTiet dnct
INNER JOIN DM_NoiDungChi dm_ndc on dnct.iID_Nguon = dm_ndc.iID_Nguon and dm_ndc.iTrangThai = 1
INNER JOIN NNS_DotNhanChiTiet_NDChi dnct_ndc on dnct.iID_DotNhanChiTiet = dnct_ndc.iID_DotNhanChiTiet and dm_ndc.iID_NoiDungChi = dnct_ndc.iID_NoiDungChi
where dnct.iID_DotNhan = @iId
GROUP BY dnct.iID_DotNhanChiTiet;

-- Lấy danh sách nguồn - chi tiết đợt nhận
SELECT dmn.*,
			dnct.GhiChu,
			dnct.iID_DotNhanChiTiet,
			dnct.iID_DotNhan,
			
			ISNULL(nguonnhan.fLuyKeNguonNhan, 0) as fLuyKeNguonNhan,
			ISNULL(dnct.SoTien, 0) as SoTien, 
			ISNULL(nguonnhan.fLuyKeNguonNhan, 0) + ISNULL(dnct.SoTien, 0) as fTongNguonNhan,
			
			ISNULL(nguonchi.fLuyKeDaPhanNDC, 0) as fLuyKeDaPhanNDC,
			ISNULL(tblDaPhan.fSoTien, 0) as SoTienDaPhanNDC,
			ISNULL(nguonchi.fLuyKeDaPhanNDC, 0) + ISNULL(tblDaPhan.fSoTien, 0) as fTongDaPhan,
			
			ISNULL(nguonnhan.fLuyKeNguonNhan, 0) + ISNULL(dnct.SoTien, 0) - ISNULL(nguonchi.fLuyKeDaPhanNDC, 0) - ISNULL(tblDaPhan.fSoTien, 0) as SoTienConLai,
			
			(select sMaLoaiDuToan from NNS_DotNhan where iID_DotNhan = @iId) as sMaLoaiDuToan,
			CAST((case when tbl1.iID_Nguon is not null then '0' else '1' end) as bit) as bLaHangCha 
			
FROM DM_Nguon dmn
LEFT JOIN NNS_DotNhanChiTiet dnct ON dmn.iID_Nguon = dnct.iID_Nguon AND dnct.iID_DotNhan = @iId
LEFT JOIN #TEMP_chi_dotnhannay tblDaPhan on dnct.iID_DotNhanChiTiet = tblDaPhan.iID_DotNhanChiTiet
LEFT JOIN #TEMP_nguonnhan nguonnhan on dmn.iID_Nguon = nguonnhan.iID_Nguon
LEFT JOIN #TEMP_nguonchi nguonchi on dmn.iID_Nguon = nguonchi.iID_Nguon
left join 
(
  select iID_Nguon from DM_Nguon where iID_Nguon not in
  (
    select distinct iID_NguonCha from DM_Nguon where iTrangThai = 1 and iID_NguonCha is not null
  ) and iTrangThai = 1
) tbl1 on tbl1.iID_Nguon = dmn.iID_Nguon
WHERE dmn.iTrangThai = 1 and dmn.iNamLamViec = @iNamLamViec
    AND (ISNULL(@maCTMT, '') = '' OR dmn.sMaCTMT LIKE CONCAT(N'%',@maCTMT,N'%')) 
	AND (ISNULL(@loai, '') = '' OR dmn.sLoai LIKE CONCAT(N'%',@loai,N'%'))
	AND (ISNULL(@khoan, '') = '' OR dmn.sKhoan LIKE CONCAT(N'%',@khoan,N'%'))
	AND (ISNULL(@noiDung, '') = '' OR dmn.sNoiDung LIKE CONCAT(N'%',@noiDung,N'%'))
ORDER BY cast('/' + replace(dmn.iSTT, '.', '/') + '/' as hierarchyid)

DROP TABLE #TEMP_nguonnhan;
DROP TABLE #TEMP_nguonchi;
DROP TABLE #TEMP_chi_dotnhannay;


	