var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var lstThongTriDetail = [];

$(document).ready(function () {
    GetDonViQuanLy();
    $("#iID_DonViQuanLyID").on("change", function () {
        GetChungTu();
    });
    $("#sMaNguonVon").on("change", function () {
        GetChungTu();
    });
    $("#sNamThongTri").on("change", function () {
        GetChungTu();
    });
});

function GetChungTu() {
    $("#sChungTu").empty();
    var iIDMaDonVi = $("#iID_DonViQuanLyID").val();
    var iNguonVon = $("#sMaNguonVon").val();
    var iNamThongTri = $("#sNamThongTri").val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTriQuyetToan/GetChungTuQuyetToanNienDo",
        data: { iIdThongTri: null, iIdMaDonVi: iIDMaDonVi, iNamThucHien: iNamThongTri, iIdNguonVon: iNguonVon },
        type: "GET",
        success: function (data) {
            if (data != null) {
                data.lstChungTu.forEach(function (item) {
                    $("#sChungTu").append("<option value='" + item.iID_BCQuyetToanNienDoID + "' >" + item.sSoDeNghi + "</option>")
                });
            }
        }
    });
}

function Loc() {
    GetThongTriQuyetToan();
}

function GetThongTriQuyetToan() {
    lstThongTriDetail = [];
    var iIdQuyetToan = $("#sChungTu").val();
    if (iIdQuyetToan == null) {
        alert("Chưa chọn chứng từ !");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriQuyetToan/GetThongTriQuyetToanDetail",
        data: { iIDQuyetToanId: iIdQuyetToan },
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
                    sTable.push("<td>" + (item.STenDuAn == null ? "" : item.STenDuAn) + "</td>");
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

function DinhDangSo() {
    $("#divTab .sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}

function XoaTextThongTri() {
    $("#iID_DonViQuanLyID").prop("selectedIndex", 0).trigger("change");
    $("#sMaThongTri").val("");
    $("#sNgayLapGanNhat").val("");
    $("#dNgayThongTri").val("");
    $("#sNamThongTri").val("");
    $("#sNguoiLap").val("");
    $("#sTruongPhong").val("");
    $("#sThuTruongDonVi").val("");
}

function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Index";
}

function Luu() {
    var thongTri = fnGetThongTriInfo();
    var lstDetail = fnGetThongTriDetailInfo(thongTri);
    if (fnValidate(thongTri)) {
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

function KiemTraTrungMaThongTri(sMaThongTri) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLThongTriQuyetToan/KiemTraTrungMaThongTri",
        type: "POST",
        data: { sMaThongTri: sMaThongTri },
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

function GetDonViQuanLy() {
    $.ajax({
        url: "/QLVonDauTu/QLThongTriQuyetToan/GetListDataDonViQuanLy",
        type: "GET",
        success: function (data) {
            if (data.results != null) {
                data.results.forEach(function (item) {
                    $("#iID_DonViQuanLyID").append("<option data-id='" + item.iID_Ma + "' value='" + item.iID_MaDonVi + "' >" + item.sTenLoaiDonVi + "</option>")
                });
            }
        }
    });
}

function fnValidate(objThongTri) {
    var lstMessError = [];
    if (objThongTri.iID_DonViID == null) {
        lstMessError.push("Chưa chọn đơn vị quản lý !");
    }
    if (objThongTri.sMaNguonVon == null || objThongTri.sMaNguonVon == "") {
        lstMessError.push("Chưa chọn nguồn vốn !");
    }
    if (objThongTri.iNamThongTri == null || objThongTri.iNamThongTri <= 0) {
        lstMessError.push("Chưa nhập năm thực hiện !");
    }
    if (objThongTri.iID_BCQuyetToanNienDo == null || objThongTri.iID_BCQuyetToanNienDo == "") {
        lstMessError.push("Chưa chọn chứng từ !");
    }
    if (objThongTri.sMaThongTri == null || objThongTri.sMaThongTri == "") {
        lstMessError.push("Chưa nhập mã thông tri !");
    }
    if (objThongTri.dNgayThongTri == null || objThongTri.dNgayThongTri == "") {
        lstMessError.push("Chưa nhập ngày tạo thông tri !");
    }
    if (objThongTri.sNguoiLap == null || objThongTri.sNguoiLap == "") {
        lstMessError.push("Chưa nhập người lập thông tri !");
    }
    if (lstThongTriDetail == null || lstThongTriDetail.length == 0) {
        lstMessError.push("Không có thông tin thông tri chi tiết !");
    }
    if (lstMessError.length == 0) return true;
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: 'Xảy ra lỗi khi lưu thông tri quyết toán', Messages: lstMessError, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
                return false;
            }
        });
}

// Helper
function fnGetThongTriInfo() {
    var obj = {};
    obj.iID_DonViID = $("#iID_DonViQuanLyID :selected").data("id");
    obj.iID_LoaiThongTriID = $("#iIdLoaiThongTri").val();
    obj.bThanhToan = true;
    obj.sMaNguonVon = $("#sMaNguonVon").val();
    obj.sMaThongTri = $("#sMaThongTri").val();
    obj.iID_BCQuyetToanNienDo = $("#sChungTu").val();
    obj.dNgayThongTri = $("#dNgayThongTri").val();
    obj.iNamThongTri = $("#sNamThongTri").val();
    obj.sNguoiLap = $("#sNguoiLap").val();
    obj.iID_LoaiThongTriID = $("#iIdLoaiThongTri").val();
    return obj;
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