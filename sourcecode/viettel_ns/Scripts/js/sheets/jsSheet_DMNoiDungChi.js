var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onCellBeforeEdit(h, c) {
    var bDisable = false;
    if (c == Bang_arrCSMaCot["sMaNguon"]) {
        var sMaNoiDungChi = Bang_LayGiaTri(h, "sMaNoiDungChi");
        if (sMaNoiDungChi != "") {
            for (var j = 0; j < Bang_nH; j++) {
                if (j == h)
                    continue;
                if (Bang_LayGiaTri(j, "sMaNoiDungChiCha") == sMaNoiDungChi) {
                    bDisable = true;
                    break;
                }
            }
        }

        if (!bDisable) {
            var iID_NoiDungChi = Bang_LayGiaTri(h, "iID_NoiDungChi");
            if (iID_NoiDungChi != "" && !CheckDelete(iID_NoiDungChi)) {
                bDisable = true;
            }
        }
        return !bDisable;
    }
}

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrMaCot[c] == "iSTT") {
        var indexNew = Bang_LayGiaTri(h, "iSTT");

        var regNumber = new RegExp("^\\d+$");
        if (!regNumber.test(indexNew)) {
            Bang_GanGiaTriThatChoO(h, c, Bang_GiaTriO_BeforEdit);
            return false;
        }

        var parentStt = Bang_GiaTriO_BeforEdit.substring(0, Bang_GiaTriO_BeforEdit.lastIndexOf('.') + 1);
        var newStt = parentStt + indexNew;
        Bang_GanGiaTriThatChoO_colName(h, "iSTT", newStt);
        // update iSTT con
        var sMaNoiDungChiCha = Bang_LayGiaTri(h, "sMaNoiDungChi");
        UpdateISTTCon(h, sMaNoiDungChiCha, newStt);
    }
    return true;
}

function UpdateISTTCon(h, sMaNoiDungChiCha, iSTTCha) {
    for (var j = 0; j < Bang_nH; j++) {
        if (j == h)
            continue;
        if (Bang_LayGiaTri(j, "sMaNoiDungChiCha") == sMaNoiDungChiCha) {
            // create new stt
            var stt = Bang_LayGiaTri(j, "iSTT");
            var arrStt = stt.split('.');
            if (arrStt.length > 0) {
                var indexOld = arrStt[arrStt.length - 1];
                var newStt = iSTTCha + "." + indexOld;
                Bang_GanGiaTriThatChoO_colName(j, "iSTT", newStt);
                // update con
                var sMaNoiDungChi = Bang_LayGiaTri(j, "sMaNoiDungChi");
                UpdateISTTCon(j, sMaNoiDungChi, newStt);
            }
        }
    }
}

function BangDuLieu_onKeypress_Delete(h, c) {
    var errDel = false;
    var sMaNoiDungChi = Bang_LayGiaTri(h, "sMaNoiDungChi");
    if (sMaNoiDungChi != "") {
        for (var j = 0; j < Bang_nH; j++) {
            if (j == h)
                continue;
            if (Bang_LayGiaTri(j, "sMaNoiDungChiCha") == sMaNoiDungChi) {
                errDel = true;
                break;
            }
        }
    }

    if (!errDel) {
        var iID_NoiDungChi = Bang_LayGiaTri(h, "iID_NoiDungChi");
        if (iID_NoiDungChi != "" && !CheckDelete(iID_NoiDungChi)) {
            errDel = true;
        }
    }

    if (errDel) {
        var Title = 'Lỗi xoá danh mục nội dung chi';
        var sMessError = 'Danh mục nội dung chi ' + Bang_LayGiaTri(h, "sMaNoiDungChi") + ' không được phép xoá.';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: sMessError, Category: ERROR },
            success: function (data) {
                window.parent.loadModal(data);
            }
        });
        return false;
    }

    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        BangDuLieu_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
        else if (Bang_nH > 0) {

        }
    }
}

function CheckDelete(iID_NoiDungChi) {
    var status = false;
    $.ajax({
        type: "POST",
        url: "/DMNoiDungChi/CheckDeleteDmNoiDungChi",
        data: { iID_NoiDUngChi: iID_NoiDungChi },
        async: false,
        success: function (data) {
            status = data.bStatus;
        }
    });
    return status;
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

// tao dong con
function BangDuLieu_onKeypress_F3(h, c) {
    if (c == undefined) {
        return;
    } else {
        var iID_NoiDungChi = Bang_LayGiaTri(h, "iID_NoiDungChi");
        if (iID_NoiDungChi != "" && !CheckDelete(iID_NoiDungChi)) {
            var Title = 'Lỗi thêm dòng con';
            var sMessError = 'Không được thêm dòng con.';
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: Title, Messages: sMessError, Category: ERROR },
                success: function (data) {
                    window.parent.loadModal(data);
                }
            });
        } else {
            BangDuLieu_ThemHangMoi(h + 1, h, 1);
        }
    }
}

function BangDuLieu_onKeypress_F2(h, c) {
    //BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    if (c == undefined) {
        BangDuLieu_ThemHangMoi(h, c);
    } else {
        BangDuLieu_ThemHangMoi(h + 1, h);
    }
    //BangDuLieu_ThemHangMoi(h, c);
}

function BangDuLieu_ThemHangMoi(h, hGiaTri, isChild = 0) {
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
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChi"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChiCha"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], "");

    if (isChild == 1) {
        var iIDNoiDungChi = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iID_NoiDungChi"]];
        var sMaNoiDungChi = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sMaNoiDungChi"]];

        // set ma nguon cha = null
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iID_Nguon"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sMaNguon"], "");

        // set ma noi dung chi cha
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_Parent"], iIDNoiDungChi);
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChiCha"], sMaNoiDungChi);
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChi"], TaoMaNoiDungChiTuDong(sMaNoiDungChi));

        // set iSTT
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], TaoISTTTuDong(sMaNoiDungChi));
    } else {
        var sMaNoiDungChiCha = "";
        var iIDNoiDungChiCha = "";
        if (hGiaTri != undefined) {
            sMaNoiDungChiCha = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sMaNoiDungChiCha"]];
            iIDNoiDungChiCha = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iID_Parent"]];

            if (sMaNoiDungChiCha != "") {
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChiCha"], sMaNoiDungChiCha);
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_Parent"], iIDNoiDungChiCha);
            }
        }

        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNoiDungChi"], TaoMaNoiDungChiTuDong(sMaNoiDungChiCha));
        // set iSTT
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], TaoISTTTuDong(sMaNoiDungChiCha));
    }
    //Set ID dòng mới 
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_NoiDungChi"], "");

    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";
    var c;
    for (c = 0; c < Bang_nC; c++) {
        Bang_arrEdit[csH][c] = true;
    }
    Bang_arrEdit[csH][Bang_arrCSMaCot["sMaNoiDungChi"]] = false;
    Bang_arrEdit[csH][Bang_arrCSMaCot["sMaNoiDungChiCha"]] = false;

    Bang_keys.fnSetFocus(csH, 0);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function DMNoiDungChi_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    if (!ValidateData()) {
        return false;
    }
    ValidateBeforeSave();
}

function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu danh mục nội dung chi';

    for (var j = 0; j < Bang_nH; j++) {
        if (Bang_arrHangDaXoa[j] == false) {
            var sMaNoiDungChi = Bang_LayGiaTri(j, "sMaNoiDungChi");
            if (sMaNoiDungChi == '') {
                sMessError.push('Hãy nhập mã nội dung chi dòng số ' + (j + 1) + '.');
            }

            var sTenNoiDungChi = Bang_LayGiaTri(j, "sTenNoiDungChi");
            if (sTenNoiDungChi == '') {
                sMessError.push('Hãy nhập tên nội dung chi dòng số ' + (j + 1) + '.');
            }

            var isCha = false;
            var sMaNoiDungChi = Bang_LayGiaTri(j, "sMaNoiDungChi");
            if (sMaNoiDungChi != "") {
                for (var k = 0; k < Bang_nH; k++) {
                    if (j == k)
                        continue;
                    if (Bang_LayGiaTri(k, "sMaNoiDungChiCha") == sMaNoiDungChi) {
                        isCha = true;
                    }
                }
            }
            if (!isCha) {
                var sMaNguon = Bang_LayGiaTri(j, "sMaNguon");
                if (sMaNguon == '') {
                    sMessError.push('Hãy nhập mã nguồn dòng số ' + (j + 1) + '.');
                }
            }
        }
    }

    if (sMessError != null && sMessError != undefined && sMessError.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: sMessError, Category: ERROR },
            success: function (data) {
                window.parent.loadModal(data);
            }
        });
        return false;
    }
    return true;
}

function AjaxRequire(listItem) {
    $.ajax({
        type: "POST",
        url: "/DMNoiDungChi/ValidateBeforeSave",
        data: { aListModel: listItem },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu danh mục nội dung chi"
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: data.listErrMess, Category: 1 },
                    async: false,
                    success: function (data) {
                        window.parent.loadModal(data);
                        return false;
                    }
                });
            } else {
                Bang_HamTruocKhiKetThuc();
            }
        }
    });
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            iID_NoiDungChi: "",
            sMaNoiDungChi: "",
            STT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            if (Bang_arrThayDoi[i][Bang_arrCSMaCot["iID_NoiDungChi"]] == true) {
                object.iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
                object.sMaNoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChi"]];
                object.STT = i + 1;
                object.bDelete = false;
                listItem.push(object);
            } else {
                if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sMaNoiDungChi"]] == true) {
                    object.iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
                    object.sMaNoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChi"]];
                    object.STT = i + 1;
                    object.bDelete = false;
                    listItem.push(object);
                }
            }
        } else {
            object.iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
            object.sMaNoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChi"]];
            object.STT = i + 1;
            object.bDelete = true;
            listItem.push(object);
        }
    }

    AjaxRequire(listItem);
}

function BangDuLieu_onKeypress_F9(h, c) {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".btn-mvc").click();
}

function BangDuLieu_onKeypress_F10() {
    //DMNoiDungChi_BangDuLieu_Save();
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
    //    //Bang_HamTruocKhiKetThuc();
    //}
}

function setFontWeight() {
    for (var j = 0; j < Bang_nH; j++) {
        var numChild = Bang_LayGiaTri(j, "numChild");
        if (numChild > 0) {
            Bang_GanGiaTriThatChoO_colName(j, "sFontBold", "bold");
            Bang_GanGiaTriThatChoO_colName(j, "sMauSac", "whitesmoke");
        }
    }
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

function GetListData() {
    var listItem = [];
    for (var i = 0; i < Bang_nH; i++) {
        var object = {
            iID_NoiDungChi: "",
            sMaNoiDungChi: "",
            sMaNoiDungChiCha: "",
            iSTT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            object.iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
            object.sMaNoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChi"]];
            object.sMaNoiDungChiCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChiCha"]];
            object.iSTT = Bang_arrGiaTri[i][Bang_arrCSMaCot["iSTT"]];
            object.bDelete = false;
            listItem.push(object);
        } else {
            object.iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
            object.sMaNoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChi"]];
            object.sMaNoiDungChiCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNoiDungChiCha"]];
            object.iSTT = Bang_arrGiaTri[i][Bang_arrCSMaCot["iSTT"]];
            object.bDelete = true;
            listItem.push(object);
        }
    }
    return listItem;
}

function TaoMaNoiDungChiTuDong(sMaNDChiCha) {
    var lstData = GetListData();
    var lstData = lstData.filter(x => x.sMaNoiDungChiCha == sMaNDChiCha);
    if (sMaNDChiCha != "")
        lstMaND = lstData.map(x => (x.sMaNoiDungChi.indexOf(sMaNDChiCha) < 0 ? x.sMaNoiDungChi : x.sMaNoiDungChi.substr(x.sMaNoiDungChi.indexOf(sMaNDChiCha) + sMaNDChiCha.length, x.sMaNoiDungChi.length - sMaNDChiCha.length)));
    else
        lstMaND = lstData.map(x => x.sMaNoiDungChi);
    lstMaND = lstMaND.filter(x => parseInt(x));
    var intMax = 0;
    if (lstMaND.length > 0)
        intMax = Math.max(...lstMaND);
    intMax++;
    return sMaNDChiCha + ("00" + intMax).substr(-3);
}

function TaoISTTTuDong(sMaNoiDungChiCha) {
    var lstData = GetListData();
    var iSTTCha = "";
    if (sMaNoiDungChiCha != "") {
        var objCha = lstData.filter(x => x.sMaNoiDungChi == sMaNoiDungChiCha);
        if (objCha != null)
            iSTTCha = objCha[0].iSTT;
    }
    var lstData = lstData.filter(x => x.sMaNoiDungChiCha == sMaNoiDungChiCha);
    var lstIndex = lstData.map(x => x.iSTT.substring(x.iSTT.lastIndexOf('.') + 1, x.iSTT.length));

    lstIndex = lstIndex.filter(x => parseInt(x));
    var intMax = 0;
    if (lstIndex.length > 0)
        intMax = Math.max(...lstIndex);
    intMax++;

    if (sMaNoiDungChiCha != "")
        return iSTTCha + "." + intMax;
    return intMax.toString();
}