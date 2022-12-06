DECLARE @sMaKhoi nvarchar(50)			set @sMaKhoi = ''
DECLARE @sTenKhoi nvarchar(500)		set @sTenKhoi = ''
DECLARE @sNote nvarchar(max)		set @sNote = ''
DECLARE @sUserLogin nvarchar(200)	set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới danh mục 

*/


INSERT INTO DM_KhoiDonViQuanLy(sMaKhoi ,sTenKhoi ,sGhiChu ,dNgayTao ,sID_MaNguoiDungTao,iNamLamViec)
VALUES(@sMaKhoi, @sTenKhoi, @sNote, GETDATE(), @sUserLogin, @iNamLamViec)