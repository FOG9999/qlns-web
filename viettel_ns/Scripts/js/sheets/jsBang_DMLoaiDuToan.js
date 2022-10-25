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
    Bang_arrEdit[csH][Bang_arrCSMaCot["sMaLoaiDuToan"]] = false;

    Bang_keys.fnSetFocus(csH, 0);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_LoaiDuToan"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaLoaiDuToan"], TaoMaLoaiDuToanTuDong());
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
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
        Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_LoaiDuToan"]] = true;
        Bang_arrThayDoi[h][Bang_arrCSMaCot["sMaLoaiDuToan"]] = true;
        Bang_arrThayDoi[h][Bang_arrCSMaCot["sTenLoaiDuToan"]] = true;
        Bang_arrThayDoi[h][Bang_arrCSMaCot["sGhiChu"]] = true;
    } else {
        BangDuLieu_ThemHangMoi(h + 1, c);
    }
    //BangDuLieu_ThemHangMoi(h, c);
}

function BangDuLieu_onKeypress_F9() {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $("#btn-search-submit").click();
}

function BangDuLieu_HamTruocKhiKetThuc(iAction) {
    var html = PopupModal("Thông báo", "Lưu dữ liệu thành công");
    window.parent.loadModal(html);
    return true;
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

function BangDuLieu_onCellAfterEdit(h, c) {
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_onCellValueChanged(h, c, GiaTriCu) {
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function AjaxRequire(listItem) {
    $.ajax({
        type: "POST",
        url: "/DMLoaiDuToan/ValidateBeforeSave",
        data: { aListModel: listItem },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu danh mục loại dự toán"
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: data.sMessage, Category: 1 },
                    async: false,
                    success: function (res) {
                        window.parent.loadModal(res);
                        return false;
                    }
                });
            } else {
                window.parent.loadHamKetThuc();
            }
        }
    });
}

function AjaxMaTrung(listItem) {
    var message = [];
    for (var i = 0; i < listItem.length; i++) {
        if (listItem[i].sMaLoaiDuToan == null || listItem[i].sMaLoaiDuToan == "" || listItem[i].sMaLoaiDuToan == undefined) {
            message.push("Vui lòng nhập mã loại dự toán dòng : " + listItem[i].STT);
        }
    }

    if (message.length > 0) {
        var Title = "Lỗi lưu danh mục loại dự toán"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: message, Category: 1 },
            async: false,
            success: function (data) {
                window.parent.loadModal(data);
                return false;
            }
        });
    } else {
        AjaxRequire(listItem);
    }
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            iID_LoaiDuToan: "",
            sMaLoaiDuToan: "",
            STT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            if (Bang_arrThayDoi[i][Bang_arrCSMaCot["iID_LoaiDuToan"]] == true) {
                object.iID_LoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiDuToan"]];
                object.sMaLoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaLoaiDuToan"]];
                object.STT = i + 1;
                object.bDelete = false;
                listItem.push(object);
            } else {
                if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sMaLoaiDuToan"]] == true) {
                    object.sMaLoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaLoaiDuToan"]];
                    object.iID_LoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiDuToan"]];
                    object.STT = i + 1;
                    object.bDelete = false;
                    listItem.push(object);
                }
            }
        } else {
            object.sMaLoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaLoaiDuToan"]];
            object.iID_LoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiDuToan"]];
            object.STT = i + 1;
            object.bDelete = true;
            listItem.push(object);
        }
    }
    AjaxMaTrung(listItem);
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
    //if (strKeyEvent == "F10") {
    //    Bang_HamTruocKhiKetThuc();
    //}
}

function GetListData() {
    var listItem = [];
    for (var i = 0; i < Bang_nH; i++) {
        var object = {
            iID_LoaiDuToan: "",
            sMaLoaiDuToan: "",
            STT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            object.iID_LoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiDuToan"]];
            object.sMaLoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaLoaiDuToan"]];
            object.STT = i + 1;
            object.bDelete = false;
            listItem.push(object);
        } else {
            object.iID_LoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiDuToan"]];
            object.sMaLoaiDuToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaLoaiDuToan"]];
            object.STT = i + 1;
            object.bDelete = true;
            listItem.push(object);
        }
    }
    return listItem;
}

function TaoMaLoaiDuToanTuDong() {
    var lstData = GetListData();
    lstData = lstData.map(x => x.sMaLoaiDuToan);

    var intMax = 0;
    if (lstData.length > 0)
        intMax = Math.max(...lstData);
    intMax++;
    return ("00" + intMax).substr(-3);
}
