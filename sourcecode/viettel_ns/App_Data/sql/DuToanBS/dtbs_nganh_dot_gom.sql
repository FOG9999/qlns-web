
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan='10' 
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @sLNS nvarchar(200)							set @sLNS='104%'
declare @sNG nvarchar(200)							set @sNG='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2ECFEA63-B695-43A6-A456-E48CAB89ABAE'

--###--


declare @iID_MaChungTuGom nvarchar(200)
select @iID_MaChungTuGom=iID_MaChungTu from DTBS_ChungTu_TLTH where iID_MaChungTu_TLTH=@iID_MaChungTu


select  distinct sNG
from    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1
        and iNamLamViec=@iNamLamViec
        and iID_MaPhongBanDich=@iID_MaPhongBan
        and (sLNS like @sLNS)
        and iID_MaChungTu in (select * from F_Split(@iID_MaChungTuGom))
