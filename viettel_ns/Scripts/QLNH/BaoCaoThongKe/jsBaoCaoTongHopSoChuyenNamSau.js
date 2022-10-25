    var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
function ResetChangePage(iCurrentPage = 1) {
    GetListData( 0);
}
function ChangePage() {
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    GetListData( iNamKeHoach);
}
function GetListData(iNamKeHoach) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {  iNamKeHoach: iNamKeHoach },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#slbNamKeHoachFillter").val(iNamKeHoach);
        }
    });
}
function printBaoCao(ext) {
    var links = [];
    var url = "";
    var txtTieuDe1 = $("#txtTieuDe1").val();
    var txtTieuDe2 = $("#txtTieuDe2").val();
    var slbDonViVND = $("#slbDonViVND").val();
    var slbDonViUSD = $("#slbDonViUSD").val();
    var txtNamKeHoach = $("#txtNamKeHoach").val();

    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.slbDonViVND = slbDonViVND;
    data.slbDonViUSD = slbDonViUSD;

    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExportBCChiTiet").val() +
        "?ext=" + ext + "&txtTieuDe1=" + txtTieuDe1
        + "&txtTieuDe2=" + txtTieuDe2
        + "&slbDonViVND=" + slbDonViVND
        + "&slbDonViUSD=" + slbDonViUSD
        + "&txtNamKeHoach=" + txtNamKeHoach;

    url = unescape(url);
    links.push(url);

    openLinks(links);
}
function ValidateDataPrint(data) {
    var Title = 'Lỗi in báo cáo quyết toán niên độ';
    var Messages = [];

    if (data.txtTieuDe1 == null || data.txtTieuDe1 == "") {
        Messages.push("Tiêu đề 1 chưa nhập !");
    }
    if (data.txtTieuDe2 == null || data.txtTieuDe2 == "") {
        Messages.push("Tiêu đề 2 chưa nhập !");
    }
    if (data.slbDonViVND == null || data.slbDonViVND == 0) {
        Messages.push("Đơn vị VND chưa chọn !");
    }
    if (data.slbDonViUSD == null || data.slbDonViUSD == 0) {
        Messages.push("Đơn vị USD chưa chọn !");
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
function InBaoCaoModal() {
    var slbNamKeHoach = $("#slbNamKeHoachFillter").val();
    //alert error
    if (slbNamKeHoach == 0) {
        var Title = 'Vui lòng chọn năm kế hoạch';
        var Error = "Bạn phải chọn 1 năm kế hoạch mới được In báo cáo!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else {
        //show modal tong hop
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLNH/BaoCaoTongHopSoChuyenNamSau/GetModalInBaoCao",
            data: { slbNamKeHoach: slbNamKeHoach  },
            success: function (data) {
                $("#modalBCTHSoChuyenNamSau").modal("show")
                $("#contentModalBCTHSoChuyenNamSau").empty().html(data);
                $("#modalBCTHSoChuyenNamSauLabel").empty().html('Báo cáo tổng hợp số chuyển năm sau');
            }
        });
    }

}
