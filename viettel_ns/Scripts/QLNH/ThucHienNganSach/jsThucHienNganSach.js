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




