alter table VDT_QT_QuyetToan_ChiTiet add iID_GoiThauId uniqueidentifier
alter table VDT_QT_QuyetToan add iID_DeNghiQuyetToanID uniqueidentifier
alter table VDT_QT_DeNghiQuyetToan add iID_LoaiQuyetToan int
alter table VDT_QT_DeNghiQuyetToan_ChiTiet add iID_GoiThauId uniqueidentifier