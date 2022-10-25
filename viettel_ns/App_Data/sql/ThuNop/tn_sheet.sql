
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu='90518505-2f59-4e8a-8b66-661c9be3c770'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code=null
declare @byNg nvarchar(1000)						set @byNg='29'
declare @loai nvarchar(2)							set @loai='2'

--#DECLARE#--

 
select	iID_MaDonVi
		, iID_MaLoaiHinh
		, iID_MaLoaiHinh_Cha
		, bLaHangCha
		, iID_MaChungTuChiTiet
		, sSoCT
		, bThoaiThu
		, sKyHieu
		, sMoTa
		, rTongThu = ISNULL(rTongThu, 0)
		, rTongChiPhi = ISNULL ((rTongQTNS + rChiPhiKhac), 0)
		, rTongQTNS = ISNULL(rTongQTNS, 0)
		, rKhauHaoTSCD = ISNULL(rKhauHaoTSCD, 0)
		, rTienLuong = ISNULL(rTienLuong, 0)
		, rQTNSKhac = ISNULL(rQTNSKhac, 0)
		, rChiPhiKhac = ISNULL (rChiPhiKhac, 0)
		, rTongNopNSNN = ISNULL(rTongNopNSNN, 0)
		, rNopThueGTGT = ISNULL(rNopThueGTGT, 0)
		, rTongNopThueTNDN = ISNULL(rTongNopThueTNDN, 0)
		, rNopThueTNDNBQP = ISNULL(rNopThueTNDNBQP, 0)
		, rPhiLePhi = ISNULL(rPhiLePhi, 0)
		, rTongNopNSNNKhac = ISNULL(rTongNopNSNNKhac, 0)
		, rNopNSNNKhacBQP = ISNULL(rNopNSNNKhacBQP, 0)
		, rChenhLech = ISNULL((rTongThu - rTongQTNS - rChiPhiKhac - rTongNopNSNN), 0)
		, rNopNSQP = ISNULL(rNopNSQP,0)
		, rBoSungKinhPhi = ISNULL(rBoSungKinhPhi,0)
		, rTrichQuyDonVi = ISNULL(rTrichQuyDonVi,0)
		, rSoChuaPhanPhoi = ISNULL((rTongThu - rTongQTNS - rChiPhiKhac - rTongNopNSNN - rNopNSQP - rBoSungKinhPhi - rTrichQuyDonVi),0)
		, sGhiChu
from	(
		select	iID_MaLoaiHinh
				, iID_MaLoaiHinh_Cha
				, bLaHangCha
				, sKyHieu
				, sTen as sMoTa 
		from	TN_DanhMucLoaiHinh
		where	iTrangThai = 1) as ml

		left join 

		(	
		select	iID_MaChungTuChiTiet
				, iID_MaLoaiHinh as iID_MaMucLuc
				, iID_MaDonVi
				, bThoaiThu
				, sSoCT
				, rTongThu
				, rTongQTNS = rKhauHaoTSCD + rTienLuong + rQTNSKhac
				, rKhauHaoTSCD
				, rTienLuong
				, rQTNSKhac
				, rChiPhiKhac
				, rTongNopNSNN = rNopThueGTGT + rTongNopThueTNDN + rNopThueTNDNBQP + rPhiLePhi + rTongNopNSNNKhac + rNopNSNNKhacBQP
				, rNopThueGTGT
				, rTongNopThueTNDN
				, rNopThueTNDNBQP
				, rPhiLePhi
				, rTongNopNSNNKhac
				, rNopNSNNKhacBQP
				, rNopNSQP
				, rBoSungKinhPhi
				, rTrichQuyDonVi
				, sGhiChu 
		from	TN_ChungTuChiTiet 
		where	iTrangThai=1
				and (iID_MaChungTu is null or iID_MaChungTu = @Id_ChungTu)
		) as ct

		on ct.iID_MaMucLuc=ml.iID_MaLoaiHinh

order by ml.sKyHieu