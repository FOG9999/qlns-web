
select da.iID_DuAnID,da.sTenDuAn,
(select sum(fGiaTriKeHoach)
FROM VDT_KHV_KeHoach5Nam_ChiTiet
	WHERE 
	iID_DuAnID = da.iID_DuAnID and
	iID_KeHoach5NamID IN
	(
		SELECT iID_KeHoach5NamID FROM VDT_KHV_KeHoach5Nam
		WHERE iID_DonViQuanLyID =  @iID_KeHoach5NamID ANd iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND dNgayQuyetDinh <= @dNgayLap
	)

)as fGiaTriKeHoach
from VDT_DA_DuAn da
where da.iID_DuAnID IN 
(
	SELECT iID_DuAnID 
	FROM VDT_KHV_KeHoach5Nam_ChiTiet
	WHERE iID_KeHoach5NamID IN
	(
		SELECT iID_KeHoach5NamID FROM VDT_KHV_KeHoach5Nam
		WHERE iID_DonViQuanLyID = @iID_KeHoach5NamID ANd iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND dNgayQuyetDinh <= @dNgayLap
	)
)