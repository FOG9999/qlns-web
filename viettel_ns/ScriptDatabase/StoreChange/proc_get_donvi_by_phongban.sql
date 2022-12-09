
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[proc_get_donvi_by_phongban]
	@iNamLamViec int,
	@iID_MaPhongBan uniqueidentifier
AS 
BEGIN
	select dv.* from NS_PhongBan_DonVi pbdv
	left join NS_DonVi dv on pbdv.iID_MaDonVi=dv.iID_MaDonVi and dv.iNamLamViec_DonVi=@iNamLamViec
	where pbdv.iID_MaPhongBan= @iID_MaPhongBan and pbdv.iNamLamViec=@iNamLamViec
END