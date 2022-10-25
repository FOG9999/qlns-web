var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
function ResetChangePage(iCurrentPage = 1) {
    GetListData(GUID_EMPTY, 0);
}
function ChangePage() {
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    var iDonVi = $("#iDonViFillter").val();

    var Title = 'Lỗi tìm kiếm báo cáo';
    var Messages = [];
    if (iDonVi == null || iDonVi == GUID_EMPTY) {
        Messages.push("Đơn vị  chưa chọn !");
    }

    if (iNamKeHoach == null || iNamKeHoach == 0) {
        Messages.push("Năm kế hoạch chưa chọn !");
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
    } else {
        GetListData(iDonVi, iNamKeHoach);
    }
}
function GetListData(iDonVi, iNamKeHoach) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iDonVi: iDonVi, iNamKeHoach: iNamKeHoach },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#iDonViFillter").val(iDonVi);
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
    var txtIdDonVi = $("#txtIdDonVi").val();
    var txtSTenDonVi = $("#txtSTenDonVi").val();

    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.slbDonViVND = slbDonViVND;
    data.slbDonViUSD = slbDonViUSD;
    data.txtNamKeHoach = txtNamKeHoach;
    data.txtIdDonVi = txtIdDonVi;
    data.txtSTenDonVi = txtSTenDonVi;


    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExportBCChiTiet").val() +
        "?ext=" + ext + "&txtTieuDe1=" + txtTieuDe1
        + "&txtTieuDe2=" + txtTieuDe2
        + "&slbDonViVND=" + slbDonViVND
        + "&slbDonViUSD=" + slbDonViUSD
        + "&txtNamKeHoach=" + txtNamKeHoach
        + "&txtIdDonVi=" + txtIdDonVi
        + "&txtSTenDonVi=" + txtSTenDonVi


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
    var slbDonVi = $("#iDonViFillter").val();
    var slbNamKeHoach = $("#slbNamKeHoachFillter").val();

    if (slbDonVi == GUID_EMPTY && slbNamKeHoach==0) {
        var Title = 'Vui lòng chọn năm kế hoạch và đơn vị';
        var Error = "Bạn phải chọn năm kế hoạch và đơn vị mới được In báo cáo!"
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
            url: "/QLNH/BaoCaoSoChuyenNamSau/GetModalInBaoCao",
            data: { slbDonVi: slbDonVi, slbNamKeHoach: slbNamKeHoach },
            success: function (data) {
                console.log(data)
                $("#modalBCSoNamSau").modal("show")
                $("#contentModalBaoCaoSoNamSau").empty().html(data);
                $("#modalBCSoNamSauLabel").empty().html('Báo cáo chi tiết số chuyển năm sau');
            }
        });
    }

}
