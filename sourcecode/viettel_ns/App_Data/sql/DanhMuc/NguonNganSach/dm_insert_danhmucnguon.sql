DECLARE @sMaCTMT nvarchar(50)			set @sMaCTMT = ''
DECLARE @sMaNguon nvarchar(500)		set @sMaNguon = ''
DECLARE @sLoai nvarchar(500)		set @sLoai = ''
DECLARE @sKhoan nvarchar(500)		set @sKhoan = ''
DECLARE @sNoiDung nvarchar(500)		set @sNoiDung = ''
DECLARE @iID_NguonCha guid		set @iID_NguonCha = ''
DECLARE @sUserLogin nvarchar(200)	set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới danh mục nội dung chi BQP

*/

BEGIN
IF(@iID_NguonCha = '00000000-0000-0000-0000-000000000000')
	BEGIN
		INSERT INTO DM_Nguon(sMaCTMT ,sMaNguon ,sLoai,sKhoan, sNoiDung,dNgayTao ,sID_MaNguoiDungTao)
		VALUES(@sMaCTMT, @sMaNguon,@sLoai,@sKhoan,@sNoiDung, GETDATE(), @sUserLogin)
	END
	ELSE
	BEGIN
		INSERT INTO DM_Nguon(sMaCTMT ,sMaNguon ,sLoai,sKhoan, sNoiDung,iID_NguonCha,dNgayTao ,sID_MaNguoiDungTao,iNamLamViec)
		VALUES(@sMaCTMT, @sMaNguon,@sLoai,@sKhoan,@sNoiDung,@iID_NguonCha, GETDATE(), @sUserLogin,@iNamLamViec)
	END

END


	
