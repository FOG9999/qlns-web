DECLARE @id uniqueidentifier set @id = '8368A69A-0A5C-4838-B4DA-ADB500EBF2FE'

--#DECLARE#--

SELECT kh.*, duan.sTenDuAn, dv.sTen as sTenDonViQuanLy, chudautu.sTenCDT as sTenChuDauTu, chutruongdt.fTMDTDuKienPheDuyet as fTongMucDauTu
FROM VDT_QDDT_KHLCNhaThau kh
INNER JOIN VDT_DA_DuAn duan ON kh.iID_DuAnID = duan.iID_DuAnID
LEFT JOIN VDT_DA_ChuTruongDauTu chutruongdt ON chutruongdt.iID_DuAnID = duan.iID_DuAnID
LEFT JOIN NS_DonVi dv on duan.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN DM_ChuDauTu chudautu on duan.iID_ChuDauTuID = chudautu.ID

WHERE kh.id = @id;