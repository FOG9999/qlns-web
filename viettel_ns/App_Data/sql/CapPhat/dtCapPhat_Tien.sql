declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '2'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
DECLARE @sLNS NVARCHAR(MAX)							SET @sLNS = '1020100,1020000'
DECLARE @iID_MaDonVi NVARCHAR(MAX)					SET @iID_MaDonVi = '12'
DECLARE @loai NVARCHAR(1)							SET @loai = null

--#DECLARE#--

SELECT		iID_MaDonVi
			, sLNS
			, sL
			, sK			
			, sM, sTM
			, SUM(rTuChi) AS rTuChi
FROM (	
			select		sLNS, sL, sK, sM, sTM, sTTM, sNG, iID_MaDonVi, rTien as rTuChi
			from		f_ns_table_chitieu_tien(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'2,4',GETDATE(),1,@loai)
			
) as CT
WHERE		( @sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS)))
GROUP BY	iID_MaDonVi			
			, sLNS
			, sL
			, sK
			, sM, sTM			
ORDER BY	sLNS
			, sL
			, sK
			, sM, sTM
			, iID_MaDonVi
