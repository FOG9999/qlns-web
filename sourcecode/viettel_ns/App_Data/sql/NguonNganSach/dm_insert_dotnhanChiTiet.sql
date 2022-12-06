DECLARE @iID_DotNhan guid		set @iID_DotNhan = ''
DECLARE @iID_Nguon guid		set @iID_Nguon = ''
DECLARE @SoTien decimal		
DECLARE @GhiChu nvarchar(max)		set @GhiChu = ''
DECLARE @iNamLamViec nvarchar		set @iNamLamViec = ''
DECLARE @iID_MaNguonNganSach int		set @iID_MaNguonNganSach = 0
DECLARE @iID_MaNamNganSach int		set @iID_MaNamNganSach = 0
DECLARE @sUserLogin nvarchar(200)		set @sUserLogin = ''


--#DECLARE#--

/*

Thêm mới danh mục đợt nhận nguồn ngân sách

*/


INSERT INTO NNS_DotNhanChiTiet(iID_DotNhan,iID_Nguon ,SoTien,GhiChu,iNamLamViec,iID_MaNguonNganSach,iID_MaNamNganSach,sID_MaNguoiDungTao,dNgayTao)
VALUES(@iID_DotNhan,@iID_Nguon,@SoTien,@GhiChu, @iNamLamViec,@iID_MaNguonNganSach,@iID_MaNamNganSach, @sUserLogin,GETDATE())