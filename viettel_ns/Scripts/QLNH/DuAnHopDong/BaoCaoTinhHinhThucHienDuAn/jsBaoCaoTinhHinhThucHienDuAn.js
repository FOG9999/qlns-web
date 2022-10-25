var CONFIRM = 0;
var ERROR = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

function ResetChangePage(iCurrentPage = 1) {
    var iID_DonViID = GUID_EMPTY;
    var iID_DuAnID = GUID_EMPTY;
    var dBatDau = null;
    var dKetThuc = null;
    GetListData(iID_DonViID, iID_DuAnID, dBatDau, dKetThuc, iCurrentPage, true);
}

function ChangePage(iCurrentPage = 1) {
    var iID_DonViID = $("#slbDonVi").val();
    var iID_DuAnID = $("#slbDuAn").val();
    var dBatDau = $("<div/>").text($.trim($("#txtdBatDau").val())).html();
    var dKetThuc = $("<div/>").text($.trim($("#txtdKetThuc").val())).html();
    GetListData(iID_DonViID, iID_DuAnID, dBatDau, dKetThuc, iCurrentPage, false);
}

function GetListData(iID_DonViID, iID_DuAnID, dBatDau, dKetThuc, iCurrentPage, isRefresh) {
    if (!isRefresh) {
        if (!ValidateData()) {
            return false;
        }
    }
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/BaoCaoTinhHinhThucHienDuAn/SearchBaoBaoThucHienDuAn",
        data: { _paging: _paging, iID_DonViID: iID_DonViID, iID_DuAnID: iID_DuAnID, dBatDau: dBatDau, dKetThuc: dKetThuc },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#slbDonVi").val(iID_DonViID);
            $("#slbDuAn").val(iID_DuAnID);
            $("#txtdBatDau").val($("<div/>").html(dBatDau).text());
            $("#txtdKetThuc").val($("<div/>").html(dKetThuc).text());
        }
    });
}

function ExportExcel(ext) {
    if (!ValidateData()) {
        return false;
    }
    var iID_DuAnID = $("#slbDuAn").val();
    var dBatDau = encodeURIComponent($.trim($("#txtdBatDau").val()));
    var dKetThuc = encodeURIComponent($.trim($("#txtdKetThuc").val()));

    window.open("/BaoCaoTinhHinhThucHienDuAn/ExportGiayDeNghiThanhToan/?ext=" + ext + "&iID_DuAnID=" + iID_DuAnID + "&dBatDau=" + dBatDau + "&dKetThuc=" + dKetThuc, "_blank");
    return false;
}

function GetDuAnByDonVi() {
    var idDonVi = $("#slbDonVi").val();
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/BaoCaoTinhHinhThucHienDuAn/GetDuAnByDonVi",
        data: { idDonVi: idDonVi },
        dataType: "json",
        success: function (result) {
            if (result && result.status == true) {
                var str = "<option value='" + GUID_EMPTY + "'>--Chọn dự án--</option>";
                $.each(result.data, function (index, val) {
                    str = str + "<option  value = '" + val.ID + "'>" + $("<div/>").text(val.sTenDuAn).html() + "</option>";
                });
                $("#slbDuAn").empty().append(str);
            }
        }
    });
}
function ValidateData() {
    var Title = 'Lỗi chọn dữ liệu';
    var Messages = [];

    var idDonVi = $("#slbDonVi").val();
    if (idDonVi == "" || idDonVi == GUID_EMPTY) {
        Messages.push("Vui lòng chọn đơn vị !");
    }
    var idDuAn = $("#slbDuAn").val();
    if (idDuAn == "" || idDuAn == GUID_EMPTY) {
        Messages.push("Vui lòng chọn dự án !");
    }
    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }
    return true;
}