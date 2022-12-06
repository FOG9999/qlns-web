declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @sLNS nvarchar(20)							set @sLNS = '1040100'
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi = '10,11,12,13,14,15,17,29,31,33,42,43,47'
declare @iID_MaCapPhat nvarchar(200)				set @iID_MaCapPhat = '73b02e34-c30f-43ef-b829-af56c5261a5a'

--#DECLARE#--

SELECT DISTINCT	iID_MaDonVi,TenHT
FROM			CP_CapPhatChiTiet
inner join (select iID_MaDonVi as id, sTen as TenHT from ns_donvi where iNamLamViec_DonVi=@iNamLamViec and iTrangThai=1) as dv on dv.id=iID_MaDonVi
WHERE			iNamLamViec=@iNamLamViec 
				AND iTrangThai = 1 
				--AND sTenDonVi <> '' 
				AND iID_MaDonVi <> ''
				AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS))) 
				AND iID_MaPhongBan = @iID_MaPhongBan 
				AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
				AND iID_MaCapPhat = @iID_MaCapPhat 
				AND rTuChi <> 0
