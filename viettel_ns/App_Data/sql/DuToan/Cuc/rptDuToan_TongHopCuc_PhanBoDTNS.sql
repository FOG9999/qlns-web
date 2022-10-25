declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @ng	nvarchar(2000)							set @ng='20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

SELECT		* 
FROM
			(
			SELECT		iiD_MaDonVi
						, Loai
						, TONG = (SUM(Cot1)+SUM(Cot2)+SUM(Cot3)+SUM(Cot4)+SUM(Cot5)+SUM(Cot6)+SUM(Cot7)+SUM(Cot8)+SUM(Cot9)+SUM(Cot10)+SUM(Cot11))/@dvt
						, COT1 = SUM(Cot1/@dvt)
						, COT2 = SUM(COT2/@dvt)
						, COT3 = SUM(COT3/@dvt)
						, COT4 = SUM(COT4/@dvt)
						, COT5 = SUM(COT5/@dvt)
						, COT6 = SUM(COT6/@dvt)
						, COT7 = SUM(COT7/@dvt)
						, COT8 = SUM(COT8/@dvt)
						, COT9 = SUM(COT9/@dvt)
						, COT10 = SUM(Cot10/@dvt)
						, COT11 = SUM(Cot11/@dvt)
			FROM		(
						--- Phần phân cấp cho đơn vị --
						SELECT		iID_MaDonVi
									, Loai = N'I. Phân cấp cho đơn vị'
									, COT1 = SUM(CASE WHEN sLNS=1020100 AND sM=6600 AND stm=6608 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT2 = SUM(CASE WHEN sLNS=1020100 AND sM=6600 AND stm=6649 AND sTTM IN(10,20) THEN rTuChi+rHangMua ELSE 0 END) 
									, COT3 = SUM(CASE WHEN sLNS=1020100 AND sM=7750 AND stm=7799 AND sTTM=10 AND sNG=22 THEN rTuChi+rHangMua ELSE 0 END)
									, COT4 = SUM(CASE WHEN sLNS=1020100 AND sM=6200 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT5 = SUM(CASE WHEN sLNS=1020100 AND sM=6250 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT6 = SUM(CASE WHEN sLNS=1020100 AND sM=6700 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT7 = SUM(CASE WHEN sLNS=1020100 AND sM=6900 AND stm=6905 AND sTTM IN (10,20,30,40) AND sNG=65 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT8 = SUM(CASE WHEN sLNS=1020100 AND sM=6500 AND stm IN (6501,6502) AND sTTM=00 AND sNG=56 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT9 = SUM(CASE WHEN sLNS=1020100 AND sM=6900 AND stm=6907 AND sTTM=00 AND sNG=56  THEN rTuChi+rHangMua ELSE 0 END) 
									, COT10 = SUM(CASE WHEN sLNS=1020100 AND sM=7000 AND stm=7001 AND sTTM IN(20,30) THEN rTuChi+rHangMua ELSE 0 END) 
									, COT11 = SUM(CASE WHEN sXauNoiMa = '1030100-010-011-9300-9301-10-56' THEN rTuChi ELSE 0 END)
						FROM		DT_ChungTuChiTiet_PhanCap
						WHERE		iTrangThai <> 0
									AND iNamLamViec=@nam @@DKPB
						GROUP BY	iID_MaDonVi
  
						UNION

						SELECT		iID_MaDonVi
									, Loai = N'I. Phân cấp cho đơn vị'
									, COT1 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6600 AND stm=6608 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT2 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6600 AND stm=6649 AND sTTM IN(10,20) THEN rTuChi+rHangMua ELSE 0 END) 
									, COT3 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=7750 AND stm=7799 AND sTTM=10 AND sNG=22 THEN rTuChi+rHangMua ELSE 0 END)
									, COT4 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6200 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT5 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6250 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT6 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6700 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT7 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6900 AND stm=6905 AND sTTM IN (10,20,30,40) AND sNG=65 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT8 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6500 AND stm IN (6501,6502) AND sTTM=00 AND sNG=56 THEN rTuChi+rHangMua ELSE 0 END) 
									, COT9 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=6900 AND stm=6907 AND sTTM=00 AND sNG=56  THEN rTuChi+rHangMua ELSE 0 END) 
									, COT10 = SUM(CASE WHEN (sLNS=1020100 OR sLNS=1020000) AND sM=7000 AND stm=7001 AND sTTM IN(20,30) THEN rTuChi+rHangMua ELSE 0 END) 
									, COT11 = SUM(CASE WHEN sXauNoiMa = '1030100-010-011-9300-9301-10-56' THEN rTuChi ELSE 0 END)
						FROM		DT_ChungTuChiTiet
						WHERE		iTrangThai <> 0
									AND iNamLamViec=@nam @@DKPB
						GROUP BY	iID_MaDonVi
					
						UNION 
						-- Phần tự chi tại ngành --
						SELECT		iID_MaDonVi,Loai=N'II. Tự chi tại ngành'
									,COT1=SUM(CASE WHEN sLNS=1040100 AND sM=6600 AND stm=6608 THEN rTuChi+rHangMua ELSE 0 END) 
									,COT2=SUM(CASE WHEN sLNS=1040100 AND sM=6600 AND stm=6649 AND sTTM IN(10,20) THEN rTuChi+rHangMua ELSE 0 END) 
									,COT3=SUM(CASE WHEN sLNS=1040100 AND sM=7750 AND stm=7799 AND sTTM=10 AND sNG=22 THEN rTuChi+rHangMua ELSE 0 END)
									,COT4=SUM(CASE WHEN sLNS=1040100 AND sM=6200  THEN rTuChi+rHangMua ELSE 0 END) 
									,COT5=SUM(CASE WHEN sLNS=1040100 AND sM=6250 THEN rTuChi+rHangMua ELSE 0 END) 
									,COT6=SUM(CASE WHEN sLNS=1040100 AND sM=6700 THEN rTuChi+rHangMua ELSE 0 END) 
									,COT7=SUM(CASE WHEN sLNS=1040100 AND sM=6900 AND stm=6905 AND sTTM IN (10,20,30,40) AND sNG=65 THEN rTuChi+rHangMua ELSE 0 END) 
									,COT8=SUM(CASE WHEN sLNS=1040100 AND sM=6500 AND stm IN (6501,6502) AND sTTM=00 AND sNG=56 THEN rTuChi+rHangMua ELSE 0 END) 
									,COT9=SUM(CASE WHEN sLNS=1040100 AND sM=6900 AND stm=6907 AND sTTM=00 AND sNG=56  THEN rTuChi+rHangMua ELSE 0 END) 
									,COT10=SUM(CASE WHEN sLNS=1040100 AND sM=7000 AND stm=7001 AND sTTM IN(20,30) THEN rTuChi+rHangMua ELSE 0 END)
									,COT11=0
						FROM		DT_ChungTuChiTiet
						WHERE		iTrangThai <> 0 AND iNamLamViec=@nam @@DKPB AND (MaLoai='' OR MaLoai='2')
						GROUP BY	iID_MaDonVi

						UNION
						-- Phần dự phòng --
						SELECT		iID_MaDonVi,Loai=N'III. Chờ phân cấp'
									, COT1=SUM(CASE WHEN sLNS=1040100 AND sM=6600 AND stm=6608 THEN rDuPhong ELSE 0 END) 
									, COT2=SUM(CASE WHEN sLNS=1040100 AND sM=6600 AND stm=6649 AND sTTM IN(10,20) THEN rDuPhong ELSE 0 END) 
									, COT3=SUM(CASE WHEN sLNS=1040100 AND sM=7750 AND stm=7799 AND sTTM=10 AND sNG=22 THEN rDuPhong ELSE 0 END)
									, COT4=SUM(CASE WHEN sLNS=1040100 AND sM=6200  THEN rDuPhong ELSE 0 END) 
									, COT5=SUM(CASE WHEN sLNS=1040100 AND sM=6250 THEN rDuPhong ELSE 0 END) 
									, COT6=SUM(CASE WHEN sLNS=1040100 AND sM=6700 THEN rDuPhong ELSE 0 END) 
									, COT7=SUM(CASE WHEN sLNS=1040100 AND sM=6900 AND stm=6905 AND sTTM IN (10,20,30,40) AND sNG=65 THEN rDuPhong ELSE 0 END) 
									, COT8=SUM(CASE WHEN sLNS=1040100 AND sM=6500 AND stm IN (6501,6502) AND sTTM=00 AND sNG=56 THEN rDuPhong ELSE 0 END) 
									, COT9=SUM(CASE WHEN sLNS=1040100 AND sM=6900 AND stm=6907 AND sTTM=00 AND sNG=56  THEN rDuPhong ELSE 0 END) 
									, COT10=SUM(CASE WHEN sLNS=1040100 AND sM=7000 AND stm=7001 AND sTTM IN(20,30) THEN rDuPhong ELSE 0 END)
									, COT11=SUM(CASE WHEN sXauNoiMa = '1030100-010-011-9300-9301-10-56' AND iID_MaDonVi = 'B5' THEN rDuPhong ELSE 0 END)
						FROM		DT_ChungTuChiTiet
						WHERE		iTrangThai <> 0 AND iNamLamViec=@nam @@DKPB AND (MaLoai='' OR MaLoai='2')
						GROUP BY	iID_MaDonVi) as a
					  
			GROUP BY	iID_MaDonVi,Loai
			HAVING		SUM(COT1)<>0 OR
						SUM(COT2)<>0 OR
						SUM(COT3)<>0 OR
						SUM(COT4)<>0 OR
						SUM(COT5)<>0 OR
						SUM(COT6)<>0 OR
						SUM(COT7)<>0 OR
						SUM(COT8)<>0 OR
						SUM(COT9)<>0 OR
						SUM(COT10)<>0 OR
						SUM(COT11)<>0) CT

			INNER JOIN 

			(SELECT iID_MaDonVi as MaDonVi,sTen FROM NS_DonVi WHERE iTrangThai=1 AND iNamLamViec_DonVi=@nam)  as b

			ON CT.iID_MaDonVi=b.MaDonVi
ORDER BY	Loai, iID_MaDonVi