
--INSERT INTO VDT_DM_LoaiCongTrinh(iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenVietTat, sTenLoaiCongTrinh, sMoTa, iThuTu, bActive, dNgayTao, sID_MaNguoiDungTao)
--VALUES(NEWID(), @iParentID, @sMaLoaiCongTrinh, @sTenVietTat, @sTenLoaiCongTrinh, @sMoTa, (CASE WHEN ISNULL(@iThuTu,0) = 0 THEN (SELECT ISNULL(MAX(iThuTu),0) + 1 FROM VDT_DM_LoaiCongTrinh) ELSE 1 END), 1, GETDATE(),@sUserLogin)

INSERT INTO VDT_DM_LoaiCongTrinh(iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenVietTat, sTenLoaiCongTrinh, sMoTa, iThuTu, bActive, dNgayTao, sID_MaNguoiDungTao)
VALUES(NEWID(), @iParentID, @sMaLoaiCongTrinh, @sTenVietTat, @sTenLoaiCongTrinh, @sMoTa,@iThuTu , 1, GETDATE(),@sUserLogin)