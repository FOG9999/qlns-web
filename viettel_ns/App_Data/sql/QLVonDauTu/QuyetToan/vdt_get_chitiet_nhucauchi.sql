DECLARE @iID_NhuCauChiID uniqueidentifier SET @iID_NhuCauChiID = '9378B259-DEC2-4881-A44B-ADCE00EB863E'
--#DECLARE#--

SELECT nhucau.*, ns.sTen as sNguonVon, dv.sTen as sDonViQuanLy
	FROM  VDT_NC_NhuCauChi nhucau
	LEFT JOIN NS_NguonNganSach as ns on nhucau.iID_NguonVonID = ns.iID_MaNguonNganSach
	LEFT JOIN NS_DonVi as dv on nhucau.iID_DonViQuanLyID = dv.iID_Ma
	where Id = @iID_NhuCauChiID