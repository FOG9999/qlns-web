DECLARE @iNamLamViec int				set @iNamLamViec = 2019
DECLARE @sLNS nvarchar(500)	set @sLNS = null
DECLARE @sL nvarchar(500)	set @sL = null
DECLARE @sK nvarchar(500)	set @sK = null
DECLARE @sM nvarchar(500)	set @sM = null
DECLARE @sTM nvarchar(500)	set @sTM = null
DECLARE @sTTM nvarchar(500)	set @sTTM = null
DECLARE @sNG nvarchar(500)	set @sNG = null
DECLARE @iID_NoiDungChi uniqueidentifier set @iID_NoiDungChi = '57fb779c-f3f5-4f74-9798-adf20114f671'
DECLARE @iMap int set @iMap = null
--#DECLARE#--

/*

Lấy danh sách danh mục lục ngân sách để mapping NDC

*/

select	mlns.iID_MaMucLucNganSach
	, mlns.iID_MaMucLucNganSach_Cha
	, mlns.bLaHangCha
	, mlns.sXauNoiMa
	, mlns.sLNS
	, mlns.sL
	, mlns.sK
	, mlns.sM
	, mlns.sTM
	, mlns.sTTM
	, mlns.sNG 
	, mlns.sMoTa
	
	, ndcmlns.Xau
	, ndcmlns.Id as IdMap
	, CAST((case when ndcmlns.Xau is not null and mlns.bLaHangCha = 0 then '1' else '0' end) as bit) as isMap
from	NS_MucLucNganSach mlns
left join NNS_NDChi_MLNS ndcmlns on mlns.sXauNoiMa = ndcmlns.Xau and ndcmlns.NamLamViec = @iNamLamViec
where		mlns.iNamLamViec = @iNamLamViec AND mlns.sLNS is not null AND mlns.sLNS <> ''
	and mlns.sXauNoiMa not in (select Xau from NNS_NDChi_MLNS where NamLamViec = @iNamLamViec and iID_NoiDungChi != @iID_NoiDungChi)
	and (@sLNS is null or mlns.sLNS like @sLNS)
	and (@sL is null or mlns.sL like @sL)
	and (@sK is null or mlns.sK like @sK)
	and (@sM is null or mlns.sM like @sM)
	and (@sTM is null or mlns.sTM like @sTM)
	and (@sTTM is null or mlns.sTTM like @sTTM)
	and (@sNG is null or mlns.sNG like @sNG)
	AND (@iMap is null or @iMap = 0 OR (@iMap = 1 AND (mlns.bLaHangCha = 1 OR (ndcmlns.Xau is not null and mlns.bLaHangCha = 0)) OR (@iMap = 2 AND (mlns.bLaHangCha = 1 OR (ndcmlns.Xau is null OR mlns.bLaHangCha <> 0)))))
order by mlns.sXauNoiMa