var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
function ResetChangePage(iCurrentPage = 1) {
    var sMaLoaiHopDong = "";
    var sTenVietTat = "";
    var sTenLoaiHopDong = "";
    var sMoTa = "";

    GetListData(sMaLoaiHopDong, sTenVietTat, sTenLoaiHopDong, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaLoaiHopDong = $("<div/>").text($.trim($("#txtMaLoaiHopDong").val())).html();
    var sTenVietTat = $("<div/>").text($.trim($("#txtTenVietTat").val())).html();
    var sTenLoaiHopDong = $("<div/>").text($.trim($("#txtTenLoaiHopDong").val())).html();
    var sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();

    GetListData(sMaLoaiHopDong, sTenVietTat, sTenLoaiHopDong, sMoTa, iCurrentPage);
}
function GetListData(sMaLoaiHopDong, sTenVietTat, sTenLoaiHopDong, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucLoaiHopDong/DanhMucLoaiHopDongSearch",
        data: { _paging: _paging, sMaLoaiHopDong: sMaLoaiHopDong, sTenVietTat: sTenVietTat, sTenLoaiHopDong: sTenLoaiHopDong, sMoTa: sMoTa},
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtMaLoaiHopDong").val($("<div/>").html(sMaLoaiHopDong).text());
            $("#txtTenVietTat").val($("<div/>").html(sTenVietTat).text());
            $("#txtTenLoaiHopDong").val($("<div/>").html(sTenLoaiHopDong).text());
            $("#txtMoTa").val($("<div/>").html(sMoTa).text());
        }
    });
}
function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucLoaiHopDong/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalLoaiHopDong").html(data);
            $("#modalLoaiHopDongLabel").html('Chi tiết loại hợp đồng');
        }
    });
}

function OpenModal(id) {

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucLoaiHopDong/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalLoaiHopDong").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalLoaiHopDongLabel").html('Thêm mới loại hợp đồng');
            } else {
                $("#modalLoaiHopDongLabel").html('Sửa thông tin loại hợp đồng');
            }
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucLoaiHopDong/OpenModalDelete",
        data: { id: id },
        success: function (data) {
            $("#contentModalLoaiHopDong").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalLoaiHopDongLabel").html('Xóa !');
            } else {
                $("#modalLoaiHopDongLabel").html('Xác nhận xóa thông tin loại hợp đồng')
            } 
        }
    });
}

function DeleteLoaiHopDong(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucLoaiHopDong/LoaiHopDongDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa danh mục loại hợp đồng';
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
function Save() {
  var data = {};
    data.ID = $("#iID_LoaiHopDongModal").val();
    data.sMaLoaiHopDong = $("<div/>").text($.trim($("#txtsMaLoaiHopDong").val())).html();
    data.sTenVietTat = $("<div/>").text($.trim($("#txtsTenVietTat").val())).html();
    data.sTenLoaiHopDong = $("<div/>").text($.trim($("#txtsTenLoaiHopDong").val())).html();
    data.sMoTa = $("<div/>").text($.trim($("#txtsMoTa").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
      type: "POST",
        url: "/QLNH/DanhMucLoaiHopDong/LoaiHopDongSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucLoaiHopDong";
            } else {
                var Title = 'Lỗi lưu loại hợp đồng';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa loại hợp đồng';
    var Messages = [];

    if (data.sMaLoaiHopDong == null || data.sMaLoaiHopDong == "") {
        Messages.push("Mã loại hợp đồng chưa nhập !");
    }
    if ($.trim($("#txtsMaLoaiHopDong").val()) != "" && $.trim($("#txtsMaLoaiHopDong").val()).length > 50) {
        Messages.push("Mã loại hợp đồng nhập quá 50 kí tự !");
    }
    if ($.trim($("#txtsTenVietTat").val()) != "" && $.trim($("#txtsTenVietTat").val()).length > 100) {
        Messages.push("Tên viết tắt nhập quá 100 kí tự !");
    }
    if (data.sTenLoaiHopDong == null || data.sTenLoaiHopDong == "") {
        Messages.push("Tên loại hợp đồng chưa nhập");
    }
    if ($.trim($("#txtsTenLoaiHopDong").val()) != "" && $.trim($("#txtsTenLoaiHopDong").val()).length > 300) {
        Messages.push("Tên loại hợp đồng nhập quá 300 kí tự !");
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