
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=NULL
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1,2'

 
--###--


SELECT  sLNS1 =LEFT(sLNS,1),
		sLNS3 =LEFT(sLNS,3),
		sLNS5 =LEFT(sLNS,5),
		sLNS, sMoTa,
		iID_MaDonVi, sTenDonVi, iID_MaKhoiDonVi, sTenKhoiDonVi,iSTT,
		rQuyetToan, rBoSung, rDonViDeNghi, rDaCapTien, rChuaCapTien 
FROM 
(
	SELECT	iID_MaDonVi,sTenDonVi,sLNS
			,rQuyetToan=SUM(CASE WHEN iThang_Quy <=4 THEN rTuChi ELSE 0 END)
			,rBoSung=SUM(CASE WHEN iThang_Quy =5 THEN rTuChi ELSE 0 END)
			,SUM(rDonViDeNghi) as rDonViDeNghi
			,SUM(rDaCapTien) as rDaCapTien
			,SUM(rChuaCapTien) as rChuaCapTien
	FROM	QTA_ChungTuChiTiet
	WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			AND (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
			AND iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
			AND (@iID_MaNguonNganSach is null or left(sLNS,1) in (select * from f_split(@iID_MaNguonNganSach)))
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
	GROUP BY iID_MaDonVi,sTenDonVi,sLNS
	HAVING SUM(CASE WHEN iThang_Quy <=5 THEN rTuChi ELSE 0 END) <>0
			OR SUM(rDonViDeNghi)<>0
			OR SUM(rDaCapTien)<>0
			OR SUM(rChuaCapTien)<>0
) qt

-- lay ten khoi don vi
inner join (
	SELECT iID_MaDonVi as dv_id,iID_MaKhoiDonVi,DM.sTen as sTenKhoiDonVi,iSTT 
	FROM(
		SELECT iID_MaDonVi,iID_MaKhoiDonVi FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) DV
		LEFT JOIN (SELECT iID_MaDanhMuc,sTen,iSTT 	FROM DC_DanhMuc WHERE iID_MaLoaiDanhMuc='bc5f9bb8-1dba-40fc-95b6-1098e5b0e68f') DM
		ON dv.iID_MaKhoiDonVi=DM.iID_MaDanhMuc
) as khoi
on qt.iID_MaDonVi=khoi.dv_id

-- lay mo ta lns
inner join (select sLNS as id, sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and LEN(sLNS)=7 and sL = '') as mlns
on qt.sLNS = mlns.id

order by sLNS1,sLNS3,sLNS5,iSTT,iID_MaDonVi
