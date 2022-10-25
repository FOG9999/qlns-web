declare @iNamLamViec int							set @iNamLamViec = '2018'
declare @sLNS nvarchar(20)							set @sLNS='4980000' 
declare @sL nvarchar(20)							set @sL= null
declare @sK nvarchar(20)							set @sK= null
declare @sM nvarchar(20)							set @sM= null
declare @sTM nvarchar(20)							set @sTM= null
declare @sTTM nvarchar(20)							set @sTTM= null
declare @sNG nvarchar(20)							set @sNG= null

select	* 
from	NS_MucLucNganSach
where	iNamLamViec=@iNamLamViec
		and iTrangThai = 1
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	DT_ChungTuChiTiet
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	DT_ChungTuChiTiet_PhanCap
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	DTBS_ChungTuChiTiet
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	DTBS_ChungTuChiTiet_PhanCap
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	CP_CapPhatChiTiet
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))

select	* 
from	QTA_ChungTuChiTiet
where	iNamLamViec = @iNamLamViec 
		and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
		and (@sL is null or sL in (select * from F_Split(@sL))) 
		and (@sK is null or sK in (select * from F_Split(@sK)))
		and (@sM is null or sM in (select * from F_Split(@sM))) 
		and (@sTM is null or sTM in (select * from F_Split(@sTM)))
		and (@sTTM is null or sTTM in (select * from F_Split(@sTTM))) 
		and (@sNG is null or sNG in (select * from F_Split(@sNG)))
