DECLARE @sMaChiPhi nvarchar(50)		set @sMaChiPhi = ''
DECLARE @sTenVietTat nvarchar(100)		set @sTenVietTat = ''
DECLARE @sTenChiPhi nvarchar(300)		set @sTenChiPhi = ''
DECLARE @sMoTa nvarchar(250)		 	set @sMoTa = ''
DECLARE @iThuTu int						set @iThuTu = 0
DECLARE @sUserLogin nvarchar(200)		set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới Loại chi phí

*/


INSERT INTO VDT_DM_ChiPhi(sMaChiPhi, sTenVietTat, sTenChiPhi, sMoTa, iThuTu, dNgayTao, sID_MaNguoiDungTao)
VALUES(@sMaChiPhi, @sTenVietTat, @sTenChiPhi, @sMoTa, @iThuTu, GETDATE(), @sUserLogin)