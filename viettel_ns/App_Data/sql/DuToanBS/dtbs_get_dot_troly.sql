
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @sLNS nvarchar(20)							set @sLNS='%109%'

--#DECLARE#--

/*

Lấy danh sách các đợt bổ sung của trợ lý phòng ban

*/

SELECT DISTINCT CONVERT(nvarchar, dNgayChungTu, 103) as iID_MaDot, dNgayChungTu,
		--	, 
		--CONVERT(nvarchar, dNgayChungTu, 103) +' ' + 
		--		CASE    
  --              WHEN sMoTa='' then ''
  --              else '  -' + sMoTa END as sMoTa 

		CONVERT(nvarchar, dNgayChungTu, 103) as sMoTa
FROM
(
SELECT 	
		dNgayChungTu,
		--sMoTa = CONVERT(nvarchar, dNgayChungTu, 103),
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
GROUP BY dNgayChungTu,sMoTa
ORDER BY dNgayChungTu
