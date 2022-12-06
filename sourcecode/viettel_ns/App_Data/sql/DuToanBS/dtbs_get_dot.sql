
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--

SELECT DISTINCT CONVERT(nvarchar, dNgayChungTu, 103) as iID_MaDot,* FROM
(
-- cac dot bs cua tro ly
SELECT 	dNgayChungTu,
		sMoTa = CONVERT(nvarchar, dNgayChungTu, 103),
		iID_MaChungTu,
		sNoiDung
FROM	DTBS_ChungTu
WHERE	iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND sID_MaNguoiDungTao=@username
        AND iID_MaChungTu IN (SELECT iID_MaChungTu FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 ) 
                    
UNION
-- DOT BS cua Tro ly tong hop
SELECT 
		dNgayChungTu,
		sMoTa = CONVERT(nvarchar, dNgayChungTu, 103) + ' (TLTH):' + sNoiDung,
		iID_MaChungTu_TLTH,
		sNoiDung
FROM	DTBS_ChungTu_TLTH
WHERE	iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND sID_MaNguoiDungTao=@username
        --AND iID_MaChungTu IN (SELECT iID_MaChungTu FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1) 
                    
) AS T1
GROUP BY dNgayChungTu, sMoTa,iID_MaChungTu,sNoiDung
ORDER BY dNgayChungTu
