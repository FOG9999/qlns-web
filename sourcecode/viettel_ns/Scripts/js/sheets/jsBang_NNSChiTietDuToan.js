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

var ERROR = 1;

document.addEventListener('keydown', eventKey);
function eventKey(e) {
    if (e != null && e != undefined) {
        if (e.key == "F9") {
            BangDuLieu_onKeypress_F9();
        }
    }
}

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

var arrData = [];
function getArrData() {
    for (var i = 0; i < Bang_arrGiaTri.length; i++) {
        var iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
        var iID_NoiDungChiCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChiCha"]];
        var depth = Bang_arrGiaTri[i][Bang_arrCSMaCot["depth"]];
        var rootParent = Bang_arrGiaTri[i][Bang_arrCSMaCot["rootParent"]];
        arrData.push({
            "iID_NoiDungChi": iID_NoiDungChi,
            "iID_NoiDungChiCha": iID_NoiDungChiCha,
            "depth": depth,
            "rootParent": rootParent
        })
    }
}

function BangDuLieu_onLoad() {
    //BangDuLieu_arrDonVi = document.getElementById('idDSDonVi').value.split("##");
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

    // update so tien row conlai
    for (h = 0; h < Bang_nH; h++) {
        UpdateSoTienRowConLai(h, Bang_arrCSMaCot["SoTien"]);
    }
    getArrData();
    TinhTongTienRowCha();
    UpdateTongSoTienGiaoDuToan();
}

function TinhTongTienRowCha() {
    for (var i = 0; i < Bang_arrGiaTri.length; i++) {
        var iID_NoiDungChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChi"]];
        var iID_NoiDungChiCha = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_NoiDungChiCha"]];
        var depth = Bang_arrGiaTri[i][Bang_arrCSMaCot["depth"]];
        var tongTien = 0;

        var rowConLai = arrData.filter(x => x.iID_NoiDungChiCha == iID_NoiDungChi && x.iID_NoiDungChi == "");
        // k có row con lai -> tinh tong
        if (rowConLai.length <= 0) {
            var arrChild = getChild(iID_NoiDungChi);
            if (arrChild.length > 0) {
                for (var j = 0; j < Bang_arrGiaTri.length; j++) {
                    var iID_NoiDungChiJ = Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_NoiDungChi"]];
                    var iID_NoiDungChiChaJ = Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_NoiDungChiCha"]];
                    var depthJ = Bang_arrGiaTri[j][Bang_arrCSMaCot["depth"]];
                    var rootParentJ = Bang_arrGiaTri[j][Bang_arrCSMaCot["rootParent"]];
                    var bLaHangCha = Bang_arrGiaTri[j][Bang_arrCSMaCot["bLaHangCha"]];
                    var soTienJ = Bang_arrGiaTri[j][Bang_arrCSMaCot["SoTien"]];
                    if (bLaHangCha == "False" && arrChild.indexOf(iID_NoiDungChiChaJ) >= 0)
                        tongTien += soTienJ;
                }
                Bang_GanGiaTriThatChoO(i, Bang_arrCSMaCot["SoTien"], tongTien);
            }
        }
    }
}

function getChild(idParent) {
    var arrChil = [];
    if (idParent != "") {
        var listChil = arrData.filter(x => x.iID_NoiDungChiCha == idParent && x.iID_NoiDungChi != "").map(x => x.iID_NoiDungChi);
        if (listChil.length > 0) {
            arrChil = arrChil.concat(listChil);
            listChil.forEach(function (a) {
                if (a.iID_NoiDungChi != "")
                    arrChil = arrChil.concat(getChild(a));
            })
        }
    }

    return arrChil;
}

function BangDuLieu_KiemTraDonViKhoiDonVi(h) {
    var sTenDonVi = Bang_arrGiaTri[h][Bang_arrCSMaCot["TenDonVi"]];
    var sKhoiDonVi = Bang_arrGiaTri[h][Bang_arrCSMaCot["sTenPhongBan"]];
    var indexC = Bang_arrCSCha[h];
    for (var i = 0; i < Bang_nH; i++) {
        if (i != h && Bang_arrCSCha[i] == indexC && Bang_arrGiaTri[i][Bang_arrCSMaCot["TenDonVi"]] == sTenDonVi && Bang_arrGiaTri[i][Bang_arrCSMaCot["sTenPhongBan"]] == sKhoiDonVi)
            return false;
    }
    return true;
}

function BangDuLieu_onCellAfterEdit(h, c) {
    if (Bang_arrLaHangCha[h] == false) {
        if (Bang_arrMaCot[c] == "SoTien") {
            UpdateSoTienRowConLai(h, c);
            UpdateTongSoTienGiaoDuToan();
        } else if (Bang_arrMaCot[c] == "TenDonVi" || Bang_arrMaCot[c] == "sTenPhongBan") {
            if (BangDuLieu_KiemTraDonViKhoiDonVi(h) == false) {
                Bang_GanGiaTriThatChoO(h, c, "");
            }
        }
    }
    return;
}

// update SoTien row ConLai cua tung noi dung chi
function UpdateSoTienRowConLai(h, c) {
    var tongTienHangCon = 0;
    var indexConlai = 0;
    for (var i = h; i >= 0; i--) {
        var cp = Bang_arrGiaTri[i][c];
        if (Bang_arrLaHangCha[i] == true) {
            indexConlai = i;
            break;
        }
        tongTienHangCon += cp;
    }

    for (var i = h + 1; i < Bang_nH; i++) {
        var cp = Bang_arrGiaTri[i][c];
        if (Bang_arrLaHangCha[i] == true)
            break;
        tongTienHangCon += cp;
    }
    TinhTongTienRowCha();
    if (indexConlai > 0 && tongTienHangCon != 0) {
        var tong = Bang_arrGiaTri[indexConlai - 1][c];
        var conlai = tong - tongTienHangCon;
        Bang_GanGiaTriThatChoO(indexConlai, c, conlai);

        if (conlai < 0) {
            Bang_GanGiaTriThatChoO_colName(indexConlai, "sMauSac", Bang_sMauSac_TuChoi);
        }
        else {
            // set color row con lai
            Bang_GanGiaTriThatChoO_colName(indexConlai, "sMauSac", "#FCF86A");
        }

        return;
    }
    return true;
}

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    if (h != null && BangDuLieu_DuocSuaChiTiet) {
        //if (Bang_arrLaHangCha[hGiaTri] == false) {
        if (Bang_ThemHangMoi(h, hGiaTri)) {
            //Them vao bang BangDuLieu_arrChiSoNhom
            BangDuLieu_arrChiSoNhom.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrChiSoNhom[hGiaTri]));
            //Them vao bang BangDuLieu_arrMaMucLucNganSach
            BangDuLieu_arrMaMucLucNganSach.splice(h, 0, Bang_TaoDoiTuongMoi(BangDuLieu_arrMaMucLucNganSach[hGiaTri]));
            //BangDuLieu_arrMaMucLucNganSach[h] = "";           

            //Gan lai ma hang ="": Hang moi
            Bang_arrMaHang[h] = "_" + BangDuLieu_arrMaMucLucNganSach[hGiaTri];
            // Gan lai iID_DuToanChiTiet
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_DuToanChiTiet"], 0);
            // Gan lai o don vi bang rong
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["TenDonVi"], "");
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_MaDonVi"], "");
            // Gan lai o ma khoi bang rong
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sMaPhongBan"], "");
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sTenPhongBan"], "");
            // Gan lai cot SoTien = 0
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["SoTien"], 0);
            // Gan lai cot GhiChu = ""
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["GhiChu"], "");
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["bLaHangCha"], "False");
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sTenNoiDungChi"], "");

            Bang_keys.fnSetFocus(h, 0);
        }
    }
}

function Bang_ThemHangMoi(cs, csGoc) {
    //var indexSplice = cs;
    if (Bang_arrLaHangCha[csGoc] == true && Bang_arrGiaTri[csGoc][Bang_arrCSMaCot["TenDonVi"]] == "")
        return false;
    //Them hang du lieu dua tren 1 hang du lieu khac
    Bang_nH = Bang_nH + 1;
    Bang_Viewport_ThayDoiHang();

    Bang_arrMaHang.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrMaHang[csGoc]));
    Bang_arrCSCha.splice(cs, 0, Bang_arrLaHangCha[csGoc] == true ? Bang_TaoDoiTuongMoi(Bang_arrCSCha[csGoc]) + 1 : Bang_TaoDoiTuongMoi(Bang_arrCSCha[csGoc]));
    Bang_arrLaHangCha.splice(cs, 0, false);
    Bang_arrGiaTri.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrGiaTri[csGoc]));
    Bang_arrHienThi.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrHienThi[csGoc]));
    Bang_arrThayDoi.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrThayDoi[csGoc]));

    var arrEdit = new Array();

    for (j = 0; j < Bang_nC; j++) {
        arrEdit.push(true);
    }

    Bang_arrEdit.splice(cs, 0, arrEdit);
    Bang_arrHangDaXoa.splice(cs, 0, false);

    //Sua lai Bang_arrThayDoi
    for (j = 0; j < Bang_nC; j++) {
        Bang_arrThayDoi[cs][j] = true;
    }
    //Sua lai Bang_arrCSCha: Cac hang cha co chi so lon hon cs se bi thay doi chi so
    for (i = 0; i < Bang_nH; i++) {
        if (Bang_arrCSCha[i] >= cs) {
            Bang_arrCSCha[i] = Bang_arrCSCha[i] + 1;
        }
    }
    Bang_HienThiDuLieu();
    return true;
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
    ChiTietDuToan_BangDuLieu_Save();
}

function BangDuLieu_onKeypress_F9() {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".sheet-search .btn-info").click();
}

function ChiTietDuToan_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    //if (!ValidateData()) {
    //    return false;
    //} else {
    Bang_HamTruocKhiKetThuc();
    //}
    //ValidateBeforeSave();
}

function ValidateData() {
    var sMessError = [];
    var Title = 'Lỗi lưu chi tiết dự toán';

    for (var j = 0; j < Bang_nH; j++) {
        if (Bang_arrLaHangCha[j] == true) {
            var sTenNoiDungChi = Bang_LayGiaTri(j, "sTenNoiDungChi");
            var sTenDonVi = Bang_LayGiaTri(j, "TenDonVi");
            var fSoTien = Bang_LayGiaTri(j, "SoTien");
            if (sTenDonVi.indexOf("Còn lại") > 0 && fSoTien != "" && fSoTien < 0) {
                sMessError.push('Số tiền giao dự toán theo nội dung chi [' + sTenNoiDungChi + '] vượt quá số tiền có.');
            }
        } else if (Bang_arrHangDaXoa[j] == false && Bang_arrLaHangCha[j] == false) {
            var iID_MaDonVi = Bang_LayGiaTri(j, "iID_MaDonVi");
            var sMaPhongBan = Bang_LayGiaTri(j, "sMaPhongBan");
            var fSoTien = Bang_LayGiaTri(j, "SoTien");

            if (iID_MaDonVi == "") {
                sMessError.push('Hãy nhập đơn vị cho dòng số ' + (j + 1) + '.');
            }
            if (sMaPhongBan == "") {
                sMessError.push('Hãy nhập phòng ban cho dòng số ' + (j + 1) + '.');
            }
            if (fSoTien == "") {
                sMessError.push('Hãy nhập số tiền cho dòng số ' + (j + 1) + '.');
            }
        }
    }

    if (sMessError != null && sMessError != undefined && sMessError.length > 0) {
        var html = PopupModal(Title, sMessError, ERROR);
        window.parent.loadModal(html);
        return false;
    }
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

function UpdateTongSoTienGiaoDuToan() {
    var tongTien = 0;
    for (var j = 0; j < Bang_nH; j++) {
        if (Bang_arrHangDaXoa[j] == false && Bang_arrLaHangCha[j] == false) {
            var fSoTien = Bang_LayGiaTri(j, "SoTien");
            tongTien += fSoTien;
        }
    }
    window.parent.document.getElementById("fTongGiaoDuToan").innerHTML = tongTien.toLocaleString('vi-VN');
}