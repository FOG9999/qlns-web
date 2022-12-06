
DECLARE @sUserLogin nvarchar(200)	set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới mapping nội dung chi mục lục nhu cầu

*/


INSERT INTO NNS_NDChi_MLNhuCau(iID_NoiDungChi, iID_MLNhuCau, NamLamViec, UserCreator, DateCreated)
SELECT @iID_NoiDungChi, Id, @iNamLamViec, @sUserLogin, GETDATE() 
FROM SKT_MLNhuCau where Id in @lstIdMLNC;