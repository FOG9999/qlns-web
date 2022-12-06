DECLARE @iID_MaChungTu uniqueidentifier set @iID_MaChungTu = '9eaa0aff-e833-4ffc-8c8f-1a9fbdb880b2'

--#DECLARE#--
select * into #tmp 
from 
(
select ctct.iid_maphongbandich as smaphongban, pb.sten as stenphongban, ctct.iid_madonvi, dv.sten as tendonvi, sum(ctct.rtuchi + ctct.rchitainganh + ctct.rhangnhap + ctct.rhangmua + ctct.rduphong) as sotienxaunoima 
from dtbs_chungtuchitiet ctct
inner join ns_phongban pb on ctct.iid_maphongbandich = pb.skyhieu and pb.itrangthai = 1
inner join ns_donvi dv on ctct.iid_madonvi = dv.iid_madonvi and inamlamviec_donvi = ctct.inamlamviec and dv.itrangthai = 1
where iid_machungtu = @iid_machungtu
group by ctct.iid_maphongbandich, pb.sten, ctct.iid_madonvi, dv.sten

union all

select ctctpc.iid_maphongbandich as smaphongban, pb.sten as stenphongban, ctctpc.iid_madonvi, dv.sten as tendonvi, sum(ctctpc.rtuchi + ctctpc.rchitainganh + ctctpc.rhangnhap + ctctpc.rhangmua + ctctpc.rduphong) as sotienxaunoima 
from DTBS_ChungTuChiTiet_PhanCap ctctpc
inner join ns_phongban pb on ctctpc.iid_maphongbandich = pb.skyhieu and pb.itrangthai = 1
inner join ns_donvi dv on ctctpc.iid_madonvi = dv.iid_madonvi and inamlamviec_donvi = ctctpc.inamlamviec and dv.itrangthai = 1
where iID_MaChungTu IN (
select iID_MaChungTuChiTiet
from DTBS_ChungTuChiTiet ctct
where iID_MaChungTu = @iID_MaChungTu
)
group by ctctpc.iid_maphongbandich, pb.sten, ctctpc.iid_madonvi, dv.sten
) as tmp

select smaphongban, stenphongban, iid_madonvi, tendonvi, sum(sotienxaunoima) as sotienxaunoima from #tmp
group by smaphongban, stenphongban, iid_madonvi, tendonvi
