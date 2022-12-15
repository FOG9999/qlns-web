var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function () {
    $("#txtSoHopDong").keyup(function (event) {
        ValidateMaxLength(this, 50);
    });
    $("#txtGiaHopDongTu").blur(function (event) {
        ValidateInputFocusOut(event, this, 5)
    }).keydown(function (event) {
        ValidateInputKeydown(event, this, 2);
    })
});

function ChangePage(iCurrentPage = 1) {
    var sSoHopDong = $("#txtSoHopDong").val();
    var fTienHopDongTu = $("#txtGiaHopDongTu").val() == "" ? "" : parseFloat(UnFormatNumber($("#txtGiaHopDongTu").val()));
    var fTienHopDongDen = $("#txtGiaHopDongDen").val() == "" ? "" : parseFloat(UnFormatNumber($("#txtGiaHopDongDen").val()));
    var dHopDongTuNgay = $("#txtHopDongTuNgay").val();
    var dHopDongDenNgay = $("#txtHopDongDenNgay").val();
    var sTenDuAn = $("#txtTenDuAn").val();
    var sTenDonVi = $("#sTenDV").val();
    var sChuDauTu = $("#sTenCDT").val();
    var sTenHopDong = $("#txtTenHopDong").val()??'';
    GetListData(sSoHopDong, fTienHopDongTu, fTienHopDongDen, dHopDongTuNgay, dHopDongDenNgay, sTenDuAn, sTenDonVi, sChuDauTu, sTenHopDong, iCurrentPage);
}

function GetListData(sSoHopDong, fTienHopDongTu, fTienHopDongDen, dHopDongTuNgay, dHopDongDenNgay, sTenDuAn, sTenDonVi, sChuDauTu, sTenHopDong, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sSoHopDong: sSoHopDong, fTienHopDongTu: fTienHopDongTu, fTienHopDongDen: fTienHopDongDen, dHopDongTuNgay: dHopDongTuNgay, dHopDongDenNgay: dHopDongDenNgay, sTenDuAn: sTenDuAn, sTenDonVi: sTenDonVi, sChuDauTu: sChuDauTu, sTenHopDong: sTenHopDong, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoHopDong").val(sSoHopDong);
            $("#txtGiaHopDongTu").val(fTienHopDongTu == 0 ? "" : parseFloat(UnFormatNumber(fTienHopDongTu)));
            $("#txtGiaHopDongDen").val(fTienHopDongDen == 0 ? "" : parseFloat(UnFormatNumber(fTienHopDongDen)));
            $("#txtHopDongTuNgay").val(dHopDongTuNgay);
            $("#txtHopDongDenNgay").val(dHopDongDenNgay);
            $("#txtTenDuAn").val(sTenDuAn);
            $("#sTenDV").val(sTenDonVi);
            $("#sTenCDT").val(sChuDauTu);
            $("#txtTenHopDong").val(sTenHopDong);
        }
    });
}

function ThemMoi() {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong/ThemMoi";
}

function DieuChinh(id) {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong/Adjusted/" + id;
}

function ChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong/ChiTiet/" + id;
}

function Sua(id) {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong/Sua/" + id;
}

/*function Xoa(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTinHopDong/Xoa",
        data: { id: id },
        success: function (r) {
            if (r == true) {
                ChangePage();
            }
        }
    });
}*/

function Xoa(id) {
    var Title = 'Xác nhận xóa thông tin hợp đồng';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "XoaItem('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function XoaItem(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTinHopDong/Xoa",
        data: { id: id },
        success: function (r) {
            if (r.status == true) {
                alert(r.sMessage)
                ChangePage();
            }
            else {
                var Title = 'Lỗi xóa thông tin hợp đồng';
                var Messages = [];
                Messages.push("Lỗi xóa Thông tin hợp đồng!");
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}
