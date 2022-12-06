
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @sTen nvarchar(20)							set @sTen=Null

 
--###--


select	sGhiChu
from	QTA_GhiChu
where	(@iID_MaDonVi is null or iID_MaDonVi=@iID_MaDonVi)
		and (@sTen is null or sTen=@sTen)
		and (@username is null or sID_MaNguoiDung=@username)
		 