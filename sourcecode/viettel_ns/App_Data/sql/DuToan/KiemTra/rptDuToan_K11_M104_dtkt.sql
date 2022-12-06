declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan=null
declare @id_nganh nvarchar(20)						set @id_nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
--declare @id_nganh nvarchar(20)						set @id_nganh=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0

--#DECLARE#--/

select	
		SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,SUBSTRING(Code,1,9) as Code4,Code,
		Nganh
		,TuChi		= sum(TuChi)
		,HangNhap		= sum(HangNhap)
		,HangMua		= sum(HangMua)
from
(

	SELECT		 Code,
				Nganh
				,TuChi		= sum(TuChi+ TangNV - GiamNV)/@dvt	+ sum(DacThu + DacThu_HN + DacThu_HM)/@dvt
				,HangNhap	= sum(HangNhap)/@dvt
				,HangMua	= sum(HangMua)/@dvt
				--,DacThu		= sum(DacThu + DacThu_HN + DacThu_HM)/@dvt
	FROM		DTKT_ChungTuChiTiet
	WHERE		iTrangThai = 1
				and NamLamViec = @NamLamViec
				and iLoai=2
				--and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@id_nganh IS NULL OR Nganh in (select * from f_split(@id_nganh)))
				and (@Id_PhongBan is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan)))
				and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec)
	GROUP BY	Code, Nganh, sMoTa

	-- dacthu
	union all
	SELECT		Code,
				Nganh
				,TuChi		=sum(DacThu)/@dvt
				,HangNhap	=0
				,HangMua	=0
				--,DacThu		= sum(DacThu)/@dvt
	FROM		DTKT_ChungTuChiTiet
	WHERE		iTrangThai = 1
				and NamLamViec = @NamLamViec
				and iLoai=1
				--and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@id_nganh IS NULL OR Nganh in (select * from f_split(@id_nganh)))
				and (@Id_PhongBan is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan)))
				and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec)
	GROUP BY	Code, Nganh, sMoTa


) as t1
where Nganh<>'00'
GROUP BY	Code, Nganh

---- lay ten nganh
--inner join 
--(select Code as id, XauNoiMa,XauNoiMa_x from DTKT_MucLuc where NamLamViec=@NamLamViec and iTrangThai=1) as mlns
--on t1.Code=mlns.id 

--order by code
