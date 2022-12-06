

CREATE TABLE #tmpLineError(iLine int)

INSERT INTO BHXH_BenhNhanError (iID_ImportID,iLine, sPropertyName, sMessage)
OUTPUT inserted.iLine INTO #tmpLineError(iLine)
SELECT @iIDImportID, tbl.iLine, N'MADV', N'Không tồn tại mã đơn vị'
FROM BHXH_BenhNhanTemp as tbl
LEFT JOIN BHXH_DonVi as dt on tbl.sMaDV = LTRIM(RTRIM(dt.iID_MaDonViBHXH))
WHERE tbl.sMaDV IS NOT NULL AND dt.iID_MaDonViBHXH IS NULL AND tbl.iID_ImportID = @iIDImportID

DECLARE @iLineErrorDonVi int = (SELECT COUNT(*) FROM #tmpLineError);

WITH tmp AS
(
	SELECT tbl.iID_BenhNhanID, sMaThe, tbl.sNgayVaoVien, tbl.sNgayRaVien, iLine,
		ROW_NUMBER() OVER (PARTITION BY tbl.sMaThe, tbl.sNgayVaoVien, tbl.sNgayRaVien ORDER BY tbl.iLine) as rn
	FROM BHXH_BenhNhanTemp as tbl
	WHERE tbl.iID_ImportID = @iIDImportID
)
INSERT INTO BHXH_BenhNhanError (iID_ImportID,iLine, sPropertyName, sMessage)
OUTPUT inserted.iLine INTO #tmpLineError(iLine)
SELECT @iIDImportID, tmp.iLine, 'MATHE,NGAY_VAO_VIEN,NGAY_RA_VIEN', N'Trùng mã thẻ, ngày vào viện , ngày ra viện !'
FROM tmp as tmp
WHERE rn > 1

UPDATE tbl
SET
	bError = 1
FROM #tmpLineError as tmp
INNER JOIN BHXH_BenhNhanTemp as tbl on tmp.iLine = tbl.iLine

SELECT @iLineErrorDonVi

DROP TABLE #tmpLineError