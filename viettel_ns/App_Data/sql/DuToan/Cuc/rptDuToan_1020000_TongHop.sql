declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=10
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns int									set @lns=1050100


/*

author:		longsam
date:		07/12/2018
desc:		Nhiem vu C

*/

--###--

select	
		sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
		,iID_MaPhongBan
		,sTenPB			=case iID_MaPhongBan 
							when '07' then N'D.Nghiệp'
							when '10' then N'Tổng cục, BTTM'
							else 'QK,QĐ,HVNT'
						 end
		,rTuChi			=sum(rTuChi)/@dvt
		,rChiTapTrung	=sum(rChiTapTrung)/@dvt
		,rHienVat		=sum(rHienVat)/@dvt
from
(
	-- tuchi
	select  sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
			,iID_MaPhongBan,sTenPB=iID_MaPhongBan
			,rTuChi
			,rChiTapTrung=rTuChi-(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1020000','1020100') THEN (rTuChi-rChiTapTrung) ELSE 0 END)
			,rHienVat
	from	DT_ChungTuChiTiet
	where	iTrangThai=1
			and iNamLamViec=@NamLamViec
			and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
			and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
			and sLNS in (1020000,1020100)

	-- phancap
	union all
	select  sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
			,iID_MaPhongBan =iID_MaPhongBanDich,sTenPB=iID_MaPhongBanDich
			,rTuChi
			,rChiTapTrung=rTuChi-(CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1020000','1020100') THEN (rTuChi-rChiTapTrung) ELSE 0 END)
			,rHienVat
	from	DT_ChungTuChiTiet_PhanCap
	where	iTrangThai=1
			and iNamLamViec=@NamLamViec
			and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
			and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
			and sLNS in (1020000,1020100)

)as a
group by sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa,iID_MaPhongBan,sTenPB
having sum(rTuChi)<>0 or sum(rChiTapTrung)<>0 or sum(rHienVat)<>0
