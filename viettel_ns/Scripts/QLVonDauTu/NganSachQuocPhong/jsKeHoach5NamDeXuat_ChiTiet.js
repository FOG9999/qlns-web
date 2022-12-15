var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var isSetILevel = true;
var isSetIdDuAn = true;

CheckDuplicateBudgetLct = (h, c) => {
    var isDupliate = false;
    var idNguonVon = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_NguonVonID"]];
    var idLoaiCongTrinh = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
    var itemReference = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];

    var iCountNV = 0;
    for (var item = 0; item <= Bang_nH; item++) {
        var itemCrReferent = Bang_arrGiaTri[item][Bang_arrCSMaCot["iIDReference"]];
        var itemCrLct = Bang_arrGiaTri[item][Bang_arrCSMaCot["iID_LoaiCongTrinhID"]];
        var itemCrNv = Bang_arrGiaTri[item][Bang_arrCSMaCot["iID_NguonVonID"]];
        if (itemReference == itemCrReferent && itemCrLct == idLoaiCongTrinh && itemCrNv == idNguonVon) {
            iCountNV += 1;
        }
    }

    if (iCountNV >= 2) {
        isDupliate = true;
    }

    return isDupliate;
}

function BangDuLieu_onCellAfterEdit(h, c) {
    var cGhiChu = Bang_arrCSMaCot["sGhiChu"];
    if (c == cGhiChu) {
        return true;
    }

    //if (CheckDuplicateBudgetLct(h, c)) {
    //    var Title = 'Lỗi th';
    //    var sMessError = 'Dự án không được trùng nguồn vốn và loại công trình!';
    //    $.ajax({
    //        type: "POST",
    //        url: "/Modal/OpenModal",
    //        data: { Title: Title, Messages: sMessError, Category: ERROR },
    //        success: function (data) {
    //            window.parent.loadModal(data);
    //        }
    //    });
    //}

    var fGiaTriNamThuNhat = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNhat"]];
    var fGiaTriNamThuHai = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuHai"]];
    var fGiaTriNamThuBa = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuBa"]];
    var fGiaTriNamThuTu = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuTu"]];
    var fGiaTriNamThuNam = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNam"]];

    var fGiaTriNamThuNhatDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNhatDc"]];
    var fGiaTriNamThuHaiDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuHaiDc"]];
    var fGiaTriNamThuBaDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuBaDc"]];
    var fGiaTriNamThuTuDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuTuDc"]];
    var fGiaTriNamThuNamDc = Bang_arrGiaTri[h][Bang_arrCSMaCot["fGiaTriNamThuNamDc"]];

    var fVon5namNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVon5namNganhDX"]];
    var fVonsaunamNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonsaunamNganhDX"]];
    var fHanmucNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanmucNganhDX"]];
    var fVon5namCTCDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVon5namCTCDX"]];
    var fVonsaunamCTCDexuat = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonsaunamCTCDexuat"]];
    var fHanmucCucTCDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanmucCucTCDX"]];

    var modelParent = $("#ModelParentId").val();

    var fHanMucDauTu = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanMucDauTu"]];
    var sNguonVon = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_NguonVonID"]];

    var fTongSo = fGiaTriNamThuNhat + fGiaTriNamThuHai + fGiaTriNamThuBa + fGiaTriNamThuTu + fGiaTriNamThuNam;
    var fTongSoDc = fGiaTriNamThuNhatDc + fGiaTriNamThuHaiDc + fGiaTriNamThuBaDc + fGiaTriNamThuTuDc + fGiaTriNamThuNamDc;
    var fGiaTriSau5Nam = fHanMucDauTu - fTongSo;
    var fTongVonBoTriNganh = fVon5namNganhDX + fVonsaunamNganhDX;
    var fTongVonBoTriCuc = fVon5namCTCDX + fVonsaunamCTCDexuat;
    var fGiaTriSauNganh = fHanmucNganhDX - fTongVonBoTriNganh;
    var fGiaTriSauCuc = fHanmucCucTCDX - fTongVonBoTriCuc;

    var Title = "Cảnh báo"
    var sMessError = [];
    if (fGiaTriSau5Nam < 0 || fGiaTriSauNganh < 0 || fGiaTriSauCuc < 0) {
        sMessError.push('Tổng vốn bố trí không được vượt quá hạn mức đầu tư.');
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

    if (modelParent != '' && modelParent != GUID_EMPTY) {
        fTongSo = fTongSoDc;
    }

    var fTongSoNhuCauNSQP = 0;
    var iNguonVonId = "0";
    if (sNguonVon.includes("-")) {
        iNguonVonId = sNguonVon.split("-")[0].trim();
    }
    else {
        iNguonVonId = sNguonVon;
    }

    if (iNguonVonId == "1") {
        fTongSoNhuCauNSQP = fHanMucDauTu;
    }

    var fGiaTriSau5NamDc = fHanMucDauTu - fTongSoDc;

    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongSo"], fTongSo, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], fTongSoNhuCauNSQP, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBoTri"], fGiaTriSau5Nam, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBoTriDc"], fGiaTriSau5NamDc, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongVonBoTriNganh"], fTongVonBoTriNganh, 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fTongVonBoTriCuc"], fTongVonBoTriCuc, 1);

    var modelType = $("#ModelLoai").val();
    if (modelType != '2') {
        BangDuLieu_CapNhapLaiHangCha(h, c);
    }

    return true;
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

        var fLKTongSoNhuCauNSQP = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 1);
        Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 1, fLKTongSoNhuCauNSQP);

        var fLKTongSo = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 2);
        Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 2, fLKTongSo);

        var modelParent = $("#ModelParentId").val();
        if (modelParent != '' && modelParent != GUID_EMPTY) {
            var fLKGiaTriNamThuNhat = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 3);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 3, fLKGiaTriNamThuNhat);

            var fLKGiaTriNamThuNhatDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 4);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 4, fLKGiaTriNamThuNhatDc);

            var fLKGiaTriNamThuHai = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 5);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 5, fLKGiaTriNamThuHai);

            var fLKGiaTriNamThuHaiDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 6);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 6, fLKGiaTriNamThuHaiDc);

            var fLKGiaTriNamThuBa = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 7);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 7, fLKGiaTriNamThuBa);

            var fLKGiaTriNamThuBaDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 8);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 8, fLKGiaTriNamThuBaDc);

            var fLKGiaTriNamThuTu = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 9);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 9, fLKGiaTriNamThuTu);

            var fLKGiaTriNamThuTuDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 10);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 10, fLKGiaTriNamThuTuDc);

            var fLKGiaTriNamThuNam = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 11);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 11, fLKGiaTriNamThuNam);

            var fLKGiaTriNamThuNamDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 12);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 12, fLKGiaTriNamThuNamDc);

            var fLKGiaTriBoTri = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 13);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 13, fLKGiaTriBoTri);

            var fLKGiaTriBoTriDc = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 14);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 14, fLKGiaTriBoTriDc);
        }
        else {
            var fLKGiaTriNamThuNhat = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 3);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 3, fLKGiaTriNamThuNhat);

            var fLKGiaTriNamThuHai = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 4);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 4, fLKGiaTriNamThuHai);

            var fLKGiaTriNamThuBa = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 5);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 5, fLKGiaTriNamThuBa);

            var fLKGiaTriNamThuTu = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 6);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 6, fLKGiaTriNamThuTu);

            var fLKGiaTriNamThuNam = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 7);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 7, fLKGiaTriNamThuNam);

            var fLKGiaTriBoTri = BangDuLieu_TinhTongHangCon(csCha, cHanMucDauTu + 8);
            Bang_GanGiaTriThatChoO(csCha, cHanMucDauTu + 8, fLKGiaTriBoTri);
        }
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
    console.log('Delete item...');
    /*
    var numChild = Bang_LayGiaTri(h, "numChild");
    
    if (numChild > 0) {
        var iIdParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"]];
        var childIndex = [];
      //  childIndex.push(h);
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
    */

    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        // check xem hàng h đã bị xóa chưa, nếu chưa bị xóa thì xóa, nếu xóa r thì thôi
        if (!Bang_arrHangDaXoa[h]) {
            BangDuLieu_XoaHang(h);
            if (h < Bang_nH) {
                Bang_keys.fnSetFocus(h, c);
            }
            else if (Bang_nH > 0) {

            }
        } else {
            BangDuLieu_XoaHang(h);
        }
    }
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];

        //khi xóa hàng được chọn, đánh dấu xóa luôn cả những hàng con của nó 
        for (let i = 0; i < Bang_arrCSCha.length; i++) {
            if (Bang_arrCSCha[i] == cs) {
                Bang_arrHangDaXoa[i] = !Bang_arrHangDaXoa[i];
                for (let j = 0; j < Bang_arrCSCha.length; j++) {
                    if (Bang_arrCSCha[j] == i) {
                        Bang_arrHangDaXoa[j] = !Bang_arrHangDaXoa[j];
                    }
                }
            }
        }
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
    if (Bang_arrHangDaXoa[h]) {
        var Title = 'Lỗi thêm mới dự án';
        var sMessError = 'Dự án ' + Bang_LayGiaTri(h, "sTen") + ' đã bị xóa, không được phép thêm mới.';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: sMessError, Category: ERROR },
            success: function (data) {
                window.parent.loadModal(data);
                return;
            }
        });
        return;
    }
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
        if (Bang_arrHangDaXoa[h]) {
            var Title = 'Lỗi thêm mới dự án';
            var sMessError = 'Dự án ' + Bang_LayGiaTri(h, "sTen") + ' đã bị xóa, không được phép thêm mới.';
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: Title, Messages: sMessError, Category: ERROR },
                success: function (data) {
                    window.parent.loadModal(data);
                    return;
                }
            });
            return;
        } else {
            BangDuLieu_ThemHangChiTietDuAn(h + 1, h);
        }
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
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSTT"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 1);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_DonViID"], localStorage.getItem("iID_DonViQuanLyID"));
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDonVi"], localStorage.getItem("sDonVi"));
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var itemParent = $("#ModelParentId").val();

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (itemParent != '' && itemParent != GUID_EMPTY) {
            if (c > 0 && c <= 26 && c != 9 && c != 10 && c != 11 && c != 13 && c != 15 && c != 17 && c != 19 && c != 21 && c != 22 && c != 2) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
        else {
            if (c > 0 && c <= 26 && c != 9 && c != 10 && c != 16 && c != 2 && c != 22 && c != 18) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
    }

    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#4299e1");
}

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    var csH = 0;
    if (hGiaTri != undefined) {
        var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sSTT"]];
        var LevelGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iLevel"]];
        var sMessError = [];
        var Title = 'Lỗi thêm dự án';
        if (STTGiaTri == "") {
            sMessError.push('Dòng dữ liệu tại dòng số ' + (hGiaTri + 1) + ' chưa được lưu.');
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
    }
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }

    // cập nhật các giá trị cho hàng cha
    if (LevelGiaTri == 1) {
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanTu"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanDen"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDiaDiem"], "");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDonViThucHienDuAn"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenNganSach"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenLoaiCongTrinh"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanMucDauTu"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSo"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhat"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhatDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHai"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHaiDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBa"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBaDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTu"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTuDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNam"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNamDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTri"], "0");

        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanmucNganhDX"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongVonBoTriNganh"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fVon5namNganhDX"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fVonsaunamNganhDX"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanmucCucTCDX"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongVonBoTriCuc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fVon5namCTCDX"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fVonnamthunhatCTC"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fVonsaunamCTCDexuat"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fCucTCDeXuat"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fDuKienBoTriNamThu2"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sGhiChu"], "0");
    }

    //TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
        Bang_arrCSCha[h] = hGiaTri;
    } else {
        Bang_ThemHang(csH, hGiaTri);
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");
    //Set ID dòng mới 
    var newId = uuidv4();

    if (hGiaTri != undefined) {
        var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sSTT"]];
        var sTenGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sTen"]];
        var sLevelParent = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iLevel"]];
        if (sLevelParent == "1") {
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], STTGiaTri + " - " + sTenGiaTri);
        } else {
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], "");
            for (var c = 0; c < Bang_nC; c++) {
                Bang_GanGiaTriThatChoO(h, c, "");
            }
            Bang_arrCSCha[h] = -1;
        }
    }

    // cập nhật giá trị cho hàng mới
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 2);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSTT"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_DonViID"], localStorage.getItem("iID_DonViQuanLyID"));
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDonVi"], localStorage.getItem("sDonVi"));

    /*// cập nhật các giá trị cho hàng cha
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanTu"], ""); 
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanDen"], ""); 
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDiaDiem"], ""); 
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDonViThucHienDuAn"], "");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenNganSach"], "");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenLoaiCongTrinh"], "");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanMucDauTu"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSo"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhat"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhatDc"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHai"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHaiDc"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBa"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBaDc"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTu"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTuDc"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNam"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNamDc"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTri"], "0");
    Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTriDc"], "0");*/

    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var itemParent = $("#ModelParentId").val();

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (itemParent != '' && itemParent != GUID_EMPTY) {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 11 && c != 13 && c != 15 && c != 17 && c != 19 && c != 21 && c != 22 && c != 2) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
        else {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 16 && c != 2) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
    }
    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_ThemHangChiTietDuAn(h, hGiaTri) {
    var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sSTT"]];
    var LevelGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iLevel"]];
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

    // cập nhật lại giá trị cho dòng cha
    if (LevelGiaTri == "1" || LevelGiaTri == "2") {
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanTu"], "");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanDen"], "");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDiaDiem"], "");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenLoaiCongTrinh"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenNganSach"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanMucDauTu"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSo"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhat"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhatDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHai"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHaiDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBa"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBaDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTu"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTuDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNam"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNamDc"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTri"], "0");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTriDc"], "0");
    }

    //TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
        Bang_arrCSCha[h] = hGiaTri;
    } else {
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrCSCha[h] = hGiaTri;
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");

    // cập nhật lại giá trị cho dòng cha
    if (LevelGiaTri == "1" || LevelGiaTri == "2") {
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanTu"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["iGiaiDoanDen"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sDiaDiem"], "");
        Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenLoaiCongTrinh"], "");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["sTenNganSach"], "");                         
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fHanMucDauTu"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSo"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fTongSoNhuCauNSQP"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhat"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNhatDc"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHai"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuHaiDc"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBa"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuBaDc"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTu"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuTuDc"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNam"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriNamThuNamDc"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTri"], "0");
        //Bang_GanGiaTriThatChoO(hGiaTri, Bang_arrCSMaCot["fGiaTriBoTriDc"], "0");
    }


    /*//TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
        Bang_arrCSCha[h] = hGiaTri;
    } else {
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrCSCha[h] = hGiaTri;
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");*/
    //Set ID dòng mới 
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSTT"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 3);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_DonViID"], localStorage.getItem("iID_DonViQuanLyID"));
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDonVi"], localStorage.getItem("sDonVi"));
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var IdReferenceGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iIDReference"]];
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iIDReference"], IdReferenceGiaTri);

    var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sSTT"]];
    var sTenGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sTen"]];
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], STTGiaTri + " - " + sTenGiaTri);
    //Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fHanMucDauTu"], "0");

    var c;
    var itemParent = $("#ModelParentId").val();

    for (c = 0; c < Bang_nC; c++) {
        if (itemParent != '' && itemParent != GUID_EMPTY) {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 11 && c != 13 && c != 15 && c != 17 && c != 19 && c != 21 && c != 22 && c != 2) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
        else {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 16 && c != 2) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
        if (LevelGiaTri == "1" || LevelGiaTri == "2") {
            Bang_arrEdit[hGiaTri][c] = false;
        }
        else {
            Bang_arrEdit[hGiaTri][c] = true;
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
    alert("Lưu dữ liệu thành công !");
    ValidateBeforeSave();
}


function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu kế hoạch trung hạn đề xuất chi tiết';
    var listItemChiTiet = [];


    for (var j = 0; j < Bang_nH; j++) {
        var itemDelete = Bang_arrHangDaXoa[j];
        var fGiaTriNamThuNhat = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuNhat"]];
        var fGiaTriNamThuHai = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuHai"]];
        var fGiaTriNamThuBa = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuBa"]];
        var fGiaTriNamThuTu = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuTu"]];
        var fGiaTriNamThuNam = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuNam"]];

        var fGiaTriNamThuNhatDc = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuNhatDc"]];
        var fGiaTriNamThuHaiDc = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuHaiDc"]];
        var fGiaTriNamThuBaDc = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuBaDc"]];
        var fGiaTriNamThuTuDc = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuTuDc"]];
        var fGiaTriNamThuNamDc = Bang_arrGiaTri[j][Bang_arrCSMaCot["fGiaTriNamThuNamDc"]];

        var fHanMucDauTu = Bang_arrGiaTri[j][Bang_arrCSMaCot["fHanMucDauTu"]];

        var fVon5namNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVon5namNganhDX"]];
        var fVonsaunamNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonsaunamNganhDX"]];
        var fHanmucNganhDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanmucNganhDX"]];
        var fVon5namCTCDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVon5namCTCDX"]];
        var fVonsaunamCTCDexuat = Bang_arrGiaTri[h][Bang_arrCSMaCot["fVonsaunamCTCDexuat"]];
        var fHanmucCucTCDX = Bang_arrGiaTri[h][Bang_arrCSMaCot["fHanmucCucTCDX"]];
        var fTongVonBoTriNganh = fVon5namNganhDX + fVonsaunamNganhDX;
        var fTongVonBoTriCuc = fVon5namCTCDX + fVonsaunamCTCDexuat;
        var fGiaTriSauNganh = fHanmucNganhDX - fTongVonBoTriNganh;
        var fGiaTriSauCuc = fHanmucCucTCDX - fTongVonBoTriCuc;

        var fTongSo = fGiaTriNamThuNhat + fGiaTriNamThuHai + fGiaTriNamThuBa + fGiaTriNamThuTu + fGiaTriNamThuNam;
        var fTongSoDc = fGiaTriNamThuNhatDc + fGiaTriNamThuHaiDc + fGiaTriNamThuBaDc + fGiaTriNamThuTuDc + fGiaTriNamThuNamDc;
        var fGiaTriSau5Nam = fHanMucDauTu - fTongSo;
        var fGiaTriSau5NamDC = fHanMucDauTu - fTongSoDc;

        if (fGiaTriSau5Nam < 0 || fGiaTriSau5NamDC < 0 || fGiaTriSauNganh < 0 || fGiaTriSauCuc < 0 ) {
            sMessError.push('Tổng vốn bố trí không được vượt quá hạn mức đầu tư.');
        }

        if (itemDelete != true) {
            var itemParent = Bang_LayGiaTri(j, "iID_ParentModified");
            var itemValue = Bang_LayGiaTri(j, "fGiaTriBoTri");
            if (itemParent != '' && itemParent != GUID_EMPTY) {
                itemValue = Bang_LayGiaTri(j, "fGiaTriBoTriDc");
            }
            if (itemValue < 0) {
                sMessError.push('Giá trị sau năm đang âm, không được nhập quá hạn mức đầu tư ' + (j + 1) + '.');
            }
            var sTen = Bang_LayGiaTri(j, "sTen");
            if (sTen == '') {
                sMessError.push('Hãy nhập tên dự án dòng số ' + (j + 1) + '.');
            }
            if (Bang_LayGiaTri(j, "iLevel") == "2") {
                var sTenDonViQL = Bang_LayGiaTri(j, "sTenDonViQL");
                if (sTenDonViQL == '') {
                    sMessError.push('Hãy chọn đơn vị quản lý dòng số ' + (j + 1) + '.');
                }
            }

            if (Bang_LayGiaTri(j, "iLevel") == "3") {
                var sTenLoaiCongTrinh = Bang_LayGiaTri(j, "sTenLoaiCongTrinh");
                if (sTenLoaiCongTrinh == '') {
                    sMessError.push('Hãy chọn loại công trình dòng số ' + (j + 1) + '.');
                }
                var sTenNganSach = Bang_LayGiaTri(j, "sTenNganSach");
                if (sTenNganSach == '') {
                    sMessError.push('Hãy chọn nguồn vốn dòng số ' + (j + 1) + '.');
                }

                //Check trùng hạng mục các chi tiết của KHTHDX
                var object = {
                    iIDReference: "",
                    sTenLoaiCongTrinh: "",
                    sTenNganSach: "",
                };
                object.iIDReference = Bang_arrGiaTri[j][Bang_arrCSMaCot["iIDReference"]];
                object.sTenLoaiCongTrinh = Bang_arrGiaTri[j][Bang_arrCSMaCot["sTenLoaiCongTrinh"]];
                object.sTenNganSach = Bang_arrGiaTri[j][Bang_arrCSMaCot["sTenNganSach"]];
                listItemChiTiet.push(object);
            }
            if (Bang_LayGiaTri(j, "iLevel") == "10") {
                var objectCt = {
                    iID_DonViQuanLyID: "",
                    sTenLoaiCongTrinh: "",
                    sTenNganSach: "",
                    iID_DuAnID: "",
                    iID_MaDonVi: "",
                }

                objectCt.iID_DonViQuanLyID = Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_DonViQuanLyID"]];
                objectCt.sTenLoaiCongTrinh = Bang_arrGiaTri[j][Bang_arrCSMaCot["sTenLoaiCongTrinh"]];
                objectCt.sTenNganSach = Bang_arrGiaTri[j][Bang_arrCSMaCot["sTenNganSach"]];
                objectCt.iID_DuAnID = Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_DuAnID"]];
                objectCt.iID_MaDonVi = Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_MaDonVi"]];
                listItemChiTiet.push(objectCt);
            }            
        }
    }

    var listItemDuplicates = [];
    var listCheckDuplicates = [];
    const duplicates = listItemChiTiet.filter(item => {
        listCheckDuplicates.forEach(function (element) {
            if (element.iIDReference == null && item.iIDReference == null || element.iIDReference == undefined && item.iIDReference == undefined) {
                listItemDuplicates = [];
            }
            else if (element.iIDReference == item.iIDReference && element.sTenLoaiCongTrinh == item.sTenLoaiCongTrinh && element.sTenNganSach == item.sTenNganSach) {
                listItemDuplicates.push(item);
            }
        })
        listCheckDuplicates.push(item);
    });
    if (listItemDuplicates.length > 0) {
        sMessError.push('Dòng chi tiết dự án có thông tin trùng loại công trình và nguồn vốn.');
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

// ẩn các giá trị k muốn show trên dòng của một nhóm dự án
function hideGroupDuAnProps(row) {
    Bang_arrGiaTri[row][Bang_arrCSMaCot['iGiaiDoanTu']] = "";
    Bang_arrGiaTri[row][Bang_arrCSMaCot['iGiaiDoanDen']] = "";
    //Bang_arrGiaTri[row][Bang_arrCSMaCot['sDonViThucHienDuAn']] = "";
    Bang_arrGiaTri[row][Bang_arrCSMaCot['sDiaDiem']] = "";
}

function hideGroupDuAnProps2(row) {
    Bang_arrGiaTri[row][Bang_arrCSMaCot['iGiaiDoanTu']] = "";
    Bang_arrGiaTri[row][Bang_arrCSMaCot['iGiaiDoanDen']] = "";
    Bang_arrGiaTri[row][Bang_arrCSMaCot['sDiaDiem']] = "";
}

// in đậm dòng là nhóm dự án, vì nếu không có dự án thuộc nó thì nó sẽ k được in đậm
function setBoldForGroupDuAnRow() {
    for (let i = 0; i < Bang_arrLaHangCha.length; i++) {
        if (Bang_arrLaHangCha[i]) {
            if (Bang_arrGiaTri[i][Bang_arrCSMaCot["iLevel"]] === '1') {
                Bang_arrGiaTri[i][Bang_arrCSMaCot["sFontBold"]] = 'bold';
                Bang_arrThayDoi[i][Bang_arrCSMaCot["sFontBold"]] = true;
                hideGroupDuAnProps(i);
            }
            if (Bang_arrGiaTri[i][Bang_arrCSMaCot["iLevel"]] === '2') {
                hideGroupDuAnProps2(i);
            }
        }
    }
    Bang_HienThiDuLieu();
}

setTimeout(() => {
    setBoldForGroupDuAnRow();
}, 1500)