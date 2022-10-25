
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/
--select gt.*,dm.sTen as  sTenNguonVon
--from VDT_DA_GoiThau_NguonVon gt 
--inner join NS_NguonNganSach dm ON dm.iID_MaNguonNganSach = gt.iID_NguonVonID
--where iID_GoiThauID = @iId

select nv.iID_MaNguonNganSach  AS iID_NguonVonID, nv.sTen as sTenNguonVon,
(
	SELECT SUM
		( fTienGoiThau ) 
	FROM
		VDT_DA_GoiThau_NguonVon 
	WHERE
		iID_NguonVonID = nv.iID_MaNguonNganSach 
		AND iID_GoiThauID IN ( SELECT iID_GoiThauID FROM VDT_DA_GoiThau WHERE ( iID_GoiThauGocID = @iId )  AND dNgayLap <= @dNgayLap ) 
	) AS fTienGoiThau
from NS_NguonNganSach nv
where nv.iID_MaNguonNganSach IN 
(
	SELECT iID_NguonVonID 
	FROM VDT_DA_GoiThau_NguonVon
	WHERE iID_GoiThauID IN
	(
		SELECT iID_GoiThauID FROM VDT_DA_GoiThau 
		WHERE ( iID_GoiThauGocID = @iId) AND dNgayLap <= @dNgayLap
	)
)