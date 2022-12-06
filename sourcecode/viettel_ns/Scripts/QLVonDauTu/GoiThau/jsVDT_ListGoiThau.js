var CONFIRM = 0;
var ERROR = 1;
function SearchData() {
    var tenDuAn = $("#txt_tenDuAn").val();
    var tenGoiThau = $("#txt_tenGoiThau").val();
    var giaTriMin = $("#txt_giaTriMin").val();
    var giaTriMax = $("#txt_giaTriMax").val();
    GetListData(tenDuAn, tenGoiThau, giaTriMin, giaTriMax);
}

function GetListData(tenDuAn, tenGoiThau, giaTriMin, giaTriMax) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLThongTinGoiThau/GoiThauListView",
        data: { tenDuAn: tenDuAn, tenGoiThau: tenGoiThau, giaTriMin: giaTriMin, giaTriMax: giaTriMax },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txt_tenDuAn").val(tenDuAn);
            $("#txt_tenGoiThau").val(tenGoiThau);
            $("#txt_giaTriMin").val(giaTriMin);
            $("#txt_giaTriMax").val(giaTriMax);
        }
    });
}
function BtnInsertDataClick() {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Add/";
}

function GetItemData(id) {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Update/" + id;
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa thông tin gói thầu';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTinGoiThau/Delete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
            else {
                var Title = 'Lỗi xóa thông tin gói thầu';
                var Messages = [];
                Messages.push("Lỗi xóa Thông tin gói thầu !");
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



function GetDieuChinh(id) {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/DieuChinh/" + id;
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Detail/" + id;
}

