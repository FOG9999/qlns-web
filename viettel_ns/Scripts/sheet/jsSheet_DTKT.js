var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onLoad() {
    if (typeof Bang_arrCSMaCot["bDongY"] == "undefined") {
        BangDuLieu_CoCotDuyet = false;
    }
    else {
        BangDuLieu_CoCotDuyet = true;
    }
    if (typeof Bang_arrCSMaCot["rTongSo"] == "undefined") {
        BangDuLieu_CoCotTongSo = false;
    }
    else {
        BangDuLieu_CoCotTongSo = true;
    }
}

function BangDuLieu_onCellAfterEdit(h, c) {

    // chi cong hang khi la NUMBER
    if (Bang_arrType[c] != "1")
        return;

    var tong = 0;
    for (var i = 0; i < Bang_nH - 1; i++) {
        if (Bang_arrLaHangCha[i] == false) {
            tong += Bang_arrGiaTri[i][c];
        }
    }
    Bang_GanGiaTriThatChoO(0, c, tong);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, c, tong);
    BangDuLieu_CapNhapLaiHangCha(h, c);
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (Bang_arrType[c] == 1) {
        var csCha = h, GiaTri;
        while (Bang_arrCSCha[csCha] >= 0) {
            csCha = Bang_arrCSCha[csCha];
            GiaTri = BangDuLieu_TinhTongHangCon(csCha, c);
            Bang_GanGiaTriThatChoO(csCha, c, GiaTri);
        }
        Mark_difference_values("edit", h, c);
    }
}

function BangDuLieu_TinhTongHangCon(csCha, c) {
    var h, vR = 0;
    for (h = 0; h < Bang_arrCSCha.length; h++) {
        if (csCha == Bang_arrCSCha[h]) {
            vR += parseFloat(Bang_arrGiaTri[h][c]);
        }
    }
    return vR;
}

var BangDuLieu_CheckAll_value = false;
function BangDuLieu_CheckAll() {
    BangDuLieu_CheckAll_value = !BangDuLieu_CheckAll_value;
    var value = "0";
    if (BangDuLieu_CheckAll_value) {
        value = "1";
    }
    else {
        value = "0";
    }
    var h, c = Bang_arrCSMaCot["bDongY"];
    for (h = 0; h < Bang_arrMaHang.length; h++) {
        if (Bang_arrLaHangCha[h] == false) {
            Bang_GanGiaTriThatChoO(h, c, value);
        }
    }
}
function BangDuLieu_onKeypress_Delete(h, c) {
    if (BangDuLieu_DuocSuaChiTiet && h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
        else if (Bang_nH > 0) {

        }
    }
}

//function BangDuLieu_ThemHangMoi(h, hGiaTri) {
//    if (h != null && BangDuLieu_DuocSuaChiTiet) {
//        if (Bang_arrLaHangCha[hGiaTri] == false) {
//            Bang_ThemHang(h, hGiaTri);


//            //Them vao bang BangDuLieu_arrChiSoNhom
//            BangDuLieu_arrChiSoNhom.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrChiSoNhom[hGiaTri]));

//            //Them vao bang BangDuLieu_arrMaMucLucNganSach
//            BangDuLieu_arrMaMucLucNganSach.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrMaMucLucNganSach[hGiaTri]));

//            //BangDuLieu_arrMaMucLucNganSach[h] = "";
//            //Gan cac gia tri tien bang 0
//            for (var c = Bang_nC_Fixed + 5; c < Bang_nC; c++) {
//                if (Bang_arrMaCot[c] == "sGhiChu" || Bang_arrMaCot[c] == "sSoCT" || Bang_arrMaCot[c] == "iID_MaDonVi" || Bang_arrMaCot[c] == "sTenDonVi") {
//                    Bang_GanGiaTriThatChoO(h, c, "");
//                }
//                else {
//                    Bang_GanGiaTriThatChoO(h, c, 0);
//                }

//            }
//            //Gan lai ma hang ="": Hang moi
//            //Bang_arrMaHang[h] = "_" + Bang_arrGiaTri[h-1][5];
//            var MaHang = Bang_arrMaHang[h - 1];
//            MaHang = MaHang.substring(MaHang.indexOf("_"), MaHang.length);
//            Bang_arrMaHang[h] = MaHang;
//            //Gan lai o đơn vị bằng rỗng
//            //Bang_GanGiaTriThatChoO(h, Bang_nC_Fixed, "");
//            Bang_keys.fnSetFocus(h, 0);
//        }
//    }
//}

//function BangDuLieu_onKeypress_F2(h, c) {
//    if (BangDuLieu_DuocSuaChiTiet) {
//        BangDuLieu_ThemHangMoi(h + 1, h);
//    }
//}

function Clone_values(coltail) {
    for (var i = 0; i < Bang_nH - 1; i++) {
        if (Bang_LayGiaTri(i, "TuChi") == 0 &&
            Bang_LayGiaTri(i, "TuChi" + coltail) != 0 &&
            Bang_LayGiaTri(i, "TuChi" + coltail) != null) {
            Bang_GanGiaTriThatChoO_colName(i, "TuChi", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));
        }

        if (Bang_LayGiaTri(i, "HangNhap") == 0 &&
            Bang_LayGiaTri(i, "HangNhap" + coltail) != 0 &&
            Bang_LayGiaTri(i, "HangNhap" + coltail) != null) {
            Bang_GanGiaTriThatChoO_colName(i, "HangNhap", parseFloat(Bang_LayGiaTri(i, "HangNhap" + coltail)));
        }
        if (Bang_LayGiaTri(i, "HangMua") == 0 &&
            Bang_LayGiaTri(i, "HangMua" + coltail) != 0 &&
            Bang_LayGiaTri(i, "HangMua" + coltail) != null) {
            Bang_GanGiaTriThatChoO_colName(i, "HangMua", parseFloat(Bang_LayGiaTri(i, "HangMua" + coltail)));
        }
        Mark_difference_values(coltail, i);
    }
}

function Mark_difference_values(coltail, h, c) {
    var color = "#00FFBF";

    if (coltail != "edit") {
        if (coltail == "_B2")
            color = "#4891dc";
        else if (coltail == "_Bql") {
            color = "#2abd54";
        }
        if (Bang_arrLaHangCha[h] == false) {
            if ((Bang_LayGiaTri(h, "TuChi") != 0 &&
                 Bang_LayGiaTri(h, "TuChi") != null) ||
                (Bang_LayGiaTri(h, "HangNhap") != 0 &&
                 Bang_LayGiaTri(h, "HangNhap") != null) ||
                (Bang_LayGiaTri(h, "HangMua") != 0 &&
                 Bang_LayGiaTri(h, "HangMua") != null)) {
                if ((Bang_LayGiaTri(h, "TuChi" + coltail) != null &&
                     Bang_LayGiaTri(h, "TuChi") != Bang_LayGiaTri(h, "TuChi" + coltail)) ||
                    (Bang_LayGiaTri(h, "HangNhap" + coltail) != null &&
                     Bang_LayGiaTri(h, "HangNhap") != Bang_LayGiaTri(h, "HangNhap" + coltail)) ||
                    (Bang_LayGiaTri(h, "HangMua" + coltail) != null &&
                     Bang_LayGiaTri(h, "HangMua") != Bang_LayGiaTri(h, "HangMua" + coltail))) {
                    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", color);
                } else {
                    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "");
                }
            }
        }
    } else {
        var colName = Bang_arrMaCot[c];
        var cotbql = Bang_arrCSMaCot[colName + "_Bql"];
        var cotb2 = Bang_arrCSMaCot[colName + "_B2"];

        if (cotb2 == undefined && cotbql != undefined) {
            if (Bang_arrGiaTri[h][c] != Bang_arrGiaTri[h][cotbql]) {
                Bang_GanGiaTriThatChoO_colName(h, "sMauSac", color);
            } else {
                Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "");
            }
        } else if (cotbql != undefined && cotb2 != undefined) {
            if (Bang_arrGiaTri[h][c] != Bang_arrGiaTri[h][cotbql] ||
                Bang_arrGiaTri[h][c] != Bang_arrGiaTri[h][cotb2]) {
                Bang_GanGiaTriThatChoO_colName(h, "sMauSac", color);
            } else {
                Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "");
            }
        }
    }
}

function BangDuLieu_onKeypress_F3() {

    var yes = confirm("F3: Đ/c chắc chắn muốn lấy toàn bộ số liệu đề nghị của BQL sang số đề nghị?");
    if (!yes)
        return;

    Clone_values("_Bql");
}

function BangDuLieu_onKeypress_F4() {

    var yes = confirm("F4: Đ/c chắc chắn muốn lấy toàn bộ số liệu đề nghị của B2 sang số đề nghị của Cục?");
    if (!yes)
        return;

    Clone_values("_B2");
}

//function BangDuLieu_onKeypress_F5(h, c) {
//    parent.jsGTST_Dialog_Show();
//}

//function BangDuLieu_onKeypress_F6(h, c) {
//    parent.jsGTBL_Dialog_Show();
//}
