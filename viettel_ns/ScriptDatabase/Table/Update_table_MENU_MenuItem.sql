USE [CTC_DB_MENU]
GO

UPDATE MENU_MenuItem SET bHoatDong='0' WHERE sURL LIKE 'QLNH/ThongTinGoiThau';
UPDATE MENU_MenuItem SET bHoatDong='0' WHERE sURL LIKE 'QLNH/KeHoachTongTheTTCP';
UPDATE MENU_MenuItem SET sTen=N'Quyết định tổng thể TTCP' WHERE sURL LIKE 'QLNH/KeHoachChiTietBQP';
