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
    var txtTieuDe1 = $.trim($("#txtTieuDe1").val());
    var txtTieuDe2 = $.trim($("#txtTieuDe2").val());
    var slbDonViVND = $("#slbDonViVND").val();
    var slbDonViUSD = $("#slbDonViUSD").val();
    var txtNamKeHoach = $("#txtNamKeHoach").val();
    var txtIdDonVi = $("#txtIdDonVi").val();
    var txtSTenDonVi = $("#txtSTenDonVi").val();
    var sTenDonViCapTren = $.trim($("#txtDonViCapTren").val());
    var sTenDonViCapDuoi = $.trim($("#txtDonViCapDuoi").val());

    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.slbDonViVND = slbDonViVND;
    data.slbDonViUSD = slbDonViUSD;
    data.txtNamKeHoach = txtNamKeHoach;
    data.txtIdDonVi = txtIdDonVi;
    data.txtSTenDonVi = txtSTenDonVi;
    data.sTenDonViCapTren = sTenDonViCapTren;
    data.sTenDonViCapDuoi = sTenDonViCapDuoi;


    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExportBCChiTiet").val() +
        "?ext=" + ext + "&txtTieuDe1=" + encodeURIComponent(txtTieuDe1)
        + "&txtTieuDe2=" + encodeURIComponent(txtTieuDe2)
        + "&slbDonViVND=" + slbDonViVND
        + "&slbDonViUSD=" + slbDonViUSD
        + "&txtNamKeHoach=" + txtNamKeHoach
        + "&txtIdDonVi=" + txtIdDonVi
        + "&txtSTenDonVi=" + txtSTenDonVi
        + "&sTenDonViCapTren=" + encodeURIComponent(sTenDonViCapTren)
        + "&sTenDonViCapDuoi=" + encodeURIComponent(sTenDonViCapDuoi)


    //url = unescape(url);
    links.push(url);

    openLinks(links);
}
function ValidateDataPrint(data) {
    var Title = 'Lỗi in báo cáo quyết toán niên độ';
    var Messages = [];

    if (data.txtTieuDe1 == null || data.txtTieuDe1 == "") {
        Messages.push("Chưa có thông tin về tiêu đề 1!");
    }
    if (data.txtTieuDe2 == null || data.txtTieuDe2 == "") {
        Messages.push("Chưa có thông tin về tiêu đề 2!");
    }
    if (data.slbDonViVND == null || data.slbDonViVND == 0) {
        Messages.push("Chưa chọn đơn vị VND!");
    }
    if (data.slbDonViUSD == null || data.slbDonViUSD == 0) {
        Messages.push("Chưa chọn đơn vị USD!");
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
                $("#modalBCSoNamSau").modal("show")
                $("#contentModalBaoCaoSoNamSau").empty().html(data);
                $("#modalBCSoNamSauLabel").empty().html('Báo cáo chi tiết số chuyển năm sau');
            }
        });
    }

}
