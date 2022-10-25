declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0

--#DECLARE#--/

select * from
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
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (Id_DonVi <> 54)
				and (@Id_PhongBan is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan)))
				--and iRequest=@request
				--and  (code like '1-1-01%' or code like '1-1-02%' or code like '1-1-03%')
				--and  (code like '1-1%')
				--and LEFT(Code, 6) in ('1-1-01', '1-1-02', '1-1-03')
				and LEFT(Code, 6) not in ('1-1-07')
				and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec)
				and (Nganh='00' or (TuChi + TangNV - GiamNV) <> 0)
	GROUP BY	Code, Nganh, sMoTa
	--HAVING		 SUM(TuChi + TangNV - GiamNV) <> 0
) as t1

-- lay ten nganh
inner join 
(select Code as id, XauNoiMa,XauNoiMa_x from DTKT_MucLuc where NamLamViec=@NamLamViec and iTrangThai=1) as mlns
on t1.Code=mlns.id 

-- Bao dam doi song
--where Nganh='00' and code like '1%'

order by code
