declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='52'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null

--###--
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
 

select * from
(


/*

- nsqp thuong xuyen, bao hiem y te
- ns nha nuoc giao
- ngan sach nha nuoc
- ngan sach khac

*/
SELECT		
			sLNS1=LEFT(sLNS,1),
			sLNS3=LEFT(sLNS,3),
			sLNS5=LEFT(sLNS,5),
			sLNS,
			sL,
			sK,
			sM,
			sTM,
			sTTM,
			sNG,
			--sMoTa,
			--sXauNoiMa,
			rTuChi=sum(rTuChi)/@dvt
			--,rHienVat=sum(rHienVat)/@dvt
			,rHienVat=0
--FROM		f_ns_chitieu_full(@NamLamViec,@Id_DonVi,@Id_PhongBan,2,null,getdate(),1,null) 
FROM		f_ns_chitieu_full_tuchi(@NamLamViec,@Id_DonVi,@Id_PhongBan,2,null,getdate(),1,null) 
group by sLNS,sL,sK,sM,sTM,sTTM,sNG--,sMoTa,sXauNoiMa

 

) as a

-- test
--where sLNS like '1020100%'
where	rTuChi<>0 or rHienVat<>0
--order by sXauNoiMa
