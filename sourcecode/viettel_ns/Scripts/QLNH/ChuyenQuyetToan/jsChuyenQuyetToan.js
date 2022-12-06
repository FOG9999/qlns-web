var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var isShowSearchDMLoaiCongTrinh = true;
var arrDonvi = [];
var arrNamKeHoach = [];

$(document).ready(function ($) {
    var iLoaiThoiGian = $("#slbLoaiThoiGian").val();
    if (iLoaiThoiGian != undefined) {
        changeLoaiThoiGian("slbLoaiThoiGian", "slbThoiGian", iLoaiThoiGian)
    }
});

function ResetChangePage(iCurrentPage = 1) {
    GetListData("", null, GUID_EMPTY,0,0, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sSoChungTu = $("<div/>").text($.trim($("#txtSoChungTuFillter").val())).html();
    var dNgayChungTu = $("#txtNgayChungTuFilter").val();
    var iDonVi = $("#iDonViFillter").val();
    var iLoaiThoiGian = $("#slbLoaiThoiGianFillter").val();
    var iThoiGian = $("#slbThoiGianFillter").val();

    GetListData(sSoChungTu, dNgayChungTu, iDonVi, iLoaiThoiGian, iThoiGian, iCurrentPage);
}

function onDetail(id) {
    window.location.href = "/QLNH/ChuyenDuLieuQuyetToan/Edit?id=" + id ;
}

function GetListData(sSoChungTu, dNgayChungTu, iDonVi, iLoaiThoiGian, iThoiGian, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sSoChungTu: sSoChungTu, dNgayChungTu: dNgayChungTu, iDonVi: iDonVi, iLoaiThoiGian: iLoaiThoiGian, iThoiGian: iThoiGian, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoChungTuFillter").val($("<div/>").html(sSoChungTu).text());
            $("#iDonViFillter").val(iDonVi);
            $("#txtNgayChungTuFilter").val(dNgayChungTu);
            $("#slbLoaiThoiGianFillter").val(0);
            $("#slbThoiGianFillter").val(0);
        }
    });
}
function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ChuyenDuLieuQuyetToan/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalChuyenQuyetToan").empty().html(data);
            $("#modalChuyenQuyetToanLabel").empty().html('Xem chi tiết thông tin chuyển quyết toán');
        }
    });
}
function changeLoaiThoiGian(name,nameChild,valueExist) {
    var loaiThoiGian = $("#" + name + "").val();
    $.ajax({
        async: false,
        url: "/QLNH/ChuyenDuLieuQuyetToan/GetListDropDownThoiGian",
        type: "POST",
        data: { iLoaiThoiGian: loaiThoiGian },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }
            if (data.data != null) {
                $("#" + nameChild + "").html("");
                $("#" + nameChild + "").append(CreateHtmlSelectThoiGian(data.data, valueExist));
            }
        }
    });
}

function Xoa(id) {
    var Title = 'Xác nhận xóa chuyển quyết toán';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}
function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/ChuyenDuLieuQuyetToan/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa thông tin chuyển quyết toán';
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

function CreateHtmlSelectThoiGian(value, valueExist) {
    var htmlOption = "";
    value.forEach(x => {
        if (valueExist != undefined && valueExist == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return htmlOption;
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ChuyenDuLieuQuyetToan/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalChuyenQuyetToan").empty().html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalChuyenQuyetToanLabel").empty().html('Thêm mới chuyển quyết toán');
            } else {
                $("#modalChuyenQuyetToanLabel").empty().html('Sửa chuyển quyết toán');
            }

            var isShowing = false;
            $('.date').datepicker({
                todayBtn: "linked",
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                language: 'vi',
                todayHighlight: true,
                format: "dd/mm/yyyy"
            }).on('hide', () => {
                isShowing = false;
            }).on('show', () => {
                isShowing = true;
            });

            $("#txtNgayChungTu").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });
        }
    });
}

function GetDataChuyenQuyetToan() {
    var data = {};
    data.ID = $("#hidChuyenQuyetToanID").val();
    data.sSoChungTu = $("<div/>").text($.trim($("#txtSoChungTu").val())).html();
    data.dNgayChungTu = $("#txtNgayChungTu").val();
    data.iLoaiThoiGian = $("#slbLoaiThoiGian").val() == 0 ? null : $("#slbLoaiThoiGian").val();
    data.iThoiGian = $("#slbThoiGian").val() == 0 ? null : $("#slbThoiGian").val();
    data.sMoTa = $("<div/>").text($.trim($("#txtMoTa").val())).html();
    data.iID_DonViID = $("#slbDonVi").val() == GUID_EMPTY ? null : $("#slbDonVi").val();
    data.iID_MaDonVi = $("<div/>").text($.trim($("#slbDonVi").find("option:selected").data("madonvi"))).html();
    return data;
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa chuyển quyết toán';
    var Messages = [];

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        Messages.push("Số chứng từ chưa nhập !");
    }
    if ($.trim($("#txtSoChungTu").val()).length > 50) {
        Messages.push("Số chứng từ nhập quá 50 kí tự !");
    }
    if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY) {
        Messages.push("Đơn vị chưa chọn !");
    }
    if (data.iLoaiThoiGian == null || data.iLoaiThoiGian == 0) {
        Messages.push("Loại thời gian chưa chọn !");
    }
    if (data.iThoiGian == null || data.iThoiGian == 0) {
        Messages.push("Thời gian chưa chọn !");
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

function Save() {
    var data = GetDataChuyenQuyetToan();
    if (!ValidateData(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/ChuyenDuLieuQuyetToan/Save",
        data: { data: data },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/ChuyenDuLieuQuyetToan/Edit?id=" + r.dataID;
            } else {
                var Title = 'Lỗi lưu thông tin chuyển quyết toán;'
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