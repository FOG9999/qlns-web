declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

IF NOT EXISTS	(
				SELECT	sGhiChu 
                FROM	CP_GhiChu 
                WHERE	sTen = @sTen 
						AND sID_MaNguoiDung = @username 
						AND iID_MaDonVi = @iID_MaDonVi
				)

				INSERT INTO CP_GhiChu(sTen, sID_MaNguoiDung, iID_MaDonVi, sGhiChu) 
                VALUES		(@sTen, @username, @iID_MaDonVi, @sGhiChu)

ELSE 

                UPDATE	CP_GhiChu 
                SET		sGhiChu=@sGhiChu 
                WHERE	sTen = @sTen 
						AND  sID_MaNguoiDung = @username 
						AND iID_MaDonVi =@iID_MaDonVi