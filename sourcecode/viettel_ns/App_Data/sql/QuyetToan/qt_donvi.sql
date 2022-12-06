
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='17'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=NULL
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'

 
--###--

select distinct iID_MaDonVi, sTenDonVi
from
(
select distinct iID_MaDonVi
from			QTA_ChungTuChiTiet 
where			iTrangThai=1 
				and iNamLamViec=@iNamLamViec
				and iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
union all

SELECT  distinct iID_MaDonVi
from	DTBS_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec+1
		and iID_MaNamNganSach in ('2','4')
		and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
		and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		and MaLoai in ('')
		and iBKhac=0
) ds

-- lay ten donvi
left join (select iID_MaDonVi as id, iID_MaDonVi + ' - ' + sTen as sTenDonVi from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec ) as dv
on dv.id=ds.iID_MaDonVi

where sTenDonVi is not null
order by iID_MaDonVi