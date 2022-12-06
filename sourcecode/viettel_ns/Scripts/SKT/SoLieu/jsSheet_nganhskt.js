var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var jsQuyetToan_Url_Frame = "";

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrMaCot[c].startsWith("sTenDonVi")) {
        var iID_MaDonVi = Bang_arrGiaTri[h][c];
        var url = unescape("/DuToan_ChungTuChiTiet/GetPhongBanCuaDonVi" + '?iID_MaDonVi=' + iID_MaDonVi);
        $.getJSON(url, function (data) {
            Bang_GanGiaTriThatChoO(h, c + 1, data.value);
        });
        Bang_keys.fnSetFocus(h, 2);
    }

    if (Bang_arrType[c] == "1") {
    
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

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    if (h != null && BangDuLieu_DuocSuaChiTiet) {
        if (Bang_arrLaHangCha[hGiaTri] == false) {

            Bang_ThemHang(h, hGiaTri);
            //Gan cac gia tri tien bang 0
            for (var c = Bang_nC_Fixed; c < Bang_nC; c++) {
                if (Bang_arrMaCot[c] == "GhiChu" || Bang_arrMaCot[c].startsWith("iID_MaDonVi") || Bang_arrMaCot[c] == "iID_MaPhongBanDich" || Bang_arrMaCot[c].startsWith("sTenDonVi")) {
                    Bang_GanGiaTriThatChoO(h, c, "");
                }
                else {
                    Bang_GanGiaTriThatChoO(h, c, 0);
                }

            }
            //Gan lai ma hang ="": Hang moi
            var cs = Bang_arrCSMaCot["Id"];
            var csa = Bang_arrCSMaCot["sTenDonVi"];
            Bang_arrMaHang[h] = Bang_arrGiaTri[h - 1][cs] + "_";
            Bang_GanGiaTriThatChoO(h, cs, Bang_arrGiaTri[h - 1][cs]);
            Bang_keys.fnSetFocus(h, 0);
        }        
    }
}

function BangDuLieu_onKeypress_F2(h, c) {
    BangDuLieu_ThemHangMoi(h + 1, h);
}