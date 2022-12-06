var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var isShowSearchDMLoaiCongTrinh = true;
var arrDonvi = [];
var arrNamKeHoach = [];

function ResetChangePage(iCurrentPage = 1) {
    GetListData( null, GUID_EMPTY, 0, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var dNgayKhoiTao = $("#txtNgayKhoiTaoFillter").val();
    var iNamKhoiTao = $("#slbNamKhoiTaoFillter").val();
    var iDonVi = $("#iDonViFillter").val();

    GetListData(dNgayKhoiTao, iDonVi, iNamKhoiTao, iCurrentPage);
}
function GetListData(dNgayKhoiTao, iDonVi, iNamKhoiTao, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { dNgayKhoiTao: dNgayKhoiTao, iDonVi: iDonVi, iNamKhoiTao: iNamKhoiTao, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#iDonViFillter").val(iDonVi);
            $("#txtNgayKhoiTaoFillter").val(dNgayKhoiTao);
            $("#slbNamKhoiTaoFillter").val(iNamKhoiTao);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/KhoiTaoCapPhat/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalKTCapPhat").empty().html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalKTCapPhatLabel").empty().html('Thêm mới khởi tạo cấp phát');
            } else {
                $("#modalKTCapPhatLabel").empty().html('Sửa khởi tạo cấp phát');
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

            $(".txtNgayDeNghiValid").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });
        }
    });
}
function onDetail(id) {
    window.location.href = "/QLNH/KhoiTaoCapPhat/Detail?id=" + id +"&edit=false";
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/KhoiTaoCapPhat/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKTCapPhat").empty().html(data);
            $("#modalKTCapPhatLabel").empty().html('Xem chi tiết thông tin khởi tạo cấp phát');
        }
    });
}

function Xoa(id) {
    var Title = 'Xác nhận xóa khởi tạo cấp phát';
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
        url: "/QLNH/KhoiTaoCapPhat/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa thông tin khởi tạo cấp phát';
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
