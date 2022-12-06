DECLARE @iDThanhToan uniqueidentifier 		set @iDThanhToan =  '00000000-0000-0000-0000-000000000000'

--#DECLARE#--


select ct.*, concat(ns.sL, '-', ns.sK, '-', ns.sM,'-', ns.sTM,'-', ns.sTTM,'-', ns.sNG,'-', ns.sTNG) as sMucLucNganSach, ROW_NUMBER() OVER(ORDER BY ct.ID) as STT
from NH_TT_ThanhToan_ChiTiet as ct
left join NS_MucLucNganSach  as ns on ns.iID_MaMucLucNganSach = ct.iID_MucLucNganSachID
where iID_ThanhToanID = @iDThanhToan
