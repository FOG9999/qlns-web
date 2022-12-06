function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }
    Bang_ThemHang(csH, hGiaTri);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var c;
    for (c = 0; c < Bang_nC; c++) {
        Bang_arrEdit[csH][c] = true;
    }

    Bang_arrEdit[csH][Bang_arrCSMaCot["sSoTien"]] = false;

    Bang_keys.fnSetFocus(csH, 0);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_NhiemVu"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_MaChungTu"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSoTien"], "");
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_ThemHangMoi_ChonChungTu(h, hGiaTri, object) {
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }
    Bang_ThemHang(csH, hGiaTri);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var c;
    for (c = 0; c < Bang_nC; c++) {
        Bang_arrEdit[csH][c] = true;
    }

    //Bang_arrEdit[csH][Bang_arrCSMaCot["sSoTien"]] = false;

    Bang_keys.fnSetFocus(csH, 0);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_NhiemVu"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_MaChungTu"], object.iID_MaChungTu);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sNhiemVu"], object.sNhiemVu);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSoTien"], object.sSoTien);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChi"], object.sMaNoiDungChi);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sTenNoiDungChi"], object.sTenNoiDungChi);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_onKeypress_F2(h, c) {
    BangDuLieu_ThemHangMoi(h + 1, h);
}

function Bang_onKeypress_Delete(h, c) {
    window.parent.DeleteSelectionLoadView(Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_MaChungTu"]]);
}

function BangDuLieu_onKeypress_Delete_ChonChungTu(h, c) {
    if (h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
    }
}

function BangDuLieu_onKeypress_Delete(h, c) {
    if (h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
    }
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        console.log(Bang_XoaHang[i]);
        var object = {
            iID_NhiemVu: "",
            sNhiemVu: "",
            STT: "",
            bDelete: false
        };
        if (Bang_arrThayDoi[i][Bang_arrCSMaCot["iID_NhiemVu"]] == true) {
            object.iID_NhiemVu = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NhiemVu"]];
            object.sNhiemVu = Bang_arrGiaTri[i][Bang_arrCSMaCot["sNhiemVu"]];
            object.STT = i + 1;
            if (Bang_arrHangDaXoa[i] == true)
                object.bDelete = true;
            listItem.push(object);
        } else {
            if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sNhiemVu"]] == true || Bang_arrThayDoi[i][Bang_arrCSMaCot["sSoTien"]] == true) {
                object.iID_NhiemVu = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NhiemVu"]];
                object.sNhiemVu = Bang_arrGiaTri[i][Bang_arrCSMaCot["sNhiemVu"]];
                object.STT = i + 1;
                if (Bang_arrHangDaXoa[i] == true)
                    object.bDelete = true;
                listItem.push(object);
            }
        }
    }

    var listItemXoa = [];
    for (var i = 0; i < Bang_arrHangDaXoa.length; i++) {
        if (Bang_arrHangDaXoa[i] == true) {
            listItemXoa.push(i);
        }
    }

    if (listItemXoa.length <= 0) {
        if (listItem.length <= 0) {
            var html = PopupModal("Lỗi lưu nhiệm vụ", ["Không có dữ liệu thay đổi để lưu!"]);
            window.parent.loadModal(html);
            return false;
        }
    }

    if (!ValidateRequired(listItem)) {
        return false;
    }

    // 20211123 comment
    //if (!ValidateByChungTu(listItem)) {
    //    return false;
    //}

    window.parent.LoadHamTruocKhiKetThuc();
}

function ValidateByChungTu(listItem) {
    var result = true;
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/NNSDuToanNhiemVu/ValidateBeforeSave",
        data: { aListValue: listItem },
        async: false,
        success: function (data) {
            if (data == null || data == undefined) {
                return result;
            }

            if (data.status == true) {
                return result;
            }

            var html = PopupModal("Lỗi lưu nhiệm vụ", data.message);
            window.parent.loadModal(html);
            result = false;
        }
    });

    return result;
}

function PopupModal(title, message) {
    var result = "";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: 1 },
        async: false,
        success: function (data) {
            result = data;
        }
    });

    return result;
}

function ValidateRequired(listItem) {
    var result = true;
    var items = listItem;
    if (items.length <= 0) {
        return result;
    }

    var message = [];
    for (var i = 0; i < items.length; i++) {
        if (!items[i].bDelete && (items[i].sNhiemVu == null || items[i].sNhiemVu == "" || items[i].sNhiemVu == undefined)) {
            message.push("Tên nhiệm vụ ở dòng " + items[i].STT + " chưa nhập.");
        }
    }

    if (message.length > 0) {
        var html = PopupModal("Lỗi lưu nhiệm vụ", message);
        window.parent.loadModal(html);
        result = false;
    }

    return result;
}

function BangDuLieu_onKeypress_F2(h, c) {
    //BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_Delete(h, c) {
    if (h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
    }
}

function BangDuLieu_ThemHangMoiViewMaster(h, hGiaTri) {
    BangDuLieu_ThemHangMoi(h, hGiaTri);
}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    if (c == undefined) {
        BangDuLieu_ThemHangMoi(h, c);
        Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_NhiemVu"]] = true;
        Bang_arrThayDoi[h][Bang_arrCSMaCot["sNhiemVu"]] = true;
        Bang_arrThayDoi[h][Bang_arrCSMaCot["sSoTien"]] = true;
    } else {
        BangDuLieu_ThemHangMoi(h + 1, h);
    }
    //BangDuLieu_ThemHangMoi(h, c);
}

function BangDuLieu_onKeypress_F9() {
    debugger
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $("#btn-search-submit").click();
}

function BangDuLieu_onCellAfterEdit(h, c) {
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_onCellValueChanged(h, c, GiaTriCu) {
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
        }
    }
}

function BangDuLieu_HamTruocKhiKetThuc(iAction) {
    var html = PopupModal("Thông báo", "Lưu dữ liệu thành công");
    window.parent.loadModal(html);
    return true;
}