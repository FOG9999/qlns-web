declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='08'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns nvarchar(20)							set @lns='1020700'

--###--/


select		*
from
(

SELECT		
			LEFT(sLNS,1) as sLNS1,
			LEFT(sLNS,3) as sLNS3,
			LEFT(sLNS,5) as sLNS5,
			sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
			iID_MaDonVi,
			rTuChi = rChiTapTrung/@dvt
			--rTuChi = case when sM = '8000' then rTuChi/@dvt else ROUND((rTuChi/10)/@dvt,-3) end
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai <> 0
			and sLNS = '1010000' and rChiTapTrung <> 0
			and iNamLamViec = @nam
			and LEN(iID_MaDonVi) = 2
			and (@id_donvi IS NULL OR iID_MaDonVi in (select * from f_split(@id_donvi)))
			and (@id_phongban is null or iID_MaPhongBanDich = @id_phongban)


) as a

left join (	select	sXauNoiMa as id, sMoTa
			from	NS_MucLucNganSach
			where	iTrangThai=1 
					and iNamLamViec=@nam) as mlns
on a.sXauNoiMa=mlns.id 

left join (select iID_MaDonVi as dv_id, sTenDonVi = iID_MaDonVi + ' - ' + sTen from ns_donvi where iTrangThai=1 and iNamLamViec_DonVi=@nam) as dv
on a.iID_MaDonVi = dv.dv_id

where rTuChi <> 0
