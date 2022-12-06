var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaTienTe = "";
    var sTenTienTe = "";
    var sMota = "";

    GetListData(sMaTienTe, sTenTienTe, sMota, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaTienTe = $('<div/>').text($.trim($("#txtMaTienTeFilter").val())).html();
    var sTenTienTe = $('<div/>').text($.trim($("#txtTenTienTeFilter").val())).html();
    var sMota = $('<div/>').text($.trim($("#txtMotaFilter").val())).html();

    GetListData(sMaTienTe, sTenTienTe, sMota, iCurrentPage);
}

function GetListData(sMaTienTe, sTenTienTe, sMota, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTienTe/DanhMucTienTeSearch",
        data: { _paging: _paging, sMaTienTe: sMaTienTe, sTenTienTe: sTenTienTe, sMoTaChiTiet: sMota },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtMaTienTeFilter").val($('<div/>').html(sMaTienTe).text());
            $("#txtTenTienTeFilter").val($('<div/>').html(sTenTienTe).text());
            $("#txtMotaFilter").val($('<div/>').html(sMota).text());
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTienTe/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalTienTe").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY ) {
                $("#modalTienTeLabel").html('Thêm mới thông tin loại đơn vị tiền tệ');
            } else {
                $("#modalTienTeLabel").html('Sửa thông tin loại đơn vị tiền tệ');
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTienTe/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalTienTe").html(data);
            $("#modalTienTeLabel").html('Xem chi tiết thông tin loại đơn vị tiền tệ');
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa loại đơn vị tiền tệ';
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
        url: "/QLNH/DanhMucTienTe/TienTeDelete",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa loại đơn vị tiền tệ';
                        $.ajax({
                            type: "POST",
                            url: "/Modal/OpenModal",
                            data: { Title: Title, Messages: [data.sMessError], Category: ERROR },
                            success: function (res) {
                                $("#divModalConfirm").html(res);
                            }
                        });
                    }
                }
            }
        }
    });
}

function Save() {
    var data = {};

    data.ID = $("#ID_TienTeModal").val();
    data.sMaTienTe = $('<div/>').text($.trim($("#txtsMaTienTe").val())).html();
    data.sTenTienTe = $('<div/>').text($.trim($("#txtsTenTienTe").val())).html();
    data.sMoTaChiTiet = $('<div/>').text($.trim($("#txtsMoTaTienTe").val())).html();


    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTienTe/TienTeSave",
        data: { data: data },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucTienTe";
            } else {
                var Title = 'Lỗi lưu tiền tệ';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa đơn vị tiền tệ';
    var Messages = [];

    if (data.sMaTienTe == null || data.sMaTienTe == "") {
        Messages.push("Mã tiền tệ chưa nhập !");
    }

    if (data.sTenTienTe == null || data.sTenTienTe == "") {
        Messages.push("Tên tiền tệ chưa nhập !");
    }

    if ($.trim($("#txtsMaTienTe").val()) != "" && $.trim($("#txtsMaTienTe").val()).length > 50) {
        Messages.push("Mã tiền tệ vượt quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenTienTe").val()) != "" && $.trim($("#txtsTenTienTe").val()).length > 50) {
        Messages.push("Tên tiền tệ vượt quá 50 kí tự !");
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
