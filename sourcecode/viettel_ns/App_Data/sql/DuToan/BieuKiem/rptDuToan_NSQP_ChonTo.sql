declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='08'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns nvarchar(20)							set @lns='1020700'

--###--/
	
select	* 
from
(
	SELECT		SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,SUBSTRING(Code,1,9) as Code4, Code,
				Nganh,
				TuChi	= case @request 
							when 0 then sum(TuChi)/@dvt
							when 1 then sum(TuChi + TangNV - GiamNV)/@dvt
							end 
	FROM		DTKT_ChungTuChiTiet
	WHERE		iTrangThai = 1
				and NamLamViec = @NamLamViec
				and iLoai=1
				and (@Id_DonVi IS NULL OR Id_DonVi = @Id_DonVi)
				and (@Id_PhongBan is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan)))				
				and LEFT(Code, 6) not in ('1-1-07')
				and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec)
				and (Nganh='00' or (TuChi + TangNV - GiamNV) <> 0)
	GROUP BY	Code, Nganh, sMoTa
) as t1

-- lay ten nganh
inner join 
(select Code as id, XauNoiMa,XauNoiMa_x from DTKT_MucLuc where NamLamViec=@NamLamViec and iTrangThai=1) as mlns
on t1.Code=mlns.id 

order by code


select * from
(

SELECT		
			LEFT(sLNS,1) as sLNS1,
			LEFT(sLNS,3) as sLNS3,
			LEFT(sLNS,5) as sLNS5,
			sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
			iID_MaDonVi,
			rTuChi = rTuChi/@dvt
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai = 1
			and iNamLamViec = @NamLamViec
			and (@Id_DonVi IS NULL OR iID_MaDonVi in (select * from f_split(@Id_DonVi)))
			and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
			and (@lns is null or sLNS in (select * from f_split(@lns)))


) as a

left join (	select	sXauNoiMa as id, sMoTa
			from	NS_MucLucNganSach
			where	iTrangThai=1 
					and iNamLamViec=@NamLamViec) as mlns
on a.sXauNoiMa=mlns.id 

left join (select iID_MaDonVi as dv_id, sTenDonVi = iID_MaDonVi + ' - ' + sTen from ns_donvi where iTrangThai=1 and iNamLamViec_DonVi=@NamLamViec) as dv
on a.iID_MaDonVi = dv.dv_id

where rTuChi <> 0
