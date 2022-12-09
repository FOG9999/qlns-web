
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[proc_get_phongban_by_idnhiemvuchi]
	@ID uniqueidentifier
AS 
BEGIN
	select pb.* from NH_KHChiTietBQP_NhiemVuChi nvc
	 left join NH_KHChiTietBQPNhiemVuChi_PhongBan nvcpb on  nvcpb.iID_KHChiTietBQP_NhiemVuChiID=nvc.ID
	left join NS_PhongBan pb on nvcpb.iID_NS_PhongBanID=pb.iID_MaPhongBan
	where nvc.ID= @ID 
END