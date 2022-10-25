
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(200)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='50'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2,4'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1,2'

 
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


SELECT	iID_MaDonVi,
		sTenDonVi, 
		NSQP = sum(NSQP),
		NSNN = sum(NSNN),
		NSDB = sum(NSDB),
		KPK = sum(KPK)
FROM 
(
	
	select 
		iID_MaDonVi		=case when iID_MaDonVi like '[0-9]%' then left(iID_MaDonVi,2) else iID_MaDonVi end
		,sLNS
		, NSQP = case when sLNS like '1%' then sum(rQuyetToan + rVuotChiTieu) else 0 end
		, NSNN = case when sLNS like '2%' then sum(rQuyetToan + rVuotChiTieu) else 0 end
		, NSDB = case when sLNS like '3%' then sum(rQuyetToan + rVuotChiTieu) else 0 end
		, KPK = case when sLNS like '4%' then sum(rQuyetToan + rVuotChiTieu) else 0 end
	from
	(
	
		-- so quyet toan: rDonViDeNghi (duyet theo chi tieu), rBoSung (Nhap quy 5)
	SELECT	iID_MaDonVi
			,sLNS

			-- lay toan bo so quyet toan (1-5)
			,rQuyetToan	= sum(rTuChi)
			,SUM(rVuotChiTieu) as rVuotChiTieu
	FROM	QTA_ChungTuChiTiet
	WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			and iThang_Quy<>5
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
			AND iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
	GROUP BY iID_MaDonVi,sTenDonVi,sLNS,iID_MaPhongBan

	HAVING SUM(CASE WHEN iThang_Quy <> 5 THEN rTuChi ELSE 0 END) <>0
			OR SUM(rVuotChiTieu)<>0

	union all

	-- so quyet toan bổ sung quý V
	SELECT	iID_MaDonVi
			,sLNS
			,rQuyetToan	=0
			,SUM(rTuChi) as rVuotChiTieu
	FROM	QTA_ChungTuChiTiet
	WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			and iThang_Quy=5
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
			AND iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
	GROUP BY iID_MaDonVi,sTenDonVi,sLNS

	HAVING	SUM(rTuChi)<>0
	-- lay so da cap tien va chua cap tien chuyen nam sau tu du toan

	) as aaa
	GROUP BY iID_MaDonVi,sLNS
	

) qt

-- lay ten donvi
inner join (select iID_MaDonVi as id, sTen as sTenDonVi from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec ) as dv
on dv.id=qt.iID_MaDonVi

group by iID_MaDonVi, sTenDonVi
order by iID_MaDonVi
