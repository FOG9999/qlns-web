declare @iNamLamViec int								set @iNamLamViec = '2018'
declare @sLNS_o nvarchar(20)							set @sLNS_o='2030100' 
declare @sL_o nvarchar(20)								set @sL_o='100'
declare @sK_o nvarchar(20)								set @sK_o='101'
declare @sM_o nvarchar(20)								set @sM_o='7000'
declare @sTM_o nvarchar(20)								set @sTM_o='7017'
declare @sTTM_o nvarchar(20)							set @sTTM_o='10'
declare @sNG_o nvarchar(20)								set @sNG_o='72'
declare @sLNS_n nvarchar(20)							set @sLNS_n='2030100' 
declare @sL_n nvarchar(20)								set @sL_n='100'
declare @sK_n nvarchar(20)								set @sK_n='103'
declare @sM_n nvarchar(20)								set @sM_n='7000'
declare @sTM_n nvarchar(20)								set @sTM_n='7017'
declare @sTTM_n nvarchar(20)							set @sTTM_n='10'
declare @sNG_n nvarchar(20)								set @sNG_n='72'

update	NS_MucLucNganSach
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec=@iNamLamViec
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	DT_ChungTuChiTiet
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	DT_ChungTuChiTiet_PhanCap
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	DTBS_ChungTuChiTiet
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	DTBS_ChungTuChiTiet_PhanCap
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	CP_CapPhatChiTiet
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))

update	QTA_ChungTuChiTiet
set		sLNS = @sLNS_n , sL = @sL_n, sK = @sK_n, sM = @sM_n, sTM = @sTM_n, sTTM = @sTTM_n, sNG = @sNG_n, 
		sXauNoiMa = @sLNS_n + '-' + @sL_n + '-' + @sK_n + '-' + @sM_n  + '-' + @sTM_n + '-' + @sTTM_n + '-' + @sNG_n
where	iNamLamViec = @iNamLamViec 
		and (@sLNS_o is null or sLNS in (select * from F_Split(@sLNS_o)))
		and (@sL_o is null or sL in (select * from F_Split(@sL_o))) 
		and (@sK_o is null or sK in (select * from F_Split(@sK_o)))
		and (@sM_o is null or sM in (select * from F_Split(@sM_o))) 
		and (@sTM_o is null or sTM in (select * from F_Split(@sTM_o)))
		and (@sTTM_o is null or sTTM in (select * from F_Split(@sTTM_o))) 
		and (@sNG_o is null or sNG in (select * from F_Split(@sNG_o)))