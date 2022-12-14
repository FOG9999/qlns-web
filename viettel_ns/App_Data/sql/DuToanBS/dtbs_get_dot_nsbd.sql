
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='tranhnh'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @sLNS nvarchar(20)							set @sLNS='%109%'

--#DECLARE#--

/*

Lấy danh sách các đợt bổ sung của trợ lý phòng ban

*/

--SELECT DISTINCT CONVERT(nvarchar, dNgayChungTu, 103) as iID_MaDot, dNgayChungTu, sMoTa FROM
--(
--SELECT 	
--		dNgayChungTu,
--		sMoTa = CONVERT(nvarchar, dNgayChungTu, 103) + ' (' + sDSLNS + '): ' + sNoiDung,
--		iID_MaChungTu,
--		sDSLNS,
--		sNoiDung

--FROM	DTBS_ChungTu
--WHERE	iTrangThai=1 
--		AND iNamLamViec=@iNamLamViec
--		AND iID_MaPhongBanDich=@iID_MaPhongBan
--		AND sID_MaNguoiDungTao=@username
--		AND (sDSLNS LIKE '104%' OR sDSLNS LIKE '109%')

--   --     AND iID_MaChungTu IN (
--			--SELECT	iID_MaChungTu 
--			--FROM	DTBS_ChungTuChiTiet
--			--WHERE	iTrangThai=1 
--			--		AND (@sLNS is null or sLNS LIKE @sLNS)
--			--) 
                    
--) AS T1
--GROUP BY dNgayChungTu,sMoTa
--ORDER BY dNgayChungTu


--SELECT 	
--		dNgayChungTu,
--		--sMoTa = CONVERT(nvarchar, dNgayChungTu, 103),
--		sMoTa = CASE    
--                WHEN sNoiDung='' then ''
--                else  coalesce(sNoiDung + ';', '') END, 
--		iID_MaChungTu,
--		sDSLNS,
--		sNoiDung

--FROM	DTBS_ChungTu
--WHERE	iTrangThai=1 
--		AND iNamLamViec=@iNamLamViec
--		AND iID_MaPhongBanDich=@iID_MaPhongBan
--		AND sID_MaNguoiDungTao=@username
--		AND (sDSLNS LIKE '104%' OR sDSLNS LIKE '109%')



SELECT DISTINCT CONVERT(nvarchar, dNgayChungTu, 103) as iID_MaDot, dNgayChungTu 
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
		AND (sDSLNS LIKE '104%' OR sDSLNS LIKE '109%')

) AS T1
GROUP BY dNgayChungTu
ORDER BY dNgayChungTu
