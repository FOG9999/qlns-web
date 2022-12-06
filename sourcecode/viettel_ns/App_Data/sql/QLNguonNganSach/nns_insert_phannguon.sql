DECLARE @sNoiDung nvarchar(250)			set @sNoiDung = ''
DECLARE @sSoChungTu nvarchar(50)		set @sSoChungTu = ''
DECLARE @dNgayChungTu datetime			set @dNgayChungTu = ''
DECLARE @sSoQuyetDinh nvarchar(50)		set @sSoQuyetDinh = ''
DECLARE @dNgayQuyetDinh datetime		set @dNgayQuyetDinh = ''
DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @iID_MaNguonNganSach int		set @iID_MaNguonNganSach = 0
DECLARE @iID_MaNamNganSach int			set @iID_MaNamNganSach = 0
DECLARE @sUserLogin nvarchar(200)		set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới nns phân nguồn

*/


INSERT INTO NNS_PhanNguon(sNoiDung, sSoChungTu, dNgayChungTu, sSoQuyetDinh, dNgayQuyetDinh, iNamLamViec, iID_MaNguonNganSach, iID_MaNamNganSach, dNgayTao, sID_MaNguoiDungTao, iIndex)
VALUES(@sNoiDung, @sSoChungTu, @dNgayChungTu, @sSoQuyetDinh, @dNgayQuyetDinh, @iNamLamViec, @iID_MaNguonNganSach, @iID_MaNamNganSach, GETDATE(), @sUserLogin, (select ISNULL(max(iIndex),0) + 1 from NNS_PhanNguon))