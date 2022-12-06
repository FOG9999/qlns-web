declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='17'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null

--###--
 

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
			sMoTa,
			sXauNoiMa,
			rTuChi=sum(rTuChi)/@dvt,
			rHienVat=sum(rHienVat)/@dvt
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai in (1,2)
			and iNamLamViec = @NamLamViec
			and MaLoai in ('','2')
			and iID_MaDonVi=@id_donvi
			--and (sLNS='1010000' or sLNS='1020700' or LEFT(sLNS,3) in (109) or LEFT(sLNS,1) in (2,4))
			and (LEFT(sLNS,1) in (1,2,4) and sLNS not in (1020000,1020100))
group by sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa


union all

select	sLNS1=LEFT(sLNS,1),
		sLNS3=LEFT(sLNS,3),
		sLNS5=LEFT(sLNS,5),
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

					and iID_MaDonVi=@id_donvi
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

					and iID_MaDonVi=@id_donvi
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

					and iID_MaDonVi=@id_donvi
					and (sLNS in (1040100))
) nv
group by sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa


) as a

-- test
--where sLNS like '1020100%'
where	rTuChi<>0 or rHienVat<>0
order by sXauNoiMa
