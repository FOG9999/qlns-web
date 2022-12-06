USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[ns_nguoidung_donvi_edit]    Script Date: 11/10/2022 3:24:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ns_nguoidung_donvi_edit]
	@nam int,
	@sMaNguoiDung nvarchar(20),
	@iID_MaDonVi nvarchar(6)	
AS
BEGIN 
	
	select	nd_dv.iID_MaNguoiDungDonVi, dv.id, dv.Ten, bCon = case when iID_MaNguoiDungDonVi is null or iTrangThai = 0 then CAST(0 as bit) else CAST(1 as bit) end
	from 
			(
			select	iID_MaNguoiDungDonVi
					, sMaNguoiDung
					, iID_MaDonVi
					, iTrangThai
			from	NS_NguoiDung_DonVi 
			where	iNamLamViec = @nam
					and sMaNguoiDung = @sMaNguoiDung
					and (@iID_MaDonVi is null or iID_MaDonVi like @iID_MaDonVi)) nd_dv			

			right join

			(
			select	iID_MaDonVi as pb_id
			from	NS_PhongBan_DonVi
			where	iNamLamViec = @nam
					and iID_MaPhongBan in (select iID_MaPhongBan from NS_NguoiDung_PhongBan where sMaNguoiDung = @sMaNguoiDung and iTrangThai = 1)
					and (@iID_MaDonVi is null or iID_MaDonVi like @iID_MaDonVi)) pb_dv	
							
			on nd_dv.iID_MaDonVi = pb_dv.pb_id

			left join 

			(
			select	iID_MaDonVi as id
					, iID_MaDonVi + ' - ' + sTen as Ten
			from	NS_DonVi
			where	iTrangThai = 1 
					and iNamLamViec_DonVi = @nam
					and (@iID_MaDonVi is null or iID_MaDonVi like @iID_MaDonVi)) dv

			on pb_dv.pb_id = dv.id
			
	where	Ten is not null
	order by id

END

GO

