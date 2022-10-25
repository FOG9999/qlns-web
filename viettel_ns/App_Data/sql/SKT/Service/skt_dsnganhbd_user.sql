declare @nam int									set @nam = 2020 
declare @user nvarchar(2000)						set @user='%b7%'


--###--

select		iID_MaNganh as MaNganh
			, iID_MaNganh + ' - ' + sTenNganh as TenNganh
			,  iID_MaNganhMLNS
from		NS_MucLucNganSach_Nganh 
where		iNamLamViec=@nam
			and iTrangThai = 1
			and (@user is null or sMaNguoiQuanLy like @user)
			and (@id_donvi is null or iID_MaNganh in (select * from f_split(@id_donvi)))
order by	MaNganh 