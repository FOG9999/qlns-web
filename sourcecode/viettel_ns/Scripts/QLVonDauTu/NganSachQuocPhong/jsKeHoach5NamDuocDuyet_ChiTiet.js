var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var ERROR = 1;

function BangDuLieu_onCellAfterEdit(h, c) {
    var cGhiChu = Bang_arrCSMaCot["sGhiChu"];
    if (c == cGhiChu) {
        return true;
    }

    var fHanMucDauTu = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanMucDauTu"]];
    var fVonBoTriTuNamDenNam = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonBoTriTuNamDenNam"]];
    var fGiaTriBoTri = fHanMucDauTu - fVonBoTriTuNamDenNam;

    var fVonBoTriTuNamDenNamDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonBoTriTuNamDenNamDc"]];
    var fGiaTriBoTriDc = fHanMucDauTu - fVonBoTriTuNamDenNamDc;

    var sNguonVon = Bang_arrGiaTri[h][Bang_arrCSMaCot["sTenNganSach"]];
    var iNguonVonId = "0";
    var fTongNhuCauNSQP = 0;
    if (sNguonVon.includes(".")) {
        iNguonVonId = sNguonVon.split(".")[0].trim();
    }
    else {
        iNguonVonId = sNguonVon;
    }

    if (iNguonVonId == "1") {
        fTongNhuCauNSQP = fHanMucDauTu;
    }

    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongNhuCauNSQP"], fTongNhuCauNSQP, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBoTri"], fGiaTriBoTri);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBoTriDc"], fGiaTriBoTriDc);

    //BangDuLieu_CapNhapLaiHangCha(h, c);
    return true;
}

function BangDuLieu_onKeypress_Delete(h, c) {
    var numChild = Bang_LayGiaTri(h, "numChild");
    if (numChild > 0) {
        var Title = 'Lỗi xoá dự án';
        var sMessError = 'Dự án cha ' + Bang_LayGiaTri(h, "sTen") + ' không được phép xoá.';
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

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

function BangDuLieu_onKeypress_F2(h, c) {
    //BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_F1_NoRow(h, c) {
    BangDuLieu_ThemHangMoiGroup(0, undefined);
}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    if (c == undefined) {
        BangDuLieu_ThemHangMoi(h, c);
    } else {
        BangDuLieu_ThemHangMoi(h + 1, h);
    }
}

function BangDuLieu_onKeypress_F3_NoRow(h, c) {
    if (c == undefined) {
        //BangDuLieu_ThemHangMoi(h, c);
        var sMessError = [];
        var Title = 'Lỗi thêm chi tiết dự án';
        sMessError.push('Chưa có dự án nào để thêm chi tiết.');
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
    } else {
        BangDuLieu_ThemHangChiTietDuAn(h + 1, h);
    }
}

function BangDuLieu_ThemHangMoiGroup(h, hGiaTri) {
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
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");
    //Set ID dòng mới 
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Id"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["STT"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Level"], 1);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (c != 1) {
            Bang_arrEdit[csH][c] = false;
        } else {
            Bang_arrEdit[csH][c] = true;
        }
    }
    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#4299e1");
}

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

    //TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
    } else {
        Bang_ThemHang(csH, hGiaTri);
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");
    //Set ID dòng mới 
    var newId = uuidv4();
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Level"], 2);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Id"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["STT"], "");
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (c > 0 && c < 7) {
            Bang_arrEdit[csH][c] = true;
        } else {
            Bang_arrEdit[csH][c] = false;
        }

        if (c > 6 && c < 18) {
            Bang_GanGiaTriThatChoO(h, c, "");
        }
    }
    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#229743");
}

function BangDuLieu_ThemHangChiTietDuAn(h, hGiaTri) {
    var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["STT"]];
    var LevelGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["Level"]];
    var sMessError = [];
    var Title = 'Lỗi thêm chi tiết dự án';
    if (STTGiaTri == "") {
        sMessError.push('Dòng dữ liệu tại dòng số ' + (hGiaTri + 1) + ' chưa được lưu.');
    }
    if (LevelGiaTri == "1") {
        sMessError.push('Không thêm chi tiết cho nhóm dự án tại dòng số ' + (hGiaTri + 1) + '.');
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
    
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }

    //TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
    } else {
        Bang_ThemHang(csH, hGiaTri);
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");
    //Set ID dòng mới 
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Id"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["STT"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["Level"], 3);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var IdReferenceGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["IdReference"]];
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["IdReference"], IdReferenceGiaTri);

    var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["STT"]];
    var sTenGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sTen"]];
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], STTGiaTri + " - " +sTenGiaTri);

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (c < 7) {
            Bang_arrEdit[csH][c] = false;
        } else {
            Bang_arrEdit[csH][c] = true;
        }
    }
    Bang_keys.fnSetFocus(csH, 7);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#FFD814");
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function KH5NamDX_ChiTiet_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    if (!ValidateData()) {
        return false;
    }
    ValidateBeforeSave();
}

function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu kế hoạch trung hạn đề xuất chi tiết';

    for (var j = 0; j < Bang_nH; j++) {
        var sTen = Bang_LayGiaTri(j, "sTen");
        if (sTen == '') {
            sMessError.push('Hãy nhập tên dự án dòng số ' + (j + 1) + '.');
        }
        if (Bang_LayGiaTri(j, "Level") == "2") {
            var sTenDonViQL = Bang_LayGiaTri(j, "sTenDonViQL");
            if (sTenDonViQL == '') {
                sMessError.push('Hãy chọn đơn vị quản lý dòng số ' + (j + 1) + '.');
            }
        }

        if (Bang_LayGiaTri(j, "Level") == "3") {
            var sTenLoaiCongTrinh = Bang_LayGiaTri(j, "sTenLoaiCongTrinh");
            if (sTenLoaiCongTrinh == '') {
                sMessError.push('Hãy chọn loại công trình dòng số ' + (j + 1) + '.');
            }
            var sTenNganSach = Bang_LayGiaTri(j, "sTenNganSach");
            if (sTenNganSach == '') {
                sMessError.push('Hãy chọn nguồn vốn dòng số ' + (j + 1) + '.');
            }
        }
        var itemParent = Bang_LayGiaTri(j, "iID_ParentID");
        var fVonBoTri = Bang_LayGiaTri(j, "fGiaTriBoTri");
        if (itemParent != '' && itemParent != GUID_EMPTY) {
            fVonBoTri = Bang_LayGiaTri(j, "fGiaTriBoTriDc");
        }
        if (fVonBoTri < 0) {
            sMessError.push('Vốn bố trí sau năm đang âm, không được nhập quá hạn mức đầu tư dòng số ' + (j + 1) + '.');
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
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/ValidateBeforeSave",
        data: { aListModel: listItem },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu kế hoạch trung hạn đề xuất chi tiết"
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
    var listItemGoc = [];
    var listItemReference = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            Id:"",
            IdReference: "",
            iID_LoaiCongTrinhID: "",
            iID_NguonVonID: "",
            STT: ""
        };

        var IdReferenceGiaTri = Bang_arrGiaTri[i][Bang_arrCSMaCot["IdReference"]];
        if (IdReferenceGiaTri != "") {
            object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
            object.IdReference = IdReferenceGiaTri;
            object.iID_LoaiCongTrinhID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
            object.iID_NguonVonID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NguonVonID"]];
            object.STT = i + 1;
            listItemReference.push(object);
            listItem.push(object);
        } else {
            object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
            object.IdReference = IdReferenceGiaTri;
            object.iID_LoaiCongTrinhID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
            object.iID_NguonVonID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NguonVonID"]];
            object.STT = i + 1;
            listItemGoc.push(object);
            listItem.push(object);
        }
        //check row gốc thay đổi trùng với dòng giữ liệu copy
        if (listItemReference.length > 0 && listItemGoc.length > 0) {

        }
        

        //if (Bang_arrThayDoi[i][Bang_arrCSMaCot["Id"]] == true) {
        //    object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
        //    object.sTen = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTen"]];
        //    object.STT = i + 1;
        //    listItem.push(object);
        //} else {
        //    if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sTen"]] == true) {
        //        object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
        //        object.sTen = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTen"]];
        //        object.STT = i + 1;
        //        listItem.push(object);
        //    }
        //}
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