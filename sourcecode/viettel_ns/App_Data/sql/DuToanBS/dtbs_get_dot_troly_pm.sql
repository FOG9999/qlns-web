
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 

--#DECLARE#--

/*

Lấy danh sách các đợt bổ sung của trợ lý phòng ban

*/

SELECT DISTINCT iID_MaChungTu, CONVERT(nvarchar, dNgayChungTu, 103) + ' - ' + sNoiDung as sMoTa, dNgayChungTu
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
		AND (@id is null or iID_MaChungTu not in (select * from f_split(@id)))
) AS T1
ORDER BY dNgayChungTu DESC
