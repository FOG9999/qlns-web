declare @dvt int									set @dvt = 1
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='06' 
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2,4'
declare @iID_MaDonVi nvarchar(10)					set @iID_MaDonVi = '72'
declare @dMaDot datetime							set @dMaDot=GETDATE()

--#DECLARE#--

select	sLNS
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG
		, rTuChi = sum(rTien)
from	f_ns_table_chitieu_tien(@iNamLamViec,null,@iID_MaPhongBan,@iID_MaNamNganSach,@dMaDot,@dvt,null)
where	iID_MaDonVi like @iID_MaDonVi + '%'
group by sLNS
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG