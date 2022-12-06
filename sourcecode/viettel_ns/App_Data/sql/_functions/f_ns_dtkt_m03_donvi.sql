USE [VIETTEL_NS]
GO
/****** Object:  StoredProcedure [dbo].[ns_dtkt_m03_donvilist]    Script Date: 7/17/2018 7:21:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Minh Hiep, Pham>
-- Create date: <08.05.2017>
-- Description:	<L?y thông tin v? các don v? d? toán>
-- =============================================

ALTER procedure [dbo].[ns_dtkt_m03_donvilist]
	(
		
	@iNamLamViec int,
	@iID_MaPhongBan nvarchar(10),
	@username nvarchar(20)
	)
	AS
	
	SET NOCOUNT ON;

SELECT	DISTINCT Id_DonVi, Ten = Id_DonVi + ' - ' + NS_DonVi.sTen 
FROM	DTKT_ChungTuChiTiet INNER JOIN NS_DonVi ON NS_DonVi.iID_MaDonVi = Id_DonVi 
WHERE	DTKT_ChungTuChiTiet.iTrangThai = 1
		AND DTKT_ChungTuChiTiet.NamLamViec = @iNamLamViec
		AND NS_DonVi.iNamLamViec_DonVi = @iNamLamViec
		AND (@iID_MaPhongBan IS NULL OR Id_PhongBan = @iID_MaPhongBan)
		AND TuChi <> 0
 
