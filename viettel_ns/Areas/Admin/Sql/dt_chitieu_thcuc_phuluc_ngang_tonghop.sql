
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021

--#DECLARE#--


declare @dn nvarchar(MAX)
set @dn = '30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94'

SELECT		iID_MaDonVi = Ma_Dv ,sTenDonVi=[dbo].F_TenDonVi(@iNamLamviec, Ma_Dv), iID_MaPhongBan = iID_MaPhongBanDich, SX=iID_MaPhongBanDich + '-'+Ma_Dv,
			C1      =SUM(C1)/@dvt,
			C2      =SUM(C2)/@dvt,
			C3      =SUM(C3)/@dvt,
			C4      =SUM(C4)/@dvt,
			C5      =SUM(C5)/@dvt,
			C6      =SUM(C6)/@dvt,
			C7      =SUM(C7)/@dvt,
			C8      =SUM(C8)/@dvt,
			C9      =SUM(C9)/@dvt,
			C10     =SUM(C10)/@dvt,
			C11     =SUM(C11)/@dvt
FROM
(

SELECT		Ma_Dv = LEFT(iID_MaDonVi, 2), iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG,
			C1      = case when sLNS = '1010000' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C2      = case when ((sLNS like '1020%' and sLNS not in ('1020200','1020900')) or sLNS = '1050100') and sNG = '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C3      = case when ((sLNS like '1020%' and sLNS not in ('1020200','1020900')) or sLNS = '1050100') and sNG <> '00' and LEN(iID_MaDonVi) = 2 then sum(rTuChi) else 0 end,
			C4      = case when sLNS like '104%' then sum(rHangNhap) else 0 end,
			C5      = case when sLNS like '104%' then sum(rHangMua) else 0 end,
			C6      = case when sLNS like '104%' then sum(rPhanCap) else 0 end,
			C7      = case when sLNS like '103%' then sum(rTuChi) else 0 end,
			C8      = case when ((LEN(iID_MaDonVi) > 2 and sLNS like '1%') or sLNS = '1020900') then sum(rTuChi) else 0 end,
			C9      = case when (sLNS like '109%' or sLNS like '105%' or sLNS = '1020200') and LEN(iID_MaDonVi) = 2 then sum(rTuChi+rHangNhap) else 0 end,
			C10     = case when sLNS like '2%' then sum(rTuChi) else 0 end,
			C11     = case when sLNS like '4%' then sum(rTuChi) else 0 end
FROM		DT_ChungTuChiTiet 
WHERE		iTrangThai in (select * from f_split(@loai))
			AND iNamLamViec=@iNamLamViec
			AND MaLoai = ''

GROUP BY	iID_MaDonVi, iID_MaPhongBanDich, sLNS, sL, sK, sM, sTM, sTTM, sNG
HAVING		SUM(rTuChi)<>0 
			or SUM(rHangNhap) <> 0
			or SUM(rHangMua) <> 0
			or SUM(rPhanCap) <> 0

) as T1

left join 

(
select		iID_MaDonVi as id, iSTT
from		NS_DonVi
where		iTrangThai = 1 and iNamLamViec_DonVi = @iNamLamViec
) as dv

on			T1.Ma_Dv = dv.id
GROUP BY	Ma_Dv,iSTT, iID_MaPhongBanDich
HAVING		sum(C1+C2+C3+C4+C5+C6+C7+C8+C9+C10+C11) <> 0
order by	iID_MaPhongBanDich,iSTT