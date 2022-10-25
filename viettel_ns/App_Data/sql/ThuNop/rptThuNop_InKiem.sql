
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chinpth'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='17' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='10'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'

--#DECLARE#--

SELECT sKyHieuCap2,(' + ' + D.sTen) sTenLoaiHinhCap2, sKyHieuCap3,(' - ' + C.sTen) sTenLoaiHinhCap3, 
	   rTongThu, rKhauHaoTSCD, rTienLuong, rQTNSKhac, rChiPhiKhac, rNopThueGTGT, rTongNopThueTNDN, rPhiLePhi, 
	   rTongNopNSNNKhac, rNopNSNNKhacBQP, rNopNSQP, rBoSungKinhPhi, rTrichQuyDonVi

FROM(

    SELECT sKyHieuCap2,sKyHieuCap3
    ,(CASE WHEN bThoaiThu=0 THEN rTongThu WHEN bThoaiThu=1 THEN rTongThu*(-1) ELSE 0 END) rTongThu
    ,(CASE WHEN bThoaiThu=0 THEN rKhauHaoTSCD WHEN bThoaiThu=1 THEN rKhauHaoTSCD*(-1) ELSE 0 END) rKhauHaoTSCD
    ,(CASE WHEN bThoaiThu=0 THEN rTienLuong WHEN bThoaiThu=1 THEN rTienLuong*(-1) ELSE 0 END) rTienLuong
    ,(CASE WHEN bThoaiThu=0 THEN rQTNSKhac WHEN bThoaiThu=1 THEN rQTNSKhac*(-1) ELSE 0 END) rQTNSKhac
    ,(CASE WHEN bThoaiThu=0 THEN rChiPhiKhac WHEN bThoaiThu=1 THEN rChiPhiKhac*(-1) ELSE 0 END) rChiPhiKhac
    ,(CASE WHEN bThoaiThu=0 THEN rNopThueGTGT WHEN bThoaiThu=1 THEN rNopThueGTGT*(-1) ELSE 0 END) rNopThueGTGT
    ,(CASE WHEN bThoaiThu=0 THEN rTongNopThueTNDN WHEN bThoaiThu=1 THEN rTongNopThueTNDN*(-1) ELSE 0 END) rTongNopThueTNDN
    ,(CASE WHEN bThoaiThu=0 THEN rPhiLePhi WHEN bThoaiThu=1 THEN rPhiLePhi*(-1) ELSE 0 END) rPhiLePhi
    ,(CASE WHEN bThoaiThu=0 THEN rTongNopNSNNKhac WHEN bThoaiThu=1 THEN rTongNopNSNNKhac*(-1) ELSE 0 END) rTongNopNSNNKhac
    ,(CASE WHEN bThoaiThu=0 THEN rNopNSNNKhacBQP WHEN bThoaiThu=1 THEN rNopNSNNKhacBQP*(-1) ELSE 0 END) rNopNSNNKhacBQP
    ,(CASE WHEN bThoaiThu=0 THEN rNopNSQP WHEN bThoaiThu=1 THEN rNopNSQP*(-1) ELSE 0 END) rNopNSQP
    ,(CASE WHEN bThoaiThu=0 THEN rBoSungKinhPhi WHEN bThoaiThu=1 THEN rBoSungKinhPhi*(-1) ELSE 0 END) rBoSungKinhPhi
    ,(CASE WHEN bThoaiThu=0 THEN rTrichQuyDonVi WHEN bThoaiThu=1 THEN rTrichQuyDonVi*(-1) ELSE 0 END) rTrichQuyDonVi

    FROM (

		SELECT iID_MaPhongBan,iID_MaDonVi,sTenDonVi,SUBSTRING(sKyHieu,1,4) sKyHieuCap2, sKyHieu sKyHieuCap3, bThoaiThu, rTongThu, rKhauHaoTSCD, rTienLuong, rQTNSKhac, rChiPhiKhac, rNopThueGTGT, rTongNopThueTNDN, rPhiLePhi, rTongNopNSNNKhac, rNopNSNNKhacBQP, rNopNSQP, rBoSungKinhPhi, rTrichQuyDonVi
		FROM TN_ChungTuChiTiet WHERE iTrangThai=1 AND iLoai=2 AND iNamLamViec = @iNamLamViec AND sKyHieu LIKE '81%' AND iID_MaPhongBan = @iID_MaPhongBan AND iID_MaDonVi=@iID_MaDonVi

		UNION ALL

		SELECT iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sKyHieu sKyHieuCap2, sKyHieu sKyHieuCap3, bThoaiThu, rTongThu, rKhauHaoTSCD, rTienLuong, rQTNSKhac, rChiPhiKhac, rNopThueGTGT, rTongNopThueTNDN, rPhiLePhi, rTongNopNSNNKhac, rNopNSNNKhacBQP, rNopNSQP, rBoSungKinhPhi, rTrichQuyDonVi
		FROM TN_ChungTuChiTiet WHERE iTrangThai=1 AND iLoai=2 AND iNamLamViec = @iNamLamViec AND sKyHieu LIKE '82%' AND iID_MaPhongBan = @iID_MaPhongBan AND iID_MaDonVi=@iID_MaDonVi
								
    ) AS A 
) AS B INNER JOIN TN_DanhMucLoaiHinh AS C ON B.sKyHieuCap3 = C.sKyHieu INNER JOIN TN_DanhMucLoaiHinh AS D ON B.sKyHieuCap2=D.sKyHieu
Order by sKyHieuCap2,sKyHieuCap3