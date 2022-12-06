
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan='17'
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach=null
declare @lns nvarchar(20)							set @lns=null

 
--###--

declare @NamNS_DaCap		nvarchar(10)
declare @NamNS_ChuaCap		nvarchar(10)

if @iID_MaNamNganSach='1,5' 
begin
	set @NamNS_DaCap='5'
	set @NamNS_ChuaCap=''
end
else if @iID_MaNamNganSach='2,4'
begin
	set @NamNS_DaCap='1'
	set @NamNS_ChuaCap='4'
end
else
begin
	set @NamNS_DaCap='1,5'
	set @NamNS_ChuaCap='4'
end



-- chỉnh đơn vị do b6 có bệnh viện tự chủ
--if @iID_MaDonVi=50 begin set @iID_MaDonVi='50,501,502' end
--else if @iID_MaDonVi=34 begin set @iID_MaDonVi='34,341,342' end

--select @iID_MaDonVi


SELECT  sLNS1 =LEFT(sLNS,1),
		sLNS3 =LEFT(sLNS,3),
		sLNS5 =LEFT(sLNS,5),
		sLNS, sMoTa,

		iID_MaDonVi,
		 sTenDonVi, 
		rQuyetToan
		,rDonViDeNghi	
		,rVuotChiTieu
		,rDaCapTien		
		,rChuaCapTien	
		,iID_MaPhongBan as PhongBan
		,sMoTa_PhongBan
		,sMoTa_PhongBan_Khoi =	case iID_MaPhongBan 
								when	'05' then N'Khối XDCB'
								when	'06' then N'Khối doanh nghiệp'
								when	'07' then N'Khối QK - QĐ - QBC'
								when	'08' then N'Khối Chế độ chính sách'
								when	'16' then N'Khối QL Các dự án'
								when	'17' then N'Khối QL Công sản'
								when	'10' then N'Khối Tổng cục - BTTM'
								else	iID_MaPhongBan end
FROM 
(
	
	select 
		iID_MaDonVi		=case when iID_MaDonVi like '[0-9]%' then left(iID_MaDonVi,2) else iID_MaDonVi end
		,sLNS=case sLNS when '3010001' then '3010000' else sLNS end
		,rQuyetToan		=sum(rQuyetToan)
		,rDonViDeNghi	=sum(rDonViDeNghi)
		,rVuotChiTieu	=sum(rVuotChiTieu)
		,rDaCapTien		=sum(rDaCapTien)
		,rChuaCapTien	=sum(rChuaCapTien)
		,iID_MaPhongBan =case iID_MaPhongBan when '16' then '05' else iID_MaPhongBan end
	from
	(
	
		-- so quyet toan: rDonViDeNghi (duyet theo chi tieu), rBoSung (Nhap quy 5)
	SELECT	iID_MaDonVi
			,sLNS=case sLNS when '3010001' then '3010000' else sLNS end
			,iID_MaPhongBan

			-- lay toan bo so quyet toan (1-5)
			,rQuyetToan	=sum(rTuChi)
			,SUM(rDonViDeNghi) as rDonViDeNghi
			,SUM(rVuotChiTieu) as rVuotChiTieu
			,rDaCapTien =0
			,rChuaCapTien =0
	FROM	QTA_ChungTuChiTiet
	WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			and iThang_Quy<>5
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
			AND iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
			AND (@iID_MaNguonNganSach is null or left(sLNS,1) in (select * from f_split(@iID_MaNguonNganSach)))
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
	GROUP BY iID_MaDonVi,sTenDonVi,sLNS,iID_MaPhongBan

	HAVING SUM(CASE WHEN iThang_Quy <=5 THEN rTuChi ELSE 0 END) <>0
			OR SUM(rDonViDeNghi)<>0
			OR SUM(rVuotChiTieu)<>0

	union all

	-- so quyet toan bổ sung quý V
	SELECT	iID_MaDonVi
			,sLNS=case sLNS when '3010001' then '3010000' else sLNS end
			,iID_MaPhongBan

			,rQuyetToan	=0
			,SUM(rDonViDeNghi) as rDonViDeNghi
			,SUM(rTuChi) as rVuotChiTieu
			,rDaCapTien =0
			,rChuaCapTien =0
	FROM	QTA_ChungTuChiTiet
	WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			and iThang_Quy=5
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
			AND iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
			AND (@iID_MaNguonNganSach is null or left(sLNS,1) in (select * from f_split(@iID_MaNguonNganSach)))
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
	GROUP BY iID_MaDonVi,sTenDonVi,sLNS,iID_MaPhongBan

	HAVING	SUM(rDonViDeNghi)<>0
			OR SUM(rTuChi)<>0
	-- lay so da cap tien va chua cap tien chuyen nam sau tu du toan
	union all
	-- 2. nam sau - da cap tien
	SELECT  iID_MaDonVi,sLNS=case sLNS when '3010001' then '3010000' else sLNS end
			,iID_MaPhongBan
			,rQuyetToan = 0
			,rDonViDeNghi = 0
			,rVuotChiTieu = 0
			,sum(rDaCapTien) as rDaCapTien
			,rChuaCapTien = 0 
	FROM    
	(
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan				
				,rDaCapTien = sum(rTuChi)
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_DaCap))

				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan

		-- hang nhap
		union all
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan				
				,rDaCapTien = case sLNS when '1040200' then SUM(rHangNhap) else 0 end
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_DaCap))
				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan

		-- hang mua
		union all
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan
				,rDaCapTien = case sLNS when '1040300' then SUM(rHangMua) else 0 end
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_DaCap))

				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan
	) as dt_namsau_dacaptien
	GROUP BY iID_MaDonVi,sLNS,iID_MaPhongBan

	union all 
	---- 3. nam sau - chua cap tien
	SELECT  iID_MaDonVi,sLNS=case sLNS when '3010001' then '3010000' else sLNS end
			,iID_MaPhongBan
			,rQuyetToan = 0
			,rDonViDeNghi = 0
			,rVuotChiTieu = 0
			,rDaCapTien=0
			,sum(rChuaCapTien) as rChuaCapTien
	FROM    
	(
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan
				,rChuaCapTien = sum(rTuChi)
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				--and (iID_MaNamNganSach in ('4'))
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_ChuaCap))
				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan

		-- hang nhap
		union all
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan
				,rChuaCapTien = case sLNS when '1040200' then SUM(rHangNhap) else 0 end
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_ChuaCap))

				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan

		-- hang mua
		union all
		-- nam sau cap ve
		SELECT  iID_MaDonVi,sLNS
				,iID_MaPhongBan
				,rChuaCapTien = case sLNS when '1040300' then SUM(rHangMua) else 0 end
		from	DTBS_ChungTuChiTiet
		where	iTrangThai=1
				and iNamLamViec=@iNamLamViec+1
				AND iID_MaNamNganSach in (select * from f_split(@NamNS_ChuaCap))

				and (@iID_MaPhongBan is null or iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)))
				and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				and MaLoai in ('','2')
				and iBKhac=0
		GROUP BY iID_MaDonVi,sLNS
				,iID_MaPhongBan
	) as dt_namsau_dacaptien
	GROUP BY iID_MaDonVi,sLNS,iID_MaPhongBan

	) as aaa
	GROUP BY iID_MaDonVi,sLNS,iID_MaPhongBan
	

) qt

---- lay ten khoi don vi
--inner join (
--	SELECT iID_MaDonVi as dv_id,iID_MaKhoiDonVi,DM.sTen as sTenKhoiDonVi,iSTT 
--	FROM(
--		SELECT iID_MaDonVi,iID_MaKhoiDonVi FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) DV
--		LEFT JOIN (SELECT iID_MaDanhMuc,sTen,iSTT 	FROM DC_DanhMuc WHERE iID_MaLoaiDanhMuc='bc5f9bb8-1dba-40fc-95b6-1098e5b0e68f') DM
--		ON dv.iID_MaKhoiDonVi=DM.iID_MaDanhMuc
--) as khoi
--on qt.iID_MaDonVi=khoi.dv_id

-- lay mo ta lns
inner join (select sLNS as id, sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and LEN(sLNS)=7 and sL = '') as mlns
on qt.sLNS = mlns.id

-- lay ten phongban
inner join (select sKyHieu as id, 'B'+sKyHieu + ' - ' + sMoTa as sMoTa_PhongBan from NS_PhongBan where iTrangThai=1) as phongban
on phongban.id=qt.iID_MaPhongBan

-- lay ten donvi
inner join (select iID_MaDonVi as id, sTen as sTenDonVi from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec ) as dv
on dv.id=qt.iID_MaDonVi

where @lns is null or left(sLNS,1) in (select * from f_split(@lns))
order by iID_MaPhongBan,sMoTa_PhongBan,sLNS1,sLNS3,sLNS5,iID_MaDonVi
