declare @iNamLamViec int
set @iNamLamViec = 2020

SELECT * FROM CP_CapPhat WHERE iTrangThai=1 AND iID_MaDonVi is not null AND ( 0=1  OR iID_MaTrangThaiDuyet=13 OR iID_MaTrangThaiDuyet=14 OR iID_MaTrangThaiDuyet=17 OR iID_MaTrangThaiDuyet=20 OR iID_MaTrangThaiDuyet=23 OR iID_MaTrangThaiDuyet=24 OR iID_MaTrangThaiDuyet=30 OR iID_MaTrangThaiDuyet=61) AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=@iID_MaNamNganSach AND iID_MaNguonNganSach=@iID_MaNguonNganSach AND iLoai = @iLoai AND sDSLNS IS NOT NULL AND sID_MaNguoiDungTao = @sID_MaNguoiDungTao