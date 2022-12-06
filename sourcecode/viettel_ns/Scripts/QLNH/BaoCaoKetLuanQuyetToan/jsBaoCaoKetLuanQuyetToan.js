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
});

function ResetChangePage() {
    var dTuNgay = "";
    var dDenNgay = "";
    var iDonvi = GUID_EMPTY;
    GetListData(dTuNgay, dDenNgay, iDonvi);
}

function GetListData(dTuNgay, dDenNgay, iDonvi) {
    $.ajax({
        type: "POST",
        dataType: "html",
        async: false,
        url: "/QLNH/BaoCaoKetLuanQuyetToan/BaoCaoKetLuanQuyetToanSearch",
        data: { dTuNgay, dDenNgay, iDonvi },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtTuNgay").val(dTuNgay);
            $("#txtDenNgay").val(dDenNgay);
            $("#iDonvi").val(iDonvi);
        }
    });
}


function ChangePage() {
    var dTuNgay = $("#txtTuNgay").val();
    var dDenNgay = $("#txtDenNgay").val();
    var iDonvi = $("#iDonvi").val();
    GetListData(dTuNgay, dDenNgay, iDonvi);

}




