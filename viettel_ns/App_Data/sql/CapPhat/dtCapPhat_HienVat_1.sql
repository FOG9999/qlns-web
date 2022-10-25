declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi = null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @loai nvarchar(20)							set @loai = '0,1'
declare @sLNS nvarchar(20)							set @sLNS = '1020100'
declare @dNgay datetime								set @dNgay = GETDATE()

--#DECLARE#--/

SELECT		iID_MaDonVi
			, sNG
			, SUM(rHienVat) AS rHienVat
FROM (
			--	DU TOAN --      
			select iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
						, loai = 0
			from	f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,'2',@dNgay,1,'1')
			--from	f_ns_chitieu(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,'2',@dNgay,1,'1')
			WHERE		sLNS IN ('1020100') and rHienVat <> 0
			GROUP BY	iID_MaDonVi
						, sNG

			UNION ALL

			-- BO SUNG --      
			select iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
						, loai = 1
			from	f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,'2',@dNgay,1,'2')
			--from	f_ns_chitieu(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,'2',@dNgay,1,'2')
			WHERE		sLNS IN ('1020100') and rHienVat <> 0
			GROUP BY	iID_MaDonVi
						, sNG           
) as CT			
WHERE		(@loai IS NULL OR loai IN (SELECT * FROM F_Split(@loai)))
GROUP BY	iID_MaDonVi
			, sNG
ORDER BY	sNG
			, iID_MaDonVi
