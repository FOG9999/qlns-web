declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username='chuctc'

--#DECLARE#--/

SELECT		Id_DonVi, Code, GhiChu, NgayChungTu, SoChungTu
FROM		DTKT_ChungTuChiTiet
join (select Id, NgayChungTu, SoChungTu from DTKT_ChungTu) as ct on DTKT_ChungTuChiTiet.Id_ChungTu=ct.Id
WHERE		iTrangThai = 1
			and NamLamViec = @NamLamViec
			and iLoai=1
			and (@Id_DonVi IS NULL OR Id_DonVi = @Id_DonVi)
			and (@Id_ChungTu is null or Id_ChungTu = @Id_ChungTu)
			and (@Id_PhongBan is null or Id_PhongBan = @Id_PhongBan)
			and (@Username IS NULL OR UserCreator = @Username)
			and GhiChu is not null
			and GhiChu != ''
			and iRequest=@request

order by NgayChungTu,SoChungTu,GhiChu
