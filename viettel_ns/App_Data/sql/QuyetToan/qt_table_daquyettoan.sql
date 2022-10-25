
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach  nvarchar(20)			set @iID_MaNamNganSach = '2'
declare @iID_MaDonVi		nvarchar(2)				set @iID_MaDonVi='44'
declare @iID_MaChungTu		nvarchar(2000)			set @iID_MaChungTu='0daa0d84-a173-4640-81a6-3b309a80f765'
--declare @sLNS		nvarchar(200)					set @sLNS='2'
declare @dNgayChungTu datetime						set @dNgayChungTu = GETDATE()
declare @iThang_Quy		int							set @iThang_Quy=3
declare @iSoChungTu int								set @iSoChungTu = 6622

declare @sLNS1		nvarchar(200)					set @sLNS1='1'
declare @sLNS		nvarchar(7)						set @sLNS=NULL
declare @sL			nvarchar(3)						set @sL=NULL
declare @sK			nvarchar(3)						set @sK=NULL
declare @sM			nvarchar(4)						set @sM=null
declare @sTM		nvarchar(4)						set @sTM=NULL
declare @sTTM		nvarchar(2)						set @sTTM=NULL
declare @sNG		nvarchar(2)						set @sNG=NULL
--###--

 
select		sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
			rDaQuyetToan	=sum(rTuChi)
from		QTA_ChungTuChiTiet
where		iTrangThai=1
			and iNamLamViec=@iNamLamViec
			and iID_MaNamNganSach=@iID_MaNamNganSach
			and iID_MaPhongBan=@iID_MaPhongBan
			and iID_MaDonVi=@iID_MaDonVi
			and (iThang_Quy<@iThang_Quy 
				or (iThang_Quy=@iThang_Quy  
					and iID_MaChungTu in (	select iID_MaChungTu from QTA_ChungTu 
											where	iTrangThai=1  
													and iNamLamViec=@iNamLamViec 
													and (dNgayChungTu<@dNgayChungTu or (dNgayChungTu=@dNgayChungTu and iSoChungTu <@iSoChungTu)))))
group by	sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
order by	sLNS,sL,sK,sM,sTM,sTTM,sNG
