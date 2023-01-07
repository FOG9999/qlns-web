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
            if (r.bIsComplete == true) {
                alert(r.sMessage);
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

function OnExportExcel() {
    var lstId = [];
    $(".iIdExport:checked").each(function (index, item) {
        lstId.push($(item).val());
    });
    if (lstId == null || lstId.length == 0) {
        alert("Chưa chọn bản ghi nào để xuất excel !");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTinGoiThau/OnExport",
        data: { ids: lstId },
        success: function (data) {
            if (data.status) {
                window.open("/QLVonDauTu/QLThongTinGoiThau/ExportReport");
            }
            else {
                var Title = 'Lỗi in báo cáo';
                var messErr = [];
                messErr.push(data.listErrMess);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
}