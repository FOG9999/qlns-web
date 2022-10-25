var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onCellBeforeEdit(h, c) {
    if (Bang_arrMaCot[c] == "sLoaiNganSach") {
        if (CheckEditLoaiNganSach(h) == false)
            return false;
        return true;
    }
    return true;
}

function CheckEditLoaiNganSach(h) {
    var isCanEdit = true;
    var sMaNguonCha = Bang_LayGiaTri(h, "sMaNguonCha");
    var sMaNguon = Bang_LayGiaTri(h, "sMaNguon");
    isCanEdit = CheckChildHaveLoaiNganSach(sMaNguon);

    if (isCanEdit == true) {
        if (sMaNguonCha != "") {
            isCanEdit = CheckParentHaveLoaiNganSach(sMaNguonCha)
        }
    }
    return isCanEdit;
}

function CheckChildHaveLoaiNganSach(sMaNguon) {
    var isEdit = true;
    for (var j = 0; j < Bang_nH; j++) {
        var sMaNguonChaIndex = Bang_LayGiaTri(j, "sMaNguonCha");
        var sMaNguonIndex = Bang_LayGiaTri(j, "sMaNguon");
        var iLoaiNganSach = Bang_LayGiaTri(j, "iLoaiNganSach");
        if (sMaNguonChaIndex == sMaNguon) {
            if (iLoaiNganSach != "") {
                isEdit = false;
                break;
            }
            else {
                isEdit = CheckChildHaveLoaiNganSach(sMaNguonIndex);
                if (isEdit == false)
                    break;
            }
        }
    }
    return isEdit;
}

function CheckParentHaveLoaiNganSach(sMaNguonCha) {
    var isEdit = true;
    for (var j = 0; j < Bang_nH; j++) {
        var sMaNguonChaIndex = Bang_LayGiaTri(j, "sMaNguonCha");
        var sMaNguonIndex = Bang_LayGiaTri(j, "sMaNguon");
        var iLoaiNganSach = Bang_LayGiaTri(j, "iLoaiNganSach");
        if (sMaNguonIndex == sMaNguonCha) {
            if (iLoaiNganSach != "") {
                isEdit = false;
                break;
            }
            else if (sMaNguonChaIndex != "") {
                isEdit = CheckParentHaveLoaiNganSach(sMaNguonChaIndex);
                if (isEdit == false)
                    break;
            }
        }
    }
    return isEdit;
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
        var sMaNguonCha = Bang_LayGiaTri(h, "sMaNguon");
        UpdateISTTCon(h, sMaNguonCha, newStt);
    }
    return true;
}

function UpdateISTTCon(h, sMaNguonCha, iSTTCha) {
    for (var j = 0; j < Bang_nH; j++) {
        if (j == h)
            continue;
        if (Bang_LayGiaTri(j, "sMaNguonCha") == sMaNguonCha) {
            // create new stt
            var stt = Bang_LayGiaTri(j, "iSTT");
            var arrStt = stt.split('.');
            if (arrStt.length > 0) {
                var indexOld = arrStt[arrStt.length - 1];
                var newStt = iSTTCha + "." + indexOld;
                Bang_GanGiaTriThatChoO_colName(j, "iSTT", newStt);

                // update con
                var sMaNguon = Bang_LayGiaTri(j, "sMaNguon");
                UpdateISTTCon(j, sMaNguon, newStt);
            }
        }
    }
}

function BangDuLieu_onKeypress_Delete(h, c) {
    var errDel = false;
    var sMaNguon = Bang_LayGiaTri(h, "sMaNguon");
    if (sMaNguon != "") {
        for (var j = 0; j < Bang_nH; j++) {
            if (j == h)
                continue;
            if (Bang_LayGiaTri(j, "sMaNguonCha") == sMaNguon) {
                errDel = true;
                break;
            }
        }
    }

    if (!errDel) {
        var iID_Nguon = Bang_LayGiaTri(h, "iID_Nguon");
        if (iID_Nguon != "" && !CheckDelete(iID_Nguon)) {
            errDel = true;
        }
    }

    if (errDel) {
        var Title = 'Lỗi xoá danh mục nguồn BTC cấp';
        var sMessError = 'Danh mục nguồn ' + Bang_LayGiaTri(h, "sMaNguon") + ' không được phép xoá!';
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

function CheckDelete(iID_Nguon) {
    var status = false;
    $.ajax({
        type: "POST",
        url: "/DMNguon/CheckDeleteDmNguon",
        data: { iID_Nguon: iID_Nguon },
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
        var iID_Nguon = Bang_LayGiaTri(h, "iID_Nguon");
        if (iID_Nguon != "" && iID_Nguon != null && !CheckDelete(iID_Nguon)) {
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
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguonCha"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguon"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], "");

    if (isChild == 1) {
        var iIDNguon = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iID_Nguon"]];
        var sMaNguon = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sMaNguon"]];

        // set ma noi dung chi cha
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_NguonCha"], iIDNguon);
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguonCha"], sMaNguon);
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguon"], TaoMaNguonTuDong(sMaNguon));

        // set iSTT
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], TaoISTTTuDong(sMaNguon));

        // set null loai ngan sach
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sLoaiNganSach"], "");
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLoaiNganSach"], "");
    } else {
        var sMaNguonCha = "";
        var iIDMaNguonCha = "";
        if (hGiaTri != undefined) {
            sMaNguonCha = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sMaNguonCha"]];
            iIDMaNguonCha = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iID_NguonCha"]];

            if (sMaNguonCha != "") {
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguonCha"], sMaNguonCha);
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_NguonCha"], iIDMaNguonCha);
            }
        }

        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaNguon"], TaoMaNguonTuDong(sMaNguonCha));
        // set iSTT
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iSTT"], TaoISTTTuDong(sMaNguonCha));
    }

    //Set ID dòng mới 
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_Nguon"], "");

    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";
    var c;
    for (c = 0; c < Bang_nC; c++) {
        Bang_arrEdit[csH][c] = true;
    }

    Bang_arrEdit[csH][Bang_arrCSMaCot["sMaNguon"]] = false;
    Bang_arrEdit[csH][Bang_arrCSMaCot["sMaNguonCha"]] = false;

    Bang_keys.fnSetFocus(csH, 0);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function DMNguon_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    if (!ValidateData()) {
        return false;
    }
    ValidateBeforeSave();
}

function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu danh mục nguồn';

    for (var j = 0; j < Bang_nH; j++) {
        if (Bang_arrHangDaXoa[j] == false) {
            var sMaNguon = Bang_LayGiaTri(j, "sMaNguon");
            if (sMaNguon == '') {
                sMessError.push('Hãy nhập mã nguồn BTC cấp dòng số ' + (j + 1) + '!');
            }
            var sNoiDung = Bang_LayGiaTri(j, "sNoiDung");
            if (sNoiDung == '') {
                sMessError.push('Hãy nhập mô tả nguồn BTC cấp dòng số ' + (j + 1) + '!');
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
        url: "/DMNguon/ValidateBeforeSave",
        data: { aListModel: listItem },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu danh mục nguồn BTC cấp"
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
            iID_Nguon: "",
            sMaNguon: "",
            STT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            if (Bang_arrThayDoi[i][Bang_arrCSMaCot["iID_Nguon"]] == true) {
                object.iID_Nguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Nguon"]];
                object.sMaNguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguon"]];
                object.STT = i + 1;
                object.bDelete = false;
                listItem.push(object);
            } else {
                if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sMaNguon"]] == true) {
                    object.iID_Nguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Nguon"]];
                    object.sMaNguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguon"]];
                    object.STT = i + 1;
                    object.bDelete = false;
                    listItem.push(object);
                }
            }
        } else {
            object.iID_Nguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Nguon"]];
            object.sMaNguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguon"]];
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
    //DMNguon_BangDuLieu_Save();
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    console.log(h);
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
        }
    }
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
            iID_Nguon: "",
            sMaNguon: "",
            sMaNguonCha: "",
            iSTT: ""
        };
        if (Bang_arrHangDaXoa[i] == false) {
            object.iID_Nguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Nguon"]];
            object.sMaNguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguon"]];
            object.sMaNguonCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguonCha"]];
            object.iSTT = Bang_arrGiaTri[i][Bang_arrCSMaCot["iSTT"]];
            object.bDelete = false;
            listItem.push(object);
        } else {
            object.iID_Nguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Nguon"]];
            object.sMaNguon = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguon"]];
            object.sMaNguonCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaNguonCha"]];
            object.iSTT = Bang_arrGiaTri[i][Bang_arrCSMaCot["iSTT"]];
            object.bDelete = true;
            listItem.push(object);
        }
    }
    return listItem;
}

function TaoMaNguonTuDong(sMaNguonCha) {
    var lstData = GetListData();
    var lstData = lstData.filter(x => x.sMaNguonCha == sMaNguonCha);
    if (sMaNguonCha != "")
        lstMaND = lstData.map(x => (x.sMaNguon.indexOf(sMaNguonCha) < 0 ? x.sMaNguon : x.sMaNguon.substr(x.sMaNguon.indexOf(sMaNguonCha) + sMaNguonCha.length, x.sMaNguon.length - sMaNguonCha.length)));
    else
        lstMaND = lstData.map(x => x.sMaNguon);
    lstMaND = lstMaND.filter(x => parseInt(x));
    var intMax = 0;
    if (lstMaND.length > 0)
        intMax = Math.max(...lstMaND);
    intMax++;
    return sMaNguonCha + ("00" + intMax).substr(-3);
}

function TaoISTTTuDong(sMaNguonCha) {
    var lstData = GetListData();
    var iSTTCha = "";
    if (sMaNguonCha != "") {
        var objCha = lstData.filter(x => x.sMaNguon == sMaNguonCha);
        if (objCha != null)
            iSTTCha = objCha[0].iSTT;
    }
    var lstData = lstData.filter(x => x.sMaNguonCha == sMaNguonCha);
    var lstIndex = lstData.map(x => x.iSTT.substring(x.iSTT.lastIndexOf('.') + 1, x.iSTT.length));

    lstIndex = lstIndex.filter(x => parseInt(x));
    var intMax = 0;
    if (lstIndex.length > 0)
        intMax = Math.max(...lstIndex);
    intMax++;

    if (sMaNguonCha != "")
        return iSTTCha + "." + intMax;
    return intMax.toString();
}