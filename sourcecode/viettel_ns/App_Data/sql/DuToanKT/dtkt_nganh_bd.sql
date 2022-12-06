
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @user nvarchar(2000)						set @user='namhh'
declare @username nvarchar(2000)					set @username='%namhh%'
declare @id_phongban_dich nvarchar(2000)			set @id_phongban_dich='10'
declare @irequest int								set @irequest = 0


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
						and (TuChi <> 0 or HangNhap <> 0 or HangMua <> 0 or DacThu <> 0 or DacThu_HN <> 0 or DacThu_HM <> 0)
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
 