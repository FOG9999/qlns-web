var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var ERROR = 1;
var Bang_arrGiaTriTemp = Bang_arrGiaTri;

//function BangDuLieu_onKeypress_F1_NoRow(h, c) {
//    if (c == undefined) {
//        BangDuLieu_ThemHangMoi(h, c, true);
//    } else {
//        if (!Bang_arrHangDaXoa[h]) {
//            BangDuLieu_ThemHangMoi(h + 1, h, true);
//        }
//    }
//}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    if (c != undefined) {
        if (!Bang_arrHangDaXoa[h] && (Bang_arrGiaTri[h][Bang_arrCSMaCot["bIsAction"]] == "True" || Bang_arrGiaTri[h][Bang_arrCSMaCot["bIsTTCP"]] == "False")) {
            BangDuLieu_ThemHangMoi(h + 1, h, false);
        }
    }
}

function BangDuLieu_ThemHangMoi(h, hGiaTri, isSameLevel) {
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }
    //Lấy STT của hàng
    var sSTT = "";
    var rowPositionMaxStt = 0;
    if (h == 0) {
        sSTT = "1";
    }
    else {
        if (isSameLevel) {
            var sTTForcus = Bang_arrGiaTri[h - 1][0];
            var lastIndexDot = sTTForcus.lastIndexOf(".");
            var regex;
            if (lastIndexDot > 0) {
                sTTForcus = sTTForcus.substr(0, lastIndexDot);
                regex = new RegExp('^' + sTTForcus + '.[0-9]+$');
            }
            else {
                regex = new RegExp('^[0-9]+$');
            }
            let arrMatch = [];

            // Add những dòng con vào 1 mảng
            for (var i = 0; i < Bang_arrGiaTri.length; i++) {
                var sTT = Bang_arrGiaTri[i][0];
                if (regex.test(sTT)) {
                    arrMatch.push({
                        value: sTT,
                        index: i + 1
                    });
                }
            }
            if (arrMatch.length > 0) {
                let lastChildRow = arrMatch.sort((a, b) => parseInt(a.value.replace(sTTForcus + '.', '')) - parseInt(b.value.replace(sTTForcus + '.', '')))[arrMatch.length - 1];
                let maxStt = lastChildRow.value;
                var regexPosition = RegExp('^' + maxStt + '(.[0-9]+)*$');
                var arrMatchPosition = [];
                for (var p = 0; p < Bang_arrGiaTri.length; p++) {
                    var sTTPosition = Bang_arrGiaTri[p][0];
                    if (regexPosition.test(sTTPosition)) {
                        arrMatchPosition.push({
                            value: sTTPosition,
                            index: p + 1
                        });
                    }
                }
                var lastPositionRow = arrMatchPosition.sort((a, b) => a.index - b.index)[arrMatchPosition.length - 1];
                rowPositionMaxStt = lastPositionRow.index;
                let maxIndexStt = parseInt(maxStt.replace(sTTForcus + '.', ''));
                // Tính ra stt mới.
                let indexSttResult = maxIndexStt + 1;
                if (lastIndexDot > 0) {
                    sSTT = sTTForcus + '.' + indexSttResult;
                } else {
                    sSTT = indexSttResult + "";
                }
            }
        } else {
            Bang_GanGiaTriThatChoO(h - 1, Bang_arrCSMaCot["bIsHasChild"], 'True');
            var sTTForcus = Bang_arrGiaTri[h - 1][0];
            const regex = new RegExp('^' + sTTForcus + '.[0-9]+$');
            let arrMatch = [];

            // Add những dòng con vào 1 mảng
            for (var i = 0; i < Bang_arrGiaTri.length; i++) {
                var sTT = Bang_arrGiaTri[i][0];
                if (regex.test(sTT)) {
                    arrMatch.push({
                        value: sTT,
                        index: i + 1
                    });
                }
            }
            if (arrMatch.length > 0) {
                let lastChildRow = arrMatch.sort((a, b) => parseInt(a.value.replace(sTTForcus + '.', '')) - parseInt(b.value.replace(sTTForcus + '.', '')))[arrMatch.length - 1];
                let maxStt = lastChildRow.value;
                var regexPosition = RegExp('^' + maxStt + '(.[0-9]+)*$');
                var arrMatchPosition = [];
                for (var p = 0; p < Bang_arrGiaTri.length; p++) {
                    var sTTPosition = Bang_arrGiaTri[p][0];
                    if (regexPosition.test(sTTPosition)) {
                        arrMatchPosition.push({
                            value: sTTPosition,
                            index: p + 1
                        });
                    }
                }
                var lastPositionRow = arrMatchPosition.sort((a, b) => a.index - b.index)[arrMatchPosition.length - 1];
                rowPositionMaxStt = lastPositionRow.index;
                let maxIndexStt = parseInt(maxStt.replace(sTTForcus + '.', ''));
                // Tính ra stt mới.
                let indexSttResult = maxIndexStt + 1;
                sSTT = sTTForcus + '.' + indexSttResult;
            } else {
                sSTT = sTTForcus + '.' + "1";
                rowPositionMaxStt = h;
            }
        }
    }
    Bang_ThemHang(rowPositionMaxStt, hGiaTri);
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["ID"], uuidv4());
    if (h > 0) {
        if (isSameLevel) {
            var iID_ParentID = Bang_arrGiaTri[h - 1][Bang_arrCSMaCot["iID_ParentID"]];
            Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["iID_ParentID"], iID_ParentID);
        }
        else {
            var iID_ParentID = Bang_arrGiaTri[h - 1][Bang_arrCSMaCot["ID"]];
            Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["iID_ParentID"], iID_ParentID);
        }
    }
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["sMaThuTu"], sSTT);
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["sTenNhiemVuChi"], '');
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["fGiaTriBQP_USD"], '');
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["fGiaTriBQP_VND"], '');
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["bIsHasChild"], 'False');
    Bang_GanGiaTriThatChoO(rowPositionMaxStt, Bang_arrCSMaCot["bIsTTCP"], 'False');
    if (!isSameLevel) {
        Bang_arrLaHangCha[h - 1] = true;
        Bang_arrLaHangCha[rowPositionMaxStt] = false;
    }
    else {
        Bang_arrLaHangCha[rowPositionMaxStt] = Bang_arrGiaTri[h - 1][Bang_arrCSMaCot["iID_ParentID"]] == "";
    }

    setActiveHC();
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[rowPositionMaxStt] = "";

    Bang_keys.fnSetFocus(rowPositionMaxStt, 1);
    Bang_HienThiDuLieu();
}

function Bang_ThemHang(cs, csGoc) {
    var i, j;
    if (typeof csGoc == "undefined") {
        //Them hang du lieu moi
        Bang_nH = Bang_nH + 1;
        Bang_Viewport_ThayDoiHang();

        Bang_arrMaHang.splice(cs, 0, "");
        Bang_arrCSCha.splice(cs, 0, -1);
        Bang_arrLaHangCha.splice(cs, 0, false);
        Bang_arrHangDaXoa.splice(cs, 0, false);
        var arrGT = new Array();
        var arrHT = new Array();
        var arrThayDoi = new Array();
        var arrEdit = new Array();

        for (j = 0; j < Bang_nC; j++) {
            switch (Bang_arrType[j]) {
                case 1:
                    arrGT.push(0);
                    break;

                case 2:
                    arrGT.push("0");
                    break;

                default:
                    arrGT.push("");
                    break;
            }
            arrHT.push("");
            arrThayDoi.push(false);
            arrEdit.push(true);
        }
        Bang_arrGiaTri.splice(cs, 0, arrGT);
        Bang_arrHienThi.splice(cs, 0, arrHT);
        Bang_arrThayDoi.splice(cs, 0, arrThayDoi);
        Bang_arrEdit.splice(cs, 0, arrEdit);

        //Sua lai Bang_arrCSCha: Cac hang cha co chi so lon hon cs se bi thay doi chi so
        for (i = 0; i < Bang_nH; i++) {
            if (Bang_arrCSCha[i] >= cs) {
                Bang_arrCSCha[i] = Bang_arrCSCha[i] + 1;
            }
        }
        Bang_HienThiDuLieu();
        return true;
    } else {
        //Them hang du lieu dua tren 1 hang du lieu khac
        Bang_nH = Bang_nH + 1;
        Bang_Viewport_ThayDoiHang();

        Bang_arrMaHang.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrMaHang[csGoc]));
        Bang_arrCSCha.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrCSCha[csGoc]));
        Bang_arrLaHangCha.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrLaHangCha[csGoc]));
        Bang_arrGiaTri.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrGiaTri[csGoc]));
        Bang_arrHienThi.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrHienThi[csGoc]));
        Bang_arrThayDoi.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrThayDoi[csGoc]));
        Bang_arrEdit.splice(cs, 0, Bang_TaoDoiTuongMoi(Bang_arrEdit[csGoc]));
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
    return false;
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function DanhLaiSTT(parentID, Bang_arrGiaTriTemp, STTCha) {
    var j = 1;
    for (var i = 0; i < Bang_arrGiaTriTemp.length; i++) {
        if (Bang_arrGiaTriTemp[i][Bang_arrCSMaCot["iID_ParentID"]] == parentID && !Bang_arrHangDaXoa[i]) {
            Bang_arrGiaTriTemp[i][Bang_arrCSMaCot["sMaThuTu"]] = STTCha + j;
            DanhLaiSTT(Bang_arrGiaTriTemp[i][Bang_arrCSMaCot["ID"]], Bang_arrGiaTriTemp, STTCha + j + ".");
            j++;
        }
    }
}

function KHChiTietBQP_BangDuLieu_Save() {
    //kiem tra ngay thang nhap tren luoi chi tiet
    if (!ValidateData()) {
        return false;
    }
    let state = $("#currentState").val();
    Bang_arrGiaTriTemp = Bang_arrGiaTri;
    DanhLaiSTT("", Bang_arrGiaTriTemp, "");
    var Bang_arrGiaTriNew = new Array(2);
    var j = 0;
    for (var i = 0; i < Bang_arrGiaTriTemp.length; i++) {
        if (!Bang_arrHangDaXoa[i]) {
            Bang_arrGiaTriNew[j] = Bang_arrGiaTriTemp[i];
            Bang_arrGiaTriNew[j][Bang_arrCSMaCot["isAdd"]] = Bang_arrThayDoi[i][Bang_arrCSMaCot["ID"]];
            j++;
        }
    }

    var tableNhiemVuChi = [];
    for (var i = 0; i < Bang_arrGiaTriNew.length; i++) {
        let rowData = {};
        rowData.ID = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["ID"]];
        rowData.sMaThuTu = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["sMaThuTu"]];
        rowData.sTenNhiemVuChi = $("<div/>").text($.trim(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["sTenNhiemVuChi"]])).html();
        rowData.iID_BQuanLyID = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_BQuanLyID"]];
        rowData.bIsTTCP = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["bIsTTCP"]] == 'True';
        if (Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_DonViID"]] != "") {
            rowData.iID_MaDonVi = $("<div/>").text(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_MaDonVi"]]).html();
            rowData.iID_DonViID = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_DonViID"]];
            rowData.hasDonVi = true;
        } else {
            rowData.iID_MaDonVi = null;
            rowData.iID_DonViID = null;
            rowData.hasDonVi = false;
        }

        rowData.sGiaTri = UnFormatNumber($.trim(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["fGiaTri"]]).toString());
        rowData.iID_ParentID = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_ParentID"]];
        rowData.isAdd = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["isAdd"]];

        rowData.fGiaTriUSD = UnFormatNumber($.trim(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["fGiaTriBQP_USD"]]).toString());
        rowData.fGiaTriVND = UnFormatNumber($.trim(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["fGiaTriBQP_VND"]]).toString());
        rowData.iID_KHTTTTCP_NhiemVuChiID = Bang_arrGiaTriNew[i][Bang_arrCSMaCot["iID_KHTTTTCP_NhiemVuChiID"]];
        rowData.fGiaTriTTCP_USD = UnFormatNumber(Bang_arrGiaTriNew[i][Bang_arrCSMaCot["fGiaTriTTCP_USD"]]);
        tableNhiemVuChi.push(rowData);
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/SaveKeHoachChiTietBQP",
        data: { lstNhiemVuChis: tableNhiemVuChi, state: state },
        success: function (data) {
            if (data.result) {
                window.parent.GoBack();
            } else {
                var Title = 'Lỗi thêm mới kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                var Messages = ['Thêm mới kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
                if (state == 'UPDATE') {
                    Title = 'Lỗi cập nhật kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                    Messages = ['Cập nhật kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
                } else if (state == 'ADJUST') {
                    Title = 'Lỗi điều chỉnh kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                    Messages = ['Điều chỉnh kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
                }

                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function ValidateData() {
    var sMessError = [];
    var sChuongTrinh = "", sDonVi = "", sGiaTriUSD = "", sGiaTriVND = "";
    var Title = 'Lỗi lưu kế hoạch chi tiết BQP';
    if (Bang_nH == 0) {
        sMessError.push("Quyết định hiện tại chưa có chương trình, nhiệm vụ chi. Vui lòng thêm chương trình, nhiệm vụ chi.");
    }
    else {
        for (var j = 0; j < Bang_nH; j++) {
            if (!Bang_arrHangDaXoa[j]) {
                var sTen = Bang_LayGiaTri(j, "sTenNhiemVuChi");
                if (sTen == '') {
                    sChuongTrinh += Bang_arrGiaTri[j][Bang_arrCSMaCot["sMaThuTu"]] + ", ";
                }
                var fGiaTriVND = Bang_LayGiaTri(j, "fGiaTriBQP_VND");
                if (fGiaTriVND == '' || parseFloat(fGiaTriVND) <= 0) {
                    sGiaTriVND += Bang_arrGiaTri[j][Bang_arrCSMaCot["sMaThuTu"]] + ", ";
                }

                var fGiaTriUSD = Bang_LayGiaTri(j, "fGiaTriBQP_USD");
                if (fGiaTriUSD == '' || parseFloat(fGiaTriUSD) <= 0) {
                    sGiaTriUSD += Bang_arrGiaTri[j][Bang_arrCSMaCot["sMaThuTu"]] + ", ";
                }

                var iDDonVi = Bang_LayGiaTri(j, "iID_DonViID");
                if ((iDDonVi == '' || iDDonVi == GUID_EMPTY) && Bang_LayGiaTri(j, "bIsTTCP") == "False") {
                    sDonVi += Bang_arrGiaTri[j][Bang_arrCSMaCot["sMaThuTu"]] + ", ";
                }
            }
        }
        if (sChuongTrinh != "") {
            sMessError.push('Hãy nhập tên chương trình, nhiệm vụ chi dòng số ' + sChuongTrinh.substring(0, sChuongTrinh.length - 2) + '.');
        }
        if (sDonVi != "") {
            sMessError.push('Chương trình, nhiệm vụ chi dòng ' + sDonVi.substring(0, sDonVi.length - 2) + ' chưa có thông tin đơn vị.');
        }
        if (sGiaTriUSD != "") {
            sMessError.push('Chương trình, nhiệm vụ chi dòng ' + sGiaTriUSD.substring(0, sGiaTriUSD.length - 2) + ' chưa có thông tin giá trị BQP phê duyệt USD.');
        }
        if (sGiaTriVND != "") {
            sMessError.push('Chương trình, nhiệm vụ chi dòng ' + sGiaTriVND.substring(0, sGiaTriVND.length - 2) + ' chưa có thông tin giá trị BQP phê duyệt VND.');
        }
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

function BangDuLieu_onKeypress_F10() {
    KHChiTietBQP_BangDuLieu_Save();
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
}


function BangDuLieu_onCellAfterEdit(h, c) {
    if (c == Bang_arrCSMaCot["fGiaTriBQP_USD"]) {
        CalculateSum(h, c);
        var fGiaTriBQP_VND = parseFloat(noExponents(Bang_arrGiaTri[h][c])) * parseFloat(noExponents(Bang_arrGiaTri[h][Bang_arrCSMaCot["TiGia"]]));
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBQP_VND"], fGiaTriBQP_VND);
        CalculateSum(h, Bang_arrCSMaCot["fGiaTriBQP_VND"]);
    }
    else if (c == Bang_arrCSMaCot["fGiaTriBQP_VND"]) {
        CalculateSum(h, c);
        var fGiaTriBQP_USD = parseFloat(noExponents(Bang_arrGiaTri[h][c])) * parseFloat(noExponents(Bang_arrGiaTri[h][Bang_arrCSMaCot["TiGia"]]));
        Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["fGiaTriBQP_USD"], fGiaTriBQP_USD);
        CalculateSum(h, Bang_arrCSMaCot["fGiaTriBQP_USD"]);
    }
    return true;
}

function noExponents(value) {
    var data = String(value).split(/[eE]/);
    if (data.length == 1) return data[0];

    var z = '',
        sign = this < 0 ? '-' : '',
        str = data[0].replace(',', ''),
        mag = Number(data[1]) + 1;

    if (mag < 0) {
        z = sign + '0.';
        while (mag++) z += '0';
        return z + str.replace(/^\-/, '');
    }
    mag -= str.length;
    while (mag--) z += '0';
    return str + z;
}

function CalculateSum(h, c) {
    var iID_ParentID = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_ParentID"]];
    while (iID_ParentID != "") {
        var sum = 0;
        var indexParent = 0;
        var iID_ParentIDTemp = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_ParentID"]];
        for (var p = 0; p < Bang_arrGiaTri.length; p++) {
            if (Bang_arrGiaTri[p][Bang_arrCSMaCot["iID_ParentID"]] == iID_ParentID && !Bang_arrHangDaXoa[p]) {
                sum += parseFloat(Bang_arrGiaTri[p][c]);
            }
            if (Bang_arrGiaTri[p][Bang_arrCSMaCot["ID"]] == iID_ParentID) {
                iID_ParentIDTemp = Bang_arrGiaTri[p][Bang_arrCSMaCot["iID_ParentID"]];
                indexParent = p;
            }
        }
        iID_ParentID = iID_ParentIDTemp;
        Bang_GanGiaTriThatChoO(indexParent, c, sum);
    }
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

function BangDuLieu_onKeypress_Delete(h, c) {
    var bHasChild = Bang_arrGiaTri[h][Bang_arrCSMaCot["bIsHasChild"]];
    var iID_ParentID = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_ParentID"]];
    var bIsTTCP = Bang_arrGiaTri[h][Bang_arrCSMaCot["bIsTTCP"]];
    if (BangDuLieu_DuocSuaChiTiet && h != null && bHasChild == 'False' && bIsTTCP == "False") {
        // check xem hàng h đã bị xóa chưa, nếu chưa bị xóa thì xóa, nếu xóa r thì thôi
        if (!Bang_arrHangDaXoa[h]) {
            BangDuLieu_XoaHang(h);
            if (h < Bang_nH) {
                Bang_keys.fnSetFocus(h, c);
            }
            var countRow = 0;
            var numParent = 0;
            for (var p = 0; p < Bang_arrGiaTri.length; p++) {
                if (Bang_arrGiaTri[p][Bang_arrCSMaCot["iID_ParentID"]] == iID_ParentID && !Bang_arrHangDaXoa[p]) {
                    countRow++;
                }
                if (Bang_arrGiaTri[p][Bang_arrCSMaCot["ID"]] == iID_ParentID) {
                    numParent = p;
                }
            }
            if (countRow == 0) {
                Bang_GanGiaTriThatChoO(numParent, Bang_arrCSMaCot["bIsHasChild"], 'False');
                Bang_arrLaHangCha[numParent] = false;
                var nameColumn = Bang_arrGiaTri[numParent][Bang_arrCSMaCot["IsVNDToUSD"]] == "True" ? "fGiaTriBQP_VND" : "fGiaTriBQP_USD";
                Bang_arrEdit[numParent][Bang_arrCSMaCot[nameColumn]] = true;
            }

        } else {
            var countRowDelete = 0;
            for (var p = 0; p < Bang_arrGiaTri.length; p++) {
                if (Bang_arrGiaTri[p][Bang_arrCSMaCot["ID"]] == iID_ParentID && Bang_arrHangDaXoa[p]) {
                    countRowDelete++;
                }
            }
            if (countRowDelete == 0) {
                BangDuLieu_XoaHang(h);
                for (var p = 0; p < Bang_arrGiaTri.length; p++) {
                    if (Bang_arrGiaTri[p][Bang_arrCSMaCot["ID"]] == iID_ParentID) {
                        Bang_GanGiaTriThatChoO(p, Bang_arrCSMaCot["bIsHasChild"], 'True');
                        Bang_arrLaHangCha[p] = true;
                        var nameColumn = Bang_arrGiaTri[p][Bang_arrCSMaCot["IsVNDToUSD"]] == "True" ? "fGiaTriBQP_VND" : "fGiaTriBQP_USD";
                        Bang_arrEdit[p][Bang_arrCSMaCot[nameColumn]] = false;
                    }
                }
            }
        }
        Bang_HienThiDuLieu();
        CalculateSum(h, Bang_arrCSMaCot["fGiaTriBQP_VND"]);
        CalculateSum(h, Bang_arrCSMaCot["fGiaTriBQP_USD"]);
    }
}

function BangDuLieu_onKeypress_F9(h, c) {
    $("#sTenNhiemVuChi, #sTenPhongBan, #sTenDonVi").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $("form.sheet-search").submit();
}
