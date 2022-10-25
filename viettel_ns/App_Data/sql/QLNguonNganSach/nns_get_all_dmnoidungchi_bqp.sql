DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @iID_MaNguonNganSach int		set @iID_MaNguonNganSach = 0
DECLARE @iID_MaNamNganSach int			set @iID_MaNamNganSach = 0
DECLARE @sTenNoiDungChi nvarchar(500)	set @sTenNoiDungChi = ''
--#DECLARE#--

/*

Lấy danh sách danh mục nội dung chi

*/

select CAST('CF37C74B-2C4F-4879-8651-F2979B5E7E53' as uniqueidentifier) as iID_NoiDungChi, 
CONCAT(sNoiDung,N' -- còn lại') as sTenNoiDungChi, 
NEWID() as iID_PhanNguon_NDChi,
@rSoTienConLai as SoTien,
N'Còn lại' as GhiChu,
@iIdNguon as iID_Nguon,
@iIdPhanNguon as iID_PhanNguon,
CAST('1' as bit) as bLaHangCha,
NULL as iID_Cha
FROM DM_Nguon WHERE iID_Nguon = @iIdNguon
union all
SELECT dm_ndc.iID_NoiDungChi,
       dm_ndc.sTenNoiDungChi,
       nns_pnndc.iID_PhanNguon_NDChi,
       ISNULL(nns_pnndc.SoTien, 0) as SoTien,
       nns_pnndc.GhiChu,
       nns_pnndc.iID_Nguon,
       nns_pnndc.iID_PhanNguon,
       CAST('0' as bit) as bLaHangCha,
       CAST('CF37C74B-2C4F-4879-8651-F2979B5E7E53' as uniqueidentifier) as iID_Cha
FROM DM_NoiDungChi dm_ndc
LEFT JOIN NNS_PhanNguon_NDChi nns_pnndc ON nns_pnndc.iID_NoiDungChi = dm_ndc.iID_NoiDungChi
AND nns_pnndc.iID_Nguon = @iIdNguon
AND nns_pnndc.iID_PhanNguon = @iIdPhanNguon
AND nns_pnndc.iNamLamViec = @iNamLamViec
AND nns_pnndc.iID_MaNamNganSach = @iID_MaNamNganSach
AND nns_pnndc.iID_MaNguonNganSach = @iID_MaNguonNganSach
WHERE dm_ndc.bPublic = 1
AND (ISNULL(@sTenNoiDungChi, '') = '' OR sTenNoiDungChi LIKE CONCAT(N'%',@sTenNoiDungChi,N'%'))