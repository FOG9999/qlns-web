
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/





--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

SELECT
gt.sSoQuyetDinh,
gt.dNgayQuyetDinh,
gt.iID_GoiThauID,
gt.sTenGoiThau,
gt.iThoiGianThucHien,
da.sTenDuAn,
nt.sTenNhaThau,
gt.fTienTrungThau,
(SELECT SUM(fTienTrungThau) FROM VDT_DA_GoiThau WHERE iID_GoiThauGocID = gt.iID_GoiThauGocID ) AS giatriDieuChinh,
(SELECT COUNT(iID_GoiThauID) FROM VDT_DA_GoiThau WHERE bIsGoc = 0 AND iID_GoiThauGocID = gt.iID_GoiThauGocID ) AS soLanDieuChinh 

FROM VDT_DA_GoiThau gt
left join VDT_DA_DuAn da ON da.iID_DuAnID = gt.iID_DuAnID
left join VDT_DM_NhaThau nt ON nt.iID_NhaThauID = gt.iID_NhaThauID
WHERE gt.bActive = 1
AND (ISNULL(@tenGoiThau, '') = '' OR gt.sTenGoiThau LIKE CONCAT(N'%',@tenGoiThau,N'%'))
AND (ISNULL(@tenDuAn, '') = '' OR da.sTenDuAn LIKE CONCAT(N'%',@tenDuAn,N'%'))
AND(ISNULL(@giaTriMin,0) = 0 OR gt.fTienTrungThau >= @giaTriMin)
AND(ISNULL(@giaTriMax,0) = 0 OR gt.fTienTrungThau <= @giaTriMax)
ORDER BY gt.dDateCreate DESC