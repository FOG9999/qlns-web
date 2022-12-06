
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @nganh	nvarchar(2000)						set @nganh='01'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=1


--###--
select	distinct Id_DonVi 
from DTKT_ChungTuChiTiet
where	iTrangThai = 1
		and NamLamViec = @nam
		and iLoai=1
		--and iRequest=0
		and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
		and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
		and (@Nganh is null or ((@Nganh = '00' and LEFT(Code, 6) in ('1-1-01', '1-1-02', '1-1-03', '1-1-07')) or (@Nganh <> '00' and Nganh<>'00')))
		and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
		--and (TuChi<>0 or DacThu<>0)
