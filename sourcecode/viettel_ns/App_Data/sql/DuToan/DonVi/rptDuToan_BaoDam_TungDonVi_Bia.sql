declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2020
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

declare @ngcha	nvarchar(2000)						set @ngcha='51'
declare @ng	nvarchar(2000)							set @ng='20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='trolyphongban6'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

SELECT		iID_MaNganh
			, sTenNganh
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa = dbo.F_MLNS_MoTa_LNS(@nam,'1020100',sL,sK,sM,sTM,sTTM,sNG)	
			, rTuChi = SUM(rTuChi)
			, rTonKho = SUM(rTonKho)
			, rHangNhap = SUM(rHangNhap)
			, rHangMua = SUM(rHangMua)
			, rPhanCap = SUM(rPhanCap)
			, rDuPhong = SUM(rDuPhong)
FROM
-- Phần bìa bảo đảm--
			(
			
			SELECT		sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG		
						, rTuChi=SUM(rTuChi/@dvt)
						, rTonKho=SUM(rTonKho/@dvt)
						, rHangNhap=SUM(rHangNhap/@dvt)
						, rHangMua=SUM(rHangMua/@dvt)
						, rPhanCap=SUM(rPhanCap/@dvt)
						, rDuPhong=SUM(rDuPhong/@dvt)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai in (1,2) 
						AND sNG <> '00' 
						AND sLNS like '104%'
						AND (MaLoai='' OR MaLoai='2')
						AND iNamLamViec = @nam  
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))						
						AND (@ng is null or sNG IN (select * from f_split(@ng)))		
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG
			HAVING		SUM(rTuChi)<>0 
						OR SUM(rHangNhap)<>0 
						OR SUM(rTonKho)<>0  
						OR SUM(rHangMua)<>0  
						OR SUM(rPhanCap)<>0 
						OR SUM(rDuPhong)<>0

			UNION ALL

			SELECT		sLNS = '1040100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG		
						, rTuChi = 0
						, rTonKho = 0
						, rHangNhap =0
						, rHangMua = 0
						, rPhanCap = SUM(rTuChi/@dvt)
						, rDuPhong = 0
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai in (1,2) 
						AND sNG <> '00' 
						AND sLNS='1020100'
						AND iNamLamViec = @nam
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))						
						AND (@ng is null or sNG IN (select * from f_split(@ng)))
						AND (@ngcha	is null or (@ngcha = '51' and iID_MaChungTu not in ('eb3e6e6f-d811-4769-b0b3-e838cadca594')))
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG
			HAVING		SUM(rTuChi)<>0	
			
			UNION ALL

			SELECT		sLNS = '1040100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG		
						, rTuChi = 0
						, rTonKho = 0
						, rHangNhap =0
						, rHangMua = 0
						, rPhanCap = SUM(rTuChi/@dvt)
						, rDuPhong = 0
			 FROM		DT_ChungTuChiTiet_PhanCap
			 WHERE		iTrangThai in (1,2) 
						AND sNG <> '00' 
						AND sLNS='1020100'
						AND MaLoai = '2'
						AND iNamLamViec = @nam
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))						
						AND (@ng is null or sNG IN (select * from f_split(@ng)))
						AND (@ngcha	is null or (@ngcha = '51' and sNG <> '23'))
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG
			HAVING		SUM(rTuChi)<>0			
			
			) as result,
			(select sNG as iID_MaNganh, sMoTa as sTenNganh from NS_MucLucNganSach where iTrangThai = 1 and iNamLamViec=@nam and sLNS = '') as b
WHERE		result.sNG = b.iID_MaNganh 
GROUP BY	iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM,sNG
ORDER BY	iID_MaNganh
			,sM
			,sTM
			,sTTM
			,sNG