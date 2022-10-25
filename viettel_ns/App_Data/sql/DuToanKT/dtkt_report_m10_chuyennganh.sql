
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban='10'
declare @sLNS		nvarchar(200)					set @sLNS = '104,109'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @id_nganh		nvarchar(200)				set @id_nganh = '51'


--###--

declare @nganhs nvarchar(200)
select @nganhs=iID_MaNganhMLNS from NS_MucLucNganSach_Nganh where iNamLamViec=@nam and iID_MaNganh=@id_nganh

select sNG, sMoTa = sNG +' - ' + sMoTa from NS_MucLucNganSach
where	iTrangThai=1
		and iNamLamViec=@nam
		and sLNS='' and sNG != ''
		and sNG in (select * from f_split(@nganhs))
order by sNG
