--#DECLARE#--

/*

lấy các đợt nhận trong năm

*/

select  dnct.iID_Nguon,dnct.SoTien,dn.dNgayQuyetDinh ,
(ROW_NUMBER() OVER(ORDER BY dn.dNgayQuyetDinh ASC) +7) AS SoCot
from NNS_DotNhan dn
left join (
	SELECT dnct.iID_Nguon, dn.iID_DotNhan, dnct.SoTien,dn.dNgayQuyetDinh
	FROM NNS_DotNhan as dn
	INNER JOIN NNS_DotNhanChiTiet as dnct on dn.iID_DotNhan = dnct.iID_DotNhan AND dnct.iID_Nguon = @iNguonNganSach
	WHERE dn.iNamLamViec = @iNamLamViec 
		AND (@dDateFrom IS NULL OR @dDateFrom <= dn.dNgayQuyetDinh)
		AND (@dDateTo IS NULL OR @dDateTo >= dn.dNgayQuyetDinh)

) 
dnct ON dnct.iID_DotNhan = dn.iID_DotNhan
where dn.sMaLoaiDuToan != '001' and dn.sMaLoaiDuToan != '002' 
AND dn.iNamLamViec = @iNamLamViec 
AND (@dDateFrom IS NULL OR @dDateFrom <= dn.dNgayQuyetDinh)
		AND (@dDateTo IS NULL OR @dDateTo >= dn.dNgayQuyetDinh)

