
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='16'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='33'

 
--###--


SELECT  sLNS1 =LEFT(sLNS,1),
		sLNS3 =LEFT(sLNS,3),
		sLNS5 =LEFT(sLNS,5),
		sLNS, sMoTa,
		iID_MaDonVi, sTenDonVi, 
		rChuaCap_NamNay		=sum(rChuaCap_NamNay),
		rChuaCap_NamTruoc	=sum(rChuaCap_NamTruoc),
		rDaCap_NamNay		=sum(rDaCap_NamNay),
		rDaCap_NamTruoc		=sum(rDaCap_NamTruoc)
FROM 
(
	 -- chuyen nam sau - chưa cấp tiền
	 SELECT	iID_MaDonVi,sLNS
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=sum(rTuChi)
			,rDaCap_NamTruoc	=0
			,rDaCap_NamNay		=0
	FROM	f_ns_bs(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'4')
	GROUP BY iID_MaDonVi,sLNS

	 -- chuyen nam sau - da cap tien truoc
	 union all
	 SELECT	iID_MaDonVi,sLNS
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=0
			,rDaCap_NamTruoc	=sum(rTuChi)
			,rDaCap_NamNay		=0
	FROM	f_ns_bs(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'5')
	GROUP BY iID_MaDonVi,sLNS

	 -- chuyen nam sau - da cap tien
	 union all
	 SELECT	iID_MaDonVi,sLNS
			,rChuaCap_NamTruoc	=0
			,rChuaCap_NamNay	=0
			,rDaCap_NamTruoc	=0
			,rDaCap_NamNay		=sum(rTuChi)
	FROM	f_ns_bs(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'1')
	GROUP BY iID_MaDonVi,sLNS
) qt


-- lay ten don vi
inner join (SELECT iID_MaDonVi as dv_id, sTen as sTenDonVi FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as dv
on qt.iID_MaDonVi=dv.dv_id

-- lay mo ta lns
inner join (select sLNS as id, sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and LEN(sLNS)=7 and sL = '') as mlns
on qt.sLNS = mlns.id

where rChuaCap_NamNay<> 0 or rChuaCap_NamTruoc<>0 or rDaCap_NamNay<>0 or rDaCap_NamTruoc<>0
GROUP BY iID_MaDonVi,sLNS,sMoTa,sTenDonVi
order by sLNS,iID_MaDonVi
