declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(MAX)					set @iID_MaDonVi='02,03,40,51,52,53,55,56,57,61,65,75,99'
declare @dvt int									set @dvt = 1000

--#DECLARE#--



 create table #temp_qt
 (
	iID_MaDonVi nvarchar(10)
	,TenDonVi	nvarchar(250)

	,N0_Quy1	float default(0)
	,N0_Quy2	float default(0)
	,N0_Quy3	float default(0)
	,N0_Quy4	float default(0)
	,N0			float default(0)

	,N1_Quy1	float default(0)
	,N1_Quy2	float default(0)
	,N1_Quy3	float default(0)
	,N1_Quy4	float default(0)
	,N1			float default(0)

	,N2_Quy1	float default(0)
	,N2_Quy2	float default(0)
	,N2_Quy3	float default(0)
	,N2_Quy4	float default(0)
	,N2			float default(0)
 

	,N3_Quy1	float default(0)
	,N3_Quy2	float default(0)
	,N3_Quy3	float default(0)
	,N3_Quy4	float default(0)
	,N3			float default(0)
 
	,N4_Quy1	float default(0)
	,N4_Quy2	float default(0)
	,N4_Quy3	float default(0)
	,N4_Quy4	float default(0)
	,N4			float default(0)
 
 )

create table #temp_ns (sXauNoiMa nvarchar(100))
insert into #temp_ns
select	distinct sXauNoiMa from NS_MucLucNganSach
	where	iTrangThai=1
			and iNamLamViec=2019
			and sNG<>''
			--and (sXauNoiMa like '1010000-010-011-6000-6001%'
				--OR sXauNoiMa like '1010000-010-011-6400-6401%')
			--and sXauNoiMa not like '1010000-010-011-6300-6302%'
			and slns='1010000'

--SELECT * FROM #temp_ns
 


 -- nam 2015
insert into #temp_qt (iID_MaDonVi,N0_Quy1) select iID_MaDonVi,rTuChi from o_qt(2015,@iID_MaDonVi,@iID_MaPhongBan,2,1,1) where sXauNoiMa like '1010000%'  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N0_Quy2) select iID_MaDonVi,rTuChi from o_qt(2015,@iID_MaDonVi,@iID_MaPhongBan,2,2,1)	where sXauNoiMa like '1010000%'	 and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N0_Quy3) select iID_MaDonVi,rTuChi from o_qt(2015,@iID_MaDonVi,@iID_MaPhongBan,2,3,1)	where sXauNoiMa like '1010000%'	 and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N0_Quy4) select iID_MaDonVi,rTuChi from o_qt(2015,@iID_MaDonVi,@iID_MaPhongBan,2,4,1)	where sXauNoiMa like '1010000%'	 and sXauNoiMa not like '1010000-010-011-6300-6302%'
																																						 
																																						 
																																						 
 -- nam 2016																																			 
insert into #temp_qt (iID_MaDonVi,N1_Quy1) select iID_MaDonVi,rTuChi from o_qt(2016,@iID_MaDonVi,@iID_MaPhongBan,2,1,1) where sXauNoiMa like '1010000%'   and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N1_Quy2) select iID_MaDonVi,rTuChi from o_qt(2016,@iID_MaDonVi,@iID_MaPhongBan,2,2,1)	where sXauNoiMa like '1010000%'	  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N1_Quy3) select iID_MaDonVi,rTuChi from o_qt(2016,@iID_MaDonVi,@iID_MaPhongBan,2,3,1)	where sXauNoiMa like '1010000%'	  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N1_Quy4) select iID_MaDonVi,rTuChi from o_qt(2016,@iID_MaDonVi,@iID_MaPhongBan,2,4,1)	where sXauNoiMa like '1010000%'	  and sXauNoiMa not like '1010000-010-011-6300-6302%'
																																						 
																																						 
																																						 
																																						 
																																						 
 -- nam 2017																																			 
insert into #temp_qt (iID_MaDonVi,N2_Quy1) select iID_MaDonVi,rTuChi from o_qt(2017,@iID_MaDonVi,@iID_MaPhongBan,2,1,1)	 where sXauNoiMa like '1010000%'  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N2_Quy2) select iID_MaDonVi,rTuChi from o_qt(2017,@iID_MaDonVi,@iID_MaPhongBan,2,2,1)	 where sXauNoiMa like '1010000%'  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N2_Quy3) select iID_MaDonVi,rTuChi from o_qt(2017,@iID_MaDonVi,@iID_MaPhongBan,2,3,1)	 where sXauNoiMa like '1010000%'  and sXauNoiMa not like '1010000-010-011-6300-6302%'
insert into #temp_qt (iID_MaDonVi,N2_Quy4) select iID_MaDonVi,rTuChi from o_qt(2017,@iID_MaDonVi,@iID_MaPhongBan,2,4,1)	 where sXauNoiMa like '1010000%'  and sXauNoiMa not like '1010000-010-011-6300-6302%'




 -- nam 2018
insert into #temp_qt (iID_MaDonVi,N3_Quy1) select iID_MaDonVi,rTuChi from f_qt(2018,@iID_MaDonVi,@iID_MaPhongBan,2,1,1)	  where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N3_Quy2) select iID_MaDonVi,rTuChi from f_qt(2018,@iID_MaDonVi,@iID_MaPhongBan,2,2,1)	  where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N3_Quy3) select iID_MaDonVi,rTuChi from f_qt(2018,@iID_MaDonVi,@iID_MaPhongBan,2,3,1)	  where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N3_Quy4) select iID_MaDonVi,rTuChi from f_qt(2018,@iID_MaDonVi,@iID_MaPhongBan,2,4,1)	  where sXauNoiMa like '1010000%'




 -- nam 2018
insert into #temp_qt (iID_MaDonVi,N4_Quy1) select iID_MaDonVi,rTuChi from f_qt(2019,@iID_MaDonVi,@iID_MaPhongBan,2,1,1)	   where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N4_Quy2) select iID_MaDonVi,rTuChi from f_qt(2019,@iID_MaDonVi,@iID_MaPhongBan,2,2,1)	   where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N4_Quy3) select iID_MaDonVi,rTuChi from f_qt(2019,@iID_MaDonVi,@iID_MaPhongBan,2,3,1)	   where sXauNoiMa like '1010000%'
insert into #temp_qt (iID_MaDonVi,N4_Quy4) select iID_MaDonVi,rTuChi from f_qt(2019,@iID_MaDonVi,@iID_MaPhongBan,2,4,1)	   where sXauNoiMa like '1010000%'



select * from
(

 select 
 iID_MaDonVi
 --,TenDonV
	,N0_Quy1	  =sum(N0_Quy1	 )									 /@dvt
	,N0_Quy2	  =sum(N0_Quy2	 )									 /@dvt
	,N0_Quy3	  =sum(N0_Quy3	 )									 /@dvt
	,N0_Quy4	  =sum(N0_Quy4	 )									 /@dvt
	,N0		  =sum(N0_Quy1+N0_Quy2+N0_Quy3+N0_Quy4)				 /@dvt
 ,N1_Quy1	  =sum(N1_Quy1	 )									 /@dvt
 ,N1_Quy2	  =sum(N1_Quy2	 )									 /@dvt
 ,N1_Quy3	  =sum(N1_Quy3	 )									 /@dvt
 ,N1_Quy4	  =sum(N1_Quy4	 )									 /@dvt
 ,N1		  =sum(N1_Quy1+N1_Quy2+N1_Quy3+N1_Quy4)				 /@dvt
 ,N2_Quy1	  =sum(N2_Quy1	 )									 /@dvt
 ,N2_Quy2	  =sum(N2_Quy2	 )									 /@dvt
 ,N2_Quy3	  =sum(N2_Quy3	 )									 /@dvt
 ,N2_Quy4	  =sum(N2_Quy4	 )									 /@dvt
 ,N2		  =sum(N2_Quy1+N2_Quy2+N2_Quy3+N2_Quy4)				 /@dvt
 ,N3_Quy1	  =sum(N3_Quy1	 )									 /@dvt
 ,N3_Quy2	  =sum(N3_Quy2	 )									 /@dvt
 ,N3_Quy3	  =sum(N3_Quy3	 )									 /@dvt
 ,N3_Quy4	  =sum(N3_Quy4	 )									 /@dvt
 ,N3		  =sum(N3_Quy1+N3_Quy2+N3_Quy3+N3_Quy4		 )		 /@dvt
 ,N4_Quy1	  =sum(N4_Quy1	 )									 /@dvt
 ,N4_Quy2	  =sum(N4_Quy2	 )									 /@dvt
 ,N4_Quy3	  =sum(N4_Quy3	 )									 /@dvt
 ,N4_Quy4	  =sum(N4_Quy4	 )									 /@dvt
 ,N4		  =sum(N4_Quy1+N4_Quy2+N4_Quy3+N4_Quy4		 )		 /@dvt
 
from #temp_qt
group by iID_MaDonVi
) as qt

-- lay ten donvi
inner join (select iID_MaDonVi as id, sTen as sTenDonVi from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=2019 ) as dv
on dv.id=qt.iID_MaDonVi


order by iID_MaDonVi


 -- xoa bang tam
 drop table #temp_qt
 drop table #temp_ns