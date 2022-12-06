declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='06' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='12'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=1

--###--/


SELECT		Id_DonVi, SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,SUBSTRING(Code,1,9) as Code4, Code,
			Nganh,
			TuChi	= case @request 
						when 0 then sum(TuChi)/@dvt
						when 1 then sum(TuChi + TangNV - GiamNV)/@dvt
						end,
			GhiChu=''
FROM		DTKT_ChungTuChiTiet
WHERE		iTrangThai = 1
			and NamLamViec = @NamLamViec
			and iLoai=1
			and (@Id_DonVi IS NULL OR Id_DonVi = @Id_DonVi)
			and (@Id_ChungTu is null or Id_ChungTu = @Id_ChungTu)
			and (@Id_PhongBan is null or Id_PhongBan in (select * from f_split(@Id_PhongBan)))
			and (@Username IS NULL OR UserCreator = @Username)
			and (left(Code, 6) not in ('1-1-07') or id_phongban='02')
			--and iRequest=1
			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec and STT <> '*')

GROUP BY	Id_DonVi, Code, Nganh, sMoTa
HAVING		((@request=0 and SUM(TuChi)<>0) or (@request=1 and SUM(TuChi + TangNV - GiamNV) <>0))
