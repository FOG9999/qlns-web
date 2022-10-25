function Clone_values(coltail) {
    for (var i = 0; i < Bang_nH; i++) {        
        var csMaCotTuChi_3 = Bang_arrCSMaCot["TuChi_DTT_3"];
        var csMaCotMuaHang_3 = Bang_arrCSMaCot["MuaHang_DTT_3"];
        var csMaCotPhanCap_3 = Bang_arrCSMaCot["PhanCap_DTT_3"];
        var csMaCotTuChi_Bql = Bang_arrCSMaCot["TuChi_Bql"];
        var csMaCotMuaHang_Bql = Bang_arrCSMaCot["MuaHang_Bql"];
        var csMaCotHangNhap_Bql = Bang_arrCSMaCot["HangNhap_Bql"];
        var csMaCotPhanCap_Bql = Bang_arrCSMaCot["PhanCap_Bql"];
        var csMaCotTuChi = Bang_arrCSMaCot["TuChi"];
        var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang"];
        var csMaCotHangNhap = Bang_arrCSMaCot["HangNhap"];
        var csMaCotPhanCap = Bang_arrCSMaCot["PhanCap"];
        var csMaCotTongCong = Bang_arrCSMaCot["TongCong"];
        var csMaCotTang = Bang_arrCSMaCot["Tang"];
        var csMaCotGiam = Bang_arrCSMaCot["Giam"];

        var GiaTriTuChi_3 = Bang_arrGiaTri[i][csMaCotTuChi_3];
        var GiaTriMuaHang_3 = Bang_arrGiaTri[i][csMaCotMuaHang_3];
        var GiaTriPhanCap_3 = Bang_arrGiaTri[i][csMaCotPhanCap_3];

        if (Bang_arrType[csMaCotTuChi_Bql] == "1" && Bang_LayGiaTri(i, "TuChi" + coltail) != 0 && Bang_LayGiaTri(i, "TuChi" + coltail) != null && Bang_LayGiaTri(i, "TuChi") == 0) {
            Bang_GanGiaTriThatChoO_colName(i, "TuChi", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));
            var GiaTriTuChi = Bang_arrGiaTri[i][csMaCotTuChi];
            if (GiaTriTuChi - GiaTriTuChi_3 > 0) {
                Bang_GanGiaTriThatChoO(i, csMaCotTang, GiaTriTuChi - GiaTriTuChi_3);
                Bang_GanGiaTriThatChoO(i, csMaCotGiam, 0);
            }
            else {
                Bang_GanGiaTriThatChoO(i, csMaCotTang, 0);
                Bang_GanGiaTriThatChoO(i, csMaCotGiam, GiaTriTuChi_3 - GiaTriTuChi);
            }                    
        }
        else if (Bang_arrType[csMaCotMuaHang_Bql] == "1" && Bang_arrType[csMaCotPhanCap_Bql] == "1") {
            if (Bang_LayGiaTri(i, "TongCong") == 0 &&
                Bang_LayGiaTri(i, "TongCong" + coltail) != 0 &&
                Bang_LayGiaTri(i, "TongCong" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "TongCong", parseFloat(Bang_LayGiaTri(i, "TongCong" + coltail)));
            }
            if (Bang_LayGiaTri(i, "MuaHang") == 0 &&
                Bang_LayGiaTri(i, "MuaHang" + coltail) != 0 &&
                Bang_LayGiaTri(i, "MuaHang" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "MuaHang", parseFloat(Bang_LayGiaTri(i, "MuaHang" + coltail)));
            }
            if (Bang_LayGiaTri(i, "HangNhap") == 0 &&
                Bang_LayGiaTri(i, "HangNhap" + coltail) != 0 &&
                Bang_LayGiaTri(i, "HangNhap" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "HangNhap", parseFloat(Bang_LayGiaTri(i, "HangNhap" + coltail)));
            }
            if (Bang_LayGiaTri(i, "PhanCap") == 0 &&
                Bang_LayGiaTri(i, "PhanCap" + coltail) != 0 &&
                Bang_LayGiaTri(i, "PhanCap" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "PhanCap", parseFloat(Bang_LayGiaTri(i, "PhanCap" + coltail)));
            }

            var GiaTriTong = Bang_arrGiaTri[i][csMaCotTongCong];

            if (GiaTriTong - GiaTriMuaHang_3 - GiaTriPhanCap_3 > 0) {
                Bang_GanGiaTriThatChoO(i, csMaCotTang, GiaTriTong - GiaTriMuaHang_3 - GiaTriPhanCap_3);
                Bang_GanGiaTriThatChoO(i, csMaCotGiam, 0);
            }
            else {
                Bang_GanGiaTriThatChoO(i, csMaCotTang, 0);
                Bang_GanGiaTriThatChoO(i, csMaCotGiam, parseFloat(GiaTriMuaHang_3 + GiaTriPhanCap_3 - GiaTriTong));
            }
        }
    }
}

//function Clone_values_dt(coltail) {
//    for (var i = 0; i < Bang_nH; i++) {

//        var csMaCotTuChi_1 = Bang_arrCSMaCot["TuChi_DTT_3"];
//        var csMaCotMuaHang_1 = Bang_arrCSMaCot["MuaHang_DTT_3"];
//        var csMaCotPhanCap_1 = Bang_arrCSMaCot["PhanCap_DTT_3"];
//        var csMaCotTongCong = Bang_arrCSMaCot["TongCong"];
//        var csMaCotTang = Bang_arrCSMaCot["Tang"];
//        var csMaCotGiam = Bang_arrCSMaCot["Giam"];

//        var GiaTriTuChi_1 = Bang_arrGiaTri[i][csMaCotTuChi_1];
//        var GiaTriMuaHang_1 = Bang_arrGiaTri[i][csMaCotMuaHang_1];

//        if (Bang_arrType[csMaCotTuChi_1] == "1") {
//            if (Bang_LayGiaTri(i, "TuChi") == 0 &&
//                Bang_LayGiaTri(i, "TuChi" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "TuChi" + coltail) != null) {
//                Bang_GanGiaTriThatChoO_colName(i, "TuChi", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));
//            }
//            if (Bang_LayGiaTri(i, "TongCong") == 0 &&
//                Bang_LayGiaTri(i, "TuChi" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "TuChi" + coltail) != null) {
//                Bang_GanGiaTriThatChoO_colName(i, "TongCong", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));                
//            }
//            Bang_GanGiaTriThatChoO(i, csMaCotTang, 0);
//            Bang_GanGiaTriThatChoO(i, csMaCotGiam, 0);

//        } else if (Bang_arrType[csMaCotMuaHang_1] == "1") {
//            if (Bang_LayGiaTri(i, "MuaHang") == 0 &&
//                Bang_LayGiaTri(i, "MuaHang" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "MuaHang" + coltail) != null) {
//                Bang_GanGiaTriThatChoO_colName(i, "MuaHang", parseFloat(Bang_LayGiaTri(i, "MuaHang" + coltail)));
//            }
//            if (Bang_LayGiaTri(i, "PhanCap") == 0 &&
//                Bang_LayGiaTri(i, "PhanCap" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "PhanCap" + coltail) != null) {
//                Bang_GanGiaTriThatChoO_colName(i, "PhanCap", parseFloat(Bang_LayGiaTri(i, "PhanCap" + coltail)));
//            }
//            if (Bang_LayGiaTri(i, "TongCong") == 0 &&
//                Bang_LayGiaTri(i, "MuaHang" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "MuaHang" + coltail) != null &&
//                Bang_LayGiaTri(i, "PhanCap" + coltail) != 0 &&
//                Bang_LayGiaTri(i, "PhanCap" + coltail) != null) {
//                Bang_GanGiaTriThatChoO_colName(i, "TongCong", parseFloat(Bang_LayGiaTri(i, "MuaHang" + coltail) + Bang_LayGiaTri(i, "PhanCap" + coltail)));
//            }
//        }
//    }
//}

//function BangDuLieu_onKeypress_F2() {
//    var csMaCotTuChi_Bql = Bang_arrCSMaCot["TuChi_Bql"];
//    var csMaCotMuaHang_Bql = Bang_arrCSMaCot["MuaHang_Bql"];
//    var csMaCotTuChi_DV = Bang_arrCSMaCot["TuChi_DV"];
//    var csMaCotMuaHang_DV = Bang_arrCSMaCot["MuaHang_DV"];

//    if (Bang_arrType[csMaCotTuChi] != "1" && Bang_arrType[csMaCotMuaHang] != "1" && Bang_arrType[csMaCotPhanCap] != "1"){
//        var yes = confirm("F2: Đ/c chắc chắn muốn lấy toàn bộ số liệu dự toán đầu năm?");
//        if (!yes)
//            return;

//        Clone_values_dt("_DTT_3");
//    }
//}

function BangDuLieu_onKeypress_F2() {
    var csMaCotTuChi = Bang_arrCSMaCot["TuChi_Bql"];
    var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang_Bql"];
    var csMaCotHangNhap = Bang_arrCSMaCot["HangNhap_Bql"];
    var csMaCotPhanCap = Bang_arrCSMaCot["PhanCap_Bql"];

    if (Bang_arrType[csMaCotTuChi] == "1" || (Bang_arrType[csMaCotHangNhap] == "1" && Bang_arrType[csMaCotMuaHang] == "1" && Bang_arrType[csMaCotPhanCap] == "1")) {
        var yes = confirm("F2: Đ/c chắc chắn muốn lấy toàn bộ số liệu đề nghị của BQL?");
        if (!yes)
            return;

        Clone_values("_Bql");
    }
}

function BangDuLieu_onKeypress_F3() {
    var csMaCotTuChi = Bang_arrCSMaCot["TuChi_B2"];
    var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang_B2"];
    var csMaCotHangNhap = Bang_arrCSMaCot["HangNhap_B2"];
    var csMaCotPhanCap = Bang_arrCSMaCot["PhanCap_B2"];

    if (Bang_arrType[csMaCotTuChi] == "1" || (Bang_arrType[csMaCotHangNhap] == "1" && Bang_arrType[csMaCotMuaHang] == "1" && Bang_arrType[csMaCotPhanCap] == "1")) {
        var yes = confirm("F3: Đ/c chắc chắn muốn lấy toàn bộ số liệu đề nghị của B2?");
        if (!yes)
            return;

        Clone_values("_B2");
    }
}