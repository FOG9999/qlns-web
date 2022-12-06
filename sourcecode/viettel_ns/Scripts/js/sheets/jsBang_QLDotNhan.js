var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_MaDonViChoCapPhat = "99";
var BangDuLieu_arrMaMucLucNganSach;
var BangDuLieu_arrChiSoNhom;
/* BangDuLieu_Url_getGiaTri: url cua ham lay gia tri sau khi nhap xong o Autocomplete*/
var BangDuLieu_Url_getGiaTri = "";
/* BangDuLieu_Url_getGiaTri: url cua ham lay gia tri ngay khi bam 1 phim tren o Autocomplete*/
var BangDuLieu_Url_getDanhSach = "";
var BangDuLieu_DuocSuaChiTiet = false;
var BangDuLieu_iID_MaChungTu = "";

var jsCapPhat_Url_Frame = '';
var jsCapPhat_Url = '';
function jsCapPhat_LoadLaiChiTiet() {
    var url = jsCapPhat_Url_Frame;
    var controls = $('input[search-control="1"]');
    var i;
    for (i = 0; i < controls.length; i++) {
        var field = $(controls[i]).attr("search-field");
        var value = $(controls[i]).val();
        url += "&" + field + "=" + encodeURI(value);
    }

    document.getElementById("ifrChiTietChungTu").src = url;

}
var jsPhanBo_Search_inteval = null;
function jsPhanBo_Search_clearInterval() {
    clearInterval(jsPhanBo_Search_inteval);
}
function jsCapPhat_Search_onkeypress(e) {

    jsPhanBo_Search_clearInterval();
    if (e.keyCode == 13) {
        jsCapPhat_LoadLaiChiTiet();
    }
    else {
        jsPhanBo_Search_inteval = setInterval(function () { jsPhanBo_Search_clearInterval(); jsCapPhat_LoadLaiChiTiet(); }, 2000);
    }
}
function BangDuLieu_onLoad() {
    BangDuLieu_arrDonVi = document.getElementById('idDSDonVi').value.split("##");
    BangDuLieu_arrChiSoNhom = Bang_LayMang1ChieuGiaTri('idDSChiSoNhom');
    BangDuLieu_arrMaMucLucNganSach = Bang_LayMang1ChieuGiaTri('idMaMucLucNganSach');
    for (i = 0; i < BangDuLieu_arrChiSoNhom.length; i++) {
        BangDuLieu_arrChiSoNhom[i] = parseInt(BangDuLieu_arrChiSoNhom[i]);
    }
    if (typeof Bang_arrCSMaCot["bDongY"] == "undefined") {
        BangDuLieu_CoCotDuyet = false;
    }
    else {
        BangDuLieu_CoCotDuyet = true;
    }
}

function BangDuLieu_LayTenDonViTheoMa(MaDonVi) {
    var strMaDonVi = String(MaDonVi);
    var i;
    for (i = 0; i < BangDuLieu_arrDonVi.length; i = i + 2) {
        if (BangDuLieu_arrDonVi[i] == strMaDonVi) {
            return BangDuLieu_arrDonVi[i + 1];
        }
    }
    return "";
}

function BangDuLieu_onCellAfterEdit(h, c) {
    var cGhiChu = Bang_arrCSMaCot["GhiChu"];
    if (c == cGhiChu) {
        return true;
    }

    var GiaTri = Bang_arrGiaTri[h][c];
    var fLuyKeNguonNhan = Bang_arrGiaTri[h][c - 1];
    var fTongNguonNhan = GiaTri + fLuyKeNguonNhan;
    Bang_GanGiaTriThatChoO(h, c + 1, fTongNguonNhan, 1);

    var fTongDaPhan = Bang_arrGiaTri[h][c + 4];
    var SoTienConLai = fTongNguonNhan - fTongDaPhan;
    Bang_GanGiaTriThatChoO(h, c + 5, SoTienConLai, 1);
    BangDuLieu_CapNhapLaiHangCha(h, c);

    return true;
}

function BangDuLieu_CapNhapLaiHangCha(h, c) {
    if (BangDuLieu_CoCotDuyet && c >= Bang_arrMaCot.length - 2) {
        return;
    }
    var j;

    var csCha = h, GiaTri;

    while (Bang_arrCSCha[csCha] >= 0) {
        csCha = Bang_arrCSCha[csCha];


        var fLuyKeNguonNhan = BangDuLieu_TinhTongHangCon(csCha, 4);
        Bang_GanGiaTriThatChoO(csCha, 4, fLuyKeNguonNhan);

        GiaTri = BangDuLieu_TinhTongHangCon(csCha, 5);
        Bang_GanGiaTriThatChoO(csCha, 5, GiaTri);

        var fTongNguonNhan = GiaTri + fLuyKeNguonNhan;
        Bang_GanGiaTriThatChoO(csCha, 6, fTongNguonNhan);

        var fTongDaPhan = Bang_arrGiaTri[h][c + 4];
        var SoTienConLai = fTongNguonNhan - fTongDaPhan;
        Bang_GanGiaTriThatChoO(csCha, 10, SoTienConLai);
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
            for (var c = Bang_nC_Fixed + 2; c < Bang_nC; c++) {
                if (Bang_arrLaHangCha[h] == false) {
                    Bang_GanGiaTriThatChoO(h, c, 0);
                }
            }
            //Gan lai ma hang ="": Hang moi
            Bang_arrMaHang[h] = "_" + BangDuLieu_arrMaMucLucNganSach[hGiaTri];
            //Gan lai o đơn vị bằng rỗng
            Bang_GanGiaTriThatChoO(h, Bang_nC_Fixed, "");
            Bang_GanGiaTriThatChoO(h, Bang_nC_Fixed + 1, "");
            Bang_keys.fnSetFocus(h, 0);
        }
    }
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    console.log(h);
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
        }
    }
}

function BangDuLieu_onKeypress_F10() {
    Bang_HamTruocKhiKetThuc();
}

function BangDuLieu_onKeypress_F9() {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".sheet-search button[type=submit]").click()
}

function BangDuLieu_onKeypress_F2(h, c) {
    //BangDuLieu_ThemHangMoi(h + 1, h);
}

function BangDuLieu_onKeypress_F4(h, c) {
    if (h != null) {
        if (Bang_arrLaHangCha[h] == false) {
            var iID_DotNhanChiTiet = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_DotNhanChiTiet"]];
            var iID_DotNhan = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_DotNhan"]];
            var iID_Nguon = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_Nguon"]];
            var sNoiDung = Bang_arrGiaTri[h][Bang_arrCSMaCot["sNoiDung"]];
            var sMaNguon = Bang_arrGiaTri[h][Bang_arrCSMaCot["sMaNguon"]];
            var sMaLoaiDuToan = Bang_arrGiaTri[h][Bang_arrCSMaCot["sMaLoaiDuToan"]];

            //So tien nhan tu BTC, da phan, con lai
            var fTongNguonNhan = Bang_arrGiaTri[h][Bang_arrCSMaCot["fTongNguonNhan"]];
            var fLuyKeDaPhanNDC = Bang_arrGiaTri[h][Bang_arrCSMaCot["fLuyKeDaPhanNDC"]];
            var SoTienDaPhanNDC = Bang_arrGiaTri[h][Bang_arrCSMaCot["SoTienDaPhanNDC"]];
            var fSoTienNhanTuBTC = fTongNguonNhan - fLuyKeDaPhanNDC;
            var fSoTienConLai = fSoTienNhanTuBTC - SoTienDaPhanNDC;

            var isClone = false;
            if (sMaLoaiDuToan == '001' || sMaLoaiDuToan == '002') {
                isClone = true;
            }
            window.parent.BtnViewPNDotNhanCTTheoNDChiClick(iID_DotNhanChiTiet, iID_Nguon, sNoiDung, sMaLoaiDuToan, isClone, fSoTienNhanTuBTC, SoTienDaPhanNDC, fSoTienConLai);
        }
    }
}

//function BangDuLieu_onKeypress_Delete(h, c) {
//    if (h != null) {
//        if (BangDuLieu_DuocSuaChiTiet && Bang_arrLaHangCha[h] == false) {
//            Bang_XoaHang(h);
//            //Xoa ca vao bang BangDuLieu_arrChiSoNhom
//            BangDuLieu_arrChiSoNhom.splice(h, 1);
//            //Xoa ca vao bang BangDuLieu_arrMaMucLucNganSach
//            BangDuLieu_arrMaMucLucNganSach.splice(h, 1);
//            if (h < Bang_nH) {
//                Bang_keys.fnSetFocus(h, c);
//            }
//            else if (Bang_nH > 0) {

//            }
//        }
//    }
//}

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

function BangDuLieu_HamTruocKhiKetThuc(iAction) {
    var html = PopupModal("Thông báo", "Lưu dữ liệu thành công");
    window.parent.loadModal(html);
    return true;
}

function PopupModal(title, message) {
    var result = "";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: 1 },
        async: false,
        success: function (data) {
            result = data;
        }
    });

    return result;
}