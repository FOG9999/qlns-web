DECLARE @id uniqueidentifier

--#DECLARE#--

SELECT iID_GoiThauID INTO #tmp_goithau
FROM VDT_DA_GoiThau 
WHERE iID_KHLCNhaThau = @id

DELETE VDT_DA_GoiThau_NguonVon WHERE iID_GoiThauID IN (SELECT * FROM #tmp_goithau)
DELETE VDT_DA_GoiThau_ChiPhi WHERE iID_GoiThauID IN (SELECT * FROM #tmp_goithau)
DELETE VDT_DA_GoiThau_HangMuc WHERE iID_GoiThauID IN (SELECT * FROM #tmp_goithau)

DELETE VDT_DA_GoiThau WHERE iID_GoiThauID IN (SELECT * FROM #tmp_goithau)