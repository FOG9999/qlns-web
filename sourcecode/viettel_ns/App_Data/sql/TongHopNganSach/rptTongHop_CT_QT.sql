declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @loai nvarchar(20)							set @loai='1' 
declare @donvi nvarchar(20)							set @donvi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2,4'
declare @dNgay datetime								set @dNgay=GETDATE()



--#DECLARE#--




SELECT	LEFT(sLNS,1) as sLNS1,
		LEFT(sLNS,3) as sLNS3,
		LEFT(sLNS,5) as sLNS5,
		sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa = CONVERT(nvarchar(7),sLNS)+'-'+
					CONVERT(nvarchar(3),sL)+'-'+
					CONVERT(nvarchar(3),sK)+'-'+
					CONVERT(nvarchar(4),sM)+'-'+
					CONVERT(nvarchar(4),sTM)+'-'+
					CONVERT(nvarchar(2),sTTM)+'-'+
					CONVERT(nvarchar(2),sNG),
		Id_DonVi = iID_MaDonVi,TenDonVi,
		SUM(rTuChi) as C1,
		SUM(rQuyetToan) as C2,
		C3 = case when sum(-rQuyetToan+rTuChi)>0 then SUM(rTuChi-rQuyetToan) else 0 end,
		C4 = case when sum(-rQuyetToan+rTuChi)<0 then SUM(-rTuChi+rQuyetToan) else 0 end
FROM
(
		-- quyet toan
		SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi=LEFT(iID_MaDonVi,2),
					0 as rTuChi,
					sum(rTuChi) as rQuyetToan
		FROM		QTA_ChungTuChiTiet
		WHERE		iTrangThai=1 
					AND iNamLamViec=@iNamLamViec 
					AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
					AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
		GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


		UNION ALL

		-- lay chi tieu
		SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi=LEFT(iID_MaDonVi,2),
					rTuChi=	 SUM(rTien),
					0 as rQuyetToan
		FROM    f_ns_table_chitieu_tien(@iNamLamviec, NULL, @iID_MaPhongBan, @iID_MaNamNganSach,getdate(),1,null)
		GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	

) a
INNER JOIN  (SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as TenDonVi FROM NS_DonVi WHERE iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv 
ON a.iID_MaDonVi=dv.dv_id

WHERE		LEFT(sLNS,1) IN (1,2,3,4) and iID_MaDonVi in (select * from f_split(@donvi)) and sLNS not like '101%'
GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,TenDonVi
HAVING		SUM(rTuChi-rQuyetToan)<>0