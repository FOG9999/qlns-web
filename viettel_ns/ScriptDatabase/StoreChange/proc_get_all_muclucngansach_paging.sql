ALTER PROC [dbo].[proc_get_all_muclucngansach_paging]
@username nvarchar(256),
@namlamviec int,
@sLNS nvarchar(50),
@sL nvarchar(50),
@sK nvarchar(50),
@sM nvarchar(50),
@sTM nvarchar(50),
@sTTM nvarchar(50),
@sNG nvarchar(50),
@sTNG nvarchar(50),
@sNoiDung nvarchar(max),
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS

BEGIN
	IF ISNULL(@username, '') <> 'hieppm'
		BEGIN
			SELECT ns.*
			INTO #Temp
			FROM (
				SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,sChuong,iNamLamViec
				FROM NS_MucLucNganSach dn
				WHERE iTrangThai=1
			) AS ns
			WHERE sLNS <> '' 
				AND (@namlamviec IS NULL OR ns.iNamLamViec = @namlamviec)
				AND (@sLNS IS NULL OR ns.sLNS like CONCAT(N'%',@sLNS,N'%'))
				AND (@sL IS NULL OR ns.sL like CONCAT(N'%',@sL,N'%'))
				AND (@sK IS NULL OR ns.sK like CONCAT(N'%',@sK,N'%'))
				AND (@sM IS NULL OR ns.sM like CONCAT(N'%',@sM,N'%'))
				AND (@sTM IS NULL OR ns.sTM like CONCAT(N'%',@sTM,N'%'))
				AND (@sTTM IS NULL OR ns.sTTM like CONCAT(N'%',@sTTM,N'%'))
				AND (@sNG IS NULL OR ns.sNG like CONCAT(N'%',@sNG,N'%'))
				AND (@sTNG IS NULL OR ns.sTNG like CONCAT(N'%',@sTNG,N'%'))
				AND (@sNoiDung IS NULL OR ns.sMoTa like CONCAT(N'%',@sNoiDung,N'%'));
			
			SET @iToTalItem =  (SELECT COUNT(tmp.iID_MaMucLucNganSach) FROM #Temp AS tmp);
			
			SELECT * FROM #Temp
			ORDER BY sLNS
			OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
			FETCH NEXT @ItemsPerPage ROWS ONLY;
			
			DROP TABLE #Temp;
		END
	ELSE
		BEGIN
			SELECT ns.*
			INTO #Tmp
			FROM (
				SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,sChuong,iNamLamViec
				FROM NS_MucLucNganSach dn
				WHERE iTrangThai=1
			) AS ns
			WHERE (@namlamviec IS NULL OR ns.iNamLamViec = @namlamviec)
				AND (@sLNS IS NULL OR ns.sLNS like CONCAT(N'%',@sLNS,N'%'))
				AND (@sL IS NULL OR ns.sL like CONCAT(N'%',@sL,N'%'))
				AND (@sK IS NULL OR ns.sK like CONCAT(N'%',@sK,N'%'))
				AND (@sM IS NULL OR ns.sM like CONCAT(N'%',@sM,N'%'))
				AND (@sTM IS NULL OR ns.sTM like CONCAT(N'%',@sTM,N'%'))
				AND (@sTTM IS NULL OR ns.sTTM like CONCAT(N'%',@sTTM,N'%'))
				AND (@sNG IS NULL OR ns.sNG like CONCAT(N'%',@sNG,N'%'))
				AND (@sTNG IS NULL OR ns.sTNG like CONCAT(N'%',@sTNG,N'%'))
				AND (@sNoiDung IS NULL OR ns.sMoTa like CONCAT(N'%',@sNoiDung,N'%'));
			
			SET @iToTalItem =  (SELECT COUNT(tmp.iID_MaMucLucNganSach) FROM #Tmp AS tmp);
			
			SELECT * FROM #Tmp
			ORDER BY sLNS
			OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
			FETCH NEXT @ItemsPerPage ROWS ONLY;
			
			DROP TABLE #Tmp;
		END
END