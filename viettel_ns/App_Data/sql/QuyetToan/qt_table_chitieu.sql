
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='07'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare  @iID_MaNamNganSach  nvarchar(20)			set @iID_MaNamNganSach = '2'
declare @dNgayChungTu datetime						set @dNgayChungTu = '2018-07-02 00:00:00.000'
declare @iID_MaDonVi1		int						set @iID_MaDonVi1='82'
declare @iID_MaDonVi		nvarchar(200)			set @iID_MaDonVi=NULL
declare @iThang_Quy		int							set @iThang_Quy=1
declare @sLNS1		nvarchar(200)					set @sLNS1='1'
declare @sLNS		nvarchar(7)						set @sLNS='1010000'
declare @sL			nvarchar(3)						set @sL=NULL
declare @sK			nvarchar(3)						set @sK=NULL
declare @sM			nvarchar(4)						set @sM=NULL
declare @sTM		nvarchar(4)						set @sTM=NULL
declare @sTTM		nvarchar(2)						set @sTTM=NULL
declare @sNG		nvarchar(2)						set @sNG=NULL

--###--

 select sLNS,sL,sK,sM,sTM, sTTM,sNG, sXauNoiMa, iID_MaDonVi, rTuChi as rChiTieu 
 from	f_chitieu(@iNamLamViec,@iID_MaDonVi1,@iID_MaPhongBan,@iID_MaNamNganSach, @iID_MaNguonNganSach,@sLNS1, @dNgayChungTu,1,null)
 where	(@sLNS is null or sLNS like @sLNS) 
		and (@sL is null or sL like @sL) 
		and (@sK is null or sK like @sK) 
		and (@sM is null or sM like @sM) 
		and (@sTM is null or sTM like @sTM) 
		and (@sTTM is null or sTTM like @sTTM) 
		and (@sNG is null or sNG like @sNG) 



		--and (@iID_MaDonVi is null or iID_MaDonVi=@iID_MaDonVi) 
