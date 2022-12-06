
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='0dba0387-adce-4ef1-8609-b40d9fea1506'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @phongban nvarchar(2)						set @phongban=null
declare @loai nvarchar(2)							set @loai='2'

--#DECLARE#--

SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
		,sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		= case when @loai <> 8 then SUM(rHangNhap+rHangMua+rPhanCap+rDuPhong)/@dvt else sum(rTuChi+rPhanCap+rDuPhong)/@dvt end,
        rTuChi      =SUM(rTuChi)/@dvt, 
        rHangNhap   =SUM(rHangNhap)/@dvt, 
        rHangMua    =SUM(rHangMua)/@dvt, 
        rPhanCap    =SUM(rPhanCap)/@dvt,
		rDuPhong	=SUM(rDuPhong)/@dvt
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
		rTuChi      =SUM(rChiTaiNganh),
		rHangNhap	=SUM(rHangNhap),
		rHangMua	=SUM(rHangMua),
        rPhanCap    =SUM(rPhanCap),
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
        AND iID_MaDonVi=@iID_MaDonVi
		and (iID_MaPhongBanDich = @phongban or @phongban is null)
        AND (iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) or iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (select * from f_split(@iID_MaChungTu)) and iTrangThai = 1) and iTrangThai = 1))

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rChiTaiNganh)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rPhanCap)<>0
        OR SUM(rDuPhong)<>0

union all

		SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG = '23',sXauNoiMa = '1040100-010-011-6950-6954-10-23',
        iID_MaDonVi,
		rTuChi      =0,
		rHangNhap	=0,
		rHangMua	=0,
        rPhanCap    =SUM(-rHangNhap - rHangMua),
        rDuPhong    =0
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND MaLoai='2'
		AND iID_MaDonVi = @iID_MaDonVi
        AND @iID_MaDonVi='51'
		and (iID_MaPhongBanDich = @phongban or @phongban is null)
        AND (iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) or iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (select * from f_split(@iID_MaChungTu)) and iTrangThai = 1) and iTrangThai = 1))

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0
) as T1
WHERE ((LEFT(sLNS,1) in ('2','4') or LEFT(sLNS,3) in ('109')) and (rTuChi <> 0 or  rPhanCap <> 0 or  rDuPhong <> 0)  and @loai = 8)
		or (LEFT(sLNS,3) in ('104') and (rHangNhap <> 0 or rHangMua <> 0 or rPhanCap <> 0) and @loai = 2) 
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
