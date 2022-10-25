
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iNamLamViecNS int							set @iNamLamViecNS = 2020
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @namTuChi nvarchar(20)						set @namTuChi='2,4'
declare @namDaCapTien nvarchar(20)					set @namDaCapTien='1'
declare @namChuaCapTien nvarchar(20)				set @namChuaCapTien='4'
declare @phuluc nvarchar(20)						set @phuluc='6'

--#DECLARE#--

;WITH C
as
(
SELECT  sLNS1 = LEFT(sLNS,1),
        sLNS3 = LEFT(sLNS,3),
        sLNS5 = LEFT(sLNS,5),
		sLNS,
		sL,sK,sM,sTM,sTTM,sNG, 
		(CONVERT(nvarchar(7),sLNS)	+'-'+
		 CONVERT(nvarchar(3),sL)	+'-'+
		 CONVERT(nvarchar(3),sK)	+'-'+
		 CONVERT(nvarchar(4),sM)	+'-'+
		 CONVERT(nvarchar(4),sTM)	+'-'+
		 CONVERT(nvarchar(2),sTTM)+'-'+
		 CONVERT(nvarchar(2),sNG))  as sXauNoiMa,
        iId_MaDonVi,sTenDonVi,
        DuToan_NT		=SUM(rDuToan_NT),
        DuToan			=SUM(rDuToan),
        QuyetToan		=SUM(rQuyetToan),
        DuToan_NamSau	=SUM(rDuToan_NamSau)
FROM
(

-- số quyết toán
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        rDuToan_NT		=0,
        rDuToan			=0,
        rQuyetToan		=SUM(rTuChi),
        rDuToan_NamSau	=0
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		and iThang_Quy<>5		
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

union all

-- 1. chi tieu năm trước
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG, 
        iId_MaDonVi, 
        rDuToan_NT		=SUM(rTuChi),
        rDuToan			=0,
        rQuyetToan		=0,
        rDuToan_NamSau	=0
FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'1,5',null,GETDATE(),1,null)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

union all

-- 2. Chỉ tiêu năm nay
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG, 
        iId_MaDonVi, 
        rDuToan_NT		=0,
        rDuToan			=SUM(rTuChi),
        rQuyetToan		=0,
        rDuToan_NamSau	=0
FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'2,4',null,GETDATE(),1,null)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

-- 3. chuyen nam sau
union all 
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG, 
        iId_MaDonVi, 
        rDuToan_NT		=0,
        rDuToan			=0,
        rQuyetToan		=0,
        rDuToan_NamSau	=SUM(rTuChi)
FROM f_ns_chitieu_full_tuchi(@iNamLamViec+1,@iID_MaDonVi,@iID_MaPhongBan,'1,4,5',null,GETDATE(),1,null)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi
) as T


-- lay ten don vi
left join (select iID_MaDonVi as id, sTenDonVi = iID_MaDonVi+' - '+sTen from ns_donvi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv
on T.iID_MaDonVi=dv.id

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi,sTenDonVi
HAVING  SUM(rDuToan_NT) <> 0 OR
        SUM(rQuyetToan) <> 0 OR
        SUM(rDuToan_NamSau) <> 0 OR
        SUM(rDuToan) <> 0 
)

select * from C
where dbo.f_tqt_check(sXauNoiMa,@phuluc)=1


