SELECT gt.* INTO #tmpGoiThau FROM VDT_DA_GoiThau gt
WHERE bActive = 1 

SELECT iID_GoiThauGocID, SUM(fTienTrungThau) AS fTienTrungThau INTO #tmpGoiThauGoc FROM VDT_DA_GoiThau
WHERE bActive = 1
GROUP BY iID_GoiThauGocID 

SELECT gt.iID_GoiThauGocID, (CASE WHEN gt.bIsGoc = 1 THEN gt.fTienTrungThau ELSE gtg.fTienTrungThau END) AS fTienTrungThau, gt.bIsGoc, gt.* FROM #tmpGoiThau AS gt
LEFT JOIN #tmpGoiThauGoc AS gtg ON gt.iID_GoiThauGocID = gtg.iID_GoiThauGocID
WHERE gt.iID_GoiThauID = @iID_GoiThauID

DROP TABLE #tmpGoiThau;
DROP TABLE #tmpGoiThauGoc;
