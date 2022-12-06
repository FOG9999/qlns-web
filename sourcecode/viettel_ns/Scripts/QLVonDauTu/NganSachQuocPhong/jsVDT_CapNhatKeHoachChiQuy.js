var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var TBL_DANH_SACH = "tblDanhSachQLHoachChiQuy";

var iIDNhuCauChiID = $("#iID_NhuCauChiID").val();
var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
var iNamKeHoach = $("#iNamKeHoach").val();
var iQuy = $("#iQuy").val();
var iIDNguonVon = $("#iIDNguonVon").val();

$(document).ready(function () {
    $("#drpDonViQuanLy").val(iID_DonViQuanLyID);
    $("#drpQuy").val(iQuy);
    $("#drpNguonNganSach").val(iIDNguonVon);

    GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, iIDNhuCauChiID, EventValidate);

    $("#drpDonViQuanLy, #txtNamKeHoach, #drpQuy, #drpNguonNganSach").change(function (e) {
        GetKinhPhiCucTaiChinhCap();
        $("#" + TBL_DANH_SACH + " tbody").html("");
    });
    EventValidate();
});

function GetKinhPhiCucTaiChinhCap() {
    $("#txtQuyTruocChuaGiaiNgan").val("");
    $("#txtQuyNayDuocCap").val("");
    $("#txtKinhPhiThucHienGiaiNganQuyNay").val("");
    $("#txtSoKinhPhiChuaGiaiNganChuyenQuySau").val("");
    $("#txtSoKinhPhiDeNghiCapQuyToi").val("");

    var iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iQuy = $("#drpQuy option:selected").val();
    var iIDNguonVon = $("#drpNguonNganSach option:selected").val();

    if (iID_DonViQuanLyID == undefined || iID_DonViQuanLyID == GUID_EMPTY
        || iNamKeHoach == ""
        || iQuy == undefined || iQuy == GUID_EMPTY
        || iIDNguonVon == undefined || iIDNguonVon == GUID_EMPTY)
        return false;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachChiQuy/GetKinhPhiCucTaiChinhCap",
        data: {
            iNamKeHoach: iNamKeHoach,
            iIdDonVi: iID_DonViQuanLyID,
            iIdNguonVon: iIDNguonVon,
            iQuy: iQuy
        },
        success: function (r) {
            if (r.data != null) {
                $("#txtQuyTruocChuaGiaiNgan").val(r.data.sQuyTruocChuaGiaiNgan);
                $("#txtQuyNayDuocCap").val(r.data.sQuyNayDuocCap);
                $("#txtKinhPhiThucHienGiaiNganQuyNay").val(r.data.sGiaiNganQuyNay);
                $("#txtSoKinhPhiChuaGiaiNganChuyenQuySau").val(r.data.sChuaGiaiNganChuyenQuySau);

                //$("#txtSoKinhPhiDeNghiCapQuyToi").val();
            }
        }
    });
}

//===================== Event button ==========================//

function OnKeyupValidateNumber(field) {
    var value = $(field).html();
    if (IsNumber(value)) {
        $(field).html(FormatNumber($(field).html() == "" ? 0 : UnFormatNumber($(field).html())));
    }

}

function KeyPressValidateNumber() {
    $(".sotien").keypress(function (e) {
        var x = event.charCode || event.keyCode;
        if (isNaN(String.fromCharCode(e.which)) && x != 46 || x === 32 || x === 13 || (x === 46 && event.currentTarget.innerText.includes('.'))) e.preventDefault();
    });
}

function CancelSaveData() {
    location.href = "/QLVonDauTu/QLKeHoachChiQuy";
}

function Loc() {
    var iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iQuy = $("#drpQuy option:selected").val();
    var iIDNguonVon = $("#drpNguonNganSach option:selected").val();

    if (iID_DonViQuanLyID == "" || iID_DonViQuanLyID == GUID_EMPTY) {
        alert("Thông tin đơn vị quản lý chưa có hoặc chưa chính xác");
        return;
    }

    if (iNamKeHoach == "") {
        alert("Chưa nhập năm thực hiện");
        return;
    }

    if (iQuy == "" || iQuy == GUID_EMPTY) {
        alert("Thông tin quý chưa có hoặc chưa chính xác");
        return;
    }

    if (iIDNguonVon == "" || iIDNguonVon == 0) {
        alert("Thông tin nguồn vốn chưa có hoặc chưa chính xác");
        return;
    }

    GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, iIDNhuCauChiID, EventValidate);
}

// event
function EventValidate() {


    //$("td.sotien[contenteditable='true']").on("keypress", function (event) {
    //    $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));
    //})


    $("td.sotien[contenteditable='true']").on("focusout", function (event) {
        $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));
    })

    $("td[contenteditable='true']").on("keydown", function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 13) {
            $(this).blur();
        }
    });
}

function Update() {
    if (!ValidateDataInsert()) return;
    LuuKeHoachChiQuy();
}

function LuuKeHoachChiQuy() {
    var data = {};
    data.Id = iIDNhuCauChiID;
    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.sNguoiLap = $("#txtNguoiLap").val();

    var dataChiTiet = GetListChiTiet();

    $.ajax({
        type: "POST",
        url: "/QLKeHoachChiQuy/LuuKeHoachChiQuy",
        data: {
            data: data,
            lstChiTiet: dataChiTiet,
            isUpdate: true
        },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLKeHoachChiQuy/Index";
            } else {
                alert("Cập nhật kế hoạch chi quý thất bại !");
            }
        }
    });
}

function GetListChiTiet() {
    var lstData = [];
    $("#" + TBL_DANH_SACH + " tbody tr").each(function () {
        var iID_DuAnId = $(this).find(".r_iID_DuAnID").val();
        var iID_LoaiCongTrinhId = $(this).find(".r_iID_LoaiCongTrinhId").val();
        var fGiaTriDeNghi = parseInt($(this).find(".r_sGiaTriDeNghi").val()??'' == "" ? 0 : UnFormatNumber($(this).find(".r_sGiaTriDeNghi").val()??''));
        var sGhiChu = $(this).find(".r_sGhiChu").text();
        var sLoaiThanhToan = $(this).find(".r_sLoaiThanhToan").text();

        if ($(this).find(".r_sGiaTriDeNghi").val()??'' != "" && fGiaTriDeNghi > 0) {
            lstData.push({
                iID_DuAnId: iID_DuAnId,
                iID_LoaiCongTrinhId: iID_LoaiCongTrinhId,
                fGiaTriDeNghi: fGiaTriDeNghi,
                sGhiChu: sGhiChu,
                sLoaiThanhToan: sLoaiThanhToan,
            })
        }
    })
    return lstData;
}

//=========================== validate ===========//
function ValidateDataInsert() {
    var sMessError = [];

    if ($("#txtSoDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập số đề nghị.");
    }

    if ($("#" + TBL_DANH_SACH + " tbody tr").length == 0) {
        sMessError.push("Chưa nhập quản lý hoạch chi Quý !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}

function Xoa() {
    $("#drpDonViQuanLy").prop("selectedIndex", 0);
    $("#txtSoDeNghi").val("");
    $("#txtNgayDeNghi").val("");
    $("#txtNamKeHoach").val("");
    $("#drpQuy").prop("selectedIndex", 0);
    $("#drpNguonNganSach").val("");
    $("#txtNguoiLap").val("");
}

function GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, iIDNhuCauChiID, callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachChiQuy/GetNhuCauChiChiTiet",
        data: { iIdDonVi: iID_DonViQuanLyID, iNamKeHoach: iNamKeHoach, iQuy: iQuy, iIDNguonVon: iIDNguonVon, iIDNhuCauChiID: iIDNhuCauChiID },
        success: function (data) {
            $("#" + TBL_DANH_SACH + " tbody").html(data);
            if (callback)
                callback();
        }
    });
}

function GetKinhPhiCucTaiChinhCap(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon) {
    if (iID_DonViQuanLyID == undefined || iID_DonViQuanLyID != GUID_EMPTY
        || iNamKeHoach == ""
        || iQuy == undefined || iQuy == GUID_EMPTY
        || iIDNguonVon == undefined || iIDNguonVon == GUID_EMPTY)
        return false;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachChiQuy/GetKinhPhiCucTaiChinhCap",
        data: {
            iNamKeHoach: iNamKeHoach,
            iIdDonVi: iID_DonViQuanLyID,
            iIdNguonVon: iIDNguonVon,
            iQuy: iQuy
        },
        success: function (r) {
            if (r.data != null) {
                $("#txtQuyTruocChuaGiaiNgan").text(r.data.sQuyTruocChuaGiaiNgan);
                $("#txtQuyNayDuocCap").text(r.data.sQuyNayDuocCap);
                $("#txtKinhPhiThucHienGiaiNganQuyNay").text(r.data.sGiaiNganQuyNay);
                $("#txtSoKinhPhiChuaGiaiNganChuyenQuySau").text(r.data.sChuaGiaiNganChuyenQuySau);

                //$("#txtSoKinhPhiDeNghiCapQuyToi").val();
            }
        }
    });
}