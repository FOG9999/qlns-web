var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onCellAfterEdit(h, c) {
    //var cGhiChu = Bang_arrCSMaCot["sGhiChu"];
    //if (c == cGhiChu) {
    //    return true;
    //}

    //var fGiaTriNamThuNhat = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNhat"]];
    //var fGiaTriNamThuHai = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuHai"]];
    //var fGiaTriNamThuBa = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuBa"]];
    //var fGiaTriNamThuTu = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuTu"]];
    //var fGiaTriNamThuNam = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNam"]];
    //var fGiaTriBoTri = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriBoTri"]];

    //var fTongSo = fGiaTriNamThuNhat + fGiaTriNamThuHai + fGiaTriNamThuBa + fGiaTriNamThuTu + fGiaTriNamThuNam;
    //var fTongSoNhuCauNSQP = fTongSo + fGiaTriBoTri;

    //Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongSo"], fTongSo, 1);
    //Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], fTongSoNhuCauNSQP, 1);

    //BangDuLieu_CapNhapLaiHangCha(h, c);
    

    return true;
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    //if (BangDuLieu_CoCotDuyet && c >= Bang_arrMaCot.length - 2) {
    //    return;
    //}
    //var j;

    //if (c > 0) {
    //    //  BangDuLieu_TinhOConLai(h, c);
    //}
    //// BangDuLieu_TinhOTongSo(h);

    //var csCha = h, GiaTri;

    //while (Bang_arrCSCha[csCha] >= 0) {
    //    csCha = Bang_arrCSCha[csCha];

    //    var cHanMucDauTu = Bang_arrCSMaCot["fHanMucDauTu"]

    //    var fLKHanMucDauTu = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu, fLKHanMucDauTu);

    //    var fLKTongSoNhuCauNSQP = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 1);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 1, fLKTongSoNhuCauNSQP);

    //    var fLKTongSo = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 2);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 2, fLKTongSo);

    //    var fLKGiaTriNamThuNhat = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 3);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 3, fLKGiaTriNamThuNhat);

    //    var fLKGiaTriNamThuHai = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 4);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 4, fLKGiaTriNamThuHai);

    //    var fLKGiaTriNamThuBa = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 5);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 5, fLKGiaTriNamThuBa);

    //    var fLKGiaTriNamThuTu = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 6);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 6, fLKGiaTriNamThuTu);

    //    var fLKGiaTriNamThuNam = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 7);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 7, fLKGiaTriNamThuNam);

    //    var fLKGiaTriBoTri = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 8);
    //    Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 8, fLKGiaTriBoTri);

    //}
}

function BangDuLieu_TinhOConLai(h, c) {
    var GiaTri1 = Bang_arrGiaTri[h][c - 1];
    var GiaTri2 = Bang_arrGiaTri[h][c];
    var GiaTri3 = Bang_arrGiaTri[h][c + 1];
    Bang_GanGiaTriThatChoO(h, c + 2, GiaTri1 - GiaTri2 - GiaTri3);
}

function BangDuLieu_TinhTongHangCon(csCha, c) {
    //var h, vR = 0;
    ////tuannn sua them dk cot sTenCongTrinh 31/7/12
    //for (h = 0; h < Bang_arrCSCha.length; h++) {
    //    if (csCha == Bang_arrCSCha[h] && Bang_arrMaCot[c] != "sTenCongTrinh") {
    //        vR += parseFloat(Bang_arrGiaTri[h][c]);
    //    }
    //}
    //return vR;
}


function BangDuLieu_onKeypress_F2(h, c) {
    //BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_F1_NoRow(h, c) {
    BangDuLieu_ThemHangMoiGroup(0, undefined);
}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    //if (c == undefined) {
    //    BangDuLieu_ThemHangMoi(h, c);
    //} else {
    //    BangDuLieu_ThemHangMoi(h + 1, h);
    //}
}


function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function KHVonNamDX_ChiTiet_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    //if (!ValidateData()) {
    //    return false;
    //}
    ValidateBeforeSave();
}


function AjaxRequire(listItemAfter, listItemDeleted) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/SaveChiTiet",
        data: { aListModel: listItemAfter, listItemDeleted},
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu kế hoạch vốn năm đề xuất chi tiết"
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
    var listItemAfter = [], listItemDeleted = [];

    for (var i = 0; i < Bang_arrGiaTri.length; i++) {
        var object = {
            iID_KeHoachVonNamDeXuatChiTietID: "",
            iID_KeHoachVonNamDeXuatID: "",
            iID_DuAnID: "",
            sMaDuAn: "",            
            fTongMucDauTuDuocDuyet: 0,
            fLuyKeVonNamTruoc: 0,
            fKeHoachVonDuocDuyetNamNay: 0,
            fVonKeoDaiCacNamTruoc: 0,
            fUocThucHien: 0,
            fThuHoiVonUngTruoc: 0,
            fThanhToan: 0,
            iID_DonViTienTeID: "",
            iID_TienTeID: "",
            fTiGiaDonVi: 0,
            fTiGia: 0,
            sTrangThaiDuAnDangKy: "",
            iLoaiDuAn: 0,
            iID_LoaiCongTrinh: "",
            iID_DonViID: "",
            fUocThucHienDC: 0,
            fThuHoiVonUngTruocDC: 0,
            fThanhToanDC: 0,
            iID_DuAn_HangMucID: ""

        };

        object.iID_KeHoachVonNamDeXuatChiTietID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNamDeXuatChiTietID"]];
        object.iID_KeHoachVonNamDeXuatID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNamDeXuatID"]];
        object.iID_DuAnID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAnID"]];
        object.sMaDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]];
        object.fTongMucDauTuDuocDuyet = Bang_arrGiaTri[i][Bang_arrCSMaCot["fTongMucDauTuDuocDuyet"]];
        object.fLuyKeVonNamTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fLuyKeVonNamTruoc"]];
        object.fKeHoachVonDuocDuyetNamNay = Bang_arrGiaTri[i][Bang_arrCSMaCot["fKeHoachVonDuocDuyetNamNay"]];
        object.fVonKeoDaiCacNamTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fVonKeoDaiCacNamTruoc"]];
        object.fUocThucHien = Bang_arrGiaTri[i][Bang_arrCSMaCot["fUocThucHien"]];
        object.fThuHoiVonUngTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThuHoiVonUngTruoc"]];
        object.fThanhToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThanhToan"]];
        object.fUocThucHienDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fUocThucHienDC"]];
        object.fThuHoiVonUngTruocDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThuHoiVonUngTruocDC"]];
        object.fThanhToanDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThanhToanDC"]];
        object.iID_LoaiCongTrinh = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinh"]];
        object.iID_DonViID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DonViQuanLyID"]];
        object.iLoaiDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sLoaiDuAn"]] == "Mở mới" ? 1 : 2;
        object.iID_DuAn_HangMucID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAn_HangMucID"]];

        if (!Bang_arrHangDaXoa[i]) {
            listItemAfter.push(object);
        }
        else listItemDeleted.push(object);
    }

    AjaxRequire(listItemAfter, listItemDeleted);
}

function BangDuLieu_onKeypress_F9(h, c) {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".btn-mvc").click();
}

function BangDuLieu_onKeypress_F10() {
    //DMNoiDungChi_BangDuLieu_Save();
}

//function Bang_onKeypress_F(strKeyEvent) {
//    var h = Bang_keys.Row();
//    var c = Bang_keys.Col() - Bang_nC_Fixed;
//    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
//    var fn = window[TenHam];
//    if (typeof fn == 'function') {
//        if (fn(h, c) == false) {
//            return false;
//        }
//    }
//    //if (strKeyEvent == "F10") {
//    //    //Bang_HamTruocKhiKetThuc();
//    //}
//}

function setFontWeight() {
    for (var j = 0; j < Bang_nH; j++) {
        var numChild = Bang_LayGiaTri(j, "numChild");
        if (numChild > 0) {
            Bang_GanGiaTriThatChoO_colName(j, "sFontBold", "bold");
            Bang_GanGiaTriThatChoO_colName(j, "sMauSac", "whitesmoke");
        }
    }
}

function BangDuLieu_onKeypress_Delete(h, c) {
    console.log('Delete item...');
    var numChild = Bang_LayGiaTri(h, "numChild");
    if (numChild > 0) {
        var iIdParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_KeHoachVonNamDexuatChiTietID"]];
        var childIndex = [];
        for (var index = (h + 1); index < Bang_nH; index++) {
            var iIdChild = Bang_arrGiaTri[index][Bang_arrCSMaCot["iIDReference"]];
            if (iIdChild == iIdParent) {
                childIndex.push(index)
            } else {
                break;
            }
        }
        if (childIndex.length != 0) {
            childIndex.forEach(function (deleteIndex) {
                BangDuLieu_XoaHang(deleteIndex);
                if (deleteIndex < Bang_nH) {
                    Bang_keys.fnSetFocus(deleteIndex, c);
                }
            })
        }
        //var Title = 'Lỗi xoá dự án';
        //var sMessError = 'Dự án cha ' + Bang_LayGiaTri(h, "sTen") + ' không được phép xoá!';
        //$.ajax({
        //    type: "POST",
        //    url: "/Modal/OpenModal",
        //    data: { Title: Title, Messages: sMessError, Category: ERROR },
        //    success: function (data) {
        //        window.parent.loadModal(data);
        //    }
        //});
        //return false;
    }

    if (BangDuLieu_DuocSuaChiTiet && h != null && !Bang_arrHangDaXoa[h]) {
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