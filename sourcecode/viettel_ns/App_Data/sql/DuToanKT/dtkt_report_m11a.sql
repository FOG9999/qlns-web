dtkt_report_m11a
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban=NULL
declare @lns		nvarchar(200)					set @lns = '104,109'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @id_nganh		nvarchar(200)				set @id_nganh = null
declare @id_donvi		nvarchar(200)				set @id_donvi = null
declare @request		int							set @request = 0
declare @nganh nvarchar(200)						set @nganh=null



--###--

-- CHUNG TU PHAN CAP BO SUNG DOT NGAY 30/3
declare @iID_MaChungTu nvarchar(2000)					set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'

select	Ng, TenNganh,
		TuChi		=sum(TuChi)/@dvt,
		HangNhap	=sum(HangNhap)/@dvt,
		HangMua		=sum(HangMua)/@dvt,
		PhanCap		=sum(PhanCap)/@dvt,	
		DuPhong		=sum(DuPhong)/@dvt,	
		DacThu		=sum(DacThu)/@dvt,		-- dac thu tai don vi
		DacThu1		=sum(DacThu1)/@dvt,		-- dac thu tai nganh
		TuChi1		=sum(TuChi1)/@dvt,	
		HangMua1	=sum(HangMua1)/@dvt,	
		HangNhap1	=sum(HangNhap1)/@dvt,	
		PhanCap1	=sum(PhanCap1)/@dvt
 from
(

--du toan dau nam
select	sNG as Ng,
		TuChi		=sum(rTuChi),
		HangNhap	=sum(rHangNhap),
		HangMua		=sum(rHangMua),
		PhanCap		=sum(rPhanCap),	
		DuPhong		=sum(rDuPhong),	
		DacThu		=0,
		DacThu1		=0,
		TuChi1		=0,
		HangMua1	=0,
		HangNhap1	=0,
		PhanCap1	=0
from
(

SELECT	--sL,sK,sM,sTM,sTTM,sNG,
        --sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
		sNG,
		rTuChi,
		rHangNhap,
		rHangMua,
		rPhanCap,
		rDuPhong
FROM    DT_ChungTuChiTiet 
WHERE   
        iTrangThai=1
        AND iNamLamViec=@nam
        AND iID_MaNamNganSach=2 
        AND iID_MaNguonNganSach=1
		--AND iKyThuat=0
        AND (MaLoai in ('','2'))
		--AND (LEFT(sLNS,3) in (104))
		AND sLNS='1040100'
		AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
        AND (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		AND (@nganh is null or sNG in (select * from f_split(@nganh)))
		--and iID_MaChungTuChiTiet not in (
		--						select iID_MaChungTu from DT_ChungTuChiTiet_PhanCap
		--						where (iID_MaChungTu in (select iID_MaChungTuChiTiet from DT_ChungTuChiTiet where iNamLamViec=@nam and MaLoai='1' and iKyThuat=1)) and rTuChi<>0) 


--du toan bo sung dot ngay 30/3
union all

select sNG, 
		rTuChi,
		rHangNhap,
		rHangMua,
		rPhanCap,
		rDuPhong 
from	DTBS_ChungTuChiTiet
where	iID_MaChungTu=@iID_MaChungTu
        and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)


----toi thoi diem hien tai
--union all
--select	sNG, 
--		rTuChi,
--		rHangNhap,
--		rHangMua,
--		rPhanCap,
--		rDuPhong 
--from	DTBS_ChungTuChiTiet
--where	iTrangThai=1 and
--		iNamLamViec=2018
--        and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		and iID_MaChungTu in (select * from f_ns_dtbs_chungtugom_dendot(@nam,'2',@id_phongban,GETDATE()))


--union all
--select	sNG, 
--		rTuChi,
--		rHangNhap,
--		rHangMua,
--		rPhanCap,
--		rDuPhong
--from	DTBS_ChungTuChiTiet_PhanCap
--where	iTrangThai=1 
--		and iNamLamViec=@nam
--		and MaLoai in ('','2')
--		and iID_MaNamNganSach=2
--		and iID_MaNguonNganSach=1
--		--and (@nganh is null or sNG in (select * from f_split(@nganh)))
--        and sLNS='1020100' 
--		and (
--				iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
			 
--			OR	-- phan cap cho b
--				iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--								 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
								 
				
--			OR -- phan cap lan 2
--				iID_MaChungTu in (	select iID_MaChungTuChiTiet 
--										from DTBS_ChungTuChiTiet 
--										where iTrangThai=1 and iID_MaChungTu in (
--													select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--													where iTrangThai=1 and iID_MaChungTu in (   
--																			select iID_MaChungTu from DTBS_ChungTu
--																			where iTrangThai=1 and iID_MaChungTu in (
--																									select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--												dtkt_report_m11a													where iTrangThai=1 and iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))


			
--			OR -- PHAN CAP GUI B KHAC
--				iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--									where iID_MaChungTu in (   
--															select iID_MaChungTu from DTBS_ChungTu
--															where iID_MaChungTu in (
--																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--																					where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))
--			)


) as dt
group by  sNG

union all

--du toan kiem tra

-- nganh bao dam
SELECT	Nganh as Ng,
 
		TuChi		=0,
		HangNhap	=0,
		HangMua		=0,
		PhanCap		=0,	
		DuPhong		=0,	
		DacThu		=0,
		DacThu1		=sum(DacThu + DacThu_HM + DacThu_HN),
		TuChi1		=case @request when 0 then sum(TuChi) when 1 then sum(TuChi + TangNV - GiamNV) end,
		HangMua1	=sum(HangMua),
		HangNhap1	=sum(HangNhap),
		PhanCap1	=0
FROM    DTKT_ChungTuChiTiet 
WHERE   iTrangThai=1    
        AND NamLamViec=@nam
		--AND iRequest=0
        AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		AND (@nganh is null or Nganh in (select * from f_split(@nganh)))
		AND Nganh not in ('00')
		AND iLoai=2
group by Nganh


-- dac thu nhap theo don vi, khong phu thuoc vao PhongBan de nghi
union all

-- nganh bao dam
SELECT	Nganh as Ng,
		TuChi		=0,
		HangNhap	=0,
		HangMua		=0,
		PhanCap		=0,	
		DuPhong		=0,	
		DacThu		=sum(DacThu),
		DacThu1		=0,
		TuChi1		=0,
		HangMua1	=0,
		HangNhap1	=0,
		PhanCap1	=0
FROM    DTKT_ChungTuChiTiet 
WHERE   iTrangThai=1    
        AND NamLamViec=@nam
		--AND iRequest=0
        AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		AND (@nganh is null or Nganh in (select * from f_split(@nganh)))
		AND Nganh not in ('00')
		AND iLoai=1
group by Nganh

union all
-- ngan sach su dung -> truc tiep
SELECT	Nganh as Ng,
		TuChi		=0,
		HangNhap	=0,
		HangMua		=0,
		PhanCap		=0,	
		DuPhong		=0,	
		DacThu		=0,
		DacThu1		=0,
		TuChi1		=0,
		HangMua1	=0,
		HangNhap1	=0,
		PhanCap1	= case @request when 0 then sum(TuChi) when 1 then sum(TuChi + TangNV - GiamNV) end
FROM    DTKT_ChungTuChiTiet 
WHERE   iTrangThai=1    
        AND NamLamViec=@nam
		--AND iRequest=0
		--AND iRequest=@request
        --AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		AND (@nganh is null or Nganh in (select * from f_split(@nganh)))
		AND Nganh not in ('00')
		AND iLoai=1
group by Nganh

) as a

left join (select sNG as id, sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@nam and sLNS='') as mucluc
on a.NG = mucluc.id

where TenNganh is not  null
group by  NG, TenNganh
order by NG





---- nganh bao dam
--SELECT	Nganh as Ng,
--		TuChi		=0,
--		HangNhap	=0,
--		HangMua		=0,
--		PhanCap		=0,	
--		DuPhong		=0,	
--		--DacThu		=sum(DacThu),
--		DacThu		=0,
--		TuChi1		=sum(TuChi),
--		HangMua1	=sum(HangMua),
--		HangNhap1	=sum(HangNhap),
--		PhanCap1	=0
--FROM    DTKT_ChungTuChiTiet 
--WHERE   iTrangThai=1    
--        AND NamLamViec=@nam
--		AND iRequest=@request
--        AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
--		AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
--		AND Nganh not in ('00')
--		AND iLoai=2
--		AND Nganh='11'
--group by Nganh
