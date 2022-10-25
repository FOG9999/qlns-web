--#DECLARE#--
/*
Lấy danh đợt nhận nguồn NS
*/

WITH orderedTree (iID_Nguon,sMaCTMT,sLoai,sKhoan,sMaNguon, iID_NguonCha,sNoiDung, depth, locations)
AS (SELECT 
           iID_Nguon,
       sMaCTMT,
       sLoai,
       sKhoan,
       sMaNguon,
           iID_NguonCha,
           sNoiDung,
           0 AS depth,
           CAST(ROW_NUMBER() OVER (ORDER BY sMaNguon) AS NVARCHAR(MAX)) AS locations
    FROM DM_Nguon
    WHERE iID_NguonCha IS NULL AND bPublic = 1
    UNION ALL
    SELECT
      child.iID_Nguon,
      child.sMaCTMT,
      child.sLoai,
      child.sKhoan,
       child.sMaNguon,
           child.iID_NguonCha,
           child.sNoiDung,
           parent.depth + 1 as depth,
           CAST(CONCAT(parent.locations, '.', child.sMaNguon) AS NVARCHAR(MAX)) AS locations
    FROM DM_Nguon child
        INNER JOIN orderedTree parent
            ON child.iID_NguonCha = parent.iID_Nguon
      WHERE child.bPublic = 1)

SELECT  dn.iID_DotNhan,dn.sMaLoaiDuToan,dn.sTenLoaiDuToan,dn.sSoChungTu,dn.sSoQuyetDinh,dn.dNgayQuyetDinh,dn.sNoiDung,dn.iIndex,
SUM(CASE WHEN dmn.depth = 0 THEN dnct.SoTien else 0 END) as SoTien
from NNS_DotNhan dn
LEFT JOIN NNS_DotNhanChiTiet dnct on dn.iID_DotNhan = dnct.iID_DotNhan
left join orderedTree dmn ON dmn.iID_Nguon = dnct.iID_Nguon
WHERE 
(ISNULL(@sSoChungTu, '') = '' OR dn.sSoChungTu LIKE CONCAT(N'%',@sSoChungTu,N'%')) 
	AND (ISNULL(@sNoiDung, '') = '' OR dn.sNoiDung LIKE CONCAT(N'%',@sNoiDung,N'%'))
	AND (@sMaLoaiDuToan = '00000000-0000-0000-0000-000000000000' OR dn.sMaLoaiDuToan LIKE CONCAT(N'%',@sMaLoaiDuToan,N'%'))
GROUP BY dn.iID_DotNhan,dn.sMaLoaiDuToan,dn.sTenLoaiDuToan,dn.sSoChungTu,dn.sSoQuyetDinh,dn.dNgayQuyetDinh,dn.sNoiDung	,dn.iIndex
order by sSoChungTu