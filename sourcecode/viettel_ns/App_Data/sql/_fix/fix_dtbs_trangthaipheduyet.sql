
declare @iNamLamViec int							set @iNamLamViec = 2018 
declare @iID_MaChungTu uniqueidentifier				set @iID_MaChungTu = '0484b34b-58fb-4b82-a01f-db8c7ad975ea'
declare @iID_MaTrangThaiDuyet int					set @iID_MaTrangThaiDuyet = 1


--###--
/*

update ma trang thai phe duyet cua chung tu

	1	- Tạo mới
	2	- Trợ lý đơn vị trình
	3	- Trợ lý tổng hợp trình

*/


 --exec sp_ns_dtbs_trangthaipheduyet_update '7848669c-b695-4f51-8c0b-7bc097377545','104'

 
 select * from dtbs_chungtu
 where	iID_MaChungTu=@iID_MaChungTu

 
 select * from DTBS_ChungTuChiTiet
 where	iID_MaChungTu=@iID_MaChungTu

 select * from DTBS_ChungTuChiTiet_PhanCap
 where iTrangThai=1 and iNamLamViec=2018

 --update DTBS_ChungTuChiTiet_PhanCap
 --set iID_MaTrangThaiDuyet=104
 --where iID_MaChungTu='e3f93be6-7445-4076-8eb5-f27a7ef6d8e9'
 

 --update DTBS_ChungTu
 --set	iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet
 --where	iID_MaChungTu=@iID_MaChungTu

 
 --update DTBS_ChungTuChiTiet
 --set	iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet
 --where	iID_MaChungTu=@iID_MaChungTu
