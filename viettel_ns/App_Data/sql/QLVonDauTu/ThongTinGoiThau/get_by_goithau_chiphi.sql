--#DECLARE#--
/*
Lấy danh sách chi phí của gói thầu 
*/


select dm_cp.iID_ChiPhi as iID_ChiPhiID, dm_cp.sTenChiPhi,
(
	SELECT SUM
		( fTienGoiThau ) 
	FROM
		VDT_DA_GoiThau_ChiPhi 
	WHERE
		iID_ChiPhiID = dm_cp.iID_ChiPhi 
		AND iID_GoiThauID IN ( SELECT iID_GoiThauID FROM VDT_DA_GoiThau WHERE ( iID_GoiThauGocID = @iId ) AND dNgayLap <= @dNgayLap ) 
	) AS fTienGoiThau
from VDT_DM_ChiPhi dm_cp
where dm_cp.iID_ChiPhi IN 
(
	SELECT iID_ChiPhiID 
	FROM VDT_DA_GoiThau_ChiPhi
	WHERE iID_GoiThauID IN
	(
		SELECT iID_GoiThauID FROM VDT_DA_GoiThau 
		WHERE ( iID_GoiThauGocID = @iId) AND dNgayLap <= @dNgayLap
	)
)
