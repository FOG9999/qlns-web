SELECT 
	hd.iID_HopDongID as IIdHopDongId,
	hdnt.iID_GoiThauID as IIdGoiThauId,
	hdnt.iID_NhaThauID as IIdNhaThauId,
	hd.sSoHopDong as SSoHopDong,
	nt.sTenNhaThau as STenNhaThau,
	hd.sHinhThucHopDong as SHinhThucHopDong,
	hdnt.fGiaTriHD as FGiaTri,
	hd.dNgayHopDong as DNgayHopDong
	FROM
		VDT_DA_HopDong_GoiThau_NhaThau hdnt	
		LEFT JOIN VDT_DA_TT_HopDong hd ON hd.iID_HopDongID = hdnt.iID_HopDongID
		LEFT JOIN VDT_DM_NhaThau nt ON nt.iID_NhaThauID = hdnt.iID_NhaThauID 
	WHERE
		 hdnt.iID_GoiThauID = @goiThauId
		AND hd.iID_HopDongID  is not null