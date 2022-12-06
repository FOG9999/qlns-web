var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;
var isSetILevel = true;
var isSetIdDuAn = true;
var isSetTenLoaiCongTrinh = true;
var isSetTenNganSach = true;
var isSetTenDonViQL = true;
var isSetTenCDT = true;

function BangDuLieu_onCellAfterEdit(h, c) {
    var sMaDuAn = Bang_arrGiaTri[h][Bang_arrCSMaCot["sMaDuAn"]];
    var arrMaDuAn = sMaDuAn.split("-");
    var sMaDuAnNew = "";
    var cTenCDT = Bang_arrCSMaCot["sTenCDT"];
    if (c == cTenCDT) {
        var sTenCDT = Bang_arrGiaTri[h][Bang_arrCSMaCot["sTenCDT"]];
        if (sTenCDT != "") {
            var arrTenCDT = sTenCDT.split("-");
            sMaDuAnNew = arrMaDuAn[0] + "-" + arrTenCDT[0].trim() + "-" + arrMaDuAn[2];
        } else {
            sMaDuAnNew = arrMaDuAn[0] + "-" + "xxx" + "-" + arrMaDuAn[2];
        }
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaDuAn"], sMaDuAnNew, 1);

        updateChiTietDuAn(h);
    }

    var csTenDonViQL = Bang_arrCSMaCot["sTenDonViQL"];
    if (c == csTenDonViQL) {
        var ssTenDonViQL = Bang_arrGiaTri[h][Bang_arrCSMaCot["sTenDonViQL"]];
        if (ssTenDonViQL != "") {
            var arrsTenDonViQL = ssTenDonViQL.split("-");
            sMaDuAnNew = arrsTenDonViQL[0].trim() + "-" + arrMaDuAn[1] + "-" + arrMaDuAn[2];
        } else {
            sMaDuAnNew = "xxx" + "-" + arrMaDuAn[1] + "-" + arrMaDuAn[2];
        }
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaDuAn"], sMaDuAnNew, 1);
    }

    Bang_arrThayDoi[h][Bang_arrCSMaCot["isMap"]] = true;
    Bang_arrThayDoi[h][Bang_arrCSMaCot["iLevel"]] = true;
    Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_DuAnID"]] = true;

    BangDuLieu_CapNhapLaiHangCha(h, c);


    return true;
}

function updateChiTietDuAn(hDuAn) {
    var iID_DuAnID = Bang_arrGiaTri[hDuAn][Bang_arrCSMaCot["iID_DuAnID"]];
    var sTenCDT = Bang_arrGiaTri[hDuAn][Bang_arrCSMaCot["sTenCDT"]];
    var sMaDuAn = Bang_arrGiaTri[hDuAn][Bang_arrCSMaCot["sMaDuAn"]];
    for (var h = 0; h < Bang_nH; h++) {
        var iLevel = Bang_LayGiaTri(h, "iLevel");
        if (iLevel == "2") {
            var iID_DuAnIDChiTiet = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_DuAnID"]];
            if (iID_DuAnID == iID_DuAnIDChiTiet) {
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sTenCDT"], sTenCDT, 1);
                Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaDuAn"], sMaDuAn, 1);
            }
        }
    }
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (BangDuLieu_CoCotDuyet && c >= Bang_arrMaCot.length - 2) {
        return;
    }
    var j;

    if (c > 0) {
        //  BangDuLieu_TinhOConLai(h, c);
    }
    // BangDuLieu_TinhOTongSo(h);

    var csCha = h, GiaTri;

    while (Bang_arrCSCha[csCha] >= 0) {
        csCha = Bang_arrCSCha[csCha];

        var cHanMucDauTu = Bang_arrCSMaCot["fHanMucDauTu"]

        var fLKHanMucDauTu = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu);
        Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu, fLKHanMucDauTu);

    }
}

function BangDuLieu_TinhOConLai(h, c) {
    var GiaTri1 = Bang_arrGiaTri[h][c - 1];
    var GiaTri2 = Bang_arrGiaTri[h][c];
    var GiaTri3 = Bang_arrGiaTri[h][c + 1];
    Bang_GanGiaTriThatChoO(h, c + 2, GiaTri1 - GiaTri2 - GiaTri3);
}

function BangDuLieu_TinhTongHangCon(csCha, c) {
    var h, vR = 0;
    //tuannn sua them dk cot sTenCongTrinh 31/7/12
    for (h = 0; h < Bang_arrCSCha.length; h++) {
        if (csCha == Bang_arrCSCha[h] && Bang_arrMaCot[c] != "sTenCongTrinh") {
            vR += parseFloat(Bang_arrGiaTri[h][c]);
        }
    }
    return vR;
}

function BangDuLieu_onKeypress_Delete(h, c) {
    var numChild = Bang_LayGiaTri(h, "numChild");
    if (numChild > 0) {
        var Title = 'Lỗi xoá dự án';
        var sMessError = 'Dự án cha ' + Bang_LayGiaTri(h, "sTenDuAn") + ' không được phép xoá.';
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
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["IdRow"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_DuAnID"], "");
    var sMaDuAnhGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sMaDuAn"]];
    var arrsMaDuAn = sMaDuAnhGiaTri.split("-")
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaDuAn"], arrsMaDuAn[0] + "-" + arrsMaDuAn[1] + "-xxxx");
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var c;
    for (c = 0; c < Bang_nC; c++) {
        Bang_arrEdit[csH][c] = true;
    }
    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#229743");
}

function BangDuLieu_ThemHangChiTietDuAn(h, hGiaTri) {
    var iID_DuAnIDGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iID_DuAnID"]];
    var LevelGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iLevel"]];
    var sMessError = [];
    var Title = 'Lỗi thêm chi tiết dự án';
    if (iID_DuAnIDGiaTri == "") {
        sMessError.push('Dòng dữ liệu tại dòng số ' + (hGiaTri + 1) + ' chưa được lưu.');
    }
    //if (LevelGiaTri == "1") {
    //    sMessError.push('Không thêm chi tiết cho nhóm dự án tại dòng số ' + (hGiaTri + 1) + ' !');
    //}
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
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["IdRow"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 2);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

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

function DM_DuAn_KH5NDD_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    if (!ValidateData()) {
        return false;
    }
    ValidateBeforeSave();
}

function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu dự án - chi tiết dự án';

    for (var j = 0; j < Bang_nH; j++) {
        var sTen = Bang_LayGiaTri(j, "sTenDuAn");
        var isHangXoa = Bang_arrHangDaXoa[j];
        if (isHangXoa) {
            continue;
        }

        // nếu k phải dòng đang chọn thì k cần validate
        if (Bang_keys.Row() != j) {
            return true;
        }

        if (sTen == '') {
            sMessError.push('Hãy nhập tên dự án dòng số ' + (j + 1) + '.');
        }
        if (Bang_LayGiaTri(j, "iLevel") == "1") {
            var sTenDonViQL = Bang_LayGiaTri(j, "sTenDonViQL");
            if (sTenDonViQL == '') {
                sMessError.push('Hãy chọn đơn vị quản lý dòng số ' + (j + 1) + '.');
            }
        }

        if (Bang_LayGiaTri(j, "iLevel") == "2") {
            var sTenLoaiCongTrinh = Bang_LayGiaTri(j, "sTenLoaiCongTrinh");
            if (sTenLoaiCongTrinh == '') {
                sMessError.push('Hãy chọn loại công trình dòng số ' + (j + 1) + '.');
            }
            var sTenNganSach = Bang_LayGiaTri(j, "sTenNganSach");
            if (sTenNganSach == '') {
                sMessError.push('Hãy chọn nguồn vốn dòng số ' + (j + 1) + '.');
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
    Bang_HamTruocKhiKetThuc();
    //$.ajax({
    //    type: "POST",
    //    url: "/QLVonDauTu/KeHoachTrungHanDeXuat/ValidateBeforeSave",
    //    data: { aListModel: listItem },
    //    async: false,
    //    success: function (data) {
    //        if (data.status == false) {
    //            var Title = "Lỗi lưu dự án - chi tiết dự án"
    //            $.ajax({
    //                type: "POST",
    //                url: "/Modal/OpenModal",
    //                data: { Title: Title, Messages: data.listErrMess, Category: 1 },
    //                async: false,
    //                success: function (data) {
    //                    window.parent.loadModal(data);
    //                    return false;
    //                }
    //            });
    //        } else {
    //            Bang_HamTruocKhiKetThuc();
    //        }
    //    }
    //});
}


function ValidateBeforeSave() {
    var listItem = [];
    var listItemGoc = [];
    var listItemReference = [];

    //for (var i = 0; i < Bang_arrThayDoi.length; i++) {
    //    var object = {
    //        Id:"",
    //        IdReference: "",
    //        iID_LoaiCongTrinhID: "",
    //        iID_NguonVonID: "",
    //        STT: ""
    //    };

    //    var IdReferenceGiaTri = Bang_arrGiaTri[i][Bang_arrCSMaCot["IdReference"]];
    //    if (IdReferenceGiaTri != "") {
    //        object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
    //        object.IdReference = IdReferenceGiaTri;
    //        object.iID_LoaiCongTrinhID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
    //        object.iID_NguonVonID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NguonVonID"]];
    //        object.STT = i + 1;
    //        listItemReference.push(object);
    //        listItem.push(object);
    //    } else {
    //        object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
    //        object.IdReference = IdReferenceGiaTri;
    //        object.iID_LoaiCongTrinhID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
    //        object.iID_NguonVonID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NguonVonID"]];
    //        object.STT = i + 1;
    //        listItemGoc.push(object);
    //        listItem.push(object);
    //    }
    //    //check row gốc thay đổi trùng với dòng giữ liệu copy
    //    if (listItemReference.length > 0 && listItemGoc.length > 0) {

    //    }


    //    //if (Bang_arrThayDoi[i][Bang_arrCSMaCot["Id"]] == true) {
    //    //    object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
    //    //    object.sTen = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTen"]];
    //    //    object.STT = i + 1;
    //    //    listItem.push(object);
    //    //} else {
    //    //    if (Bang_arrThayDoi[i][Bang_arrCSMaCot["sTen"]] == true) {
    //    //        object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["Id"]];
    //    //        object.sTen = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTen"]];
    //    //        object.STT = i + 1;
    //    //        listItem.push(object);
    //    //    }
    //    //}
    //}

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

function BangDuLieu_onKeypress_F7_NoRow() {
    var yes = confirm("F2: Đ/c chắc chắn muốn chọn toàn bộ dự án hiện tại?");
    if (!yes)
        return;

    CheckAll(1);
}

function BangDuLieu_onKeypress_F8_NoRow() {
    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ chọn toàn bộ dự án hiện tại?");
    if (!yes)
        return;

    CheckAll(0);
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        var iLevel = Bang_arrGiaTri[i][Bang_arrCSMaCot["iLevel"]];
        if (!Bang_arrLaHangCha[i] || iLevel == 2) {
            Bang_GanGiaTriThatChoO_colName(i, "isMap", cs);
        }
    }
}

function DM_DuAn_BangDuLieu_ChonDuAn(h) {
    var iLevel = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
    var sMaDuAnCha = Bang_arrGiaTri[h][Bang_arrCSMaCot["sMaDuAn"]];
    if (!Bang_arrLaHangCha[h] || iLevel == 2) {
        Bang_GanGiaTriThatChoO_colName(h, "isMap", 1);
    }
    if (Bang_arrLaHangCha[h]) {
        for (var i = h; i < Bang_nH; i++) {
            var iLevel = Bang_arrGiaTri[i][Bang_arrCSMaCot["iLevel"]];
            var sMaDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]];
            if (sMaDuAn == sMaDuAnCha && (!Bang_arrLaHangCha[i] || iLevel == 2)) {
                Bang_GanGiaTriThatChoO_colName(i, "isMap", 1);
            }
        }
    }
}