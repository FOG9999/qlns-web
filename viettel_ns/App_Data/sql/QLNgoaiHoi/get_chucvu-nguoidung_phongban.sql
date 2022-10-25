DECLARE @sMaNguoiDung nvarchar(50) 		set @sMaNguoiDung =  ''

--#DECLARE#--


select top 1 * from NS_NguoiDung_PhongBan  as nd
		inner join NS_PhongBan as pb on nd.iID_MaPhongBan = pb.iID_MaPhongBan
	    where nd. sMaNguoiDung = @sMaNguoiDung and ChucVu is not null
