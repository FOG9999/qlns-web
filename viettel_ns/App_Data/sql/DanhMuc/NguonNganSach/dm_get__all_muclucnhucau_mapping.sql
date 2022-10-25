DECLARE @iNamLamViec int				set @iNamLamViec = 2019
DECLARE @Loai nvarchar(500)	set @Loai = null
DECLARE @Nganh_Parent nvarchar(500)	set @Nganh_Parent = null
DECLARE @Nganh nvarchar(500)	set @Nganh = null
DECLARE @M nvarchar(500)	set @M = null
DECLARE @KyHieu nvarchar(500)	set @KyHieu = null
DECLARE @iID_NoiDungChi uniqueidentifier set @iID_NoiDungChi = null
DECLARE @iMap int set @iMap = null
--#DECLARE#--

/*

Lấy danh sách danh mục nội dung chi

*/

select	mlnc.Id
	, mlnc.Id_Parent
	, mlnc.IsParent
	, mlnc.KyHieu
	, mlnc.Loai
	, mlnc.Nganh_Parent
	, mlnc.Nganh
	, mlnc.M
	, mlnc.STT
	, mlnc.STTBC
	, mlnc.MoTa 
	, mlnc.KyHieuCha
	, ndcmlnc.iID_MLNhuCau
	, ndcmlnc.Id as IdMap
	, CAST((case when ndcmlnc.iID_MLNhuCau is not null and mlnc.IsParent = 0 then '1' else '0' end) as bit) as isMap
from		SKT_MLNhuCau mlnc
left join NNS_NDChi_MLNhuCau ndcmlnc on mlnc.Id = ndcmlnc.iID_MLNhuCau and ndcmlnc.NamLamViec = @iNamLamViec
where		mlnc.NamLamViec=@iNamLamViec
	and mlnc.Id not in (select iID_MLNhuCau from NNS_NDChi_MLNhuCau where NamLamViec = @iNamLamViec  and iID_NoiDungChi != @iID_NoiDungChi)
	and (@Loai is null or mlnc.Loai like @Loai)
	and (@KyHieu is null or mlnc.KyHieu like @KyHieu)
	and (@Nganh is null or mlnc.Nganh like @Nganh)
	and (@M is null or mlnc.M like @M)
	and (mlnc.Nganh_Parent is null or @Nganh_Parent is null or mlnc.Nganh_Parent like @Nganh_Parent)
	AND (@iMap is null or @iMap = 0 OR (@iMap = 1 AND (mlnc.IsParent = 1 OR (ndcmlnc.iID_MLNhuCau is not null and mlnc.IsParent = 0)) OR (@iMap = 2 AND (mlnc.IsParent = 1 OR (ndcmlnc.iID_MLNhuCau is null OR mlnc.IsParent <> 0)))))
order by	mlnc.KyHieu