DECLARE @iLoaiNganSach int set @iLoaiNganSach = null
DECLARE @iNamLamViec int set @iNamLamViec = 2018
--#DECLARE#--

select * from DM_Nguon WHERE (@iLoaiNganSach is null or iLoaiNganSach = @iLoaiNganSach)
				AND bPublic = 1
				AND iNamLamViec = @iNamLamViec
order by cast('/' + replace(iSTT, '.', '/') + '/' as hierarchyid)