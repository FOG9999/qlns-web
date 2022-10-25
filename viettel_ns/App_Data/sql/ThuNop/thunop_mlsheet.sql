
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2016
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @sKyHieu nvarchar(1000)						set @sKyHieu=null
declare @byNg nvarchar(1000)						set @byNg='29'
declare @loai nvarchar(10)							set @loai = '1'

--#DECLARE#--

 
select		iID_MaLoaiHinh
			, iID_MaLoaiHinh_Cha
			, bLaHangCha
			, bLaTong
			, sKyHieu
			, sTT
			, sTen
from		TN_DanhMucLoaiHinh 
where		iTrangThai = 1
			--and iNamLamViec=@NamLamViec
			and (@sKyHieu is null or sKyHieu like @sKyHieu)
order by	sKyHieu