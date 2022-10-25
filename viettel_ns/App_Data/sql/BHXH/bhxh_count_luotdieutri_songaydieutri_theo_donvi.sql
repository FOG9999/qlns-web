declare @iNamLamViec int set @iNamLamViec = 2019
declare @lstDonViBHXH nvarchar(MAX) set @lstDonViBHXH = '1,2,3,4,5'
--#DECLARE#--
select dv.iID_BHXH_DonViID, bn.iID_MaDonVi, bn.iThang, count(bn.iID_MaDonVi) AS iLuotDieuTri, sum(bn.iSoNgayDieuTri) as iSoNgayDieuTri --INTO #tmpTongHop
from BHXH_BenhNhan bn
INNER JOIN BHXH_DonVi dv ON dv.iID_MaDonViBHXH = bn.iID_MaDonVi
WHERE bn.iNamLamViec = @iNamLamViec
AND bn.iID_MaDonVi IN (select * FROM splitstring(@lstDonViBHXH))
GROUP BY dv.iID_BHXH_DonViID, bn.iID_MaDonVi, bn.iThang