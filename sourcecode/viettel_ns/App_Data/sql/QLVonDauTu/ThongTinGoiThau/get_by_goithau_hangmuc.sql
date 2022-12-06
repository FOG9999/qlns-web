
select hm.iID_DuAn_HangMucID  AS iID_HangMucID, hm.sTenHangMuc,
(
	SELECT SUM
		( fTienGoiThau ) 
	FROM
		VDT_DA_GoiThau_HangMuc 
	WHERE
		iID_HangMucID = hm.iID_DuAn_HangMucID 
		AND iID_GoiThauID IN ( SELECT iID_GoiThauID FROM VDT_DA_GoiThau WHERE ( iID_GoiThauGocID = @iId )  AND dNgayLap <= @dNgayLap ) 
	) AS fTienGoiThau
from VDT_DA_DuAn_HangMuc hm
where hm.iID_DuAn_HangMucID IN 
(
	SELECT iID_HangMucID 
	FROM VDT_DA_GoiThau_HangMuc
	WHERE iID_GoiThauID IN
	(
		SELECT iID_GoiThauID FROM VDT_DA_GoiThau 
		WHERE ( iID_GoiThauGocID = @iId) AND dNgayLap <= @dNgayLap
	)
)