declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='06' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='12'
declare @iThang_Quy nvarchar(20)					set @iThang_Quy=null

--#DECLARE#--/

SELECT	iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sKyHieuCap2,(' + ' + C.sTen) sTenLoaiHinhCap2,
		sKyHieuCap3,(' - ' + B.sTen) sTenLoaiHinhCap3, (rTongThu / @dvt) rTongThu, (rTongChiPhi / @dvt) rTongChiPhi, (rKhauHaoTSCD / @dvt) rKhauHaoTSCD, (rTienLuong / @dvt) rTienLuong, 
		(rQTNSKhac / @dvt) rQTNSKhac, (rChiPhiKhac / @dvt) rChiPhiKhac, (rTongNopNSNN / @dvt) rTongNopNSNN, (rNopThueGTGT / @dvt) rNopThueGTGT, (rTongNopThueTNDN / @dvt) rTongNopThueTNDN, 
		(rPhiLePhi / @dvt) rPhiLePhi, (rTongNopNSNNKhac / @dvt) rTongNopNSNNKhac, (rNopNSNNKhacBQP / @dvt) rNopNSNNKhacBQP, ((rTongThu - (rTongChiPhi + rTongNopNSNN)) / @dvt) rChenhLech, 
		(rNopNSQP / @dvt) rNopNSQP, (rBoSungKinhPhi / @dvt) rBoSungKinhPhi, (rTrichQuyDonVi / @dvt) rTrichQuyDonVi, ((rTongThu - (rTongChiPhi + rTongNopNSNN) - (rNopNSQP + rBoSungKinhPhi + rTrichQuyDonVi)) / @dvt)  rSoChuaPhanPhoi
FROM(
	SELECT	iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sKyHieuCap2,sKyHieuCap3
			,SUM(CASE WHEN bThoaiThu=0 THEN rTongThu WHEN bThoaiThu=1 THEN rTongThu*(-1) ELSE 0 END) rTongThu
			,SUM(CASE WHEN bThoaiThu=0 THEN (rKhauHaoTSCD + rTienLuong + rQTNSKhac + rChiPhiKhac) WHEN bThoaiThu=1 THEN (rKhauHaoTSCD + rTienLuong + rQTNSKhac + rChiPhiKhac)*(-1) ELSE 0 END) rTongChiPhi
			,SUM(CASE WHEN bThoaiThu=0 THEN rKhauHaoTSCD WHEN bThoaiThu=1 THEN rKhauHaoTSCD*(-1) ELSE 0 END) rKhauHaoTSCD
			,SUM(CASE WHEN bThoaiThu=0 THEN rTienLuong WHEN bThoaiThu=1 THEN rTienLuong*(-1) ELSE 0 END) rTienLuong
			,SUM(CASE WHEN bThoaiThu=0 THEN rQTNSKhac WHEN bThoaiThu=1 THEN rQTNSKhac*(-1) ELSE 0 END) rQTNSKhac
			,SUM(CASE WHEN bThoaiThu=0 THEN rChiPhiKhac WHEN bThoaiThu=1 THEN rChiPhiKhac*(-1) ELSE 0 END) rChiPhiKhac
			,SUM(CASE WHEN bThoaiThu=0 THEN (rNopThueGTGT + rTongNopThueTNDN + rPhiLePhi + rTongNopNSNNKhac) WHEN bThoaiThu = 1 THEN (rNopThueGTGT + rTongNopThueTNDN + rPhiLePhi + rTongNopNSNNKhac)*(-1) ELSE 0 END) rTongNopNSNN
			,SUM(CASE WHEN bThoaiThu=0 THEN rNopThueGTGT WHEN bThoaiThu=1 THEN rNopThueGTGT*(-1) ELSE 0 END) rNopThueGTGT
			,SUM(CASE WHEN bThoaiThu=0 THEN rTongNopThueTNDN WHEN bThoaiThu=1 THEN rTongNopThueTNDN*(-1) ELSE 0 END) rTongNopThueTNDN
			,SUM(CASE WHEN bThoaiThu=0 THEN rPhiLePhi WHEN bThoaiThu=1 THEN rPhiLePhi*(-1) ELSE 0 END) rPhiLePhi
			,SUM(CASE WHEN bThoaiThu=0 THEN rTongNopNSNNKhac WHEN bThoaiThu=1 THEN rTongNopNSNNKhac*(-1) ELSE 0 END) rTongNopNSNNKhac
			,SUM(CASE WHEN bThoaiThu=0 THEN rNopNSNNKhacBQP WHEN bThoaiThu=1 THEN rNopNSNNKhacBQP*(-1) ELSE 0 END) rNopNSNNKhacBQP
			,SUM(CASE WHEN bThoaiThu=0 THEN rNopNSQP WHEN bThoaiThu=1 THEN rNopNSQP*(-1) ELSE 0 END) rNopNSQP
			,SUM(CASE WHEN bThoaiThu=0 THEN rBoSungKinhPhi WHEN bThoaiThu=1 THEN rBoSungKinhPhi*(-1) ELSE 0 END) rBoSungKinhPhi
			,SUM(CASE WHEN bThoaiThu=0 THEN rTrichQuyDonVi WHEN bThoaiThu=1 THEN rTrichQuyDonVi*(-1) ELSE 0 END) rTrichQuyDonVi

	FROM (

		SELECT	iID_MaPhongBan,iID_MaDonVi,sTenDonVi,SUBSTRING(sKyHieu,1,4) sKyHieuCap2, sKyHieu sKyHieuCap3, bThoaiThu, 
				rTongThu, rKhauHaoTSCD, rTienLuong, rQTNSKhac, rChiPhiKhac, rNopThueGTGT, rTongNopThueTNDN, rPhiLePhi, rTongNopNSNNKhac, rNopNSNNKhacBQP, rNopNSQP, rBoSungKinhPhi, rTrichQuyDonVi
		FROM	TN_ChungTuChiTiet 
		WHERE	iTrangThai=1 AND iLoai=2 AND iNamLamViec = @iNamLamViec AND sKyHieu LIKE '81%' 
				AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi))) 
				AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan) 
				AND (@iThang_Quy IS NULL OR iThang_Quy IN (SELECT * FROM F_Split(@iThang_Quy)))
				AND ((@username = 'chuctc2' and sID_MaNguoiDungTao = 'chuctc2') or (@username in ('trolyphongban1','nhanlh') and (iID_MaPhongBan <> '07' or (iID_MaPhongBan = '07' and sID_MaNguoiDungTao = 'chuctc2'))) or (@username not in ('chuctc2','trolyphongban1','nhanlh') and sID_MaNguoiDungTao <> 'chuctc2') or (@username like 'trolyphongbanb%' and sID_MaNguoiDungTao <> 'trolyphongbanb%'))

		UNION ALL

		SELECT	iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sKyHieu sKyHieuCap2, sKyHieu sKyHieuCap3, bThoaiThu, 
				rTongThu, rKhauHaoTSCD, rTienLuong, rQTNSKhac, rChiPhiKhac, rNopThueGTGT, rTongNopThueTNDN, rPhiLePhi, rTongNopNSNNKhac, rNopNSNNKhacBQP, rNopNSQP, rBoSungKinhPhi, rTrichQuyDonVi
		FROM	TN_ChungTuChiTiet 
		WHERE	iTrangThai=1 AND iLoai=2 AND iNamLamViec = @iNamLamViec AND sKyHieu LIKE '82%' 
				AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi))) 
				AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan) 
				AND (@iThang_Quy IS NULL OR iThang_Quy IN (SELECT * FROM F_Split(@iThang_Quy)))
				AND ((@username = 'chuctc2' and sID_MaNguoiDungTao = 'chuctc2') or (@username in ('trolyphongban1','nhanlh') and (@iNamLamViec <> '2019' or (@iNamLamViec = '2019' and (iID_MaPhongBan <> '07' or (iID_MaPhongBan = '07' and sID_MaNguoiDungTao = 'chuctc2'))))) or (@username not in ('chuctc2','trolyphongban1','nhanlh') and sID_MaNguoiDungTao <> 'chuctc2') or (@username like 'trolyphongbanb%' and sID_MaNguoiDungTao <> 'trolyphongbanb%'))		
	) AS I 
	GROUP BY iID_MaDonVi, sTenDonVi,sKyHieuCap2,sKyHieuCap3,iID_MaPhongBan

	HAVING	SUM(CASE WHEN bThoaiThu=0 THEN rTongThu WHEN bThoaiThu=1 THEN rTongThu*(-1) ELSE 0 END) <>0 OR 
			SUM(CASE WHEN bThoaiThu=0 THEN rTongThu WHEN bThoaiThu=1 THEN rTongThu*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rKhauHaoTSCD WHEN bThoaiThu=1 THEN rKhauHaoTSCD*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rTienLuong WHEN bThoaiThu=1 THEN rTienLuong*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rQTNSKhac WHEN bThoaiThu=1 THEN rQTNSKhac*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rChiPhiKhac WHEN bThoaiThu=1 THEN rChiPhiKhac*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rNopThueGTGT WHEN bThoaiThu=1 THEN rNopThueGTGT*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rTongNopThueTNDN WHEN bThoaiThu=1 THEN rTongNopThueTNDN*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rPhiLePhi WHEN bThoaiThu=1 THEN rPhiLePhi*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rTongNopNSNNKhac WHEN bThoaiThu=1 THEN rTongNopNSNNKhac*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rNopNSNNKhacBQP WHEN bThoaiThu=1 THEN rNopNSNNKhacBQP*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rNopNSQP WHEN bThoaiThu=1 THEN rNopNSQP*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rBoSungKinhPhi WHEN bThoaiThu=1 THEN rBoSungKinhPhi*(-1) ELSE 0 END) <>0 OR
			SUM(CASE WHEN bThoaiThu=0 THEN rTrichQuyDonVi WHEN bThoaiThu=1 THEN rTrichQuyDonVi*(-1) ELSE 0 END) <>0
) AS O
INNER JOIN TN_DanhMucLoaiHinh AS B ON O.sKyHieuCap3 = B.sKyHieu INNER JOIN TN_DanhMucLoaiHinh AS C ON O.sKyHieuCap2=C.sKyHieu
order by iID_MaPhongBan,iID_MaDonVi,sKyHieuCap2,sKyHieuCap3