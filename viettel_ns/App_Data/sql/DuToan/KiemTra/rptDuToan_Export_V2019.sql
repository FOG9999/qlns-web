declare @dvt int									set @dvt = 1
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan=null
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null

--###--


/*

- nsqp thuong xuyen, bao hiem y te
- ns nha nuoc giao
- ngan sach nha nuoc
- ngan sach khac

*/

select 
	
		sLNS1,
		sLNS3,
		sLNS5,
		sLNS,
		sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sXauNoiMa,
		sum(Nam2019) as Nam2019,
		sum(Nam2020) as Nam2020

from

(
	select 
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
		sXauNoiMa,
		0		as Nam2019,
		rTuChi  as Nam2020
	from 
	(
	

	SELECT		
				sLNS,
				sL,
				sK,
				sM,
				sTM,
				sTTM,
				sNG,
				sMoTa,
				sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
				rTuChi=sum(rTuChi)/@dvt,
				rHienVat=sum(rHienVat)/@dvt
	FROM		DT_ChungTuChiTiet
	WHERE		iTrangThai in (1,2)
				and iNamLamViec = @NamLamViec
				and MaLoai in ('','2')
				and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
				--and (sLNS='1010000' or sLNS='1020700' or LEFT(sLNS,3) in (109) or LEFT(sLNS,1) in (2,4))
				and (LEFT(sLNS,1) in (1,2,4) and sLNS not in (1020000,1020100))
	group by sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa


	union all

	select	
			sLNS,
			sL,
			sK,
			sM,
			sTM,
			sTTM,
			sNG,
			sMoTa,
			sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
			rTuChi=sum(rTuChi)/@dvt,
			rHienVat=sum(rHienVat)/@dvt
	from 
	(
			SELECT		
			
						sLNS='1020100',
						sL,
						sK,
						sM,
						sTM,
						sTTM,
						sNG,
						sMoTa,
						rTuChi,
						rHienVat
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai in (1,2)
						and iNamLamViec = @NamLamViec
						and MaLoai in ('','2')

						and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
						and (sLNS in (1020000,1020100))
						--and (sLNS in (1020000,1020100,1050000,1050100))

			union all
			SELECT		
						sLNS='1020100',
						sL,
						sK,
						sM,
						sTM,
						sTTM,
						sNG,
						sMoTa,
						rTuChi,
						rHienVat
			FROM		DT_ChungTuChiTiet_PhanCap
			WHERE		iTrangThai = 1
						and iNamLamViec = @NamLamViec
						and iID_MaDonVi=@id_donvi
						and (sLNS in (1020000,1020100))



			-- ngan sach hang nhap
			union all
			SELECT		
			
						sLNS='1040200',
						sL,
						sK,
						sM,
						sTM,
						sTTM,
						sNG,
						sMoTa,
						rTuChi = rHangNhap,
						rHienVat =0
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai in (1,2)
						and iNamLamViec = @NamLamViec
						and MaLoai in ('','2')
						and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))

						and (sLNS in (1040100))


			-- ngan sach hang mua
			union all
			SELECT		
			
						sLNS='1040300',
						sL,
						sK,
						sM,
						sTM,
						sTTM,
						sNG,
						sMoTa,
						rTuChi = rHangMua,
						rHienVat =0
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai in (1,2)
						and iNamLamViec = @NamLamViec
						and MaLoai in ('','2')

						and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))

						and (sLNS in (1040100))
	) nv
	group by sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
) as a
where sLNS in (1010000)


---- du toan 2019
union all
select 
		sLNS1,
		sLNS3,
		sLNS5,
		sLNS,
		sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		--sXauNoiMa,
		sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,

		sum(rTuChi)	as Nam2019,
		0			as Nam2020

from f_ns_chitieu_full_tuchi(2019,@Id_DonVi,null,2,null,getdate(),1000,null)
where sLNS=1010000
group by
		sLNS1,
		sLNS3,
		sLNS5,
		sLNS,
		sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG


 


)  as Nam2020

                
-- test
--where sLNS in (1010000)
where	sLNS in (1010000) 
		and	(nam2019<>0 or nam2020<>0)    
		
		-- loc ky muc luc luong
		and (
				sTM not in (6003,6049,6102,6401,6403,6304)
				and sM not in (6250,8000)
			)                                                                           

group by
		sLNS1,
		sLNS3,
		sLNS5,
		sLNS,
		sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sXauNoiMa

order by sXauNoiMa




