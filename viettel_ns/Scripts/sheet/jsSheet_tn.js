var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsThuNop_Url_Frame = "";
var BangDuLieu_iID_MaPhongBan;

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
    if (BangDuLieu_iID_MaPhongBan == "06") {
        var cThueGTGT = Bang_arrCSMaCot["rNopThueGTGT"];
        var cThueTNDN = Bang_arrCSMaCot["rTongNopThueTNDN"];
        var cPhiLePhi = Bang_arrCSMaCot["rPhiLePhi"];
        var cNopNSNNKhac = Bang_arrCSMaCot["rTongNopNSNNKhac"];
        var cNopNSQP = Bang_arrCSMaCot["rNopNSQP"];

        var rThueGTGT = Bang_arrGiaTri[h][cThueGTGT];
        var rThueTNDN = Bang_arrGiaTri[h][cThueTNDN];
        var rPhiLePhi = Bang_arrGiaTri[h][cPhiLePhi];
        var rNopNSNNKhac = Bang_arrGiaTri[h][cNopNSNNKhac];

        var rNopNSNN = rThueGTGT + rThueTNDN + rNopNSNNKhac + rPhiLePhi;

        Bang_GanGiaTriThatChoO_colName(h, "rTongNopNSNN", rNopNSNN);

        BangDuLieu_CapNhapLaiHangCha(h, c);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rTongNopNSNN"]);
        BangDuLieu_CapNhapLaiHangTong(c);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rTongNopNSNN"]);
        return true;
    }
    else {
        var cTongThu = Bang_arrCSMaCot["rTongThu"];
        var cKhauHao = Bang_arrCSMaCot["rKhauHaoTSCD"];
        var cTienLuong = Bang_arrCSMaCot["rTienLuong"];
        var cQTNSKhac = Bang_arrCSMaCot["rQTNSKhac"];
        var cChiPhiKhac = Bang_arrCSMaCot["rChiPhiKhac"];
        var cThueGTGT = Bang_arrCSMaCot["rNopThueGTGT"];
        var cThueTNDN = Bang_arrCSMaCot["rTongNopThueTNDN"];
        var cPhiLePhi = Bang_arrCSMaCot["rPhiLePhi"];
        var cNopNSNNKhac = Bang_arrCSMaCot["rTongNopNSNNKhac"];
        var cNopNSQP = Bang_arrCSMaCot["rNopNSQP"];
        var cBoSungKinhPhi = Bang_arrCSMaCot["rBoSungKinhPhi"];
        var cTrichQuyDonVi = Bang_arrCSMaCot["rTrichQuyDonVi"];

        var rTongThu = Bang_arrGiaTri[h][cTongThu];
        var rKhauHaoTSCD = Bang_arrGiaTri[h][cKhauHao];
        var rTienLuong = Bang_arrGiaTri[h][cTienLuong];
        var rQTNSKhac = Bang_arrGiaTri[h][cQTNSKhac];
        var rChiPhiKhac = Bang_arrGiaTri[h][cChiPhiKhac];
        var rThueGTGT = Bang_arrGiaTri[h][cThueGTGT];
        var rThueTNDN = Bang_arrGiaTri[h][cThueTNDN];
        var rPhiLePhi = Bang_arrGiaTri[h][cPhiLePhi];
        var rNopNSNNKhac = Bang_arrGiaTri[h][cNopNSNNKhac];
        var rNopNSQP = Bang_arrGiaTri[h][cNopNSQP];
        var rBoSungKinhPhi = Bang_arrGiaTri[h][cBoSungKinhPhi];
        var rTrichQuyDonVi = Bang_arrGiaTri[h][cTrichQuyDonVi];

        var rTongQTNS = rKhauHaoTSCD + rTienLuong + rQTNSKhac;
        var rTongChiPhi = rTongQTNS + rChiPhiKhac;
        var rNopNSNN = rThueGTGT + rThueTNDN + rNopNSNNKhac + rPhiLePhi;
        var rChenhLech = rTongThu - rTongChiPhi - rNopNSNN;
        var rChuaPhanPhoi = rChenhLech - rNopNSQP - rBoSungKinhPhi - rTrichQuyDonVi;

        Bang_GanGiaTriThatChoO_colName(h, "rTongQTNS", rTongQTNS);
        Bang_GanGiaTriThatChoO_colName(h, "rTongChiPhi", rTongChiPhi);
        Bang_GanGiaTriThatChoO_colName(h, "rTongNopNSNN", rNopNSNN);
        Bang_GanGiaTriThatChoO_colName(h, "rChenhLech", rChenhLech);
        Bang_GanGiaTriThatChoO_colName(h, "rSoChuaPhanPhoi", rChuaPhanPhoi);

        BangDuLieu_CapNhapLaiHangCha(h, c);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rTongQTNS"]);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rTongChiPhi"]);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rTongNopNSNN"]);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rChenhLech"]);
        BangDuLieu_CapNhapLaiHangCha(h, Bang_arrCSMaCot["rSoChuaPhanPhoi"]);
        BangDuLieu_CapNhapLaiHangTong(c);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rTongQTNS"]);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rTongChiPhi"]);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rTongNopNSNN"]);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rChenhLech"]);
        BangDuLieu_CapNhapLaiHangTong(Bang_arrCSMaCot["rSoChuaPhanPhoi"]);

        return true;
    }
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (Bang_arrType[c] == 1) {
        var csCha = h, GiaTri;

        while (Bang_arrCSCha[csCha] >= 0) {
            csCha = Bang_arrCSCha[csCha];
            GiaTri = BangDuLieu_TinhTongHangCon(csCha, c);
            Bang_GanGiaTriThatChoO(csCha, c, GiaTri);

        }

    }
}

function BangDuLieu_CapNhapLaiHangTong(c) {
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

function BangDuLieu_ThemHangMoi(h, hGiaTri) {

    if (h != null && BangDuLieu_DuocSuaChiTiet) {
        if (Bang_arrLaHangCha[hGiaTri] == false) {
            Bang_ThemHang(h, hGiaTri);
            //Them vao bang BangDuLieu_arrChiSoNhom
            BangDuLieu_arrChiSoNhom.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrChiSoNhom[hGiaTri]));
            //Them vao bang BangDuLieu_arrMaMucLucNganSach
            BangDuLieu_arrMaMucLucNganSach.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrMaMucLucNganSach[hGiaTri]));
            //BangDuLieu_arrMaMucLucNganSach[h] = "";
            //Gan cac gia tri tien bang 0
            for (var c = Bang_nC_Fixed + 5; c < Bang_nC; c++) {
                if (Bang_arrMaCot[c] == "sGhiChu" || Bang_arrMaCot[c] == "sSoCT" || Bang_arrMaCot[c] == "iID_MaDonVi" || Bang_arrMaCot[c] == "sTenDonVi") {
                    Bang_GanGiaTriThatChoO(h, c, "");
                }
                else {
                    Bang_GanGiaTriThatChoO(h, c, 0);
                }

            }
            //Gan lai ma hang ="": Hang moi
            //Bang_arrMaHang[h] = "_" + Bang_arrGiaTri[h-1][5];
            var MaHang = Bang_arrMaHang[h - 1];
            MaHang = MaHang.substring(MaHang.indexOf("_"), MaHang.length);
            Bang_arrMaHang[h] = MaHang;
            //Gan lai o đơn vị bằng rỗng
            //Bang_GanGiaTriThatChoO(h, Bang_nC_Fixed, "");
            Bang_keys.fnSetFocus(h, 0);
        }
    }
}

function BangDuLieu_onKeypress_F2(h, c) {
    BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_Delete(h, c) {
    if (h != null) {
        if (BangDuLieu_DuocSuaChiTiet && Bang_arrLaHangCha[h] == false) {
            Bang_XoaHang(h);
            //Xoa ca vao bang BangDuLieu_arrChiSoNhom
            BangDuLieu_arrChiSoNhom.splice(h, 1);
            //Xoa ca vao bang BangDuLieu_arrMaMucLucNganSach
            BangDuLieu_arrMaMucLucNganSach.splice(h, 1);
            if (h < Bang_nH) {
                Bang_keys.fnSetFocus(h, c);
            }
            else if (Bang_nH > 0) {

            }
        }
    }
}

function BangDuLieu_onKeypress_F12(h, c) {
    document.location.href = BangDuLieu_Url_F12;
}