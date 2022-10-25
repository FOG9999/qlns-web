function Clone_values_dt(coltail) {
    for (var i = 0; i < Bang_nH; i++) {

        var csMaCotTuChi_1 = Bang_arrCSMaCot["TuChi_DTT_2"];
        var csMaCotMuaHang_1 = Bang_arrCSMaCot["MuaHang_DTT_3"];
        var csMaCotPhanCap_1 = Bang_arrCSMaCot["PhanCap_DTT_3"];
        var csMaCotTongCong = Bang_arrCSMaCot["TongCong"];
        var csMaCotTang = Bang_arrCSMaCot["Tang"];
        var csMaCotGiam = Bang_arrCSMaCot["Giam"];

        var GiaTriTuChi_1 = Bang_arrGiaTri[i][csMaCotTuChi_1];
        var GiaTriMuaHang_1 = Bang_arrGiaTri[i][csMaCotMuaHang_1];

        if (Bang_arrType[csMaCotTuChi_1] == "1") {
            if (Bang_LayGiaTri(i, "TuChi_DV") == 0 &&
                Bang_LayGiaTri(i, "TuChi" + coltail) != 0 &&
                Bang_LayGiaTri(i, "TuChi" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "TuChi_DV", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));
            }
            if (Bang_LayGiaTri(i, "TongCong") == 0 &&
                Bang_LayGiaTri(i, "TuChi" + coltail) != 0 &&
                Bang_LayGiaTri(i, "TuChi" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "TongCong", parseFloat(Bang_LayGiaTri(i, "TuChi" + coltail)));
            }
            Bang_GanGiaTriThatChoO(i, csMaCotTang, 0);
            Bang_GanGiaTriThatChoO(i, csMaCotGiam, 0);

        } else if (Bang_arrType[csMaCotMuaHang_1] == "1") {
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
            if (Bang_LayGiaTri(i, "TongCong") == 0 &&
                Bang_LayGiaTri(i, "MuaHang" + coltail) != 0 &&
                Bang_LayGiaTri(i, "MuaHang" + coltail) != null &&
                Bang_LayGiaTri(i, "HangNhap" + coltail) != 0 &&
                Bang_LayGiaTri(i, "HangNhap" + coltail) != null &&
                Bang_LayGiaTri(i, "PhanCap" + coltail) != 0 &&
                Bang_LayGiaTri(i, "PhanCap" + coltail) != null) {
                Bang_GanGiaTriThatChoO_colName(i, "TongCong", parseFloat(Bang_LayGiaTri(i, "HangNhap" + coltail) + parseFloat(Bang_LayGiaTri(i, "MuaHang" + coltail) + Bang_LayGiaTri(i, "PhanCap" + coltail)));
            }
        }
    }
}

function BangDuLieu_onKeypress_F8() {
    var csMaCotTuChi = Bang_arrCSMaCot["TuChi"];
    var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang"];
    var csMaCotHangNhap = Bang_arrCSMaCot["HangNhap"];

    if (Bang_arrType[csMaCotTuChi] == "1" || Bang_arrType[csMaCotMuaHang] == "1") {
        var yes = confirm("F8: Đ/c chắc chắn muốn lấy toàn bộ số liệu dự toán đầu năm?");
        if (!yes)
            return;

        Clone_values_dt("_DTT_2");
    }
}