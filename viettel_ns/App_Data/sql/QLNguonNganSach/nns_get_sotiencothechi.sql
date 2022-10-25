DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @iID_MaNguonNganSach int		set @iID_MaNguonNganSach = 0
DECLARE @iID_MaNamNganSach int			set @iID_MaNamNganSach = 0
DECLARE @sTenNoiDungChi nvarchar(500)	set @sTenNoiDungChi = ''
--#DECLARE#--

/*

Lấy danh sách danh mục nội dung chi

*/

SELECT ISNULL(sum(nns_pnndc.SoTien),0) + @rSoTienConLai
FROM DM_NoiDungChi dm_ndc
LEFT JOIN NNS_PhanNguon_NDChi nns_pnndc ON nns_pnndc.iID_NoiDungChi = dm_ndc.iID_NoiDungChi
AND nns_pnndc.iID_Nguon = @iIdNguon
AND nns_pnndc.iID_PhanNguon = @iIdPhanNguon
AND nns_pnndc.iNamLamViec = @iNamLamViec
AND nns_pnndc.iID_MaNamNganSach = @iID_MaNamNganSach
AND nns_pnndc.iID_MaNguonNganSach = @iID_MaNguonNganSach
WHERE dm_ndc.bPublic = 1
AND (ISNULL(@sTenNoiDungChi, '') = '' OR dm_ndc.sTenNoiDungChi LIKE CONCAT(N'%',@sTenNoiDungChi,N'%'))