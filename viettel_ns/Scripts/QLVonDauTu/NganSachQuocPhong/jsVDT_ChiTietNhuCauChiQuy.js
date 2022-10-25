var TBL_DANH_SACH = "tblDanhSachQLHoachChiQuy";

$(document).ready(function () {
    var iIDNhuCauChiID = $("#iID_NhuCauChiID").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    var iQuy = $("#iQuy").val();
    var iIDNguonVon = $("#iIDNguonVon").val();

    GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, iIDNhuCauChiID, function () {
        $("[contenteditable=true]").removeAttr('contenteditable');
    });
    GetKinhPhiCucTaiChinhCap(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon)
});

function CancelSaveData() {
    location.href = "/QLVonDauTu/QLKeHoachChiQuy";
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