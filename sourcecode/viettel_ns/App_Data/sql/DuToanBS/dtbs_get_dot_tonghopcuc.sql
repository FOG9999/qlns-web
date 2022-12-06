
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @username nvarchar(20)						set @username='duongdt'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--

/*

dtbs

Lấy danh sách các đợt gom của trợ lý tổng hợp
- Về logic, có thể gom nhiều đợt trong một ngày.
Nhưng thường chỉ gom một phiều một ngày.
- Ở đây đã lấy theo mã chứng từ, không lấy theo ngày gom.

*/

SELECT DISTINCT iID_MaDot, dNgayChungTu, CONVERT(nvarchar, dNgayChungTu, 103) as dDotNgay, sMoTa FROM
(
-- DOT BS cua Tro ly tong hop cuc
SELECT 
		CONVERT(nvarchar(36),iID_MaChungTu_TLTHCuc) as iID_MaDot,
		dNgayChungTu,
		sMoTa = CONVERT(nvarchar, dNgayChungTu, 103) + ' (TLTH): ' + sNoiDung,
		sNoiDung
FROM	DTBS_ChungTu_TLTHCuc
WHERE	iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND (@username is null or sID_MaNguoiDungTao=@username)
) AS T1
GROUP BY dNgayChungTu, sMoTa, iID_MaDot
ORDER BY dNgayChungTu
 