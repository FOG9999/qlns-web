var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_NCCQCT = "tblThucHienNganSachChiTiet";
var arrChiQuy = [];
var data = [];
var arrDonvi = [];
var arrBQuanly = [];

$(document).ready(function ($) {
    $("#IDTable").val("1");
    ChangeVoucher();
   
});

function ResetChangePage() {
    var tabTable = $("#tabTable").val();
    var iTuNam = new Date().getFullYear();
    var iDenNam = new Date().getFullYear();
    var iNam = new Date().getFullYear();
    var iDonvi = GUID_EMPTY;
    var iQuyList = 0;
    ChangeVoucher();
    GetListData(tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam);
}

function GetListData(tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam) {
    var data = {};
    data.tabTable = tabTable;
    data.iTuNam = iTuNam;
    data.iDenNam = iDenNam;
    data.iDonvi = iDonvi;
    data.iQuyList = iQuyList
    data.iNam = iNam;
    $.ajax({
        type: "POST",
        dataType: "html",
        async: false,
        url: "/QLNH/ThucHienNganSach/ThucHienNganSachSearch",
        data: { tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#tabTable").val(tabTable);
            $("#txtTuNam").val(iTuNam);
            $("#txtDenNam").val(iDenNam);
            $("#iDonvi").val(iDonvi);
            $("#iQuyList").val(iQuyList);
            $("#txtNam").val(iNam);
        }
    });
    ChangeVoucher();
}


function ChangePage() {
    var tabTable = $("#tabTable").val();
    var iTuNam = $("#txtTuNam").val();
    var iDenNam = $("#txtDenNam").val();
    var iDonvi = $("#iDonvi").val();
    var iQuyList = $("#iQuyList").val();
    var iNam = $("#txtNam").val();
    GetListData(tabTable, iTuNam, iDenNam, iDonvi, iQuyList, iNam);

}

function ChangeVoucher() {
    var value = $("#tabTable").val();
    if (value == "2") {
        $("#tblThucHienNganSachGiaiDoan").css("display", "");
        $("#SearchThucHienNganSachGiaiDoan").css("display", "");
        $("#SearchThucHienNganSach").css("display", "none");
        $("#tblThucHienNganSach").css("display", "none");
    } else {
        $("#tblThucHienNganSach").css("display", "");
        $("#tblThucHienNganSachGiaiDoan").css("display", "none");
        $("#SearchThucHienNganSachGiaiDoan").css("display", "none");
        $("#SearchThucHienNganSach").css("display", "");
    }
}

function printBaoCao(ext) {
    var links = [];
    var linksToHai = [];
    var url = "";
    var txtTieuDe1 = $.trim($("#txtTieuDe1").val());
    var txtTieuDe2 = $.trim($("#txtTieuDe2").val());
    var slbDonViVND = $("#slbDonViVND").val();
    var slbDonViUSD = $("#slbDonViUSD").val();
    var sTenDonViCapTren = $.trim($("#txtDonViCapTren").val());
    var sTenDonViCapDuoi = $.trim($("#txtDonViCapDuoi").val());
    var sTenDonVi = $("#txtPrintsTenDonVi").val();
    var iInMotTo = 0;
    if ($('#chkInToMot').is(":checked")) {
        iInMotTo += 1;
    }
    if ($('#chkInToHai').is(":checked")) {
        iInMotTo += 2;
    }
    var tabTable = $("#txtPrinttabTable").val();
    var iTuNam = $("#txtPrintiTuNam").val();
    var iDenNam = $("#txtPrintiDenNam").val();
    var iDonvi = $("#txtPrintiDonvi").val();
    var iQuyList = $("#txtPrintiQuyList").val();
    var iNam = $("#txtPrintiNam").val();


    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.slbDonViVND = slbDonViVND;
    data.slbDonViUSD = slbDonViUSD;
    data.sTenDonViCapTren = sTenDonViCapTren;
    data.sTenDonViCapDuoi = sTenDonViCapDuoi;
    data.iInMotTo = iInMotTo;

    data.sTenDonVi = sTenDonVi;
    data.tabTable = tabTable;
    data.iTuNam = iTuNam;
    data.iDenNam = iDenNam;
    data.iDonvi = iDonvi;
    data.iQuyList = iQuyList;
    data.iNam = iNam;



    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExport").val() +
        "?ext=" + ext + "&dvt = 1"
        + "&txtTieuDe1=" + encodeURIComponent(txtTieuDe1)
        + "&txtTieuDe2=" + encodeURIComponent(txtTieuDe2)
        + "&slbDonViVND=" + slbDonViVND
        + "&slbDonViUSD=" + slbDonViUSD
        + "&sTenDonViCapTren=" + encodeURIComponent(sTenDonViCapTren)
        + "&sTenDonViCapDuoi=" + encodeURIComponent(sTenDonViCapDuoi)
        + "&iInMotTo=" + iInMotTo
        + "&sTenDonVi=" + sTenDonVi
        + "&tabTable=" + tabTable
        + "&iTuNam=" + iTuNam
        + "&iDenNam=" + iDenNam
        + "&iDonvi=" + iDonvi
        + "&iQuyList=" + iQuyList
        + "&iNam=" + iNam
        + "&InToHai=1"


    //url = unescape(url);
    links.push(url);

    openLinks(links);

    if (iInMotTo == 3) {
        urlToHai = $("#urlExport").val() +
            "?ext=" + ext + "&dvt = 1"
            + "&txtTieuDe1=" + encodeURIComponent(txtTieuDe1)
            + "&txtTieuDe2=" + encodeURIComponent(txtTieuDe2)
            + "&slbDonViVND=" + slbDonViVND
            + "&slbDonViUSD=" + slbDonViUSD
            + "&sTenDonViCapTren=" + encodeURIComponent(sTenDonViCapTren)
            + "&sTenDonViCapDuoi=" + encodeURIComponent(sTenDonViCapDuoi)
            + "&iInMotTo=" + iInMotTo
            + "&sTenDonVi=" + sTenDonVi
            + "&tabTable=" + tabTable
            + "&iTuNam=" + iTuNam
            + "&iDenNam=" + iDenNam
            + "&iDonvi=" + iDonvi
            + "&iQuyList=" + iQuyList
            + "&iNam=" + iNam
            + "&InToHai=2"

        linksToHai.push(urlToHai);

        openLinks(linksToHai);
    }
}
function ValidateDataPrint(data) {
    var Title = 'Lỗi in báo cáo tình hình thực hiện ngân sách';
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
    if (data.iInMotTo == 0 && data.tabTable == 2) {
        Messages.push("Chưa chọn số tờ!");
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
    var tabTable = $("#tabTable").val();
    var iTuNam = $("#txtTuNam").val();
    var iDenNam = $("#txtDenNam").val();
    var iDonvi = $("#iDonvi").val();
    var iQuyList = $("#iQuyList").val();
    var iNam = $("#txtNam").val();
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ThucHienNganSach/GetModalInBaoCao",
        data: { tabTable: tabTable, iTuNam: iTuNam, iDenNam: iDenNam, iDonvi: iDonvi, iQuyList: iQuyList, iNam: iNam },
        success: function (data) {
            $("#modalThucHienNganSach").modal("show")
            $("#contentModalThucHienNganSach").empty().html(data);
            $("#modalThucHienNganSachLabel").empty().html('Báo cáo tình hình thực hiện ngân sách');
        }
    });

}




