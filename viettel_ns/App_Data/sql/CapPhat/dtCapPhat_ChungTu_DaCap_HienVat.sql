declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaCapPhat uniqueidentifier				set @iID_MaCapPhat=null

--#DECLARE#--/

SELECT		*
FROM		CP_CapPhatChiTiet
WHERE		iTrangThai = 1 
			AND iNamLamViec = @iNamLamViec 
			AND ( @iID_MaCapPhat IS NULL OR iID_MaCapPhat IN (SELECT * FROM F_Split(@iID_MaCapPhat))) 
			AND rTuChi<>0 
			and sLNS = ''								  

ORDER BY	sXauNoiMa




                          
                     

