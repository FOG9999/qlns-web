declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='97'
declare @iThang_Quy nvarchar(20)					set @iThang_Quy='1'

--#DECLARE#--/

SELECT		bLaHangCha
			, iID_MaLoaiHinh
			, iID_MaLoaiHinh_Cha
			, sKyHieu 
			, sTen as sMoTa
			, bLaTong

FROM		TN_DanhMucLoaiHinh WHERE iTrangThai=1 
ORDER BY	sKyhieu,iSTT