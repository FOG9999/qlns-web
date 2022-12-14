-- SET ANSI_NULLS ON
-- GO
-- SET QUOTED_IDENTIFIER ON
-- GO
-- CREATE procedure [dbo].[proc_get_donvi_by_phongban]
	-- @iNamLamViec int,
	-- @iID_MaPhongBan uniqueidentifier
-- AS 
-- BEGIN
	-- select dv.* from NS_PhongBan_DonVi pbdv
	-- left join NS_DonVi dv on pbdv.iID_MaDonVi=dv.iID_MaDonVi and dv.iNamLamViec_DonVi=@iNamLamViec
	-- where pbdv.iID_MaPhongBan= @iID_MaPhongBan and pbdv.iNamLamViec=@iNamLamViec
-- END

ALTER procedure [dbo].[proc_get_donvi_by_phongban]
	@iNamLamViec int,
	@iID_MaPhongBan uniqueidentifier
AS 
BEGIN
	SELECT dv.* FROM NS_DonVi dv
	LEFT JOIN NS_PhongBan_DonVi pbdv ON pbdv.iID_MaDonVi=dv.iID_MaDonVi AND dv.iNamLamViec_DonVi=@iNamLamViec
	WHERE pbdv.iID_MaPhongBan= @iID_MaPhongBan AND pbdv.iNamLamViec=@iNamLamViec
END