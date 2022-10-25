declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='99'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null

--###--/
select * from

(

SELECT		distinct
			iID_MaDonVi 
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai = 1
			and iNamLamViec = @NamLamViec
			and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
) as a


join (	select  iID_MaDonVi as dv_id,	sMoTa = iID_MaDonVi + ' - ' + sTen 
			from	NS_DonVi
			where	iTrangThai=1 
					and iNamLamViec_DonVi=@NamLamViec) as dv
on a.iID_MaDonVi=dv.dv_id
ORDER BY iID_MaDonVi
