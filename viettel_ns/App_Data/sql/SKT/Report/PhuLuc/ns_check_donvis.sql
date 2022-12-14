declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='12'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07'
declare @Id_PhongBan_Dich nvarchar(20)				set @Id_PhongBan_Dich=null

--#DECLARE#--

  
select distinct id_donvi
from skt_chungtu
where ((select count(*) from SKT_ChungTuChiTiet where SKT_ChungTu.Id=SKT_ChungTuChiTiet.Id_ChungTu) > 0)
	-- ko lay benh vien
	and (LEN(Id_DonVi)=2 or id_donvi in ('GS-10', 'TC-10'))

	and (@Id_PhongBan_Dich is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan_Dich)))
order by Id_DonVi