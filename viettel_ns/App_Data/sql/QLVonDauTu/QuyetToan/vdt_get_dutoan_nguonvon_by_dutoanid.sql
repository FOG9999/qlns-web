DECLARE @iIdDuToanId uniqueidentifier set @iIdDuToanId = '88D34274-9C73-4E9D-8754-AE8000F8CE17'

--#DECLARE#--

select nv.*, nns.sTen as sTenNguonVon 
from VDT_DA_DuToan_Nguonvon nv
INNER JOIN VDT_DA_DuToan dt ON nv.iID_DuToanID = dt.iID_DuToanID AND dt.bActive = 1 
INNER JOIN NS_NguonNganSach nns ON nns.iID_MaNguonNganSach = nv.iID_NguonVonID
WHERE dt.iID_DuToanID = @iIdDuToanId
ORDER BY nns.iID_MaNguonNganSach