

declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @id_donvi nvarchar(20)						set @id_donvi=null
declare @id_phongban nvarchar(20)					set @id_phongban='10'
declare @nganh nvarchar(20)							set @nganh='60'



--###--


-- TU CHI TAI NGANH DAU NAM

--SELECT	sL,sK,sM,sTM,sTTM,sNG,
--        --sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
--		--sNG,
--		rTuChi,
--		rHangNhap,
--		rHangMua,
--		rPhanCap,
--		rDuPhong
--FROM    DT_ChungTuChiTiet 
--WHERE   
--        iTrangThai=1
--        AND iNamLamViec=@nam
--        AND iID_MaNamNganSach=2 
--        AND iID_MaNguonNganSach=1
--		--AND iKyThuat=0
--        AND (MaLoai in ('','2'))
--		--AND (LEFT(sLNS,3) in (104))
--		AND sLNS='1040100'
--		AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
--        AND (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		AND (@nganh is null or sNG in (select * from f_split(@nganh)))
		 

---- dau nam: phan cap
--select sL,sK,sM,sTM,sTTM,sNG, 

--	rTuChi = sum(rTuChi), 
--	rHangNhap = sum(rHangNhap), 
--	rHangMua = sum(rHangMua) 
--from
--(
--SELECT	
--		sL,sK,sM,sTM,sTTM,sNG,
--        --sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
--		--sNG,
--		rTuChi,
--		rHangNhap,
--		rHangMua,
--		rPhanCap,
--		rDuPhong
--FROM    DT_ChungTuChiTiet_PhanCap
--WHERE   
--        iTrangThai=1
--        AND iNamLamViec=@nam
--        AND iID_MaNamNganSach=2 
--        AND iID_MaNguonNganSach=1
--		--AND iKyThuat=0
--        AND (MaLoai in ('','2'))
--		--AND (LEFT(sLNS,3) in (104))
--		--AND sLNS='1040100'
--		AND sLNS like '102%'
--		AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
--        AND (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		AND (@nganh is null or sNG in (select * from f_split(@nganh)))

--) as a
--group by sL,sK,sM,sTM,sTTM,sNG
--order by sL,sK,sM,sTM,sTTM,sNG


-- so phan cap nganh ky thuat lan 2

---- dau nam: phan cap
select sL,sK,sM,sTM,sTTM,sNG, 

	rTuChi = sum(rTuChi), 
	rHangNhap = sum(rHangNhap), 
	rHangMua = sum(rHangMua) 
from
(
SELECT	
		sL,sK,sM,sTM,sTTM,sNG,
        --sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
		--sNG,
		rTuChi,
		rHangNhap,
		rHangMua,
		rPhanCap,
		rDuPhong
FROM    DT_ChungTuChiTiet_PhanCap
WHERE   
        iTrangThai=1
        AND iNamLamViec=@nam
        AND iID_MaNamNganSach=2 
        AND iID_MaNguonNganSach=1
		--AND iKyThuat=0
        AND (MaLoai in ('','2'))
		--AND (LEFT(sLNS,3) in (104))
		--AND sLNS='1040100'
		AND sLNS like '104%'
		--AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
		--and iID_MaDonVi='99'
        AND (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		AND (@nganh is null or sNG in (select * from f_split(@nganh)))

) as a
group by sL,sK,sM,sTM,sTTM,sNG
order by sL,sK,sM,sTM,sTTM,sNG
