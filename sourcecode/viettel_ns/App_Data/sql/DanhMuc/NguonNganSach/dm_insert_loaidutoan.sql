DECLARE @sCode nvarchar(50)			set @sCode = ''
DECLARE @sName nvarchar(500)		set @sName = ''
DECLARE @sNote nvarchar(max)		set @sNote = ''
DECLARE @sUserLogin nvarchar(200)	set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới danh mục loại dự toán

*/


INSERT INTO DM_LoaiDuToan(sMaLoaiDuToan ,sTenLoaiDuToan ,sGhiChu ,dNgayTao ,sID_MaNguoiDungTao, iNamLamViec)
VALUES(@sCode, @sName, @sNote, GETDATE(), @sUserLogin, @iNamLamViec)