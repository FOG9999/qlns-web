declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='97'
declare @iThang_Quy nvarchar(20)					set @iThang_Quy='1'
declare @username nvarchar(20)						set @username='thuyct'

--#DECLARE#--/

SELECT	iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLoaiDvi 
		,COT2=(SUM(CASE WHEN (iID_MaCotBaoCao IN (2) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (2) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT3=(SUM(CASE WHEN (iID_MaCotBaoCao IN (3) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (3) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT4=(SUM(CASE WHEN (iID_MaCotBaoCao IN (4) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (4) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT5=(SUM(CASE WHEN (iID_MaCotBaoCao IN (5) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (5) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT6=(SUM(CASE WHEN (iID_MaCotBaoCao IN (6) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (6) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT7=(SUM(CASE WHEN (iID_MaCotBaoCao IN (7) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (7) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT8=(SUM(CASE WHEN (iID_MaCotBaoCao IN (8) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (8) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT9=(SUM(CASE WHEN (iID_MaCotBaoCao IN (9) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (9) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT10=(SUM(CASE WHEN (iID_MaCotBaoCao IN (10) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (10) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT11=(SUM(CASE WHEN (iID_MaCotBaoCao IN (11) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (11) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT12=(SUM(CASE WHEN (iID_MaCotBaoCao IN (12) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (12) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT13=(SUM(CASE WHEN (iID_MaCotBaoCao IN (13) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (13) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT14=(SUM(CASE WHEN (iID_MaCotBaoCao IN (14) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (14) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT15=(SUM(CASE WHEN (iID_MaCotBaoCao IN (15) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (15) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT16=(SUM(CASE WHEN (iID_MaCotBaoCao IN (16) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (16) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt)
		,COT17=(SUM(CASE WHEN (iID_MaCotBaoCao IN (17) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (17) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) / @dvt) 
FROM(                                        

	SELECT	iID_MaPhongBan, iID_MaDonVi, sTenDonVi, sKyHieu, bThoaiThu, rKhauHaoTSCD, rNopNSQP, Dvi.loaiDvi sLoaiDvi, CH.iID_MaCotBaoCao
	FROM	TN_ChungTuChiTiet as tn_dl, TN_CauHinhBaoCao as CH, @@DS_Dvi as Dvi 
	WHERE	tn_dl.iTrangThai=1 AND tn_dl.iNamLamViec=@iNamLamViec 
			AND CH.sLoaiNS = 'NSQP' AND CH.sLoaiHinh LIKE '%' + sKyHieu + '%' 
			AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi))) 
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan) 
			AND (@iThang_Quy IS NULL OR iThang_Quy IN (SELECT * FROM F_Split(@iThang_Quy)))
			AND iID_MaDonVi = Dvi.maDonVi 
			AND ((@username = 'chuctc2' and tn_dl.sID_MaNguoiDungTao = 'chuctc2') or (@username in ('trolyphongban1','nhanlh') and (@iNamLamViec <> '2019' or (@iNamLamViec = '2019' and (tn_dl.iID_MaPhongBan <> '07' or (tn_dl.iID_MaPhongBan = '07' and tn_dl.sID_MaNguoiDungTao = 'chuctc2'))))) or (@username not in ('chuctc2','trolyphongban1','nhanlh') and tn_dl.sID_MaNguoiDungTao <> 'chuctc2') or (@username like 'trolyphongbanb%' and tn_dl.sID_MaNguoiDungTao <> 'trolyphongbanb%'))
                                        
) AS R
GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLoaiDvi
HAVING	SUM(CASE WHEN (iID_MaCotBaoCao IN (2) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (2) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (3) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (3) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (4) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (4) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (5) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (5) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (6) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (6) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (7) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (7) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (8) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (8) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (9) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (9) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (10) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (10) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (11) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (11) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (12) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (12) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (13) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (13) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		(SUM(CASE WHEN (iID_MaCotBaoCao IN (14) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (14) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) + SUM(CASE WHEN (bThoaiThu=0) THEN (rKhauHaoTSCD) WHEN (bThoaiThu=1) THEN rKhauHaoTSCD*(-1)  ELSE 0 END)) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (15) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (15) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (16) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (16) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0 OR  
		SUM(CASE WHEN (iID_MaCotBaoCao IN (17) AND bThoaiThu=0) THEN (rNopNSQP) WHEN (iID_MaCotBaoCao IN (17) AND bThoaiThu=1) THEN rNopNSQP*(-1)  ELSE 0 END) <>0                                         
order by iID_MaPhongBan,iID_MaDonVi,sTenDonVi,sLoaiDvi                        