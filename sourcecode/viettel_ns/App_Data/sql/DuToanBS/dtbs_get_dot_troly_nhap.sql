
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @username nvarchar(20)						set @username='trolyphongbanb2'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='02' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @sLNS nvarchar(20)							set @sLNS='%109%'

--#DECLARE#--

/*

Lấy danh sách các đợt bổ sung của trợ lý phòng ban

*/

SELECT DISTINCT iID_MaChungTu as iID_MaDot, dNgayChungTu,
		CONVERT(nvarchar, dNgayChungTu, 103) + ' - ' + sMoTa as sMoTa
FROM
(
SELECT 	
		dNgayChungTu,
		sMoTa = CASE    
                WHEN sNoiDung='' then ''
                else  coalesce(sNoiDung + ';', '') END, 
		iID_MaChungTu,
		sDSLNS,
		sNoiDung

FROM	DTBS_ChungTu
WHERE	iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND iID_MaPhongBanDich=@iID_MaPhongBan
		AND sID_MaNguoiDungTao=@username
) AS T1
GROUP BY iID_MaChungTu,dNgayChungTu,sMoTa
ORDER BY dNgayChungTu
