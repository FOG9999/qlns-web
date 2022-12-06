var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var TBL_DANH_SACH = "tblDanhSachQLHoachChiQuy";

$(document).ready(function () {
    $("#drpDonViQuanLy, #txtNamKeHoach, #drpQuy, #drpNguonNganSach").change(function (e) {
        GetKinhPhiCucTaiChinhCap();
        $("#" + TBL_DANH_SACH + " tbody").html("");
    });
    $('.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        autoclose: true,
        language: 'vi',
        todayHighlight: true,
        format: "dd/mm/yyyy"
    }).on('hide', () => {
        isShowing = false;
    }).on('show', () => {
        isShowing = true;
    });

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

    GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, EventValidate);
}

// event
function EventValidate() {
    $("td.sotien[contenteditable='true']").on("keypress", function (event) {
        return ValidateNumberKeyPress(this, event);
    })
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

function GetNhuCauChiChiTiet(iID_DonViQuanLyID, iNamKeHoach, iQuy, iIDNguonVon, callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachChiQuy/GetNhuCauChiChiTiet",
        data: { iIdDonVi: iID_DonViQuanLyID, iNamKeHoach: iNamKeHoach, iQuy: iQuy, iIDNguonVon: iIDNguonVon },
        success: function (data) {
            $("#" + TBL_DANH_SACH + " tbody").html(data);
            if (callback)
                callback();
        }
    });
}

function Insert() {
    if (!ValidateDataInsert()) return;
    LuuKeHoachChiQuy();
}

function LuuKeHoachChiQuy() {
    var data = {};
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iQuy = $("#drpQuy option:selected").val();
    data.iID_NguonVonID = $("#drpNguonNganSach option:selected").val();
    data.sNguoiLap = $("#txtNguoiLap").val();

    var dataChiTiet = GetListChiTiet();

    $.ajax({
        type: "POST",
        url: "/QLKeHoachChiQuy/LuuKeHoachChiQuy",
        data: {
            data: data,
            lstChiTiet: dataChiTiet
        },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLKeHoachChiQuy/Index";
            } else {
                alert("Tạo kế hoạch chi quý thất bại !");
            }
        }
    });
}

function GetListChiTiet() {
    var lstData = [];
    $("#" + TBL_DANH_SACH + " tbody tr").each(function () {
        var iID_DuAnId = $(this).find(".r_iID_DuAnID").val();
        var iID_LoaiCongTrinhId = $(this).find(".r_iID_LoaiCongTrinhId").val();
        var fGiaTriDeNghi = parseInt($(this).find(".r_sGiaTriDeNghi").val() == "" ? 0 : UnFormatNumber($(this).find(".r_sGiaTriDeNghi").val()));
        var sGhiChu = $(this).find(".r_sGhiChu").text();
        var sLoaiThanhToan = $(this).find(".r_sLoaiThanhToan").text();

        if ($(this).find(".r_sGiaTriDeNghi").val() != "" && fGiaTriDeNghi > 0) {
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
    var isAdd = true;
    var iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iQuy = $("#drpQuy option:selected").val();
    var iIDNguonVon = $("#drpNguonNganSach option:selected").val();
     var nameDonViQL = $(`option[value = "${iID_DonViQuanLyID}"]`).get()[0].outerText
     var sSoDeNghi = $("#txtSoDeNghi").val().trim();
     var sNguoiLap = $("#txtNguoiLap").val().trim();
    if ($("#drpDonViQuanLy").val() == null || $("#drpDonViQuanLy").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập đơn vị quản lý.");
    }
     if ($("#txtSoDeNghi").val().trim() == "") {
         sMessError.push("Chưa nhập số đề nghị.");
     } else {
         if (sSoDeNghi.length > 50) {
             sMessError.push("Số đề nghị không được quá 50 ký tự.");
         }
     }

     if (sNguoiLap.length > 255) {
         sMessError.push("Người lập không được quá 255 ký tự.");
     }

    if ($("#txtNgayDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập ngày đề nghị.");
    }
    if ($("#txtNamKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập năm kế hoạch.");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn.");
    }
    if ($("#drpQuy").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn.");
    }
    if ($("#" + TBL_DANH_SACH + " tbody tr").length == 0) {
        sMessError.push("Chưa nhập quản lý hoạch chi Quý !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    } else {
        $.ajax({
            type: "POST",
            //contentType:"json",
            url: "/QLVonDauTu/QLKeHoachChiQuy/ValidationKHChiQuy",
            async: false, 
            data: {
                iNamKeHoach: iNamKeHoach, iIdDonVi: iID_DonViQuanLyID,
                iIdNguonVon: iIDNguonVon, iQuy: iQuy,
                sSoDeNghi: sSoDeNghi, sNguoiLap: sNguoiLap
            },
            success: function (r) {
                if (!r.bIsComplete) {
                    if (r.sMessError == undefined || r.sMessError == null || r.sMessError == "") {                    
                        sMessError.push(`Đơn vị ${nameDonViQL} đã có kế hoạch chi quý năm ${iNamKeHoach}!`);
                        alert(sMessError.join('\n'));
                        isAdd = false;
                    } else {
                        alert(r.sMessError);
                        isAdd = false;
                    }
                }
            }
        });
    }
    if (!isAdd) {
        return false;
    } 
    return true;
  
    
}
 
//function ValidationDataInToDB() {
//    var iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
//    var iNamKeHoach = $("#txtNamKeHoach").val();
//    var iQuy = $("#drpQuy option:selected").val();
//    var iIDNguonVon = $("#drpNguonNganSach option:selected").val();
//    $.ajax({
//        type: "POST",
//        url: "/QLKeHoachChiQuy/ValidattionKHChiQuy",
//        data: {
//            iNamKeHoach: iNamKeHoach, iIdDonVi: iID_DonViQuanLyID,
//            iIdNguonVon: iIDNguonVon, iQuy: iQuy           
//        },
//        success: function (r) {
//            if (!r)
//            {
//                alert("Tạo kế hoạch chi quý thất bại !");
//            }
//        }
//    });
//}

function Xoa() {
    $("#drpDonViQuanLy").prop("selectedIndex", 0);
    $("#txtSoDeNghi").val("");
    $("#txtNgayDeNghi").val("");
    $("#txtNamKeHoach").val("");
    $("#drpQuy").prop("selectedIndex", 0);
    $("#drpNguonNganSach").val("");
    $("#txtNguoiLap").val("");
}