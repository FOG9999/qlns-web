var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrType[c] != "1")
        return;    
    if (Bang_arrMaCot[c] == "TuChi" || Bang_arrMaCot[c] == "HuyDong" || Bang_arrMaCot[c] == "MuaHang" || Bang_arrMaCot[c] == "PhanCap") {
        BangDuLieu_TinhTong(h, c);
    }
    else if (Bang_arrMaCot[c] == "TuChi_DV" || Bang_arrMaCot[c] == "HuyDong_DV" || Bang_arrMaCot[c] == "MuaHang_DV" || Bang_arrMaCot[c] == "PhanCap_DV") {
        BangDuLieu_TinhTong_DV(h, c);
    }
    var tong = 0;
    for (var i = 1; i < Bang_nH - 1; i++) {
        if (Bang_arrLaHangCha[i] == false) {
            tong += Bang_arrGiaTri[i][c];
        }
    }
    Bang_GanGiaTriThatChoO(0, c, tong);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, c, tong);
    BangDuLieu_CapNhapLaiHangCha(h, c);
}

function BangDuLieu_TinhTong(h, c) {
    var csMaCotTuChi_3 = Bang_arrCSMaCot["TuChi_DTT_3"];
    var csMaCotMuaHang_3 = Bang_arrCSMaCot["MuaHang_DTT_3"];
    var csMaCotPhanCap_3 = Bang_arrCSMaCot["PhanCap_DTT_3"];
    var csMaCotHuyDong = Bang_arrCSMaCot["HuyDong"];
    var csMaCotTuChi = Bang_arrCSMaCot["TuChi"];
    var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang"];
    var csMaCotPhanCap = Bang_arrCSMaCot["PhanCap"];
    var csMaCotTongCong = Bang_arrCSMaCot["TongCong"];
    var csMaCotTang = Bang_arrCSMaCot["Tang"];
    var csMaCotGiam = Bang_arrCSMaCot["Giam"];

    var GiaTriTuChi_3 = Bang_arrGiaTri[h][csMaCotTuChi_3];
    var GiaTriMuaHang_3 = Bang_arrGiaTri[h][csMaCotMuaHang_3];
    var GiaTriPhanCap_3 = Bang_arrGiaTri[h][csMaCotPhanCap_3];
    var GiaTriHuyDong = Bang_arrGiaTri[h][csMaCotHuyDong];
    var GiaTriTuChi = Bang_arrGiaTri[h][csMaCotTuChi];
    var GiaTriMuaHang = Bang_arrGiaTri[h][csMaCotMuaHang];
    var GiaTriPhanCap = Bang_arrGiaTri[h][csMaCotPhanCap];

    if (Bang_arrType[csMaCotTuChi] == "1") {
        var Tong_1 = GiaTriHuyDong + GiaTriTuChi;
        alert(GiaTriTuChi_3);
        Bang_GanGiaTriThatChoO(h, csMaCotTongCong, Tong_1);
        if (Tong_1 - GiaTriTuChi_3 > 0) {            
            Bang_GanGiaTriThatChoO(h, csMaCotTang, Tong_1 - GiaTriTuChi_3);
            Bang_GanGiaTriThatChoO(h, csMaCotGiam, 0);
        }
        else{
            Bang_GanGiaTriThatChoO(h, csMaCotTang, 0);
            Bang_GanGiaTriThatChoO(h, csMaCotGiam, GiaTriTuChi_3 - Tong_1);
        }
    }
    else if (Bang_arrType[csMaCotMuaHang] == "1") {
        var Tong_2 = GiaTriHuyDong + GiaTriMuaHang + GiaTriPhanCap;
        var Tong_3 = GiaTriMuaHang_3 + GiaTriPhanCap_3;
        Bang_GanGiaTriThatChoO(h, csMaCotTongCong, Tong_2);
        if (Tong_2 - Tong_3 > 0) {
            Bang_GanGiaTriThatChoO(h, csMaCotTang, Tong_2 - Tong_3);
            Bang_GanGiaTriThatChoO(h, csMaCotGiam, 0);
        }
        else {
            Bang_GanGiaTriThatChoO(h, csMaCotTang, 0);
            Bang_GanGiaTriThatChoO(h, csMaCotGiam, Tong_3 - Tong_2);
        }
    }

    var tong = 0, tongT = 0, tongG = 0;
    for (var i = 1; i < Bang_nH - 1; i++) {
        if (Bang_arrLaHangCha[i] == false) {
            tong += Bang_arrGiaTri[i][csMaCotTongCong];
            tongT += Bang_arrGiaTri[i][csMaCotTang];
            tongG += Bang_arrGiaTri[i][csMaCotGiam];
        }
    }
    Bang_GanGiaTriThatChoO(0, csMaCotTongCong, tong);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, csMaCotTongCong, tong);
    BangDuLieu_CapNhapLaiHangCha(h, csMaCotTongCong);
    Bang_GanGiaTriThatChoO(0, csMaCotTang, tongT);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, csMaCotTang, tongT);
    BangDuLieu_CapNhapLaiHangCha(h, csMaCotTang);
    Bang_GanGiaTriThatChoO(0, csMaCotGiam, tongG);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, csMaCotGiam, tongG);
    BangDuLieu_CapNhapLaiHangCha(h, csMaCotGiam);
}

function BangDuLieu_TinhTong_DV(h, c) {
    var csMaCotHuyDong = Bang_arrCSMaCot["HuyDong_DV"];
    var csMaCotTuChi = Bang_arrCSMaCot["TuChi_DV"];
    var csMaCotMuaHang = Bang_arrCSMaCot["MuaHang_DV"];
    var csMaCotPhanCap = Bang_arrCSMaCot["PhanCap_DV"];
    var csMaCotTongCong = Bang_arrCSMaCot["TongCong_DV"];

    var GiaTriHuyDong = Bang_arrGiaTri[h][csMaCotHuyDong];
    var GiaTriTuChi = Bang_arrGiaTri[h][csMaCotTuChi];
    var GiaTriMuaHang = Bang_arrGiaTri[h][csMaCotMuaHang];
    var GiaTriPhanCap = Bang_arrGiaTri[h][csMaCotPhanCap];

    if (Bang_arrType[csMaCotTuChi] == "1") {
        var Tong_1 = GiaTriHuyDong + GiaTriTuChi;
        Bang_GanGiaTriThatChoO(h, csMaCotTongCong, Tong_1);        
    }
    else if (Bang_arrType[csMaCotMuaHang] == "1") {
        var Tong_2 = GiaTriHuyDong + GiaTriMuaHang + GiaTriPhanCap;
        Bang_GanGiaTriThatChoO(h, csMaCotTongCong, Tong_2);        
    }

    var tong = 0;
    for (var i = 1; i < Bang_nH - 1; i++) {
        if (Bang_arrLaHangCha[i] == false) {
            tong += Bang_arrGiaTri[i][csMaCotTongCong];
        }
    }
    Bang_GanGiaTriThatChoO(0, csMaCotTongCong, tong);
    Bang_GanGiaTriThatChoO(Bang_nH - 1, csMaCotTongCong, tong);
    BangDuLieu_CapNhapLaiHangCha(h, csMaCotTongCong);    
}
function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (Bang_arrType[c] == 1) {
        var csCha = h, GiaTri;
        while (Bang_arrCSCha[csCha] > 0) {
            csCha = Bang_arrCSCha[csCha];
            GiaTri = BangDuLieu_TinhTongHangCon(csCha, c);
            Bang_GanGiaTriThatChoO(csCha, c, GiaTri);
        }
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