
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @nam int							set @nam = 2020
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(2000)					set @iID_MaDonVi='00,01,02,03,10,11,112,12,121,122,13,131,132,14,141,142,15,151,152,17,171,172,18,19,191,192,20,21,22,23,231,24,241,29,291,30,31,33,331,34,341,35,38,40,41,42,43,44,45,46,47,48,50,501,51,52,53,531,532,533,534,54,55,56,57,61,65,66,67,68,69,70,71,72,73,75,76,77,78,79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,971,972,98,99,A7,A8,B16,B17,B5,B6,B8,C1,C2,C3,C4,HK,L1'

 
--###--


SELECT  iID_MaDonVi, 
		sTenDonVi, 
		NSSD		=sum(NSSD),
		NSBD_DC		=sum(NSBD_DC),
		NSBD_CC		=sum(NSBD_CC)
FROM 
(
	 -- chuyen nam sau - chưa cấp tiền
	 SELECT	iID_MaDonVi		=case when iID_MaDonVi like '[0-9]%' then left(iID_MaDonVi,2) else iID_MaDonVi end
			,NSSD		= case when sLNS not like '104%' then sum(rTuChi) else 0 end
			,NSBD_DC	= 0
			,NSBD_CC	= case when sLNS like '104%' then sum(rTuChi) else 0 end
	FROM	f_ns_bs(@nam,@iID_MaDonVi,@iID_MaPhongBan,'4')
	GROUP BY iID_MaDonVi,sLNS

	 -- chuyen nam sau - da cap tien truoc
	 union all
	 SELECT	iID_MaDonVi		=case when iID_MaDonVi like '[0-9]%' then left(iID_MaDonVi,2) else iID_MaDonVi end
			,NSSD		= case when sLNS not like '104%' then sum(rTuChi) else 0 end
			,NSBD_DC	= case when sLNS like '104%' then sum(rTuChi) else 0 end
			,NSBD_CC	= 0
	FROM	f_ns_bs(@nam,@iID_MaDonVi,@iID_MaPhongBan,'5')
	GROUP BY iID_MaDonVi,sLNS

	 -- chuyen nam sau - da cap tien
	 union all
	 SELECT	iID_MaDonVi		=case when iID_MaDonVi like '[0-9]%' then left(iID_MaDonVi,2) else iID_MaDonVi end
			,NSSD		= case when sLNS not like '104%' then sum(rTuChi) else 0 end
			,NSBD_DC	= case when sLNS like '104%' then sum(rTuChi) else 0 end
			,NSBD_CC	= 0
	FROM	f_ns_bs(@nam,@iID_MaDonVi,@iID_MaPhongBan,'1')
	GROUP BY iID_MaDonVi,sLNS
) qt


-- lay ten don vi
inner join (SELECT iID_MaDonVi as dv_id, iID_MaDonVi + ' - ' + sTen as sTenDonVi FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as dv
on qt.iID_MaDonVi=dv.dv_id

where NSSD + NSBD_DC + NSBD_CC <> 0 and sTenDonVi is not null
GROUP BY iID_MaDonVi,sTenDonVi
order by iID_MaDonVi
