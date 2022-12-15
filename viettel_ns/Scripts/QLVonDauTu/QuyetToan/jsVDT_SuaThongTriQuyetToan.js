var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iIDThongTriID = "";
var lstThongTriDetail = [];

$(document).ready(function () {
    iIDThongTriID = $("#iIDThongTriID").val();
    var bIsViewDetail = $("#bIsViewDetail").val();
    if (bIsViewDetail == 1) {
        $("#sMaThongTri").prop("disabled", "disabled");
        $("#sNguoiLap").prop("disabled", "disabled");
    }
    GetThongTriQuyetToan();
});

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}

function GetThongTriQuyetToan() {
    lstThongTriDetail = [];
    var iIdQuyetToan = $("#iIDQuyetToanId").val();
    var iIdThongTri = $("#iIDThongTriID").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriQuyetToan/GetThongTriQuyetToanDetail",
        data: { iIDQuyetToanId: iIdQuyetToan, iIdThongTri: iIdThongTri },
        success: function (data) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (data != null && data.data != null) {
                var sTable = [];
                $.each(data.data, function (index, item) {
                    sTable.push("<tr>");
                    sTable.push("<td style='text-align:center'>" + (item.L == null ? "" : item.L) + "</td>");
                    sTable.push("<td style='text-align:center'>" + (item.K == null ? "" : item.K) + "</td>");
                    sTable.push("<td style='text-align:center'>" + (item.M == null ? "" : item.M) + "</td>");
                    sTable.push("<td style='text-align:center'>" + (item.TM == null ? "" : item.TM) + "</td>");
                    sTable.push("<td style='text-align:center'>" + (item.TTM == null ? "" : item.TTM) + "</td>");
                    sTable.push("<td style='text-align:center'>" + (item.NG == null ? "" : item.NG) + "</td>");
                    sTable.push("<td>" + (item.STenLoaiCongTrinh == null ? "" : item.STenLoaiCongTrinh) + "</td>");
                    sTable.push("<td class='sotien' style='text-align:right'>" + (item.FSoTien == null ? 0 : item.FSoTien) + "</td>");
                    sTable.push("</tr>");
                });

                $("#tbl_capphatthanhtoankpqp tbody").html(sTable.join(""));
                DinhDangSo();
                lstThongTriDetail = data.data;
            } else {
                $("#tbl_capphatthanhtoankpqp tbody").empty();
            }
        }
    });
}

function Luu() {
    var thongTri = fnGetData();
    var lstDetail = fnGetThongTriDetailInfo(thongTri);
    if (CheckLoi(thongTri)) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTriQuyetToan/Luu",
            type: "POST",
            data: { model: thongTri, lstDetail: lstDetail },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data == true) {
                    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Index";
                }
            },
            error: function (data) {

            }
        })
    }
}

function fnGetData() {
    var obj = {};
    obj.iID_ThongTriID = $("#iIDThongTriID").val();
    obj.iID_BCQuyetToanNienDo = $("#iIDQuyetToanId").val();
    obj.sMaThongTri = $("#sMaThongTri").val();
    obj.sNguoiLap = $("#sNguoiLap").val();
    return obj;
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (doiTuong.sMaThongTri == "")
        messErr.push("Mã thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.sNguoiLap == "")
        messErr.push("Người lập thông tri chưa có hoặc chưa chính xác.");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function KiemTraTrungMaThongTri(sMaThongTri) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLThongTriQuyetToan/KiemTraTrungMaThongTri",
        type: "POST",
        data: { sMaThongTri: sMaThongTri, iID_ThongTriID: iIDThongTriID },
        dataType: "json",
        async: false,
        cache: false,
        success: function (data) {
            check = data;
        },
        error: function (data) {

        }
    })
    return check;
}

function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Index";
}

function fnGetThongTriDetailInfo(objThongTri) {
    var lstData = [];
    if (lstThongTriDetail == null || lstThongTriDetail.length == 0) return [];
    $.each(lstThongTriDetail, function (index, item) {
        var obj = {};
        obj.sSoThongTri = objThongTri.sMaThongTri;
        obj.iID_DuAnID = item.IIdDuAnId;
        obj.fSoTien = item.FSoTien;
        obj.iID_LoaiCongTrinhID = item.IIdLoaiCongTrinhId;
        obj = fnConvertMucLucNganSach(obj, item);
        lstData.push(obj);
    });
    return lstData;
}

function fnConvertMucLucNganSach(objResult, objDetail) {
    if (objDetail.NG != null && objDetail.NG != "")
        objResult.iID_NganhID = objDetail.IIdMucLucNganSach;
    else if (objDetail.TTM != null && objDetail.TTM != "")
        objResult.iID_TietMucID = objDetail.IIdMucLucNganSach;
    else if (objDetail.TM != null && objDetail.TM != "")
        objResult.iID_TieuMucID = objDetail.IIdMucLucNganSach;
    else if (objDetail.M != null && objDetail.M != "")
        objResult.iID_MucID = objDetail.IIdMucLucNganSach;
    return objResult;
}