
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @user nvarchar(2000)						set @user=null
declare @username nvarchar(2000)					set @username=null
declare @id_phongban_dich nvarchar(2000)			set @id_phongban_dich='10'
declare @irequest int								set @irequest = 1


--###--

select distinct	MaNganh
				, TenNganh 
				, iID_MaNganhMLNS
from
				(
				select	Ng
				from	DTKT_ChungTuChiTiet
				where	iTrangThai=1
						and NamLamViec=@nam
						and (@id_phongban_dich is null or Id_PhongBanDich=@id_phongban_dich)
						and iRequest = @irequest
						and iLoai=2
						and (@user is null or UserCreator=@user)
						and (TangNV <> 0 or TangNV_HN <> 0 or TangNV_HM <> 0 or GiamNV <> 0 or GiamNV_HN <> 0 or GiamNV_HM <> 0)
				) as t1

				-- lay ten nganh
				inner join 
				(
				select	MaNganh = iID_MaNganh
						, TenNganh = (iID_MaNganh + ' - ' + sTenNganh)
						, iID_MaNganhMLNS 
				from    NS_MucLucNganSach_Nganh
				where   iTrangThai=1
						and iNamLamViec=@nam
						and (@username is null or sMaNguoiQuanLy like @username)) as nganh
				on t1.Ng=nganh.MaNganh
 