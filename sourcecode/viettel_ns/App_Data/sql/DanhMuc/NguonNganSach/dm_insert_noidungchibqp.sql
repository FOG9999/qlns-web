DECLARE @sCode nvarchar(50)			set @sCode = ''
DECLARE @sName nvarchar(500)		set @sName = ''
DECLARE @sNote nvarchar(max)		set @sNote = ''
DECLARE @sUserLogin nvarchar(200)	set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới danh mục nội dung chi BQP

*/


INSERT INTO DM_NoiDungChi(sMaNoiDungChi ,sTenNoiDungChi , iID_Parent, sGhiChu ,dNgayTao ,sID_MaNguoiDungTao, iNamLamViec)
VALUES(@sCode, @sName, @iID_Parent, @sNote, GETDATE(), @sUserLogin, @iNamLamViec)