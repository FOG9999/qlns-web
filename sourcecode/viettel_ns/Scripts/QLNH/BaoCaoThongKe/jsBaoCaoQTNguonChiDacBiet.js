var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

// Action button làm mới
function ResetChangePage(iCurrentPage = 1) {
    GetListData("");
}

// Action changepage
function ChangePage() {
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    if (iNamKeHoach == "") {
        PopupError();
        return false;
    }
    GetListData(iNamKeHoach);
}

// Action tìm kiếm
function GetListData(iNamKeHoach) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iNamKeHoach: iNamKeHoach },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#slbNamKeHoachFillter").val(iNamKeHoach);
        }
    });
}

// Xuất báo cáo
//function ExportQTNguonChiDacBiet(fileType) {
//    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
//    if (iNamKeHoach == "") {
//        PopupError();
//        return false;
//    }
//    var url = "/QLNH/BaoCaoQuyetToanNguonChiDacBiet/ExportQuyetToanNguonChiDacBiet?iNamKeHoach=" + iNamKeHoach + "&ext=" + fileType;
//    var arrLink = [];
//    arrLink.push(url);
//    openLinks(arrLink);
//}

// Action in báo cáo
function printBaoCao(ext) {
    var links = [];
    var url = "";
    var txtTieuDe1 = $.trim($("#txtTieuDe1").val());
    var txtTieuDe2 = $.trim($("#txtTieuDe2").val());
    var txtNamKeHoach = $("#txtNamKeHoach").val();
    var sTenDonViCapTren = $.trim($("#txtDonViCapTren").val());
    var sTenDonViCapDuoi = $.trim($("#txtDonViCapDuoi").val());

    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.txtNamKeHoach = txtNamKeHoach;
    data.sTenDonViCapTren = sTenDonViCapTren;
    data.sTenDonViCapDuoi = sTenDonViCapDuoi;


    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExportBCChiTiet").val() +
        "?ext=" + ext + "&txtTieuDe1=" + encodeURIComponent(txtTieuDe1)
        + "&txtTieuDe2=" + encodeURIComponent(txtTieuDe2)
        + "&txtNamKeHoach=" + txtNamKeHoach
        + "&sTenDonViCapTren=" + encodeURIComponent(sTenDonViCapTren)
        + "&sTenDonViCapDuoi=" + encodeURIComponent(sTenDonViCapDuoi)

    //url = unescape(url);
    links.push(url);

    openLinks(links);
}

// Validate data print
function ValidateDataPrint(data) {
    var Title = 'Lỗi in báo cáo quyết toán thuộc các nguồn chi đặc biệt';
    var Messages = [];

    if (data.txtTieuDe1 == null || data.txtTieuDe1 == "") {
        Messages.push("Chưa có thông tin về tiêu đề 1!");
    }
    if (data.txtTieuDe2 == null || data.txtTieuDe2 == "") {
        Messages.push("Chưa có thông tin về tiêu đề 2!");
    }
    if ($.trim(data.sTenDonViCapTren) == "") {
        Messages.push("Chưa nhập đơn vị cấp trên!");
    }
    if ($.trim(data.sTenDonViCapDuoi) == "") {
        Messages.push("Chưa nhập đơn vị!");
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

// Show popup error
function PopupError() {
    var Title = 'Thông báo';
    var messErr = ["Vui lòng chọn năm!"];
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: messErr, Category: ERROR },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

// Show modal config chữ ký, tiêu đề, đơn vị
function InBaoCaoModal() {
    var slbNamKeHoach = $.trim($("#slbNamKeHoachFillter").val());

    if (slbNamKeHoach == "") {
        var Title = 'Thông báo';
        var Error = "Bạn phải chọn năm kế hoạch mới được In báo cáo!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else {
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLNH/BaoCaoQuyetToanNguonChiDacBiet/GetModalInBaoCao",
            data: { slbNamKeHoach: slbNamKeHoach },
            success: function (data) {
                $("#modalBCQTNguonChi").modal("show")
                $("#contentModalBCQTNguonChi").empty().html(data);
                $("#modalBCQTNguonChiLabel").empty().html('Báo cáo quyết toán thuộc các nguồn chi đặc biệt');
            }
        });
    }

}