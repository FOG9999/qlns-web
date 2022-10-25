declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan=null
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null

--###--/


select * from
(

SELECT		
			--Id_DonVi, SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,SUBSTRING(Code,1,9) as Code4, Code,
			sNG,
			rTuChi	= sum(rTuChi)/@dvt
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai = 1
			and iNamLamViec = @NamLamViec
			and (@Id_DonVi IS NULL OR iID_MaDonVi = @Id_DonVi)
			and (@Id_PhongBan is null or iID_MaPhongBan in (select * from f_split(@Id_PhongBan)))
			and sNG<>'00'
			and left(sLNS,3) not in (104)
			and left(sLNS,1) in (1,2)
GROUP BY	sNG
HAVING		SUM(rTuChi) <>0


) as a

left join (	select	sXauNoiMa as id, sMoTa = sXauNoiMa + ' - ' + sMoTa 
			from	NS_MucLucNganSach
			where	iTrangThai=1 
					and iNamLamViec=@NamLamViec
					and sL='') as mlns
on a.sNG=mlns.id

--where sNG = '37'
ORDER BY sNG
