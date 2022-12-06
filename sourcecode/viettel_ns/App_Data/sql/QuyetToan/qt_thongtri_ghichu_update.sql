
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @sTen nvarchar(20)							set @sTen=Null

 
--###--


IF NOT EXISTS(
	SELECT sGhiChu 
	FROM QTA_GhiChu 
	WHERE sTen = @sTen AND sID_MaNguoiDung = @username AND (@iID_MaDonVi is null or iID_MaDonVi = @iID_MaDonVi)
)
	INSERT INTO QTA_GhiChu(sTen, sID_MaNguoiDung, iID_MaDonVi, sGhiChu) 
		VALUES(@sTen, @username, @iID_MaDonVi, @sGhiChu)
ELSE 
	UPDATE QTA_GhiChu 
	SET sGhiChu=@sGhiChu 
	WHERE  sTen = @sTen AND	 sID_MaNguoiDung = @username AND iID_MaDonVi =@iID_MaDonVi
