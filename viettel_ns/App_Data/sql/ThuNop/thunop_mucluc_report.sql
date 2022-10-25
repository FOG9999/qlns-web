
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'

--#DECLARE#--

 
select		SUBSTRING(sKyHieu,1,1) as sKyHieu1
			, SUBSTRING(sKyHieu,1,3) as sKyHieu2
			, SUBSTRING(sKyHieu,1,5) as sKyHieu3
			, SUBSTRING(sKyHieu,1,7) as sKyHieu4
			, sKyHieu
			, sTT = LTRIM(RTRIM(sTT))
			, sTen = case when len(sKyHieu) = 1 then sTEN
						  else concat(space(LEN(sKyHieu) * 2), sTen)
						  end
from		TN_DanhMucLoaiHinh 
where		iTrangThai = 1
			and Len(sKyHieu) = 7
order by	sKyHieu