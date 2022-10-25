declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='08'
declare @username nvarchar(20)						set @username=null
declare @request int								set @request=0

--#DECLARE#--/

SELECT Distinct	Id_DonVi as Id, Ten = Id_DonVi + ' - ' + dv.sTen 
FROM	DTKT_ChungTuChiTiet INNER JOIN (select * from NS_DonVi where iNamLamViec_DonVi=@nam) as dv ON dv.iID_MaDonVi = Id_DonVi 
WHERE	DTKT_ChungTuChiTiet.iTrangThai = 1
		AND DTKT_ChungTuChiTiet.NamLamViec = @nam
		AND (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
		AND (@username is null or UserCreator=@username)
		AND iLoai=1
		--AND iRequest=@request
