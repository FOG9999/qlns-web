var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";

$(document).ready(function () {
    $("tr").hover(function () {
        $(this).css("background-color", "#e7f8fe");
    }, function () {
        $(this).css("background-color", "");
    });

    TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);
    TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

    DinhDangSo();

    LoadViewHangMuc();

    LayDshangMuc();
});

function LayDshangMuc() {
    var chuTruongId = $('#iIDChuTruongDauTuID').val();

    // Tạo danh sách ở phần Hạng mục
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LayDanhSachHangMucTheoChuTruongId",
        type: "POST",
        data: { iID: chuTruongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            $("#tblHangMucChinh tbody tr").remove();
            arr_HangMuc = [];

            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var dongMoi = "";

                    if (data[i].iID_ParentID == undefined || data[i].iID_ParentID == null) {
                        dongMoi += "<tr style='cursor: pointer;' class='parent'>";
                    } else {
                        dongMoi += "<tr style='cursor: pointer;'>";
                    }
                    
                    dongMoi += "<td class='r_STT width-50'>" + data[i].smaOrder + "</td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/>";
                    dongMoi += "<td class='r_sTenHangMuc'>" + (!data[i].sTenHangMuc ? "" : data[i].sTenHangMuc) + "</td>"
                    dongMoi += "<td> " + (!data[i].sTenLoaiCongTrinh ? "" : data[i].sTenLoaiCongTrinh) + "</td>";
                    
                    //dongMoi += "<td align='center'>";

                    

                    $("#tblHangMucChinh tbody").append(dongMoi);
                }
                arr_HangMuc = data;

            }
        }
    });
}


function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html()));
    })
}

function TinhLaiDongTong(idBang) {
    var tongGiaTriToTrinh = 0;
    var tongGiaTriThamDinh = 0;
    var tongGiaTriPheDuyet = 0;

    $("#" + idBang + " .r_GiaTriToTrinh").each(function () {
        tongGiaTriToTrinh += parseInt(UnFormatNumber($(this).html()));
    });

    $("#" + idBang + " .r_GiaTriThamDinh").each(function () {
        tongGiaTriThamDinh += parseInt(UnFormatNumber($(this).html()));
    });

    $("#" + idBang + " .r_GiaTriPheDuyet").each(function () {
        tongGiaTriPheDuyet += parseInt(UnFormatNumber($(this).html()));
    });

    $("#" + idBang + " .cpdt_tong_giatritotrinh").html(FormatNumber(tongGiaTriToTrinh));
    $("#" + idBang + " .cpdt_tong_gaitrithamdinh").html(FormatNumber(tongGiaTriThamDinh));
    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(tongGiaTriPheDuyet));
}

function Huy() {
    window.location.href = "/QLVonDauTu/ChuTruongDauTu/Index";
}

function isStringEmpty(x) {
    if (x == null || x == undefined || x == "") {
        return true;
    }

    return false;
}

function arrHasValue(x) {
    if (x == null || x == undefined || x.length <= 0) {
        return false;
    }

    return true;
}

function GetListDataHangMuc() {
    var items = $("#arr_HangMuc").val();
    if (items == null || items == undefined) {
        return [];
    }

    return JSON.parse(items);
}

function LoadViewHangMuc() {
    var items = GetListDataHangMuc();

    var html = "";
    if (!arrHasValue(items)) {
        $("#tblHangMucDuAn tbody").html(html);
        return;
    }

    $("#tblHangMucDuAn tbody").html(html);

    var listParent = [];

    items.forEach(x => {
        if (isStringEmpty(x.sHangMucCha)) {
            listParent.push(x);
        }
    });

    let count = 1;
    listParent.forEach(x => {
        html += '<tr style="font-weight:bold">';

        var stt = count;
        html += '<td>' + stt + '</td>';
        html += '<td>' + x.sMaHangMuc + '</td>';
        html += '<td>' + x.sTenHangMuc + '</td>';
        html += '<td>' + x.sHangMucCha + '</td>';

        html += '</tr>';
        count++;
        html += LoadViewHangMucTree(stt, items, x.sMaHangMuc);
    });

    $("#tblHangMucDuAn tbody").append(html);
}

function LoadViewHangMucTree(stt, items, id) {
    var html = "";

    var listParent = [];

    items.forEach(x => {
        if (x.sHangMucCha == id) {
            listParent.push(x);
        }
    });

    if (listParent.length <= 0) {
        return;
    }

    var countChild = 1;
    listParent.forEach(x => {
        var isCheckParent = items.find(k => k.sHangMucCha == x.sMaHangMuc);
        if (isCheckParent == null || isCheckParent == undefined) {
            html += '<tr>';
        } else {
            html += '<tr style="font-weight:bold">';
        }

        var sttChild = stt + "." + countChild;
        html += '<td>' + sttChild + '</td>';
        html += '<td>' + x.sMaHangMuc + '</td>';
        html += '<td>' + x.sTenHangMuc + '</td>';
        html += '<td>' + x.sHangMucCha + '</td>';

        html += '</tr>';

        countChild++;

        html += LoadViewHangMucTree(sttChild, items, x.sMaHangMuc);
    });

    return html;
}