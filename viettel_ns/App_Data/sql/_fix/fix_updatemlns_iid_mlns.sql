declare @iNamLamViec int								set @iNamLamViec = '2018'
declare @sLNS_o nvarchar(7)								set @sLNS_o='2030100' 
declare @sL_o nvarchar(3)								set @sL_o='100'
declare @sK_o nvarchar(3)								set @sK_o='101'
declare @sM_o nvarchar(4)								set @sM_o='7000'
declare @sTM_o nvarchar(4)								set @sTM_o='7017'
declare @sTTM_o nvarchar(4)								set @sTTM_o='10'
declare @sNG_o nvarchar(2)								set @sNG_o='72'
declare @sXauNoiMa nvarchar(33)							set @sXauNoiMa = @sLNS_o + '-' + @sL_o + '-' + @sK_o + '-' + @sM_o + '-' + @sTM_o + '-' + @sTTM_o + '-' + @sNG_o



UPDATE	DT_ChungTuChiTiet
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	DT_ChungTuChiTiet.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)

UPDATE	DT_ChungTuChiTiet_PhanCap
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	DT_ChungTuChiTiet_PhanCap.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DT_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)

UPDATE	DTBS_ChungTuChiTiet
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	DTBS_ChungTuChiTiet.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)

UPDATE	DTBS_ChungTuChiTiet_PhanCap
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	DTBS_ChungTuChiTiet_PhanCap.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet_PhanCap.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)

UPDATE	CP_CapPhatChiTiet
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND CP_CapPhatChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND CP_CapPhatChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND CP_CapPhatChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	CP_CapPhatChiTiet.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND CP_CapPhatChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)

UPDATE	QTA_ChungTuChiTiet
SET		iID_MaMucLucNganSach_Cha = (
						SELECT	iID_MaMucLucNganSach_Cha 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND QTA_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , iID_MaMucLucNganSach = (
						SELECT	iID_MaMucLucNganSach 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND QTA_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
        , sMoTa = ( 
						SELECT	sMoTa 
						FROM	NS_MucLucNganSach 
						WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND QTA_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)
WHERE	QTA_ChungTuChiTiet.sXauNoiMa = @sXauNoiMa
		AND EXISTS (
				SELECT	* 
				FROM	NS_MucLucNganSach 
				WHERE	NS_MucLucNganSach.iTrangThai = 1 AND iNamLamViec=@iNamLamViec AND QTA_ChungTuChiTiet.sXauNoiMa = NS_MucLucNganSach.sXauNoiMa)