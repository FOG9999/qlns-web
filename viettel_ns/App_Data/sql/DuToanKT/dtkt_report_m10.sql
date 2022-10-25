
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'

declare @sNG		nvarchar(200)					set @sNG = '22'
declare @loai		int								set @loai=1		--0: tat ca		1: phancap
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47'

--###-- 


SELECT  iID_MaDonVi, TenDonVi,iID_MaKhoiDonVi,TenKhoiDonVi,iSTT_MaKhoiDonVi,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,CT.sMoTa,
        sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
		loai,
        rTuChi,
        rHienVat,
		id = sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG + '-' + sLNS
FROM 
(

    SELECT  iID_MaDonVi,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
			loai=1,
            SUM(rTuChi+rHangMua+rHangNhap)/@dvt AS rTuChi,
            SUM(rHienVat/@dvt) AS rHienVat 
    FROM    DT_ChungTuChiTiet 
    WHERE   
            iTrangThai=1
            --AND iKyThuat=1 
            --AND MaLoai=1  
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=2
            AND iID_MaNguonNganSach=1
            --AND (sLNS LIKE '2%' or left(sLNS,3) like '405%')
			AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
            AND (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
			
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
    HAVING  SUM(rTuChi)>0 OR SUM(rHienVat)>0

	UNION

	SELECT  iID_MaDonVi,
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,
            sM +'.'+ sTM +'.'+ sTTM +'.'+ sNG AS NG,
			loai=2,
            SUM(rTuChi+rHangMua+rHangNhap)/@dvt AS rTuChi,
            SUM(rHienVat/@dvt) AS rHienVat

    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1  
            --AND MaLoai<>'1' AND MaLoai <>'3'  
            --AND (sLNS LIKE '2%' or left(sLNS,3) like '405%')
            AND iNamLamViec=@iNamLamViec
            AND iID_MaNamNganSach=2
            AND iID_MaNguonNganSach=1
			AND (@sNG is null or sNG IN (SELECT * FROM f_split(@sNG)))
            --AND iID_MaPhongBanDich=@iID_MaPhongBan
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,iID_MaDonVi,sMoTa 
    HAVING SUM(rTuChi)>0 OR SUM(rHienVat)>0 


) AS CT  

LEFT JOIN (
select dv_id, TenDonVi, khoi.sTen as TenKhoiDonVi, iID_MaKhoiDonVi, khoi.iSTT as iSTT_MaKhoiDonVi from
	(select iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as TenDonVi, iID_MaKhoiDonVi from NS_DonVi where iNamLamViec_DonVi=2018) as dv
	left join (select * from DC_DanhMuc where iID_MaLoaiDanhMuc in (select iID_MaLoaiDanhMuc from DC_LoaiDanhMuc where sTenBang=N'KhoiDonVi')) as khoi
	on khoi.iID_MaDanhMuc=dv.iID_MaKhoiDonVi)
	as NS_DonVi

ON NS_DonVi.dv_id=CT.iID_MaDonVi
