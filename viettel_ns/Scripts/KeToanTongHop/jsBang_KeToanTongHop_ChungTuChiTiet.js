/* BangDuLieu_CoCotTongSo: xac dinh bang co cot duyet hay khong*/
var BangDuLieu_CoCotDuyet = false;
/* BangDuLieu_Url_getGiaTri: url cua ham lay gia tri sau khi nhap xong o Autocomplete*/
var BangDuLieu_Url_getGiaTri = "";
/* BangDuLieu_Url_getGiaTri: url cua ham lay gia tri ngay khi bam 1 phim tren o Autocomplete*/
var BangDuLieu_Url_getDanhSach = "";
var BangDuLieu_hOld = 0;

/* Su kien BangDuLieu_onLoad
*/
function BangDuLieu_onLoad() {
    if (typeof Bang_arrCSMaCot["bDongY"] == "undefined") {
        BangDuLieu_CoCotDuyet = false;
    }
    else {
        BangDuLieu_CoCotDuyet = true;
    }
    if (Bang_ChiDoc == false && Bang_nH == 0) {
        BangDuLieu_ThemHangMoi(-1);
        Bang_DaThayDoi = false;
    }
    parent.GoiHam_ChungTu_BangDuLieu_fnSetFocus(Bang_nH, 0);
    TinhTongChiTiet();

}

function TinhTongChiTiet() {
    var i = 0;
    var c = Bang_arrCSMaCot["rSoTien"];
    var TongSo = 0;
    var GiaTri = 0;
    var iSoLuongChungTuChiTiet = 0;
    for (i = 0; i < Bang_nH; i++) {
        GiaTri = parseFloat(Bang_arrGiaTri[i][c]);
        TongSo = TongSo + GiaTri;
        if (parseFloat(Bang_arrGiaTri[i][c]) > 0) {// ham kiem tra so luong chung tu chi tiet
            iSoLuongChungTuChiTiet++;
        }
    }
    document.getElementById('lblTongSo').innerHTML = FormatNumber(TongSo);
    parent.iSoLuongChungTu = iSoLuongChungTuChiTiet;

}

function BangDuLieu_onCellBeforeEdit(h, c) {

    if (Bang_arrMaCot[c].endsWith("_No")) {
        if (Bang_arrMaCot[c] == "sTenTaiKhoan_No") {
            //            if (Bang_LayGiaTri(h, "sTenTaiKhoan_Co") != "") {
            //                //Trường hợp đã có tài khoản nợ thì không nhập tài khoản có
            //                return false;
            //            }
        }
        else {
            if (Bang_LayGiaTri(h, "sTenTaiKhoan_No") == "") {
                //Trường hợp phải có tên tài khoản mới nhập các thông tin thêm
                return false;
            }
        }
    }
    else if (Bang_arrMaCot[c].endsWith("_Co")) {
        if (Bang_arrMaCot[c] == "sTenTaiKhoan_Co") {
            //            if (Bang_LayGiaTri(h, "sTenTaiKhoan_No") != "") {
            //                //Trường hợp đã có tài khoản nợ thì không nhập tài khoản có
            //                return false;
            //            }
        }
        else {
            if (Bang_LayGiaTri(h, "sTenTaiKhoan_Co") == "") {
                //Trường hợp phải có tên tài khoản mới nhập các thông tin thêm
                return false;
            }
        }
    }
    return true;
}

function BangDuLieu_onCellValueChanged(h, c) {
    if (h == 0 && Bang_arrMaCot[c] == "sNoiDung") {
        BangDuLieu_GoiHamThayDoiNoiDung(Bang_arrGiaTri[h][c]);
        if (document.getElementById("sNoiDung").value == "") {
            BangDuLieu_GoiHamThayDoiNoiDung(Bang_arrGiaTri[h][c]);
        }
    }
    if (Bang_arrMaCot[c] == "sTenTaiKhoan_Co" && Bang_arrGiaTri[h][c] == "") {
        Bang_GanGiaTriThatChoO_colName(h, "sTenGiaiThich_Co", "");
        Bang_GanGiaTriThatChoO_colName(h, "sTenPhongBan_Co", "");
        Bang_GanGiaTriThatChoO_colName(h, "sTenDonVi_Co", "");

        Bang_GanGiaTriThatChoO_colName(h, "iID_MaTaiKhoanGiaiThich_Co", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaPhongBan_Co", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaDonVi_Co", "");
    }
    if (Bang_arrMaCot[c] == "sTenTaiKhoan_No" && Bang_arrGiaTri[h][c] == "") {
        Bang_GanGiaTriThatChoO_colName(h, "sTenGiaiThich_No", "");
        Bang_GanGiaTriThatChoO_colName(h, "sTenPhongBan_No", "");
        Bang_GanGiaTriThatChoO_colName(h, "sTenDonVi_No", "");

        Bang_GanGiaTriThatChoO_colName(h, "iID_MaTaiKhoanGiaiThich_No", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaPhongBan_No", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaDonVi_No", "");
    }
}

/* Ham BangDuLieu_CheckAll_value
- Muc dinh: Check or Uncheck tat ca cac o co kieu du lieu la kieu 2(Checkbox)
*/
var BangDuLieu_CheckAll_value = false;
function BangDuLieu_CheckAll(checked) {
    if (typeof checked == "undefined") {
        BangDuLieu_CheckAll_value = !BangDuLieu_CheckAll_value;
    }
    else {
        BangDuLieu_CheckAll_value = checked;
    }
    var value = "0";
    if (BangDuLieu_CheckAll_value) {
        value = "1";
    }
    else {
        value = "0";
    }
    var h, c = Bang_arrCSMaCot["bDongY"];
    for (h = 0; h < Bang_arrMaHang.length; h++) {
        Bang_GanGiaTriThatChoO(h, c, value);
    }
}

/* Ham BangDuLieu_TinhTongHangCon
*   - Muc dinh: Tao 1 hang moi o tai vi tri 'h' lay du lieu tai vi tri 'hGiaTri'
*   - Dau vao:  + h: vi tri them
*               + hGiaTri: vi tri hang lay du lieu, =null: them 1 hang trong
*/
function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    if (Bang_ChiDoc == false) {
        var csH = 0;
        if (h != null && h >= 0) {
            //Thêm 1 hàng mới vào hàng h
            csH = h;
        }
        else {
            //Thêm 1 hàng mới vào cuối bảng
            csH = Bang_nH;
        }
        Bang_ThemHang(csH, hGiaTri);
        //Sua MaHang="": Day la hang them moi
        Bang_arrMaHang[csH] = "";
        Bang_keys.fnSetFocus(csH, 0);
        if (Bang_nH == 1) {
            var iThang = parseInt(document.getElementById('iThang').value);
            cs = Bang_arrCSMaCot["iThang"];
            Bang_GanGiaTriThatChoO(csH, cs, iThang);
            cs = Bang_arrCSMaCot["iNgay"];
            var date = new Date();
            Bang_GanGiaTriThatChoO(csH, cs, date.getDate());
        }
        cs = Bang_arrCSMaCot["bDongY"];
        Bang_GanGiaTriThatChoO(csH, cs, "0");
        cs = Bang_arrCSMaCot["sLyDo"];
        Bang_GanGiaTriThatChoO(csH, cs, "");
        cs = Bang_arrCSMaCot["sMauSac"];
        Bang_GanGiaTriThatChoO(csH, cs, "");
        for (i = 0; i < Bang_nC; i++) {
            if (Bang_arrMaCot[i] != 'bDongY' && Bang_arrMaCot[i] != 'sLyDo' && Bang_arrMaCot[i] != 'sID_MaNguoiDungTao') {
                Bang_arrEdit[0][i] = true;
            }
            if (Bang_arrMaCot[i] == 'sID_MaNguoiDungTao') {
                Bang_arrEdit[0][i] = false;
            }
        }
    }
}

/* Su kien BangDuLieu_onKeypress_F2
- Muc dinh: goi ham them hang khi an phim F2
*/
function BangDuLieu_onKeypress_F2(h, c) {
    if (Bang_ChiDoc == false) {
        BangDuLieu_ThemHangMoi(h + 1, h);
        //Gán các giá trị của hàng mới thêm =0
        var arrTruongTien = "rSoTien".split(',');
        var i, cs;
        for (i = 0; i < arrTruongTien.length; i++) {
            cs = Bang_arrCSMaCot[arrTruongTien[i]];
            Bang_GanGiaTriThatChoO(h + 1, cs, 0);
        }
        for (i = 0; i < Bang_nC; i++) {
            if (Bang_arrMaCot[i] != 'bDongY' && Bang_arrMaCot[i] != 'sLyDo' && Bang_arrMaCot[i] != 'sID_MaNguoiDungTao') {
                Bang_arrEdit[h + 1][i] = true;
            }
            if (Bang_arrMaCot[i] == 'sID_MaNguoiDungTao') {
                Bang_arrEdit[h + 1][i] = false;
            }
        }
        BangDuLieu_CapNhapSTT();
    }
}

/* Su kien BangDuLieu_onKeypress_Delete
- Muc dinh: goi ham xoa hang khi an phim DELETE
*/
function BangDuLieu_onKeypress_Delete(h, c) {
    if (h != null) {
        Bang_XoaHang(h);
        if (h < Bang_nH) {
            Bang_keys.fnSetFocus(h, c);
        }
        else if (Bang_nH > 0) {

        }
        BangDuLieu_CapNhapSTT();
    }
}

function BangDuLieu_CapNhapSTT() {
    for (var h = 0; h < Bang_nH; h++) {
        var cs = Bang_arrCSMaCot["iSTT"];
        Bang_GanGiaTriThatChoO(h, cs, h + 1);
    }
}
//var GhepKyTu = "";
//function BangDuLieu_onCellKeypress(h, c, e, iKey) {
//    var MaKhoa;
//    if (iKey == 0) {
//        MaKhoa = e.charCode;
//    } else {
//        MaKhoa = iKey;
//    }
//    if (browserID == 'msie') {
//        if (96 <= MaKhoa && MaKhoa <= 105) {
//            MaKhoa = MaKhoa - 48;
//        }
//    }
//    else if (browserID == 'mozilla') {
//    }
//    else {
//    }

//    if ((48 <= MaKhoa && MaKhoa <= 58) ||
//                    (96 <= MaKhoa && MaKhoa <= 122) ||
//                    (65 <= MaKhoa && MaKhoa <= 90)) {
//        //Nhập số
//        KeyTable_sKyTuVuaNhap = String.fromCharCode(MaKhoa);
//        GhepKyTu = GhepKyTu + KeyTable_sKyTuVuaNhap;
//    }
//    if (Bang_arrMaCot[c] == "iNgay") {
//        var GiaTri = Bang_arrGiaTri[h][c];
//        if (GiaTri != GhepKyTu) {
//            if (GhepKyTu.length == 2) {
//                Bang_keys.fnSetFocus(h, c + 1);
//                GhepKyTu = "";
//            }
//        }
//    }
//    return true;
//}
//function BangDuLieu_onCellKeyUp(h, c, item) {
//    if (Bang_arrType[c] == 0) {
//        var GiaTri = new String(Bang_arrGiaTri[h][c]);
//        var KyTu = new String("");
//        var sStr = new String("");
//        if (GiaTri.toString().length > 5) {
//            if (Bang_arrMaCot[c] == "sSoChungTuChiTiet") {
//                KyTu = GiaTri.toUpperCase();
//                Bang_GanGiaTriThatChoO(h, c, KyTu);
//                return true;
//            }
//            KyTu = GiaTri.toString().substring(0, 1);
//            KyTu = KyTu.toUpperCase();
//            sStr = GiaTri.toString().substring(1, GiaTri.toString().length);
//            Bang_GanGiaTriThatChoO(h, c, KyTu + sStr);
//            return true;
//        }
//    }
//}

/* Su kien BangDuLieu_onCellAfterEdit
*   - Muc dinh: Su kien xuat hien sau khi nhap du lieu moi tren o (h,c) cua bang du lieu
*   - Dau vao:  + h: chi so hang 
*               + c: chi so cot
*/
function BangDuLieu_onCellAfterEdit(h, c, item) {
    var cs;

    if (Bang_arrType[c] == 0) {
        var GiaTri = new String(Bang_arrGiaTri[h][c]);
        var KyTu = new String("");
        var sStr = new String("");
        if (GiaTri.toString().length > 0) {
            if (Bang_arrMaCot[c] == "sSoChungTuChiTiet") {
                KyTu = GiaTri.toUpperCase();
                Bang_GanGiaTriThatChoO(h, c, KyTu);
                return true;
            }
            KyTu = GiaTri.toString().substring(0, 1);
            KyTu = KyTu.toUpperCase();
            sStr = GiaTri.toString().substring(1, GiaTri.toString().length);
            Bang_GanGiaTriThatChoO(h, c, KyTu + sStr);
        }
    }
    if (Bang_arrMaCot[c] == "sTenTaiKhoan_Co") {
        Bang_GanGiaTriThatChoO_colName(h, "sTenTaiKhoanGiaiThich_Co", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaTaiKhoanGiaiThich_Co", "");
        if (Bang_arrGiaTri[h][c] != "") {
            if (item.CoTaiKhoanGiaiThich == "1") {
                cs = Bang_arrCSMaCot["sTenTaiKhoanGiaiThich_Co"];
                Bang_keys.fnSetFocus(h, cs);
            }
            else {
                cs = Bang_arrCSMaCot["sTenPhongBan_Co"];
                Bang_keys.fnSetFocus(h, cs);
            }
            return false;
        }
        else {
        }

    }
    if (Bang_arrMaCot[c] == "sTenTaiKhoan_No") {
        Bang_GanGiaTriThatChoO_colName(h, "sTenTaiKhoanGiaiThich_No", "");
        Bang_GanGiaTriThatChoO_colName(h, "iID_MaTaiKhoanGiaiThich_No", "");
        if (Bang_arrGiaTri[h][c] != "") {
            if (item.CoTaiKhoanGiaiThich == "1") {
                cs = Bang_arrCSMaCot["sTenTaiKhoanGiaiThich_No"];
                Bang_keys.fnSetFocus(h, cs);
            }
            else {
                cs = Bang_arrCSMaCot["sTenPhongBan_No"];
                Bang_keys.fnSetFocus(h, cs);
            }
            return false;
        }
        else {
        }
    }
    if (Bang_arrMaCot[c] == "rSoTien") {
        TinhTongChiTiet();
        return true;
    }
    return false;
}

function BangDuLieu_onEnter_NotSetCellFocus() {
    BangDuLieu_onKeypress_F2(Bang_nH - 1);
}

function BangDuLieu_onBodyFocus() {
    Bang_keys.focus();
}

function BangDuLieu_onBodyBlur() {
    Bang_keys.blur();
}

function BangDuLieu_save() {
    Bang_HamTruocKhiKetThuc();
}

function BangDuLieu_onKeypress_F10(h, c) {
    parent.ChungTuChiTiet_onKeypress_F10(-1, -1);
    return false;
}
//Gán giá trị cho trường iID_MaChungTu

function GanGiaTriTruong_iID_MaChungTu(GiaTri) {
    document.getElementById('iID_MaChungTu').value = GiaTri;
}

//Frame chứng từ gọi khi có thay đổi giá trị các trường của chứng từ
function ChungTuChiTiet_ThayDoiTruongChungTu(Truong, GiaTri) {
    document.getElementById(Truong).value = GiaTri;
    if (Truong == "iNgay" && Bang_nH == 1) {
        Bang_GanGiaTriThatChoO(0, 0, parseInt(GiaTri));
        var cs = Bang_arrCSMaCot["iNgayCT"];
        if (Bang_arrGiaTri[0][cs] != "") {
            Bang_GanGiaTriThatChoO(0, cs, GiaTri);
        }
    }
}

//Parent kiểm tra có thay đổi dữ liệu hay không
function ChungTuChiTiet_KiemTraCoThayDoi() {
    return Bang_DaThayDoi;
}

//Parent hủy có thay đổi dữ liệu
function ChungTuChiTiet_HuyCoThayDoi() {
    Bang_DaThayDoi = false;
}

//Gọi hàm thay đổi nội dung trên form parent
function BangDuLieu_GoiHamThayDoiNoiDung(sNoiDung) {
    parent.ChungTuChiTiet_ThayDoiTruongNoiDung(sNoiDung);
}
function BangDuLieu_onKeypress_F12(h, c) {
    // parent.jsKTTH_Dialog_TinhTong_Show();
    var strNoidung = "";
    var strLable = "";
    var rSoTien_TongSo = 0;
    var Count_TKNo = 0;
    var GiaTri_TKNo = "", GiaTri_TKCo = "", GiaTri_TKNo_Count = "";
    var iID_MaTaiKhoan_No = "", iID_MaTaiKhoan_Co = "";
    for (i = 0; i < Bang_nH; i++) {
        var rSoTien = parseFloat(Bang_arrGiaTri[i][Bang_arrCSMaCot["rSoTien"]]);
        rSoTien_TongSo += rSoTien;
        var TKNo = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_MaTaiKhoan_No"]];
        var TKCo = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_MaTaiKhoan_Co"]];
        if (TKNo != "") {
            if (i > 0 && GiaTri_TKNo_Count.indexOf(TKNo) == -1) {
                Count_TKNo++;
            }
            GiaTri_TKNo_Count += "," + TKNo;
        }
        if (TKCo != "" && iID_MaTaiKhoan_Co == "") { iID_MaTaiKhoan_Co = TKCo; }
        if (TKNo != "" && iID_MaTaiKhoan_No == "") { iID_MaTaiKhoan_No = TKNo; }
        if (Count_TKNo > 0 && iID_MaTaiKhoan_Co != "" && iID_MaTaiKhoan_No != "") {
            break; ;
        }
    }
    if (Count_TKNo > 0) {
        strLable = "Có tài khoản " + iID_MaTaiKhoan_Co + ":  <span style=\"color: #ec3237;\">" + FormatNumber(rSoTien_TongSo) + "</span>";
    }
    else {
        strLable = "Nợ tài khoản " + iID_MaTaiKhoan_No + ":  <span style=\"color: #ec3237;\">" + FormatNumber(rSoTien_TongSo) + "</span>";
    }

    strNoidung += " <table class=\"mGrid\" width=\"100%\" cellpadding=\"3\" cellspacing=\"3\" border=\"0\">";
    strNoidung += "<tr ><th style=\"width: 50px;\" align=\"center\" > <div style=\"font-size: 12px;\">STT</div> </th>";
    strNoidung += "<th style=\"width: 120px;\" align=\"center\"><div style=\"font-size: 12px;\">";
    if (Count_TKNo > 0) {
        strNoidung += "Tài khoản nợ</div> </th>";
    }
    else {
        strNoidung += "Tài khoản có</div> </th>";
    }
    strNoidung += "<th align=\"center\"><div style=\"font-size: 12px;\">Số tiền</div> </th></tr>";
   
    var number = 0;
    if (Count_TKNo > 0) {
        for (var i = 0; i < Bang_nH; i++) {

            var TKNo = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_MaTaiKhoan_No"]];
            var rSoTien = parseFloat(Bang_arrGiaTri[i][Bang_arrCSMaCot["rSoTien"]]);
            if (TKNo != "") {
                if (GiaTri_TKNo.indexOf(TKNo) == -1) {
                    number++;
                    strNoidung += " <tr><td align=\"center\">";
                    strNoidung += number;
                    strNoidung += "</td>";
                    strNoidung += " <td align=\"left\">";
                    strNoidung += TKNo;
                    strNoidung += "</td>";
                    for (var j = i + 1; j < Bang_nH; j++) {
                        if (TKNo == Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_MaTaiKhoan_No"]]) {
                            rSoTien += parseFloat(Bang_arrGiaTri[j][Bang_arrCSMaCot["rSoTien"]]);
                        }
                    }
                    strNoidung += "<td align=\"right\">";
                    //tien
                    strNoidung += "<b>" + FormatNumber(rSoTien) + "</b>";
                    strNoidung += "</td>";
                    strNoidung += "</tr>";
                }
                GiaTri_TKNo += "," + TKNo;
            }
        }
        
    }
    else {
        for (var i = 0; i < Bang_nH; i++) {
            var TKCo = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_MaTaiKhoan_Co"]];
            var rSoTien = parseFloat(Bang_arrGiaTri[i][Bang_arrCSMaCot["rSoTien"]]);
            if (TKCo != "") {
                if (GiaTri_TKCo.indexOf(TKCo) == -1) {
                    number++;
                    strNoidung += " <tr><td align=\"center\">";
                    strNoidung += number;
                    strNoidung += "</td>";
                    strNoidung += " <td align=\"left\">";
                    strNoidung += TKCo;
                    strNoidung += "</td>";
                    for (var j = i + 1; j < Bang_nH; j++) {
                        if (TKCo == Bang_arrGiaTri[j][Bang_arrCSMaCot["iID_MaTaiKhoan_Co"]]) {
                            rSoTien += parseFloat(Bang_arrGiaTri[j][Bang_arrCSMaCot["rSoTien"]]);
                        }
                    }
                    strNoidung += "<td align=\"right\">";
                    //tien
                    strNoidung += "<b>" + FormatNumber(rSoTien) + "</b>";
                    strNoidung += "</td>";
                    strNoidung += "</tr>";
                }
                GiaTri_TKCo += "," + TKCo;

            }
        }
    }
   
    strNoidung += "</table>";
    parent.jsKTTH_Dialog_F12_Show(strNoidung, strLable);
}
function BangDuLieu_onKeypress_F5(h, c) {
    parent.jsKTTH_Dialog_Tao_CTGS_Show();
}
function BangDuLieu_onKeypress_F11(h, c) {
    parent.jsKTTH_Dialog_TinhTong_Show();  
}