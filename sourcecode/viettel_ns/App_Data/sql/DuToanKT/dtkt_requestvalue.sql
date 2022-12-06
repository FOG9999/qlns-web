
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='53'
declare @Id_PhongBanDich nvarchar(10)				set @Id_PhongBanDich='10'
declare @iLoai int									set @iLoai=2

--#DECLARE#--
 
select		Id_Mucluc
			, Id_DonVi
			, Code
			, TuChi			= ISNULL(SUM(TuChi) / @dvt, 0)
			, DacThu		= ISNULL(SUM(DacThu) / @dvt, 0)
			, HangNhap		= ISNULL(SUM(HangNhap) / @dvt, 0)
			, DacThu_HN		= ISNULL(SUM(DacThu_HN) / @dvt, 0)
			, HangMua		= ISNULL(SUM(HangMua) / @dvt, 0)
			, DacThu_HM		= ISNULL(SUM(DacThu_HM) / @dvt, 0)
from		DTKT_ChungTuChiTiet 
where		iTrangThai = 1
			and NamLamViec = @NamLamViec
			and iRequest = 0
			and iLoai = @iLoai
			and Id_DonVi = @Id_DonVi
			and Id_PhongBanDich = @Id_PhongBanDich			
group by	Id_Mucluc, Id_DonVi, Code