
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @lns nvarchar(200)							set @lns='2'
declare @sTM nvarchar(200)							set @sTM=null
declare @sTTM nvarchar(200)							set @sTTM=null
declare @sNG nvarchar(200)							set @sNG='72'

 
--###--


SELECT  sLNS1 =LEFT(sLNS,1),
		sLNS3 =LEFT(sLNS,3),
		sLNS5 =LEFT(sLNS,5),
		sLNS,sL,sK,sM,
		--sMoTa,
		sMoTa=dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,'','',''),
		sXauNoiMa = sLNS+'.'+sL+'.'+sK+'.'+sM,
		iID_MaDonVi, sTenDonVi, 
		
		rChuaCap_NamNay		=sum(rChuaCap_NamNay),
		rChuaCap_NamTruoc	=sum(rChuaCap_NamTruoc),
		rDaCap_NamNay		=sum(rDaCap_NamNay),
		rDaCap_NamTruoc		=sum(rDaCap_NamTruoc)
FROM 
(
	--SELECT	iID_MaDonVi,sLNS,sL,sK,sM,
	--		rChuaCap_NamNay=SUM(CASE WHEN iID_MaNamNganSach=4 THEN (rTuChi + rHangNhap+rHangMua) ELSE 0 END),
	--		--rChuaCap_NamTruoc=SUM(CASE WHEN iID_MaNamNganSach=1 THEN rChuaCapTien ELSE 0 END),
	--		rChuaCap_NamTruoc=0,
	--		rDaCap_NamNay=SUM(CASE WHEN iID_MaNamNganSach=1 THEN (rTuChi + rHangNhap+rHangMua) ELSE 0 END),
	--		rDaCap_NamTruoc=SUM(CASE WHEN iID_MaNamNganSach in (5) THEN (rTuChi + rHangNhap+rHangMua) ELSE 0 END)
	--FROM	DTBS_ChungTuChiTiet
	--WHERE	iTrangThai=1 
	--		AND iNamLamViec=@iNamLamViec
	--		AND (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
	--		AND (@iID_MaDonVi is  null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
	--		AND LEFT(sLNS,1) in (select * from f_split(@lns))
	--		AND (@sTM is null or sTM=@sTM)
	--		AND (@sTTM is null or sTTM=@sTTM)
	--		AND (@sNG is null or sNG=@sNG)
	--GROUP BY iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM
	--HAVING sum(rTuChi)<>0 or sum(rHangNhap)<>0 or sum(rHangMua)<>0

	 -- chuyen nam sau - chưa cấp tiền
	 SELECT	iID_MaDonVi,sLNS,sL,sK,sM
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=sum(rTuChi)
			,rDaCap_NamTruoc	=0
			,rDaCap_NamNay		=0
	FROM	f_ns_bs(@iNamLamViec+1,@iID_MaDonVi,@iID_MaPhongBan,'4')
	GROUP BY iID_MaDonVi,sLNS,sL,sK,sM

	 -- chuyen nam sau - da cap tien truoc
	 union all
	 SELECT	iID_MaDonVi,sLNS,sL,sK,sM
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=0
			,rDaCap_NamTruoc	=sum(rTuChi)
			,rDaCap_NamNay		=0
	FROM	f_ns_bs(@iNamLamViec+1,@iID_MaDonVi,@iID_MaPhongBan,'5')
	GROUP BY iID_MaDonVi,sLNS,sL,sK,sM

	 -- chuyen nam sau - da cap tien
	 union all
	 SELECT	iID_MaDonVi,sLNS,sL,sK,sM
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=0
			,rDaCap_NamTruoc	=0
			,rDaCap_NamNay		=sum(rTuChi)
	FROM	f_ns_bs(@iNamLamViec+1,@iID_MaDonVi,@iID_MaPhongBan,'1')
	GROUP BY iID_MaDonVi,sLNS,sL,sK,sM

) qt


-- lay ten don vi
inner join (SELECT iID_MaDonVi as dv_id, sTenDonVi = iID_MaDonVi + ' - ' + sTen FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as dv
on qt.iID_MaDonVi=dv.dv_id

 --lay mo ta lns
--join (select sXauNoiMa as id, sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec) as mlns
--on mlns.id = (sLNS+'-'+sL+'-'+sK+'-'+sM)

where	(rChuaCap_NamNay<> 0 or rChuaCap_NamTruoc<>0 or rDaCap_NamNay<>0 or rDaCap_NamTruoc<>0)
		and (@lns is null or left(sLNS,1) in (select * from f_split(@lns)))
GROUP BY iID_MaDonVi,sLNS,sL,sK,sM,sTenDonVi

order by sLNS,sM,iID_MaDonVi
