
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @ng	nvarchar(2000)							set @ng='30,52'
declare @username nvarchar(2000)					set @username='tranhnh'
declare @id_phongban nvarchar(2000)					set @id_phongban='10'


--###--

select distinct Ng, TenNganh from
(
select	Ng 
from	DTKT_ChungTuChiTiet
where	iTrangThai=1
		and NamLamViec=@nam
		and (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		and iLoai=1
		and (@username is null or UserCreator=@username)
		--and (@ng is null or Ng in (select * from f_split(@ng)))
		and (TuChi<>0 or HangNhap<>0 or HangMua<>0)
		and Ng in (select iID_MaNganh from NS_MucLucNganSach_Nganh where iNamLamViec=@nam AND sMaNguoiQuanLy like '%'+@username+'%')
) as t1

-- lay ten nganh
inner join 
(select iID_MaNganh, iID_MaNganh + ' - ' + sTenNganh as TenNganh from NS_MucLucNganSach_Nganh where iNamLamViec=@nam) as nganh
on t1.Ng=nganh.iID_MaNganh
 